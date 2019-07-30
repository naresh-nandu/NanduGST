using ClosedXML.Excel;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Reconcilation;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class Reconcilation_GSTR1_EWBController : Controller
    {
        // GET: Reconcilation_GSTR1_EWB
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            ViewBag.fromPeriod = DateTime.Now.ToString("dd/MM/yyyy");
            ViewBag.toPeriod = DateTime.Now.ToString("dd/MM/yyyy");
            return View();
        }
        [HttpPost]
        public ActionResult Home(FormCollection frm,string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            try
            {
                string gstin = frm["strGSTIN"];
                string fromperiod = frm["fromperiod"];
                ViewBag.fromPeriod = fromperiod;
                string toperiod = frm["toperiod"];
                ViewBag.toPeriod = toperiod;
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, gstin, Session["Role_Name"].ToString());
                if (!String.IsNullOrEmpty(command))
                {
                    if (command == "btngetMisMatch")
                    {                       
                        var das = Models.Reconcilation.GSRT1_EWB_Reconcilation.Retrieve_Recon(gstin, fromperiod, toperiod);                     
                        using (DataSet ds = das)
                        {
                             
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].TableName = "Missinge-WaybillsinGSTR1ExcB2CS";
                            }
                            if (ds.Tables[1].Rows.Count > 0)
                            {
                                ds.Tables[1].TableName = "Missinge-WaybillsinB2CS";
                            }
                           
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                foreach (DataTable dt in ds.Tables)
                                {                                     
                                    if (dt.Rows.Count > 0)
                                    {
                                        wb.Worksheets.Add(dt);
                                    }
                                }
                                //Export the Excel file.
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment; filename=Reconcilation_For_" + gstin + ".xls");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {
                                    if (wb.Worksheets.Count > 0)
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                    }

                                    Response.Flush();
                                    Response.End();
                                }
                            }
                        }
                    }
                }
                else
                {                    
                    var ReconDataALL = GSRT1_EWB_Reconcilation.Retrieve_Data_Recon(gstin, fromperiod, toperiod);
                    ViewBag.ReconDataAll = ReconDataALL;
                    var ReconDataB2cs = GSRT1_EWB_Reconcilation.Retrieve_Data_Recon1(gstin, fromperiod, toperiod);
                    ViewBag.ReconDataB2cs = ReconDataB2cs;
                }
                }        
            catch(Exception ex)
            {
                TempData["error"] = ex.Message;
            }
            return View();
        }
    }
}