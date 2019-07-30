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
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;
using WeP_DAL;
using WeP_DAL.Reconcilation;
using static WeP_DAL.Reconcilation.ReconcilationDataAccess6;

namespace SmartAdminMvc.Controllers
{
    public class InvoiceDataCorrection_GSTR6Controller : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult InvoiceDataGSTR6()
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
                if (Session["CF_GSTIN_GSTR6"] != null)
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                    strGStin = Session["CF_GSTIN_GSTR6"].ToString();
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

                }
                if (Session["CF_PERIOD_GSTR6"] != null)
                {
                    ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                    fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                }
                if (Session["CF_TOPERIOD_GSTR6"] != null)
                {
                    ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                    toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
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

                if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME_GSTR6"].ToString());
                    if (Session["CF_CTIN"] != null)
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN"].ToString());
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
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                Session["action"] = "B2B";

                ViewBag.FilterType = LoadDropDowns.Exist_FilterTypet("2");

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

        public ActionResult InvoiceDataGSTR6(FormCollection frm)
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
                string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strFilterType = "", strAction = "", strInvIds = "", strInvIds6A = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
                string Accept = "", Reject = "", Pending = "", Trigger = "", Export = "", Exportsome = "";
                string strRefIds = "";
                DataSet dsInvDataCorrection, dsInvDataCorrection1, dsInvDataCorrection6, dsInvDataCorrection6A;

                Accept = frm["Accept"];
                Reject = frm["Reject"];
                Pending = frm["Pending"];
                Trigger = frm["Trigger"];
                Export = frm["Export"];
                Exportsome = frm["Exportsome"];

                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strFilterType = frm["strFilterType"];
                strInvIds = frm["InvIds"];
                strInvIds6A = frm["InvIds6A"];
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
                Session["strFilterType"] = strFilterType;
                ViewBag.Period = strPeriod;
                ViewBag.ToPeriod = strtoperiod;
                ViewBag.FilterType = LoadDropDowns.Exist_FilterTypet(strFilterType);

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

                #region "Accept"

                if (!string.IsNullOrEmpty(Accept))
                {
                    if (strFilterType == "1")
                    {
                        string[] invid6;
                        string[] invid6A;
                        string gstrid = "", gstr6aid = "";

                        invid6 = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                        invid6A = strInvIds6A.TrimStart(',').TrimEnd(',').Split(',');
                        gstr6aid = string.Join(",", invid6);
                        gstrid = string.Join(",", invid6A);

                        string UpdateType = "INUM";
                        if (strInvIds != null && strInvIds6A != null)
                        {
                            for (int i = 0; i < invid6.Count(); i++)
                            {

                                int result = InvoiceUpdate(strAction, gstr6aid, gstrid, UpdateType, custid);
                                if (result == 1)
                                {
                                    TempData["ReconcilationResponse"] = "Invoice Updated Successfully";
                                }
                                else if (result == -2)
                                {
                                    TempData["ReconcilationResponse"] = "Unable to update as the other field values of the invoice do not match";
                                }
                                else
                                {
                                    TempData["ReconcilationResponse"] = "Invoice Update Failure";
                                }
                            }
                        }
                        else
                        {
                            TempData["ReconcilationResponse"] = "Please Select Proper Invoices.";
                        }

                    }

                    else if (strFilterType == "2")
                    {
                        string[] invid;
                        string[] RefIds;

                        string UpdateType = "INUM";
                        string GSTR6AInvoiceNo = "";
                        string GSTR6InvoiceNo = "";
                        // invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                        if (strInvIds != null)
                        {
                            int result = 0;

                            strInvIds = strInvIds.TrimStart('|').TrimEnd('|');
                            invid = strInvIds.Split('|');
                            for (int i = 0; i < invid.Count(); i++)
                            {
                                string ids = invid[i].ToString();
                                RefIds = ids.Split(',');
                                GSTR6AInvoiceNo = RefIds[6].ToString();
                                GSTR6InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate(strAction, GSTR6AInvoiceNo, GSTR6InvoiceNo, UpdateType, custid);
                            }
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices modified successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            if (result == 1)
                            {
                                TempData["ReconcilationResponse"] = "Invoice Updated Successfully";
                            }
                            else if (result == -2)
                            {
                                TempData["ReconcilationResponse"] = "Unable to update as the other field values of the invoice do not match";
                            }
                            else
                            {
                                TempData["ReconcilationResponse"] = "Invoice Update Failure";
                            }
                        }

                    }

                    else if (strFilterType == "3")
                    {
                        string[] invid;
                        string[] RefIds;

                        string UpdateType = "INUM";
                        string GSTR6AInvoiceNo = "";
                        string GSTR6InvoiceNo = "";
                        // invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');

                        if (strInvIds != null)
                        {
                            int result = 0;

                            strInvIds = strInvIds.TrimStart('|').TrimEnd('|');
                            invid = strInvIds.Split('|');
                            for (int i = 0; i < invid.Count(); i++)
                            {
                                string ids = invid[i].ToString();
                                RefIds = ids.Split(',');
                                GSTR6AInvoiceNo = RefIds[6].ToString();
                                GSTR6InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate(strAction, GSTR6AInvoiceNo, GSTR6InvoiceNo, UpdateType, custid);
                            }
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices modified successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                            if (result == 1)
                            {
                                TempData["ReconcilationResponse"] = "Invoice Updated Successfully";
                            }
                            else if (result == -2)
                            {
                                TempData["ReconcilationResponse"] = "Unable to update as the other field values of the invoice do not match";
                            }
                            else
                            {
                                TempData["ReconcilationResponse"] = "Invoice Update Failure";
                            }
                        }
                    }

                    else
                    {
                        TempData["ReconcilationResponse"] = "Please Select Corresponding Invoices";
                    }

                }

                else if (!string.IsNullOrEmpty(Export))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "InvoiceDataCorrection Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "InvoiceDataCorrection_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "InvoiceDataCorrection Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "InvoiceDataCorrection_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }

                else if (!string.IsNullOrEmpty(Exportsome))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "InvoiceDataCorrection Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "InvoiceDataCorrection_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");
                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "InvoiceDataCorrection Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "InvoiceDataCorrection_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
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
                Session["CF_PERIOD_GSTR6"] = strPeriod;
                Session["CF_TOPERIOD_GSTR6"] = strtoperiod;
                #endregion

                #region "Loading Data"
                ReconcilationViewModel6 Model = new ReconcilationViewModel6();
                ReconcilationViewModel6 Model2 = new ReconcilationViewModel6();


                if (strFilterType == "2")
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributes> LstInvoiceDataCorrection = new List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributes>();
                        List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedManyAttributes> LstInvoiceDataCorrectionSome = new List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedManyAttributes>();
                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6and6A_INUM(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6and6A_INUM(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            TempData["InvoiceDataCorrectionSome"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        }
                        else                                     //matched invoices
                        {
                            dsInvDataCorrection = null;
                            dsInvDataCorrection1 = null;
                            
                        }

                        if (dsInvDataCorrection.Tables.Count > 0 || dsInvDataCorrection1.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributes
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            pos6 = dr.IsNull("pos6") ? "" : dr["pos6"].ToString(),
                                            pos6a = dr.IsNull("pos6a") ? "" : dr["pos6a"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                            
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedManyAttributes
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            pos6 = dr.IsNull("pos6") ? "" : dr["pos6"].ToString(),
                                            pos6a = dr.IsNull("pos6a") ? "" : dr["pos6a"].ToString(),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributes
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            nt_num6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            nt_dt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            nt_num6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            nt_dt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedManyAttributes
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            nt_num6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            nt_dt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            nt_num6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            nt_dt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.InvoiceCorrectionMatchedInvoices = LstInvoiceDataCorrection;
                        Model.InvoiceCorrectionMatchedInvoices_some = LstInvoiceDataCorrectionSome;

                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;
                        ViewBag.LstInvoiceDataCorrectionSome = LstInvoiceDataCorrectionSome.Count;

                        TempData["ExportData"] = dsInvDataCorrection.Tables[1];
                        TempData["ExportData1"] = dsInvDataCorrection.Tables[3];
                    }

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;

                }

                else if (strFilterType == "1")
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess6.MissingInGstr6Attributes> LstInvoiceDataCorrection6 = new List<ReconcilationDataAccess6.MissingInGstr6Attributes>();
                        List<ReconcilationDataAccess6.MissingInGstr6AAttributes> LstInvoiceDataCorrection6A = new List<ReconcilationDataAccess6.MissingInGstr6AAttributes>();
                        if (strRadio == "FinancialPeriod")       //All Invoices
                        {
                            dsInvDataCorrection6 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection6A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            TempData["InvoiceDataCorrection2"] = dsInvDataCorrection6;
                            TempData["InvoiceDataCorrection2A"] = dsInvDataCorrection6A;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        }
                        else                                      //All Invoices
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

                            //dsInvDataCorrection2 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            //dsInvDataCorrection2A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            //TempData["InvoiceDataCorrection2"] = dsInvDataCorrection2;
                            //TempData["InvoiceDataCorrection2A"] = dsInvDataCorrection2A;
                            //Session["f_year"] = "FinancialYear";
                            //ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            //ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            //ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                            //ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();

                            dsInvDataCorrection6 = null;
                            dsInvDataCorrection6A = null;
                        }

                        if (dsInvDataCorrection6.Tables.Count > 0 || dsInvDataCorrection6A.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection6.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection6.Add(new ReconcilationDataAccess6.MissingInGstr6Attributes
                                        {
                                            
                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(), 
                                            CtinName = dr.IsNull("CtinName") ? "" : dr["CtinName"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(), 
                                            cfs = dr.IsNull("cfs") ? "" : dr["cfs"].ToString(),
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                            camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                            samt= dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                            csamt= dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                           
                                            
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection6A.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection6A.Add(new ReconcilationDataAccess6.MissingInGstr6AAttributes
                                        {
                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                            CtinName = dr.IsNull("CtinName") ? "" : dr["CtinName"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),                                           
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                            camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                            samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                            csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),

                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection6.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection6.Add(new ReconcilationDataAccess6.MissingInGstr6Attributes
                                        {

                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                            CtinName = dr.IsNull("CtinName") ? "" : dr["CtinName"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                            nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                                            cfs = dr.IsNull("cfs") ? "" : dr["cfs"].ToString(),
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                            camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                            samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                            csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection6A.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection6A.Add(new ReconcilationDataAccess6.MissingInGstr6AAttributes
                                        {
                                            invid = dr.IsNull("invid") ? 0 : Convert.ToInt32(dr["invid"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                                            ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                                            CtinName = dr.IsNull("CtinName") ? "" : dr["CtinName"].ToString(),
                                            inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                                            nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),                                           
                                            idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                                            nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                                            val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                                            iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                            camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                            samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                            csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                                            txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                                            TotaltaxAmount = dr.IsNull("TotaltaxAmount") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount"]),
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Model2.MissingInvoicesInGSTR6 = LstInvoiceDataCorrection6;
                        Model2.MissingInvoicesInGSTR6A = LstInvoiceDataCorrection6A;
                        ViewBag.LstInvoiceDataCorrection6 = LstInvoiceDataCorrection6.Count;
                        ViewBag.LstInvoiceDataCorrection6A = LstInvoiceDataCorrection6A.Count;

                        TempData["ExportData"] = dsInvDataCorrection6.Tables[1];
                        TempData["ExportData"] = dsInvDataCorrection6A.Tables[1];
                    }
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model2, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;
                }

                else
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDT> LstInvoiceDataCorrection = new List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDT>();
                        List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDTMany> LstInvoiceDataCorrectionSome = new List<ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDTMany>();
                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6and6A_IDT(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6and6A_IDT(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            TempData["InvoiceDataCorrectionMatch"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        }
                        else                                     //matched invoices
                        {


                            dsInvDataCorrection = null;
                            dsInvDataCorrection1 = null;
                           
                        }

                        if (dsInvDataCorrection.Tables.Count > 0 || dsInvDataCorrection1.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDT
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDTMany
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDT
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            nt_num6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            nt_dt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            nt_num6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            nt_dt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess6.InvoiceDataCorrectionMatchedAttributesIDTMany
                                        {
                                            invid6 = dr.IsNull("invid6") ? 0 : Convert.ToInt32(dr["invid6"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin6 = dr.IsNull("gstin6") ? "" : dr["gstin6"].ToString(),
                                            CtinName6 = dr.IsNull("CtinName6") ? "" : dr["CtinName6"].ToString(),
                                            ctin6 = dr.IsNull("ctin6") ? "" : dr["ctin6"].ToString(),
                                            nt_num6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            nt_dt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            val6 = dr.IsNull("val6") ? 0 : Convert.ToDecimal(dr["val6"]),
                                            inum6 = dr.IsNull("inum6") ? "" : dr["inum6"].ToString(),
                                            idt6 = dr.IsNull("idt6") ? "" : dr["idt6"].ToString(),
                                            txval6 = dr.IsNull("txval6") ? 0 : Convert.ToDecimal(dr["txval6"]),
                                            TotaltaxAmount6 = dr.IsNull("TotaltaxAmount6") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6"]),
                                            iamt6 = dr.IsNull("iamt6") ? 0 : Convert.ToDecimal(dr["iamt6"]),
                                            camt6 = dr.IsNull("camt6") ? 0 : Convert.ToDecimal(dr["camt6"]),
                                            samt6 = dr.IsNull("samt6") ? 0 : Convert.ToDecimal(dr["samt6"]),
                                            csamt6 = dr.IsNull("csamt6") ? 0 : Convert.ToDecimal(dr["csamt6"]),
                                            CtinName6a = dr.IsNull("CtinName6a") ? "" : dr["CtinName6a"].ToString(),
                                            ctin6a = dr.IsNull("ctin6a") ? "" : dr["ctin6a"].ToString(),
                                            nt_num6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            nt_dt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            inum6a = dr.IsNull("inum6a") ? "" : dr["inum6a"].ToString(),
                                            idt6a = dr.IsNull("idt6a") ? "" : dr["idt6a"].ToString(),
                                            iamt6a = dr.IsNull("iamt6a") ? 0 : Convert.ToDecimal(dr["iamt6a"]),
                                            camt6a = dr.IsNull("camt6a") ? 0 : Convert.ToDecimal(dr["camt6a"]),
                                            samt6a = dr.IsNull("samt6a") ? 0 : Convert.ToDecimal(dr["samt6a"]),
                                            csamt6a = dr.IsNull("csamt6a") ? 0 : Convert.ToDecimal(dr["csamt6a"]),
                                            val6a = dr.IsNull("val6a") ? 0 : Convert.ToDecimal(dr["val6a"]),
                                            txval6a = dr.IsNull("txval6a") ? 0 : Convert.ToDecimal(dr["txval6a"]),
                                            TotaltaxAmount6a = dr.IsNull("TotaltaxAmount6a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount6a"])
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.InvoiceCorrectionMatchedInvoices_idt = LstInvoiceDataCorrection;
                        Model.InvoiceCorrectionMatchedInvoices_idt_many = LstInvoiceDataCorrectionSome;

                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;
                        ViewBag.LstInvoiceDataCorrectionSome = LstInvoiceDataCorrectionSome.Count;

                        TempData["ExportData"] = dsInvDataCorrection.Tables[1];
                        TempData["ExportData1"] = dsInvDataCorrection.Tables[3];
                    }
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;
                }

                #endregion

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult InvoiceValue()
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
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";

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
                if (Session["CF_PERIOD_GSTR6"] != null)
                {
                    ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                    fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                }
                if (Session["CF_TOPERIOD_GSTR6"] != null)
                {
                    ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                    toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                }

                if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME_GSTR6"].ToString());
                    if (Session["CF_CTIN"] != null)
                    {
                        Ctin = Session["CF_CTIN"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN"].ToString());
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
                        Ctin = Session["CF_CTIN"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME_GSTR6"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                Session["action"] = "B2B";
                var GridFill = new ReconcilationDataModel(CustId, UserId).GetMismatchInvoice(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                var dt = new ReconcilationDataModel(CustId, UserId).DT_GetMismatchInvoice(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                TempData["ExportData"] = dt;
                ViewBag.GridFill = GridFill;
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

        public ActionResult InvoiceValue(FormCollection frm, string Accept, string[] InvID, string Export)
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

            string strGSTINNo = frm["ddlGSTINNo"];
            string strCTIN = frm["ddlSupGSTIN"];
            string strPeriod = frm["period"];
            string strAction = frm["ddlActionType"];
            string strCname = frm["ddlSupplierName"];
            string strtoperiod = frm["toperiod"];

            Session["action"] = strAction;
            Session["GSTIN"] = strGSTINNo;
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
                TempData["Message"] = "To Date should always be greater than or equal to from Date";
            }

            if (!string.IsNullOrEmpty(Accept))
            {
                if (InvID != null)
                {
                    string[] GSTRInvID;
                    string UpdateType = "IVAL";
                    string GSTR6AInvoiceNo = "";
                    string GSTR6InvoiceNo = "";

                    foreach (var id in InvID)
                    {
                        string ids = id.ToString();
                        GSTRInvID = ids.Split(',');
                        GSTR6AInvoiceNo = GSTRInvID[6].ToString();
                        GSTR6InvoiceNo = GSTRInvID[0].ToString();

                        int result = InvoiceUpdate(strAction, GSTR6AInvoiceNo, GSTR6InvoiceNo, UpdateType, custid);
                        if (result == 1)
                        {
                            TempData["Message"] = "Invoice Updated Successfully";
                        }
                        else if (result == -2)
                        {
                            TempData["Message"] = "Unable to update as the other field values of the invoice do not match";
                        }
                        else if (result == -3)
                        {
                            TempData["Message"] = "Unable to update the invoice value";
                        }
                        else
                        {
                            TempData["Message"] = "Invoice Update Failure";
                        }
                    }
                }
                else
                {
                    TempData["Message"] = "Please Select Corresponding Missing Invoice in GSTR6";
                }
            }

            else if (!string.IsNullOrEmpty(Export))
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["ExportData"] as DataTable;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=ValueCorrection_Invoices.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(custid, userid);
                SelectList Actionlist = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName");
                ViewBag.ActionList = Actionlist;
                return View();
            }
            else
            {
                //
            }


            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            }

            if (string.IsNullOrEmpty(strCname))
            {
                strCname = "ALL";
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strCname, "");
            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strCname);
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strCname, "");
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strCname, "");
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strCname, "", strCTIN);
            }
            ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strCname, "", strCTIN);


            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["Cname"] = strCname;
            TempData["ToPeriod"] = strtoperiod;
            if (strAction != "")
            {
                var GridFill = new ReconcilationDataModel(custid, userid).GetMismatchInvoice(strGSTINNo, strCname, strCTIN, strPeriod, strAction, strtoperiod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMismatchInvoice(strGSTINNo, strCname, strCTIN, strPeriod, strAction, strtoperiod);
                TempData["ExportData"] = dt;
                ViewBag.GridFill = GridFill;
            }

            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;


            return View();
        }
        public int InvoiceUpdate(string ActionType, string FromInvID, string ToInvID, string UpdateType, int custid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_GSTR6_Invoice_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = ActionType;
                cmd.Parameters.Add("@FromInvIds", SqlDbType.NVarChar).Value = FromInvID;
                cmd.Parameters.Add("@ToInvIds", SqlDbType.NVarChar).Value = ToInvID;
                cmd.Parameters.Add("@UpdateType", SqlDbType.VarChar).Value = UpdateType;
                
                cmd.Parameters.Add("@ErrorCode", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@ErrorCode"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
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
    }
}