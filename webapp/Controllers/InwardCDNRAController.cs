using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class InwardCDNRAController : Controller
    {
        // GET: CDNRA
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
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
            new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();

            ViewBag.Categories = categories;
            return View();
            }
            catch(Exception ex)
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

            string gstin, name, cgstin, rns, revinvoice, reviseddate, reversecharge, notetype, notenum, notedate, elig;
            decimal igstr, igsta, cgstr, cgsta, sgstr, sgsta, cessr, cessa, taxvalue;
            int custid, userid;
            try
            {
                custid = Convert.ToInt32(Session["Cust_ID"]);
                userid = Convert.ToInt32(Session["User_Id"]);
                gstin = form["gstin"];
                name = form["name"];
                cgstin = form["cgstin"];
                rns = form["rns"];
                revinvoice = form["revinvoice"];
                reviseddate = form["reviseddate"];
                taxvalue = Convert.ToDecimal(form["taxvalue"]);
                reversecharge = form["reversecharge"];
                notetype = form["notetype"];
                notenum = form["notenum"];
                notedate = form["notedate"];
                elig = form["elig"];
                igstr = Convert.ToDecimal(form["igstr"]);
                igsta = Convert.ToDecimal(form["igsta"]);
                cgstr = Convert.ToDecimal(form["cgstr"]);
                cgsta = Convert.ToDecimal(form["cgsta"]);
                sgstr = Convert.ToDecimal(form["sgstr"]);
                sgsta = Convert.ToDecimal(form["sgsta"]);
                cessr = Convert.ToDecimal(form["cessr"]);
                cessa = Convert.ToDecimal(form["cessa"]);

                //db.Ins_Inward_Gstr2_CDNRA(name, gstin, custid, cgstin, notetype, notenum, notedate, rns, revinvoice, reviseddate, reversecharge, taxvalue, igstr, igsta, cgstr, cgsta, sgstr, sgsta, cessr, cessa, elig, userid);
                db.SaveChanges();

                TempData["Message"] = "Record inserted succesfully";

                var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == custid).Select(c =>
                new {
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