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
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.Common;
using WeP_DAL;
using WeP_DAL.Reconcilation;
using static WeP_DAL.Reconcilation.ReconcilationDataAccess;

namespace SmartAdminMvc.Controllers
{
    public class InvoiceCorrectionController : Controller
    {

        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        [HttpGet]
        [AcceptVerbs(HttpVerbs.Get)]
        public ActionResult InvoiceCorrection()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string fromPeriod = "", toPeriod = "", strGstin = "", FromDate = "", ToDate = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            try
            {
                #region "dropdown loading and variable intilization"


                if (Session["CF_GSTIN"] != null)
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN"].ToString(), Session["Role_Name"].ToString());
                    strGstin = Session["CF_GSTIN"].ToString();
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
                else
                {
                    ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                    fromPeriod = DateTime.Now.ToString("MMyyyy");
                }
                if (Session["CF_TOPERIOD"] != null)
                {
                    ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    toPeriod = Session["CF_TOPERIOD"].ToString();
                }
                else
                {
                    ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
                    toPeriod = DateTime.Now.ToString("MMyyyy");
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

                #endregion
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        [HttpPost]
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult InvoiceCorrection(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            try
            {
                string fromPeriod = "", toPeriod = "", strGstin = "", newctin = "", strRadio = "", fromdate = "", todate = "",ctin = "",val = "", gstinUpdate = "";
                string Export = "", Upload = "",Export_B2B,Export_CDNR;
                string strRefIds = "";
                DataSet dsReconSummary;

                Export = frm["Export"];
                Export_B2B = frm["Export_b2b"];
                Export_CDNR = frm["Export_cdn"];
                Upload = frm["Upload"];

                gstinUpdate = frm["ddlGSTINNoUpdate"];
                strGstin = frm["ddlGSTINNo"];
                fromPeriod = frm["period"];
                toPeriod = frm["toperiod"];
                newctin = frm["newctin"];
                strRefIds = frm["RefIds"];
                strRadio = frm["reconsetting"];
                fromdate = frm["fromdate"];
                todate = frm["todate"];
                //year = frm["years"];
                ctin = frm["ctin"];
                val = frm["gstr2val"];

                Session["CF_GSTIN"] = strGstin;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
               // Session["YEAR"] = year;
                Session["CF_PERIOD"] = fromPeriod;
                Session["CF_TOPERIOD"] = toPeriod;
                ViewBag.Period = fromPeriod;
                ViewBag.ToPeriod = toPeriod;

                Session["Radio"] = strRadio;



                String fromm = DateTime.ParseExact(fromPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                String too = DateTime.ParseExact(toPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
                DateTime from_dt = Convert.ToDateTime(fromm);
                DateTime to_dt = Convert.ToDateTime(too);

                String diff = (to_dt - from_dt).TotalDays.ToString();
                int diff_dt = Convert.ToInt32(diff);
                if (diff_dt < 0)
                {
                    TempData["Message"] = "To Date should always be greater than or equal to from Date";
                }

               

                else if (!string.IsNullOrEmpty(Export))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin + " and Period : " + fromPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "ReconciliationSummary_" + strGstin + "_" + fromPeriod + "_" + toPeriod + ".xlsx");
                    }
                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData"] as DataTable, "ReconciliationSummary_" + strGstin + "_" + fromdate + "_" + todate + ".xlsx");
                    }
                }

                else if (!string.IsNullOrEmpty(Export_B2B))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin + " and Period : " + fromPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "ReconciliationSummary_B2B_" + strGstin + "_" + fromPeriod + "_" + toPeriod + ".xlsx");
                    }
                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData1"] as DataTable, "ReconciliationSummary_B2B_" + strGstin + "_" + fromdate + "_" + todate + ".xlsx");
                    }
                }

                else if (!string.IsNullOrEmpty(Export_CDNR))
                {

                    if (strRadio == "FinancialPeriod")
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin + " and Period : " + fromPeriod, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData2"] as DataTable, "ReconciliationSummary_CDNR_" + strGstin + "_" + fromPeriod + "_" + toPeriod + ".xlsx");
                    }
                    else
                    {
                        Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGstin, "");
                        CommonFunctions.ExportExcel_XLSX(TempData["ExportData2"] as DataTable, "ReconciliationSummary_CDNR_" + strGstin + "_" + fromdate + "_" + todate + ".xlsx");
                    }
                }

                else if (!string.IsNullOrEmpty(Upload))
                {
                    if(strRadio == "FinancialPeriod")
                    { 
                        var ctin_validation = new ReconcilationDataModel(custid, userid).ctin_Validation(gstinUpdate, fromPeriod, toPeriod, ctin, newctin);
                        TempData["ReconcilationResponse"] = "Supplier GSTIN updated Successsfully.";
                    }
                    else
                    {
                        var ctin_validation = new ReconcilationDataModel(custid, userid).ctin_Validation_IdtWise(gstinUpdate, fromdate, todate, ctin, newctin);
                        TempData["ReconcilationResponse"] = "Supplier GSTIN updated Successsfully.";
                    }
                }
               
                else
                {
                    //
                }

                #region "Dropdown Loading"
                if (string.IsNullOrEmpty(strGstin))
                {
                    strGstin = "ALL";
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGstin, Session["Role_Name"].ToString());
                }
                //if (string.IsNullOrEmpty(year))
                //{

                //    ViewBag.yearlist = LoadDropDowns.Financial_year();
                //}
                //else
                //{
                //    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(year);
                //}

                Session["CF_GSTIN"] = strGstin;
                Session["CF_PERIOD"] = fromPeriod;
                Session["CF_TOPERIOD"] = toPeriod;
                Session["CF_FROMDATE"] = fromdate;
                Session["CF_TODATE"] = todate;
                #endregion

                #region "Data Loading"
                ReconcilationViewModel Model = new ReconcilationViewModel();
                List<ReconcilationDataAccess.ReconciliationSummaryAttributes> LstReconSummary = new List<ReconcilationDataAccess.ReconciliationSummaryAttributes>();
                if (strRadio != "FinancialYear")
                {
                    if (ctin == null || val == "0" || val == "0.00" && Upload == null)
                    {
                        TempData["ErrorMessage"] = "Can not Update Supplier GSTIN.";
                    }
                    else if (ctin != null || val != "0" || val != "0.00" && Upload == null)
                    {
                        ViewBag.ctinValidation = ctin;
                        Session["CTIN"] = ctin;
                    }
                    else
                    {

                    }
                    dsReconSummary = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_CTIN_wise(strGstin, fromPeriod, toPeriod);
                    TempData["ReconciliationSummary"] = dsReconSummary;
                    Session["f_year"] = "FinancialPeriod";
                    ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                    ViewBag.ToDate = Session["CF_TODATE"].ToString();
                    ViewBag.Period = Session["CF_PERIOD"].ToString();
                    ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();

                }
                else
                {
                    if (ctin == null || val == "0" || val == "0.00" && Upload == null)
                    {
                        TempData["ErrorMessage"] = "Can not Update Supplier GSTIN.";
                    }
                    else if (ctin != null || val != "0" || val != "0.00" && Upload == null)
                    {
                        ViewBag.ctinValidation = ctin;
                        Session["CTIN"] = ctin;
                    }

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

                    dsReconSummary = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR2and2A_CTIN_wise_idt_wise(strGstin, fromdate, todate);
                    TempData["ReconciliationSummary"] = dsReconSummary;
                    Session["f_year"] = "FinancialYear";
                    ViewBag.Period = Session["CF_PERIOD"].ToString();
                    ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                    ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                    ViewBag.ToDate = Session["CF_TODATE"].ToString();

                }

                if (dsReconSummary.Tables.Count > 0)
                {
                    foreach (DataRow dr in dsReconSummary.Tables[0].Rows)
                    {
                        LstReconSummary.Add(new ReconcilationDataAccess.ReconciliationSummaryAttributes
                        {
                            gstin = dr.IsNull("gstin") ? "" : dr["gstin"].ToString(),
                            SupplierGstin = dr.IsNull("SupplierGstin") ? "" : dr["SupplierGstin"].ToString(),
                            SupplierName = dr.IsNull("SupplierName") ? "" : dr["SupplierName"].ToString(),
                            GSTR2Val = dr.IsNull("GSTR2Val") ? 0 : Convert.ToDecimal(dr["GSTR2Val"]),
                            GSTR2Taxamount = dr.IsNull("GSTR2Taxamount") ? 0 : Convert.ToDecimal(dr["GSTR2Taxamount"]),
                            GSTR2taxVal = dr.IsNull("GSTR2taxVal") ? 0 : Convert.ToDecimal(dr["GSTR2taxVal"]),
                            GSTR2AVal = dr.IsNull("GSTR2AVal") ? 0 : Convert.ToDecimal(dr["GSTR2AVal"]),
                            GSTR2ATaxamount = dr.IsNull("GSTR2ATaxamount") ? 0 : Convert.ToDecimal(dr["GSTR2ATaxamount"]),
                            GSTR2AtaxVal = dr.IsNull("GSTR2AtaxVal") ? 0 : Convert.ToDecimal(dr["GSTR2AtaxVal"]),
                            TaxDiff = dr.IsNull("TaxDiff") ? 0 : Convert.ToDecimal(dr["TaxDiff"]),
                            TaxDiffPercent = dr.IsNull("TaxDiffPercent") ? 0 : Convert.ToDecimal(dr["TaxDiffPercent"]),
                            supplierdetails = dr.IsNull("supplierdetails") ? "" : dr["supplierdetails"].ToString()
                        });
                    }
                }
                Model.ReconciliationSummary = LstReconSummary;
                ViewBag.LstReconSummary = LstReconSummary.Count;

                TempData["ExportData"] = dsReconSummary.Tables[1];
                TempData["ExportData1"] = dsReconSummary.Tables[2];
                TempData["ExportData2"] = dsReconSummary.Tables[3];
                ViewBag.FromDate = Session["CF_FROMDATE"].ToString();
                ViewBag.ToDate = Session["CF_TODATE"].ToString();
                ViewBag.Period = Session["CF_PERIOD"].ToString();
                ViewBag.ToPeriod = Session["CF_TOPERIOD"].ToString();
                #endregion
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGstin, Session["Role_Name"].ToString());

                var jsonresult = Json(new { success = true, data = Model, SuccessMessage = TempData["ReconcilationResponse"], ErrorMessage = TempData["ErrorMessage"],
                    ctin = ViewBag.ctinValidation},  JsonRequestBehavior.AllowGet);
                jsonresult.MaxJsonLength = Int32.MaxValue;
                return jsonresult;

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return Json(new { success = false, ErrorMessage = TempData["ErrorMessage"] }, JsonRequestBehavior.AllowGet);
            }

        }

  

        public ActionResult ctinWise(string gstin, string ctin, string supp_name)
        {
           
            if (supp_name == "")
            {
                Session["CF_SUPNAME"] = "OTHERS";
            }
            else
            {
              Session["CF_SUPNAME"] = supp_name;
            }
            Session["CF_GSTIN"] = gstin;
            Session["CF_CTIN"] = ctin;
            

           return RedirectToAction("reconcilation", "gstr2", new { Page = "ViewDetails" });           
        }

        public ActionResult Validate(string ctin, string val)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            if (ctin == null || val == "0" || val == "0.00")
            {
                TempData["ReconciliationSummary"] = "Can not Update Supplier GSTIN.";
            }
            else
            {
                ViewBag.ctinValidation = ctin;
                Session["CTIN"] = ctin;
            }
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN"].ToString(), Session["Role_Name"].ToString());
            ViewBag.Period = Session["CF_PERIOD"].ToString();
            ViewBag.ToPeriod  = Session["CF_TOPERIOD"].ToString(); 
                if (Session["YEAR"] != null)
                {
                    ViewBag.yearlist = LoadDropDowns.Exist_Financial_year(Session["YEAR"].ToString());
                }
                else
                {
                    ViewBag.yearlist = LoadDropDowns.Financial_year();
                }

            return View("InvoiceCorrection");
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