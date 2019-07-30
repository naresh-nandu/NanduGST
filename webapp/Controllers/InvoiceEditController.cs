using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;

using SmartAdminMvc.Models.EditInvoice;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class InvoiceEditController : Controller
    {

        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        readonly B2BFunctions B2BSP = new B2BFunctions();
        [HttpGet]
        public ActionResult Index(string inum, string idt)

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

            var records = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == inum && li.idt == idt && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("invoicepdf", "downloadpdf");
            }
            else
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                var buyname = db.TBL_Buyer.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { buyername = c.BuyerName, buyerid = c.BuyerId }).OrderBy(c => c.buyername).ToList();
                ViewBag.buyname = buyname;
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");

                var master = (from b2b in db.TBL_EXT_GSTR1_B2B_INV
                              where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                              select b2b).First();

                object lineitms = (from b2b in db.TBL_EXT_GSTR1_B2B_INV
                                   where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                                   select b2b).ToList();
                string buyerid = (from b2b in db.TBL_EXT_GSTR1_B2B_INV
                                  where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                                  select b2b.buyerid).First().ToString();

                ViewBag.master = master;
                ViewBag.lineitms = lineitms;
                int buyid = Convert.ToInt32(buyerid);

                var serviceType = (from li in db.TBL_EXT_GSTR1_B2B_INV
                                   where li.inum == inum && li.idt == idt && li.referenceno == refnum && li.GoodsAndService != null
                                   select li.GoodsAndService).FirstOrDefault();

                var transMode = (from li in db.TBL_EXT_GSTR1_B2B_INV
                                 where li.inum == inum && li.idt == idt && li.referenceno == refnum && li.TransportMode != null
                                 select li.TransportMode).FirstOrDefault();
                Session["transModeb2b"] = transMode;
                Session["serviceTypeb2b"] = serviceType;
                ViewBag.buyername = db.TBL_Buyer.Where(b => b.BuyerId == buyid).Select(b => b.BuyerName).SingleOrDefault().ToString();
                if (Session["transModeb2b"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2b"].ToString());
                }
                else
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                }
                if (Session["serviceTypeb2b"] != null)
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2b"].ToString());
                }
                else
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
                }
                return View();
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form, string command)
        {

            string refnum, Iid, Idate;
            Iid = Convert.ToString(Session["inum"]);
            Idate = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);

            string cgstin, gstin, invoice, reversecharge, pos, invoicedate, etin, invtype, itemdesc, uqc, details;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, camt, samt, csamt;
            int buyerid, userid;
            string serviceType = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";
            details = form["comment1"];
            userid = Convert.ToInt32(Session["User_Id"]);

            gstin = form["gstin"];
            etin = form["etin"];

            buyerid = Convert.ToInt32(form["lblname"]);
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            var shipadd = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == invoice && li.idt == invoicedate select li.ShippingAddress).FirstOrDefault();
            string fp = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == invoice && li.idt == invoicedate select li.fp).FirstOrDefault();
            invalue = Convert.ToDecimal(form["invalue"]);
            reversecharge = form["reversecharge"];
            pos = form["pos"];
            invtype = form["invtype"];

            serviceType = form["ddlServiceType"];
            transMode = form["transMode"];
            vechicleNo = form["vehicleNo"];
            dateOfSupply = form["dateOfSupply"];
            cinNo = form["cinNo"];

            Session["transModeb2b"] = transMode;
            Session["serviceTypeb2b"] = serviceType;
            if (Session["transModeb2b"] != null)
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2b"].ToString());
            }
            else
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
            }
            if (Session["serviceTypeb2b"] != null)
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2b"].ToString());
            }
            else
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
            }
            if (command == "save")
            {

                ObjectParameter op = new ObjectParameter("retval", typeof(int));
                db.usp_Update_OUTWARD_GSTR1_B2B_Master(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, etin, invtype, buyerid, details, serviceType, transMode, vechicleNo, dateOfSupply, cinNo, op);

                int ret = Convert.ToInt32(op.Value);
                if (ret == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice updated" + invoice, "");
                    TempData["Message"] = "Details updated successfully";
                }
            }


            else if (command == "generate")
            {
                return RedirectToAction("Demo", "DownloadPDF", new { Invid = Iid, invdate = Idate });
            }

            else if (command == "add")
            {
                string hsnid = Convert.ToString(form["HSN"]);
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
                decimal invoicevalue = invalue + taxablevalue + iamt + camt + samt + csamt;
                string period = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                string refno = Session["CustRefNo"].ToString();

                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                db.usp_Insert_Outward_GSTR1_B2B_EXT1(cgstin, period, gstin, invoice, invoicedate, invoicevalue, pos, reversecharge, etin, invtype, rate, taxablevalue, iamt, camt, samt, csamt, refno, hsnid, itemdesc, qty, unitprice, discount, uqc, buyerid, userid, details, shipadd, serviceType, transMode, vechicleNo, dateOfSupply, cinNo, op);
                var id = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == invoice && li.idt == invoicedate && li.referenceno == refno select li.invid).ToList();
                foreach (int invid in id)
                {
                    TBL_EXT_GSTR1_B2B_INV b2binv = db.TBL_EXT_GSTR1_B2B_INV.Where(u => u.invid == invid).SingleOrDefault();
                    b2binv.val = invoicevalue;
                    db.SaveChanges();

                }

                string ret = Convert.ToString(op.Value);
                if (ret == "-1")
                {
                    TempData["Message"] = "Item not Added.";
                    return RedirectToAction("Index", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
                }

                else if (ret == "-2")
                {
                    TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item  cannot be Added";
                    return RedirectToAction("Index", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
                }
                TempData["Message"] = "Item added successfully";
                B2CsFunctions.CDNRPush(refnum, gstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));

                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice item added" + invoice, "");

            }
            return RedirectToAction("Index", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
        }

        [HttpPost]
        public JsonResult Edit(int id, string hsncode, string itemdesc, decimal qty, string uqc, decimal unitprice, decimal discount, decimal txval, decimal rate, decimal igsta, decimal cgsta, decimal sgsta, decimal cessa)
        {

            TBL_EXT_GSTR1_B2B_INV inv = db.TBL_EXT_GSTR1_B2B_INV.Where(s => s.invid == id).SingleOrDefault();
            var message = inv;
            int userid = Convert.ToInt32(Session["User_Id"]);

            int output = B2BSP.B2BItemUpdate(id, rate, txval, igsta, cgsta, sgsta, cessa, hsncode, itemdesc, qty, unitprice, discount, uqc, userid);


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
            TBL_EXT_GSTR1_B2B_INV inv = db.TBL_EXT_GSTR1_B2B_INV.Where(s => s.invid == Id).SingleOrDefault();
            var message = inv;

            int output = B2BSP.B2BItemDelete(Id);

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