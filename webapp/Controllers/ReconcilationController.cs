#region Using

using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Reconcilation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;
using WeP_DAL;
using WeP_DAL.Reconcilation;
using static WeP_DAL.Reconcilation.ReconcilationDataAccess;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class ReconcilationController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Request.QueryString["InvId"]);
            string action = Convert.ToString(Request.QueryString["ActionType"]);
            string type = Convert.ToString(Request.QueryString["GSTRType"]);
            Session["id"] = id;
            Session["action"] = action;
            Session["type"] = type;
            return View();
        }

        #region "Matched Invoices"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MatchedInvoices()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");

            try
            {
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
                Session["action"] = "B2B";
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MatchedInvoices(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                #region "Variable Declaration and Intialization"
                string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strtoperiod = "",strRadio = "", fromdate = "", todate = "";
                string Accept = "", Export = "", AcceptALL = "";
                string strRefIds = "";
                DataSet dsMatchedInvoices;

                Accept = frm["Accept"];
                Export = frm["Export"];
                AcceptALL = frm["AcceptALL"];

                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];
                //year = frm["years"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                //Session["YEAR"] = year;
                ViewBag.Period = strPeriod;
                ViewBag.ToPeriod = strtoperiod;
                ViewBag.ActionType = strPeriod;

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

                #endregion

                #region "Accept-AcceptALL-Export"
                if (!string.IsNullOrEmpty(Accept))
                {
                    string[] invid;
                    if (strRefIds != null)
                    {
                        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                        string[] RefIds = strRefIds.Split('|');
                        for (int i = 0; i < RefIds.Count(); i++)
                        {
                            string ids = RefIds[i].ToString();
                            invid = ids.Split(',');
                            if (!string.IsNullOrEmpty(ids))
                            {
                                con.Open();
                                SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices", con);
                                dCmd.Parameters.Clear();
                                dCmd.CommandType = CommandType.StoredProcedure;
                                dCmd.CommandTimeout = 0;
                                dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                                dCmd.Parameters.Add(new SqlParameter("@RefIds", Convert.ToInt32(invid[0])));
                                dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                                dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                                dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                                dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                                dCmd.ExecuteNonQuery();
                                con.Close();

                            }
                        }

                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                        TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";
                    }
                }

                if (!string.IsNullOrEmpty(AcceptALL))
                {
                    if (Session["Matched_B2BRefIds"].ToString() != "")
                    {
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        if (strAction == "B2B")
                        {
                            dCmd.Parameters.Add(new SqlParameter("@RefIds", Session["Matched_B2BRefIds"].ToString()));
                        }
                        if (strAction == "CDNR" || strAction == "CDN")
                        {
                            dCmd.Parameters.Add(new SqlParameter("@RefIds", Session["Matched_CDNRefIds"].ToString()));
                        }
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();

                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "ALL Matched Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                        TempData["ReconcilationResponse"] = "ALL Invocies Accepted Successfully";
                    }
                }

                else if (!string.IsNullOrEmpty(Export))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Matched_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");
                    }
                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Matched_Invoices_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                   
                }
                else
                {
                    //
                }
                #endregion

                #region "Dropdown Loading"
                if (string.IsNullOrEmpty(strGSTINNo))
                {
                    strGSTINNo = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

                TempData["ActionType"] = strAction;
                TempData["GSTIN"] = strGSTINNo;
                TempData["CTIN"] = strCTIN;
                TempData["Period"] = strPeriod;
                TempData["ToPeriod"] = strtoperiod;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;
                #endregion

                #region "Data Loading"
                ReconcilationViewModel Model = new ReconcilationViewModel();
                if (!string.IsNullOrEmpty(strAction))
                {
                    List<ReconcilationDataAccess.MatchedInvoicesAttributes> LstMatchedInvoices = new List<ReconcilationDataAccess.MatchedInvoicesAttributes>();
                    if (strRadio != "FinancialYear")
                    {
                        dsMatchedInvoices = new ReconcilationDataModel(custid, userid).GetMatchedInvoices(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                        TempData["MatchedInvoices"] = dsMatchedInvoices;
                        Session["f_year"] = "FinancialPeriod";
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                        ViewBag.Period = Session["CF_PERIOD"].ToString();
                        ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    }
                    else
                    {

                        dsMatchedInvoices = new ReconcilationDataModel(custid, userid).GetMatchedInvoices_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                        TempData["MatchedInvoices"] = dsMatchedInvoices;
                        Session["f_year"] = "FinancialYear";
                        ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
   

                    }
                        if (dsMatchedInvoices.Tables.Count > 0)
                    {
                        switch (strAction)
                        {
                            case "B2B":
                                foreach (DataRow dr in dsMatchedInvoices.Tables[0].Rows)
                                {
                                    LstMatchedInvoices.Add(new ReconcilationDataAccess.MatchedInvoicesAttributes
                                    {
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"])
                                    });
                                }
                                break;
                            case "CDNR":
                                foreach (DataRow dr in dsMatchedInvoices.Tables[0].Rows)
                                {
                                    LstMatchedInvoices.Add(new ReconcilationDataAccess.MatchedInvoicesAttributes
                                    {
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                        nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"])
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    Model.MatchedInvoices = LstMatchedInvoices;
                    ViewBag.LstMatchedInvoices = LstMatchedInvoices.Count;

                    TempData["ExportData"] = dsMatchedInvoices.Tables[1];
                }
                #endregion

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst;
                var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }

        }

        #endregion

        #region "MissingGSTR2A"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MissinginGSTR2A()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {

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
                //if (Session["YEAR"] != null)
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(Session["YEAR"].ToString());
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
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
                Session["action"] = "B2B";
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MissinginGSTR2A(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strInvIds = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
            string Accept = "", Reject = "",  Trigger = "", Export = "", strRefIds = "";
            DataSet dsMissinginGSTR2A;

            Accept = frm["Accept"];
            Reject = frm["Reject"];
            //Pending = frm["Pending"];
            Trigger = frm["Trigger"];
            Export = frm["Export"];
            try
            {
                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strInvIds = frm["InvIds"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                ViewBag.Period = strPeriod;
                ViewBag.ToPeriod = strtoperiod;
                ViewBag.ActionType = strPeriod;

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

                #region "Accept Invoice"
                if (!string.IsNullOrEmpty(Accept))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";

                }
                #endregion

                #region "Reject Invoices"
                else if (!string.IsNullOrEmpty(Reject))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "R"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Rejected Successfully";

                }
                #endregion

                //#region "Hold Invoices"
                //else if (!string.IsNullOrEmpty(Pending))
                //{
                //    string[] invid;
                //    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                //    foreach (var id in invid)
                //    {
                //        string ids = id;
                //        con.Open();
                //        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2A", con);
                //        dCmd.Parameters.Clear();
                //        dCmd.CommandType = CommandType.StoredProcedure;
                //        dCmd.CommandTimeout = 0;
                //        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                //        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                //        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                //        dCmd.Parameters.Add(new SqlParameter("@Activity", "H"));
                //        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                //        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                //        dCmd.ExecuteNonQuery();
                //        con.Close();
                //    }
                //    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                //    TempData["ReconcilationResponse"] = "Selected Invocies Hold Successfully";

                //}
                //#endregion

                #region "Trigger Invoice"
                else if (!string.IsNullOrEmpty(Trigger))
                {
                    string[] RefIds;
                    string[] sup_Gstin;
                    string GSTINNO = string.Empty;
                    string INVNO = string.Empty;
                    string strEmail = string.Empty;
                    string strMobileNo = string.Empty;
                    string InvDate = string.Empty;
                    string InvVal = string.Empty;
                    string gstin_2 = string.Empty;
                    string UserEmail = string.Empty;
                    if (strRefIds != null)
                    {
                        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                        RefIds = strRefIds.Split('|');
                        for (int i = 0; i < RefIds.Count(); i++)
                        {
                            string ids = RefIds[i].ToString();
                            sup_Gstin = ids.Split(',');
                            GSTINNO = sup_Gstin[1].ToString();
                            INVNO = sup_Gstin[2].ToString();
                            InvDate = sup_Gstin[3].ToString();
                            InvVal = sup_Gstin[4].ToString();
                            gstin_2 = sup_Gstin[5].ToString();
                            strEmail = (from lst in db.TBL_Supplier
                                        where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                        select lst.EmailId).FirstOrDefault();
                            strMobileNo = (from lst in db.TBL_Supplier
                                           where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                           select lst.MobileNo).FirstOrDefault();

                            UserEmail = (from lst in db.UserLists
                                         where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                         select lst.Email).FirstOrDefault();

                            string comp = Session["CompanyName"].ToString();
                            string mob = Session["MobileNo"].ToString();

                            string name = Session["Name"].ToString();


                            if (strEmail != null && strMobileNo != null)
                            {
 
                                    //Emails are correctly formatted
                                    string[] emails;
                                    emails = strEmail.Split(';');

                                    foreach(var email in emails)
                                    {
                                        Notification.SendEmail_Reconciliation(email, UserEmail, string.Format(" Please Review the Missing Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + InvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                                    }
                                Notification.SendSMS(strMobileNo, string.Format("Please Review the Missing Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", Invoice Value : " + InvVal));
                                Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                TempData["ReconcilationResponse"] = "Selected Invocies Triggered Successfully";
                            }
                            else
                            {
                                Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Not triggered because Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";

                            }

                        }
                    }
                }
                #endregion

                #region "Export Data"
                else if (!string.IsNullOrEmpty(Export))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Missing in GSTR2A Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Missing_in_GSTR2A_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");
                    }

                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Missing_in_GSTR2A_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate +".xlsx");
                    }
                }

                #endregion

                else
                {
                    //
                }

                #region "Load Dropdowns & Retrieve Data"
                if (string.IsNullOrEmpty(strGSTINNo))
                {
                    strGSTINNo = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

                TempData["ActionType"] = strAction;
                TempData["GSTIN"] = strGSTINNo;
                TempData["CTIN"] = strCTIN;
                TempData["Period"] = strPeriod;
                TempData["ToPeriod"] = strtoperiod;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;

                ReconcilationViewModel Model = new ReconcilationViewModel();
                if (!string.IsNullOrEmpty(strAction))
                {
                    List<ReconcilationDataAccess.MissingInGstr2AAttributes> LstMissinginGSTR2A = new List<ReconcilationDataAccess.MissingInGstr2AAttributes>();
                    if (strRadio != "FinancialYear")
                    {
                        dsMissinginGSTR2A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                        Session["f_year"] = "FinancialPeriod";
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                        ViewBag.Period = Session["CF_PERIOD"].ToString();
                        ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    }
                    else
                    {


                        dsMissinginGSTR2A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                        Session["f_year"] = "FinancialYear";
                        ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                    }
                    if (dsMissinginGSTR2A.Tables.Count > 0)
                    {
                        switch (strAction)
                        {
                            case "B2B":
                                foreach (DataRow dr in dsMissinginGSTR2A.Tables[0].Rows)
                                {
                                    LstMissinginGSTR2A.Add(new ReconcilationDataAccess.MissingInGstr2AAttributes
                                    {
                                        invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                        TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                                        inv_typ = dr.IsNull("inv_typ") ? "" : dr["inv_typ"].ToString()
                                    });
                                }
                                break;
                            case "CDNR":
                                foreach (DataRow dr in dsMissinginGSTR2A.Tables[0].Rows)
                                {
                                    LstMissinginGSTR2A.Add(new ReconcilationDataAccess.MissingInGstr2AAttributes
                                    {
                                        invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                        TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"])
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    Model.MissingInvoicesInGSTR2A = LstMissinginGSTR2A;
                    ViewBag.LstMissinginGSTR2A = LstMissinginGSTR2A.Count;

                    TempData["ExportData"] = dsMissinginGSTR2A.Tables[1];
                }
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst;
                #endregion

                var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Missing GSTR2"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MissinginGSTR2()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {
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
                //if (Session["YEAR"] != null)
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(Session["YEAR"].ToString());
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
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
                Session["action"] = "B2B";
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MissinginGSTR2(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                #region "variable declaration and intialization"
                string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strInvIds = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
                string Accept = "", Reject = "", Pending = "", Trigger = "", Export = "";
                string strRefIds = "";
                DataSet dsMissinginGSTR2;

                Accept = frm["Accept"];
                Reject = frm["Reject"];
                Pending = frm["Pending"];
                Trigger = frm["Trigger"];
                Export = frm["Export"];

                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strInvIds = frm["InvIds"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];
                //year = frm["years"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                //Session["YEAR"] = year;
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
                #endregion

                #region "Accept-Reject-Pending-Trigger-Export
                if (!string.IsNullOrEmpty(Accept))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2 Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";
                }

                else if (!string.IsNullOrEmpty(Reject))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "R"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2 Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices Rejected Successfully";

                }

                else if (!string.IsNullOrEmpty(Pending))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "H"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2 Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices Hold Successfully";

                }
                else if (!string.IsNullOrEmpty(Trigger))
                {
                    string[] RefIds;
                    string[] sup_Gstin;
                    string GSTINNO = string.Empty;
                    string INVNO = string.Empty;
                    string strEmail = string.Empty;
                    string strMobileNo = string.Empty;
                    string InvDate = string.Empty;
                    string InvVal = string.Empty;
                    string gstin_2 = string.Empty;
                    string UserEmail = string.Empty;
                    if (strRefIds != null)
                    {
                        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                        RefIds = strRefIds.Split('|');
                        for (int i = 0; i < RefIds.Count(); i++)
                        {
                            string ids = RefIds[i].ToString();
                            sup_Gstin = ids.Split(',');
                            GSTINNO = sup_Gstin[1].ToString();
                            INVNO = sup_Gstin[2].ToString();
                            InvDate = sup_Gstin[3].ToString();
                            InvVal = sup_Gstin[4].ToString();
                            gstin_2 = sup_Gstin[5].ToString();
                            strEmail = (from lst in db.TBL_Supplier
                                        where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                        select lst.EmailId).FirstOrDefault();
                            strMobileNo = (from lst in db.TBL_Supplier
                                           where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                           select lst.MobileNo).FirstOrDefault();

                            UserEmail = (from lst in db.UserLists
                                         where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                         select lst.Email).FirstOrDefault();

                            string comp = Session["CompanyName"].ToString();
                            string mob = Session["MobileNo"].ToString();

                            string name = Session["Name"].ToString();

                            if (strEmail != null && strMobileNo != null)
                            {
                                
                                        //Emails are correctly formatted
                                        string[] emails;
                                        emails = strEmail.Split(';');
                                        foreach (var email in emails)
                                        {
                                            Notification.SendEmail_Reconciliation(email, UserEmail, string.Format(" Please Review the Missing Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + InvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                                        }
                                    Notification.SendSMS(strMobileNo, string.Format("Please Review the Missing Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", Invoice Value : " + InvVal));
                                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                    TempData["ReconcilationResponse"] = "Selected Invocies Triggered Successfully";
                            }
                            else
                            {
                                Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2 Invocies Not triggered because Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                            }

                        }
                    }
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Missing in GSTR2 Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Missing_in_GSTR2_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");
                    }

                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Missing_in_GSTR2_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }
                    
                }
                else
                {
                    //
                }
                #endregion

                #region "Dropdown Loading"
                if (string.IsNullOrEmpty(strGSTINNo))
                {
                    strGSTINNo = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                //if (string.IsNullOrEmpty(year))
                //{

                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(year);
                //}

                TempData["ActionType"] = strAction;
                TempData["GSTIN"] = strGSTINNo;
                TempData["CTIN"] = strCTIN;
                TempData["Period"] = strPeriod;
                TempData["ToPeriod"] = strtoperiod;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;
                #endregion

                #region "Loading Data"
                ReconcilationViewModel Model = new ReconcilationViewModel();
                if (!string.IsNullOrEmpty(strAction))
                {
                    if (strRadio != "FinancialYear")
                    {
                        dsMissinginGSTR2 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                        TempData["MissinginGSTR2"] = dsMissinginGSTR2;
                        Session["f_year"] = "FinancialPeriod";
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                        ViewBag.Period = Session["CF_PERIOD"].ToString();
                        ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    }
                    else
                    {

                        dsMissinginGSTR2 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                        TempData["MissinginGSTR2"] = dsMissinginGSTR2;
                        Session["f_year"] = "FinancialYear";
                        ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                    }

                    List<ReconcilationDataAccess.MissingInGstr2Attributes> LstMissinginGSTR2 = new List<ReconcilationDataAccess.MissingInGstr2Attributes>();

                    if (dsMissinginGSTR2.Tables.Count > 0)
                    {
                        switch (strAction)
                        {
                            case "B2B":
                                foreach (DataRow dr in dsMissinginGSTR2.Tables[0].Rows)
                                {
                                    LstMissinginGSTR2.Add(new ReconcilationDataAccess.MissingInGstr2Attributes
                                    {
                                        invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                        TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                                        inv_typ = dr.IsNull("inv_typ") ? "" : dr["inv_typ"].ToString()
                                    });
                                }
                                break;
                            case "CDNR":
                                foreach (DataRow dr in dsMissinginGSTR2.Tables[0].Rows)
                                {
                                    LstMissinginGSTR2.Add(new ReconcilationDataAccess.MissingInGstr2Attributes
                                    {
                                        invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                        TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"])
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    Model.MissingInvoicesInGSTR2 = LstMissinginGSTR2;
                    ViewBag.LstMissinginGSTR2 = LstMissinginGSTR2.Count;

                    TempData["ExportData"] = dsMissinginGSTR2.Tables[1];

                }
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                #endregion

                var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Mismatched Invoices"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MismatchInvoices()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");

            try
            {

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
                Session["action"] = "B2B";
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MismatchInvoices(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
            string Trigger = "", Export = "", Modify = "", strRefIds = "";
            DataSet dsMismatchInvoices;

            Modify = frm["Modify"];
           // Pending = frm["Pending"];
            Trigger = frm["Trigger"];
            Export = frm["Export"];
            try
            {
                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strSupplierName = frm["ddlSupplierName"];
                strAction = frm["ddlActionType"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
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


                #region "Modify Invoice"
                if (!string.IsNullOrEmpty(Modify))
                {
                    string[] invid;
                    string[] RefIds;
                    if (strRefIds != null)
                    {
                        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                        RefIds = strRefIds.Split('|');
                        for (int i = 0; i < RefIds.Count(); i++)
                        {
                            string ids = RefIds[i].ToString();
                            invid = ids.Split(',');
                            con.Open();
                            SqlCommand dCmd = new SqlCommand("usp_Update_Mismatched_Invoices", con);
                            dCmd.Parameters.Clear();
                            dCmd.CommandType = CommandType.StoredProcedure;
                            dCmd.CommandTimeout = 0;
                            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                            dCmd.Parameters.Add(new SqlParameter("@GSTR2invid", Convert.ToInt32(invid[0])));
                            dCmd.Parameters.Add(new SqlParameter("@GSTR2Ainvid", Convert.ToInt32(invid[6])));
                            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                            dCmd.ExecuteNonQuery();
                            con.Close();
                        }

                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices modified successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                        TempData["ReconcilationResponse"] = "Selected Invoices Modified Successfully";
                    }
                }
                #endregion

                //#region "Hold Invoices"
                //else if (!string.IsNullOrEmpty(Pending))
                //{
                //    string[] invid;
                //    string[] RefIds;
                //    if (strRefIds != null)
                //    {
                //        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                //        RefIds = strRefIds.Split('|');
                //        for (int i = 0; i < RefIds.Count(); i++)
                //        { 
                //            string ids = RefIds[i].ToString();
                //            invid = ids.Split(',');
                //            con.Open();
                //            SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_Mismatch_GSTR2and2A", con);
                //            dCmd.Parameters.Clear();
                //            dCmd.CommandType = CommandType.StoredProcedure;
                //            dCmd.CommandTimeout = 0;
                //            dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                //            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                //            dCmd.Parameters.Add(new SqlParameter("@Activity", "G"));
                //            dCmd.Parameters.Add(new SqlParameter("@GSTR2invId", Convert.ToInt32(invid[0])));
                //            dCmd.Parameters.Add(new SqlParameter("@GSTR2AinvId", Convert.ToInt32(invid[6])));
                //            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                //            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                //            dCmd.ExecuteNonQuery();
                //            con.Close();
                //        }
                //        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices Kept Pending successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                //        TempData["ReconcilationResponse"] = "Selected Invoices Hold Successfully";
                //    }
                //}
                //#endregion

                #region "Trigger Invoice"
                else if (!string.IsNullOrEmpty(Trigger))
                {
                    string[] sup_Gstin;
                    string[] RefIds;
                    string GSTINNO = string.Empty;
                    string INVNO = string.Empty;
                    string strEmail = string.Empty;
                    string strMobileNo = string.Empty;
                    string InvDate = string.Empty;
                    string GSTR2InvVal = string.Empty;
                    string GSTR2AInvVal = string.Empty;
                    string gstin_2 = string.Empty;
                    string UserEmail = string.Empty;
                    if (strRefIds != null)
                    {
                        strRefIds = strRefIds.TrimStart('|').TrimEnd('|');
                        RefIds = strRefIds.Split('|');
                        for (int i = 0; i < RefIds.Count(); i++)
                        {
                            string ids = RefIds[i].ToString();
                            sup_Gstin = ids.Split(',');
                            GSTINNO = sup_Gstin[1].ToString();
                            INVNO = sup_Gstin[2].ToString();
                            InvDate = sup_Gstin[3].ToString();
                            GSTR2InvVal = sup_Gstin[4].ToString();
                            GSTR2AInvVal = sup_Gstin[5].ToString();
                            gstin_2 = sup_Gstin[7].ToString();

                            strEmail = (from lst in db.TBL_Supplier
                                        where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                        select lst.EmailId).FirstOrDefault();
                            strMobileNo = (from lst in db.TBL_Supplier
                                           where lst.GSTINno == GSTINNO && lst.CustomerId == custid && lst.RowStatus == true
                                           select lst.MobileNo).FirstOrDefault();
                            UserEmail = (from lst in db.UserLists
                                         where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                         select lst.Email).FirstOrDefault();

                            string comp = Session["CompanyName"].ToString();
                            string mob = Session["MobileNo"].ToString();

                            string name = Session["Name"].ToString();

                            if (strEmail != null && strMobileNo != null)
                            {
                                        string[] emails;
                                        emails = strEmail.Split(';');
                                        foreach (var email in emails)
                                        {
                                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Mismatched Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>GSTR2 Invoice Value</th><th>GSTR2A Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + GSTR2InvVal + "</td><td>" + GSTR2AInvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                                        }
                      
                                    Notification.SendSMS(strMobileNo, string.Format("Please Review the Mismatched Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", GSTR2 Invoice Value : " + GSTR2InvVal + ", GSTR2A Invoice Value : " + GSTR2AInvVal));
                                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                    TempData["ReconcilationResponse"] = "Selected Invoices Triggered Successfully";
                            }
                            else
                            {
                                Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                                TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                            }

                        }
                    }
                }
                #endregion

                #region "Export Data"
                else if (!string.IsNullOrEmpty(Export))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Mismatched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Mismatch_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Mismatch_Invoices_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }
                #endregion

                else
                {
                    //
                }

                #region "Loading Dropdowns & Retrieve Data"
                if (string.IsNullOrEmpty(strGSTINNo))
                {
                    strGSTINNo = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                ViewBag.ActionList = Actionlst;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());

                TempData["ActionType"] = strAction;
                TempData["GSTIN"] = strGSTINNo;
                TempData["SupplierName"] = strSupplierName;
                TempData["CTIN"] = strCTIN;
                TempData["Period"] = strPeriod;
                TempData["ToPeriod"] = strtoperiod;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;

                ReconcilationViewModel Model = new ReconcilationViewModel();
                if (!string.IsNullOrEmpty(strAction))
                {
                    if (strRadio != "FinancialYear")
                    {
                      dsMismatchInvoices = new ReconcilationDataModel(custid, userid).GetMismatchInvoiceinGSTR2andGSTR2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                        Session["f_year"] = "FinancialPeriod";
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                        ViewBag.Period = Session["CF_PERIOD"].ToString();
                        ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    }
                    else
                    {
                        dsMismatchInvoices = new ReconcilationDataModel(custid, userid).GetMismatchInvoiceinGSTR2andGSTR2A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                        Session["f_year"] = "FinancialYear";
                        ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                        ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                        ViewBag.ToDate = Session["CF_TODATE"].ToString();
                    } 
                    List<ReconcilationDataAccess.MismatchInvoicesAttributes> LstMismatchInvoices = new List<ReconcilationDataAccess.MismatchInvoicesAttributes>();
                    if (dsMismatchInvoices.Tables.Count > 0)
                    {
                        switch (strAction)
                        {
                            case "B2B":
                                foreach (DataRow dr in dsMismatchInvoices.Tables[0].Rows)
                                {
                                    LstMismatchInvoices.Add(new ReconcilationDataAccess.MismatchInvoicesAttributes
                                    {
                                        invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                        ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                        inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                        idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                        val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                        txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                        TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                    });
                                }
                                break;
                            case "CDNR":
                                foreach (DataRow dr in dsMismatchInvoices.Tables[0].Rows)
                                {
                                    LstMismatchInvoices.Add(new ReconcilationDataAccess.MismatchInvoicesAttributes
                                    {
                                        invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                        ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                        nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                        ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                        nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                        nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                        inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                        idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                        val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                        txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                        TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                    });
                                }
                                break;
                            default:
                                break;
                        }
                    }

                    Model.MismatchInvoices = LstMismatchInvoices;
                    ViewBag.LstMismatchInvoices = LstMismatchInvoices.Count;

                    TempData["ExportData"] = dsMismatchInvoices.Tables[1];
                }

                #endregion

                var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Hold Invoices"

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult HoldInvoices()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");

            try
            {
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
                Session["action"] = "B2B";

               // ViewBag.FilterType = LoadDropDowns.Exist_FilterType_HoldUnhold("1");

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }

            return View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult HoldInvoices(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strInvIds = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "", strFilterType = "";
            string Unhold = "",  Reject = "", Accept = "", Export = "", Export_match = "", strRefIds = "";
            DataSet  dsMissinginGSTR2, dsMatchInvoices;

            Unhold = frm["Unhold"];

            Reject = frm["Reject"];

            Export = frm["Export"];
            Export_match = frm["Export_match"];

            Accept = frm["Accept"];

            try
            {
                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strFilterType = frm["strFilterType"];
                strInvIds = frm["InvIds"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                ViewBag.Period = strPeriod;
                ViewBag.ToPeriod = strtoperiod;
                ViewBag.ActionType = strPeriod;

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

                #region "Unhold, Reject, Accept Invoices"
                if (!string.IsNullOrEmpty(Unhold))
                {
                   
                        string[] invid;
                        invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                        //string activity = "";
                        foreach (var id in invid)
                        {
                            string ids = id;
                            con.Open();
                            SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                            dCmd.Parameters.Clear();
                            dCmd.CommandType = CommandType.StoredProcedure;
                            dCmd.CommandTimeout = 0;
                            dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                            dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                            dCmd.Parameters.Add(new SqlParameter("@Activity", "H"));
                            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                            dCmd.ExecuteNonQuery();
                            con.Close();
                        }

                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Unhold Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        TempData["ReconcilationResponse"] = "Selected Invocies Unhold Successfully";
                 
                }


                if (!string.IsNullOrEmpty(Reject))
                {
                    
                        string[] invid;
                        invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                        //string activity = "";
                        foreach (var id in invid)
                        {
                            string ids = id;
                            con.Open();
                            SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                            dCmd.Parameters.Clear();
                            dCmd.CommandType = CommandType.StoredProcedure;
                            dCmd.CommandTimeout = 0;
                            dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                            dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                            dCmd.Parameters.Add(new SqlParameter("@Activity", "E"));
                            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                            dCmd.ExecuteNonQuery();
                            con.Close();
                        }

                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        TempData["ReconcilationResponse"] = "Selected Invocies Rejected Successfully";
                   
                }

                if (!string.IsNullOrEmpty(Accept))
                {
                    string[] invid;
                    invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                    string activity = "";
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR2", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", activity));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Hold Invocies are Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Hold Invocies Accepted Successfully";
                }
                #endregion

                #region "Export Data"
                else if (!string.IsNullOrEmpty(Export))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Mismatched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Hold_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Hold_Invoices_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }

                else if (!string.IsNullOrEmpty(Export_match))
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Mismatched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    if (strRadio != "FinancialYear")
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "Hold_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "Hold_Invoices_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }
                #endregion

                else
                {
                    //
                }

                #region "Dropdown Loading"
                if (string.IsNullOrEmpty(strGSTINNo))
                {
                    strGSTINNo = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                }

                if (string.IsNullOrEmpty(strSupplierName))
                {
                    strSupplierName = "ALL";
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }

                if (string.IsNullOrEmpty(strCTIN))
                {
                    strCTIN = "ALL";
                    ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
                }
                else
                {
                    ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                }
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
                //if (string.IsNullOrEmpty(year))
                //{

                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(year);
                //}

                TempData["ActionType"] = strAction;
                TempData["GSTIN"] = strGSTINNo;
                TempData["CTIN"] = strCTIN;
                TempData["Period"] = strPeriod;
                TempData["ToPeriod"] = strtoperiod;
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;
                #endregion

                #region "Loading Data"
                ReconcilationViewModel Model = new ReconcilationViewModel();
                ReconcilationViewModel Model2 = new ReconcilationViewModel();
              
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess.MissingInGstr2Attributes> LstMissinginGSTR2 = new List<ReconcilationDataAccess.MissingInGstr2Attributes>();
                        List<ReconcilationDataAccess.Reconciliation_MatchedHold> LstMatchHold = new List<ReconcilationDataAccess.Reconciliation_MatchedHold>();

                    if (strRadio == "FinancialPeriod")       //All Invoices
                        {
                            dsMissinginGSTR2 = new ReconcilationDataModel(custid, userid).Get_Hold_MissingInvoiceinGSTR2(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsMatchInvoices = new ReconcilationDataModel(custid, userid).Get_Hold_MissingInvoiceinGSTR2(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["HoldMissingingstr2A"] = dsMissinginGSTR2;
                            TempData["HoldMatch"] = dsMatchInvoices;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }
                        else                                     
                        {
                            

                            dsMissinginGSTR2 = new ReconcilationDataModel(custid, userid).Get_Hold_MissingInvoiceinGSTR2_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            dsMatchInvoices = new ReconcilationDataModel(custid, userid).Get_Hold_MissingInvoiceinGSTR2_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);

                            TempData["HoldMissingingstr2A"] = dsMissinginGSTR2;
                            TempData["HoldMatch"] = dsMatchInvoices;
                            Session["f_year"] = "FinancialYear";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }

                        if (dsMissinginGSTR2.Tables.Count > 0 || dsMatchInvoices.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsMissinginGSTR2.Tables[0].Rows)
                                    {
                                        LstMissinginGSTR2.Add(new ReconcilationDataAccess.MissingInGstr2Attributes
                                        {
                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                            pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                                            inv_typ = dr.IsNull("inv_typ") ? "" : dr["inv_typ"].ToString()
                                        });
                                    }

                                foreach (DataRow dr in dsMatchInvoices.Tables[2].Rows)
                                {
                                    LstMatchHold.Add(new ReconcilationDataAccess.Reconciliation_MatchedHold
                                    {
                                        invid2a = dr.IsNull("invid2a") ? 0 : Convert.ToInt32(dr["invid2a"]),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        fp2 = dr.IsNull("fp2") ? "" : dr["fp2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                        TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                    });
                                }

                                break;
                                case "CDNR":
                                    foreach (DataRow dr in dsMissinginGSTR2.Tables[0].Rows)
                                    {
                                        LstMissinginGSTR2.Add(new ReconcilationDataAccess.MissingInGstr2Attributes
                                        {
                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                            nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                                            nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"])
                                        });
                                    }

                                      foreach (DataRow dr in dsMatchInvoices.Tables[2].Rows)
                                    {
                                    LstMatchHold.Add(new ReconcilationDataAccess.Reconciliation_MatchedHold
                                    {
                                        ntid2a = dr.IsNull("ntid2a") ? 0 : Convert.ToInt32(dr["ntid2a"]),
                                        gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                        fp2 = dr.IsNull("fp2") ? "" : dr["fp2"].ToString(),
                                        ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                        nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                        nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                        inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                        idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                        val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                        txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                        TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                        TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                    });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                    Model.MissingInvoicesInGSTR2 = LstMissinginGSTR2;
                    Model.MatchedHold = LstMatchHold;
                    ViewBag.LstMissinginGSTR2 = LstMissinginGSTR2.Count;
                    
                    ViewBag.LstMatchHold = LstMatchHold.Count;

                    TempData["ExportData"] = dsMissinginGSTR2.Tables[1];
                    TempData["ExportData1"] = dsMatchInvoices.Tables[3];
                }
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;
            
                #endregion


            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }

        #endregion

        #region "Updated ITC"
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult UpdateITC(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            try
            {
                if (sort != null || page != 0)
                {
                    string strSupplierName = TempData["SupplierName"] as string;
                    string strAction = TempData["ActionType"] as string;
                    string strGSTINNO = TempData["GSTIN"] as string;
                    string strCTIN = TempData["CTIN"] as string;
                    string strPeriod = TempData["Period"] as string;
                    string strFlag = TempData["Flag"] as string;
                    ViewBag.Period = strPeriod;
                    TempData.Keep();

                    if (string.IsNullOrEmpty(strGSTINNO))
                    {
                        strGSTINNO = "ALL";
                    }

                    if (string.IsNullOrEmpty(strSupplierName))
                    {
                        strSupplierName = "ALL";
                        ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                        ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
                    }
                    else
                    {
                        ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, strSupplierName);
                        ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, strSupplierName);
                    }

                    if (string.IsNullOrEmpty(strCTIN))
                    {
                        strCTIN = "ALL";
                        ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
                    }
                    else if ((string.IsNullOrEmpty(strSupplierName) || strSupplierName == "ALL") && (!string.IsNullOrEmpty(strCTIN) || strCTIN == "ALL"))
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR2_2A_CTIN(custid, userid, strCTIN);
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, strSupplierName, strCTIN);
                    }
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNO, Session["Role_Name"].ToString());
                    ViewBag.FlagList = LoadDropDowns.Exist_GetFlags(strFlag);

                    ViewBag.UpdateITC = new ReconcilationDataModel(custid, userid).GetUpdateITCData(strGSTINNO, strSupplierName, strCTIN, strPeriod, strAction, strFlag);

                    return View();
                }
                else
                {
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName");
                    ViewBag.ActionList = Actionlst;
                    ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
                    ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                    ViewBag.FlagList = LoadDropDowns.GetFlags();
                }
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult UpdateITC(FormCollection frm, string Accept, string Reject, string Pending, string Trigger, string Modify, string[] InvID)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strFlag = "";

            strGSTINNo = frm["ddlGSTINNo"];
            strCTIN = frm["ddlSupGSTIN"];
            strPeriod = frm["period"];
            strSupplierName = frm["ddlSupplierName"];
            strAction = frm["ddlActionType"];
            strFlag = frm["ddlFlag"];

            Session["GSTIN"] = strGSTINNo;
            Session["action"] = strAction;
            ViewBag.Period = strPeriod;
            //Accept Invoice
            if (!string.IsNullOrEmpty(Accept))
            {
                //
            }
            //Reject Invoice
            else if (!string.IsNullOrEmpty(Reject))
            {
                //
            }

            //Modify Invoice
            else if (!string.IsNullOrEmpty(Modify))
            {
                TempData["ReconcilationResponse"] = "Invoices Modified Successfully";
            }

            //Pending Invoices
            else if (!string.IsNullOrEmpty(Pending))
            {
                string[] invid;
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_Mismatch_GSTR2and2A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "P"));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR2invId", Convert.ToInt32(invid[0])));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR2AinvId", Convert.ToInt32(invid[6])));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invoices data Kept Pending successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices marked as 'Pending' Successfully";
                }
            }

            //Trigger Invoice
            else if (!string.IsNullOrEmpty(Trigger))
            {
                string[] sup_Gstin;
                string GSTINNO = string.Empty;
                string INVNO = string.Empty;
                string strEmail = string.Empty;
                string strMobileNo = string.Empty;
                string InvDate = string.Empty;
                string GSTR2InvVal = string.Empty;
                string GSTR2AInvVal = string.Empty;
                string UserEmail = string.Empty;
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        string ids = id.ToString();
                        sup_Gstin = ids.Split(',');
                        GSTINNO = sup_Gstin[1].ToString();
                        INVNO = sup_Gstin[2].ToString();
                        InvDate = sup_Gstin[3].ToString();
                        GSTR2InvVal = sup_Gstin[4].ToString();
                        GSTR2AInvVal = sup_Gstin[5].ToString();

                        strEmail = (from lst in db.TBL_Supplier
                                    where lst.GSTINno == GSTINNO
                                    select lst.EmailId).FirstOrDefault();
                        strMobileNo = (from lst in db.TBL_Supplier
                                       where lst.GSTINno == GSTINNO
                                       select lst.MobileNo).FirstOrDefault();
                        UserEmail = (from lst in db.UserLists
                                     where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                     select lst.Email).FirstOrDefault();
                        if (strEmail != null && strMobileNo != null)
                        {
                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Mismatched Invoices For this GSTIN :" + GSTINNO + "."), string.Format("Please Review the Mismatched Invoices For this GSTIN :" + GSTINNO + ".<br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>GSTR2 Invoice Value</th><th>GSTR2A Invoice Value</th></tr><tr><td>" + strGSTINNo + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + GSTR2InvVal + "</td><td>" + GSTR2AInvVal + "</td></tr></table>"));
                            Notification.SendSMS(strMobileNo, string.Format("Please Review the Mismatched Invoices For this GSTIN :" + GSTINNO + ". Invoice No : " + INVNO + ", CTIN : " + strGSTINNo + ", Invoice Date : " + InvDate + ", GSTR2 Invoice Value : " + GSTR2InvVal + ", GSTR2A Invoice Value : " + GSTR2AInvVal));
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Selected Invoices Triggered Successfully";
                        }
                        else
                        {
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                        }

                    }
                }
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
            }

            if (string.IsNullOrEmpty(strSupplierName))
            {
                strSupplierName = "ALL";
                ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, strSupplierName);
                ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, strSupplierName);
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
            }
            else if ((string.IsNullOrEmpty(strSupplierName) || strSupplierName == "ALL") && (!string.IsNullOrEmpty(strCTIN) || strCTIN == "ALL"))
            {
                ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR2_2A_CTIN(custid, userid, strCTIN);
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, strSupplierName, strCTIN);
            }

            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["SupplierName"] = strSupplierName;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["Flag"] = strFlag;

            if (!string.IsNullOrEmpty(strAction))
            {
                ViewBag.UpdateITC = new ReconcilationDataModel(custid, userid).GetUpdateITCData(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strFlag);
            }
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            ViewBag.FlagList = LoadDropDowns.Exist_GetFlags(strFlag);
            return View();
        }
        #endregion

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
    }
}
