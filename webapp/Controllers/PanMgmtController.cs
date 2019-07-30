using System.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Data;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models;
using System.Net;
using SmartAdminMvc.Models.PANMgmt;

namespace SmartAdminMvc.Controllers
{
    public class PanMgmtController : Controller
    {
        readonly WePGSPDBEntities db = new WePGSPDBEntities();
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        readonly PanFunctions PANSP = new PanFunctions();
        
        // GET: PanMgmt
        public ActionResult Registration(string option, string search)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);

            string op = option;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "PAN",
                Value = "PAN"
            });
            items.Add(new SelectListItem
            {
                Text = "Company Name",
                Value = "Company Name"
            });

            ViewBag.Search = new SelectList(items, "Text", "Value", op);

            if (option == "PAN")
            {
                var panlst = (from li in db.TBL_Cust_PAN where li.CustId == custid && li.rowstatus == true && (li.PANNo.Contains(search) || li.PANNo.StartsWith(search) || li.PANNo == search || search == null) select li).ToList();
                ViewBag.panlist = panlst;
                return View();
            }
            else if (option == "Company Name")
            {
                var panlst = (from li in db.TBL_Cust_PAN where li.CustId == custid && li.rowstatus == true && (li.CompanyName.Contains(search) || li.CompanyName.StartsWith(search) || li.CompanyName == search || search == null) select li).ToList();
                ViewBag.panlist = panlst;
                return View();
            }

            var panlist = (from li in db.TBL_Cust_PAN where li.CustId == custid && li.rowstatus == true select li).ToList();
            ViewBag.panlist = panlist;
            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Registration(FormCollection form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            TBL_Cust_PAN li = new TBL_Cust_PAN();
            string pan = form["pan"];
            var pannumber = (from list in db.TBL_Cust_PAN
                             where list.PANNo == pan && list.rowstatus == true
                             select list);

            if (pannumber.Count() > 0)
            {
                TempData["msg"] = "PAN already exists";
            }
            else
            {
                li.PANNo = pan.ToUpper();
                li.CustId = Convert.ToInt32(Session["Cust_ID"]);
                li.CreatedBy = Convert.ToInt32(Session["User_ID"]);
                li.CreatedDate = DateTime.Now;
                li.rowstatus = true;
                li.CompanyName = form["companyname"];
                db.TBL_Cust_PAN.Add(li);
                db.SaveChanges();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "PAN Number : " + li.PANNo + "is added", "");
                int userid = Convert.ToInt32(Session["User_ID"]);
                var Mobileno = (from ul in db.UserLists where ul.UserId == userid select ul.MobileNo).FirstOrDefault();
                var Email = (from ul in db.UserLists where ul.UserId == userid select ul.Email).FirstOrDefault();
                Notification.SendSMS(Mobileno.ToString(), "Your New PAN Number " + pan.ToUpper() + " is registered Successfully.");
                Notification.SendEmail(Email, "Regarding to New PAN Number Registered.", "Your New PAN Number " + pan.ToUpper() + " is registred Successfully.");
                var panid = (from ul in db.TBL_Cust_PAN where ul.PANNo == pan select ul.PANId);
                int panID = (from ul in db.TBL_Cust_PAN where ul.PANNo == pan select ul.PANId).FirstOrDefault();
                if (panid.Count() > 0)
                {
                    con.Open();
                    if (Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase attachments = Request.Files;
                        for (int i = 0; i < attachments.Count; i++)
                        {
                            HttpPostedFileBase attachment = attachments[i];
                            if (attachment.ContentLength > 0 && !String.IsNullOrEmpty(attachment.FileName))
                            {
                                string ext = Path.GetExtension(attachment.FileName);                      // getting the file extension of uploaded file
                                string type = String.Empty;
                                switch (ext)                                         // this switch code validate the files which allow to upload only PDF  file 
                                {
                                    case ".pdf":
                                        type = "application/pdf";
                                        break;

                                }
                                //do your file saving or any related tasks here.
                                if (type != String.Empty)
                                {

                                    Stream fs = attachment.InputStream;
                                    BinaryReader br = new BinaryReader(fs);                                 //reads the   binary files
                                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);                           //counting the file length into bytes

                                    SqlCommand com = new SqlCommand();
                                    com.Parameters.Clear();
                                    com.Parameters.Add("@FileName", SqlDbType.VarChar).Value = attachment.FileName;
                                    com.Parameters.Add("@FileContentType", SqlDbType.VarChar).Value = type;
                                    com.Parameters.Add("@FileData", SqlDbType.Binary).Value = bytes;
                                    com.Parameters.Add("@PANId", SqlDbType.Int).Value = panID;
                                    com.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);
                                    com.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"]);
                                    com.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    com.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = true;
                                    Functions.InsertIntoTable("TBL_Cust_PAN_Docs", com, con);
                                }
                                else
                                {
                                    // if file is other than speified extension 
                                }
                            }
                        }
                    }
                    con.Close();
                }
                TempData["msg"] = "PAN details saved Successfully";
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            var panlist = (from ul in db.TBL_Cust_PAN where ul.CustId == custid && ul.rowstatus == true select ul).ToList();
            ViewBag.panlist = panlist;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "PAN",
                Value = "PAN"
            });
            items.Add(new SelectListItem
            {
                Text = "Company Name",
                Value = "Company Name"
            });

            ViewBag.Search = new SelectList(items, "Text", "Value");
            return View();
        }

        public ActionResult PanDocuments()
        {
            int id = Convert.ToInt32(Request.QueryString["SesPanDocsId"]);

            Session["SesPanDocsId"] = id;
            return View();
        }


        public ActionResult Delete(int Id)
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            var masterpan = (from li in db.TBL_Customer where li.CustId == custid select li.PANNo).FirstOrDefault();
            var panno = (from li in db.TBL_Cust_PAN where li.PANId == Id select li.PANNo).FirstOrDefault();
            if (masterpan == panno)
            {
                TempData["msg"] = "Master PAN You Can not Deleted";
            }
            else
            {

                var pan = (from li in db.TBL_Cust_PAN where li.PANId == Id select li.PANNo).SingleOrDefault();
                int OutputVal = PANSP.PANDelete(Id);
                if (OutputVal == 1)
                {
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "PAN Number : " + pan + "is deleted", "");
                    TempData["msg"] = "PAN and its related documents,GSTIN are Deleted Successfully";
                }
                else
                {
                    TempData["msg"] = "Something went Wrong,Operation failure";
                }
            }
            return RedirectToAction("Registration");
        }

        [HttpGet]
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_Cust_PAN pan = db.TBL_Cust_PAN.Find(Id);
            if (pan == null)
            {
                return HttpNotFound();
            }
            var pandocid = (from li in db.TBL_Cust_PAN_Docs where li.PANId == Id select li.PANdocId).ToList();
            foreach (int docid in pandocid)
            {
                var pandoc = (from li in db.TBL_Cust_PAN_Docs where li.PANdocId == docid select li.FileName).ToList();
                ViewBag.pan = pandoc;
            }
            ViewBag.Pandoc = ViewBag.pan;
            return View(pan);
        }

        public ActionResult Edit(int id, FormCollection form)
        {

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            string pannuber = form["PANNo"].ToUpper();
            var panno = (from li in db.TBL_Cust_PAN
                         where li.CustId == custid && li.PANNo == pannuber
                         select li.PANNo).ToList();
            var panid = (from li in db.TBL_Cust_PAN
                         where li.CustId == custid && li.PANNo == pannuber
                         select li.PANId).FirstOrDefault();
            int pid = Convert.ToInt32(panid);


            if (panno.Count > 0 && id != pid)
            {
                TempData["msg"] = "PAN details already exists ";
            }

            else
            {
                TBL_Cust_PAN pan = db.TBL_Cust_PAN.Where(u => u.PANId == id).SingleOrDefault();
                pan.PANNo = form["PANNo"].ToUpper();
                pan.CompanyName = form["CompanyName"];
                pan.ModifiedBy = Convert.ToInt32(Session["User_ID"]);
                pan.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                if (Request.Files.Count > 0)
                {

                    var DocId = (from li in db.TBL_Cust_PAN_Docs where li.PANId == id select li.PANdocId).ToList();
                    foreach (int docid in DocId)
                    {
                        var pandoc = db.TBL_Cust_PAN_Docs.FirstOrDefault(u => u.PANdocId == docid);
                        pandoc.rowstatus = false;
                        db.SaveChanges();
                    }


                    con.Open();
                    if (Request.Files.Count > 0)
                    {
                        HttpFileCollectionBase attachments = Request.Files;
                        for (int i = 0; i < attachments.Count; i++)
                        {
                            HttpPostedFileBase attachment = attachments[i];
                            if (attachment.ContentLength > 0 && !String.IsNullOrEmpty(attachment.FileName))
                            {
                                string ext = Path.GetExtension(attachment.FileName);                      // getting the file extension of uploaded file
                                string type = String.Empty;
                                switch (ext)                                         // this switch code validate the files which allow to upload only PDF  file 
                                {
                                    case ".pdf":
                                        type = "application/pdf";
                                        break;
                                }
                                //do your file saving or any related tasks here.
                                if (type != String.Empty)
                                {
                                    Stream fs = attachment.InputStream;
                                    BinaryReader br = new BinaryReader(fs);                                 //reads the   binary files
                                    Byte[] bytes = br.ReadBytes((Int32)fs.Length);                           //counting the file length into bytes
                                    
                                    SqlCommand com = new SqlCommand();
                                    com.Parameters.Clear();
                                    com.Parameters.Add("@FileName", SqlDbType.VarChar).Value = attachment.FileName;
                                    com.Parameters.Add("@FileContentType", SqlDbType.VarChar).Value = type;
                                    com.Parameters.Add("@FileData", SqlDbType.Binary).Value = bytes;
                                    com.Parameters.Add("@PANId", SqlDbType.Int).Value = id;
                                    com.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);
                                    com.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_ID"]);
                                    com.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                    com.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = true;
                                    Functions.InsertIntoTable("TBL_Cust_PAN_Docs", com, con);
                                }
                                else
                                {
                                    // if file is other than speified extension 
                                }
                            }
                        }

                        con.Close();
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "PAN Number : " + pan.PANNo + "is updated ", "");
                    TempData["msg"] = "PAN Updated Successfully";
                }
            }
            return RedirectToAction("Registration");
        }
    }
}