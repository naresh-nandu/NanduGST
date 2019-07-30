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
using static WeP_DAL.Reconcilation.ReconcilationDataAccess;

namespace SmartAdminMvc.Controllers
{
    public class InvoiceDataCorrectionController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult InvoiceData()
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

        public ActionResult InvoiceData(FormCollection frm)
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
                string Accept = "", Reject = "", Pending = "", Trigger = "", Export = "", Exportsome = "";
                string strRefIds = "";
                DataSet dsInvDataCorrection, dsInvDataCorrection1, dsInvDataCorrection2, dsInvDataCorrection2A;

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
                strInvIds2A = frm["InvIds2A"];
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
                        string[] invid2;
                        string[] invid2A;
                        string gstrid = "", gstr2aid = "";

                        invid2 = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                        invid2A = strInvIds2A.TrimStart(',').TrimEnd(',').Split(',');
                        gstr2aid = string.Join(",", invid2);
                        gstrid = string.Join(",", invid2A);

                        string UpdateType = "INUM";
                        if (strInvIds != null && strInvIds2A != null)
                        {
                            for (int i = 0; i < invid2.Count(); i++)
                            {

                                int result = InvoiceUpdate(strAction, gstr2aid, gstrid, UpdateType,custid);
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

                                result = InvoiceUpdate(strAction, GSTR2AInvoiceNo, GSTR2InvoiceNo, UpdateType,custid);
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

                                result = InvoiceUpdate(strAction, GSTR2AInvoiceNo, GSTR2InvoiceNo, UpdateType,custid);
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
                Session["CF_PERIOD"] = strPeriod;
                Session["CF_TOPERIOD"] = strtoperiod;
                #endregion

                #region "Loading Data"
                    ReconcilationViewModel Model = new ReconcilationViewModel();
                    ReconcilationViewModel Model2 = new ReconcilationViewModel();
                    // ReconcilationViewModel Model2A = new ReconcilationViewModel();


                    if (strFilterType == "2")
                    {
                        if (!string.IsNullOrEmpty(strAction))
                        {
                            List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributes> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributes>();
                            List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedSomeAttributes> LstInvoiceDataCorrectionSome = new List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedSomeAttributes>();
                        if (strRadio == "FinancialPeriod")     //matched invoices
                        {
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_INUM(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                            dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_INUM(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

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
                            dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_inum_idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                                dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_inum_idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
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
                                            LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributes
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
                                                pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                                inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                                ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                                inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                                idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                                val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                                txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                                TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                                pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                                inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                            });
                                        }
                                        foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                        {
                                            LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedSomeAttributes
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
                                                pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                                inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                                ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                                inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                                idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                                val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                                txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                                TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                                pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                                inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                            });
                                        }
                                        break;
                                    case "CDNR":
                                        foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributes
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
                                        foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                        {
                                            LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedSomeAttributes
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
                            List<ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2ttributes> LstInvoiceDataCorrection2 = new List<ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2ttributes>();
                            List<ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2Attributes> LstInvoiceDataCorrection2A = new List<ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2Attributes>();
                            if (strRadio == "FinancialPeriod")       //All Invoices
                            {
                                dsInvDataCorrection2 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                                dsInvDataCorrection2A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                                TempData["InvoiceDataCorrection2"] = dsInvDataCorrection2;
                                TempData["InvoiceDataCorrection2A"] = dsInvDataCorrection2A;
                                Session["f_year"] = "FinancialPeriod";
                                ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                                ViewBag.ToDate = Session["CF_TODATE"].ToString();
                                ViewBag.Period = Session["CF_PERIOD"].ToString();
                                ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
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

                            dsInvDataCorrection2 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2_Idt(strGSTINNo, strSupplierName, strCTIN, strAction,fromdate, todate);
                                dsInvDataCorrection2A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                                TempData["InvoiceDataCorrection2"] = dsInvDataCorrection2;
                                TempData["InvoiceDataCorrection2A"] = dsInvDataCorrection2A;
                                Session["f_year"] = "FinancialYear";
                                ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                                ViewBag.ToDate = Session["CF_TODATE"].ToString();
                                ViewBag.Period = Session["CF_PERIOD"].ToString();
                                ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                            }

                            if (dsInvDataCorrection2.Tables.Count > 0 || dsInvDataCorrection2A.Tables.Count > 0)
                            {
                                switch (strAction)
                                {
                                    case "B2B":
                                        foreach (DataRow dr in dsInvDataCorrection2.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection2.Add(new ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2ttributes
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
                                        foreach (DataRow dr in dsInvDataCorrection2A.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection2A.Add(new ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2Attributes
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
                                        foreach (DataRow dr in dsInvDataCorrection2.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection2.Add(new ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2ttributes
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
                                        foreach (DataRow dr in dsInvDataCorrection2A.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection2A.Add(new ReconcilationDataAccess.InvoiceDataCorrectionAllGSTR2Attributes
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

                            Model2.InvoiceCorrectionAllInvoicesGSTR2 = LstInvoiceDataCorrection2;
                            Model2.InvoiceCorrectionAllInvoicesGSTR2A = LstInvoiceDataCorrection2A;
                            ViewBag.LstInvoiceDataCorrection2 = LstInvoiceDataCorrection2.Count;
                            ViewBag.LstInvoiceDataCorrection2A = LstInvoiceDataCorrection2A.Count;

                            TempData["ExportData"] = dsInvDataCorrection2.Tables[1];
                            TempData["ExportData"] = dsInvDataCorrection2A.Tables[1];
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
                            List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDT> LstInvoiceDataCorrection = new List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDT>();
                            List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDTDuplicate> LstInvoiceDataCorrectionSome = new List<ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDTDuplicate>();
                            if (strRadio == "FinancialPeriod")     //matched invoices
                            {
                                dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                                dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);

                                TempData["InvoiceDataCorrection"] = dsInvDataCorrection;
                                TempData["InvoiceDataCorrectionMatch"] = dsInvDataCorrection1;
                                Session["f_year"] = "FinancialPeriod";
                                ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                                ViewBag.ToDate = Session["CF_TODATE"].ToString();
                                ViewBag.Period = Session["CF_PERIOD"].ToString();
                                ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                            }
                            else                                     //matched invoices
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

                                dsInvDataCorrection = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
                                dsInvDataCorrection1 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_Idt(strGSTINNo, strSupplierName, strCTIN, strAction, fromdate, todate);
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
                                            LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDT
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
                                                pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                                inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                                ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                                inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                                idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                                val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                                txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                                TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                                pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                                inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                            });
                                        }
                                        foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                        {
                                            LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDTDuplicate
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
                                                pos2 = dr.IsNull("pos2") ? "" : dr["pos2"].ToString(),
                                                inv_typ2 = dr.IsNull("inv_typ2") ? "" : dr["inv_typ2"].ToString(),
                                                ctin2a = dr.IsNull("ctin2a") ? "" : dr["ctin2a"].ToString(),
                                                inum2a = dr.IsNull("inum2a") ? "" : dr["inum2a"].ToString(),
                                                idt2a = dr.IsNull("idt2a") ? "" : dr["idt2a"].ToString(),
                                                val2a = dr.IsNull("val2a") ? 0 : Convert.ToDecimal(dr["val2a"]),
                                                txval2a = dr.IsNull("txval2a") ? 0 : Convert.ToDecimal(dr["txval2a"]),
                                                TotaltaxAmount2a = dr.IsNull("TotaltaxAmount2a") ? 0 : Convert.ToDecimal(dr["TotaltaxAmount2a"]),
                                                pos2a = dr.IsNull("pos2a") ? "" : dr["pos2a"].ToString(),
                                                inv_typ2a = dr.IsNull("inv_typ2a") ? "" : dr["inv_typ2a"].ToString()
                                            });
                                        }
                                        break;
                                    case "CDNR":
                                        foreach (DataRow dr in dsInvDataCorrection.Tables[0].Rows)
                                        {
                                            LstInvoiceDataCorrection.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDT
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
                                        foreach (DataRow dr in dsInvDataCorrection1.Tables[2].Rows)
                                        {
                                            LstInvoiceDataCorrectionSome.Add(new ReconcilationDataAccess.InvoiceDataCorrectionMatchedAttributesIDTDuplicate
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

                            Model.InvoiceCorrectionMatchedInvoices_idt = LstInvoiceDataCorrection;
                            Model.InvoiceCorrectionMatchedInvoices_idt_duplicate = LstInvoiceDataCorrectionSome;

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

                if (Session["CF_SUPNAME"] != null && Session["CF_SUPNAME"].ToString() != "ALL")
                {
                    cName = Session["CF_SUPNAME"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, "", "Supplier_Name", Session["CF_SUPNAME"].ToString());
                    if (Session["CF_CTIN"] != null)
                    {
                        Ctin = Session["CF_CTIN"].ToString();
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
                        Ctin = Session["CF_CTIN"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(CustId, UserId, strGStin, fromPeriod, toPeriod, Session["CF_SUPNAME"].ToString(), "", Session["CF_CTIN"].ToString());
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
                    string GSTR2AInvoiceNo = "";
                    string GSTR2InvoiceNo = "";

                    foreach (var id in InvID)
                    {
                        string ids = id.ToString();
                        GSTRInvID = ids.Split(',');
                        GSTR2AInvoiceNo = GSTRInvID[6].ToString();
                        GSTR2InvoiceNo = GSTRInvID[0].ToString();

                        int result = InvoiceUpdate(strAction, GSTR2AInvoiceNo, GSTR2InvoiceNo, UpdateType,custid);
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
                    TempData["Message"] = "Please Select Corresponding Missing Invoice in GSTR2";
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
                ViewBag.CTINNoList = LoadDropDowns.GetGSTR2_2A_CTIN(custid, userid);
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