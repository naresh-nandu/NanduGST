#region Using

using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI.WebControls;
using WeP_BAL.GSTRUpload;
using WeP_DAL.GSTR9Attribute;

#endregion

namespace SmartAdminMvc.Controllers
{
    public class GstrUploadController : Controller
    {
        private static SqlCommand cmd = new SqlCommand();
        private DataSet dsErrorRecords = new DataSet();
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private List<string> exception = new List<string>();
        private string path2 = null;

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]

        #region "CSV File Uploading"
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string PartnerType = "";
            if (Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                PartnerType = "HK";
            }
            else if (Session["Partner_Company"].ToString() == "JK Foundation")
            {
                PartnerType = "JK";
            }
            else
            {
                PartnerType = "WEP";
            }

            ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType("1", Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
            ViewBag.MakerList = LoadDropDowns.GetMakerUserlist(CustId, UserId);
            SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName");
            ViewBag.GSTRList = GSTRlst;

            Session.Remove("TotalRecordsCount");
            Session.Remove("ProcessedRecordsCount");
            Session.Remove("ErrorRecordsCount");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Post)]
        public ActionResult Index(HttpPostedFileBase FileUpload, FormCollection frmcl, string command, int[] ids)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            string PartnerType = "";
            if (Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                PartnerType = "HK";
            }
            else if (Session["Partner_Company"].ToString() == "JK Foundation")
            {
                PartnerType = "JK";
            }
            else
            {
                PartnerType = "WEP";
            }

            try
            {
                string strGSTRId, sourcetype = "", MakerId = "";
                int UserId = Convert.ToInt32(Session["User_ID"].ToString());
                int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();
                strGSTRId = frmcl["ddlGSTR"];
                sourcetype = frmcl["SourceType"];
                MakerId = frmcl["ddlMaker"];
                ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType(sourcetype, Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                ViewBag.MakerList = LoadDropDowns.Exist_GetMakerUserlist(CustId, UserId, MakerId);
                if (strGSTRId != null || strGSTRId == "")
                {
                    TempData["sourcetype"] = Convert.ToString(sourcetype);
                    TempData["strGSTRId"] = Convert.ToString(strGSTRId);
                    SelectList GSTRlist = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                    ViewBag.GSTRList = GSTRlist;
                }
                else
                {
                    SelectList GSTRlist = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName");
                    ViewBag.GSTRList = GSTRlist;
                }
                string actionType = "", fp = "", gstin = "";
                actionType = frmcl["actionType"];
                fp = frmcl["period"];
                gstin = frmcl["gstin"];

                #region "Uploadad CSV File"

                if (command == "Import")
                {
                    SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                    ViewBag.GSTRList = GSTRlst;

                    if (Request.Files.Count > 0)
                    {
                        TempData["strGSTRId"] = Convert.ToString(strGSTRId);
                        var file = Request.Files[0];
                        if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                        {
                            string fileName = Path.GetFileName(FileUpload.FileName);
                            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", UserId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));

                            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            if (FileExtension.Trim() == "csv")
                            {
                                if (file.ContentLength > 0 && file.ContentLength < 5242880)
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    FileUpload.SaveAs(path);
                                    Session["fileName"] = path;

                                    #region "Importing data and Pushing to Corresponding place"

                                    switch (strGSTRId)
                                    {
                                        case "1":
                                            #region "GSTR1 data Import and Pushing to GSTR1 Save"
                                            ImportCSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 CSV File: " + fileName + " is Imported Successfully", "");

                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully";
                                            break;
                                        #endregion
                                        case "2":
                                            #region "GSTR2 data Import and Pushing to GSTR2 Save"
                                            ImportGSTR2CSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 CSV File: " + fileName + " is Imported Successfully", "");

                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully ";
                                            break;
                                        #endregion
                                        case "3":
                                            #region "Import GSTR Amdnt and Pushing to GSTR1 save"
                                            ImportCSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 CSV File: " + fileName + " is Imported Successfully", "");

                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully ";
                                            break;
                                        #endregion
                                        case "4":
                                            #region " GSTR6 data Import and pushing to GSTR6 Save "
                                            ImportGSTR6CSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR6 CSV File: " + fileName + " is Imported Successfully", "");

                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully";
                                            break;
                                        #endregion
                                        case "5":
                                            #region "GSTR4 data Import and pushing to GSTR4 Save"
                                            ImportGSTR4CSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR4 CSV File: " + fileName + " is Imported Successfully", "");
                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully";
                                            break;
                                        #endregion
                                        case "6":
                                            #region "GSTR7 data Import and pushing to GSTR7 Save"
                                            ImportGSTR7CSV(Convert.ToInt32(strGSTRId), path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR7 CSV File: " + fileName + " is Imported Successfully", "");
                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully";
                                            break;

                                             #endregion
                                        case "7":
                                            #region "GSTR9 data Import and pushing to GSTR9 Save"
                                            ImportGSTR9CSV(Convert.ToInt32(strGSTRId),path, UserEmail);
                                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR9 CSV File: " + fileName + " is Imported Successfully", "");
                                            TempData["UploadMessage"] = "GSTR CSV File imported successfully";
                                            break;
                                         #endregion
                                        default:
                                            break;
                                    }
                                    #endregion

                                    GridView gv = new GridView();
                                    gv.DataSource = dsErrorRecords.Tables[0];
                                    gv.DataBind();
                                    Session["Cars"] = gv;

                                    ViewBag.ImportSummary = new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Retrieve_CSV_Data(strGSTRId, Session["fileName"].ToString(), Session["TemplateType"].ToString(), sourcetype);

                                    ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType(sourcetype, Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                                    SelectList GSTRlist1 = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                                    ViewBag.GSTRList = GSTRlist1;
                                    return View();
                                }
                                else
                                {
                                    TempData["UploadMessage"] = "File size should be 1 kb to 5 MB only.";
                                }
                            }
                            else
                            {
                                TempData["UploadMessage"] = "File format should be .csv";
                            }
                        }
                        else
                        {
                            TempData["UploadMessage"] = "Please select a file.";
                        }
                    }
                    else
                    {
                        TempData["UploadMessage"] = "Please select a file.";
                    }
                }
                #endregion

                #region "Deleting CSV Data"
                else if (command == "Delete")
                {
                    new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Delete_CSV_Data(strGSTRId, Session["fileName"].ToString(), actionType, gstin, fp, Session["TemplateType"].ToString(), sourcetype);
                    TempData["UploadMessage"] = "Data Deleted Successfully";
                    ViewBag.ImportSummary = new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Retrieve_CSV_Data(strGSTRId, Session["fileName"].ToString(), Session["TemplateType"].ToString(), sourcetype);
                }
                #endregion

                #region "Deleting CSV Data Of External"
                else if (command == "ExDelete")
                {
                    if (strGSTRId == "7")
                    {
                       new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Delete_View_Sammary_Data_GSTR9(gstin, fp);
                       TempData["UploadMessage"] = "Data Deleted Successfully";
                    }
                    else { 
                    new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Delete_View_Sammary_Data(strGSTRId, actionType, gstin, fp, sourcetype);
                    TempData["UploadMessage"] = "Data Deleted Successfully";
                    ViewBag.externalSummary = GstrUploadBal.Retrieve_View_Summary(strGSTRId, UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    var dt_actions_summary = GstrUploadBal.DT_Retrieve_View_Summary_CSV(Convert.ToString(strGSTRId), UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    Session["PushSummary"] = dt_actions_summary;
                }
                }
                #endregion

                #region "Retreving External data"
                else if (command == "ExRetrive")
                {
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
                    {
                        UserId = Convert.ToInt32(MakerId);
                    }
                    if (strGSTRId == "7")
                    {
                        ViewBag.gstr9summary = GstrUploadBal.Retrieve_View_Summary_GSTR9(Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), UserId);

                    }
                    else
                    {
                        ViewBag.externalSummary = GstrUploadBal.Retrieve_View_Summary(strGSTRId, UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                        var dt_actions_summary = GstrUploadBal.DT_Retrieve_View_Summary_CSV(Convert.ToString(strGSTRId), UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                        Session["PushSummary"] = dt_actions_summary;

                        ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType(sourcetype, Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                    }
                }
                #endregion'

                #region "Pushing Data from EXT and SA"
                else if (command == "Upload")
                {
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
                    {
                        UserId = Convert.ToInt32(MakerId);
                    }
                    string strSourceType = "";
                    if (sourcetype == "1")
                    {
                        strSourceType = "CSV";
                    }
                    if (sourcetype == "2")
                    {
                        strSourceType = "BP";
                    }
                    if (sourcetype == "3")
                    {
                        strSourceType = "POS";
                    }
                    if (sourcetype == "4")
                    {
                        strSourceType = "ERP";
                    }
                    if (sourcetype == "5")
                    {
                        strSourceType = "Manual";
                    }
                    switch (strGSTRId)
                    {
                        case "1":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CS_INV(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "EXP":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_EXP(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "NIL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_NIL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "HSN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_HSN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXP":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_TXP(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "AT":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_AT(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "DOC Issue":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_DOCISSUE(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR1 Data is Uploaded Successfully";
                            break;

                        case "2":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_B2BUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPG":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_IMPG(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_IMPS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDNUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "HSN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_HSN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "NIL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_NIL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXPD":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_TXPD(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXI":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_TXI(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ITCRVSL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_ITCRVSL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            TempData["UploadMessage"] = "GSTR2 Data is Uploaded successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 CSV Data is Uploaded Successfully", "");
                            break;
                        case "3":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2BA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2BA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CSA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CSA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CLA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CLA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "EXPA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_EXPA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNRA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNRA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNRA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNURA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNURA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXPA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_TXPA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ATA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_ATA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 Amendment CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR1 Amendment Data is Uploaded successfully";
                            break;
                        case "4":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_B2BA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDNA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNRA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDNA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ISD":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_ISD(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR6 CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR6 Data is Uploaded successfully";
                            break;
                        case "5":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_CDNR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_CDNUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "AT":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_AT(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXP":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_TXP(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_B2BUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_IMPS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXOS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_TXOS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_B2BA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNRA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_CDNRA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNURA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_CDNURA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ATA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_ATA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXPA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_TXPA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BURA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_B2BURA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPSA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_IMPSA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXOSA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR4_TXOSA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR4 CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR4 Data is Uploaded successfully";
                            break;
                        case "6":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "TDS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR7_TDS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TDSA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR7_TDSA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;

                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            TempData["UploadMessage"] = "GSTR7 Data is Uploaded successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR6 CSV Data is Uploaded Successfully", "");
                            break;

                        case "7":
                            try
                            {
                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR9(Session["CustRefNo"].ToString(), CustId, UserId));
                                
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }

                            TempData["UploadMessage"] = "GSTR9 Data is Uploaded successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR9 CSV Data is Uploaded Successfully", "");
                            break;
                        default:
                            break;

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
                        {
                            //
                        }
                    }
                }

                #endregion


            }
            catch (Exception ex)
            {
                TempData["UploadMessage"] = ex.Message;
            }
            return View();
        }

        #endregion

        #region "CSV Large File Uploading to Blob"

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
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string PartnerType = "";
            if (Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                PartnerType = "HK";
            }
            else if (Session["Partner_Company"].ToString() == "JK Foundation")
            {
                PartnerType = "JK";
            }
            else
            {
                PartnerType = "WEP";
            }

            try
            {
                ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType("1", Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                ViewBag.MakerList = LoadDropDowns.GetMakerUserlist(CustId, UserId);
                SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName");
                ViewBag.GSTRList = GSTRlst;
                ViewBag.TemplateList = LoadDropDowns.GetGSTRList("Template A,Template B");
            }
            catch (Exception Ex)
            {
                TempData["ErrorMsg"] = Ex.Message;
            }
            return View();
        }
        
        [HttpPost]
        public ActionResult Home(FormCollection Form, HttpPostedFileBase FileUpload, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            int UserId = Convert.ToInt32(Session["User_ID"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"].ToString());

            string PartnerType = "";
            if (Session["Partner_Company"].ToString() == "Hamara Kendra")
            {
                PartnerType = "HK";
            }
            else if (Session["Partner_Company"].ToString() == "JK Foundation")
            {
                PartnerType = "JK";
            }
            else
            {
                PartnerType = "WEP";
            }

            try
            {
                string strGSTRId = "", sourcetype = "", MakerId = "", template = "";
                string actionType = "", fp = "", gstin = "", GSTR = "";
                string UserEmail = (from user in db.UserLists
                                    where user.UserId == UserId
                                    select user.Email).SingleOrDefault();
                strGSTRId = Form["ddlGSTR"];
                sourcetype = "1";
                MakerId = Form["ddlMaker"];
                template = Convert.ToString(Form["ddlTemplate"]);
                GSTR = Convert.ToString(Form["ddlGSTR"]);

                actionType = Form["actionType"];
                fp = Form["period"];
                gstin = Form["gstin"];


                ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType("1", Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                ViewBag.MakerList = LoadDropDowns.Exist_GetMakerUserlist(CustId, UserId, MakerId);
                SelectList GSTRlst = new SelectList(db.TBL_GSTR_TYPE, "GSTRId", "GSTRName", Convert.ToString(strGSTRId));
                ViewBag.GSTRList = GSTRlst;
                ViewBag.TemplateList = LoadDropDowns.Exist_GetGSTRList("Template A,Template B", template);

                string token = String.Format("{0}_{1}_{2}_{3}"
                        , Guid.NewGuid()
                        , DateTime.Now.Ticks, CustId, UserId
                        );

                #region "IMPORT DATA"
                if (command == "Import")
                {
                    if (string.IsNullOrEmpty(GSTR))
                    {
                        ViewBag.Error = "please select GSTR";
                        return View();
                    }
                    if (string.IsNullOrEmpty(GSTR))
                    {
                        if (string.IsNullOrEmpty(template))
                        {
                            ViewBag.Error = "please select template";
                            return View();
                        }
                    }
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                        {
                            string fileName = Path.GetFileName(FileUpload.FileName);
                            fileName = Path.GetFileName(FileUpload.FileName);
                            string fileName2 = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_");

                            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            string FileNameWithoutExtension = fileName.Substring(0, fileName.LastIndexOf('.') + 1);
                            #region xls extension
                            if (FileExtension.Trim() == "xls")
                            {
                                if (GSTR.Equals("1"))
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    FileUpload.SaveAs(path);

                                    #region Conversion of xls to CSV file
                                    DataTable dt = new DataTable();
                                    string conString = string.Empty;
                                    //  conString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0;TypeGuessRows=0\";";
                                    conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;MaxScanRows=0\";";

                                    OleDbCommand oleExcelCommand = default(OleDbCommand);
                                    OleDbConnection oleExcelConnection = default(OleDbConnection);
                                    string sSheetName = null;
                                    oleExcelConnection = new OleDbConnection(conString);
                                    oleExcelConnection.Open();

                                    sSheetName = "GSTR1 Template";
                                    oleExcelCommand = oleExcelConnection.CreateCommand();
                                    try
                                    {
                                        oleExcelCommand.CommandText = "Select * From [" + sSheetName + "$] ";
                                        oleExcelCommand.CommandType = CommandType.Text;


                                        using (OleDbDataReader dr2 = oleExcelCommand.ExecuteReader())
                                        {
                                            dt.Load(dr2);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                    oleExcelConnection.Close();
                                    string fileNameWithCsv = FileNameWithoutExtension + "csv";
                                    path2 = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileNameWithCsv);

                                    System.IO.File.Delete(path2);
                                    using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                                    {
                                        List<string> headerValues = new List<string>();
                                        foreach (DataColumn dr in dt.Columns)
                                        {
                                            headerValues.Add(dr.ToString());
                                        }
                                        swr.WriteLine(string.Join(",", headerValues.ToArray()));
                                        int i = 0;
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            object[] array = row.ItemArray;
                                            string InvDateColDate = row[9].ToString();
                                            string ShippingBillDateColDate = row[31].ToString();
                                            string NoteDateColDate = row[34].ToString();
                                           
                                            string FormattedInvDateColDate = "";
                                            string FormattedShippingBillDateColDate = "";
                                            string FormattedNoteDateColDate = "";

                                         
                                            if (!string.IsNullOrEmpty(InvDateColDate))
                                            {
                                                FormattedInvDateColDate = ConvertDate(InvDateColDate, "Y");
                                            }
                                            if (!string.IsNullOrEmpty(ShippingBillDateColDate))
                                            {
                                                FormattedShippingBillDateColDate = ConvertDate(ShippingBillDateColDate, "Y");
                                            }
                                            if (!string.IsNullOrEmpty(NoteDateColDate))
                                            {
                                                FormattedNoteDateColDate = ConvertDate(NoteDateColDate, "Y");
                                            }
                                            for (i = 0; i < array.Length - 1; i++)
                                            {
                                                
                                                if (i == 9)
                                                {
                                                    swr.Write(FormattedInvDateColDate + ",");
                                                }
                                                else if (i == 31)
                                                {
                                                    swr.Write(FormattedShippingBillDateColDate + ",");
                                                }
                                                else if (i == 34)
                                                {
                                                    swr.Write(FormattedNoteDateColDate + ",");
                                                }
                                                else
                                                {
                                                    swr.Write(array[i].ToString() + ",");
                                                }
                                            }
                                            swr.Write(array[i].ToString());
                                            swr.WriteLine();

                                        }
                                    }

                                }
                                if (GSTR.Equals("2"))
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    FileUpload.SaveAs(path);

                                    #region Conversion of xls to CSV file
                                    DataTable dt = new DataTable();
                                    string conString = string.Empty;
                                    // conString = @"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + path + ";Extended Properties=\"Excel 8.0;HDR=Yes;IMEX=0;TypeGuessRows=0 \";";
                                    conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;MaxScanRows=0\";";

                                    OleDbCommand oleExcelCommand = default(OleDbCommand);
                                    OleDbConnection oleExcelConnection = default(OleDbConnection);
                                    string sSheetName = null;
                                    oleExcelConnection = new OleDbConnection(conString);
                                    oleExcelConnection.Open();

                                    sSheetName = "GSTR2";
                                    oleExcelCommand = oleExcelConnection.CreateCommand();
                                    try
                                    {
                                        oleExcelCommand.CommandText = "Select * From [" + sSheetName + "$] ";
                                        oleExcelCommand.CommandType = CommandType.Text;


                                        using (OleDbDataReader dr2 = oleExcelCommand.ExecuteReader())
                                        {
                                            dt.Load(dr2);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                    oleExcelConnection.Close();

                                    string fileNameWithCsv = FileNameWithoutExtension + "csv";
                                    path2 = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileNameWithCsv);

                                    System.IO.File.Delete(path2);
                                    using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                                    {
                                        List<string> headerValues = new List<string>();
                                        foreach (DataColumn dr in dt.Columns)
                                        {
                                            headerValues.Add(dr.ToString());
                                        }
                                        swr.WriteLine(string.Join(",", headerValues.ToArray()));
                                        int i = 0;
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            object[] array = row.ItemArray;
                                            
                                            string InvoiceDateColDate = row[8].ToString();
                                            string RecievedDateColDate = row[61].ToString();
                                            string NoteDateColDate = row[34].ToString();
                                            string BillOfEntryDateColDate = row[29].ToString();

                                            string FormattedInvoiceDateColDate = "";
                                            string FormattedRecievedDateColDate = "";
                                            string FormattedNoteDateColDate = "";
                                            string FormattedBillOfEntryDateColDate = "";
                                          

                                            if (!string.IsNullOrEmpty(RecievedDateColDate))
                                            {
                                                FormattedRecievedDateColDate = ConvertDate(RecievedDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(NoteDateColDate))
                                            {
                                                FormattedNoteDateColDate = ConvertDate(NoteDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(BillOfEntryDateColDate))
                                            {
                                                FormattedBillOfEntryDateColDate = ConvertDate(BillOfEntryDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(InvoiceDateColDate))
                                            {
                                                FormattedInvoiceDateColDate = ConvertDate(InvoiceDateColDate, "Y");
                                            }

                                            for (i = 0; i < array.Length - 1; i++)
                                            {
                                               
                                                if (i == 8)
                                                {
                                                    swr.Write(FormattedInvoiceDateColDate + ",");
                                                }
                                                else if (i == 29)
                                                {
                                                    swr.Write(FormattedBillOfEntryDateColDate + ",");
                                                }
                                                else if (i == 34)
                                                {
                                                    swr.Write(FormattedNoteDateColDate + ",");
                                                }
                                                else if (i == 61)
                                                {
                                                    swr.Write(FormattedRecievedDateColDate + ",");
                                                }
                                                else
                                                {
                                                    swr.Write(array[i].ToString() + ",");
                                                }
                                            }
                                            swr.Write(array[i].ToString());
                                            swr.WriteLine();

                                        }
                                    }

                                }
                                #endregion
                                #endregion

                                if (CheckTheTemplate(path2, template))
                                {
                                    Session["DownloadLink"] = UploadToBlob(path2, fileName2, token, CustId.ToString(), UserId.ToString(), template, GSTR);
                                    System.IO.File.Delete(path2);
                                    if (exception.Count > 0)
                                    {
                                        string excep = "";
                                        string excep1 = "";

                                        foreach (var a in exception)
                                        {
                                            excep = a.ToString();
                                        }
                                        try
                                        {
                                            excep1 = excep.Substring(0, 120);
                                        }
                                        catch (Exception)
                                        {
                                            excep1 = excep;
                                        }
                                        TempData["Error"] = excep1;
                                    }
                                    else
                                    {
                                        if (Session["DownloadLink"] != null)
                                        {
                                            ViewBag.Sucess = "File Uploaded successfuly,Generated error report";
                                        }
                                        else
                                        {
                                            ViewBag.Sucess = "File Uploaded successfuly,No error report";
                                        }
                                    }
                                }
                                else
                                {
                                    ViewBag.Error = "Template is not proper,Please check the file";
                                }
                            }

                            #endregion

                            #region xlsx extension
                            if (FileExtension.Trim() == "xlsx")
                            {
                                if (GSTR.Equals("1"))
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    FileUpload.SaveAs(path);
                                    #region Conversion of xlsx to CSV file
                                    DataTable dt = new DataTable();
                                    string conString = string.Empty;
                                    conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;MaxScanRows=0\";";

                                    OleDbCommand oleExcelCommand = default(OleDbCommand);
                                    OleDbConnection oleExcelConnection = default(OleDbConnection);
                                    string sSheetName = null;
                                    oleExcelConnection = new OleDbConnection(conString);
                                    oleExcelConnection.Open();

                                    sSheetName = "GSTR1 Template";

                                    oleExcelCommand = oleExcelConnection.CreateCommand();
                                    try
                                    {
                                        oleExcelCommand.CommandText = "Select * From [" + sSheetName + "$] ";
                                        oleExcelCommand.CommandType = CommandType.Text;



                                        using (OleDbDataReader dr2 = oleExcelCommand.ExecuteReader())
                                        {
                                            dt.Load(dr2);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }
                                    oleExcelConnection.Close();

                                    string fileNameWithCsv = FileNameWithoutExtension + "csv";
                                    path2 = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileNameWithCsv);

                                    System.IO.File.Delete(path2);
                                    using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                                    {
                                        List<string> headerValues = new List<string>();
                                        foreach (DataColumn dr in dt.Columns)
                                        {
                                            headerValues.Add(dr.ToString());
                                        }
                                        swr.WriteLine(string.Join(",", headerValues.ToArray()));

                                        int i = 0;
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            object[] array = row.ItemArray;
                                            string InvDateColDate = row[9].ToString();
                                            string ShippingBillDateColDate = row[31].ToString();
                                            string NoteDateColDate = row[34].ToString();
                                         
                                            string FormattedInvDateColDate = "";
                                            string FormattedShippingBillDateColDate = "";
                                            string FormattedNoteDateColDate = "";

                                         
                                            if (!string.IsNullOrEmpty(InvDateColDate))
                                            {
                                                FormattedInvDateColDate = ConvertDate(InvDateColDate, "Y");
                                            }
                                            if (!string.IsNullOrEmpty(ShippingBillDateColDate))
                                            {
                                                FormattedShippingBillDateColDate = ConvertDate(ShippingBillDateColDate, "Y");
                                            }
                                            if (!string.IsNullOrEmpty(NoteDateColDate))
                                            {
                                                FormattedNoteDateColDate = ConvertDate(NoteDateColDate, "Y");
                                            }
                                            for (i = 0; i < array.Length - 1; i++)
                                            {
                                                
                                                if (i == 9)
                                                {
                                                    swr.Write(FormattedInvDateColDate + ",");
                                                }
                                                else if (i == 31)
                                                {
                                                    swr.Write(FormattedShippingBillDateColDate + ",");
                                                }
                                                else if (i == 34)
                                                {
                                                    swr.Write(FormattedNoteDateColDate + ",");
                                                }
                                                else
                                                {
                                                    swr.Write(array[i].ToString() + ",");
                                                }
                                            }
                                            swr.Write(array[i].ToString());
                                            swr.WriteLine();

                                        }

                                    }
                                }
                                if (GSTR.Equals("2"))
                                {
                                    string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                    if (System.IO.File.Exists(path))
                                    {
                                        System.IO.File.Delete(path);
                                    }
                                    FileUpload.SaveAs(path);
                                  
                                    string sSheetName = "GSTR2";
                                    DataTable dt = new DataTable();
                                    string conString = string.Empty;
                                     conString = @"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + path + ";Extended Properties=\"Excel 12.0;HDR=YES;IMEX=1;MaxScanRows=0\";";
                                    //conString = "Provider=Microsoft.ACE.OLEDB.12.0;Data Source={0};Extended Properties='Excel 8.0;HDR=YES;IMEX=1;MaxScanRows=0'";
                                    //conString = string.Format(conString, path);
                                    //using (OleDbConnection conn = new OleDbConnection(conString))
                                    //{

                                    //    using (OleDbCommand comm = new OleDbCommand())
                                    //    {
                                    //        comm.CommandText = "Select * from [" + sSheetName + "$]";
                                    //        comm.Connection = conn;
                                    //        using (OleDbDataAdapter da = new OleDbDataAdapter())
                                    //        {
                                    //            da.SelectCommand = comm;
                                    //            da.Fill(dt);

                                    //        }
                                    //    }
                                    //}


                                    OleDbCommand oleExcelCommand = default(OleDbCommand);
                                    OleDbConnection oleExcelConnection = default(OleDbConnection);
                                   
                                    oleExcelConnection = new OleDbConnection(conString);
                                    oleExcelConnection.Open();

                                    sSheetName = "GSTR2";
                                    oleExcelCommand = oleExcelConnection.CreateCommand();
                                    try
                                    {
                                        oleExcelCommand.CommandText = "Select * From [" + sSheetName + "$] ";
                                        oleExcelCommand.CommandType = CommandType.Text;


                                        using (OleDbDataReader dr2 = oleExcelCommand.ExecuteReader())
                                        {
                                            dt.Load(dr2);
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        Console.WriteLine(ex);
                                    }

                                    oleExcelConnection.Close();

                                    string fileNameWithCsv = FileNameWithoutExtension + "csv";
                                    path2 = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileNameWithCsv);

                                    System.IO.File.Delete(path2);
                                    using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                                    {
                                        List<string> headerValues = new List<string>();
                                        foreach (DataColumn dr in dt.Columns)
                                        {
                                            headerValues.Add(dr.ToString());
                                        }
                                        swr.WriteLine(string.Join(",", headerValues.ToArray()));
                                        int i = 0;
                                        foreach (DataRow row in dt.Rows)
                                        {
                                            object[] array = row.ItemArray;

                                         
                                            string InvoiceDateColDate = row[8].ToString();
                                            string RecievedDateColDate = row[61].ToString();
                                            string NoteDateColDate = row[34].ToString();
                                            string BillOfEntryDateColDate = row[29].ToString();

                                            string FormattedInvoiceDateColDate = "";
                                            string FormattedRecievedDateColDate = "";
                                            string FormattedNoteDateColDate = "";
                                            string FormattedBillOfEntryDateColDate = "";

                                           

                                            if (!string.IsNullOrEmpty(RecievedDateColDate))
                                            {
                                                FormattedRecievedDateColDate = ConvertDate(RecievedDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(NoteDateColDate))
                                            {
                                                FormattedNoteDateColDate = ConvertDate(NoteDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(BillOfEntryDateColDate))
                                            {
                                                FormattedBillOfEntryDateColDate = ConvertDate(BillOfEntryDateColDate, "Y");
                                            }

                                            if (!string.IsNullOrEmpty(InvoiceDateColDate))
                                            {
                                                FormattedInvoiceDateColDate = ConvertDate(InvoiceDateColDate, "Y");
                                            }

                                            for (i = 0; i < array.Length - 1; i++)
                                            {
                                                if (i == 8)
                                                {
                                                    swr.Write(FormattedInvoiceDateColDate + ",");
                                                }
                                                else if (i == 29)
                                                {
                                                    swr.Write(FormattedBillOfEntryDateColDate + ",");
                                                }
                                                else if (i == 34)
                                                {
                                                    swr.Write(FormattedNoteDateColDate + ",");
                                                }
                                                else if (i == 61)
                                                {
                                                    swr.Write(FormattedRecievedDateColDate + ",");
                                                }
                                                else
                                                {
                                                    swr.Write(array[i].ToString() + ",");
                                                }
                                            }
                                            swr.Write(array[i].ToString());
                                            swr.WriteLine();

                                        }
                                    }
                                }

                                #endregion

                                if (CheckTheTemplate(path2, template))
                                {
                                    Session["DownloadLink"] = UploadToBlob(path2, fileName2, token, CustId.ToString(), UserId.ToString(), template, GSTR);
                                    if (exception.Count > 0)
                                    {
                                        string excep = "";
                                        string excep1 = "";

                                        foreach (var a in exception)
                                        {
                                            excep = a.ToString();
                                        }
                                        try
                                        {
                                            excep1 = excep.Substring(0, 120);
                                        }
                                        catch (Exception)
                                        {
                                            excep1 = excep;
                                        }
                                        TempData["Error"] = excep1;
                                    }
                                    else
                                    {
                                        if (Session["DownloadLink"] != null)
                                        {
                                            ViewBag.Sucess = "File Uploaded successfuly,Generated error report";
                                        }
                                        else
                                        {
                                            ViewBag.Sucess = "File Uploaded successfuly,No error report";
                                        }
                                    }
                                }
                                else
                                {
                                    ViewBag.Error = "Template is not proper,Please check the file";
                                }


                            }
                            #endregion

                            #region CSV extension
                            if (FileExtension.Trim() == "csv")
                            {
                                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                if (System.IO.File.Exists(path))
                                {
                                    System.IO.File.Delete(path);
                                }
                                FileUpload.SaveAs(path);
                                if (string.IsNullOrEmpty(path2))
                                {
                                    path2 = path;
                                }
                                FileUpload.SaveAs(path);
                                {
                                    if (CheckTheTemplate(path, template))
                                    {
                                        Session["DownloadLink"] = UploadToBlob(path, fileName2, token, CustId.ToString(), UserId.ToString(), template, GSTR);
                                        if (exception.Count > 0)
                                        {
                                            string excep = "";
                                            string excep1 = "";

                                            foreach (var a in exception)
                                            {
                                                excep = a.ToString();
                                            }
                                            try
                                            {
                                                excep1 = excep.Substring(0, 120);
                                            }
                                            catch (Exception)
                                            {
                                                excep1 = excep;
                                            }
                                            TempData["Error"] = excep1;
                                        }
                                        else
                                        {
                                            if (Session["DownloadLink"] != null)
                                            {
                                                ViewBag.Sucess = "File Uploaded successfuly,Generated error report";
                                            }
                                            else
                                            {
                                                ViewBag.Sucess = "File Uploaded successfuly,No error report";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        ViewBag.Error = "Template is not proper,Please check the file";
                                    }
                                }
                            }
                            #endregion
                        }
                        else
                        {
                            ViewBag.Error = "Please select a file.";
                        }
                    }
                }
                #endregion


                #region "Deleting CSV Data"
                else if (command == "Delete")
                {
                    new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Delete_CSV_Data(strGSTRId, Session["fileName"].ToString(), actionType, gstin, fp, Session["TemplateType"].ToString(), sourcetype);
                    TempData["UploadMessage"] = "Data Deleted Successfully";
                    ViewBag.ImportSummary = new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Retrieve_CSV_Data(strGSTRId, Session["fileName"].ToString(), Session["TemplateType"].ToString(), sourcetype);
                }
                #endregion

                #region "Deleting CSV Data Of External"
                else if (command == "ExDelete")
                {
                    new GstrUploadBal(Convert.ToInt32(Session["Cust_ID"]), UserId, UserEmail, Session["CustRefNo"].ToString()).Delete_View_Sammary_Data(strGSTRId, actionType, gstin, fp, sourcetype);
                    TempData["UploadMessage"] = "Data Deleted Successfully";
                    ViewBag.externalSummary = GstrUploadBal.Retrieve_View_Summary(strGSTRId, UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    var dt_actions_summary = GstrUploadBal.DT_Retrieve_View_Summary_CSV(Convert.ToString(strGSTRId), UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    Session["PushSummary"] = dt_actions_summary;
                }
                #endregion

                #region "Retreving External data"
                else if (command == "ExRetrive")
                {
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
                    {
                        UserId = Convert.ToInt32(MakerId);
                    }
                    ViewBag.externalSummary = GstrUploadBal.Retrieve_View_Summary(strGSTRId, UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    var dt_actions_summary = GstrUploadBal.DT_Retrieve_View_Summary_CSV(Convert.ToString(strGSTRId), UserEmail, Session["CustRefNo"].ToString(), Convert.ToInt32(Session["Cust_ID"]), sourcetype, UserId);
                    Session["PushSummary"] = dt_actions_summary;

                    ViewBag.SOURCEList = LoadDropDowns.Exist_GetSourceType(sourcetype, Session["MakerCheckerApproverSetting"].ToString(), Session["MakerCheckerApproverType"].ToString(), PartnerType);
                }
                #endregion

                #region "Pushing Data from EXT and SA"
                else if (command == "Upload")
                {
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
                    {
                        UserId = Convert.ToInt32(MakerId);
                    }
                    string strSourceType = "";
                    if (sourcetype == "1")
                    {
                        strSourceType = "CSV";
                    }
                    if (sourcetype == "2")
                    {
                        strSourceType = "BP";
                    }
                    if (sourcetype == "3")
                    {
                        strSourceType = "POS";
                    }
                    if (sourcetype == "4")
                    {
                        strSourceType = "ERP";
                    }
                    if (sourcetype == "5")
                    {
                        strSourceType = "Manual";
                    }
                    switch (strGSTRId)
                    {
                        case "1":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CS_INV(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "EXP":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_EXP(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "NIL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_NIL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "HSN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_HSN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXP":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_TXP(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "AT":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_AT(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "DOC Issue":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_DOCISSUE(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR1 Data is Uploaded Successfully";
                            break;

                        case "2":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_B2BUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPG":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_IMPG(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "IMPS":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_IMPS(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNUR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_CDNUR(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "HSN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_HSN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "NIL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_NIL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXPD":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_TXPD(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXI":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_TXI(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ITCRVSL":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR2_ITCRVSL(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            TempData["UploadMessage"] = "GSTR2 Data is Uploaded successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 CSV Data is Uploaded Successfully", "");
                            break;

                        case "3":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2BA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2BA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CSA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CSA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2CLA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_B2CLA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "EXPA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_EXPA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNRA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNRA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNRA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNURA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_CDNURA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "TXPA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_TXPA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ATA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR1_ATA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 Amendment CSV Data is Uploaded Successfully", "");
                            TempData["UploadMessage"] = "GSTR1 Amendment Data is Uploaded successfully";
                            break;

                        case "4":
                            try
                            {
                                using (DataTable ds = (DataTable)Session["PushSummary"])
                                {
                                    foreach (DataRow r in ds.Rows)
                                    {
                                        var action = r.ItemArray[0].ToString();

                                        switch (action)
                                        {
                                            case "B2B":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_B2B(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDN":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNR":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDN(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "B2BA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_B2BA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDNA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "CDNRA":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_CDNA(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                            case "ISD":
                                                Task.Factory.StartNew(() => GstrUploadBal.PushGSTR6_ISD(strSourceType, Session["CustRefNo"].ToString(), "", CustId, UserId));
                                                break;
                                        }
                                    }
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine(ex);
                                throw;
                            }

                            TempData["UploadMessage"] = "GSTR6 Data is Uploaded successfully";
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR6 CSV Data is Uploaded Successfully", "");
                            break;
                       

                        default:
                            break;

                    }
                }
                #endregion

                ViewBag.ErrorSummary = Retrieve_Error_Summary(CustId, UserId, GSTR, template);

            }
            catch (Exception Ex)
            {
                TempData["ErrorMsg"] = Ex.Message;
                ViewBag.Error = Ex.Message.ToString();
            }
            return View();
        }
        #endregion


        public static string ConvertDate(string dateToBeFromatted,string periodFormat)
        {
            DateTime asd = DateTime.Parse(dateToBeFromatted);
            string date=null;
            var Date = date;
            if(periodFormat.Equals("Y"))
             Date = asd.ToString("dd/MM/yyyy");
            if(periodFormat.Equals("N"))
               Date = asd.ToString("MMyyyy");

            return Date;
        }



        #region converting datatable into String format
        public static string DataTableToCSV(DataTable datatable, char seperator)
        {
            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < datatable.Columns.Count; i++)
            {
                sb.Append(datatable.Columns[i]);
                if (i < datatable.Columns.Count - 1)
                {
                    sb.Append(seperator);
                }
            }
            sb.AppendLine();
            foreach (DataRow dr in datatable.Rows)
            {
                for (int i = 0; i < datatable.Columns.Count; i++)
                {
                    sb.Append(dr[i].ToString());

                    if (i < datatable.Columns.Count - 1)
                    {
                        sb.Append(seperator);
                    }
                }
                sb.AppendLine();
            }
            return sb.ToString();
        }
        #endregion 

        #region Update

        private bool CheckTheTemplate(string path, string template)
        {
            Stream stream = System.IO.File.OpenRead(path);
            int count = 0;

            var firstLine = System.IO.File.ReadLines(path).First();

            if (firstLine != null)
            {
                count = firstLine.Split(',').Length;
            }
            stream.Close();
            if (template.Equals("Template A") && (count == 56 || count == 64 || count == 52 || count == 62))
            {
                return true;
            }
            else if (template.Equals("Template B") && (count == 54 || count == 62 || count == 48 || count == 62))
            {
                return true;
            }
            else
            {
                return false;
            }
        }


        public string UploadToBlob(string file, string fileName, string token, string clientId, string userId, string template, string GSTR)
        {

            string accountName = "wepdigital";
            string accessKey = "2PmhcsQ7++7SpzjnfiYj170zstfJ5Odjkzdnaek9o5WuvhrpTt5PTPhmS2Ux57rt/2hmXOo/IEeAS/yHvLmcXw==";
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
            string containerName = "wepgst";
            string folderName = "/Upload";
            string url = "";
            string fileNameFinal = fileName + token + ".csv";
            string containerUrl = "";

            string containerFolder = "";

            try
            {

                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                if (template.Equals("Template B"))
                {
                    containerFolder = containerName + folderName + "/Template B";
                }
                else
                {
                    containerFolder = containerName + folderName + "/Template A";
                }

                var container = blobClient.GetContainerReference(containerFolder);

                using (FileStream stream = System.IO.File.Open(file, FileMode.Open))
                {
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileNameFinal);
                    url = cloudBlockBlob.Uri.ToString();

                    cloudBlockBlob.UploadFromStream(stream);
                }
            }
            catch (Exception ex)
            {
                exception.Add(ex.ToString());

            }
            containerUrl = url.Substring(0, url.IndexOf(containerName) + containerName.Length);
            string filepath = url.Substring(url.IndexOf(containerName) + containerName.Length + 1);

            return InsertdataIntoTable(containerName, containerUrl, filepath, fileNameFinal, url, userId, clientId, DateTime.Now, "Upload", "1", GSTR, template);
        }

        public string InsertdataIntoTable(string containername, string containerurl, string filePath, string filename, string bloburl, string Createdby, string CustomerId, DateTime Createddate, string TransType, string Rowstatus, string GSTRType, string TemplateType)
        {
            string TransId = "";
            string dlink = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                string query = "INSERT INTO TBL_Blob_Transactions (Containername, Containerurl,Filepath,bloburl,FileName,Createdby,CustomerId,Createddate,TransType,RowStatus,GSTRType,TemplateType)";
                query += " VALUES (@Containername, @Containerurl, @Filepath,@bloburl,@FileName, @Createdby, @CustomerId, @Createddate, @TransType, @RowStatus,@GSTRType,@TemplateType)";
                query += ";SELECT SCOPE_IDENTITY()";
                try
                {
                    SqlCommand myCommand = new SqlCommand(query, conn);
                    myCommand.Parameters.AddWithValue("@Containername", containername);
                    myCommand.Parameters.AddWithValue("@Containerurl", containerurl);
                    myCommand.Parameters.AddWithValue("@Filepath", filePath);
                    myCommand.Parameters.AddWithValue("@bloburl", bloburl);
                    myCommand.Parameters.AddWithValue("@FileName", filename);
                    myCommand.Parameters.AddWithValue("@Createdby", Createdby);
                    myCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                    myCommand.Parameters.AddWithValue("@Createddate", Createddate);
                    myCommand.Parameters.AddWithValue("@TransType", TransType);
                    myCommand.Parameters.AddWithValue("@RowStatus", Rowstatus);
                    myCommand.Parameters.AddWithValue("@GSTRType", GSTRType);
                    myCommand.Parameters.AddWithValue("@TemplateType", TemplateType);

                    object TransIdTemp = myCommand.ExecuteScalar();
                    TransId = TransIdTemp.ToString();
                    conn.Close();

                    if (GSTRType == "1")
                    {
                        dlink = GSTR1(TransId, CustomerId, Createdby, conn, filename);
                    }
                    else if (GSTRType == "2")
                    {
                        dlink = GSTR2(TransId, CustomerId, Createdby, conn, filename);
                    }
                }
                catch (Exception e)
                {
                    exception.Add(e.ToString());
                }

                return dlink;
            }
        }

        public bool InsertdataIntoTableResponse(string containername, string containerurl, string filePath, string filename, string bloburl, string Createdby, string CustomerId, DateTime Createddate, string TransType, string Rowstatus, string GSTRType, string TemplateType, string MasterTransid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                string query = "INSERT INTO TBL_Blob_Transactions (Containername, Containerurl,Filepath,bloburl,FileName,Createdby,CustomerId,Createddate,TransType,RowStatus,GSTRType,TemplateType,MasterTransid)";
                query += " VALUES (@Containername, @Containerurl, @Filepath,@bloburl,@FileName, @Createdby, @CustomerId, @Createddate, @TransType, @RowStatus,@GSTRType,@TemplateType,@MasterTransid)";
                query += ";SELECT SCOPE_IDENTITY()";
                try
                {
                    SqlCommand myCommand = new SqlCommand(query, conn);
                    myCommand.Parameters.AddWithValue("@Containername", containername);
                    myCommand.Parameters.AddWithValue("@Containerurl", containerurl);
                    myCommand.Parameters.AddWithValue("@Filepath", filePath);
                    myCommand.Parameters.AddWithValue("@bloburl", bloburl);
                    myCommand.Parameters.AddWithValue("@FileName", filename);
                    myCommand.Parameters.AddWithValue("@Createdby", Createdby);
                    myCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                    myCommand.Parameters.AddWithValue("@Createddate", Createddate);
                    myCommand.Parameters.AddWithValue("@TransType", TransType);
                    myCommand.Parameters.AddWithValue("@RowStatus", Rowstatus);
                    myCommand.Parameters.AddWithValue("@GSTRType", GSTRType);
                    myCommand.Parameters.AddWithValue("@TemplateType", TemplateType);
                    myCommand.Parameters.AddWithValue("@MasterTransid", MasterTransid);
                    myCommand.ExecuteScalar();
                    conn.Close();

                    return true;
                }
                catch (Exception e)
                {
                    exception.Add(e.ToString());
                    return false;
                }
            }
        }

        public string GSTR1(string transId, string customerId, string createdby, SqlConnection conn, string fileName)
        {
            DataTable dt = new DataTable();
            string dlink = "";
            string FileName = "Response" + fileName;
            using (SqlCommand sqlcmd = new SqlCommand("USP_Blob_Trans_Gstr1_tempA_Recs", conn))
            {
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@Transid", transId);
                sqlcmd.Parameters.AddWithValue("@Custid", customerId);
                sqlcmd.Parameters.AddWithValue("@Createdby", createdby);
                sqlcmd.CommandTimeout = 0;
                conn.Open();

                try
                {
                    using (SqlDataReader reader = sqlcmd.ExecuteReader())
                    {
                        try
                        {
                            dt.Load(reader);

                            System.IO.File.Delete(path2);
                            using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                            // change buffer size and Encoding to your needs
                            {
                                List<string> headerValues = new List<string>();
                                foreach (DataColumn dr in dt.Columns)
                                {
                                    headerValues.Add(dr.ToString());
                                }
                                swr.WriteLine(string.Join(",", headerValues.ToArray()));
                                foreach (DataRow dr in dt.Rows)
                                {
                                    swr.WriteLine(string.Join(",", dr.ItemArray).Replace("\r", ""));
                                }
                            }


                            using (StringWriter writer = new StringWriter())
                            {
                                Uri link = UploadResponseToTheBlob(path2, FileName, customerId, createdby, transId, "1");
                                dlink = link.ToString();
                                conn.Close();
                            }

                        }
                        catch (Exception ex)
                        {
                            exception.Add(ex.ToString());
                        }
                        return dlink;
                    }
                }
                catch (Exception ex)
                {
                    exception.Add(ex.ToString());
                    return null;
                }
            }
        }

        public string GSTR2(string transId, string customerId, string createdby, SqlConnection conn, string fileName)
        {
            DataTable dt = new DataTable();
            string dlink = "";
            string FileName = "Response" + fileName;
            using (SqlCommand sqlcmd = new SqlCommand("USP_Blob_Trans_Gstr2_tempA_Recs", conn))
            {
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Parameters.AddWithValue("@Transid", transId);
                sqlcmd.Parameters.AddWithValue("@Custid", customerId);
                sqlcmd.Parameters.AddWithValue("@Createdby", createdby);
                sqlcmd.CommandTimeout = 0;
                conn.Open();

                try
                {
                    using (SqlDataReader reader = sqlcmd.ExecuteReader())
                    {
                        try
                        {
                            dt.Load(reader);

                            System.IO.File.Delete(path2);
                            using (StreamWriter swr = new StreamWriter(System.IO.File.Open(path2, FileMode.CreateNew)))
                            // change buffer size and Encoding to your needs
                            {
                                List<string> headerValues = new List<string>();
                                foreach (DataColumn dr in dt.Columns)
                                {
                                    headerValues.Add(dr.ToString());
                                }
                                swr.WriteLine(string.Join(",", headerValues.ToArray()));
                                foreach (DataRow dr in dt.Rows)
                                {
                                    swr.WriteLine(string.Join(",", dr.ItemArray).Replace("\r", ""));
                                }
                            }


                            using (StringWriter writer = new StringWriter())
                            {
                                Uri link = UploadResponseToTheBlob(path2, FileName, customerId, createdby, transId, "2");
                                dlink = link.ToString();
                                conn.Close();
                            }

                        }
                        catch (Exception ex)
                        {
                            exception.Add(ex.ToString());
                        }
                        return dlink;
                    }
                }
                catch (Exception ex)
                {
                    exception.Add(ex.ToString());
                    return null;
                }
            }
        }
        #endregion


        #region Upload response to blob
        public Uri UploadResponseToTheBlob(string file, string fileName, string clientId, string userId, string transid, string GSTR)
        {

            string accountName = "wepdigital";
            string accessKey = "2PmhcsQ7++7SpzjnfiYj170zstfJ5Odjkzdnaek9o5WuvhrpTt5PTPhmS2Ux57rt/2hmXOo/IEeAS/yHvLmcXw==";
            string connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
            string containerName = "wepgst";
            string folderName = "/Error Response";

            string containerUrl = "";
            Uri link = null;

            try
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName + folderName);

                using (FileStream stream = System.IO.File.Open(file, FileMode.Open))
                {
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);
                    link = cloudBlockBlob.Uri;

                    cloudBlockBlob.UploadFromStream(stream);
                }
                string url = link.ToString();

                containerUrl = url.Substring(0, url.IndexOf(containerName) + containerName.Length);
                string filepath = url.Substring(url.IndexOf(containerName) + containerName.Length + 1);
                string GSTRRecord = "GSTR-" + GSTR + " Error Record";
                InsertdataIntoTableResponse(containerName, containerUrl, filepath, fileName, link.ToString(), userId, clientId, DateTime.Now, "Error Record", "1", GSTRRecord, "", transid);

                return link;
            }
            catch (Exception ex)
            {
                exception.Add(ex.ToString());
                return null;
            }
        }
        #endregion

        #region Retrieve Error Summary
        public static List<IDictionary> Retrieve_Error_Summary(int CustId, int UserId, string GSTRType, string TemplateType)
        {
            DataSet ds = new DataSet();
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Uploaded_Files_Summary", con);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    dCmd.Parameters.Add(new SqlParameter("@GSTRType", GSTRType));
                    dCmd.Parameters.Add(new SqlParameter("@TemplateType", TemplateType));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        #endregion

        public ActionResult Download()
        {
            return new DownloadFileActionResult((GridView)Session["Cars"], "ImportErrors.xls");
        }

        #region "CSV Uploding related Methods"

        private void ImportCSV(int GstrTypeId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

            int gstr1TemplateType = 0;
            int templateTypeId = 1; /* This Value is passed to the underlying procedure call */
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                value = line.Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }

            }
            sr.Close();

            sqlcon.Open();

            if (GstrTypeId == 3)
            {
                GstrTypeId = 1;
            }

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            // Check the Type of Template (i.e GSTR1 Template A / GSTR1 Temlate B)

            if (dt.Columns.Count == 57) // Template A
            {
                gstr1TemplateType = 1;
                Session["TemplateType"] = gstr1TemplateType;
                templateTypeId = 1;
            }
            else if (dt.Columns.Count == 55) // Template B
            {
                gstr1TemplateType = 2;
                Session["TemplateType"] = gstr1TemplateType;
                templateTypeId = 2;
            }
            else if (dt.Columns.Count == 65) // Template A (Amendments)
            {
                gstr1TemplateType = 3;
                Session["TemplateType"] = gstr1TemplateType;
                templateTypeId = 1;
            }
            else if (dt.Columns.Count == 63) // Template B (Amendments)
            {
                gstr1TemplateType = 4;
                Session["TemplateType"] = gstr1TemplateType;
                templateTypeId = 2;
            }

            if (gstr1TemplateType == 1)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[56] = fileId;
                }
            }
            else if (gstr1TemplateType == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[54] = fileId;
                }
            }
            else if (gstr1TemplateType == 3)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[64] = fileId;
                }
            }
            else if (gstr1TemplateType == 4)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[62] = fileId;
                }
            }
            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            tableName = "TBL_CSV_GSTR1_RECS";
            copy.DestinationTableName = tableName;


            if (gstr1TemplateType == 1)
            {
                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // Compcode
                copy.ColumnMappings.Add(2, 3); // Unitcode
                copy.ColumnMappings.Add(3, 4); // Doctype
                copy.ColumnMappings.Add(4, 5); // Gstin
                copy.ColumnMappings.Add(5, 6); // Fp
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // Etin
                copy.ColumnMappings.Add(8, 9); // Inum
                copy.ColumnMappings.Add(9, 10); // Idt
                copy.ColumnMappings.Add(10, 11); // Inv_Typ
                copy.ColumnMappings.Add(11, 12); // Hsncode
                copy.ColumnMappings.Add(12, 13); // Hsndesc
                copy.ColumnMappings.Add(13, 14); // Uqc

                copy.ColumnMappings.Add(14, 15); // Qty
                copy.ColumnMappings.Add(15, 16); // Unitprice
                copy.ColumnMappings.Add(16, 17); // Rt
                copy.ColumnMappings.Add(17, 18); // Txval
                copy.ColumnMappings.Add(18, 19); // Iamt
                copy.ColumnMappings.Add(19, 20); // Camt
                copy.ColumnMappings.Add(20, 21); // Samt
                copy.ColumnMappings.Add(21, 22); // Csamt
                copy.ColumnMappings.Add(22, 23); // Totval
                copy.ColumnMappings.Add(23, 24); // Val
                copy.ColumnMappings.Add(24, 25); // Pos
                copy.ColumnMappings.Add(25, 26); // Rchrg
                copy.ColumnMappings.Add(26, 27); // Rrate
                copy.ColumnMappings.Add(27, 28); // Sply_Ty


                copy.ColumnMappings.Add(28, 29); // Exp_Typ 
                copy.ColumnMappings.Add(29, 30); // Sbpcode
                copy.ColumnMappings.Add(30, 31); // Sbnum
                copy.ColumnMappings.Add(31, 32); // Sbdt
                copy.ColumnMappings.Add(32, 33); // Ntty
                copy.ColumnMappings.Add(33, 34); // Nt_Num
                copy.ColumnMappings.Add(34, 35); // Nt_Dt
                copy.ColumnMappings.Add(35, 36); // Rsn
                copy.ColumnMappings.Add(36, 37); // P_Gst
                copy.ColumnMappings.Add(37, 38); // Cdnur_Typ

                // Bypass Source - 38

                copy.ColumnMappings.Add(39, 39); // Gt
                copy.ColumnMappings.Add(40, 40); // Cur_Gt

                // Bypass Source - 41

                copy.ColumnMappings.Add(42, 41); // Ad_Recvd_Amt
                copy.ColumnMappings.Add(43, 42); // Ad_Adj_Amt

                // Bypass Source - 44

                copy.ColumnMappings.Add(45, 43); // Nil_Sply_Ty
                copy.ColumnMappings.Add(46, 44); // Nil_Amt
                copy.ColumnMappings.Add(47, 45); // Expt_Amt
                copy.ColumnMappings.Add(48, 46); // Ngsup_Amt

                // Bypass Source - 49

                copy.ColumnMappings.Add(50, 47); // Doc_Nature
                copy.ColumnMappings.Add(51, 48); // From_Serial_Number
                copy.ColumnMappings.Add(52, 49); // To_Serial_Number
                copy.ColumnMappings.Add(53, 50); // Totnum
                copy.ColumnMappings.Add(54, 51); // Cancel
                copy.ColumnMappings.Add(55, 52); // Net_Issue

                copy.ColumnMappings.Add(56, 0); // Fileid

            }
            else if (gstr1TemplateType == 2)
            {
                copy.ColumnMappings.Add(0, 2); // Compcode
                copy.ColumnMappings.Add(1, 3); // Unitcode
                copy.ColumnMappings.Add(2, 4); // Doctype
                copy.ColumnMappings.Add(3, 1); // Slno
                copy.ColumnMappings.Add(4, 5); // Gstin
                copy.ColumnMappings.Add(5, 6); // Fp
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // Etin
                copy.ColumnMappings.Add(8, 9); // Inum
                copy.ColumnMappings.Add(9, 10); // Idt
                copy.ColumnMappings.Add(10, 11); // Inv_Typ
                copy.ColumnMappings.Add(11, 17); // Rt
                copy.ColumnMappings.Add(12, 18); // Txval
                copy.ColumnMappings.Add(13, 19); // Iamt
                copy.ColumnMappings.Add(14, 20); // Camt
                copy.ColumnMappings.Add(15, 21); // Samt
                copy.ColumnMappings.Add(16, 22); // Csamt
                copy.ColumnMappings.Add(17, 23); // Totval
                copy.ColumnMappings.Add(18, 24); // Val
                copy.ColumnMappings.Add(19, 25); // Pos
                copy.ColumnMappings.Add(20, 26); // Rchrg
                copy.ColumnMappings.Add(21, 12); // Hsncode
                copy.ColumnMappings.Add(22, 13); // Hsndesc
                copy.ColumnMappings.Add(23, 14); // Uqc
                copy.ColumnMappings.Add(24, 15); // Qty
                copy.ColumnMappings.Add(25, 39); // Gt
                copy.ColumnMappings.Add(26, 40); // Cur_Gt

                // Bypass Source - 27

                copy.ColumnMappings.Add(28, 29); // Exp_Typ 
                copy.ColumnMappings.Add(29, 30); // Sbpcode
                copy.ColumnMappings.Add(30, 31); // Sbnum
                copy.ColumnMappings.Add(31, 32); // Sbdt

                // Bypass Source - 32

                copy.ColumnMappings.Add(33, 34); // Nt_Num
                copy.ColumnMappings.Add(34, 35); // Nt_Dt
                copy.ColumnMappings.Add(35, 33); // Ntty
                copy.ColumnMappings.Add(36, 36); // Rsn
                copy.ColumnMappings.Add(37, 37); // P_Gst

                // Bypass Source - 38

                copy.ColumnMappings.Add(39, 43); // Nil_Sply_Ty
                copy.ColumnMappings.Add(40, 44); // Nil_Amt
                copy.ColumnMappings.Add(41, 45); // Expt_Amt
                copy.ColumnMappings.Add(42, 46); // Ngsup_Amt

                // Bypass Source - 43

                copy.ColumnMappings.Add(44, 41); // Ad_Recvd_Amt

                // Bypass Source - 45

                copy.ColumnMappings.Add(46, 42); // Ad_Adj_Amt

                // Bypass Source - 47

                copy.ColumnMappings.Add(48, 47); // Doc_Nature
                copy.ColumnMappings.Add(49, 48); // From_Serial_Number
                copy.ColumnMappings.Add(50, 49); // To_Serial_Number
                copy.ColumnMappings.Add(51, 50); // Totnum
                copy.ColumnMappings.Add(52, 51); // Cancel
                copy.ColumnMappings.Add(53, 52); // Net_Issue

                copy.ColumnMappings.Add(54, 0); // Fileid

            }
            else if (gstr1TemplateType == 3)
            {

                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // Compcode
                copy.ColumnMappings.Add(2, 3); // Unitcode
                copy.ColumnMappings.Add(3, 4); // Doctype
                copy.ColumnMappings.Add(4, 5); // Gstin
                copy.ColumnMappings.Add(5, 6); // Fp
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // Etin
                copy.ColumnMappings.Add(8, 9); // Inum
                copy.ColumnMappings.Add(9, 10); // Idt
                copy.ColumnMappings.Add(10, 11); // Inv_Typ
                copy.ColumnMappings.Add(11, 12); // Hsncode
                copy.ColumnMappings.Add(12, 13); // Hsndesc
                copy.ColumnMappings.Add(13, 14); // Uqc

                copy.ColumnMappings.Add(14, 15); // Qty
                copy.ColumnMappings.Add(15, 16); // Unitprice
                copy.ColumnMappings.Add(16, 17); // Rt
                copy.ColumnMappings.Add(17, 18); // Txval
                copy.ColumnMappings.Add(18, 19); // Iamt
                copy.ColumnMappings.Add(19, 20); // Camt
                copy.ColumnMappings.Add(20, 21); // Samt
                copy.ColumnMappings.Add(21, 22); // Csamt
                copy.ColumnMappings.Add(22, 23); // Totval
                copy.ColumnMappings.Add(23, 24); // Val
                copy.ColumnMappings.Add(24, 25); // Pos
                copy.ColumnMappings.Add(25, 26); // Rchrg
                copy.ColumnMappings.Add(26, 27); // Rrate
                copy.ColumnMappings.Add(27, 28); // Sply_Ty


                copy.ColumnMappings.Add(28, 29); // Exp_Typ 
                copy.ColumnMappings.Add(29, 30); // Sbpcode
                copy.ColumnMappings.Add(30, 31); // Sbnum
                copy.ColumnMappings.Add(31, 32); // Sbdt
                copy.ColumnMappings.Add(32, 33); // Ntty
                copy.ColumnMappings.Add(33, 34); // Nt_Num
                copy.ColumnMappings.Add(34, 35); // Nt_Dt
                copy.ColumnMappings.Add(35, 36); // Rsn
                copy.ColumnMappings.Add(36, 37); // P_Gst
                copy.ColumnMappings.Add(37, 38); // Cdnur_Typ

                // Bypass Source - 38

                copy.ColumnMappings.Add(39, 39); // Gt
                copy.ColumnMappings.Add(40, 40); // Cur_Gt

                // Bypass Source - 41

                copy.ColumnMappings.Add(42, 41); // Ad_Recvd_Amt
                copy.ColumnMappings.Add(43, 42); // Ad_Adj_Amt

                // Bypass Source - 44

                copy.ColumnMappings.Add(45, 43); // Nil_Sply_Ty
                copy.ColumnMappings.Add(46, 44); // Nil_Amt
                copy.ColumnMappings.Add(47, 45); // Expt_Amt
                copy.ColumnMappings.Add(48, 46); // Ngsup_Amt

                // Bypass Source - 49

                copy.ColumnMappings.Add(50, 47); // Doc_Nature
                copy.ColumnMappings.Add(51, 48); // From_Serial_Number
                copy.ColumnMappings.Add(52, 49); // To_Serial_Number
                copy.ColumnMappings.Add(53, 50); // Totnum
                copy.ColumnMappings.Add(54, 51); // Cancel
                copy.ColumnMappings.Add(55, 52); // Net_Issue

                // Bypass Source - 56

                copy.ColumnMappings.Add(57, 53); // Omon
                copy.ColumnMappings.Add(58, 54); // Oinum
                copy.ColumnMappings.Add(59, 55); // Oidt
                copy.ColumnMappings.Add(60, 56); // Ont_Num
                copy.ColumnMappings.Add(61, 57); // Ont_Dt

                // Bypass Source - 62

                copy.ColumnMappings.Add(63, 58); // Diff_Percent

                copy.ColumnMappings.Add(64, 0); // Fileid

            }
            else if (gstr1TemplateType == 4)
            {
                copy.ColumnMappings.Add(0, 2); // Compcode
                copy.ColumnMappings.Add(1, 3); // Unitcode
                copy.ColumnMappings.Add(2, 4); // Doctype
                copy.ColumnMappings.Add(3, 1); // Slno
                copy.ColumnMappings.Add(4, 5); // Gstin
                copy.ColumnMappings.Add(5, 6); // Fp
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // Etin
                copy.ColumnMappings.Add(8, 9); // Inum
                copy.ColumnMappings.Add(9, 10); // Idt
                copy.ColumnMappings.Add(10, 11); // Inv_Typ
                copy.ColumnMappings.Add(11, 17); // Rt
                copy.ColumnMappings.Add(12, 18); // Txval
                copy.ColumnMappings.Add(13, 19); // Iamt
                copy.ColumnMappings.Add(14, 20); // Camt
                copy.ColumnMappings.Add(15, 21); // Samt
                copy.ColumnMappings.Add(16, 22); // Csamt
                copy.ColumnMappings.Add(17, 23); // Totval
                copy.ColumnMappings.Add(18, 24); // Val
                copy.ColumnMappings.Add(19, 25); // Pos
                copy.ColumnMappings.Add(20, 26); // Rchrg
                copy.ColumnMappings.Add(21, 12); // Hsncode
                copy.ColumnMappings.Add(22, 13); // Hsndesc
                copy.ColumnMappings.Add(23, 14); // Uqc
                copy.ColumnMappings.Add(24, 15); // Qty
                copy.ColumnMappings.Add(25, 39); // Gt
                copy.ColumnMappings.Add(26, 40); // Cur_Gt

                // Bypass Source - 27

                copy.ColumnMappings.Add(28, 29); // Exp_Typ 
                copy.ColumnMappings.Add(29, 30); // Sbpcode
                copy.ColumnMappings.Add(30, 31); // Sbnum
                copy.ColumnMappings.Add(31, 32); // Sbdt

                // Bypass Source - 32

                copy.ColumnMappings.Add(33, 34); // Nt_Num
                copy.ColumnMappings.Add(34, 35); // Nt_Dt
                copy.ColumnMappings.Add(35, 33); // Ntty
                copy.ColumnMappings.Add(36, 36); // Rsn
                copy.ColumnMappings.Add(37, 37); // P_Gst

                // Bypass Source - 38

                copy.ColumnMappings.Add(39, 43); // Nil_Sply_Ty
                copy.ColumnMappings.Add(40, 44); // Nil_Amt
                copy.ColumnMappings.Add(41, 45); // Expt_Amt
                copy.ColumnMappings.Add(42, 46); // Ngsup_Amt

                // Bypass Source - 43

                copy.ColumnMappings.Add(44, 41); // Ad_Recvd_Amt

                // Bypass Source - 45

                copy.ColumnMappings.Add(46, 42); // Ad_Adj_Amt

                // Bypass Source - 47

                copy.ColumnMappings.Add(48, 47); // Doc_Nature
                copy.ColumnMappings.Add(49, 48); // From_Serial_Number
                copy.ColumnMappings.Add(50, 49); // To_Serial_Number
                copy.ColumnMappings.Add(51, 50); // Totnum
                copy.ColumnMappings.Add(52, 51); // Cancel
                copy.ColumnMappings.Add(53, 52); // Net_Issue

                // Bypass Source - 54

                copy.ColumnMappings.Add(55, 53); // Omon
                copy.ColumnMappings.Add(56, 54); // Oinum
                copy.ColumnMappings.Add(57, 55); // Oidt
                copy.ColumnMappings.Add(58, 56); // Ont_Num
                copy.ColumnMappings.Add(59, 57); // Ont_Dt

                // Bypass Source - 60

                copy.ColumnMappings.Add(61, 58); // Diff_Percent

                copy.ColumnMappings.Add(62, 0); // Fileid

            }
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR1_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();
                sqlcmd.Parameters.Add("@TemplateTypeId", SqlDbType.TinyInt).Value = templateTypeId;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);


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

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }
            }
            sqlcon.Close();
            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }

        private void ImportGSTR2CSV(int GstrTypeId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;
            Session["TemplateType"] = 1;
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
            if (GstrTypeId == 1)
            {
                //
            }
            else if (GstrTypeId == 2)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[62] = fileId;
                }

            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            if (GstrTypeId == 2)
            {
                tableName = "TBL_CSV_GSTR2_RECS";
                copy.DestinationTableName = tableName;

                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // Compcode
                copy.ColumnMappings.Add(2, 3); // Unitcode
                copy.ColumnMappings.Add(3, 4); // Doctype
                copy.ColumnMappings.Add(4, 5); // Gstin
                copy.ColumnMappings.Add(5, 6); // Fp
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // Inum
                copy.ColumnMappings.Add(8, 9); // Idt
                copy.ColumnMappings.Add(9, 10); // Inv_Typ
                copy.ColumnMappings.Add(10, 11); // Hsncode
                copy.ColumnMappings.Add(11, 12); // Hsndesc
                copy.ColumnMappings.Add(12, 13); // Uqc
                copy.ColumnMappings.Add(13, 14); // Qty

                copy.ColumnMappings.Add(14, 15); // Unitprice
                copy.ColumnMappings.Add(15, 16); // Rt
                copy.ColumnMappings.Add(16, 17); // Txval
                copy.ColumnMappings.Add(17, 18); // Iamt
                copy.ColumnMappings.Add(18, 19); // Camt
                copy.ColumnMappings.Add(19, 20); // Samt
                copy.ColumnMappings.Add(20, 21); // Csamt
                copy.ColumnMappings.Add(21, 22); // Totval
                copy.ColumnMappings.Add(22, 23); // Val
                copy.ColumnMappings.Add(23, 24); // Pos
                copy.ColumnMappings.Add(24, 25); // Rchrg
                copy.ColumnMappings.Add(25, 26); // Rrate
                copy.ColumnMappings.Add(26, 27); // Is_Sez
                copy.ColumnMappings.Add(27, 28); // Stin
                copy.ColumnMappings.Add(28, 29); // Boe_Num
                copy.ColumnMappings.Add(29, 30); // Boe_Dt
                copy.ColumnMappings.Add(30, 31); // Boe_Val
                copy.ColumnMappings.Add(31, 32); // Portcode
                copy.ColumnMappings.Add(32, 33); // Ntty
                copy.ColumnMappings.Add(33, 34); // Nt_Num
                copy.ColumnMappings.Add(34, 35); // Nt_Dt
                copy.ColumnMappings.Add(35, 36); // Rsn
                copy.ColumnMappings.Add(36, 37); // P_Gst
                copy.ColumnMappings.Add(37, 38); // Rtin

                // Bypass Source - 38

                copy.ColumnMappings.Add(39, 39); // Nil_Sply_Ty
                copy.ColumnMappings.Add(40, 40); // Nilsply
                copy.ColumnMappings.Add(41, 41); // Exptdsply
                copy.ColumnMappings.Add(42, 42); // Ngsply
                copy.ColumnMappings.Add(43, 43); // Cpddr

                // Bypass Source - 44

                copy.ColumnMappings.Add(45, 44); // Sply_Ty
                copy.ColumnMappings.Add(46, 45); // Ad_Paid_Amt
                copy.ColumnMappings.Add(47, 46); // Ad_Adj_Amt

                // Bypass Source - 48

                copy.ColumnMappings.Add(49, 47); // Itc_Elg
                copy.ColumnMappings.Add(50, 48); // Tx_I 
                copy.ColumnMappings.Add(51, 49); // Tx_C
                copy.ColumnMappings.Add(52, 50); // Tx_S
                copy.ColumnMappings.Add(53, 51); // Tx_Cs

                // Bypass Source - 54

                copy.ColumnMappings.Add(55, 52); // Ruletype
                copy.ColumnMappings.Add(56, 53); // Rvsl_Iamt 
                copy.ColumnMappings.Add(57, 54); // Rvsl_Camt
                copy.ColumnMappings.Add(58, 55); // Rvsl_Samt
                copy.ColumnMappings.Add(59, 56); // Rvsl_Csamt
                copy.ColumnMappings.Add(60, 57); // receivedby
                copy.ColumnMappings.Add(61, 58); // receiveddate
                copy.ColumnMappings.Add(62, 0); // Fileid

            }
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR2_EXT_PERF";// "usp_Import_CSV_GSTR2_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();

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

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }

            }

            sqlcon.Close();

            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }

        private void ImportGSTR6CSV(int GstrTypeId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            int templateTypeId = 1;
            Session["TemplateType"] = templateTypeId;
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));
            if (GstrTypeId == 1)
            {
                //
            }
            else if (GstrTypeId == 4)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[35] = fileId;
                }
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            if (GstrTypeId == 4)
            {
                tableName = "TBL_CSV_GSTR6_RECS";
                copy.DestinationTableName = tableName;

                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // Doctype
                copy.ColumnMappings.Add(2, 3); // Gstin
                copy.ColumnMappings.Add(3, 4); // Fp
                copy.ColumnMappings.Add(4, 5); // typ
                copy.ColumnMappings.Add(5, 6); // ctin
                copy.ColumnMappings.Add(6, 7); // isd_docty
                copy.ColumnMappings.Add(7, 8); // docnum
                copy.ColumnMappings.Add(8, 9); // docdt
                copy.ColumnMappings.Add(9, 10); // elglst
                copy.ColumnMappings.Add(10, 11); // iamti
                copy.ColumnMappings.Add(11, 12); // iamts
                copy.ColumnMappings.Add(12, 13); // iamtc
                copy.ColumnMappings.Add(13, 14); // samts
                copy.ColumnMappings.Add(14, 15); // samti
                copy.ColumnMappings.Add(15, 16); // camti
                copy.ColumnMappings.Add(16, 17); // camtc
                copy.ColumnMappings.Add(17, 18); // rt
                copy.ColumnMappings.Add(18, 19); // txval
                copy.ColumnMappings.Add(19, 20); // iamt
                copy.ColumnMappings.Add(20, 21); // camt
                copy.ColumnMappings.Add(21, 22); // samt
                copy.ColumnMappings.Add(22, 23); // csamt
                copy.ColumnMappings.Add(23, 24); // totval
                copy.ColumnMappings.Add(24, 25); // statecd
                copy.ColumnMappings.Add(25, 26); // ntty
                copy.ColumnMappings.Add(26, 27); // nt_num
                copy.ColumnMappings.Add(27, 28); // nt_dt
                // Bypass Source - 28
                copy.ColumnMappings.Add(29, 29); // rcpty
                copy.ColumnMappings.Add(30, 30); // odocnum
                copy.ColumnMappings.Add(31, 31); // odocdt
                copy.ColumnMappings.Add(32, 32); // ont_num
                copy.ColumnMappings.Add(33, 33); // ont_dt
                copy.ColumnMappings.Add(34, 34); // rstatecd
                copy.ColumnMappings.Add(35, 0); // fileid

            }
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR6_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();
                sqlcmd.Parameters.Add("@TemplateTypeId", SqlDbType.TinyInt).Value = templateTypeId;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);

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

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }
            }
            sqlcon.Close();
            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }

        private void ImportGSTR4CSV(int GstrTypeId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            int templateTypeId = 1;
            Session["TemplateType"] = templateTypeId;
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            if (GstrTypeId == 5)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[34] = fileId;
                }
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            if (GstrTypeId == 5)
            {
                tableName = "TBL_CSV_GSTR4_RECS";
                copy.DestinationTableName = tableName;

                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // CompCode
                copy.ColumnMappings.Add(2, 3); // Unit code
                copy.ColumnMappings.Add(3, 4); // doc type
                copy.ColumnMappings.Add(4, 5); // gstin
                copy.ColumnMappings.Add(5, 6); // period
                copy.ColumnMappings.Add(6, 7); // Ctin
                copy.ColumnMappings.Add(7, 8); // inum
                copy.ColumnMappings.Add(8, 9); // idt
                copy.ColumnMappings.Add(9, 10); // inv type
                copy.ColumnMappings.Add(10, 11); // rate
                copy.ColumnMappings.Add(11, 12); // txval
                copy.ColumnMappings.Add(12, 13); // iamt
                copy.ColumnMappings.Add(13, 14); // camt
                copy.ColumnMappings.Add(14, 15); // samt
                copy.ColumnMappings.Add(15, 16); // csamt
                copy.ColumnMappings.Add(16, 17); // val
                copy.ColumnMappings.Add(17, 18); // pos
                copy.ColumnMappings.Add(18, 19); // rchrg
                copy.ColumnMappings.Add(19, 20); // ntty
                copy.ColumnMappings.Add(20, 21); // nt_num
                copy.ColumnMappings.Add(21, 22); // nt_dt
                copy.ColumnMappings.Add(22, 23); // cdnr_type
                copy.ColumnMappings.Add(24, 24); // sply_ty
                copy.ColumnMappings.Add(25, 25); // ad_adj_amt
                copy.ColumnMappings.Add(26, 26); // ad_rcvd_amt
                copy.ColumnMappings.Add(27, 27); // trnovr
                copy.ColumnMappings.Add(29, 28); // omon
                copy.ColumnMappings.Add(30, 29); // oinum
                copy.ColumnMappings.Add(31, 30); // oidt
                copy.ColumnMappings.Add(32, 31); // ont_num
                copy.ColumnMappings.Add(33, 32); // ont_dt
                copy.ColumnMappings.Add(34, 0); // fileid

            }
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR4_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();
                sqlcmd.Parameters.Add("@TemplateTypeId", SqlDbType.TinyInt).Value = templateTypeId;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);

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

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }
            }
            sqlcon.Close();
            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }

        private void ImportGSTR7CSV(int GstrTypeId, string fileName, string userEmail)
        {
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            int templateTypeId = 1;
            Session["TemplateType"] = templateTypeId;
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();

            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = Convert.ToInt32(GstrTypeId);
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Models.Common.Functions.InsertIntoTable("TBL_RECVD_FILES", cmd, sqlcon));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            if (GstrTypeId == 6)
            {
                foreach (DataRow dr in dt.Rows)
                {
                    dr[12] = fileId;
                }
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            if (GstrTypeId == 6)
            {
                tableName = "TBL_CSV_GSTR7_RECS";
                copy.DestinationTableName = tableName;

                copy.ColumnMappings.Add(0, 1); // Slno
                copy.ColumnMappings.Add(1, 2); // doctype
                copy.ColumnMappings.Add(2, 3); //gstin(deductee)
                copy.ColumnMappings.Add(3, 4); // period
                copy.ColumnMappings.Add(4, 5); // ctin
                copy.ColumnMappings.Add(5, 6); // taxable value
                copy.ColumnMappings.Add(6, 7); // iamt
                copy.ColumnMappings.Add(7, 8); // camt
                copy.ColumnMappings.Add(8, 9); // samt
                copy.ColumnMappings.Add(9, 10); // octin
                copy.ColumnMappings.Add(10, 11); // operiod
                copy.ColumnMappings.Add(11, 12); // otacable value


                copy.ColumnMappings.Add(12, 0); // fileid

            }
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR7_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = Session["CustRefNo"].ToString();
                sqlcmd.Parameters.Add("@TemplateTypeId", SqlDbType.TinyInt).Value = templateTypeId;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Convert.ToInt32(Session["Cust_ID"]);

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

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }
            }
            sqlcon.Close();
            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
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


        #region "IMPORT CSV FOR gstr9"
        public void ImportGSTR9CSV(int GstrTypeId, string fileName, string userEmail)
        {
            DataSet dsOutputRecords = new DataSet();
            DataTable dtGstinRecords = new DataTable();
            Session["TemplateType"] = 1;
            SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            int fileId = 0;
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;
            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');
            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                value = sr.ReadLine().Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }
            }
            sr.Close();

            sqlcon.Open();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(Functions.InsertIntoTable("TBL_GSTR9_RECVD_FILES", cmd, sqlcon));
            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[22] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(sqlcon);
            copy.BulkCopyTimeout = 2000;
            copy.BatchSize = 5000;
            tableName = "TBL_CSV_GSTR9_GEN_RECS";
            copy.DestinationTableName = tableName;

            #region "COLUMNS MAPPING"
            copy.ColumnMappings.Add(0, 1);// [SerialNo]
            copy.ColumnMappings.Add(1, 2);// [gstin]
            copy.ColumnMappings.Add(2, 3);// [period]
            copy.ColumnMappings.Add(3, 4);// [Action]
            copy.ColumnMappings.Add(4, 5);// [natureofsupplies]
            copy.ColumnMappings.Add(5, 6);// [txval]
            copy.ColumnMappings.Add(6, 7); // [iamt]
            copy.ColumnMappings.Add(7, 8); // [camt]
            copy.ColumnMappings.Add(8, 9); // [samt]
            copy.ColumnMappings.Add(9, 10); // [csamt]
            copy.ColumnMappings.Add(10, 11); // [itctype] 
            copy.ColumnMappings.Add(11, 12); // [taxpayable]
            copy.ColumnMappings.Add(12, 13); // [paid through cash] 
            copy.ColumnMappings.Add(13, 14); // [interest]
            copy.ColumnMappings.Add(14, 15); // [panality] 
            copy.ColumnMappings.Add(15, 16); // [latefee]
            copy.ColumnMappings.Add(16, 17); // [hsn] 
            copy.ColumnMappings.Add(17, 18); // [uqc] 
            copy.ColumnMappings.Add(18, 19); // [total quantity]
            copy.ColumnMappings.Add(19, 20); // [rate] Isconcesstional
            copy.ColumnMappings.Add(20, 21); // [rate]
            copy.ColumnMappings.Add(21, 22);
            copy.ColumnMappings.Add(22, 0); // [FileId]  



            #endregion
            copy.WriteToServer(dt);
            copy.Close();
            using (SqlCommand sqlcmd = new SqlCommand())
            {
                sqlcmd.CommandText = "usp_Import_CSV_GSTR9_EXT_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.Connection = sqlcon;

                sqlcmd.Parameters.Add("@FileId", SqlDbType.Int).Value = fileId;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.Int).Value = Session["User_ID"].ToString();
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = Session["Cust_ID"].ToString();

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
                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                    sqlcon.Close();
                }
            }

            TempData["TotalRecordsCount"] = totalRecords.ToString();
            Session["TotalRecordsCount"] = TempData["TotalRecordsCount"] as string;
            TempData["ProcessedRecordsCount"] = processedRecords.ToString();
            Session["ProcessedRecordsCount"] = TempData["ProcessedRecordsCount"] as string;
            TempData["ErrorRecordsCount"] = errorRecords.ToString();
            Session["ErrorRecordsCount"] = TempData["ErrorRecordsCount"] as string;
        }
        #endregion
    }
}