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
using WeP_DAL.Reconcilation;
using static WeP_DAL.Reconcilation.ReconcilationDataAccess;

#endregion

namespace SmartAdminMvc.Controllers
{
    public class VendorReconcilationController : Controller
    {
        string fileNameKey = String.Format("{0}_{1}", Guid.NewGuid(), DateTime.Now.Ticks);
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: VendorReconcilation
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
                                (ds.Tables[9].Rows.Count > 200000) || (ds.Tables[10].Rows.Count > 200000) || (ds.Tables[11].Rows.Count > 200000) ||
                                (ds.Tables[12].Rows.Count > 200000) || (ds.Tables[13].Rows.Count > 200000) || (ds.Tables[14].Rows.Count > 200000) ||
                                (ds.Tables[15].Rows.Count > 200000) || (ds.Tables[16].Rows.Count > 200000) || (ds.Tables[17].Rows.Count > 200000) ||
                                (ds.Tables[18].Rows.Count > 200000) || (ds.Tables[19].Rows.Count > 200000) || (ds.Tables[20].Rows.Count > 200000))
                            {
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
                                    WirteToFile(splittedtables[0], "ModifiedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[17], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[18].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[18], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[18], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[19].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[19], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-B2BA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-B2BA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[19], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[20].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[20].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[20], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-CDNRA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-CDNRA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[20], "Missing in GSTR2-CDNRA");
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
                                    ds.Tables[17].TableName = "ModifiedRecord-B2B";
                                }
                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    ds.Tables[18].TableName = "ModifiedRecord-CDNR";
                                }
                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    ds.Tables[19].TableName = "Missing in GSTR2-B2BA";
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    ds.Tables[20].TableName = "Missing in GSTR2-CDNRA";
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
                                    WirteToFile(splittedtables[0], "ModifiedRecord-B2B_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-B2B_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[17], "ModifiedRecord-B2B");
                                }

                                if (ds.Tables[18].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[18].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[18], count);
                                    WirteToFile(splittedtables[0], "ModifiedRecord-CDNR_1");
                                    WirteToFile(splittedtables[1], "ModifiedRecord-CDNR_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[18], "ModifiedRecord-CDNR");
                                }

                                if (ds.Tables[19].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[19].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[19], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-B2BA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-B2BA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[19], "Missing in GSTR2-B2BA");
                                }
                                if (ds.Tables[20].Rows.Count > 800000)
                                {
                                    int count = ds.Tables[20].Rows.Count;
                                    if (count % 2 != 0)
                                    {
                                        count = count + 1;
                                    }
                                    DataTable[] splittedtables = getSpliitedTables(ds.Tables[20], count);
                                    WirteToFile(splittedtables[0], "Missing in GSTR2-CDNRA_1");
                                    WirteToFile(splittedtables[1], "Missing in GSTR2-CDNRA_2");
                                }
                                else
                                {
                                    WirteToFile(ds.Tables[20], "Missing in GSTR2-CDNRA");
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
                                    ds.Tables[17].TableName = "ModifiedRecord-B2B";
                                }
                                if (ds.Tables[18].Rows.Count > 0)
                                {
                                    ds.Tables[18].TableName = "ModifiedRecord-CDNR";
                                }
                                if (ds.Tables[19].Rows.Count > 0)
                                {
                                    ds.Tables[19].TableName = "Missing in GSTR2-B2BA";
                                }
                                if (ds.Tables[20].Rows.Count > 0)
                                {
                                    ds.Tables[20].TableName = "Missing in GSTR2-CDNRA";
                                }
                                CommonFunctions.ExportExcel_XLSX(ds, "ReconciliationLog_For_" + strGSTIN + "_" + strPeriod + "_" + strtoperiod + ".xlsx");
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

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Reset done successfully for the GSTIN : " + strGSTIN + " and FromPeriod : " + strPeriod + " and ToPeriod : " + strtoperiod + " and SupplierName : " + strSupplierName + " and SupplierGSTIN : " + strGSTIN, "");
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


                var jsonresult = Json(new
                {
                    success = true,
                    SuccessMessage = TempData["ReconcilationResponse"],
                    ErrorMessage = TempData["ErrorMessage"],
                    MatchedInvoices = ViewBag.MatchedInvoices,
                    MissinginGSTR2A = ViewBag.MissinginGSTR2A,
                    MissinginGSTR2 = ViewBag.MissinginGSTR2,
                    MismatchInvoices = ViewBag.MismatchInvoices,
                    AcceptedInvoices = ViewBag.AcceptedInvoices,
                    RejectedInvoices = ViewBag.RejectedInvoices,
                    PendingInvoices = ViewBag.PendingInvoices,
                    ModifiedInvoices = ViewBag.ModifiedInvoices,
                    HoldInvoices = ViewBag.HoldInvoices,
                    MatchedTaxAmountGstr2 = ViewBag.MatchedTaxAmountGstr2,
                    MatchedTaxAmountGstr2A = ViewBag.MatchedTaxAmountGstr2A,
                    MatchedTaxableValueGstr2 = ViewBag.MatchedTaxableValueGstr2,
                    MatchedTaxableValueGstr2A = ViewBag.MatchedTaxableValueGstr2A,
                    MissingInGstr2ATaxAmountGstr2 = ViewBag.MissingInGstr2ATaxAmountGstr2,
                    MissingInGstr2ATaxAmountGstr2A = ViewBag.MissingInGstr2ATaxAmountGstr2A,
                    MissingInGstr2ATaxableValueGstr2 = ViewBag.MissingInGstr2ATaxableValueGstr2,
                    MissingInGstr2ATaxableValueGstr2A = ViewBag.MissingInGstr2ATaxableValueGstr2A,
                    MissingInGstr2TaxAmountGstr2 = ViewBag.MissingInGstr2TaxAmountGstr2,
                    MissingInGstr2TaxAmountGstr2A = ViewBag.MissingInGstr2TaxAmountGstr2A,
                    MissingInGstr2TaxableValueGstr2 = ViewBag.MissingInGstr2TaxableValueGstr2,
                    MissingInGstr2TaxableValueGstr2A = ViewBag.MissingInGstr2TaxableValueGstr2A,
                    MismatchTaxAmountGstr2 = ViewBag.MismatchTaxAmountGstr2,
                    MismatchTaxAmountGstr2A = ViewBag.MismatchTaxAmountGstr2A,
                    MismatchTaxableValueGstr2 = ViewBag.MismatchTaxableValueGstr2,
                    MismatchTaxableValueGstr2A = ViewBag.MismatchTaxableValueGstr2A,
                    HoldTaxAmountGSTR2 = ViewBag.HoldTaxAmountGSTR2,
                    HoldTaxAmountGSTR2A = ViewBag.HoldTaxAmountGSTR2A,
                    HoldTaxableValueGSTR2 = ViewBag.HoldTaxableValueGSTR2,
                    HoldTaxableValueGSTR2A = ViewBag.HoldTaxableValueGSTR2A
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
        [HttpPost]
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
                string strGSTINNo = "",Comments="", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
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
                        //string start, end = "";
                        //LoadDropDowns.Financial_year_Dates(out start, out end, year);
                        //string start_date = start;
                        //string end_date = end;

                        //int result = FinancialYearDate(year, fromdate, todate);
                        //    if (result != 1)
                        //    {
                        //    TempData["ErrorMessage"] = "From Date and To Date should be from " + start_date + " to " + end_date;
                        //        return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        //    }


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
            string Accept = "", Reject = "", Pending = "", Trigger = "", Export = "", strRefIds = "";
            DataSet dsMissinginGSTR2A;

            Accept = frm["Accept"];
            Reject = frm["Reject"];
            Pending = frm["Pending"];
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
                // year = frm["years"];

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

                #region "Hold Invoices"
                else if (!string.IsNullOrEmpty(Pending))
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
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "H"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR2A Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Hold Successfully";

                }
                #endregion

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
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Missing_in_GSTR2A_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
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
                        //string start, end = "";
                        //LoadDropDowns.Financial_year_Dates(out start, out end, year);
                        //string start_date = start;
                        //string end_date = end;

                        //int result = FinancialYearDate(year, fromdate, todate);
                        //if (result != 1)
                        //{
                        //    TempData["ErrorMessage"] = "From Date and To Date should be from " + start_date + " to " + end_date;
                        //    return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        //}


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
            string Pending = "", Trigger = "", Export = "", Modify = "", strRefIds = "";
            DataSet dsMismatchInvoices;

            Modify = frm["Modify"];
            Pending = frm["Pending"];
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
                //year = frm["years"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                // Session["YEAR"] = year;
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

                #region "Hold Invoices"
                else if (!string.IsNullOrEmpty(Pending))
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
                            SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_Mismatch_GSTR2and2A", con);
                            dCmd.Parameters.Clear();
                            dCmd.CommandType = CommandType.StoredProcedure;
                            dCmd.CommandTimeout = 0;
                            dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                            dCmd.Parameters.Add(new SqlParameter("@Activity", "G"));
                            dCmd.Parameters.Add(new SqlParameter("@GSTR2invId", Convert.ToInt32(invid[0])));
                            dCmd.Parameters.Add(new SqlParameter("@GSTR2AinvId", Convert.ToInt32(invid[6])));
                            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                            dCmd.ExecuteNonQuery();
                            con.Close();
                        }
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices Kept Pending successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        TempData["ReconcilationResponse"] = "Selected Invoices Hold Successfully";
                    }
                }
                #endregion

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
                //if (string.IsNullOrEmpty(year))
                //{

                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(year);
                //}
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
                        //string start, end = "";
                        //LoadDropDowns.Financial_year_Dates(out start, out end, year);
                        //string start_date = start;
                        //string end_date = end;

                        //int result = FinancialYearDate(year, fromdate, todate);
                        //if (result != 1)
                        //{
                        //    TempData["ErrorMessage"] = "From Date and To Date should be from " + start_date + " to " + end_date;
                        //    return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                        //}

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
            string fileName = "/" + datatableName + ".csv";
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

        public DataTable[] getSpliitedTables(DataTable ds, int count)
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