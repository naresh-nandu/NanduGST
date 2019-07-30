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
using System.Configuration;
using System.IO;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using System.Xml;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR6;
using System.Globalization;
using WeP_BAL.Reconcilation;
using ClosedXML.Excel;

#endregion
namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr6Controller : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Save()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR6 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
            ViewBag.ActionList = Actionlst;

            LoadSessionVariables();

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm, string GSTR6Save, string GetJson, string OTPSubmit, string command, string[] RefIds)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string strInvIds = "";
            string strGSTINNo = frm["ddlGSTINNo"];
            string strFp = DateTime.Now.ToString(frm["period"]);
            string strAction = frm["ddlActionType"];
            TempData["ActionType"] = strAction;
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            strInvIds = frm["InvIds"];
            ViewBag.Period = strFp;
            ViewBag.ActionType = strAction;

            DeleteGstr6Sa objDeleteGSTR6 = new DeleteGstr6Sa();
            #region "GSTR6 SAVE"
            if (!string.IsNullOrEmpty(GSTR6Save))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr6SaveDataModel fnGSTR6Save = new GspSendGstr6SaveDataModel();
                    string strJsonData = Gstr6DataModel.GetJsonGSTR6Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    string SaveResponse = fnGSTR6Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    TempData["SaveResponse"] = SaveResponse;

                    SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst1;
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                    return View();
                }
            }
            #endregion

            #region "GET JSON PAYLOAD"
            else if (!string.IsNullOrEmpty(GetJson))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr6SaveDataModel fnGSTR6Save = new GspSendGstr6SaveDataModel();
                    string strJsonData = Gstr6DataModel.GetJsonGSTR6Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                    ViewBag.GET_GSTR6JsonResponse = strJsonData;
                }
            }
            #endregion

            #region "OTP SUBMIT FOR AUTH TOKEN"
            else if (!string.IsNullOrEmpty(OTPSubmit))
            {
                string strOTP = frm["OTP"].ToString();
                Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTINNo);
                string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
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

            #region "GSTR6 DELETE"
            //Deleting Invoices from GSTR6 B2B
               else if (RefIds != null)
                {
                if (command == "GSTR6B2B")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR6.GSTR6Delete("B2B", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                //Deleting Invoices from GSTR6 CDN
                else if (command == "GSTR6CDN")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR6.GSTR6Delete("CDN", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                //Deleting Invoices from GSTR6 B2BA
                else if (command == "GSTR6B2BA")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR6.GSTR6Delete("B2BA", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                //Deleting Invoices from GSTR6 CDNA
                else if (command == "GSTR6CDNA")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR6.GSTR6Delete("CDNA", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                //Deleting Invoices from GSTR6 ISD
                else if (command == "GSTR6ISD")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR6.GSTR6Delete("ISD", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                 }
             

            //Deleting Invoices from GSTR6 CDN
            //else if (command == "GSTR6CDN")
            //{
            //    objDeleteGSTR6.GSTR6Delete("CDN", ids);
            //    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            //}

            ////Deleting Invoices from GSTR6 B2BA
            //else if (command == "GSTR6B2BA")
            //{
            //    objDeleteGSTR6.GSTR6Delete("B2BA", ids);
            //    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            //}
            ////Deleting Invoices from GSTR6 CDNA
            //else if (command == "GSTR6CDNA")
            //{
            //    objDeleteGSTR6.GSTR6Delete("CDNA", ids);
            //    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            //}
            ////Deleting Invoices from GSTR6 ISD
            //else if (command == "GSTR6ISD")
            //{
            //    objDeleteGSTR6.GSTR6Delete("ISD", ids);
            //    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            //}


            #endregion

            #region "ONCHANGE EVENT - OTP REQUEST"
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());

                ViewBag.TitleHeaders = "GSTR-6 Save";

                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                ViewBag.OTPSession = OTPPOPUPValue;
                ViewBag.AUTH_Response = OTPAUTHResponse;
                ViewBag.AUTH_GSTINNo = strGSTINNo;
            }
            #endregion

            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.TitleHeaders = "GSTR-6 Save ";
            LoadSessionVariables();
            LoadGSTR6Data(strAction, strGSTINNo, strFp);
            return View();
        }

        public void LoadSessionVariables()
        {
           
           Session["G6B2BRefIds"] = string.Empty;
            Session["G6CDNRRefIds"] = string.Empty;
            Session["G6CDNRARefIds"] = string.Empty;
            Session["G6B2BARefIds"] = string.Empty;
            Session["G6ISDRefIds"] = string.Empty;
            Session["G6ISDARefIds"] = string.Empty;

            Session["dataCount_G6B2B"] = 0;
            Session["dataCount_G6B2BA"] = 0;
            Session["dataCount_G6CDNR"] = 0;
            Session["dataCount_G6CDNRA"] = 0;
            Session["dataCount_G6ISD"] = 0;
            Session["dataCount_G6ISD"] = 0;
        }

        public void LoadGSTR6Data(string strAction, string strGSTINNo, string strFp)
        {
            LoadSessionVariables();
            if (strAction == "B2B" || strAction == "ALL")
            {
                var B2B = Gstr6DataModel.GetB2B6(strGSTINNo, strFp, "");
                ViewBag.B2B = B2B;
            }
            if (strAction == "B2BA" || strAction == "ALL")
            {
                var B2BA = Gstr6DataModel.GetB2BA6(strGSTINNo, strFp, "");
                ViewBag.B2BA = B2BA;
            }
         
            if (strAction == "CDN" || strAction == "ALL")
            {
                var CDN = Gstr6DataModel.GetCDNR6(strGSTINNo, strFp, "");
                ViewBag.CDN = CDN;
            }
            if (strAction == "CDNA" || strAction == "ALL")
            {
                var CDNA = Gstr6DataModel.GetCDNRA6(strGSTINNo, strFp, "");
                ViewBag.CDNA = CDNA;
            }
            if (strAction == "ISD" || strAction == "ALL")
            {
                var ISD = Gstr6DataModel.GetISD6(strGSTINNo, strFp, "");
                ViewBag.ISD = ISD;
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult File(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR6 File Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            if (sort != null || page != 0)
            {
                string strFp = TempData["Period"] as string;
                string strActionId = TempData["ActionId"] as string;
                string strGSTINId = TempData["GSTINId"] as string;
                string strAction = TempData["ActionType"] as string;
                string strGSTINNo = TempData["GSTINNo"] as string;
                TempData.Keep();
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = Gstr6DataModel.GetListGSTR6File(strGSTINNo, strAction, strFp);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ESign, string GSTR6File, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int strGSTINId = Convert.ToInt32(frm["ddlGSTINNo"]);
            int strActionId = Convert.ToInt32(frm["ddlActionType"]);

            string strGSTINNo = (from lst in db.TBL_Cust_GSTIN
                                 where lst.GSTINId == strGSTINId
                                 select lst.GSTINNo).SingleOrDefault();

            string strAction = (from lst1 in db.TBL_GSTR_ACTION_TYPE
                                where lst1.ActionId == strActionId
                                select lst1.ActionName).SingleOrDefault();

            string strFp = DateTime.Now.ToString("MMyyyy");

            TempData["ActionId"] = Convert.ToString(strActionId);
            TempData["GSTINId"] = Convert.ToString(strGSTINId);
            TempData["ActionType"] = strAction;
            TempData["GSTINNo"] = strGSTINNo;
            TempData["Period"] = strFp;

            if (!string.IsNullOrEmpty(GSTR6File))
            {
                string strJsonData = Gstr6DataModel.GetJsonGSTR6File(strAction, strGSTINNo, strFp);

                string guid = Guid.NewGuid().ToString();
                guid = guid.Replace("-", "").ToUpper();

                Models.ESign.OldTCUtilsBuffer tcutil = new Models.ESign.OldTCUtilsBuffer();
                string fileName = Path.GetFileName("sample.txt");
                string path = Path.Combine(Server.MapPath("~\\App_Data\\ESign\\"), fileName);
                path = path.Replace("\\", "/");

                Task<string> ResStatus = tcutil.uploadBuffer(guid, "mgrdsfa@gmail.com", path);

                string cs = tcutil.GetCS(guid);
                XmlDocument xmlDoc = new XmlDocument();
                xmlDoc.LoadXml(ResStatus.Result.ToString());

                string status = "";
                XmlDocument xmlDocument1 = new XmlDocument();
                xmlDocument1.LoadXml(ResStatus.Result.ToString());
                foreach (XmlNode xmlNode in xmlDocument1.SelectNodes("response/child::node()"))
                {
                    if (xmlNode.Name == "status")
                        status = xmlNode.InnerText;
                }

                if (status == "0")
                {
                    ViewBag.ESIGNDialog = "OPEN_ESIGNPOPUP";
                    TempData["ESignPopup"] = "renderWidget('widgetdiv','https://wepindia.truecopy.in/corp/v2/widgetsigner.tc?uuid=" + guid + "&cs=" + cs + "&fn=WeP');";
                }
                else
                {
                    ViewBag.ESIGNDialog = "CLOSE_ESIGNPOPUP";
                }

                var GridFill = Gstr6DataModel.GetListGSTR6File(strGSTINNo, strAction, strFp);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                return View(GridFill);
            }
            else if (!string.IsNullOrEmpty(ESign))
            {

                TempData["FileResponse"] = "GSTR6 Filed Successfully... Status - SUCCESS and Ack No. - ASDFSDF1241343";

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
            else if (!string.IsNullOrEmpty(OTPSubmit))
            {
                string strOTP = frm["OTP"].ToString();
                Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, "");
                string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                if (status == "1")
                {
                    TempData["AuthMsg"] = "Authenticated Successfully";
                }
                else
                {
                    TempData["AuthMsg"] = "Authentication Error";
                }

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            else
            {
                ViewBag.TitleHeaders = "GSTR-6 File - [" + strAction + "]";
                var GridFill = Gstr6DataModel.GetListGSTR6File(strGSTINNo, strAction, strFp);

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());
                return View(GridFill);
            }
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Reconcilation()
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["User_ID"] == null)
                    return RedirectToAction("Login", "Account");

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";

                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                fromPeriod = DateTime.Now.ToString("MMyyyy");
                toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.TotalInvoicesTitle = "Matched Invoices";
                ViewBag.MismatchInvoicesTitle = "Mismatch Invoices in GSTR 6 and 6A";
                ViewBag.MissingInvoicesGSTR6ATitle = "Missing Invoices in GSTR-6A";
                ViewBag.MissingInvoicesGSTR6Title = "Missing Invoices in GSTR-6";

                if (Session["CF_GSTIN_GSTR6"] != null)
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                }
                if (Session["CF_PERIOD_GSTR6"] != null)
                {
                    ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                    fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                } 
                if (Session["CF_TOPERIOD_GSTR6"] != null)
                {
                    ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                    toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                } 
                if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME_GSTR6"].ToString());
                   // ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(CustId, Session["CF_SUPNAME_GSTR6"].ToString());
                    if (Session["CF_CTIN_GSTR6"] != null)
                    {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN_GSTR6"].ToString());
                       // ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(CustId, Session["CF_SUPNAME_GSTR6"].ToString(), Session["CF_CTIN_GSTR6"].ToString());
                    }
                    else
                    {
                       // ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(CustId, Session["CF_SUPNAME_GSTR6"].ToString());
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name");
                   // ViewBag.SupplierName = LoadDropDowns.SupplierName(CustId);
                    if (Session["CF_CTIN_GSTR6"] != null)
                    {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN_GSTR6"].ToString());
                       // ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(CustId, UserId, Session["CF_CTIN_GSTR6"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                       // ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(CustId, UserId);
                    }
                }

                GetReconcileCount(CustId, UserId, strGStin, cName, Ctin, fromPeriod, toPeriod);

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Reconcilation(FormCollection frm, string strCommand)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["User_ID"] == null)
                    return RedirectToAction("Login", "Account");

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                string strGSTIN = "", strPeriod = "", strSupplierName = "", strCTIN = "", strtoperiod = "";

                strGSTIN = frm["ddlGSTINNo"];
                strPeriod = frm["period"];
                strSupplierName = frm["ddlSupplierName"];
                strCTIN = frm["ddlSupGSTIN"];
                strtoperiod = frm["toperiod"];

                ViewBag.Period = strPeriod;
                ViewBag.ToPeriod = strtoperiod;

                String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                String too = DateTime.ParseExact(strtoperiod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                DateTime from_dt = Convert.ToDateTime(fromm);
                DateTime to_dt = Convert.ToDateTime(too);

                String diff = (to_dt - from_dt).TotalDays.ToString();
                int diff_dt = Convert.ToInt32(diff);
                if (diff_dt < 0)
                {
                    TempData["Message"] = "To Date should always be greater than or equal to from Date";
                }

                if (string.IsNullOrEmpty(strGSTIN))
                {
                    strGSTIN = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTIN, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.SupplierName(CustId);
                    ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(CustId, UserId);
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(CustId, strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(CustId, strSupplierName);
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(CustId, UserId);
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(CustId, strSupplierName, strCTIN);
                }

                if ((string.IsNullOrEmpty(strSupplierName) || strSupplierName == "ALL") && !string.IsNullOrEmpty(strCTIN))
                {
                    ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(CustId, UserId, strCTIN);
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(CustId, strSupplierName, strCTIN);
                }
                Session["CF_GSTIN_GSTR6"] = strGSTIN;
                Session["CF_SUPNAME_GSTR6"] = strSupplierName;
                Session["CF_CTIN_GSTR6"] = strCTIN;
                Session["CF_PERIOD_GSTR6"] = strPeriod;
                Session["CF_TOPERIOD_GSTR6"] = strtoperiod;

                if (strCommand == "RawData")
                {
                    var das = ReconcilationBl.GetDataSetReconciliationLog_GSTR6(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);
                    using (DataSet ds = das)
                    {
                        //Set Name of DataTables.
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            ds.Tables[0].TableName = "ActivityLogSummary";
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            ds.Tables[1].TableName = "All Invoices-B2B";
                        }
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            ds.Tables[2].TableName = "All Invoices-CDNR";
                        }
                        if (ds.Tables[3].Rows.Count > 0)
                        {
                            ds.Tables[3].TableName = "MatchedRecord-B2B";
                        }
                        if (ds.Tables[4].Rows.Count > 0)
                        {
                            ds.Tables[4].TableName = "MatchedRecord-CDNR";
                        }
                        if (ds.Tables[5].Rows.Count > 0)
                        {
                            ds.Tables[5].TableName = "MissingGSTR6ARecords-B2B";
                        }
                        if (ds.Tables[6].Rows.Count > 0)
                        {
                            ds.Tables[6].TableName = "MissingGSTR6ARecords-CDNR";
                        }
                        if (ds.Tables[7].Rows.Count > 0)
                        {
                            ds.Tables[7].TableName = "MissingGSTR6Records-B2B";
                        }
                        if (ds.Tables[8].Rows.Count > 0)
                        {
                            ds.Tables[8].TableName = "MissingGSTR6Records-CDNR";
                        }
                        if (ds.Tables[9].Rows.Count > 0)
                        {
                            ds.Tables[9].TableName = "MismatchedRecords-B2B";
                        }
                        if (ds.Tables[10].Rows.Count > 0)
                        {
                            ds.Tables[10].TableName = "MismatchedRecords-CDNR";
                        }
                        if (ds.Tables[11].Rows.Count > 0)
                        {
                            ds.Tables[11].TableName = "AccpetedRecord-B2B";
                        }
                        if (ds.Tables[12].Rows.Count > 0)
                        {
                            ds.Tables[12].TableName = "AccpetedRecord-CDNR";
                        }
                        if (ds.Tables[13].Rows.Count > 0)
                        {
                            ds.Tables[13].TableName = "RejectedRecord-B2B";
                        }
                        if (ds.Tables[14].Rows.Count > 0)
                        {
                            ds.Tables[14].TableName = "RejectedRecord-CDNR";
                        }
                        if (ds.Tables[15].Rows.Count > 0)
                        {
                            ds.Tables[15].TableName = "PendingRecord-B2B";
                        }
                        if (ds.Tables[16].Rows.Count > 0)
                        {
                            ds.Tables[16].TableName = "PendingRecord-CDNR";
                        }
                        if (ds.Tables[17].Rows.Count > 0)
                        {
                            ds.Tables[17].TableName = "ModifiedRecord-B2B";
                        }
                        if (ds.Tables[18].Rows.Count > 0)
                        {
                            ds.Tables[18].TableName = "ModifiedRecord-CDNR";
                        }
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
                            //Export the Excel file.
                            Response.Clear();
                            Response.Buffer = true;
                            Response.Charset = "";
                            Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                            // Response.AddHeader("content-disposition", "attachment;filename=ReconciliationLog_For" + strGSTIN + ".xlsx");
                            Response.AddHeader("content-disposition", "attachment; filename=ReconciliationLog_For_" + strGSTIN + "_" + strPeriod + "_" + strtoperiod + ".xls");
                            using (MemoryStream MyMemoryStream = new MemoryStream())
                            {
                                if (wb.Worksheets.Count > 0)
                                {
                                    wb.SaveAs(MyMemoryStream);
                                    MyMemoryStream.WriteTo(Response.OutputStream);
                                }
                                Response.Flush();
                                Response.End();
                            }
                        }
                    }
                }
                if (strCommand == "Reset")
                {
                    Task.Factory.StartNew(() => ReconcilationBl.ReconciliationReset_GSTR6(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod));
                    TempData["Message"] = "Reconciliation reset done successfully";
                }
                GetReconcileCount(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return View();
        }

        public void GetReconcileCount(int CustId, int UserId, string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string strtoperiod)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoice_Statistics_GSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtoperiod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();
                    ds.Clear();

                    da.Fill(dt);
                    conn.Close();

                    if (dt.Rows.Count > 0)
                    {
                        ViewBag.MatchedInvoices = dt.Rows[0][1].ToString();
                        ViewBag.MissinginGSTR6A = dt.Rows[1][1].ToString();
                        ViewBag.MissinginGSTR6 = dt.Rows[2][1].ToString();
                        ViewBag.MismatchInvoices = dt.Rows[3][1].ToString();

                        ViewBag.AcceptedInvoices = dt.Rows[4][1].ToString();
                        ViewBag.RejectedInvoices = dt.Rows[5][1].ToString();
                        ViewBag.PendingInvoices = dt.Rows[6][1].ToString();
                        ViewBag.ModifiedInvoices = dt.Rows[7][1].ToString();
                    }
                    else
                    {
                        ViewBag.MatchedInvoices = "0";
                        ViewBag.MissinginGSTR6A = "0";
                        ViewBag.MissinginGSTR6 = "0";
                        ViewBag.MismatchInvoices = "0";
                        ViewBag.AcceptedInvoices = "0";
                        ViewBag.ModifiedInvoices = "0";
                        ViewBag.RejectedInvoices = "0";
                        ViewBag.PendingInvoices = "0";
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

        }

    }
}