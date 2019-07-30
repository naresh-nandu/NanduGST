using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class InwardB2BEditController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: InwardB2BEdit
        public ActionResult Index(string inum,string idt)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            Session["inum"] = inum;
            Session["idt"] = idt;

            string refnum;
            refnum = Session["CustRefNo"].ToString();

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            var supname = db.TBL_Supplier.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { suppliername = c.SupplierName, supplierid = c.SupplierId }).ToList();
            ViewBag.supname = supname;
            ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            ViewBag.Rates = LoadDropDowns.LoadRates();
            ViewBag.UQCList = LoadDropDowns.LoadUQC();
            ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "INWARD");

            var master = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                          where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                          select b2b).First();

            object lineitms = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                               where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                               select b2b).ToList();
            string supid = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                              where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                              select b2b.supplierid).First().ToString();

            ViewBag.master = master;
            ViewBag.lineitms = lineitms;
            int supplier = Convert.ToInt32(supid);
            ViewBag.suppliername = db.TBL_Supplier.Where(b => b.SupplierId == supplier).Select(b => b.SupplierName).SingleOrDefault().ToString();
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection form,string command)
        {

            string cgstin, gstin, invoice, reversecharge, pos, invoicedate, etin, invtype, hsncode, itemdesc, uqc,elg;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, camt, samt, csamt;
            int supid, userid;

            userid = Convert.ToInt32(Session["User_Id"]);
            gstin = form["gstin"];
            supid = Convert.ToInt32(form["name"]);
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            invalue = Convert.ToDecimal(form["invalue"]);
            reversecharge = form["reversecharge"];
            pos = form["pos"];
            invtype = form["invtype"];

            if (command == "save")
            {

                ObjectParameter op = new ObjectParameter("retval", typeof(int));

                db.usp_Update_INWARD_GSTR2_B2B_Master(cgstin, "fp", gstin, invoice, invoicedate, invalue, pos, reversecharge, invtype, supid, op);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice updated" + Session["inum"].ToString(), "");
                int ret = Convert.ToInt32(op.Value);
                if (ret == 1)
                {
                    TempData["Message"] = "Details updated successfully";
                }
            }
            else if(command=="add")
            {
                string hsnid = form["HSN"];
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                camt = Convert.ToDecimal(form["camount"]);
                samt = Convert.ToDecimal(form["samount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                elg = form["eligibility"];

                string fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                string refno = Session["CustRefNo"].ToString();
                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                db.usp_Insert_INWARD_GSTR2_B2B_EXT(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, invtype, rate, taxablevalue, iamt, camt, samt, csamt, refno, hsnid, itemdesc, qty, unitprice, discount, uqc, supid, userid, elg, op);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item added" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item added successfully";

            }
            return RedirectToAction("Index",new { inum = Session["inum"].ToString(),idt= Session["idt"].ToString() });
        }
        [HttpPost]
        public JsonResult Edit(int id, int hsncode, string itemdesc, string qty, string uqc, string unitprice, string discount, string txval, string rate, string igsta, string cgsta, string sgsta, string cessa)
        {

            TBL_EXT_GSTR2_B2B_INV inv = db.TBL_EXT_GSTR2_B2B_INV.Where(s => s.b2bid== id).SingleOrDefault();
            var message = inv;

            decimal taxablevalue, rt, IGSTa, CGSTa, SGSTa, CESSa, price, disc, quantities;

            string hsncd = (from hsn in db.TBL_HSN_MASTER
                            where hsn.hsnId == hsncode
                            select hsn.hsnCode).FirstOrDefault();

            price = Convert.ToDecimal(unitprice);
            disc = Convert.ToDecimal(discount);
            taxablevalue = Convert.ToDecimal(txval);
            rt = Convert.ToDecimal(rate);
            IGSTa = Convert.ToDecimal(igsta);
            CGSTa = Convert.ToDecimal(cgsta);
            SGSTa = Convert.ToDecimal(sgsta);
            CESSa = Convert.ToDecimal(cessa);
            quantities = Convert.ToDecimal(qty);
            ObjectParameter op = new ObjectParameter("retval", typeof(int));


            db.usp_Update_INWARD_GSTR2_B2B_Items(id, hsncd, itemdesc, quantities, price, disc, uqc, rt, taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, Convert.ToInt32(Session["User_ID"]), op);
            //db.usp_Update_OUTWARD_GSTR1_B2B_Items(id, hsncd, itemdesc, quantities, price, disc, uqc, rt, taxablevalue, IGSTa, CGSTa, SGSTa, CGSTa, Convert.ToInt32(Session["User_ID"]), op);

            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item updated" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item updated successfully";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult Delete(int Id)
        {
            TBL_EXT_GSTR2_B2B_INV inv = db.TBL_EXT_GSTR2_B2B_INV.Where(s => s.b2bid == Id).SingleOrDefault();
            var message = inv;
            ObjectParameter op = new ObjectParameter("retval", typeof(int));

             db.usp_Delete_INWARD_GSTR2_B2B_Items(Id, op);

            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item deleted" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item deleted successfully..";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}