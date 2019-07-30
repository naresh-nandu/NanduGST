using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class ReconcilationSettingController : Controller
    {
        readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: ReconcilationSetting
        public ActionResult Index()
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
                int userid = Convert.ToInt32(Session["User_ID"]);
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "One",
                    Value = "1"
                });
                items.Add(new SelectListItem
                {
                    Text = "Two",
                    Value = "2"
                });
                items.Add(new SelectListItem
                {
                    Text = "Five",
                    Value = "5"
                });
                items.Add(new SelectListItem
                {
                    Text = "Ten",
                    Value = "10"
                });
              
                ViewBag.Rates = new SelectList(items, "Text", "Value");

                ViewBag.GSTINList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Result= Reconciliation_Setting(custid);

            }
            catch(Exception ex)
            {
                TempData["MSG"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection Col)
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
                int userid = Convert.ToInt32(Session["User_ID"]);
                string GSTIN = Col["GSTIN"];
                string Rates = Col["Rates"];

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Settings_Reconciliation where Gstinno = @GSTINNo";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", GSTIN);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                //Update
                                SqlCommand cmd = new SqlCommand();
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@reconValueAdjust", SqlDbType.Int).Value = Rates;
                                cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                                cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                Functions.UpdateTable("TBL_Settings_Reconciliation", "Gstinno", GSTIN, cmd, conn);
                                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Reconcilation Adjusted Amount Added Successfully For: " + GSTIN, "");
                                TempData["MSG"] = "Reconcilation Adjusted Amount Added Successfully For this GSTIN: " + GSTIN;
                            }
                            else
                            {
                                //insert
                                SqlCommand cmd = new SqlCommand();
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = custid;
                                cmd.Parameters.Add("@Gstinno", SqlDbType.VarChar).Value = GSTIN;
                                cmd.Parameters.Add("@reconValueAdjust", SqlDbType.Int).Value = Rates;
                                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1;
                                cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = userid;
                                Functions.InsertIntoTable("TBL_Settings_Reconciliation", cmd, conn);
                                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Reconcilation Adjusted Amount Updated Successfully For: " + GSTIN, "");
                                TempData["MSG"] = "Reconcilation Adjusted Amount Updated Successfully For this GSTIN: " + GSTIN;
                            }
                        }
                    }
                }

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "One",
                    Value = "1"
                });
                items.Add(new SelectListItem
                {
                    Text = "Two",
                    Value = "2"
                });
                items.Add(new SelectListItem
                {
                    Text = "Five",
                    Value = "5"
                });
                items.Add(new SelectListItem
                {
                    Text = "Ten",
                    Value = "10"
                });

                ViewBag.Rates = new SelectList(items, "Text", "Value");

                ViewBag.GSTINList = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Result = Reconciliation_Setting(custid);

            }
            catch (Exception ex)
            {

                TempData["MSG"] = ex.Message;
            }
            return View();
        }

        public static List<IDictionary> Reconciliation_Setting(int CustID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Reconciliation_Setting", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@custid", CustID)); 
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    #endregion
                }
                catch(Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
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