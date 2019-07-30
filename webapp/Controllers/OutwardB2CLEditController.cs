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
    public class OutwardB2CLEditController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        B2ClFunctions B2CLSP = new B2ClFunctions();
        // GET: OutwardB2CLEdit
        [HttpGet]
        public ActionResult B2CLEdit(string InvId, string InvDate)
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

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
            ViewBag.Rates = LoadDropDowns.LoadRates();
            ViewBag.UQCList = LoadDropDowns.LoadUQC();
            ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                           where (li.inum == InvId && li.idt == InvDate && li.referenceno == refnum)
                           select li.buyerid).FirstOrDefault();
            int Bid = Convert.ToInt32(buyerid);
            Session["Bid"] = Bid;



            var master = (from b2b in db.TBL_EXT_GSTR1_B2CL_INV
                          where b2b.inum == InvId && b2b.idt == InvDate && b2b.referenceno == refnum && b2b.val != null
                          select b2b).FirstOrDefault();

            object lineitms = (from b2b in db.TBL_EXT_GSTR1_B2CL_INV
                               where b2b.inum == InvId && b2b.idt == InvDate && b2b.referenceno == refnum
                               select b2b).ToList();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                               where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                             where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();
            Session["transModeb2cl"] = transMode;
            Session["serviceTypeb2cl"] = serviceType;
            if (Session["transModeb2cl"] != null)
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cl"].ToString());
            }
            else
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
            }
            if (Session["serviceTypeb2cl"] != null)
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cl"].ToString());
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

        [HttpPost]
        public ActionResult B2CLEdit(FormCollection form, string command)
        {
            string refnum, Inid, Idate;
            Inid = Convert.ToString(Session["inum"]);
            Idate = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);
            string cgstin, invoice, pos, invoicedate, etin, itemdesc, uqc;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, csamt;
            string serviceType = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";

            string Invid = Convert.ToString(Session["inum"]);
            string InvDate = Convert.ToString(Session["idt"]);
            var invoiceid = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.inum == Invid && li.idt == InvDate select li.invid).FirstOrDefault();
            int Iid = Convert.ToInt32(invoiceid);
            int Bid = Convert.ToInt32(Session["Bid"]);
            // buyer Modification
            TBL_Buyer buyer = db.TBL_Buyer.Where(u => u.BuyerId == Bid).SingleOrDefault();

            buyer.BuyerName = form["buyername"];
            buyer.Address = form["address"];
            db.SaveChanges();

            int userid = Convert.ToInt32(Session["User_Id"]);
            string gstin = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Iid select li.gstin).FirstOrDefault();
            string fp = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Iid select li.fp).FirstOrDefault();
            string refno = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Iid select li.referenceno).FirstOrDefault();
            var createdby = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Iid select li.createdby).FirstOrDefault();
            int createdid = Convert.ToInt32(createdby);
            string addinfo = form["comment1"];


            etin = form["etin"];
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            if (form["invalue"] != null)
            {
                invalue = Convert.ToDecimal(form["invalue"]);
            }
            else
            {
                invalue = 0;
            }

            pos = form["pos"];

            serviceType = form["ddlServiceType"];
            transMode = form["transMode"];
            vechicleNo = form["vehicleNo"];
            dateOfSupply = form["dateOfSupply"];
            cinNo = form["cinNo"];

            Session["transModeb2cl"] = transMode;
            Session["serviceTypeb2cl"] = serviceType;
            if (Session["transModeb2cl"] != null)
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cl"].ToString());
            }
            else
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
            }
            if (Session["serviceTypeb2cl"] != null)
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cl"].ToString());
            }
            else
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
            }
            if (command == "save")
            {

                ObjectParameter op = new ObjectParameter("retval", typeof(int));
               db.usp_Update_OUTWARD_GSTR1_B2CL_Master(cgstin, fp, Invid, InvDate, invalue, pos, etin, refno, Bid, createdid, addinfo,serviceType, transMode, vechicleNo, dateOfSupply, cinNo, op);


                int ret = Convert.ToInt32(op.Value);
                if (ret == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice updated" + invoice, "");
                    TempData["Message"] = "Details updated successfully";
                }
                else if (ret == -1)
                {
                    TempData["Message"] = "Details not updated successfully";

                }
                else if (ret == -2)
                {
                    TempData["Message"] = "B2CL invoice format is for interstate with invoice value more than 2.5 Lakhs.";

                }

            }

            else if (command == "generate")
            {
                return RedirectToAction("B2CL", "DownloadPdf", new { Invid = Inid, Invdate = Idate });
            }

            else if (command == "add")
            {
                string hsnid = Convert.ToString(form["HSN"]);
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                decimal invoicevalue = 0;
                invoicevalue = invalue + taxablevalue + iamt + csamt;
                string strStateCode = form["cgstin"].ToString().Substring(0, 2);
                if (invoicevalue >= 250000 && strStateCode != pos)
                {
                    ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                   db.usp_Insert_Outward_GSTR1_B2CL_EXT1(cgstin, fp, invoice, invoicedate, invoicevalue, pos, etin, rate, taxablevalue, iamt, 0, 0, csamt, refno, hsnid, itemdesc, qty, unitprice, discount, uqc, Bid, userid, addinfo, serviceType, transMode, vechicleNo, dateOfSupply, cinNo, op);
                    var id = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.inum == invoice && li.idt == invoicedate && li.referenceno == refno select li.invid).ToList();
                    foreach (int invid in id)
                    {
                        TBL_EXT_GSTR1_B2CL_INV b2binv = db.TBL_EXT_GSTR1_B2CL_INV.Where(u => u.invid == invid).SingleOrDefault();
                        b2binv.val = invoicevalue;
                        db.SaveChanges();

                    }
                    string ret = Convert.ToString(op.Value);
                    if (ret == "-1")
                    {
                        TempData["Message"] = "Item not Added.";
                        return RedirectToAction("b2cledit", "outwardb2cledit", new { InvId = Session["inum"].ToString(), Invdate = Session["idt"].ToString() });
                    }

                    else if (ret == "-2")
                    {
                        TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item  cannot be Added";
                        return RedirectToAction("b2cledit", "outwardb2cledit", new { InvId = Session["inum"].ToString(), Invdate = Session["idt"].ToString() });
                    }
                    TempData["Message"] = "Item added successfully";
                    B2CsFunctions.B2CLPush(refno, cgstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CL invoice item added" + invoice, "");

                }
                else
                {
                    TempData["Message"] = "B2CL invoice format  for interstate with invoice value more than 2.5 Lakhs.";
                }
            }
            return RedirectToAction("b2cledit", "outwardb2cledit", new { InvId = Session["inum"].ToString(), Invdate = Session["idt"].ToString() });
        }

        public JsonResult B2CLDelete(int Id)
        {
            TBL_EXT_GSTR1_B2CL_INV inv = db.TBL_EXT_GSTR1_B2CL_INV.Where(s => s.invid == Id).SingleOrDefault();
            var message = inv;
            //condition Checking Start
            var taxblevalue = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Id select li.txval).SingleOrDefault();
            var igst = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Id select li.iamt).SingleOrDefault();
            var cess = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Id select li.csamt).SingleOrDefault();
            var TotalAmount = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == Id select li.val).SingleOrDefault();

            decimal oldinvoiceamount = Convert.ToDecimal(taxblevalue + igst + cess);
            decimal TotalTaxAmount = Convert.ToDecimal(TotalAmount);

            decimal Invoicevalue = TotalTaxAmount - oldinvoiceamount;
            if (Invoicevalue >= 250000)
            {
                int output = B2CLSP.B2CLItemDelete(Id);
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
            }
            else
            {
                TempData["Message"] = "We Can Not Delete because of B2CL invoice format for interstate with invoice value more than 2.5 Lakhs.";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult B2CLUpdate(int id, string hsncode, string itemdesc, decimal qty, string uqc, decimal unitprice, decimal discount, decimal txval, decimal rate, decimal igsta, decimal cessa)
        {
            TBL_EXT_GSTR1_B2CL_INV inv = db.TBL_EXT_GSTR1_B2CL_INV.Where(u => u.invid == id).SingleOrDefault();
            var message = inv;
            int userid = Convert.ToInt32(Session["User_Id"]);

            var createdby = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == id select li.createdby).FirstOrDefault();
            int createdId = Convert.ToInt32(createdby);

            //condition Checking Start
            var taxblevalue = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == id select li.txval).SingleOrDefault();
            var igst = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == id select li.iamt).SingleOrDefault();
            var cess = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == id select li.csamt).SingleOrDefault();
            var TotalAmount = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.invid == id select li.val).SingleOrDefault();

            decimal oldinvoiceamount = Convert.ToDecimal(taxblevalue + igst + cess);
            decimal newinoiceamont = Convert.ToDecimal(txval) + Convert.ToDecimal(igsta) + Convert.ToDecimal(cessa);
            decimal TotalTaxAmount = Convert.ToDecimal(TotalAmount);

            decimal Invoicevalue = 0;
            Invoicevalue = TotalTaxAmount - oldinvoiceamount + newinoiceamont;
            if (Invoicevalue >= 250000)
            {
                int output = B2CLSP.B2CLItemUpdate(id, rate, txval, igsta, 0, 0, cessa, hsncode, itemdesc, qty, unitprice, discount, uqc, userid);

                if (output == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CL invoice ID" + inv + "updated", "");
                    TempData["Message"] = "Invoice Item  Details Updated Successfully";
                }

                else if (output == -1)
                {
                    TempData["Message"] = "Item not available";
                }

                else if (output == -2)
                {
                    TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item details cannot be Updated";
                }

            }
            else
            {
                TempData["Message"] = "B2CL invoice format is for interstate with invoice value more than 2.5 Lakhs.";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }


    }
}