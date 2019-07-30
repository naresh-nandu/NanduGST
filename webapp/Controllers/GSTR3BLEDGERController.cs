using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR3;
using SmartAdminMvc.Models.Ledger;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
namespace SmartAdminMvc.Controllers
{
    public class Gstr3BLedgerController : Controller
    {
        readonly JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        #region "Ledger and ITC"
        [HttpGet]
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                //
            }
            catch(Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            finally
            {
                ViewBag.strPeriod= DateTime.Now.ToString("MMyyyy");
                ViewBag.fromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.toDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "LEDGER"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlst;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection Form,string Command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            
            string strGstin = "",fromDate = "",toDate = "",strAction = "", strPeriod;

            try
            {
                strGstin = Form["strGstin"];
                fromDate = Form["fromDate"];
                toDate = Form["toDate"];
                strAction = Form["strAction"];
                strPeriod = Form["strPeriod"];
                if (string.IsNullOrEmpty(fromDate))
                {
                    ViewBag.fromDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.fromDate = fromDate;
                }
                if (string.IsNullOrEmpty(toDate))
                {
                    ViewBag.toDate = DateTime.Now.ToString("dd/MM/yyyy");
                }
                else
                {
                    ViewBag.toDate = toDate;
                }
                if (string.IsNullOrEmpty(strPeriod))
                {
                    ViewBag.strPeriod= DateTime.Now.ToString("MMyyyy");
                }
                else
                {
                    ViewBag.strPeriod = strPeriod;
                }
                switch (Command)
                {
                    case "Download":
                        Gstr3BLedgerDataModel Obj = new Gstr3BLedgerDataModel();
                        var result = Obj.SendRequest_Ledger(strGstin, strPeriod, strAction, fromDate, toDate, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                        TempData["SuccessMessage"] = result;
                        break;
                    case "OTPSubmit":
                        string strOTP = Form["OTP"].ToString();
                        Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP,strGstin);
                        string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGstin, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                        if (status == "1")
                        {
                            TempData["AuthMsg"] = "Authenticated Successfully";
                        }
                        else
                        {
                            TempData["AuthMsg"] = status;
                        }
                        break;
                    default:
                        string OTPPOPUPValue = "", OTPAUTHResponse = "";
                        Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGstin, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);

                        ViewBag.OTPSession = OTPPOPUPValue;
                        ViewBag.AUTH_Response = OTPAUTHResponse;
                        ViewBag.AUTH_GSTINNo = strGstin;
                        break;
                }
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            finally
            {
                strAction = Form["strAction"];
                strGstin = Form["strgstin"];
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(Convert.ToInt32(Session["User_ID"]), Convert.ToInt32(Session["Cust_ID"]),strGstin, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "LEDGER"), "ActionName", "ActionName", strAction);
                ViewBag.ActionList = Actionlst;
            }
            return View();
        }

        #endregion

        // GET: GSTR3BLEDGER
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
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
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string offset, string GetLedger, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            string strGSTIN = "", strPeriod = "", strRefId = "";
            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];

                strPeriod = frm["period"];

                strRefId = frm["refid"];
                ViewBag.Period = strPeriod;

                if (!string.IsNullOrEmpty(offset))
                {
                    var ledgerSummary = (LedgerSummary)TempData["LedgerSummary"];
                   
                    var liabilityOutParameter = 0;
                    var itcOutParameter = 0;
                    var cashOutParameter = 0;

                    var offsetITCParameter = 0;
                    var offsetCashParameter = 0;

                    OffsetLiabilityGstr3B offsetLiabilityGSTR3B = new OffsetLiabilityGstr3B();
                    Pdcash pdcash = new Pdcash();
                    pdcash.liab_ldg_id = 1233;
                    pdcash.trans_typ = 3002;
                    Pditc dditc = new Pditc();
                    

                    //CGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    cashOutParameter = 0;
                    OffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.cgstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.cgstTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.cgst_bal
                                                          , ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal
                                                          , out offsetITCParameter
                                                          , out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          , out cashOutParameter);

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.c_pdc = offsetITCParameter;
                    pdcash.cpd = offsetCashParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.cgstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.cgst_bal = itcOutParameter;
                    ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal = cashOutParameter;

                    //SGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    cashOutParameter = 0;
                    offsetITCParameter = 0;
                    offsetCashParameter = 0;
                    OffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.sgstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.sgstTaxBal
                                                          ,ledgerSummary.CashITCBalance.itc_bal.sgst_bal
                                                          , ledgerSummary.CashITCBalance.cash_bal.sgst_tot_bal
                                                          , out offsetITCParameter
                                                          , out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          , out cashOutParameter);

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.s_pds = offsetITCParameter;
                    pdcash.spd = offsetCashParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.sgstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.sgst_bal = itcOutParameter;
                    ledgerSummary.CashITCBalance.cash_bal.sgst_tot_bal = cashOutParameter;

                    //IGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    cashOutParameter = 0;
                    offsetITCParameter = 0;
                    offsetCashParameter = 0;
                    OffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.igstTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.igst_bal
                                                          , ledgerSummary.CashITCBalance.cash_bal.igst_tot_bal
                                                          , out offsetITCParameter
                                                          , out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          , out cashOutParameter);

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.i_pdi = offsetITCParameter;
                    pdcash.ipd = offsetCashParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.igst_bal = itcOutParameter;
                    ledgerSummary.CashITCBalance.cash_bal.igst_tot_bal = cashOutParameter;

                    //CESS
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    cashOutParameter = 0;
                    offsetITCParameter = 0;
                    offsetCashParameter = 0;
                    OffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.cessbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.cessTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.cess_bal
                                                          , ledgerSummary.CashITCBalance.cash_bal.cess_tot_bal
                                                          , out offsetITCParameter
                                                          , out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          , out cashOutParameter);

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.cs_pdcs = offsetITCParameter;
                    pdcash.cspd = offsetCashParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.cessbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.cess_bal = itcOutParameter;
                    ledgerSummary.CashITCBalance.cash_bal.cess_tot_bal = cashOutParameter;

                    //CROSS CGST ADJUSTMENT using ITC IGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    offsetITCParameter = 0;
                    CrossOffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.cgstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.igstTaxBal
                                                          ,ledgerSummary.CashITCBalance.itc_bal.igst_bal
                                                          //, ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal
                                                          , out offsetITCParameter
                                                          //, out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          //, out cashOutParameter
                                                          );

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.c_pdi = offsetITCParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.cgstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.igst_bal = itcOutParameter;

                    //CROSS SGST ADJUSTMENT using ITC IGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    offsetITCParameter = 0;
                    CrossOffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.sgstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.sgstTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.sgst_bal
                                                          //, ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal
                                                          , out offsetITCParameter
                                                          //, out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          //, out cashOutParameter
                                                          );

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.s_pdi = offsetITCParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.sgstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.sgst_bal = itcOutParameter;

                    //CROSS IGST ADJUSTMENT using ITC CGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    offsetITCParameter = 0;
                    CrossOffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.cgstTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.cgst_bal
                                                          //, ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal
                                                          , out offsetITCParameter
                                                          //, out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          //, out cashOutParameter
                                                          );

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.i_pdc = offsetITCParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.cgst_bal = itcOutParameter;

                    //CROSS IGST ADJUSTMENT using ITC SGST
                    liabilityOutParameter = 0;
                    itcOutParameter = 0;
                    offsetITCParameter = 0;
                    CrossOffsetLiabilityAdjustment(ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot
                                                          //, ledgerSummary.ITC_Ledger.itcLdgDtls.cl_bal.sgstTaxBal
                                                          , ledgerSummary.CashITCBalance.itc_bal.sgst_bal
                                                          //, ledgerSummary.CashITCBalance.cash_bal.cgst_tot_bal
                                                          , out offsetITCParameter
                                                          //, out offsetCashParameter
                                                          , out liabilityOutParameter
                                                          , out itcOutParameter
                                                          //, out cashOutParameter
                                                          );

                    //Insert data into OffsetLiabilityGSTR3B object
                    dditc.i_pds = offsetITCParameter;
                    //Update ledgerSummary Object
                    ledgerSummary.LiabilityLedger.cl_bal.igstbal.tot = liabilityOutParameter;
                    ledgerSummary.CashITCBalance.itc_bal.sgst_bal = itcOutParameter;

                    offsetLiabilityGSTR3B.pdcash.Add(pdcash);
                    offsetLiabilityGSTR3B.pditc = dditc;

                    string jsondata = new JavaScriptSerializer().Serialize(offsetLiabilityGSTR3B);
                    //Call GSTR3B - Offset Liability GSTR3B data
                    var responseMessage=new OffsetLiabilityGstr3BSendData().SendRequest(jsondata
                                                                    , strGSTIN
                                                                    , strPeriod
                                                                    , Session["User_ID"].ToString()
                                                                    , Session["Cust_ID"].ToString()
                                                                    , Session["UserName"].ToString());
                    if (responseMessage == "Payment of tax successfully done")
                    {
                        TempData["LedgerSummary"] = ledgerSummary;
                    }
                    else
                    {
                        //here I need to call ledger three api
                        ledgerSummary = new LedgerSummary();
                        var apiResultLiabilityLedger = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 1);
                        ledgerSummary.LiabilityLedger = apiResultLiabilityLedger == "" ? new LiabilityLedger() : javaScriptSerializer.Deserialize<LiabilityLedger>(apiResultLiabilityLedger);
                        var apiResultITCLedger = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 2);
                        ledgerSummary.ITC_Ledger = apiResultITCLedger == "" ? new ItcLedger() : javaScriptSerializer.Deserialize<ItcLedger>(apiResultITCLedger);
                        var apiResultCashITC = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 3);
                        ledgerSummary.CashITCBalance = apiResultCashITC == "" ? new CashItcBalance() : javaScriptSerializer.Deserialize<CashItcBalance>(apiResultCashITC);

                    }
                    TempData["OffsetLiabilityResponseMessage"] = responseMessage;
                    if (string.IsNullOrEmpty(strGSTIN))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.GSTINEmptyOrNull = "Please Select GSTIN Number";
                        return View(ledgerSummary);
                    }
                    if (string.IsNullOrEmpty(strPeriod))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.PeriodEmptyOrNull = "Please Select Period";

                        return View(ledgerSummary);
                    }

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());

                    TempData["gstn"] = strGSTIN;
                    ViewBag.gstn = strGSTIN;
                    TempData["Period"] = strPeriod;
                    return View(ledgerSummary);
                }
                else if (!string.IsNullOrEmpty(GetLedger))
                {
                    if (string.IsNullOrEmpty(strGSTIN))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.GSTINEmptyOrNull = "Please Select GSTIN Number";
                        return View();
                    }
                    if (string.IsNullOrEmpty(strPeriod))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.PeriodEmptyOrNull = "Please Select Period";

                        return View();
                    }
                    //here I need to call ledger three api
                    LedgerSummary LedgerSummary = new LedgerSummary();
                    var apiResultLiabilityLedger = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 1);
                    LedgerSummary.LiabilityLedger = apiResultLiabilityLedger == "" ? new LiabilityLedger() : javaScriptSerializer.Deserialize<LiabilityLedger>(apiResultLiabilityLedger);
                    var apiResultITCLedger = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 2);
                    LedgerSummary.ITC_Ledger = apiResultITCLedger == "" ? new ItcLedger() : javaScriptSerializer.Deserialize<ItcLedger>(apiResultITCLedger);
                    var apiResultCashITC = new Gstr3BLedgerDataModel().SendRequest(strGSTIN, strPeriod, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), 3);
                    LedgerSummary.CashITCBalance = apiResultCashITC == "" ? new CashItcBalance() : javaScriptSerializer.Deserialize<CashItcBalance>(apiResultCashITC);
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                    TempData["LedgerSummary"] = LedgerSummary;
                    return View(LedgerSummary);
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
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.DownloadResponse = ex.Message;
                return View();
            }
        }

        public void OffsetLiabilityAdjustment(
            int liabilityParameter_1,//1
            int itcParameter_5,//2
            int cashParameter_9,//3
            out int offsetITCParameter,
            out int offsetCashParameter,
            out int liabilityOutParameter,
            out int itcOutParameter,
            out int cashOutParameter
            )
        {
            //var remainingOUTPUT = 0;//R
            offsetITCParameter = 0;
            offsetCashParameter = 0;

            if (liabilityParameter_1 > 0)
            {
                if (itcParameter_5 > 0)
                {
                    if (liabilityParameter_1 > itcParameter_5)
                    {
                        liabilityParameter_1 = liabilityParameter_1 - itcParameter_5;
                        offsetITCParameter = itcParameter_5;
                        itcParameter_5 = 0;
                        if (liabilityParameter_1 > 0)
                        {
                            if (cashParameter_9 > 0)
                            {
                                if (liabilityParameter_1 > cashParameter_9)
                                {
                                    liabilityParameter_1 = liabilityParameter_1 - cashParameter_9;
                                    offsetCashParameter = cashParameter_9;
                                    cashParameter_9 = 0;
                                }
                                else
                                {
                                    cashParameter_9 = cashParameter_9 - liabilityParameter_1;
                                    offsetCashParameter = cashParameter_9 - liabilityParameter_1;
                                    liabilityParameter_1 = 0;
                                }
                            }
                        }
                    }
                    else
                    {

                        itcParameter_5 = itcParameter_5 - liabilityParameter_1;
                        offsetITCParameter = itcParameter_5 - liabilityParameter_1;
                        liabilityParameter_1 = 0;
                    }
                }
                else
                {
                    if (cashParameter_9 > 0)
                    {
                        if (liabilityParameter_1 > cashParameter_9)
                        {
                            liabilityParameter_1 = liabilityParameter_1 - cashParameter_9;
                            offsetCashParameter = cashParameter_9;
                            cashParameter_9 = 0;
                        }
                        else
                        {
                            cashParameter_9 = cashParameter_9 - liabilityParameter_1;
                            offsetCashParameter = cashParameter_9 - liabilityParameter_1;
                            liabilityParameter_1 = 0;
                        }
                    }
                }
            }
            //Initialize Output Parameter
            liabilityOutParameter = liabilityParameter_1;
            itcOutParameter = itcParameter_5;
            cashOutParameter = cashParameter_9;

        }
        public void CrossOffsetLiabilityAdjustment(
           int liabilityParameter_1,//1
           int itcParameter_11,//2
           out int offsetITCParameter,
           out int liabilityOutParameter,
           out int itcOutParameter
           )
        {
            //var remainingOUTPUT = 0;//R
            offsetITCParameter = 0;

            if (liabilityParameter_1 > 0)
            {
                if (itcParameter_11 > 0)
                {
                    if (liabilityParameter_1 > itcParameter_11)
                    {
                        liabilityParameter_1 = liabilityParameter_1 - itcParameter_11;
                        offsetITCParameter = itcParameter_11;
                        itcParameter_11 = 0;
                    }
                    else
                    {
                        itcParameter_11 = itcParameter_11 - liabilityParameter_1;
                        offsetITCParameter = itcParameter_11 - liabilityParameter_1;
                        liabilityParameter_1 = 0;
                    }
                }
            }
            //Initialize Output Parameter
            liabilityOutParameter = liabilityParameter_1;
            itcOutParameter = itcParameter_11;

        }
    }
}