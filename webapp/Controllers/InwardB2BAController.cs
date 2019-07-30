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
    public class InwardB2BAController : Controller
    {
        // GET: Inward
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
                new
                {
                    CategoryID = c.GSTINId,
                    CategoryName = c.GSTINNo
                }).ToList();

                ViewBag.Categories = categories;

                if (Session["MasterdtlsB2BA"] != null)
                {

                    DataTable dt = ((DataTable)Session["MasterdtlsB2BA"]);

                    return View(ConvertToDictionary(dt));
                }
                else
                {
                    return View();
                }
            }
            catch(Exception ex)
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
                DataTable dt = ((DataTable)Session["MasterdtlsB2BA"]);

                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }

                Session["MasterdtlsB2BA"] = dt;

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult Save(FormCollection form,string command)
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
                    Session["gstinb2ba"] = form["gstin"];
                    Session["nameb2ba"] = form["name"];
                    Session["invoiceb2ba"] = form["invoice"];
                    Session["invoicedateb2ba"] = form["invoicedate"];
                    Session["revinvoiceb2ba"] = form["revinvoice"];
                    Session["reviseddateb2ba"] = form["reviseddate"];
                    Session["taxvalueb2ba"] = form["taxvalue"];
                    Session["reversechargeb2ba"] = form["reversecharge"];
                    Session["cgstinb2ba"] = form["cgstin"];
                    Session["posb2ba"] = form["pos"];


                    int count;
                    DataTable dt = new DataTable();


                    if (Session["sr.no"] == null)
                    {
                        Session["sr.no"] = 1;
                        count = (int)Session["sr.no"];
                    }
                    else
                    {
                        count = (int)Session["sr.no"];
                        count = count + 1;
                        Session["sr.no"] = count;
                    }

                    if (Session["MasterdtlsB2BA"] == null)
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
                        dt.Columns.Add("Eligibility", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterdtlsB2BA"]);
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
                    dr[12] = form["eligibility"];

                    dt.Rows.Add(dr);
                    Session["MasterdtlsB2BA"] = dt;

                    return RedirectToAction("Index");

                }
                else if (command == "save")
                {
                    string HSN, Category, Eligibility, cgstin, gstin, name, invoice, revinvoice, reversecharge, pos, revdate, invoicedate;
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
                    //attractcharge = form["attractcharge"];
                    reversecharge = form["reversecharge"];
                    //itctaxvalue = form["itctaxvalue"];
                    //itcavail = form["itcavail"];
                    //supplytype = form["supplytype"];
                    pos = form["pos"];
                    revdate = form["reviseddate"];
                    invoicedate = form["invoicedate"];

                    ObjectParameter obj = new ObjectParameter("InvoiceId", typeof(int));
                    //db.Ins_Inward_Gstr2_B2BA(name, gstin, custid, cgstin, revdate, revinvoice, invoicedate, invoice, taxvalue, pos, reversecharge, userid, obj);
                    retInvoiceId = Convert.ToInt32(obj.Value);


                    DataTable dt = (DataTable)Session["MasterdtlsB2BA"];

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
                        Eligibility = Convert.ToString(dr["Eligibility"]);

                        //db.Ins_Inward_Gstr2_B2BA_invItems(retInvoiceId, itemnum, Category, HSN, Taxablevalue, IGSTr, IGSTa, CGSTr, CGSTa, SGSTr, SGSTa, CESSr, CESSa, Eligibility);


                    }

                    Session["gstinb2ba"] = null;
                    Session["nameb2ba"] = null;
                    Session["invoiceb2ba"] = null;
                    Session["invoicedateb2ba"] = null;
                    Session["revinvoiceb2ba"] = null;
                    Session["reviseddateb2ba"] = null;
                    Session["taxvalueb2ba"] = null;
                    Session["reversechargeb2ba"] = null;
                    Session["cgstinb2ba"] = null;
                    Session["posb2ba"] = null;
                    Session["MasterdtlsB2BA"] = null;
                    Session["sr.no"] = null;
                    TempData["Message"] = "Record inserted succesfully";

                    var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == custid).Select(c =>
                    new
                    {
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