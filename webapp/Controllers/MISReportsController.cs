using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models.MISReports;
using System.Web.UI.WebControls;
using System.Web.UI;
using System.IO;

namespace SmartAdminMvc.Controllers
{
    public class MisReportsController : Controller
    {
        // GET: MISReports
        public ActionResult GSTINSales(int page = 0, string sort = null)
        {
        if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.GSTINSalesSummary(GSTIN, fp,custid);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.GSTINSalesSummarydt(GSTIN, fp,custid);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }
        }

        [HttpPost]
        public ActionResult GSTINSales(FormCollection Col)
        {
            string GSTIN = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
             GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN ="ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.GSTINSalesSummary(GSTIN,fp, custid);
            ViewBag.Report =ReportResult;
            var ReportResultdt = Reports.GSTINSalesSummarydt(GSTIN, fp, custid);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
    }

        // GET: MISReports
        public ActionResult GSTINPurchase(int page = 0, string sort = null)
        {

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.GSTINPurchaseSummary(GSTIN, fp, custid);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.GSTINPurchaseSummarydt(GSTIN, fp, custid);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }

        }
        [HttpPost]
        public ActionResult GSTINPurchase(FormCollection Col)
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN = "ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.GSTINPurchaseSummary(GSTIN, fp, custid);
            ViewBag.Report = ReportResult;
            var ReportResultdt = Reports.GSTINPurchaseSummarydt(GSTIN, fp, custid);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
        }
        // GET: MISReports
        public ActionResult CustomerSalesReport(int page = 0, string sort = null)
          {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");


            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.CustomerSalesReport(GSTIN, fp,custid);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.CustomerSalesReportdt(GSTIN, fp, custid);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }
        }
        [HttpPost]
        public ActionResult CustomerSalesReport(FormCollection Col)
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN = "ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.CustomerSalesReport(GSTIN, fp,custid);
            ViewBag.Report = ReportResult;
            var ReportResultdt = Reports.CustomerSalesReportdt(GSTIN, fp,custid);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
        }
        // GET: MISReports
        public ActionResult InvoiceType(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.InvoiceType(GSTIN, fp,custid);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.InvoiceTypedt(GSTIN, fp,custid);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }
        }
        [HttpPost]
        public ActionResult InvoiceType(FormCollection Col)
        {
            string GSTIN = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN = "ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.InvoiceType(GSTIN, fp,custid);
            ViewBag.Report = ReportResult;
            var ReportResultdt = Reports.InvoiceTypedt(GSTIN, fp,custid);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
        }
        // GET: MISReports
        public ActionResult SupplierInwardRegister(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.SupplierInwardRegister(GSTIN, fp,custid);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.SupplierInwardRegisterdt(GSTIN, fp, custid);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }
        }
        [HttpPost]
        public ActionResult SupplierInwardRegister(FormCollection Col)
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN = "ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.SupplierInwardRegister(GSTIN, fp, custid);
            ViewBag.Report = ReportResult;
            var ReportResultdt = Reports.SupplierInwardRegisterdt(GSTIN, fp, custid);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
        }

        // GET: MISReports
        public ActionResult GSTR6ISD(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");

            if (sort != null || page != 0)
            {
                string GSTIN = Session["cgstin"] as string;
                string fp = Session["periodidr"] as string;
                TempData["cgstin"] = GSTIN;
                ViewBag.Period = fp;
                TempData.Keep();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                var ReportResult = Reports.GSTR6ISDSummary(GSTIN, fp);
                ViewBag.Report = ReportResult;
                var ReportResultdt = Reports.GSTR6ISDSummarydt(GSTIN, fp);
                TempData["Report"] = ReportResultdt;
                return View();
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                return this.View();
            }
        }

        [HttpPost]
        public ActionResult GSTR6ISD(FormCollection Col)
        {
            string GSTIN = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            GSTIN = Col["cgstin"];
            if (GSTIN == "")
            {
                GSTIN = "ALL";
            }
            string fp = Col["periodidr"];
            TempData["cgstin"] = GSTIN;
            Session["cgstin"] = GSTIN;
            Session["periodidr"] = fp;
            ViewBag.Period = fp;
            var ReportResult = Reports.GSTR6ISDSummary(GSTIN, fp);
            ViewBag.Report = ReportResult;
            var ReportResultdt = Reports.GSTR6ISDSummarydt(GSTIN, fp);
            TempData["Report"] = ReportResultdt;
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            return View();
        }

        [HttpPost]
        public ActionResult ExportDataSales()
        {
           
                GridView gv = new GridView();
                gv.DataSource = TempData["Report"];
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=SalesSummary Report.xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                return RedirectToAction("GSTINSales");

                    }
        public ActionResult ExportDataPurchase()

        { 

                    GridView gv = new GridView();
                    gv.DataSource = TempData["Report"];
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=PurchaseSummary Report.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return RedirectToAction("GSTINPurchase");
                }

        public ActionResult ExportDataCustomer()
        {
            GridView gv = new GridView();
                    gv.DataSource = TempData["Report"];
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=CustomerSales Report.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return RedirectToAction("CustomerSalesReport");
                }

        public ActionResult ExportDataInvoice()
        {
            GridView gv = new GridView();
                    gv.DataSource = TempData["Report"];
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=SalesSummary-InvoiceType Report.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return RedirectToAction("InvoiceType");
                }

        public ActionResult ExportDataSupplier()
        {
            GridView gv = new GridView();
                    gv.DataSource = TempData["Report"];
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=SupplierInwardRegister Report.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    return RedirectToAction("SupplierInwardRegister");
                }

        public ActionResult ExportDataGstr6ISD()
        {

            GridView gv = new GridView();
            gv.DataSource = TempData["Report"];
            gv.DataBind();
            Response.ClearContent();
            Response.Buffer = true;
            Response.AddHeader("content-disposition", "attachment; filename=GSTR6_ISDSave_Summary Report.xls");
            Response.ContentType = "application/ms-excel";
            Response.Charset = "";
            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);
            gv.RenderControl(htw);
            Response.Output.Write(sw.ToString());
            Response.Flush();
            Response.End();
            return RedirectToAction("GSTR6ISD");

        }

    }
           
    }
