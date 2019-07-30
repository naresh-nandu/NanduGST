using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_BAL.TermsandConditions;

namespace SmartAdminMvc.Controllers
{
    public class TermsAndConditionsController : Controller
    {
        [HttpGet]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int userid = Convert.ToInt32(Session["User_ID"]);
            int custid = Convert.ToInt32(Session["Cust_ID"]);

            try
            {
                ViewBag.result = TermandCondition.GetTermsAndConditions(custid, userid);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        [HttpPost]
        public ActionResult Index(FormCollection Form, FormCollection col)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string taskSummary = "";
            taskSummary = Form["taskSummary"];

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlCommand cmd = new SqlCommand();

                cmd.Connection = conn;
                cmd.CommandText = "Select * from TBL_TermsandConditions where custid = @custid and rowstatus = 1";
                cmd.Parameters.AddWithValue("@custid", custid);
                using (SqlDataAdapter adt = new SqlDataAdapter(cmd))
                {
                    DataTable dt = new DataTable();
                    adt.Fill(dt);
                    if (dt.Rows.Count >= 5)
                    {
                        TempData["Message"] = "Can not add more than 5, Exceeding Limit.";
                    }

                    else
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@TermsandCondition", SqlDbType.NVarChar, 500).Value = taskSummary;
                        cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1; // rowstatus
                        cmd.Parameters.Add("@custid", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"].ToString());
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                        Functions.InsertIntoTable("TBL_TermsandConditions", cmd, conn);
                        TempData["msg"] = "Terms and Conditions added Successfully";
                    }
                }
            }
            ViewBag.result = TermandCondition.GetTermsAndConditions(custid, userid);
            return View();

        }

        public ActionResult Delete(int Id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 0; // rowstatus
                        cmd.Parameters.Add("@Lastmodifiedby", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                        cmd.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                        Functions.UpdateTable("TBL_TermsandConditions", "termsid", Id.ToString(), cmd, conn);
                    }
                }
                TempData["msg"] = "Terms and Conditions deleted Successfully";
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit(int Id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int userid = Convert.ToInt32(Session["User_ID"]);
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            try
            {
                ViewBag.result = TermandCondition.GetTermsAndConditions(custid, userid);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Edit(FormCollection frm, int Id)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                string taskSummary = "";
                taskSummary = frm["taskSummary"];
                
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@TermsandCondition", SqlDbType.NVarChar, 500).Value = taskSummary;
                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1; // rowstatus
                cmd.Parameters.Add("@Lastmodifiedby", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                cmd.Parameters.Add("@LastModifiedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                Functions.UpdateTable("TBL_TermsandConditions", "termsid", Id.ToString(), cmd, conn);
            }
            TempData["msg"] = "Terms and Conditions updated Successfully";
            return RedirectToAction("Index");
        }


    }
}