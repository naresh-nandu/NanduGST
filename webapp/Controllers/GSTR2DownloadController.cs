using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR2;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr2DownloadController : Controller
    {
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
                    string CTIN = TempData["Ctin"] as string;
                    TempData.Keep();

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, GSTIN, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierGSTIN(iCustId, CTIN);
                    var GridFill = GetListGSTR2Download(GSTIN, Period, Action);
                    return View(GridFill);
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
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

            string strGSTIN = "", strAction = "", strPeriod = "", strRefId = "", strCTIN = "", strToken = "";

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];
                strAction = frm["action"];
                strPeriod = frm["period"];
                strRefId = frm["refid"];
                strToken = frm["token"];
                strCTIN = frm["ctin"];

                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(iCustId);
                    GspGetGstr2DataModel GSP_getGSTR2 = new GspGetGstr2DataModel();
                    string DownloadRes = GSP_getGSTR2.SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strRefId, strToken, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    ViewBag.DownloadResponse = DownloadRes;
                    if (strAction == "RETSTATUS")
                    {
                        ViewBag.Retstatus = Session["RETSTATUS"].ToString();
                    }
                    else if (strAction == "FILEDET")
                    {
                        ViewBag.Retstatus = Session["RETSTATUS"].ToString();
                    }
                    else
                    {
                        ViewBag.Retstatus = null;
                    }
                    var ds = GetDatatableGSTR2Download(strGSTIN, strPeriod, strAction);
                    var GridFill = GetListGSTR2Download(strGSTIN, strPeriod, strAction);

                    TempData["ExportData"] = ds;
                    TempData["gstn"] = strGSTIN;
                    TempData["period"] = strPeriod;
                    TempData["Action"] = strAction;
                    TempData["Ctin"] = strCTIN;

                    ViewBag.TitleHeaders = "GSTR-2 " + strAction + " Download";

                    return View(GridFill);
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR2_Download.xls");
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
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierGSTIN(iCustId, strCTIN);
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

        public static List<IDictionary> GetListGSTR2Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2_D_SA", conn);
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

        public static DataTable GetDatatableGSTR2Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2_D_SA", conn);
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