using ClosedXML.Excel;
using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR7;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;
using WeP_BAL.Common;
using WeP_BAL.GstrDownload;
using WeP_DAL.GSTRDelete;
using static WeP_DAL.GSTRDelete.GstrCommonDeleteDal;

namespace SmartAdminMvc.Controllers
{
    public class Gstr7DownloadController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: GSTR1Download
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(int page = 0, string sort = null)
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

            try
            {
                if (sort != null || page != 0)
                {
                    string Action = TempData["Action"] as string;
                    string Period = TempData["Period"] as string;
                    string GSTIN = TempData["gstn"] as string;
                    string strtoPeriod = TempData["toPeriod"] as string;
                    string CTIN = TempData["Ctin"] as string;
                    TempData.Keep();

                    ViewBag.Period = Period;
                    ViewBag.toPeriod = strtoPeriod;
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    ViewBag.RecTypeList = LoadDropDowns.GetGSTRList("TDS,TDSA");
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7D"), "ActionName", "ActionName", Action);
                    ViewBag.ActionList = Actionlst;
                    var GridFill = GetListGSTR1Download(GSTIN, Period, Action, strtoPeriod);
                    return View(GridFill);
                }
                else
                {
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                   
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7D"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }

            }
            catch (Exception ex)
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
              
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7D"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.DownloadResponse = ex.Message.ToString();
                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string Download, string Export, string Command, string OTPSubmit, string command1)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string strGSTIN = "", strAction = "", strPeriod = "", strAUTHGSTIN = "", strRefId = "",  strToken = "", strfromPeriod = "", strRecType = "", strtoPeriod = "", strcommand = "", fp = "", gstin = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strfromPeriod = frm["period"];
                strtoPeriod = frm["toperiod"];
                strRecType = frm["RecType"];
                strRefId = frm["refid"];
                strToken = frm["token"];
              
                strAUTHGSTIN = frm["AUTH_GSTINNo"];

                if (!string.IsNullOrEmpty(command1))
                {
                    strcommand = command1;
                    string[] period = strcommand.Split('+');
                    int index = 0;
                    int index1 = 1;
                    int index2 = 2;

                    command1 = period[index];
                    fp = period[index1];
                    gstin = period[index2];
                }

                ViewBag.Period = strPeriod;
                ViewBag.toPeriod = strtoPeriod;
                TempData["gstn"] = strGSTIN;
                TempData["Period"] = strPeriod;
                TempData["toPeriod"] = strtoPeriod;
                TempData["Action"] = strAction;
                TempData["RecType"] = strRecType;

                String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                String too = DateTime.ParseExact(strtoPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                DateTime from_dt = Convert.ToDateTime(fromm);
                DateTime to_dt = Convert.ToDateTime(too);

                String diff = (to_dt - from_dt).TotalDays.ToString();
                int diff_dt = Convert.ToInt32(diff);
                if (diff_dt < 0)
                {
                    TempData["Message"] = "To Date should always be greater than or equal to from Date";
                }

                #region "DOWNLOAD"
                if (!string.IsNullOrEmpty(Download))
                {
                    List<IDictionary> GridFill = null;

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                    ViewBag.RecTypeList = LoadDropDowns.Exist_GetGSTRList("TDS,TDSA",strRecType);
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7D"), "ActionName", "ActionName", strAction);
                    ViewBag.ActionList = Actionlst;

                    GspGetGSTR7DataModel GSP_getGSTR7 = new GspGetGSTR7DataModel();
                    if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "ALL")
                    {
                        List<string> mySelectValues = new List<string>();
                        foreach (var items in ViewBag.GSTINNoList)
                        {
                            string gstinno = items.Value;
                            mySelectValues.Add(gstinno);
                        }
                        if (string.IsNullOrEmpty(strAUTHGSTIN))
                        {
                            int icount = 0;
                            for (int i = icount; i < mySelectValues.Count; i++)
                            {
                                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                                Models.GSTAUTH.GspGstAuth.GSTAuthentication(mySelectValues[i], Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                                ViewBag.OTPSession = OTPPOPUPValue;
                                ViewBag.AUTH_Response = OTPAUTHResponse;
                                ViewBag.AUTH_GSTINNo = mySelectValues[i];

                                if (OTPPOPUPValue == "OPEN_POPUP")
                                {
                                    break;
                                }

                                if (OTPPOPUPValue == "CLOSE_POPUP")
                                {

                                    string from_period = strPeriod;
                                    string to_period = strtoPeriod;
                                    string from_month = from_period.Substring(0, 2);
                                    string from_year = from_period.Substring(from_period.Length - 4);
                                    string to_month = to_period.Substring(0, 2);
                                    string to_year = to_period.Substring(to_period.Length - 4);
                                    int cons = 12;
                                    int count1 = 1;
                                    int frm_mon = Convert.ToInt32(from_month);
                                    int to_mon = Convert.ToInt32(to_month);
                                    int diff_month = cons - frm_mon;
                                    int count_month = diff_month + to_mon;
                                    int frm_yr = Convert.ToInt32(from_year);
                                    int diff_year = Convert.ToInt32(to_year) - frm_yr;
                                    if (diff_year > 1)
                                    {
                                        count_month = diff_month + (cons * (diff_year - 1)) + to_mon;
                                    }
                                    if (to_year == from_year)
                                    {
                                        int months = to_mon - frm_mon;
                                        for (int j = 0; j <= months; j++)
                                        {
                                            string period;
                                            if (j == 0)
                                            {
                                                period = from_period;
                                            }
                                            else
                                            {
                                                frm_mon++;
                                                period = Convert.ToString(frm_mon) + from_year;
                                                if (period.Length == 5)
                                                {
                                                    period = "0" + period;
                                                }
                                            }
                                            strPeriod = period;
                                           
                                           
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR7.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            
                                        }
                                    }

                                    else
                                    {
                                        for (int j = 0; j <= count_month; j++)
                                        {
                                            string period;
                                            if (j == 0)
                                            {
                                                period = from_period;
                                            }
                                            else
                                            {
                                                frm_mon++;
                                                if (frm_mon > 12)
                                                {
                                                    frm_mon = count1;
                                                    frm_yr++;
                                                }
                                                period = Convert.ToString(frm_mon) + Convert.ToString(frm_yr);
                                                if (period.Length == 5)
                                                {
                                                    period = "0" + period;
                                                }
                                            }
                                            strPeriod = period;
                                         
                                           
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR7.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            
                                        }
                                    }
                                }

                                strPeriod = strfromPeriod;
                            }
                        }
                        else
                        {
                            var index = mySelectValues.IndexOf(strAUTHGSTIN);
                            for (int i = index; i < mySelectValues.Count; i++)
                            {
                                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                                Models.GSTAUTH.GspGstAuth.GSTAuthentication(mySelectValues[i], Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                                ViewBag.OTPSession = OTPPOPUPValue;
                                ViewBag.AUTH_Response = OTPAUTHResponse;
                                ViewBag.AUTH_GSTINNo = mySelectValues[i];

                                if (OTPPOPUPValue == "OPEN_POPUP")
                                {
                                    break;
                                }

                                if (OTPPOPUPValue == "CLOSE_POPUP")
                                {
                                    string from_period = strPeriod;
                                    string to_period = strtoPeriod;
                                    string from_month = from_period.Substring(0, 2);
                                    string from_year = from_period.Substring(from_period.Length - 4);
                                    string to_month = to_period.Substring(0, 2);
                                    string to_year = to_period.Substring(to_period.Length - 4);
                                    int cons = 12;
                                    int count1 = 1;
                                    int frm_mon = Convert.ToInt32(from_month);
                                    int to_mon = Convert.ToInt32(to_month);
                                    int diff_month = cons - frm_mon;
                                    int count_month = diff_month + to_mon;
                                    int frm_yr = Convert.ToInt32(from_year);
                                    int diff_year = Convert.ToInt32(to_year) - frm_yr;
                                    if (diff_year > 1)
                                    {
                                        count_month = diff_month + (cons * (diff_year - 1)) + to_mon;
                                    }
                                    if (to_year == from_year)
                                    {
                                        int months = to_mon - frm_mon;
                                        for (int j = 0; j <= months; j++)
                                        {
                                            string period;
                                            if (j == 0)
                                            {
                                                period = from_period;
                                            }
                                            else
                                            {
                                                frm_mon++;
                                                period = Convert.ToString(frm_mon) + from_year;
                                                if (period.Length == 5)
                                                {
                                                    period = "0" + period;
                                                }
                                            }
                                            strPeriod = period;
                                        
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR7.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            
                                        }
                                    }
                                    else
                                    {
                                        for (int j = 0; j <= count_month; j++)
                                        {
                                            string period;
                                            if (j == 0)
                                            {
                                                period = from_period;
                                            }
                                            else
                                            {
                                                frm_mon++;
                                                if (frm_mon > 12)
                                                {
                                                    frm_mon = count1;
                                                    frm_yr++;
                                                }
                                                period = Convert.ToString(frm_mon) + Convert.ToString(frm_yr);
                                                if (period.Length == 5)
                                                {
                                                    period = "0" + period;
                                                }
                                            }
                                            strPeriod = period;
                                            
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR7.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            
                                        }
                                    }

                                }
                                index++;
                                strPeriod = strfromPeriod;
                            }
                        }
                        Thread.Sleep(8000);
                        ViewBag.DownloadResponse = "GSTR-1  Download is in progress. Please check after sometime.";
                    }
                    else if (strAction == "RETSTATUS")
                    {
                        string DownloadRes = GSP_getGSTR7.SendRequest(strGSTIN, strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                        ViewBag.DownloadResponse = DownloadRes;
                    }
                    else
                    {
                        string from_period = strPeriod;
                        string to_period = strtoPeriod;
                        string from_month = from_period.Substring(0, 2);
                        string from_year = from_period.Substring(from_period.Length - 4);
                        string to_month = to_period.Substring(0, 2);
                        string to_year = to_period.Substring(to_period.Length - 4);
                        int cons = 12;
                        int count1 = 1;
                        int frm_mon = Convert.ToInt32(from_month);
                        int to_mon = Convert.ToInt32(to_month);
                        int diff_month = cons - frm_mon;
                        int count_month = diff_month + to_mon;
                        int frm_yr = Convert.ToInt32(from_year);
                        int diff_year = Convert.ToInt32(to_year) - frm_yr;

                        if (diff_year > 1)
                        {
                            count_month = diff_month + (cons * (diff_year - 1)) + to_mon;
                        }

                        if (to_year == from_year)
                        {
                            int months = to_mon - frm_mon;
                            for (int i = 0; i <= months; i++)
                            {
                                string period;
                                if (i == 0)
                                {
                                    period = from_period;
                                }
                                else
                                {
                                    frm_mon++;
                                    period = Convert.ToString(frm_mon) + from_year;
                                    if (period.Length == 5)
                                    {
                                        period = "0" + period;
                                    }
                                }
                                strPeriod = period;
                            
                                    string DownloadRes = GSP_getGSTR7.SendRequest(strGSTIN, strPeriod, strAction, strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                    ViewBag.DownloadResponse = DownloadRes;

                                

                            }
                        }
                        else
                        {
                            for (int i = 0; i <= count_month; i++)
                            {
                                string period;
                                if (i == 0)
                                {
                                    period = from_period;
                                }
                                else
                                {
                                    frm_mon++;
                                    if (frm_mon > 12)
                                    {
                                        frm_mon = count1;
                                        frm_yr++;
                                    }
                                    period = Convert.ToString(frm_mon) + Convert.ToString(frm_yr);
                                    if (period.Length == 5)
                                    {
                                        period = "0" + period;
                                    }
                                }
                                strPeriod = period;
                            
                                    string DownloadRes = GSP_getGSTR7.SendRequest(strGSTIN, strPeriod, strAction,strRecType, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                    ViewBag.DownloadResponse = DownloadRes;

                                
                            }
                        }




                        ViewBag.TitleHeaders = "GSTR-1 " + strAction + " Download";

                    }
                }
                #endregion

                #region "EXPORT DATA"
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR7_Download_" + strGSTIN + "_" + strPeriod + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                }
                #endregion

                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = frm["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                    }
                }
                #endregion

                #region "ONCHANGE EVENT"
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion

                ViewBag.TitleHeaders = "GSTR-7 DOWNLOAD ";
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                ViewBag.RecTypeList = LoadDropDowns.GetGSTRList("TDS,TDSA");
                SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7D"), "ActionName", "ActionName", strAction);
                ViewBag.ActionList = Actionlst1;


                #region "LOAD DATA BASED ON ACTION"
                if (strAction == "TDS" ||strAction == "FILEDET")
                {
                    if (!string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strfromPeriod) && !string.IsNullOrEmpty(strtoPeriod))
                    {

                        DataSet GSTR7DSummary = new Gstr7Download(iCustId, iUserId).Retrieve_GSTR7_Download_EXT_SA_Summary(strAction, strGSTIN, strfromPeriod, strtoPeriod);
                        TempData["GSTR7Summary"] = GSTR7DSummary;
                        List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes> GSTR2ASummaryMgmt = new List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes>();

                        if (GSTR7DSummary.Tables.Count > 0)
                        {
                            foreach (DataRow dr in GSTR7DSummary.Tables[0].Rows)
                            {
                                GSTR2ASummaryMgmt.Add(new GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes
                                {
                                    ActionType = dr.IsNull("ActionType") ? "" : dr["ActionType"].ToString(),
                                    Gstin = dr.IsNull("Gstin") ? "" : dr["Gstin"].ToString(),
                                    fp = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                    RecordCount = dr.IsNull("RecordCount") ? 0 : Convert.ToInt32(dr["RecordCount"]),
                                    NoOfInvoices = dr.IsNull("NoOfInvoices") ? 0 : Convert.ToInt32(dr["NoOfInvoices"]),
                                    txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                    RecType = dr.IsNull("RecType") ? "" : dr["RecType"].ToString()

                                });
                            }
                        }
                        Gstr2ADownloadViewModel Model = new Gstr2ADownloadViewModel();
                        Model.GSTR2ASummary = GSTR2ASummaryMgmt;
                        ViewBag.GSTR2aSummary = GSTR2ASummaryMgmt.Count;

                        #region "EXPORT DATA TO EXCEL"
                        if (Command == "exportrawData")
                        {

                            DataSet GstrSummaryraw = new Gstr1Download(iCustId, iUserId).Retrieve_GSTR1_Download_EXT_SA_Bulk_Rawdata(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
                            TempData["Gstr1SummaryRaw"] = GstrSummaryraw;
                            if (strAction == "ALL" || strAction == "FILEDET")
                            {
                                using (DataSet ds = GstrSummaryraw)
                                {
                                    //Set Name of DataTables.
                                    if (ds.Tables[0].Rows.Count > 0)
                                    {
                                        ds.Tables[0].TableName = "B2B";
                                    }
                                    if (ds.Tables[1].Rows.Count > 0)
                                    {
                                        ds.Tables[1].TableName = "B2BA";
                                    }
                                    if (ds.Tables[2].Rows.Count > 0)
                                    {
                                        ds.Tables[2].TableName = "B2CL";
                                    }
                                    if (ds.Tables[3].Rows.Count > 0)
                                    {
                                        ds.Tables[3].TableName = "B2CLA";
                                    }
                                    if (ds.Tables[4].Rows.Count > 0)
                                    {
                                        ds.Tables[4].TableName = "B2CS";
                                    }
                                    if (ds.Tables[5].Rows.Count > 0)
                                    {
                                        ds.Tables[5].TableName = "B2CSA";
                                    }
                                    if (ds.Tables[6].Rows.Count > 0)
                                    {
                                        ds.Tables[6].TableName = "EXP";
                                    }
                                    if (ds.Tables[7].Rows.Count > 0)
                                    {
                                        ds.Tables[7].TableName = "EXPA";
                                    }
                                    if (ds.Tables[8].Rows.Count > 0)
                                    {
                                        ds.Tables[8].TableName = "CDNR";
                                    }
                                    if (ds.Tables[9].Rows.Count > 0)
                                    {
                                        ds.Tables[9].TableName = "CDNRA";
                                    }
                                    if (ds.Tables[10].Rows.Count > 0)
                                    {
                                        ds.Tables[10].TableName = "CDNUR";
                                    }
                                    if (ds.Tables[11].Rows.Count > 0)
                                    {
                                        ds.Tables[11].TableName = "CDNURA";
                                    }
                                    if (ds.Tables[12].Rows.Count > 0)
                                    {
                                        ds.Tables[12].TableName = "HSNSUM";
                                    }
                                    if (ds.Tables[13].Rows.Count > 0)
                                    {
                                        ds.Tables[13].TableName = "NIL";
                                    }
                                    if (ds.Tables[14].Rows.Count > 0)
                                    {
                                        ds.Tables[14].TableName = "DOCISSUE";
                                    }
                                    if (ds.Tables[15].Rows.Count > 0)
                                    {
                                        ds.Tables[15].TableName = "AT";
                                    }
                                    if (ds.Tables[16].Rows.Count > 0)
                                    {
                                        ds.Tables[16].TableName = "ATA";
                                    }
                                    if (ds.Tables[17].Rows.Count > 0)
                                    {
                                        ds.Tables[17].TableName = "TXP";
                                    }
                                    if (ds.Tables[18].Rows.Count > 0)
                                    {
                                        ds.Tables[18].TableName = "TXPA";
                                    }
                                    CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR1Summary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xls");

                                }
                            }
                            else
                            {
                                CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR1Summary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xls");
                            }
                        }
                        #endregion

                        #region "EXPORT DATA TO EXCEL PERIOD WISE"
                        if (command1 == "exportrawData_periodwise")
                        {

                            DataSet GstrSummaryraw = new Gstr1Download(iCustId, iUserId).Retrieve_GSTR1_Download_EXT_SA_Bulk_Rawdata(strAction, gstin, fp, fp, "");
                            TempData["Gstr1SummaryRaw"] = GstrSummaryraw;
                            GridView gv1 = new GridView();
                            gv1.DataSource = TempData["Gstr1SummaryRaw"];
                            gv1.DataBind();
                            Response.ClearContent();
                            Response.Buffer = true;
                            Response.AddHeader("content-disposition", "attachment; filename=GSTR1Summary" + gstin + "_" + fp + "_" + strAction + ".xls");
                            Response.ContentType = "application/ms-excel";
                            Response.Charset = "";
                            StringWriter sw1 = new StringWriter();
                            HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                            gv1.RenderControl(htw1);
                            Response.Output.Write(sw1.ToString());
                            Response.Flush();
                            Response.End();

                        }
                        #endregion

                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                        SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1D"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlst2;
                        return View(Model);
                    }

                }

                else if (strAction == "RETSUM")
                {
                    if (!string.IsNullOrEmpty(strGSTIN) && !string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strfromPeriod))
                    {
                        DataSet RetSum = GetGstr1RetSum(strGSTIN, strfromPeriod, strtoPeriod);
                        TempData["RetSum"] = RetSum;
                        List<GstrCommonDeleteDal.Gstr1RetSum> Gstr1RetSum = new List<GstrCommonDeleteDal.Gstr1RetSum>();
                        if (RetSum.Tables.Count > 0)
                        {
                            foreach (DataRow dr in RetSum.Tables[0].Rows)
                            {
                                Gstr1RetSum.Add(new GstrCommonDeleteDal.Gstr1RetSum
                                {
                                    SNo = dr.IsNull("SNo") ? "" : dr["SNo"].ToString(),
                                    Period = dr.IsNull("Period") ? "" : dr["Period"].ToString(),
                                    actiontype = dr.IsNull("Action Type") ? "" : dr["Action Type"].ToString(),
                                    totalrecords = dr.IsNull("Total Records") ? "" : dr["Total Records"].ToString(),
                                    totalvalue = dr.IsNull("Total Value") ? "" : dr["Total Value"].ToString(),
                                    totaltaxableValue = dr.IsNull("Total Taxable") ? "" : dr["Total Taxable"].ToString(),
                                    totaligst = dr.IsNull("Total IGST Amt") ? "" : dr["Total IGST Amt"].ToString(),
                                    totalcgst = dr.IsNull("Total CGST Amt") ? "" : dr["Total CGST Amt"].ToString(),
                                    totalsgst = dr.IsNull("Total SGST Amt") ? "" : dr["Total SGST Amt"].ToString(),
                                    totalcess = dr.IsNull("Total CESS Amt") ? "" : dr["Total CESS Amt"].ToString(),
                                    nilsupply = dr.IsNull("Total NIL Supply Amt") ? "" : dr["Total NIL Supply Amt"].ToString(),
                                    nilexempted = dr.IsNull("Total NIL Exempted Amt") ? "" : dr["Total NIL Exempted Amt"].ToString(),
                                    nongst = dr.IsNull("Total NonGST Supply Amt") ? "" : dr["Total NonGST Supply Amt"].ToString(),
                                    totaldocissued = dr.IsNull("Total Doc Issued") ? "" : dr["Total Doc Issued"].ToString(),
                                    totaldoccancelled = dr.IsNull("Total Doc Cancelled") ? "" : dr["Total Doc Cancelled"].ToString(),
                                    netdocissued = dr.IsNull("Net Doc Issued") ? "" : dr["Net Doc Issued"].ToString()

                                });
                            }
                        }
                        Gstr2ADownloadViewModel Model = new Gstr2ADownloadViewModel();
                        Model.Gstr1RetSum = Gstr1RetSum;
                        ViewBag.RetSum = Gstr1RetSum.Count;

                        #region "EXPORT DATA TO EXCEL"
                        if (Command == "exportrawData")
                        {

                            DataSet GstrRetSum = GetGstr1RetSum(strGSTIN, strfromPeriod, strtoPeriod);
                            TempData["RetSum"] = GstrRetSum;
                            GridView gv1 = new GridView();
                            gv1.DataSource = TempData["RetSum"];
                            gv1.DataBind();
                            Response.ClearContent();
                            Response.Buffer = true;
                            Response.AddHeader("content-disposition", "attachment; filename=GSTR1Summary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xls");
                            Response.ContentType = "application/ms-excel";
                            Response.Charset = "";
                            StringWriter sw1 = new StringWriter();
                            HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                            gv1.RenderControl(htw1);
                            Response.Output.Write(sw1.ToString());
                            Response.Flush();
                            Response.End();
                        }
                        #endregion

                        ViewBag.Retstatus = null;
                        return View(Model);
                    }
                }

                else if (strAction == "RETSTATUS")
                {
                    if (!string.IsNullOrEmpty(strGSTIN) && !string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strAction))
                    {
                        ViewBag.TitleHeaders = "GSTR-1 DOWNLOAD [ " + strAction + " ] ";
                        if (strAction == "RETSTATUS")
                        {
                            DataSet RetStatus = GetGstr1RetStatus(strGSTIN, strfromPeriod);
                            TempData["RetStatus"] = RetStatus;
                            List<GstrCommonDeleteDal.Gstr1RetStatus> Gstr1RetStatus = new List<GstrCommonDeleteDal.Gstr1RetStatus>();
                            if (RetStatus.Tables.Count > 0)
                            {
                                foreach (DataRow dr in RetStatus.Tables[0].Rows)
                                {
                                    Gstr1RetStatus.Add(new GstrCommonDeleteDal.Gstr1RetStatus
                                    {
                                        gstin = dr.IsNull("ActionType") ? "" : dr["ActionType"].ToString(),
                                        fp = dr.IsNull("Gstin") ? "" : dr["Gstin"].ToString(),
                                        actiontype = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                        referenceno = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                        status = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                        errorreport = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                        createddate = dr.IsNull("fp") ? "" : dr["fp"].ToString()

                                    });
                                }
                            }
                            Gstr2ADownloadViewModel Model = new Gstr2ADownloadViewModel();
                            Model.Gstr1RetStatus = Gstr1RetStatus;
                            ViewBag.RetStatus = Gstr1RetStatus.Count;

                            #region "EXPORT DATA TO EXCEL"
                            if (Command == "exportrawData")
                            {

                                DataSet GstrRetSum = GetGstr1RetSum(strGSTIN, strfromPeriod, strtoPeriod);
                                TempData["RetSum"] = GstrRetSum;
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["RetStatus"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GSTR1Summary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                            }
                            #endregion

                            ViewBag.Retstatus = null;
                            return View(Model);

                        }
                    }
                    return View();
                }
                #endregion


                return View();
            }
            catch (Exception ex)
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1D"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.DownloadResponse = ex.Message.ToString();
                return View();
            }
        }

        public ActionResult Download(string Gstin, string Period, string Referenceid)
        {
            DataSet das = new DataSet();
            string jsondata = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select TOP 1 * from TBL_GSTR7_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period and referenceno = @ReferenceId order by 1 desc";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", Gstin);
                        sqlcmd.Parameters.AddWithValue("@Period", Period);
                        sqlcmd.Parameters.AddWithValue("@ReferenceId", Referenceid);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                jsondata = dt.Rows[0]["errorreport"].ToString();
                            }
                        }
                    }
                    var xd = new XmlDocument();
                    jsondata = "{ \"errors\": {" + jsondata.Trim().TrimStart('{').TrimEnd('}') + @"} }";
                    xd = JsonConvert.DeserializeXmlNode(jsondata);
                    das.ReadXml(new XmlNodeReader(xd));

                    using (DataSet ds = das)
                    {
                        using (XLWorkbook wb = new XLWorkbook())
                        {
                            foreach (DataTable dt1 in ds.Tables)
                            {
                                //Add DataTable as Worksheet.
                                wb.Worksheets.Add(dt1);
                            }

                            //Export the Excel file.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            Response.AddHeader("content-disposition", "attachment;filename=Import_Json_Error.xlsx");
                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                wb.SaveAs(MyMemoryStream);
                                MyMemoryStream.WriteTo(Response.OutputStream);
                                Response.Flush();
                                Response.End();
                            }
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
            return RedirectToAction("Index");
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

        public static List<IDictionary> GetListGSTR1Download(string strGSTINNo, string strPeriod, string strAction, string strtoperiod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dt);
        }

        public static DataTable GetDatatableGSTR1Download(string strGSTINNo, string strPeriod, string strAction, string strtoperiod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return dt;
        }

        public static List<IDictionary> GetListGSTR1_RETSUM_Download(string strGSTINNo, string strPeriod, string strtoperiod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dt);
        }

        public static DataTable GetDatatableGSTR1_RETSUM_Download(string strGSTINNo, string strPeriod, string strtoperiod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_RETSUM", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return dt;

        }

        public DataSet GetGstr1RetSum(string strGSTINNo, string strPeriod, string strtoperiod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (Con.State == ConnectionState.Closed)
                    {
                        Con.Open();
                    }
                    SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_RETSUM", Con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    cmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    cmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(cmd);
                    da.Fill(ds);

                    Con.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;

        }

        public DataSet GetGstr1RetStatus(string strGSTINNo, string strPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR1_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period order by 1 desc";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strPeriod);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            ds.Clear();
                            da.Fill(ds);
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

            return ds;

        }
        public static List<IDictionary> GetListGSTR1RETSTATUS(string strGSTINNo, string strPeriod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR1_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period order by 1 desc";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strPeriod);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            dt.Clear();
                            da.Fill(dt);
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
            return ConvertToDictionary(dt);
        }
    }
}