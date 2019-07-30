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
    public class OutwardB2CSEditController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: OutwardB2CLEdit
        [HttpGet]
        public ActionResult B2CSEdit(string InvId, string InvDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            Session["inum"] = InvId;
            Session["idt"] = InvDate;

            string refnum;
            refnum = Session["CustRefNo"].ToString();
            
            var records = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("invoicepdf", "downloadpdf");
            }
            else
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");

                var buyerid = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                               where (li.inum == InvId && li.idt == InvDate && li.referenceno == refnum)
                               select li.buyerid).FirstOrDefault();
                int Bid = Convert.ToInt32(buyerid);
                Session["Bid"] = Bid;

                var master = (from b2b in db.TBL_EXT_GSTR1_B2CS_INV
                              where b2b.inum == InvId && b2b.idt == InvDate && b2b.referenceno == refnum
                              select b2b).FirstOrDefault();

                object lineitms = (from b2b in db.TBL_EXT_GSTR1_B2CS_INV
                                   where b2b.inum == InvId && b2b.idt == InvDate && b2b.referenceno == refnum
                                   select b2b).ToList();
                var serviceType = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                   where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.GoodsAndService != null
                                   select li.GoodsAndService).FirstOrDefault();

                var transMode = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                 where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.TransportMode != null
                                 select li.TransportMode).FirstOrDefault();
                Session["transModeb2cs"] = transMode;
                Session["serviceTypeb2cs"] = serviceType;
                if (Session["transModeb2cs"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cs"].ToString());
                }
                else
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                }
                if (Session["serviceTypeb2cs"] != null)
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cs"].ToString());
                }
                else
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
                }

                ViewBag.master = master;
                ViewBag.lineitms = lineitms;
                ViewBag.Name = (from li in db.TBL_Buyer where li.BuyerId == Bid select li.BuyerName).FirstOrDefault();
                ViewBag.Address = (from li in db.TBL_Buyer where li.BuyerId == Bid select li.Address).FirstOrDefault();
                ViewBag.rowVal = 0;
                return View();
            }
        }

        [HttpPost]
        public ActionResult B2CSEdit(FormCollection form, string command)
        {
            string refnum, Inid, Idate;
            Inid = Convert.ToString(Session["inum"]);
            Idate = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);

            string cgstin, invoice, pos, invoicedate, etin, itemdesc, uqc;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, igst, cgst, csamt;
            string serviceType = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";

            int Bid = Convert.ToInt32(Session["Bid"]);
            string Invid = Convert.ToString(Session["inum"]);
            string InvDate = Convert.ToString(Session["idt"]);
            var invoiceid = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.inum == Invid && li.idt == InvDate select li.invid).FirstOrDefault();
            int Iid = Convert.ToInt32(invoiceid);
            string gstin = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == Iid select li.gstin).FirstOrDefault();
            string fp = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == Iid select li.fp).FirstOrDefault();
            string refno = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == Iid select li.referenceno).FirstOrDefault();
            var createdby = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == Iid select li.createdby).FirstOrDefault();
            int createdid = Convert.ToInt32(createdby);
            string addinfo = form["comment1"];

            // buyer Modification
            TBL_Buyer buyer = db.TBL_Buyer.Where(u => u.BuyerId == Bid).SingleOrDefault();
            buyer.BuyerName = form["buyername"];
            buyer.Address = form["address"];
            db.SaveChanges();

            int userid = Convert.ToInt32(Session["User_Id"]);

            etin = form["etin"];
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            invalue = Convert.ToDecimal(form["invalue"]);
            pos = form["pos"];

            serviceType = form["ddlServiceType"];
            transMode = form["transMode"];
            vechicleNo = form["vehicleNo"];
            dateOfSupply = form["dateOfSupply"];
            cinNo = form["cinNo"];

            Session["transModeb2cs"] = transMode;
            Session["serviceTypeb2cs"] = serviceType;
            if (Session["transModeb2cs"] != null)
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cs"].ToString());
            }
            else
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
            }
            if (Session["serviceTypeb2cs"] != null)
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cs"].ToString());
            }
            else
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
            }
            if (command == "save")
            {
                int output = B2CsFunctions.B2CSMasterUpdate(cgstin, fp, Invid, InvDate, invalue, pos, etin, refno, Bid, createdid, addinfo,serviceType, transMode, vechicleNo, dateOfSupply, cinNo);
                if (output == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice updated" + invoice, "");
                    TempData["Message"] = "Details updated successfully";
                    B2CsFunctions.B2CSPush(refno, cgstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));
                }
                else if (output == -1)
                {
                    TempData["Message"] = "Details Not updated successfully";

                }

            }

            else if (command == "generate")
            {
                return RedirectToAction("B2CS", "DownloadPdf", new { Invid = Inid, Invdate = Idate });
            }

            else if (command == "add")
            {
                string hsnid = Convert.ToString(form["HSN"]);
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                igst = Convert.ToDecimal(form["camount"]);
                cgst = Convert.ToDecimal(form["samount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                decimal invoicevalue = 0;
                invoicevalue = invalue + taxablevalue + iamt + igst + cgst + csamt;
                string strStateCode = form["cgstin"].ToString().Substring(0, 2);
                if (invoicevalue >= 250000 && strStateCode != pos)
                {
                    TempData["Message"] = "B2CS invoice value should be less than 2.5 Lakhs for Inter state";

                }
                else
                {
                  string Result = B2CsFunctions.Insert(cgstin, fp, invoice, invoicedate, invoicevalue, pos, etin, rate, taxablevalue, iamt, igst, cgst, csamt, refno, hsnid, itemdesc, qty, unitprice, discount, uqc, Bid, userid, addinfo, serviceType, transMode, vechicleNo, dateOfSupply, cinNo);
                    B2CsFunctions.B2CSPush(refno, cgstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));
                    var id = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.inum == invoice && li.idt == invoicedate && li.referenceno == refno select li.invid).ToList();
                    foreach (int invid in id)
                    {
                        TBL_EXT_GSTR1_B2CS_INV b2binv = db.TBL_EXT_GSTR1_B2CS_INV.Where(u => u.invid == invid).SingleOrDefault();
                        b2binv.val = invoicevalue;
                        db.SaveChanges();

                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CS invoice item added" + invoice, "");
                    TempData["Message"] = "Item added successfully";
                }
            }
            return RedirectToAction("b2csedit", "outwardb2csedit", new { InvId = Session["inum"].ToString(), Invdate = Session["idt"].ToString() });
        }

        public JsonResult B2CSDelete(int Id)
        {
            TBL_EXT_GSTR1_B2CS_INV inv = db.TBL_EXT_GSTR1_B2CS_INV.Where(s => s.invid == Id).SingleOrDefault();
            var message = inv;
            int result = B2CsFunctions.B2CSItemDelete(Id);

            if (result == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CS invoice item deleted" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item deleted successfully";
            }
            else if (result == -1)
            {
                TempData["Message"] = "Item not available";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult B2CSUpdate(int id, string hsncode, string itemdesc, string qty, string uqc, string unitprice, string discount, string txval, string rate, string igsta, string cgsta, string sgsta, string cessa)
        {
            TBL_EXT_GSTR1_B2CS_INV inv = db.TBL_EXT_GSTR1_B2CS_INV.Where(u => u.invid == id).SingleOrDefault();
            var message = inv;

            var createdby = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.createdby).FirstOrDefault();
            int createdId = Convert.ToInt32(createdby);

            //condition Checking Start
            var taxblevalue = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.txval).SingleOrDefault();
            var igst = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.iamt).SingleOrDefault();
            var cgst = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.iamt).SingleOrDefault();
            var sgst = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.iamt).SingleOrDefault();
            var cess = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.csamt).SingleOrDefault();
            var TotalAmount = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.invid == id select li.val).SingleOrDefault();

            decimal oldinvoiceamount = Convert.ToDecimal(taxblevalue + igst + cess + cgst + sgst);
            decimal newinoiceamont = Convert.ToDecimal(txval) + Convert.ToDecimal(igsta) + Convert.ToDecimal(cessa) + Convert.ToDecimal(cgsta) + Convert.ToDecimal(sgsta);
            decimal TotalTaxAmount = Convert.ToDecimal(TotalAmount);

            decimal Invoicevalue = TotalTaxAmount - oldinvoiceamount + newinoiceamont;
            if (Invoicevalue < 250000)
            {

                int output = B2CsFunctions.B2CSItemUpdate(id, hsncode, itemdesc, Convert.ToDecimal(qty), Convert.ToDecimal(unitprice), Convert.ToDecimal(discount), uqc, Convert.ToDecimal(rate), Convert.ToDecimal(txval), Convert.ToDecimal(igsta), Convert.ToDecimal(cgsta), Convert.ToDecimal(sgsta), Convert.ToDecimal(cessa), createdId);

                if (output == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CS invoice ID" + inv + "updated", "");
                    TempData["Message"] = "Invoice Item  Details Updated Successfully";
                }

                else if (output == -1)
                {
                    TempData["Message"] = "Item Not exist for Given Invoice";
                }

            }
            else
            {
                TempData["Message"] = "B2CS invoice value is for less than 2.5 Lakhs";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
    }
}