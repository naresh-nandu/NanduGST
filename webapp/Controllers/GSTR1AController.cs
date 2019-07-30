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
using SmartAdminMvc.Models.GSTR1A;
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
using SmartAdminMvc.Models.Common;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr1AController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Save(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            if (sort != null || page != 0)
            {
                string strActionId = TempData["ActionId"] as string;
                string strGSTINId = TempData["GSTINId"] as string;
                string strAction = TempData["ActionType"] as string;
                string strGSTINNo = TempData["GSTINNo"] as string;
                TempData.Keep();
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = GSTR1ADataModel.GetListGSTR1ASave(strGSTINNo, strAction);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm, string GSTR1ASave, string OTPSubmit)
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

            if (!string.IsNullOrEmpty(GSTR1ASave))
            {
                TempData["SaveResponse"] = "GSTR1A Saved Successfully... Trans Id - TXOID0000000011 and Ref Id - LAPN24235325555";

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
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

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                ViewBag.TitleHeaders = "GSTR-1A Save - [" + strAction + "]";
                var GridFill = GSTR1ADataModel.GetListGSTR1ASave(strGSTINNo, strAction);
                return View(GridFill);
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
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = GSTR1ADataModel.GetListGSTR1AFile(strGSTINNo, strAction, strFp);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ESign, string GSTR1AFile, string OTPSubmit)
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

            if (!string.IsNullOrEmpty(GSTR1AFile))
            {
                string strJsonData = GSTR1ADataModel.GetJsonGSTR1ASave(strAction, strGSTINNo, strFp);

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

                var GridFill = GSTR1ADataModel.GetListGSTR1AFile(strGSTINNo, strAction, strFp);
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                return View(GridFill);
            }
            else if (!string.IsNullOrEmpty(ESign))
            {
                TempData["FileResponse"] = "GSTR1A Filed Successfully... Status - SUCCESS and Ack No. - ASDFSDF1241343";

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
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

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
                return View();
            }
            else
            {
                ViewBag.TitleHeaders = "GSTR-1A File - [" + strAction + "]";
                var GridFill = GSTR1ADataModel.GetListGSTR1AFile(strGSTINNo, strAction, strFp);

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1A"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());
                return View(GridFill);
            }
        }
    }
}