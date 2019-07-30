using ClosedXML.Excel;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR2;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;
using WeP_BAL.GstrDownload;
using WeP_DAL.GSTRDelete;
using static WeP_DAL.GSTRDelete.GstrCommonDeleteDal;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr2ADownloadController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: GSTRDownload

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
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                if (sort != null || page != 0)
                {
                    string Action = TempData["Action"] as string;
                    string Period = TempData["Period"] as string;
                    string GSTIN = TempData["gstn"] as string;
                    string strtoPeriod = TempData["toPeriod"] as string;
                    TempData.Keep();

                    ViewBag.Period = Period;
                    ViewBag.toPeriod = strtoPeriod;
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A"), "ActionName", "ActionName", Convert.ToString(Action));
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
                else
                {
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A" && o.ActionName != "TDS" && o.ActionName != "TDSA"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.DownloadResponse = ex.Message;
                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string Download, string command, string OTPSubmit, string command1)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string strGSTIN = "", strAction = "", strPeriod = "", strToken = "", strAUTHGSTIN = "", strfromPeriod = "", strtoPeriod = "", strcommand = "", fp = "", gstin = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strfromPeriod = frm["period"];
                strtoPeriod = frm["toperiod"];
                strToken = frm["token"];
                strAUTHGSTIN = frm["AUTH_GSTINNo"];
                ViewBag.Period = strPeriod;

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

                #region "GSTR2 DOWNLOAD"
                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.TitleHeaders = "GSTR-2A " + strAction + "Download";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                    GspGetGstr2ADataModel GSP_getGSTR2A = new GspGetGstr2ADataModel();

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
                                            if (strAction == "ALL")
                                            {
                                                List<string> SelectValues = new List<string>();
                                                SelectValues.Add("B2B");
                                                SelectValues.Add("CDN");
                                                SelectValues.Add("B2BA");
                                                SelectValues.Add("CDNA");
                                                SelectValues.Add("ISD");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            }

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
                                            if (strAction == "ALL")
                                            {
                                                List<string> SelectValues = new List<string>();
                                                SelectValues.Add("B2B");
                                                SelectValues.Add("CDN");
                                                SelectValues.Add("B2BA");
                                                SelectValues.Add("CDNA");
                                                SelectValues.Add("ISD");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                   );
                                                Thread.Sleep(3000);
                                            }
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
                                            if (strAction == "ALL")
                                            {
                                                List<string> SelectValues = new List<string>();
                                                SelectValues.Add("B2B");
                                                SelectValues.Add("CDN");
                                                SelectValues.Add("B2BA");
                                                SelectValues.Add("CDNA");
                                                SelectValues.Add("ISD");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            }
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
                                            if (strAction == "ALL")
                                            {
                                                List<string> SelectValues = new List<string>();
                                                SelectValues.Add("B2B");
                                                SelectValues.Add("CDN");
                                                SelectValues.Add("B2BA");
                                                SelectValues.Add("CDNA");
                                                SelectValues.Add("ISD");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                );
                                                Thread.Sleep(3000);
                                            }
                                        }
                                    }

                                }
                                index++;
                                strPeriod = strfromPeriod;
                            }
                        }
                        Thread.Sleep(8000);
                        ViewBag.DownloadResponse = "GSTR-2A Bulk Download is in progress. Please check after sometime.";
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
                                if (strAction == "ALL")
                                {
                                    List<string> SelectValues = new List<string>();
                                    SelectValues.Add("B2B");
                                    SelectValues.Add("CDN");
                                    SelectValues.Add("B2BA");
                                    SelectValues.Add("CDNA");
                                    SelectValues.Add("ISD");

                                    for (int J = 0; J < SelectValues.Count(); J++)
                                    {
                                        Thread.Sleep(5000);
                                        Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(strGSTIN, strPeriod, SelectValues[J], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                        );
                                        Thread.Sleep(3000);
                                    }
                                    ViewBag.DownloadResponse = "GSTR-2A Bulk Download is in progress. Please check after sometime.";
                                }
                                else
                                {
                                    string DownloadRes = GSP_getGSTR2A.SendRequest(strGSTIN, strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                    ViewBag.DownloadResponse = DownloadRes;
                                }
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
                                if (strAction == "ALL")
                                {
                                    List<string> SelectValues = new List<string>();
                                    SelectValues.Add("B2B");
                                    SelectValues.Add("CDN");
                                    SelectValues.Add("B2BA");
                                    SelectValues.Add("CDNA");
                                    SelectValues.Add("ISD");

                                    for (int J = 0; J < SelectValues.Count(); J++)
                                    {
                                        Thread.Sleep(5000);
                                        Task.Factory.StartNew(() => GSP_getGSTR2A.SendRequestVoid(strGSTIN, strPeriod, SelectValues[J], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                        );
                                        Thread.Sleep(3000);
                                    }
                                    ViewBag.DownloadResponse = "GSTR-2A Bulk Download is in progress. Please check after sometime.";
                                }
                                else
                                {
                                    string DownloadRes = GSP_getGSTR2A.SendRequest(strGSTIN, strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                    ViewBag.DownloadResponse = DownloadRes;
                                }
                            }
                        }
                    }

                }
                #endregion

                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    ViewBag.TitleHeaders = "GSTR-2A " + strAction + "Download";
                    string strOTP = frm["OTP"].ToString();
                    string strAUTH_GSTIN = frm["AUTH_GSTINNo"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strAUTH_GSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strAUTH_GSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Index(frm, "Download", "", "", "");
                        }
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Index(frm, "Download", "", "", "");
                        }
                    }
                }
                #endregion

                #region "ONCHANGE EVENT & OTP REQUEST"
                else
                {
                    ViewBag.TitleHeaders = "GSTR-2A Download";
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion

                #region "LOAD DATA BASED ON ACTION"
                if (strAction == "B2B" || strAction == "CDN" || strAction == "B2BA" || strAction == "CDNA" || strAction == "ISD" || strAction == "ISDA" || strAction == "TDS" || strAction == "TDSA" || strAction == "ALL" || strAction == "FILEDET")
                {
                    if (!string.IsNullOrEmpty(strGSTIN) && !string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strfromPeriod))
                    {
                        
                        DataSet GSTR2ASummary = new Gstr2ADownload(iCustId, iUserId).Retrieve_GSTR2A_Download_EXT_SA_Bulk_Summary(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
                        TempData["GSTR2ASummary"] = GSTR2ASummary;
                        List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes> GSTR2ASummaryMgmt = new List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes>();

                        if (GSTR2ASummary.Tables.Count > 0)
                        {
                            foreach (DataRow dr in GSTR2ASummary.Tables[0].Rows)
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
                        if (command == "exportrawData")
                        {
                            DataSet GstrSummaryraw = new Gstr2ADownload(iCustId, iUserId).Retrieve_GSTR2A_Download_EXT_SA_Bulk_Rawdata(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
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
                                        ds.Tables[1].TableName = "CDNR";
                                    }
                                    if (ds.Tables[2].Rows.Count > 0)
                                    {
                                        ds.Tables[2].TableName = "ISD";
                                    }
                                    if (ds.Tables[3].Rows.Count > 0)
                                    {
                                        ds.Tables[3].TableName = "ISDA";
                                    }
                                    if (ds.Tables[4].Rows.Count > 0)
                                    {
                                        ds.Tables[4].TableName = "B2BA";
                                    }
                                    if (ds.Tables[5].Rows.Count > 0)
                                    {
                                        ds.Tables[5].TableName = "CDNRA";
                                    }
                                    CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR2ASummary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xlsx");

                                }
                            }

                            else
                            {

                                CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR2ASummary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xlsx");
                            }
                        }
                        #endregion

                        #region "EXPORT DATA TO EXCEL PERIOD WISE"
                        if (command1 == "exportrawData_periodwise")
                        {

                            DataSet GstrSummaryraw = new Gstr2ADownload(iCustId, iUserId).Retrieve_GSTR2A_Download_EXT_SA_Bulk_Rawdata(strAction, gstin, fp, fp, "");
                            CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR2ASummary" + gstin + "_" + fp + "_" + strAction + ".xlsx");
                        }
                        #endregion

                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                        SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlst2;
                        return View(Model);
                    }
                }
                #endregion
            }
            catch (Exception ex)
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.DownloadResponse = ex.Message;
            }
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
            SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2A"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst1;
            return View();
        }



    }
}