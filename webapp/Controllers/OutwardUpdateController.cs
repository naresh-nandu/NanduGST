using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using SmartAdminMvc.Models.EditInvoice;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models;
using System.Collections;

namespace SmartAdminMvc.Controllers
{
    public class OutwardUpdateController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        AdvanceTaxFunctions Result = new AdvanceTaxFunctions();
        
        [HttpGet]
        public ActionResult AT(string gstin, string period)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                ViewBag.Gstin = gstin;
                ViewBag.Period = period;
                ViewBag.Rates = LoadDropDowns.LoadRates();
                int CreatedBy= Convert.ToInt32(Session["User_ID"]);

                string refnum = Session["CustRefNo"].ToString();
                var records = (from li in db.TBL_EXT_GSTR1_AT where li.gstin == gstin && li.fp == period && li.referenceno == refnum select li).ToList();
                if (records.Count <= 0)
                {
                    TempData["Message"] = "Invoice Deleted Successfully.";
                    return RedirectToAction("UpdationList", "OutwardRegister");
                }
                var AT = Result.Getinvoice(gstin,period, CreatedBy);
                ViewBag.AT = AT;
            }
            catch(Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult AT(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);
                string rolename = Convert.ToString(Session["Role_Name"]);
                string Refno = Session["CustRefNo"].ToString();
                int Result;
                string GSTIN, FP,POS, ActionType;
                decimal Rate, AdvanceAmount, IGST, CGST, SGST, CESS;
                GSTIN = Form["cgstin"];
                FP = Form["period"];
                Session["GSTIN"] = GSTIN;
                Session["FP"] = FP;
                ActionType ="AT";
                Rate = Convert.ToDecimal(Form["rt"]);
                POS = Form["pos"];
                AdvanceAmount = Convert.ToDecimal(Form["ad_amt"]);
                IGST = Convert.ToDecimal(Form["iamount"]);
                CGST = Convert.ToDecimal(Form["camount"]);
                SGST = Convert.ToDecimal(Form["samount"]);
                CESS = Convert.ToDecimal(Form["csamount"]);
                Result = AdvanceTaxFunctions.Insert(GSTIN, FP, ActionType, POS, Rate, AdvanceAmount, IGST, CGST, SGST, CESS, Refno,UserId);
                if (Result == 1)
                {
                    TempData["Message"] = "Item Saved Successfully";
                    AdvanceTaxFunctions.ATPush(Refno, GSTIN,CustId,UserId);
                    //AdvanceTaxFunctions.TXPPush(Refno, GSTIN);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward AT/TXP invoice generated For: " + GSTIN, "");
                   
                    
                }
                else if (Result == -2)
                {
                    TempData["Message"] = "Item details cannot be inserted";
                   
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("AT", "OutwardUpdate", new { gstin = Session["GSTIN"].ToString(), period = Session["FP"].ToString() });
        }


        public JsonResult ATUpdate(int id,decimal rt,string pos,decimal adamt,decimal igsta,decimal cgsta,decimal sgsta,decimal cessa)
        {
            TBL_EXT_GSTR1_AT inv = db.TBL_EXT_GSTR1_AT.Where(u => u.atid == id).SingleOrDefault();
            var message = inv;
            string Mode = "M";
            int result;
            int  UserId= Convert.ToInt32(Session["User_ID"]);
           
            result = AdvanceTaxFunctions.ATUpdate(id, Mode, pos, rt, adamt, igsta, cgsta, sgsta, cessa, UserId);
            if (result == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "AT updated", "");
                TempData["Message"] = "Invoice Item  Details Updated Successfully";
            }

            else if (result == -2)
            {
                TempData["Message"] = "Item not available";
            }

            else 
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult ATDelete(int id)
        {
            TBL_EXT_GSTR1_AT inv = db.TBL_EXT_GSTR1_AT.Where(u => u.atid == id).SingleOrDefault();
            var message = inv;
            int result;
            string Mode = "D";
            int Createdby= Convert.ToInt32(Session["User_ID"]);
            result = AdvanceTaxFunctions.ATDelete(id, Mode,Createdby);
            if (result == 2)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "AT deleted", "");
                TempData["Message"] = "Invoice Item Deleted Successfully";
            }

            else if (result == -1)
            {
                TempData["Message"] = "Item not available";
            }

            else 
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult TXP(string gstin, string period)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                ViewBag.Gstin = gstin;
                ViewBag.Period = period;
                ViewBag.Rates = LoadDropDowns.LoadRates();
                int CreatedBy = Convert.ToInt32(Session["User_ID"]);
                string refnum = Session["CustRefNo"].ToString();
                var records = (from li in db.TBL_EXT_GSTR1_TXP where li.gstin == gstin && li.fp == period && li.referenceno == refnum select li).ToList();
                if (records.Count <= 0)
                {
                    TempData["Message"] = "Invoice Deleted Successfully.";
                    return RedirectToAction("UpdationList", "OutwardRegister");
                }
                var TXP = Result.GetTXPinvoice(gstin, period, CreatedBy);
                ViewBag.TXP = TXP;
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult TXP(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);
                string Refno = Session["CustRefNo"].ToString();
                int Result;
                string GSTIN, FP, POS, ActionType;
                decimal Rate, AdvanceAmount, IGST, CGST, SGST, CESS;
                GSTIN = Form["cgstin"];
                FP = Form["period"];
                Session["GSTIN"] = GSTIN;
                Session["FP"] = FP;
                ActionType = "TXP";
                Rate = Convert.ToDecimal(Form["rt"]);
                POS = Form["pos"];
                AdvanceAmount = Convert.ToDecimal(Form["ad_amt"]);
                IGST = Convert.ToDecimal(Form["iamount"]);
                CGST = Convert.ToDecimal(Form["camount"]);
                SGST = Convert.ToDecimal(Form["samount"]);
                CESS = Convert.ToDecimal(Form["csamount"]);
                Result = AdvanceTaxFunctions.Insert(GSTIN, FP, ActionType, POS, Rate, AdvanceAmount, IGST, CGST, SGST, CESS, Refno, UserId);
                if (Result == 1)
                {
                    TempData["Message"] = "Item Saved Successfully";
                    AdvanceTaxFunctions.TXPPush(Refno, GSTIN,CustId,UserId);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward AT/TXP invoice generated For: " + GSTIN, "");
                    
                }
                else if (Result == -2)
                {
                    TempData["Message"] = "Item details cannot be inserted";
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("TXP", "OutwardUpdate", new { gstin = Session["GSTIN"].ToString(), period = Session["FP"].ToString() });
        }


        public JsonResult TXPUpdate(int id, decimal rt, string pos, decimal adamt, decimal igsta, decimal cgsta, decimal sgsta, decimal cessa)
        {
            TBL_EXT_GSTR1_TXP inv = db.TBL_EXT_GSTR1_TXP.Where(u => u.txpid == id).SingleOrDefault();
            var message = inv;
            string Mode = "M";
            int result;
            int UserId = Convert.ToInt32(Session["User_ID"]);

            result = AdvanceTaxFunctions.TXPUpdate(id, Mode, pos, rt, adamt, igsta, cgsta, sgsta, cessa, UserId);
            if (result == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TXP Updated", "");
                TempData["Message"] = "Invoice Item Details Updated Successfully";
            }

            else if (result == -2)
            {
                TempData["Message"] = "Item not available";
            }

            else
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult TXPDelete(int id)
        {
            TBL_EXT_GSTR1_TXP inv = db.TBL_EXT_GSTR1_TXP.Where(u => u.txpid == id).SingleOrDefault();
            var message = inv;
            int result;
            string Mode = "D";
            int Createdby = Convert.ToInt32(Session["User_ID"]);
            result = AdvanceTaxFunctions.TXPDelete(id, Mode, Createdby);
            if (result == 2)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "TXP Deleted", "");
                TempData["Message"] = "Invoice Item Deleted Successfully";
            }

            else if (result == -1)
            {
                TempData["Message"] = "Item not available";
            }

            else 
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult DocIssue(string gstin, string period)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                ViewBag.Gstin = gstin;
                ViewBag.Period = period;
                ViewBag.DocType = OutwardFunctions.GetDocType();
                int CreatedBy = Convert.ToInt32(Session["User_ID"]);
                string refnum = Session["CustRefNo"].ToString();
                var records = (from li in db.TBL_EXT_GSTR1_DOC where li.gstin == gstin && li.fp == period && li.referenceno == refnum && li.rowstatus!=2 select li).ToList();
                if (records.Count <= 0)
                {
                    TempData["Message"] = "Invoice Deleted Successfully.";
                    return RedirectToAction("UpdationList", "OutwardRegister");
                }
                var DOC = Result.GetDOCinvoice(gstin, period, CreatedBy);
                Session["DOCNum"] = DOC.Count();
                ViewBag.DOC = DOC;
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult DocIssue(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);
                string GSTIN, FP;
                int Result,CreatedBy, Itemnum, NatureOfDocument, TotalNumber, Cancelled, NetIssued;
                string FromSno, ToSno;

                Itemnum = Convert.ToInt32(Session["DOCNum"])+1;
                GSTIN = Form["cgstin"];
                FP = Form["period"];
                Session["GSTIN"] = GSTIN;
                Session["FP"] = FP;
                CreatedBy = Convert.ToInt32(Session["User_ID"]);
                string Refno = Session["CustRefNo"].ToString();
                NatureOfDocument = Convert.ToInt32(Form["DocTpe"]);
                FromSno = Form["FromSno"];
                ToSno = Form["ToSno"];
                TotalNumber = Convert.ToInt32(Form["Total"]);
                Cancelled = Convert.ToInt32(Form["Cancelled"]);
                NetIssued = Convert.ToInt32(Form["NetIssue"]);

                Result = DocFunctions.Insert(GSTIN, FP, NatureOfDocument, Itemnum, FromSno, ToSno, TotalNumber, Cancelled, NetIssued, Refno, UserId);

                if (Result == 1)
                {
                    TempData["Message"] = "Invoice Saved Successfully";
                    DocFunctions.DOCPush(Refno, GSTIN,CustId,UserId);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward DocIssue invoice generated For: " + GSTIN, "");
                    
                   
                }
                else if (Result == -2)
                {
                    TempData["Message"] = "Item details cannot be inserted";
                   
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("DocIssue", "OutwardUpdate", new { gstin = Session["GSTIN"].ToString(), period = Session["FP"].ToString() });
        }

        public JsonResult DocUpdate(int id, int docnum, string fromsno, string tosno, int totalnum, int cancel, int netissue)
        {
            TBL_EXT_GSTR1_DOC inv = db.TBL_EXT_GSTR1_DOC.Where(u => u.docid == id).SingleOrDefault();
            var message = inv;
            string Mode = "M";
            int result;
            int UserId = Convert.ToInt32(Session["User_ID"]);

            result = DocFunctions.DocUpdate(id,Mode,docnum,fromsno,tosno,totalnum,cancel,netissue,UserId);
            if (result == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "DOCIssue updated", "");
                TempData["Message"] = "Invoice Item Details Updated Successfully";
            }
            
            else if (result == -2)
            {
                TempData["Message"] = "Data Already uploaded to GSTIN Server.So Item details cannot be Updated";
            }

            else
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DocDelete(int id)
        {
            TBL_EXT_GSTR1_DOC inv = db.TBL_EXT_GSTR1_DOC.Where(u => u.docid == id).SingleOrDefault();
            var message = inv;
            int result;
            string Mode = "D";
            int Createdby = Convert.ToInt32(Session["User_ID"]);
            result = DocFunctions.DocDelete(id, Mode, Createdby);
            if (result == 2)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "DOCIssue Deleted", "");
                TempData["Message"] = "Invoice Item Deleted Successfully";
            }

            else if (result == -1)
            {
                TempData["Message"] = "Item not available";
            }

            else 
            {
                TempData["Message"] = "Something Went Wrong";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult NilRated(string gstin, string period,string type)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                
                ViewBag.DocType = OutwardFunctions.GetNilSupplyType();
                int CreatedBy = Convert.ToInt32(Session["User_ID"]);

                var Nil = Result.GetNilinvoice(gstin, period,CreatedBy,type);
                ViewBag.Nil = Nil;
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult NilRated(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);
                string Refno = Session["CustRefNo"].ToString();

                int Result,NilId;
                string GSTIN, FP, SupplyType, NilAmount, ExemptedAmount, NonGSTAmount;
                decimal NilRated, Exempted, NonGST;

                NilId = Convert.ToInt32(Form["nilid"]);
                GSTIN = Form["cgstin"];
                FP = Form["Period"];
                SupplyType = Form["supplytype"];
                Session["GSTIN"] = GSTIN;
                Session["FP"] = FP;
                Session["TYPE"] = SupplyType;
                NilAmount = Form["nilrated"];
                if (NilAmount == "")
                {
                    NilRated = 0;
                }
                else
                {
                    NilRated = Convert.ToDecimal(Form["nilrated"]);
                }

                ExemptedAmount = Form["exempted"];
                if (ExemptedAmount == "")
                {
                    Exempted = 0;
                }
                else
                {
                    Exempted = Convert.ToDecimal(Form["exempted"]);
                }
                NonGSTAmount = Form["non-gst"];
                if (NonGSTAmount == "")
                {
                    NonGST = 0;
                }
                else
                {
                    NonGST = Convert.ToDecimal(Form["non-gst"]);
                }


                Result = NilFunctions.Update(NilId,NilRated,Exempted,NonGST,SupplyType,UserId);

                if (Result == 1)
                {
                    TempData["Message"] = "Invoice Updated Successfully";
                    NilFunctions.NilPush(Refno, GSTIN,CustId,UserId);
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward Nil Rated invoice generated For: " + GSTIN, "");
                    
                }
                else if (Result == -2)
                {
                    TempData["Message"] = "Item details cannot be Updated";
                    return RedirectToAction("NilRated");
                }
                else
                {
                    TempData["Message"] = "Something Went Wrong";
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("NilRated", "OutwardUpdate", new { gstin = Session["GSTIN"].ToString(), period = Session["FP"].ToString(), type=Session["TYPE"].ToString() });
        }

    }
}