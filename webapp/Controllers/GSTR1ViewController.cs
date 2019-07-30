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
using System.Threading;
using GSTR1Save;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    //[System.Web.Mvc.Authorize(Roles="Active")]

    public class GSTR1ViewController : Controller
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
            ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE.Where(o => o.GSTRId != 2), "GSTRId", "GSTRName");
            ViewBag.GSTRList = GSTRlst;

            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
            ViewBag.ActionList = Actionlst;
            ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
            //Display_RetStatus();

            LoadSessionVariables();
            return View();
        }


        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string GSTR1Delete, string GSTR1ReUpload, string OTPSubmit, int[] ids, string command)
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
                //int strGSTINId = Convert.ToInt32(frm["ddlGSTINNo"]);
                int GSTR_Type = 0;
                string strGSTINNo = frm["ddlGSTINNo"];
                string strAction = frm["ddlActionType"];
                string strFp = frm["period"];
                string strRecordsType = frm["ddlRecordType"];
                string GSTRType = frm["ddlGSTR"];
                if (GSTRType != "")
                {
                    ViewBag.GSTRType = Convert.ToInt32(GSTRType);
                    GSTR_Type = Convert.ToInt32(GSTRType);
                }
                Session["GSTRType"] = GSTRType;

                string strRefIds = frm["InvIds"];
                strRefIds = strRefIds.TrimStart(',').TrimEnd(',');

                TempData["ActionType"] = strAction;
                TempData["GSTINNo"] = strGSTINNo;
                TempData["period"] = strFp;
                ViewBag.Period = strFp;
                ViewBag.ActionType = strAction;
                ViewBag.RecordsType = strRecordsType;
                //Display_RetStatus();

                Delete_GSTR1_SA objDeleteGSTR1 = new Delete_GSTR1_SA();

                #region "GSTR 1 DELETE FROM GST SERVER"
                if (!string.IsNullOrEmpty(GSTR1Delete))
                {
                    if (strGSTINNo == "Select" || strGSTINNo == "")
                    {
                        TempData["ViewResponse"] = "Please Select GSTIN";
                        ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                        ViewBag.ActionList = Actionlst0;
                        ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
                        return View();
                    }
                    else if (strAction == "Select Action" || strAction == "")
                    {
                        TempData["ViewResponse"] = "Please Select Action Type";
                        ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                        ViewBag.ActionList = Actionlst0;
                        ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
                        return View();
                    }
                    else
                    {
                        GSP_SendGSTR1ViewDataModel GSP_fnGSTR1View = new GSP_SendGSTR1ViewDataModel();
                        //SendGSTR1ViewDataModel GSP_fnGSTR1View = new SendGSTR1ViewDataModel();
                        string strJsonData = GSTR1ViewDataModel.GetJsonGSTR1_View(strGSTINNo, strFp, strAction, "D");
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                        string ViewResponse = GSP_fnGSTR1View.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                        //TempData["ViewResponse"] = "GSTR1 Saved Successfully... Trans Id - TXOID0000000011 and Ref Id - LAPN24235325555";
                        TempData["ViewResponse"] = ViewResponse;

                        //SelectList GSTINNolst = new SelectList(db.TBL_Cust_GSTIN.Where(o => o.CustId == iCustId), "GSTINId", "GSTINNo");
                        ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());
                        SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                        ViewBag.ActionList = Actionlst1;
                        ViewBag.RecordsTypeList = LoadGSTINNo.Exist_GetGSTR_RecordsType(strRecordsType);
                        SelectList GSTRlst1 = new SelectList(db.TBL_GSTR_TYPE.Where(o => o.GSTRId != 2), "GSTRId", "GSTRName", GSTRType);
                        ViewBag.GSTRList = GSTRlst1;
                        LoadSessionVariables();
                        return View();
                    }
                }
                #endregion

                #region "GSTR 1 REUPLOAD ERROR DATA"
                else if (!string.IsNullOrEmpty(GSTR1ReUpload))
                {
                    if (strGSTINNo == "Select" || strGSTINNo == "")
                    {
                        TempData["ViewResponse"] = "Please Select GSTIN";
                        ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                        ViewBag.ActionList = Actionlst0;
                        ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
                        return View();
                    }
                    else if (strAction == "Select Action" || strAction == "")
                    {
                        TempData["ViewResponse"] = "Please Select Action Type";
                        ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                        ViewBag.ActionList = Actionlst0;
                        ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
                        return View();
                    }
                    else
                    {
                        #region "GSTR-1 BATCH PROCESSING"
                        GSP_SendGSTR1SaveDataModel GSP_fnGSTR1Save = new GSP_SendGSTR1SaveDataModel();
                        string strJsonData = GSTR1DataModel.GetJsonGSTR1Save(strGSTINNo, strFp, strAction, "1");
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                        #region "B2B"
                        if (strAction == "B2B")
                        {
                            int total_invCount = 0;
                            var tot_objGSTR = JsonConvert.DeserializeObject<B2BBatch>(strJsonData);
                            for (int a = 0; a < tot_objGSTR.b2b.Count(); a++)
                            {
                                var tot_jsonB2B = JsonConvert.SerializeObject(tot_objGSTR.b2b[a], Newtonsoft.Json.Formatting.Indented);
                                var tot_objB2B = JsonConvert.DeserializeObject<B2B>(tot_jsonB2B.ToString());
                                total_invCount += tot_objB2B.inv.Count;
                            }
                            if (total_invCount > Convert.ToInt32(ConfigurationManager.AppSettings["GSTR1_BATCH"]))
                            {
                                var objGSTR = JsonConvert.DeserializeObject<B2BBatch>(strJsonData);
                                string gstin, fp, strctin;
                                decimal gt, cur_gt;
                                gstin = objGSTR.gstin;
                                fp = objGSTR.fp;
                                gt = objGSTR.gt;
                                cur_gt = objGSTR.cur_gt;

                                int batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["GSTR1_BATCH"]);
                                B2BBatch master = new B2BBatch();
                                List<B2B> master1 = new List<B2B>();
                                int finalb2b_count = 0;
                                int finalinv_count = 0;
                                string split = "no";
                                finalb2b_count = objGSTR.b2b.Count;
                                for (int i = 0; i < objGSTR.b2b.Count(); i++)
                                {
                                    master.gstin = gstin;
                                    master.fp = fp;
                                    master.gt = gt;
                                    master.cur_gt = cur_gt;

                                    var jsonB2B = JsonConvert.SerializeObject(objGSTR.b2b[i], Newtonsoft.Json.Formatting.Indented);
                                    var objB2B = JsonConvert.DeserializeObject<B2B>(jsonB2B.ToString());
                                    strctin = objB2B.ctin;
                                    for (int j = 0; j < objB2B.inv.Count(); j += batchSize)
                                    {
                                        if (split == "no")
                                        {
                                            finalinv_count += objB2B.inv.Count;
                                            if (finalinv_count <= batchSize)
                                            {
                                                var cc = objB2B.inv.GetRange(j, Math.Min(batchSize, objB2B.inv.Count - j));
                                                if (cc.Count > 0)
                                                {
                                                    master1.Add(new B2B { ctin = strctin, inv = cc });
                                                }
                                                master.b2b = master1;
                                                break;
                                            }
                                            else
                                            {
                                                var cc = objB2B.inv.GetRange(j, Math.Min(batchSize, objB2B.inv.Count - (finalinv_count - batchSize)));
                                                if (cc.Count > 0)
                                                {
                                                    master1.Add(new B2B { ctin = strctin, inv = cc });
                                                }
                                                master.b2b = master1;
                                                var jsonString = JsonConvert.SerializeObject(master, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                                Thread.Sleep(2000);
                                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoid(jsonString, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                                    , Session["G1_SAEB2BRefIds"].ToString())
                                                );
                                                Thread.Sleep(1000);
                                                master1.Clear();
                                                finalinv_count = finalinv_count - batchSize;
                                                split = "yes";
                                            }
                                        }
                                        if (split == "yes")
                                        {
                                            if (finalinv_count <= batchSize)
                                            {
                                                var cc = objB2B.inv.GetRange(objB2B.inv.Count - finalinv_count, Math.Min(batchSize, finalinv_count));
                                                if (cc.Count > 0)
                                                {
                                                    master1.Add(new B2B { ctin = strctin, inv = cc });
                                                }
                                                master.b2b = master1;
                                                split = "no";
                                                break;
                                            }
                                            else
                                            {
                                                var cc = objB2B.inv.GetRange(objB2B.inv.Count - finalinv_count, Math.Min(batchSize, finalinv_count - (finalinv_count - batchSize)));
                                                if (cc.Count > 0)
                                                {
                                                    master1.Add(new B2B { ctin = strctin, inv = cc });
                                                }
                                                master.b2b = master1;
                                                var jsonString = JsonConvert.SerializeObject(master, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                                Thread.Sleep(2000);
                                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoid(jsonString, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                                    , Session["G1_SAEB2BRefIds"].ToString())
                                                );
                                                Thread.Sleep(1000);
                                                master1.Clear();
                                                finalinv_count = finalinv_count - batchSize;
                                                split = "";

                                            }
                                        }
                                        if (split == "")
                                        {
                                            var cc = objB2B.inv.GetRange(objB2B.inv.Count - finalinv_count, Math.Min(batchSize, finalinv_count));
                                            if (cc.Count > 0)
                                            {
                                                master1.Add(new B2B { ctin = strctin, inv = cc });
                                            }
                                            master.b2b = master1;
                                            split = "no";
                                            break;
                                        }
                                    }
                                }
                                var jsonString1 = JsonConvert.SerializeObject(master, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                Thread.Sleep(2000);
                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoid(jsonString1, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                    , Session["G1_SAEB2BRefIds"].ToString())
                                );
                                Thread.Sleep(1000);
                                master1.Clear();
                                TempData["ViewResponse"] = "GSTR1 Save is in progress... Please check after sometime in AuditLog Report for Reference Id";
                            }
                            else
                            {
                                string ViewResponse = GSP_fnGSTR1Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp
                                , Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                , Session["G1_SAEB2BRefIds"].ToString(), Session["G1_SAEB2CLRefIds"].ToString(), Session["G1_SAEB2CSRefIds"].ToString()
                                , Session["G1_SAECDNRRefIds"].ToString(), Session["G1_SAEEXPRefIds"].ToString(), Session["G1_SAEHSNRefIds"].ToString()
                                , Session["G1_SAENILRefIds"].ToString(), Session["G1_SAETXPRefIds"].ToString(), Session["G1_SAEATRefIds"].ToString()
                                , Session["G1_SAEDOCRefIds"].ToString(), Session["G1_SAECDNURRefIds"].ToString()
                                , Session["G1_SAEB2BARefIds"].ToString(), Session["G1_SAEB2CLARefIds"].ToString(), Session["G1_SAEB2CSARefIds"].ToString()
                                , Session["G1_SAECDNRARefIds"].ToString(), Session["G1_SAEEXPARefIds"].ToString(), Session["G1_SAETXPARefIds"].ToString()
                                , Session["G1_SAEATARefIds"].ToString(), Session["G1_SAECDNURARefIds"].ToString());

                                TempData["ViewResponse"] = ViewResponse;
                            }
                        }
                        #endregion

                        #region "CDNR"
                        else if (strAction == "CDNR")
                        {
                            int total_invCount = 0;
                            var tot_objGSTR = JsonConvert.DeserializeObject<CDNRBatch>(strJsonData);
                            for (int a = 0; a < tot_objGSTR.cdnr.Count(); a++)
                            {
                                var tot_jsonCDNR = JsonConvert.SerializeObject(tot_objGSTR.cdnr[a], Newtonsoft.Json.Formatting.Indented);
                                var tot_objCDNR = JsonConvert.DeserializeObject<CDNR>(tot_jsonCDNR.ToString());
                                total_invCount += tot_objCDNR.nt.Count;
                            }
                            if (total_invCount > Convert.ToInt32(ConfigurationManager.AppSettings["GSTR1_BATCH"]))
                            {
                                var objGSTR = JsonConvert.DeserializeObject<CDNRBatch>(strJsonData);
                                string gstin, fp, strctin;
                                decimal gt, cur_gt;
                                gstin = objGSTR.gstin;
                                fp = objGSTR.fp;
                                gt = objGSTR.gt;
                                cur_gt = objGSTR.cur_gt;

                                int batchSize = Convert.ToInt32(ConfigurationManager.AppSettings["GSTR1_BATCH"]);
                                CDNRBatch CDNRmaster = new CDNRBatch();
                                List<CDNR> CDNRmaster1 = new List<CDNR>();
                                int finalb2b_count = 0;
                                int finalinv_count = 0;
                                string split = "no";
                                finalb2b_count = objGSTR.cdnr.Count;
                                for (int i = 0; i < objGSTR.cdnr.Count(); i++)
                                {
                                    CDNRmaster.gstin = gstin;
                                    CDNRmaster.fp = fp;
                                    CDNRmaster.gt = gt;
                                    CDNRmaster.cur_gt = cur_gt;

                                    var jsonCDNR = JsonConvert.SerializeObject(objGSTR.cdnr[i], Newtonsoft.Json.Formatting.Indented);
                                    var objCDNR = JsonConvert.DeserializeObject<CDNR>(jsonCDNR.ToString());
                                    strctin = objCDNR.ctin;
                                    for (int j = 0; j < objCDNR.nt.Count(); j += batchSize)
                                    {
                                        if (split == "no")
                                        {
                                            finalinv_count += objCDNR.nt.Count;
                                            if (finalinv_count <= batchSize)
                                            {
                                                var cc = objCDNR.nt.GetRange(j, Math.Min(batchSize, objCDNR.nt.Count - j));
                                                if (cc.Count > 0)
                                                {
                                                    CDNRmaster1.Add(new CDNR { ctin = strctin, nt = cc });
                                                }
                                                CDNRmaster.cdnr = CDNRmaster1;
                                                break;
                                            }
                                            else
                                            {
                                                var cc = objCDNR.nt.GetRange(j, Math.Min(batchSize, objCDNR.nt.Count - (finalinv_count - batchSize)));
                                                if (cc.Count > 0)
                                                {
                                                    CDNRmaster1.Add(new CDNR { ctin = strctin, nt = cc });
                                                }
                                                CDNRmaster.cdnr = CDNRmaster1;
                                                var jsonString = JsonConvert.SerializeObject(CDNRmaster, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoidCDNR(jsonString, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                                    , Session["G1_SAECDNRRefIds"].ToString())
                                                );
                                                Thread.Sleep(1000);
                                                CDNRmaster1.Clear();
                                                finalinv_count = finalinv_count - batchSize;
                                                split = "yes";
                                            }
                                        }
                                        if (split == "yes")
                                        {
                                            if (finalinv_count <= batchSize)
                                            {
                                                var cc = objCDNR.nt.GetRange(objCDNR.nt.Count - finalinv_count, Math.Min(batchSize, finalinv_count));
                                                if (cc.Count > 0)
                                                {
                                                    CDNRmaster1.Add(new CDNR { ctin = strctin, nt = cc });
                                                }
                                                CDNRmaster.cdnr = CDNRmaster1;
                                                split = "no";
                                                break;
                                            }
                                            else
                                            {
                                                var cc = objCDNR.nt.GetRange(objCDNR.nt.Count - finalinv_count, Math.Min(batchSize, finalinv_count - (finalinv_count - batchSize)));
                                                if (cc.Count > 0)
                                                {
                                                    CDNRmaster1.Add(new CDNR { ctin = strctin, nt = cc });
                                                }
                                                CDNRmaster.cdnr = CDNRmaster1;
                                                var jsonString = JsonConvert.SerializeObject(CDNRmaster, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoidCDNR(jsonString, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                                    , Session["G1_SAECDNRRefIds"].ToString())
                                                );
                                                Thread.Sleep(1000);
                                                CDNRmaster1.Clear();
                                                finalinv_count = finalinv_count - batchSize;
                                                split = "";

                                            }
                                        }
                                        if (split == "")
                                        {
                                            var cc = objCDNR.nt.GetRange(objCDNR.nt.Count - finalinv_count, Math.Min(batchSize, finalinv_count));
                                            if (cc.Count > 0)
                                            {
                                                CDNRmaster1.Add(new CDNR { ctin = strctin, nt = cc });
                                            }
                                            CDNRmaster.cdnr = CDNRmaster1;
                                            split = "no";
                                            break;
                                        }
                                    }
                                }
                                var jsonString1 = JsonConvert.SerializeObject(CDNRmaster, Newtonsoft.Json.Formatting.Indented, new JsonSerializerSettings { DefaultValueHandling = DefaultValueHandling.Ignore });
                                Task.Factory.StartNew(() => GSP_fnGSTR1Save.SendRequestVoidCDNR(jsonString1, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                    , Session["G1_SAECDNRRefIds"].ToString())
                                );
                                Thread.Sleep(1000);
                                CDNRmaster1.Clear();
                                TempData["ViewResponse"] = "GSTR1 Save is in progress... Please check after sometime in AuditLog Report for Reference Id";
                            }
                            else
                            {
                                string ViewResponse = GSP_fnGSTR1Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp
                                , Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                                , Session["G1_SAEB2BRefIds"].ToString(), Session["G1_SAEB2CLRefIds"].ToString(), Session["G1_SAEB2CSRefIds"].ToString()
                                , Session["G1_SAECDNRRefIds"].ToString(), Session["G1_SAEEXPRefIds"].ToString(), Session["G1_SAEHSNRefIds"].ToString()
                                , Session["G1_SAENILRefIds"].ToString(), Session["G1_SAETXPRefIds"].ToString(), Session["G1_SAEATRefIds"].ToString()
                                , Session["G1_SAEDOCRefIds"].ToString(), Session["G1_SAECDNURRefIds"].ToString()
                                , Session["G1_SAEB2BARefIds"].ToString(), Session["G1_SAEB2CLARefIds"].ToString(), Session["G1_SAEB2CSARefIds"].ToString()
                                , Session["G1_SAECDNRARefIds"].ToString(), Session["G1_SAEEXPARefIds"].ToString(), Session["G1_SAETXPARefIds"].ToString()
                                , Session["G1_SAEATARefIds"].ToString(), Session["G1_SAECDNURARefIds"].ToString());

                                TempData["ViewResponse"] = ViewResponse;
                            }
                        }
                        #endregion

                        #region "OTHER ACTION"
                        else
                        {
                            string ViewResponse = GSP_fnGSTR1Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp
                            , Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()
                            , Session["G1_SAEB2BRefIds"].ToString(), Session["G1_SAEB2CLRefIds"].ToString(), Session["G1_SAEB2CSRefIds"].ToString()
                            , Session["G1_SAECDNRRefIds"].ToString(), Session["G1_SAEEXPRefIds"].ToString(), Session["G1_SAEHSNRefIds"].ToString()
                            , Session["G1_SAENILRefIds"].ToString(), Session["G1_SAETXPRefIds"].ToString(), Session["G1_SAEATRefIds"].ToString()
                            , Session["G1_SAEDOCRefIds"].ToString(), Session["G1_SAECDNURRefIds"].ToString()
                            , Session["G1_SAEB2BARefIds"].ToString(), Session["G1_SAEB2CLARefIds"].ToString(), Session["G1_SAEB2CSARefIds"].ToString()
                            , Session["G1_SAECDNRARefIds"].ToString(), Session["G1_SAEEXPARefIds"].ToString(), Session["G1_SAETXPARefIds"].ToString()
                            , Session["G1_SAEATARefIds"].ToString(), Session["G1_SAECDNURARefIds"].ToString());

                            TempData["ViewResponse"] = ViewResponse;
                        }
                        #endregion

                        #endregion
                        
                        //SelectList GSTINNolst = new SelectList(db.TBL_Cust_GSTIN.Where(o => o.CustId == iCustId), "GSTINId", "GSTINNo");
                        ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());
                        SelectList Actionlst2 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                        ViewBag.ActionList = Actionlst2;
                        ViewBag.RecordsTypeList = LoadGSTINNo.Exist_GetGSTR_RecordsType(strRecordsType);
                        SelectList GSTRlst1 = new SelectList(db.TBL_GSTR_TYPE.Where(o => o.GSTRId != 2), "GSTRId", "GSTRName", GSTRType);
                        ViewBag.GSTRList = GSTRlst1;
                        LoadSessionVariables();
                        return View();
                    }
                }
                #endregion

                #region "OTP SUBMIT FOR AUTH TOKEN"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = frm["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTINNo);
                    string status = Models.GSTAUTH.GSP_GSTAuthwithOTP.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                    if (status == "1")
                    {
                        TempData["ViewResponse"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["ViewResponse"] = status;
                    }

                    ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                    SelectList Actionlst3 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                    ViewBag.ActionList = Actionlst3;
                    ViewBag.RecordsTypeList = LoadGSTINNo.Exist_GetGSTR_RecordsType(strRecordsType);
                }
                #endregion

                #region "GSTR1 SA UPLOADED DATA DELETE ALL"
                // ALL Delete by Action
                //Deleting Invoices from GSTR1 B2B
                else if (command == "GSTR1B2B")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("B2B", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, strRefIds);
                    //objDeleteGSTR1.GSTR1DeleteALL("B2B", Session["B2BRefIds"].ToString());
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUB2B"] = 0;
                }
                else if (command == "GSTR1B2B_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2BRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("B2B", strGSTINNo, strFp, Session["G1_SAUB2BRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, Session["G1_SAUB2BRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUB2B"] = 0;
                }
                else if (command == "GSTR1B2BA")
                {
                   // objDeleteGSTR1.GSTR1_Checksum_Calculation("B2BA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2BA", strGSTINNo, strFp, strRefIds);
                    //objDeleteGSTR1.GSTR1DeleteALL("B2B", Session["B2BRefIds"].ToString());
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUB2BA"] = 0;
                }
                else if (command == "GSTR1B2BA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2BARefIds"].ToString()))
                    {
                        //objDeleteGSTR1.GSTR1_Checksum_Calculation("B2BA", strGSTINNo, strFp, Session["G1_SAUB2ABRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2BA", strGSTINNo, strFp, Session["G1_SAUB2BARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUB2BA"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CL
                else if (command == "GSTR1B2CL")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CL", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_GSAU1B2CL"] = 0;
                }
                else if (command == "GSTR1B2CL_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2CLRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CL", strGSTINNo, strFp, Session["G1_SAUB2CLRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, Session["G1_SAUB2CLRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_GSAU1B2CL"] = 0;
                }

                else if (command == "GSTR1B2CLA")
                {
                  //  objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CLA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2CLA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_GSAU1B2CLA"] = 0;
                }
                else if (command == "GSTR1B2CLA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2CLARefIds"].ToString()))
                    {
                        //objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CLA", strGSTINNo, strFp, Session["G1_SAUB2CLARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2CLA", strGSTINNo, strFp, Session["G1_SAUB2CLARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_GSAU1B2CLA"] = 0;
                }
                //Deleting Invoices from GSTR1 AT
                else if (command == "GSTR1AT")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("AT", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUAT"] = 0;
                }

                else if (command == "GSTR1AT_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUATRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("AT", strGSTINNo, strFp, Session["G1_SAUATRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, Session["G1_SAUATRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUAT"] = 0;
                }

                else if (command == "GSTR1ATA")
                {
                    //objDeleteGSTR1.GSTR1_Checksum_Calculation("AT", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("ATA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUATA"] = 0;
                }

                else if (command == "GSTR1ATA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUATARefIds"].ToString()))
                    {
                       // objDeleteGSTR1.GSTR1_Checksum_Calculation("ATA", strGSTINNo, strFp, Session["G1_SAUATARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("ATA", strGSTINNo, strFp, Session["G1_SAUATARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUATA"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNUR
                else if (command == "GSTR1CDNUR")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNUR", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUCDNUR"] = 0;
                }

                else if (command == "GSTR1CDNUR_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUCDNURRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNUR", strGSTINNo, strFp, Session["G1_SAUCDNURRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, Session["G1_SAUCDNURRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUCDNUR"] = 0;
                }

                else if (command == "GSTR1CDNURA")
                {
                   // objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNURA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("CDNURA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUCDNURA"] = 0;
                }

                else if (command == "GSTR1CDNURA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUCDNURARefIds"].ToString()))
                    {
                       // objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNURA", strGSTINNo, strFp, Session["G1_SAUCDNURARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("CDNURA", strGSTINNo, strFp, Session["G1_SAUCDNURARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUCDNURA"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNR
                else if (command == "GSTR1CDNR")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNR", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUCDNR"] = 0;
                }
                else if (command == "GSTR1CDNR_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUCDNRRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNR", strGSTINNo, strFp, Session["G1_SAUCDNRRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, Session["G1_SAUCDNRRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUCDNR"] = 0;
                }

                else if (command == "GSTR1CDNRA")
                {
                    //objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNRA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("CDNRA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUCDNRA"] = 0;
                }
                else if (command == "GSTR1CDNRA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUCDNRARefIds"].ToString()))
                    {
                        //objDeleteGSTR1.GSTR1_Checksum_Calculation("CDNRA", strGSTINNo, strFp, Session["G1_SAUCDNRARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("CDNRA", strGSTINNo, strFp, Session["G1_SAUCDNRARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUCDNRA"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CS
                else if (command == "GSTR1B2CS")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CS", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUB2CS"] = 0;
                }
                else if (command == "GSTR1B2CS_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2CSRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CS", strGSTINNo, strFp, Session["G1_SAUB2CSRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, Session["G1_SAUB2CSRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUB2CS"] = 0;
                }
                else if (command == "GSTR1B2CSA")
                {
                   // objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CSA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("B2CSA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUB2CSA"] = 0;
                }
                else if (command == "GSTR1B2CSA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUB2CSARefIds"].ToString()))
                    {
                        //objDeleteGSTR1.GSTR1_Checksum_Calculation("B2CSA", strGSTINNo, strFp, Session["G1_SAUB2CSARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("B2CSA", strGSTINNo, strFp, Session["G1_SAUB2CSARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUB2CSA"] = 0;
                }
                //Deleting Invoices from GSTR1 DOCISSUE
                else if (command == "GSTR1DOC")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("DOC", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUDOC"] = 0;
                }
                else if (command == "GSTR1DOC_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUDOCRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("DOC", strGSTINNo, strFp, Session["G1_SAUDOCRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, Session["G1_SAUDOCRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUDOC"] = 0;
                }
                //Deleting Invoices from GSTR1 EXP
                else if (command == "GSTR1EXP")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("EXP", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUEXP"] = 0;
                }
                else if (command == "GSTR1EXP_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUEXPRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("EXP", strGSTINNo, strFp, Session["G1_SAUEXPRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, Session["G1_SAUEXPRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUEXP"] = 0;
                }
                else if (command == "GSTR1EXPA")
                {
                    //objDeleteGSTR1.GSTR1_Checksum_Calculation("EXPA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("EXPA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUEXPA"] = 0;
                }
                else if (command == "GSTR1EXPA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUEXPARefIds"].ToString()))
                    {
                        //objDeleteGSTR1.GSTR1_Checksum_Calculation("EXPA", strGSTINNo, strFp, Session["G1_SAUEXPARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("EXPA", strGSTINNo, strFp, Session["G1_SAUEXPARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUEXPA"] = 0;
                }
                //Deleting Invoices from GSTR1 HSN
                else if (command == "GSTR1HSN")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("HSN", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUHSN"] = 0;
                }
                else if (command == "GSTR1HSN_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUHSNRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("HSN", strGSTINNo, strFp, Session["G1_SAUHSNRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, Session["G1_SAUHSNRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUHSN"] = 0;
                }
                //Deleting Invoices from GSTR1 NIL
                else if (command == "GSTR1NIL")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("NIL", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUNIL"] = 0;
                }
                else if (command == "GSTR1NIL_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUNILRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("NIL", strGSTINNo, strFp, Session["G1_SAUNILRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, Session["G1_SAUNILRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUNIL"] = 0;
                }
                //Deleting Invoices from GSTR1 TXPD
                else if (command == "GSTR1TXP")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("TXP", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUTXP"] = 0;
                }
                else if (command == "GSTR1TXP_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUTXPRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("TXP", strGSTINNo, strFp, Session["G1_SAUTXPRefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, Session["G1_SAUTXPRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUTXP"] = 0;
                }
                else if (command == "GSTR1TXPA")
                {
                    objDeleteGSTR1.GSTR1_Checksum_Calculation("TXPA", strGSTINNo, strFp, strRefIds);
                    objDeleteGSTR1.GSTR1DeleteALL("TXPA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAUTXPA"] = 0;
                }
                else if (command == "GSTR1TXPA_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAUTXPARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1_Checksum_Calculation("TXPA", strGSTINNo, strFp, Session["G1_SAUTXPARefIds"].ToString());
                        objDeleteGSTR1.GSTR1DeleteALL("TXPA", strGSTINNo, strFp, Session["G1_SAUTXPARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAUTXPA"] = 0;
                }
                #endregion

                #region "GSTR1 SA ERROR RECORDS DELETE ALL"
                // ALL Delete by Action
                //Deleting Invoices from GSTR1 B2B
                else if (command == "GSTR1B2B_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2B"] = 0;
                }
                else if (command == "GSTR1B2B_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2BRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2B", strGSTINNo, strFp, Session["G1_SAEB2BRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2B"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CL
                else if (command == "GSTR1B2CL_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2CL"] = 0;
                }
                else if (command == "GSTR1B2CL_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2CLRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2CL", strGSTINNo, strFp, Session["G1_SAEB2CLRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2CL"] = 0;
                }
                //Deleting Invoices from GSTR1 AT
                else if (command == "GSTR1AT_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEAT"] = 0;
                }
                else if (command == "GSTR1AT_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEATRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("AT", strGSTINNo, strFp, Session["G1_SAEATRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEAT"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNUR
                else if (command == "GSTR1CDNUR_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAECDNUR"] = 0;
                }
                else if (command == "GSTR1CDNUR_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAECDNURRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("CDNUR", strGSTINNo, strFp, Session["G1_SAECDNURRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAECDNUR"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNR
                else if (command == "GSTR1CDNR_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAECDNR"] = 0;
                }
                else if (command == "GSTR1CDNR_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAECDNRRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("CDNR", strGSTINNo, strFp, Session["G1_SAECDNRRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAECDNR"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CS
                else if (command == "GSTR1B2CS_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2CS"] = 0;
                }
                else if (command == "GSTR1B2CS_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2CSRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2CS", strGSTINNo, strFp, Session["G1_SAEB2CSRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2CS"] = 0;
                }
                else if (command == "GSTR1B2BA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2BA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2BA"] = 0;
                }
                else if (command == "GSTR1B2BA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2BARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2BA", strGSTINNo, strFp, Session["G1_SAEB2BARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2BA"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CL
                else if (command == "GSTR1B2CLA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2CLA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2CLA"] = 0;
                }
                else if (command == "GSTR1B2CLA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2CLARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2CLA", strGSTINNo, strFp, Session["G1_SAEB2CLARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2CLA"] = 0;
                }
                //Deleting Invoices from GSTR1 AT
                else if (command == "GSTR1ATA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("ATA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEATA"] = 0;
                }
                else if (command == "GSTR1ATA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEATARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("ATA", strGSTINNo, strFp, Session["G1_SAEATARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEATA"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNUR
                else if (command == "GSTR1CDNURA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("CDNURA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAECDNURA"] = 0;
                }
                else if (command == "GSTR1CDNURA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAECDNURARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("CDNURA", strGSTINNo, strFp, Session["G1_SAECDNURARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAECDNURA"] = 0;
                }
                //Deleting Invoices from GSTR1 CDNR
                else if (command == "GSTR1CDNRA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("CDNRA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAECDNRA"] = 0;
                }
                else if (command == "GSTR1CDNRA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAECDNRARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("CDNRA", strGSTINNo, strFp, Session["G1_SAECDNRARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAECDNRA"] = 0;
                }
                //Deleting Invoices from GSTR1 B2CS
                else if (command == "GSTR1B2CSA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("B2CSA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEB2CSA"] = 0;
                }
                else if (command == "GSTR1B2CSA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEB2CSARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("B2CSA", strGSTINNo, strFp, Session["G1_SAEB2CSARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEB2CSA"] = 0;
                }
                //Deleting Invoices from GSTR1 DOCISSUE
                else if (command == "GSTR1DOC_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEDOC"] = 0;
                }
                else if (command == "GSTR1DOC_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEDOCRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("DOC", strGSTINNo, strFp, Session["G1_SAEDOCRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEDOC"] = 0;
                }
                //Deleting Invoices from GSTR1 EXP
                else if (command == "GSTR1EXP_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEEXP"] = 0;
                }
                else if (command == "GSTR1EXP_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEEXPRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("EXP", strGSTINNo, strFp, Session["G1_SAEEXPRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEEXP"] = 0;
                }

                else if (command == "GSTR1EXPA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("EXPA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEEXPA"] = 0;
                }
                else if (command == "GSTR1EXPA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEEXPARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("EXPA", strGSTINNo, strFp, Session["G1_SAEEXPARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEEXPA"] = 0;
                }
                //Deleting Invoices from GSTR1 HSN
                else if (command == "GSTR1HSN_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAEHSN"] = 0;
                }
                else if (command == "GSTR1HSN_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAEHSNRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("HSN", strGSTINNo, strFp, Session["G1_SAEHSNRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAEHSN"] = 0;
                }
                //Deleting Invoices from GSTR1 NIL
                else if (command == "GSTR1NIL_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAENIL"] = 0;
                }
                else if (command == "GSTR1NIL_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAENILRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("NIL", strGSTINNo, strFp, Session["G1_SAENILRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAENIL"] = 0;
                }
                //Deleting Invoices from GSTR1 TXPD
                else if (command == "GSTR1TXP_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAETXP"] = 0;
                }
                else if (command == "GSTR1TXP_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAETXPRefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("TXP", strGSTINNo, strFp, Session["G1_SAETXPRefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAETXP"] = 0;
                }

                else if (command == "GSTR1TXPA_ER")
                {
                    objDeleteGSTR1.GSTR1DeleteALL("TXPA", strGSTINNo, strFp, strRefIds);
                    TempData["ViewResponse"] = "Selected Invoice(S) Deleted Successfully";
                    Session["dataCount_G1SAETXPA"] = 0;
                }
                else if (command == "GSTR1TXPA_ER_ALL")
                {
                    if (!string.IsNullOrEmpty(Session["G1_SAETXPARefIds"].ToString()))
                    {
                        objDeleteGSTR1.GSTR1DeleteALL("TXPA", strGSTINNo, strFp, Session["G1_SAETXPARefIds"].ToString());
                        TempData["ViewResponse"] = "ALL Invoice(S) Deleted Successfully";
                    }
                    Session["dataCount_G1SAETXPA"] = 0;
                }
                #endregion

                #region "ONCHANGE EVENT - OTP REQUEST"
                else
                {
                    ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                    SelectList Actionlst4 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", strAction);
                    ViewBag.ActionList = Actionlst4;
                    ViewBag.RecordsTypeList = LoadGSTINNo.Exist_GetGSTR_RecordsType(strRecordsType);
                    ViewBag.TitleHeaders = "GSTR-1 View - " + strRecordsType;

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
                #endregion
                
                ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                ViewBag.RecordsTypeList = LoadGSTINNo.Exist_GetGSTR_RecordsType(strRecordsType);
                if (GSTR_Type == 1)
                {
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    if (strAction != null)
                    {
                        SelectList Actionlt = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlt;
                    }
                }
                else if (GSTR_Type == 3)
                {
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1Amdnt"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    if (strAction != null)
                    {
                        SelectList Actionlt = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1Amdnt"), "ActionName", "ActionName", Convert.ToString(strAction));
                        ViewBag.ActionList = Actionlt;
                    }
                }

                SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE.Where(o => o.GSTRId != 2), "GSTRId", "GSTRName", GSTRType);
                ViewBag.GSTRList = GSTRlst;

                LoadSessionVariables();
                //LoadGSTR1ViewData(strAction, strGSTINNo, strFp);
                //Display_RetStatus();
            }
            catch (Exception ex)
            {
                ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.RecordsTypeList = LoadGSTINNo.GetGSTR_RecordsType();
                LoadSessionVariables();
                //Display_RetStatus();
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public void LoadSessionVariables()
        {
            // Uploaded Data
            Session["G1_SAUB2BRefIds"] = string.Empty;
            Session["G1_SAUB2CLRefIds"] = string.Empty;
            Session["G1_SAUB2CSRefIds"] = string.Empty;
            Session["G1_SAUCDNRRefIds"] = string.Empty;
            Session["G1_SAUCDNURRefIds"] = string.Empty;
            Session["G1_SAUEXPRefIds"] = string.Empty;
            Session["G1_SAUHSNRefIds"] = string.Empty;
            Session["G1_SAUNILRefIds"] = string.Empty;
            Session["G1_SAUTXPRefIds"] = string.Empty;
            Session["G1_SAUATRefIds"] = string.Empty;
            Session["G1_SAUDOCRefIds"] = string.Empty;

            Session["G1_SAUB2BARefIds"] = string.Empty;
            Session["G1_SAUB2CLARefIds"] = string.Empty;
            Session["G1_SAUB2CSARefIds"] = string.Empty;
            Session["G1_SAUCDNRARefIds"] = string.Empty;
            Session["G1_SAUCDNURARefIds"] = string.Empty;
            Session["G1_SAUEXPARefIds"] = string.Empty;
            Session["G1_SAUTXPARefIds"] = string.Empty;
            Session["G1_SAUATARefIds"] = string.Empty;

            Session["dataCount_G1SAUB2B"] = 0;
            Session["dataCount_GSAU1B2CL"] = 0;
            Session["dataCount_G1SAUB2CS"] = 0;
            Session["dataCount_G1SAUCDNR"] = 0;
            Session["dataCount_G1SAUCDNUR"] = 0;
            Session["dataCount_G1SAUEXP"] = 0;
            Session["dataCount_G1SAUAT"] = 0;
            Session["dataCount_G1SAUTXP"] = 0;
            Session["dataCount_G1SAUNIL"] = 0;
            Session["dataCount_G1SAUDOC"] = 0;
            Session["dataCount_G1SAUHSN"] = 0;

            Session["dataCount_G1SAUB2BA"] = 0;
            Session["dataCount_GSAU1B2CLA"] = 0;
            Session["dataCount_G1SAUB2CSA"] = 0;
            Session["dataCount_G1SAUCDNRA"] = 0;
            Session["dataCount_G1SAUCDNURA"] = 0;
            Session["dataCount_G1SAUEXPA"] = 0;
            Session["dataCount_G1SAUATA"] = 0;
            Session["dataCount_G1SAUTXPA"] = 0;
            Session["dataCount_G1SAUNILA"] = 0;
            Session["dataCount_G1SAUDOCA"] = 0;
            Session["dataCount_G1SAUHSNA"] = 0;



            // Error Records
            Session["G1_SAEB2BRefIds"] = string.Empty;
            Session["G1_SAEB2CLRefIds"] = string.Empty;
            Session["G1_SAEB2CSRefIds"] = string.Empty;
            Session["G1_SAECDNRRefIds"] = string.Empty;
            Session["G1_SAECDNURRefIds"] = string.Empty;
            Session["G1_SAEEXPRefIds"] = string.Empty;
            Session["G1_SAEHSNRefIds"] = string.Empty;
            Session["G1_SAENILRefIds"] = string.Empty;
            Session["G1_SAETXPRefIds"] = string.Empty;
            Session["G1_SAEATRefIds"] = string.Empty;
            Session["G1_SAEDOCRefIds"] = string.Empty;

            Session["G1_SAEB2BARefIds"] = string.Empty;
            Session["G1_SAEB2CLARefIds"] = string.Empty;
            Session["G1_SAEB2CSARefIds"] = string.Empty;
            Session["G1_SAECDNRARefIds"] = string.Empty;
            Session["G1_SAECDNURARefIds"] = string.Empty;
            Session["G1_SAEEXPARefIds"] = string.Empty;
            Session["G1_SAETXPARefIds"] = string.Empty;
            Session["G1_SAEATARefIds"] = string.Empty;

            Session["dataCount_G1SAEB2B"] = 0;
            Session["dataCount_G1SAEB2CL"] = 0;
            Session["dataCount_G1SAEB2CS"] = 0;
            Session["dataCount_G1SAECDNR"] = 0;
            Session["dataCount_G1SAECDNUR"] = 0;
            Session["dataCount_G1SAEEXP"] = 0;
            Session["dataCount_G1SAEAT"] = 0;
            Session["dataCount_G1SAETXP"] = 0;
            Session["dataCount_G1SAENIL"] = 0;
            Session["dataCount_G1SAEDOC"] = 0;
            Session["dataCount_G1SAEHSN"] = 0;

            Session["dataCount_G1SAEB2BA"] = 0;
            Session["dataCount_G1SAEB2CLA"] = 0;
            Session["dataCount_G1SAEB2CSA"] = 0;
            Session["dataCount_G1SAECDNRA"] = 0;
            Session["dataCount_G1SAECDNURA"] = 0;
            Session["dataCount_G1SAEEXPA"] = 0;
            Session["dataCount_G1SAEATA"] = 0;
            Session["dataCount_G1SAETXPA"] = 0;
            Session["dataCount_G1SAENILA"] = 0;
            Session["dataCount_G1SAEDOCA"] = 0;
            Session["dataCount_G1SAEHSNA"] = 0;

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
            SqlDataAdapter adt = new SqlDataAdapter("Select TOP 1 * from TBL_GSTR1_SAVE_RETSTATUS where status != '1' and customerid = '" + Session["Cust_ID"].ToString() + "' and createdby = '" + Session["User_ID"].ToString() + "' order by 1 desc", con);
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