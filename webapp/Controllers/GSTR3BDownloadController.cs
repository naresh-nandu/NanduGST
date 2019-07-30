using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR1;
using SmartAdminMvc.Models.GSTR3;
using System;
using System.Collections;
using System.Collections.Generic;
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

namespace SmartAdminMvc.Controllers
{
    public class Gstr3BDownloadController : Controller
    {
        // GET: GSTR3BDownload
        public ActionResult Index(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {

                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

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

                    var GridFill = GetListGSTR3BDownload(GSTIN, Period, strtoPeriod);
                    return View(GridFill);
                }
                else
                {
                    string GSTIN = TempData["gstn"] as string;

                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.toPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    return View();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string Download, string Export, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            string strGSTIN = "", strAction = "", strPeriod = "", strAUTHGSTIN="", strRefId = "", strStateCode = "", strToken = "", strfromPeriod = "", strtoPeriod = "";
            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strfromPeriod = frm["period"];
                strtoPeriod = frm["toperiod"];
                strRefId = frm["refid"];
                strAUTHGSTIN = frm["AUTH_GSTINNo"];

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

                if (!string.IsNullOrEmpty(Download))
                {
                    List<IDictionary> GridFill = null;
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    GspGetGstr3BDataModel GSP_getGSTR3B = new GspGetGstr3BDataModel();
                    if (strAction == "RETSTATUS")
                    {
                        string DownloadRes = GSP_getGSTR3B.SendRequest(strGSTIN, strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "");
                        ViewBag.DownloadResponse = DownloadRes;

                        if (strAction == "RETSTATUS")
                        {
                            ViewBag.Retstatus = Session["RETSTATUS"].ToString();
                        }
                        else
                        {
                            var ds = GetDatatableGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);
                            GridFill = GetListGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);

                            TempData["ExportData"] = ds;

                            ViewBag.Retstatus = null;
                        }
                    }
                    else
                    {
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
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
                                                Task.Factory.StartNew(() => GSP_getGSTR3B.SendRequest(mySelectValues[i], strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "")
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
                                                Task.Factory.StartNew(() => GSP_getGSTR3B.SendRequest(mySelectValues[i], strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "")
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
                                                Task.Factory.StartNew(() => GSP_getGSTR3B.SendRequest(mySelectValues[i], strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "")
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
                                                Task.Factory.StartNew(() => GSP_getGSTR3B.SendRequest(mySelectValues[i], strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "")
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
                            ViewBag.DownloadResponse = "GSTR-3B  Download is in progress. Please check after sometime.";
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
                                    string DownloadRes = GSP_getGSTR3B.SendRequest(strGSTIN, strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "");
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
                                    string DownloadRes = GSP_getGSTR3B.SendRequest(strGSTIN, strPeriod, strAction, strStateCode, strRefId, strToken, strfromPeriod, strtoPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), "");
                                    ViewBag.DownloadResponse = DownloadRes;
                                }
                            }

                            if (strAction == "RETSTATUS")
                            {
                                ViewBag.Retstatus = Session["RETSTATUS"].ToString();
                            }
                            else if (strAction == "RETSUM")
                            {
                                var ds = GetDatatableGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);
                                GridFill = GetListGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);

                                TempData["ExportData"] = ds;

                                ViewBag.Retstatus = null;
                            }
                            else
                            {
                                var ds = GetDatatableGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);
                                GridFill = GetListGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);

                                TempData["ExportData"] = ds;

                                ViewBag.Retstatus = null;
                            }

                        }

                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());

                        ViewBag.TitleHeaders = "GSTR-3B" + strAction + " Download";
                        return View(GridFill);
                    }
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR3B_Download_ " + strGSTIN + "_" + strfromPeriod + "-" + strtoPeriod + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    //return RedirectToAction("GSTR2ADownload");
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                    return View();
                }
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
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());

                List<IDictionary> GridFill1 = null;
                var ds1 = GetDatatableGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);
                 GridFill1 = GetListGSTR3BDownload(strGSTIN, strfromPeriod, strtoPeriod);

                TempData["ExportData"] = ds1;
                
                
                return View(GridFill1);
            }
            catch (Exception ex)
            {
                ViewBag.DownloadResponse = ex.Message;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                return View();
            }
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

        private object GetDatatableGSTR3BDownload(string strGSTIN, string strPeriod, string strtoperiod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", strGSTIN));
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

        private List<IDictionary> GetListGSTR3BDownload(string strGSTIN, string strPeriod, string strtoperiod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_D_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", strGSTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
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
    }
}