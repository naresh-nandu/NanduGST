#region Using
using System;
using System.Linq;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System.Collections.Generic;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Configuration;
using System.IO;
using System.Web.UI.WebControls;
using SmartAdminMvc.Models.GSTR3;
using System.Web.Routing;
using WeP_DAL;
#endregion

namespace SmartAdminMvc.Controllers
{
    public class Gstr3BUploadController : Controller
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet dsOutputRecords = new DataSet();
        DataTable dtErrorRecords = new DataTable();
        DataTable dtGstinRecords = new DataTable();
        static SqlCommand cmd = new SqlCommand();
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        decimal WalletBalance = 0, WalletMinBalance = 0;
        string GSK = "";
        bool isChecked = false;

        public object GSTR3BDeleteALL1 { get; private set; }

        // GET: GSTR3BUpload
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]

        public ActionResult Index(string GSTR3BSave, string GSTINNo)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");


            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            if (Session["fp"] != null)
            {
                ViewBag.Period = Session["fp"].ToString();
                
            }
            else
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            }

            if (TempData["gstinid"] != null)
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(TempData["gstinid"]), Session["Role_Name"].ToString());
             
            }
            else if (TempData["GSTINNoList"] == null)
            {

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = TempData["GSTINNoList"];
            }
            if (TempData["OTPTempdata"] != null)
            {
                TempData["OTPSession"] = TempData["OTPTempdata"];
                ViewBag.AUTH_Response = TempData["AUTH_Response"];
                ViewBag.AUTH_GSTINNo = TempData["AUTH_GSTINNo"];
                
            }
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GSTR3B",
                Value = "GSTR3B"
            });


            ViewBag.GSTRList = new SelectList(items, "Text", "Value");
            return View();

        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult SaveGSTR3B(FormCollection form, string GSTR3bSave, string GSTR3bDelete, string OTPSubmit)
        {
            var GSTINNo = "";
            var Fp = "";

            try
            {
                //code

                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
                delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

                if (!string.IsNullOrEmpty(GSTR3bDelete))
                {
                    GSTINNo = form["gstinid"];
                    Fp = form["fp"];
                    int mode = 1;
                    GSTR3BDeleteALL1.GSTR3BDeleteALL(GSTINNo, Fp, mode);
                    TempData["DeleteMessage"] = "Imported GSTR3B Data Deleted Successfully";
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());
                    TempData["GSTINNoList"] = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());
                    return RedirectToAction("index");

                }

                if (!string.IsNullOrEmpty(GSTR3bSave))
                {
                    GSTINNo = form["gstinid"];
                    Fp = form["fp"];

                    Helper.GetWalletBalance(Session["Cust_ID"].ToString(), "PACK_RETURNFILING", "GSTR3B", GSTINNo, Fp, out WalletBalance, out GSK, out isChecked);
                    if (GSK == "True")
                    {
                        if (isChecked)
                        {
                            //
                        }
                        else
                        {
                            if (WalletBalance < 50)
                            {
                                TempData["SaveResponse"] = "Your Wallet Balance is Low. Please do recharge for Filing";
                                return RedirectToAction("Index");
                            }
                        }
                    }


                    GspSendGstr3BSaveDataModel GSP_fnGSTR1Save = new GspSendGstr3BSaveDataModel();
                    string strJsonData = Gstr3BUploadDataModel.GetJsonGSTR3BSave(GSTINNo, Fp, "", "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    string SaveResponse = GSP_fnGSTR1Save.SendRequest(strJsonData, GSTINNo, Fp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    TempData["SaveResponse"] = SaveResponse;

                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());
                    ViewBag.TitleHeaders = "GSTR-3B Save";

                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(GSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    TempData["OTPTempdata"] = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    TempData["AUTH_Response"] = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = GSTINNo;
                    TempData["AUTH_GSTINNo"] = GSTINNo;
                }

                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    GSTINNo = form["gstnmodal"];
                    Fp = form["fpmodal"];
                    string strOTP = form["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, GSTINNo);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(GSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                    }
                }
                else
                {
                    GSTINNo = form["gstinlist"];
                    Fp = form["fpsave"];
                    TempData["gstnmodal"] = GSTINNo;
                    TempData["fpmodal"] = Fp;
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());

                    ViewBag.TitleHeaders = "GSTR-3B Save";

                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(GSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    TempData["AUTH_Response"] = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = GSTINNo;
                    TempData["OTPTempdata"] = OTPPOPUPValue;
                }

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());
                TempData["GSTINNoList"] = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(GSTINNo), Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return RedirectToAction("Index");
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult index(HttpPostedFileBase FileUpload, FormCollection frmcl, string command, int[] ids, FormCollection frm, string GSTR3BSave)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string strGSTINNo = frm["ddlGSTINNo"];
            string strFp = frm["period"];
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            ViewBag.Period = strFp;

            try
            {
                int UserId = Convert.ToInt32(Session["User_ID"].ToString());
                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();
                int strGSTRId = Convert.ToInt32(frmcl["ddlGSTR"]);
                TempData["strGSTRId"] = Convert.ToString(strGSTRId);


                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "GSTR3B",
                    Value = "GSTR3B"
                });


                ViewBag.GSTRList = new SelectList(items, "Text", "Value");



                if (Request.Files.Count > 0)
                {
                    TempData["strGSTRId"] = Convert.ToString(strGSTRId);
                    var file = Request.Files[0];
                    if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = Path.GetFileName(FileUpload.FileName);
                        fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", UserId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                        string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                        if (FileExtension.Trim() == "csv")
                        {
                            if (file.ContentLength > 0 && file.ContentLength < 5242880)
                            {
                                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);


                                FileUpload.SaveAs(path);
                                {
                                    ImportCSV(strGSTRId, path, UserEmail);
                                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR3B CSV File: " + fileName + " is Imported Successfully", "");
                                }
                                GridView gv = new GridView();
                                gv.DataSource = dtErrorRecords;
                                gv.DataBind();
                                Session["Cars"] = gv;

                                // Call the Push Procedure for all the retrieve GSTINs

                                if (dtGstinRecords != null)
                                {
                                    string gstin;
                                    gstin = string.Empty;
                                    foreach (DataRow row in dtGstinRecords.Rows)
                                    {
                                        gstin = row["gstin"].ToString();
                                        PushGSTR3B(gstin);
                                    }
                                }


                            }
                            else
                            {
                                TempData["UploadMessage"] = "File size should be 1 kb to 5 MB only.";
                            }
                        }
                        else
                        {
                            TempData["UploadMessage"] = "File format should be .csv";
                        }
                    }
                    else
                    {
                        TempData["UploadMessage"] = "Please select a file.";
                    }
                }
                else
                {
                    TempData["UploadMessage"] = "Please select a file.";
                }
            }





            catch (Exception ex)
            {
                TempData["UploadMessage"] = ex.Message;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            }
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            return View();
        }


        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["Cars"], "ImportErrors.xls");
        }

        private List<IDictionary> ImportCSV(int strGSTRId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');
            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = 3;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[12] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;


            tableName = "TBL_CSV_GSTR3B_RECS";
            copy.DestinationTableName = tableName;

            copy.ColumnMappings.Add(0, 1); // Slno
            copy.ColumnMappings.Add(1, 2); // Gstin
            copy.ColumnMappings.Add(2, 3); // Fp
            copy.ColumnMappings.Add(3, 4); // Nature Of Supplies
            copy.ColumnMappings.Add(4, 6); // Pos
            copy.ColumnMappings.Add(5, 7); // Txval
            copy.ColumnMappings.Add(6, 8); // Iamt
            copy.ColumnMappings.Add(7, 9); // Camt
            copy.ColumnMappings.Add(8, 10); // Samt
            copy.ColumnMappings.Add(9, 11); // Csamt
            copy.ColumnMappings.Add(10, 12); // Inter
            copy.ColumnMappings.Add(11, 13); // Intra
            copy.ColumnMappings.Add(12, 0);  // FileId



            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_GSTR3B_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();

                SqlParameter totalRecordsCount = new SqlParameter();
                totalRecordsCount.ParameterName = "@TotalRecordsCount";
                totalRecordsCount.IsNullable = true;
                totalRecordsCount.DbType = DbType.Int32;
                totalRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(totalRecordsCount);

                SqlParameter processedRecordsCount = new SqlParameter();
                processedRecordsCount.ParameterName = "@ProcessedRecordsCount";
                processedRecordsCount.IsNullable = true;
                processedRecordsCount.DbType = DbType.Int32;
                processedRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(processedRecordsCount);

                SqlParameter errorRecordsCount = new SqlParameter();
                errorRecordsCount.ParameterName = "@ErrorRecordsCount";
                errorRecordsCount.IsNullable = true;
                errorRecordsCount.DbType = DbType.Int32;
                errorRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(errorRecordsCount);
                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsOutputRecords);
                    if (dsOutputRecords != null && dsOutputRecords.Tables.Count >= 1)
                    {
                        dtErrorRecords = dsOutputRecords.Tables[0];
                        dtGstinRecords = dsOutputRecords.Tables[1];
                    }
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                    cmd.ExecuteNonQuery();

                    con.Close();

                }


            }



            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
            DataTable dtVal = new DataTable();
            con.Close();
            sr.Dispose();
            return ConvertToDictionary(dtVal);


        }

        
        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {

            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));
            return dictionaryList.ToList<IDictionary>();
        }


        private void PushGSTR3B(string Gstin)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR3B_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                    dCmd.ExecuteNonQuery();

                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR3B External csv data uploaded to GSTR Tables.", "");
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        public ActionResult home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");


            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult home(HttpPostedFileBase FileUpload, FormCollection frmcl, string command, int[] ids, FormCollection frm, string GSTR3BSave)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string strGSTINNo = frm["ddlGSTINNo"];
            string strFp = frm["period"];
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            ViewBag.Period = strFp;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            try
            {
                int UserId = Convert.ToInt32(Session["User_ID"].ToString());
                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();
                int strGSTRId = Convert.ToInt32(frmcl["ddlGSTR"]);
                TempData["strGSTRId"] = Convert.ToString(strGSTRId);


                var result = GetResult(strGSTINNo, strFp);
                ViewBag.Result = result;


            }


            catch (Exception ex)
            {
                TempData["UploadMessage"] = ex.Message;
            }
            return View();
        }


        #region "Get Details"
        public Gstr3b GetRetrieveGSTR3BSA(string GSTINNo, string Fp)
        {
            DataSet ds = new DataSet();
            Gstr3b gstr3b = new Gstr3b();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Summary_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (DataRow values in ds.Tables[0].Rows)
                    {

                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 1)
                        {
                            gstr3b.sup_details = new SupDetails() { iamt = (DBNull.Value == itemArray[2] ? 0 : Convert.ToDecimal(itemArray[2])), camt = (DBNull.Value == itemArray[3] ? 0 : Convert.ToDecimal(itemArray[3])), samt = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])), csamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])) };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 2)
                        {
                            gstr3b.InterSup = new Inter_Sup() { txval = (DBNull.Value == itemArray[1] ? 0 : Convert.ToDecimal(itemArray[1])), iamt = (DBNull.Value == itemArray[2] ? 0 : Convert.ToDecimal(itemArray[2])) };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 3)
                        {
                            gstr3b.ItcElg = new ItcElg() { iamt = (DBNull.Value == itemArray[2] ? 0 : Convert.ToDecimal(itemArray[2])), camt = (DBNull.Value == itemArray[3] ? 0 : Convert.ToDecimal(itemArray[3])), samt = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])), csamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])) };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 4)
                        {
                            gstr3b.Inward_sup = new InwardSup() { inter = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])), intra = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])) };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 5)
                        {
                            gstr3b.IntrLtfee = new IntrLtfee() { iamt = (DBNull.Value == itemArray[2] ? 0 : Convert.ToDecimal(itemArray[2])), camt = (DBNull.Value == itemArray[3] ? 0 : Convert.ToDecimal(itemArray[3])), samt = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])), csamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])) };
                        }
                    }
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return gstr3b;

        }// for retrieving the values on the dashboard
        public string GetEligibleITC(string gstinid, string fp)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            EligibleItc eligibleITC = new EligibleItc();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@gstin", gstinid));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (System.Data.DataRow values in ds.Tables[0].Rows)
                    {

                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 9 ||
                                                                     Convert.ToInt64(itemArray[0]) == 10
                                                                || Convert.ToInt64(itemArray[0]) == 11
                                                                || Convert.ToInt64(itemArray[0]) == 12
                                                                || Convert.ToInt64(itemArray[0]) == 13)
                        {
                            eligibleITC.TBL_GSTR3B_itc_elg_itc_avl.Add(new TblGstr3BItcElgItcAvl()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                ty = (DBNull.Value == itemArray[0] ? "" : Convert.ToString(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            });

                        }
                        if (Convert.ToInt64(itemArray[0]) == 14 ||
                                                                    Convert.ToInt64(itemArray[0]) == 15
                                                              )
                        {
                            eligibleITC.TBL_GSTR3B_itc_elg_itc_rev.Add(new TblGstr3BItcElgItcRev()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                ty = (DBNull.Value == itemArray[0] ? "" : Convert.ToString(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))

                            });
                        }
                        if (Convert.ToInt64(itemArray[0]) == 16)
                        {
                            eligibleITC.TBL_GSTR3B_itc_elg_itc_net = new TblGstr3BItcElgItcNet()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                ty = (DBNull.Value == itemArray[0] ? "" : Convert.ToString(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            };

                        }
                        if (Convert.ToInt64(itemArray[0]) == 17 ||
                                                               Convert.ToInt64(itemArray[0]) == 18
                                                         )
                        {
                            eligibleITC.TBL_GSTR3B_itc_elg_itc_inelg.Add(new TblGstr3BItcElgItcInelg()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                ty = (DBNull.Value == itemArray[0] ? "" : Convert.ToString(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))

                            });
                        }
                        var das = ds.Tables[0].AsEnumerable();                        
                    }
                    #endregion
                }

                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(eligibleITC);
        }
        public string GetOutwordSupply(string gstinid, string fp)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            OutwardSuppliesInwardSupplies gstr3b = new OutwardSuppliesInwardSupplies();
            gstr3b.gstinid = gstinid;
            gstr3b.fp = fp;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {

                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@gstin", gstinid));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (System.Data.DataRow values in ds.Tables[0].Rows)
                    {

                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 4)
                        {
                            gstr3b.TBL_GSTR3B_sup_det_isup_rev = new TBL_GSTR3B_sup_det_isup_rev()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 1)
                        {
                            gstr3b.TBL_GSTR3B_sup_det_osup_det = new TBL_GSTR3B_sup_det_osup_det()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            };
                        }

                        if (Convert.ToInt64(itemArray[0]) == 3)
                        {
                            gstr3b.TBL_GSTR3B_sup_det_osup_nil_exmp = new TBL_GSTR3B_sup_det_osup_nil_exmp()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4]))
                            };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 5)
                        {
                            gstr3b.TBL_GSTR3B_sup_det_osup_nongst = new TBL_GSTR3B_sup_det_osup_nongst()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])),
                            };
                        }
                        if (Convert.ToInt64(itemArray[0]) == 2)
                        {
                            gstr3b.TBL_GSTR3B_sup_det_osup_zero = new TBL_GSTR3B_sup_det_osup_zero()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToDecimal(itemArray[4])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            };
                        }


                    }


                    var das = ds.Tables[0].AsEnumerable();
                    #endregion

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(gstr3b);
        }
        public string GetInterestAndLatefee(string gstinid, string fp)
        {
            Session["gstinid"] = gstinid;
            Session["fp"] = fp;
            
            System.Data.DataSet ds = new System.Data.DataSet();
            TBL_GSTR3B_InterestAndLateFee interestAndLateFee = new TBL_GSTR3B_InterestAndLateFee();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {

                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }

                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@gstin", gstinid));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (System.Data.DataRow values in ds.Tables[0].Rows)
                    {
                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 21)
                        {
                            interestAndLateFee = new TBL_GSTR3B_InterestAndLateFee()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToDecimal(itemArray[5])),
                                camt = (DBNull.Value == itemArray[6] ? 0 : Convert.ToDecimal(itemArray[6])),
                                samt = (DBNull.Value == itemArray[7] ? 0 : Convert.ToDecimal(itemArray[7])),
                                csamt = (DBNull.Value == itemArray[8] ? 0 : Convert.ToDecimal(itemArray[8]))
                            };
                        }

                    }
                    interestAndLateFee.Gstinid = gstinid;
                    interestAndLateFee.Fp = fp;


                    var das = ds.Tables[0].AsEnumerable();
                    #endregion

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(interestAndLateFee);
        }
        public string GetInwardSupplies(string gstinid, string fp)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            InwardSupplies inwardSupplies = new InwardSupplies();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {

                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@gstin", gstinid));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (System.Data.DataRow values in ds.Tables[0].Rows)
                    {

                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 19 || Convert.ToInt64(itemArray[0]) == 20)
                        {
                            inwardSupplies.TBL_GSTR3B_inward_sup_isup_details.Add(new TBL_GSTR3B_inward_sup_isup_details()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                ty = (DBNull.Value == itemArray[0] ? "" : Convert.ToString(itemArray[0])),
                                inter = (DBNull.Value == itemArray[9] ? 0 : Convert.ToInt32(itemArray[9])),
                                intra = (DBNull.Value == itemArray[10] ? 0 : Convert.ToInt32(itemArray[10])),
                            });
                        }

                    }

                    var das = ds.Tables[0].AsEnumerable();
                    #endregion

                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(inwardSupplies);
        }
        public string GetInterStateSupplyById(string gstinid, string fp)
        {
            System.Data.DataSet ds = new System.Data.DataSet();
            InterSup interSup = new InterSup();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@gstin", gstinid));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    foreach (System.Data.DataRow values in ds.Tables[0].Rows)
                    {
                        var itemArray = values.ItemArray;
                        if (Convert.ToInt64(itemArray[0]) == 6)
                        {
                            //TBL_GSTR3B_inter_sup_unreg_det
                            interSup.TBL_GSTR3B_inter_sup_unreg_det.Add(new TBL_GSTR3B_inter_sup_unreg_det(
                                //inter_supid= (DBNull.Value == itemArray[3] ? 0 : Convert.ToDecimal(itemArray[3])),

                                )
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                //inter_supid = (DBNull.Value == itemArray[0] ? 0 : Convert.ToInt32(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToInt32(itemArray[5])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToInt32(itemArray[4])),
                                pos = (DBNull.Value == itemArray[3] ? 0 : Convert.ToInt32(itemArray[3]))
                            });
                        }
                        if (Convert.ToInt64(itemArray[0]) == 7)
                        {
                            interSup.TBL_GSTR3B_inter_sup_comp_det.Add(new TBL_GSTR3B_inter_sup_comp_det()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                //inter_supid = (DBNull.Value == itemArray[0] ? 0 : Convert.ToInt32(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToInt32(itemArray[5])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToInt32(itemArray[4])),
                                pos = (DBNull.Value == itemArray[3] ? 0 : Convert.ToInt32(itemArray[3]))
                            });
                        }

                        if (Convert.ToInt64(itemArray[0]) == 8)
                        {
                            //TBL_GSTR3B_inter_sup_uin_det
                            interSup.TBL_GSTR3B_inter_sup_uin_det.Add(new TBL_GSTR3B_inter_sup_uin_det()
                            {
                                Id = (DBNull.Value == itemArray[2] ? 0 : Convert.ToInt32(itemArray[2])),
                                //inter_supid = (DBNull.Value == itemArray[0] ? 0 : Convert.ToInt32(itemArray[0])),
                                iamt = (DBNull.Value == itemArray[5] ? 0 : Convert.ToInt32(itemArray[5])),
                                txval = (DBNull.Value == itemArray[4] ? 0 : Convert.ToInt32(itemArray[4])),
                                pos = (DBNull.Value == itemArray[3] ? 0 : Convert.ToInt32(itemArray[3]))
                            });
                        }
                        
                    }
                    
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

            }
            return Newtonsoft.Json.JsonConvert.SerializeObject(interSup);
        }

        #endregion

        #region "Actionresult For Partial Views"
        public ActionResult RetrieveGSTR3BSA(string GSTINNo, string Fp)
        {
            if (string.IsNullOrEmpty(GSTINNo))
            {
                TempData["GSTNEmpty"] = "Please select the Gstin";

            }
            var result = GetRetrieveGSTR3BSA(GSTINNo, Fp);
            result.gstinid = GSTINNo;
            result.fp = Fp;
            return PartialView(result);
        }
        public ActionResult itc_elg(string gstinid, string fp)
        {
            ViewBag.gstinid = gstinid;
            Session["gstinid"] = gstinid;
            ViewBag.fp = fp;
            TempData["gstinid"] = gstinid;
            Session["fp"] = fp;

            TempData["fp"] = fp;
            return View("itc_elg");
        }
        public ActionResult sup_det(string gstinid, string fp)
        {
            ViewBag.gstinid = gstinid;
            Session["gstinid"] = gstinid;
            ViewBag.fp = fp;
            TempData["gstinid"] = gstinid;
            Session["fp"] = fp;

            TempData["fp"] = fp;
            return View();
        }
        public ActionResult intr_ltfee(string gstinid, string fp)
        {
            ViewBag.gstinid = gstinid;
            ViewBag.fp = fp;
            Session["gstinid"] = gstinid;
            TempData["gstinid"] = gstinid;
            Session["fp"] = fp;
            TempData["fp"] = fp;
            return View("intr_ltfee");
        }
        public ActionResult inward_sup(string gstinid, string fp)
        {
            ViewBag.gstinid = gstinid;
            ViewBag.fp = fp;
            TempData["gstinid"] = gstinid;
            TempData["fp"] = fp;
            Session["gstinid"] = gstinid;
            Session["fp"] = fp;
            return View("inward_sup");
        }
        public ActionResult inter_sup(string gstinid, string fp)
        {
            ViewBag.gstinid = gstinid;
            ViewBag.fp = fp;
            TempData["gstinid"] = gstinid;
            TempData["fp"] = fp;
            Session["gstinid"] = gstinid;
            Session["fp"] = fp;
            return View();
        }
        #endregion

        #region "Delete"

        [HttpGet]
        public ActionResult Deleteitcelg(string gstinid, string fp)
        {
            var message = fp;
            int mode = 4;
            delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

            GSTR3BDeleteALL1.GSTR3BDeleteALL(gstinid, fp, mode);

            return Json(message, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult Deletesupdet(string gstinid, string fp)
        {
            var message = fp;
            int mode = 2;
            delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

            GSTR3BDeleteALL1.GSTR3BDeleteALL(gstinid, fp, mode);

            return Json(message, JsonRequestBehavior.AllowGet);


        }

        [HttpGet]
        public ActionResult Deleteltfee(string gstinid, string fp)
        {
            var message = fp;
            int mode = 6;
            delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

            GSTR3BDeleteALL1.GSTR3BDeleteALL(gstinid, fp, mode);

            return Json(message, JsonRequestBehavior.AllowGet);


        }
        [HttpGet]
        public ActionResult Deleteinward(string gstinid, string fp)
        {
            var message = fp;
            int mode = 5;
            delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

            GSTR3BDeleteALL1.GSTR3BDeleteALL(gstinid, fp, mode);

            return Json(message, JsonRequestBehavior.AllowGet);


        }
        [HttpGet]
        public ActionResult Deleteintersup(string gstinid, string fp)
        {
            var message = fp;
            int mode = 3;
            delete3bdata GSTR3BDeleteALL1 = new delete3bdata();

            GSTR3BDeleteALL1.GSTR3BDeleteALL(gstinid, fp, mode);

            return Json(message, JsonRequestBehavior.AllowGet);


        }
        [HttpPost]
        public JsonResult DeleteInterSup(string pos, string detId, string txtVal, string iAmt, string supplynumber, string gstinId, string fp)
        {
            var IsValueDeletd = false;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    try
                    {
                        #region commented

                        conn.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Delete_GSTR3B_SA", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@GstinNo", gstinId));
                        dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
                        dCmd.Parameters.Add(new SqlParameter("@SupplyNum", supplynumber));
                        dCmd.Parameters.Add(new SqlParameter("@DetId", detId));
                        dCmd.Connection = conn;
                        var recordDeleted = dCmd.ExecuteNonQuery();
                        conn.Close();

                        #endregion
                    }
                    catch
                    {

                    }
                }

                IsValueDeletd = true;
                return Json(new { result = IsValueDeletd }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception e)
            {
                IsValueDeletd = false;
                return Json(new { result = IsValueDeletd }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region "Save"
        [HttpPost]
        public JsonResult SaveOutwordSupply(OutwardSuppliesInwardSupplies outwardSuppliesInwardSupplies)
        {
            bool dataUpdated = false;
            //Add stored procedure to add the value into database
            try
            {
                if (outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev != null)
                {
                    UpdateGSTR3BRecord(outwardSuppliesInwardSupplies.gstinid,
                                       outwardSuppliesInwardSupplies.fp,
                                       4,//supply number
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.Id,
                                       "0",
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.txval,
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.iamt,
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.camt,
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.camt,//camt and samt should be same
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_isup_rev.csamt,

                                       0,
                                       0
                                       );
                }
                if (outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_zero != null)
                {
                    UpdateGSTR3BRecord(outwardSuppliesInwardSupplies.gstinid,
                                       outwardSuppliesInwardSupplies.fp,
                                       2,//supply number
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_zero.Id,
                                       "0",
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_zero.txval,
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_zero.iamt,
                                       0,
                                       0,
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_zero.csamt,
                                       0,
                                       0
                                       );
                }
                if (outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nil_exmp != null)
                {
                    UpdateGSTR3BRecord(outwardSuppliesInwardSupplies.gstinid,
                                      outwardSuppliesInwardSupplies.fp,
                                      3,//supply number
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nil_exmp.Id,
                                      "0",
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nil_exmp.txval,
                                      0,
                                      0,
                                      0,
                                      0,
                                      0,
                                      0
                                      );
                }
                if (outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det != null)
                {
                    UpdateGSTR3BRecord(outwardSuppliesInwardSupplies.gstinid,
                                      outwardSuppliesInwardSupplies.fp,
                                      1,//supply number
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.Id,
                                      "0",
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.txval,
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.iamt,
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.camt,

                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.camt,//camt and samt should be same
                                       outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_det.csamt,
                                      0,
                                      0
                                      );

                }
                if (outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nongst != null)
                {
                    UpdateGSTR3BRecord(outwardSuppliesInwardSupplies.gstinid,
                                      outwardSuppliesInwardSupplies.fp,
                                      5,//supply number
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nongst.Id,
                                      "0",
                                      outwardSuppliesInwardSupplies.TBL_GSTR3B_sup_det_osup_nongst.txval,
                                      0,
                                      0,
                                      0,
                                      0,
                                      0,
                                      0
                                      );
                }

                dataUpdated = true;
            }
            catch (Exception e)
            {
                dataUpdated = false;
            }
            return Json(new { result = dataUpdated }, JsonRequestBehavior.AllowGet);
        }
        [HttpPost]
        public JsonResult SaveEligibleITC(string json)
        {
            bool IsRecordUpdated = false;
            try
            {

                EligibleItc _EligibleITC = Newtonsoft.Json.JsonConvert.DeserializeObject<EligibleItc>(json);
                //Supply Number 9 10 11 12 13
                foreach (var item in _EligibleITC.TBL_GSTR3B_itc_elg_itc_avl)
                {
                    UpdateGSTR3BRecord(_EligibleITC.EligibleITCCommon.Gstinid,
                                          _EligibleITC.EligibleITCCommon.Fp,
                                          Convert.ToInt32(item.ty),//supply number
                                          item.Id,
                                          "0",
                                           0,
                                          item.iamt,
                                          item.camt,
                                          item.samt,
                                          item.csamt,
                                          0,
                                          0
                                          );
                }
                //Supply Number14 15 
                foreach (var item in _EligibleITC.TBL_GSTR3B_itc_elg_itc_rev)
                {
                    UpdateGSTR3BRecord(_EligibleITC.EligibleITCCommon.Gstinid,
                                         _EligibleITC.EligibleITCCommon.Fp,
                                         Convert.ToInt32(item.ty),//supply number
                                         item.Id,
                                         "0",
                                          0,
                                         item.iamt,
                                         item.camt,
                                         item.samt,
                                         item.csamt,

                                         0,
                                         0
                                         );
                }
                //Supply Number 17 18
                foreach (var item in _EligibleITC.TBL_GSTR3B_itc_elg_itc_inelg)
                {
                    UpdateGSTR3BRecord(_EligibleITC.EligibleITCCommon.Gstinid,
                                       _EligibleITC.EligibleITCCommon.Fp,
                                       Convert.ToInt32(item.ty),//supply number
                                       item.Id,
                                       "0",
                                        0,
                                       item.iamt,
                                       item.camt,
                                       item.samt,
                                       item.csamt,

                                       0,
                                       0
                                       );
                }
                //Supply Number 16
                UpdateGSTR3BRecord(_EligibleITC.EligibleITCCommon.Gstinid,
                                       _EligibleITC.EligibleITCCommon.Fp,
                                       Convert.ToInt32(_EligibleITC.TBL_GSTR3B_itc_elg_itc_net.ty),//supply number
                                       _EligibleITC.TBL_GSTR3B_itc_elg_itc_net.Id,
                                       "0",
                                        0,
                                       _EligibleITC.TBL_GSTR3B_itc_elg_itc_net.iamt,
                                       _EligibleITC.TBL_GSTR3B_itc_elg_itc_net.camt,
                                        _EligibleITC.TBL_GSTR3B_itc_elg_itc_net.samt,
                                       _EligibleITC.TBL_GSTR3B_itc_elg_itc_net.csamt,

                                       0,
                                       0
                                       );
                IsRecordUpdated = true;
            }
            catch (Exception ex)
            {
                IsRecordUpdated = false;
            }

            return Json(new { result = IsRecordUpdated }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveInterestAndLatefee(TBL_GSTR3B_InterestAndLateFee intrLateFee)
        {
            bool IsRecordUpdated = false;
            try
            {
                UpdateGSTR3BRecord(intrLateFee.Gstinid,
                                   intrLateFee.Fp,
                                   21,//supply number
                                   intrLateFee.Id,
                                    "0",
                                    0,
                                    intrLateFee.iamt,
                                    intrLateFee.camt,
                                      intrLateFee.camt,//camt and samt should be same
                                    intrLateFee.csamt,

                                    0,
                                     0
                                     );
                IsRecordUpdated = true;
            }

            catch (Exception ex)
            {
                IsRecordUpdated = false;
            }
            return Json(new { result = IsRecordUpdated }, JsonRequestBehavior.AllowGet);

        }

        [HttpPost]
        public JsonResult SaveInwardSupplies(string json)
        {
            bool IsRecoedUpdated = false;
            try
            {
                InwardSupplies _InwardSupplies = Newtonsoft.Json.JsonConvert.DeserializeObject<InwardSupplies>(json);
                //Supply Number 19 20
                foreach (var item in _InwardSupplies.TBL_GSTR3B_inward_sup_isup_details)
                {
                    UpdateGSTR3BRecord(_InwardSupplies.Common.Gstinid,
                                       _InwardSupplies.Common.Fp,
                                          Convert.ToInt32(item.ty),//supply number
                                          item.Id,
                                          "0",
                                          0,
                                          0,
                                          0,
                                          0,
                                          0,
                                          item.inter,
                                          item.intra
                                          );
                }
                IsRecoedUpdated = true;
            }
            catch (Exception ex)
            {
                IsRecoedUpdated = false;
            }

            return Json(new { result = IsRecoedUpdated }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SaveInterSup(string jsonn)
        {
            var updatedValue = false;
            try
            {
                InterSup _InterSup = Newtonsoft.Json.JsonConvert.DeserializeObject<InterSup>(jsonn);

                //Supply Number 6  TBL_GSTR3B_inter_sup_unreg_det
                foreach (var item in _InterSup.TBL_GSTR3B_inter_sup_unreg_det)
                {
                    UpdateGSTR3BRecord(_InterSup.Common.Gstinid,
                                        _InterSup.Common.Fp,
                                        6,//supply number
                                        item.Id,
                                        item.pos.ToString(),
                                        item.txval,
                                        item.iamt,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0
                                    );
                }
                //Supply Number 7
                foreach (var item in _InterSup.TBL_GSTR3B_inter_sup_comp_det)
                {
                    UpdateGSTR3BRecord(_InterSup.Common.Gstinid,
                                        _InterSup.Common.Fp,
                                        7,//supply number
                                        item.Id,
                                        item.pos.ToString(),
                                        item.txval,
                                        item.iamt,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0
                                    );
                }
                //Supply Number 8
                foreach (var item in _InterSup.TBL_GSTR3B_inter_sup_uin_det)
                {
                    UpdateGSTR3BRecord(_InterSup.Common.Gstinid,
                                        _InterSup.Common.Fp,
                                        8,//supply number
                                        item.Id,
                                        item.pos.ToString(),
                                        item.txval,
                                        item.iamt,
                                        0,
                                        0,
                                        0,
                                        0,
                                        0
                                    );
                }

                updatedValue = true;
                return Json(new { result = updatedValue }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                updatedValue = false;
                return Json(new { result = updatedValue }, JsonRequestBehavior.AllowGet);
            }


        }

        #endregion

        #region ""
        public List<IDictionary> GetResult(string GSTINNo, string Fp)
        {


            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {

                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    foreach (DataRow values in ds.Tables[0].Rows)
                    {
                        var a = values.ItemArray;
                        var i = a[0];
                        var j = a[1];
                    }
                    var das = ds.Tables[0].AsEnumerable();
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }
        public void UpdateGSTR3BRecord(string GstinNo
                                       , string Fp
                                       , int SupplyNum
                                       , int DetId
                                       , string Pos
                                       , decimal Txval
                                       , decimal Iamt
                                       , decimal Camt
                                       , decimal Samt
                                       , decimal Csamt
                                       , decimal InterStateSupplies
                                       , decimal IntraStateSupplies)
        {
            int dataUpdated = 0;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_GSTR3B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GstinNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                    dCmd.Parameters.Add(new SqlParameter("@SupplyNum", SupplyNum));
                    dCmd.Parameters.Add(new SqlParameter("@DetId ", DetId));
                    dCmd.Parameters.Add(new SqlParameter("@Pos", Pos));
                    dCmd.Parameters.Add(new SqlParameter("@Txval", Txval));
                    dCmd.Parameters.Add(new SqlParameter("@Iamt", Iamt));
                    dCmd.Parameters.Add(new SqlParameter("@Camt", Camt));
                    dCmd.Parameters.Add(new SqlParameter("@Samt", Samt));
                    dCmd.Parameters.Add(new SqlParameter("@Csamt", Csamt));
                    dCmd.Parameters.Add(new SqlParameter("@InterStateSupplies", InterStateSupplies));
                    dCmd.Parameters.Add(new SqlParameter("@IntraStateSupplies", IntraStateSupplies));
                    dCmd.Connection = conn;
                    dataUpdated = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }


        }
        #endregion
    }
}

