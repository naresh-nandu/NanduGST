using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.Data;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardB2BAController : Controller
    {
        // GET: OutwardB2BA
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
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

            if (Session["OutwardB2BA"] != null)
            {
                DataTable dt = ((DataTable)Session["OutwardB2BA"]);

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
            try
            {

            DataTable dt = ((DataTable)Session["OutwardB2BA"]);

            foreach (DataRow dtr in dt.Rows)
            {
                if (Convert.ToInt32(dtr[0]) == Id)
                {
                    dt.Rows.Remove(dtr);
                    break;
                }

            }

            Session["OutwardB2BA"] = dt;

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
            try
            {
            
            if (command == "add")
            {

                Session["gstinob2ba"] = form["gstin"];
                Session["nameob2ba"] = form["name"];
                Session["invoiceob2ba"] = form["invoice"];
                Session["invoicedateob2ba"] = form["invoicedate"];
                Session["revinvoiceob2ba"] = form["revinvoice"];
                Session["reviseddateob2ba"] = form["reviseddate"];
                Session["taxvalueob2ba"] = form["taxvalue"];
                Session["reversechargeob2ba"] = form["reversecharge"];
                Session["cgstinob2ba"] = form["cgstin"];
                Session["posob2ba"] = form["pos"];
                Session["etinob2ba"] = form["etin"];
                Session["prsob2ba"] = form["prs"];
                Session["onumob2ba"] = form["onum"];
                Session["odateob2ba"] = form["odate"]; 

                int count;
                DataTable dt = new DataTable();


                if (Session["num"] == null)
                {
                    Session["num"] = 1;
                    count = (int)Session["num"];
                }
                else
                {
                    count = (int)Session["num"];
                    count = count + 1;
                    Session["num"] = count;
                }

                if (Session["OutwardB2BA"] == null)
                {

                    dt.Columns.Add("Srno", typeof(int));
                    dt.Columns.Add("HSN", typeof(string));
                    dt.Columns.Add("Category", typeof(char));
                    dt.Columns.Add("Taxablevalue", typeof(string));
                    dt.Columns.Add("IGST Rate", typeof(string));
                    dt.Columns.Add("IGST Amount", typeof(string));
                    dt.Columns.Add("CGST Rate", typeof(string));
                    dt.Columns.Add("CGST Amount", typeof(string));
                    dt.Columns.Add("SGST Rate", typeof(string));
                    dt.Columns.Add("SGST Amount", typeof(string));
                    dt.Columns.Add("CESS Rate", typeof(string));
                    dt.Columns.Add("CESS Amount", typeof(string));
                }
                else
                {
                    dt = ((DataTable)Session["OutwardB2BA"]);
                }
                DataRow dr = dt.NewRow();
                dr[0] = count;
                dr[1] = form["HSN"];
                dr[2] = form["category"];
                dr[3] = form["taxablevalue"];
                dr[4] = form["irate"];
                dr[5] = form["iamount"];
                dr[6] = form["crate"];
                dr[7] = form["camount"];
                dr[8] = form["srate"];
                dr[9] = form["samount"];
                dr[10] = form["csrate"];
                dr[11] = form["csamount"];

                dt.Rows.Add(dr);
                Session["OutwardB2BA"] = dt;

                return RedirectToAction("Index");
            }
            else if (command == "save")
            {
                string HSN, Category, Eligibility, cgstin, gstin, name, invoice, revinvoice, reversecharge, pos, revdate, invoicedate, onum, odate, prs, etin;
                decimal taxvalue, Taxablevalue, IGSTr, IGSTa, CGSTa, CGSTr, SGSTa, SGSTr, CESSa, CESSr;
                int custid, retInvoiceId, itemnum = 0, userid;
                userid = Convert.ToInt32(Session["User_Id"]);
                custid = Convert.ToInt32(Session["Cust_ID"]);
                gstin = form["gstin"];
                name = form["name"];
                cgstin = form["cgstin"];
                invoice = form["invoice"];
                revinvoice = form["revinvoice"];
                taxvalue = Convert.ToDecimal(form["taxvalue"]);
                etin = form["etin"];
                reversecharge = form["reversecharge"];
                onum = form["onum"];
                odate = form["odate"];
                prs = form["prs"];
                pos = form["pos"];

                revdate = form["reviseddate"];
                invoicedate = form["invoicedate"];

                ObjectParameter obj = new ObjectParameter("InvoiceId", typeof(int));
                //db.Ins_Inward_Gstr2_B2BA(name, gstin, custid, cgstin, revdate, revinvoice, invoicedate, invoice, taxvalue, pos, reversecharge, 1, obj);
                //db.Ins_Outward_Gstr1_B2BA(name, gstin, custid, cgstin, revdate, revinvoice, taxvalue, pos, reversecharge, invoice, invoicedate, prs, onum, odate, etin, userid, obj);
                retInvoiceId = Convert.ToInt32(obj.Value);


                DataTable dt = (DataTable)Session["OutwardB2BA"];

                foreach (DataRow dr in dt.Rows)
                {
                    itemnum = itemnum + 1;
                    HSN = Convert.ToString(dr["HSN"]);
                    Category = Convert.ToString(dr["Category"]);
                    Taxablevalue = Convert.ToDecimal(dr["Taxablevalue"]);
                    IGSTr = Convert.ToDecimal(dr["IGST Rate"]);
                    IGSTa = Convert.ToDecimal(dr["IGST Amount"]);
                    CGSTr = Convert.ToDecimal(dr["CGST Rate"]);
                    CGSTa = Convert.ToDecimal(dr["CGST Amount"]);
                    SGSTr = Convert.ToDecimal(dr["SGST Rate"]);
                    SGSTa = Convert.ToDecimal(dr["SGST Amount"]);
                    CESSr = Convert.ToDecimal(dr["CESS Rate"]);
                    CESSa = Convert.ToDecimal(dr["CESS Amount"]);
                    //Eligibility = Convert.ToString(dr["Eligibility"]);

                    //db.Ins_Inward_Gstr2_B2BA_invItems(retInvoiceId, itemnum, Category, HSN, Taxablevalue, IGSTr, IGSTa, CGSTr, CGSTa, CESSr, CESSa, Eligibility);
                    //db.Ins_Outward_Gstr1_B2BA_invItems(retInvoiceId, itemnum, Category, HSN, Taxablevalue, IGSTr, IGSTa, CGSTr, CGSTa, SGSTr, SGSTa, CESSr, CESSa);


                }
               
                Session["gstinob2ba"] =null;
                Session["nameob2ba"] = null;
                Session["invoiceob2ba"] = null;
                Session["invoicedateob2ba"] = null;
                Session["revinvoiceob2ba"] = null;
                Session["reviseddateob2ba"] = null;
                Session["taxvalueob2ba"] = null;
                Session["reversechargeob2ba"] = null;
                Session["cgstinob2ba"] = null;
                Session["posob2ba"] = null;
                Session["etinob2ba"] = null;
                Session["prsob2ba"] = null;
                Session["onumob2ba"] = null;
                Session["odateob2ba"] = null;

                Session["OutwardB2BA"] = null;
                Session["num"] = null;
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