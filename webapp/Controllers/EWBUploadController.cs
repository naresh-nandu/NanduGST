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
using SmartAdminMvc.Models.EWAY;
using System.Threading.Tasks;
using System.Threading;
using WeP_EWayBill;

namespace SmartAdminMvc.Controllers
{
    public class EwbUploadController : Controller
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataTable dtErrorRecords = new DataTable();
        static SqlCommand cmd = new SqlCommand();
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: EWBUpload
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            ViewBag.EWBList = LoadDropDowns.GetEWBTypeList();
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(HttpPostedFileBase FileUpload, FormCollection frmcl, int[] ids, string Import, string btnEWBGEN, string btnCONSEWBGEN)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                int UserId = Convert.ToInt32(Session["User_ID"].ToString());
                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();

                string strGSTINNo = "", strEWBTypeId = "", strRefIds = "";
                strEWBTypeId = frmcl["ddlEWB"].ToString();
                ViewBag.EWBType = strEWBTypeId;

                ViewBag.EWBList = LoadDropDowns.Exist_GetEWBTypeList(strEWBTypeId);

                #region "IMPORT DATA"
                if (!string.IsNullOrEmpty(Import))
                {
                    if (Request.Files.Count > 0)
                    {
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
                                        if (strEWBTypeId == "1")
                                        {
                                            ImportEWBCSV(strEWBTypeId, path, UserEmail);
                                        }
                                        if (strEWBTypeId == "2")
                                        {
                                            ImportCONSEWBCSV(strEWBTypeId, path, UserEmail);
                                        }
                                        if (strEWBTypeId == "3")
                                        {
                                            ImportUpdateVehicleNoCSV(strEWBTypeId, path, UserEmail);
                                        }
                                        GridView gv = new GridView();
                                        gv.DataSource = dtErrorRecords;
                                        gv.DataBind();
                                        Session["Cars"] = gv;
                                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "EWayBill CSV File: " + fileName + " is Imported Successfully", "");
                                        if (strEWBTypeId == "1")
                                        {
                                            TempData["SuccessMessage"] = "Data Imported Successfully... Please Go to EWayBill List Page for EWB Generation";
                                        }
                                        if (strEWBTypeId == "2")
                                        {
                                            TempData["SuccessMessage"] = "Data Imported Successfully... Please Go to Cons.EWayBill List Page for CONSEWB Generation";
                                        }
                                        if (strEWBTypeId == "3")
                                        {
                                            TempData["SuccessMessage"] = "Data Imported Successfully... Please Go to Updated VehiclNo List Page for VehicleNo Updation";
                                        }
                                    }

                                    // Call the Push Procedure for all the retrieve GSTINs
                                }
                                else
                                {
                                    TempData["ErrorMessage"] = "File size should be 1 kb to 5 MB only.";
                                }
                            }
                            else
                            {
                                TempData["ErrorMessage"] = "File format should be .csv";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please select a file.";
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please select a file.";
                    }
                }
                #endregion

                #region "GENERATE BULK EWAYBILL"
                else if (!string.IsNullOrEmpty(btnEWBGEN))
                {
                    Thread.Sleep(1000);
                    Task.Factory.StartNew(() => EwbGeneration.EWAYBILL_GEN_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "EWayBill Generation is in progress... Please check after sometime";
                }
                #endregion

                #region "GENERATE BULK CONSOLIDATED EWAYBILL"
                else if (!string.IsNullOrEmpty(btnCONSEWBGEN))
                {
                    Thread.Sleep(1000);
                    Task.Factory.StartNew(() => EwbGeneration.CONS_EWAYBILL_GEN_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "EWayBill Generation is in progress... Please check after sometime";
                }
                #endregion

                #region "ONCHANGE EVENT"                
                else
                {
                    //
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["Cars"], "ImportErrors.xls");
        }

        #region "IMPORT CSV FOR EWB"
        private void ImportEWBCSV(string strEWBTypeId, string fileName, string userEmail)
        {
            DataSet dsOutputRecords = new DataSet();
            DataTable dtGstinRecords = new DataTable();
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
            cmd.Parameters.Add("@ewbtypeid", SqlDbType.TinyInt).Value = strEWBTypeId;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Functions.InsertIntoTable("TBL_EWB_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[58] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;
            tableName = "TBL_CSV_EWB_GEN_RECS";
            copy.DestinationTableName = tableName;

            #region "COLUMNS MAPPING"
            copy.ColumnMappings.Add(0, 1);// SerialNo
            copy.ColumnMappings.Add(1, 2);// Location
            copy.ColumnMappings.Add(2, 3);// TransactionType
            copy.ColumnMappings.Add(3, 4);// TransactionSubType
            copy.ColumnMappings.Add(4, 5);// TransactionSubTypeDesc
            copy.ColumnMappings.Add(5, 6);// DocType
            copy.ColumnMappings.Add(6, 7); // DocNo
            copy.ColumnMappings.Add(7, 8); // DocDate
            copy.ColumnMappings.Add(8, 9); // InvoiceType
            copy.ColumnMappings.Add(9, 10); // FromGSTIN
            copy.ColumnMappings.Add(10, 11); // FromTradeName
            copy.ColumnMappings.Add(11, 12); // FromAddress1
            copy.ColumnMappings.Add(12, 13); // FromAddress2
            copy.ColumnMappings.Add(13, 14); // FromPlace
            copy.ColumnMappings.Add(14, 15); // FromPinCode
            copy.ColumnMappings.Add(15, 16); // FromStateCode
            copy.ColumnMappings.Add(16, 17); // ActFromStateCode
            copy.ColumnMappings.Add(17, 18); // ToGSTIN
            copy.ColumnMappings.Add(18, 19); // ToTradeName
            copy.ColumnMappings.Add(19, 20); // ToAddress1
            copy.ColumnMappings.Add(20, 21); // ToAddress2
            copy.ColumnMappings.Add(21, 22); // ToPlace
            copy.ColumnMappings.Add(22, 23); // ToPinCode
            copy.ColumnMappings.Add(23, 24); // ToStateCode
            copy.ColumnMappings.Add(24, 25); // ActToStateCode
            copy.ColumnMappings.Add(25, 54); // transactionType
            copy.ColumnMappings.Add(26, 55); // dispatchFromGSTIN
            copy.ColumnMappings.Add(27, 56); // dispatchFromTradeName
            copy.ColumnMappings.Add(28, 57); // shipToGSTIN
            copy.ColumnMappings.Add(29, 60); // shipToTradeName
            copy.ColumnMappings.Add(30, 58); // otherValue
            copy.ColumnMappings.Add(31, 26); // TotalValue
            copy.ColumnMappings.Add(32, 27); // TotalIGST
            copy.ColumnMappings.Add(33, 28); // TotalCGST
            copy.ColumnMappings.Add(34, 29); // TotalSGST
            copy.ColumnMappings.Add(35, 30); // TotalCess
            copy.ColumnMappings.Add(36, 59); // cessNonAdvolValue
            copy.ColumnMappings.Add(37, 31); // TotalInvValue
            copy.ColumnMappings.Add(38, 32); // TransMode
            copy.ColumnMappings.Add(39, 33); // TransDistance
            copy.ColumnMappings.Add(40, 34); // TransporterId
            copy.ColumnMappings.Add(41, 35); // TransporterName
            copy.ColumnMappings.Add(42, 36); // TransNo
            copy.ColumnMappings.Add(43, 37); // TransDocDate
            copy.ColumnMappings.Add(44, 38); // VehicleNo
            copy.ColumnMappings.Add(45, 39); // VehicleType
            copy.ColumnMappings.Add(46, 40); // HSNName
            copy.ColumnMappings.Add(47, 41); // HSNDescription
            copy.ColumnMappings.Add(48, 42); // HSNCode
            copy.ColumnMappings.Add(49, 43); // Qty
            copy.ColumnMappings.Add(50, 44); // UQC
            copy.ColumnMappings.Add(51, 45); // TaxableAmt
            copy.ColumnMappings.Add(52, 46); // IGSTRate
            copy.ColumnMappings.Add(53, 47); // CGSTRate
            copy.ColumnMappings.Add(54, 48); // SGSTRate
            copy.ColumnMappings.Add(55, 49); // CessRate
            copy.ColumnMappings.Add(56, 50); // CessNonAdVol
            copy.ColumnMappings.Add(57, 51); // AllowDuplication
            copy.ColumnMappings.Add(58, 0); // FileId  

            #endregion

            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_EWB_GEN";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileId", SqlDbType.Int).Value = fileId;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["User_ID"].ToString();
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();

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
                    con.Close();
                }
            }

            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }
        #endregion

        #region "IMPORT CSV FOR CONSOLIDATED EWB"
        private void ImportCONSEWBCSV(string strEWBTypeId, string fileName, string userEmail)
        {
            DataSet dsOutputRecords = new DataSet();
            DataTable dtGstinRecords = new DataTable();
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
            cmd.Parameters.Add("@ewbtypeid", SqlDbType.TinyInt).Value = strEWBTypeId;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_EWB_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[11] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;
            tableName = "TBL_CSV_EWB_GEN_CONS_RECS";
            copy.DestinationTableName = tableName;

            #region "COLUMNS MAPPING"
            copy.ColumnMappings.Add(0, 1); // SerialNo
            copy.ColumnMappings.Add(1, 10); // Location
            copy.ColumnMappings.Add(2, 2); // userGSTIN
            copy.ColumnMappings.Add(3, 3); // VehicleNo
            copy.ColumnMappings.Add(4, 4); // FromPlace
            copy.ColumnMappings.Add(5, 5); // TransMode
            copy.ColumnMappings.Add(6, 6); // TransDocNo
            copy.ColumnMappings.Add(7, 7); // TransDocDate
            copy.ColumnMappings.Add(8, 8); // FromStateCode
            copy.ColumnMappings.Add(9, 9); // EWayBillNo
            copy.ColumnMappings.Add(10, 11); // AllowDuplication
            copy.ColumnMappings.Add(11, 0); // FileId
            #endregion

            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_EWB_CONS_GEN";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileId", SqlDbType.Int).Value = fileId;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["User_ID"].ToString();
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();

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
                    con.Close();
                }
            }

            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }
        #endregion

        #region "IMPORT CSV FOR UPDATE VEHICLE NO"
        private void ImportUpdateVehicleNoCSV(string strEWBTypeId, string fileName, string userEmail)
        {
            DataSet dsOutputRecords = new DataSet();
            DataTable dtGstinRecords = new DataTable();
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
            cmd.Parameters.Add("@ewbtypeid", SqlDbType.TinyInt).Value = strEWBTypeId;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_EWB_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[11] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;
            tableName = "TBL_CSV_EWB_UPD_VEHNO_RECS";
            copy.DestinationTableName = tableName;

            #region "COLUMNS MAPPING"
            copy.ColumnMappings.Add(0, 1);
            copy.ColumnMappings.Add(1, 2);
            copy.ColumnMappings.Add(2, 3);
            copy.ColumnMappings.Add(3, 4);
            copy.ColumnMappings.Add(4, 5);
            copy.ColumnMappings.Add(5, 6);
            copy.ColumnMappings.Add(6, 7);
            copy.ColumnMappings.Add(7, 8);
            copy.ColumnMappings.Add(8, 9);
            copy.ColumnMappings.Add(9, 10);
            copy.ColumnMappings.Add(10, 11);
            copy.ColumnMappings.Add(11, 0);
            #endregion

            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_EWB_UPD_VEHNO_GEN";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileId", SqlDbType.Int).Value = fileId;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["User_ID"].ToString();
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();

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
                    con.Close();
                }
            }

            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }
        #endregion
    }
}