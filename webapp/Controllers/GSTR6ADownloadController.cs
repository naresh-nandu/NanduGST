using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR6;
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
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartAdminMvc.Controllers
{
    public class Gstr6ADownloadController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: GSTR6ADownload
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
                ViewBag.FromDate = DateTime.Now.ToString("dd-mm-yyyy");
                if (sort != null || page != 0)
                {
                    string Action = TempData["Action"] as string;
                    string Period = TempData["Period"] as string;
                    string GSTIN = TempData["gstn"] as string;
                    string CTIN = TempData["Ctin"] as string;
                    TempData.Keep();

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, CTIN);
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName", Convert.ToString(Action));
                    ViewBag.ActionList = Actionlst;
                    var GridFill = GetListGSTR6ADownload(GSTIN, Period, Action);
                    return View(GridFill);
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    return View();
                }
            }
            catch (Exception ex)
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
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
            {
                return RedirectToAction("Login", "Account");
            }

            string strGSTIN = "", strAction = "", strPeriod = "", strCTIN = "", strToken = "", strAUTHGSTIN = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strToken = frm["token"];
                strAUTHGSTIN = frm["AUTH_GSTINNo"];
                ViewBag.Period = strPeriod;
                strCTIN = frm["ctin"];

                #region "GSTR6A DOWNLOAD"
                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                                        
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
                                    Task.Factory.StartNew(() => new GspGetGstr6ADataModel(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).SendRequestVoid(mySelectValues[i], strPeriod, strAction, strCTIN, strToken)
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
                                    Task.Factory.StartNew(() => new GspGetGstr6ADataModel(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).SendRequestVoid(mySelectValues[i], strPeriod, strAction, strCTIN, strToken)
                                    );
                                    Thread.Sleep(3000);
                                }
                                index++;
                            }
                        }

                        ViewBag.DownloadResponse = "GSTR-6A Bulk Download is in progress. Please check after sometime.";
                    }
                    else
                    {
                        string DownloadRes = new GspGetGstr6ADataModel(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strToken);

                        ViewBag.DownloadResponse = DownloadRes;
                    }
                    if (strAction == "FILEDET" && !string.IsNullOrEmpty(Session["R6AstringAction"] as string))
                    {
                        if (Session["R6AstringAction"].ToString().ToUpper() == "B2B")
                        {
                            strAction = "B2B";
                        }
                        else if (Session["R6AstringAction"].ToString().ToUpper() == "CDN")
                        {
                            strAction = "CDN";
                        }
                        else if (Session["R6AstringAction"].ToString().ToUpper() == "B2BA")
                        {
                            strAction = "B2BA";
                        }
                        else if (Session["R6AstringAction"].ToString().ToUpper() == "CDNA")
                        {
                            strAction = "CDNA";
                        }
                    }
                    if (strAction == "B2B" || strAction == "CDN" || strAction == "B2BA" || strAction == "CDNA")
                    {
                        var ds = GetDatatableGSTR6ADownload(strGSTIN, strPeriod, strAction);
                        var GridFill = GetListGSTR6ADownload(strGSTIN, strPeriod, strAction);

                        TempData["ExportData"] = ds;

                        TempData["gstn"] = strGSTIN;
                        TempData["period"] = strPeriod;
                        TempData["Action"] = strAction;
                        TempData["Ctin"] = strCTIN;

                        ViewBag.TitleHeaders = "GSTR-6A " + strAction + " Download";
                        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlst;
                        ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, strCTIN);
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
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR6A_Download_" + strGSTIN + "_" + strPeriod + "_" + strAction + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, strCTIN);
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
                if (strAction == "B2B" || strAction == "CDN" || strAction == "B2BA" || strAction == "CDNA")
                {
                    var ds = GetDatatableGSTR6ADownload(strGSTIN, strPeriod, strAction);
                    var GridFill = GetListGSTR6ADownload(strGSTIN, strPeriod, strAction);

                    TempData["ExportData"] = ds;

                    TempData["gstn"] = strGSTIN;
                    TempData["period"] = strPeriod;
                    TempData["Action"] = strAction;
                    string FromDate = TempData["FromDate"] as string;

                    ViewBag.TitleHeaders = "GSTR-6A " + strAction + " Download";
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, strCTIN);
                    return View(GridFill);
                }
                #endregion

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst1;
                ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, strCTIN);
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6A"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst1;
                ViewBag.CTINNoList = LoadDropDowns.Exist_CustomerGSTIN(iCustId, strCTIN);
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
        public static List<IDictionary> GetListGSTR6ADownload(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6A_SA", conn);
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

        public static DataTable GetDatatableGSTR6ADownload(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6A_SA", conn);
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
    }
}