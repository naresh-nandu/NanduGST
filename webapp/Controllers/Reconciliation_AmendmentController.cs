using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Reconcilation;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_BAL.Common;
using WeP_DAL;
using WeP_DAL.Reconcilation;
using static WeP_DAL.Reconcilation.ReconcilationDataAccess;

namespace SmartAdminMvc.Controllers
{
    public class Reconciliation_AmendmentController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Amendment()
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
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", FromDate = "", ToDate = "";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");

            try
            {
                if (Session["CF_GSTIN"] != null)
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN"].ToString(), Session["Role_Name"].ToString());
                    strGStin = Session["CF_GSTIN"].ToString();
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

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
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME"].ToString());
                    if (Session["CF_CTIN"] != null)
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                else
                {
                    ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, "", "Supplier_Name");
                    if (Session["CF_CTIN"] != null)
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME"].ToString(), "", Session["CF_CTIN"].ToString());
                    }
                    else
                    {
                        ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGStin, fromPeriod, toPeriod, cName, "");
                    }
                }
                Session["action"] = "B2B";

                ViewBag.FilterType = LoadDropDowns.Exist_Amendment_FilterTypet("1");

                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;
            }
            catch (Exception ex)
            {
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult Amendment(FormCollection frm)
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
                string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strFilterType = "", strAction = "", strInvIds = "", strInvIds2A = "", strtoperiod = "", strRadio = "", fromdate = "", todate = "";
                string Accept = "", Export = "", Exportinum = "", Exportidt = "", Export_many = "";
                string strRefIds = "";
                DataSet dsInvDataCorrection, dsInvDataCorrection1;

                Accept = frm["Accept"];
                Export = frm["Export"];
                Exportinum = frm["Exportinum"];
                Exportidt = frm["Exportidt"];
                Export_many = frm["Export_many"];
                strGSTINNo = frm["ddlGSTINNo"];
                strCTIN = frm["ddlSupGSTIN"];
                strPeriod = frm["period"];
                strAction = frm["ddlActionType"];
                strSupplierName = frm["ddlSupplierName"];
                strFilterType = frm["strFilterType"];
                strInvIds = frm["InvIds"];
                strInvIds2A = frm["InvIds2A"];
                strtoperiod = frm["toperiod"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];

                Session["GSTIN"] = strGSTINNo;
                Session["action"] = strAction;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
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

                #region "Accept,Export"

                if (!string.IsNullOrEmpty(Accept))
                {

                    if (strFilterType == "1")
                    {
                        string[] invid;
                        string[] RefIds;

                        string GSTR2AInvoiceNo = "";
                        string GSTR2InvoiceNo = "";
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
                                GSTR2AInvoiceNo = RefIds[6].ToString();
                                GSTR2InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate_GSTR2a_Amendment(strAction, GSTR2InvoiceNo, GSTR2AInvoiceNo, custid,userid);
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

                   else if (strFilterType == "2")
                    {
                        string[] invid;
                        string[] RefIds;

                        string GSTR2AInvoiceNo = "";
                        string GSTR2InvoiceNo = "";
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
                                GSTR2AInvoiceNo = RefIds[6].ToString();
                                GSTR2InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate_GSTR2Amendment(strAction, GSTR2InvoiceNo, GSTR2AInvoiceNo, custid, userid);
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

                        string GSTR2AInvoiceNo = "";
                        string GSTR2InvoiceNo = "";
                        string UpdateType = "INUM";
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
                                GSTR2AInvoiceNo = RefIds[6].ToString();
                                GSTR2InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate_Amend_Inum_Idt(strAction, GSTR2AInvoiceNo, GSTR2InvoiceNo, UpdateType, custid, userid);
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
                        string[] invid;
                        string[] RefIds;

                        string GSTR2AInvoiceNo = "";
                        string GSTR2InvoiceNo = "";
                        string UpdateType = "INUM";

                        if (strInvIds != null)
                        {
                            int result = 0;

                            strInvIds = strInvIds.TrimStart('|').TrimEnd('|');
                            invid = strInvIds.Split('|');
                            for (int i = 0; i < invid.Count(); i++)
                            {
                                string ids = invid[i].ToString();
                                RefIds = ids.Split(',');
                                GSTR2AInvoiceNo = RefIds[6].ToString();
                                GSTR2InvoiceNo = RefIds[0].ToString();

                                result = InvoiceUpdate_Amend_Inum_Idt(strAction, GSTR2AInvoiceNo, GSTR2InvoiceNo,  UpdateType, custid,userid);
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


                }

                else if (!string.IsNullOrEmpty(Export))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }

                else if (!string.IsNullOrEmpty(Exportinum))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }

                else if (!string.IsNullOrEmpty(Exportidt))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");

                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
                    }

                }

                else if (!string.IsNullOrEmpty(Export_many))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xlsx");
                    }

                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Amendments Invoices data exported successfully for the GSTIN : " + strGSTINNo, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "Amendments_" + strGSTINNo + "_" + strAction + "_" + fromdate + "-" + todate + ".xlsx");
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

                #region "Loading Data"
                ReconcilationViewModel Model = new ReconcilationViewModel();
                ReconcilationViewModel Model2 = new ReconcilationViewModel();
                // ReconcilationViewModel Model2A = new ReconcilationViewModel();


                if (strFilterType == "1")
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess.ReconciliationAmendmentGstr2a> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.ReconciliationAmendmentGstr2a>();
                       
                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendmentInvoiceinGSTR2A_Amed(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            //dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_INUM(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                           // TempData["InvoiceDataCorrectionSome"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }
                        else                                     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendmentInvoiceinGSTR2A_Amed_Idt_Wise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                          //  dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_inum_idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            Session["f_year"] = "FinancialYear";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }

                        if (dsInvDataCorrection.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendmentGstr2a
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendmentGstr2a
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.AmendmentGstr2a = LstInvoiceDataCorrection;
                       // Model.InvoiceCorrectionMatchedInvoices_some = LstInvoiceDataCorrectionSome;

                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;
                        TempData["ExportData"] = dsInvDataCorrection.Tables[1];
                    }

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;

                }

                if (strFilterType == "3")
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess.ReconciliationAmendment_Inum> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.ReconciliationAmendment_Inum>();
                        List<ReconcilationDataAccess.ReconciliationAmendment_Inum_Many> LstInvoiceDataCorrection1 = new List<ReconcilationDataAccess.ReconciliationAmendment_Inum_Many>();

                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendment_inum(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetAmendment_inum(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            TempData["InvoiceDataCorrectionSome"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }
                        else                                     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendment_inum_Idt_Wise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetAmendment_inum_Idt_Wise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            TempData["InvoiceDataCorrectionSome"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialYear";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }

                        if (dsInvDataCorrection.Tables.Count > 0 || dsInvDataCorrection1.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendment_Inum
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }

                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrection1.Add(new ReconcilationDataAccess.ReconciliationAmendment_Inum_Many
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }

                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendment_Inum
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            //nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            //nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }

                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrection1.Add(new ReconcilationDataAccess.ReconciliationAmendment_Inum_Many
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            //nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            //nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }

                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.Amendment_Inum = LstInvoiceDataCorrection;
                        Model.Amendment_Inum_Many = LstInvoiceDataCorrection1;

                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;
                        ViewBag.LstInvoiceDataCorrection1 = LstInvoiceDataCorrection1.Count;
                        TempData["ExportData"] = dsInvDataCorrection.Tables[1];
                        TempData["ExportData1"] = dsInvDataCorrection1.Tables[3];
                    }

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                    SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
                    ViewBag.ActionList = Actionlst;
                    var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
                    jsonresult.MaxJsonLength = Int32.MaxValue;
                    return jsonresult;

                }

                if (strFilterType == "4")
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess.ReconciliationAmendment_Idt> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.ReconciliationAmendment_Idt>();
                        List<ReconcilationDataAccess.ReconciliationAmendment_Idt_Many> LstInvoiceDataCorrection1 = new List<ReconcilationDataAccess.ReconciliationAmendment_Idt_Many>();

                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendment_idt(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetAmendment_idt(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            TempData["InvoiceDataCorrectionSome"] = dsInvDataCorrection1;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }
                        else                                     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendment_idt_Idt_Wise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetAmendment_idt_Idt_Wise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            Session["f_year"] = "FinancialYear";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }

                        if (dsInvDataCorrection.Tables.Count > 0 || dsInvDataCorrection1.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendment_Idt
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }

                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrection1.Add(new ReconcilationDataAccess.ReconciliationAmendment_Idt_Many
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendment_Idt
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            //nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            //nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }

                                    foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                    {
                                        LstInvoiceDataCorrection1.Add(new ReconcilationDataAccess.ReconciliationAmendment_Idt_Many
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                           // nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            //nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            //inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            //idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.Amendment_Idt = LstInvoiceDataCorrection;
                        Model.Amendment_Idt_Many = LstInvoiceDataCorrection1;

                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;
                        ViewBag.LstInvoiceDataCorrection1 = LstInvoiceDataCorrection1.Count;
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

                else
                {
                    if (!string.IsNullOrEmpty(strAction))
                    {
                        List<ReconcilationDataAccess.ReconciliationAmendmentGstr2_2a> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.ReconciliationAmendmentGstr2_2a>();
                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendmentInvoiceinGSTR2_2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            Session["f_year"] = "FinancialPeriod";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }
                        else                                     //matched invoices
                        {

                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetAmendmentInvoiceinGSTR2_2A_IdtWise(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);

                            TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                            Session["f_year"] = "FinancialYear";
                            ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                            ViewBag.ToDate = Session["CF_TODATE"].ToString();
                            ViewBag.Period = Session["CF_PERIOD"].ToString();
                            ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                        }

                        if (dsInvDataCorrection.Tables.Count > 0)
                        {
                            switch (strAction)
                            {
                                case "B2B":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendmentGstr2_2a
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                            inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            oinum2a = dr.IsNull("oinum2a") ? "" : dr["oinum2a"].ToString(),
                                            oidt2a = dr.IsNull("oidt2a") ? "" : dr["oidt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                            pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                            inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                        });
                                    }
                                    break;
                                case "CDNR":
                                    foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                    {
                                        LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.ReconciliationAmendmentGstr2_2a
                                        {
                                            invid2 = dr.IsNull("invid2") ? 0 : Convert.ToInt32(dr["invid2"]),
                                            ctin_inum = dr.IsNull("ctin_inum") ? "" : dr["ctin_inum"].ToString(),
                                            gstin2 = dr.IsNull("gstin2") ? "" : dr["gstin2"].ToString(),
                                            ctin2 = dr.IsNull("ctin2") ? "" : dr["ctin2"].ToString(),
                                            CtinName2 = dr.IsNull("CtinName2") ? "" : dr["CtinName2"].ToString(),
                                            nt_num2 = dr.IsNull("nt_num2") ? "" : dr["nt_num2"].ToString(),
                                            nt_dt2 = dr.IsNull("nt_dt2") ? "" : dr["nt_dt2"].ToString(),
                                            inum2 = dr.IsNull("inum2") ? "" : dr["inum2"].ToString(),
                                            idt2 = dr.IsNull("idt2") ? "" : dr["idt2"].ToString(),
                                            val2 = dr.IsNull("val2") ? 0 : Convert.ToDecimal(dr["val2"]),
                                            txval2 = dr.IsNull("txval2") ? 0 : Convert.ToDecimal(dr["txval2"]),
                                            iamt2 = dr.IsNull("iamt2") ? 0 : Convert.ToDecimal(dr["iamt2"]),
                                            camt2 = dr.IsNull("camt2") ? 0 : Convert.ToDecimal(dr["camt2"]),
                                            Samt2 = dr.IsNull("Samt2") ? 0 : Convert.ToDecimal(dr["Samt2"]),
                                            Csamt2 = dr.IsNull("Csamt2") ? 0 : Convert.ToDecimal(dr["Csamt2"]),
                                            TotaltaxAmount2 = dr.IsNull("TotaltaxAmount2") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2"]),
                                            ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                            nt_num2a = dr.IsNull("nt_num2a") ? "" : dr["nt_num2a"].ToString(),
                                            nt_dt2a = dr.IsNull("nt_dt2a") ? "" : dr["nt_dt2a"].ToString(),
                                            ont_num2a = dr.IsNull("ont_num2a") ? "" : dr["ont_num2a"].ToString(),
                                            ont_dt2a = dr.IsNull("ont_dt2a") ? "" : dr["ont_dt2a"].ToString(),
                                            inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                            idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                            val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                            txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                            iamt2a = dr.IsNull("iamt2a") ? 0 : Convert.ToDecimal(dr["iamt2a"]),
                                            camt2a = dr.IsNull("camt2a") ? 0 : Convert.ToDecimal(dr["camt2a"]),
                                            Samt2a = dr.IsNull("Samt2a") ? 0 : Convert.ToDecimal(dr["Samt2a"]),
                                            Csamt2a = dr.IsNull("Csamt2a") ? 0 : Convert.ToDecimal(dr["Csamt2a"]),
                                            TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"])
                                        });
                                    }
                                    break;
                                default:
                                    break;
                            }
                        }

                        Model.AmendmentGstr2_2a = LstInvoiceDataCorrection;
                        ViewBag.LstInvoiceDataCorrection = LstInvoiceDataCorrection.Count;

                        TempData["ExportData"] = dsInvDataCorrection.Tables[1];
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
            return View();
        }

        public int InvoiceUpdate(string ActionType, string FromInvID, string ToInvID, string UpdateType, int custid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_GSTR2_Invoice_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = ActionType;
                cmd.Parameters.Add("@FromInvIds", SqlDbType.NVarChar).Value = FromInvID;
                cmd.Parameters.Add("@ToInvIds", SqlDbType.NVarChar).Value = ToInvID;
                cmd.Parameters.Add("@UpdateType", SqlDbType.VarChar).Value = UpdateType;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar).Value = custid;
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

        public int InvoiceUpdate_GSTR2Amendment(string ActionType, string ToInvID, string FromInvID, int custid, int userid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_GSTR2_Invoice_Data_From_Gstr2A_Amendment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = ActionType;
                cmd.Parameters.Add("@Gstr2Invid", SqlDbType.VarChar).Value = ToInvID;
                cmd.Parameters.Add("@Gstr2AInvId", SqlDbType.VarChar).Value = FromInvID;
                //cmd.Parameters.Add("@Gstin", SqlDbType.VarChar).Value = UpdateType;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar).Value = custid;
                cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userid;
                cmd.Parameters.Add("@ErrorCode", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@ErrorMessage", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
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

        public int InvoiceUpdate_GSTR2a_Amendment(string ActionType, string ToInvID, string FromInvID,  int custid, int userid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_GSTR2A_Invoice_Data_From_Gstr2A_Amendment", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = ActionType;
                cmd.Parameters.Add("@Gstr2Invid", SqlDbType.VarChar).Value = ToInvID;
                cmd.Parameters.Add("@Gstr2AInvId", SqlDbType.VarChar).Value = FromInvID;
               // cmd.Parameters.Add("@Gstin", SqlDbType.VarChar).Value = UpdateType;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar).Value = custid;
                cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userid;
                cmd.Parameters.Add("@ErrorCode", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
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

        public int InvoiceUpdate_Amend_Inum_Idt(string ActionType, string ToInvID, string FromInvID, string UpdateType, int custid, int userid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_GSTR2_Invoice_Data_FromAmendment_INUM_IDT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add("@ActionType", SqlDbType.VarChar).Value = ActionType;
                cmd.Parameters.Add("@FromInvIds", SqlDbType.VarChar).Value = ToInvID;
                cmd.Parameters.Add("@ToInvIds", SqlDbType.VarChar).Value = FromInvID;
                cmd.Parameters.Add("@UpdateType", SqlDbType.VarChar).Value = UpdateType;
                cmd.Parameters.Add("@custid", SqlDbType.VarChar).Value = custid;
                //cmd.Parameters.Add("@UserId", SqlDbType.VarChar).Value = userid;
                cmd.Parameters.Add("@ErrorCode", SqlDbType.SmallInt).Direction = ParameterDirection.Output;
                //cmd.Parameters.Add("@ErrorMessage", SqlDbType.VarChar).Direction = ParameterDirection.Output;
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

    }
}