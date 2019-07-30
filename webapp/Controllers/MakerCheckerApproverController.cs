using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class MakerCheckerApproverController : Controller
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: MakerCheckerApprover
        [HttpGet]
        public ActionResult Home()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            string strUserEmail = "", strUserId = "";
            ViewBag.MakerUserList = GetUserlist(custid, "Maker");
            ViewBag.CheckerUserList = GetUserlist(custid, "Checker");

            strUserEmail = Helper.GetCustomerDetails("Email", "TBL_Customer", " WHERE CustId = " + custid + " and rowstatus = 1");
            strUserId = Helper.GetCustomerDetails("UserId", "UserList", " WHERE CustId = " + custid + " and Email = '" + strUserEmail + "' and rowstatus = 1");

            ViewBag.ApproverUserList = Exist_GetUserlist(custid, "Approver", strUserId);

            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection frm)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            try
            {
                string MakerId = "", CheckerId = "", ApproverId = "", btnCreate = "", btnContinue = "", MakerName = "", CheckerName = "", ApproverName = "";

                MakerId = frm["ddlMaker"];
                CheckerId = frm["ddlChecker"];
                ApproverId = frm["ddlApprover"];
                btnCreate = frm["btnCreate"];
                btnContinue = frm["btnContinue"];

                MakerName = Helper.GetCustomerDetails("Name", "UserList", " WHERE UserId = " + MakerId + " and CustId = " + custid + " and rowstatus = 1");
                CheckerName = Helper.GetCustomerDetails("Name", "UserList", " WHERE UserId = " + CheckerId + " and CustId = " + custid + " and rowstatus = 1");
                ApproverName = Helper.GetCustomerDetails("Name", "UserList", " WHERE UserId = " + ApproverId + " and CustId = " + custid + " and rowstatus = 1");
                                
                if (!string.IsNullOrEmpty(btnCreate))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select * from TBL_Cust_MakerCheckerApproverAccess_Users Where MakerUserId = @MakerId and ApproverUserId = @ApproverId and CustId = @CustId and RowStatus = 1";
                            sqlcmd.Parameters.AddWithValue("@MakerId", MakerId);
                            sqlcmd.Parameters.AddWithValue("@CheckerId", CheckerId);
                            sqlcmd.Parameters.AddWithValue("@ApproverId", ApproverId);
                            sqlcmd.Parameters.AddWithValue("@CustId", custid);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    ViewBag.MakerUserList = Exist_GetUserlist(custid, "Maker", MakerId);
                                    ViewBag.CheckerUserList = Exist_GetUserlist(custid, "Checker", CheckerId);
                                    ViewBag.ApproverUserList = Exist_GetUserlist(custid, "Approver", ApproverId);
                                    ViewBag.ModalDialog = "OPEN_POPUP";
                                }
                                else
                                {
                                    SqlCommand cmd = new SqlCommand();
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@MakerUserId", SqlDbType.Int).Value = MakerId;
                                    cmd.Parameters.Add("@MakerName", SqlDbType.NVarChar).Value = MakerName;
                                    cmd.Parameters.Add("@CheckerUserId", SqlDbType.Int).Value = CheckerId;
                                    cmd.Parameters.Add("@CheckerName", SqlDbType.NVarChar).Value = CheckerName;
                                    cmd.Parameters.Add("@ApproverUserId", SqlDbType.Int).Value = ApproverId;
                                    cmd.Parameters.Add("@ApproverName", SqlDbType.NVarChar).Value = ApproverName;
                                    cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = custid;
                                    cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userid;
                                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    cmd.Parameters.Add("@RowStatus", SqlDbType.Bit).Value = 1;
                                    SQLHelper.InsertIntoTable("TBL_Cust_MakerCheckerApproverAccess_Users", cmd, conn);

                                    TempData["SuccessMessage"] = "Maker-Checker-Approver Linked Successfully";
                                    ViewBag.MakerUserList = GetUserlist(custid, "Maker");
                                    ViewBag.CheckerUserList = GetUserlist(custid, "Checker");
                                    ViewBag.ApproverUserList = Exist_GetUserlist(custid, "Approver", ApproverId);
                                }
                            }
                        }
                        conn.Close();
                    }
                }
                else if (!string.IsNullOrEmpty(btnContinue))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand cmd = new SqlCommand())
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@CheckerUserId", SqlDbType.Int).Value = CheckerId;
                            cmd.Parameters.Add("@CheckerName", SqlDbType.NVarChar).Value = CheckerName;
                            SQLHelper.UpdateTable("TBL_Cust_MakerCheckerApproverAccess_Users", "MakerUserId", MakerId, cmd, conn);

                            TempData["SuccessMessage"] = "Maker-Checker-Approver Linkage updated Successfully";
                            ViewBag.MakerUserList = GetUserlist(custid, "Maker");
                            ViewBag.CheckerUserList = GetUserlist(custid, "Checker");
                            ViewBag.ApproverUserList = Exist_GetUserlist(custid, "Approver", ApproverId);
                        }
                        conn.Close();
                    }
                }
                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            
            return View();
        }

        public static SelectList GetMakerUserlist(int CustId, string strType)
        {
            SelectList LstUserlist = null;
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                DataTable dt = new DataTable();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = Con;
                    sqlcmd.CommandText = "Select * from UserList where UserId not in (select MakerUserId from TBL_Cust_MakerCheckerApproverAccess_Users where RowStatus = 1 and CustId = @CustId) and CustId = @CustId and rowstatus = 1 and MakerCheckerApproverType = @MakerCheckerApproverType and ISNULL(MakerCheckerApproverType, '') <> ''";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    sqlcmd.Parameters.AddWithValue("@MakerCheckerApproverType", strType);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        adt.Fill(dt);
                    }
                }

                var userList = (from ob in dt.AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                               ).ToList();
                LstUserlist = new SelectList(userList, "UserId", "Name");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return LstUserlist;
        }


        public static SelectList GetUserlist(int CustId, string strType)
        {
            SelectList LstUserlist = null;
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                DataTable dt = new DataTable();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = Con;
                    sqlcmd.CommandText = "Select * from UserList where CustId = @CustId and rowstatus = 1 and MakerCheckerApproverType = @MakerCheckerApproverType";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    sqlcmd.Parameters.AddWithValue("@MakerCheckerApproverType", strType);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        adt.Fill(dt);
                    }
                }

                var userList = (from ob in dt.AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                               ).ToList();
                LstUserlist = new SelectList(userList, "UserId", "Name");
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return LstUserlist;
        }

        public static SelectList Exist_GetUserlist(int CustId, string strType, string UserId)
        {
            SelectList LstUserlist = null;
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                DataTable dt = new DataTable();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = Con;
                    sqlcmd.CommandText = "Select * from UserList where CustId = @CustId and rowstatus = 1 and MakerCheckerApproverType = @MakerCheckerApproverType";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    sqlcmd.Parameters.AddWithValue("@MakerCheckerApproverType", strType);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        adt.Fill(dt);
                    }
                }

                var userList = (from ob in dt.AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                               ).ToList();
                LstUserlist = new SelectList(userList, "UserId", "Name", UserId);
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return LstUserlist;
        }
    }
}