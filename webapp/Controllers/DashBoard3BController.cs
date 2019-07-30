using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using System;

namespace SmartAdminMvc.Controllers
{
    public class DashBoard3BController : Controller
    {
        // GET: DashBoard
        public ActionResult Summary(int page = 0, string sort = null)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                if (sort != null || page != 0)
                {
                    string Period = TempData["period"] as string;
                    if (Period != null)
                    {
                        ViewBag.Period = Period;

                        var summary = GetSummary(CustId, UserId, Period);
                        GetSummaryPerc(CustId, UserId, Period);
                        ViewBag.Summary = summary;
                    }
                    else
                    {
                        Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.Period = Period;
                        var summary = GetSummary(CustId, UserId, Period);
                        GetSummaryPerc(CustId, UserId, Period);
                        ViewBag.Summary = summary;
                    }
                    return View();
                }
                else
                {
                    string Period = DateTime.Now.ToString("MMyyyy");
                    ViewBag.Period = Period;
                    var summary = GetSummary(CustId, UserId, Period);
                    GetSummaryPerc(CustId, UserId, Period);
                    ViewBag.Summary = summary;

                    return View();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Summary(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                string Period = frm["period"];
                ViewBag.Period = Period;
                TempData["period"] = Period;

                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                var summary = GetSummary(CustId, UserId, Period);
                GetSummaryPerc(CustId, UserId, Period);
                ViewBag.Summary = summary;

                return View();
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return View();
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




        public List<IDictionary> GetSummary(int CustID, int UserID, string strPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Dashboard", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustID));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
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

        public void GetSummaryPerc(int CustID, int UserID, string strPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Summary_Dashboard_Pie", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustID));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strPeriod));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(table);
                    ViewBag.TotalGSTIN = table.Rows[0]["TotalGSTINs"].ToString();
                    ViewBag.TotalTXI = table.Rows[0]["TotalTXI"].ToString();
                    ViewBag.TotalITC = table.Rows[0]["TotalITC"].ToString();
                    ViewBag.TotalLedger = table.Rows[0]["TotalLedger"].ToString();
                    ViewBag.GSTR1Percent = table.Rows[0]["GSTR1Filed_Percent"].ToString();
                    ViewBag.GSTR2Percent = table.Rows[0]["GSTR2Filed_Percent"].ToString();
                    ViewBag.GSTR3Percent = table.Rows[0]["GSTR3Filed_Percent"].ToString();
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
    }
}