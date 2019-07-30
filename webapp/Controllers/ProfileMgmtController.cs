using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class ProfileMgmtController : Controller
    {
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();


        [HttpGet]
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection Form, string command, HttpPostedFileBase FileUpload)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                #region All Setting
                Nullable<bool> InvoiceSetting = null, GSTR3BSetting = null, C_CTINSetting = null,
                               S_CTINSetting = null, H_CTINSetting = null, CreditNoteSetting = null,
                               GSTR1Setting = null, EwaytoGSTR1 = null, GSTR1toEway = null, EwayPrint = null, MakerCheckerApproverSetting = null,
                               AutoGenInvNoSetting = null, GenerateGSTR1HSNSetting = null, Recon_Accept_OtherFields = null, AutoGenInwardInvNoSetting = null,
                               TDSSetting = null;
                int recon_val = 0;
                int inv_format = 0;

                string Invoice = Form["invoicesetting"];
                string GSTR3B = Form["GSTR3BSetting"];
                string C_CTIN = Form["C_CTINValidation"];
                string S_CTIN = Form["S_CTINValidation"];
                string H_CTIN = Form["H_CTINValidation"];
                string CreditNote = Form["CreditNoteValidation"];
                string GSTR1_Setting = Form["GSTR1Setting"];
                string Eway_to_GSTR1 = Form["Eway_to_GSTR1"];
                string GSTR1_to_Eway = Form["GSTR1_to_Eway"];
                string Eway_Print = Form["EwayPrint"];
                string strMakerCheckerApprover = Form["MakerCheckerApprover"];
                string strAutoGenInvNo = Form["autogeninvnosetting"];
                string Recon_Setting = Form["Recon_val"];
                string invformat = Form["invFormat"];
                string strGenerateGSTR1HSN = Form["GenerateGSTR1HSNReqd"];
                string Rec_OtherFields = Form["otherfields"];
                string strAutoGenInwardInvNo = Form["autogeninwardinvnosetting"];
                string strTDS = Form["tdssetting"];

                if (invformat == "Default")
                {
                    inv_format = 0;
                }
                else if (invformat == "Additional Fields")
                {
                    inv_format = 1;
                }
                else
                {
                    inv_format = 0;
                }

                if (Eway_Print == "Enable")
                {
                    EwayPrint = true;
                }
                else if (Eway_Print == "Disable")
                {
                    EwayPrint = false;
                }
                else
                {
                    EwayPrint = false;
                }

                if (GSTR1_to_Eway == "Enable")
                {
                    GSTR1toEway = true;
                }
                else if (GSTR1_to_Eway == "Disable")
                {
                    GSTR1toEway = false;
                }
                else
                {
                    GSTR1toEway = false;
                }

                if (Eway_to_GSTR1 == "Enable")
                {
                    EwaytoGSTR1 = true;
                }
                else if (Eway_to_GSTR1 == "Disable")
                {
                    EwaytoGSTR1 = false;
                }
                else
                {
                    EwaytoGSTR1 = false;
                }

                if (GSTR1_Setting == "Enable")
                {
                    GSTR1Setting = true;
                }
                else if (GSTR1_Setting == "Disable")
                {
                    GSTR1Setting = false;
                }
                else
                {
                    GSTR1Setting = false;
                }

                if (Invoice == "Enable")
                {
                    InvoiceSetting = true;
                }
                else if (Invoice == "Disable")
                {
                    InvoiceSetting = false;
                }
                else
                {
                    InvoiceSetting = false;
                }

                if (GSTR3B == "Enable")
                {
                    GSTR3BSetting = true;
                }
                else if (GSTR3B == "Disable")
                {
                    GSTR3BSetting = false;
                }
                else
                {
                    GSTR3BSetting = false;
                }

                if (C_CTIN == "Enable")
                {
                    C_CTINSetting = true;
                }
                else if (C_CTIN == "Disable")
                {
                    C_CTINSetting = false;
                }
                else
                {
                    C_CTINSetting = false;
                }

                if (H_CTIN == "Enable")
                {
                    H_CTINSetting = true;
                }
                else if (H_CTIN == "Disable")
                {
                    H_CTINSetting = false;
                }
                else
                {
                    H_CTINSetting = false;
                }

                if (S_CTIN == "Enable")
                {
                    S_CTINSetting = true;
                }
                else if (S_CTIN == "Disable")
                {
                    S_CTINSetting = false;
                }
                else
                {
                    S_CTINSetting = false;
                }

                if (CreditNote == "Enable")
                {
                    CreditNoteSetting = true;
                }
                else if (CreditNote == "Disable")
                {
                    CreditNoteSetting = false;
                }
                else
                {
                    CreditNoteSetting = false;
                }

                if (strMakerCheckerApprover == "Enable")
                {
                    MakerCheckerApproverSetting = true;
                }
                else if (strMakerCheckerApprover == "Disable")
                {
                    MakerCheckerApproverSetting = false;
                }
                else
                {
                    MakerCheckerApproverSetting = false;
                }

                if (strAutoGenInvNo == "Enable")
                {
                    AutoGenInvNoSetting = true;
                }
                else if (strAutoGenInvNo == "Disable")
                {
                    AutoGenInvNoSetting = false;
                }
                else
                {
                    AutoGenInvNoSetting = false;
                }

                if (Recon_Setting == "1")
                {
                    recon_val = 1;
                }
                else if (Recon_Setting == "5")
                {
                    recon_val = 5;
                }
                else if (Recon_Setting == "10")
                {
                    recon_val = 10;
                }
                else if (Recon_Setting == "50")
                {
                    recon_val = 50;
                }
                else
                {
                    recon_val = 1;
                }

                if (strGenerateGSTR1HSN == "Enable")
                {
                    GenerateGSTR1HSNSetting = true;
                }
                else if (strGenerateGSTR1HSN == "Disable")
                {
                    GenerateGSTR1HSNSetting = false;
                }
                else
                {
                    GenerateGSTR1HSNSetting = false;
                }

                if (Rec_OtherFields == "Enable")
                {
                    Recon_Accept_OtherFields = true;
                }
                else if (Rec_OtherFields == "Disable")
                {
                    Recon_Accept_OtherFields = false;
                }
                else
                {
                    Recon_Accept_OtherFields = false;
                }

                if (strAutoGenInwardInvNo == "Enable")
                {
                    AutoGenInwardInvNoSetting = true;
                }
                else if (strAutoGenInwardInvNo == "Disable")
                {
                    AutoGenInwardInvNoSetting = false;
                }
                else
                {
                    AutoGenInwardInvNoSetting = false;
                }

                if (strTDS == "Enable")
                {
                    TDSSetting = true;
                }
                else if (strTDS == "Disable")
                {
                    TDSSetting = false;
                }
                else
                {
                    TDSSetting = false;
                }


                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@InvoicePrintRequired", SqlDbType.Bit).Value = InvoiceSetting;
                cmd.Parameters.Add("@GSTR3BAutoPopulate", SqlDbType.Bit).Value = GSTR3BSetting;
                cmd.Parameters.Add("@CtinValdnCustMgmtReqd", SqlDbType.Bit).Value = C_CTINSetting;
                cmd.Parameters.Add("@CtinValdnSupMgmtReqd", SqlDbType.Bit).Value = S_CTINSetting;
                cmd.Parameters.Add("@HsnValdnHsnMstrReqd", SqlDbType.Bit).Value = H_CTINSetting;
                cmd.Parameters.Add("@TaxValCalnReqd", SqlDbType.Bit).Value = GSTR1Setting;
                cmd.Parameters.Add("@Eway_To_GSTR1", SqlDbType.Bit).Value = EwaytoGSTR1;
                cmd.Parameters.Add("@GSTR1_to_Eway", SqlDbType.Bit).Value = GSTR1toEway;
                cmd.Parameters.Add("@EwayPrint", SqlDbType.Bit).Value = EwayPrint;
                cmd.Parameters.Add("@CdnValdnOrigInum", SqlDbType.Bit).Value = CreditNoteSetting;
                cmd.Parameters.Add("@MakerCheckerApproverReqd", SqlDbType.Bit).Value = MakerCheckerApproverSetting;
                cmd.Parameters.Add("@AutoGenInvNoSettingReqd", SqlDbType.Bit).Value = AutoGenInvNoSetting;
                cmd.Parameters.Add("@AdjustAmount", SqlDbType.Int).Value = recon_val;
                cmd.Parameters.Add("@InvoiceFormat", SqlDbType.Int).Value = inv_format;
                cmd.Parameters.Add("@GenerateGSTR1HSNReqd", SqlDbType.Bit).Value = GenerateGSTR1HSNSetting;                
                cmd.Parameters.Add("@Recon_Accept_OtherFields", SqlDbType.Bit).Value = Recon_Accept_OtherFields;
                cmd.Parameters.Add("@AutoGenInwardInvNoSettingReqd", SqlDbType.Bit).Value = AutoGenInwardInvNoSetting;
                cmd.Parameters.Add("@TDSSettingReqd", SqlDbType.Bit).Value = TDSSetting;
                cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                Functions.UpdateTable("TBL_Cust_Settings", "CustId", Convert.ToString(custid), cmd, con);

                // LOGO UPDATE STARTS
                string strPath = "";
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0];
                    if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                    {
                        string fileName = Path.GetFileName(FileUpload.FileName);
                        string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                        if (FileExtension.Trim() == "jpg" || FileExtension.Trim() == "png" || FileExtension.Trim() == "gif")
                        {
                            strPath = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                            FileUpload.SaveAs(strPath);
                        }
                        SQLHelper.UpdateOrInsertTable("UPDATE TBL_Cust_Settings SET LogoPath = 'Content/images/" + fileName + "' WHERE CustId = '" + custid + "'");
                    }
                }
                // LOGO UPDATE ENDS
                con.Close();
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Setting Changed Successfully", "");
                TempData["Message"] = "Settings Updated Successfully";

                string Invoice_Setting = "", CustomerCTINSetting = "", SupplierCTINSetting = "", HSNCTINSetting = "", CreditNote_Setting = "", GSTR3B_Setting = "";
                string TaxValSettings = "", EwaytoGSTR1s = "", GSTR1toEways = "", EwayPrints = "", EwbEamil = "", Location = "", MakerCheckerApprover = "", AutoGenInvNo = "", Reconciliation_Setting = "";
                string InvFormat = "", GenerateGSTR1HSN = "", InvoicePrintLogo = "",Rec_Oth_Fields = "", AutoGenInwardInvNo = "", TDS = "";
                OutwardFunctions.Settings(custid, out Invoice_Setting, out CustomerCTINSetting, out SupplierCTINSetting, out HSNCTINSetting, out CreditNote_Setting, out GSTR3B_Setting,
                    out TaxValSettings, out EwaytoGSTR1s, out GSTR1toEways, out EwayPrints, out EwbEamil, out Location, out MakerCheckerApprover, out AutoGenInvNo, out Reconciliation_Setting, 
                    out InvFormat, out GenerateGSTR1HSN, out InvoicePrintLogo, out Rec_Oth_Fields, out AutoGenInwardInvNo, out TDS);
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
                Session["Reconciliation_Setting"] = Reconciliation_Setting;
                Session["InvFormat"] = InvFormat;
                Session["GenerateGSTR1HSN"] = GenerateGSTR1HSN;
                Session["InvoicePrintLogo"] = InvoicePrintLogo;
                Session["ReconOtherFields"] = Rec_Oth_Fields;
                Session["AutoGenInwardInvNoSetting"] = AutoGenInwardInvNo;
                Session["TDSSetting"] = TDS;
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            return View();
        }

        [HttpGet]
        public ActionResult Setting()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Setting(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            Nullable<bool> OP = null;
            string Option = Form["Yes"];
            if (Option == "Yes")
            {
                OP = true;
            }
            else if (Option == "No")
            {
                OP = false;
            }

            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@InvoicePrintRequired", SqlDbType.Bit).Value = OP;
            cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
            Functions.UpdateTable("TBL_Cust_Settings", "CustId", Convert.ToString(custid), cmd, con);
            con.Close();
            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Customer Setting Changed Successfully", "");
            TempData["Message"] = "Invoice Settings Updated Successfully";

            return RedirectToAction("Setting");
        }


        [HttpGet]
        public ActionResult CustomerProfile()
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
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                var userprofile = (from profile in db.TBL_Customer
                                   where profile.CustId == custid
                                   select profile).ToList();
                return View(userprofile);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        [HttpGet]
        public ActionResult Update(int? Id)
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
                if (Id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                TBL_Customer li = db.TBL_Customer.Find(Id);
                if (li == null)
                {
                    return HttpNotFound();
                }
                return View(li);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        [HttpPost]
        public ActionResult EditUser(TBL_Customer li, HttpPostedFileBase FileUpload)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int cid = Convert.ToInt32(Session["Cust_ID"]);
            try
            {
                if (ModelState.IsValid)
                {
                    // Customer Modification
                    var customer = db.TBL_Customer.First(p => p.CustId == cid);

                    customer.Name = li.Name;
                    customer.Designation = li.Designation;
                    customer.Company = li.Company;
                    customer.Address = li.Address;
                    db.SaveChanges();

                    //User Modification

                    var userid = (from ul in db.UserLists where ul.CustId == cid select ul.UserId).FirstOrDefault();
                    int uid = Convert.ToInt32(userid);
                    var User = db.UserLists.First(p => p.UserId == uid);
                    User.Name = li.Name;
                    User.Designation = li.Designation;
                    db.SaveChanges();

                    // LOGO UPDATE STARTS
                    string strPath = "";
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                        {
                            string fileName = Path.GetFileName(FileUpload.FileName);
                            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            if (FileExtension.Trim() == "jpg" || FileExtension.Trim() == "png" || FileExtension.Trim() == "gif")
                            {
                                strPath = Path.Combine(Server.MapPath("~/Content/images"), fileName);
                                FileUpload.SaveAs(strPath);
                            }
                            SQLHelper.UpdateOrInsertTable("UPDATE TBL_Customer SET LogoPath = 'Content/images/" + fileName + "' WHERE CustId = '" + cid + "' and rowstatus = 1");
                        }
                    }
                    // LOGO UPDATE ENDS

                    TempData["msg"] = "Profile Updated Successfully";
                    return RedirectToAction("CustomerProfile");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Errors Occured in Updation");
                }
                return View("CustomerProfile");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public static List<IDictionary> Reconciliation_Setting(int CustID)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Reconciliation_Setting", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@custid", CustID));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
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
    }
}