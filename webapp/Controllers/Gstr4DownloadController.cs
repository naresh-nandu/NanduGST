using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR2;
using SmartAdminMvc.Models.GSTR4;
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
    public class Gstr4DownloadController : Controller
    {
        // GET: GSTRDownload

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

                    return View();
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
        public ActionResult Home(FormCollection frm, string Download, string Export, string OTPSubmit)
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
                    GspGetGstr4DataModel GSP_getGSTR4 = new GspGetGstr4DataModel();
                    string DownloadRes = GSP_getGSTR4.SendRequest(strGSTIN, strPeriod, strAction, strCTIN, strRefId, strToken, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

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
                    TempData["gstn"] = strGSTIN;
                    TempData["period"] = strPeriod;
                    TempData["Action"] = strAction;
                    TempData["Ctin"] = strCTIN;

                    ViewBag.TitleHeaders = "GSTR-4 " + strAction + " Download";
                    return View();
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR4_Download.xls");
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

        public static List<IDictionary> GetListGSTR4Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4_D_SA", conn);
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

        public static DataTable GetDatatableGSTR4Download(string strGSTINNo, string strPeriod, string strAction)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4_D_SA", conn);
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