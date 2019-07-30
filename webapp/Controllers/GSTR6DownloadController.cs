using ClosedXML.Excel;
using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR2;
using SmartAdminMvc.Models.GSTR6;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
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
    [System.Web.Mvc.Authorize]
    public class Gstr6DownloadController : Controller
    {
        // GET: GSTRDownload
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Home(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

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
                    ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierGSTIN(iCustId, CTIN);
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName", Action);
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
                else
                {
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
                
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

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Home(FormCollection frm, string Download, string Export, string OTPSubmit, string command, string command1)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string strGSTIN = "", strAction = "", strPeriod = "", strRefId = "", strCTIN = "", strcommand = "", fp = "", gstin = "", strToken = "", strAUTHGSTIN = "", strtoPeriod = "", strfromPeriod = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strRefId = frm["refid"];
                strToken = frm["token"];
                strAUTHGSTIN = frm["AUTH_GSTINNo"];
                strCTIN = frm["ctin"];
                strtoPeriod = frm["toperiod"];
                strfromPeriod = frm["period"];

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
                TempData["ctin"] = strCTIN;

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

                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.TitleHeaders = "GSTR-6 " + strAction + "Download";

                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);

                    GspGetGstr6DataModel GSP_getGSTR6 = new GspGetGstr6DataModel();
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
                                                SelectValues.Add("ISDA");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
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
                                                SelectValues.Add("ISDA");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
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
                                                SelectValues.Add("ISDA");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
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
                                                SelectValues.Add("ISDA");

                                                for (int J = 0; J < SelectValues.Count(); J++)
                                                {
                                                    Thread.Sleep(5000);
                                                    Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, SelectValues[j], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                                    );
                                                    Thread.Sleep(3000);
                                                }
                                            }
                                            else
                                            {
                                                Thread.Sleep(5000);
                                                Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
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
                        ViewBag.DownloadResponse = "GSTR-6 Bulk Download is in progress. Please check after sometime.";
                    }

                    else if (strAction == "RETSTATUS")
                    {
                        string DownloadRes = GSP_getGSTR6.SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
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
                                if (strAction == "ALL")
                                {
                                    List<string> SelectValues = new List<string>();
                                    SelectValues.Add("B2B");
                                    SelectValues.Add("CDN");
                                    SelectValues.Add("B2BA");
                                    SelectValues.Add("CDNA");
                                    SelectValues.Add("ISD");
                                    SelectValues.Add("ISDA");

                                    for (int J = 0; J < SelectValues.Count(); J++)
                                    {
                                        Thread.Sleep(5000);
                                        Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(strGSTIN, strPeriod, SelectValues[J], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                        );
                                        Thread.Sleep(3000);
                                    }
                                    ViewBag.DownloadResponse = "GSTR-6 Bulk Download is in progress. Please check after sometime.";
                                }
                                else
                                {
                                    string DownloadRes = GSP_getGSTR6.SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
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
                                        Task.Factory.StartNew(() => GSP_getGSTR6.SendRequestVoid(strGSTIN, strPeriod, SelectValues[J], strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                        );
                                        Thread.Sleep(3000);
                                    }
                                    ViewBag.DownloadResponse = "GSTR-6 Bulk Download is in progress. Please check after sometime.";
                                }
                                else
                                {
                                    string DownloadRes = GSP_getGSTR6.SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                    ViewBag.DownloadResponse = DownloadRes;
                                }
                            }

                        }
                    }
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR6_Download.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
                    return View();
                }
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    ViewBag.TitleHeaders = "GSTR-6 " + strAction + "Download";

                    string strOTP = frm["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Home(frm, "Download", "", "","","");
                        }
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Home(frm, "Download", "", "","","");
                        }
                    }
                }
                #region "ONCHANGE EVENT & OTP REQUEST"
                else
                {
                    ViewBag.TitleHeaders = "GSTR-6 Download";
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion

                #region "LOAD DATA BASED ON ACTION"
                if (strAction == "B2B" || strAction == "CDN" || strAction == "B2BA" || strAction == "CDNA" || strAction == "ISD" || strAction == "ISDA" || strAction == "ALL")
                {
                    if (!string.IsNullOrEmpty(strGSTIN) && !string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strfromPeriod))
                    {

                        DataSet GSTR6Summary = new Gstr6Download(iCustId, iUserId).Retrieve_GSTR6_Download_EXT_SA_Bulk_Summary(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
                        TempData["GSTR6Summary"] = GSTR6Summary;
                        List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes> GSTR2ASummaryMgmt = new List<GstrCommonDeleteDal.Gstr2ADownloadSummaryAttributes>();

                        if (GSTR6Summary.Tables.Count > 0)
                        {
                            foreach (DataRow dr in GSTR6Summary.Tables[0].Rows)
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
                        Gstr6DownloadViewModel Model = new Gstr6DownloadViewModel();
                        Model.GSTR6Summary = GSTR2ASummaryMgmt;
                        ViewBag.GSTR6Summary = GSTR2ASummaryMgmt.Count;

                        #region "EXPORT DATA TO EXCEL"
                        if (command == "exportrawData")
                        {
                            DataSet GstrSummaryraw = new Gstr6Download(iCustId, iUserId).Retrieve_GSTR6_Download_EXT_SA_Bulk_Rawdata(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
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
                                    CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR6Summary" + strGSTIN + "_" + strfromPeriod + "_" + strtoPeriod + "_" + strAction + ".xlsx");

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

                            DataSet GstrSummaryraw = new Gstr6Download(iCustId, iUserId).Retrieve_GSTR6_Download_EXT_SA_Bulk_Rawdata(strAction, strGSTIN, strfromPeriod, strtoPeriod, "");
                            CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR6Summary" + strGSTIN + "_" + strfromPeriod + "_" + strAction + ".xlsx");
                        }
                        #endregion

                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                        SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlst2;
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierGSTIN(iCustId, strCTIN);
                        return View(Model);
                    }
                }

                else if (strAction == "RETSTATUS")
                {
                    if (!string.IsNullOrEmpty(strGSTIN) && !string.IsNullOrEmpty(strPeriod) && !string.IsNullOrEmpty(strAction))
                    {
                        ViewBag.TitleHeaders = "GSTR-6 DOWNLOAD [ " + strAction + " ] ";
                        if (strAction == "RETSTATUS")
                        {
                            DataSet RetStatus = GetGstr6RetStatus(strGSTIN, strfromPeriod);
                            TempData["RetStatus"] = RetStatus;
                            List<GstrCommonDeleteDal.Gstr1RetStatus> Gstr6RetStatus = new List<GstrCommonDeleteDal.Gstr1RetStatus>();
                            if (RetStatus.Tables.Count > 0)
                            {
                                foreach (DataRow dr in RetStatus.Tables[0].Rows)
                                {
                                    Gstr6RetStatus.Add(new GstrCommonDeleteDal.Gstr1RetStatus
                                    {
                                        gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                        fp = dr.IsNull("fp") ? "" : dr["fp"].ToString(),
                                        actiontype = dr.IsNull("actiontype") ? "" : dr["actiontype"].ToString(),
                                        referenceno = dr.IsNull("referenceno") ? "" : dr["referenceno"].ToString(),
                                        status = dr.IsNull("status") ? "" : dr["status"].ToString(),
                                        errorreport = dr.IsNull("errorreport") ? "" : dr["errorreport"].ToString(),
                                        createddate = dr.IsNull("createddate") ? "" : dr["createddate"].ToString()

                                    });
                                }
                            }
                            Gstr6DownloadViewModel Model = new Gstr6DownloadViewModel();
                            Model.Gstr6RetStatus = Gstr6RetStatus;
                            ViewBag.RetStatus = Gstr6RetStatus.Count;

                            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                            SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName", Convert.ToString(strAction));
                            ViewBag.ActionList = Actionlst2;
                            ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
                            //ViewBag.Retstatus = null;
                            return View(Model);

                        }
                    }
                    return View();
                }

                
                
                #endregion

            }

            catch (Exception ex)
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
                ViewBag.DownloadResponse = ex.Message;
            }
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
            SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6D"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst1;
            ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
            return View();
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

        public static List<IDictionary> GetListGSTR6Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return ConvertToDictionary(dt);
        }

        public static DataTable GetDatatableGSTR6Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return dt;
        }

        public DataSet GetGstr6RetStatus(string strGSTINNo, string strPeriod)
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
                        sqlcmd.CommandText = "Select * from TBL_GSTR6_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period order by 1 desc";
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
        public static List<IDictionary> GetListGSTR6RETSTATUS(string strGSTINNo, string strPeriod)
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
                        sqlcmd.CommandText = "Select * from TBL_GSTR6_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period order by 1 desc";
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
                        sqlcmd.CommandText = "Select TOP 1 * from TBL_GSTR6_SAVE_RETSTATUS where gstin = @GSTINNo and fp = @Period and referenceno = @ReferenceId order by 1 desc";
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

    }
}