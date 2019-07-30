using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.EditInvoice;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class ExpUpdateController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        [HttpGet]
        public ActionResult EXP(string InvoiceNo, string InvoiceDate)

        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            Session["inum"] = InvoiceNo;
            Session["idt"] = InvoiceDate;

            string refnum;
            refnum = Session["CustRefNo"].ToString();

            var records = (from li in db.TBL_EXT_GSTR1_EXP_INV where li.inum == InvoiceNo && li.idt == InvoiceDate && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("invoicepdf", "downloadpdf");
            }
            
            else
            {

                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                ViewBag.Taxpayer_GSTIN = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");
                ViewBag.ExportType = OutwardFunctions.GetExpType();

                var buyerid = (from li in db.TBL_EXT_GSTR1_EXP_INV
                               where (li.inum == InvoiceNo && li.idt == InvoiceDate && li.referenceno == refnum)
                               select li.buyerid).FirstOrDefault();
                int Bid = Convert.ToInt32(buyerid);
            
                Session["Bid"] = Bid;

                var master = (from li in db.TBL_EXT_GSTR1_EXP_INV
                              where li.inum == InvoiceNo && li.idt == InvoiceDate && li.referenceno == refnum
                              select li).First();

                object lineitms = (from li in db.TBL_EXT_GSTR1_EXP_INV
                                   where li.inum == InvoiceNo && li.idt == InvoiceDate && li.referenceno == refnum
                                   select li).ToList();
                ViewBag.Name = (from li in db.TBL_Buyer where li.BuyerId == Bid select li.BuyerName).FirstOrDefault();
                ViewBag.Address = (from li in db.TBL_Buyer where li.BuyerId == Bid select li.Address).FirstOrDefault();


                ViewBag.master = master;
                ViewBag.lineitms = lineitms;

                return View();
            }
        }

        [HttpPost]
        public ActionResult EXP(FormCollection form, string command)
        {

            string refnum, Iid, Idate;
            Iid = Convert.ToString(Session["inum"]);
            Idate = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);

            string Gstin, Fp, Inum, Idt,sbnum, sbdt, sbpcode, ShippingAddress, Addinfo,itemdesc,hsn,uqc,ExpType,ReceiverName;
            decimal Val,qty, discount, unitprice, rate, taxablevalue, iamt;
            Gstin = form["cgstin"];
            Inum = form["invoiceno"];
            Idt = form["invoicedate"];
            ExpType = form["ex_typ"];
            Fp = Idt.Substring(3, 2) + Idt.Substring(6, 4);
            Val =Convert.ToDecimal(form["invoicevalue"]);
            sbnum = form["billno"];
            sbdt = form["billdate"];
            sbpcode= form["portcode"];
            ShippingAddress= form["saddress"];
            ReceiverName = form["sname"];
            Addinfo = form["addinfo"];
            int userid= Convert.ToInt32(Session["User_Id"]);
            int BuyerId = Convert.ToInt32(Session["Bid"]);


            if (command == "save")
            {
                int result = ExpFunctions.MasterItemUpdate(Gstin, Fp, Inum, Idt, Val, sbnum, sbdt, sbpcode, ShippingAddress, Addinfo,ReceiverName);
                
               
                if (result == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward EXP invoice updated" + Inum, "");
                    TempData["Message"] = "Details updated successfully";
                }
                else
                {
                    TempData["Message"] = "Details updated Failure";
                }
            }


            else if (command == "generate")
            {
                return RedirectToAction("EXP", "PDFDownload", new { InvoiceNo = Iid, InvoiceDate = Idate });
            }

            else if (command == "add")
            {
                int CreatedBy = Convert.ToInt32(Session["User_Id"]);
                hsn = form["hsn"];
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamt"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                decimal invoicevalue = Val + taxablevalue + iamt;

                string result = ExpFunctions.Insert(Gstin, Fp, ExpType, Inum, Idt, invoicevalue, sbnum, sbdt, sbpcode, rate, taxablevalue, iamt, refnum, hsn, itemdesc, qty, unitprice, discount, uqc, CreatedBy, Addinfo, ShippingAddress,BuyerId,ReceiverName);
                //var id = (from li in db.TBL_EXT_GSTR1_EXP_INV where li.inum == Inum && li.idt == Idt && li.referenceno == refnum select li.invid).ToList();
                //foreach (int invid in id)
                //{
                //    TBL_EXT_GSTR1_EXP_INV b2binv = db.TBL_EXT_GSTR1_EXP_INV.Where(u => u.invid == invid).SingleOrDefault();
                //    b2binv.val = invoicevalue;
                //    db.SaveChanges();

                //}

                if (result == "-1")
                {
                    TempData["Message"] = "Item not Added.";
                    return RedirectToAction("Index", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
                }

                else if (result =="-2")
                {
                    TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item  cannot be Added";
                    return RedirectToAction("Index", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
                }
                else
                { 
                    TempData["Message"] = "Item added successfully";
                    ExpFunctions.EXPPush(refnum, Gstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward EXP invoice item added : " + Inum, "");
                }

            }
            return RedirectToAction("EXP", new { InvoiceNo = Session["inum"].ToString(), InvoiceDate = Session["idt"].ToString() });
        }

        [HttpPost]
        public JsonResult Edit(int id, string hsncode, string itemdesc, decimal qty, string uqc, decimal unitprice, decimal discount, decimal txval, decimal rate, decimal igsta)
        {

            TBL_EXT_GSTR1_EXP_INV inv = db.TBL_EXT_GSTR1_EXP_INV.Where(s => s.invid == id).SingleOrDefault();
            var message = inv;
            int userid = Convert.ToInt32(Session["User_Id"]);

            int output = ExpFunctions.ItemUpdate(id, hsncode, itemdesc, qty, unitprice, discount, uqc, rate, txval, igsta, userid);
            if (output == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice item updated" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item updated successfully";
            }
            else if (output == -1)
            {
                TempData["Message"] = "Item not available";
            }

            else if (output == -2)
            {
                TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item details cannot be Updated";
            }
            return Json(message, JsonRequestBehavior.AllowGet);


        }

        public JsonResult Delete(int Id)
        {
            TBL_EXT_GSTR1_EXP_INV inv = db.TBL_EXT_GSTR1_EXP_INV.Where(s => s.invid == Id).SingleOrDefault();
            var message = inv;

            string refnum, inum, idt;
            inum = Convert.ToString(Session["inum"]);
            idt = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);

            int output = ExpFunctions.ItemDelete(Id);

            if (output == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice item deleted" + Session["inum"].ToString(), "");

                TempData["Message"] = "Item deleted successfully..";
            }
            else if (output == -1)
            {
                TempData["Message"] = "Item not available";
            }

            else if (output == -2)
            {
                TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item details cannot be Deleted";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult AutoPopulate(int Prefix)
        {
            //Note : you can bind same list from database  
            var hsndetail = (from obj in db.TBL_HSN_MASTER
                             where obj.hsnId == Prefix
                             select new
                             {
                                 hsnDescription = obj.hsnDescription,
                                 unitPrice = obj.unitPrice,
                                 rate = obj.rate
                             }).SingleOrDefault();
            return Json(hsndetail, JsonRequestBehavior.AllowGet);
        }
    }
}