using Rotativa;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WeP_BAL.TermsandConditions;

namespace SmartAdminMvc.Controllers
{
    public class DownloadPdfController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        
        static int i,j;
        static string Iid;
        static string Idate;
        static string InvType;
        static string MakerCheckerApproverSetting;
        static string strLogoPath;

        #region "B2B"
        public ActionResult Demo(string InvId, string InvDate)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            i = CustId;
            j = UserId;
            Iid = InvId;
            Idate = InvDate;
            InvType = Session["InvFormat"].ToString();
            ViewBag.InvFormat = Session["InvFormat"].ToString();
            MakerCheckerApproverSetting = Session["MakerCheckerApproverSetting"].ToString();
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i,j);
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR1_B2B_INV> inv = (from li in db.TBL_EXT_GSTR1_B2B_INV
                                               where li.inum == InvId && li.idt == InvDate
                                               select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR1_B2B_INV
                        where li.inum == InvId && li.idt == InvDate
                        select li.ctin).FirstOrDefault();

            var addinfo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.Addinfo).FirstOrDefault();

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2B_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.gstin).FirstOrDefault();
            var rchrg = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.rchrg).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var Shippinadd = (from ul in db.TBL_EXT_GSTR1_B2B_INV
                              where ul.inum == InvId && ul.idt == InvDate
                              select ul.ShippingAddress).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2B_INV
                       where li.inum == InvId && li.idt == InvDate
                       select li.pos).FirstOrDefault();


            var cusgst = (from li in db.TBL_EXT_GSTR1_B2B_INV
                          where li.inum == InvId && li.idt == InvDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                       where li.inum == InvId && li.idt == InvDate && li.CINno != null
                       select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == InvId && li.idt == InvDate && li.GoodsAndService != null
                         select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == InvId && li.idt == InvDate && li.TransportMode != null
                         select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                             where li.inum == InvId && li.idt == InvDate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2B_INV
                             where li.inum == InvId && li.idt == InvDate && li.DateOfSupply != null
                             select li.DateOfSupply).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }

            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;           

            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.Shippinadd = Shippinadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvId;
            ViewBag.InvDate = InvDate;
            ViewBag.rchrg = rchrg;
            ViewBag.addinfo = addinfo;
            ViewBag.LogoPath = strLogoPath;

            Session["B2B_InvID"] = InvId;
            return View();
        }


        public ActionResult ExportPdf()
        {
            return new ActionAsPdf("demo1")
            {
                //FileName = Server.MapPath("~/Content/Invoice.pdf")
                FileName = Session["B2B_InvID"].ToString() + ".pdf"
            };
        }

        public ActionResult demo1()
        {

            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            List<TBL_EXT_GSTR1_B2B_INV> inv = (from li in db.TBL_EXT_GSTR1_B2B_INV
                                               where li.inum == Iid && li.idt == Idate
                                               select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR1_B2B_INV
                        where li.inum == Iid && li.idt == Idate
                        select li.ctin).FirstOrDefault();
            var buyerid = (from li in db.TBL_EXT_GSTR1_B2B_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.gstin).FirstOrDefault();
            var addinfo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.Addinfo).FirstOrDefault();
            var rchrg = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.rchrg).FirstOrDefault();
            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var Shippinadd = (from ul in db.TBL_EXT_GSTR1_B2B_INV
                              where ul.inum == Iid && ul.idt == Idate
                              select ul.ShippingAddress).FirstOrDefault();
            var suplieradd = (from ul in db.TBL_EXT_GSTR1_B2B_INV
                              where ul.inum == Iid && ul.idt == Idate
                              select ul.ShippingAddress).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2B_INV
                       where li.inum == Iid && li.idt == Idate
                       select li.pos).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_B2B_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();
            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();
            var cinNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == Iid && li.idt == Idate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2B_INV
                               where li.inum == Iid && li.idt == Idate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2B_INV
                             where li.inum == Iid && li.idt == Idate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                             where li.inum == Iid && li.idt == Idate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2B_INV
                                where li.inum == Iid && li.idt == Idate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();



            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }

            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.Shippinadd = Shippinadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = Iid;
            ViewBag.InvDate = Idate;
            ViewBag.addinfo = addinfo;
            ViewBag.rchrg = rchrg;
            ViewBag.LogoPath = strLogoPath;

            return PartialView("_pdfdownload");
        }
        #endregion

        #region "CDNR"
        public ActionResult Cdnr(string InvId, string InvDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            i = CustId;
            j = UserId;
            Iid = InvId;
            Idate = InvDate;
            InvType = Session["InvFormat"].ToString();
            ViewBag.InvFormat = Session["InvFormat"].ToString();
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            MakerCheckerApproverSetting = Session["MakerCheckerApproverSetting"].ToString();
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR1_CDNR> inv = (from li in db.TBL_EXT_GSTR1_CDNR
                                            where li.nt_num == InvId && li.nt_dt == InvDate
                                            select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR1_CDNR
                        where li.nt_num == InvId && li.nt_dt == InvDate
                        select li.ctin).FirstOrDefault();

            var ntty = (from li in db.TBL_EXT_GSTR1_CDNR
                        where li.nt_num == InvId && li.nt_dt == InvDate
                        select li.ntty).FirstOrDefault();

            var invnumber = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.nt_num == InvId && li.nt_dt == InvDate
                             select li.inum).FirstOrDefault();

            var invdate = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == InvId && li.nt_dt == InvDate
                           select li.idt).FirstOrDefault();
            var addinfo = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == InvId && li.nt_dt == InvDate
                           select li.Addinfo).FirstOrDefault();

            var buyerid = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == InvId && li.nt_dt == InvDate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_CDNR
                         where li.nt_num == InvId && li.nt_dt == InvDate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();

            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_CDNR
                       where li.nt_num == InvId && li.nt_dt == InvDate
                       select li.pos).FirstOrDefault();
            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_CDNR
                          where li.nt_num == InvId && li.nt_dt == InvDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();
            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_CDNR
                         where li.inum == InvId && li.idt == InvDate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_CDNR
                               where li.inum == InvId && li.idt == InvDate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.inum == InvId && li.idt == InvDate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.inum == InvId && li.idt == InvDate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_CDNR
                                where li.inum == InvId && li.idt == InvDate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();


            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;

            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = invnumber;
            ViewBag.InvDate = invdate;
            ViewBag.addinfo = addinfo;
            ViewBag.ntty = ntty;
            ViewBag.Docno = InvId;
            ViewBag.Docdate = InvDate;
            ViewBag.LogoPath = strLogoPath;

            Session["CDNR_InvID"] = InvId;

            return View();
        }


        public ActionResult ExportCdnrPdf()
        {
            return new ActionAsPdf("cdnrpdf")
            {
                //FileName = Server.MapPath("~/Content/Invoice.pdf")
                FileName = Session["CDNR_InvID"].ToString() + ".pdf"
            };
        }

        public ActionResult cdnrpdf()
        {
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            List<TBL_EXT_GSTR1_CDNR> inv = (from li in db.TBL_EXT_GSTR1_CDNR
                                            where li.nt_num == Iid && li.nt_dt == Idate
                                            select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR1_CDNR
                        where li.nt_num == Iid && li.nt_dt == Idate
                        select li.ctin).FirstOrDefault();

            var ntty = (from li in db.TBL_EXT_GSTR1_CDNR
                        where li.nt_num == Iid && li.nt_dt == Idate
                        select li.ntty).FirstOrDefault();

            var invnumber = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.nt_num == Iid && li.nt_dt == Idate
                             select li.inum).FirstOrDefault();

            var invdate = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == Iid && li.nt_dt == Idate
                           select li.idt).FirstOrDefault();

            var addinfo = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == Iid && li.nt_dt == Idate
                           select li.Addinfo).FirstOrDefault();

            var buyerid = (from li in db.TBL_EXT_GSTR1_CDNR
                           where li.nt_num == Iid && li.nt_dt == Idate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_CDNR
                         where li.nt_num == Iid && li.nt_dt == Idate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_CDNR
                       where li.nt_num == Iid && li.nt_dt == Idate
                       select li.pos).FirstOrDefault();

            var cusgst = (from li in db.TBL_EXT_GSTR1_CDNR
                          where li.nt_num == Iid && li.nt_dt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_CDNR
                         where li.inum == Iid && li.idt ==Idate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_CDNR
                               where li.inum == Iid && li.idt == Idate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.inum == Iid && li.idt == Idate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_CDNR
                             where li.inum == Iid && li.idt == Idate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_CDNR
                                where li.inum == Iid && li.idt == Idate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;

            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = invnumber;
            ViewBag.InvDate = invdate;
            ViewBag.ntty = ntty;
            ViewBag.Docno = Iid;
            ViewBag.addinfo = addinfo;
            ViewBag.Docdate = Idate;
            ViewBag.LogoPath = strLogoPath;

            return PartialView("_CdnrPdf");

        }

        #endregion

        #region "B2CL"
        public ActionResult B2CL(string InvId, string InvDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            i = CustId;
            j = UserId;
            Iid = InvId;
            Idate = InvDate;
            InvType = Session["InvFormat"].ToString();
            ViewBag.InvFormat = Session["InvFormat"].ToString();
            MakerCheckerApproverSetting = Session["MakerCheckerApproverSetting"].ToString();
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR1_B2CL_INV> inv = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                                                where li.inum == InvId && li.idt == InvDate
                                                select li).ToList();

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                       where li.inum == InvId && li.idt == InvDate
                       select li.pos).FirstOrDefault();

            var addinfo = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                          where li.inum == InvId && li.idt == InvDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == InvId && li.idt == InvDate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                               where li.inum == InvId && li.idt == InvDate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                             where li.inum == InvId && li.idt == InvDate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                             where li.inum == InvId && li.idt == InvDate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                                where li.inum == InvId && li.idt == InvDate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();


            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvId;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = InvDate;
            ViewBag.LogoPath = strLogoPath;

            Session["B2CL_InvID"] = InvId;
            return View();
        }

        public ActionResult ExportB2CLPdf()
        {
            return new ActionAsPdf("b2clpdf")
            {
                //FileName = Server.MapPath("~/Content/Invoice.pdf")
                FileName = Session["B2CL_InvID"].ToString() + ".pdf"
            };
        }

        public ActionResult b2clpdf()
        {
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            List<TBL_EXT_GSTR1_B2CL_INV> inv = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                                                where li.inum == Iid && li.idt == Idate
                                                select li).ToList();

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                       where li.inum == Iid && li.idt == Idate
                       select li.pos).FirstOrDefault();
            var addinfo = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();


            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_B2B_INV
                         where li.inum == Iid && li.idt == Idate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                               where li.inum == Iid && li.idt == Idate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                             where li.inum == Iid && li.idt == Idate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                             where li.inum == Iid && li.idt == Idate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2CL_INV
                                where li.inum == Iid && li.idt == Idate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();


            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = Iid;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = Idate;
            ViewBag.LogoPath = strLogoPath;

            return PartialView("_B2clPdf");
        }

        #endregion

        #region "B2CS"
        public ActionResult B2CS(string InvId, string InvDate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            i = CustId;
            j = UserId;
            Iid = InvId;
            Idate = InvDate;
            InvType = Session["InvFormat"].ToString();
            ViewBag.InvFormat = Session["InvFormat"].ToString();
            MakerCheckerApproverSetting = Session["MakerCheckerApproverSetting"].ToString();
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR1_B2CS_INV> inv = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                                where li.inum == InvId && li.idt == InvDate
                                                select li).ToList();

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                       where li.inum == InvId && li.idt == InvDate
                       select li.pos).FirstOrDefault();

            var addinfo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                          where li.inum == InvId && li.idt == InvDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            var cinNo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                         where li.inum == InvId && li.idt == InvDate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                               where li.inum == InvId && li.idt == InvDate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                             where li.inum == InvId && li.idt == InvDate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                             where li.inum == InvId && li.idt == InvDate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                where li.inum == InvId && li.idt == InvDate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();


            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvId;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = InvDate;
            ViewBag.LogoPath = strLogoPath;

            Session["B2CS_InvID"] = InvId;
            return View();
        }

        public ActionResult ExportB2CSPdf()
        {
            return new ActionAsPdf("b2cspdf")
            {
                //FileName = Server.MapPath("~/Content/Invoice.pdf")
                FileName = Session["B2CS_InvID"].ToString() + ".pdf"
            };
        }

        public ActionResult b2cspdf()
        {
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            List<TBL_EXT_GSTR1_B2CS_INV> inv = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                                where li.inum == Iid && li.idt == Idate
                                                select li).ToList();

            var buyerid = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.buyerid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.gstin).FirstOrDefault();

            var supname = (from ul in db.TBL_Buyer
                           where ul.BuyerId == buyerid
                           select ul.BuyerName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.Address).FirstOrDefault();
            var supgst = (from ul in db.TBL_Buyer
                          where ul.BuyerId == buyerid
                          select ul.GSTINno).FirstOrDefault();

            var pos = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                       where li.inum == Iid && li.idt == Idate
                       select li.pos).FirstOrDefault();
            var addinfo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.Addinfo).FirstOrDefault();
            var cusgst = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();


            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();
            var cinNo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                         where li.inum == Iid && li.idt == Idate && li.CINno != null
                         select li.CINno).FirstOrDefault();

            var serviceType = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                               where li.inum == Iid && li.idt == Idate && li.GoodsAndService != null
                               select li.GoodsAndService).FirstOrDefault();

            var transMode = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                             where li.inum == Iid && li.idt == Idate && li.TransportMode != null
                             select li.TransportMode).FirstOrDefault();

            var vehicleNo = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                             where li.inum == Iid && li.idt == Idate && li.vehicleNo != null
                             select li.vehicleNo).FirstOrDefault();

            var dateOfSupply = (from li in db.TBL_EXT_GSTR1_B2CS_INV
                                where li.inum == Iid && li.idt == Idate && li.DateOfSupply != null
                                select li.DateOfSupply).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            ViewBag.cinNo = cinNo;
            ViewBag.serviceType = serviceType;
            ViewBag.transMode = transMode;
            ViewBag.vehicleNo = vehicleNo;
            ViewBag.dateOfSupply = dateOfSupply;
            ViewBag.InvFormat = InvType;

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = Iid;
            ViewBag.addinfo = addinfo;
            ViewBag.InvDate = Idate;
            ViewBag.LogoPath = strLogoPath;

            return PartialView("_B2clPdf");
        }

        #endregion


        #region "G2B2B"
        public ActionResult G2B2B(string InvId, string InvDate)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            i = CustId;
            j = UserId;
            Iid = InvId;
            Idate = InvDate;
            InvType = Session["InvFormat"].ToString();
            ViewBag.InvFormat = Session["InvFormat"].ToString();
            MakerCheckerApproverSetting = Session["MakerCheckerApproverSetting"].ToString();
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR2_B2B_INV> inv = (from li in db.TBL_EXT_GSTR2_B2B_INV
                                               where li.inum == InvId && li.idt == InvDate
                                               select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR2_B2B_INV
                        where li.inum == InvId && li.idt == InvDate
                        select li.ctin).FirstOrDefault();
            
            var supplierid = (from li in db.TBL_EXT_GSTR2_B2B_INV
                           where li.inum == InvId && li.idt == InvDate
                           select li.supplierid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR2_B2B_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.gstin).FirstOrDefault();
            var rchrg = (from li in db.TBL_EXT_GSTR2_B2B_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.rchrg).FirstOrDefault();

            var supname = (from ul in db.TBL_Supplier
                           where ul.SupplierId == supplierid
                           select ul.SupplierName).FirstOrDefault();

            var supadd = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supplierid
                          select ul.Address).FirstOrDefault();

            var supgst = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supplierid
                          select ul.GSTINno).FirstOrDefault();

            var pos = gstin.ToString().Substring(0, 2);


            var cusgst = (from li in db.TBL_EXT_GSTR2_B2B_INV
                          where li.inum == InvId && li.idt == InvDate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }
            

            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvId;
            ViewBag.InvDate = InvDate;
            ViewBag.rchrg = rchrg;
            ViewBag.LogoPath = strLogoPath;

            Session["G2B2B_InvID"] = InvId;
            return View();
        }


        public ActionResult G2B2BExportPdf()
        {
            return new ActionAsPdf("G2B2BPrint")
            {
                FileName = Session["G2B2B_InvID"].ToString() + ".pdf"
            };
        }

        public ActionResult G2B2BPrint()
        {

            ViewBag.TermsAndCondition = TermandCondition.GetTermsAndConditions(i, j);
            List<TBL_EXT_GSTR2_B2B_INV> inv = (from li in db.TBL_EXT_GSTR2_B2B_INV
                                               where li.inum == Iid && li.idt == Idate
                                               select li).ToList();
            var ctin = (from li in db.TBL_EXT_GSTR2_B2B_INV
                        where li.inum == Iid && li.idt == Idate
                        select li.ctin).FirstOrDefault();
            var supplierid = (from li in db.TBL_EXT_GSTR2_B2B_INV
                           where li.inum == Iid && li.idt == Idate
                           select li.supplierid).FirstOrDefault();
            var gstin = (from li in db.TBL_EXT_GSTR2_B2B_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.gstin).FirstOrDefault();
            
            var rchrg = (from li in db.TBL_EXT_GSTR2_B2B_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.rchrg).FirstOrDefault();
            var supname = (from ul in db.TBL_Supplier
                           where ul.SupplierId == supplierid
                           select ul.SupplierName).FirstOrDefault();
            var supadd = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supplierid
                          select ul.Address).FirstOrDefault();
            
            var supgst = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supplierid
                          select ul.GSTINno).FirstOrDefault();

            var pos = gstin.ToString().Substring(0, 2);

            var cusgst = (from li in db.TBL_EXT_GSTR2_B2B_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();
            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }

            ViewBag.InvFormat = InvType;
            ViewBag.MakerCheckerApproverSetting = MakerCheckerApproverSetting;
            ViewBag.gstin = gstin;
            ViewBag.ctin = ctin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.supgst = supgst;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = Iid;
            ViewBag.InvDate = Idate;
            ViewBag.rchrg = rchrg;
            ViewBag.LogoPath = strLogoPath;

            return PartialView("_g2b2b");
        }
        #endregion

        #region "B2BUR"
        public ActionResult Reversecharge(string InvId, string InvDate)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            i = CustId;
            Iid = InvId;
            Idate = InvDate;
            strLogoPath = Session["InvoicePrintLogo"].ToString();

            List<TBL_EXT_GSTR2_B2BUR_INV> inv = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                                                 where li.inum == InvId && li.idt == InvDate
                                                 select li).ToList();


            var supid = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.supplierid).FirstOrDefault();

            var gstin = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                         where li.inum == InvId && li.idt == InvDate
                         select li.gstin).FirstOrDefault();

            var suplierid = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                             where li.inum == InvId && li.idt == InvDate
                             select li.supplierid).FirstOrDefault();
            int sid = Convert.ToInt32(suplierid);

            var sname = (from li in db.TBL_Supplier where li.SupplierId == sid select li.SupplierName).FirstOrDefault();
            ViewBag.SupplierName = sname;

            var supname = (from ul in db.TBL_Supplier
                           where ul.SupplierId == supid
                           select ul.SupplierName).FirstOrDefault();

            var supadd = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supid
                          select ul.Address).FirstOrDefault();

            string pos = gstin.ToString().Substring(0, 2);

            var cusgst = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == CustId && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == CustId && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();

            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = InvId;
            ViewBag.InvDate = InvDate;
            ViewBag.LogoPath = strLogoPath;
            Session["B2BUR_InvID"] = InvId;
            return View();
        }
        public ActionResult ExportReversePdf()
        {
            return new ActionAsPdf("ReversePDF")
            {
                //FileName = Server.MapPath("~/Content/Invoice.pdf")
                FileName = Session["B2BUR_InvID"].ToString() + ".pdf"
            };
        }
        
        public ActionResult ReversePDF()
        {
            List<TBL_EXT_GSTR2_B2BUR_INV> inv = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                                                 where li.inum == Iid && li.idt == Idate
                                                 select li).ToList();

            var supid = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.supplierid).FirstOrDefault();

            var gstin = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                         where li.inum == Iid && li.idt == Idate
                         select li.gstin).FirstOrDefault();

            var suplierid = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                             where li.inum == Iid && li.idt == Idate
                             select li.supplierid).FirstOrDefault();
            int sid = Convert.ToInt32(suplierid);

            var sname = (from li in db.TBL_Supplier where li.SupplierId == sid select li.SupplierName).FirstOrDefault();
            ViewBag.SupplierName = sname;
            var supname = (from ul in db.TBL_Supplier
                           where ul.SupplierId == supid
                           select ul.SupplierName).FirstOrDefault();

            var supadd = (from ul in db.TBL_Supplier
                          where ul.SupplierId == supid
                          select ul.Address).FirstOrDefault();

            string pos = gstin.ToString().Substring(0, 2);
            var cusgst = (from li in db.TBL_EXT_GSTR2_B2BUR_INV
                          where li.inum == Iid && li.idt == Idate
                          select li.gstin).FirstOrDefault();

            var cusAddress = (from li in db.TBL_Cust_GSTIN
                              where li.CustId == i && li.GSTINNo == cusgst && li.rowstatus == true
                              select li.Address).FirstOrDefault();

            string panno = cusgst.ToString().Substring(2, 10);

            var cusname = (from li in db.TBL_Cust_PAN
                           where li.CustId == i && li.PANNo == panno && li.rowstatus == true
                           select li.CompanyName).FirstOrDefault();

            var statename = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateName).FirstOrDefault();
            var StateType = (from li in db.TBL_STATE_MASTER
                             where li.StateCode == pos
                             select li.StateType).FirstOrDefault();

            if (StateType == "STATE")
            {
                ViewBag.typegst = "SGST";
            }
            else
            {
                ViewBag.typegst = "UTGST";
            }

            ViewBag.gstin = gstin;
            ViewBag.supname = supname;
            ViewBag.supadd = supadd;
            ViewBag.pos = pos;
            ViewBag.state = statename;
            ViewBag.cusname = cusname;
            ViewBag.cusAddress = cusAddress;
            ViewBag.cusgst = cusgst;
            ViewBag.inv = inv;
            ViewBag.InvID = Iid;
            ViewBag.InvDate = Idate;
            ViewBag.LogoPath = strLogoPath;
            return PartialView("_ReversePDF");
        }

        #endregion

        [System.Web.Mvc.HttpGet]
        public ActionResult InvoicePdf()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            
            ViewBag.MakerList = LoadDropDowns.GetMakerUserlist(CustId, UserId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CL",
                Value = "B2CL"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CS",
                Value = "B2CS"
            });
            items.Add(new SelectListItem
            {
                Text = "CDNR",
                Value = "CDNR"
            });
            items.Add(new SelectListItem
            {
                Text = "EXP",
                Value = "EXP"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult InvoicePdf(FormCollection form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string RefNo = Session["CustRefNo"].ToString();

            string MakerId = "", strUserId = "";
            MakerId = form["ddlMaker"];
            string action = form["option"];
            TempData["option"] = form["option"];
            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                strUserId = MakerId;
                TempData["UserId"] = MakerId;
            }
            else
            {
                strUserId = Convert.ToString(UserId);
                TempData["UserId"] = UserId;
            }
            TempData["RefNo"] = RefNo;
            Session["op"] = action;

            var invoice = Getinvoice(action, strUserId, RefNo);
            ViewBag.invoice = invoice;

            ViewBag.MakerList = LoadDropDowns.Exist_GetMakerUserlist(CustId, UserId, MakerId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CL",
                Value = "B2CL"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CS",
                Value = "B2CS"
            });
            items.Add(new SelectListItem
            {
                Text = "CDNR",
                Value = "CDNR"
            });
            items.Add(new SelectListItem
            {
                Text = "EXP",
                Value = "EXP"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value", Convert.ToString(TempData["option"]));
            return View();
        }

        public List<IDictionary> Getinvoice(string action, string strUserId, string strRefNo)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Generate_PDF_GSTR1_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", strRefNo));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", action));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", strUserId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
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

        public ActionResult Delete(string InvId, string Invdate, string ActionType, string strUserId, string strRefNo)
        {
            string CustRefNo = Session["CustRefNo"].ToString();


            if (ActionType == "B2B")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.invid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR1_EXT(ActionType, Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 B2CL Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete..!";
                    }
                }

            }


            else if (ActionType == "B2CL")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_B2CL_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.invid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR1_EXT(ActionType, Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 B2CL Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete..!";
                    }
                }

            }


            else if (ActionType == "B2CS")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_B2CS_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.invid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR1_EXT("B2CS", Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 B2CS Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete..!";
                    }
                }

            }

            else if (ActionType == "EXP")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_EXP_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.invid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR1_EXT("EXP", Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 EXP Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete..!";
                    }
                }

            }

            else if (ActionType == "CDNR")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_CDNR where li.nt_num == InvId && li.nt_dt == Invdate && li.referenceno == CustRefNo select li.cdnrid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR1_EXT(ActionType, Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 B2CL Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete..!";
                    }
                }

            }

            var invoice = Getinvoice(ActionType, strUserId, strRefNo);
            ViewBag.invoice = invoice;


            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CL",
                Value = "B2CL"
            });
            items.Add(new SelectListItem
            {
                Text = "B2CS",
                Value = "B2CS"
            });
            items.Add(new SelectListItem
            {
                Text = "CDNR",
                Value = "CDNR"
            });
            items.Add(new SelectListItem
            {
                Text = "EXP",
                Value = "EXP"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value", ActionType);

            return RedirectToAction("InvoicePdf");
        }

    }
}
