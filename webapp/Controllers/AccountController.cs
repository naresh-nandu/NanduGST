#region Using

using Microsoft.AspNet.Identity;
using Microsoft.Owin.Security;
using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;
using WeP_DAL;
#endregion

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class AccountController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        #region "LOGIN"
        // GET: /account/login
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            try
            {
                Session["Partner_Company"] = "NA";
                Session["LogoPath"] = "Content/images/icon-wep-logo.gif";
                string partner_path = Request.Url.Host;
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    partner_path = partner_path.ToLower();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where lower(DomainName) = @PartnerPath and RowStatus = 1";
                        sqlcmd.Parameters.AddWithValue("@PartnerPath", partner_path);
                        using (SqlDataAdapter P_adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable P_dt = new DataTable();
                            P_adt.Fill(P_dt);
                            if (P_dt.Rows.Count > 0)
                            {
                                Session["Partner_LogoutURL"] = P_dt.Rows[0]["LogoutURL"].ToString();
                                Session["Partner_Company"] = P_dt.Rows[0]["Company"].ToString();
                                Session["Partner_LogoPath"] = P_dt.Rows[0]["LogoPath"].ToString();
                            }
                        }
                    }
                }
                string[] myCookies = Request.Cookies.AllKeys;
                foreach (string cookie in myCookies)
                {
                    Response.Cookies[cookie].Expires = DateTime.Now.AddDays(-1);
                }

                if (this.Request.IsAuthenticated)
                {
                    return this.RedirectToLocal(returnUrl);
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            // Info.
            return this.View();
        }

        // POST: /account/login
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Login(FormCollection frm, string returnUrl)
        {
            try
            {
                UserList u = new UserList();
                u.Email = frm["email"];
                u.Password = frm["password"];
                Session["email"] = u.Email;
                // Verification.
                if (ModelState.IsValid)
                {
                    int Op = LoginChecking(u.Email, u.Password);

                    Session["Partner_CustEmail"] = u.Email;
                    Session["Partner_CustPassword"] = u.Password;
                    Session["Partner_Company"] = "NA";
                    string partner_path = Request.Url.Host;
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        partner_path = partner_path.ToLower();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where lower(DomainName) = @PartnerPath and RowStatus = 1";
                            sqlcmd.Parameters.AddWithValue("@PartnerPath", partner_path);
                            using (SqlDataAdapter P_adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable P_dt = new DataTable();
                                P_adt.Fill(P_dt);
                                if (P_dt.Rows.Count > 0)
                                {
                                    Session["Partner_LogoutURL"] = P_dt.Rows[0]["LogoutURL"].ToString();
                                    Session["Partner_Company"] = P_dt.Rows[0]["Company"].ToString();
                                    Session["Partner_LogoPath"] = P_dt.Rows[0]["LogoPath"].ToString();
                                }
                            }
                        }
                    }

                    //For Layout Menu
                    Session["Role_ID"] = (from user in db.UserLists
                                          where user.Email == u.Email && user.rowstatus == true
                                          select user.RoleId).SingleOrDefault();
                    int RoleId = Convert.ToInt32(Session["Role_ID"]);
                    Session["Role_Name"] = (from Roles in db.MAS_Roles
                                            where Roles.Role_ID == RoleId
                                            select Roles.Role_Name).SingleOrDefault();

                    Session["Cust_ID"] = (from user in db.UserLists
                                          where user.Email == u.Email && user.rowstatus == true
                                          select user.CustId).SingleOrDefault();

                    Session["User_ID"] = (from user in db.UserLists
                                          where user.Email == u.Email && user.rowstatus == true
                                          select user.UserId).SingleOrDefault();
                    Session["UserName"] = (from user in db.UserLists
                                           where user.Email == u.Email && user.rowstatus == true
                                           select user.Name).SingleOrDefault();
                    int custid = (from user in db.UserLists
                                  where user.Email == u.Email && user.rowstatus == true
                                  select user.CustId).SingleOrDefault();

                    int userid = (from user in db.UserLists
                                  where user.Email == u.Email && user.rowstatus == true
                                  select user.UserId).SingleOrDefault();
                    
                    Session["Cust_GSTIN"] = (from user in db.TBL_Customer
                                             where user.CustId == custid
                                             select user.GSTINNo).SingleOrDefault();

                    Session["CompanyName"] = (from user in db.TBL_Customer
                                              where user.CustId == custid
                                              select user.Company).SingleOrDefault();
                    Session["CustRefNo"] = (from user in db.TBL_Customer
                                            where user.CustId == custid
                                            select user.ReferenceNo).SingleOrDefault();

                    Session["Email"] = (from user in db.UserLists
                                        where user.UserId == userid
                                        select user.Email).SingleOrDefault();

                    Session["MobileNo"] = (from user in db.UserLists
                                           where user.UserId == userid
                                           select user.MobileNo).SingleOrDefault();

                    Session["Name"] = (from user in db.UserLists
                                       where user.UserId == userid
                                       select user.Name).SingleOrDefault();

                    
                    string strLogoPath = WeP_DAL.Helper.GetCustomerDetails("LogoPath", "TBL_Customer", " WHERE CustId = " + Session["Cust_ID"].ToString() + " and rowstatus = 1");
                    if (!string.IsNullOrEmpty(strLogoPath))
                    {
                        Session["LogoPath"] = strLogoPath;
                    }
                    else
                    {
                        Session["LogoPath"] = "Content/images/icon-wep-logo.gif";
                    }
                    string strMakerCheckerApprover = WeP_DAL.Helper.GetCustomerDetails("MakerCheckerApproverType", "UserList", " WHERE CustId = " + Session["Cust_ID"].ToString() + " and UserId = " + Session["User_ID"].ToString() + " and rowstatus = 1");
                    if (!string.IsNullOrEmpty(strMakerCheckerApprover))
                    {
                        Session["MakerCheckerApproverType"] = strMakerCheckerApprover;
                    }
                    else
                    {
                        Session["MakerCheckerApproverType"] = "NA";
                    }

                    //Start-Session Values for Settings

                    #region "SETTINGS"
                    string Invoice_Setting = "", CustomerCTINSetting = "", SupplierCTINSetting = "", HSNCTINSetting = "", CreditNote_Setting = "", GSTR3B_Setting = "";
                    string TaxValSettings = "", EwaytoGSTR1s = "", GSTR1toEways = "", EwayPrints = "", EwbEamil = "", Location = "", MakerCheckerApprover = "", AutoGenInvNo = "";
                    string ReconSetting = "", InvFormat = "", GenerateGSTR1HSN = "", InvoicePrintLogo = "", rec_othFields = "", AutoGenInwardInvNo = "", TDS = "";
                    OutwardFunctions.Settings(custid, out Invoice_Setting, out CustomerCTINSetting, out SupplierCTINSetting, out HSNCTINSetting, out CreditNote_Setting,
                        out GSTR3B_Setting, out TaxValSettings, out EwaytoGSTR1s, out GSTR1toEways, out EwayPrints, out EwbEamil, out Location, out MakerCheckerApprover, out AutoGenInvNo,
                        out ReconSetting, out InvFormat, out GenerateGSTR1HSN, out InvoicePrintLogo, out rec_othFields, out AutoGenInwardInvNo, out TDS);

                    Session["Setting"] = Invoice_Setting;
                    Session["CustomerCTINSetting"] = CustomerCTINSetting;
                    Session["SupplierCTINSetting"] = SupplierCTINSetting;
                    Session["HSNCTINSetting"] = HSNCTINSetting;
                    Session["CreditNoteSetting"] = CreditNote_Setting;
                    Session["GSTR3BSetting"] = GSTR3B_Setting;
                    Session["TaxValSettings"] = TaxValSettings;
                    Session["EwaytoGSTR1Setting"] = EwaytoGSTR1s;
                    Session["GSTR1toEwaySetting"] = GSTR1toEways;
                    Session["EwayPrintSetting"] = EwayPrints;
                    Session["ewbEmailSetting"] = EwbEamil;
                    Session["LocationSetting"] = Location;
                    Session["MakerCheckerApproverSetting"] = MakerCheckerApprover;
                    Session["AutoGenInvNoSetting"] = AutoGenInvNo;
                    Session["Reconciliation_Setting"] = ReconSetting;
                    Session["InvFormat"] = InvFormat;
                    Session["GenerateGSTR1HSN"] = GenerateGSTR1HSN;
                    Session["InvoicePrintLogo"] = InvoicePrintLogo;
                    Session["Reconciliation_Setting"] = ReconSetting;
                    Session["ReconOtherFields"] = rec_othFields;
                    Session["AutoGenInwardInvNoSetting"] = AutoGenInwardInvNo;
                    Session["TDSSetting"] = TDS;
                    #endregion

                    //END-Session Values for Settings
                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = sqlcon;
                            sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId";
                            sqlcmd.Parameters.AddWithValue("@CustId", custid);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    Session["TRP_ID"] = dt.Rows[0]["TRPId"].ToString();
                                    Session["Package_ID"] = dt.Rows[0]["PackageId"].ToString();
                                    Session["WalletPack"] = dt.Rows[0]["WalletPack"].ToString();
                                }
                                else
                                {
                                    Session["TRP_ID"] = "";
                                    Session["Package_ID"] = "";
                                    Session["WalletPack"] = "";
                                }
                            }
                        }
                    }

                    if (Op == -1)
                    {

                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Logged In", "");
                        this.SignInUser(u.Email, false);
                        return this.RedirectToLocal(returnUrl);
                    }
                    else if (Op == -2)
                    {
                        ModelState.AddModelError(string.Empty, "Your account is blocked/ InActive. Contact Admin!");
                    }

                    else if (Op == -3)
                    {
                        ModelState.AddModelError(string.Empty, "Customer is available but status is Pending approval/ Blocked, So User not allowed to Login");
                    }

                    else if (Op == -4)
                    {
                        ModelState.AddModelError(string.Empty, "Customer status is inactive, So User not allowed to Login");
                    }
                    else if (Op == -5)
                    {
                        ModelState.AddModelError(string.Empty, "Invalid username or password.");
                    }
                    else
                    {
                        // Setting.
                        ModelState.AddModelError(string.Empty, "Please recheck username and password.");
                    }
                }
            }
            catch (Exception ex)
            {
                // Info
                TempData["ErrorMessage"] = ex.Message;
            }

            // If we got this far, something failed, redisplay form
            return this.View();

        }

        public int LoginChecking(string Username, string Password)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("Retrieve_UserAccess", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Email", Username));
                cmd.Parameters.Add(new SqlParameter("@Password", Password));
                cmd.Parameters.Add("@RetValue", SqlDbType.Int).Direction = ParameterDirection.Output;

                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@RetValue"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }
        #endregion


        #region "CUSTOMER REGISTRATION"
        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public ActionResult CustomerRegistration()
        {
            Session["Partner_Company"] = "NA";
            Session["LogoPath"] = "Content/images/icon-wep-logo.gif";
            string partner_path = Request.Url.Host;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                partner_path = partner_path.ToLower();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where lower(DomainName) = @PartnerPath and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@PartnerPath", partner_path);
                    using (SqlDataAdapter P_adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable P_dt = new DataTable();
                        P_adt.Fill(P_dt);
                        if (P_dt.Rows.Count > 0)
                        {
                            Session["Partner_LogoutURL"] = P_dt.Rows[0]["LogoutURL"].ToString();
                            Session["Partner_Company"] = P_dt.Rows[0]["Company"].ToString();
                            Session["Partner_LogoPath"] = P_dt.Rows[0]["LogoPath"].ToString();
                        }
                    }
                }
            }
            return View();
        }

        // POST: /account/register
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]
        public ActionResult CustomerRegistration(FormCollection collection, string Register, string AadhaarNo, string OTP)
        {
            try
            {
                Session["Partner_Company"] = "NA";
                Session["LogoPath"] = "Content/images/icon-wep-logo.gif";
                string partner_path = Request.Url.Host;
                string strPartnerId = "";
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    partner_path = partner_path.ToLower();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where lower(DomainName) = @PartnerPath and RowStatus = 1";
                        sqlcmd.Parameters.AddWithValue("@PartnerPath", partner_path);
                        using (SqlDataAdapter P_adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable P_dt = new DataTable();
                            P_adt.Fill(P_dt);
                            if (P_dt.Rows.Count > 0)
                            {
                                Session["Partner_LogoutURL"] = P_dt.Rows[0]["LogoutURL"].ToString();
                                Session["Partner_Company"] = P_dt.Rows[0]["Company"].ToString();
                                Session["Partner_LogoPath"] = P_dt.Rows[0]["LogoPath"].ToString();
                            }
                        }
                    }
                }

                if (!String.IsNullOrEmpty(AadhaarNo) && String.IsNullOrEmpty(OTP) && String.IsNullOrEmpty(Register))
                {
                    ViewBag.name = collection["name"].ToString();
                    ViewBag.designation = collection["designation"].ToString();
                    ViewBag.company = collection["company"].ToString();
                    ViewBag.gst = collection["gst"].ToString();
                    ViewBag.cgst = collection["cgst"].ToString();
                    ViewBag.email = collection["email"].ToString();
                    ViewBag.mobile = collection["mobile"].ToString();
                    ViewBag.pan = collection["pan"].ToString();
                    ViewBag.statecode = collection["statecode"].ToString();
                    ViewBag.AadhaarNo = collection["AadhaarNo"].ToString();
                    ViewBag.Address = collection["Address"].ToString();
                    ViewBag.CustRefNo = collection["CustRefNo"].ToString();

                    if (Check_Aadhaar_Number(ViewBag.AadhaarNo))
                    {
                        ViewBag.AadhaarImage = "Correct";
                        AadhaarAttributes objAttr = new AadhaarAttributes();
                        objAttr.AadhaarNo = collection["AadhaarNo"].ToString();
                        objAttr.ReqOTPVal = "3";
                        objAttr.Userid = ConfigurationManager.AppSettings["AADHAAR_OTP_USERID"];
                        objAttr.DeviceId = ConfigurationManager.AppSettings["AADHAAR_OTP_DEVICEID"];
                        string jsondata = "";
                        jsondata = JsonConvert.SerializeObject(objAttr);
                        using (var client = new WebClient())
                        {
                            client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                            string data = "=" + jsondata;
                            var result = client.UploadString(ConfigurationManager.AppSettings["AADHAAR_OTP_URL"], "POST", data);
                            string[] strArray = result.Split(',');
                            string Message = strArray[0].ToString();
                            string[] strArray1 = Message.Split(':');
                            ViewBag.StrOTPMsg = strArray1[1].ToString();
                        }
                    }
                    else
                    {
                        ViewBag.AadhaarImage = "Wrong";
                    }
                    return View();
                }
                else if (!String.IsNullOrEmpty(AadhaarNo) && !String.IsNullOrEmpty(OTP) && String.IsNullOrEmpty(Register))
                {
                    ViewBag.name = collection["name"].ToString();
                    ViewBag.designation = collection["designation"].ToString();
                    ViewBag.company = collection["company"].ToString();
                    ViewBag.gst = collection["gst"].ToString();
                    ViewBag.cgst = collection["cgst"].ToString();
                    ViewBag.email = collection["email"].ToString();
                    ViewBag.mobile = collection["mobile"].ToString();
                    ViewBag.pan = collection["pan"].ToString();
                    ViewBag.statecode = collection["statecode"].ToString();
                    ViewBag.AadhaarNo = collection["AadhaarNo"].ToString();
                    ViewBag.OTP = collection["OTP"].ToString();
                    ViewBag.Address = collection["Address"].ToString();
                    ViewBag.CustRefNo = collection["CustRefNo"].ToString();

                    AadhaarAttributes objAttr = new AadhaarAttributes();
                    objAttr.AadhaarNo = collection["AadhaarNo"].ToString();
                    objAttr.ReqOTPVal = string.Empty;
                    objAttr.ResOTPVal = collection["OTP"].ToString();
                    objAttr.Userid = ConfigurationManager.AppSettings["AADHAAR_OTP_USERID"];
                    objAttr.DeviceId = ConfigurationManager.AppSettings["AADHAAR_OTP_DEVICEID"];
                    string jsondata = "";
                    jsondata = JsonConvert.SerializeObject(objAttr);
                    using (var client = new WebClient())
                    {
                        client.Headers[HttpRequestHeader.ContentType] = "application/x-www-form-urlencoded";
                        string data = "=" + jsondata;
                        var result = client.UploadString(ConfigurationManager.AppSettings["AADHAAR_OTP_URL"], "POST", data);
                        string[] strArray = result.Split(',');
                        string Message = strArray[0].ToString();
                        string[] strArray1 = Message.Split(':');
                        ViewBag.StrOTPMsg = strArray1[1].ToString();
                    }

                    return View();
                }
                else if (!String.IsNullOrEmpty(Register))
                {
                    TBL_Customer cust = new TBL_Customer();
                    cust.Name = collection["name"];
                    cust.Designation = collection["designation"];
                    cust.Company = collection["company"];
                    cust.GSTINNo = collection["gst"].ToUpper();
                    cust.Email = collection["email"];
                    cust.MobileNo = collection["mobile"];
                    string strStateCode = collection["gst"].ToString().Substring(0, 2);
                    string strPANNo = collection["gst"].ToString().Substring(2, 10);
                    cust.PANNo = collection["pan"].ToString().ToUpper();
                    cust.Statecode = strStateCode;
                    cust.AadharNo = collection["AadhaarNo"].ToString();
                    cust.Address = collection["Address"].ToString();
                    cust.ReferenceNo = collection["CustRefNo"].ToString();
                    cust.CreatedDate = DateTime.Now;
                    cust.ValidFrom = null;
                    cust.ValidTo = null;

                    using (WePGSPDBEntities context = new WePGSPDBEntities())
                    {
                        ObjectParameter op = new ObjectParameter("RetValue", typeof(int));
                        ObjectParameter op1 = new ObjectParameter("CustId", typeof(int));

                        context.Ins_CustomerRegistration(cust.Name, cust.Designation, cust.Company, cust.GSTINNo, cust.Email, cust.MobileNo, cust.PANNo, cust.Statecode, cust.ValidFrom, cust.ValidFrom, cust.AadharNo, cust.ReferenceNo, cust.Address, op, op1);

                        var value = op.Value; //Get the output value.

                        if ((int)value == 1)
                        {
                            //Cust Docs Saving
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
                                            com.Parameters.Add("@CustomerId", SqlDbType.Int).Value = op1.Value;
                                            com.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                            Functions.InsertIntoTable("TBL_Cust_Docs", com, con);
                                        }
                                        else
                                        {
                                            // if file is other than speified extension 
                                        }
                                    }
                                }
                            }

                            // Customer Approval

                            db.Update_ApproveCustomer(Convert.ToInt32(op1.Value), 1, collection["gstusername"]);

                            // Customer Approval

                            string stremail = collection["email"].ToString();

                            int iCustId = (from user in db.UserLists
                                           where user.Email == stremail
                                           select user.CustId).SingleOrDefault();

                            int iUserId = (from user in db.UserLists
                                           where user.Email == stremail
                                           select user.UserId).SingleOrDefault();

                            // Partner ID UPDATE In TBL_CUSTOMER Table Starts
                            if (!string.IsNullOrEmpty(strPartnerId))
                            {
                                SqlCommand partnercmd = new SqlCommand();
                                partnercmd.Parameters.Clear();
                                partnercmd.Parameters.Add("@PartnerId", SqlDbType.Int).Value = strPartnerId;
                                Functions.UpdateTable("TBL_Customer", "CustId", Convert.ToString(iCustId), partnercmd, con);
                            }
                            // Partner ID UPDATE In TBL_CUSTOMER Table Ends

                            // PAN No Saving
                            TBL_Cust_PAN li = new TBL_Cust_PAN();
                            li.CompanyName = collection["company"];
                            li.PANNo = strPANNo.ToUpper();
                            li.CustId = iCustId;
                            li.CreatedBy = iUserId;
                            li.CreatedDate = DateTime.Now;
                            li.rowstatus = true;
                            db.TBL_Cust_PAN.Add(li);
                            db.SaveChanges();
                            int panID = (from ul in db.TBL_Cust_PAN where ul.PANNo == strPANNo.ToUpper() select ul.PANId).FirstOrDefault();
                            //PAN No Docs Saving
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
                                            //..
                                        }
                                    }
                                }
                            }
                            con.Close();

                            // AUTH KEYS CREATION STARTS

                            string gstinno = collection["gst"].ToUpper().ToString();
                            int GstinId = (from ul in db.TBL_Cust_GSTIN
                                           where ul.GSTINNo == gstinno
                                           select ul.GSTINId).SingleOrDefault();

                            SqlCommand cmd = new SqlCommand();
                            con.Open();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = collection["gstusername"].ToString(); // GSTIN Username
                            cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedOTP", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@EncryptedSEK", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@AuthToken", SqlDbType.NVarChar).Value = "";
                            cmd.Parameters.Add("@Expiry", SqlDbType.Int).Value = 0;
                            cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = GstinId; // GSTIN Id
                            cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now.AddHours(-6);
                            cmd.Parameters.Add("@AuthorizationToken", SqlDbType.NVarChar).Value = collection["gst"].ToUpper().ToString(); // GSTIN No
                            cmd.Parameters.Add("@userid", SqlDbType.Int).Value = iUserId;
                            cmd.Parameters.Add("@custid", SqlDbType.Int).Value = iCustId;
                            Functions.InsertIntoTable("TBL_AUTH_KEYS", cmd, con);
                            con.Close();
                            // AUTH KEYS CREATION ENDS

                            string strCustRefNo = (from ul in db.TBL_Customer
                                                   where ul.Email == cust.Email
                                                   select ul.ReferenceNo).SingleOrDefault();

                            TempData["msg"] = "Customer Registration done Successfully, This Registration subject to verification of documents. In case of document discrepancies registration will be blocked. You will get Login Details to your registered Mobile Number and Email";
                            if (Session["Partner_Company"].ToString() == "Hamara Kendra")
                            {
                                Notification.SendSMS(cust.MobileNo.ToString(), string.Format("Greetings from WeP Digital!! Your Registration at Hamara Kendra GST Suvidha is subject to verification of KYC documents. Your Login e - mail Id is " + cust.Email + " and Password is " + cust.MobileNo.ToString() + ". Customer Reference No " + strCustRefNo + ". Click here to login to GST Return Filing Suvidha https://gst-ws.hamarakendra.com"));
                                Notification.SendEmail(cust.Email, "Your Registration is Approved By Hamara Kendra GST Suvidha Administrator.", "Greetings from WeP Digital!! Your Registration at Hamara Kendra GST Suvidha is subject to verification of KYC documents. Your Login e - mail Id is " + cust.Email + " and Password is " + cust.MobileNo.ToString() + ". <br/>Customer Reference No " + strCustRefNo + ".< br /> Click here to login to GST Return Filing Suvidha https://gst-ws.hamarakendra.com");
                            }
                            else
                            {
                                Notification.SendSMS(cust.MobileNo.ToString(), string.Format("Greetings from WeP Digital!! Your Registration at WeP GST Panel is subject to verification of KYC documents. Your Login e - mail Id is " + cust.Email + " and Password is " + cust.MobileNo.ToString() + ". Customer Reference No " + strCustRefNo + "."));
                                Notification.SendEmail(cust.Email, "Your Registration is Approved By Wep Administrator.", "Greetings from WeP Digital!! Your Registration at WeP GST Panel is subject to verification of KYC documents. Your Login e - mail Id is " + cust.Email + " and Password is " + cust.MobileNo.ToString() + ". Customer Reference No " + strCustRefNo + ".");
                            }
                            return View();
                        }
                        else
                        {
                            ModelState.AddModelError("", "GSTIN or Email ID or Mobile Number of this Customer already exists");
                        }
                    }
                }
                else
                {
                    ViewBag.name = collection["name"].ToString();
                    ViewBag.company = collection["company"].ToString();
                    ViewBag.email = collection["email"].ToString();
                    ViewBag.mobile = collection["mobile"].ToString();
                    ViewBag.gst = collection["gst"].ToString().ToUpper();
                    ViewBag.cgst = collection["gst"].ToString().ToUpper();
                    string strStateCode = collection["gst"].ToString().Substring(0, 2);
                    string strPANNo = collection["gst"].ToString().Substring(2, 10);
                    ViewBag.pan = strPANNo;
                    ViewBag.statecode = strStateCode;
                    ViewBag.Address = collection["Address"].ToString();
                    ViewBag.CustRefNo = collection["CustRefNo"].ToString();
                }
            }
            catch (Exception ex)
            {
                TempData["msg"] = TempData["msg"] + " - [ " + ex.Message + " ] ";
            }
            return View();
        }

        private bool Check_Aadhaar_Number(string Aadhaar_Number)
        {
            try
            {
                return VerhoeffCheckDigit.Check(Aadhaar_Number);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return false;
            }
        }
        #endregion


        #region "FORGOT PASSWORD"
        // GET: /account/forgotpassword
        [System.Web.Mvc.AllowAnonymous]
        [System.Web.Mvc.HttpGet]
        public ActionResult ForgotPassword()
        {
            Session["Partner_Company"] = "NA";
            Session["LogoPath"] = "Content/images/icon-wep-logo.gif";
            string partner_path = Request.Url.Host;
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                partner_path = partner_path.ToLower();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where lower(DomainName) = @PartnerPath and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@PartnerPath", partner_path);
                    using (SqlDataAdapter P_adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable P_dt = new DataTable();
                        P_adt.Fill(P_dt);
                        if (P_dt.Rows.Count > 0)
                        {
                            Session["Partner_LogoutURL"] = P_dt.Rows[0]["LogoutURL"].ToString();
                            Session["Partner_Company"] = P_dt.Rows[0]["Company"].ToString();
                            Session["Partner_LogoPath"] = P_dt.Rows[0]["LogoPath"].ToString();
                        }
                    }
                }
            }

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AllowAnonymous]
        //[ValidateAntiForgeryToken]

        public ActionResult ForgotPassword(UserList u)
        {
            try
            {
                var emailid = (from i in db.UserLists
                               where i.Email == u.Email
                               select i.Email).FirstOrDefault();

                if (u.Email == emailid)
                {
                    var password = (from i in db.UserLists
                                    where i.Email == u.Email
                                    select i.Password).FirstOrDefault();
                    Notification.SendEmail(emailid, "Your Password!", "As per the request for mailing password from you, the same is being sent to you. \n\n Your Password is: " + password + ". \n\n Please do not reply to this mail! \n\n");

                    TempData["msg"] = "You have Recieved your Password to your Registred Email ";
                }
                else
                {
                    ModelState.AddModelError("", "Email Id Does not Exist ");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return View();
        }
        #endregion


        #region "CHANGE PASSWORD"
        [System.Web.Mvc.HttpGet]
        public ActionResult ChangePassword()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult ChangePassword(FormCollection form)
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
                int userid = Convert.ToInt32(Session["User_ID"]);
                int Custid = Convert.ToInt32(Session["Cust_ID"]);
                string password = (from userlist in db.UserLists
                                   where userlist.UserId == userid
                                   select userlist.Password).SingleOrDefault();
                string Email = (from userlist in db.UserLists
                                where userlist.UserId == userid
                                select userlist.Email).SingleOrDefault();
                string Mobileno = (from userlist in db.UserLists
                                   where userlist.UserId == userid
                                   select userlist.MobileNo).SingleOrDefault();
                string Newpassword = form["newp"];
                string CNewpassword = form["cnewp"];
                string OldPassword = form["oldp"];

                if (password == OldPassword)
                {
                    db.Update_UserChangePassword(Custid, Email, Mobileno, OldPassword, Newpassword, CNewpassword, userid);
                    db.SaveChanges();
                    TempData["Msg"] = " Your Password changed successfully";
                    Notification.SendSMS(Mobileno.ToString(), "Your Password changed successfully.");
                }

                else
                {
                    ModelState.AddModelError("", "Old Password is InCorrect");
                }
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return View();
        }
        #endregion


        private void SignInUser(string username, bool isPersistent)
        {
            // Initialization.
            var claims = new List<Claim>();

            try
            {
                // Setting
                claims.Add(new Claim(ClaimTypes.Name, username));
                var claimIdenties = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);
                var ctx = Request.GetOwinContext();
                var authenticationManager = ctx.Authentication;

                // Sign In.
                authenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, claimIdenties);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        private ActionResult RedirectToLocal(string returnUrl)
        {
            try
            {
                // Verification.
                if (Url.IsLocalUrl(returnUrl))
                {
                    // Info.
                    return this.Redirect(returnUrl);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            // Info.
            return this.RedirectToAction("Summary", "Dashboard");
        }

        // GET: /account/error
        [System.Web.Http.AllowAnonymous]
        public ActionResult Error()
        {
            // We do not want to use any existing identity information
            EnsureLoggedOut();
            return View();
        }


        // POST: /account/Logout
        // [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "User Logged Out", "");
            Session.Clear();
            Session.Abandon();
            Session.RemoveAll();
            AuthenticationManager.SignOut();

            return RedirectToAction("Login", "Account");
        }

        private IAuthenticationManager AuthenticationManager
        {
            get
            {
                return HttpContext.GetOwinContext().Authentication;
            }
        }


        private void EnsureLoggedOut()
        {
            // If the request is (still) marked as authenticated we send the user to the logout action
            if (Request.IsAuthenticated)
            {
                Logout();
            }
        }


        // GET: /account/lock
        [System.Web.Mvc.AllowAnonymous]
        public ActionResult Lock()
        {
            return View();
        }

        #region Wallet Transactions

        [System.Web.Mvc.HttpGet]
        // GET: TRPWalletTransactions
        public ActionResult WalletTransactions()
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
                int CustTRPId = Convert.ToInt32(Session["Cust_ID"]);

                ViewBag.FromDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("MM/dd/yyyy");
                DateTime fromdate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));
                DateTime todate = Convert.ToDateTime(DateTime.Now.ToString("MM/dd/yyyy"));

                var WalletTransactions = GetWalletTransactions(CustTRPId, fromdate, todate);
                ViewBag.WalletTransactions = WalletTransactions;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult WalletTransactions(FormCollection frm)
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
                int CustId = Convert.ToInt32(Session["Cust_ID"]);

                DateTime fromdate = DateTime.ParseExact(frm["from"], "MM/dd/yyyy", CultureInfo.InvariantCulture);
                DateTime todate = DateTime.ParseExact(frm["to"], "MM/dd/yyyy", CultureInfo.InvariantCulture).AddDays(1);
                ViewBag.FromDate = frm["from"];
                ViewBag.ToDate = frm["to"];

                var WalletTransactions = GetWalletTransactions(CustId, fromdate, todate);
                ViewBag.WalletTransactions = WalletTransactions;
            }
            catch (Exception ex)
            {
                TempData["Error"] = ex.Message;
            }
            return View();
        }

        public static List<IDictionary> GetWalletTransactions(int CustId, DateTime FromDate, DateTime ToDate)
        {
            DataTable dt = new DataTable();
            try
            {
                #region commented                
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    sqlcon.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = sqlcon;
                        sqlcmd.CommandText = "Select * from TBL_WEP_WALLET_TRANSACTIONS where TRPId IS NULL and CustId = @CustId and CreatedDate between @FromDate and @ToDate";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        sqlcmd.Parameters.AddWithValue("@FromDate", FromDate);
                        sqlcmd.Parameters.AddWithValue("@ToDate", ToDate);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                    sqlcon.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(dt);
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
        #endregion
    }
}