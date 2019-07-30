using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardB2CSAController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: OutwardB2CSA
        public ActionResult Home()
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
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public ActionResult Save(FormCollection form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string cgstin, prs, onum, odate, hsn, type, typ, etin, statecode, omonth, ohsn, ostatecode, otype;
            decimal igstr, igsta, cgstr, cgsta, sgstr, sgsta, cessr, cessa, taxvalue;
            int custid, userid;
            try
            {
                custid = Convert.ToInt32(Session["Cust_ID"]);
                userid = Convert.ToInt32(Session["User_Id"]);
                omonth = form["omonth"];
                ohsn = form["ohsn"];
                otype = form["otype"];
                ostatecode = form["odate"];
                prs = form["prs"];
                onum = form["onum"];
                cgstin = form["cgstin"];
                odate = form["odate"];
                hsn = form["hsn"];
                type = form["type"];
                taxvalue = Convert.ToDecimal(form["taxvalue"]);
                typ = form["typ"];
                etin = form["etin"];
                statecode = form["statecode"];
                igstr = Convert.ToDecimal(form["igstr"]);
                igsta = Convert.ToDecimal(form["igsta"]);
                cgstr = Convert.ToDecimal(form["cgstr"]);
                cgsta = Convert.ToDecimal(form["cgsta"]);
                sgstr = Convert.ToDecimal(form["sgstr"]);
                sgsta = Convert.ToDecimal(form["sgsta"]);
                cessr = Convert.ToDecimal(form["cessr"]);
                cessa = Convert.ToDecimal(form["cessa"]);

                //db.Ins_Outward_Gstr1_B2CSA(custid, cgstin, omonth, otype, ohsn, ostatecode, type, hsn, statecode, taxvalue, igstr, igsta, cgstr, cgsta, sgstr, sgsta, cessr, cessa, prs, onum, odate, etin, typ, userid);
                db.SaveChanges();

                TempData["Message"] = "Record inserted succesfully";

                var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == custid).Select(c =>
                new
                {
                    CategoryID = c.GSTINId,
                    CategoryName = c.GSTINNo
                }).ToList();

                ViewBag.Categories = categories;
                return View("Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}