using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Net;
using System.Data.Entity;
using System.Data;
using System.IO;
using System.Text.RegularExpressions;
using System.Data.SqlClient;
using System.Configuration;
using SmartAdminMvc.Models.SearchTaxPayer;
using System.Web.UI.WebControls;
using System.Web.UI;

namespace SmartAdminMvc.Controllers
{
    public class BuyerMgmtController : Controller
    {

        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: Buyer
        public ActionResult Index()
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            var buyerDetails = (from buyer in db.TBL_Buyer
                                where buyer.CustomerId == custid && buyer.RowStatus == true
                                select buyer).ToList();
            ViewBag.Custmerlist = buyerDetails;

            return View();

        }

        [HttpPost]
        public ActionResult Index(FormCollection frm, string Create, string Export)
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
                string username = Session["UserName"].ToString();

                var GSTIN = (from li in db.TBL_Buyer
                             where (li.CustomerId == custid) && (li.RowStatus == true)
                             select li.GSTINno).FirstOrDefault();
                var Email = (from li in db.TBL_Buyer
                             where (li.CustomerId == custid) && (li.RowStatus == true)
                             select li.EmailId).FirstOrDefault();
                var Mobile = (from li in db.TBL_Buyer
                              where (li.CustomerId == custid) && (li.RowStatus == true)
                              select li.MobileNo).FirstOrDefault();

                

                if (!string.IsNullOrEmpty(Create))
                {

                    if (GSTIN == frm["gstin"] || Email == frm["email"] || Mobile == frm["mob"])
                    {
                        TempData["ErrorMessage"] = "Customer details already exists ";
                    }
                    else
                    {

                        TBL_Buyer buyer = new TBL_Buyer();
                        buyer.BuyerName = frm["name"];
                        buyer.POCName = frm["POC"];
                        buyer.NatureOfBusiness = frm["NOB"];
                        buyer.EmailId = frm["email"];
                        buyer.MobileNo = frm["mob"];
                        buyer.GSTINno = frm["gstin"].ToUpper();
                        buyer.StateCode = frm["statecode"];
                        buyer.PANNO = frm["PAN"].ToUpper();
                        buyer.DateofCompRegistered = frm["DOR"];
                        buyer.ConstitutionOfBusiness = frm["COB"];
                        buyer.Address = frm["address"];
                        buyer.CustomerId = custid;
                        buyer.CreatedDate = DateTime.Now;
                        buyer.LastmodifiedBy = null;
                        buyer.LastModifiedDate = null;
                        buyer.RowStatus = true;

                        db.TBL_Buyer.Add(buyer);

                        int j = db.SaveChanges();
                        if (j > 0)
                        {
                            TempData["Message"] = "Customer Registered Successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Created : " + buyer.BuyerName, "");
                        }
                    }
                }

                else if (!string.IsNullOrEmpty(Export))
                {
                   // var count = 0;
                    var buyerDetails1 = (from buyer in db.TBL_Buyer
                                        where buyer.CustomerId == custid && buyer.RowStatus == true
                                         select new { buyer.BuyerName, buyer.EmailId,buyer.MobileNo,buyer.GSTINno,buyer.Address}).ToList();
                    
                    ViewBag.Custmerlist1 = buyerDetails1;

                    GridView gv = new GridView();
                    gv.DataSource = ViewBag.Custmerlist1;
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=CustomerManagement.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                   
                }

                else
                {
                    ViewBag.CusName = frm["name"];
                    ViewBag.POC = frm["POC"];
                    ViewBag.NOB = frm["NOB"];
                    ViewBag.Email = frm["email"];
                    ViewBag.MOB = frm["mob"];
                    ViewBag.GSTINNo = frm["gstin"].ToUpper();
                    ViewBag.StateCode = frm["statecode"];
                    ViewBag.PANNo = frm["PAN"].ToUpper();
                    ViewBag.DOR = frm["DOR"];
                    ViewBag.COB = frm["COB"];
                    ViewBag.Address = frm["address"];

                    string strStatus = GspSearchTaxPayerGstin.SendRequest(frm["gstin"].ToUpper(), userid.ToString(), custid.ToString(), username);
                    if (strStatus == "1")
                    {
                        TempData["Message"] = "The GSTIN passed in the request is Valid";
                        ViewBag.GSTINNo = frm["gstin"].ToUpper();
                        ViewBag.StateCode = frm["gstin"].ToUpper().Substring(0, 2);
                        ViewBag.PANNo = frm["gstin"].ToUpper().Substring(2, 10);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = strStatus;
                    }
                }

                var buyerDetails = (from buyer in db.TBL_Buyer
                                    where buyer.CustomerId == custid && buyer.RowStatus == true
                                    select buyer).ToList();
                ViewBag.Custmerlist = buyerDetails;
            }

            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Upload(HttpPostedFileBase FileUpload, FormCollection frm)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];
            SqlConnection con = new SqlConnection(ConnString);
            SqlCommand cmd = new SqlCommand();
            DataTable dt = new DataTable();
            if (Request.Files.Count > 0)
            {
                var file = Request.Files[0];

                if (file != null && file.ContentLength > 0)
                {
                    //Workout our file path
                    string fileName = Path.GetFileName(FileUpload.FileName);
                    fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads/"), fileName);

                    //Try and upload
                    try
                    {
                        int TotalRecords, ProcessedRecords;
                        FileUpload.SaveAs(path);

                        ProcessCSV(path);

                        cmd.Connection = con;

                        cmd.CommandText = "usp_Import_CSV_Master";
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.CommandTimeout = 0;
                        cmd.Parameters.Add("@userid", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                        cmd.Parameters.Add("@custId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                        cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "Buyer";
                        cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = path;
                        cmd.Parameters.Add("@TotalRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@ProcessedRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;
                        cmd.Parameters.Add("@ErrorRecordsCount", SqlDbType.Int).Direction = ParameterDirection.Output;


                        con.Open();
                        cmd.ExecuteNonQuery();

                        TotalRecords = Convert.ToInt32(cmd.Parameters["@TotalRecordsCount"].Value);
                        ProcessedRecords = Convert.ToInt32(cmd.Parameters["@ProcessedRecordsCount"].Value);

                        TempData["Message"] = "Customers detail uploaded. TotalRecords: " + TotalRecords + " Processed Records Count: " + ProcessedRecords + "";

                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                        throw;
                    }
                }
                else
                {
                    //Catch errors
                }
            }
            //Tidy up
            dt.Dispose();
            return RedirectToAction("Index");
        }

        [HttpGet]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TBL_Buyer Buyer = db.TBL_Buyer.Find(id);
            if (Buyer == null)
            {
                return HttpNotFound();
            }
            return View(Buyer);
        }

        [HttpPost]
        public ActionResult Edit(int id, FormCollection frm)
        {
            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);

                TBL_Buyer buyer = db.TBL_Buyer.Where(u => u.BuyerId == id).SingleOrDefault();
                var createdDate = (from li in db.TBL_Buyer where li.BuyerId == id select li.CreatedDate).Single();
                var ceatedby = (from li in db.TBL_Buyer where li.BuyerId == id select li.CreatedBy).Single();
                buyer.BuyerName = frm["BuyerName"];
                buyer.POCName = frm["POCName"];
                buyer.NatureOfBusiness = frm["NatureOfBusiness"];
                buyer.EmailId = frm["EmailId"];
                buyer.MobileNo = frm["MobileNo"];
                buyer.GSTINno = frm["GSTINno"].ToUpper();
                buyer.StateCode = frm["StateCode"];
                buyer.PANNO = frm["PANNO"].ToUpper();
                buyer.DateofCompRegistered = frm["DateofCompRegistered"];
                buyer.ConstitutionOfBusiness = frm["ConstitutionOfBusiness"];
                buyer.Address = frm["Address"];
                buyer.CreatedDate = createdDate;
                buyer.CreatedBy = ceatedby;
                buyer.LastmodifiedBy = custid;
                buyer.LastModifiedDate = DateTime.Now;
                buyer.RowStatus = true;

                db.SaveChanges();
                TempData["Message"] = "Customer Updated Successfully";
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer details Modified : " + buyer.BuyerName, "");

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public ActionResult Delete(int Id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                var deleterecord = db.TBL_Buyer.SingleOrDefault(x => x.BuyerId == Id); //returns a single item.

                if (deleterecord != null)
                {
                    deleterecord.RowStatus = false;
                    db.SaveChanges();
                }
                string BuyerName = (from li in db.TBL_Buyer
                                    where li.BuyerId == Id
                                    select li.BuyerName).FirstOrDefault().ToString();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer deleted : " + BuyerName, "");
                TempData["Message"] = "Customer Deleted Successfully";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        private void ProcessCSV(string fileName)
        {
            string ConnString = ConfigurationManager.AppSettings["ConnectionString"];

            SqlConnection con = new SqlConnection(ConnString);
            SqlCommand cmd = new SqlCommand();
            con.Open();
            //Set up our variables
            string line = string.Empty;
            string[] strArray;
            DataTable dt = new DataTable();

            // work out where we should split on comma, but not in a sentence
            Regex r = new Regex(",(?=(?:[^\"]*\"[^\"]*\")*(?![^\"]*\"))");
            //Set the filename in to our stream
            StreamReader sr = new StreamReader(fileName);

            //Read the first line and split the string at , with our regular expression in to an array
            line = sr.ReadLine();
            strArray = r.Split(line);

            //For each item in the new split array, dynamically builds our Data columns. Save us having to worry about it.
            Array.ForEach(strArray, s => dt.Columns.Add(new DataColumn()));

            //Read each line in the CVS file until it’s empty
            while ((line = sr.ReadLine()) != null)
            {
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@MasterType", SqlDbType.NVarChar).Value = "Buyer";
                cmd.Parameters.Add("@Uploadcontent", SqlDbType.NVarChar).Value = line;
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_Id"]);
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = Convert.ToInt32(Session["User_Id"]);
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = Convert.ToDateTime(DateTime.Now);
                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = true;
                Models.Common.Functions.InsertIntoTable("TBL_TMP_CSV_UploadMaster", cmd, con);

            }

            //Tidy Streameader up
            sr.Dispose();
            con.Close();
            //return a the new DataTable
        }
    }
}