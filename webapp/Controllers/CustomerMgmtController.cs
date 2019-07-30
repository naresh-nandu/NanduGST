#region Using

using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.IO;
using System.Web.UI;
using System.Net.Mail;
using System.Net;
using System.Text;
using SmartAdminMvc.Models.Common;

#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class CustomerMgmtController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        public ActionResult Updation(string option, string search)
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
                    Text = "Customer Name",
                    Value = "Customer Name"
                });
                items.Add(new SelectListItem
                {
                    Text = "GSTIN Number",
                    Value = "GSTIN Number"
                });
                items.Add(new SelectListItem
                {
                    Text = "Company",
                    Value = "Company"
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

                ViewBag.Categories = new SelectList(items, "Text", "Value", op);

                if (option == "Customer Name")
                {

                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.Name.Contains(search) || li.Name.StartsWith(search) || li.Name == search || search == null)
                                               select li).ToList();
                    return View(cust);



                }

                else if (option == "GSTIN Number")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.GSTINNo.Contains(search) || li.GSTINNo.StartsWith(search) || li.GSTINNo == search || search == null)
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Email ID")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.Email.Contains(search) || li.Email.StartsWith(search) || li.Email == search || search == null)
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Mobile Number")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.MobileNo.Contains(search) || li.MobileNo.StartsWith(search) || li.MobileNo == search || search == null)
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Designation")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.Designation.Contains(search) || li.Designation.StartsWith(search) || li.Designation == search || search == null)
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Company")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where li.StatusCode == 1 && (li.Company.Contains(search) || li.Company.StartsWith(search) || li.Company == search || search == null)
                                               select li).ToList();

                    return View(cust);
                }

                List<TBL_Customer> customer = (from cust in db.TBL_Customer
                                               where cust.StatusCode == 1
                                               select cust).ToList();
                return View(customer);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public ActionResult Verification(string option, string search)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                if (!Request.IsAuthenticated)
                {
                    return RedirectToAction("Login", "Account");
                }
                if (Session["User_ID"] == null)
                    return RedirectToAction("Login", "Account");

                string op = option;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "Customer Name",
                    Value = "Customer Name"
                });
                items.Add(new SelectListItem
                {
                    Text = "GSTIN Number",
                    Value = "GSTIN Number"
                });
                items.Add(new SelectListItem
                {
                    Text = "Company",
                    Value = "Company"
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

                ViewBag.Categories = new SelectList(items, "Text", "Value", op);


                if (option == "Customer Name")
                {

                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.Name.Contains(search) || li.Name.StartsWith(search) || li.Name == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();
                    return View(cust);



                }

                else if (option == "GSTIN Number")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.GSTINNo.Contains(search) || li.GSTINNo.StartsWith(search) || li.GSTINNo == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Email ID")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.Email.Contains(search) || li.Email.StartsWith(search) || li.Email == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();

                    return View(cust);
                }
                else if (option == "Mobile Number")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.MobileNo.Contains(search) || li.MobileNo.StartsWith(search) || li.MobileNo == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();

                    return View(cust);
                }
                else if (option == "Company")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.Company.Contains(search) || li.Company.StartsWith(search) || li.Company == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();

                    return View(cust);
                }

                else if (option == "Designation")
                {
                    List<TBL_Customer> cust = (from li in db.TBL_Customer
                                               where (li.Designation.Contains(search) || li.Designation.StartsWith(search) || li.Designation == search || search == null)
                                               orderby li.CustId descending
                                               select li).ToList();

                    return View(cust);
                }

                List<TBL_Customer> customer = (from cust in db.TBL_Customer
                                               orderby cust.CustId descending
                                               select cust).ToList();

                return View(customer);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]

        public ActionResult Approve(int? id, string command)
        {
            try
            {
                if (id == null)
                {
                    TempData["msg"] = "Please Select Customer to Perform this Operation?";
                }
                else
                {
                    Session["uid"] = id;
                    int uid = Convert.ToInt32(Session["uid"]);
                    if (command == "Verify Customer")
                    {
                        var user = db.TBL_Customer.FirstOrDefault(u => u.CustId == id);
                        user.StatusCode = 5;
                        db.SaveChanges();

                        TBL_Customer ul = new TBL_Customer();
                        ul.Email = (from li in db.TBL_Customer
                                    where li.CustId == uid
                                    select li.Email).SingleOrDefault();
                        ul.MobileNo = (from li in db.TBL_Customer
                                       where li.CustId == uid
                                       select li.MobileNo).SingleOrDefault();
                        ul.Name = (from li in db.TBL_Customer
                                   where li.CustId == uid
                                   select li.Name).SingleOrDefault();

                        Notification.SendSMS(ul.MobileNo.ToString(), "Your Documents is successfully verified By WeP Administrator.");
                        Notification.SendEmail(ul.Email, "Regarding WeP GST Panel.", "Your Documents is successfully verified By WeP Administrator.");

                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Documents Verified: " + ul.Name, "");

                        TempData["msg"] = "Customer Documents is Verified Successfully";

                    }
                    else if (command == "Reject Customer")
                    {
                        var user = db.TBL_Customer.FirstOrDefault(u => u.CustId == id);
                        user.StatusCode = 6;
                        db.SaveChanges();

                        TBL_Customer ul = new TBL_Customer();
                        ul.Email = (from li in db.TBL_Customer
                                    where li.CustId == uid
                                    select li.Email).SingleOrDefault();
                        ul.MobileNo = (from li in db.TBL_Customer
                                       where li.CustId == uid
                                       select li.MobileNo).SingleOrDefault();
                        ul.Name = (from li in db.TBL_Customer
                                   where li.CustId == uid
                                   select li.Name).SingleOrDefault();
                        Notification.SendSMS(ul.MobileNo.ToString(), "Your are Rejected By Wep Administrator due to improper / Incomplete documents.Please Contact WeP Administrator.");
                        Notification.SendEmail(ul.Email, "" + "Regarding WeP GST Panel.", "Your are Rejected By Wep Administrator due to improper / Incomplete documents.Please Contact WeP Administrator.");

                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Rejected :" + ul.Name, "");

                        TempData["msg"] = "Customer Rejected Successfully";

                    }
                }
                return RedirectToAction("Verification");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public ActionResult Block(int Id)
        {
            try
            {
                int cid = Convert.ToInt32(Session["Cust_ID"]);
                string Name = (from li in db.TBL_Customer
                               where li.CustId == cid
                               select li.Name).SingleOrDefault().ToString();
                db.Update_Customer_Block_Unblock(Id, 0, cid);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Blocked :" + Name, "");
                TempData["msg"] = "Customer Blocked Successfully";
                return RedirectToAction("Updation");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult UnBlock(int Id)
        {
            try
            {
                int cid = Convert.ToInt32(Session["Cust_ID"]);
                string Name = (from li in db.TBL_Customer
                               where li.CustId == cid
                               select li.Name).SingleOrDefault().ToString();
                db.Update_Customer_Block_Unblock(Id, 1, cid);
                db.SaveChanges();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer UnBlocked :" + Name, "");
                TempData["msg"] = "Customer UnBlocked Successfully";
                return RedirectToAction("Updation");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public ActionResult CustomerDocuments()
        {
            int id = Convert.ToInt32(Request.QueryString["SesCustDocsId"]);

            Session["SesCustDocsId"] = id;
            return View();
        }

        [ActionName("FileDownload")]
        public FileContentResult FileDownload(int id)
        {
            try
            {
                byte[] fileData;
                string fileName;
                //create object of LINQ to SQL class
                WePGSPDBEntities dataContext = new WePGSPDBEntities();
                //using LINQ expression to get record from database for given id value
                var record = from p in dataContext.TBL_Cust_Docs
                             where p.CustomerId == id
                             select p;
                //only one record will be returned from database as expression uses condtion on primary field
                //so get first record from returned values and retrive file content (binary) and filename
                fileData = record.First().FileData.ToArray();
                fileName = record.First().FileName;
                //return file and provide byte file content and file name
                return File(fileData, "text", fileName);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return this.ViewBag.ex;

            }
            //declare byte array to get file content from database and string to store file name

        }



        public FileContentResult download(int id)
        {
            var attachment = db.TBL_Cust_Docs.First(a => a.CustomerId == id); //extract file record from db

            var fattach = new FileContentResult(attachment.FileData.ToArray(), "application/octet-stream");
            fattach.FileDownloadName = attachment.FileName;  // I had to use this for download if I didn't it will open the file
            return fattach;
        }
    }



}