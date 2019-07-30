using ClosedXML.Excel;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_DAL.GSTR9Attribute;

namespace SmartAdminMvc.Controllers
{
    public class Gstr9ReportController : Controller
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataTable dtErrorRecords = new DataTable();
        static SqlCommand cmd = new SqlCommand();
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: GSTR9Report
        public ActionResult Report()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GetReportList = WeP_BAL.EwayBill.LoadDropDowns.GSTR9Part();
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            return View();
        }
        [HttpPost]
        public ActionResult Report(FormCollection form, string command, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string FinancialYear, reportType, strGSTIN, FromDate = "", ToDate = "";
            string strOTP = "";
            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                reportType = form["reportType"];
                FinancialYear = form["FinancialYear"];
                if (!String.IsNullOrEmpty(FinancialYear))
                {
                    string str = FinancialYear.Substring(0, 4);
                    FromDate = str.Insert(0, "04");
                    var result = FinancialYear.Substring(FinancialYear.Length - 4);
                    ToDate = result.Insert(0, "03");
                }
                strGSTIN = form["strGSTIN"];

                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.GetReportList = WeP_BAL.EwayBill.LoadDropDowns.Exist_GSTR9Part(reportType);
                ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);
                if (!String.IsNullOrEmpty(command))
                {
                    var das = Gstr9.Retrieve_GSTR9_Report_Part2(reportType, strGSTIN, FromDate, ToDate);

                    using (DataSet ds = das)
                    {
                        //Set Name of DataTables.
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].TableName = "GSTR9 Report";
                        }
                      


                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt in ds.Tables)
                            {
                                //Add DataTable as Worksheet.
                                if (dt.Rows.Count > 0)
                                {
                                    wb.Worksheets.Add(dt);
                                }
                            }

                            //Export the Excel file.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment; filename=GSTR9Report_For_" + strGSTIN + "_" + FinancialYear + ".xls");

                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {

                                if (wb.Worksheets.Count > 0)
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                }

                                Response.Flush();
                                Response.End();

                            }
                        }
                    }
                    return RedirectToAction("Report");

                }
                #region "OTP SUBMIT FOR AUTHTOKEN"
                if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    strOTP = form["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["SuccessMessage"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                    }
                }
                #endregion

                #region "OTP REQUEST"
                else
                {
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                    ViewBag.TitleHeaders = "GSTR-9";

                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion

            }
            catch (Exception ex)
            {
                TempData["error"] = "Please try again";
            }
            return View();
        }

        public ActionResult GSTR9Upload(HttpPostedFileBase FileUpload, FormCollection frmcl, int[] ids, string Import, string btnEWBGEN, string btnCONSEWBGEN)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                int UserId = Convert.ToInt32(Session["User_ID"].ToString());
                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();

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
                                        ImportGSTR9CSV(path, UserEmail);
                                        GridView gv = new GridView();
                                        gv.DataSource = dtErrorRecords;
                                        gv.DataBind();
                                        Session["Cars"] = gv;
                                    }
                                    TempData["SuccessMessage"] = "File Uploaded Successfully";
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
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            return View("Report");
        }

        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["Cars"], "ImportErrors.xls");
        }
        #region "IMPORT CSV FOR gstr9"
        public void ImportGSTR9CSV(string fileName, string userEmail)
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

            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Functions.InsertIntoTable("TBL_GSTR9_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[18] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;
            tableName = "TBL_CSV_GSTR9_GEN_RECS";
            copy.DestinationTableName = tableName;

            #region "COLUMNS MAPPING"
            copy.ColumnMappings.Add(0, 1);// [SerialNo]
            copy.ColumnMappings.Add(1, 2);// [gstin]
            copy.ColumnMappings.Add(2, 3);// [period]
            copy.ColumnMappings.Add(3, 4);// [natureofsupplies]
            copy.ColumnMappings.Add(4, 5);// [txval]
            copy.ColumnMappings.Add(5, 6);// [igst]
            copy.ColumnMappings.Add(6, 7); // [cgst]
            copy.ColumnMappings.Add(7, 8); // [sgst]
            copy.ColumnMappings.Add(8, 9); // [csamt]
            copy.ColumnMappings.Add(9, 10); // [taxpayable]
            copy.ColumnMappings.Add(10, 11); // [taxpaidthroughcash] 
            copy.ColumnMappings.Add(11, 12); // [interest]
            copy.ColumnMappings.Add(12, 13); // [penalty] 
            copy.ColumnMappings.Add(13, 14); // [latefee_oth]
            copy.ColumnMappings.Add(14, 15); // [hsn] 
            copy.ColumnMappings.Add(15, 16); // [uqc]
            copy.ColumnMappings.Add(16, 17); // [totalquantity] 
            copy.ColumnMappings.Add(17, 18); // [rate] 
            copy.ColumnMappings.Add(18, 0); // [FileId]  







            #endregion
            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_GSTR9";
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