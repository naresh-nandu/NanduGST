using ClosedXML.Excel;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Report;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using System.Web.Mvc;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class DownloadReportsController : Controller
    {
        private string Report;
        private string strFromPeriod;
        private string strUserId;
        private string strCustId;
        private string strCustRefNo;
        private string strToPeriod;
        private string GSTIN;
      
        [System.Web.Mvc.HttpGet]
        public ActionResult Index()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            ViewBag.ReportList = LoadDropDowns.GetGSTRList("Outward Data,Outward Amendments Data,Inward Data,GSTR - 1 Upload Data,GSTR - 1 Amendments Upload Data,GSTR - 1 Summary Data,GSTR-1 Download Data,GSTR - 2A Data,Reconciliation Summary Data,Modified Reconciliation Summary,Final GSTR - 2 Upload Data,GSTR - 3B Uploaded Data,GSTR - 3B Download Data,GSTR - 6 Import Data,GSTR - 6 Upload Data,GSTR6 Reconciliation Summary Data,GSTR6 Modified Reconciliation Summary,GSTR-6 Download Data");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            ViewBag.GSTINList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            ViewBag.Period2 = DateTime.Now.ToString("MMyyyy");
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            return View();
        }
        [HttpPost]
        public ActionResult ProcessData(FormCollection Form)
        {

            ViewBag.ReportList = LoadDropDowns.GetGSTRList("Outward Data,Outward Amendments Data,Inward Data,GSTR - 1 Upload Data,GSTR - 1 Amendments Upload Data,GSTR - 1 Summary Data,GSTR-1 Download Data,GSTR - 2A Data,Reconciliation Summary Data,Modified Reconciliation Summary,Final GSTR - 2 Upload Data,GSTR - 3B Uploaded Data,GSTR - 3B Download Data,GSTR - 6 Import Data,GSTR - 6 Upload Data,GSTR6 Reconciliation Summary Data,GSTR6 Modified Reconciliation Summary,GSTR-6 Download Data");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            ViewBag.GSTINList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            Report = Convert.ToString(Form["ddlReport"]);

            if (string.IsNullOrEmpty(Convert.ToString(Form["ddlGSTIN"])))
            {
                GSTIN = "ALL";
            }
            else { 
            GSTIN = Convert.ToString(Form["ddlGSTIN"]);
            }
            ViewBag.Period2= DateTime.Now.ToString("MMyyyy");
            ViewBag.Period= DateTime.Now.ToString("MMyyyy");
            strFromPeriod = Convert.ToString(Form["Fromperiod"]);
            strUserId = Convert.ToString(Session["User_ID"]);
            strCustId = Convert.ToString(Session["Cust_ID"]);
            strCustRefNo = Session["CustRefNo"].ToString();
            string strUserName = Convert.ToString(Session["UserName"]);
            strToPeriod = Convert.ToString(Form["Toperiod"]);
            string token = String.Format("{0}"
                        , Guid.NewGuid()
                        );
           
            if (string.IsNullOrEmpty(strFromPeriod) || string.IsNullOrEmpty(Report))
            {
                TempData["Error"] = "Please select the Fields";
            }
            else
            {
                ViewBag.Token = "Reference id: " + token+" .Please use this reference id for downloading the file after Sometime ";
                string message = "Log File Generated for " + Report + " From " + strFromPeriod + " To " + strToPeriod + " For GSTIN- " + GSTIN + " .And the reference Id is " + token;
                Helper.InsertAuditLog(strUserId, strUserName, message, "");
                   string fileLocation = Server.MapPath("~/App_Data/uploads/Logs");
                string fileName = String.Concat(token.Substring(1, 3),token.Substring(10,3),".xlsx");
                
                #region "Outward Data"
                if (Report.Equals("Outward Data"))
                    {

                    fileName = String.Concat("OutwardData" + fileName);
                    Task.Factory.StartNew(() =>
                      OutwardData(token,Path.Combine(fileLocation,fileName)));
                    }
                #endregion

                if (Report.Equals("Inward Data"))
                {
                    fileName = String.Concat("InwardData" + fileName);
                    Task.Factory.StartNew(() =>
                    InwardData(token, Path.Combine(fileLocation, fileName)));
                }

                if (Report.Equals("GSTR-6 Download Data"))
                {
                    fileName = String.Concat("GSTR6DownloadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR6DownloadLog(token, Path.Combine(fileLocation, fileName)));
                }

                if (Report.Equals("GSTR-1 Download Data"))
                {
                    fileName = String.Concat("GSTR1DownloadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR1Download(token, Path.Combine(fileLocation, fileName)));
                }

                if (Report.Equals("GSTR - 1 Upload Data"))
                {
                    fileName = String.Concat("GSTR1UploadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR1Uploaded(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 1 Amendments Upload Data"))
                {
                    fileName = String.Concat("GSTR1AmendmentsUploadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR1AmendmentsUploaded(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("Outward Amendments Data"))
                {
                    fileName = String.Concat("OutwardAmendmentsData" + fileName);
                    Task.Factory.StartNew(() =>
                    OutwardDataAmendment(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 1 Summary Data"))
                {
                    fileName = String.Concat("GSTR1SummaryData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR1Summary(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 2A Data"))
                {
                    fileName = String.Concat("GSTR2AData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR2ADownloaded(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("Reconciliation Summary Data"))
                {
                    fileName = String.Concat("ReconSummaryData" + fileName);
                    Task.Factory.StartNew(() =>
                    ReconcilationSummmary(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("Modified Reconciliation Summary"))
                {
                    fileName = String.Concat("ModifiedReconSummary" + fileName);
                    Task.Factory.StartNew(() =>
                    AfterReconcilation(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("Final GSTR - 2 Upload Data"))
                {
                    fileName = String.Concat("FinalGSTR2UploadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR2Uploaded(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 3B Uploaded Data"))
                {
                 
                       fileName = String.Concat("GSTR3BUploadedData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR3BUploadData(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 3B Download Data"))
                {
                    fileName = String.Concat("GSTR3BDownloadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR3BDowloadData(token, Path.Combine(fileLocation, fileName)));
                }
                
                if (Report.Equals("GSTR - 6 Import Data"))
                {
                    fileName = String.Concat("GSTR6ImportData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR6Imported(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR - 6 Upload Data"))
                {
                    fileName = String.Concat("GSTR6UploadData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR6Uploaded(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR6 Reconciliation Summary Data"))
                {
                    fileName = String.Concat("GSTR6ReconSummaryData" + fileName);
                    Task.Factory.StartNew(() =>
                    GSTR6ReconcilationSummary(token, Path.Combine(fileLocation, fileName)));
                }
                if (Report.Equals("GSTR6 Modified Reconciliation Summary"))
                {
                    fileName = String.Concat("GSTR6ModReconSummary" + fileName);
                    Task.Factory.StartNew(() =>
                    AfterReconcilation(token, Path.Combine(fileLocation, fileName)));
                }


            }
            ViewBag.Period = strFromPeriod;
            ViewBag.Period2 = strToPeriod;
            ViewBag.GSTINList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
            ViewBag.ReportList = LoadDropDowns.Exist_GetGSTRList("Outward Data,Outward Amendments Data,Inward Data,GSTR - 1 Upload Data,GSTR - 1 Amendments Upload Data,GSTR - 1 Summary Data,GSTR-1 Download Data,GSTR - 2A Data,Reconciliation Summary Data,Modified Reconciliation Summary,Final GSTR - 2 Upload Data,GSTR - 3B Uploaded Data,GSTR - 3B Download Data,GSTR - 6 Import Data,GSTR - 6 Upload Data,GSTR6 Reconciliation Summary Data,GSTR6 Modified Reconciliation Summary,GSTR-6 Download Data", Report);
            return View("Index");
        }



        public void OutwardData(string token,string path)
        {
            
                DataSet das = LogFilesDataModel.GetDataSetOutwardLogMultiPeriod(strCustRefNo, strFromPeriod, strToPeriod, GSTIN);
                using (DataSet ds = das)
                {
                //Set Name of DataTables.

                ds.Tables[0].TableName = "AT";
                ds.Tables[1].TableName = "B2B";
                ds.Tables[2].TableName = "B2CL";
                ds.Tables[3].TableName = "B2CS";
                ds.Tables[4].TableName = "CDNR";
                ds.Tables[5].TableName = "CDNUR";
                ds.Tables[6].TableName = "DOCISSUE";
                ds.Tables[7].TableName = "EXP";
                ds.Tables[8].TableName = "HSN";
                ds.Tables[9].TableName = "NIL";
                ds.Tables[10].TableName = "TXP";
                

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

                    try { 
                            wb.SaveAs(path);
                           
                            string logtype = "OutwardLog_" + strFromPeriod;
                            UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod,strToPeriod, logtype, strUserId, strCustId);
                            uploadToBlob.UploadReportsToBlob(path, "OutwardLog_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "OutwardLog_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "OutwardLog", "", token);
                }

            }
                }
           

        }

        public void OutwardDataAmendment(string token,string path)
        {

            DataSet das = LogFilesDataModel.GetDataSetOutwardAmendmentLogMultiPeriod(strCustRefNo, strFromPeriod, strToPeriod, GSTIN);
            using (DataSet ds = das)
            {
                //Set Name of DataTables.

                ds.Tables[0].TableName = "ATA";
                ds.Tables[1].TableName = "B2BA";
                ds.Tables[2].TableName = "B2CLA";
                ds.Tables[3].TableName = "B2CSA";
                ds.Tables[4].TableName = "CDNRA";
                ds.Tables[5].TableName = "CDNURA";
                ds.Tables[6].TableName = "EXPA";
                ds.Tables[7].TableName = "TXPA";


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
                        try { 
                        wb.SaveAs(path);
                        string logtype = "OutwardAmendmentLog_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "OutwardAmendmentLog_", token);

                    }
                    catch (Exception ex)
                    {
                        UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "OutwardAmendmentLog_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "OutwardAmendmentLog", "", token);
                    } 
            }
            }


        }

        public void InwardData(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetInwardLogMultiPeriod(strCustId, strCustRefNo, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {

                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "B2BUR";
                ds.Tables[2].TableName = "CDNR";
                ds.Tables[3].TableName = "CDNUR";
                ds.Tables[4].TableName = "HSN";
                ds.Tables[5].TableName = "IMPG";
                ds.Tables[6].TableName = "IMPS";
                ds.Tables[7].TableName = "ITCRVSL";
                ds.Tables[8].TableName = "NIL";
                ds.Tables[9].TableName = "TXI";
                ds.Tables[10].TableName = "TXPD";
                ds.Tables[11].TableName = "GSTR2 Summary";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    //Export the Excel file.
                    try { 
                    wb.SaveAs(path);
                    string logtype = "InwardLog_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "InwardLog_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "InwardLog_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "InwardLog", "", token);
                }

            }
            }
        }

        public void GSTR1Uploaded(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR1UploadedDataMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "B2CL";
                ds.Tables[2].TableName = "B2CS";
                ds.Tables[3].TableName = "EXP";
                ds.Tables[4].TableName = "CDNR";
                ds.Tables[5].TableName = "CDNUR";
                ds.Tables[6].TableName = "HSN";
                ds.Tables[7].TableName = "NIL";
                ds.Tables[8].TableName = "TXP";
                ds.Tables[9].TableName = "AT";
                ds.Tables[10].TableName = "DOCISSUE";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                    
                        wb.SaveAs(path);
                    
                        string logtype = "GSTR1_UploadedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR1_UploadedData_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR1_UploadedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR1_UploadedDataLog", "", token);
                }

            }
            }
        }

        public void GSTR1AmendmentsUploaded(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR1AmendUploadedDataMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2BA";
                ds.Tables[1].TableName = "B2CLA";
                ds.Tables[2].TableName = "B2CSA";
                ds.Tables[3].TableName = "EXPA";
                ds.Tables[4].TableName = "CDNRA";
                ds.Tables[5].TableName = "CDNURA";
                ds.Tables[6].TableName = "TXPA";
                ds.Tables[7].TableName = "ATA";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "GSTR1_Amendments_UploadedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR1_Amendments_UploadedData_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR1_Amendments_UploadedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR1_Amendments_UploadedDataLog", "", token);
                }

            }
            }
        }

        public void GSTR1Summary(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR1SummaryMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "SUMMARY";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "GSTR1_SummaryData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR1_SummaryData_", token);

                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR1_SummaryData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR1_SummaryDataLog", "", token);
                }
            }
            }
        }

        public void GSTR2ADownloaded(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR2ADownloadedDataMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "Base Data-B2B";
                ds.Tables[1].TableName = "Modified B2B";
                ds.Tables[2].TableName = "Base Data-CDNR";
                ds.Tables[3].TableName = "Modified CDNR";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "GSTR2A_DownloadedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR2A_DownloadedData_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR2A_DownloadedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR2A_DownloadedDataLog", "", token);
                }


            }
            }
        }

        public void ReconcilationSummmary(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetReconciliationSummaryMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B-Matched Invocies";
                ds.Tables[1].TableName = "CDN-Matched Invocies";
                ds.Tables[2].TableName = "B2B-MissinginGSTR2A Invocies";
                ds.Tables[3].TableName = "CDN-MissinginGSTR2A Invocies";
                ds.Tables[4].TableName = "B2B-MissinginGSTR2 Invocies";
                ds.Tables[5].TableName = "CDN-MissinginGSTR2 Invocies";
                ds.Tables[6].TableName = "B2B-Mismatch Invocies";
                ds.Tables[7].TableName = "CDN-Mismatch Invocies";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "ReconciliationSummary_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "ReconciliationSummary_", token);

                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "ReconciliationSummary_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "ReconciliationSummaryLog", "", token);
                }
            }
            }
        }

        public void AfterReconcilation(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetReconciliationStatusMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B Accepted from Missingin2A";
                ds.Tables[1].TableName = "CDN Accepted from Missingin2A";
                ds.Tables[2].TableName = "B2B-Rejected Invocies";
                ds.Tables[3].TableName = "CDN-Rejected Invocies";
                ds.Tables[4].TableName = "B2B-Modified Invocies";
                ds.Tables[5].TableName = "CDN-Modified Invocies";
                ds.Tables[6].TableName = "B2B-Hold Invocies";
                ds.Tables[7].TableName = "CDN-Hold Invocies";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "ReconciliationStatus_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "ReconciliationStatus_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "ReconciliationStatus_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "ReconciliationStatusLog", "", token);
                }


            }
            }
        }

        public void GSTR2Uploaded(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR2UploadedDataMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "B2BUR";
                ds.Tables[2].TableName = "IMPG";
                ds.Tables[3].TableName = "IMPS";
                ds.Tables[4].TableName = "CDNR";
                ds.Tables[5].TableName = "CDNUR";
                ds.Tables[6].TableName = "HSN";
                ds.Tables[7].TableName = "NIL";
                ds.Tables[8].TableName = "TXPD";
                ds.Tables[9].TableName = "TXI";
                ds.Tables[10].TableName = "ITCRVSL";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "GSTR2_UploadedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR2_UploadedData_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR2_UploadedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR2_UploadedDataLog", "", token);
                }


            }
            }
        }

        public void GSTR6Imported(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR6ImportMultiPeriod(strCustRefNo, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "CDNR";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "GSTR6_ImportedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR6_ImportedData_", token);
                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR6_ImportedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR6_ImportedDataLog", "", token);
                }


            }
            }
        }

        public void GSTR6Uploaded(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR6UploadDataMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "CDNR";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    try { 
                    //Saving to Blob and getting the download link
                    wb.SaveAs(path);
                    string logtype = "GSTR6_UploadedData_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR6_UploadedData_", token);

                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR6_UploadedData_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR6_UploadedDatalog", "", token);
                }

            }
            }
        }

        public void GSTR6ReconcilationSummary(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR6ReconciliationSummaryMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B-Matched Invocies";
                ds.Tables[1].TableName = "CDN-Matched Invocies";
                ds.Tables[2].TableName = "B2B-MissinginGSTR6A Invocies";
                ds.Tables[3].TableName = "CDN-MissinginGSTR6A Invocies";
                ds.Tables[4].TableName = "B2B-MissinginGSTR6 Invocies";
                ds.Tables[5].TableName = "CDN-MissinginGSTR6 Invocies";
                ds.Tables[6].TableName = "B2B-Mismatch Invocies";
                ds.Tables[7].TableName = "CDN-Mismatch Invocies";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                    wb.SaveAs(path);
                    string logtype = "GSTR6_ReconciliationSummary_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR6_ReconciliationSummary_", token);

                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR6_ReconciliationSummary_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR6_ReconciliationSummaryLog", "", token);
                }

            }
            }
        }

        public void GSTR1Download(string token, string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR1DownloadLogMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "B2BCL";
                ds.Tables[2].TableName = "B2BCS";
                ds.Tables[3].TableName = "EXP";
                ds.Tables[4].TableName = "CDNR";
                ds.Tables[5].TableName = "CDNUR";
                ds.Tables[6].TableName = "HSN";
                ds.Tables[7].TableName = "NIL";
                ds.Tables[8].TableName = "DOCISSUE";
                ds.Tables[9].TableName = "AT";
                ds.Tables[10].TableName = "TXP";
                ds.Tables[11].TableName = "B2BA";
                ds.Tables[12].TableName = "B2CLA";
                ds.Tables[13].TableName = "B2CSA";
                ds.Tables[14].TableName = "EXPA";
                ds.Tables[15].TableName = "CDNRA";
                ds.Tables[16].TableName = "CDNURA";
                ds.Tables[17].TableName = "ATA";
                ds.Tables[18].TableName = "TXPA";
                
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }
                    try { 
                    //Saving to Blob and getting the download link
                    wb.SaveAs(path);
                    string logtype = "GSTR1Download_" + strFromPeriod;
                    UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                    uploadToBlob.UploadReportsToBlob(path, "GSTR1Download_", token);

                }
                    catch (Exception ex)
                {
                    UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "GSTR1Download_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "GSTR1DownloadLog", "", token);
                }

            }
            }

        }

        public void AfterReconcilationGSTR6(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetReconciliationStatusGSTR6MultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B-Accepted Invocies";
                ds.Tables[1].TableName = "CDN-Accepted Invocies";
                ds.Tables[2].TableName = "B2B-Rejected Invocies";
                ds.Tables[3].TableName = "CDN-Rejected Invocies";
                ds.Tables[4].TableName = "B2B-Modified Invocies";
                ds.Tables[5].TableName = "CDN-Modified Invocies";
                ds.Tables[6].TableName = "B2B-Pending Invocies";
                ds.Tables[7].TableName = "CDN-Pending Invocies";

                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    try
                    {
                        //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        string logtype = "ReconciliationStatus_" + strFromPeriod;
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "ReconciliationStatus_", token);
                    }
                    catch (Exception ex)
                    {
                        UploadToBlob.InsertExceptiondataIntoTable("", "", ex.ToString().Substring(0, 200), "ReconciliationStatus_", "", strUserId, strCustId, DateTime.Now, "Error", "0", "ReconciliationStatusLog", "", token);
                    }


                }
            }
        }

        public void GSTR6DownloadLog(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetReconciliationStatusGSTR6MultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "B2B";
                ds.Tables[1].TableName = "CDNR";
                ds.Tables[2].TableName = "ISD";
                ds.Tables[3].TableName = "B2BA";
                ds.Tables[4].TableName = "CDNRA";
                ds.Tables[5].TableName = "ISDA";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    string logtype = "GSTR6Download_" + strFromPeriod;
                    try
                    {
                        //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR6Download_", token);
                    }
                    catch (Exception ex)
                    {
                     UploadToBlob.InsertExceptiondataIntoTable("","",ex.ToString().Substring(0,200), "GSTR6Download_","",strUserId,strCustId,DateTime.Now,"Error","0", "GSTR6DownloadLog","", token);
                    }



                }
            }
        }

        public void GSTR3BDowloadData(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR3BDownloadMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "GSTR3B Download Data";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    string logtype = "GSTR3BDownload_" + strFromPeriod;
                    try
                    {
                        //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR3BDownload_", token);
                    }
                    catch (Exception ex)
                    {
                     UploadToBlob.InsertExceptiondataIntoTable("","",ex.ToString().Substring(0,200), "GSTR3BDownload_","",strUserId,strCustId,DateTime.Now,"Error","0", "GSTR3BDownloadLog","", token);
                    }



                }
            }
        }

        public void GSTR3BUploadData(string token,string path)
        {
            var das = LogFilesDataModel.GetDataSetGSTR3BUploadMultiPeriod(strCustId, strUserId, strFromPeriod, strToPeriod, GSTIN);

            using (DataSet ds = das)
            {
                //Set Name of DataTables.
                ds.Tables[0].TableName = "GSTR3B Uploaded Data";
                using (XLWorkbook wb = new XLWorkbook())
                {
                    foreach (DataTable dt in ds.Tables)
                    {
                        //Add DataTable as Worksheet.
                        wb.Worksheets.Add(dt);
                    }

                    string logtype = "GSTR3BUpload_" + strFromPeriod;
                    try
                    {
                        //Saving to Blob and getting the download link
                        wb.SaveAs(path);
                        UploadToBlob uploadToBlob = new UploadToBlob("All", strFromPeriod, strToPeriod, logtype, strUserId, strCustId);
                        uploadToBlob.UploadReportsToBlob(path, "GSTR3BUpload_", token);
                    }
                    catch (Exception ex)
                    {
                     UploadToBlob.InsertExceptiondataIntoTable("","",ex.ToString().Substring(0,200), "GSTR3BUpload_","",strUserId,strCustId,DateTime.Now,"Error","0", "GSTR3BUploadLog","", token);
                    }



                }
            }
        }



        public ActionResult CheckAndDownload(FormCollection Form)
        {
            ViewBag.ReportList = LoadDropDowns.GetGSTRList("Outward Data,Outward Amendments Data,Inward Data,GSTR - 1 Upload Data,GSTR - 1 Amendments Upload Data,GSTR - 1 Summary Data,GSTR-1 Download Data,GSTR - 2A Data,Reconciliation Summary Data,Modified Reconciliation Summary,Final GSTR - 2 Upload Data,GSTR - 2 Summary Data,GSTR - 3B Uploaded Data,GSTR - 3B Download Data,GSTR - 6 Import Data,GSTR - 6 Upload Data,GSTR6 Reconciliation Summary Data,GSTR6 Modified Reconciliation Summary,GSTR-6 Download Data");
            ViewBag.GSTINList = LoadDropDowns.GetGSTRList("All");
            string Token = Convert.ToString(Form["ReferenceId"]);
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection conn = new SqlConnection(connectionString);
            conn.Open();
            string flag="0";
            string query1 = "Select bloburl from TBL_Blob_Transactions where Token=@token";
            string query2 = "Select UploadStatus from TBL_Blob_Transactions where Token=@token";
            try
            {
                SqlCommand myCommand = new SqlCommand(query1, conn);

                myCommand.Parameters.AddWithValue("@Token", Token.Trim(' '));

                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string url = String.Format("{0}", reader["bloburl"]);
                        Session["DownloadReport"] = url;
                       
                    }
                    else {
                        flag = "1";
                    }
                    
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            try {
                SqlCommand myCommand = new SqlCommand(query2, conn);

                myCommand.Parameters.AddWithValue("@Token", Token.Trim(' '));

                using (SqlDataReader reader = myCommand.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        string status = String.Format("{0}", reader["UploadStatus"]);
                        if (status.Equals("0"))
                            flag = "2";
                        
                       
                    }


                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            if (flag.Equals("1"))
            {
                TempData["RecordError"] = "Report Generation in progress,Please try after sometime";
            }
            else if (flag.Equals("2"))
            {
                TempData["RecordError"] = "No Data Found";
            }
           
                return RedirectToAction("Index", "DownloadReports");
        }

    }
}