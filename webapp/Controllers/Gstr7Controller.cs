using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR7;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr7Controller : Controller
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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR7 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
            ViewBag.ActionList = Actionlst;

            LoadSessionVariables();

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm, string GSTR7Save, string GetJson, string OTPSubmit, string command, string[] RefIds)
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
            strInvIds = frm["id"];
            ViewBag.Period = strFp;
            ViewBag.ActionType = strAction;

            DeleteGstr7Sa objDeleteGSTR7 = new DeleteGstr7Sa();
            #region "GSTR7 SAVE"
            if (!string.IsNullOrEmpty(GSTR7Save))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr7SaveDataModel fnGSTR7Save = new GspSendGstr7SaveDataModel();
                    string strJsonData = Gstr7DataModel.GetJsonGSTR7Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    string SaveResponse = fnGSTR7Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    TempData["SaveResponse"] = SaveResponse;

                    SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
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
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr7SaveDataModel fnGSTR7Save = new GspSendGstr7SaveDataModel();
                    string strJsonData = Gstr7DataModel.GetJsonGSTR7Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                    ViewBag.GET_GSTR7JsonResponse = strJsonData;
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

            #region "GSTR7 DELETE"
            //Deleting Invoices from GSTR7 TDS
            else if (RefIds != null)
            {
                if (command == "GSTR7TDS")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR7.GSTR7Delete("TDS", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
                //Deleting Invoices from GSTR7 TDSA
                else if (command == "GSTR7TDSA")
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    foreach (var id in invid)
                    {
                        string ids = id;
                        objDeleteGSTR7.GSTR7Delete("TDSA", ids);
                        TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                    }
                }
           
            }


            

            #endregion

            #region "ONCHANGE EVENT - OTP REQUEST"
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());

                ViewBag.TitleHeaders = "GSTR-7 Save";

                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                ViewBag.OTPSession = OTPPOPUPValue;
                ViewBag.AUTH_Response = OTPAUTHResponse;
                ViewBag.AUTH_GSTINNo = strGSTINNo;
            }
            #endregion

            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.TitleHeaders = "GSTR-7 Save ";
            LoadSessionVariables();
            LoadGSTR7Data(strAction, strGSTINNo, strFp);
            return View();
        }

        public void LoadSessionVariables()
        {

            Session["G7TDSRefIds"] = string.Empty;
            Session["G7TDSARefIds"] = string.Empty;
          

            Session["dataCount_G7TDS"] = 0;
            Session["dataCount_G7TDSA"] = 0;
          
        }

        public void LoadGSTR7Data(string strAction, string strGSTINNo, string strFp)
       {
            LoadSessionVariables();
            if (strAction == "TDS" || strAction == "ALL")
            {
                var TDS = Gstr7DataModel.GetTDS7(strGSTINNo, strFp, "");
                ViewBag.TDS = TDS;
            }
            if (strAction == "TDSA" || strAction == "ALL")
            {
                var TDSA = Gstr7DataModel.GetTDSA7(strGSTINNo, strFp, "");
                ViewBag.TDSA = TDSA;
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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR7 File Page.";
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
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = Gstr7DataModel.GetListGSTR7File(strGSTINNo, strAction, strFp);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ESign, string GSTR7File, string OTPSubmit)
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

            if (!string.IsNullOrEmpty(GSTR7File))
            {
                string strJsonData = Gstr7DataModel.GetJsonGSTR7File(strAction, strGSTINNo, strFp);

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

                var GridFill = Gstr7DataModel.GetListGSTR7File(strGSTINNo, strAction, strFp);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                return View(GridFill);
            }
            else if (!string.IsNullOrEmpty(ESign))
            {

                TempData["FileResponse"] = "GSTR7 Filed Successfully... Status - SUCCESS and Ack No. - ASDFSDF1241343";

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName");
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

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            else
            {
                ViewBag.TitleHeaders = "GSTR-7 File - [" + strAction + "]";
                var GridFill = Gstr7DataModel.GetListGSTR7File(strGSTINNo, strAction, strFp);

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR7"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());
                return View(GridFill);
            }
        }

     


    }
}