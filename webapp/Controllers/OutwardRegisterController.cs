using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.EditInvoice;
using SmartAdminMvc.Models.Inward;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class OutwardRegisterController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        [HttpGet]
        public ActionResult EXP()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward EXP Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                
                ViewBag.Taxpayer_GSTIN = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");
                ViewBag.ExportType = OutwardFunctions.GetExpType();

                if (Session["MasterDtExp"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterDtExp"]);

                    return View(ConvertToDictionary(dt));
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EXP(FormCollection Form, string command)
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
                int UserId = Convert.ToInt32(Session["User_ID"]);
                string Refno = Session["CustRefNo"].ToString();
                string Result = string.Empty;
                if (command == "add")
                {
                    decimal total_invoicevalue, invoicevalue;
                    Session["isautoexp"] = Form["isauto"];
                    Session["invoicenoexp"] = Form["invoiceno"];
                    Session["invoicedateexp"] = Form["invoicedate"];
                    Session["cgstinexp"] = Form["cgstin"];
                    Session["extypexp"] = Form["ex_typ"];
                    
                    Session["AddInfoExp"] = Form["addinfo"];
                    Session["nameexp"] = Form["name"];
                    Session["addressexp"] = Form["address"];
                    Session["snameexp"] = Form["sname"];
                    Session["saddressexp"] = Form["saddress"];

                    if (Session["MasterDtExp"] == null)
                    {
                        Session["discountreqexp"] = Form["isDiscountreq"];
                    }

                    if (Form["portcode"] == "")
                    {
                        Session["portcodeexp"] = "-";
                    }
                    else
                    {
                        Session["portcodeexp"] = Form["portcode"];
                    }

                    if (Form["billno"] == "")
                    {
                        Session["billnoexp"] = "-";
                    }
                    else
                    {
                        Session["billnoexp"] = Form["billno"];
                    }
                    if (Form["billdate"] == "")
                    {
                        Session["billdateexp"] = "-";
                    }
                    else
                    {
                        Session["billdateexp"] = Form["billdate"];
                    }

                    int count;
                    DataTable dt = new DataTable();


                    if (Session["SnoExp"] == null)
                    {
                        Session["SnoExp"] = 1;
                        count = (int)Session["SnoExp"];
                    }
                    else
                    {
                        count = (int)Session["SnoExp"];
                        count = count + 1;
                        Session["SnoExp"] = count;
                    }

                    if (Session["MasterDtExp"] == null)
                    {

                        dt.Columns.Add("SnoExp", typeof(int));
                        dt.Columns.Add("HSN", typeof(string));
                        dt.Columns.Add("Item Description", typeof(string));
                        dt.Columns.Add("Quantity", typeof(string));
                        dt.Columns.Add("UQC", typeof(string));
                        dt.Columns.Add("Unit Price", typeof(string));
                        dt.Columns.Add("Discount", typeof(string));
                        dt.Columns.Add("Taxablevalue", typeof(string));
                        dt.Columns.Add("Rate", typeof(string));
                        dt.Columns.Add("IGST Amount", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterDtExp"]);
                    }


                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = Form["hsn"];
                    dr[2] = Form["itemdesc"];
                    dr[3] = Form["qty"];
                    dr[4] = Form["uqc"];
                    dr[5] = Form["unitprice"];
                    dr[6] = Form["discount"];
                    dr[7] = Form["taxablevalue"];
                    dr[8] = Form["rate"];
                    dr[9] = Form["iamt"];
                    dt.Rows.Add(dr);
                    Session["MasterDtExp"] = dt;

                    if (Session["invoicevalueexp"] == null)
                    {
                        invoicevalue = 0;
                    }
                    else
                    {
                        invoicevalue = Convert.ToDecimal(Form["invoicevalue"]);
                    }
                    total_invoicevalue = Convert.ToDecimal(Form["taxablevalue"]) + Convert.ToDecimal(Form["iamt"]) + invoicevalue;
                    Session["invoicevalueexp"] = total_invoicevalue;


                    return RedirectToAction("EXP");
                }
                else if (command == "save1" || command == "save2")
                {
                    string ExpType, Gstin, InvoiceNo, InvoiceDate, Fp, PortCode, BillNo, BillDate, AddInfo, Hsn, Itemdesc, Uqc, Name, Address, SName, Saddress;
                    int Itemnum = 0, CreatedBy;
                    decimal Qty, UnitPrice, Discount, TaxableValue, Rate, IGST, Invoice_Value;


                    if (Form["portcode"] == "-")
                    {
                        PortCode = "";
                    }
                    else
                    {
                        PortCode = Convert.ToString(Form["portcode"]);
                    }

                    if (Form["billno"] == "-")
                    {
                        BillNo = "";
                    }
                    else
                    {
                        BillNo = Convert.ToString(Form["billno"]);
                    }
                    if (Form["billdate"] == "-")
                    {
                        BillDate = "";
                    }
                    else
                    {
                        BillDate = Convert.ToString(Form["billdate"]);
                    }

                    Gstin = Form["cgstin"];
                    InvoiceNo = Form["invoiceno"];
                    InvoiceDate = Form["invoicedate"];
                    Name = Form["name"];
                    Address = Form["address"];
                    SName = Form["sname"];
                    Saddress = Form["saddress"];
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@BuyerName", SqlDbType.VarChar).Value = Name;
                    cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = Address;
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = CustId;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = UserId;
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    string bid = Models.Common.Functions.InsertIntoTable("TBL_Buyer", cmd, con);
                    con.Close();
                    int BuyerId = Convert.ToInt32(bid);
                    if (InvoiceNo == "-")
                    {
                        InvoiceNo = "NA";
                    }
                    else
                    {

                        string Mode = "O";
                        int Res = InwardFunction.InvoiceValidation(Gstin, InvoiceNo, InvoiceDate, Mode);
                        if (Res == 1)
                        {
                            TempData["Message"] = "Invoice number " + InvoiceNo + " already exist";
                            Session["invoicenoexp"] = Form["invoiceno"];
                            return RedirectToAction("EXP");
                        }

                    }
                    Fp = InvoiceDate.Substring(3, 2) + InvoiceDate.Substring(6, 4);
                    ExpType = Form["ex_typ"];

                    AddInfo = Form["addinfo"];

                    Invoice_Value = Convert.ToDecimal(Form["invoicevalue"]);
                    CreatedBy = UserId;

                    DataTable dt = (DataTable)Session["MasterDtExp"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        Itemnum = Itemnum + 1;
                        Hsn = Convert.ToString(dr["HSN"]);
                        Itemdesc = Convert.ToString(dr["Item Description"]);
                        Qty = Convert.ToDecimal(dr["Quantity"]);
                        Rate = Convert.ToDecimal(dr["rate"]);
                        TaxableValue = Convert.ToDecimal(dr["Taxablevalue"]);
                        IGST = Convert.ToDecimal(dr["IGST Amount"]);
                        UnitPrice = Convert.ToDecimal(dr["Unit Price"]);
                        Discount = Convert.ToDecimal(dr["Discount"]);
                        Uqc = Convert.ToString(dr["UQC"]);
                        Result = ExpFunctions.Insert(Gstin, Fp, ExpType, InvoiceNo, InvoiceDate, Invoice_Value, BillNo, BillDate, PortCode, Rate, TaxableValue, IGST, Refno, Hsn, Itemdesc, Qty, UnitPrice, Discount, Uqc, CreatedBy, AddInfo, Saddress, BuyerId, SName);

                        if (Result == null)
                        {
                            InvoiceNo = "NA";
                        }
                        else
                        {
                            InvoiceNo = Result;
                        }
                    }
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                    {
                        //
                    }
                    else
                    {
                        ExpFunctions.EXPPush(Refno, Gstin, CustId, UserId);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward EXP invoice generated: " + InvoiceNo, "");
                    Session["isautoexp"] = null;
                    Session["invoicenoexp"] = null;
                    Session["invoicedateexp"] = null;
                    Session["cgstinexp"] = null;
                    Session["extypexp"] = null;
                    Session["portcodeexp"] = null;
                    Session["billnoexp"] = null;
                    Session["billdateexp"] = null;
                    Session["ShippingAddressExp"] = null;
                    Session["AddInfoExp"] = null;
                    Session["invoicevalueexp"] = null;
                    Session["discountreqexp"] = null;
                    Session["SnoExp"] = null;
                    Session["MasterDtExp"] = null;
                    Session["nameexp"] = null;
                    Session["addressexp"] = null;
                    Session["snameexp"] = null;
                    Session["saddressexp"] = null;
                    if (command == "save1")
                    {
                        TempData["Message"] = "Invoice Saved Successfully";
                        return RedirectToAction("EXP");
                    }
                    else if (command == "save2")
                    {
                        return RedirectToAction("EXP", "PDFDownload", new { InvoiceNo = InvoiceNo, InvoiceDate = InvoiceDate });
                    }

                }


            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult ExpDelete(int Id)
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

                DataTable dt = ((DataTable)Session["MasterDtExp"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]);
                        Session["invoicevalueexp"] = Convert.ToDecimal(Session["invoicevalueexp"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        TempData["Message"] = "Item Deleted Successfully";
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterDtExp"] = null;
                }
                else
                {
                    Session["MasterDtExp"] = dt;
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("EXP");
        }

        [HttpGet]
        public ActionResult AdvanceTax()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward AT/TXP Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);

                Session["PeriodAT"] = DateTime.Now.ToString("MMyyyy");
                ViewBag.TaxPayerGSTIN = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.Actiontype = OutwardFunctions.GetActionType();
                if (Session["MasterDtAT"] != null)
                {

                    DataTable dt1 = ((DataTable)Session["MasterDtAT"]);

                    return View(ConvertToDictionary(dt1));
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult AdvanceTax(FormCollection Form, string command)
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
                int UserId = Convert.ToInt32(Session["User_ID"]);

                string Refno = Session["CustRefNo"].ToString();
                int Result;
                if (command == "add")
                {
                    Session["GSTINAT"] = Form["cgstin"];
                    Session["PeriodAT"] = Form["period"];
                    int count;
                    DataTable dt = new DataTable();


                    if (Session["SnoAT"] == null)
                    {
                        Session["SnoAT"] = 1;
                        count = (int)Session["SnoAT"];
                    }
                    else
                    {
                        count = (int)Session["SnoAT"];
                        count = count + 1;
                        Session["SnoAT"] = count;
                    }

                    if (Session["MasterDtAT"] == null)
                    {

                        dt.Columns.Add("SnoAT", typeof(int));
                        dt.Columns.Add("Advance Tax Type", typeof(string));
                        dt.Columns.Add("Rate", typeof(string));
                        dt.Columns.Add("Place of Supply", typeof(string));
                        dt.Columns.Add("Advance Amount", typeof(string));
                        dt.Columns.Add("IGST Amount", typeof(string));
                        dt.Columns.Add("CGST Amount", typeof(string));
                        dt.Columns.Add("SGST Amount", typeof(string));
                        dt.Columns.Add("CESS Amount", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterDtAT"]);
                    }


                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = Form["actiontype"];
                    dr[2] = Form["rate"];
                    dr[3] = Form["pos"];
                    dr[4] = Form["ad_amt"];
                    dr[5] = Form["iamount"];
                    dr[6] = Form["camount"];
                    dr[7] = Form["samount"];
                    dr[8] = Form["csamount"];
                    dt.Rows.Add(dr);
                    Session["MasterDtAT"] = dt;

                    ViewBag.TaxPayerGSTIN = LoadDropDowns.GSTIN(UserId, CustId, Session["Role_Name"].ToString());
                    ViewBag.Rates = LoadDropDowns.LoadRates();
                    ViewBag.Actiontype = OutwardFunctions.GetActionType();

                    if (Session["MasterDtAT"] != null)
                    {

                        DataTable dt1 = ((DataTable)Session["MasterDtAT"]);

                        return View(ConvertToDictionary(dt1));
                    }

                    return View();
                }
                else if (command == "save")
                {
                    string GSTIN, FP, ActionType = string.Empty, POS;
                    int Itemnum = 0, CreatedBy;
                    decimal Rate, AdvanceAmount, IGST, CGST, SGST, CESS;

                    GSTIN = Form["cgstin"];
                    FP = Form["period"];
                    CreatedBy = Convert.ToInt32(Session["User_ID"]);

                    DataTable dt = (DataTable)Session["MasterDtAT"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        Itemnum = Itemnum + 1;
                        ActionType = Convert.ToString(dr["Advance Tax Type"]);
                        Rate = Convert.ToDecimal(dr["Rate"]);
                        POS = Convert.ToString(dr["Place of Supply"]);
                        AdvanceAmount = Convert.ToDecimal(dr["Advance Amount"]);
                        IGST = Convert.ToDecimal(dr["IGST Amount"]);
                        CGST = Convert.ToDecimal(dr["CGST Amount"]);
                        SGST = Convert.ToDecimal(dr["SGST Amount"]);
                        CESS = Convert.ToDecimal(dr["CESS Amount"]);
                        Result = AdvanceTaxFunctions.Insert(GSTIN, FP, ActionType, POS, Rate, AdvanceAmount, IGST, CGST, SGST, CESS, Refno, CreatedBy);

                        if (Result == 1)
                        {
                            TempData["Message"] = "Invoice Saved Successfully";
                            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                            {
                                //
                            }
                            else
                            {
                                AdvanceTaxFunctions.ATPush(Refno, GSTIN, CustId, UserId);
                                AdvanceTaxFunctions.TXPPush(Refno, GSTIN, CustId, UserId);
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward AT/TXP invoice generated For: " + GSTIN, "");


                        }

                        else if (Result == -2)
                        {
                            TempData["Message"] = "Item details cannot be inserted";
                            return RedirectToAction("AdvanceTax");
                        }

                    }
                    Session["GSTINAT"] = null;
                    Session["PeriodAT"] = null;
                    Session["MasterDtAT"] = null;
                    Session["SnoAT"] = null;
                    return RedirectToAction("AdvanceTax");

                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult ATDelete(int Id)
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

                DataTable dt = ((DataTable)Session["MasterDtAT"]);
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {

                        dt.Rows.Remove(dtr);
                        TempData["Message"] = "Item Deleted Successfully";
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterDtAT"] = null;
                }
                else
                {
                    Session["MasterDtAT"] = dt;
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("AdvanceTax");
        }
        [HttpGet]
        public ActionResult NilRated()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward NIL Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);

                ViewBag.TaxPayerGSTIN = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.SupplyType = OutwardFunctions.GetNilSupplyType();
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
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                int UserId = Convert.ToInt32(Session["User_ID"]);

                string Refno = Session["CustRefNo"].ToString();

                int Result;
                string GSTIN, FP, SupplyType, NilAmount, ExemptedAmount, NonGSTAmount;
                decimal NilRated, Exempted, NonGST;

                GSTIN = Form["cgstin"];
                FP = Form["Period"];
                SupplyType = Form["supplytype"];
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

                Result = NilFunctions.Insert(GSTIN, FP, NilRated, Exempted, NonGST, SupplyType, Refno, UserId);

                if (Result == 1)
                {
                    TempData["Message"] = "Invoice Saved Successfully";
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                    {
                        //
                    }
                    else
                    {
                        NilFunctions.NilPush(Refno, GSTIN, CustId, UserId);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward Nil Rated invoice generated For: " + GSTIN, "");
                    return RedirectToAction("NilRated");
                }
                else if (Result == -2)
                {
                    TempData["Message"] = "Item details cannot be inserted";
                    return RedirectToAction("NilRated");
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        [HttpGet]
        public ActionResult DocIssue()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward DOCISSUE Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);

                Session["PeriodDoc"] = DateTime.Now.ToString("MMyyyy");
                ViewBag.TaxPayerGSTIN = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.DocType = OutwardFunctions.GetDocType();
                if (Session["MasterDtDoc"] != null)
                {

                    DataTable dt1 = ((DataTable)Session["MasterDtDoc"]);

                    return View(ConvertToDictionary(dt1));
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }
        public ActionResult DocIssue(FormCollection Form, string command)
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
                int UserId = Convert.ToInt32(Session["User_ID"]);

                string Refno = Session["CustRefNo"].ToString();
                int Result;
                if (command == "add")
                {
                    Session["GSTINDoc"] = Form["cgstin"];
                    Session["PeriodDoc"] = Form["period"];
                    int count;
                    DataTable dt = new DataTable();


                    if (Session["SnoDoc"] == null)
                    {
                        Session["SnoDoc"] = 1;
                        count = (int)Session["SnoDoc"];
                    }
                    else
                    {
                        count = (int)Session["SnoDoc"];
                        count = count + 1;
                        Session["SnoDoc"] = count;
                    }

                    if (Session["MasterDtDoc"] == null)
                    {

                        dt.Columns.Add("SnoDoc", typeof(int));
                        dt.Columns.Add("NatureofDocument", typeof(int));
                        dt.Columns.Add("From Serial Number", typeof(string));
                        dt.Columns.Add("To Serial Number", typeof(string));
                        dt.Columns.Add("Total Number", typeof(string));
                        dt.Columns.Add("Cancelled", typeof(string));
                        dt.Columns.Add("Net Issued", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterDtDoc"]);
                    }


                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = Form["DocType"];
                    dr[2] = Form["FromSno"];
                    dr[3] = Form["ToSno"];
                    dr[4] = Form["Total"];
                    dr[5] = Form["Cancelled"];
                    dr[6] = Form["NetIssue"];
                    dt.Rows.Add(dr);
                    Session["MasterDtDoc"] = dt;
                    ViewBag.TaxPayerGSTIN = LoadDropDowns.GSTIN(UserId, CustId, Session["Role_Name"].ToString());
                    ViewBag.DocType = OutwardFunctions.GetDocType();
                    if (Session["MasterDtDoc"] != null)
                    {

                        DataTable dt1 = ((DataTable)Session["MasterDtDoc"]);

                        return View(ConvertToDictionary(dt1));
                    }
                    return View();
                }
                else if (command == "save")
                {
                    string GSTIN, FP;
                    int Itemnum = 0, NatureOfDocument, TotalNumber, Cancelled, NetIssued;
                    string FromSno, ToSno;

                    GSTIN = Form["cgstin"];
                    FP = Form["period"];

                    DataTable dt = (DataTable)Session["MasterDtDoc"];
                    foreach (DataRow dr in dt.Rows)
                    {
                        Itemnum = Itemnum + 1;
                        NatureOfDocument = Convert.ToInt32(dr["NatureofDocument"]);
                        FromSno = Convert.ToString(dr["From Serial Number"]);
                        ToSno = Convert.ToString(dr["To Serial Number"]);
                        TotalNumber = Convert.ToInt32(dr["Total Number"]);
                        Cancelled = Convert.ToInt32(dr["Cancelled"]);
                        NetIssued = Convert.ToInt32(dr["Net Issued"]);

                        Result = DocFunctions.Insert(GSTIN, FP, NatureOfDocument, Itemnum, FromSno, ToSno, TotalNumber, Cancelled, NetIssued, Refno, UserId);

                        if (Result == 1)
                        {
                            TempData["Message"] = "Invoice Saved Successfully";
                            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                            {
                                //
                            }
                            else
                            {
                                DocFunctions.DOCPush(Refno, GSTIN, CustId, UserId);
                            }
                            db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward DocIssue invoice generated For: " + GSTIN, "");

                        }
                        else if (Result == -2)
                        {
                            TempData["Message"] = "Item details cannot be inserted";
                            return RedirectToAction("DocIssue");
                        }


                    }
                    Session["GSTINDoc"] = null;
                    Session["PeriodDoc"] = null;
                    Session["MasterDtDoc"] = null;
                    Session["SnoDoc"] = null;
                    return RedirectToAction("DocIssue");
                }
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult DocDelete(int Id)
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

                DataTable dt = ((DataTable)Session["MasterDtDoc"]);
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {

                        dt.Rows.Remove(dtr);
                        TempData["Message"] = "Item Deleted Successfully";
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterDtDoc"] = null;
                }
                else
                {
                    Session["MasterDtDoc"] = dt;
                }

            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return RedirectToAction("DocIssue");
        }

        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }

        [HttpGet]
        public ActionResult UpdationList()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            try
            {
                ViewBag.MakerList = LoadDropDowns.GetMakerUserlist(CustId, UserId);
                ViewBag.ActionList = OutwardFunctions.GetActionList();
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UpdationList(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            try
            {
                string RefNo = Session["CustRefNo"].ToString();

                string MakerId = "", strUserId = "";
                MakerId = Form["ddlMaker"];
                string Action = Form["Action"];
                TempData["Action"] = Action;
                if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
                {
                    strUserId = MakerId;
                    TempData["UserId"] = MakerId;
                }
                else
                {
                    strUserId = Convert.ToString(UserId);
                    TempData["UserId"] = UserId;
                }
                TempData["RefNo"] = RefNo;

                var invoice = Getinvoice(Action, strUserId, RefNo);
                ViewBag.invoice = invoice;

                ViewBag.MakerList = LoadDropDowns.Exist_GetMakerUserlist(CustId, UserId, MakerId);
                ViewBag.ActionList = OutwardFunctions.Exist_GetActionList(Action);
            }
            catch (Exception ex)
            {
                TempData["Message"] = ex.Message;
            }
            return View();
        }

        public ActionResult Delete(string gstin, string period, string ActionType, string strUserId, string strRefNo)
        {
            string CustRefNo = Session["CustRefNo"].ToString();
            if (ActionType == "AT")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_AT where li.gstin == gstin && li.fp == period && li.referenceno == CustRefNo select li.atid).ToList();


                foreach (int Refid in invoiceid)
                {
                    int Result = DeleteAll(ActionType, Refid);

                    if (Result == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 AT Invoice For: " + gstin + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (Result == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete!";
                    }
                    else if (Result == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete!";
                    }
                }
            }
            else if (ActionType == "DOCIssue")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_DOC where li.gstin == gstin && li.fp == period && li.referenceno == CustRefNo select li.docid).ToList();


                foreach (int Refid in invoiceid)
                {
                    int Result = DeleteAll(ActionType, Refid);

                    if (Result == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 DOC ISSUE Invoice For: " + gstin + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (Result == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete!";
                    }
                    else if (Result == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete!";
                    }
                }
            }
            else if (ActionType == "NIL")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_NIL where li.gstin == gstin && li.fp == period && li.referenceno == CustRefNo select li.nilid).ToList();


                foreach (int Refid in invoiceid)
                {
                    int Result = DeleteAll(ActionType, Refid);

                    if (Result == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 NIL Invoice For: " + gstin + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (Result == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete!";
                    }
                    else if (Result == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete!";
                    }
                }
            }
            else if (ActionType == "TXP")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR1_TXP where li.gstin == gstin && li.fp == period && li.referenceno == CustRefNo select li.txpid).ToList();


                foreach (int Refid in invoiceid)
                {
                    int Result = DeleteAll(ActionType, Refid);

                    if (Result == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR1 TXP Invoice For: " + gstin + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (Result == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 Save. So Please Delete in GSTR View and Delete!";
                    }
                    else if (Result == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR1 View. So Please Delete in GSTR View and Delete!";
                    }
                }

            }

            return RedirectToAction("UpdationList");
        }
        public int DeleteAll(string Action, int InvId)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ActionType", Action));
                cmd.Parameters.Add(new SqlParameter("@RefId", InvId));
                cmd.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@ErrorCode"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        public List<IDictionary> Getinvoice(string action, string strUserId, string strRefNo)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_OutwardUpdate_GSTR1_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", strRefNo));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", action));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", strUserId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ConvertToDictionary(ds.Tables[0]);

        }
    }
}