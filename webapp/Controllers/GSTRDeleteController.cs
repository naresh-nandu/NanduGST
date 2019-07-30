#region Using

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR1;
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Http;
using Microsoft.Owin.Host.SystemWeb;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.IO;
using System.Configuration;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using SmartAdminMvc.Models.ESign;
using System.Threading;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using WeP_BAL.GSTRDelete;
using WeP_DAL.GSTRDelete;
using static WeP_DAL.GSTRDelete.GstrCommonDeleteDal;
using System.Web.UI.WebControls;
using System.Web.UI;
using WeP_BAL.Common;


#endregion

namespace SmartAdminMvc.Controllers
{
    public class GstrDeleteController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: GSTRDelete
        #region "GSTR Delete"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Home()
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
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                ViewBag.GSTRList = LoadDropDowns.GetGSTRList("GSTR1,GSTR1 AMENDMENT,GSTR2,GSTR6");

                ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");

                ViewBag.RecordTypeList = LoadDropDowns.GetRecordTypeList("GSTR DELETE");
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Home(FormCollection frm, string OTPSubmit, string GSTRSummary, string command, int[] ids, string actiontype)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            string GSTRTypeId = "";
            try
            {
                #region "Variable Declare and Initializations"

                string RecordType = frm["ddlRecordType"];
                string InvoiceNums = frm["invnumber"];
                string strGSTINNo = frm["ddlGSTINNo"];
                string strFp = frm["period"];
                string strAction = frm["ddlActionType"];
                string action = frm["actionType"];
                string strGSTRType = frm["ddlGSTR"];
                string strOTP = frm["OTP"].ToString();
                string strAUTH_GSTIN = frm["AUTH_GSTINNo"].ToString();

                ViewBag.GSTRType = strGSTRType;
                
                Session["GSTRType"] = strGSTRType;
                Session["Action"] = strAction;

                TempData["ActionType"] = strAction;
                TempData["GSTINNo"] = strGSTINNo;
                TempData["period"] = strFp;
                TempData["InvNumber"] = InvoiceNums;
                ViewBag.Period = strFp;
                ViewBag.ActionType = strAction;

                #endregion

                #region "Loading Existing Drop Downs"
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                ViewBag.GSTRList = LoadDropDowns.Exist_GetGSTRList("GSTR1,GSTR1 AMENDMENT,GSTR2,GSTR6", strGSTRType);
                ViewBag.ActionList = LoadDropDowns.Exist_GetGSTRActionList(strGSTRType, strAction);
                if (!string.IsNullOrEmpty(strAction))
                {
                    ViewBag.ActionList = LoadDropDowns.Exist_GetGSTRActionList(strGSTRType, strAction);
                }
                

                ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList(RecordType, "GSTR DELETE");
                #endregion

                #region "Swich Case for All Actions based on Button Click"
                switch (command)
                {
                    case "GSTRSummary":
                        DataSet GstrDeleteSummary = new GstrCommonDeleteBal(CustId, UserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary(strGSTRType, strAction, strGSTINNo, strFp, RecordType, InvoiceNums);
                        TempData["GstrDeleteSummary"] = GstrDeleteSummary;
                        List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes> GstrDeleteSummaryMgmt = new List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes>();

                        #region "Data Assign to Attributes"
                        if (GstrDeleteSummary.Tables.Count > 0)
                        {
                            foreach (DataRow dr in GstrDeleteSummary.Tables[0].Rows)
                            {
                                GstrDeleteSummaryMgmt.Add(new GstrCommonDeleteDal.GstrDeleteSummaryAttributes
                                {
                                    ActionType = dr.IsNull("ActionType") ? "" : dr["ActionType"].ToString(),
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
                        #endregion
                        GstrDeleteViewModel Model = new GstrDeleteViewModel();
                        Model.GSTRDeleteSummary = GstrDeleteSummaryMgmt;
                        ViewBag.DeleteSummary = GstrDeleteSummaryMgmt.Count;
                        ViewBag.ReportName = "Delete Summary";
                        return View(Model);

                    case "getDeleteSummaryExport":
                        CommonFunctions.ExportExcel_XLSX(TempData["GstrDeleteSummary"] as DataSet, "GSTRReport_DeleteSummary.xlsx");
                        return RedirectToAction("Home");

                    case "exportrawData":
                        DataSet GstrDeleteraw = new GstrCommonDeleteBal(CustId, UserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Rawdata(strGSTRType, action, strGSTINNo, strFp, RecordType, InvoiceNums);
                        CommonFunctions.ExportExcel_XLSX(GstrDeleteraw, "GSTRReport_DeleteSummary_" + action + ".xlsx");
                        
                        return RedirectToAction("Home");

                    case "deleterawData":

                        Task.Factory.StartNew(() => new GstrCommonDeleteBal(CustId, UserId).Delete_GSTR1_Delete_EXT_SA_Bulk(strGSTRType, action, strGSTINNo, strFp, RecordType, InvoiceNums));
                        Thread.Sleep(10000);

                        if (RecordType == "UPLOADED TO GSTN")
                        {
                            GspSendGstr1ViewDataModel GSP_fnGSTR1View = new GspSendGstr1ViewDataModel();
                            string strJsonData = GstrCommonDeleteBal.GetJsonGSTR1_View(strGSTINNo, strFp, action, "D");
                            strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                            string ViewResponse = GSP_fnGSTR1View.SendRequest(strJsonData, action, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                            TempData["ViewResponse"] = ViewResponse;
                        }
                        else
                        {
                            TempData["ViewResponse"] = "GSTR Data Deleted for : " + action;
                        }

                        DataSet GstrDeleteSummary1 = new GstrCommonDeleteBal(CustId, UserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary(strGSTRType, strAction, strGSTINNo, strFp, RecordType, InvoiceNums);
                        TempData["GstrDeleteSummary"] = GstrDeleteSummary1;
                        List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes> GstrDeleteSummaryMgmt1 = new List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes>();

                        #region "Data Assign to Attributes"
                        if (GstrDeleteSummary1.Tables.Count > 0)
                        {
                            foreach (DataRow dr in GstrDeleteSummary1.Tables[0].Rows)
                            {
                                GstrDeleteSummaryMgmt1.Add(new GstrCommonDeleteDal.GstrDeleteSummaryAttributes
                                {
                                    ActionType = dr.IsNull("ActionType") ? "" : dr["ActionType"].ToString(),
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
                        #endregion

                        GstrDeleteViewModel Model1 = new GstrDeleteViewModel();
                        Model1.GSTRDeleteSummary = GstrDeleteSummaryMgmt1;
                        ViewBag.DeleteSummary = GstrDeleteSummaryMgmt1.Count;
                        ViewBag.ReportName = "Delete Summary";
                        return View(Model1);
                        
                    default:
                        break;

                }
                #endregion

                #region "OTP Request"
                if (RecordType == "UPLOADED TO GSTN" && string.IsNullOrEmpty(strGSTINNo))
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTINNo;

                }
                #endregion

                #region "OTP SUBMIT FOR AUTHTOKEN"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {

                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strAUTH_GSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strAUTH_GSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["SaveResponse"] = "Authenticated Successfully";

                    }
                    else
                    {
                        TempData["AuthMsg"] = status;

                    }
                }
                #endregion
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }

            return View();
        }
                        
        #endregion
    }
}