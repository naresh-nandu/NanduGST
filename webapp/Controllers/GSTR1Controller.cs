#region Using

using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR1;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;
using WeP_BAL.GSTRDelete;
using WeP_DAL;
using WeP_DAL.GSTRDelete;
using static WeP_DAL.GSTRDelete.GstrCommonDeleteDal;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    //[System.Web.Mvc.Authorize(Roles="Active")]

    public class Gstr1Controller : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        #region "INVOICE DETAIL"
        public ActionResult InvoiceDetail()
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

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "IL"), "ActionName", "ActionName");
            ViewBag.ActionList = Actionlst;

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult InvoiceDetail(FormCollection Form, string command)
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

            string strGSTINNo = Form["ddlGSTINNo"];
            string strFp = Form["period"];
            string strAction = Form["ddlActionType"];
            ViewBag.ActionType = Form["ddlActionType"];
            string strRefIds = Form["InvIds"];
            strRefIds = strRefIds.TrimStart(',').TrimEnd(',');

            string strRefIdsOther = Form["InvIdsOther"];
            strRefIdsOther = strRefIdsOther.TrimStart(',').TrimEnd(',');

            string RefNo = Session["CustRefNo"].ToString();

            if (command == "GSTR1B2CS")
            {

                int Result = B2CsInvoiceDelete.Delete(strRefIds, strGSTINNo, strFp);
                if (Result == 1)
                {
                    TempData["Message"] = "Invoice Data Deleted Successfully";
                }
                else
                {
                    TempData["Message"] = "Something went Wrong,Please contact Administrator";
                }
            }

            if (command == "GSTR1B2CSOther")
            {

                int Result = B2CsInvoiceDelete.DeleteOther(strRefIdsOther, strGSTINNo, strFp);
                if (Result == 1)
                {
                    TempData["Message"] = "Invoice Data Deleted Successfully";
                }
                else
                {
                    TempData["Message"] = "Something went Wrong,Please contact Administrator";
                }
            }
            if (strGSTINNo != "" && strFp != "" && strAction != "")
            {
                var B2CSInv = Gstr1DataModel.GetB2CSInv(strGSTINNo, strFp, "");
                ViewBag.B2CSInv = B2CSInv;

                var B2CSInvOther = Gstr1DataModel.B2CSInvOther(strGSTINNo, strFp, RefNo);
                ViewBag.B2CSInvOther = B2CSInvOther;
            }
            ViewBag.Period = strFp;
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "IL"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
            return View();
        }
        #endregion


        #region "GSTR1 SAVE"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Save()
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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR1 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

            ViewBag.GSTRList = LoadDropDowns.GetGSTRList("GSTR1,GSTR1 AMENDMENT");

            ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");

            ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList("NOT UPLOADED TO GSTN", "GSTR1");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Save(FormCollection frm)
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
            string GSTR1Save = "", GetJson = "", OTPSubmit = "", command = "", btnHSNSummary = "";

            GSTR1Save = frm["GSTR1Save"];
            GetJson = frm["GetJson"];
            OTPSubmit = frm["OTPSubmit"];
            command = frm["command"];
            btnHSNSummary = frm["btnHSNSummary"];

            try
            {
                decimal WalletBalance = 0;
                string GSK = "";
                bool isChecked = false;
                string strGSTINNo = "", strFp = "", strFlag = "", strAction = "", strOTP = "", strAUTH_GSTIN = "", strGSTRType = "", strExportAction = "", strRecordType = "";

                strRecordType = frm["ddlRecordType"] == null ? "" : frm["ddlRecordType"];
                strGSTINNo = frm["ddlGSTINNo"] == null ? "" : frm["ddlGSTINNo"];
                strFp = frm["period"] == null ? "" : frm["period"];
                strOTP = frm["OTP"] == null ? "" : frm["OTP"];
                strAUTH_GSTIN = frm["AUTH_GSTINNo"] == null ? "" : frm["AUTH_GSTINNo"];
                strGSTRType = frm["ddlGSTR"] == null ? "" : frm["ddlGSTR"];
                strAction = frm["ddlActionType"] == null ? "" : frm["ddlActionType"];
                strExportAction = frm["strExportAction"] == null ? "" : frm["strExportAction"];
                int iBatchCount = Convert.ToInt32(ConfigurationManager.AppSettings["GSTR1_BATCH"]);
                ViewBag.GSTRType = strGSTRType;

                if (strRecordType == "NOT UPLOADED TO GSTN")
                {
                    strFlag = "";
                }
                if (strRecordType == "GSTN ERROR RECORDS")
                {
                    strFlag = "1";
                }

                Session["GSTRType"] = strGSTRType;
                Session["Action"] = strAction;

                TempData["ActionType"] = strAction;
                TempData["GSTINNo"] = strGSTINNo;
                TempData["period"] = strFp;
                ViewBag.Period = strFp;
                ViewBag.ActionType = strAction;

                ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList(strRecordType, "GSTR1");

                #region "Dropdown loads"
                ViewBag.ActionList = LoadDropDowns.GetGSTRActionList(strGSTRType);
                if (!string.IsNullOrEmpty(strAction))
                {
                    ViewBag.ActionList = LoadDropDowns.Exist_GetGSTRActionList(strGSTRType, strAction);
                }
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                ViewBag.GSTRList = LoadDropDowns.Exist_GetGSTRList("GSTR1,GSTR1 AMENDMENT", strGSTRType);
                #endregion

                #region "GSTR1 SAVE BATCH PROCESSING"
                if (!string.IsNullOrEmpty(GSTR1Save))
                {
                    if (strGSTINNo == "Select" || strGSTINNo == "")
                    {
                        TempData["SaveResponse"] = "Please Select GSTIN";
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");
                    }
                    else if (strAction == "Select Action" || strAction == "")
                    {
                        TempData["SaveResponse"] = "Please Select Action Type";
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");
                    }
                    else
                    {
                        #region "WALLET CHECK"
                        Helper.GetWalletBalance(Session["Cust_ID"].ToString(), "PACK_RETURNFILING", "GSTR1", strGSTINNo, strFp, out WalletBalance, out GSK, out isChecked);
                        if (GSK == "True")
                        {
                            if (isChecked)
                            {
                                //
                            }
                            else
                            {
                                if (WalletBalance < 50)
                                {
                                    SelectList GSTRlst2 = new SelectList(db.TBL_GSTR_TYPE.Where(o => o.GSTRId != 2), "GSTRId", "GSTRName");
                                    ViewBag.GSTRList = GSTRlst2;

                                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                                    ViewBag.ActionList = Actionlst;
                                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                                    TempData["ErrorMessage"] = "Your Wallet Balance is Low. Please do recharge for Filing";
                                }
                            }
                        }
                        #endregion

                        #region "GSTR-1 BATCH PROCESSING"

                        DataTable batch_dt = new DataTable();
                        using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            if (sqlcon.State == ConnectionState.Closed)
                            {
                                sqlcon.Open();
                            }
                            using (SqlCommand sqlcmd = new SqlCommand())
                            {
                                sqlcmd.Connection = sqlcon;
                                sqlcmd.CommandText = "usp_Construct_JSON_GSTR1_RETSAVE";
                                sqlcmd.CommandType = CommandType.StoredProcedure;
                                sqlcmd.CommandTimeout = 0;
                                sqlcmd.Parameters.AddWithValue("@Gstin", strGSTINNo);
                                sqlcmd.Parameters.AddWithValue("@Fp", strFp);
                                sqlcmd.Parameters.AddWithValue("@Flag", strFlag);
                                sqlcmd.Parameters.AddWithValue("@ActionType", strAction);
                                sqlcmd.Parameters.AddWithValue("@rangeDefault", iBatchCount);
                                using (SqlDataAdapter p_adt = new SqlDataAdapter(sqlcmd))
                                {
                                    p_adt.Fill(batch_dt);
                                    if (batch_dt.Rows.Count > 0)
                                    {
                                        for (int i = 0; i < batch_dt.Rows.Count; i++)
                                        {
                                            string strMinInvId = batch_dt.Rows[i]["MinInvId"].ToString();
                                            string strMaxInvId = batch_dt.Rows[i]["MaxInvId"].ToString();
                                            string strActionType = batch_dt.Rows[i]["ActionType"].ToString();
                                            BatchProcessing(strGSTINNo, strFp, strMinInvId, strMaxInvId, strFlag, strActionType,
                                                Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                                            //Thread.Sleep(1000);
                                            //Task.Factory.StartNew(() => BatchProcessing(strGSTINNo, strFp, strMinInvId, strMaxInvId, "", strAction,
                                            //    Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString()));
                                        }
                                        TempData["SaveResponse"] = "GSTR1 Save is in progress... Please check after sometime in AuditLog Report for Reference Id";
                                    }
                                }
                            }
                            sqlcon.Close();
                        }

                        #endregion

                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());
                        ViewBag.ActionList = LoadDropDowns.GetGSTRActionList(strGSTRType);
                        if (!string.IsNullOrEmpty(strAction))
                        {
                            ViewBag.ActionList = LoadDropDowns.Exist_GetGSTRActionList(strGSTRType, strAction);
                        }
                        ViewBag.GSTRList = LoadDropDowns.Exist_GetGSTRList("GSTR1,GSTR1 AMENDMENT", strGSTRType);
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
                        ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");
                    }
                    else if (strAction == "Select Action" || strAction == "")
                    {
                        TempData["SaveResponse"] = "Please Select Action Type";
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");
                    }
                    else
                    {
                        string strJsonData = Gstr1DataModel.GetJsonGSTR1Save(strGSTINNo, strFp, strAction, strFlag);
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                        ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                        ViewBag.GET_GSTR1JsonResponse = strJsonData;
                    }
                }
                #endregion

                #region "GENERATE HSN SUMMARY"
                else if (!string.IsNullOrEmpty(btnHSNSummary))
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                    ViewBag.TitleHeaders = "GSTR-1 Save";

                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Connection = conn;
                            cmd.CommandType = CommandType.StoredProcedure;
                            cmd.CommandTimeout = 0;
                            cmd.CommandText = "USP_EXT_GSTR1_HSN_Auto_Generation";
                            cmd.Parameters.Add("@Gstin", SqlDbType.VarChar).Value = strGSTINNo;
                            cmd.Parameters.Add("@FP", SqlDbType.VarChar).Value = strFp;
                            cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = iCustId;
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = iUserId;
                            cmd.Parameters.Add("@ReferenceNo", SqlDbType.VarChar).Value = Session["CustRefNo"].ToString();
                            cmd.ExecuteNonQuery();
                        }
                        conn.Close();
                    }
                    TempData["SaveResponse"] = "HSN Summary Generated Successfully...";
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
                        TempData["ErrorMessage"] = status;
                    }

                }
                #endregion

                #region "OTP REQUEST"
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTINNo), Session["Role_Name"].ToString());
                    ViewBag.TitleHeaders = "GSTR-1 Save";

                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTINNo;
                }
                #endregion

                #region "RETRIEVE GSTR1 SUMMARY"
                GstrDeleteViewModel Model = new GstrDeleteViewModel();
                if (!string.IsNullOrEmpty(strAction) && !string.IsNullOrEmpty(strFp) && !string.IsNullOrEmpty(strGSTINNo) && !string.IsNullOrEmpty(strGSTRType))
                {
                    DataSet GSTR1Summary = new GstrCommonDeleteBal(iCustId, iUserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary(strGSTRType, strAction, strGSTINNo, strFp, strRecordType, "");
                    TempData["GSTR1Summary"] = GSTR1Summary;
                    List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes> GSTR1SummaryMgmt = new List<GstrCommonDeleteDal.GstrDeleteSummaryAttributes>();

                    if (GSTR1Summary.Tables.Count > 0)
                    {
                        foreach (DataRow dr in GSTR1Summary.Tables[0].Rows)
                        {
                            GSTR1SummaryMgmt.Add(new GstrCommonDeleteDal.GstrDeleteSummaryAttributes
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

                    Model.GSTR1Summary = GSTR1SummaryMgmt;
                    ViewBag.GSTRSummary = GSTR1SummaryMgmt.Count;

                    #region "Export Raw Data"
                    if (command == "exportrawData")
                    {
                        DataSet GstrSummaryraw = new GstrCommonDeleteBal(iCustId, iUserId).Retrieve_GSTR1_Delete_EXT_SA_Bulk_Rawdata(strGSTRType, strExportAction, strGSTINNo, strFp, strRecordType, "");
                        CommonFunctions.ExportExcel_XLSX(GstrSummaryraw, strGSTRType + "SummaryRawData_" + strGSTINNo + "_" + strFp + "_" + strExportAction + ".xlsx");
                    }
                    #endregion

                }
                #endregion

                return Json(new { success = true, data = Model, SuccessMessage = TempData["SaveResponse"], ErrorMessage = TempData["ErrorMessage"],
                                    OTPSession = ViewBag.OTPSession, AUTH_Response = ViewBag.AUTH_Response, AUTH_GSTINNo = ViewBag.AUTH_GSTINNo,
                                    GetJsonSession = ViewBag.GetJsonSession, GET_GSTR1JsonResponse = ViewBag.GET_GSTR1JsonResponse }, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                ViewBag.GSTRList = LoadDropDowns.GetGSTRList("GSTR1,GSTR1 AMENDMENT");
                ViewBag.ActionList = LoadDropDowns.GetGSTRActionList("GSTR1");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                ViewBag.RecordTypeList = LoadDropDowns.Exist_GetRecordTypeList("NOT UPLOADED TO GSTN", "GSTR1");
                TempData["ErrorMessage"] = ex.Message;

                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }


        public void BatchProcessing(string strGSTINNo, string strFp, string strMinInvId, string strMaxInvId, string strFlag, string strAction,
            string strUserId, string strCustId, string strUserName)
        {
            DataTable batch_dt1 = new DataTable();
            using (SqlConnection sqlcon1 = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (sqlcon1.State == ConnectionState.Closed)
                {
                    sqlcon1.Open();
                }
                using (SqlCommand sqlcmd1 = new SqlCommand())
                {
                    sqlcmd1.Connection = sqlcon1;
                    sqlcmd1.CommandText = "usp_Construct_JSON_GSTR1_RETSAVE_BATCH";
                    sqlcmd1.CommandType = CommandType.StoredProcedure;
                    sqlcmd1.CommandTimeout = 0;
                    sqlcmd1.Parameters.AddWithValue("@Gstin", strGSTINNo);
                    sqlcmd1.Parameters.AddWithValue("@Fp", strFp);
                    sqlcmd1.Parameters.AddWithValue("@MinInvId", strMinInvId);
                    sqlcmd1.Parameters.AddWithValue("@MaxInvId", strMaxInvId);
                    sqlcmd1.Parameters.AddWithValue("@Flag", strFlag);
                    sqlcmd1.Parameters.AddWithValue("@ActionType", strAction);
                    using (SqlDataAdapter p_adt1 = new SqlDataAdapter(sqlcmd1))
                    {
                        p_adt1.Fill(batch_dt1);
                        StringBuilder MyStringBuilder = new StringBuilder();
                        var reader = sqlcmd1.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                MyStringBuilder.Append(reader.GetValue(0));
                            }
                        }
                        reader.Close();
                        var returnJson = MyStringBuilder.ToString();
                        returnJson = returnJson.TrimStart('[').TrimEnd(']');

                        new GspSendGstr1SaveDataModel(strCustId, strUserId, strUserName).SendRequestVoid(returnJson, strAction, strGSTINNo, strFp, strMinInvId, strMaxInvId);
                    }
                }
                sqlcon1.Close();
            }
        }
        
        #endregion


        #region "GSTR1 FILE"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult File()
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
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR1 File Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            ViewBag.DSCList = LoadDropDowns.GetDSC_Certificates();
            ViewBag.FileTypeList = LoadDropDowns.GetFilingType();
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult File(FormCollection frm, string ViewSummary, string ESign, string btnDSC, string btnEVC, string GSTR1File,
            string GetJson, string OTPSubmit, string GSTR1Submit, string ExportSummary)
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
            string OutFileStatus = "", OutFileResponse = "";
            try
            {
                string strFp = frm["period"];
                string strGSTINNo = frm["ddlGSTINNo"];
                string strFileType = frm["ddlFileType"];
                string strDSC = "";
                string strPanNo = frm["panno"];
                string strEVCOTP = frm["evcotp"];
                string flag = "1";

                ViewBag.Period = strFp;
                ViewBag.PANNo = strPanNo;
                ViewBag.TitleHeaders = "GSTR-1 Submit & File";
                ViewBag.DSCList = LoadDropDowns.GetDSC_Certificates();
                ViewBag.FileTypeList = LoadDropDowns.Exist_GetFilingType(strFileType);
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTINNo, Session["Role_Name"].ToString());

                #region "GSTR-1 VIEW SUMMARY"
                if (!string.IsNullOrEmpty(ViewSummary))
                {
                    if (string.IsNullOrEmpty(strGSTINNo))
                    {
                        TempData["ErrorMessage"] = "Please Select GSTIN No";
                    }
                    else if (string.IsNullOrEmpty(strFp))
                    {
                        TempData["ErrorMessage"] = "Please Select Period";
                    }
                    else
                    {
                        GspGetGstr1DataModel GSP_getGSTR1 = new GspGetGstr1DataModel();
                        string DownloadRetSum = GSP_getGSTR1.SendRequest(strGSTINNo, strFp, "RETSUM", "", "", "", "", "", "", Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), flag);
                        TempData["GSTR1Response"] = DownloadRetSum;
                    }
                }
                #endregion

                #region "Export GSTR-1 Summary"
                if (!string.IsNullOrEmpty(ExportSummary))
                {
                    GridView gv = new GridView();
                    gv.DataSource = Gstr1DataModel.GetDataTableListGSTR1Summary(strGSTINNo, strFp);
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR1_Summary_" + strGSTINNo + "_" + strFp + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return View();
                }
                #endregion

                #region "GSTR-1 SUBMIT"
                else if (!string.IsNullOrEmpty(GSTR1Submit))
                {
                    if (string.IsNullOrEmpty(strGSTINNo))
                    {
                        TempData["ErrorMessage"] = "Please Select GSTIN No";
                    }
                    else if (string.IsNullOrEmpty(strFp))
                    {
                        TempData["ErrorMessage"] = "Please Select Period";
                    }
                    else
                    {                        
                        string strJsonData = "{ \"gstin\" : \"" + strGSTINNo + "\", \"ret_period\" : \"" + strFp + "\" }";
                        string SubmitResponse = new GspSendGstr1SubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).GSTR1SubmitRequest();

                        TempData["GSTR1Response"] = SubmitResponse;
                    }
                }
                #endregion

                #region "GSTR-1 FILE"
                else if (!string.IsNullOrEmpty(GSTR1File))
                {
                    if (string.IsNullOrEmpty(strPanNo))
                    {
                        TempData["ErrorMessage"] = "Please Enter PAN NO for GSTR1 Filing";
                    }
                    else if (string.IsNullOrEmpty(strFileType))
                    {
                        TempData["ErrorMessage"] = "Please Select FILING Type for GSTR1 Filing";
                    }
                    else
                    {
                        string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR1_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                        if (System.IO.File.Exists(filingpath))
                        {
                            if (strFileType == "DSC")
                            {
                                string strJsonData = "";
                                if (System.IO.File.Exists(filingpath))
                                {
                                    using (StreamReader r = new StreamReader(filingpath))
                                    {
                                        strJsonData = r.ReadToEnd();
                                    }
                                }
                                string Base64JsonPayload = Helper.Base64Encode(strJsonData.Trim());
                                string strHashValue = "";
                                using (var sha256Obj = SHA256.Create())
                                {
                                    byte[] outputBytes = sha256Obj.ComputeHash(Encoding.ASCII.GetBytes(Base64JsonPayload));
                                    strHashValue = BitConverter.ToString(outputBytes).Replace("-", "").ToLower().Trim();
                                }
                                ViewBag.HashValue = strHashValue;
                                ViewBag.DSCDialog = "OPEN_DSCPOPUP";
                            }
                            else if (strFileType == "EVC")
                            {
                                if (!string.IsNullOrEmpty(strPanNo) && strPanNo.Length == 10)
                                {
                                    ViewBag.EVCAuthResponse = Models.GSTAUTH.GspGstEvcOtp.SendRequest(strGSTINNo, strPanNo, "R1", Session["User_ID"].ToString(), Session["Cust_ID"].ToString());
                                }

                                ViewBag.EVCDialog = "OPEN_EVCPOPUP";
                            }
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please do GSTR1 RETSUM Download for GSTR1 Filing";
                        }
                    }
                }
                #endregion

                #region "GET JSON PAYLOAD"
                else if (!string.IsNullOrEmpty(GetJson))
                {
                    if (strGSTINNo == "Select" || strGSTINNo == "")
                    {
                        TempData["GSTR1Response"] = "Please Select GSTIN";
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                        SelectList Actionlst0 = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "GSTR1"), "ActionName", "ActionName");
                        ViewBag.ActionList = Actionlst0;
                        return View();
                    }
                    else
                    {
                        string strJsonData = Gstr1DataModel.GetJsonGSTR1File(strGSTINNo, strFp);
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');
                        ViewBag.GetJsonSession = "OPEN_JSON_PAYLOAD";
                        ViewBag.GET_GSTR1JsonResponse = strJsonData;
                    }
                }
                #endregion

                #region "DSC REQUEST"
                else if (!string.IsNullOrEmpty(btnDSC))
                {
                    string strJsonData = "";
                    string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR1_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                    if (System.IO.File.Exists(filingpath))
                    {
                        using (StreamReader r = new StreamReader(filingpath))
                        {
                            strJsonData = r.ReadToEnd();
                        }
                        strDSC = frm["outputsigneddata"];

                        new GspSendGstr1SubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), 
                                Session["UserName"].ToString()).GSTR1FileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);

                        if (OutFileStatus == "0")
                        {
                            new GspSendGstr1SubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                Session["UserName"].ToString()).GSTR1FileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);
                        }
                        else if (OutFileStatus == "1")
                        {
                            TempData["GSTR1Response"] = OutFileResponse;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = OutFileResponse;
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please do GSTR1 RETSUM Download for GSTR1 Filing";
                    }
                }
                #endregion

                #region "EVC REQUEST"
                else if (!string.IsNullOrEmpty(btnEVC))
                {
                    string strJsonData = "";
                    string filingpath = Path.Combine(Server.MapPath("~/App_Data/ReturnFiling"), "GSTR1_RETSUM_" + strGSTINNo + "_" + strFp + ".json");
                    if (System.IO.File.Exists(filingpath))
                    {
                        using (StreamReader r = new StreamReader(filingpath))
                        {
                            strJsonData = r.ReadToEnd();
                        }

                        new GspSendGstr1SubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                Session["UserName"].ToString()).GSTR1FileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);

                        if (OutFileStatus == "0")
                        {
                            new GspSendGstr1SubmitFileDataModel(strJsonData, strGSTINNo, strFp, Session["Cust_ID"].ToString(), Session["User_ID"].ToString(),
                                Session["UserName"].ToString()).GSTR1FileRequest(strFileType, strDSC, strPanNo, strEVCOTP, out OutFileStatus, out OutFileResponse);
                        }
                        else if (OutFileStatus == "1")
                        {
                            TempData["GSTR1Response"] = OutFileResponse;
                        }
                        else
                        {
                            TempData["ErrorMessage"] = OutFileResponse;
                        }
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please do GSTR1 RETSUM Download for GSTR1 Filing";
                    }
                }
                #endregion

                #region "ESIGN REQUEST"
                else if (!string.IsNullOrEmpty(ESign))
                {
                    //
                }
                #endregion

                #region "OTP SUBMIT"
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

                #region "ONCHANGE EVENT"
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTINNo;
                }
                #endregion

                ViewBag.GSTR1File = Gstr1DataModel.GetListGSTR1File(strGSTINNo, strFp);
            }
            catch (Exception ex)
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        #endregion
    }
}