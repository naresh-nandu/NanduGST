using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SmartAdminMvc.Controllers
{
    public class HsnCodesCreationController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: HSNCodesCreation
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
                        
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Rates = LoadDropDowns.LoadRates();
            ViewBag.HSNCodes = LoadDropDowns.LoadDistinctHSN();
            var GridFill = GetListHSNDetails(iCustId);
            return View(GridFill);
        }


        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["User_ID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

                decimal rate = 0;
                decimal unitprice = 0;
                string hsncode = null, hsndesc = null, hsntype;


                hsntype = frm["hsntype"];
                if (frm["isauto"] == "Yes")
                {
                    hsncode = frm["hsncode"];

                    int hsnid = Convert.ToInt32(frm["hsndesc"]);


                    hsndesc = (from hsn in db.TBL_HSN_MASTER
                               where hsn.hsnId == hsnid
                               select hsn.hsnDescription
                                      ).SingleOrDefault();
                    rate = Convert.ToDecimal(frm["rate"]);
                }
                else if (frm["isauto"] == "No")
                {
                    hsncode = frm["hsncode1"];
                    hsndesc = frm["hsndesc1"];
                    rate = Convert.ToDecimal(frm["rate1"]);
                }
                                
                int outputparam;

                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Insert_HSN_Master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@hsnCode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@hsnDescription", hsndesc));
                cmd.Parameters.Add(new SqlParameter("@unitPrice", unitprice));
                cmd.Parameters.Add(new SqlParameter("@rate", rate));
                cmd.Parameters.Add(new SqlParameter("@CustomerId", iCustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", iUserId));
                cmd.Parameters.Add(new SqlParameter("@hsnType", hsntype));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);

                if (outputparam == 1)
                {
                    TempData["Message"] = "HSN Created Successfully";
                }

                else if (outputparam == -1)
                {
                    TempData["Message"] = "HSN details already exist.";
                }
            }
            catch (Exception ex)
            {                
                Console.Write(ex);
                throw;
            }
            return RedirectToAction("Index");
        }

        public ActionResult Edit(int Id)
        {
            ViewBag.Rates = LoadDropDowns.LoadRates();

            TBL_HSN_MASTER hsnmaster = db.TBL_HSN_MASTER.Where(s => s.hsnId == Id).SingleOrDefault();
            var Rate = (from li in db.TBL_HSN_MASTER where li.hsnId == Id select li.rate).SingleOrDefault();
            ViewBag.Rate = Rate;
            return PartialView(hsnmaster);
        }

        [HttpPost]
        public ActionResult Edit(TBL_HSN_MASTER hsn)
        {
            if (string.IsNullOrEmpty(hsn.hsnCode))
            {
                TempData["Message"] = "Please Enter HSN Code";
            }
            else if (string.IsNullOrEmpty(hsn.hsnDescription))
            {
                TempData["Message"] = "Please Enter HSN Description";
            }
            else if (hsn.rate == null)
            {
                TempData["Message"] = "Please Select HSN Rate";
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    try
                    {
                        int outputparam;
                        decimal unitprice = 0;
                        #region commented
                        conn.Open();
                        SqlCommand dCmd = new SqlCommand("usp_update_HSNDetails", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@hsnid", hsn.hsnId));
                        dCmd.Parameters.Add(new SqlParameter("@action", "Edit"));
                        dCmd.Parameters.Add(new SqlParameter("@hsncode", hsn.hsnCode));
                        dCmd.Parameters.Add(new SqlParameter("@hsndescription", hsn.hsnDescription));
                        dCmd.Parameters.Add(new SqlParameter("@unitprice", unitprice));
                        dCmd.Parameters.Add(new SqlParameter("@rate", hsn.rate));
                        dCmd.Parameters.Add(new SqlParameter("@CustomerId", Convert.ToInt32(Session["Cust_ID"].ToString())));
                        dCmd.Parameters.Add(new SqlParameter("@userid", Convert.ToInt32(Session["User_ID"].ToString())));
                        dCmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                        dCmd.ExecuteNonQuery();
                        conn.Close();
                        outputparam = Convert.ToInt32(dCmd.Parameters["@RetVal"].Value);

                        if (outputparam == 1)
                        {
                            TempData["Message"] = "HSN details Updated Successfully.";
                        }

                        else if (outputparam == -1)
                        {
                            TempData["Message"] = "HSN details cannot be updated as it already exist.";
                        }

                        #endregion
                    }
                    catch (Exception ex)
                    {
                        TempData["Message"] = ex.Message;
                    }
                }
            }
            return RedirectToAction("Index");
        }

        public ActionResult Delete(int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    int outputparam;
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_update_HSNDetails", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@hsnid", Id));
                    dCmd.Parameters.Add(new SqlParameter("@action", "Delete"));
                    dCmd.Parameters.Add(new SqlParameter("@hsncode", "Test"));
                    dCmd.Parameters.Add(new SqlParameter("@hsndescription", "Test"));
                    dCmd.Parameters.Add(new SqlParameter("@unitprice", Convert.ToDecimal(00.00)));
                    dCmd.Parameters.Add(new SqlParameter("@rate", Convert.ToDecimal(00.00)));
                    dCmd.Parameters.Add(new SqlParameter("@CustomerId", Convert.ToInt32(Session["Cust_ID"].ToString())));
                    dCmd.Parameters.Add(new SqlParameter("@userid", Convert.ToInt32(Session["User_ID"].ToString())));
                    dCmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                    dCmd.ExecuteNonQuery();
                    conn.Close();
                    
                    outputparam = Convert.ToInt32(dCmd.Parameters["@RetVal"].Value);

                    if (outputparam == 2)
                    {
                        TempData["Message"] = "HSN details Deleted Successfully.";
                    }

                    else if (outputparam == -2)
                    {
                        TempData["Message"] = "HSN details cannot be deleted.";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return RedirectToAction("Index");

        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase FileUpload, FormCollection frm)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection conn = new SqlConnection(ConnString);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {

                    //Workout our file path
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads/"), fileName);

                    if (Path.GetExtension(fileName) == ".csv")
                    {
                        //Try and upload
                        try
                        {
                            int TotalRecords, ProcessedRecords;
                            FileUpload.SaveAs(path);

                            ProcessCSV(path);

                            cmd.Connection = conn;
                            cmd.CommandText = "usp_Import_CSV_Master";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                            cmd.Parameters.Add("@custId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                            cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "HSN";
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = path;
                            cmd.Parameters.Add("@TotalRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("@ProcessedRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                            cmd.Parameters.Add("@ErrorRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;

                            conn.Open();
                            cmd.ExecuteNonQuery();
                            TotalRecords = Convert.ToInt32(cmd.Parameters["@TotalRecordsCount"].Value);
                            ProcessedRecords = Convert.ToInt32(cmd.Parameters["@ProcessedRecordsCount"].Value);
                            
                            TempData["Message"] = "HSN detail uploaded.TotalRecords: " + TotalRecords + " Processed Records Count: " + ProcessedRecords + "";
                            conn.Close();
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "HSN CSV data uploaded", "");                                                        
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            throw;
                        }
                    }
                    else
                    {
                        TempData["Message"] = "File format should be .CSV";
                    }
                }
                else
                {
                    //
                }
            }
            //Tidy up
            dt.Dispose();
            return RedirectToAction("Index");
        }

        private void ProcessCSV(string fileName)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];

            SqlConnection conn = new SqlConnection(ConnString);
            SqlCommand cmd = new SqlCommand();
            conn.Open();
            //Set up our variables            
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName);

            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            strArray = r.Split(line);

            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            //Read each line in the CVS file until it’s empty
            
            while ((line = sr.ReadLine()) != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "HSN";
                cmd.Parameters.Add("@Uploadcontent", SqlDbType.NVarChar).Value = line;
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = true;
                Models.Common.Functions.InsertIntoTable("TBL_TMP_CSV_UploadMaster", cmd, conn);
            }

            //Tidy Streameader up
            sr.Dispose();
            conn.Close();
            //return a the new DataTable
        }

        public static List<IDictionary> GetListHSNDetails(int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_HSN_MASTER", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public JsonResult AutoPopulate(string hsncode)
        {
            var hsndesc = (from ob in db.TBL_HSN_MASTER
                           where (ob.CustomerId == 1 && ob.rowstatus == true && ob.hsnCode == hsncode)
                           select new
                           {
                               hsnid = ob.hsnId,
                               hsndesc = ob.hsnDescription
                           }
                      ).ToList();

            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result = javaScriptSerializer.Serialize(hsndesc);
            return Json(result, JsonRequestBehavior.AllowGet);

        }
        public JsonResult AutoPopulateRates(int hsnid)
        {
            var hsninfo = (from ob in db.TBL_HSN_MASTER
                           where (ob.hsnId == hsnid)
                           select new
                           {
                               unitprice = ob.unitPrice,
                               rate = ob.rate
                           }
                   ).SingleOrDefault();
            
            return Json(hsninfo, JsonRequestBehavior.AllowGet);
        }

        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }
    }
}