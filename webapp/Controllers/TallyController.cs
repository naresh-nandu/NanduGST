﻿#region Using
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Http;
using Microsoft.Owin.Host.SystemWeb;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Configuration;
using System.IO;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Web.UI.WebControls;
using SmartAdminMvc.Models.GSTRUpload;
using System.Data.OleDb;
using SmartAdminMvc.Models.Report;
using System.Web.UI;
using System.Globalization;
using WeP_DAL;
#endregion

namespace SmartAdminMvc.Controllers
{
    public class TallyController : Controller
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        static SqlCommand cmd = new SqlCommand();
        DataTable dtErrorRecords = new DataTable();
        public WePGSPDBEntities db = new WePGSPDBEntities();

        string error = "";
        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        // GET: GSTRUpload
        public ActionResult tally()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
        
                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
                string rolename = Convert.ToString(Session["Role_Name"]);
                string RefNo = Convert.ToString(Session["CustRefNo"]);
                Session["RefNo"] = RefNo;
                ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, rolename);
                // strGSTRId = TempData["strGSTRId"] as string;
                string strGSTRId = "";
                //  SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                // ViewBag.GSTRList = GSTRlst;
                List<SelectListItem> GSTRlst = new List<SelectListItem>();
                GSTRlst.Add(new SelectListItem
                {
                    Text = "1",
                    Value = "GSTR1"
                });
                GSTRlst.Add(new SelectListItem
                {
                    Text = "2",
                    Value = "GSTR2"
                });
                ViewBag.GSTRList = new SelectList(GSTRlst, "Text", "Value", strGSTRId);
                if (strGSTRId == "1")
                {
                    var AT = GetAT("", "TALLY");
                    ViewBag.AT = AT;                            //1

                    var ATA = GetATA("", "TALLY");
                    ViewBag.ATA = ATA;                          //2

                    var TXPD = GetTXPD("", "TALLY");
                    ViewBag.TXPD = TXPD;                        //3

                    var TXPDA = GetTXPDA("", "TALLY");
                    ViewBag.TXPDA = TXPDA;                      //4  

                    var B2B = GetB2B("", "TALLY");
                    ViewBag.B2B = B2B;                          //5  

                    var B2BA = GetB2BA("", "TALLY");
                    ViewBag.B2BA = B2BA;                        //6

                    var B2CSA = GetB2CSA("", "TALLY");
                    ViewBag.B2CSA = B2CSA;                      //7

                    var B2CS = GetB2CS("", "TALLY");
                    ViewBag.B2CS = B2CS;                        //8

                    var B2CLA = GetB2CLA("", "TALLY");
                    ViewBag.B2CLA = B2CLA;                      //9

                    var B2CL = GetB2CL("", "TALLY");
                    ViewBag.B2CL = B2CL;                        //10

                    var CDNRA = GetCDNRA("", "TALLY");
                    ViewBag.CDNRA = CDNRA;                      //11

                    var CDNR = GetCDNR("", "TALLY");
                    ViewBag.CDNR = CDNR;                        //12

                    var CDNURA = GetCDNURA("", "TALLY");
                    ViewBag.CDNURA = CDNURA;                    //13

                    var CDNUR = GetCDNUR("", "TALLY");
                    ViewBag.CDNUR = CDNUR;                      //14

                    var DOC = GetDOC("", "TALLY");
                    ViewBag.DOC = DOC;                          //15

                    var EXPA = GetEXPA("", "TALLY");
                    ViewBag.EXPA = EXPA;                        //16

                    var EXP = GetEXP("", "TALLY");
                    ViewBag.EXP = EXP;                          //17

                    var NIL = GetNIL("", "TALLY");
                    ViewBag.NIL = NIL;                          //18

                    var HSN = GetHSN("", "TALLY");
                    ViewBag.HSN = HSN;                          //19
                }
                if (strGSTRId == "2")
                {
                    var B2B = GetB2B2("", "TALLY");
                    ViewBag.B2B = B2B;

                    var IMPG = GetIMPG("", "TALLY");
                    ViewBag.IMPG = IMPG;

                    var IMPS = GetIMPS("", "TALLY");
                    ViewBag.IMPS = IMPS;

                    var CDN = GetCDN("", "TALLY");
                    ViewBag.CDN = CDN;

                    var TXI = GetTXI("", "TALLY");
                    ViewBag.TXI = TXI;

                    var HSN = GetHSN2("", "TALLY");
                    ViewBag.HSN = HSN;

                    var NIL = GetNIL2("", "TALLY");
                    ViewBag.NIL = NIL;

                    var TXPD = GetTXPD2("", "TALLY");
                    ViewBag.TXPD = TXPD;

                    var B2BUR = GetB2BUR("", "TALLY");
                    ViewBag.B2BUR = B2BUR;

                    var ITC = GetITC("", "TALLY");
                    ViewBag.ITC = ITC;

                    var CDNUR = GetCDNUR2("", "TALLY");
                    ViewBag.CDNUR = CDNUR;
                }
                TempData["TotalRecordsCount"] = Session["TotalRecordsCount"];
                TempData["ProcessedRecordsCount"] = Session["ProcessedRecordsCount"];
                TempData["ErrorRecordsCount"] = Session["ErrorRecordsCount"];
                TempData["ErrorRecords"] = Session["ErrorRecords"];

            Session.Remove("TotalRecordsCount");
            Session.Remove("ProcessedRecordsCount");
            Session.Remove("ErrorRecordsCount");
            Session.Remove("ErrorRecords");

            return View();
        }
        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult tally(HttpPostedFileBase FileUpload, FormCollection frmcl, string command, int[] ids)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            string strInvIds = "";
            try
            {
                int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
                int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
                string rolename = Convert.ToString(Session["Role_Name"]);
                string refno = frmcl["refno"];
                ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(iUserId, iCustId, rolename);
                var GSTINNo = frmcl["gstn"].ToString();
                string strFp = (frmcl["periodidr"]);
                ViewBag.Period = strFp;
                var strGto = (frmcl["gtoid"]).ToString();
                ViewBag.GrossTurnOver = strGto;
                var strCgto = (frmcl["cgtoid"]).ToString();
                ViewBag.CurrentGrossTurnOver = strCgto;

                string UserEmail = (from user in db.UserLists
                                    where user.UserId == iUserId
                                    select user.Email).SingleOrDefault();

                int strGSTRId = Convert.ToInt32(frmcl["ddlGSTR"]);
                TempData["strGSTRId"] = Convert.ToString(strGSTRId);

                strInvIds = frmcl["InvIds"];
                strInvIds = strInvIds.TrimStart(',').TrimEnd(',');
                GSTR_EXT objGSTREXT = new GSTR_EXT();

                List<SelectListItem> GSTRlst = new List<SelectListItem>();
                GSTRlst.Add(new SelectListItem
                {
                    Text = "1",
                    Value = "GSTR1"
                });
                GSTRlst.Add(new SelectListItem
                {
                    Text = "2",
                    Value = "GSTR2"
                });
                #region  "Upload Data From External tables to Staging Tables"
                if (command == "GSTRUpload")
                {
                    ViewBag.GSTRList = new SelectList(GSTRlst, "Text", "Value", strGSTRId);
                    if (strGSTRId == 1)
                    {
                        PushGSTR1("", "TALLY");
                        PushGSTR1Amendments("", "TALLY");
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 CSV Data is Uploaded Successfully", "");
                    }
                    Session["UploadMessage"] = "Data Uploaded Successfully";
                }
                #endregion

                #region "GSTR1 EXT Delete ALL"
                // DELETE ALL
                //Deleting Invoices from GSTR1 B2B               //1
                else if (command == "GSTR1B2B")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2B", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2B EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2BA               //2
                else if (command == "GSTR1B2BA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2BA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2BA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }       
                //Deleting Invoices from GSTR1 B2CL                //3
                else if (command == "GSTR1B2CL")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CL EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2CLA                //4
                else if (command == "GSTR1B2CLA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CLA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CLA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 AT                    //5
                else if (command == "GSTR1AT")
                {
                    objGSTREXT.GSTR1_Delete_ALL("AT", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 AT EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 ATA                   //6
                else if (command == "GSTR1ATA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("ATA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 ATA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNUR                   //7
                else if (command == "GSTR1CDNUR")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNUR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNUR EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNURA                  //8
                else if (command == "GSTR1CDNURA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNURA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNURA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNR                    //9
                else if (command == "GSTR1CDNR")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNR EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNRA                   //10
                else if (command == "GSTR1CDNRA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNRA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNRA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2CS                     //11
                else if (command == "GSTR1B2CS")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CS", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CS EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2CSA                     //12
                else if (command == "GSTR1B2CSA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CSA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CSA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 DOCISSUE                  //13
                else if (command == "GSTR1DOC")
                {
                    objGSTREXT.GSTR1_Delete_ALL("DOC", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 DOC EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 EXP                       //14
                else if (command == "GSTR1EXP")
                {
                    objGSTREXT.GSTR1_Delete_ALL("EXP", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 EXP EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 EXPA                      //15
                else if (command == "GSTR1EXPA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("EXPA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 EXPA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 HSN                       //16 
                else if (command == "GSTR1HSN")
                {
                    objGSTREXT.GSTR1_Delete_ALL("HSN", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 HSN EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR1 NIL                       //17
                else if (command == "GSTR1NIL")
                {
                    objGSTREXT.GSTR1_Delete_ALL("NIL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 NIL EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 TXPD                      //18
                else if (command == "GSTR1TXPD")
                {
                    objGSTREXT.GSTR1_Delete_ALL("TXP", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 TXP EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 TXPDA                     //19
                else if (command == "GSTR1TXPDA")
                {
                    objGSTREXT.GSTR1_Delete_ALL("TXPA", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 TXPA EXT Deleted", "");
                    Session["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                #endregion

                #region "Uploadad XLSX File"
                else if (command == "Convert")
                {
                    //SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                    //ViewBag.GSTRList = GSTRlst;
                    ViewBag.GSTRList = new SelectList(GSTRlst, "Text", "Value");
                    if (Request.Files.Count > 0)
                    {
                        TempData["strGSTRId"] = Convert.ToString(strGSTRId);
                        var file = Request.Files[0];
                        if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                        {
                            string fileName = Path.GetFileName(FileUpload.FileName);
                            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", iUserId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            if (FileExtension.Trim() == "xlsx" || FileExtension.Trim() == "xls")
                            {
                                if (file.ContentLength > 0)
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    FileUpload.SaveAs(path);
                                    if (strGSTRId == 1)
                                    {
                                        var GridFill = ImportCSV(strGSTRId, path, FileExtension, UserEmail, strGto, strCgto, strFp, GSTINNo);
                                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 Tally File: " + fileName + " is Imported Successfully", "");
                                    }
                                    else
                                    {
                                        //  var GridFill = ImportGSTR2CSV(strGSTRId, path, UserEmail);
                                        // db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 CSV File: " + fileName + " is Imported Successfully", "");
                                    }
                                    Session["UploadMessage"] = "GSTR1 Tally File imported successfully...";

                                    if (strGSTRId == 1)
                                    {
                                        var AT = GetAT("", "TALLY");
                                        ViewBag.AT = AT;                            //1

                                        var ATA = GetATA("", "TALLY");
                                        ViewBag.ATA = ATA;                          //2

                                        var TXPD = GetTXPD("", "TALLY");
                                        ViewBag.TXPD = TXPD;                        //3

                                        var TXPDA = GetTXPDA("", "TALLY");
                                        ViewBag.TXPDA = TXPDA;                      //4   

                                        var B2B = GetB2B("", "TALLY");
                                        ViewBag.B2B = B2B;                          //5  

                                        var B2BA = GetB2BA("", "TALLY");
                                        ViewBag.B2BA = B2BA;                        //6

                                        var B2CSA = GetB2CSA("", "TALLY");
                                        ViewBag.B2CSA = B2CSA;                      //7

                                        var B2CS = GetB2CS("", "TALLY"); 
                                        ViewBag.B2CS = B2CS;                        //8

                                        var B2CLA = GetB2CLA("", "TALLY");
                                        ViewBag.B2CLA = B2CLA;                      //9

                                        var B2CL = GetB2CL("", "TALLY");
                                        ViewBag.B2CL = B2CL;                        //10

                                        var CDNRA = GetCDNRA("", "TALLY");
                                        ViewBag.CDNRA = CDNRA;                      //11

                                        var CDNR = GetCDNR("", "TALLY");
                                        ViewBag.CDNR = CDNR;                        //12

                                        var CDNURA = GetCDNURA("", "TALLY");
                                        ViewBag.CDNURA = CDNURA;                    //13

                                        var CDNUR = GetCDNUR("", "TALLY");
                                        ViewBag.CDNUR = CDNUR;                      //14

                                        var DOC = GetDOC("", "TALLY");
                                        ViewBag.DOC = DOC;                          //15

                                        var EXPA = GetEXPA("", "TALLY");
                                        ViewBag.EXPA = EXPA;                        //16

                                        var EXP = GetEXP("", "TALLY");
                                        ViewBag.EXP = EXP;                          //17

                                        var NIL = GetNIL("", "TALLY");
                                        ViewBag.NIL = NIL;                          //18

                                        var HSN = GetHSN("", "TALLY");
                                        ViewBag.HSN = HSN;                          //19

                                        Session["GUActionID"] = strGSTRId;
                                    }
                                    if (strGSTRId == 2)
                                    {
                                        var B2B = GetB2B2("", "TALLY");
                                        ViewBag.B2B = B2B;

                                        var IMPG = GetIMPG("", "TALLY");
                                        ViewBag.IMPG = IMPG;

                                        var IMPS = GetIMPS("", "TALLY");
                                        ViewBag.IMPS = IMPS;

                                        var CDN = GetCDN("", "TALLY");
                                        ViewBag.CDN = CDN;

                                        var TXI = GetTXI("", "TALLY");
                                        ViewBag.TXI = TXI;

                                        var HSN = GetHSN2("", "TALLY");
                                        ViewBag.HSN = HSN;

                                        var NIL = GetNIL2("", "TALLY");
                                        ViewBag.NIL = NIL;

                                        var TXPD = GetTXPD2("", "TALLY");
                                        ViewBag.TXPD = TXPD;

                                        var B2BUR = GetB2BUR("", "TALLY");
                                        ViewBag.B2BUR = B2BUR;

                                        var ITC = GetITC("", "TALLY");
                                        ViewBag.ITC = ITC;

                                        var CDNUR = GetCDNUR2("", "TALLY");
                                        ViewBag.CDNUR = CDNUR;

                                        Session["GUActionID"] = strGSTRId;
                                    }
                                    ViewBag.GSTRList = new SelectList(GSTRlst, "Text", "Value", strGSTRId);
                                    return View();
                                }
                                else
                                {
                                    Session["UploadMessage"] = "Please select a proper format file.";
                                }
                            }
                            else
                            {
                                TempData["UploadMessage1"] = "File format should be .xlsx or .xls";
                            }
                        }
                        else
                        {
                            Session["UploadMessage"] = "Please select a file.";
                        }
                    }
                    else
                    {
                        Session["UploadMessage"] = "Please select a file.";
                    }
                }
                #endregion

                #region "CTIN VALIDATION"
                else if (command == "CTINValidation")
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_ValidGSTIN where Status IS NULL", conn);
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        { }
                    }
                }
                #endregion
                //onsubmit Event of DropDown
                #region "ONCHANGE EVENT"
                if (strGSTRId == 1)
                {
                    var AT = GetAT("", "TALLY");
                    ViewBag.AT = AT;                            //1

                    var ATA = GetATA("", "TALLY");
                    ViewBag.ATA = ATA;                          //2

                    var TXPD = GetTXPD("", "TALLY");
                    ViewBag.TXPD = TXPD;                        //3

                    var TXPDA = GetTXPDA("", "TALLY");
                    ViewBag.TXPDA = TXPDA;                      //4   

                    var B2B = GetB2B("", "TALLY");
                    ViewBag.B2B= B2B;                      //5  

                    var B2BA = GetB2BA("", "TALLY");
                    ViewBag.B2BA = B2BA;                        //6

                    var B2CSA = GetB2CSA("", "TALLY");
                    ViewBag.B2CSA = B2CSA;                      //7

                    var B2CS = GetB2CS("", "TALLY");
                    ViewBag.B2CS= B2CS;                    //8

                    var B2CLA = GetB2CLA("", "TALLY");
                    ViewBag.B2CLA = B2CLA;                      //9

                    var B2CL= GetB2CL("", "TALLY");
                    ViewBag.B2CL= B2CL;                    //10

                    var CDNRA = GetCDNRA("", "TALLY");
                    ViewBag.CDNRA = CDNRA;                      //11

                    var CDNR = GetCDNR("", "TALLY");
                    ViewBag.CDNR= CDNR;                    //12

                    var CDNURA = GetCDNURA("", "TALLY");
                    ViewBag.CDNURA = CDNURA;                    //13

                    var CDNUR= GetCDNUR("", "TALLY");
                    ViewBag.CDNUR= CDNUR;                  //14

                    var DOC = GetDOC("", "TALLY");
                    ViewBag.DOC = DOC;                          //15

                    var EXPA = GetEXPA("", "TALLY");
                    ViewBag.EXPA = EXPA;                        //16

                    var EXP = GetEXP("", "TALLY");
                    ViewBag.EXP= EXP;                      //17

                    var NIL = GetNIL("", "TALLY");
                    ViewBag.NIL = NIL;                          //18

                    var HSN = GetHSN("", "TALLY");
                    ViewBag.HSN = HSN;                          //19
                    Session["GUActionID"] = strGSTRId;
                }
                else if (strGSTRId == 2)
                {
                    var B2B = GetB2B2("", "TALLY");
                    ViewBag.B2B = B2B;

                    var IMPG = GetIMPG("", "TALLY");
                    ViewBag.IMPG = IMPG;

                    var IMPS = GetIMPS("", "TALLY");
                    ViewBag.IMPS = IMPS;

                    var CDN = GetCDN("", "TALLY");
                    ViewBag.CDN = CDN;

                    var TXI = GetTXI("", "TALLY");
                    ViewBag.TXI = TXI;

                    var HSN = GetHSN2("", "TALLY");
                    ViewBag.HSN = HSN;

                    var NIL = GetNIL2("", "TALLY");
                    ViewBag.NIL = NIL;

                    var TXPD = GetTXPD2("", "TALLY");
                    ViewBag.TXPD = TXPD;

                    var B2BUR = GetB2BUR("", "TALLY");
                    ViewBag.B2BUR = B2BUR;

                    var ITC = GetITC("", "TALLY");
                    ViewBag.ITC = ITC;

                    var CDNUR = GetCDNUR2("", "TALLY");
                    ViewBag.CDNUR = CDNUR;
                    Session["GUActionID"] = strGSTRId;
                }
                #endregion
                //SelectList GSTRlist = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                //ViewBag.GSTRList = GSTRlist;
                ViewBag.GSTRList = new SelectList(GSTRlst, "Text", "Value", strGSTRId);
            }
            catch (Exception ex)
            {
                TempData["UploadMessage1"] = ex.Message;
            }
            return View();
        }
        public ActionResult Download()
        {
            try
            {
                using (DataSet ds = (DataSet)Session["errors"])
                {
                    var table_count = ds.Tables.Count;
                    //Set Name of DataTables.        
                    ds.Tables[0].TableName = "B2B";
                    ds.Tables[1].TableName = "B2BA";
                    ds.Tables[2].TableName = "B2CL";
                    ds.Tables[3].TableName = "B2CLA";
                    ds.Tables[4].TableName = "B2CS";
                    ds.Tables[5].TableName = "B2CSA";
                    ds.Tables[6].TableName = "CDNR";
                    ds.Tables[7].TableName = "CDNRA";
                    ds.Tables[8].TableName = "CDNUR";
                    ds.Tables[9].TableName = "CDNURA";
                    ds.Tables[10].TableName = "EXP";
                    ds.Tables[11].TableName = "EXPA";
                    ds.Tables[12].TableName = "AT";
                    ds.Tables[13].TableName = "ATA";
                    ds.Tables[14].TableName = "TXP";
                    ds.Tables[15].TableName = "TXPA";
                    ds.Tables[16].TableName = "NIL";
                    ds.Tables[17].TableName = "HSN";
                    ds.Tables[18].TableName = "DOC";
                    using (XLWorkbook wb = new XLWorkbook())
                    {
                        foreach (DataTable dt in ds.Tables)
                        {
                            wb.Worksheets.Add(dt);   //Add DataTable as Worksheet.
                        }
                        //Export the Excel file.
                        Response.Clear();
                        Response.Buffer = true;
                        Response.Charset = "";
                        Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                        Response.AddHeader("content-disposition", "attachment;filename=TallyErrorFile.xlsx");
                        using (MemoryStream MyMemoryStream = new MemoryStream())
                        {
                            wb.SaveAs(MyMemoryStream);
                            MyMemoryStream.WriteTo(Response.OutputStream);
                            Response.Flush();
                            Response.End();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return RedirectToAction("tally");
        }
        private List<IDictionary> ImportCSV(int GstrTypeId, string fileName,string FileExtension, string userEmail, string gto, string cgto, string fp, string gstinno)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            string constr = ConfigurationManager.AppSettings["ConnectionString"];
            int Fileid = 0;
            string tableName = string.Empty;
            SqlCommand com = new SqlCommand();
            DataSet ds = new DataSet();
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;
            string errortype = "";
            sqlcon.Open();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            Fileid = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            string excelPath = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);

            string conString = string.Empty;
            if (FileExtension == "xlsx")
            {
                conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                conString = string.Format(conString, excelPath);
            }
            else if (FileExtension == "xls")
            {
                conString = "Provider=Microsoft.Jet.OLEDB.4.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
               // conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1'";
                conString = string.Format(conString, excelPath);
            }
            using (OleDbConnection excel_con = new OleDbConnection(conString))
            {
                excel_con.Open();
                DataTable sheets = GetSchemaTable(conString);
                foreach (DataRow r in sheets.Rows)
                {
                    string columnname, columnname6, columnname1 = "", columnname2 = "";
                    var sheet = r.ItemArray[2].ToString();
                    string sheetName = sheet.TrimEnd('$');
                    string aa = sheetName;
                    #region "Sheet Names"
                    if (aa == "docs" || aa == "DOCS" || aa == "Docs" || aa == "DOC" || aa == "Doc" || aa == "doc")
                    {
                        aa = "DOCISSUE";   //1
                    }
                    else if (aa == "exemp" || sheet == "EXEMP" || sheet == "Exemp")
                    {
                        aa = "NIL";        //2
                    }
                    else if (aa == "at" || aa == "AT" || aa == "At")
                    {
                        aa = "AT";         //3
                    }
                    else if (aa == "ata" || aa == "ATA" || aa == "Ata")
                    {
                        aa = "ATA";        //4
                    }
                    else if (aa == "atadj" || aa == "ATADJ" || aa == "Atadj" || aa == "txp" || aa == "TXP" || aa == "Txp")
                    {
                        aa = "TXP";         //5
                    }
                    else if (aa == "atadja" || aa == "ATADJA" || aa == "Atadja" || aa == "txpa" || aa == "TXPA" || aa == "Txpa")
                    {
                        aa = "TXPA";        //6
                    }
                    else if (aa == "b2b" || aa == "B2B" || aa == "B2b")
                    {
                        aa = "B2B";         //7
                    }
                    else if (aa == "b2ba" || aa == "B2BA" || aa == "B2ba")
                    {
                        aa = "B2BA";        //8
                    }
                    else if (aa == "b2cl" || aa == "B2CL" || aa == "B2cl")
                    {
                        aa = "B2CL";        //9
                    }
                    else if (aa == "b2cla" || aa == "B2CLA" || aa == "B2cla")
                    {
                        aa = "B2CLA";       //10
                    }
                    else if (aa == "b2cs" || aa == "B2CS" || aa == "B2cs")
                    {
                        aa = "B2CS";        //11
                    }
                    else if (aa == "b2csa" || aa == "B2CSA" || aa == "B2csa")
                    {
                        aa = "B2CSA";       //12
                    }
                    else if (aa == "cdnr" || aa == "CDNR" || aa == "Cdnr")
                    {
                        aa = "CDNR";        //13
                    }
                    else if (aa == "cdnra" || aa == "CDNRA" || aa == "Cdnra")
                    {
                        aa = "CDNRA";       //14
                    }
                    else if (aa == "cdnur" || aa == "CDNUR" || aa == "Cdnur")
                    {
                        aa = "CDNUR";       //15
                    }
                    else if (aa == "cdnura" || aa == "CDNURA" || aa == "Cdnura")
                    {
                        aa = "CDNURA";      //16
                    }
                    else if (aa == "exp" || aa == "EXP" || aa == "Exp")
                    {
                        aa = "EXP";         //17
                    }
                    else if (aa == "expa" || aa == "EXPA" || aa == "Expa")
                    {
                        aa = "EXPA";        //18
                    }
                    else if (aa == "hsn" || aa == "HSN" || aa == "Hsn")
                    {
                        aa = "HSN";         //19
                    }
                    else
                    {

                    }
                    #endregion
                    if (GstrTypeId == 1)
                    {
                        switch (aa)
                        {
                            case "DOCISSUE":              //1
                                #region "Docissue"
                                string query = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda1 = new OleDbDataAdapter(query, excel_con);
                                OleDbCommand command1 = new OleDbCommand(query, excel_con);
                                OleDbDataReader dr2 = command1.ExecuteReader();
                                DataTable dt_doc = new DataTable();
                                int count = dr2.FieldCount;
                                if (count == 5)
                                {
                                    oda1.Fill(dt_doc);
                                    columnname = dt_doc.Columns[0].ColumnName;
                                    columnname1 = dt_doc.Columns[3].ColumnName;
                                    if ((columnname == "Nature of Document" || columnname == "Nature  of Document") && columnname1 == "Total Number")
                                    {
                                    }
                                    else
                                    {
                                        string query3 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda1 = new OleDbDataAdapter(query3, excel_con);
                                        command1 = new OleDbCommand(query3, excel_con);
                                        dr2 = command1.ExecuteReader();
                                        dt_doc = new DataTable();
                                        oda1.Fill(dt_doc);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_doc.Columns.Add("gt", typeof(string));
                                        dt_doc.Columns.Add("cgt", typeof(string));
                                        dt_doc.Columns.Add("fp", typeof(string));
                                        dt_doc.Columns.Add("gstin", typeof(string));
                                        dt_doc.Columns.Add("doctyp", typeof(string));
                                        dt_doc.Columns.Add("netissued", typeof(int));
                                        dt_doc.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_doc.Rows)
                                        {
                                            try
                                            {
                                                if (dt_doc.Rows.Count >= 1)
                                                {
                                                    dr[5] = gto;
                                                    dr[6] = cgto;
                                                    dr[7] = fp;
                                                    dr[8] = gstinno;
                                                    var sheet_name = "DOCISSUE";
                                                    dr[9] = sheet_name;
                                                    dr[11] = Fileid;
                                                    var nature = dr[0].ToString();
                                                    if (dr[4].ToString() == "" || dr[3].ToString() == "" || dr[0].ToString() == "")
                                                    {
                                                        var cancl = dr[4].ToString();
                                                        cancl = "NULL";
                                                        var totnum = dr[3].ToString();
                                                        totnum = "NULL";
                                                        var net_iss = dr[10].ToString();
                                                        net_iss = "NULL";
                                                        var natr = dr[0].ToString();
                                                        natr = "NULL";
                                                    }
                                                    else
                                                    {
                                                        var natr = dr[0].ToString();
                                                        if (natr == "Invoices for outward supply" || natr == "1-Invoices for outward supply")
                                                        {
                                                            dr[0] = "1.Invoices for outward supply";
                                                        }
                                                        else if (natr == "Invoices for inward supply from unregistered person" || natr == "2-Invoices for inward supply from unregistered person")
                                                        {
                                                            dr[0] = "2.Invoices for inward supply from unregistered person";
                                                        }
                                                        else if (natr == "Revised Invoice" || natr == "3-Revised Invoice")
                                                        {
                                                            dr[0] = "3.Revised Invoice";
                                                        }
                                                        else if (natr == "Debit Note" || natr == "4-Debit Note")
                                                        {
                                                            dr[0] = "4.Debit Note";
                                                        }
                                                        else if (natr == "Credit Note" || natr == "5-Credit Note")
                                                        {
                                                            dr[0] = "5.Credit Note";
                                                        }
                                                        else if (natr == "Receipt voucher" || natr == "6-Receipt voucher")
                                                        {
                                                            dr[0] = "6.Receipt voucher";
                                                        }
                                                        else if (natr == "Payment Voucher" || natr == "7-Payment Voucher")
                                                        {
                                                            dr[0] = "7.Payment Voucher";
                                                        }
                                                        else if (natr == "Refund voucher" || natr == "8-Refund voucher")
                                                        {
                                                            dr[0] = "8.Refund voucher";
                                                        }
                                                        else if (natr == "Delivery Challan for job work" || natr == "9-Delivery Challan for job work")
                                                        {
                                                            dr[0] = "9.Delivery Challan for job work";
                                                        }
                                                        else if (natr == "Delivery Challan for supply on approval" || natr == "10-Delivery Challan for supply on approval")
                                                        {
                                                            dr[0] = "10.Delivery Challan for supply on approval";
                                                        }
                                                        else if (natr == "Delivery Challan in case of liquid gas" || natr == "11-Delivery Challan in case of liquid gas")
                                                        {
                                                            dr[0] = "11.Delivery Challan in case of liquid gas";
                                                        }
                                                        else if (natr == "Delivery Challan in cases other than by way of supply (excluding at S.no. 9 to 11)" || natr == "12-Delivery Challan in cases other than by way of supply (excluding at S.no. 9 to 11)")
                                                        {
                                                            dr[0] = "12.Delivery Challan in cases other than by way of supply (excluding at S.no. 9 to 11)";
                                                        }
                                                        else
                                                        {
                                                            dr[0] = natr;
                                                        }
                                                        dr[10] = Convert.ToInt32(dr[3].ToString()) - Convert.ToInt32(dr[4].ToString());
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 47); //Doc_Nature
                                            sqlBulkCopy.ColumnMappings.Add(1, 48); // From_Serial_Number
                                            sqlBulkCopy.ColumnMappings.Add(2, 49); // To_Serial_Number
                                            sqlBulkCopy.ColumnMappings.Add(3, 50); // Totnum
                                            sqlBulkCopy.ColumnMappings.Add(4, 51);  //cancel
                                            sqlBulkCopy.ColumnMappings.Add(5, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(8, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(9, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(10, 52);  //net_issue
                                            sqlBulkCopy.ColumnMappings.Add(11, 0);  // fileid
                                            sqlBulkCopy.WriteToServer(dt_doc);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    // error = "DOCISSUE ";
                                    TempData["Error"] += "DOCISSUE " + " ";
                                }
                                break;
                            #endregion
                            case "NIL":                   //2
                                #region "NIL"                          
                                string query4 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda2 = new OleDbDataAdapter(query4, excel_con);
                                OleDbCommand command2 = new OleDbCommand(query4, excel_con);
                                OleDbDataReader dr3 = command2.ExecuteReader();
                                DataTable dt_exemp = new DataTable();
                                int count1 = dr3.FieldCount;
                                if (count1 == 4)
                                {
                                    oda2.Fill(dt_exemp);
                                    columnname = dt_exemp.Columns[0].ColumnName;
                                    columnname1 = dt_exemp.Columns[1].ColumnName;
                                    if (columnname == "Description" && columnname1 == "Nil Rated Supplies")
                                    {
                                    }
                                    else
                                    {
                                        string query5 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda2 = new OleDbDataAdapter(query5, excel_con);
                                        command2 = new OleDbCommand(query5, excel_con);
                                        dr2 = command2.ExecuteReader();
                                        dt_exemp = new DataTable();
                                        oda2.Fill(dt_exemp);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_exemp.Columns.Add("gt", typeof(string));
                                        dt_exemp.Columns.Add("cgt", typeof(string));
                                        dt_exemp.Columns.Add("fp", typeof(string));
                                        dt_exemp.Columns.Add("gstin", typeof(string));
                                        dt_exemp.Columns.Add("doctyp", typeof(string));
                                        dt_exemp.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_exemp.Rows)
                                        {
                                            try
                                            {
                                                if (dt_exemp.Rows.Count >= 1)
                                                {
                                                    dr[4] = gto;
                                                    dr[5] = cgto;
                                                    dr[6] = fp;
                                                    dr[7] = gstinno;
                                                    var sheet_name = "NIL";
                                                    dr[8] = sheet_name;
                                                    dr[9] = Fileid;

                                                    var sply_typ = dr[0].ToString();
                                                    if (sply_typ == "Inter-State supplies to registered persons")
                                                    {
                                                        dr[0] = "INTRB2B";
                                                    }
                                                    else if (sply_typ == "Intra-State supplies to registered persons")
                                                    {
                                                        dr[0] = "INTRAB2C";
                                                    }
                                                    else if (sply_typ == "Inter-State supplies to unregistered persons")
                                                    {
                                                        dr[0] = "INTRB2C";
                                                    }
                                                    else if (sply_typ == "Intra-State supplies to unregistered persons")
                                                    {
                                                        dr[0] = "INTRAB2B";
                                                    }
                                                    else
                                                    {
                                                        dr[0] = sply_typ;
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 43); //nil_sply_ty
                                            sqlBulkCopy.ColumnMappings.Add(1, 44); //nil_amt
                                            sqlBulkCopy.ColumnMappings.Add(2, 45); //expt_amt
                                            sqlBulkCopy.ColumnMappings.Add(3, 46); //ngsup_amt
                                            sqlBulkCopy.ColumnMappings.Add(4, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(5, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(7, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(8, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(9, 0);  //fileid                                    
                                            sqlBulkCopy.WriteToServer(dt_exemp);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    // error = "EXEMP ";
                                    TempData["Error"] += "EXEMP " + " ";
                                }
                                break;
                            #endregion
                            case "AT":                    //3
                                #region "AT"                          
                                string query6 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda3 = new OleDbDataAdapter(query6, excel_con);
                                OleDbCommand command3 = new OleDbCommand(query6, excel_con);
                                OleDbDataReader dr4 = command3.ExecuteReader();
                                DataTable dt_at = new DataTable();
                                int count2 = dr4.FieldCount;
                                if (count2 == 4)
                                {
                                    oda3.Fill(dt_at);
                                    columnname = dt_at.Columns[0].ColumnName;
                                    columnname1 = dt_at.Columns[1].ColumnName;

                                    if (columnname == "Place Of Supply" && columnname1 == "Rate")
                                    {
                                    }
                                    else
                                    {
                                        string query7 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda3 = new OleDbDataAdapter(query7, excel_con);
                                        command3 = new OleDbCommand(query7, excel_con);
                                        dr4 = command3.ExecuteReader();
                                        dt_at = new DataTable();
                                        oda3.Fill(dt_at);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_at.Columns.Add("gt", typeof(string));
                                        dt_at.Columns.Add("cgt", typeof(string));
                                        dt_at.Columns.Add("fp", typeof(string));
                                        dt_at.Columns.Add("gstin", typeof(string));
                                        dt_at.Columns.Add("doctyp", typeof(string));
                                        dt_at.Columns.Add("supply_typ", typeof(string));
                                        dt_at.Columns.Add("igst", typeof(string));
                                        dt_at.Columns.Add("cgst", typeof(string));
                                        dt_at.Columns.Add("sgst", typeof(string));
                                        dt_at.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        foreach (DataRow dr in dt_at.Rows)
                                        {
                                            try
                                            {
                                                if (dt_at.Rows.Count >= 1)
                                                {
                                                    dr[4] = gto;
                                                    dr[5] = cgto;
                                                    dr[6] = fp;
                                                    dr[7] = gstinno;
                                                    var sheet_name = "AT";
                                                    dr[8] = sheet_name;
                                                    dr[13] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";
                                                    if (dr[0].ToString() == "" || dr[1].ToString() == "" || dr[2].ToString() == "")
                                                    {
                                                        var poss = dr[0].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[1].ToString();
                                                        rt = "NULL";
                                                        var rec_amt = dr[2].ToString();
                                                        rec_amt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[0] = (dr[0].ToString()).Substring(0, 2);
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[0].ToString() == gstin)
                                                        {
                                                            dr[9] = suply_typ;
                                                            dr[11] = (Convert.ToDecimal(dr[2].ToString()) * Convert.ToDecimal(dr[1].ToString())) / 200;
                                                            dr[12] = dr[11];
                                                            dr[10] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[9] = sply_typ;
                                                            dr[10] = (Convert.ToDecimal(dr[2].ToString()) * Convert.ToDecimal(dr[1].ToString())) / 100;
                                                            dr[11] = 0.00;
                                                            dr[12] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            //Set the database table name
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 25);  //POS
                                            sqlBulkCopy.ColumnMappings.Add(1, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(2, 41);  //Ad_Recvd_Amt
                                            sqlBulkCopy.ColumnMappings.Add(3, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(4, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(5, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(7, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(8, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(9, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(10, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(11, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(12, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(13, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_at);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }

                                }
                                else if (count2 == 5)
                                {
                                    oda3.Fill(dt_at);
                                    columnname = dt_at.Columns[0].ColumnName;
                                    columnname1 = dt_at.Columns[1].ColumnName;
                                    columnname2 = dt_at.Columns[4].ColumnName;
                                    if (columnname2 != "F5") { 
                                        if (columnname == "Place Of Supply" && columnname1 == "Applicable % of Tax Rate")
                                        {
                                        }
                                        else
                                        {
                                            string query_at = "SELECT * FROM [" + sheet + "A4:AA]";
                                            oda3 = new OleDbDataAdapter(query_at, excel_con);
                                            command3 = new OleDbCommand(query_at, excel_con);
                                            dr4 = command3.ExecuteReader();
                                            dt_at = new DataTable();
                                            oda3.Fill(dt_at);
                                        }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_at.Columns.Add("gt", typeof(string));
                                        dt_at.Columns.Add("cgt", typeof(string));
                                        dt_at.Columns.Add("fp", typeof(string));
                                        dt_at.Columns.Add("gstin", typeof(string));
                                        dt_at.Columns.Add("doctyp", typeof(string));
                                        dt_at.Columns.Add("supply_typ", typeof(string));
                                        dt_at.Columns.Add("igst", typeof(string));
                                        dt_at.Columns.Add("cgst", typeof(string));
                                        dt_at.Columns.Add("sgst", typeof(string));
                                        dt_at.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        foreach (DataRow dr in dt_at.Rows)
                                        {
                                            try
                                            {
                                                if (dt_at.Rows.Count >= 1)
                                                {
                                                    dr[5] = gto;
                                                    dr[6] = cgto;
                                                    dr[7] = fp;
                                                    dr[8] = gstinno;
                                                    var sheet_name = "AT";
                                                    dr[9] = sheet_name;
                                                    dr[14] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";
                                                    if (dr[0].ToString() == "" || dr[2].ToString() == "" || dr[3].ToString() == "")
                                                    {
                                                        var poss = dr[0].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[2].ToString();
                                                        rt = "NULL";
                                                        var rec_amt = dr[3].ToString();
                                                        rec_amt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[0] = (dr[0].ToString()).Substring(0, 2);
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[0].ToString() == gstin)
                                                        {
                                                            dr[10] = suply_typ;
                                                            dr[12] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 200;
                                                            dr[13] = dr[12];
                                                            dr[11] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[10] = sply_typ;
                                                            dr[11] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 100;
                                                            dr[12] = 0.00;
                                                            dr[13] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            //Set the database table name
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 25);  //POS
                                            sqlBulkCopy.ColumnMappings.Add(1, 58); //diff_Perc
                                            sqlBulkCopy.ColumnMappings.Add(2, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(3, 41);  //Ad_Recvd_Amt
                                            sqlBulkCopy.ColumnMappings.Add(4, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(5, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(8, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(9, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(10, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(11, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(12, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(13, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(14, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_at);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                    else
                                    {
                                        //error = "AT ";
                                        TempData["Error"] += "AT " + " ";
                                    }
                                }
                                else
                                {
                                    //error = "AT ";
                                    TempData["Error"] += "AT "+" ";
                                }
                                break;
                            #endregion
                            case "ATA":                   //4
                                #region "ATA"
                                string query8 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda4 = new OleDbDataAdapter(query8, excel_con);
                                OleDbCommand command4 = new OleDbCommand(query8, excel_con);
                                OleDbDataReader dr5 = command4.ExecuteReader();
                                DataTable dt_ata = new DataTable();
                                int count11 = dr5.FieldCount;
                                if (count11 == 6)
                                {
                                    oda4.Fill(dt_ata);
                                    columnname = dt_ata.Columns[0].ColumnName;
                                    columnname1 = dt_ata.Columns[1].ColumnName;
                                    if (columnname == "Financial Year" && columnname1 == "Original Month")
                                    {
                                    }
                                    else
                                    {
                                        string query9 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda4 = new OleDbDataAdapter(query9, excel_con);
                                        command4 = new OleDbCommand(query9, excel_con);
                                        dr5 = command4.ExecuteReader();
                                        dt_ata = new DataTable();
                                        oda4.Fill(dt_ata);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_ata.Columns.Add("gt", typeof(string));
                                        dt_ata.Columns.Add("cgt", typeof(string));
                                        dt_ata.Columns.Add("fp", typeof(string));
                                        dt_ata.Columns.Add("gstin", typeof(string));
                                        dt_ata.Columns.Add("doctyp", typeof(string));
                                        dt_ata.Columns.Add("supply_typ", typeof(string));
                                        dt_ata.Columns.Add("igst", typeof(string));
                                        dt_ata.Columns.Add("cgst", typeof(string));
                                        dt_ata.Columns.Add("sgst", typeof(string));
                                        dt_ata.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        foreach (DataRow dr in dt_ata.Rows)
                                        {
                                            try
                                            {
                                                if (dt_ata.Rows.Count >= 1)
                                                {
                                                    dr[6] = gto;
                                                    dr[7] = cgto;
                                                    dr[8] = fp;
                                                    dr[9] = gstinno;
                                                    var sheet_name = "ATA";
                                                    dr[10] = sheet_name;
                                                    dr[15] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";
                                                    if (dr[2].ToString() == "" || dr[3].ToString() == "" || dr[4].ToString() == "")
                                                    {
                                                        var poss = dr[2].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[3].ToString();
                                                        rt = "NULL";
                                                        var rec_amt = dr[4].ToString();
                                                        rec_amt = "NULL";
                                                        dr[11] = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = (dr[2].ToString()).Substring(0, 2); //pos
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[2].ToString() == gstin)
                                                        {
                                                            dr[11] = suply_typ;
                                                            dr[13] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 200;
                                                            dr[14] = dr[13];
                                                            dr[12] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[11] = sply_typ;
                                                            dr[12] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 100;
                                                            dr[13] = 0.00;
                                                            dr[14] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            //Set the database table name
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 62);  //Financial_year
                                            sqlBulkCopy.ColumnMappings.Add(1, 53);  //or_month
                                            sqlBulkCopy.ColumnMappings.Add(2, 25);  //or_pos
                                            sqlBulkCopy.ColumnMappings.Add(3, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(4, 41);  //Ad_Recvd_Amt
                                            sqlBulkCopy.ColumnMappings.Add(5, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(6, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(8, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(9, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(10, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(11, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(12, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(13, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(14, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_ata);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                   // error = "ATA ";
                                    TempData["Error"] += "ATA "+" ";
                                }
                                break;
                            #endregion
                            case "TXP":                   //5
                                #region "TXP"
                                string query10 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda5 = new OleDbDataAdapter(query10, excel_con);
                                OleDbCommand command5 = new OleDbCommand(query10, excel_con);
                                OleDbDataReader dr6 = command5.ExecuteReader();
                                DataTable dt_atadj = new DataTable();
                                int count3 = dr6.FieldCount;
                                if (count3 == 4)
                                {
                                    oda5.Fill(dt_atadj);
                                    columnname = dt_atadj.Columns[0].ColumnName;
                                    columnname1 = dt_atadj.Columns[1].ColumnName;
                                    if (columnname == "Place Of Supply" && columnname1 == "Rate")
                                    {
                                    }
                                    else
                                    {
                                        string query11 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda5 = new OleDbDataAdapter(query11, excel_con);
                                        command5 = new OleDbCommand(query11, excel_con);
                                        dr6 = command5.ExecuteReader();
                                        dt_atadj = new DataTable();
                                        oda5.Fill(dt_atadj);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_atadj.Columns.Add("gt", typeof(string));
                                        dt_atadj.Columns.Add("cgt", typeof(string));
                                        dt_atadj.Columns.Add("fp", typeof(string));
                                        dt_atadj.Columns.Add("gstin", typeof(string));
                                        dt_atadj.Columns.Add("doctyp", typeof(string));
                                        dt_atadj.Columns.Add("supply_typ", typeof(string));
                                        dt_atadj.Columns.Add("igst", typeof(string));
                                        dt_atadj.Columns.Add("cgst", typeof(string));
                                        dt_atadj.Columns.Add("sgst", typeof(string));
                                        dt_atadj.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_atadj.Rows)
                                        {
                                            try
                                            {
                                                if (dt_atadj.Rows.Count >= 1)
                                                {
                                                    dr[4] = gto;
                                                    dr[5] = cgto;
                                                    dr[6] = fp;
                                                    dr[7] = gstinno;
                                                    var sheet_name = "TXP";
                                                    dr[8] = sheet_name;
                                                    dr[13] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";

                                                    if (dr[0].ToString() == "" || dr[1].ToString() == "" || dr[2].ToString() == "")
                                                    {
                                                        var poss = dr[0].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[1].ToString();
                                                        rt = "NULL";
                                                        var adj_amt = dr[2].ToString();
                                                        adj_amt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[0] = (dr[0].ToString()).Substring(0, 2); //pos
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[0].ToString() == gstin)
                                                        {
                                                            dr[9] = suply_typ;
                                                            dr[11] = (Convert.ToDecimal(dr[2].ToString()) * Convert.ToDecimal(dr[1].ToString())) / 200;
                                                            dr[12] = dr[11];
                                                            dr[10] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[9] = sply_typ;
                                                            dr[10] = (Convert.ToDecimal(dr[2].ToString()) * Convert.ToDecimal(dr[1].ToString())) / 100;
                                                            dr[11] = 0.00;
                                                            dr[12] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        //Set the database table name
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(1, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(2, 42);  //ad_adjsuted
                                            sqlBulkCopy.ColumnMappings.Add(3, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(4, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(5, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(7, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(8, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(9, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(10, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(11, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(12, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(13, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_atadj);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }

                                }
                                else if (count3 == 5)
                                {
                                    oda5.Fill(dt_atadj);
                                    columnname = dt_atadj.Columns[0].ColumnName;
                                    columnname1 = dt_atadj.Columns[1].ColumnName;
                                    columnname2 = dt_atadj.Columns[4].ColumnName;
                                    if (columnname2 != "F5")
                                       {
                                        if (columnname == "Place Of Supply" && columnname1 == "Applicable % of Tax Rate")
                                        {
                                        }
                                        else
                                        {
                                            string query_txp = "SELECT * FROM [" + sheet + "A4:AA]";
                                            oda5 = new OleDbDataAdapter(query_txp, excel_con);
                                            command5 = new OleDbCommand(query_txp, excel_con);
                                            dr6 = command5.ExecuteReader();
                                            dt_atadj = new DataTable();
                                            oda5.Fill(dt_atadj);
                                        }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_atadj.Columns.Add("gt", typeof(string));
                                        dt_atadj.Columns.Add("cgt", typeof(string));
                                        dt_atadj.Columns.Add("fp", typeof(string));
                                        dt_atadj.Columns.Add("gstin", typeof(string));
                                        dt_atadj.Columns.Add("doctyp", typeof(string));
                                        dt_atadj.Columns.Add("supply_typ", typeof(string));
                                        dt_atadj.Columns.Add("igst", typeof(string));
                                        dt_atadj.Columns.Add("cgst", typeof(string));
                                        dt_atadj.Columns.Add("sgst", typeof(string));
                                        dt_atadj.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_atadj.Rows)
                                        {
                                            try
                                            {
                                                if (dt_atadj.Rows.Count >= 1)
                                                {
                                                    dr[5] = gto;
                                                    dr[6] = cgto;
                                                    dr[7] = fp;
                                                    dr[8] = gstinno;
                                                    var sheet_name = "TXP";
                                                    dr[9] = sheet_name;
                                                    dr[14] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";

                                                    if (dr[0].ToString() == "" || dr[2].ToString() == "" || dr[3].ToString() == "")
                                                    {
                                                        var poss = dr[0].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[2].ToString();
                                                        rt = "NULL";
                                                        var adj_amt = dr[3].ToString();
                                                        adj_amt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[0] = (dr[0].ToString()).Substring(0, 2); //pos
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[0].ToString() == gstin)
                                                        {
                                                            dr[10] = suply_typ;
                                                            dr[12] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 200;
                                                            dr[13] = dr[11];
                                                            dr[11] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[10] = sply_typ;
                                                            dr[11] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 100;
                                                            dr[12] = 0.00;
                                                            dr[13] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        //Set the database table name
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(1, 58); //diff_Perc
                                            sqlBulkCopy.ColumnMappings.Add(2, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(3, 42);  //ad_adjsuted
                                            sqlBulkCopy.ColumnMappings.Add(4, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(5, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(6, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(8, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(9, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(10, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(11, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(12, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(13, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(14, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_atadj);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "ATADJ ";
                                    TempData["Error"] += "ATADJ " + " ";
                                }
                        }
                                else
                                {
                                    //error = "ATADJ ";
                                    TempData["Error"] += "ATADJ "+" ";
                                }
                                break;
                            #endregion
                            case "TXPA":                  //6
                                #region "TXPA"
                                string query12 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda6 = new OleDbDataAdapter(query12, excel_con);
                                OleDbCommand command6 = new OleDbCommand(query12, excel_con);
                                OleDbDataReader dr7 = command6.ExecuteReader();
                                DataTable dt_atadja = new DataTable();
                                int count12 = dr7.FieldCount;
                                if (count12 == 6)
                                {
                                    oda6.Fill(dt_atadja);
                                    columnname = dt_atadja.Columns[0].ColumnName;
                                    columnname1 = dt_atadja.Columns[1].ColumnName;
                                    if (columnname == "Financial Year" && columnname1 == "Original Month")
                                    {
                                    }
                                    else
                                    {
                                        string query13 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda6 = new OleDbDataAdapter(query13, excel_con);
                                        command6 = new OleDbCommand(query13, excel_con);
                                        dr7 = command6.ExecuteReader();
                                        dt_atadja = new DataTable();
                                        oda6.Fill(dt_atadja);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_atadja.Columns.Add("gt", typeof(string));
                                        dt_atadja.Columns.Add("cgt", typeof(string));
                                        dt_atadja.Columns.Add("fp", typeof(string));
                                        dt_atadja.Columns.Add("gstin", typeof(string));
                                        dt_atadja.Columns.Add("doctyp", typeof(string));
                                        dt_atadja.Columns.Add("supply_typ", typeof(string));
                                        dt_atadja.Columns.Add("igst", typeof(string));
                                        dt_atadja.Columns.Add("cgst", typeof(string));
                                        dt_atadja.Columns.Add("sgst", typeof(string));
                                        dt_atadja.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_atadja.Rows)
                                        {
                                            try
                                            {
                                                if (dt_atadja.Rows.Count >= 1)
                                                {
                                                    dr[6] = gto;
                                                    dr[7] = cgto;
                                                    dr[8] = fp;
                                                    dr[9] = gstinno;
                                                    var sheet_name = "TXPA";
                                                    dr[10] = sheet_name;
                                                    dr[15] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";
                                                    if (dr[3].ToString() == "" || dr[4].ToString() == "" || dr[2].ToString() == "")
                                                    {
                                                        var poss = dr[2].ToString();
                                                        poss = "NULL";
                                                        var rt = dr[3].ToString();
                                                        rt = "NULL";
                                                        var adj_amt = dr[4].ToString();
                                                        adj_amt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = (dr[2].ToString()).Substring(0, 2); //pos                                                   
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[2].ToString() == gstin)
                                                        {
                                                            dr[11] = suply_typ;
                                                            dr[13] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 200;
                                                            dr[14] = dr[13];
                                                            dr[12] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[11] = sply_typ;
                                                            dr[12] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 100;
                                                            dr[13] = 0.00;
                                                            dr[14] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            //Set the database table name
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 62);  //financial_year
                                            sqlBulkCopy.ColumnMappings.Add(1, 53);  //or_month
                                            sqlBulkCopy.ColumnMappings.Add(2, 25);  //or_pos
                                            sqlBulkCopy.ColumnMappings.Add(3, 17);  //rt
                                            sqlBulkCopy.ColumnMappings.Add(4, 42);  //Ad_Recvd_Amt
                                            sqlBulkCopy.ColumnMappings.Add(5, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(6, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(8, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(9, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(10, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(11, 28);  //supply_typ
                                            sqlBulkCopy.ColumnMappings.Add(12, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(13, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(14, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_atadja);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                   // error = "ATADJA ";
                                    TempData["Error"] += "ATADJA "+" ";
                                }
                                break;
                            #endregion
                            case "B2B":
                                #region "B2B"
                                string query1 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda = new OleDbDataAdapter(query1, excel_con);
                                OleDbCommand command = new OleDbCommand(query1, excel_con);
                                OleDbDataReader dr1 = command.ExecuteReader();
                                DataTable dt_b2b = new DataTable();
                                int count4 = dr1.FieldCount;
                                #region "B2B New Template"
                                if (count4 == 13)
                                {
                                    oda.Fill(dt_b2b);
                                    columnname = dt_b2b.Columns[0].ColumnName;
                                    columnname1 = dt_b2b.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Receiver Name")
                                    {
                                    }
                                    else
                                    {
                                        string query3 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda = new OleDbDataAdapter(query3, excel_con);
                                        command = new OleDbCommand(query3, excel_con);
                                        dr1 = command.ExecuteReader();
                                        dt_b2b = new DataTable();
                                        oda.Fill(dt_b2b);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2b.Columns[3].DataType;
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2b.Columns.Add("NewInvDate", typeof(string));
                                        }
                                        dt_b2b.Columns.Add("gt", typeof(string));
                                        dt_b2b.Columns.Add("cgt", typeof(string));
                                        dt_b2b.Columns.Add("fp", typeof(string));
                                        dt_b2b.Columns.Add("gstin", typeof(string));
                                        dt_b2b.Columns.Add("doctyp", typeof(string));
                                        dt_b2b.Columns.Add("igst", typeof(string));
                                        dt_b2b.Columns.Add("cgst", typeof(string));
                                        dt_b2b.Columns.Add("sgst", typeof(string));
                                        dt_b2b.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2b.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2b.Columns.Remove("Invoice date");
                                            dt_b2b.Columns["NewInvDate"].SetOrdinal(3);
                                        }

                                        foreach (DataRow dr in dt_b2b.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2b.Rows.Count >= 1)
                                                {
                                                    dr[13] = gto;
                                                    dr[14] = cgto;
                                                    dr[15] = fp;
                                                    dr[16] = gstinno;
                                                    var sheet_name = "B2B";
                                                    dr[17] = sheet_name;
                                                    dr[21] = Fileid;

                                                    var dt = dr[3].ToString();
                                                    if (dt == "")
                                                    {
                                                        dt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = dt;
                                                    }
                                                    var inv_typ = dr[8].ToString();
                                                    if (inv_typ.ToString() == "")
                                                    {
                                                        inv_typ = "";
                                                        dr[8] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Regular" || inv_typ.ToString() == "R" || inv_typ.ToString() == "REGULAR" || inv_typ.ToString() == "r" || inv_typ.ToString() == "regular")
                                                    {
                                                        inv_typ = "R";
                                                        dr[8] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ with payment" || inv_typ.ToString() == "SEZ WITH PAYMENT" || inv_typ.ToString() == "sewp" || inv_typ.ToString() == "SEWP")
                                                    {
                                                        inv_typ = "SEWP";
                                                        dr[8] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ  without payment" || inv_typ.ToString() == "SEZ WITHOUT PAYMENT" || inv_typ.ToString() == "sewop" || inv_typ.ToString() == "SEWOP"||inv_typ.ToString() == "SEZ without payment" )
                                                    {
                                                        inv_typ = "SEWOP";
                                                        dr[8] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Deemed Exports" || inv_typ.ToString() == "DEEMED EXPORTS" || inv_typ.ToString() == "de" || inv_typ.ToString() == "DE" || inv_typ.ToString() == "Deemed Exports" || inv_typ.ToString() == "Deemed Exp")
                                                    {
                                                        inv_typ = "DE";
                                                        dr[8] = inv_typ;
                                                    }
                                                    else
                                                    {
                                                        dr[8] = inv_typ;
                                                    }

                                                    if (dr[5].ToString() == "" || dr[0].ToString() == "" || dr[10].ToString() == "" || dr[11].ToString() == "")
                                                    {
                                                        var poss = dr[5].ToString();
                                                        poss = "NULL";
                                                        var ctinn = dr[0].ToString();
                                                        ctinn = "NULL";
                                                        var rt = Convert.ToString(dr[10].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[11].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[5] = (dr[5].ToString()).Substring(0, 2);  //pos
                                                        var ctin = dr[0].ToString();
                                                        ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        var rate = dr[10].ToString();
                                                        // var diff_perc = dr[7].ToString();

                                                        if (dr[5].ToString() == gstin && inv_typ == "")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[19] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 200;
                                                            dr[20] = dr[19];
                                                            dr[18] = 0.00;
                                                            //}
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[19] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 20000;
                                                            //    dr[20] = dr[19];
                                                            //    dr[18] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[18] = 0.00;
                                                            //}
                                                        }
                                                        else if (dr[5].ToString() != gstin && inv_typ == "")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[18] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 100;
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                            //}
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 10000;
                                                            //    dr[20] = 0.00;
                                                            //    dr[19] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[18] = 0.00;
                                                            //}
                                                        }
                                                        else if (dr[5].ToString() == gstin && inv_typ == "R")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[19] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 200;
                                                            dr[20] = dr[19];
                                                            dr[18] = 0.00;
                                                            //}
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[19] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 20000;
                                                            //    dr[20] = dr[19];
                                                            //    dr[18] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[18] = 0.00;
                                                            //}
                                                        }
                                                        else if (dr[5].ToString() != gstin && inv_typ == "R")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[18] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 100;
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{

                                                            //    dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 10000; 
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[18] = 0.00;
                                                            //}
                                                        }
                                                        else if (((dr[5].ToString() == gstin) || (dr[5].ToString() != gstin)) && ((inv_typ == "DE") || (inv_typ == "SEWP")))
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[18] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 100;
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            // dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 10000;
                                                            //dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 10000;
                                                            //dr[19] = 0.00;
                                                            //dr[20] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[18] = 0.00;
                                                            //}
                                                        }
                                                        else if (((dr[5].ToString() == gstin) || (dr[5].ToString() != gstin)) && (inv_typ == "SEWOP"))
                                                        {
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); // Ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 61); // Receiver_Name
                                            sqlBulkCopy.ColumnMappings.Add(2, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(3, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(4, 24);  //val
                                            sqlBulkCopy.ColumnMappings.Add(5, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(6, 26); //rchrg
                                            sqlBulkCopy.ColumnMappings.Add(7, 58); //diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(8, 11); //inv_typ
                                            sqlBulkCopy.ColumnMappings.Add(9, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(10, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(11, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(12, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(13, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(14, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(15, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(16, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(17, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(18, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(19, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(20, 21);  //sgst 
                                            sqlBulkCopy.ColumnMappings.Add(21, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2b);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                #endregion
                                #region "B2B New Template"
                                else if (count4 == 11)
                                {
                                    oda.Fill(dt_b2b);
                                    columnname = dt_b2b.Columns[0].ColumnName;
                                    columnname1 = dt_b2b.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Invoice Number")
                                    {
                                    }
                                    else
                                    {
                                        string query2 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda = new OleDbDataAdapter(query2, excel_con);
                                        command = new OleDbCommand(query2, excel_con);
                                        dr1 = command.ExecuteReader();
                                        dt_b2b = new DataTable();
                                        oda.Fill(dt_b2b);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2b.Columns[2].DataType;
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2b.Columns.Add("NewInvDate", typeof(string));
                                        }
                                        dt_b2b.Columns.Add("gt", typeof(string));
                                        dt_b2b.Columns.Add("cgt", typeof(string));
                                        dt_b2b.Columns.Add("fp", typeof(string));
                                        dt_b2b.Columns.Add("gstin", typeof(string));
                                        dt_b2b.Columns.Add("doctyp", typeof(string));
                                        dt_b2b.Columns.Add("igst", typeof(string));
                                        dt_b2b.Columns.Add("cgst", typeof(string));
                                        dt_b2b.Columns.Add("sgst", typeof(string));
                                        dt_b2b.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        dt_b2b.Columns.Add("diff_perc", typeof(string));
                                        dt_b2b.Columns.Add("receiver_name", typeof(string));
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2b.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2b.Columns.Remove("Invoice date");
                                            dt_b2b.Columns["NewInvDate"].SetOrdinal(2);
                                        }

                                        foreach (DataRow dr in dt_b2b.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2b.Rows.Count >= 1)
                                                {
                                                    dr[11] = gto;
                                                    dr[12] = cgto;
                                                    dr[13] = fp;
                                                    dr[14] = gstinno;
                                                    var sheet_name = "B2B";
                                                    dr[15] = sheet_name;
                                                    dr[19] = Fileid;
                                                    dr[20] = "NULL";
                                                    dr[21] = "NULL";

                                                    var dt = dr[2].ToString();
                                                    if (dt == "")
                                                    {
                                                        dt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                    }

                                                    var inv_typ = dr[6].ToString();
                                                    if (inv_typ.ToString() == "")
                                                    {
                                                        inv_typ = "";
                                                        dr[6] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Regular" || inv_typ.ToString() == "R" || inv_typ.ToString() == "REGULAR" || inv_typ.ToString() == "r" || inv_typ.ToString() == "regular")
                                                    {
                                                        inv_typ = "R";
                                                        dr[6] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ with payment" || inv_typ.ToString() == "SEZ WITH PAYMENT" || inv_typ.ToString() == "sewp" || inv_typ.ToString() == "SEWP")
                                                    {
                                                        inv_typ = "SEWP";
                                                        dr[6] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ  without payment" || inv_typ.ToString() == "SEZ WITHOUT PAYMENT" || inv_typ.ToString() == "sewop" || inv_typ.ToString() == "SEWOP" || inv_typ.ToString() == "SEZ without payment")
                                                    {
                                                        inv_typ = "SEWOP";
                                                        dr[6] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Deemed Exports" || inv_typ.ToString() == "DEEMED EXPORTS" || inv_typ.ToString() == "de" || inv_typ.ToString() == "DE" || inv_typ.ToString() == "Deemed Exports" || inv_typ.ToString() == "Deemed Exp")
                                                    {
                                                        inv_typ = "DE";
                                                        dr[6] = inv_typ;
                                                    }
                                                    else
                                                    {
                                                        dr[6] = inv_typ;
                                                    }

                                                    if (dr[4].ToString() == "" || dr[0].ToString() == "" || dr[8].ToString() == "" || dr[9].ToString() == "")
                                                    {
                                                        var poss = dr[4].ToString();
                                                        poss = "NULL";
                                                        var ctinn = dr[0].ToString();
                                                        ctinn = "NULL";
                                                        var rt = Convert.ToString(dr[8].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[9].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[4] = (dr[4].ToString()).Substring(0, 2);  //pos
                                                        var ctin = dr[0].ToString();
                                                        ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        var rate = dr[8].ToString();
                                                        if (dr[4].ToString() == gstin && inv_typ == "")
                                                        {
                                                            dr[17] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 200;
                                                            dr[18] = dr[17];
                                                            dr[16] = 0.00;
                                                        }

                                                        else if (dr[4].ToString() != gstin && inv_typ == "")
                                                        {
                                                            dr[16] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 100;
                                                            dr[17] = 0.00;
                                                            dr[18] = 0.00;
                                                        }

                                                        else if (dr[4].ToString() == gstin && inv_typ == "R")
                                                        {
                                                            dr[17] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 200;
                                                            dr[18] = dr[17];
                                                            dr[16] = 0.00;
                                                        }

                                                        else if (dr[4].ToString() != gstin & inv_typ == "R")
                                                        {
                                                            dr[16] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 100;
                                                            dr[17] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                        else if (((dr[4].ToString() == gstin) || (dr[4].ToString() != gstin)) && ((inv_typ == "DE") || (inv_typ == "SEWP")))
                                                        {
                                                            dr[16] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 100;
                                                            dr[17] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                        else if ((dr[4].ToString() == gstin || dr[4].ToString() != gstin) && inv_typ == "SEWOP")
                                                        {
                                                            dr[16] = 0.00;
                                                            dr[17] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[16] = 0.00;
                                                            dr[17] = 0.00;
                                                            dr[18] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch
                                            { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); // Ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(2, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(3, 24);  //val
                                            sqlBulkCopy.ColumnMappings.Add(4, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(5, 26); //rchrg
                                            sqlBulkCopy.ColumnMappings.Add(6, 11); //inv_typ
                                            sqlBulkCopy.ColumnMappings.Add(7, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(8, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(9, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(10, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(11, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(12, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(13, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(14, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(15, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(16, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(17, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(18, 21);  //sgst 
                                            sqlBulkCopy.ColumnMappings.Add(19, 0);  //fileid
                                            sqlBulkCopy.ColumnMappings.Add(20, 58);  //diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(21, 61);  //receiver_name
                                            sqlBulkCopy.WriteToServer(dt_b2b);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                   // error = "B2B ";
                                    TempData["Error"] += "B2B " + " ";
                                }
                                #endregion

                                break;
                            #endregion
                            case "B2BA":                  //8
                                #region "B2BA"
                                string query14 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda7 = new OleDbDataAdapter(query14, excel_con);
                                OleDbCommand command7 = new OleDbCommand(query14, excel_con);
                                OleDbDataReader dr8 = command7.ExecuteReader();
                                DataTable dt_b2ba = new DataTable();
                                int count13 = dr8.FieldCount;
                                if (count13 == 15)
                                {
                                    oda7.Fill(dt_b2ba);
                                    columnname = dt_b2ba.Columns[0].ColumnName;
                                    columnname1 = dt_b2ba.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Receiver Name")
                                    {
                                    }
                                    else
                                    {
                                        string query15 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda7 = new OleDbDataAdapter(query15, excel_con);
                                        command7 = new OleDbCommand(query15, excel_con);
                                        dr8 = command7.ExecuteReader();
                                        dt_b2ba = new DataTable();
                                        oda7.Fill(dt_b2ba);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2ba.Columns[3].DataType;
                                        Type type1 = dt_b2ba.Columns[5].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2ba.Columns.Add("NewInvDate", typeof(string));
                                            dt_b2ba.Columns.Add("NewInvDate1", typeof(string));
                                        }
                                        dt_b2ba.Columns.Add("gt", typeof(string));
                                        dt_b2ba.Columns.Add("cgt", typeof(string));
                                        dt_b2ba.Columns.Add("fp", typeof(string));
                                        dt_b2ba.Columns.Add("gstin", typeof(string));
                                        dt_b2ba.Columns.Add("doctyp", typeof(string));
                                        dt_b2ba.Columns.Add("igst", typeof(string));
                                        dt_b2ba.Columns.Add("cgst", typeof(string));
                                        dt_b2ba.Columns.Add("sgst", typeof(string));
                                        dt_b2ba.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2ba.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Original Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Revised Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2ba.Columns.Remove("Original Invoice date");
                                            dt_b2ba.Columns.Remove("Revised Invoice date");
                                            dt_b2ba.Columns["NewInvDate"].SetOrdinal(3);
                                            dt_b2ba.Columns["NewInvDate1"].SetOrdinal(5);
                                        }
                                        foreach (DataRow dr in dt_b2ba.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2ba.Rows.Count >= 1)
                                                {
                                                    dr[15] = gto;
                                                    dr[16] = cgto;
                                                    dr[17] = fp;
                                                    dr[18] = gstinno;
                                                    var sheet_name = "B2BA";
                                                    dr[19] = sheet_name;
                                                    dr[23] = Fileid;

                                                    var dt = dr[3].ToString();
                                                    var dt1 = dr[5].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = dt;
                                                        dr[5] = dt1;
                                                    }
                                                    var inv_typ = dr[10].ToString();
                                                    if (inv_typ.ToString() == "")
                                                    {
                                                        inv_typ = "";
                                                        dr[10] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Regular" || inv_typ.ToString() == "R" || inv_typ.ToString() == "REGULAR" || inv_typ.ToString() == "r" || inv_typ.ToString() == "regular")
                                                    {
                                                        inv_typ = "R";
                                                        dr[10] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ with payment" || inv_typ.ToString() == "SEZ WITH PAYMENT" || inv_typ.ToString() == "sewp" || inv_typ.ToString() == "SEWP")
                                                    {
                                                        inv_typ = "SEWP";
                                                        dr[10] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "SEZ without payment" || inv_typ.ToString() == "SEZ WITHOUT PAYMENT" || inv_typ.ToString() == "sewop" || inv_typ.ToString() == "SEWOP" || inv_typ.ToString() == "SEZ  without payment")
                                                    {
                                                        inv_typ = "SEWOP";
                                                        dr[10] = inv_typ;
                                                    }
                                                    else if (inv_typ.ToString() == "Deemed Exports" || inv_typ.ToString() == "DEEMED EXPORTS" || inv_typ.ToString() == "de" || inv_typ.ToString() == "DE" || inv_typ.ToString() == "Deemed Exports")
                                                    {
                                                        inv_typ = "DE";
                                                        dr[10] = inv_typ;
                                                    }
                                                    else
                                                    {
                                                        dr[10] = inv_typ;
                                                    }
                                                    if (dr[0].ToString() == "" || dr[7].ToString() == "" || dr[12].ToString() == "" || dr[13].ToString() == "")
                                                    {
                                                        var poss = dr[7].ToString();
                                                        poss = "NULL";
                                                        var ctinn = dr[0].ToString();
                                                        ctinn = "NULL";
                                                        var rt = Convert.ToString(dr[12].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[13].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[7] = (dr[7].ToString()).Substring(0, 2);  //pos
                                                        var ctin = dr[0].ToString();

                                                        ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        var rate = dr[12].ToString();
                                                        // var diff_perc = dr[9].ToString();

                                                        if (dr[7].ToString() == gstin && inv_typ == "R")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[21] = (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 200;
                                                            dr[22] = dr[21];
                                                            dr[20] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[21] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 20000;
                                                            //    dr[22] = dr[21];
                                                            //    dr[20] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //}
                                                        }
                                                        else if (dr[7].ToString() != gstin && inv_typ == "R")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[20] = (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 100;
                                                            dr[21] = 0.00;
                                                            dr[22] = 0.00;
                                                            //}
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[20] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 10000;
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //}
                                                        }
                                                        else if ((dr[7].ToString() != gstin) || (dr[7].ToString() != gstin) && ((inv_typ == "DE") || (inv_typ == "SEWP")))
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[20] = (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 100;
                                                            dr[21] = 0.00;
                                                            dr[22] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[20] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 10000;
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //}
                                                        }
                                                        else if ((dr[7].ToString() != gstin) || (dr[7].ToString() != gstin) && inv_typ == "SEWOP")
                                                        {
                                                            dr[20] = 0.00;
                                                            dr[21] = 0.00;
                                                            dr[22] = 0.00;
                                                        }

                                                        else
                                                        {
                                                            dr[20] = 0.00;
                                                            dr[21] = 0.00;
                                                            dr[22] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            //Set the database table name
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); // Ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 61); // Receiver_Name
                                            sqlBulkCopy.ColumnMappings.Add(2, 54); //orinum
                                            sqlBulkCopy.ColumnMappings.Add(3, 55); //oidt
                                            sqlBulkCopy.ColumnMappings.Add(4, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(5, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(6, 24);  //val
                                            sqlBulkCopy.ColumnMappings.Add(7, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(8, 26); //rchrg
                                            sqlBulkCopy.ColumnMappings.Add(9, 58); //Diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(10, 11); //inv_typ
                                            sqlBulkCopy.ColumnMappings.Add(11, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(12, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(13, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(14, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(15, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(16, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(17, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(18, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(19, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(20, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(21, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(22, 21);  //sgst 
                                            sqlBulkCopy.ColumnMappings.Add(23, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2ba);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "B2BA ";
                                    TempData["Error"] += "B2BA"+" ";
                                }
                                break;
                            #endregion
                            case "B2CL":                  //9
                                #region "B2CL"
                                string query16 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda8 = new OleDbDataAdapter(query16, excel_con);
                                OleDbCommand command8 = new OleDbCommand(query16, excel_con);
                                OleDbDataReader dr9 = command8.ExecuteReader();
                                DataTable dt_b2cl = new DataTable();
                                int count5 = dr9.FieldCount;
                                if (count5 == 9)
                                {
                                    oda8.Fill(dt_b2cl);
                                    columnname = dt_b2cl.Columns[0].ColumnName;
                                    columnname1 = dt_b2cl.Columns[1].ColumnName;
                                    if (columnname == "Invoice Number" && columnname1 == "Invoice date")
                                    {
                                    }
                                    else
                                    {
                                        string query17 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda8 = new OleDbDataAdapter(query17, excel_con);
                                        command8 = new OleDbCommand(query17, excel_con);
                                        dr9 = command8.ExecuteReader();
                                        dt_b2cl = new DataTable();
                                        oda8.Fill(dt_b2cl);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2cl.Columns[1].DataType;
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2cl.Columns.Add("NewInvDate", typeof(string));
                                        }

                                        dt_b2cl.Columns.Add("gt", typeof(string));
                                        dt_b2cl.Columns.Add("cgt", typeof(string));
                                        dt_b2cl.Columns.Add("fp", typeof(string));
                                        dt_b2cl.Columns.Add("gstin", typeof(string));
                                        dt_b2cl.Columns.Add("doctyp", typeof(string));
                                        dt_b2cl.Columns.Add("igst", typeof(string));
                                        dt_b2cl.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2cl.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2cl.Columns.Remove("Invoice date");
                                            dt_b2cl.Columns["NewInvDate"].SetOrdinal(1);
                                        }
                                        foreach (DataRow dr in dt_b2cl.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2cl.Rows.Count >= 1)
                                                {
                                                    dr[9] = gto;
                                                    dr[10] = cgto;
                                                    dr[11] = fp;
                                                    dr[12] = gstinno;
                                                    var sheet_name = "B2CL";
                                                    dr[13] = sheet_name;
                                                    dr[15] = Fileid;

                                                    var dt = dr[1].ToString();
                                                    if (dt == "")
                                                    {
                                                        dt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[1] = dt;
                                                    }
                                                    if (dr[3].ToString() == "" || dr[5].ToString() == "" || dr[6].ToString() == "")
                                                    {
                                                        var poss = dr[3].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[5].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[6].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = (dr[3].ToString()).Substring(0, 2);       //pos
                                                                                                          //  var diff_perc = dr[4].ToString();                 //diff_perc
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);   //gstin
                                                        if (dr[3].ToString() != gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[14] = (Convert.ToDecimal(dr[6].ToString()) * Convert.ToDecimal(dr[5].ToString())) / 100;
                                                            // var igst = dr[14].ToString();
                                                            //  }
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[14] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[6].ToString()) * Convert.ToDecimal(dr[5].ToString())) / 10000;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[14] = 0.00;
                                                            //}
                                                        }
                                                        else
                                                        {
                                                            dr[14] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(1, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(2, 24);  //val
                                            sqlBulkCopy.ColumnMappings.Add(3, 25); //POS
                                            sqlBulkCopy.ColumnMappings.Add(4, 58); //diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(5, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(6, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(7, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(8, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(9, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(10, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(11, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(12, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(13, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(14, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2cl);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else if (count5 == 8)
                                {
                                    oda8.Fill(dt_b2cl);
                                    columnname = dt_b2cl.Columns[0].ColumnName;
                                    columnname1 = dt_b2cl.Columns[1].ColumnName;
                                    if (columnname == "Invoice Number" && columnname1 == "Invoice date")
                                    {
                                    }
                                    else
                                    {
                                        string query18 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda8 = new OleDbDataAdapter(query18, excel_con);
                                        command8 = new OleDbCommand(query18, excel_con);
                                        dr9 = command8.ExecuteReader();
                                        dt_b2cl = new DataTable();
                                        oda8.Fill(dt_b2cl);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2cl.Columns[1].DataType;
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2cl.Columns.Add("NewInvDate", typeof(string));
                                        }
                                        dt_b2cl.Columns.Add("gt", typeof(string));
                                        dt_b2cl.Columns.Add("cgt", typeof(string));
                                        dt_b2cl.Columns.Add("fp", typeof(string));
                                        dt_b2cl.Columns.Add("gstin", typeof(string));
                                        dt_b2cl.Columns.Add("doctyp", typeof(string));
                                        dt_b2cl.Columns.Add("igst", typeof(string));
                                        dt_b2cl.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        dt_b2cl.Columns.Add("diff_perc", typeof(string));
                                        if (type == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2cl.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2cl.Columns.Remove("Invoice date");
                                            dt_b2cl.Columns["NewInvDate"].SetOrdinal(1);
                                        }
                                        foreach (DataRow dr in dt_b2cl.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2cl.Rows.Count >= 1)
                                                {
                                                    dr[8] = gto;
                                                    dr[9] = cgto;
                                                    dr[10] = fp;
                                                    dr[11] = gstinno;
                                                    var sheet_name = "B2CL";
                                                    dr[12] = sheet_name;
                                                    dr[14] = Fileid;
                                                    dr[15] = "NULL";

                                                    var dt = dr[1].ToString();
                                                    if (dt == "")
                                                    {
                                                        dt = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[1] = dt;
                                                        // dr[2] = GetDateFormat(dt);
                                                    }
                                                    if (dr[3].ToString() == "" || dr[4].ToString() == "" || dr[5].ToString() == "")
                                                    {
                                                        var poss = dr[3].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[4].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[5].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = (dr[3].ToString()).Substring(0, 2);

                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[3].ToString() != gstin)
                                                        {
                                                            dr[13] = (Convert.ToDecimal(dr[5].ToString()) * Convert.ToDecimal(dr[4].ToString())) / 100;
                                                            var igst = dr[13].ToString();
                                                        }
                                                        else
                                                        {
                                                            TempData["Message"] = "Inter state supplies";

                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(1, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(2, 24);  //val
                                            sqlBulkCopy.ColumnMappings.Add(3, 25); //POS
                                            sqlBulkCopy.ColumnMappings.Add(4, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(5, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(6, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(7, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(8, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(9, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(10, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(11, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(12, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(13, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(14, 0);  //fileid
                                            sqlBulkCopy.ColumnMappings.Add(15, 58);  //diff_perc
                                            sqlBulkCopy.WriteToServer(dt_b2cl);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                  //  error = "B2CL ";
                                    TempData["Error"] += "B2CL "+" ";
                                }
                                break;
                            #endregion
                            case "B2CLA":                 //10
                                #region "B2CLA"
                                string query19 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda9 = new OleDbDataAdapter(query19, excel_con);
                                OleDbCommand command9 = new OleDbCommand(query19, excel_con);
                                OleDbDataReader dr10 = command9.ExecuteReader();
                                DataTable dt_b2cla = new DataTable();
                                int count14 = dr10.FieldCount;
                                if (count14 == 11)
                                {
                                    oda9.Fill(dt_b2cla);
                                    columnname = dt_b2cla.Columns[0].ColumnName;
                                    columnname1 = dt_b2cla.Columns[1].ColumnName;
                                    if (columnname == "Original Invoice Number" && (columnname1 == "Original Invoice date" || columnname1 == "Original Invoice Date"))
                                    {
                                    }
                                    else
                                    {
                                        string query20 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda9 = new OleDbDataAdapter(query20, excel_con);
                                        command9 = new OleDbCommand(query20, excel_con);
                                        dr10 = command9.ExecuteReader();
                                        dt_b2cla = new DataTable();
                                        oda9.Fill(dt_b2cla);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_b2cla.Columns[1].DataType;
                                        Type type1 = dt_b2cla.Columns[4].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_b2cla.Columns.Add("NewInvDate", typeof(string));
                                            dt_b2cla.Columns.Add("NewInvDate1", typeof(string));
                                        }

                                        dt_b2cla.Columns.Add("gt", typeof(string));
                                        dt_b2cla.Columns.Add("cgt", typeof(string));
                                        dt_b2cla.Columns.Add("fp", typeof(string));
                                        dt_b2cla.Columns.Add("gstin", typeof(string));
                                        dt_b2cla.Columns.Add("doctyp", typeof(string));
                                        dt_b2cla.Columns.Add("igst", typeof(string));
                                        dt_b2cla.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_b2cla.Rows)
                                            {
                                                try
                                                {

                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Original Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Revised Invoice date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_b2cla.Columns.Remove("Original Invoice date");
                                            dt_b2cla.Columns.Remove("Revised Invoice date");
                                            dt_b2cla.Columns["NewInvDate"].SetOrdinal(1);
                                            dt_b2cla.Columns["NewInvDate1"].SetOrdinal(4);
                                        }
                                        foreach (DataRow dr in dt_b2cla.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2cla.Rows.Count >= 1)
                                                {
                                                    dr[11] = gto;
                                                    dr[12] = cgto;
                                                    dr[13] = fp;
                                                    dr[14] = gstinno;
                                                    var sheet_name = "B2CLA";
                                                    dr[15] = sheet_name;
                                                    dr[17] = Fileid;

                                                    var dt = dr[1].ToString();
                                                    var dt1 = dr[4].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[1] = dt;
                                                        dr[4] = dt1;
                                                    }

                                                    if (dr[2].ToString() == "" || dr[7].ToString() == "" || dr[8].ToString() == "")
                                                    {
                                                        var poss = dr[2].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[7].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[8].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = (dr[2].ToString()).Substring(0, 2);
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        // var diff_perc = dr[6].ToString();
                                                        if (dr[2].ToString() != gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[16] = (Convert.ToDecimal(dr[8].ToString()) * Convert.ToDecimal(dr[7].ToString())) / 100;
                                                            // }
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[16] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[8].ToString()) * Convert.ToDecimal(dr[7].ToString())) / 10000;

                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[16] = 0.00;
                                                            //}
                                                        }
                                                        else
                                                        {
                                                            TempData["Message"] = "Inter state supplies";
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 54); //or_inum
                                            sqlBulkCopy.ColumnMappings.Add(1, 55); //or_idt                            
                                            sqlBulkCopy.ColumnMappings.Add(2, 25); //POS
                                            sqlBulkCopy.ColumnMappings.Add(3, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(4, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(5, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(6, 58); //Diff_Percent
                                            sqlBulkCopy.ColumnMappings.Add(7, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(8, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(9, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(10, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(11, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(12, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(13, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(14, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(15, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(16, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(17, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2cla);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "B2CLA ";
                                    TempData["Error"] += "B2CLA "+" ";
                                }
                                break;
                            #endregion
                            case "B2CS":                  //11
                                #region "B2CS"
                                string query21 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda10 = new OleDbDataAdapter(query21, excel_con);
                                OleDbCommand command10 = new OleDbCommand(query21, excel_con);
                                OleDbDataReader dr11 = command10.ExecuteReader();
                                DataTable dt_b2cs = new DataTable();
                                int count6 = dr11.FieldCount;
                                if (count6 == 6)
                                {
                                    oda10.Fill(dt_b2cs);
                                    columnname = dt_b2cs.Columns[0].ColumnName;
                                    columnname1 = dt_b2cs.Columns[1].ColumnName;
                                    if (columnname == "Type" && columnname1 == "Place Of Supply")
                                    {
                                    }
                                    else
                                    {
                                        string query22 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda10 = new OleDbDataAdapter(query22, excel_con);
                                        command10 = new OleDbCommand(query22, excel_con);
                                        dr11 = command10.ExecuteReader();
                                        dt_b2cs = new DataTable();
                                        oda10.Fill(dt_b2cs);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_b2cs.Columns.Add("gt", typeof(string));
                                        dt_b2cs.Columns.Add("cgt", typeof(string));
                                        dt_b2cs.Columns.Add("fp", typeof(string));
                                        dt_b2cs.Columns.Add("gstin", typeof(string));
                                        dt_b2cs.Columns.Add("doctyp", typeof(string));
                                        dt_b2cs.Columns.Add("supply_typ", typeof(string));
                                        dt_b2cs.Columns.Add("igst", typeof(string));
                                        dt_b2cs.Columns.Add("cgst", typeof(string));
                                        dt_b2cs.Columns.Add("sgst", typeof(string));
                                        dt_b2cs.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        foreach (DataRow dr in dt_b2cs.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2cs.Rows.Count >= 1)
                                                {
                                                    dr[6] = gto;
                                                    dr[7] = cgto;
                                                    dr[8] = fp;
                                                    dr[9] = gstinno;
                                                    var sheet_name = "B2CS";
                                                    dr[10] = sheet_name;
                                                    dr[15] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";

                                                    if (dr[1].ToString() == "" || dr[2].ToString() == "" || dr[3].ToString() == "")
                                                    {
                                                        var poss = dr[1].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[2].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[3].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[1] = (dr[1].ToString()).Substring(0, 2);
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[1].ToString() == gstin)
                                                        {
                                                            dr[11] = suply_typ;
                                                            dr[13] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 200;
                                                            dr[14] = dr[13];
                                                            dr[12] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[11] = sply_typ;
                                                            dr[12] = (Convert.ToDecimal(dr[3].ToString()) * Convert.ToDecimal(dr[2].ToString())) / 100;
                                                            dr[13] = 0.00;
                                                            dr[14] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(1, 25); //POS
                                            sqlBulkCopy.ColumnMappings.Add(2, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(3, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(4, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(5, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(6, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(7, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(8, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(9, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(10, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(11, 28); //sply_Type
                                            sqlBulkCopy.ColumnMappings.Add(12, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(13, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(14, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2cs);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }

                                }
                                else if (count6 == 7)
                                {
                                    oda10.Fill(dt_b2cs);
                                    columnname = dt_b2cs.Columns[1].ColumnName;
                                    columnname1 = dt_b2cs.Columns[2].ColumnName;
                                    columnname2 = dt_b2cs.Columns[6].ColumnName;
                                    if (columnname2 != "F7")
                                    { 
                                    if (columnname == "Place Of Supply" && columnname1 == "Applicable % of Tax Rate")
                                    {
                                    }
                                    else
                                    {
                                        string query_b2cs = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda10 = new OleDbDataAdapter(query_b2cs, excel_con);
                                        command10 = new OleDbCommand(query_b2cs, excel_con);
                                        dr11 = command10.ExecuteReader();
                                        dt_b2cs = new DataTable();
                                        oda10.Fill(dt_b2cs);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_b2cs.Columns.Add("gt", typeof(string));
                                        dt_b2cs.Columns.Add("cgt", typeof(string));
                                        dt_b2cs.Columns.Add("fp", typeof(string));
                                        dt_b2cs.Columns.Add("gstin", typeof(string));
                                        dt_b2cs.Columns.Add("doctyp", typeof(string));
                                        dt_b2cs.Columns.Add("supply_typ", typeof(string));
                                        dt_b2cs.Columns.Add("igst", typeof(string));
                                        dt_b2cs.Columns.Add("cgst", typeof(string));
                                        dt_b2cs.Columns.Add("sgst", typeof(string));
                                        dt_b2cs.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        foreach (DataRow dr in dt_b2cs.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2cs.Rows.Count >= 1)
                                                {
                                                    dr[7] = gto;
                                                    dr[8] = cgto;
                                                    dr[9] = fp;
                                                    dr[10] = gstinno;
                                                    var sheet_name = "B2CS";
                                                    dr[11] = sheet_name;
                                                    dr[16] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";

                                                    if (dr[1].ToString() == "" || dr[3].ToString() == "" || dr[4].ToString() == "")
                                                    {
                                                        var poss = dr[1].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[3].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[4].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[1] = (dr[1].ToString()).Substring(0, 2);
                                                        var poss = dr[1].ToString();
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[1].ToString() == gstin)
                                                        {
                                                            dr[12] = suply_typ;
                                                            dr[14] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 200;
                                                            dr[15] = dr[13];
                                                            dr[12] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[12] = sply_typ;
                                                            dr[13] = (Convert.ToDecimal(dr[4].ToString()) * Convert.ToDecimal(dr[3].ToString())) / 100;
                                                            dr[14] = 0.00;
                                                            dr[15] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(1, 25); //POS
                                            sqlBulkCopy.ColumnMappings.Add(2, 58); //diff_Perc
                                            sqlBulkCopy.ColumnMappings.Add(3, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(4, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(5, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(6, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(7, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(8, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(9, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(10, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(11, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(12, 28); //sply_Type
                                            sqlBulkCopy.ColumnMappings.Add(13, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(14, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(15, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(16, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_b2cs);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                    else
                                    {
                                        //error = "B2CS ";
                                        TempData["Error"] += "B2CS " + " ";
                                    }
                                }
                                else
                                {
                                    //error = "B2CS ";
                                    TempData["Error"] += "B2CS "+" ";
                                }
                                break;
                            #endregion
                            case "B2CSA":                 //12
                                #region "B2CSA"
                                string query23 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda11 = new OleDbDataAdapter(query23, excel_con);
                                OleDbCommand command11 = new OleDbCommand(query23, excel_con);
                                OleDbDataReader dr12 = command11.ExecuteReader();
                                DataTable dt_b2csa = new DataTable();
                                int count15 = dr12.FieldCount;

                                if (count15 == 8)
                                {
                                    oda11.Fill(dt_b2csa);
                                    columnname = dt_b2csa.Columns[0].ColumnName;
                                    columnname1 = dt_b2csa.Columns[1].ColumnName;
                                    if (columnname == "Financial Year" && columnname1 == "Original Month")
                                    {
                                    }
                                    else
                                    {
                                        string query24 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda11 = new OleDbDataAdapter(query24, excel_con);
                                        command11 = new OleDbCommand(query24, excel_con);
                                        dr11 = command11.ExecuteReader();
                                        dt_b2csa = new DataTable();
                                        oda11.Fill(dt_b2csa);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_b2csa.Columns.Add("gt", typeof(string));
                                        dt_b2csa.Columns.Add("cgt", typeof(string));
                                        dt_b2csa.Columns.Add("fp", typeof(string));
                                        dt_b2csa.Columns.Add("gstin", typeof(string));
                                        dt_b2csa.Columns.Add("doctyp", typeof(string));
                                        dt_b2csa.Columns.Add("igst", typeof(string));
                                        dt_b2csa.Columns.Add("cgst", typeof(string));
                                        dt_b2csa.Columns.Add("sgst", typeof(string));
                                        dt_b2csa.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        dt_b2csa.Columns.Add("supply_typ", typeof(string));
                                        foreach (DataRow dr in dt_b2csa.Rows)
                                        {
                                            try
                                            {
                                                if (dt_b2csa.Rows.Count >= 1)
                                                {
                                                    dr[8] = gto;
                                                    dr[9] = cgto;
                                                    dr[10] = fp;
                                                    dr[11] = gstinno;
                                                    var sheet_name = "B2CSA";
                                                    dr[12] = sheet_name;
                                                    dr[16] = Fileid;
                                                    var sply_typ = "INTER";
                                                    var suply_typ = "INTRA";

                                                    if (dr[2].ToString() == "" || dr[4].ToString() == "" || dr[5].ToString() == "")
                                                    {
                                                        var poss = dr[2].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[4].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[5].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = (dr[2].ToString()).Substring(0, 2); //pos
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (dr[2].ToString() == gstin)
                                                        {
                                                            dr[17] = suply_typ;
                                                            dr[14] = (Convert.ToDecimal(dr[5].ToString()) * Convert.ToDecimal(dr[4].ToString())) / 200;
                                                            dr[15] = dr[14];
                                                            dr[13] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[17] = sply_typ;
                                                            dr[13] = (Convert.ToDecimal(dr[5].ToString()) * Convert.ToDecimal(dr[4].ToString())) / 100;
                                                            dr[14] = 0.00;
                                                            dr[15] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 62); //Financial_yr
                                            sqlBulkCopy.ColumnMappings.Add(1, 53); //or_month
                                            sqlBulkCopy.ColumnMappings.Add(2, 25); //POS
                                            //sqlBulkCopy.ColumnMappings.Add(3, 28); //Type
                                            sqlBulkCopy.ColumnMappings.Add(4, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(5, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(6, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(7, 8); //etin
                                            sqlBulkCopy.ColumnMappings.Add(8, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(9, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(10, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(11, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(12, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(13, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(14, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(15, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(16, 0);  //fileid
                                            sqlBulkCopy.ColumnMappings.Add(17, 28);  //sply_typ
                                            sqlBulkCopy.WriteToServer(dt_b2csa);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                   // error = "B2CSA ";
                                    TempData["Error"] += "B2CSA "+" ";
                                }
                                break;
                            #endregion
                            case "CDNR":                  //13
                                #region "CDNR"
                                string query25 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda12 = new OleDbDataAdapter(query25, excel_con);
                                OleDbCommand command12 = new OleDbCommand(query25, excel_con);
                                OleDbDataReader dr13 = command12.ExecuteReader();
                                DataTable dt_cdnr = new DataTable();
                                int count7 = dr13.FieldCount;
                                if (count7 == 14)
                                {
                                    oda12.Fill(dt_cdnr);
                                    columnname = dt_cdnr.Columns[0].ColumnName;
                                    columnname1 = dt_cdnr.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Receiver Name")
                                    {
                                    }
                                    else
                                    {
                                        string query26 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda12 = new OleDbDataAdapter(query26, excel_con);
                                        command12 = new OleDbCommand(query26, excel_con);
                                        dr13 = command12.ExecuteReader();
                                        dt_cdnr = new DataTable();
                                        oda12.Fill(dt_cdnr);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_cdnr.Columns[3].DataType;
                                        Type type1 = dt_cdnr.Columns[5].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_cdnr.Columns.Add("NewInvDate", typeof(string));
                                            dt_cdnr.Columns.Add("NewInvDate1", typeof(string));
                                        }
                                        dt_cdnr.Columns.Add("gt", typeof(string));
                                        dt_cdnr.Columns.Add("cgt", typeof(string));
                                        dt_cdnr.Columns.Add("fp", typeof(string));
                                        dt_cdnr.Columns.Add("gstin", typeof(string));
                                        dt_cdnr.Columns.Add("doctyp", typeof(string));
                                        dt_cdnr.Columns.Add("igst", typeof(string));
                                        dt_cdnr.Columns.Add("cgst", typeof(string));
                                        dt_cdnr.Columns.Add("sgst", typeof(string));
                                        dt_cdnr.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_cdnr.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch (Exception ex)
                                                { continue; }
                                            }
                                            dt_cdnr.Columns.Remove("Invoice/Advance Receipt date");
                                            dt_cdnr.Columns.Remove("Note/Refund Voucher date");
                                            dt_cdnr.Columns["NewInvDate"].SetOrdinal(3);
                                            dt_cdnr.Columns["NewInvDate1"].SetOrdinal(5);
                                        }
                                        foreach (DataRow dr in dt_cdnr.Rows)
                                        {
                                            try
                                            {
                                                if (dt_cdnr.Rows.Count >= 1)
                                                {
                                                    dr[14] = gto;
                                                    dr[15] = cgto;
                                                    dr[16] = fp;
                                                    dr[17] = gstinno;
                                                    var sheet_name = "CDNR";
                                                    dr[18] = sheet_name;
                                                    dr[22] = Fileid;

                                                    var dt = dr[3].ToString();
                                                    var dt1 = dr[5].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = dt;
                                                        dr[5] = dt1;
                                                    }
                                                    //if (dr[6].ToString() == "")
                                                    //{
                                                    //    dr[6] = "NULL";
                                                    //}
                                                    //else
                                                    //{
                                                        var note_typ = dr[6].ToString();

                                                        if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                        {
                                                            dr[6] = "CREDIT";
                                                        }
                                                        else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                        {
                                                            dr[6] = "DEBIT";
                                                        }
                                                        else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                        {
                                                            dr[6] = "REFUND";
                                                        }
                                                        else
                                                        {
                                                            dr[6] = note_typ;
                                                        }
                                                   // }

                                                    if (dr[7].ToString() == "" || dr[11].ToString() == "" || dr[10].ToString() == "" || dr[0].ToString() == "")
                                                    {
                                                        var ctin = dr[0].ToString();
                                                        ctin = "NULL";
                                                        var poss = dr[7].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[9].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[10].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[7] = (dr[7].ToString()).Substring(0, 2); //pos
                                                                                                    // var ctin = dr[0].ToString();
                                                                                                    //ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                                                                    //  var diff_perc = dr[9].ToString();                          //diff_perc
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);          //gstin
                                                        if (dr[7].ToString() == gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[20] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 200;
                                                            dr[21] = dr[20];
                                                            dr[19] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[20] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 20000;
                                                            //    dr[21] = dr[20];
                                                            //    dr[19] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //    dr[19] = 0.00;
                                                            //}
                                                        }
                                                        else if (dr[7].ToString() != gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[19] = (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 100;
                                                            dr[20] = 0.00;
                                                            dr[21] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[19] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 10000;
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[19] = 0.00;
                                                            //    dr[20] = 0.00;
                                                            //    dr[21] = 0.00;
                                                            //}
                                                        }
                                                        else
                                                        {
                                                            dr[21] = 0.00;
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); //ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 61); //Receiver_Name
                                            sqlBulkCopy.ColumnMappings.Add(2, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(3, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(4, 34); //nt_num
                                            sqlBulkCopy.ColumnMappings.Add(5, 35); //nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(6, 33); //ntty
                                            sqlBulkCopy.ColumnMappings.Add(7, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(8, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(9, 58); //diff_Perc
                                            sqlBulkCopy.ColumnMappings.Add(10, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(11, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(12, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(13, 37); //p_gst
                                            sqlBulkCopy.ColumnMappings.Add(14, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(15, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(16, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(17, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(18, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(19, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(20, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(21, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(22, 0);  //fileid                                                                         
                                            sqlBulkCopy.WriteToServer(dt_cdnr);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else if (count7 == 13)
                                {
                                    oda12.Fill(dt_cdnr);
                                    columnname = dt_cdnr.Columns[0].ColumnName;
                                    columnname1 = dt_cdnr.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Invoice/Advance Receipt Number")
                                    {
                                    }
                                    else
                                    {
                                        string query27 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda12 = new OleDbDataAdapter(query27, excel_con);
                                        command12 = new OleDbCommand(query27, excel_con);
                                        dr13 = command12.ExecuteReader();
                                        dt_cdnr = new DataTable();
                                        oda12.Fill(dt_cdnr);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_cdnr.Columns[2].DataType;
                                        Type type1 = dt_cdnr.Columns[4].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_cdnr.Columns.Add("NewInvDate", typeof(string));
                                            dt_cdnr.Columns.Add("NewInvDate1", typeof(string));
                                        }
                                        dt_cdnr.Columns.Add("gt", typeof(string));
                                        dt_cdnr.Columns.Add("cgt", typeof(string));
                                        dt_cdnr.Columns.Add("fp", typeof(string));
                                        dt_cdnr.Columns.Add("gstin", typeof(string));
                                        dt_cdnr.Columns.Add("doctyp", typeof(string));
                                        dt_cdnr.Columns.Add("igst", typeof(string));
                                        dt_cdnr.Columns.Add("cgst", typeof(string));
                                        dt_cdnr.Columns.Add("sgst", typeof(string));
                                        dt_cdnr.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        dt_cdnr.Columns.Add("diff_perc", typeof(string));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_cdnr.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch
                                                { continue; }
                                            }
                                            dt_cdnr.Columns.Remove("Invoice/Advance Receipt date");
                                            dt_cdnr.Columns.Remove("Note/Refund Voucher date");
                                            dt_cdnr.Columns["NewInvDate"].SetOrdinal(2);
                                            dt_cdnr.Columns["NewInvDate1"].SetOrdinal(4);
                                        }
                                        foreach (DataRow dr in dt_cdnr.Rows)
                                        {
                                            try
                                            {
                                                if (dt_cdnr.Rows.Count >= 1)
                                                {
                                                    dr[13] = gto;
                                                    dr[14] = cgto;
                                                    dr[15] = fp;
                                                    dr[16] = gstinno;
                                                    var sheet_name = "CDNR";
                                                    dr[17] = sheet_name;
                                                    dr[21] = Fileid;
                                                    dr[22] = "NULL";

                                                    var dt = dr[2].ToString();
                                                    var dt1 = dr[4].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                        dr[4] = dt1;
                                                    }
                                                    var note_typ = dr[5].ToString();
                                                    if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                    {
                                                        dr[5] = "CREDIT";
                                                    }
                                                    else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                    {
                                                        dr[5] = "DEBIT";
                                                    }

                                                    else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                    {
                                                        dr[5] = "REFUND";
                                                    }
                                                    else
                                                    {
                                                        dr[5] = note_typ;
                                                    }
                                                    if (dr[7].ToString() == "" || dr[9].ToString() == "" || dr[10].ToString() == "" || dr[0].ToString() == "" || dr[6].ToString() == "")
                                                    {
                                                        var ctin = dr[0].ToString();
                                                        ctin = "NULL";
                                                        var poss = dr[7].ToString();
                                                        poss = "NULL";
                                                        var rt = Convert.ToString(dr[9].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[10].ToString());
                                                        tx = "NULL";
                                                        var rsn = Convert.ToString(dr[6].ToString());
                                                        rsn = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[7] = (dr[7].ToString()).Substring(0, 2); //pos
                                                        var ctin = dr[0].ToString();
                                                        ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                        var rsn = dr[6].ToString();
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);

                                                        if (dr[7].ToString() == gstin)
                                                        {
                                                            dr[19] = (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 200;
                                                            dr[20] = dr[19];
                                                            dr[18] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[18] = (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 100;
                                                            dr[19] = 0.00;
                                                            dr[20] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); //ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(2, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(3, 34); //nt_num
                                            sqlBulkCopy.ColumnMappings.Add(4, 35); //nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(5, 33); //ntty
                                            sqlBulkCopy.ColumnMappings.Add(6, 36); //rsn
                                            sqlBulkCopy.ColumnMappings.Add(7, 25); //pos
                                            sqlBulkCopy.ColumnMappings.Add(8, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(9, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(10, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(11, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(12, 37); //p_gst
                                            sqlBulkCopy.ColumnMappings.Add(13, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(14, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(15, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(16, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(17, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(18, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(19, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(20, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(21, 0);  //fileid 
                                            sqlBulkCopy.ColumnMappings.Add(22, 58);  //diff_perc                                
                                            sqlBulkCopy.WriteToServer(dt_cdnr);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                   // error = "CDNR ";
                                    TempData["Error"] += "CDNR "+" ";
                                }
                                break;
                            #endregion
                            case "CDNRA":                 //14
                                #region "CDNRA"
                                string query28 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda13 = new OleDbDataAdapter(query28, excel_con);
                                OleDbCommand command13 = new OleDbCommand(query28, excel_con);
                                OleDbDataReader dr14 = command13.ExecuteReader();
                                DataTable dt_cdnra = new DataTable();
                                int count16 = dr14.FieldCount;
                                if (count16 == 16)
                                {
                                    oda13.Fill(dt_cdnra);
                                    columnname = dt_cdnra.Columns[0].ColumnName;
                                    columnname1 = dt_cdnra.Columns[1].ColumnName;
                                    if (columnname == "GSTIN/UIN of Recipient" && columnname1 == "Receiver Name")
                                    {
                                    }
                                    else
                                    {
                                        string query29 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda13 = new OleDbDataAdapter(query29, excel_con);
                                        command13 = new OleDbCommand(query29, excel_con);
                                        dr14 = command13.ExecuteReader();
                                        dt_cdnra = new DataTable();
                                        oda13.Fill(dt_cdnra);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_cdnra.Columns[3].DataType;
                                        Type type1 = dt_cdnra.Columns[5].DataType;
                                        Type type2 = dt_cdnra.Columns[7].DataType;
                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_cdnra.Columns.Add("NewInvDate", typeof(string));
                                            dt_cdnra.Columns.Add("NewInvDate1", typeof(string));
                                            dt_cdnra.Columns.Add("NewInvDate2", typeof(string));
                                        }
                                        dt_cdnra.Columns.Add("gt", typeof(string));
                                        dt_cdnra.Columns.Add("cgt", typeof(string));
                                        dt_cdnra.Columns.Add("fp", typeof(string));
                                        dt_cdnra.Columns.Add("gstin", typeof(string));
                                        dt_cdnra.Columns.Add("doctyp", typeof(string));
                                        dt_cdnra.Columns.Add("igst", typeof(string));
                                        dt_cdnra.Columns.Add("cgst", typeof(string));
                                        dt_cdnra.Columns.Add("sgst", typeof(string));
                                        dt_cdnra.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_cdnra.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Original Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Original Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate2"] = Convert.ToDateTime(Convert.ToString(dr["Revised Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_cdnra.Columns.Remove("Original Note/Refund Voucher date");
                                            dt_cdnra.Columns.Remove("Original Invoice/Advance Receipt date");
                                            dt_cdnra.Columns.Remove("Revised Note/Refund Voucher date");
                                            dt_cdnra.Columns["NewInvDate"].SetOrdinal(3);
                                            dt_cdnra.Columns["NewInvDate1"].SetOrdinal(5);
                                            dt_cdnra.Columns["NewInvDate2"].SetOrdinal(7);
                                        }
                                        foreach (DataRow dr in dt_cdnra.Rows)
                                        {
                                            try
                                            {
                                                if (dt_cdnra.Rows.Count >= 1)
                                                {
                                                    dr[16] = gto;
                                                    dr[17] = cgto;
                                                    dr[18] = fp;
                                                    dr[19] = gstinno;
                                                    var sheet_name = "CDNRA";
                                                    dr[20] = sheet_name;
                                                    dr[24] = Fileid;

                                                    var dt = dr[3].ToString();
                                                    var dt1 = dr[5].ToString();
                                                    var dt2 = dr[7].ToString();
                                                    if (dt == "" || dt1 == "" || dt2 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                        dt2 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[3] = dt;
                                                        dr[5] = dt1;
                                                        dr[7] = dt2;
                                                    }

                                                    //if (dr[8].ToString() == "")
                                                    //{
                                                    //    dr[8] = "NULL";
                                                    //}
                                                    //else
                                                    //{
                                                        var note_typ = dr[8].ToString();
                                                        if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                        {
                                                            dr[8] = "CREDIT";
                                                        }
                                                        else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                        {
                                                            dr[8] = "DEBIT";
                                                        }
                                                        else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                        {
                                                            dr[8] = "REFUND";
                                                        }
                                                        else
                                                        {
                                                            dr[8] = note_typ;
                                                        }
                                                  //  }
                                                    if (dr[0].ToString() == "" || dr[9].ToString() == "" || dr[12].ToString() == "" || dr[13].ToString() == "")
                                                    {
                                                        var ctin = dr[0].ToString();
                                                        ctin = "NULL";
                                                        var sply_typ = dr[9].ToString();
                                                        sply_typ = "NULL";
                                                        var rt = Convert.ToString(dr[12].ToString());
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[13].ToString());
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                        var ctin = dr[0].ToString();
                                                        ctin = ctin.ToString().Remove(ctin.ToString().Length - 13);  //ctin
                                                                                                                     // var diff_perc = dr[11].ToString();
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (ctin.ToString() == gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[22] = (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 200;
                                                            dr[23] = dr[22];
                                                            dr[21] = 0.00;
                                                            // }
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[22] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 20000;
                                                            //    dr[23] = dr[22];
                                                            //    dr[21] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //    dr[23] = 0.00;
                                                            //}
                                                        }
                                                        else if (ctin.ToString() != gstin)
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[21] = (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 100;
                                                            dr[22] = 0.00;
                                                            dr[23] = 0.00;
                                                            //}
                                                            //else if (diff_perc == "65.00")
                                                            //{
                                                            //    dr[21] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[13].ToString()) * Convert.ToDecimal(dr[12].ToString())) / 10000;
                                                            //    dr[22] = 0.00;
                                                            //    dr[23] = 0.00;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[21] = 0.00;
                                                            //    dr[22] = 0.00;
                                                            //    dr[23] = 0.00;
                                                            //}
                                                        }
                                                        else
                                                        {
                                                            dr[21] = 0.00;
                                                            dr[22] = 0.00;
                                                            dr[23] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 7); //ctin
                                            sqlBulkCopy.ColumnMappings.Add(1, 61); //rec_name
                                            sqlBulkCopy.ColumnMappings.Add(2, 56); //or_nt_num
                                            sqlBulkCopy.ColumnMappings.Add(3, 57); //or_nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(4, 54); //oinum
                                            sqlBulkCopy.ColumnMappings.Add(5, 55); //oidt
                                            sqlBulkCopy.ColumnMappings.Add(6, 34); //nt_num
                                            sqlBulkCopy.ColumnMappings.Add(7, 35); //nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(8, 33); //ntty
                                            sqlBulkCopy.ColumnMappings.Add(9, 28); //sply_type
                                            sqlBulkCopy.ColumnMappings.Add(10, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(11, 58); //app_%_tax_rt
                                            sqlBulkCopy.ColumnMappings.Add(12, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(13, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(14, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(15, 37); //p_gst
                                            sqlBulkCopy.ColumnMappings.Add(16, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(17, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(18, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(19, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(20, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(21, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(22, 20);  //cgst
                                            sqlBulkCopy.ColumnMappings.Add(23, 21);  //sgst
                                            sqlBulkCopy.ColumnMappings.Add(24, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_cdnra);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    error = "CDNRA ";
                                    TempData["Error"] += "CDNRA "+" ";
                                }
                                break;
                            #endregion
                            case "CDNUR":                 //15
                                #region "CDNUR"        
                                string query30 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda14 = new OleDbDataAdapter(query30, excel_con);
                                OleDbCommand command14 = new OleDbCommand(query30, excel_con);
                                OleDbDataReader dr15 = command14.ExecuteReader();
                                DataTable dt_cdnur = new DataTable();
                                int count8 = dr15.FieldCount;
                                if (count8 == 13)
                                {
                                    oda14.Fill(dt_cdnur);
                                    columnname = dt_cdnur.Columns[0].ColumnName;
                                    columnname1 = dt_cdnur.Columns[1].ColumnName;
                                    if (columnname == "UR Type" && columnname1 == "Note/Refund Voucher Number")
                                    {
                                    }
                                    else
                                    {
                                        string query31 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda14 = new OleDbDataAdapter(query31, excel_con);
                                        command14 = new OleDbCommand(query31, excel_con);
                                        dr15 = command14.ExecuteReader();
                                        dt_cdnur = new DataTable();
                                        oda14.Fill(dt_cdnur);
                                    }
                                    columnname6 = dt_cdnur.Columns[6].ColumnName;
                                    if (columnname6 == "Place Of Supply")
                                    {
                                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                        {
                                            Type type = dt_cdnur.Columns[2].DataType;
                                            Type type1 = dt_cdnur.Columns[5].DataType;
                                            if (type == typeof(string) && type1 == typeof(string))
                                            {
                                            }
                                            else
                                            {
                                                dt_cdnur.Columns.Add("NewInvDate", typeof(string));
                                                dt_cdnur.Columns.Add("NewInvDate1", typeof(string));
                                            }
                                            dt_cdnur.Columns.Add("gt", typeof(string));
                                            dt_cdnur.Columns.Add("cgt", typeof(string));
                                            dt_cdnur.Columns.Add("fp", typeof(string));
                                            dt_cdnur.Columns.Add("gstin", typeof(string));
                                            dt_cdnur.Columns.Add("doctyp", typeof(string));
                                            dt_cdnur.Columns.Add("igst", typeof(string));
                                            dt_cdnur.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                            if (type == typeof(string) && type1 == typeof(string))
                                            {
                                            }
                                            else
                                            {
                                                foreach (DataRow dr in dt_cdnur.Rows)
                                                {
                                                    try
                                                    {
                                                        dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                        dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    }
                                                    catch { continue; }
                                                }
                                                dt_cdnur.Columns.Remove("Note/Refund Voucher date");
                                                dt_cdnur.Columns.Remove("Invoice/Advance Receipt date");
                                                dt_cdnur.Columns["NewInvDate"].SetOrdinal(2);
                                                dt_cdnur.Columns["NewInvDate1"].SetOrdinal(5);
                                            }
                                            foreach (DataRow dr in dt_cdnur.Rows)
                                            {
                                                try
                                                {
                                                    if (dt_cdnur.Rows.Count >= 1)
                                                    {
                                                        dr[13] = gto;
                                                        dr[14] = cgto;
                                                        dr[15] = fp;
                                                        dr[16] = gstinno;
                                                        var sheet_name = "CDNUR";
                                                        dr[17] = sheet_name;
                                                        dr[19] = Fileid;

                                                        var dt = dr[2].ToString();
                                                        var dt1 = dr[5].ToString();
                                                        if (dt == "" || dt1 == "")
                                                        {
                                                            dt = "NULL";
                                                            dt1 = "NULL";
                                                        }
                                                        else
                                                        {
                                                            dr[2] = dt;
                                                            dr[5] = dt1;
                                                        }
                                                        //if (dr[3].ToString() == "")
                                                        //{
                                                        //    dr[3] = "NULL";
                                                        //}
                                                        //else
                                                        //{
                                                            var note_typ = dr[3].ToString();

                                                            if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                            {
                                                                dr[3] = "CREDIT";
                                                            }
                                                            else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                            {
                                                                dr[3] = "DEBIT";
                                                            }
                                                            else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                            {
                                                                dr[3] = "REFUND";
                                                            }
                                                            else
                                                            {
                                                                dr[3] = note_typ;
                                                            }
                                                       // }
                                                        //if (dr[0].ToString() == "")
                                                        //{
                                                        //    dr[0] = "NULL";
                                                        //}
                                                        //else
                                                        //{
                                                            var cdnur_typ = dr[0].ToString();

                                                            if (cdnur_typ == "EXPWP" || cdnur_typ == "expwp" || cdnur_typ == "Expwp")
                                                            {
                                                                dr[0] = "EXPWP";
                                                            }
                                                            else if (cdnur_typ == "EXPWOP" || cdnur_typ == "expwop" || cdnur_typ == "Expwop")
                                                            {
                                                                dr[0] = "EXPWOP";
                                                            }
                                                            else if (cdnur_typ == "b2cl" || cdnur_typ == "B2CL" || cdnur_typ == "B2cl")
                                                            {
                                                                dr[0] = "B2CL";
                                                            }
                                                            else
                                                            {
                                                                dr[0] = cdnur_typ;
                                                            }
                                                       // }
                                                        if (dr[6].ToString() == "" || dr[9].ToString() == "" || dr[10].ToString() == "")
                                                        {
                                                            var poss = dr[6].ToString();
                                                            poss = "NULL";
                                                            var rt = Convert.ToString(dr[9].ToString());
                                                            rt = "NULL";
                                                            var tx = Convert.ToString(dr[10].ToString());
                                                            tx = "NULL";
                                                        }
                                                        else
                                                        {
                                                            dr[6] = (dr[6].ToString()).Substring(0, 2);  //pos

                                                            var gstin = gstinno.Remove(gstinno.Length - 13); //gstin                          
                                                            // var diff_perc = dr[8].ToString();
                                                            if (dr[6].ToString() != gstin && ((cdnur_typ == "EXPWP") || (cdnur_typ == "expwp") || (cdnur_typ == "Expwp") || (cdnur_typ == "B2CL") || (cdnur_typ == "b2cl") || (cdnur_typ == "B2cl")))
                                                            {
                                                                //if (diff_perc == "")
                                                                //{
                                                                dr[18] = (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 100;
                                                                //}
                                                                //else if (diff_perc == "65.00" || diff_perc == "")
                                                                //{
                                                                //    dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 10000;
                                                                //}
                                                                //else
                                                                //{
                                                                //    dr[18] = 0.00;
                                                                //}
                                                            }
                                                            else if ((dr[6].ToString() == gstin) && ((cdnur_typ == "EXPWP") || (cdnur_typ == "expwp") || (cdnur_typ == "Expwp") || (cdnur_typ == "B2CL") || (cdnur_typ == "b2cl") || (cdnur_typ == "B2cl")))
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                            else if ((dr[6].ToString() == gstin) || (dr[6].ToString() != gstin) && (cdnur_typ == "EXPWOP") || (cdnur_typ == "expwop") || (cdnur_typ == "Expwop"))
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                            else
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch
                                                {
                                                    continue;
                                                }
                                            }
                                            try
                                            {
                                                sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                                sqlBulkCopy.ColumnMappings.Add(0, 38); //cdnur_typ
                                                sqlBulkCopy.ColumnMappings.Add(1, 34); //nt_num
                                                sqlBulkCopy.ColumnMappings.Add(2, 35); //nt_dt
                                                sqlBulkCopy.ColumnMappings.Add(3, 33); //ntty
                                                sqlBulkCopy.ColumnMappings.Add(4, 9); //inum
                                                sqlBulkCopy.ColumnMappings.Add(5, 10); //idt
                                                sqlBulkCopy.ColumnMappings.Add(6, 25); //pos
                                                sqlBulkCopy.ColumnMappings.Add(7, 24); //val
                                                sqlBulkCopy.ColumnMappings.Add(8, 58); //diff_perc
                                                sqlBulkCopy.ColumnMappings.Add(9, 17); //rt
                                                sqlBulkCopy.ColumnMappings.Add(10, 18); //txval
                                                sqlBulkCopy.ColumnMappings.Add(11, 22); //csamt
                                                sqlBulkCopy.ColumnMappings.Add(12, 37); //p_gst
                                                sqlBulkCopy.ColumnMappings.Add(13, 39);  //gt
                                                sqlBulkCopy.ColumnMappings.Add(14, 40);  //cur_gt
                                                sqlBulkCopy.ColumnMappings.Add(15, 6);  //fp
                                                sqlBulkCopy.ColumnMappings.Add(16, 5);  //gstin
                                                sqlBulkCopy.ColumnMappings.Add(17, 4);  //doctyp
                                                sqlBulkCopy.ColumnMappings.Add(18, 19);  //igst
                                                sqlBulkCopy.ColumnMappings.Add(19, 0);  //fileid
                                                sqlBulkCopy.WriteToServer(dt_cdnur);
                                                sqlBulkCopy.Close();
                                            }
                                            catch { continue; }
                                        }
                                    }
                                    else if (columnname6 == "Reason For Issuing document")
                                    {
                                        //  oda14.Fill(dt_cdnur);
                                        columnname = dt_cdnur.Columns[0].ColumnName;
                                        columnname1 = dt_cdnur.Columns[1].ColumnName;
                                        if (columnname == "UR Type" && columnname1 == "Note/Refund Voucher Number")
                                        {
                                        }
                                        else
                                        {
                                            string query32 = "SELECT * FROM [" + sheet + "A4:AA]";
                                            oda14 = new OleDbDataAdapter(query32, excel_con);
                                            command14 = new OleDbCommand(query32, excel_con);
                                            dr15 = command14.ExecuteReader();
                                            dt_cdnur = new DataTable();
                                            oda14.Fill(dt_cdnur);
                                        }
                                        using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                        {
                                            Type type = dt_cdnur.Columns[2].DataType;
                                            Type type1 = dt_cdnur.Columns[5].DataType;
                                            if (type == typeof(string) || type1 == typeof(string))
                                            {
                                            }
                                            else
                                            {
                                                dt_cdnur.Columns.Add("NewInvDate", typeof(string));
                                                dt_cdnur.Columns.Add("NewInvDate1", typeof(string));
                                            }
                                            dt_cdnur.Columns.Add("gt", typeof(string));
                                            dt_cdnur.Columns.Add("cgt", typeof(string));
                                            dt_cdnur.Columns.Add("fp", typeof(string));
                                            dt_cdnur.Columns.Add("gstin", typeof(string));
                                            dt_cdnur.Columns.Add("doctyp", typeof(string));
                                            dt_cdnur.Columns.Add("igst", typeof(string));
                                            dt_cdnur.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                            dt_cdnur.Columns.Add("diff_perc", typeof(string));

                                            if (type == typeof(string) || type1 == typeof(string))
                                            {
                                            }
                                            else
                                            {
                                                foreach (DataRow dr in dt_cdnur.Rows)
                                                {
                                                    try
                                                    {
                                                        dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                        dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    }
                                                    catch { continue; }
                                                }
                                                dt_cdnur.Columns.Remove("Note/Refund Voucher date");
                                                dt_cdnur.Columns.Remove("Invoice/Advance Receipt date");
                                                dt_cdnur.Columns["NewInvDate"].SetOrdinal(2);
                                                dt_cdnur.Columns["NewInvDate1"].SetOrdinal(5);
                                            }
                                            foreach (DataRow dr in dt_cdnur.Rows)
                                            {
                                                try
                                                {
                                                    if (dt_cdnur.Rows.Count >= 1)
                                                    {
                                                        dr[13] = gto;
                                                        dr[14] = cgto;
                                                        dr[15] = fp;
                                                        dr[16] = gstinno;
                                                        var sheet_name = "CDNUR";
                                                        dr[17] = sheet_name;
                                                        dr[19] = Fileid;
                                                        dr[20] = "NULL";

                                                        var dt = dr[2].ToString();
                                                        var dt1 = dr[5].ToString();
                                                        if (dt == "" || dt1 == "")
                                                        {
                                                            dt = "NULL";
                                                            dt1 = "NULL";
                                                        }
                                                        else
                                                        {
                                                            dr[2] = dt;
                                                            dr[5] = dt1;
                                                        }

                                                        var note_typ = dr[3].ToString();

                                                        if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                        {
                                                            dr[3] = "CREDIT";
                                                        }
                                                        else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                        {
                                                            dr[3] = "DEBIT";
                                                        }
                                                        else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                        {
                                                            dr[3] = "REFUND";
                                                        }
                                                        else
                                                        {
                                                            dr[3] = note_typ;
                                                        }
                                                        var cdnur_typ = dr[0].ToString();
                                                        var typ1 = "EXPWP";
                                                        var typ2 = "EXPWOP";
                                                        var typ3 = "B2CL";
                                                        if (cdnur_typ == "EXPWP" || cdnur_typ == "expwp" || cdnur_typ == "Expwp")
                                                        {
                                                            dr[0] = typ1;
                                                        }
                                                        else if (cdnur_typ == "EXPWOP" || cdnur_typ == "expwop" || cdnur_typ == "Expwop")
                                                        {
                                                            dr[0] = typ2;
                                                        }
                                                        else if (cdnur_typ == "b2cl" || cdnur_typ == "B2CL" || cdnur_typ == "B2cl")
                                                        {
                                                            dr[0] = typ3;
                                                        }
                                                        else
                                                        {
                                                            dr[0] = cdnur_typ;
                                                        }
                                                        if (dr[7].ToString() == "" || dr[9].ToString() == "" || dr[10].ToString() == "")
                                                        {
                                                            var poss = dr[7].ToString();
                                                            poss = "NULL";
                                                            var rt = Convert.ToString(dr[9].ToString());
                                                            rt = "NULL";
                                                            var tx = Convert.ToString(dr[10].ToString());
                                                            tx = "NULL";
                                                        }
                                                        else
                                                        {

                                                            dr[7] = (dr[7].ToString()).Substring(0, 2);  //pos
                                                            var gstin = gstinno.Remove(gstinno.Length - 13); //gstin

                                                            if (dr[7].ToString() != gstin && ((cdnur_typ == "EXPWP") || (cdnur_typ == "expwp") || (cdnur_typ == "Expwp") || (cdnur_typ == "B2CL") || (cdnur_typ == "b2cl") || (cdnur_typ == "B2cl")))
                                                            {
                                                                dr[18] = (Convert.ToDecimal(dr[10].ToString()) * Convert.ToDecimal(dr[9].ToString())) / 100;
                                                            }
                                                            else if ((dr[7].ToString() == gstin) && ((cdnur_typ == "EXPWP") || (cdnur_typ == "expwp") || (cdnur_typ == "Expwp") || (cdnur_typ == "B2CL") || (cdnur_typ == "b2cl") || (cdnur_typ == "B2cl")))
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                            else if (((dr[7].ToString() == gstin) || (dr[7].ToString() != gstin)) && (cdnur_typ == "EXPWOP") || (cdnur_typ == "expwop") || (cdnur_typ == "Expwop"))
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                            else
                                                            {
                                                                dr[18] = 0.00;
                                                            }
                                                        }
                                                    }
                                                }
                                                catch { continue; }
                                            }
                                            try
                                            {
                                                sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                                sqlBulkCopy.ColumnMappings.Add(0, 38); //cdnur_typ
                                                sqlBulkCopy.ColumnMappings.Add(1, 34); //nt_num
                                                sqlBulkCopy.ColumnMappings.Add(2, 35); //nt_dt
                                                sqlBulkCopy.ColumnMappings.Add(3, 33); //ntty
                                                sqlBulkCopy.ColumnMappings.Add(4, 9); //inum
                                                sqlBulkCopy.ColumnMappings.Add(5, 10); //idt
                                                sqlBulkCopy.ColumnMappings.Add(6, 36); //rsn
                                                sqlBulkCopy.ColumnMappings.Add(8, 24); //val
                                                sqlBulkCopy.ColumnMappings.Add(7, 25); //pos
                                                sqlBulkCopy.ColumnMappings.Add(9, 17); //rt
                                                sqlBulkCopy.ColumnMappings.Add(10, 18); //txval
                                                sqlBulkCopy.ColumnMappings.Add(11, 22); //csamt
                                                sqlBulkCopy.ColumnMappings.Add(12, 37); //p_gst
                                                sqlBulkCopy.ColumnMappings.Add(13, 39);  //gt
                                                sqlBulkCopy.ColumnMappings.Add(14, 40);  //cur_gt
                                                sqlBulkCopy.ColumnMappings.Add(15, 6);  //fp
                                                sqlBulkCopy.ColumnMappings.Add(16, 5);  //gstin
                                                sqlBulkCopy.ColumnMappings.Add(17, 4);  //doctyp
                                                sqlBulkCopy.ColumnMappings.Add(18, 19);  //igst
                                                sqlBulkCopy.ColumnMappings.Add(19, 0);  //fileid
                                                sqlBulkCopy.ColumnMappings.Add(20, 58);  //diff_perc
                                                sqlBulkCopy.WriteToServer(dt_cdnur);
                                                sqlBulkCopy.Close();
                                            }
                                            catch { continue; }
                                        }
                                    }
                                }
                                else
                                {
                                    //error = "CDNUR ";
                                    TempData["Error"] += "CDNUR "+" ";
                                }
                                break;
                            #endregion
                            case "CDNURA":                //16
                                #region "CDNURA"
                                string query33 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda15 = new OleDbDataAdapter(query33, excel_con);
                                OleDbCommand command15 = new OleDbCommand(query33, excel_con);
                                OleDbDataReader dr16 = command15.ExecuteReader();
                                DataTable dt_cdnura = new DataTable();
                                int count17 = dr16.FieldCount;
                                if (count17 == 15)
                                {
                                    oda15.Fill(dt_cdnura);
                                    columnname = dt_cdnura.Columns[0].ColumnName;
                                    columnname1 = dt_cdnura.Columns[1].ColumnName;
                                    if (columnname == "UR Type" && columnname1 == "Original Note/Refund Voucher Number")
                                    {
                                    }
                                    else
                                    {
                                        string query34 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda15 = new OleDbDataAdapter(query34, excel_con);
                                        command15 = new OleDbCommand(query34, excel_con);
                                        dr16 = command15.ExecuteReader();
                                        dt_cdnura = new DataTable();
                                        oda15.Fill(dt_cdnura);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_cdnura.Columns[2].DataType;
                                        Type type1 = dt_cdnura.Columns[4].DataType;
                                        Type type2 = dt_cdnura.Columns[6].DataType;
                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_cdnura.Columns.Add("NewInvDate", typeof(string));
                                            dt_cdnura.Columns.Add("NewInvDate1", typeof(string));
                                            dt_cdnura.Columns.Add("NewInvDate2", typeof(string));
                                        }
                                        dt_cdnura.Columns.Add("gt", typeof(string));
                                        dt_cdnura.Columns.Add("cgt", typeof(string));
                                        dt_cdnura.Columns.Add("fp", typeof(string));
                                        dt_cdnura.Columns.Add("gstin", typeof(string));
                                        dt_cdnura.Columns.Add("doctyp", typeof(string));
                                        dt_cdnura.Columns.Add("igst", typeof(string));
                                        dt_cdnura.Columns.Add("cgst", typeof(string));
                                        dt_cdnura.Columns.Add("sgst", typeof(string));
                                        dt_cdnura.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_cdnura.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Original Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Original Invoice/Advance Receipt date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate2"] = Convert.ToDateTime(Convert.ToString(dr["Revised Note/Refund Voucher date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_cdnura.Columns.Remove("Original Note/Refund Voucher date");
                                            dt_cdnura.Columns.Remove("Original Invoice/Advance Receipt date");
                                            dt_cdnura.Columns.Remove("Revised Note/Refund Voucher date");
                                            dt_cdnura.Columns["NewInvDate"].SetOrdinal(2);
                                            dt_cdnura.Columns["NewInvDate1"].SetOrdinal(4);
                                            dt_cdnura.Columns["NewInvDate2"].SetOrdinal(6);
                                        }
                                        foreach (DataRow dr in dt_cdnura.Rows)
                                        {
                                            try
                                            {
                                                if (dt_cdnura.Rows.Count >= 1)
                                                {
                                                    dr[15] = gto;
                                                    dr[16] = cgto;
                                                    dr[17] = fp;
                                                    dr[18] = gstinno;
                                                    var sheet_name = "CDNURA";
                                                    dr[19] = sheet_name;
                                                    dr[21] = Fileid;

                                                    var dt = dr[2].ToString();
                                                    var dt1 = dr[4].ToString();
                                                    var dt2 = dr[6].ToString();
                                                    if (dt == "" || dt1 == "" || dt2 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                        dt2 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                        dr[4] = dt1;
                                                        dr[6] = dt2;
                                                    }

                                                    //if (dr[0].ToString() == "")
                                                    //{
                                                    //    dr[0] = "NULL";
                                                    //}
                                                    //else
                                                    //{
                                                        var cdnur_typ = dr[0].ToString();
                                                        if (cdnur_typ == "EXPWP" || cdnur_typ == "expwp" || cdnur_typ == "Expwp")
                                                        {
                                                            dr[0] = "EXPWP";
                                                        }
                                                        else if (cdnur_typ == "EXPWOP" || cdnur_typ == "expwop" || cdnur_typ == "Expwop")
                                                        {
                                                            dr[0] = "EXPWOP";
                                                        }
                                                        else if (cdnur_typ == "b2cl" || cdnur_typ == "B2CL" || cdnur_typ == "B2cl")
                                                        {
                                                            dr[0] = "B2CL";
                                                        }
                                                        else
                                                        {
                                                            dr[0] = cdnur_typ;
                                                        }
                                                   // }
                                                    //if (dr[3].ToString() == "")
                                                    //{
                                                    //    dr[3] = "NULL";
                                                    //}
                                                    //else
                                                    //{
                                                        var note_typ = dr[3].ToString();

                                                        if (note_typ == "CREDIT" || note_typ == "credit" || note_typ == "Credit" || note_typ == "C")
                                                        {

                                                            dr[3] = "CREDIT";
                                                        }
                                                        else if (note_typ == "DEBIT" || note_typ == "debit" || note_typ == "Debit" || note_typ == "D")
                                                        {

                                                            dr[3] = "DEBIT";
                                                        }

                                                        else if (note_typ == "REFUND" || note_typ == "refund" || note_typ == "Refund" || note_typ == "R")
                                                        {

                                                            dr[3] = "REFUND";
                                                        }
                                                        else
                                                        {
                                                            dr[3] = note_typ;
                                                        }
                                                  //  }
                                                    if (dr[8].ToString() == "" || dr[11].ToString() == "" || dr[12].ToString() == "")
                                                    {
                                                        var sply_typ = dr[8].ToString();
                                                        sply_typ = "NULL";
                                                        var rt = dr[11].ToString();
                                                        rt = "NULL";
                                                        var tx = dr[12].ToString();
                                                        tx = "NULL";
                                                    }
                                                    else
                                                    {
                                                       // var cdnur_typ = dr[0].ToString();
                                                        var gstin = gstinno.Remove(gstinno.Length - 13); //gstin

                                                        if (cdnur_typ == "EXPWP" || cdnur_typ == "expwp" || cdnur_typ == "Expwp" || cdnur_typ == "B2CL" || cdnur_typ == "b2cl" || cdnur_typ == "B2cl")
                                                        {
                                                            dr[20] = (Convert.ToDecimal(dr[12].ToString()) * Convert.ToDecimal(dr[11].ToString())) / 100;
                                                        }

                                                        else if (cdnur_typ == "EXPWOP" || cdnur_typ == "expwop" || cdnur_typ == "Expwop")
                                                        {
                                                            dr[20] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[20] = 0.00;
                                                        }

                                                        //  var diff_perc = dr[10].ToString();
                                                        // var sply_typ = dr[8].ToString();
                                                        //  if (sply_typ == "INTER")
                                                        // {
                                                        //if (diff_perc == "65.00" || diff_perc == "65")
                                                        //{
                                                        // dr[20] = (Convert.ToDecimal(dr[12].ToString()) * Convert.ToDecimal(dr[11].ToString())) / 100;
                                                        // }
                                                        //else if (diff_perc == "")
                                                        //{
                                                        //    dr[20] = (Convert.ToDecimal(dr[12].ToString()) * Convert.ToDecimal(dr[11].ToString())) / 100;
                                                        //}
                                                        //else
                                                        //{
                                                        //    dr[20] = 0.00;
                                                        //}
                                                        //  }

                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 38); //cdnur_typ
                                            sqlBulkCopy.ColumnMappings.Add(1, 56); //or_nt_num
                                            sqlBulkCopy.ColumnMappings.Add(2, 57); //or_nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(3, 54); //or_inum
                                            sqlBulkCopy.ColumnMappings.Add(4, 55); //or_inv_dt
                                            sqlBulkCopy.ColumnMappings.Add(5, 34); //rev_nt_num
                                            sqlBulkCopy.ColumnMappings.Add(6, 35); //rev_nt_dt
                                            sqlBulkCopy.ColumnMappings.Add(7, 33); //ntty
                                            sqlBulkCopy.ColumnMappings.Add(8, 28); //sply_type
                                            sqlBulkCopy.ColumnMappings.Add(9, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(10, 58); //app_%_tax_rt
                                            sqlBulkCopy.ColumnMappings.Add(11, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(12, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(13, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(14, 37); //p_gst
                                            sqlBulkCopy.ColumnMappings.Add(15, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(16, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(17, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(18, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(19, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(20, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(21, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_cdnura);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "CDNURA ";
                                    TempData["Error"] += "CDNURA "+" ";
                                }
                                break;
                            #endregion
                            case "EXP":                   //17
                                #region "EXP"
                                string query35 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda16 = new OleDbDataAdapter(query35, excel_con);
                                OleDbCommand command16 = new OleDbCommand(query35, excel_con);
                                OleDbDataReader dr17 = command16.ExecuteReader();
                                DataTable dt_exp = new DataTable();
                                int count9 = dr17.FieldCount;
                                if (count9 == 11)
                                {
                                    oda16.Fill(dt_exp);
                                    columnname = dt_exp.Columns[0].ColumnName;
                                    columnname1 = dt_exp.Columns[1].ColumnName;
                                    if (columnname == "Export Type" && columnname1 == "Invoice Number")
                                    {
                                    }
                                    else
                                    {
                                        string query36 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda16 = new OleDbDataAdapter(query36, excel_con);
                                        command16 = new OleDbCommand(query36, excel_con);
                                        dr17 = command16.ExecuteReader();
                                        dt_exp = new DataTable();
                                        oda16.Fill(dt_exp);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_exp.Columns[2].DataType;
                                        Type type1 = dt_exp.Columns[6].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_exp.Columns.Add("NewInvDate", typeof(string));
                                            dt_exp.Columns.Add("NewInvDate1", typeof(string));
                                        }
                                        dt_exp.Columns.Add("gt", typeof(string));
                                        dt_exp.Columns.Add("cgt", typeof(string));
                                        dt_exp.Columns.Add("fp", typeof(string));
                                        dt_exp.Columns.Add("gstin", typeof(string));
                                        dt_exp.Columns.Add("doctyp", typeof(string));
                                        dt_exp.Columns.Add("igst", typeof(string));
                                        dt_exp.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_exp.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Shipping Bill Date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_exp.Columns.Remove("Invoice date");
                                            dt_exp.Columns.Remove("Shipping Bill Date");
                                            dt_exp.Columns["NewInvDate"].SetOrdinal(2);
                                            dt_exp.Columns["NewInvDate1"].SetOrdinal(6);
                                        }
                                        foreach (DataRow dr in dt_exp.Rows)
                                        {
                                            try
                                            {
                                                if (dt_exp.Rows.Count >= 1)
                                                {
                                                    dr[11] = gto;
                                                    dr[12] = cgto;
                                                    dr[13] = fp;
                                                    dr[14] = gstinno;
                                                    var sheet_name = "EXP";
                                                    dr[15] = sheet_name;
                                                    dr[17] = Fileid;

                                                    var dt = dr[2].ToString();
                                                    var dt1 = dr[6].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                        dr[6] = dt1;
                                                    }
                                                    if (dr[0].ToString() == "" || dr[8].ToString() == "" || dr[9].ToString() == "")
                                                    {
                                                        var rt = dr[8].ToString();
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[9].ToString());
                                                        tx = "NULL";
                                                        var exptyp = dr[0].ToString();
                                                        exptyp = "NULL";
                                                        
                                                    }
                                                    else
                                                    {
                                                        var exp_typ = dr[0].ToString();
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        // var diff_perc = dr[7].ToString();
                                                        if (exp_typ == "WPAY" || exp_typ == "wpay" || exp_typ == "Wpay")
                                                        {
                                                            //if (diff_perc == "")
                                                            //{
                                                            dr[16] = (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 100;
                                                            // var igst = dr[16].ToString();
                                                            //}
                                                            //else if (diff_perc == "65.00" || diff_perc == "65")
                                                            //{
                                                            //    dr[16] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[9].ToString()) * Convert.ToDecimal(dr[8].ToString())) / 10000;
                                                            //}
                                                            //else
                                                            //{
                                                            //    dr[16] = 0.00;
                                                            //}

                                                        }
                                                        else if (exp_typ == "WOPAY" || exp_typ == "wopay" || exp_typ == "Wopay")
                                                        {
                                                            dr[16] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[16] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 29); //exp_typ
                                            sqlBulkCopy.ColumnMappings.Add(1, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(2, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(3, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(4, 30); //sbpcode
                                            sqlBulkCopy.ColumnMappings.Add(5, 31); //sbnum
                                            sqlBulkCopy.ColumnMappings.Add(6, 32); //sbdt
                                            sqlBulkCopy.ColumnMappings.Add(7, 58); //diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(8, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(9, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(10, 22); //cess
                                            sqlBulkCopy.ColumnMappings.Add(11, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(12, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(13, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(14, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(15, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(16, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(17, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_exp);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else if (count9 == 9)
                                {
                                    oda16.Fill(dt_exp);
                                    columnname = dt_exp.Columns[0].ColumnName;
                                    columnname1 = dt_exp.Columns[1].ColumnName;
                                    if (columnname == "Export Type" && columnname1 == "Invoice Number")
                                    {
                                    }
                                    else
                                    {
                                        string query37 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda16 = new OleDbDataAdapter(query37, excel_con);
                                        command16 = new OleDbCommand(query37, excel_con);
                                        dr17 = command16.ExecuteReader();
                                        dt_exp = new DataTable();
                                        oda16.Fill(dt_exp);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_exp.Columns[2].DataType;
                                        Type type1 = dt_exp.Columns[6].DataType;
                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_exp.Columns.Add("NewInvDate", typeof(string));
                                            dt_exp.Columns.Add("NewInvDate1", typeof(string));
                                        }
                                        dt_exp.Columns.Add("gt", typeof(string));
                                        dt_exp.Columns.Add("cgt", typeof(string));
                                        dt_exp.Columns.Add("fp", typeof(string));
                                        dt_exp.Columns.Add("gstin", typeof(string));
                                        dt_exp.Columns.Add("doctyp", typeof(string));
                                        dt_exp.Columns.Add("igst", typeof(string));
                                        dt_exp.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
                                        dt_exp.Columns.Add("diff_perc", typeof(string));
                                        dt_exp.Columns.Add("cess", typeof(string));

                                        if (type == typeof(string) && type1 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_exp.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Shipping Bill Date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_exp.Columns.Remove("Invoice date");
                                            dt_exp.Columns.Remove("Shipping Bill Date");
                                            dt_exp.Columns["NewInvDate"].SetOrdinal(2);
                                            dt_exp.Columns["NewInvDate1"].SetOrdinal(6);
                                        }
                                        foreach (DataRow dr in dt_exp.Rows)
                                        {
                                            try
                                            {
                                                if (dt_exp.Rows.Count >= 1)
                                                {
                                                    dr[9] = gto;
                                                    dr[10] = cgto;
                                                    dr[11] = fp;
                                                    dr[12] = gstinno;
                                                    var sheet_name = "EXP";
                                                    dr[13] = sheet_name;
                                                    dr[15] = Fileid;
                                                    dr[16] = "NULL";

                                                    var dt = dr[2].ToString();
                                                    var dt1 = dr[6].ToString();
                                                    if (dt == "" || dt1 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                        dr[6] = dt1;
                                                    }

                                                    var exp_typ = dr[0].ToString();
                                                    if (dr[0].ToString() == "" || dr[7].ToString() == "" || dr[8].ToString() == "")
                                                    {
                                                        var poss = dr[7].ToString();
                                                        poss = "NULL";
                                                        var tx = Convert.ToString(dr[8].ToString());
                                                        tx = "NULL";
                                                        var exptyp = dr[0].ToString();
                                                        exptyp = "NULL";
                                                     
                                                    }
                                                    else
                                                    {
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        if (exp_typ == "WPAY" || exp_typ == "wpay" || exp_typ == "Wpay")
                                                        {
                                                            dr[14] = (Convert.ToDecimal(dr[8].ToString()) * Convert.ToDecimal(dr[7].ToString())) / 100;
                                                            var igst = dr[14].ToString();
                                                        }
                                                        else if (exp_typ == "WOPAY" || exp_typ == "wopay" || exp_typ == "Wopay")
                                                        {
                                                            dr[14] = 0.00;
                                                        }
                                                        else
                                                        {
                                                            dr[14] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 29); //exp_typ
                                            sqlBulkCopy.ColumnMappings.Add(1, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(2, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(3, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(4, 30); //sbpcode
                                            sqlBulkCopy.ColumnMappings.Add(5, 31); //sbnum
                                            sqlBulkCopy.ColumnMappings.Add(6, 32); //sbdt
                                            sqlBulkCopy.ColumnMappings.Add(7, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(8, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(9, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(10, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(11, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(12, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(13, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(14, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.ColumnMappings.Add(16, 58);  //diff_perc
                                            sqlBulkCopy.ColumnMappings.Add(17, 22);  //cess
                                            sqlBulkCopy.WriteToServer(dt_exp);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "EXP ";
                                    TempData["Error"] += "EXP "+" ";
                                }
                                break;
                            #endregion
                            case "EXPA":                  //18
                                #region "EXPA"
                                string query38 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda17 = new OleDbDataAdapter(query38, excel_con);
                                OleDbCommand command17 = new OleDbCommand(query38, excel_con);
                                OleDbDataReader dr18 = command17.ExecuteReader();
                                DataTable dt_expa = new DataTable();
                                int count18 = dr18.FieldCount;
                                if (count18 == 13)
                                {
                                    oda17.Fill(dt_expa);
                                    columnname = dt_expa.Columns[0].ColumnName;
                                    columnname1 = dt_expa.Columns[1].ColumnName;
                                    if (columnname == "Export Type" && columnname1 == "Original Invoice Number")
                                    {
                                    }
                                    else
                                    {
                                        string query39 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda17 = new OleDbDataAdapter(query39, excel_con);
                                        command17 = new OleDbCommand(query39, excel_con);
                                        dr18 = command17.ExecuteReader();
                                        dt_expa = new DataTable();
                                        oda17.Fill(dt_expa);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        Type type = dt_expa.Columns[2].DataType;
                                        Type type1 = dt_expa.Columns[4].DataType;
                                        Type type2 = dt_expa.Columns[8].DataType;
                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            dt_expa.Columns.Add("NewInvDate", typeof(string));
                                            dt_expa.Columns.Add("NewInvDate1", typeof(string));
                                            dt_expa.Columns.Add("NewInvDate2", typeof(string));
                                        }
                                        dt_expa.Columns.Add("gt", typeof(string));
                                        dt_expa.Columns.Add("cgt", typeof(string));
                                        dt_expa.Columns.Add("fp", typeof(string));
                                        dt_expa.Columns.Add("gstin", typeof(string));
                                        dt_expa.Columns.Add("doctyp", typeof(string));
                                        dt_expa.Columns.Add("igst", typeof(string));
                                        dt_expa.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        if (type == typeof(string) && type1 == typeof(string) && type2 == typeof(string))
                                        {
                                        }
                                        else
                                        {
                                            foreach (DataRow dr in dt_expa.Rows)
                                            {
                                                try
                                                {
                                                    dr["NewInvDate"] = Convert.ToDateTime(Convert.ToString(dr["Original Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate1"] = Convert.ToDateTime(Convert.ToString(dr["Revised Invoice date"])).ToString("dd-MM-yyyy");
                                                    dr["NewInvDate2"] = Convert.ToDateTime(Convert.ToString(dr["Shipping Bill Date"])).ToString("dd-MM-yyyy");
                                                }
                                                catch { continue; }
                                            }
                                            dt_expa.Columns.Remove("Original Invoice date");
                                            dt_expa.Columns.Remove("Revised Invoice date");
                                            dt_expa.Columns.Remove("Shipping Bill Date");
                                            dt_expa.Columns["NewInvDate"].SetOrdinal(2);
                                            dt_expa.Columns["NewInvDate1"].SetOrdinal(4);
                                            dt_expa.Columns["NewInvDate2"].SetOrdinal(8);
                                        }
                                        foreach (DataRow dr in dt_expa.Rows)
                                        {
                                            try
                                            {
                                                if (dt_expa.Rows.Count >= 1)
                                                {
                                                    dr[13] = gto;
                                                    dr[14] = cgto;
                                                    dr[15] = fp;
                                                    dr[16] = gstinno;
                                                    var sheet_name = "EXPA";
                                                    dr[17] = sheet_name;
                                                    dr[19] = Fileid;


                                                    var dt = dr[2].ToString();
                                                    var dt1 = dr[4].ToString();
                                                    var dt2 = dr[8].ToString();
                                                    if (dt == "" || dt1 == "" || dt2 == "")
                                                    {
                                                        dt = "NULL";
                                                        dt1 = "NULL";
                                                        dt2 = "NULL";
                                                    }
                                                    else
                                                    {
                                                        dr[2] = dt;
                                                        dr[4] = dt1;
                                                        dr[8] = dt2;
                                                    }
                                                    if (dr[0].ToString() == "" || dr[10].ToString() == "" || dr[11].ToString() == "")
                                                    {
                                                        var rt = dr[10].ToString();
                                                        rt = "NULL";
                                                        var tx = Convert.ToString(dr[11].ToString());
                                                        tx = "NULL";
                                                        var exptyp = dr[0].ToString();
                                                        exptyp = "NULL";
                                                      
                                                    }
                                                    else
                                                    {
                                                        var gstin = gstinno.Remove(gstinno.Length - 13);
                                                        var exptyp = Convert.ToString(dr[0].ToString());
                                                        var diff_perc = dr[9].ToString();
                                                        if (exptyp == "WPAY" || exptyp == "wpay" || exptyp == "Wpay")
                                                        {
                                                            dr[18] = (Convert.ToDecimal(dr[11].ToString())) * (Convert.ToDecimal(dr[10].ToString())) / 100;
                                                        }
                                                        //    if (exptyp == "WPAY" && diff_perc == "")
                                                        //{
                                                        //    dr[18] = (Convert.ToDecimal(dr[11].ToString())) * (Convert.ToDecimal(dr[10].ToString())) / 100;
                                                        //}
                                                        //else if (exptyp == "WPAY" && (diff_perc == "65.00") || (diff_perc == "65"))
                                                        //{
                                                        //    dr[18] = (Convert.ToDecimal(diff_perc)) * (Convert.ToDecimal(dr[11].ToString()) * Convert.ToDecimal(dr[10].ToString())) / 10000;
                                                        //}
                                                        else if (exptyp == "WOPAY" || exptyp == "wopay" || exptyp == "Wopay")
                                                        {
                                                            dr[18] = 0.00;
                                                        }
                                                        //    else if (exptyp == "WOPAY" && ((diff_perc == "") || (diff_perc == "65.00") || (diff_perc == "65")))
                                                        //{
                                                        //    dr[18] = 0.00;
                                                        //}
                                                        else
                                                        {
                                                            dr[18] = 0.00;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 29); //exp_typ
                                            sqlBulkCopy.ColumnMappings.Add(1, 54); //oinum
                                            sqlBulkCopy.ColumnMappings.Add(2, 55); //oidt
                                            sqlBulkCopy.ColumnMappings.Add(3, 9); //inum
                                            sqlBulkCopy.ColumnMappings.Add(4, 10); //idt
                                            sqlBulkCopy.ColumnMappings.Add(5, 24); //val
                                            sqlBulkCopy.ColumnMappings.Add(6, 30); //sbpcode
                                            sqlBulkCopy.ColumnMappings.Add(7, 31); //sbnum
                                            sqlBulkCopy.ColumnMappings.Add(8, 32); //sbdt
                                            sqlBulkCopy.ColumnMappings.Add(9, 58); //app_%_tax_rt
                                            sqlBulkCopy.ColumnMappings.Add(10, 17); //rt
                                            sqlBulkCopy.ColumnMappings.Add(11, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(12, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(13, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(14, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(15, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(16, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(17, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(18, 19);  //igst
                                            sqlBulkCopy.ColumnMappings.Add(19, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_expa);
                                            sqlBulkCopy.Close();
                                        }
                                        catch
                                        { continue; }
                                    }
                                }
                                else
                                {
                                    error = "EXPA ";
                                    TempData["Error"] += "EXPA "+" ";
                                }
                                break;
                            #endregion
                            case "HSN":                   //19
                                #region "HSN"
                                string query40 = "SELECT * FROM [" + sheet + "]";
                                OleDbDataAdapter oda18 = new OleDbDataAdapter(query40, excel_con);
                                OleDbCommand command18 = new OleDbCommand(query40, excel_con);
                                OleDbDataReader dr19 = command18.ExecuteReader();
                                DataTable dt_hsn = new DataTable();
                                int count10 = dr19.FieldCount;
                                if (count10 == 10)
                                {
                                    oda18.Fill(dt_hsn);
                                    columnname = dt_hsn.Columns[0].ColumnName;
                                    columnname1 = dt_hsn.Columns[1].ColumnName;
                                    if (columnname == "HSN" && columnname1 == "Description")
                                    {
                                    }
                                    else
                                    {
                                        string query41 = "SELECT * FROM [" + sheet + "A4:AA]";
                                        oda18 = new OleDbDataAdapter(query41, excel_con);
                                        command18 = new OleDbCommand(query41, excel_con);
                                        dr19 = command18.ExecuteReader();
                                        dt_hsn = new DataTable();
                                        oda18.Fill(dt_hsn);
                                    }
                                    using (SqlBulkCopy sqlBulkCopy = new SqlBulkCopy(sqlcon))
                                    {
                                        dt_hsn.Columns.Add("gt", typeof(string));
                                        dt_hsn.Columns.Add("cgt", typeof(string));
                                        dt_hsn.Columns.Add("fp", typeof(string));
                                        dt_hsn.Columns.Add("gstin", typeof(string));
                                        dt_hsn.Columns.Add("doctyp", typeof(string));
                                        dt_hsn.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

                                        foreach (DataRow dr in dt_hsn.Rows)
                                        {
                                            try
                                            {
                                                if (dt_hsn.Rows.Count >= 1)
                                                {
                                                    dr[10] = gto;
                                                    dr[11] = cgto;
                                                    dr[12] = fp;
                                                    dr[13] = gstinno;
                                                    var sheet_name = "HSN";
                                                    dr[14] = sheet_name;
                                                    dr[15] = Fileid;
                                                    if (dr[2].ToString() == "")
                                                    {
                                                        dr[2] = "";
                                                    }
                                                    else
                                                    {
                                                        var uqc = dr[2].ToString();
                                                        if (uqc == "BAG" || uqc == "bag" || uqc == "Bag" || uqc == "BAG-BAGS" || uqc == "Bag-Bags" || uqc == "bag-bags" || uqc == "BAGS" || uqc == "Bags" || uqc == "bags")
                                                        {
                                                            uqc = "BAG";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BAL" || uqc == "bal" || uqc == "Bal" || uqc == "BAL-BALE" || uqc == "Bal-Bale" || uqc == "bal-bale" || uqc == "BALE" || uqc == "Bale" || uqc == "bale")
                                                        {
                                                            uqc = "BAL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BDL" || uqc == "bdl" || uqc == "Bdl" || uqc == "BDL-BUNDLES" || uqc == "Bdl-Bundles" || uqc == "bdl-bundles" || uqc == "BUNDLES" || uqc == "Bundles" || uqc == "bundles")
                                                        {
                                                            uqc = "BDL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BKL" || uqc == "bkl" || uqc == "Bkl" || uqc == "BKL-BUCKLES" || uqc == "Bkl-Buckles" || uqc == "bkl-buckles" || uqc == "BUCKLES" || uqc == "Buckles" || uqc == "buckles")
                                                        {
                                                            uqc = "BKL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BOU" || uqc == "bou" || uqc == "Bou" || uqc == "BOU-BILLION OF UNITS" || uqc == "Bou-Billion Of Units" || uqc == "bou-billion of units" || uqc == "BILLION OF UNITS" || uqc == "Billion Of Units" || uqc == "billion of units" || uqc == "Bou-Billion of units")
                                                        {
                                                            uqc = "BOU";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BOX" || uqc == "box" || uqc == "Box" || uqc == "BOX-BOX" || uqc == "Box-Box" || uqc == "box-box")
                                                        {
                                                            uqc = "BOX";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BTL" || uqc == "btl" || uqc == "Btl" || uqc == "BTL-BOTTLES" || uqc == "Btl-Bottles" || uqc == "btl-bottles" || uqc == "BOTTLES" || uqc == "Bottles" || uqc == "bottles")
                                                        {
                                                            uqc = "BTL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "BUN" || uqc == "bun" || uqc == "Bun" || uqc == "BUN-BUNCHES" || uqc == "Bun-Bunches" || uqc == "bun-bunches" || uqc == "BUNCHES" || uqc == "Bunches" || uqc == "bunches")
                                                        {
                                                            uqc = "BUN";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CAN" || uqc == "can" || uqc == "Can" || uqc == "CAN-CANS" || uqc == "Can-Cans" || uqc == "can-cans" || uqc == "CANS" || uqc == "Cans" || uqc == "cans")
                                                        {
                                                            uqc = "CAN";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CAN" || uqc == "can" || uqc == "Can" || uqc == "CAN-CANS" || uqc == "Can-Cans" || uqc == "can-cans" || uqc == "CANS" || uqc == "Cans" || uqc == "cans")
                                                        {
                                                            uqc = "CAN";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CBM" || uqc == "cbm" || uqc == "Cbm" || uqc == "CBM-CUBIC METERS" || uqc == "Cbm-Cubic Meters" || uqc == "cbm-cubic meters" || uqc == "CUBIC METERS" || uqc == "Cubic Meters" || uqc == "cubic meters")
                                                        {
                                                            uqc = "CBM";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CCM" || uqc == "ccm" || uqc == "Ccm" || uqc == "CCM-CUBIC CENTIMETERS" || uqc == "Ccm-Cubic Centimeters" || uqc == "ccm-cubic centimeters" || uqc == "CUBIC CENTIMETERS" || uqc == "Cubic Centimeters" || uqc == "cubic centimeters")
                                                        {
                                                            uqc = "CCM";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CMS" || uqc == "cms" || uqc == "Cms" || uqc == "CMS-CENTIMETERS" || uqc == "Cms-Centimeters" || uqc == "cms-centimeters" || uqc == "CENTIMETERS" || uqc == "Centimeters" || uqc == "centimeters")
                                                        {
                                                            uqc = "CMS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "CTN" || uqc == "ctn" || uqc == "Ctn" || uqc == "CNT-CARTONS" || uqc == "Ctn-Cartons" || uqc == "ctn-cartons" || uqc == "CARTONS" || uqc == "Cartons" || uqc == "cartons")
                                                        {
                                                            uqc = "CTN";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "DOZ" || uqc == "doz" || uqc == "Doz" || uqc == "DOZ-DOZENS" || uqc == "Doz-Dozens" || uqc == "doz-dozens" || uqc == "DOZENS" || uqc == "Dozens" || uqc == "dozens")
                                                        {
                                                            uqc = "DOZ";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "DRM" || uqc == "drm" || uqc == "Drm" || uqc == "DRM-DRUMS" || uqc == "Drm-Drums" || uqc == "drm-drums" || uqc == "DRUMS" || uqc == "Drums" || uqc == "drums")
                                                        {
                                                            uqc = "DRM";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "GGK" || uqc == "ggk" || uqc == "Ggk" || uqc == "GGK-GREAT GROSS" || uqc == "Ggk-Great Gross" || uqc == "ggk-great gross" || uqc == "GREAT GROSS" || uqc == "Great Gross" || uqc == "great gross")
                                                        {
                                                            uqc = "GGK";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "GMS" || uqc == "gms" || uqc == "Gms" || uqc == "GMS-GRAMMES" || uqc == "Gms-Grammes" || uqc == "gms-grammes" || uqc == "GRAMMES" || uqc == "Grammes" || uqc == "grammes")
                                                        {
                                                            uqc = "GMS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "GRS" || uqc == "grs" || uqc == "Grs" || uqc == "GRS-GROSS" || uqc == "Grs-Gross" || uqc == "grs-gross" || uqc == "GROSS" || uqc == "Gross" || uqc == "gross")
                                                        {
                                                            uqc = "GRS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "GYD" || uqc == "gyd" || uqc == "Gyd" || uqc == "GYD-GROSS YARDS" || uqc == "Gyd-Gross Yards" || uqc == "gyd-gross yards" || uqc == "GROSS YARDS" || uqc == "Gross Yards" || uqc == "gross yards")
                                                        {
                                                            uqc = "GYD";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "KGS" || uqc == "kgs" || uqc == "Ggs" || uqc == "KGS-KILOGRAMS" || uqc == "Kgs-Kilograms" || uqc == "kgs-kilograms" || uqc == "KILOGRAMS" || uqc == "Kilograms" || uqc == "kilograms")
                                                        {
                                                            uqc = "KGS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "KLR" || uqc == "klr" || uqc == "Klr" || uqc == "KLR-KILOLITRE" || uqc == "Klr-Kilolitre" || uqc == "klr-kilolitre" || uqc == "KILOLITRE" || uqc == "Kilolitre" || uqc == "kilolitre")
                                                        {
                                                            uqc = "KLR";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "KME" || uqc == "kme" || uqc == "Kme" || uqc == "KME-KILOMETRE" || uqc == "Kme-Kilometre" || uqc == "kme-kilometre" || uqc == "KILOMETRE" || uqc == "Kilometre" || uqc == "kilometre")
                                                        {
                                                            uqc = "KME";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "MLT" || uqc == "Mlt" || uqc == "mlt" || uqc == "MLT-MILLILITRE" || uqc == "Mlt-Millilitre" || uqc == "mlt-millilitre" || uqc == "MILLILITRE" || uqc == "millilitre" || uqc == "millilitre")
                                                        {
                                                            uqc = "MLT";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "MTR" || uqc == "Mtr" || uqc == "mtr" || uqc == "MTR-METERS" || uqc == "Mtr-Meters" || uqc == "mtr-meters" || uqc == "METERS" || uqc == "Meters" || uqc == "meters")
                                                        {
                                                            uqc = "MTR";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "MTS" || uqc == "Mts" || uqc == "mts" || uqc == "MTS-METRIC TON" || uqc == "Mts-Metric Ton" || uqc == "mtr-metric ton" || uqc == "METRIC TON" || uqc == "Metric Ton" || uqc == "metric ton")
                                                        {
                                                            uqc = "MTS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "NOS" || uqc == "Nos" || uqc == "nos" || uqc == "NOS-NUMBERS" || uqc == "Nos-Numbers" || uqc == "nos-numbers" || uqc == "NUMBERS" || uqc == "Numbers" || uqc == "numbers")
                                                        {
                                                            uqc = "NOS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "PAC" || uqc == "Pac" || uqc == "pac" || uqc == "PAC-PACKS" || uqc == "Pac-Packs" || uqc == "pac-packs" || uqc == "PACKS" || uqc == "Packs" || uqc == "packs")
                                                        {
                                                            uqc = "PAC";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "PCS" || uqc == "Pcs" || uqc == "pcs" || uqc == "PCS-PIECES" || uqc == "Pcs-Pieces" || uqc == "pcs-pieces" || uqc == "PIECES" || uqc == "Pieces" || uqc == "pieces")
                                                        {
                                                            uqc = "PCS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "PRS" || uqc == "Prs" || uqc == "prs" || uqc == "PRS-PAIRS" || uqc == "Prs-Pairs" || uqc == "prs-pairs" || uqc == "PAIRS" || uqc == "Pairs" || uqc == "pairs")
                                                        {
                                                            uqc = "PRS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "QTL" || uqc == "Qtl" || uqc == "qtl" || uqc == "QTL-QUINTAL" || uqc == "Qtl-Quintal" || uqc == "Qtl-Quintal" || uqc == "QUINTAL" || uqc == "Quintal" || uqc == "quintal")
                                                        {
                                                            uqc = "QTL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "ROL" || uqc == "Rol" || uqc == "rol" || uqc == "ROL-ROLLS" || uqc == "Rol-Rolls" || uqc == "rol-rolls" || uqc == "ROLLS" || uqc == "Rolls" || uqc == "rolls")
                                                        {
                                                            uqc = "ROL";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "SET" || uqc == "Set" || uqc == "set" || uqc == "SET-SETS" || uqc == "Set-Sets" || uqc == "set-sets" || uqc == "SETS" || uqc == "Sets" || uqc == "sets")
                                                        {
                                                            uqc = "SET";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "SQF" || uqc == "Sqf" || uqc == "sqf" || uqc == "SQF-SQUARE FEET" || uqc == "Sqf-Square Feet" || uqc == "sqf-square feet" || uqc == "SQUARE FEET" || uqc == "Square Feet" || uqc == "square feet")
                                                        {
                                                            uqc = "SQF";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "SQM" || uqc == "Sqm" || uqc == "sqm" || uqc == "SQM-SQUARE METERS" || uqc == "Sqm-Square Meters" || uqc == "sqm-square meters" || uqc == "SQUARE METERS" || uqc == "Square Meters" || uqc == "square meters")
                                                        {
                                                            uqc = "SQM";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "SQY" || uqc == "Sqy" || uqc == "sqy" || uqc == "SQY-SQUARE YARDS" || uqc == "Sqy-Square Yards" || uqc == "sqy-square yards" || uqc == "SQUARE YARDS" || uqc == "Square Yards" || uqc == "square yards")
                                                        {
                                                            uqc = "SQY";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "TBS" || uqc == "Tbs" || uqc == "tbs" || uqc == "TBS-TABLETS" || uqc == "Tbs-Tablets" || uqc == "tbs-tablets" || uqc == "TABLETS" || uqc == "Tablets" || uqc == "tablets")
                                                        {
                                                            uqc = "TBS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "TGM" || uqc == "Tgm" || uqc == "tgm" || uqc == "TGM-TEN GROSS" || uqc == "Tgm-Ten Gross" || uqc == "tgm-ten gross" || uqc == "TEN GROSS" || uqc == "Ten Gross" || uqc == "ten gross")
                                                        {
                                                            uqc = "TGM";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "THD" || uqc == "Thd" || uqc == "thd" || uqc == "THD-THOUSANDS" || uqc == "Thd-Thousands" || uqc == "thd-thousands" || uqc == "THOUSANDS" || uqc == "Thousands" || uqc == "thousands")
                                                        {
                                                            uqc = "THD";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "TON" || uqc == "Ton" || uqc == "ton" || uqc == "TON-TONNES" || uqc == "Ton-Tonnes" || uqc == "ton-tonnes" || uqc == "TONNES" || uqc == "Tonnes" || uqc == "tonnes")
                                                        {
                                                            uqc = "TON";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "TUB" || uqc == "Tub" || uqc == "tub" || uqc == "TUB-TUBES" || uqc == "Tub-Tubes" || uqc == "tub-tubes" || uqc == "TUBES" || uqc == "Tubes" || uqc == "tubes")
                                                        {
                                                            uqc = "TUB";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "UGS" || uqc == "Ugs" || uqc == "ugs" || uqc == "UGS-US GALLONS" || uqc == "Ugs-Us Gallons" || uqc == "ugs-us gallons" || uqc == "US GALLONS" || uqc == "Us Gallons" || uqc == "us gallons")
                                                        {
                                                            uqc = "UGS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "UNT" || uqc == "Unt" || uqc == "unt" || uqc == "UNT-UNITS" || uqc == "Unt-Units" || uqc == "unt-units" || uqc == "UNITS" || uqc == "Units" || uqc == "units")
                                                        {
                                                            uqc = "UNT";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "YDS" || uqc == "Yds" || uqc == "yds" || uqc == "YDS-YARDS" || uqc == "Yds-Yards" || uqc == "yds-yards" || uqc == "YARDS" || uqc == "Yards" || uqc == "yards")
                                                        {
                                                            uqc = "YDS";
                                                            dr[2] = uqc;
                                                        }
                                                        else if (uqc == "OTH" || uqc == "Oth" || uqc == "oth" || uqc == "OTH-OTHERS" || uqc == "Oth-Others" || uqc == "oth-others" || uqc == "OTHERS" || uqc == "Others" || uqc == "others")
                                                        {
                                                            uqc = "OTH";
                                                            dr[2] = uqc;
                                                        }
                                                        else
                                                        {
                                                            dr[2] = uqc;
                                                        }
                                                    }
                                                }
                                            }
                                            catch { continue; }
                                        }
                                        try
                                        {
                                            sqlBulkCopy.DestinationTableName = "TBL_CSV_GSTR1_RECS_TALLY";
                                            sqlBulkCopy.ColumnMappings.Add(0, 12); //hsncode
                                            sqlBulkCopy.ColumnMappings.Add(1, 13);//hsndesc
                                            sqlBulkCopy.ColumnMappings.Add(2, 14); //uqc
                                            sqlBulkCopy.ColumnMappings.Add(3, 15); //qty
                                            sqlBulkCopy.ColumnMappings.Add(4, 23); //totval
                                            sqlBulkCopy.ColumnMappings.Add(8, 21); //samt
                                            sqlBulkCopy.ColumnMappings.Add(5, 18); //txval
                                            sqlBulkCopy.ColumnMappings.Add(6, 19); //iamt
                                            sqlBulkCopy.ColumnMappings.Add(9, 22); //csamt
                                            sqlBulkCopy.ColumnMappings.Add(7, 20); //camt
                                            sqlBulkCopy.ColumnMappings.Add(10, 39);  //gt
                                            sqlBulkCopy.ColumnMappings.Add(11, 40);  //cur_gt
                                            sqlBulkCopy.ColumnMappings.Add(12, 6);  //fp
                                            sqlBulkCopy.ColumnMappings.Add(13, 5);  //gstin
                                            sqlBulkCopy.ColumnMappings.Add(14, 4);  //doctyp
                                            sqlBulkCopy.ColumnMappings.Add(15, 0);  //fileid
                                            sqlBulkCopy.WriteToServer(dt_hsn);
                                            sqlBulkCopy.Close();
                                        }
                                        catch { continue; }
                                    }
                                }
                                else
                                {
                                    //error = "HSN ";
                                    TempData["Error"] += "HSN "+" ";
                                }
                                break;
                                #endregion
                        }
                    }
                    else
                    { }
                }
                excel_con.Close();
            }
            #region "Error Stored Procedure"
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_GSTR1_EXT_PERF_Tally";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();
                sqlcmd.Parameters.Add("@TemplateTypeId", SqlDbType.TinyInt).Value = 1;
                sqlcmd.Parameters.Add("@IsTallyDoc", SqlDbType.NVarChar).Value = 'Y';

                SqlParameter totalRecordsCount = new SqlParameter();
                totalRecordsCount.ParameterName = "@TotalRecordsCount";
                totalRecordsCount.IsNullable = true;
                totalRecordsCount.DbType = DbType.Int32;
                totalRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(totalRecordsCount);

                SqlParameter processedRecordsCount = new SqlParameter();
                processedRecordsCount.ParameterName = "@ProcessedRecordsCount";
                processedRecordsCount.IsNullable = true;
                processedRecordsCount.DbType = DbType.Int32;
                processedRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(processedRecordsCount);

                SqlParameter errorRecordsCount = new SqlParameter();
                errorRecordsCount.ParameterName = "@ErrorRecordsCount";
                errorRecordsCount.IsNullable = true;
                errorRecordsCount.DbType = DbType.Int32;
                errorRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(errorRecordsCount);

                sqlcmd.Parameters.Add(new SqlParameter("@ErrorRecords", SqlDbType.NVarChar, 1000)).Direction = System.Data.ParameterDirection.Output;
                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(ds);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                    errortype = Convert.ToString(sqlcmd.Parameters["@ErrorRecords"].Value);
                    if (errorRecords == 0)
                    {
                        errortype = "No Error";
                    }
                    else
                    {
                        errortype = errortype.TrimEnd(',');
                    }
                    TempData["TotalRecordsCount"] = totalRecords.ToString();
                    Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
                    TempData["ProcessedRecordsCount"] = processedRecords.ToString();
                    Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
                    TempData["ErrorRecordsCount"] = errorRecords.ToString();
                    Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
                    TempData["ErrorRecords"] = errortype.ToString();
                    Session["ErrorRecords"] = TempData["ErrorRecords"] as string;
                    Session["errors"] = ds;
                    sqlcon.Close();
                }
            }
            DataTable dtVal = new DataTable();
            return ConvertToDictionary(dtVal);
        }
        #endregion
        static DataTable GetSchemaTable(string connectionString)
        {
            using (OleDbConnection connection = new
                       OleDbConnection(connectionString))
            {
                connection.Open();
                DataTable schemaTable = connection.GetOleDbSchemaTable(
                    OleDbSchemaGuid.Tables,
                    new object[] { null, null, null, "TABLE" });
                return schemaTable;
            }
        }
        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();
            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();
            return dictionaryList.ToList<IDictionary>();
        }

        #region "GSTR1 without Amendments"
        public List<IDictionary> GetB2B(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2BRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetB2CS(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2CSRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetB2CL(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CL_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2CLRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }      
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetCDNR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNR_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnrid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDNRRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetEXP(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1EXP_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVEXPRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
           return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetHSN(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1HSN_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["hsnid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVHSNRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetNIL(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1NIL_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["nilid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVNILRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetTXPD(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1TXP_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txpid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVTXPRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetAT(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1AT_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["atid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVATRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }        
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetDOC(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1DOCISSUE_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["docid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVDOCRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }        
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetCDNUR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNUR_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnurid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDNURRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }        
            return ConvertToDictionary(ds.Tables[0]);
        }
        #endregion

        #region "GSTR1amendmentRetrieve"
        public List<IDictionary> GetB2BA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2BA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2BARefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetB2CSA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CSA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2CSARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }    
        public List<IDictionary> GetB2CLA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CLA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2CLARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }    
        public List<IDictionary> GetCDNRA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNRA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnraid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDNRARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetEXPA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1EXPA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVEXPARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetTXPDA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1TXPA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txpaid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVTXPARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetATA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1ATA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["ataid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVATRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetCDNURA(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNURA_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnuraid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDNURARefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        #endregion

        #region "GSTR2"
        public List<IDictionary> GetB2B2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2B_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["b2bid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2B2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetIMPG(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPG_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["impgid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVIMPG2RefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetIMPS(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["impsid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVIMPS2RefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetCDN(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDN_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDN2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetTXI(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXI_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txiid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVTXI2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetHSN2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2HSN_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["hsnid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVHSN2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetNIL2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2NIL_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["nilid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVNIL2RefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }
        public List<IDictionary> GetTXPD2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXPD_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txpdid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVTXPD2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetB2BUR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2BUR_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["b2burid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVB2BUR2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetITC(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2ITCRVSL_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["itcrvslid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVITCRVSL2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetCDNUR2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDNUR_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnurid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    Session["CSVCDNUR2RefIds"] = RefIds;
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        #endregion
        public void PushGSTR1(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", Convert.ToInt32(Session["Cust_ID"])));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", Convert.ToInt32(Session["User_ID"])));
                    dCmd.ExecuteNonQuery();
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 External Tally data uploaded to GSTR Tables.", "");
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public void PushGSTR1Amendments(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1_AMEND_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "TALLY"));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", ""));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", Convert.ToInt32(Session["Cust_ID"])));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", Convert.ToInt32(Session["User_ID"])));
                    dCmd.ExecuteNonQuery();
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 External Tally data uploaded to GSTR Tables.", "");
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}