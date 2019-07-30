#region Using

using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Net;
using System.Configuration;
using System.Text;
using System.Web;
using SmartAdminMvc.Models.Common;

#endregion

namespace SmartAdminMvc.Controllers
{

    [Authorize]

    [OutputCache(NoStore = true, Duration = 0)]

    public class GstinConfigurationController : Controller
    {
        readonly WePGSPDBEntities db = new WePGSPDBEntities();

        [HttpGet]
        public ActionResult GSTIN(string option, string search)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                string op = option;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "GSTIN",
                    Value = "GSTIN"
                });
                items.Add(new SelectListItem
                {
                    Text = "State Code",
                    Value = "State Code"
                });

                ViewBag.Categories = new SelectList(items, "Text", "Value", op);

                int id = Convert.ToInt32(Session["Cust_ID"]);
                if (option == "GSTIN")
                {
                    List<TBL_Cust_GSTIN> cust = (from li in db.TBL_Cust_GSTIN
                                                 where li.CustId == id && li.rowstatus == true && (li.GSTINNo.Contains(search) || li.GSTINNo.StartsWith(search) || li.GSTINNo == search || search == null)
                                                 select li).ToList();

                    return View(cust);
                }
                else if (option == "State Code")
                {
                    List<TBL_Cust_GSTIN> cust = (from li in db.TBL_Cust_GSTIN
                                                 where li.CustId == id && li.rowstatus == true && (li.Statecode.Contains(search) || li.Statecode.StartsWith(search) || li.Statecode == search || search == null)
                                                 select li).ToList();
                    return View(cust);
                }

                List<TBL_Cust_GSTIN> custgstin = (from gs in db.TBL_Cust_GSTIN
                                                  where gs.CustId == id && gs.rowstatus == true
                                                  select gs).ToList();
                return View(custgstin);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        [HttpPost]
        [ValidateInput(false)]
        //[ValidateAntiForgeryToken]

        public ActionResult GSTIN(FormCollection col)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                TBL_Cust_GSTIN li = new TBL_Cust_GSTIN();
                string gst = col["gst"];
                var gstnumber = (from list in db.TBL_Cust_GSTIN
                                 where list.GSTINNo == gst && list.rowstatus == true
                                 select list);

                if (gstnumber.Count() > 0)
                {
                    TempData["msg"] = "GSTIN already exists";
                }
                else
                {
                    int j = Convert.ToInt32(Session["Cust_ID"]);
                    string PANNo = col["gst"].ToString().Substring(2, 10).ToUpper();
                    var pan = (from ul in db.TBL_Cust_PAN
                               where ul.CustId == j && ul.PANNo == PANNo
                               select ul.PANNo).ToList();

                    if (pan.Count > 0)
                    {
                        string strStateCode = col["gst"].ToString().Substring(0, 2);
                        string strPANNo = col["gst"].ToString().Substring(2, 10);

                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@GSTINUserName", SqlDbType.NVarChar).Value = col["gstusername"].ToString(); // GSTIN Username
                            cmd.Parameters.Add("@GSTINNo", SqlDbType.NVarChar).Value = col["gst"].ToUpper();
                            cmd.Parameters.Add("@PANNo", SqlDbType.NVarChar).Value = strPANNo.ToUpper();
                            cmd.Parameters.Add("@Statecode", SqlDbType.NVarChar).Value = strStateCode;
                            cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = col["address"]; // Address
                            cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1; // rowstatus
                            cmd.Parameters.Add("@custid", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"].ToString());
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.Parameters.Add("@EWBUserId", SqlDbType.NVarChar).Value = col["ewbuserid"]; // GSTIN Username
                            cmd.Parameters.Add("@EWBUserName", SqlDbType.NVarChar).Value = col["ewbusername"];
                            cmd.Parameters.Add("@EWBPassword", SqlDbType.NVarChar).Value = col["ewbpassword"]; // Address
                            string strGSTINId = Functions.InsertIntoTable("TBL_Cust_GSTIN", cmd, conn);


                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@GSTINId", SqlDbType.Int).Value = strGSTINId; // GSTIN Username                        
                            cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1; // rowstatus
                            cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"].ToString());
                            cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"].ToString());
                            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            Functions.InsertIntoTable("UserAccess_GSTIN", cmd, conn);

                            // AUTH KEYS CREATION STARTS

                            int GstinId = (from ul in db.TBL_Cust_GSTIN
                                           where ul.GSTINNo == li.GSTINNo && ul.rowstatus == true
                                           select ul.GSTINId).SingleOrDefault();


                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = col["gstusername"].ToString(); // GSTIN Username
                            cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedOTP", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@Expiry", SqlDbType.Int).Value = 0;
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = GstinId; // GSTIN Id
                            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                            cmd.Parameters.Add("@AuthorizationToken", SqlDbType.NVarChar).Value = col["gst"].ToUpper(); // GSTIN No
                            cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"]);
                            cmd.Parameters.Add("@custid", SqlDbType.Int).Value = Session["Cust_ID"].ToString();
                            Functions.InsertIntoTable("TBL_AUTH_KEYS", cmd, conn);

                            // AUTH KEYS CREATION ENDS

                            // AUTH KEYS EWAYBILL CREATION STARTS
                            SqlCommand ewbcmd = new SqlCommand();
                            ewbcmd.Parameters.Clear();
                            ewbcmd.Parameters.Add("@EWBUserId", SqlDbType.NVarChar).Value = col["ewbuserid"].ToString(); // GSTIN No
                            ewbcmd.Parameters.Add("@GSTIN", SqlDbType.NVarChar).Value = col["gst"].ToString(); // GSTIN No
                            ewbcmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = col["ewbusername"].ToString(); // GSTIN Username
                            ewbcmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = col["ewbpassword"].ToString(); // GSTIN No
                            ewbcmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = "";
                            ewbcmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = "";
                            ewbcmd.Parameters.Add("@EncryptedPassword", SqlDbType.NVarChar).Value = "";
                            ewbcmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = "";
                            ewbcmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = "";
                            ewbcmd.Parameters.Add("@GSTINId", SqlDbType.Int).Value = GstinId; // GSTIN Id                        
                            ewbcmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"]);
                            ewbcmd.Parameters.Add("@custid", SqlDbType.Int).Value = Session["Cust_ID"].ToString();
                            ewbcmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                            Functions.InsertIntoTable("TBL_AUTH_KEYS_EWB", ewbcmd, conn);
                            // AUTH KEYS EWAYBILL CREATION ENDS
                        }
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTIN  Added : " + li.GSTINNo, "");

                        int userid = Convert.ToInt32(Session["User_ID"]);
                        var Mobileno = (from ul in db.UserLists where ul.UserId == userid select ul.MobileNo).FirstOrDefault();
                        var Email = (from ul in db.UserLists where ul.UserId == userid select ul.Email).FirstOrDefault();
                        Notification.SendSMS(Mobileno.ToString(), "Your New GSTIN " + gst.ToUpper() + " is registered Successfully.");
                        Notification.SendEmail(Email, "Regarding to New GSTIN Registered.", "Your New GSTIN " + gst.ToUpper() + " is registered Successfully.");
                        
                        TempData["msg"] = "GSTIN Registered Successfully";
                    }
                    else
                    {
                        TempData["msg"] = "The PAN in the GSTIN does not match with the PAN of the registered customer";
                    }
                }
                return RedirectToAction("gstin");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where GSTINId = @GSTINId and rowstatus = 1";
                        sqlcmd.Parameters.AddWithValue("@GSTINId", id);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                ViewBag.GSTINID = id;
                                ViewBag.GSTINUSERNAME = dt.Rows[0]["GSTINUserName"].ToString();
                                ViewBag.GSTINNO = dt.Rows[0]["GSTINNo"].ToString();
                                ViewBag.PANNO = dt.Rows[0]["PANNo"].ToString();
                                ViewBag.STATECODE = dt.Rows[0]["Statecode"].ToString();
                                ViewBag.ADDRESS = dt.Rows[0]["Address"].ToString();
                                ViewBag.EWBUSERID = dt.Rows[0]["EWBUserId"].ToString();
                                ViewBag.EWBUSERNAME = dt.Rows[0]["EWBUserName"].ToString();
                                ViewBag.EWBPASSWORD = dt.Rows[0]["EWBPassword"].ToString();
                            }
                        }
                    }
                }

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        [HttpPost]
        //[ValidateAntiForgeryToken]
        public ActionResult Edit(FormCollection frm)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                SqlCommand cmd = new SqlCommand();
                string gst = frm["gst"];
                string gstinid = frm["GSTINId"];
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@GSTINUserName", SqlDbType.NVarChar).Value = frm["gstusername"]; // GSTIN Username
                    cmd.Parameters.Add("@GSTINNo", SqlDbType.NVarChar).Value = gst.ToUpper();
                    cmd.Parameters.Add("@Address", SqlDbType.NVarChar).Value = frm["address"]; // Address
                    cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1; // rowstatus
                    cmd.Parameters.Add("@custid", SqlDbType.Int).Value = CustId;
                    cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = UserId;
                    cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    cmd.Parameters.Add("@EWBUserId", SqlDbType.NVarChar).Value = frm["ewbuserid"]; // GSTIN Username
                    cmd.Parameters.Add("@EWBUserName", SqlDbType.NVarChar).Value = frm["ewbusername"];
                    cmd.Parameters.Add("@EWBPassword", SqlDbType.NVarChar).Value = frm["ewbpassword"]; // Address
                    Functions.UpdateTable("TBL_Cust_GSTIN", "GSTINId", gstinid, cmd, conn);
                }
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTIN updated : " + gst, "");

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        if (conn.State == ConnectionState.Closed)
                        {
                            conn.Open();
                        }
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from UserAccess_GSTIN where UserId = @UserId and CustId = @CustId and GSTINId = @GSTINId and Rowstatus = 1";
                        sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        sqlcmd.Parameters.AddWithValue("@GSTINId", gstinid);
                        using (SqlDataAdapter Useradt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable Userdt = new DataTable();
                            Useradt.Fill(Userdt);
                            if (Userdt.Rows.Count > 0)
                            {
                                //
                            }
                            else
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@GSTINId", SqlDbType.Int).Value = gstinid;
                                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = CustId;
                                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = UserId;
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = UserId;
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                cmd.Parameters.Add("@Rowstatus", SqlDbType.Bit).Value = true;
                                Functions.InsertIntoTable("UserAccess_GSTIN", cmd, conn);
                            }
                        }
                    }
                }

                int GstinId = (from ul in db.TBL_Cust_GSTIN
                               where ul.GSTINNo == gst && ul.rowstatus == true
                               select ul.GSTINId).SingleOrDefault();

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", gst);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                
                                // AUTH KEYS CREATION STARTS
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = frm["gstusername"]; // GSTIN Username                                
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = GstinId; // GSTIN Id
                                cmd.Parameters.Add("@AuthorizationToken", SqlDbType.NVarChar).Value = frm["gst"]; // GSTIN No
                                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = UserId;
                                cmd.Parameters.Add("@custid", SqlDbType.Int).Value = CustId;
                                Functions.UpdateTable("TBL_AUTH_KEYS", "AuthorizationToken", frm["gst"].ToString(), cmd, conn);
                                // AUTH KEYS CREATION ENDS                    
                            }
                            else
                            {
                                // AUTH KEYS CREATION STARTS
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = frm["gstusername"]; // GSTIN Username
                                cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = "";
                                cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = "";
                                cmd.Parameters.Add("@EncryptedOTP", SqlDbType.NVarChar).Value = "";
                                cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = "";
                                cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = "";
                                cmd.Parameters.Add("@Expiry", SqlDbType.Int).Value = 0;
                                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = GstinId; // GSTIN Id
                                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                                cmd.Parameters.Add("@AuthorizationToken", SqlDbType.NVarChar).Value = frm["gst"]; // GSTIN No
                                cmd.Parameters.Add("@userid", SqlDbType.Int).Value = UserId;
                                cmd.Parameters.Add("@custid", SqlDbType.Int).Value = CustId;
                                Functions.InsertIntoTable("TBL_AUTH_KEYS", cmd, conn);
                                // AUTH KEYS CREATION ENDS
                            }
                        }
                    }
                }

                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS_EWB where GSTIN = @GSTINNo";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", gst);
                        using (SqlDataAdapter ewbadt = new SqlDataAdapter(sqlcmd))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            DataTable ewbdt = new DataTable();
                            ewbadt.Fill(ewbdt);
                            if (ewbdt.Rows.Count > 0)
                            {
                                
                                // AUTH KEYS EWAYBILL CREATION STARTS
                                SqlCommand ewbcmd = new SqlCommand();
                                ewbcmd.Parameters.Clear();
                                ewbcmd.Parameters.Add("@EWBUserId", SqlDbType.NVarChar).Value = frm["ewbuserid"].ToString(); // GSTIN No
                                ewbcmd.Parameters.Add("@GSTIN", SqlDbType.NVarChar).Value = frm["gst"].ToString(); // GSTIN No
                                ewbcmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = frm["ewbusername"].ToString(); // GSTIN Username
                                ewbcmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = frm["ewbpassword"].ToString(); // GSTIN No                    
                                ewbcmd.Parameters.Add("@GSTINId", SqlDbType.Int).Value = GstinId; // GSTIN Id                        
                                ewbcmd.Parameters.Add("@userid", SqlDbType.Int).Value = UserId;
                                ewbcmd.Parameters.Add("@custid", SqlDbType.Int).Value = CustId;
                                Functions.UpdateTable("TBL_AUTH_KEYS_EWB", "GSTIN", gst, ewbcmd, conn);
                                // AUTH KEYS EWAYBILL CREATION ENDS
                            }
                            else
                            {
                                // AUTH KEYS EWAYBILL CREATION STARTS
                                SqlCommand ewbcmd = new SqlCommand();
                                ewbcmd.Parameters.Clear();
                                ewbcmd.Parameters.Add("@EWBUserId", SqlDbType.NVarChar).Value = frm["ewbuserid"].ToString(); // GSTIN No
                                ewbcmd.Parameters.Add("@GSTIN", SqlDbType.NVarChar).Value = frm["gst"].ToString(); // GSTIN No
                                ewbcmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = frm["ewbusername"].ToString(); // GSTIN Username
                                ewbcmd.Parameters.Add("@Password", SqlDbType.NVarChar).Value = frm["ewbpassword"].ToString(); // GSTIN No
                                ewbcmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = "";
                                ewbcmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = "";
                                ewbcmd.Parameters.Add("@EncryptedPassword", SqlDbType.NVarChar).Value = "";
                                ewbcmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = "";
                                ewbcmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = "";
                                ewbcmd.Parameters.Add("@GSTINId", SqlDbType.Int).Value = GstinId; // GSTIN Id                        
                                ewbcmd.Parameters.Add("@userid", SqlDbType.Int).Value = UserId;
                                ewbcmd.Parameters.Add("@custid", SqlDbType.Int).Value = CustId;
                                ewbcmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                                Functions.InsertIntoTable("TBL_AUTH_KEYS_EWB", ewbcmd, conn);
                                // AUTH KEYS EWAYBILL CREATION ENDS
                            }
                        }
                    }
                }
                TempData["msg"] = "GSTIN updated Successfully";

                return RedirectToAction("gstin");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
                int cid = Convert.ToInt32(Session["Cust_ID"]);

                var gst = (from li in db.TBL_Customer
                           where li.CustId == cid
                           select li.GSTINNo).FirstOrDefault();
                var deletegst = (from li in db.TBL_Cust_GSTIN
                                 where li.GSTINId == Id
                                 select li.GSTINNo).FirstOrDefault();
                if (gst == deletegst)
                {
                    TempData["msg"] = "Your Master GSTIN cannot be deleted";
                }
                else
                {
                    var user = db.TBL_Cust_GSTIN.FirstOrDefault(u => u.GSTINId == Id);
                    user.rowstatus = false;
                    db.SaveChanges();

                    var user1 = db.UserAccess_GSTIN.FirstOrDefault(u => u.GSTINId == Id);
                    user1.Rowstatus = false;
                    db.SaveChanges();

                    Functions.UpdateOrInsertTable("Delete from TBL_AUTH_KEYS where AuthorizationToken = '"+ deletegst +"'");
                    Functions.UpdateOrInsertTable("Delete from TBL_AUTH_KEYS_EWB where GSTIN = '" + deletegst + "'");

                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTIN Number Deleted : " + gst.ToString(), "");
                    TempData["msg"] = "GSTIN Deleted Successfully";
                }
                return RedirectToAction("gstin");
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

    }
}