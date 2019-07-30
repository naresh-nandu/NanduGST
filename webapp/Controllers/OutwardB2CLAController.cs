using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;


namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardB2CLAController : Controller
    {
        // GET: OutwardB2CLA
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: OutwardB2CL
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            { 
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == custid).Select(c =>
            new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();

            ViewBag.Categories = categories;

            if (Session["OutwardB2CLA"] != null)
            {
                DataTable dt = ((DataTable)Session["OutwardB2CLA"]);

                return View(ConvertToDictionary(dt));
            }
            else
            {
                return View();
            }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }
       

        public ActionResult Delete(int Id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try { 
            DataTable dt = ((DataTable)Session["OutwardB2CLA"]);

            foreach (DataRow dtr in dt.Rows)
            {
                if (Convert.ToInt32(dtr[0]) == Id)
                {
                    dt.Rows.Remove(dtr);
                    break;
                }

            }

            Session["OutwardB2CLA"] = dt;

            return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult Save(FormCollection form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try { 
           
            if (command == "add")
            {

                Session["nameob2cla"] = form["name"];
                Session["etinob2cla"] = form["etin"];
                Session["prsob2cla"] = form["prs"];
                Session["statecodeob2cla"] = form["statecode"];
                Session["cgstinob2cla"] = form["cgstin"];
                Session["invoicenoob2cla"] = form["invoiceno"];
                Session["oinvoicenoob2cla"] = form["oinvoiceno"];
                Session["taxvalueob2cla"] = form["taxvalue"];
                Session["posob2cla"] = form["pos"];
                Session["oinvoicedateob2cla"] = form["oinvoicedate"];
                Session["invoicedateob2cla"] = form["invoicedate"];
                Session["ordernumob2cla"] = form["ordernum"];
                Session["orderdateob2cla"] = form["orderdate"];

                int count;
                DataTable dt = new DataTable();


                if (Session["index"] == null)
                {
                    Session["index"] = 1;
                    count = (int)Session["index"];
                }
                else
                {
                    count = (int)Session["index"];
                    count = count + 1;
                    Session["index"] = count;
                }

                if (Session["OutwardB2CLA"] == null)
                {

                    dt.Columns.Add("Srno", typeof(int));
                    dt.Columns.Add("HSN", typeof(string));
                    dt.Columns.Add("Category", typeof(char));
                    dt.Columns.Add("Taxablevalue", typeof(string));
                    dt.Columns.Add("IGST Rate", typeof(string));
                    dt.Columns.Add("IGST Amount", typeof(string));
                    //dt.Columns.Add("CGST Rate", typeof(string));
                    //dt.Columns.Add("CGST Amount", typeof(string));
                    //dt.Columns.Add("SGST Rate", typeof(string));
                    //dt.Columns.Add("SGST Amount", typeof(string));
                    dt.Columns.Add("CESS Rate", typeof(string));
                    dt.Columns.Add("CESS Amount", typeof(string));
                    //dt.Columns.Add("Eligibility", typeof(string));
                }
                else
                {
                    dt = ((DataTable)Session["OutwardB2CLA"]);
                }
                DataRow dr = dt.NewRow();
                dr[0] = count;
                dr[1] = form["HSN"];
                dr[2] = form["category"];
                dr[3] = form["taxablevalue"];
                dr[4] = form["irate"];
                dr[5] = form["iamount"];
                dr[6] = form["csrate"];
                dr[7] = form["csamount"];

                dt.Rows.Add(dr);
                Session["OutwardB2CLA"] = dt;

                return RedirectToAction("Index");
            }
            else if (command == "save")
            {

                string HSN, Category, cgstin, name, invoice, oinvoice, pos, odate, invoicedate, statecode, etin, prs, ordernum, orderdate;
                decimal taxvalue, Taxablevalue, IGSTr, IGSTa, CESSa, CESSr;
                int custid, retInvoiceId, itemnum = 0, userid;

                userid = Convert.ToInt32(Session["User_Id"]);
                custid = Convert.ToInt32(Session["Cust_ID"]);
                name = form["name"];
                etin = form["etin"];
                ordernum = form["ordernum"];
                orderdate = form["orderdate"];
                prs = form["prs"];
                statecode = form["statecode"];
                cgstin = form["cgstin"];
                invoice = form["invoiceno"];
                oinvoice = form["oinvoiceno"];
                taxvalue = Convert.ToDecimal(form["taxvalue"]);
                pos = form["pos"];
                odate = form["oinvoicedate"];
                invoicedate = form["invoicedate"];

                ObjectParameter obj = new ObjectParameter("InvoiceId", typeof(int));
                //db.Ins_Outward_Gstr1_B2CL(custid, cgstin, statecode, name, invoice, invoicedate, taxvalue, pos, prs, oinvoice, odate, etin, userid, obj);
                //db.Ins_Outward_Gstr1_B2CLA(custid, cgstin, statecode, oinvoice, odate, name, invoice, invoicedate, taxvalue, pos, prs, ordernum, orderdate, etin, userid, obj);
                retInvoiceId = Convert.ToInt32(obj.Value);


                DataTable dt = (DataTable)Session["OutwardB2CLA"];

                foreach (DataRow dr in dt.Rows)
                {
                    itemnum = itemnum + 1;
                    HSN = Convert.ToString(dr["HSN"]);
                    Category = Convert.ToString(dr["Category"]);
                    Taxablevalue = Convert.ToDecimal(dr["Taxablevalue"]);
                    IGSTr = Convert.ToDecimal(dr["IGST Rate"]);
                    IGSTa = Convert.ToDecimal(dr["IGST Amount"]);
                    CESSr = Convert.ToDecimal(dr["CESS Rate"]);
                    CESSa = Convert.ToDecimal(dr["CESS Amount"]);
                }

               
                Session["nameob2cla"] = null;
                Session["etinob2cla"] = null;
                Session["prsob2cla"] = null;
                Session["statecodeob2cla"] = null;
                Session["cgstinob2cla"] = null;
                Session["invoicenoob2cla"] = null;
                Session["oinvoicenoob2cla"] = null;
                Session["taxvalueob2cla"] = null;
                Session["posob2cla"] = null;
                Session["oinvoicedateob2cla"] = null;
                Session["invoicedateob2cla"] = null;
                Session["ordernumob2cla"] = null;
                Session["orderdateob2cla"] = null;

                Session["OutwardB2CLA"] = null;
                Session["index"] = null;
                TempData["Message"] = "Record inserted succesfully";

                var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == custid).Select(c =>
                new {
                    CategoryID = c.GSTINId,
                    CategoryName = c.GSTINNo
                }).ToList();

                ViewBag.Categories = categories;
                return View("Index");
            }
            return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }
    }
}