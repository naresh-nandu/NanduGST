#region Using


using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data.Entity.Core.Objects;
using System.Web;
using System.Data;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using SmartAdminMvc.Models.SearchTaxPayer;
using System.Web.UI.WebControls;
using SmartAdminMvc.Models.Common;
using System.Web.UI;
#endregion
namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class SupplierMgmtController : Controller
    {
        // GET: graphs/flot
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        public ActionResult Create(string option, string search)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                if(TempData["TotalRecordsCount"] != null)
                {
                    TempData["TotalRecordsCount"] = TempData["TotalRecordsCount"].ToString();
                    TempData["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"].ToString();
                    TempData["ErrorRecordsCount"] = TempData["ErrorRecordsCount"].ToString();
                }
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                
                var supDetails = (from sup in db.TBL_Supplier
                                  where sup.CustomerId == custid && sup.RowStatus == true
                                  select sup).ToList();
                ViewBag.Supplierlist = supDetails;
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        [HttpPost]
        public ActionResult Create(FormCollection frm, string Create, string Export)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string username = Session["UserName"].ToString();
                       

            string Name, Email, GSTIN, NOB, LOB, PAN, COB, Address, MobNo, POC;
            string StateCode;
            string DOR;
            try
            {
                Name = frm["name"];
                Email = frm["email"];
                GSTIN = frm["gstin"];
                StateCode = frm["statecode"];
                NOB = frm["NOB"];
                MobNo = frm["mob"];
                DOR = frm["DOR"];
                LOB = "";
                PAN = frm["PAN"];
                COB = frm["COB"];
                Address = frm["address"];
                POC = frm["POC"];

                if (!string.IsNullOrEmpty(Email))
                {
                    //Count total no of Email in the list
                    int totalEmail = Email.Split(';').Count();
                    Regex regex = new Regex(@"[A-Za-z0-9._%-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}");
                    int regexMatch = regex.Matches(Email).Count;
                    //Count no of email with which regex matches
                    if (totalEmail == regexMatch)
                    {
                        //Emails are correctly formatted
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter the valid Email Address";//Raise error on one or more email is incorrect in the list
                        return RedirectToAction("Create");
                    }
                }

                if (!string.IsNullOrEmpty(Create))
                {
                    ObjectParameter ret = new ObjectParameter("RetValue", typeof(int));
                    db.Ins_Supplierdetails(Name, POC, NOB, Email, MobNo, GSTIN.ToUpper(), StateCode, PAN, DOR, COB, Address, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]), ret);
                    db.SaveChanges();
                    
                    if ((int)ret.Value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Supplier created : " + Name, "");
                        TempData["Message"] = "Supplier created Successfully";
                    }
                    else if ((int)ret.Value == 2)
                    {
                        TempData["ErrorMessage"] = "Supplier details already exists";
                    }
                }

                else if (!string.IsNullOrEmpty(Export))
                {
                    var supDetails1 = (from sup in db.TBL_Supplier
                                      where sup.CustomerId == custid && sup.RowStatus == true
                                      select new { sup.SupplierName, sup.EmailId, sup.MobileNo, sup.GSTINno, sup.Address }).ToList();

                    ViewBag.Supplierlist1 = supDetails1;

                    GridView gv = new GridView();
                    gv.DataSource = ViewBag.Supplierlist1;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=SupplierManagement.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }

                else
                {
                    ViewBag.SupName = frm["name"];
                    ViewBag.POC = frm["POC"];
                    ViewBag.NOB = frm["NOB"];
                    ViewBag.Email = frm["email"];
                    ViewBag.MOB = frm["mob"];
                    ViewBag.GSTINNo = frm["gstin"].ToUpper();
                    ViewBag.StateCode = frm["statecode"];
                    ViewBag.PANNo = frm["PAN"].ToUpper();
                    ViewBag.DOR = frm["DOR"];
                    ViewBag.COB = frm["COB"];
                    ViewBag.Address = frm["address"];

                    string strStatus = GspSearchTaxPayerGstin.SendRequest(frm["gstin"].ToUpper(), userid.ToString(), custid.ToString(), username);
                    if (strStatus == "1")
                    {
                        TempData["Message"] = "The GSTIN passed in the request is Valid";
                        ViewBag.GSTINNo = frm["gstin"].ToUpper();
                        ViewBag.StateCode = frm["gstin"].ToUpper().Substring(0, 2);
                        ViewBag.PANNo = frm["gstin"].ToUpper().Substring(2, 10);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = strStatus;
                    }
                }
                var supDetails = (from sup in db.TBL_Supplier
                                  where sup.CustomerId == custid && sup.RowStatus == true
                                  select sup).ToList();
                ViewBag.Supplierlist = supDetails;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }           
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase FileUpload, FormCollection frm)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection con = new SqlConnection(ConnString);
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
                            FileUpload.SaveAs(path);

                            ProcessCSV(path);
                            DataSet dsErrorRecords = new DataSet();
                            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

                            cmd.Connection = con;
                            cmd.CommandText = "usp_Import_CSV_Master";
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                            cmd.Parameters.Add("@custId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                            cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "Supplier";
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = path;

                            SqlParameter totalRecordsCount = new SqlParameter();
                            totalRecordsCount.ParameterName = "@TotalRecordsCount";
                            totalRecordsCount.IsNullable = true;
                            totalRecordsCount.DbType = DbType.Int32;
                            totalRecordsCount.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(totalRecordsCount);

                            SqlParameter processedRecordsCount = new SqlParameter();
                            processedRecordsCount.ParameterName = "@ProcessedRecordsCount";
                            processedRecordsCount.IsNullable = true;
                            processedRecordsCount.DbType = DbType.Int32;
                            processedRecordsCount.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(processedRecordsCount);

                            SqlParameter errorRecordsCount = new SqlParameter();
                            errorRecordsCount.ParameterName = "@ErrorRecordsCount";
                            errorRecordsCount.IsNullable = true;
                            errorRecordsCount.DbType = DbType.Int32;
                            errorRecordsCount.Direction = ParameterDirection.Output;
                            cmd.Parameters.Add(errorRecordsCount);

                            using (SqlDataAdapter adp = new SqlDataAdapter(cmd))
                            {
                                adp.Fill(dsErrorRecords);
                                totalRecords = Convert.ToInt32(cmd.Parameters["@TotalRecordsCount"].Value);
                                processedRecords = Convert.ToInt32(cmd.Parameters["@ProcessedRecordsCount"].Value);
                                errorRecords = Convert.ToInt32(cmd.Parameters["@ErrorRecordsCount"].Value);
                            }
                            TempData["TotalRecordsCount"] = totalRecords.ToString();
                            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
                            TempData["ErrorRecordsCount"] = errorRecords.ToString();

                            GridView grid = new GridView();
                            grid.DataSource = dsErrorRecords.Tables[0];
                            grid.DataBind();
                            Session["SupplierErrorRecords"] = grid;
                            con.Close();

                            TempData["Message"] = "Suppliers detail Uploaded Successfully";
                         
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
                    //Catch errors
                }
            }
            //Tidy up
            dt.Dispose();
            return RedirectToAction("Create");
        }

        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["SupplierErrorRecords"], "SupplierErrorRecords.xls");

        }

        public ActionResult Delete(int Id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var deleterecord = db.TBL_Supplier.SingleOrDefault(x => x.SupplierId == Id);
                deleterecord.RowStatus = false;
                db.SaveChanges();
                string SupplierName = (from li in db.TBL_Supplier
                           where li.SupplierId == Id
                                    select li.SupplierName).FirstOrDefault().ToString();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Supplier deleted : " + SupplierName, "");
                TempData["Message"] = "Supplier Deleted Successfully";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult Edit(int Id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                TBL_Supplier supplier = db.TBL_Supplier.Where(s => s.SupplierId == Id).SingleOrDefault();

                return View(supplier);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Edit(TBL_Supplier sup)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                string Name, Email, GSTIN, NOB, PAN, COB, Address, MobNo, POC, DOR;
                int StateCode, Id;

                Name = sup.SupplierName;
                Email = sup.EmailId;
                GSTIN = sup.GSTINno.ToUpper();
                NOB = sup.NatureOfBusiness;
                PAN = sup.PANNO;
                COB = sup.ConstitutionOfBusiness;
                MobNo = sup.MobileNo;
                POC = sup.POCName;
                StateCode = Convert.ToInt32(sup.StateCode);
                Id = sup.SupplierId;
                Address = sup.Address;
                DOR = sup.DateofCompRegistered;

                if (!string.IsNullOrEmpty(Email))
                {
                    //Count total no of Email in the list
                    int totalEmail = Email.Split(';').Count();
                    Regex regex = new Regex(@"[A-Za-z0-9._%-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}");
                    int regexMatch = regex.Matches(Email).Count;
                    //Count no of email with which regex matches
                    if (totalEmail == regexMatch)
                    {
                        //Emails are correctly formatted
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter the valid Email Address";//Raise error on one or more email is incorrect in the list
                        return RedirectToAction("Edit");
                    }
                }


                db.Update_SupplierDetails (Id, Name, POC, NOB, Email, MobNo, GSTIN, StateCode, PAN, DOR, COB, Address, Convert.ToInt32(Session["User_ID"]));
                db.SaveChanges();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Supplier details updated : "+ Name, "");

                TempData["Message"] = "Supplier Details Updated Successfully";
                return RedirectToAction("Create");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private  void ProcessCSV(string fileName)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];

            SqlConnection con = new SqlConnection(ConnString);
             SqlCommand cmd=new SqlCommand();
            con.Open();
            //Set up our variables
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();
            DataRow row;
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
                cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "Supplier";
                cmd.Parameters.Add("@Uploadcontent", SqlDbType.NVarChar).Value = line;
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = true;
                Models.Common.Functions.InsertIntoTable("TBL_TMP_CSV_UploadMaster", cmd, con);

            }
           
            //Tidy Streameader up
            sr.Dispose();
            con.Close();
            //return a the new DataTable
        }
    }
}