using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using System.Data.SqlClient;
using SmartAdminMvc.Models;
using System.Configuration;
using System.Net;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class TRPRegistrationController : Controller
    {
        public WePGSPDBEntities db = new WePGSPDBEntities();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        SqlConnection con1 = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString1"]);

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index(string option, string search)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int i = Convert.ToInt32(Session["Cust_ID"]);

            string op = option;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "TRP Id",
                Value = "TRP Id"
            });
            items.Add(new SelectListItem
            {
                Text = "TRP Name",
                Value = "TRP Name"
            });
            items.Add(new SelectListItem
            {
                Text = "Email ID",
                Value = "Email ID"
            });
            items.Add(new SelectListItem
            {
                Text = "Mobile Number",
                Value = "Mobile Number"
            });


            ViewBag.Categories = new SelectList(items, "Text", "Value", op);


            if (option == "TRP Id")
            {

                List<TBL_Aggregator> aggregator = (from li in db.TBL_Aggregator
                                                   where li.CustomerId == i && (li.AggregatorId.Contains(search) || li.AggregatorId.StartsWith(search) || li.AggregatorId == search || search == null) && li.rowstatus == true
                                                   select li).ToList();
                return View(aggregator);



            }

            else if (option == "TRP Name")
            {
                List<TBL_Aggregator> aggregator = (from li in db.TBL_Aggregator
                                                 where li.CustomerId == i && (li.AggregatorName.Contains(search) || li.AggregatorName.StartsWith(search) || li.AggregatorName == search || search == null) && li.rowstatus == true
                                                   select li).ToList();

                return View(aggregator);
            }

            else if (option == "Email ID")
            {
                List<TBL_Aggregator> aggregator = (from li in db.TBL_Aggregator
                                                 where li.CustomerId == i && (li.Email.Contains(search) || li.Email.StartsWith(search) || li.Email == search || search == null) && li.rowstatus == true
                                                   select li).ToList();

                return View(aggregator);
            }

            else if (option == "Mobile Number")
            {
                List<TBL_Aggregator> aggregator = (from li in db.TBL_Aggregator
                                                   where li.CustomerId == i && (li.MobileNo.Contains(search) || li.MobileNo.StartsWith(search) || li.MobileNo == search || search == null) && li.rowstatus == true
                                                   select li).ToList();

                return View(aggregator);
            }



            List<TBL_Aggregator> user = (from li in db.TBL_Aggregator
                                   where li.CustomerId== i && li.rowstatus==true
                                   select li).ToList();

            return View(user);
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(FormCollection frm)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());


            int custid = Convert.ToInt32(Session["Cust_ID"]);

            var AId = (from li in db.TBL_Aggregator
                         where li.CustomerId == custid
                         select li.AggregatorId).FirstOrDefault();
            var AEmail = (from li in db.TBL_Aggregator
                         where li.CustomerId == custid
                         select li.Email).FirstOrDefault();
            var AMobile = (from li in db.TBL_Aggregator
                          where li.CustomerId == custid
                          select li.MobileNo).FirstOrDefault();

           
            if (AId == frm["aggregatorid"] || AEmail == frm["email"] || AMobile == frm["phone"])
            {
                TempData["msg"] = "TRP details already exists ";
            }
            else { 
            string UserEmail = (from user in db.UserLists
                                   where user.UserId == UserId
                                select user.Email).SingleOrDefault();

            string UserPassword = (from user in db.UserLists
                                where user.UserId == UserId
                                select user.Password).SingleOrDefault();

            string GSTINNo = (from user in db.TBL_Customer
                                  where user.CustId == CustId
                                  select user.GSTINNo).SingleOrDefault();

            SqlCommand cmd = new SqlCommand();
            con.Open();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@AggregatorName", SqlDbType.VarChar).Value = frm["Name"];
            cmd.Parameters.Add("@AggregatorId", SqlDbType.VarChar).Value = frm["aggregatorid"];
            cmd.Parameters.Add("@Email", SqlDbType.VarChar).Value = frm["email"];
            cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = frm["phone"];
            cmd.Parameters.Add("@Password", SqlDbType.VarChar).Value = frm["phone"];
            cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();
            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Session["User_ID"].ToString();
            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
            Models.Common.Functions.InsertIntoTable("TBL_Aggregator", cmd, con);
            con.Close();

            // Aggregator Separate Database 

            con1.Open();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@full_name", SqlDbType.VarChar).Value = frm["Name"];
            cmd.Parameters.Add("@email", SqlDbType.VarChar).Value = frm["email"];
            cmd.Parameters.Add("@role", SqlDbType.VarChar).Value = "A";
            cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = frm["phone"];
            string AggrId = Models.Common.Functions.InsertIntoTable("tbl_users", cmd, con1);
            con1.Close();

            con1.Open();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@aggrigator_id", SqlDbType.Int).Value = AggrId;
            cmd.Parameters.Add("@aggrigator_email", SqlDbType.VarChar).Value = frm["email"];
            cmd.Parameters.Add("@user_email", SqlDbType.VarChar).Value = UserEmail;
            cmd.Parameters.Add("@user_gstn", SqlDbType.VarChar).Value = GSTINNo;
            cmd.Parameters.Add("@user_full_name", SqlDbType.VarChar).Value = Session["UserName"].ToString();
            cmd.Parameters.Add("@user_password", SqlDbType.VarChar).Value = UserPassword;
            Models.Common.Functions.InsertIntoTable("tbl_aggregator_map", cmd, con1);
            con1.Close();

                // Aggregator Separate Database 
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TRP Registred", "");

                TempData["msg"] = "TRP Registered Successfully";
            }
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "TRP Id",
                Value = "TRP Id"
            });
            items.Add(new SelectListItem
            {
                Text = "TRP Name",
                Value = "TRP Name"
            });
            items.Add(new SelectListItem
            {
                Text = "Email ID",
                Value = "Email ID"
            });
            items.Add(new SelectListItem
            {
                Text = "Mobile Number",
                Value = "Mobile Number"
            });


            ViewBag.Categories = new SelectList(items, "Text", "Value");
            List<TBL_Aggregator> userlist = (from li in db.TBL_Aggregator
                                         where li.CustomerId == CustId
                                         select li).ToList();

         

            return View(userlist);
        }


        public ActionResult Block(int Id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int cid = Convert.ToInt32(Session["User_ID"]);
                var user = db.TBL_Aggregator.FirstOrDefault(u => u.TrpId == Id);
                user.rowstatus = false;
                db.SaveChanges();

                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TRP Blocked", "");
                TempData["msg"] = "TRP Details Blocked Successfully";
                return RedirectToAction("Index");

            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ActionResult UnBlock(int Id)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int cid = Convert.ToInt32(Session["User_ID"]);
                var user = db.TBL_Aggregator.FirstOrDefault(u => u.TrpId == Id);
                user.rowstatus = true;
                db.SaveChanges();

                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TRP UnBlocked", "");
                TempData["msg"] = "TRP Details Unblocked Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;
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
                TBL_Aggregator li = db.TBL_Aggregator.Find(Id);
                db.TBL_Aggregator.Remove(li);
                db.SaveChanges();
                TempData["msg"] = "TRP Details Deleted Successfully";
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TRP Deleted", "");
                return RedirectToAction("Index");
            }

            catch (Exception ex)
            {
                throw ex;
            }

        }



        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_Aggregator Aggregator = db.TBL_Aggregator.Find(id);
            if (Aggregator == null)
            {
                return HttpNotFound();
            }
            return View(Aggregator);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection form)
        {
            try
            {

                int custid = Convert.ToInt32(Session["Cust_ID"]);
                string AggregatorID = form["AggregatorId"];
                string Email = form["Email"];
                string Mobile = form["MobileNo"];


                var TRPCount = (from li in db.TBL_Aggregator where (li.AggregatorId == AggregatorID || li.Email == Email || li.MobileNo == Mobile) && li.TrpId != id && li.CustomerId == custid select li).ToList();
                //var AId = (from li in db.TBL_Aggregator
                //           where li.CustomerId == custid
                //           select li.AggregatorId).FirstOrDefault();
                //var AEmail = (from li in db.TBL_Aggregator
                //              where li.CustomerId == custid
                //              select li.Email).FirstOrDefault();
                //var AMobile = (from li in db.TBL_Aggregator
                //               where li.CustomerId == custid
                //               select li.MobileNo).FirstOrDefault();
                //var TrpId = (from li in db.TBL_Aggregator
                //             where li.CustomerId == custid
                //             select li.TrpId).FirstOrDefault();

                if (TRPCount.Count()>0)
                {
                    TempData["msg"] = "TRP details already exists";
                }
                else { 
                TBL_Aggregator Aggregator = db.TBL_Aggregator.Where(u => u.TrpId == id).SingleOrDefault();
                Aggregator.AggregatorId = form["AggregatorId"];
                Aggregator.AggregatorName = form["AggregatorName"];
                Aggregator.Email = form["Email"];
                Aggregator.MobileNo = form["MobileNo"];
                

                db.SaveChanges();
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TRP Updated", "");
                    TempData["msg"] = "TRP Details Updated Successfully";
                }
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                throw ex;

            }
        }


    }
}