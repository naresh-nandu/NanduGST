using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTR3;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL.GSTR3B;

namespace SmartAdminMvc.Controllers
{
    public class Gstr3BReportController : Controller
    {
        // GET: GSTR3BReport
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Report(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Report(FormCollection frm, string Report, string Export, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            string strGSTIN = "", strPeriod = "";
            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            try
            {
                strGSTIN = frm["gstin"];

                strPeriod = frm["period"];

                ViewBag.Period = strPeriod;

                if (!string.IsNullOrEmpty(Report))
                {
                    List<IDictionary> GridFill = null;
                    if (string.IsNullOrEmpty(strGSTIN))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.GSTINEmptyOrNull = "Please Select GSTIN Number";
                        return View(GridFill);
                    }
                    if (string.IsNullOrEmpty(strPeriod))
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.PeriodEmptyOrNull = "Please Select Period";

                        return View(GridFill);
                    }

                    ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, strGSTIN, Session["Role_Name"].ToString());
                    var ds = GetDatatableGSTR3BReport(strGSTIN, strPeriod);
                    GridFill = GetListGSTR3BReport(strGSTIN, strPeriod);
                    TempData["ExportData"] = ds;

                    TempData["gstn"] = strGSTIN;
                    ViewBag.gstn = strGSTIN;
                    TempData["Period"] = strPeriod;
                    //TempData["Action"] = strAction;

                    //ViewBag.TitleHeaders = "GSTR-3B " + strAction + " Download";
                    return View(GridFill);
                }
                else if (!string.IsNullOrEmpty(Export))
                {
                    GridView gv = new GridView();
                    gv.DataSource = TempData["ExportData"] as DataTable;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTR3BReport_Download_" + strGSTIN + "_" + strPeriod + ".xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                    ViewBag.CTINNoList = LoadDropDowns.CustomerGSTIN(iCustId);
                    return View();
                }

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, Convert.ToString(strGSTIN), Session["Role_Name"].ToString());
                return View();
            }
            catch (Exception ex)
            {
                return View();
            }
        }

        private object GetDatatableGSTR3BReport(string strGSTIN, string strPeriod)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    int Status = Gstr3BBusinessLayer.Check_GStr3B_D_Data_Exist_Or_Not(strGSTIN, strPeriod);
                    if (Status == 1)
                    {

                        SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Download_Report", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                        dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                        SqlDataAdapter da = new SqlDataAdapter(dCmd);
                        dt.Clear();
                        da.Fill(dt);
                    }
                    else
                    {

                        SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Report", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                        dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                        SqlDataAdapter da = new SqlDataAdapter(dCmd);
                        dt.Clear();
                        da.Fill(dt);
                    }


                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return dt;
        }

        private List<IDictionary> GetListGSTR3BReport(string strGSTIN, string strPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    int Status = Gstr3BBusinessLayer.Check_GStr3B_D_Data_Exist_Or_Not(strGSTIN, strPeriod);
                    if (Status == 1)
                    {
                        SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Download_Report", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                        dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                        SqlDataAdapter da = new SqlDataAdapter(dCmd);
                        ds.Clear();
                        da.Fill(ds);
                    }
                    else
                    {
                        SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Report", conn);
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                        dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                        SqlDataAdapter da = new SqlDataAdapter(dCmd);
                        ds.Clear();
                        da.Fill(ds);
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
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

    }
}