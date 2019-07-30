using Rotativa;
using SmartAdminMvc.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class PdfDownloadController : Controller
    {
        readonly WePGSPDBEntities db = new WePGSPDBEntities();
        static int i;
        static string InvNo;
        static string Invdate;
        // GET: PDFDownload
        public ActionResult EXP(string InvoiceNo, string InvoiceDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["Cust_ID"] != null)
            {
                i = Convert.ToInt32(Session["Cust_ID"]);
            }


            InvNo = InvoiceNo;
            Invdate = InvoiceDate;
            List<TBL_EXT_GSTR1_EXP_INV> inv = (from li in db.TBL_EXT_GSTR1_EXP_INV
                                                where li.inum == InvoiceNo && li.idt == InvoiceDate
                                                select li).ToList();

            
            var gstin = (from li in db.TBL_EXT_GSTR1_EXP_INV
                         where li.inum == InvoiceNo && li.idt == InvoiceDate
                         select li.gstin).FirstOrDefault();
            var ShippingAdd = (from li in db.TBL_EXT_GSTR1_EXP_INV
                         where li.inum == InvoiceNo && li.idt == InvoiceDate
                         select li.ShippingAddress).FirstOrDefault();
            var SName = (from li in db.TBL_EXT_GSTR1_EXP_INV
                               where li.inum == InvoiceNo && li.idt == InvoiceDate
                               select li.ReceiverName).FirstOrDefault();
            var buyerid = (from li in db.TBL_EXT_GSTR1_EXP_INV
                           where li.inum == InvoiceNo && li.idt == InvoiceDate
                           select li.buyerid).FirstOrDefault();
            int Bid = Convert.ToInt32(buyerid);
            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == Bid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == Bid
                          select ul.Address).FirstOrDefault();
            var addinfo = (from li in db.TBL_EXT_GSTR1_EXP_INV
                           where li.inum == InvoiceNo && li.idt == InvoiceDate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_EXP_INV
                          where li.inum == InvoiceNo && li.idt == InvoiceDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();
            

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.ShippingAdd = ShippingAdd;
            ViewBag.SName = SName;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvoiceNo;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = InvoiceDate;
            Session["EXP_InvID"] = InvNo;
            return View();
        }


        public ActionResult ExportEXPPdf()
        {

            return new ActionAsPdf("EXPPDF")
            {
               
                FileName = Session["EXP_InvID"].ToString() + ".pdf"
            };

        }

        public ActionResult EXPPDF()
        {



            List<TBL_EXT_GSTR1_EXP_INV> inv = (from li in db.TBL_EXT_GSTR1_EXP_INV
                                               where li.inum == InvNo && li.idt == Invdate
                                                select li).ToList();

            
            var gstin = (from li in db.TBL_EXT_GSTR1_EXP_INV
                         where li.inum == InvNo && li.idt == Invdate
                         select li.gstin).FirstOrDefault();

            var buyerid = (from li in db.TBL_EXT_GSTR1_EXP_INV
                           where li.inum == InvNo && li.idt == Invdate
                           select li.buyerid).FirstOrDefault();
            var SName = (from li in db.TBL_EXT_GSTR1_EXP_INV
                         where li.inum == InvNo && li.idt == Invdate
                         select li.ReceiverName).FirstOrDefault();
            int Bid = Convert.ToInt32(buyerid);
            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == Bid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == Bid
                          select ul.Address).FirstOrDefault();


            var addinfo = (from li in db.TBL_EXT_GSTR1_EXP_INV
                           where li.inum == InvNo && li.idt == Invdate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_EXP_INV
                          where li.inum == InvNo && li.idt == Invdate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();


           
            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.SName = SName;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvNo;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = Invdate;

            return PartialView("_EXPPdf");
        }

    }
}