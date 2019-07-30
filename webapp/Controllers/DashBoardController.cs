using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Controllers
{
    public class DashBoardController : Controller
    {
        [System.Web.Mvc.HttpGet]
        public ActionResult Index(string gstin = "", string period = "")
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);

            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

            if (!string.IsNullOrEmpty(gstin.ToUpper()) && !string.IsNullOrEmpty(period))
            {
                ViewBag.Period = period;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, gstin.ToUpper(), Session["Role_Name"].ToString());

                GetSummaryDetails(CustId, UserId, ViewBag.Period, gstin.ToUpper());
            }
            else
            {
                string gstinno = Session["Cust_GSTIN"].ToString();

                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, gstinno, Session["Role_Name"].ToString());

                GetSummaryDetails(CustId, UserId, ViewBag.Period, gstinno);
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"]);
            int CustId = Convert.ToInt32(Session["Cust_ID"]);

            string gstin = frm["ddlGSTINNo"];

            string period = frm["period"];
            ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
            if (!String.IsNullOrEmpty(gstin) && !String.IsNullOrEmpty(period))
            {
                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());

                ViewBag.Period = period;
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(iUserId, iCustId, gstin, Session["Role_Name"].ToString());

                GetSummaryDetails(CustId, UserId, ViewBag.Period, gstin);
            }
            else
            {
                ViewBag.Period = DateTime.Now.ToString("MMyyyy");
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
            }
            return View();
        }


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

                TempData["MakerCheckerMessage"] = TempData["MakerCheckerMsg"];

                if (sort != null || page != 0)
                {
                    string Period = TempData["period"] as string;
                    if (Period != null)
                    {
                        ViewBag.Period = Period;

                        var summary = GetSummary(CustId, UserId, Period);
                        GetSummaryPerc(CustId, UserId, Period);
                        ViewBag.Summary = summary;
                        var ItcLedger = GetITCLedger(CustId, UserId, Period);
                        ViewBag.ITCLedger = ItcLedger;
                    }
                    else
                    {
                        Period = DateTime.Now.ToString("MMyyyy");
                        ViewBag.Period = Period;
                        var summary = GetSummary(CustId, UserId, Period);
                        GetSummaryPerc(CustId, UserId, Period);
                        ViewBag.Summary = summary;
                        var ItcLedger = GetITCLedger(CustId, UserId, Period);
                        ViewBag.ITCLedger = ItcLedger;
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
                    var ItcLedger = GetITCLedger(CustId, UserId, Period);
                    ViewBag.ITCLedger = ItcLedger;
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
                var ItcLedger = GetITCLedger(CustId, UserId, Period);
                ViewBag.ITCLedger = ItcLedger;

                return View();
            }
            catch (Exception ex)
            {
                TempData["msg"] = ex.Message;
            }
            return View();
        }

        public ActionResult Table()
        {
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Summary_Dashboard_Flat", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustID));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strPeriod));
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

        public List<IDictionary> GetITCLedger(int CustID, int UserID, string strPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_DashBoard_Summary_ITC_LEDGER", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustID));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                    dCmd.Parameters.Add(new SqlParameter("@FP", strPeriod));
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
                    ViewBag.GSTR3BPercent= table.Rows[0]["GSTR3BFiled_Percent"].ToString();
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

        public void GetSummaryDetails(int CustID, int UserID, string strPeriod, string strGSTINNo)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Summary_Dashboard_Details", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustID));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserID));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", strGSTINNo));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(table);
                    ViewBag.OIGST = table.Rows[0]["OIGST"].ToString();
                    ViewBag.OCGST = table.Rows[0]["OCGST"].ToString();
                    ViewBag.OSGST = table.Rows[0]["OSGST"].ToString();
                    ViewBag.OCESS = table.Rows[0]["OCESS"].ToString();
                    ViewBag.IIGST = table.Rows[0]["IIGST"].ToString();
                    ViewBag.ICGST = table.Rows[0]["ICGST"].ToString();
                    ViewBag.ISGST = table.Rows[0]["ISGST"].ToString();
                    ViewBag.ICESS = table.Rows[0]["ICESS"].ToString();
                    ViewBag.LIGST = table.Rows[0]["LIGST"].ToString();
                    ViewBag.LCGST = table.Rows[0]["LCGST"].ToString();
                    ViewBag.LSGST = table.Rows[0]["LSGST"].ToString();
                    ViewBag.LCESS = table.Rows[0]["LCESS"].ToString();
                    ViewBag.GSTR1_U = table.Rows[0]["GSTR1_U"].ToString();
                    ViewBag.GSTR2_U = table.Rows[0]["GSTR2_U"].ToString();
                    ViewBag.GSTR3_U = table.Rows[0]["GSTR3_U"].ToString();
                    ViewBag.GSTR1_F = table.Rows[0]["GSTR1_F"].ToString();
                    ViewBag.GSTR2_F = table.Rows[0]["GSTR2_F"].ToString();
                    ViewBag.GSTR3_F = table.Rows[0]["GSTR3_F"].ToString();
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


        protected void OutputTaxLiability()
        {
            ViewBag.OutputTaxLiability = "Output Tax Liability";
            ViewBag.OutIGST = "0";
            ViewBag.OutCGST = "0";
            ViewBag.OutSGST = "0";
            ViewBag.OutCESS = "0";
        }

        protected void ITC()
        {
            ViewBag.ITC = "ITC";
            ViewBag.ITCIGST = "0";
            ViewBag.ITCCGST = "0";
            ViewBag.ITCSGST = "0";
            ViewBag.ITCCESS = "0";
        }

        protected void Cash()
        {
            ViewBag.LedgerCash = "Ledger Cash";
            ViewBag.CashIGST = "0";
            ViewBag.CashCGST = "0";
            ViewBag.CashSGST = "0";
            ViewBag.CashCESS = "0";
        }
    }
}