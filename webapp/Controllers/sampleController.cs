using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class sampleController : Controller
    {
        // GET: sample
        [HttpGet]
        public ActionResult Index()
        {
            ViewBag.SupplyType = LoadGSTINNo.GetEWayBillType("SupplyType");

            //ViewBag.SupplyTypeOld = LoadGSTINNo.GetEWayBillTypeO("SupplyType");

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection frm)
        {

            string strSupplyType = "";
            strSupplyType = frm["ddlSupplyType"];

            Session["transactiontype"] = frm["SupplyType"].ToString();

            ViewBag.SupplyType = LoadGSTINNo.GetEWayBillType("SupplyType");

            ViewBag.SupplyTypeOld = LoadGSTINNo.Exist_GetEWayBillType("SupplyType", strSupplyType);
            return View();
        }
    }
}