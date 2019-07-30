#region Using

using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data.Entity.Core.Objects;

#endregion

namespace SmartAdminMvc.Controllers
{
    [Authorize]
    public class TablesController : Controller
    {

        WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: tables/normal
        public ActionResult Normal()
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int i = Convert.ToInt32(Session["Cust_ID"]);
            var Rolelist = from li in db.MAS_Roles
                           where li.CustomerID ==i 
                           select new { li.Role_ID, li.Role_Name };
            ViewBag.Role = Rolelist.ToList();

            
            var categories = db.TBL_Cust_GSTIN.Where(m=>m.CustId==i).Select(c => new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();
        }
        [HttpPost]
        public ActionResult Normal(FormCollection form)
        {
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            UserList user = new UserList();
            user.CustId = Convert.ToInt32(Session["Cust_ID"]);
            user.Name = form["name"];
            user.Designation = form["designation"];
            user.Username = form["username"];
            user.Password = form["Password"];
            user.Email = form["email"];
            user.MobileNo = form["phone"];
            user.RoleId = Convert.ToInt32(form["Role_Name"]);
            user.ValidFrom = null;
            user.ValidTo = null;
            user.CreatedBy = Convert.ToInt32(Session["Cust_ID"]);
            user.EsignedUser = null;
            string id = form["CategoryId"];
            using (WePGSPDBEntities context = new WePGSPDBEntities())
            {
                ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
                ObjectParameter op1 = new ObjectParameter("UserId", typeof(int));

                var query = context.Ins_UserList(user.CustId, user.Name, user.Designation, user.Username, user.Password, user.Email, user.MobileNo, user.RoleId, user.ValidFrom, user.ValidTo, user.CreatedBy, user.EsignedUser, op, op1);



                var value = op.Value; //Get the output value.
                var value1 = op1.Value;
                if ((int)value == 1)
                {
                    context.SaveChanges();
                    UserAccess_GSTIN usergst = new UserAccess_GSTIN();
                    string[] AllStrings = form["CategoryId"].Split(',');
                    
                    foreach (var gst in AllStrings)
                    {
                        usergst.GSTINId = int.Parse(gst);
                        usergst.CustId = Convert.ToInt32(Session["Cust_ID"]);                        
                        usergst.UserId =(int)value1 ;
                        usergst.CreatedDate = DateTime.Now;
                        usergst.CreatedBy= Convert.ToInt32(Session["User_ID"]);
                        usergst.Rowstatus =true;
                    context.UserAccess_GSTIN.Add(usergst);
                    context.SaveChanges();
                    }
                }
            }
            int i = Convert.ToInt32(Session["Cust_ID"]);
            var Rolelist = from li in db.MAS_Roles
                           where li.CustomerID == i
                           select new { li.Role_ID, li.Role_Name };
            ViewBag.Role = Rolelist.ToList();


            var categories = db.TBL_Cust_GSTIN.Where(m => m.CustId == i).Select(c => new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();
        }

        // GET: tables/data-tables
        public ActionResult DataTables()
        {
            int i = Convert.ToInt32(Session["Cust_ID"]);
            var gstinlist = from li in db.TBL_Cust_GSTIN
                           where li.CustId == i
                           select new { li.GSTINId, li.GSTINNo };
            ViewBag.gstin = gstinlist.ToList();

            return View();
        }

        // GET: tables/jq-grid
        public ActionResult JQGrid()
        {
            int uid = Convert.ToInt32(Session["Cust_ID"]);
            List<UserList> userlist = (from user in db.UserLists
                                       where user.CustId == uid
                                       select user).ToList();
            return View(userlist.AsEnumerable());
        }

        public ActionResult Block(int id)
        {
            int cid =Convert.ToInt32(Session["Cust_ID"]);
            int uid = Convert.ToInt32(Session["User_ID"]);
            db.Update_User_Block_Unblock(cid,id,0,uid);
            return RedirectToAction("JQGrid");
        }

        public ActionResult UnBlock(int id)
        {
            int cid = Convert.ToInt32(Session["Cust_ID"]);
            int uid = Convert.ToInt32(Session["User_ID"]);
            db.Update_User_Block_Unblock(cid, uid, 1, uid);
            return RedirectToAction("JQGrid");
        }

        [HttpGet]
        public ActionResult getData(int id)
        {
            Session["sid"] = id;
            
            var v = from p in db.TBL_Cust_GSTIN
                    where (p.GSTINId == id)
                    select new 
                    {
                       p.TBL_Customer.UserLists
                    };
            List<UserList> listtable = new List<UserList>();
            foreach (var p in v)
            {
                
                foreach (var a in p.UserLists) {
                    UserList list = new UserList();
                    list.CustId = a.CustId;
                    list.UserId = a.UserId;
                    list.Name = a.Name;
                    list.Designation = a.Designation;
                    list.Email = a.Email;
                    list.MobileNo = a.MobileNo;
                    list.Status = a.Status;
                    listtable.Add(list);
                    }
            }
            return Json(listtable, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult UserEdit(int custid,int userid)
        {
            Session["uid"] = userid;
            int i = Convert.ToInt32(Session["Cust_ID"]);
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
                              select new
                              {
                                  CategoryID= p.GSTINId,
                                  CategoryName= p.GSTINNo
                              }).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();
        }

        [HttpPost]
        public ActionResult UserEdit(FormCollection form)
        {
            int userid = Convert.ToInt32(Session["uid"]);
            UserList user = db.UserLists.Where(x=>x.UserId==userid).SingleOrDefault();
            
            user.Name = form["name"];
            user.Designation = form["designation"];
            user.Username = form["username"];
            user.Password = form["Password"];
            user.Email = form["email"];
            user.MobileNo = form["phone"];
            user.RoleId = Convert.ToInt32(form["Role_Name"]);
            user.ValidFrom = null;
            user.ValidTo = null;
           // user.CreatedBy = Convert.ToInt32(Session["Cust_ID"]);
            user.EsignedUser = null;
            db.SaveChanges();
            //  string id = form["CategoryId"];
            //using (WePGSPDBEntities1 context = new WePGSPDBEntities1())
            //{
            //    ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
            //    ObjectParameter op1 = new ObjectParameter("UserId", typeof(int));

            //    var query = context.Ins_UserList(user.CustId, user.Name, user.Designation, user.Username, user.Password, user.Email, user.MobileNo, user.RoleId, user.ValidFrom, user.ValidTo, user.CreatedBy, user.EsignedUser, op, op1);



            //    var value = op.Value; //Get the output value.
            //    var value1 = op1.Value;
            //if ((int)value == 1)
            //{
            //    context.SaveChanges();
            //    UserAccess_GSTIN usergst = new UserAccess_GSTIN();
            //    string[] AllStrings = form["CategoryId"].Split(',');

            //    foreach (var gst in AllStrings)
            //    {
            //        usergst.GSTINId = int.Parse(gst);
            //        usergst.CustId = Convert.ToInt32(Session["Cust_ID"]);
            //        usergst.UserId = (int)value1;
            //        usergst.CreatedDate = DateTime.Now;
            //        usergst.CreatedBy = Convert.ToInt32(Session["User_ID"]);
            //        usergst.Rowstatus = true;
            //        context.UserAccess_GSTIN.Add(usergst);
            //        context.SaveChanges();
            //    }
            //}
            // }

            int i = Convert.ToInt32(Session["Cust_ID"]);
            var Rolelist = from li in db.MAS_Roles
                           where li.CustomerID == i
                           select new { li.Role_ID, li.Role_Name };
            ViewBag.Role = Rolelist.ToList();

            return View();
        }

        public ActionResult UserDelete(int? userid)
        {

            UserList li = db.UserLists.Find(userid);
            
           List< UserAccess_GSTIN> ugst = (from gst in db.UserAccess_GSTIN
                                     where gst.UserId == userid
                                     select gst).ToList();
            foreach(var a in ugst)
            {
                db.UserAccess_GSTIN.Remove(a);
                db.SaveChanges();
            }
            db.UserLists.Remove(li);
            db.SaveChanges();
            return RedirectToAction("JQGrid","Tables");
        }
        [HttpGet]
        public ActionResult AddUser(int gstin)
        {
            int i = Convert.ToInt32(Session["Cust_ID"]);
            var Rolelist = from li in db.MAS_Roles
                           where li.CustomerID == i
                           select new { li.Role_ID, li.Role_Name };
            ViewBag.Role = Rolelist.ToList();


            var categories = db.TBL_Cust_GSTIN.Where(m => m.GSTINId == gstin).Select(c => new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();
        }

        [HttpPost]
        public ActionResult AddUser(int? gstin,FormCollection form)
        {
            UserList user = new UserList();
            user.CustId = Convert.ToInt32(Session["Cust_ID"]);
            user.Name = form["name"];
            user.Designation = form["designation"];
            user.Username = form["username"];
            user.Password = form["Password"];
            user.Email = form["email"];
            user.MobileNo = form["phone"];
            user.RoleId = Convert.ToInt32(form["Role_Name"]);
            user.ValidFrom = null;
            user.ValidTo = null;
            user.CreatedBy = Convert.ToInt32(Session["Cust_ID"]);
            user.EsignedUser = null;
            string id = form["CategoryId"];
            using (WePGSPDBEntities context = new WePGSPDBEntities())
            {
                ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
                ObjectParameter op1 = new ObjectParameter("UserId", typeof(int));

                var query = context.Ins_UserList(user.CustId, user.Name, user.Designation, user.Username, user.Password, user.Email, user.MobileNo, user.RoleId, user.ValidFrom, user.ValidTo, user.CreatedBy, user.EsignedUser, op, op1);



                var value = op.Value; //Get the output value.
                var value1 = op1.Value;
                if ((int)value == 1)
                {
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
                    }
                }
            }
            int i = Convert.ToInt32(Session["Cust_ID"]);
            var Rolelist = from li in db.MAS_Roles
                           where li.CustomerID == i
                           select new { li.Role_ID, li.Role_Name };
            ViewBag.Role = Rolelist.ToList();


            var categories = db.TBL_Cust_GSTIN.Where(m => m.GSTINId == gstin).Select(c => new {
                CategoryID = c.GSTINId,
                CategoryName = c.GSTINNo
            }).ToList();
            ViewBag.Categories = new MultiSelectList(categories, "CategoryID", "CategoryName");

            return View();

        }
    }
}