using SmartAdminMvc.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace SmartAdminMvc.Controllers
{
    public class HsnRateFinderController : Controller
    {
        WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: HSNRateFinder
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            return View();
        }
        public JsonResult AutoPopulate(string Text)
        {

            decimal rate;
            JavaScriptSerializer javaScriptSerializer = new JavaScriptSerializer();
            string result;
            if (Decimal.TryParse(Text,out rate))
            {
                var hsndesc = (from ob in db.TBL_HSN_MASTER
                               where (ob.hsnCode.Contains(Text) || ob.hsnDescription.Contains(Text) || ob.rate == rate) && ob.CustomerId==1 && ob.hsnType==null
                               select new
                               {
                                   hsncode = ob.hsnCode,
                                   hsndesc = ob.hsnDescription,
                                   rate = ob.rate
                               }
                       ).ToList();

                result = javaScriptSerializer.Serialize(hsndesc);
            }
            else
            {
                var hsndesc = (from ob in db.TBL_HSN_MASTER
                               where (ob.hsnCode.Contains(Text) || ob.hsnDescription.Contains(Text) ) && ob.CustomerId == 1 && ob.hsnType == null
                               select new
                               {
                                   hsncode = ob.hsnCode,
                                   hsndesc = ob.hsnDescription,
                                   rate = ob.rate
                               }
                 ).ToList();

                result = javaScriptSerializer.Serialize(hsndesc);
            }
            
            

            
            
            return Json(result, JsonRequestBehavior.AllowGet);

          
        }

        
    }
}