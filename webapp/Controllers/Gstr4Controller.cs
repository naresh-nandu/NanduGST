using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR4;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Xml;
using WeP_BAL.Common;
using WeP_BAL.GSTRDelete;
using WeP_DAL.GSTRDelete;
using static WeP_DAL.GSTRDelete.GstrCommonDeleteDal;

namespace SmartAdminMvc.Controllers
{
    public class Gstr4Controller : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        #region "GSTR4 Save"
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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR4 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

            ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR4");

            ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList("NOT UPLOADED TO GSTN", "GSTR1");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm, string GSTR4Save, string GetJson, string OTPSubmit, string command, string[] RefIds)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string strInvIds = "", strExportAction = "", strRecordType = "", strFlag = "";
            string strGSTINNo = frm["ddlGSTINNo"];
            string strFp = DateTime.Now.ToString(frm["period"]);
            string strAction = frm["ddlActionType"];
            strRecordType = frm["ddlRecordType"];
            TempData["ActionType"] = strAction;
            TempData["GSTINNo"] = strGSTINNo;
            TempData["period"] = strFp;
            strExportAction = frm["strExportAction"];

            if (strRecordType == "NOT UPLOADED TO GSTN")
            {
                strFlag = "";
            }
            if (strRecordType == "GSTN ERROR RECORDS")
            {
                strFlag = "1";
            }

            ViewBag.Period = strFp;
            ViewBag.ActionType = strAction;

            ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList(strRecordType, "GSTR4");

            #region "GSTR4 SAVE"
            if (!string.IsNullOrEmpty(GSTR4Save))
            {
                if (strGSTINNo == "Select" || strGSTINNo == "")
                {
                    TempData["SaveResponse"] = "Please Select GSTIN";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr4SaveDataModel fnGSTR4Save = new GspSendGstr4SaveDataModel();
                    string strJsonData = Gstr4DataModel.GetJsonGSTR4Save(strGSTINNo, strFp, strAction, strFlag);
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    string SaveResponse = fnGSTR4Save.SendRequest(strJsonData, strAction, strGSTINNo, strFp, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());

                    TempData["SaveResponse"] = SaveResponse;

                    SelectList Actionlst1 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionName", "ActionName");
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
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else if (strAction == "Select Action" || strAction == "")
                {
                    TempData["SaveResponse"] = "Please Select Action Type";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst0;
                    return View();
                }
                else
                {
                    GspSendGstr4SaveDataModel fnGSTR4Save = new GspSendGstr4SaveDataModel();
                    string strJsonData = Gstr4DataModel.GetJsonGSTR4Save(strGSTINNo, strFp, strAction, "");
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                    ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                    ViewBag.GET_GSTR4JsonResponse = strJsonData;
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
            
            #region "ONCHANGE EVENT - OTP REQUEST"
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());

                ViewBag.TitleHeaders = "GSTR-4 Save";

                string OTPPOPUPValue = "", OTPAUTHResponse = "";
                Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                ViewBag.OTPSession = OTPPOPUPValue;
                ViewBag.AUTH_Response = OTPAUTHResponse;
                ViewBag.AUTH_GSTINNo = strGSTINNo;
            }
            #endregion

            #region "RETRIEVE GSTR1 SUMMARY"
            GstrDeleteViewModel Model = new GstrDeleteViewModel();
            if (!string.IsNullOrEmpty(strAction) && !string.IsNullOrEmpty(strFp) && !string.IsNullOrEmpty(strGSTINNo))
            {
                DataSet GSTR4Summary = new GstrCommonDeleteBal(iCustId, iUserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary("GSTR4", strAction, strGSTINNo, strFp, strRecordType, "");
                TempData["GSTR4Summary"] = GSTR4Summary;
                List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes> GSTR4SummaryMgmt = new List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes>();

                if (GSTR4Summary.Tables.Count > 0)
                {
                    foreach (DataRow dr in GSTR4Summary.Tables[0].Rows)
                    {
                        GSTR4SummaryMgmt.Add(new GstrCommonDeleteDal.GstrDeleteSummaryAttributes
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

                Model.GSTR4Summary = GSTR4SummaryMgmt;
                ViewBag.GSTR4Summary = GSTR4SummaryMgmt.Count;

                #region "Export Raw Data"
                if (command == "exportrawData")
                {
                    DataSet GstrSummaryraw = new GstrCommonDeleteBal(iCustId, iUserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Rawdata("GSTR4", strExportAction, strGSTINNo, strFp, strRecordType, "");
                    CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, "GSTR4" + "SummaryRawData_" + strGSTINNo + "_" + strFp + "_" + strExportAction + ".xlsx");
                }
                #endregion

            }
            #endregion
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            ViewBag.ActionList = LoadDropDowns.Exist_GetGSTRActionList("GSTR4", strAction);
            ViewBag.TitleHeaders = "GSTR-4 Save ";
            return View(Model);
        }

        #endregion


        #region "GSTR4 File"

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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR4 File Page.";
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
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionId", "ActionName", Convert.ToString(strActionId));
                ViewBag.ActionList = Actionlst;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

                var GridFill = Gstr4DataModel.GetListGSTR4File(strGSTINNo, strAction, strFp);
                return View(GridFill);
            }
            else
            {
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR4"), "ActionId", "ActionName");
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

                return View();
            }
        }

        //[System.Web.Mvc.HttpPost]
        //[System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        //public ActionResult File(FormCollection frm, string ESign, string GSTR4File, string OTPSubmit)
        //{
        //    if (!Request.IsAuthenticated)
        //    {
        //        return RedirectToAction("Login", "Account");
        //    }
        //    if (Session["User_ID"] == null)
        //        return RedirectToAction("Login", "Account");

        //    int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
        //    int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
        //    int strGSTINId = Convert.ToInt32(frm["ddlGSTINNo"]);
        //    int strActionId = Convert.ToInt32(frm["ddlActionType"]);

        //    string strGSTINNo = (from lst in db.TBL_Cust_GSTIN
        //                         where lst.GSTINId == strGSTINId
        //                         select lst.GSTINNo).SingleOrDefault();

        //    string strAction = (from lst1 in db.TBL_GSTR_ACTION_TYPE
        //                        where lst1.ActionId == strActionId
        //                        select lst1.ActionName).SingleOrDefault();

        //    string strFp = DateTime.Now.ToString("MMyyyy");

        //    TempData["ActionId"] = Convert.ToString(strActionId);
        //    TempData["GSTINId"] = Convert.ToString(strGSTINId);
        //    TempData["ActionType"] = strAction;
        //    TempData["GSTINNo"] = strGSTINNo;
        //    TempData["Period"] = strFp;

        //    if (!string.IsNullOrEmpty(GSTR4File))
        //    {
        //        string strJsonData = Gstr4DataModel.GetJsonGSTR4File(strAction, strGSTINNo, strFp);

        //        string guid = Guid.NewGuid().ToString();
        //        guid = guid.Replace("-", "").ToUpper();

        //        Models.ESign.OldTCUtilsBuffer tcutil = new Models.ESign.OldTCUtilsBuffer();
        //        string fileName = Path.GetFileName("sample.txt");
        //        string path = Path.Combine(Server.MapPath("~\\App_Data\\ESign\\"), fileName);
        //        path = path.Replace("\\", "/");

        //        Task<string> ResStatus = tcutil.uploadBuffer(guid, "mgrdsfa@gmail.com", path);

        //        string cs = tcutil.GetCS(guid);
        //        XmlDocument xmlDoc = new XmlDocument();
        //        xmlDoc.LoadXml(ResStatus.Result.ToString());

        //        string status = "";
        //        XmlDocument xmlDocument1 = new XmlDocument();
        //        xmlDocument1.LoadXml(ResStatus.Result.ToString());
        //        foreach (XmlNode xmlNode in xmlDocument1.SelectNodes("response/child::node()"))
        //        {
        //            if (xmlNode.Name == "status")
        //                status = xmlNode.InnerText;
        //        }

        //        if (status == "0")
        //        {
        //            ViewBag.ESIGNDialog = "OPEN_ESIGNPOPUP";
        //            TempData["ESignPopup"] = "renderWidget('widgetdiv','https://wepindia.truecopy.in/corp/v2/widgetsigner.tc?uuid=" + guid + "&cs=" + cs + "&fn=WeP');";
        //        }
        //        else
        //        {
        //            ViewBag.ESIGNDialog = "CLOSE_ESIGNPOPUP";
        //        }

        //        var GridFill = Gstr4DataModel.GetListGSTR4File(strGSTINNo, strAction, strFp);
        //        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName", Convert.ToString(strActionId));
        //        ViewBag.ActionList = Actionlst;

        //        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());

        //        return View(GridFill);
        //    }
        //    else if (!string.IsNullOrEmpty(ESign))
        //    {

        //        TempData["FileResponse"] = "GSTR4 Filed Successfully... Status - SUCCESS and Ack No. - ASDFSDF1241343";

        //        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName");
        //        ViewBag.ActionList = Actionlst;

        //        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());

        //        return View();
        //    }
        //    else if (!string.IsNullOrEmpty(OTPSubmit))
        //    {
        //        string strOTP = frm["OTP"].ToString();
        //        Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, "");
        //        string status = Models.GSTAUTH.GSP_GSTAuthwithOTP.SendRequest(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
        //        if (status == "1")
        //        {
        //            TempData["AuthMsg"] = "Authenticated Successfully";
        //        }
        //        else
        //        {
        //            TempData["AuthMsg"] = "Authentication Error";
        //        }

        //        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName");
        //        ViewBag.ActionList = Actionlst;

        //        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No_Based_On_Id(iUserId, iCustId, Session["Role_Name"].ToString());
        //        return View();
        //    }
        //    else
        //    {
        //        ViewBag.TitleHeaders = "GSTR-4 File - [" + strAction + "]";
        //        var GridFill = Gstr4DataModel.GetListGSTR4File(strGSTINNo, strAction, strFp);

        //        SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR6"), "ActionId", "ActionName", Convert.ToString(strActionId));
        //        ViewBag.ActionList = Actionlst;

        //        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No_Based_On_Id(iUserId, iCustId, Convert.ToString(strGSTINId), Session["Role_Name"].ToString());
        //        return View(GridFill);
        //    }
        //}

        #endregion


    }
}