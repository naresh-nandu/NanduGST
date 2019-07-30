#region Using

using ClosedXML.Excel;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR2;
using SmartAdminMvc.Models.Reconcilation;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.UI;
using System.Xml;
using WeP_BAL.Common;
using WeP_BAL.Reconcilation;
using WeP_DAL;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr2Controller : Controller
    {
        string fileNameKey = String.Format("{0}_{1}", Guid.NewGuid(), DateTime.Now.Ticks);
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        #region "GSTR2 SAVE"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Save(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR2 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
            ViewBag.ActionList = Actionlst;

            LoadSessionVariables();

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm, string GSTR2Save, string GetJson, string OTPSubmit, string command, int[] ids)
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

            string strGSTINNo = frm["ddlGSTINNo"];
            string strFp = DateTime.Now.ToString(frm["period"]);
            string strAction = frm["ddlActionType"];
            TempData["ActionType"] = strAction;
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            ViewBag.Period = strFp;
            ViewBag.ActionType = strAction;

            DeleteGstr2Sa objDeleteGSTR2 = new DeleteGstr2Sa();

            #region "GSTR2 SAVE"
            if (!string.IsNullOrEmpty(GSTR2Save))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr2SaveDataModel fnGSTR2Save = new GspSendGstr2SaveDataModel();
                    string strJsonData = Gstr2DataModel.GetJsonGSTR2Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    string SaveResponse = fnGSTR2Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    TempData["SaveResponse"] = SaveResponse;

                    SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
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
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr2SaveDataModel fnGSTR2Save = new GspSendGstr2SaveDataModel();
                    string strJsonData = Gstr2DataModel.GetJsonGSTR2Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                    ViewBag.GET_GSTR2JsonResponse = strJsonData;

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

            #region "GSTR2 DELETE"
            //Deleting Invoices from GSTR2 B2B
            else if (command == "GSTR2B2B")
            {
                objDeleteGSTR2.GSTR2Delete("B2B", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 IMPG
            else if (command == "GSTR2IMPG")
            {
                objDeleteGSTR2.GSTR2Delete("IMPG", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 IMPS
            else if (command == "GSTR2IMPS")
            {
                objDeleteGSTR2.GSTR2Delete("IMPS", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 CDN
            else if (command == "GSTR2CDN")
            {
                objDeleteGSTR2.GSTR2Delete("CDN", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 Nil
            else if (command == "GSTR2NIL")
            {
                objDeleteGSTR2.GSTR2Delete("NIL", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 TXI
            else if (command == "GSTR2TXI")
            {
                objDeleteGSTR2.GSTR2Delete("TXI", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 TXPD
            else if (command == "GSTR2TXPD")
            {
                objDeleteGSTR2.GSTR2Delete("TXP", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 HSN
            else if (command == "GSTR2HSN")
            {
                objDeleteGSTR2.GSTR2Delete("HSN", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }


            //Deleting Invoices from GSTR2 B2BUR
            else if (command == "GSTR2B2BUR")
            {
                objDeleteGSTR2.GSTR2Delete("B2BUR", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 ITC
            else if (command == "GSTR2ITC")
            {
                objDeleteGSTR2.GSTR2Delete("ITCRVSL", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR2 CDNUR
            else if (command == "GSTR2CDNUR")
            {
                objDeleteGSTR2.GSTR2Delete("CDNUR", ids);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            #endregion

            #region "ONCHANGE EVENT - OTP REQUEST"
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                ViewBag.TitleHeaders = "GSTR-2 Save";

                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                ViewBag.OTPSession = OTPPOPUPValue;
                ViewBag.AUTH_Response = OTPAUTHResponse;
                ViewBag.AUTH_GSTINNo = strGSTINNo;
            }
            #endregion

            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.TitleHeaders = "GSTR-2 Save ";
            LoadSessionVariables();
            LoadGSTR2Data(strAction, strGSTINNo, strFp);

            return View();
        }

        public void LoadSessionVariables()
        {
            Session["G2B2BRefIds"] = string.Empty;
            Session["G2B2BURRefIds"] = string.Empty;
            Session["G2CDNRRefIds"] = string.Empty;
            Session["G2IMPGRefIds"] = string.Empty;
            Session["G2IMPSRefIds"] = string.Empty;
            Session["G2HSNRefIds"] = string.Empty;
            Session["G2NILRefIds"] = string.Empty;
            Session["G2ITCRVSLRefIds"] = string.Empty;
            Session["G2TXPDRefIds"] = string.Empty;
            Session["G2TXIRefIds"] = string.Empty;
            Session["G2CDNURRefIds"] = string.Empty;
        }

        public void LoadGSTR2Data(string strAction, string strGSTINNo, string strFp)
        {
            LoadSessionVariables();
            if (strAction == "B2B" || strAction == "ALL")
            {
                var B2B = Gstr2DataModel.GetB2B(strGSTINNo, strFp, "");
                ViewBag.B2B = B2B;
            }
            if (strAction == "IMPG" || strAction == "ALL")
            {
                var IMPG = Gstr2DataModel.GetIMPG(strGSTINNo, strFp, "");
                ViewBag.IMPG = IMPG;
            }
            if (strAction == "IMPS" || strAction == "ALL")
            {
                var IMPS = Gstr2DataModel.GetIMPS(strGSTINNo, strFp, "");
                ViewBag.IMPS = IMPS;
            }
            if (strAction == "CDN" || strAction == "ALL")
            {
                var CDN = Gstr2DataModel.GetCDN(strGSTINNo, strFp, "");
                ViewBag.CDN = CDN;
            }
            if (strAction == "NIL" || strAction == "ALL")
            {
                var NIL = Gstr2DataModel.GetNIL(strGSTINNo, strFp, "");
                ViewBag.NIL = NIL;
            }
            if (strAction == "TXLI" || strAction == "ALL")
            {
                var TXI = Gstr2DataModel.GetTXI(strGSTINNo, strFp, "");
                ViewBag.TXI = TXI;
            }
            if (strAction == "TXPD" || strAction == "ALL")
            {
                var TXPD = Gstr2DataModel.GetTXPD(strGSTINNo, strFp, "");
                ViewBag.TXPD = TXPD;
            }
            if (strAction == "HSNSUM" || strAction == "ALL")
            {
                var HSN = Gstr2DataModel.GetHSN(strGSTINNo, strFp, "");
                ViewBag.HSN = HSN;
            }
            if (strAction == "B2BUR" || strAction == "ALL")
            {
                var B2BUR = Gstr2DataModel.GetB2BUR(strGSTINNo, strFp, "");
                ViewBag.B2BUR = B2BUR;
            }
            if (strAction == "ITCRVSL" || strAction == "ALL")
            {
                var ITC = Gstr2DataModel.GetITC(strGSTINNo, strFp, "");
                ViewBag.ITC = ITC;
            }
            if (strAction == "CDNUR" || strAction == "ALL")
            {
                var CDNUR = Gstr2DataModel.GetCDNUR(strGSTINNo, strFp, "");
                ViewBag.CDNUR = CDNUR;
            }
        }
        #endregion

        #region "GSTR2 FILE"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult File(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR2 File Page.";
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
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = Gstr2DataModel.GetListGSTR2File(strGSTINNo, strAction, strFp);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ESign, string GSTR2File, string OTPSubmit)
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

            if (!string.IsNullOrEmpty(GSTR2File))
            {
                string strJsonData = Gstr2DataModel.GetJsonGSTR2File(strAction, strGSTINNo, strFp);

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
                    {
                        status = xmlNode.InnerText;
                    }
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

                var GridFill = Gstr2DataModel.GetListGSTR2File(strGSTINNo, strAction, strFp);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                return View(GridFill);
            }
            else if (!string.IsNullOrEmpty(ESign))
            {
                TempData["FileResponse"] = "GSTR2 Filed Successfully... Status - SUCCESS and Ack No. - ASDFSDF1241343";

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName");
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

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            else
            {
                ViewBag.TitleHeaders = "GSTR-2 File - [" + strAction + "]";
                var GridFill = Gstr2DataModel.GetListGSTR2File(strGSTINNo, strAction, strFp);

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());
                return View(GridFill);
            }
        }
        #endregion

        #region "RECONCILIATION"
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
                {
                    return RedirectToAction("Login", "Account");
                }

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                fromPeriod = DateTime.Now.ToString("MMyyyy");
                toPeriod = DateTime.Now.ToString("MMyyyy");
                ViewBag.TotalInvoicesTitle = "Matched Invoices";
                ViewBag.MismatchInvoicesTitle = "Mismatch Invoices in GSTR 2 and 2A";
                ViewBag.MissingInvoicesGSTR2ATitle = "Missing Invoices in GSTR-2A";
                ViewBag.MissingInvoicesGSTR2Title = "Missing Invoices in GSTR-2";

                if (Session["CF_GSTIN"] != null)
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, Session["CF_GSTIN"].ToString(), Session["Role_Name"].ToString());
                    strGStin = Session["CF_GSTIN"].ToString();
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

                }
                if (Session["CF_PERIOD"] != null)
                {
                    ViewBag.Period = Session["CF_PERIOD"].ToString();
                    fromPeriod = Session["CF_PERIOD"].ToString();
                }
                if (Session["CF_TOPERIOD"] != null)
                {
                    ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    toPeriod = Session["CF_TOPERIOD"].ToString();
                }
                if (Session["CF_FROMDATE"] != null)
                {
                    ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                    FromDate = Session["CF_FROMDATE"].ToString();
                }
                else
                {
                    ViewBag.FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                    FromDate = DateTime.Now.ToString("dd-MM-yyyy");
                }
                if (Session["CF_TODATE"] != null)
                {
                    ViewBag.ToDate = Session["CF_TODATE"].ToString();
                    ToDate = Session["CF_TODATE"].ToString();
                }
                else
                {
                    ViewBag.ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                    ToDate = DateTime.Now.ToString("dd-MM-yyyy");
                }
                
                if (Session["CF_SUPNAME"] != null && Session["CF_SUPNAME"].ToString() != "ALL")
                {
                    cName = Session["CF_SUPNAME"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME"].ToString());
                    if (Session["CF_CTIN"] != null)
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name");
                    if (Session["CF_CTIN"] != null)
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                ViewBag.MatchedInvoices = "0";
                ViewBag.MissinginGSTR2A = "0";
                ViewBag.MissinginGSTR2 = "0";
                ViewBag.MismatchInvoices = "0";
                ViewBag.AcceptedInvoices = "0";
                ViewBag.ModifiedInvoices = "0";
                ViewBag.RejectedInvoices = "0";
                ViewBag.PendingInvoices = "0";
                ViewBag.HoldInvoices = "0";

                ViewBag.MatchedTaxAmountGSTR2 = "0";
                ViewBag.MatchedTaxAmountGSTR2A = "0";
                ViewBag.MatchedTaxableValueGSTR2 = "0";
                ViewBag.MatchedTaxableValueGSTR2A = "0";

                ViewBag.MissingIngstr2aTaxAmountGSTR2 = "0";
                ViewBag.MissingIngstr2aTaxAmountGSTR2A = "0";
                ViewBag.MissingIngstr2aTaxableValueGSTR2 = "0";
                ViewBag.MissingIngstr2aTaxableValueGSTR2A = "0";

                ViewBag.MissingIngstr2TaxAmountGSTR2 = "0";
                ViewBag.MissingIngstr2TaxAmountGSTR2A = "0";
                ViewBag.MissingIngstr2TaxableValueGSTR2 = "0";
                ViewBag.MissingIngstr2TaxableValueGSTR2A = "0";

                ViewBag.MismatchTaxAmountGSTR2 = "0";
                ViewBag.MismatchTaxAmountGSTR2A = "0";
                ViewBag.MismatchTaxableValueGSTR2 = "0";
                ViewBag.MismatchTaxableValueGSTR2A = "0";

                ViewBag.HoldTaxAmountGSTR2 = "0";
                ViewBag.HoldTaxAmountGSTR2A = "0";
                ViewBag.HoldTaxableValueGSTR2 = "0";
                ViewBag.HoldTaxableValueGSTR2A = "0";

                ViewBag.RejectedHoldInvoices = "0";
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
        public ActionResult Reconcilation(FormCollection frm)
        {
            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["User_ID"] == null)
                {
                    return RedirectToAction("Login", "Account");
                }

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                string strGSTIN = "", strPeriod = "", strSupplierName = "", strCTIN = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
                string Reset = "", RawData = "", PanRawData = "", btnSupplierWise = "";
                DataSet GetCount;
                DataSet GetCount_Idt;

                Reset = frm["Reset"];
                RawData = frm["RawData"];
                PanRawData = frm["PanRawData"];
                btnSupplierWise = frm["supplierwise"];
                strGSTIN = frm["ddlGSTINNo"];
                strPeriod = frm["period"];
                strSupplierName = frm["ddlSupplierName"];
                strCTIN = frm["ddlSupGSTIN"];
                strtoperiod = frm["toperiod"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];
                //year = frm["years"];

                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
               // Session["YEAR"] = year;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;

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
                    TempData["ReconcilationResponse"] = "To Date should always be greater than or equal to from Date";
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
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGSTIN, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

                Session["CF_GSTIN"] = strGSTIN;
                Session["CF_SUPNAME"] = strSupplierName;
                Session["CF_CTIN"] = strCTIN;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;

                if (!string.IsNullOrEmpty(RawData))
                {

                    if (strRadio != "FinancialYear")
                    {
                        var das = ReconcilationBl.GetDataSetReconciliationLog(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);

                        using (DataSet ds = das)
                        {
                            if ((ds.Tables[0].Rows.Count > 200000) || (ds.Tables[1].Rows.Count > 200000) || (ds.Tables[2].Rows.Count > 200000) ||
                                (ds.Tables[3].Rows.Count > 200000) || (ds.Tables[4].Rows.Count > 200000) || (ds.Tables[5].Rows.Count > 200000) ||
                                (ds.Tables[6].Rows.Count > 200000) || (ds.Tables[7].Rows.Count > 200000) || (ds.Tables[8].Rows.Count > 200000) ||
                                (ds.Tables[9].Rows.Count > 200000) || (ds.Tables[10].Rows.Count > 200000)|| (ds.Tables[11].Rows.Count > 200000)||
                                (ds.Tables[12].Rows.Count > 200000)|| (ds.Tables[13].Rows.Count > 200000)|| (ds.Tables[14].Rows.Count > 200000)||
                                (ds.Tables[15].Rows.Count > 200000)|| (ds.Tables[16].Rows.Count > 200000)|| (ds.Tables[17].Rows.Count > 200000)||
                                (ds.Tables[18].Rows.Count > 200000)|| (ds.Tables[19].Rows.Count > 200000)|| (ds.Tables[20].Rows.Count > 200000))
                            {
                                if (ds.Tables[0].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[0].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables=  getSpliitedTables(ds.Tables[0],count);
                                    
                                    WirteToFile(splittedtables[0], "ActivityLogSummary_1");
                                    WirteToFile(splittedtables[1], "ActivityLogSummary_2");
                           
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[0], "ActivityLogSummary");
                                }

                                if (ds.Tables[1].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[1].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[1], count);

                                    WirteToFile(splittedtables[0], "All Invoices-B2B_1");
                                    WirteToFile(splittedtables[1], "All Invoices-B2B_2");
      
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[1], "All Invoices-B2B");
                                }

                                if (ds.Tables[2].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[2].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[2], count);
                                    WirteToFile(splittedtables[0], "All Invoices-CDNR_1");
                                    WirteToFile(splittedtables[1], "All Invoices-CDNR_2");

                                }
                                else
                                {
                                    WirteToFile(ds.Tables[2], "All Invoices-CDNR");
                                }

                                if (ds.Tables[3].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[3].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[3], count);
 
                                    WirteToFile(splittedtables[0], "MatchedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "MatchedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[3], "MatchedRecord-B2B");
                                }

                                if (ds.Tables[4].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[4].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[4], count);
                                    WirteToFile(splittedtables[0], "MatchedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "MatchedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[4], "MatchedRecord-CDNR");
                                }

                                if (ds.Tables[5].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[5].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[5], count);

                                    WirteToFile(splittedtables[0], "MissingGSTR2ARecords-B2B_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2ARecords-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[5], "MissingGSTR2ARecords-B2B");
                                }

                                if (ds.Tables[6].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[6].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[6], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2ARecords-CDNR_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2ARecords-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[6], "MissingGSTR2ARecords-CDNR");
                                }

                                if (ds.Tables[7].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[7].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[7], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2Records-B2B_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2Records-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[7], "MissingGSTR2Records-B2B");
                                }

                                if (ds.Tables[8].Rows.Count > 500000)
                                {
                                    int count = ds.Tables[8].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[8], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2Records-CDNR_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2Records-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[8], "MissingGSTR2Records-CDNR");
                                }

                                if (ds.Tables[9].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[9].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[9], count);
                                    WirteToFile(splittedtables[0], "MismatchedRecords-B2B_1");
                                    WirteToFile(splittedtables[1], "MismatchedRecords-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[9], "MismatchedRecords-B2B");
                                }

                                if (ds.Tables[10].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[10].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[10], count);
                                    WirteToFile(splittedtables[0], "MismatchedRecords-CDNR_1");
                                    WirteToFile(splittedtables[1], "MismatchedRecords-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[10], "MismatchedRecords-CDNR");
                                }

                                if (ds.Tables[11].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[11].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[11], count);
                                    WirteToFile(splittedtables[0], "AccpetedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "AccpetedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[11], "AccpetedRecord-B2B");
                                }

                                if (ds.Tables[12].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[12].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[12], count);
                                    WirteToFile(splittedtables[0], "AccpetedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "AccpetedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[12], "AccpetedRecord-CDNR");
                                }

                                if (ds.Tables[13].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[13].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[13], count);
                                    WirteToFile(splittedtables[0], "RejectedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "RejectedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[13], "RejectedRecord-B2B");
                                }

                                if (ds.Tables[14].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[14].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[14], count);
                                    WirteToFile(splittedtables[0], "RejectedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "RejectedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[14], "RejectedRecord-CDNR");
                                }

                                if (ds.Tables[15].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[15].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[15], count);
                                    WirteToFile(splittedtables[0], "HoldRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "HoldRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[15], "HoldRecord-B2B");
                                }

                                if (ds.Tables[16].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[16].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[16], count);
                                    WirteToFile(splittedtables[0], "HoldRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "HoldRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[16], "HoldRecord-CDNR");
                                }

                                if (ds.Tables[17].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[17].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[17], count);
                                    WirteToFile(splittedtables[0], "Rejected_HoldRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "Rejected_HoldRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[17], "Rejected_HoldRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[18].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[18], count);
                                    WirteToFile(splittedtables[0], "Rejected_HoldRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "Rejected_HoldRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[18], "Rejected_HoldRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[19].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[19], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[19], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[20].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[20].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[20], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[20], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[21].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[21].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[21], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-B2BA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-B2BA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[21], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[22].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[22].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[22], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-CDNRA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-CDNRA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[22], "Missing in GSTR2-CDNRA");
                                }

                                if (ds.Tables[23].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[23].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[23], count);
                                    WirteToFile(splittedtables[0], "Matched_GSTR2andAmd_B2B_1");
                                    WirteToFile(splittedtables[1], "Matched_GSTR2andAmd_B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[23], "Matched_GSTR2andAmd_B2B");
                                }

                                if (ds.Tables[24].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[24].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[24], count);
                                    WirteToFile(splittedtables[0], "Matched_GSTR2andAmd_CDNR_1");
                                    WirteToFile(splittedtables[1], "Matched_GSTR2andAmd_CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[24], "Matched_GSTR2andAmd_CDNR");
                                }

                                if (ds.Tables[25].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[25].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[25], count);
                                    WirteToFile(splittedtables[0], "MissinginGSTR2_AMD_B2B_1");
                                    WirteToFile(splittedtables[1], "MissinginGSTR2_AMD_B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[25], "MissinginGSTR2_AMD_B2B");
                                }

                                if (ds.Tables[26].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[26].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[26], count);
                                    WirteToFile(splittedtables[0], "MissinginGSTR2_AMD_CDNR_1");
                                    WirteToFile(splittedtables[1], "MissinginGSTR2_AMD_CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[26], "MissinginGSTR2_AMD_CDNR");
                                }

                                UploadToBlob uploadToBlob = new UploadToBlob("", strPeriod, "", UserId.ToString(), CustId.ToString());
                                string fileuir = null;
                                fileuir = uploadToBlob.UploadReconcilationDataToBlob(fileNameKey);
                                Response.Redirect(fileuir);
                            }

                            else
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
                                    ds.Tables[5].TableName = "MissingGSTR2ARecords-B2B";
                                }
                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    ds.Tables[6].TableName = "MissingGSTR2ARecords-CDNR";
                                }
                                if (ds.Tables[7].Rows.Count > 0)
                                {
                                    ds.Tables[7].TableName = "MissingGSTR2Records-B2B";
                                }
                                if (ds.Tables[8].Rows.Count > 0)
                                {
                                    ds.Tables[8].TableName = "MissingGSTR2Records-CDNR";
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
                                    ds.Tables[15].TableName = "HoldRecord-B2B";
                                }
                                if (ds.Tables[16].Rows.Count > 0)
                                {
                                    ds.Tables[16].TableName = "HoldRecord-CDNR";
                                }
                                if (ds.Tables[17].Rows.Count > 0)
                                {
                                    ds.Tables[17].TableName = "Rejected_HoldRecord-B2B";
                                }
                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    ds.Tables[18].TableName = "Rejected_HoldRecord-CDNR";
                                }
                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    ds.Tables[19].TableName = "ModifiedRecord-B2B";
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    ds.Tables[20].TableName = "ModifiedRecord-CDNR";
                                }
                                if (ds.Tables[21].Rows.Count > 0)
                                {
                                    ds.Tables[21].TableName = "Missing in GSTR2-B2BA";
                                }
                                if (ds.Tables[22].Rows.Count > 0)
                                {
                                    ds.Tables[22].TableName = "Missing in GSTR2-CDNRA";
                                }
                                if (ds.Tables[23].Rows.Count > 0)
                                {
                                    ds.Tables[23].TableName = "Matched_GSTR2andAmd_B2B";
                                }
                                if (ds.Tables[24].Rows.Count > 0)
                                {
                                    ds.Tables[24].TableName = "Matched_GSTR2andAmd_CDNR";
                                }
                                if (ds.Tables[25].Rows.Count > 0)
                                {
                                    ds.Tables[25].TableName = "MissinginGSTR2_AMD_B2B";
                                }
                                if (ds.Tables[26].Rows.Count > 0)
                                {
                                    ds.Tables[26].TableName = "MissinginGSTR2_AMD_CDNR";
                                }
                                CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationLog_For_" + strGSTIN + "_" + strPeriod + "_" + strtoperiod + ".xlsx");
                            }

                        }
                    }

                    else
                    {
                        var das = ReconcilationBl.GetDataSetReconciliationLog_IdtWise(CustId, UserId, strGSTIN, strSupplierName, strCTIN, fromdate, todate);

                        using (DataSet ds = das)
                        {
                            if ((ds.Tables[0].Rows.Count > 200000) || (ds.Tables[1].Rows.Count > 200000) || (ds.Tables[2].Rows.Count > 200000) ||
                               (ds.Tables[3].Rows.Count > 200000) || (ds.Tables[4].Rows.Count > 200000) || (ds.Tables[5].Rows.Count > 200000) ||
                               (ds.Tables[6].Rows.Count > 200000) || (ds.Tables[7].Rows.Count > 200000) || (ds.Tables[8].Rows.Count > 200000) ||
                               (ds.Tables[9].Rows.Count > 200000) || (ds.Tables[10].Rows.Count > 200000) || (ds.Tables[11].Rows.Count > 200000) ||
                               (ds.Tables[12].Rows.Count > 200000) || (ds.Tables[13].Rows.Count > 200000) || (ds.Tables[14].Rows.Count > 200000) ||
                               (ds.Tables[15].Rows.Count > 200000) || (ds.Tables[16].Rows.Count > 200000) || (ds.Tables[17].Rows.Count > 200000) ||
                               (ds.Tables[18].Rows.Count > 200000) || (ds.Tables[19].Rows.Count > 200000) || (ds.Tables[20].Rows.Count > 200000))
                            {
                                //Set Name of DataTables.
                                if (ds.Tables[0].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[0].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[0], count);

                                    WirteToFile(splittedtables[0], "ActivityLogSummary_1");
                                    WirteToFile(splittedtables[1], "ActivityLogSummary_2");

                                }
                                else
                                {
                                    WirteToFile(ds.Tables[0], "ActivityLogSummary");
                                }

                                if (ds.Tables[1].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[1].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[1], count);

                                    WirteToFile(splittedtables[0], "All Invoices-B2B_1");
                                    WirteToFile(splittedtables[1], "All Invoices-B2B_2");

                                }
                                else
                                {
                                    WirteToFile(ds.Tables[1], "All Invoices-B2B");
                                }

                                if (ds.Tables[2].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[2].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[2], count);
                                    WirteToFile(splittedtables[0], "All Invoices-CDNR_1");
                                    WirteToFile(splittedtables[1], "All Invoices-CDNR_2");

                                }
                                else
                                {
                                    WirteToFile(ds.Tables[2], "All Invoices-CDNR");
                                }

                                if (ds.Tables[3].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[3].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[3], count);

                                    WirteToFile(splittedtables[0], "MatchedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "MatchedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[3], "MatchedRecord-B2B");
                                }

                                if (ds.Tables[4].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[4].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[4], count);
                                    WirteToFile(splittedtables[0], "MatchedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "MatchedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[4], "MatchedRecord-CDNR");
                                }

                                if (ds.Tables[5].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[5].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[5], count);

                                    WirteToFile(splittedtables[0], "MissingGSTR2ARecords-B2B_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2ARecords-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[5], "MissingGSTR2ARecords-B2B");
                                }

                                if (ds.Tables[6].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[6].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[6], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2ARecords-CDNR_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2ARecords-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[6], "MissingGSTR2ARecords-CDNR");
                                }

                                if (ds.Tables[7].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[7].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[7], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2Records-B2B_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2Records-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[7], "MissingGSTR2Records-B2B");
                                }

                                if (ds.Tables[8].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[8].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[8], count);
                                    WirteToFile(splittedtables[0], "MissingGSTR2Records-CDNR_1");
                                    WirteToFile(splittedtables[1], "MissingGSTR2Records-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[8], "MissingGSTR2Records-CDNR");
                                }

                                if (ds.Tables[9].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[9].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[9], count);
                                    WirteToFile(splittedtables[0], "MismatchedRecords-B2B_1");
                                    WirteToFile(splittedtables[1], "MismatchedRecords-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[9], "MismatchedRecords-B2B");
                                }

                                if (ds.Tables[10].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[10].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[10], count);
                                    WirteToFile(splittedtables[0], "MismatchedRecords-CDNR_1");
                                    WirteToFile(splittedtables[1], "MismatchedRecords-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[10], "MismatchedRecords-CDNR");
                                }

                                if (ds.Tables[11].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[11].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[11], count);
                                    WirteToFile(splittedtables[0], "AccpetedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "AccpetedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[11], "AccpetedRecord-B2B");
                                }

                                if (ds.Tables[12].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[12].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[12], count);
                                    WirteToFile(splittedtables[0], "AccpetedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "AccpetedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[12], "AccpetedRecord-CDNR");
                                }

                                if (ds.Tables[13].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[13].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[13], count);
                                    WirteToFile(splittedtables[0], "RejectedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "RejectedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[13], "RejectedRecord-B2B");
                                }

                                if (ds.Tables[14].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[14].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[14], count);
                                    WirteToFile(splittedtables[0], "RejectedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "RejectedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[14], "RejectedRecord-CDNR");
                                }

                                if (ds.Tables[15].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[15].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[15], count);
                                    WirteToFile(splittedtables[0], "HoldRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "HoldRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[15], "HoldRecord-B2B");
                                }

                                if (ds.Tables[16].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[16].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[16], count);
                                    WirteToFile(splittedtables[0], "HoldRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "HoldRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[16], "HoldRecord-CDNR");
                                }

                                if (ds.Tables[17].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[17].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[17], count);
                                    WirteToFile(splittedtables[0], "Rejected_HoldRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "Rejected_HoldRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[17], "Rejected_HoldRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[18].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[18], count);
                                    WirteToFile(splittedtables[0], "Rejected_HoldRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "Rejected_HoldRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[18], "Rejected_HoldRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[19].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[19], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[19], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[20].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[20].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[20], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[20], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[21].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[21].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[21], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-B2BA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-B2BA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[21], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[22].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[22].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[22], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-CDNRA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-CDNRA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[22], "Missing in GSTR2-CDNRA");
                                }
                                if (ds.Tables[23].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[23].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[23], count);
                                    WirteToFile(splittedtables[0], "Matched_GSTR2andAmd_B2B_1");
                                    WirteToFile(splittedtables[1], "Matched_GSTR2andAmd_B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[23], "Matched_GSTR2andAmd_B2B");
                                }

                                if (ds.Tables[24].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[24].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[24], count);
                                    WirteToFile(splittedtables[0], "Matched_GSTR2andAmd_CDNR_1");
                                    WirteToFile(splittedtables[1], "Matched_GSTR2andAmd_CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[24], "Matched_GSTR2andAmd_CDNR");
                                }

                                if (ds.Tables[25].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[25].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[25], count);
                                    WirteToFile(splittedtables[0], "MissinginGSTR2_AMD_B2B_1");
                                    WirteToFile(splittedtables[1], "MissinginGSTR2_AMD_B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[25], "MissinginGSTR2_AMD_B2B");
                                }

                                if (ds.Tables[26].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[26].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[26], count);
                                    WirteToFile(splittedtables[0], "MissinginGSTR2_AMD_CDNR_1");
                                    WirteToFile(splittedtables[1], "MissinginGSTR2_AMD_CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[26], "MissinginGSTR2_AMD_CDNR");
                                }

                                UploadToBlob uploadToBlob = new UploadToBlob("", strPeriod, "", UserId.ToString(), CustId.ToString());
                                string fileuir = null;
                                fileuir = uploadToBlob.UploadReconcilationDataToBlob(fileNameKey);
                                Response.Redirect(fileuir);
                            }

                            else
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
                                    ds.Tables[5].TableName = "MissingGSTR2ARecords-B2B";
                                }
                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    ds.Tables[6].TableName = "MissingGSTR2ARecords-CDNR";
                                }
                                if (ds.Tables[7].Rows.Count > 0)
                                {
                                    ds.Tables[7].TableName = "MissingGSTR2Records-B2B";
                                }
                                if (ds.Tables[8].Rows.Count > 0)
                                {
                                    ds.Tables[8].TableName = "MissingGSTR2Records-CDNR";
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
                                    ds.Tables[15].TableName = "HoldRecord-B2B";
                                }
                                if (ds.Tables[16].Rows.Count > 0)
                                {
                                    ds.Tables[16].TableName = "HoldRecord-CDNR";
                                }
                                if (ds.Tables[17].Rows.Count > 0)
                                {
                                    ds.Tables[17].TableName = "Rejected_HoldRecord-B2B";
                                }
                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    ds.Tables[18].TableName = "Rejected_HoldRecord-CDNR";
                                }
                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    ds.Tables[19].TableName = "ModifiedRecord-B2B";
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    ds.Tables[20].TableName = "ModifiedRecord-CDNR";
                                }
                                if (ds.Tables[21].Rows.Count > 0)
                                {
                                    ds.Tables[21].TableName = "Missing in GSTR2-B2BA";
                                }
                                if (ds.Tables[22].Rows.Count > 0)
                                {
                                    ds.Tables[22].TableName = "Missing in GSTR2-CDNRA";
                                }
                                if (ds.Tables[23].Rows.Count > 0)
                                {
                                    ds.Tables[23].TableName = "Matched_GSTR2andAmd_B2B";
                                }
                                if (ds.Tables[24].Rows.Count > 0)
                                {
                                    ds.Tables[24].TableName = "Matched_GSTR2andAmd_CDNR";
                                }
                                if (ds.Tables[25].Rows.Count > 0)
                                {
                                    ds.Tables[25].TableName = "MissinginGSTR2_AMD_B2B";
                                }
                                if (ds.Tables[26].Rows.Count > 0)
                                {
                                    ds.Tables[26].TableName = "MissinginGSTR2_AMD_CDNR";
                                }
                                CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationLog_For_" + strGSTIN + "_" + fromdate + "_" + todate + ".xlsx");
                            }
                        }
                    }

                }

                if (!string.IsNullOrEmpty(PanRawData))
                {
                    if (strRadio != "FinancialYear")
                    {
                        var das = ReconcilationBl.GetDataSetReconciliationLog_PanWise(CustId, UserId, strSupplierName, strCTIN, strPeriod, strtoperiod);
                        using (DataSet ds = das)
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

                            CommonFunctions.ExportExcel_XLSX(ds, "PANBased_ReconciliationLog_For_" + strGSTIN + "_" + strPeriod + "_" + strtoperiod + ".xlsx");
                        }
                    }

                    else
                    {
                        var das = ReconcilationBl.GetDataSetReconciliationLog_PanWise_IdtWise(CustId, UserId, strSupplierName, strCTIN, fromdate, todate);
                        using (DataSet ds = das)
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

                            CommonFunctions.ExportExcel_XLSX(ds, "PANBased_ReconciliationLog_For_" + strGSTIN + "_" + fromdate + "_" + todate + ".xlsx");

                        }
                    }

                }
                else if (!string.IsNullOrEmpty(Reset))
                {
                    Task.Factory.StartNew(() => ReconcilationBl.ReconciliationReset(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod));

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Reset done successfully for the GSTIN : " + strGSTIN + " and FromPeriod : " + strPeriod + " and ToPeriod : " + strtoperiod + " and SupplierName : " + strSupplierName + " and SupplierGSTIN : " +strGSTIN, "");
                    TempData["ReconcilationResponse"] = "Reconciliation Reset is in progress. Please check after sometime";
                }

                else if (!string.IsNullOrEmpty(btnSupplierWise))
                {

                        if (strRadio != "FinancialYear")
                        {
                            var das = ReconcilationBl.GetDataSetReconciliationLog(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);

                            using (DataSet ds = das)
                            {

                                if (ds.Tables[5].Rows.Count > 0)
                                {

                                    WirteToFile(ds.Tables[5], "MissingGSTR2ARecords-B2B");
                                }

                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[6], "MissingGSTR2ARecords-CDNR");
                                }

                                if (ds.Tables[7].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[7], "MissingGSTR2Records-B2B");
                                }

                                if (ds.Tables[8].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[8], "MissingGSTR2Records-CDNR");
                                }

                                if (ds.Tables[9].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[9], "MismatchedRecords-B2B");
                                }

                                if (ds.Tables[10].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[10], "MismatchedRecords-CDNR");
                                }

                                if (ds.Tables[11].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[11], "AccpetedRecord-B2B");
                                }

                                if (ds.Tables[12].Rows.Count > 0)
                                {

                                    WirteToFile(ds.Tables[12], "AccpetedRecord-CDNR");
                                }

                                if (ds.Tables[13].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[13], "RejectedRecord-B2B");
                                }

                                if (ds.Tables[14].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[14], "RejectedRecord-CDNR");
                                }

                                if (ds.Tables[15].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[15], "HoldRecord-B2B");
                                }

                                if (ds.Tables[16].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[16], "HoldRecord-CDNR");
                                }

                                if (ds.Tables[17].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[17], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[18], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[19], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[20], "Missing in GSTR2-CDNRA");
                                }
                           
                            UploadToBlob uploadToBlob = new UploadToBlob("", strPeriod, "", UserId.ToString(), CustId.ToString());
                                string fileuir = null;
                                fileuir = uploadToBlob.UploadReconcilationDataToBlob(fileNameKey);

                                string url = this.Url.Content(fileuir);
                                //string link = HtmlHelper.GenerateLink(this.ControllerContext.RequestContext, System.Web.Routing.RouteTable.Routes, "fileuir", "Root", "About", "Home", null, null);
                                string gstin_login = string.Empty;
                                string strEmail = string.Empty;
                                string UserEmail = string.Empty;
                                string[] suppl_gstin;
                                string RowStatus = string.Empty;
                                string comp = Session["CompanyName"].ToString();
                                string mob = Session["MobileNo"].ToString();

                                UserEmail = (from lst in db.UserLists
                                             where lst.UserId == UserId && lst.CustId == CustId && lst.rowstatus == true
                                             select lst.Email).FirstOrDefault();


                                suppl_gstin = (from lst in db.TBL_Supplier
                                               where lst.SupplierName == strSupplierName && lst.CustomerId == CustId && lst.RowStatus == true
                                               select lst.GSTINno).ToArray();

                                gstin_login = (from lst in db.TBL_Customer
                                               where lst.CustId == CustId && lst.RowStatus == true
                                               select lst.GSTINNo).FirstOrDefault();


                                foreach (var gst in suppl_gstin)
                                {
                                    strEmail = (from lst in db.TBL_Supplier
                                                where lst.SupplierName == strSupplierName && lst.GSTINno == gst && lst.CustomerId == CustId && lst.RowStatus == true
                                                select lst.EmailId).FirstOrDefault();

                                    string[] emails;
                                    if (string.IsNullOrEmpty(strEmail))
                                    {
                                        TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                                    }
                                    else
                                    {
                                        emails = strEmail.Split(';');
                                        foreach (var email in emails)
                                        {
                                            Notification.SendEmail_Reconciliation(email, UserEmail, string.Format("Reconciliation Summary"), string.Format("We on behalf of " + comp + " having GSTIN: " + gstin_login + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them. Please find the Reconciliation Summary for the Supplier having GSTIN :" + gst + ".<br/><br/> Download the summary using the link below <br /> " + url));

                                            TempData["ReconcilationResponse"] = "Reconciliation Summary triggered to selected supplier successfully.";

                                        }
                                    }
                                }
                                //Response.Redirect("Reconcilation");
                               
                            }
                        }

                        else
                        {
                            var das = ReconcilationBl.GetDataSetReconciliationLog_IdtWise(CustId, UserId, strGSTIN, strSupplierName, strCTIN, fromdate, todate);

                            using (DataSet ds = das)
                            {

                                //Set Name of DataTables.

                                if (ds.Tables[5].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[5], "MissingGSTR2ARecords-B2B");
                                }

                                if (ds.Tables[6].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[6], "MissingGSTR2ARecords-CDNR");
                                }

                                if (ds.Tables[7].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[7], "MissingGSTR2Records-B2B");
                                }

                                if (ds.Tables[8].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[8], "MissingGSTR2Records-CDNR");
                                }

                                if (ds.Tables[9].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[9], "MismatchedRecords-B2B");
                                }

                                if (ds.Tables[10].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[10], "MismatchedRecords-CDNR");
                                }

                                if (ds.Tables[11].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[11], "AccpetedRecord-B2B");
                                }

                                if (ds.Tables[12].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[12], "AccpetedRecord-CDNR");
                                }

                                if (ds.Tables[13].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[13], "RejectedRecord-B2B");
                                }

                                if (ds.Tables[14].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[14], "RejectedRecord-CDNR");
                                }

                                if (ds.Tables[15].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[15], "HoldRecord-B2B");
                                }

                                if (ds.Tables[16].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[16], "HoldRecord-CDNR");
                                }

                                if (ds.Tables[17].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[17], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[18], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[19], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    WirteToFile(ds.Tables[20], "Missing in GSTR2-CDNRA");
                                }
                                UploadToBlob uploadToBlob = new UploadToBlob("", strPeriod, "", UserId.ToString(), CustId.ToString());
                                string fileuir = null;
                                fileuir = uploadToBlob.UploadReconcilationDataToBlob(fileNameKey);
                                string url = this.Url.Content(fileuir);

                                string gstin_login = string.Empty;
                                string strEmail = string.Empty;
                                string UserEmail = string.Empty;
                                string[] suppl_gstin;
                                string RowStatus = string.Empty;
                                string comp = Session["CompanyName"].ToString();
                                string mob = Session["MobileNo"].ToString();

                                UserEmail = (from lst in db.UserLists
                                             where lst.UserId == UserId && lst.CustId == CustId && lst.rowstatus == true
                                             select lst.Email).FirstOrDefault();


                                suppl_gstin = (from lst in db.TBL_Supplier
                                               where lst.SupplierName == strSupplierName && lst.CustomerId == CustId && lst.RowStatus == true
                                               select lst.GSTINno).ToArray();

                                gstin_login = (from lst in db.TBL_Customer
                                               where lst.CustId == CustId && lst.RowStatus == true
                                               select lst.GSTINNo).FirstOrDefault();

                                foreach (var gst in suppl_gstin)
                                {
                                    strEmail = (from lst in db.TBL_Supplier
                                                where lst.SupplierName == strSupplierName && lst.GSTINno == gst && lst.CustomerId == CustId && lst.RowStatus == true
                                                select lst.EmailId).FirstOrDefault();

                                    string[] emails;
                                    if (string.IsNullOrEmpty(strEmail))
                                    {
                                        TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                                    }
                                    else
                                    {
                                        emails = strEmail.Split(';');
                                        foreach (var email in emails)
                                        {
                                        Notification.SendEmail_Reconciliation(email, UserEmail, string.Format("Reconciliation Summary"), string.Format("We on behalf of " + comp + " having GSTIN: " + gstin_login + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them. Please find the Reconciliation Summary for the Supplier having GSTIN :" + gst + ".<br/><br/> Download the summary using the link below <br /> " + url));

                                        TempData["ReconcilationResponse"] = "Reconciliation Summary triggered to selected supplier successfully.";
                                        }
                                    }
                                }
                            }
                        }
                  
                }
                else
                {
                    //
                }
                if (strRadio != "FinancialYear")
                {
                    GetCount = new ReconcilationDataModel(CustId, UserId).GetReconcileCount(strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);
                    if (GetCount.Tables.Count > 0)
                    {
                        if (GetCount.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.MatchedInvoices = GetCount.Tables[0].Rows[0]["TotalCount"];
                            ViewBag.MissinginGSTR2A = GetCount.Tables[0].Rows[1]["TotalCount"];
                            ViewBag.MissinginGSTR2 = GetCount.Tables[0].Rows[2]["TotalCount"];
                            ViewBag.MismatchInvoices = GetCount.Tables[0].Rows[3]["TotalCount"];
                            ViewBag.AcceptedInvoices = GetCount.Tables[0].Rows[4]["TotalCount"];
                            ViewBag.RejectedInvoices = GetCount.Tables[0].Rows[5]["TotalCount"];
                            ViewBag.PendingInvoices = GetCount.Tables[0].Rows[6]["TotalCount"];
                            ViewBag.ModifiedInvoices = GetCount.Tables[0].Rows[7]["TotalCount"];
                            ViewBag.HoldInvoices = GetCount.Tables[0].Rows[8]["TotalCount"];
                            ViewBag.RejectedHoldInvoices = GetCount.Tables[0].Rows[9]["TotalCount"];

                            ViewBag.MatchedTaxAmountGstr2 = GetCount.Tables[1].Rows[0]["TotalCount"];
                            ViewBag.MatchedTaxAmountGstr2A = GetCount.Tables[1].Rows[1]["TotalCount"];
                            ViewBag.MatchedTaxableValueGstr2 = GetCount.Tables[1].Rows[2]["TotalCount"];
                            ViewBag.MatchedTaxableValueGstr2A = GetCount.Tables[1].Rows[3]["TotalCount"];

                            ViewBag.MissingInGstr2ATaxAmountGstr2 = GetCount.Tables[1].Rows[4]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxAmountGstr2A = GetCount.Tables[1].Rows[5]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxableValueGstr2 = GetCount.Tables[1].Rows[6]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxableValueGstr2A = GetCount.Tables[1].Rows[7]["TotalCount"];

                            ViewBag.MissingInGstr2TaxAmountGstr2 = GetCount.Tables[1].Rows[8]["TotalCount"];
                            ViewBag.MissingInGstr2TaxAmountGstr2A = GetCount.Tables[1].Rows[9]["TotalCount"];
                            ViewBag.MissingInGstr2TaxableValueGstr2 = GetCount.Tables[1].Rows[10]["TotalCount"];
                            ViewBag.MissingInGstr2TaxableValueGstr2A = GetCount.Tables[1].Rows[11]["TotalCount"];

                            ViewBag.MismatchTaxAmountGstr2 = GetCount.Tables[1].Rows[12]["TotalCount"];
                            ViewBag.MismatchTaxAmountGstr2A = GetCount.Tables[1].Rows[13]["TotalCount"];
                            ViewBag.MismatchTaxableValueGstr2 = GetCount.Tables[1].Rows[14]["TotalCount"];
                            ViewBag.MismatchTaxableValueGstr2A = GetCount.Tables[1].Rows[15]["TotalCount"];

                            ViewBag.HoldTaxAmountGSTR2 = GetCount.Tables[1].Rows[16]["TotalCount"];
                            ViewBag.HoldTaxAmountGSTR2A = GetCount.Tables[1].Rows[17]["TotalCount"];
                            ViewBag.HoldTaxableValueGSTR2 = GetCount.Tables[1].Rows[18]["TotalCount"];
                            ViewBag.HoldTaxableValueGSTR2A = GetCount.Tables[1].Rows[19]["TotalCount"];

                            
                        }
                        else
                        {
                            ViewBag.MatchedInvoices = "0";
                            ViewBag.MissinginGSTR2A = "0";
                            ViewBag.MissinginGSTR2 = "0";
                            ViewBag.MismatchInvoices = "0";
                            ViewBag.AcceptedInvoices = "0";
                            ViewBag.ModifiedInvoices = "0";
                            ViewBag.RejectedInvoices = "0";
                            ViewBag.PendingInvoices = "0";
                            ViewBag.HoldInvoices = "0";

                            ViewBag.MatchedTaxAmountGstr2 = "0";
                            ViewBag.MatchedTaxAmountGstr2A = "0";
                            ViewBag.MatchedTaxableValueGstr2 = "0";
                            ViewBag.MatchedTaxableValueGstr2A = "0";

                            ViewBag.MissingInGstr2ATaxAmountGstr2 = "0";
                            ViewBag.MissingInGstr2ATaxAmountGstr2A = "0";
                            ViewBag.MissingInGstr2ATaxableValueGstr2 = "0";
                            ViewBag.MissingInGstr2ATaxableValueGstr2A = "0";

                            ViewBag.MissingInGstr2TaxAmountGstr2 = "0";
                            ViewBag.MissingInGstr2TaxAmountGstr2A = "0";
                            ViewBag.MissingInGstr2TaxableValueGstr2 = "0";
                            ViewBag.MissingInGstr2TaxableValueGstr2A = "0";

                            ViewBag.MismatchTaxAmountGstr2 = "0";
                            ViewBag.MismatchTaxAmountGstr2A = "0";
                            ViewBag.MismatchTaxableValueGstr2 = "0";
                            ViewBag.MismatchTaxableValueGstr2A = "0";

                            ViewBag.HoldTaxAmountGstr2 = "0";
                            ViewBag.HoldTaxAmountGstr2A = "0";
                            ViewBag.HoldTaxableValueGstr2 = "0";
                            ViewBag.HoldTaxableValueGstr2A = "0";

                            ViewBag.RejectedHoldInvoices = "0";
                        }
                    }
                       // GetReconcileCount(CustId, UserId, strGSTIN, strSupplierName, strCTIN, strPeriod, strtoperiod);
                    Session["f_year"] = "FinancialPeriod";
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                    ViewBag.ToDate = Session["CF_TODATE"].ToString();
                }
                else if (strRadio != "FinancialPeriod")
                {

                    GetCount_Idt = new ReconcilationDataModel(CustId, UserId).GetReconcileCount_Idt(strGSTIN, strSupplierName, strCTIN, fromdate, todate);
                    if (GetCount_Idt.Tables.Count > 0)
                    {
                        if (GetCount_Idt.Tables[0].Rows.Count > 0)
                        {
                            ViewBag.MatchedInvoices = GetCount_Idt.Tables[0].Rows[0]["TotalCount"];
                            ViewBag.MissinginGSTR2A = GetCount_Idt.Tables[0].Rows[1]["TotalCount"];
                            ViewBag.MissinginGSTR2 = GetCount_Idt.Tables[0].Rows[2]["TotalCount"];
                            ViewBag.MismatchInvoices = GetCount_Idt.Tables[0].Rows[3]["TotalCount"];
                            ViewBag.AcceptedInvoices = GetCount_Idt.Tables[0].Rows[4]["TotalCount"];
                            ViewBag.RejectedInvoices = GetCount_Idt.Tables[0].Rows[5]["TotalCount"];
                            ViewBag.PendingInvoices = GetCount_Idt.Tables[0].Rows[6]["TotalCount"];
                            ViewBag.ModifiedInvoices = GetCount_Idt.Tables[0].Rows[7]["TotalCount"];
                            ViewBag.HoldInvoices = GetCount_Idt.Tables[0].Rows[8]["TotalCount"];
                            ViewBag.RejectedHoldInvoices = GetCount_Idt.Tables[0].Rows[9]["TotalCount"];

                            ViewBag.MatchedTaxAmountGstr2 = GetCount_Idt.Tables[1].Rows[0]["TotalCount"];
                            ViewBag.MatchedTaxAmountGstr2A = GetCount_Idt.Tables[1].Rows[1]["TotalCount"];
                            ViewBag.MatchedTaxableValueGstr2 = GetCount_Idt.Tables[1].Rows[2]["TotalCount"];
                            ViewBag.MatchedTaxableValueGstr2A = GetCount_Idt.Tables[1].Rows[3]["TotalCount"];

                            ViewBag.MissingInGstr2ATaxAmountGstr2 = GetCount_Idt.Tables[1].Rows[4]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxAmountGstr2A = GetCount_Idt.Tables[1].Rows[5]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxableValueGstr2 = GetCount_Idt.Tables[1].Rows[6]["TotalCount"];
                            ViewBag.MissingInGstr2ATaxableValueGstr2A = GetCount_Idt.Tables[1].Rows[7]["TotalCount"];

                            ViewBag.MissingInGstr2TaxAmountGstr2 = GetCount_Idt.Tables[1].Rows[8]["TotalCount"];
                            ViewBag.MissingInGstr2TaxAmountGstr2A = GetCount_Idt.Tables[1].Rows[9]["TotalCount"];
                            ViewBag.MissingInGstr2TaxableValueGstr2 = GetCount_Idt.Tables[1].Rows[10]["TotalCount"];
                            ViewBag.MissingInGstr2TaxableValueGstr2A = GetCount_Idt.Tables[1].Rows[11]["TotalCount"];

                            ViewBag.MismatchTaxAmountGstr2 = GetCount_Idt.Tables[1].Rows[12]["TotalCount"];
                            ViewBag.MismatchTaxAmountGstr2A = GetCount_Idt.Tables[1].Rows[13]["TotalCount"];
                            ViewBag.MismatchTaxableValueGstr2 = GetCount_Idt.Tables[1].Rows[14]["TotalCount"];
                            ViewBag.MismatchTaxableValueGstr2A = GetCount_Idt.Tables[1].Rows[15]["TotalCount"];

                            ViewBag.HoldTaxAmountGSTR2 = GetCount_Idt.Tables[1].Rows[16]["TotalCount"];
                            ViewBag.HoldTaxAmountGSTR2A = GetCount_Idt.Tables[1].Rows[17]["TotalCount"];
                            ViewBag.HoldTaxableValueGSTR2 = GetCount_Idt.Tables[1].Rows[18]["TotalCount"];
                            ViewBag.HoldTaxableValueGSTR2A = GetCount_Idt.Tables[1].Rows[19]["TotalCount"];

                            
                        }
                        else
                        {
                            ViewBag.MatchedInvoices = "0";
                            ViewBag.MissinginGSTR2A = "0";
                            ViewBag.MissinginGSTR2 = "0";
                            ViewBag.MismatchInvoices = "0";
                            ViewBag.AcceptedInvoices = "0";
                            ViewBag.ModifiedInvoices = "0";
                            ViewBag.RejectedInvoices = "0";
                            ViewBag.PendingInvoices = "0";
                            ViewBag.HoldInvoices = "0";

                            ViewBag.MatchedTaxAmountGstr2 = "0";
                            ViewBag.MatchedTaxAmountGstr2A = "0";
                            ViewBag.MatchedTaxableValueGstr2 = "0";
                            ViewBag.MatchedTaxableValueGstr2A = "0";

                            ViewBag.MissingInGstr2ATaxAmountGstr2 = "0";
                            ViewBag.MissingInGstr2ATaxAmountGstr2A = "0";
                            ViewBag.MissingInGstr2ATaxableValueGstr2 = "0";
                            ViewBag.MissingInGstr2ATaxableValueGstr2A = "0";

                            ViewBag.MissingInGstr2TaxAmountGstr2 = "0";
                            ViewBag.MissingInGstr2TaxAmountGstr2A = "0";
                            ViewBag.MissingInGstr2TaxableValueGstr2 = "0";
                            ViewBag.MissingInGstr2TaxableValueGstr2A = "0";

                            ViewBag.MismatchTaxAmountGstr2 = "0";
                            ViewBag.MismatchTaxAmountGstr2A = "0";
                            ViewBag.MismatchTaxableValueGstr2 = "0";
                            ViewBag.MismatchTaxableValueGstr2A = "0";

                            ViewBag.HoldTaxAmountGSTR2 = "0";
                            ViewBag.HoldTaxAmountGSTR2A = "0";
                            ViewBag.HoldTaxableValueGSTR2 = "0";
                            ViewBag.HoldTaxableValueGSTR2A = "0";
                            ViewBag.RejectedHoldInvoices = "0";
                        }
                    }

                    Session["f_year"] = "FinancialYear";
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                    ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                    ViewBag.ToDate = Session["CF_TODATE"].ToString();
                }

                else
                {

                }
                    

                var jsonresult = Json(new { success = true, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"],
                    MatchedInvoices = ViewBag.MatchedInvoices, MissinginGSTR2A = ViewBag.MissinginGSTR2A, MissinginGSTR2 = ViewBag.MissinginGSTR2,
                    MismatchInvoices = ViewBag.MismatchInvoices, AcceptedInvoices = ViewBag.AcceptedInvoices, RejectedInvoices = ViewBag.RejectedInvoices,
                    PendingInvoices = ViewBag.PendingInvoices, ModifiedInvoices = ViewBag.ModifiedInvoices,
                    HoldInvoices = ViewBag.HoldInvoices,
                    MatchedTaxAmountGstr2 = ViewBag.MatchedTaxAmountGstr2,
                    MatchedTaxAmountGstr2A = ViewBag.MatchedTaxAmountGstr2A, MatchedTaxableValueGstr2 = ViewBag.MatchedTaxableValueGstr2,
                    MatchedTaxableValueGstr2A = ViewBag.MatchedTaxableValueGstr2A, MissingInGstr2ATaxAmountGstr2 = ViewBag.MissingInGstr2ATaxAmountGstr2,
                    MissingInGstr2ATaxAmountGstr2A = ViewBag.MissingInGstr2ATaxAmountGstr2A, MissingInGstr2ATaxableValueGstr2 = ViewBag.MissingInGstr2ATaxableValueGstr2,
                    MissingInGstr2ATaxableValueGstr2A = ViewBag.MissingInGstr2ATaxableValueGstr2A, MissingInGstr2TaxAmountGstr2 = ViewBag.MissingInGstr2TaxAmountGstr2,
                    MissingInGstr2TaxAmountGstr2A = ViewBag.MissingInGstr2TaxAmountGstr2A, MissingInGstr2TaxableValueGstr2 = ViewBag.MissingInGstr2TaxableValueGstr2,
                    MissingInGstr2TaxableValueGstr2A = ViewBag.MissingInGstr2TaxableValueGstr2A, MismatchTaxAmountGstr2 = ViewBag.MismatchTaxAmountGstr2,
                    MismatchTaxAmountGstr2A = ViewBag.MismatchTaxAmountGstr2A, MismatchTaxableValueGstr2 = ViewBag.MismatchTaxableValueGstr2,
                    MismatchTaxableValueGstr2A = ViewBag.MismatchTaxableValueGstr2A, HoldTaxAmountGSTR2 = ViewBag.HoldTaxAmountGSTR2,
                    HoldTaxAmountGSTR2A = ViewBag.HoldTaxAmountGSTR2A, HoldTaxableValueGSTR2 = ViewBag.HoldTaxableValueGSTR2,HoldTaxableValueGSTR2A = ViewBag.HoldTaxableValueGSTR2A,
                    RejectedHoldInvoices = ViewBag.RejectedHoldInvoices
                }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            { 
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }


        public int FinancialYearDate(string year, string from, string to)
        {
            int outputparam;
            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Validate_Financial_Year_Dates", con);
                dCmd.Parameters.Clear();
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@FY ", year));
                dCmd.Parameters.Add(new SqlParameter("@fromdate ", from));
                dCmd.Parameters.Add(new SqlParameter("@todate ", to));
                dCmd.Parameters.Add("@retval", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                dCmd.ExecuteNonQuery();
                con.Close();

                outputparam = Convert.ToInt32(dCmd.Parameters["@retval"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        #endregion

        public void WirteToFile(DataTable dt, string datatableName)
        {
            string fileName = "/"+datatableName + ".csv";
            string folderName = string.Format("Recon{0}", fileNameKey);
            string path5 = Path.Combine(Server.MapPath("~/App_Data/uploads/"), folderName);
           
            if (System.IO.File.Exists(path5))
            {
                System.IO.File.Delete(path5);
               
            }
            System.IO.Directory.CreateDirectory(path5);
            //string path2 = Path.Combine(Server.MapPath(path5), fileName);
            string path2 = string.Concat(path5, fileName);
            System.IO.File.Delete(path2);
            using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
            // change buffer size and Encoding to your needs
            {
                List<string> headerValues = new List<string>();
                foreach (DataColumn dr in dt.Columns)
                {
                    headerValues.Add(dr.ToString());
                }
                swr.WriteLine(string.Join(",", headerValues.ToArray()));
                foreach (DataRow dr in dt.Rows)
                {
                    swr.WriteLine(string.Join(",", dr.ItemArray).Replace("\r", ""));
                }
            }
        }

        public DataTable[] getSpliitedTables (DataTable ds, int count)
        {
            DataTable[] splittedtables = ds.AsEnumerable()
                                   .Select((row, index) => new { row, index })
                                   .GroupBy(x => x.index / (count / 2))  // integer division, the fractional part is truncated
                                   .Select(g => g.Select(x => x.row).CopyToDataTable())
                                   .ToArray();

            return splittedtables;
        }

        public DataTable GetSupplierGSTINS(int CustId, string strSupplierName, string rowstatus)
        {
            DataTable ds = new DataTable();
            string suppl_gstin = string.Empty;
            try
            {
                suppl_gstin = (from lst in db.TBL_Supplier
                               where lst.SupplierName == strSupplierName && lst.CustomerId == CustId && lst.RowStatus == true
                               select lst.GSTINno).FirstOrDefault();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }

    }
}