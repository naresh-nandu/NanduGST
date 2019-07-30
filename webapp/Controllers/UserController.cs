using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class UserController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        [HttpGet]
        public ActionResult Registration(string option, string search)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                int iUserId = Convert.ToInt32(Session["User_ID"]);
                int iCustId = Convert.ToInt32(Session["Cust_ID"]);

                //searching dropdown
                string op = option;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "User Name",
                    Value = "User Name"
                });
                items.Add(new SelectListItem
                {
                    Text = "Designation",
                    Value = "Designation"
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


                ViewBag.Category = new SelectList(items, "Text", "Value", op);


                //Roles dropdown

                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == iCustId
                               select new { li.Role_ID, li.Role_Name };
                ViewBag.Role = Rolelist.ToList();


                //Gstin dropdown

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());

                ViewBag.MakerCheckerApproverList = LoadDropDowns.GetMakerCheckerApproverList();

                //searching Result
                if (option == "User Name")
                {
                    List<UserList> username = (from li in db.UserLists
                                               where li.CustId == iCustId && li.rowstatus == true && (li.Name.Contains(search) || li.Name.StartsWith(search) || li.Name == search || search == null)
                                               select li).ToList();
                    ViewBag.userlist = username;
                }

                else if (option == "Designation")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == iCustId && li.rowstatus == true && (li.Designation.Contains(search) || li.Designation.StartsWith(search) || li.Designation == search || search == null)
                                                  select li).ToList();
                    ViewBag.userlist = Designation;
                }

                else if (option == "Email ID")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == iCustId && li.rowstatus == true && (li.Email.Contains(search) || li.Email.StartsWith(search) || li.Email == search || search == null)
                                                  select li).ToList();
                    ViewBag.userlist = Designation;
                }

                else if (option == "Mobile Number")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == iCustId && li.rowstatus == true && (li.MobileNo.Contains(search) || li.MobileNo.StartsWith(search) || li.MobileNo == search || search == null)
                                                  select li).ToList();
                    ViewBag.userlist = Designation;
                }
                else
                {
                    List<UserList> user = (from li in db.UserLists
                                           where li.CustId == iCustId && li.rowstatus == true
                                           select li).ToList();
                    ViewBag.userlist = user;
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
        public ActionResult Registration(FormCollection form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                int iUserId = Convert.ToInt32(Session["User_ID"]);
                int iCustId = Convert.ToInt32(Session["Cust_ID"]);

                string strMakerCheckerApprover = "", strStationCode = "";
                strMakerCheckerApprover = form["ddlMakerCheckerApprover"].ToString();
                strStationCode = form["stationcode"].ToString();
                UserList user = new UserList();
                user.CustId = Convert.ToInt32(Session["CUST_ID"]);
                user.Name = form["name"];
                user.Designation = form["designation"];
                user.Email = form["email"];
                user.Password = form["Password"];
                user.MobileNo = form["phone"];
                user.RoleId = Convert.ToInt32(form["Role_Name"]);
                user.ValidFrom = null;
                user.ValidTo = null;
                user.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                user.EsignedUser = null;

                using (WePGSPDBEntities context = new WePGSPDBEntities())
                {
                    ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
                    ObjectParameter op1 = new ObjectParameter("UserId", typeof(int));

                    context.Ins_UserList(user.CustId, user.Name, user.Designation, "", user.Password, user.Email, user.MobileNo, user.RoleId, user.ValidFrom, user.ValidTo, user.CreatedBy, user.EsignedUser, op, op1);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "New User " + user.Username + " is created.", "");

                    var value = op.Value; //Get the output value.
                    var value1 = op1.Value;

                    // Updating MakerCheckerApprover in UserList Table
                    if (!string.IsNullOrEmpty(strMakerCheckerApprover))
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            conn.Open();
                            int strUserId = (int)op1.Value;
                            SqlCommand sqlcmd = new SqlCommand();
                            sqlcmd.Parameters.Clear();
                            sqlcmd.Parameters.Add("@MakerCheckerApproverType", SqlDbType.NVarChar).Value = strMakerCheckerApprover;
                            Functions.UpdateTable("UserList", "UserId", strUserId.ToString(), sqlcmd, conn);
                            conn.Close();
                        }
                    }

                    // Updating Station Code in UserList Table
                    if (!string.IsNullOrEmpty(strStationCode))
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            conn.Open();
                            int strUserId = (int)op1.Value;
                            SqlCommand sqlcmd = new SqlCommand();
                            sqlcmd.Parameters.Clear();
                            sqlcmd.Parameters.Add("@StationCode", SqlDbType.NVarChar).Value = strStationCode;
                            Functions.UpdateTable("UserList", "UserId", strUserId.ToString(), sqlcmd, conn);
                            conn.Close();
                        }
                    }

                    if ((int)value == 1)
                    {
                        if (Session["Partner_Company"].ToString() == "Hamara Kendra")
                        {
                            Notification.SendSMS(user.MobileNo.ToString(), "You are added as a user by your admin for Hamara Kendra GST Suvidha. Your Login Email Id is " + user.Email + " and Password is " + user.Password + ". Click here to login to GST Return Filing Suvidha https://gst-ws.hamarakendra.com");
                            Notification.SendEmail(user.Email, "You are added as a user by your admin for GST Return Filing Suvidha.", "Your Login Email Id is " + user.Email + " and Password is " + user.Password + ".<br/>Click here to login to GST Return Filing Suvidha https://gst-ws.hamarakendra.com");
                        }
                        else
                        {
                            Notification.SendSMS(user.MobileNo.ToString(), "You are added as a user by your admin for WeP GST Panel. Your Login Email Id is " + user.Email + " and Password is " + user.Password);
                            Notification.SendEmail(user.Email, "You are added as a user by your admin for WeP GST Panel.", "Your Login Email Id is " + user.Email + " and Password is " + user.Password);
                        }
                        context.SaveChanges();
                        UserAccess_GSTIN usergst = new UserAccess_GSTIN();
                        string[] AllStrings = form["GSTINId"].Split(',');

                        foreach (var gst in AllStrings)
                        {
                            var gstid = (from li in db.TBL_Cust_GSTIN
                                         where li.GSTINNo == gst && li.rowstatus == true
                                         select li.GSTINId).FirstOrDefault();
                            usergst.GSTINId = Convert.ToInt32(gstid);
                            usergst.CustId = Convert.ToInt32(Session["Cust_ID"]);
                            usergst.UserId = (int)value1;
                            usergst.CreatedDate = DateTime.Now;
                            usergst.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                            usergst.Rowstatus = true;
                            context.UserAccess_GSTIN.Add(usergst);
                            context.SaveChanges();

                            var GSTINNo = (from li in db.TBL_Cust_GSTIN
                                           where li.GSTINId == usergst.GSTINId && li.rowstatus == true
                                           select li.GSTINNo).FirstOrDefault();
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User " + user.Username + " is mapped to " + GSTINNo, "");
                            TempData["msg"] = "User Registration Done Successfully ";
                        }
                    }
                    else if ((int)value == 2)
                    {

                        TempData["msg"] = "Mobile Number or Email Id already exist.";

                    }

                    else if ((int)value == 3)
                    {
                        TempData["msg"] = "User is not valid.";

                    }
                }



                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == iCustId
                               select new { li.Role_ID, li.Role_Name };
                ViewBag.Role = Rolelist.ToList();

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(iUserId, iCustId, Session["Role_Name"].ToString());
                ViewBag.MakerCheckerApproverList = LoadDropDowns.GetMakerCheckerApproverList();

                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "User Name",
                    Value = "User Name"
                });
                items.Add(new SelectListItem
                {
                    Text = "Designation",
                    Value = "Designation"
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
                ViewBag.Category = new SelectList(items, "Text", "Value");

                List<UserList> userlist = (from li in db.UserLists
                                           where li.CustId == iCustId && li.rowstatus == true
                                           select li).ToList();
                ViewBag.userlist = userlist;

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult Edit()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {

                int i = Convert.ToInt32(Session["Cust_ID"]);
                var gstinlist = from li in db.TBL_Cust_GSTIN
                                where li.CustId == i && li.rowstatus == true
                                select new { li.GSTINId, li.GSTINNo };
                ViewBag.gstin = gstinlist.ToList();
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult getData(int id)
        {
            try
            {
                Session["sid"] = id;

                var v = (from ulig in db.UserAccess_GSTIN
                         join uli in db.UserLists on ulig.UserId equals uli.UserId
                         where (ulig.GSTINId == id && ulig.Rowstatus == true)
                         select new
                         {
                             uli.CustId,
                             uli.UserId,
                             uli.Name,
                             uli.Designation,
                             uli.Email,
                             uli.MobileNo,
                             uli.Status
                         });


                List<UserList> listtable = new List<UserList>();
                foreach (var p in v)
                {
                    UserList ul = new UserList();
                    ul.CustId = p.CustId;
                    ul.UserId = p.UserId;
                    ul.Name = p.Name;
                    ul.Designation = p.Designation;
                    ul.Email = p.Email;
                    ul.MobileNo = p.MobileNo;
                    ul.Status = p.Status;
                    listtable.Add(ul);


                }

                return Json(listtable, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public ActionResult UserDelete(int? userid, int? gstin)
        {
            try
            {
                int uid = Convert.ToInt32(Session["User_ID"]);
                int cid = Convert.ToInt32(Session["Cust_ID"]);
                var username = (from li in db.UserLists
                                where li.UserId == userid
                                select li.Username).FirstOrDefault();
                if (uid == userid)
                {
                    TempData["msg"] = "You cannot deleted your Details";
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User " + username + " own access deletion is failure.", "");
                }
                else
                {
                    var roleid = (from li in db.UserLists where li.UserId == userid select li.RoleId).SingleOrDefault();
                    var rolename = (from li in db.MAS_Roles where li.Role_ID == roleid select li.Role_Name).SingleOrDefault();
                    if (rolename == "Super Admin" || rolename == "Admin")
                    {
                        TempData["msg"] = "You donnot have access to delete admin or super admin details";
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User " + username + " deletion is failure, because this user is Admin/Super Admin.", "");
                    }
                    else
                    {
                        var user = db.UserLists.FirstOrDefault(u => u.UserId == userid);
                        user.rowstatus = false;
                        db.SaveChanges();

                        var ugst = (from gst in db.UserAccess_GSTIN
                                    where gst.UserId == userid && gst.CustId == cid
                                    select gst.UserAccessId).ToList();
                        foreach (int a in ugst)
                        {
                            var useraceess = db.UserAccess_GSTIN.FirstOrDefault(u => u.UserAccessId == a);
                            useraceess.Rowstatus = false;
                            db.SaveChanges();
                        }
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Deleted", "");
                        TempData["msg"] = "User Deleted Successfully";
                    }
                }
                return RedirectToAction("edit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpGet]
        public ActionResult AddUser(int gstin)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                int i = Convert.ToInt32(Session["Cust_ID"]);
                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == i
                               select new { li.Role_ID, li.Role_Name };
                ViewBag.Role = Rolelist.ToList();


                var categories = db.TBL_Cust_GSTIN.Where(m => m.GSTINId == gstin).Select(c => new
                {
                    CategoryID = c.GSTINId,
                    CategoryName = c.GSTINNo
                }).ToList();
                ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        [HttpPost]
        public ActionResult AddUser(int? gstin, FormCollection form)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                UserList user = new UserList();
                user.CustId = Convert.ToInt32(Session["CUST_ID"]);
                user.Name = form["name"];
                user.Designation = form["designation"];
                user.Email = form["email"];
                user.Password = form["Password"];
                user.MobileNo = form["phone"];
                user.RoleId = Convert.ToInt32(form["Role_Name"]);
                user.ValidFrom = null;
                user.ValidTo = null;
                user.CreatedBy = Convert.ToInt32(Session["CUST_ID"]);
                user.EsignedUser = null;

                using (WePGSPDBEntities context = new WePGSPDBEntities())
                {
                    ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
                    ObjectParameter op1 = new ObjectParameter("UserId", typeof(int));

                    context.Ins_UserList(user.CustId, user.Name, user.Designation, "", user.Password, user.Email, user.MobileNo, user.RoleId, user.ValidFrom, user.ValidTo, user.CreatedBy, user.EsignedUser, op, op1);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Registrered", "");


                    var value = op.Value; //Get the output value.
                    var value1 = op1.Value;
                    if ((int)value == 1)
                    {
                        Notification.SendSMS(user.MobileNo.ToString(), "You are added as a user by your admin for WeP GST Panel. Your Login Email Id is " + user.Email + " and Password is " + user.Password);
                        Notification.SendEmail(user.Email, "You are added as a user by your admin for WeP GST Panel.", "Your Login Email Id is " + user.Email + " and Password is " + user.Password);

                        context.SaveChanges();
                        UserAccess_GSTIN usergst = new UserAccess_GSTIN();
                        string[] AllStrings = form["CategoryId"].Split(',');

                        foreach (var gst in AllStrings)
                        {
                            usergst.GSTINId = int.Parse(gst);
                            usergst.CustId = Convert.ToInt32(Session["Cust_ID"]);
                            usergst.UserId = (int)value1;
                            usergst.CreatedDate = DateTime.Now;
                            usergst.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                            usergst.Rowstatus = true;
                            context.UserAccess_GSTIN.Add(usergst);
                            context.SaveChanges();

                            TempData["msg"] = "User Added Successfully ";
                        }
                    }
                    else if ((int)value == 2)
                    {

                        TempData["msg"] = "User Name or Email Id already exists under the given customer";

                    }

                    else if ((int)value == 3)
                    {
                        TempData["msg"] = "Customer is not valid(i.e. Customer status may be not yet)";

                    }
                }

                int i = Convert.ToInt32(Session["Cust_ID"]);
                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == i
                               select new { li.Role_ID, li.Role_Name };
                ViewBag.Role = Rolelist.ToList();


                var categories = db.TBL_Cust_GSTIN.Where(m => m.GSTINId == gstin).Select(c => new
                {
                    CategoryID = c.GSTINId,
                    CategoryName = c.GSTINNo
                }).ToList();
                ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

                return RedirectToAction("edit");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }


        [HttpGet]
        public ActionResult UserEdit(int custid, int userid)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["uid"] = userid;
                Session["cid"] = custid;
                int i = Convert.ToInt32(Session["Cust_ID"]);
                string strMakerCheckerApproverType = "", strStationCode = "";
                strMakerCheckerApproverType = Helper.GetCustomerDetails("MakerCheckerApproverType", "UserList", " WHERE CustId = " + i + " and UserId = " + userid + " and rowstatus = 1");
                ViewBag.MakerCheckerApproverList = LoadDropDowns.Exist_GetMakerCheckerApproverList(strMakerCheckerApproverType);
                strStationCode = Helper.GetCustomerDetails("StationCode", "UserList", " WHERE CustId = " + i + " and UserId = " + userid + " and rowstatus = 1");
                ViewBag.StationCode = strStationCode;

                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == userid && p.CustId == custid && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList

                        };
                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }
                foreach (var a in v)
                {
                    ViewBag.Name = a.UserList.Name;
                    ViewBag.Designation = a.UserList.Designation;
                    ViewBag.UserName = a.UserList.Username;
                    ViewBag.Password = a.UserList.Password;
                    ViewBag.Email = a.UserList.Email;
                    ViewBag.Mobile = a.UserList.MobileNo;
                    ViewBag.Roleid = a.UserList.RoleId;
                }


                var categories = (from p in custgst
                                  where p.rowstatus == true
                                  select new
                                  {
                                      gstid = p.GSTINId,
                                      gstnum = p.GSTINNo
                                  }).ToList();
                ViewBag.GSTINNumber = new SelectList(categories, "gstid", " gstnum");

                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == i
                               select new
                               {
                                   RoleID = li.Role_ID,
                                   RoleName = li.Role_Name
                               };
                ViewBag.Role = new SelectList(Rolelist, "RoleID", "RoleName", ViewBag.Roleid);


                var GSTIN = (from p in db.TBL_Cust_GSTIN
                             where p.CustId == i && p.rowstatus == true
                             select new
                             {
                                 CategoryID = p.GSTINId,
                                 CategoryName = p.GSTINNo
                             }).ToList();
                ViewBag.Categories = new SelectList(GSTIN, "CategoryID", "CategoryName");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult UserEdit(FormCollection form, string command)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                //adding New GSTIN Number for User Updation

                if (command == "add")
                {
                    int i = Convert.ToInt32(Session["Cust_ID"]);
                    int UserID = Convert.ToInt32(Session["User_ID"]);
                    int cutid = Convert.ToInt32(Session["cid"]);
                    int userid = Convert.ToInt32(Session["uid"]);


                    var Email = (from li in db.UserLists
                                 where li.UserId == UserID
                                 select li.Email).FirstOrDefault();
                    var Mobile = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.MobileNo).FirstOrDefault();
                    var UserNo = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.UserId).FirstOrDefault();

                    if ((Email == form["email"] || Mobile == form["phone"]) && UserNo != userid)
                    {
                        TempData["msg"] = "User details already exists ";
                    }
                    else
                    {
                        UserList user = db.UserLists.Where(x => x.UserId == userid).SingleOrDefault();

                        user.Name = form["name"];
                        user.Designation = form["designation"];
                        user.Email = form["email"];
                        user.Password = form["Password"];
                        user.MobileNo = form["phone"];
                        user.RoleId = Convert.ToInt32(form["CategoryName"]);
                        user.ValidFrom = null;
                        user.ValidTo = null;
                        user.EsignedUser = null;
                        db.SaveChanges();

                        UserAccess_GSTIN usergst = new UserAccess_GSTIN();
                        string[] AllStrings = form["CategoryId"].Split(',');

                        foreach (var gst in AllStrings)
                        {
                            int gstloop = Convert.ToInt32(gst);
                            var gstnumber = (from li in db.UserAccess_GSTIN
                                             where li.GSTINId == gstloop && li.UserId == userid && li.CustId == cutid && li.Rowstatus == true
                                             select li.GSTINId).ToList();
                            if (gstnumber.Count > 0)
                            {
                                TempData["msg"] = "This GSTIN Number Already Exist";
                            }
                            else if (gstnumber == null)
                            {
                                TempData["msg"] = "Please Select GSTIN Number";
                            }
                            else
                            {
                                usergst.GSTINId = int.Parse(gst);
                                usergst.CustId = cutid;
                                usergst.UserId = userid;
                                usergst.CreatedDate = DateTime.Now;
                                usergst.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                                usergst.Rowstatus = true;
                                db.UserAccess_GSTIN.Add(usergst);
                                db.SaveChanges();
                                TempData["msg"] = "Successfully Added New GSTIN Number For This User";
                            }
                        }
                    }

                    var Rolelist = from li in db.MAS_Roles
                                   where li.CustomerID == i
                                   select new { li.Role_ID, li.Role_Name };
                    ViewBag.Role = Rolelist.ToList();

                    var v = from p in db.UserAccess_GSTIN
                            where (p.UserId == userid)
                            select new
                            {
                                p.TBL_Cust_GSTIN,
                                p.TBL_Customer,
                                p.UserList

                            };
                    List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                    foreach (var a in v)
                    {
                        custgst.Add(a.TBL_Cust_GSTIN);
                    }
                    foreach (var a in v)
                    {
                        ViewBag.Name = a.UserList.Name;
                        ViewBag.Designation = a.UserList.Designation;
                        ViewBag.Password = a.UserList.Password;
                        ViewBag.Email = a.UserList.Email;
                        ViewBag.Mobile = a.UserList.MobileNo;
                    }

                    var categories = (from p in custgst
                                      where p.rowstatus == true
                                      select new
                                      {
                                          gstid = p.GSTINId,
                                          gstnum = p.GSTINNo
                                      }).ToList();
                    ViewBag.GSTINNumber = new SelectList(categories, "gstid", " gstnum");


                    var GSTIN = (from p in db.TBL_Cust_GSTIN
                                 where p.CustId == i && p.rowstatus == true
                                 select new
                                 {
                                     CategoryID = p.GSTINId,
                                     CategoryName = p.GSTINNo
                                 }).ToList();
                    ViewBag.Categories = new SelectList(GSTIN, "CategoryID", "CategoryName");

                }

                //delete User Accesed GSTIN Number

                else if (command == "delete")
                {
                    int i = Convert.ToInt32(Session["Cust_ID"]);
                    int id = Convert.ToInt32(form["gstid"]);
                    int uid = Convert.ToInt32(Session["User_ID"]);

                    int userid = Convert.ToInt32(Session["uid"]);
                    if (uid == userid)
                    {
                        TempData["msg"] = "You cannot deleted your Details";
                    }
                    else
                    {
                        var roleid = (from li in db.UserLists where li.UserId == userid select li.RoleId).SingleOrDefault();
                        var rolename = (from li in db.MAS_Roles where li.Role_ID == roleid select li.Role_Name).SingleOrDefault();
                        if (rolename == "Super Admin" || rolename == "Admin")
                        {
                            TempData["msg"] = "You donnot have access to delete admin or super admin details";
                            int k = Convert.ToInt32(Session["Cust_ID"]);
                            var gstinlst = from li in db.TBL_Cust_GSTIN
                                           where li.CustId == k && li.rowstatus == true
                                           select new { li.GSTINId, li.GSTINNo };
                            ViewBag.gstin = gstinlst.ToList();

                            return View("Edit");

                        }
                        else
                        {
                            List<UserAccess_GSTIN> ugst = (from gst in db.UserAccess_GSTIN
                                                           where gst.GSTINId == id && gst.UserId == userid && gst.Rowstatus == true
                                                           select gst).ToList();
                            foreach (var a in ugst)
                            {
                                db.UserAccess_GSTIN.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }
                    var Rolelist = from li in db.MAS_Roles
                                   where li.CustomerID == i
                                   select new { li.Role_ID, li.Role_Name };
                    ViewBag.Role = Rolelist.ToList();

                    var v = from p in db.UserAccess_GSTIN
                            where (p.UserId == userid)
                            select new
                            {
                                p.TBL_Cust_GSTIN,
                                p.TBL_Customer,
                                p.UserList

                            };
                    List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                    foreach (var a in v)
                    {
                        custgst.Add(a.TBL_Cust_GSTIN);
                    }
                    foreach (var a in v)
                    {
                        ViewBag.Name = a.UserList.Name;
                        ViewBag.Designation = a.UserList.Designation;
                        ViewBag.UserName = a.UserList.Username;
                        ViewBag.Password = a.UserList.Password;
                        ViewBag.Email = a.UserList.Email;
                        ViewBag.Mobile = a.UserList.MobileNo;

                    }

                    var categories = (from p in custgst
                                      where p.rowstatus == true
                                      select new
                                      {
                                          gstid = p.GSTINId,
                                          gstnum = p.GSTINNo
                                      }).ToList();
                    ViewBag.GSTINNumber = new SelectList(categories, "gstid", " gstnum");


                    var GSTIN = (from p in db.TBL_Cust_GSTIN
                                 where p.CustId == i && p.rowstatus == true
                                 select new
                                 {
                                     CategoryID = p.GSTINId,
                                     CategoryName = p.GSTINNo
                                 }).ToList();
                    ViewBag.Categories = new SelectList(GSTIN, "CategoryID", "CategoryName");

                    TempData["msg"] = "Selected GSTIN Number is Deleted Successfully";
                }

                //Updating User Details
                else
                {
                    int i = Convert.ToInt32(Session["Cust_ID"]);
                    int userid = Convert.ToInt32(Session["uid"]);
                    int UserID = Convert.ToInt32(Session["User_ID"]);
                    var Email = (from li in db.UserLists
                                 where li.UserId == UserID
                                 select li.Email).FirstOrDefault();
                    var Mobile = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.MobileNo).FirstOrDefault();
                    var UserNo = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.UserId).FirstOrDefault();

                    if ((Email == form["email"] || Mobile == form["phone"]) && UserNo != userid)
                    {
                        TempData["msg"] = "User details already exists ";
                    }
                    else
                    {
                        string strMakerCheckerApprover = "", strStationCode = "";
                        strMakerCheckerApprover = form["ddlMakerCheckerApprover"];
                        strStationCode = form["stationcode"];
                        UserList user = db.UserLists.Where(x => x.UserId == userid).SingleOrDefault();
                        user.Name = form["name"];
                        user.Designation = form["designation"];
                        user.Email = form["email"];
                        user.Password = form["Password"];
                        user.MobileNo = form["phone"];
                        user.RoleId = Convert.ToInt32(form["CategoryName"]);
                        user.ValidFrom = null;
                        user.ValidTo = null;
                        user.EsignedUser = null;
                        db.SaveChanges();

                        // Updating MakerCheckerApprover in UserList Table
                        if (!string.IsNullOrEmpty(strMakerCheckerApprover))
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                            {
                                conn.Open();
                                SqlCommand sqlcmd = new SqlCommand();
                                sqlcmd.Parameters.Clear();
                                sqlcmd.Parameters.Add("@MakerCheckerApproverType", SqlDbType.NVarChar).Value = strMakerCheckerApprover;
                                Functions.UpdateTable("UserList", "UserId", userid.ToString(), sqlcmd, conn);
                                conn.Close();
                            }
                        }

                        // Updating Station Code in UserList Table
                        if (!string.IsNullOrEmpty(strStationCode))
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                            {
                                conn.Open();
                                SqlCommand sqlcmd = new SqlCommand();
                                sqlcmd.Parameters.Clear();
                                sqlcmd.Parameters.Add("@StationCode", SqlDbType.NVarChar).Value = strStationCode;
                                Functions.UpdateTable("UserList", "UserId", userid.ToString(), sqlcmd, conn);
                                conn.Close();
                            }
                        }

                        TempData["msg"] = "User Updated Successfully";
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Updated", "");
                    }
                    var Rolelist = from li in db.MAS_Roles
                                   where li.CustomerID == i
                                   select new { li.Role_ID, li.Role_Name };
                    ViewBag.Role = Rolelist.ToList();

                    var v = from p in db.UserAccess_GSTIN
                            where (p.UserId == userid)
                            select new
                            {
                                p.TBL_Cust_GSTIN,
                                p.TBL_Customer,
                                p.UserList

                            };
                    List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                    foreach (var a in v)
                    {
                        custgst.Add(a.TBL_Cust_GSTIN);
                    }
                    foreach (var a in v)
                    {
                        ViewBag.Name = a.UserList.Name;
                        ViewBag.Designation = a.UserList.Designation;
                        ViewBag.Password = a.UserList.Password;
                        ViewBag.Email = a.UserList.Email;
                        ViewBag.Mobile = a.UserList.MobileNo;
                    }

                    var categories = (from p in custgst
                                      where p.rowstatus == true
                                      select new
                                      {
                                          gstid = p.GSTINId,
                                          gstnum = p.GSTINNo
                                      }).ToList();
                    ViewBag.GSTINNumber = new SelectList(categories, "gstid", " gstnum");


                    var GSTIN = (from p in db.TBL_Cust_GSTIN
                                 where p.CustId == i && p.rowstatus == true
                                 select new
                                 {
                                     CategoryID = p.GSTINId,
                                     CategoryName = p.GSTINNo
                                 }).ToList();
                    ViewBag.Categories = new SelectList(GSTIN, "CategoryID", "CategoryName");



                }
                int j = Convert.ToInt32(Session["Cust_ID"]);
                var gstinlist = from li in db.TBL_Cust_GSTIN
                                where li.CustId == j && li.rowstatus == true
                                select new { li.GSTINId, li.GSTINNo };
                ViewBag.gstin = gstinlist.ToList();

                return View("Edit");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                int i = Convert.ToInt32(Session["Cust_ID"]);
                var gstinlist = from li in db.TBL_Cust_GSTIN
                                where li.CustId == i && li.rowstatus == true
                                select new { li.GSTINId, li.GSTINNo };
                ViewBag.gstin = gstinlist.ToList();
                return View("edit");
            }
        }

        [HttpGet]
        public ActionResult EditUser(int Id)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                Session["uid"] = Id;
                Session["cid"] = Convert.ToInt32(Session["Cust_ID"]);
                int i = Convert.ToInt32(Session["Cust_ID"]);
                string strMakerCheckerApproverType = "", strStationCode = "";
                strMakerCheckerApproverType = Helper.GetCustomerDetails("MakerCheckerApproverType", "UserList", " WHERE CustId = " + i + " and UserId = " + Id + " and rowstatus = 1");
                ViewBag.MakerCheckerApproverList = LoadDropDowns.Exist_GetMakerCheckerApproverList(strMakerCheckerApproverType);
                strStationCode = Helper.GetCustomerDetails("StationCode", "UserList", " WHERE CustId = " + i + " and UserId = " + Id + " and rowstatus = 1");
                ViewBag.StationCode = strStationCode;

                var userlist = (from li in db.UserLists where li.UserId == Id && li.CustId == i select li).SingleOrDefault();
                ViewBag.userlist = userlist;

                var Role = (from li in db.UserLists where li.UserId == Id && li.CustId == i select li.RoleId).SingleOrDefault();
                ViewBag.Role = Role;


                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == Id && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList

                        };
                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == i && p.rowstatus == true
                               select new
                               {
                                   gstid = p.GSTINId,
                                   gstnum = p.GSTINNo
                               }).ToList();
                ViewBag.GSTINNumber = new SelectList(GSTINNo, "gstid", " gstnum");

                var Rolelist = from li in db.MAS_Roles
                               where li.CustomerID == i
                               select new
                               {
                                   RoleID = li.Role_ID,
                                   RoleName = li.Role_Name
                               };
                ViewBag.Role = new SelectList(Rolelist, "RoleID", "RoleName", ViewBag.Role);


                var GSTIN = (from p in db.TBL_Cust_GSTIN
                             where p.CustId == i && p.rowstatus == true
                             select new
                             {
                                 CategoryID = p.GSTINId,
                                 CategoryName = p.GSTINNo
                             }).ToList();
                ViewBag.Categories = new SelectList(GSTIN, "CategoryID", "CategoryName");

                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }



        [HttpPost]
        public ActionResult EditUser(FormCollection form, string command)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {
                //adding New GSTIN Number for User Updation

                if (command == "add")
                {
                    int UserID = Convert.ToInt32(Session["User_ID"]);
                    int cutid = Convert.ToInt32(Session["cid"]);
                    int userid = Convert.ToInt32(Session["uid"]);


                    var Email = (from li in db.UserLists
                                 where li.UserId == UserID
                                 select li.Email).FirstOrDefault();
                    var Mobile = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.MobileNo).FirstOrDefault();
                    var UserNo = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.UserId).FirstOrDefault();

                    if ((Email == form["email"] || Mobile == form["phone"]) && UserNo != userid)
                    {
                        TempData["msg"] = "User details already exists ";
                    }
                    else
                    {                        
                        UserList user = db.UserLists.Where(x => x.UserId == userid).SingleOrDefault();
                        user.Name = form["name"];
                        user.Designation = form["designation"];
                        user.Email = form["email"];
                        user.Password = form["Password"];
                        user.MobileNo = form["phone"];
                        user.RoleId = Convert.ToInt32(form["CategoryName"]);
                        user.ValidFrom = null;
                        user.ValidTo = null;
                        user.EsignedUser = null;
                        db.SaveChanges();
                                                
                        UserAccess_GSTIN usergst = new UserAccess_GSTIN();
                        string[] AllStrings = form["CategoryId"].Split(',');

                        foreach (var gst in AllStrings)
                        {
                            int gstloop = Convert.ToInt32(gst);
                            var gstnumber = (from li in db.UserAccess_GSTIN
                                             where li.GSTINId == gstloop && li.UserId == userid && li.CustId == cutid && li.Rowstatus == true
                                             select li.GSTINId).ToList();
                            if (gstnumber.Count > 0)
                            {
                                TempData["msg"] = "This GSTIN Number Already Exist";
                            }
                            else
                            {
                                usergst.GSTINId = int.Parse(gst);
                                usergst.CustId = cutid;
                                usergst.UserId = userid;
                                usergst.CreatedDate = DateTime.Now;
                                usergst.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                                usergst.Rowstatus = true;
                                db.UserAccess_GSTIN.Add(usergst);
                                db.SaveChanges();
                                TempData["msg"] = "Successfully Added New GSTIN Number For This User";
                            }
                        }
                    }

                }

                //delete User Accesed GSTIN Number

                else if (command == "delete")
                {
                    int id = Convert.ToInt32(form["gstid"]);
                    int uid = Convert.ToInt32(Session["User_ID"]);

                    int userid = Convert.ToInt32(Session["uid"]);
                    if (uid == userid)
                    {
                        TempData["msg"] = "You cannot deleted your Details";
                    }
                    else
                    {
                        var roleid = (from li in db.UserLists where li.UserId == userid select li.RoleId).SingleOrDefault();
                        var rolename = (from li in db.MAS_Roles where li.Role_ID == roleid select li.Role_Name).SingleOrDefault();
                        if (rolename == "Super Admin" || rolename == "Admin")
                        {
                            TempData["msg"] = "You donnot have access to delete admin or super admin details";

                            return RedirectToAction("Registration");
                        }
                        else
                        {
                            List<UserAccess_GSTIN> ugst = (from gst in db.UserAccess_GSTIN
                                                           where gst.GSTINId == id && gst.UserId == userid && gst.Rowstatus == true
                                                           select gst).ToList();
                            foreach (var a in ugst)
                            {
                                db.UserAccess_GSTIN.Remove(a);
                                db.SaveChanges();
                            }

                        }
                    }


                    TempData["msg"] = "Selected GSTIN Number is Deleted Successfully";
                }

                //Updating User Details
                else
                {
                    int userid = Convert.ToInt32(Session["uid"]);
                    int UserID = Convert.ToInt32(Session["User_ID"]);
                    var Email = (from li in db.UserLists
                                 where li.UserId == UserID
                                 select li.Email).FirstOrDefault();
                    var Mobile = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.MobileNo).FirstOrDefault();
                    var UserNo = (from li in db.UserLists
                                  where li.UserId == UserID
                                  select li.UserId).FirstOrDefault();

                    if ((Email == form["email"] || Mobile == form["phone"]) && UserNo != userid)
                    {
                        TempData["msg"] = "User details already exists ";
                    }
                    else
                    {
                        string strMakerCheckerApprover = "", strStationCode = "";
                        strMakerCheckerApprover = form["ddlMakerCheckerApprover"];
                        strStationCode = form["stationcode"];
                        UserList user = db.UserLists.Where(x => x.UserId == userid).SingleOrDefault();
                        user.Name = form["name"];
                        user.Designation = form["designation"];
                        user.Email = form["email"];
                        user.Password = form["Password"];
                        user.MobileNo = form["phone"];
                        user.RoleId = Convert.ToInt32(form["CategoryName"]);
                        user.ValidFrom = null;
                        user.ValidTo = null;
                        user.EsignedUser = null;
                        db.SaveChanges();

                        // Updating MakerCheckerApprover in UserList Table
                        if (!string.IsNullOrEmpty(strMakerCheckerApprover))
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                            {
                                conn.Open();
                                SqlCommand sqlcmd = new SqlCommand();
                                sqlcmd.Parameters.Clear();
                                sqlcmd.Parameters.Add("@MakerCheckerApproverType", SqlDbType.NVarChar).Value = strMakerCheckerApprover;
                                Functions.UpdateTable("UserList", "UserId", userid.ToString(), sqlcmd, conn);
                                conn.Close();
                            }
                        }

                        // Updating Station Code in UserList Table
                        if (!string.IsNullOrEmpty(strStationCode))
                        {
                            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                            {
                                conn.Open();
                                SqlCommand sqlcmd = new SqlCommand();
                                sqlcmd.Parameters.Clear();
                                sqlcmd.Parameters.Add("@StationCode", SqlDbType.NVarChar).Value = strStationCode;
                                Functions.UpdateTable("UserList", "UserId", userid.ToString(), sqlcmd, conn);
                                conn.Close();
                            }
                        }

                        TempData["msg"] = "User Updated Successfully";
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Updated", "");
                    }

                }
                return RedirectToAction("Registration");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                int i = Convert.ToInt32(Session["Cust_ID"]);
                var gstinlist = from li in db.TBL_Cust_GSTIN
                                where li.CustId == i && li.rowstatus == true
                                select new { li.GSTINId, li.GSTINNo };
                ViewBag.gstin = gstinlist.ToList();
                return RedirectToAction("Registration");
            }
        }

        public int DeleteUser(int Id)
        {
            int status = 0;

            int uid = Convert.ToInt32(Session["User_ID"]);
            int cid = Convert.ToInt32(Session["Cust_ID"]);
            var username = (from li in db.UserLists
                            where li.UserId == Id
                            select li.Username).FirstOrDefault();
            if (uid == Id)
            {
                status = 1;

                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User " + username + " own access deletion is failure.", "");
            }

            else
            {
                var roleid = (from li in db.UserLists where li.UserId == Id select li.RoleId).SingleOrDefault();
                var rolename = (from li in db.MAS_Roles where li.Role_ID == roleid select li.Role_Name).SingleOrDefault();
                if (rolename == "Super Admin" || rolename == "Admin")
                {
                    status = 2;

                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User " + username + " deletion is failure, because this user is Admin/Super Admin.", "");
                }
                else
                {
                    var user = db.UserLists.FirstOrDefault(u => u.UserId == Id);
                    user.rowstatus = false;
                    db.SaveChanges();

                    var ugst = (from gst in db.UserAccess_GSTIN
                                where gst.UserId == Id && gst.CustId == cid
                                select gst.UserAccessId).ToList();
                    foreach (int a in ugst)
                    {
                        var useraceess = db.UserAccess_GSTIN.FirstOrDefault(u => u.UserAccessId == a);
                        useraceess.Rowstatus = false;
                        db.SaveChanges();
                    }
                    status = 3;
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Deleted", "");

                }
            }

            return status;
        }

        public ActionResult Updation(string option, string search)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            try
            {

                int uid = Convert.ToInt32(Session["Cust_ID"]);

                string op = option;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "User Name",
                    Value = "User Name"
                });
                items.Add(new SelectListItem
                {
                    Text = "Designation",
                    Value = "Designation"
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
                ViewBag.Category = new SelectList(items, "Text", "Value", op);

                if (option == "User Name")
                {
                    List<UserList> username = (from li in db.UserLists
                                               where li.CustId == uid && li.rowstatus == true && (li.Name.Contains(search) || li.Name.StartsWith(search) || li.Name == search || search == null)
                                               select li).ToList();
                    return View(username);
                }

                else if (option == "Designation")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == uid && li.rowstatus == true && (li.Designation.Contains(search) || li.Designation.StartsWith(search) || li.Designation == search || search == null)
                                                  select li).ToList();
                    return View(Designation);
                }

                else if (option == "Email ID")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == uid && li.rowstatus == true && (li.Email.Contains(search) || li.Email.StartsWith(search) || li.Email == search || search == null)
                                                  select li).ToList();
                    return View(Designation);
                }

                else if (option == "Mobile Number")
                {
                    List<UserList> Designation = (from li in db.UserLists
                                                  where li.CustId == uid && li.rowstatus == true && (li.MobileNo.Contains(search) || li.MobileNo.StartsWith(search) || li.MobileNo == search || search == null)
                                                  select li).ToList();
                    return View(Designation);
                }
                List<UserList> userlist = (from user in db.UserLists
                                           where user.CustId == uid && user.rowstatus == true
                                           select user).ToList();
                return View(userlist.AsEnumerable());
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult Block(int id)
        {
            try
            {

                int cid = Convert.ToInt32(Session["Cust_ID"]);
                int uid = Convert.ToInt32(Session["User_ID"]);

                if (uid == id)
                {
                    TempData["msg"] = "You cannot Block your Details";
                }

                else
                {
                    var roleid = (from li in db.UserLists where li.UserId == id select li.RoleId).SingleOrDefault();
                    var rolename = (from li in db.MAS_Roles where li.Role_ID == roleid select li.Role_Name).SingleOrDefault();
                    if (rolename == "Super Admin" || rolename == "Admin")
                    {
                        TempData["msg"] = "You donnot have access to delete admin or super admin details";
                    }
                    else
                    {
                        db.Update_User_Block_Unblock(cid, id, 0, uid);
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Blocked", "");
                        TempData["msg"] = "User Blocked Successfully";
                    }
                }
                return RedirectToAction("Updation");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult UnBlock(int id)
        {
            try
            {
                int cid = Convert.ToInt32(Session["Cust_ID"]);
                int uid = Convert.ToInt32(Session["User_ID"]);
                db.Update_User_Block_Unblock(cid, id, 1, uid);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User UnBlocked", "");
                TempData["msg"] = "User UnBlocked Successfully";
                return RedirectToAction("Updation");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

    }

}