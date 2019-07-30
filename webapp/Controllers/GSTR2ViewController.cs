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

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    //[System.Web.Mvc.Authorize(Roles="Active")]

    public class GSTR2ViewController : Controller
    {
        public WePGSPDBEntities db = new WePGSPDBEntities();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

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

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            if (sort != null || page != 0)
            {
                //string strActionId = TempData["ActionId"] as string;
                //string strGSTINId = TempData["GSTINId"] as string;
                string strAction = TempData["ActionType"] as string;
                string strGSTINNo = TempData["GSTINNo"] as string;
                string strFp = TempData["period"] as string;
                ViewBag.Period = strFp;
                ViewBag.ActionType = strAction;
                TempData.Keep();

                ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst;
                Display_RetStatus();
                LoadGSTR1ViewData(strAction, strGSTINNo, strFp);

                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                Display_RetStatus();
                return View();
            }
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string GSTR2Delete, string GSTR2ReUpload, string OTPSubmit, int[] ids, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            //int strGSTINId = Convert.ToInt32(frm["ddlGSTINNo"]);
            string strGSTINNo = frm["ddlGSTINNo"];
            string strAction = frm["ddlActionType"];
            //string strGSTINNo = (from lst in db.TBL_Cust_GSTIN
            //                     where lst.GSTINId == strGSTINId
            //                     select lst.GSTINNo).SingleOrDefault();

            string strFp = frm["period"];

            //TempData["ActionId"] = Convert.ToString(strActionId);
            //TempData["GSTINId"] = Convert.ToString(strGSTINId);
            TempData["ActionType"] = strAction;
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            ViewBag.Period = strFp;
            ViewBag.ActionType = strAction;

            Display_RetStatus();

            Delete_GSTR1_SA objDeleteGSTR1 = new Delete_GSTR1_SA();

            if (!string.IsNullOrEmpty(GSTR2Delete))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GSP_SendGSTR1ViewDataModel GSP_fnGSTR1View = new GSP_SendGSTR1ViewDataModel();
                    //SendGSTR1ViewDataModel GSP_fnGSTR1View = new SendGSTR1ViewDataModel();
                    string strJsonData = GSTR1ViewDataModel.GetJsonGSTR1_View(strGSTINNo, strFp, strAction, "D");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    //string ViewResponse = GSP_fnGSTR1View.SendRequest(strJsonData, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    //TempData["SaveResponse"] = "GSTR1 Saved Successfully... Trans Id - TXOID0000000011 and Ref Id - LAPN24235325555";
                    TempData["ViewResponse"] = "";// ViewResponse;

                    //SelectList GSTINNolst = new SelectList(db.TBL_Cust_GSTIN.Where(o => o.CustId == iCustId), "GSTINId", "GSTINNo");
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst1;
                    return View();
                }
            }
            else if (!string.IsNullOrEmpty(GSTR2ReUpload))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GSP_SendGSTR1SaveDataModel GSP_fnGSTR1Save = new GSP_SendGSTR1SaveDataModel();
                    //SendGSTR1ViewDataModel GSP_fnGSTR1View = new SendGSTR1ViewDataModel();
                    string strJsonData = GSTR1DataModel.GetJsonGSTR1Save(strGSTINNo, strFp, strAction, "1");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    //string ViewResponse = GSP_fnGSTR1Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    //TempData["SaveResponse"] = "GSTR1 Saved Successfully... Trans Id - TXOID0000000011 and Ref Id - LAPN24235325555";
                    TempData["ViewResponse"] = ""; // ViewResponse;

                    //SelectList GSTINNolst = new SelectList(db.TBL_Cust_GSTIN.Where(o => o.CustId == iCustId), "GSTINId", "GSTINNo");
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst2;
                    return View();
                }
            }
            else if (!string.IsNullOrEmpty(OTPSubmit))
            {
                string strOTP = frm["OTP"].ToString();
                Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTINNo);
                string status = Models.GSTAUTH.GSP_GSTAuthwithOTP.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                if (status == "1")
                {
                    TempData["AuthMsg"] = "Authenticated Successfully";
                }
                else
                {
                    TempData["AuthMsg"] = status;
                }

                ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                SelectList Actionlst3 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                ViewBag.ActionList = Actionlst3;
            }
            #region "GSTR2 SA Delete Single"
            //Deleting Invoices from GSTR1 B2B
            else if (command == "GSTR2B2B")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2B", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CL
            else if (command == "GSTR1B2CL")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CL", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 AT
            else if (command == "GSTR1AT")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("AT", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNUR
            else if (command == "GSTR1CDNUR")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNUR", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNR
            else if (command == "GSTR1CDNR")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNR", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CS
            else if (command == "GSTR1B2CS")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CS", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 DOCISSUE
            else if (command == "GSTR1DOC")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("DOC", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 EXP
            else if (command == "GSTR1EXP")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("EXP", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 HSN
            else if (command == "GSTR1HSN")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("HSN", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR1 NIL
            else if (command == "GSTR1NIL")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("NIL", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 TXPD
            else if (command == "GSTR1TXPD")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1_Checksum_Calculation("TXP", strGSTINNo, strFp, strRefIds);
                objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            #endregion

            #region "GSTR2 SA Delete ALL"
            // ALL Delete by Action
            //Deleting Invoices from GSTR1 B2B
            else if (command == "GSTR1B2BALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2B", strGSTINNo, strFp, Session["SAUB2BRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, Session["SAUB2BRefIds"].ToString());
                //objDeleteGSTR1.GSTR1DeleteALL("B2B", Session["B2BRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CL
            else if (command == "GSTR1B2CLALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CL", strGSTINNo, strFp, Session["SAUB2CLRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, Session["SAUB2CLRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 AT
            else if (command == "GSTR1ATALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("AT", strGSTINNo, strFp, Session["SAUATRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, Session["SAUATRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNUR
            else if (command == "GSTR1CDNURALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNUR", strGSTINNo, strFp, Session["SAUCDNURRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, Session["SAUCDNURRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNR
            else if (command == "GSTR1CDNRALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNR", strGSTINNo, strFp, Session["SAUCDNRRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, Session["SAUCDNRRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CS
            else if (command == "GSTR1B2CSALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CS", strGSTINNo, strFp, Session["SAUB2CSRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, Session["SAUB2CSRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 DOCISSUE
            else if (command == "GSTR1DOCALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("DOC", strGSTINNo, strFp, Session["SAUDOCRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, Session["SAUDOCRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 EXP
            else if (command == "GSTR1EXPALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("EXP", strGSTINNo, strFp, Session["SAUEXPRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, Session["SAUEXPRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 HSN
            else if (command == "GSTR1HSNALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("HSN", strGSTINNo, strFp, Session["SAUHSNRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, Session["SAUHSNRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR1 NIL
            else if (command == "GSTR1NILALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("NIL", strGSTINNo, strFp, Session["SAUNILRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, Session["SAUNILRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 TXPD
            else if (command == "GSTR1TXPDALL")
            {
                objDeleteGSTR1.GSTR1_Checksum_Calculation("TXP", strGSTINNo, strFp, Session["SAUTXPRefIds"].ToString());
                objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, Session["SAUTXPRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            #endregion

            #region "GSTR2 SA Error Records Delete Single"
            //Deleting Invoices from GSTR1 B2B
            else if (command == "GSTR1B2B_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CL
            else if (command == "GSTR1B2CL_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 AT
            else if (command == "GSTR1AT_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNUR
            else if (command == "GSTR1CDNUR_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNR
            else if (command == "GSTR1CDNR_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CS
            else if (command == "GSTR1B2CS_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 DOCISSUE
            else if (command == "GSTR1DOC_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 EXP
            else if (command == "GSTR1EXP_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 HSN
            else if (command == "GSTR1HSN_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }

            //Deleting Invoices from GSTR1 NIL
            else if (command == "GSTR1NIL_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 TXPD
            else if (command == "GSTR1TXPD_ER")
            {
                string strRefIds = "";
                foreach (int invid in ids)
                {
                    strRefIds += invid.ToString() + ",";
                }
                objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, strRefIds);
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            #endregion

            #region "GSTR2 SA Error Records Delete ALL"
            // ALL Delete by Action
            //Deleting Invoices from GSTR1 B2B
            else if (command == "GSTR1B2BALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, Session["SAEB2BRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CL
            else if (command == "GSTR1B2CLALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, Session["SAEB2CLRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 AT
            else if (command == "GSTR1ATALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, Session["SAEATRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNUR
            else if (command == "GSTR1CDNURALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, Session["SAECDNURRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 CDNR
            else if (command == "GSTR1CDNRALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, Session["SAECDNRRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 B2CS
            else if (command == "GSTR1B2CSALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, Session["SAEB2CSRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 DOCISSUE
            else if (command == "GSTR1DOCALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, Session["SAEDOCRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 EXP
            else if (command == "GSTR1EXPALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, Session["SAEEXPRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 HSN
            else if (command == "GSTR1HSNALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, Session["SAEHSNRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 NIL
            else if (command == "GSTR1NILALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, Session["SAENILRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            //Deleting Invoices from GSTR1 TXPD
            else if (command == "GSTR1TXPDALL_ER")
            {
                objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, Session["SAETXPRefIds"].ToString());
                TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
            }
            #endregion
            else
            {
                ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                SelectList Actionlst4 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                ViewBag.ActionList = Actionlst4;
                ViewBag.TitleHeaders = "GSTR-2 View";

                SqlDataAdapter adt = new SqlDataAdapter("Select TOP 1 * from TBL_AUTH_KEYS where AuthorizationToken = '" + strGSTINNo + "'", con);
                DataTable dt = new DataTable();
                adt.Fill(dt);
                if (dt.Rows.Count > 0)
                {
                    var varFinish = DateTime.Now;
                    var varValue = dt.Rows[0]["CreatedDate"];
                    TimeSpan varTime = (DateTime)varFinish - (DateTime)varValue;
                    int intMinutes = (int)varTime.TotalMinutes;

                    if (Convert.ToInt32(dt.Rows[0]["Expiry"]) > intMinutes)
                    {
                        if (359 >= intMinutes) // 6 Hours
                        {
                            ViewBag.OTPSession = "CLOSE_POPUP";
                        }
                        else
                        {
                            Models.GSTAUTH.GenerateKeys.GeneratingEncryptedKeys(strGSTINNo);
                            ViewBag.AUTH_Response = Models.GSTAUTH.GSP_GSTAuth.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                            ViewBag.OTPSession = "OPEN_POPUP";
                        }
                    }
                    else
                    {
                        Models.GSTAUTH.GenerateKeys.GeneratingEncryptedKeys(strGSTINNo);
                        ViewBag.AUTH_Response = Models.GSTAUTH.GSP_GSTAuth.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                        ViewBag.OTPSession = "OPEN_POPUP";
                    }
                }
                else
                {
                    //Models.GSTAUTH.GenerateKeys.GeneratingEncryptedKeys(strGSTINNo);
                    //Models.GSTAUTH.GSTAuth.SendRequest(strGSTINNo);
                    //ViewBag.OTPSession = "OPEN_POPUP";
                }
            }
            ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR2"), "ActionName", "ActionName", strAction);
            ViewBag.ActionList = Actionlst;
            LoadGSTR1ViewData(strAction, strGSTINNo, strFp);

            return View();
        }

        public void LoadSessionVariables()
        {
            // Uploaded Data
            Session["G2SAUB2BRefIds"] = "";
            Session["G2SAUB2BURRefIds"] = "";            
            Session["G2SAUCDNRRefIds"] = "";
            Session["G2SAUCDNURRefIds"] = "";
            Session["G2SAUIMPGRefIds"] = "";
            Session["G2SAUIMPSRefIds"] = "";
            Session["G2SAUHSNRefIds"] = "";
            Session["G2SAUNILRefIds"] = "";
            Session["G2SAUTXPDRefIds"] = "";
            Session["G2SAUITCRefIds"] = "";
            Session["G2SAUTXLIRefIds"] = "";

            // Error Records
            Session["SAEB2BRefIds"] = "";
            Session["SAEB2CLRefIds"] = "";
            Session["SAEB2CSRefIds"] = "";
            Session["SAECDNRRefIds"] = "";
            Session["SAECDNURRefIds"] = "";
            Session["SAEEXPRefIds"] = "";
            Session["SAEHSNRefIds"] = "";
            Session["SAENILRefIds"] = "";
            Session["SAETXPRefIds"] = "";
            Session["SAEATRefIds"] = "";
            Session["SAEDOCRefIds"] = "";
        }

        public void LoadGSTR1ViewData(string strAction, string strGSTINNo, string strFp)
        {
            LoadSessionVariables();
            // GSTR1 Uploaded Data
            if (strAction == "B2B" || strAction == "ALL")
            {
                var B2B = GSTR1ViewDataModel.GetB2B(strGSTINNo, strFp, "U");
                ViewBag.B2B = B2B;
            }
            if (strAction == "B2CL" || strAction == "ALL")
            {
                var B2CL = GSTR1ViewDataModel.GetB2CL(strGSTINNo, strFp, "U");
                ViewBag.B2CL = B2CL;
            }
            if (strAction == "CDNR" || strAction == "ALL")
            {
                var CDNR = GSTR1ViewDataModel.GetCDNR(strGSTINNo, strFp, "U");
                ViewBag.CDNR = CDNR;
            }
            if (strAction == "B2CS" || strAction == "ALL")
            {
                var B2CS = GSTR1ViewDataModel.GetB2CS(strGSTINNo, strFp, "U");
                ViewBag.B2CS = B2CS;
            }
            if (strAction == "EXP" || strAction == "ALL")
            {
                var EXP = GSTR1ViewDataModel.GetEXP(strGSTINNo, strFp, "U");
                ViewBag.EXP = EXP;
            }
            if (strAction == "HSNSUM" || strAction == "ALL")
            {
                var HSN = GSTR1ViewDataModel.GetHSN(strGSTINNo, strFp, "U");
                ViewBag.HSN = HSN;
            }
            if (strAction == "NIL" || strAction == "ALL")
            {
                var NIL = GSTR1ViewDataModel.GetNIL(strGSTINNo, strFp, "U");
                ViewBag.NIL = NIL;
            }
            if (strAction == "TXPD" || strAction == "ALL")
            {
                var TXP = GSTR1ViewDataModel.GetTXP(strGSTINNo, strFp, "U");
                ViewBag.TXP = TXP;
            }
            if (strAction == "AT" || strAction == "ALL")
            {
                var AT = GSTR1ViewDataModel.GetAT(strGSTINNo, strFp, "U");
                ViewBag.AT = AT;
            }
            if (strAction == "DOCISSUE" || strAction == "ALL")
            {
                var DOC = GSTR1ViewDataModel.GetDOC(strGSTINNo, strFp, "U");
                ViewBag.DOC = DOC;
            }
            if (strAction == "CDNUR" || strAction == "ALL")
            {
                var CDNUR = GSTR1ViewDataModel.GetCDNUR(strGSTINNo, strFp, "U");
                ViewBag.CDNUR = CDNUR;
            }

            // GSTR1 Error Records
            if (strAction == "B2B" || strAction == "ALL")
            {
                var B2B_ER = GSTR1ViewDataModel.GetB2B_ER(strGSTINNo, strFp, "1");
                ViewBag.B2B_ER = B2B_ER;
            }
            if (strAction == "B2CL" || strAction == "ALL")
            {
                var B2CL_ER = GSTR1ViewDataModel.GetB2CL_ER(strGSTINNo, strFp, "1");
                ViewBag.B2CL_ER = B2CL_ER;
            }
            if (strAction == "CDNR" || strAction == "ALL")
            {
                var CDNR_ER = GSTR1ViewDataModel.GetCDNR_ER(strGSTINNo, strFp, "1");
                ViewBag.CDNR_ER = CDNR_ER;
            }
            if (strAction == "B2CS" || strAction == "ALL")
            {
                var B2CS_ER = GSTR1ViewDataModel.GetB2CS_ER(strGSTINNo, strFp, "1");
                ViewBag.B2CS_ER = B2CS_ER;
            }
            if (strAction == "EXP" || strAction == "ALL")
            {
                var EXP_ER = GSTR1ViewDataModel.GetEXP_ER(strGSTINNo, strFp, "1");
                ViewBag.EXP_ER = EXP_ER;
            }
            if (strAction == "HSNSUM" || strAction == "ALL")
            {
                var HSN_ER = GSTR1ViewDataModel.GetHSN_ER(strGSTINNo, strFp, "1");
                ViewBag.HSN_ER = HSN_ER;
            }
            if (strAction == "NIL" || strAction == "ALL")
            {
                var NIL_ER = GSTR1ViewDataModel.GetNIL_ER(strGSTINNo, strFp, "1");
                ViewBag.NIL_ER = NIL_ER;
            }
            if (strAction == "TXPD" || strAction == "ALL")
            {
                var TXP_ER = GSTR1ViewDataModel.GetTXP_ER(strGSTINNo, strFp, "1");
                ViewBag.TXP_ER = TXP_ER;
            }
            if (strAction == "AT" || strAction == "ALL")
            {
                var AT_ER = GSTR1ViewDataModel.GetAT_ER(strGSTINNo, strFp, "1");
                ViewBag.AT_ER = AT_ER;
            }
            if (strAction == "DOCISSUE" || strAction == "ALL")
            {
                var DOC_ER = GSTR1ViewDataModel.GetDOC_ER(strGSTINNo, strFp, "1");
                ViewBag.DOC_ER = DOC_ER;
            }
            if (strAction == "CDNUR" || strAction == "ALL")
            {
                var CDNUR_ER = GSTR1ViewDataModel.GetCDNUR_ER(strGSTINNo, strFp, "1");
                ViewBag.CDNUR_ER = CDNUR_ER;
            }
        }

        public string Checking_GSTR1_RetStatus()
        {
            string Status_Response = "";
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select referenceno, gstin, fp, actiontype from TBL_GSTR1_SAVE_RETSTATUS where status = '1' and customerid = '" + Session["Cust_ID"].ToString() + "' and createdby = '" + Session["Cust_ID"].ToString() + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    string gstinno = dt.Rows[i]["gstin"].ToString();
                    string period = dt.Rows[i]["fp"].ToString();
                    string RefId = dt.Rows[i]["referenceno"].ToString();

                    GetGSTR1DataModel GSP_getGSTR1 = new GetGSTR1DataModel();
                    //GSP_GetGSTR1DataModel GSP_getGSTR1 = new GSP_GetGSTR1DataModel();
                    Status_Response = GSP_getGSTR1.SendRequest(gstinno, period, "RETSTATUS", "", "", RefId, "", Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                }
            }
            con.Close();
            return Status_Response;
        }

        public void Display_RetStatus()
        {
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_GSTR1_SAVE_RETSTATUS where status != '1' and customerid = '" + Session["Cust_ID"].ToString() + "' and createdby = '" + Session["User_ID"].ToString() + "' order by 1 desc", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if (dt.Rows.Count > 0)
            {
                ViewBag.RefNo = dt.Rows[0]["referenceno"].ToString();
                ViewBag.Status = dt.Rows[0]["status"].ToString();
                ViewBag.Gstin = dt.Rows[0]["gstin"].ToString();
            }
            con.Close();
        }
    }
}