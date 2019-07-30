using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR4;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartAdminMvc.Controllers
{
    public class Gstr4ADownloadController : Controller
    {
        // GET: Gstr4ADownload

        private readonly WePGSPDBEntities db = new WePGSPDBEntities();


        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
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
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.FromDate = DateTime.Now.ToString("dd-mm-yyyy");
                if (sort != null || page != 0)
                {
                    string Action = TempData["Action"] as string;
                    string Period = TempData["Period"] as string;
                    string GSTIN = TempData["gstn"] as string;
                    string FromDate = TempData["FromDate"] as string;
                    TempData.Keep();

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName", Convert.ToString(Action));
                    ViewBag.ActionList = Actionlst;
                    var GridFill = GetListGSTR4ADownload(GSTIN, Period, Action);
                    return View(GridFill);
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public ActionResult Index(FormCollection frm, string Download, string Export, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            string strGSTIN = "", strAction = "", strPeriod = "", strToken = "", strAUTHGSTIN = "", strFromDate = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strToken = frm["token"];
                strAUTHGSTIN = frm["AUTH_GSTINNo"];
                strFromDate = frm["fromdate"];
                ViewBag.Period = strPeriod;

                #region "GSTR4A DOWNLOAD"
                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                    GspGetGstr4ADataModel GSP_getGSTR4A = new GspGetGstr4ADataModel();

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

                                if (OTPPOPUPValue == "CLOSE_POPUP")
                                {
                                    Thread.Sleep(5000);
                                    Task.Factory.StartNew(() => GSP_getGSTR4A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strFromDate, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                    );
                                    Thread.Sleep(3000);
                                }
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

                                if (OTPPOPUPValue == "CLOSE_POPUP")
                                {
                                    Thread.Sleep(5000);
                                    Task.Factory.StartNew(() => GSP_getGSTR4A.SendRequestVoid(mySelectValues[i], strPeriod, strAction, strToken, strFromDate, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                                    );
                                    Thread.Sleep(3000);
                                }
                                index++;
                            }
                        }

                        ViewBag.DownloadResponse = "GSTR-4A Bulk Download is in progress. Please check after sometime.";
                    }
                    else
                    {
                        string DownloadRes = GSP_getGSTR4A.SendRequest(strGSTIN, strPeriod, strAction, strToken, strFromDate, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                        ViewBag.DownloadResponse = DownloadRes;
                    }
                    if (strAction == "FILEDET" && !string.IsNullOrEmpty(Session["R4AstringAction"] as string))
                    {
                        if (Session["R4AstringAction"].ToString().ToUpper() == "B2B")
                        {
                            strAction = "B2B";
                        }
                        else if (Session["R4AstringAction"].ToString().ToUpper() == "CDNR")
                        {
                            strAction = "CDNR";
                        }
                        else if (Session["R4AstringAction"].ToString().ToUpper() == "CDNRA")
                        {
                            strAction = "CDNRA";
                        }
                        else if (Session["R4AstringAction"].ToString().ToUpper() == "B2BA")
                        {
                            strAction = "B2BA";
                        }
                        else if (Session["R4AstringAction"].ToString().ToUpper() == "TDS")
                        {
                            strAction = "TDS";
                        }
                    }
                    if (strAction == "B2B" || strAction == "CDNR" || strAction == "CDNRA" || strAction == "B2BA" || strAction == "TDS")
                    {
                        var ds = GetDatatableGSTR4ADownload(strGSTIN, strPeriod, strAction);
                        var GridFill = GetListGSTR4ADownload(strGSTIN, strPeriod, strAction);

                        TempData["ExportData"] = ds;

                        TempData["gstn"] = strGSTIN;
                        TempData["period"] = strPeriod;
                        TempData["Action"] = strAction;
                        string FromDate = TempData["FromDate"] as string;

                        ViewBag.TitleHeaders = "GSTR-4A " + strAction + " Download";
                        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlst;
                        return View(GridFill);
                    }
                }
                #endregion

                #region "EXPORT DATA TO EXCEL"
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR4A_Download_" + strGSTIN + "_" + strPeriod + "_" + strAction + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
                #endregion

                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = frm["OTP"].ToString();
                    string strAUTH_GSTIN = frm["AUTH_GSTINNo"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strAUTH_GSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strAUTH_GSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Index(frm, "Download", "", "");
                        }
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                        if (string.IsNullOrEmpty(strGSTIN) || strGSTIN == "Select ALL")
                        {
                            return Index(frm, "Download", "", "");
                        }
                    }
                }
                #endregion

                #region "ONCHANGE EVENT & OTP REQUEST"
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                #endregion

                #region "LOAD DATA BASED ON ACTION"
                if (strAction == "B2B" || strAction == "CDNR" || strAction == "B2BA" || strAction == "CDNRA" || strAction == "TDS")
                {
                    var ds = GetDatatableGSTR4ADownload(strGSTIN, strPeriod, strAction);
                    var GridFill = GetListGSTR4ADownload(strGSTIN, strPeriod, strAction);

                    TempData["ExportData"] = ds;

                    TempData["gstn"] = strGSTIN;
                    TempData["period"] = strPeriod;
                    TempData["Action"] = strAction;
                    string FromDate = TempData["FromDate"] as string;

                    ViewBag.TitleHeaders = "GSTR-4A " + strAction + " Download";
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    return View(GridFill);
                }
                #endregion

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4A"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst1;
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.DownloadResponse = ex.Message;
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
        public static List<IDictionary> GetListGSTR4ADownload(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4A_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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

        public static DataTable GetDatatableGSTR4ADownload(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4A_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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

    }
}