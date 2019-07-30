using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.CDNR;
using System;
using System.Collections.Generic;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models.EditInvoice;

namespace SmartAdminMvc.Controllers
{
    public class OutwardCDNREditController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        CDNRFunctions CDNRSP = new CDNRFunctions();
        // GET: OutwardCDNREdit
        public ActionResult Home(string InvId, string InvDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            Session["inum"] = InvId;
            Session["idt"] = InvDate;
            string refnum;
            refnum = Session["CustRefNo"].ToString();

            var records = (from li in db.TBL_EXT_GSTR1_CDNR where li.nt_num == InvId && li.nt_dt == InvDate && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("invoicepdf", "downloadpdf");
            }
            else
            {

                var buyname = db.TBL_Buyer.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { buyername = c.BuyerName, buyerid = c.BuyerId }).OrderBy(c => c.buyername).ToList();
                ViewBag.buyname = buyname;
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");

                var master = (from cdnr in db.TBL_EXT_GSTR1_CDNR
                              where cdnr.nt_num == InvId && cdnr.nt_dt == InvDate && cdnr.referenceno == refnum
                              select cdnr).FirstOrDefault();

                object lineitms = (from cdnr in db.TBL_EXT_GSTR1_CDNR
                                   where cdnr.nt_num == InvId && cdnr.nt_dt == InvDate && cdnr.referenceno == refnum
                                   select cdnr).ToList();

                string buyerid = (from cdnr in db.TBL_EXT_GSTR1_CDNR
                                  where cdnr.nt_num == InvId && cdnr.nt_dt == InvDate && cdnr.referenceno == refnum
                                  select cdnr.buyerid).First().ToString();

                var serviceType = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                   where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.GoodsAndService != null
                                   select li.GoodsAndService).FirstOrDefault();

                var transMode = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                 where li.inum == InvId && li.idt == InvDate && li.referenceno == refnum && li.TransportMode != null
                                 select li.TransportMode).FirstOrDefault();
                Session["transModecdnr"] = transMode;
                Session["serviceTypecdnr"] = serviceType;
                if (Session["transModecdnr"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModecdnr"].ToString());
                }
                else
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                }
                if (Session["serviceTypecdnr"] != null)
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypecdnr"].ToString());
                }
                else
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
                }


                ViewBag.master = master;
                ViewBag.lineitms = lineitms;
                int buyid = Convert.ToInt32(buyerid);
                ViewBag.buyername = db.TBL_Buyer.Where(b => b.BuyerId == buyid).Select(b => b.BuyerName).SingleOrDefault().ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult Home(FormCollection form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string refnum, Iid, Idate;
            Iid = Convert.ToString(Session["inum"]);
            Idate = Convert.ToString(Session["idt"]);
            refnum = Convert.ToString(Session["CustRefNo"]);

            string gstin, ctin, cfs, notetype, noteno, notedate, invoiceno, invoicedate, pos, itemdesc, uqc, addinfo, Pregst, rsn;
            decimal Invoicevalue, rate, txval, iamt, camt, samt, csamt, qty, unitprice, discount;
            int buyerid, custid, userid;
            string serviceType = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";

            addinfo = form["comment1"];
            custid = Convert.ToInt32(Session["Cust_ID"]);
            userid = Convert.ToInt32(Session["User_Id"]);
            ctin = form["gstin"];
            buyerid = Convert.ToInt32(form["name"]);
            gstin = form["cgstin"];
            rsn = form["rns"];
            pos = form["pos"];
            cfs = form["cfs"];
            invoiceno = form["invoice"];
            Pregst = form["pgst"];
            invoicedate = form["invoicedate"];
            txval = Convert.ToDecimal(form["taxvalue"]);
            notetype = form["notetype"];
            noteno = form["notenum"];
            string period = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
            notedate = form["notedate"];
            Invoicevalue = Convert.ToDecimal(form["invalue"]);

            serviceType = form["ddlServiceType"];
            transMode = form["transMode"];
            vechicleNo = form["vehicleNo"];
            dateOfSupply = form["dateOfSupply"];
            cinNo = form["cinNo"];

            Session["transModecdnr"] = transMode;
            Session["serviceTypecdnr"] = serviceType;
            if (Session["transModecdnr"] != null)
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModecdnr"].ToString());
            }
            else
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
            }
            if (Session["serviceTypecdnr"] != null)
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypecdnr"].ToString());
            }
            else
            {
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
            }

            if (command == "save")
            {
                int output = CDNRSP.CDNRMasterUpdate(gstin, period, ctin, notetype, noteno, notedate, invoiceno, invoicedate, refnum, userid, addinfo, serviceType, transMode, vechicleNo, dateOfSupply, cinNo);
                if (output == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward CDNR invoice updated" + invoiceno, "");
                    TempData["Message"] = "Details updated successfully";
                }
                else
                {
                    TempData["Message"] = "Details not updated successfully";
                }
            }

            else if (command == "generate")
            {
                return RedirectToAction("Cdnr", "DownloadPDF", new { Invid = Iid, Invdate = Idate });
            }


            else if (command == "add")
            {
                string hsncode = Convert.ToString(form["HSN"]);
                itemdesc = form["itemdesc"];
                txval = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                camt = Convert.ToDecimal(form["camount"]);
                samt = Convert.ToDecimal(form["samount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                decimal invalue = Invoicevalue + txval + iamt + camt + samt + csamt;



                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                db.usp_Insert_Outward_GSTR1_CDNR_EXT1(gstin, period, ctin, cfs, notetype, noteno, notedate, invoiceno, invoicedate, invalue, pos, rate, txval, iamt, camt, samt, csamt, refnum, hsncode, itemdesc, qty, unitprice, discount, uqc, buyerid, userid, addinfo, Pregst, rsn, serviceType, transMode, vechicleNo, dateOfSupply, cinNo, op);
                var id = (from li in db.TBL_EXT_GSTR1_CDNR where li.nt_num == noteno && li.nt_dt == notedate && li.referenceno == refnum select li.cdnrid).ToList();
                foreach (int cdnrid in id)
                {
                    TBL_EXT_GSTR1_CDNR cdnrinv = db.TBL_EXT_GSTR1_CDNR.Where(u => u.cdnrid == cdnrid).SingleOrDefault();
                    cdnrinv.val = invalue;
                    db.SaveChanges();

                }

                string ret = Convert.ToString(op.Value);
                if (ret == "-1")
                {
                    TempData["Message"] = "Item not Added.";
                    return RedirectToAction("Home", new { Invid = Iid, Invdate = Idate });
                }

                else if (ret == "-2")
                {
                    TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item  cannot be Added";
                    return RedirectToAction("Home", new { Invid = Iid, Invdate = Idate });
                }
                TempData["Message"] = "Item added successfully";
                B2CsFunctions.CDNRPush(refnum, gstin, Convert.ToInt32(Session["Cust_ID"]), Convert.ToInt32(Session["User_ID"]));
                //db.usp_Push_GSTR1CDNR_EXT_SA("Manual", refnum, gstin);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice item added" + invoiceno, "");

            }

            return RedirectToAction("Home", new { Invid = Iid, Invdate = Idate });
        }


        [HttpPost]
        public JsonResult Edit(int id, string hsncode, string itemdesc, decimal qty, string uqc, decimal unitprice, decimal discount, decimal txval, decimal rate, decimal igsta, decimal cgsta, decimal sgsta, decimal cessa)
        {

            TBL_EXT_GSTR1_CDNR inv = db.TBL_EXT_GSTR1_CDNR.Where(s => s.cdnrid == id).SingleOrDefault();
            var message = inv;
            string refnum = Convert.ToString(Session["CustRefNo"]);
            int userid = Convert.ToInt32(Session["User_Id"]);

            int output = CDNRSP.CDNRItemUpdate(id, rate, txval, igsta, cgsta, sgsta, cessa, refnum, hsncode, itemdesc, qty, unitprice, discount, uqc, userid);

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
            TBL_EXT_GSTR1_CDNR inv = db.TBL_EXT_GSTR1_CDNR.Where(s => s.cdnrid == Id).SingleOrDefault();
            var message = inv;

            int output = CDNRSP.CDNRItemDelete(Id);

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

    }
}