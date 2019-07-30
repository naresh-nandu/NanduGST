using GSTR1Save;
using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR1;
using SmartAdminMvc.Models.GSTR1BulkData;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartAdminMvc.Controllers
{
    public class Gstr1BulkDataController : Controller
    {
        [HttpGet]
        // GET: GSTRBulkData
        public ActionResult Reconcilation()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

            return View();
        }

        [HttpPost]
        public ActionResult Reconcilation(FormCollection frm, string GSTR1Save, string Download, string ErrorDownload)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            try
            {
                string strGSTINNo = "", strPeriod = "";
                strGSTINNo = frm["ddlGSTINNo"];
                strPeriod = frm["period"];
                ViewBag.Period = strPeriod;
                if (!string.IsNullOrEmpty(GSTR1Save))
                {
                    //
                }

                #region "ONCHANGE EVENT"
                else
                {
                    ViewBag.InputData = GSTR1BulkDataModel.GetInputData(strGSTINNo, strPeriod);

                    ViewBag.OutputData = GSTR1BulkDataModel.GetOutputData(strGSTINNo, strPeriod);
                }
                #endregion
                ViewBag.STR_GSTINNO = strGSTINNo;
                ViewBag.STR_PERIOD = strPeriod;

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                TempData["Response"] = ex.Message;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                return View();
            }
        }

        [HttpGet]
        // GET: GSTRBulkData
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection frm, string GSTR1Save, string Download, string ErrorDownload)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int totcount = 0, totprocessed = 0, toterror = 0;
            try
            {
                string strGSTINNo = "", strPeriod = "";
                strGSTINNo = frm["ddlGSTINNo"];
                strPeriod = frm["period"];
                ViewBag.Period = strPeriod;
                #region "GSTR1 SAVE BATCH PROCESSING"
                if (!string.IsNullOrEmpty(GSTR1Save))
                {
                    //
                }
                #endregion

                #region "Process Data Download"
                else if (!string.IsNullOrEmpty(Download))
                {
                    GridView gv = new GridView();
                    gv.DataSource = GSTR1BulkDataModel.GetGSTR1_Data(strGSTINNo, strPeriod, Download);
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=ProcessData_" + Download + "_" + strGSTINNo + "_" + strPeriod + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                    return View();
                }
                #endregion

                #region "Error Data Download"
                else if (!string.IsNullOrEmpty(ErrorDownload))
                {
                    GridView gv = new GridView();
                    gv.DataSource = GSTR1BulkDataModel.GetGSTR1_Error_Data(strGSTINNo, strPeriod);
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=ErrorData_" + ErrorDownload + "_" + strGSTINNo + "_" + strPeriod + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                    return View();
                }
                #endregion

                #region "ONCHANGE EVENT"
                else
                {
                    ViewBag.BulkDataProcessed = GSTR1BulkDataModel.GetBulkDataProcessed(strGSTINNo, strPeriod);
                    GSTR1BulkDataModel.GetGSTR1_Error_Data_Count(strGSTINNo, strPeriod, out totcount, out totprocessed, out toterror);
                    ViewBag.TotalCount = totcount;
                    ViewBag.TotalProcessed = totprocessed;
                    ViewBag.TotalError = toterror;
                }
                #endregion

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                TempData["Response"] = ex.Message;
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                return View();
            }
        }
                        
    }
}