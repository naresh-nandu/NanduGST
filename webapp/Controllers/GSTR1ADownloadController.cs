using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Xml;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class Gstr1ADownloadController : Controller
    {
        // GET: GSTR1ADownload
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            try
            {
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                ViewBag.DownloadResponse = "";

                if (sort != null || page != 0)
                {
                    string Action = TempData["Action"] as string;
                    string GSTIN = TempData["gstn"] as string;
                    TempData.Keep();
                }
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm, string Download, string Export)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            try
            {
                string GSTIN, Action, period, fromdate, ctin;
                GSTIN = frm["gstin"];
                Action = frm["action"];
                period = frm["period"];
                fromdate = frm["fromdate"];
                ctin = frm["ctin"];

                if (!string.IsNullOrEmpty(Download))
                {
                    ViewBag.DownloadResponse = "GSTR1A Downloaded Successfully";
                    var GridFill = GetListGSTR1ADownload(GSTIN, Action);
                    var ds = GetDatatableGSTR1ADownload(GSTIN, Action);
                    TempData["ExportData"] = ds;

                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                    TempData["gstn"] = GSTIN;
                    TempData["Action"] = Action;
                    return View(GridFill);
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR1A_Download.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                    return View();
                }
                return View();
            }
            catch (Exception ex)
            {
                ViewBag.DownloadResponse = ex.Message;
                return View();
            }
        }


        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }

        public static List<IDictionary> GetListGSTR1ADownload(string GSTINNo, string ActionType)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Retrieve_Download_GSTR1A_ByAction", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public static DataTable GetDatatableGSTR1ADownload(string GSTINNo, string ActionType)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Retrieve_Download_GSTR1A_ByAction", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return dt;
        }
    }
}