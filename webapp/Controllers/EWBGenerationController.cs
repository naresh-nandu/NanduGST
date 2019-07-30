using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using WeP_BAL;
using WeP_BAL.EwayBill;
using WeP_DAL;
using WeP_DAL.EwayBill;
using WeP_EWayBill;
using System.Web.UI;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using System.Net.Mail;
using iTextSharp.tool.xml;
using System.IO;
using System.Net;
using SmartAdminMvc.Models.EWAY;

namespace SmartAdminMvc.Controllers
{
    public class BusinessLayer : EwbBusinessLayer { }
    public class EwbGenerationController : Controller
    {
        #region "Eway Bill Generation"
        [HttpGet]
        public ActionResult EWB()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            try
            {
                Session["docDate"] = DateTime.Now.ToString("dd/MM/yyyy");

                #region "Eway Bill Dropdowns"
                if (Session["strGSTINNo"] != null)
                {
                    ViewBag.GstinNo = Session["strGSTINNo"].ToString();
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(UserId, CustId, Session["strGSTINNo"].ToString(), Session["Role_Name"].ToString());

                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(Session["strGSTINNo"].ToString());
                    int branchId = 0;
                    string strBranchId = Session["branchId"].ToString();
                    if (!string.IsNullOrEmpty(strBranchId))
                    {
                        branchId = 0;
                    }
                    ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.branchList(UserId, CustId, GstinId, Session["Role_Name"].ToString());
                    if (branchId != 0)
                    {
                        ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(UserId, CustId, GstinId, Session["Role_Name"].ToString(), branchId);
                    }
                }
                else
                {
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                }

                if (Session["transMode"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transMode"].ToString());
                }
                else
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                }

                if (Session["supplyType"] != null)
                {
                    ViewBag.SupplyType = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("SupplyType", Session["supplyType"].ToString());
                    ViewBag.SubSupplyType = Models.Common.LoadDropDowns.DropDownSubSupplyType("SubSupplyType", Session["supplyType"].ToString());
                    if (Session["subsupplyType"].ToString() != null)
                    {
                        ViewBag.SubSupplyType = Models.Common.LoadDropDowns.Exist_DropDownSubSupplyType("SubSupplyType", Session["supplyType"].ToString(), Session["subsupplyType"].ToString());
                    }
                }
                else
                {
                    ViewBag.SupplyType = Models.Common.LoadDropDowns.DropDownEwayBill("SupplyType");
                    ViewBag.SubSupplyType = Models.Common.LoadDropDowns.DropDownSubSupplyType("SubSupplyType", "");
                }
                if (Session["documentType"] != null)
                {
                    ViewBag.DocumentType = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("DocumentType", Session["documentType"].ToString());
                }
                else
                {
                    ViewBag.DocumentType = Models.Common.LoadDropDowns.DropDownEwayBill("DocumentType");
                }

                if (Session["invoiceType"] != null)
                {
                    ViewBag.InvoiceTypeList = WeP_BAL.EwayBill.LoadDropDowns.Exist_invoiceTypeList(Session["invoiceType"].ToString());
                }
                else
                {
                    ViewBag.InvoiceTypeList = WeP_BAL.EwayBill.LoadDropDowns.invoiceTypeList();
                }

                if (Session["TransactionType"] != null)
                {
                    ViewBag.TransactionTypeList = WeP_BAL.EwayBill.LoadDropDowns.Exist_TransactionTypeList(Session["TransactionType"].ToString());
                }
                else
                {
                    ViewBag.TransactionTypeList = WeP_BAL.EwayBill.LoadDropDowns.TransactionTypeList();
                }
                if(Session["CessNonAdvoleway"]==null)
                {
                    Session["CessNonAdvoleway"] = 0;
                    Session["Othereway"] = 0;
                }


                    ViewBag.Units = Models.Common.LoadDropDowns.DropDownEwayBill("Units");
                ViewBag.TransactionTypeList = WeP_BAL.EwayBill.LoadDropDowns.TransactionTypeList();
                #endregion

                #region "Existing Eway Bill Dropdowns while Deleting Line Items"
                if (Session["MasterdtlsEWAY"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsEWAY"]);
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(Session["strGSTINNo"].ToString());
                    ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(UserId, CustId, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(Session["branchId"]));
                    return View(ConvertToDictionary(dt));
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EWB(FormCollection Form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string Refno = Session["CustRefNo"].ToString();
            string email = "";
            try
            {
                #region "Variable Intialization and Declaration
                string dispatchName, dispatchGstin, dispatchAdd1, dispatchAdd2, dispatchCity,
                       dispatchStateCode, actFromStateCode, dispatchPinCode, ShipToName, ShipToGstin, ShipToAdd1,
                       ShipToAdd2, ShipToCity, ShipToStateCode, actToStateCode, ShipToPinCode, transMode, transName,
                       transId, transdocNo, transdocDate, approDistance, vehicleType, vehicleNo, invoiceType,
                       hsn, itemDesc, productName, uqc, supplyType, subsupplyType, documentType, strGSTINNo,
                       docNo, docDate, strBranchId, subsupplyDesc = "", ActDispatchFromGSTIN, ActDispatchFromTradeName, ActShipToGSTIN, ActShipToTradeName, TransactionType;
                decimal rt, taxValue, Igst, Cgst, Sgst, Cess, Qty, Unitprice,otherval,cessnonadvolval;
                int itemNo = 0, branchId = 0;
                string ewbNo = "", ewbDate = "";
                #endregion

                #region "Loading Existing DropDowns"
                TransactionType = Form["TransactionType"];
                supplyType = Form["supType"];
                subsupplyType = Form["subSupType"];
                documentType = Form["docType"];
                transMode = Form["transMode"];
                strGSTINNo = Form["ddlGSTINNo"];
                strBranchId = Form["branchId"];
                if (string.IsNullOrEmpty(strBranchId))
                {
                    strBranchId = "0";
                }
                branchId = Convert.ToInt32(strBranchId);
                invoiceType = Form["invoiceType"];
                supplyType = Form["supType"];
                subsupplyType = Form["subSupType"];
                documentType = Form["docType"];
                transMode = Form["transMode"];
                strGSTINNo = Form["ddlGSTINNo"];
                docNo = Form["docNo"];
                docDate = Form["docDate"];
                invoiceType = Form["invoiceType"];
                dispatchName = Form["dispatchName"];
                dispatchGstin = Form["dispatchGstin"];
                dispatchAdd1 = Form["dispatchAdd1"];
                dispatchAdd2 = Form["dispatchAdd2"];
                dispatchCity = Form["dispatchCity"];
                dispatchStateCode = Form["dispatchStateCode"];
                actFromStateCode = Form["actfromStateCode"];
                dispatchPinCode = Form["dispatchPinCode"];
                ShipToName = Form["ShipToName"];
                ShipToGstin = Form["ShipToGstin"];
                ShipToAdd1 = Form["ShipToAdd1"];
                ShipToAdd2 = Form["ShipToAdd2"];
                ShipToCity = Form["ShipToCity"];
                ShipToStateCode = Form["ShipToStateCode"];
                actToStateCode = Form["acttoStateCode"];
                ShipToPinCode = Form["ShipToPinCode"];
                transMode = Form["transMode"];
                transName = Form["transName"];
                transId = Form["transId"];
                transdocNo = Form["transdocNo"];
                transdocDate = Form["transdocDate"];
                approDistance = Form["approDistance"];
                ViewBag.Distance = approDistance;
                vehicleType = Form["vehicleType"];
                vehicleNo = Form["vehicleNo"];
                ActDispatchFromGSTIN = Form["ActDispatchFromGSTIN"];
                ActDispatchFromTradeName = Form["ActDispatchFromTradeName"];
                ActShipToGSTIN = Form["ActShipToGSTIN"];
                ActShipToTradeName = Form["ActShipToTradeName"];
                
                ViewBag.GstinNo = strGSTINNo;

                if (subsupplyType == "8")
                {
                    ViewBag.subsupplyDesc = subsupplyType;
                    subsupplyDesc = Form["subsupplyDesc"];
                }
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.branchList(UserId, CustId, GstinId, Session["Role_Name"].ToString());
                    if (branchId != 0)
                    {
                        ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(UserId, CustId, GstinId, Session["Role_Name"].ToString(), branchId);
                    }
                }

                #region "Storing Session Varaible for loading Existing Dropdown when line Items are Deleted"
                #region "Master List"
                Session["supplyType"] = supplyType;
                Session["subsupplyType"] = subsupplyType;
                Session["documentType"] = documentType;
                Session["transMode"] = transMode;
                Session["strGSTINNo"] = strGSTINNo;
                Session["invoiceType"] = invoiceType;
                Session["branchId"] = branchId;
                Session["docNo"] = docNo;
                Session["docDate"] = docDate;
                Session["subsupplyDesc"] = subsupplyDesc;
                Session["TransactionType"] = TransactionType;
                #endregion

                #region "Dispatch Address"
                Session["dispatchName"] = dispatchName;
                Session["dispatchGstin"] = dispatchGstin;
                Session["dispatchAdd1"] = dispatchAdd1;
                Session["dispatchAdd2"] = dispatchAdd2;
                Session["dispatchCity"] = dispatchCity;
                Session["dispatchStateCode"] = dispatchStateCode;
                Session["dispatchPinCode"] = dispatchPinCode;
                Session["actfromStateCode"] = actFromStateCode;
                #endregion

                #region "ShipTo Address"
                Session["ShipToName"] = ShipToName;
                Session["ShipToGstin"] = ShipToGstin;
                Session["ShipToAdd1"] = ShipToAdd1;
                Session["ShipToAdd2"] = ShipToAdd2;
                Session["ShipToCity"] = ShipToCity;
                Session["ShipToStateCode"] = ShipToStateCode;
                Session["ShipToPinCode"] = ShipToPinCode;
                Session["acttoStateCode"] = actToStateCode;
                #endregion

                #region TransPort and Vechicle Details"
                Session["transMode"] = transMode;
                Session["transName"] = transName;
                Session["transId"] = transId;
                Session["transdocNo"] = transdocNo;
                Session["transdocDate"] = transdocDate;
                Session["approDistance"] = approDistance;
                Session["vehicleType"] = vehicleType;
                Session["vehicleNo"] = vehicleNo;
                #endregion

                #region "New Parameters"
                Session["ActDispatchFromGSTIN"] = ActDispatchFromGSTIN;
                Session["ActDispatchFromTradeName"] = ActDispatchFromTradeName;
                Session["ActShipToGSTIN"] = ActShipToGSTIN;
                Session["ActShipToTradeName"] = ActShipToTradeName;
               
                #endregion
                #endregion

                ViewBag.TransactionTypeList = WeP_BAL.EwayBill.LoadDropDowns.Exist_TransactionTypeList(TransactionType);
                ViewBag.InvoiceTypeList = WeP_BAL.EwayBill.LoadDropDowns.Exist_invoiceTypeList(invoiceType);
                ViewBag.SupplyType = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("SupplyType", supplyType);
                ViewBag.DocumentType = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("DocumentType", documentType);
                if (supplyType != null)
                {
                    ViewBag.SubSupplyType = Models.Common.LoadDropDowns.DropDownSubSupplyType("SubSupplyType", supplyType);
                    if (subsupplyType != null)
                    {
                        ViewBag.SubSupplyType = Models.Common.LoadDropDowns.Exist_DropDownSubSupplyType("SubSupplyType", supplyType, subsupplyType);
                    }
                }
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", transMode);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.Units = Models.Common.LoadDropDowns.DropDownEwayBill("Units");

                #endregion

                #region "Switch for Add and save"
                switch (command)
                {
                    case "add":
                        decimal invoicevalue, invalue;

                        #region "calculations of Item List Values"
                        decimal igst = Convert.ToDecimal(Form["iamount"]);
                        decimal igstval = Convert.ToDecimal(Session["totaligstvalue"]) + igst;
                        Session["totaligstvalue"] = igstval;

                        decimal cgst = Convert.ToDecimal(Form["camount"]);
                        decimal cgstval = Convert.ToDecimal(Session["totalcgstvalue"]) + cgst;
                        Session["totalcgstvalue"] = cgstval;

                        decimal sgst = Convert.ToDecimal(Form["samount"]);
                        decimal sgstval = Convert.ToDecimal(Session["totalsgstvalue"]) + sgst;
                        Session["totalsgstvalue"] = sgstval;

                        decimal cess = Convert.ToDecimal(Form["csamount"]);
                        decimal cessval = Convert.ToDecimal(Session["totalcessvalue"]) + cess;
                        Session["totalcessvalue"] = cessval;

                        decimal teaxableValue = Convert.ToDecimal(Form["taxablevalue"]);
                        decimal teaxableValueval = Convert.ToDecimal(Session["totaltexablevalue"]) + teaxableValue +
                            Convert.ToDecimal(Session["totaligstvalue"]) + Convert.ToDecimal(Session["totalcgstvalue"]) +
                            Convert.ToDecimal(Session["totalsgstvalue"]) + Convert.ToDecimal(Session["totalcessvalue"]);
                        Session["totaltexablevalue"] = teaxableValueval;
                        decimal totTaxableValue = Convert.ToDecimal(Session["TotalTaxableValue"]);
                        totTaxableValue += teaxableValue;
                        Session["TotalTaxableValue"] = totTaxableValue;
                         
                        cessnonadvolval = Convert.ToDecimal(Form["CessNonAdvolvalue"]);
                        otherval = Convert.ToDecimal(Form["Othervalue"]);
                        Session["CessNonAdvoleway"] = cessnonadvolval;
                        Session["Othereway"] = otherval;
                        #endregion

                        #region "Creating Datatable for Storing Line Item Details"

                        int count;
                        DataTable dt = new DataTable();

                        if (Session["SnoEwb"] == null)
                        {
                            Session["SnoEwb"] = 1;
                            count = (int)Session["SnoEwb"];
                        }
                        else
                        {
                            count = (int)Session["SnoEwb"];
                            count = count + 1;
                            Session["SnoEwb"] = count;
                        }

                        if (Session["MasterdtlsEWAY"] == null)
                        {

                            dt.Columns.Add("Srno", typeof(string));
                            dt.Columns.Add("HSN", typeof(string));
                            dt.Columns.Add("ItemDesc", typeof(string));
                            dt.Columns.Add("Product Name", typeof(string));
                            dt.Columns.Add("Quantity", typeof(string));
                            dt.Columns.Add("UQC", typeof(string));
                            dt.Columns.Add("Unit Price", typeof(string));

                            dt.Columns.Add("Taxablevalue", typeof(string));
                            dt.Columns.Add("Rate", typeof(string));
                            dt.Columns.Add("IGST Amount", typeof(string));
                            dt.Columns.Add("CGST Amount", typeof(string));
                            dt.Columns.Add("SGST Amount", typeof(string));
                            dt.Columns.Add("CESS Amount", typeof(string));
                        }
                        else
                        {
                            dt = ((DataTable)Session["MasterdtlsEWAY"]);
                        }


                        DataRow dr = dt.NewRow();

                        dr[0] = count;
                        dr[1] = Form["hsn"];
                        dr[2] = Form["itemdesc"];
                        dr[3] = Form["productname"];
                        dr[4] = Form["qty"];
                        dr[5] = Form["unit"];
                        dr[6] = Form["unitprice"];
                        dr[7] = Form["taxablevalue"];
                        dr[8] = Form["rate"];
                        dr[9] = Form["iamount"];
                        dr[10] = Form["camount"];
                        dr[11] = Form["samount"];
                        dr[12] = Form["csamount"];
                        dt.Rows.Add(dr);
                        Session["MasterdtlsEWAY"] = dt;
                        if (Session["invalueoeway"] == null)
                        {
                            invalue = 0;
                        }
                        else
                        {
                            invalue = Convert.ToDecimal(Form["invalue"]);
                        }
                        invoicevalue = Convert.ToDecimal(Form["taxablevalue"]) + Convert.ToDecimal(Form["camount"]) + Convert.ToDecimal(Form["samount"]) + Convert.ToDecimal(Form["iamount"]) + invalue;
                        Session["invalueoeway"] = invoicevalue;

                        if (Session["MasterdtlsEWAY"] != null)
                        {
                            DataTable dt1 = ((DataTable)Session["MasterdtlsEWAY"]);
                            return View(ConvertToDictionary(dt1));
                        }
                        #endregion
                        break;
                    case "Close":
                        DataTable p_dt = new DataTable();
                        using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            sqlcon.Open();
                            using (SqlCommand sqlcmd = new SqlCommand())
                            {
                                sqlcmd.Connection = sqlcon;
                                sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION Where userGSTIN = @userGSTIN and docNo = @docNo and docDate = @docDate and ISNULL(ewayBillNo, '') != ''";
                                sqlcmd.Parameters.AddWithValue("@userGSTIN", strGSTINNo);
                                sqlcmd.Parameters.AddWithValue("@docNo", docNo);
                                sqlcmd.Parameters.AddWithValue("@docDate", docDate);

                                using (SqlDataAdapter p_adt = new SqlDataAdapter(sqlcmd))
                                {
                                    p_adt.Fill(p_dt);
                                    if (p_dt.Rows.Count > 0)
                                    {
                                        ewbNo = p_dt.Rows[0]["ewayBillNo"].ToString();
                                        return RedirectToAction("EWB", "EWBPrint", new { ewbNo });
                                    }
                                }
                            }
                            sqlcon.Close();
                        }
                        break;
                    case "Confirm":
                    case "save":

                        DataTable DT = (DataTable)Session["MasterdtlsEWAY"];
                        foreach (DataRow DR in DT.Rows)
                        {
                            #region "Datatable line Items For Inserting DB"
                            itemNo = itemNo + 1;
                            hsn = Convert.ToString(DR["HSN"]);
                            itemDesc = Convert.ToString(DR["ItemDesc"]);
                            productName = Convert.ToString(DR["Product Name"]);
                            Qty = Convert.ToDecimal(DR["Quantity"]);
                            rt = Convert.ToDecimal(DR["rate"]);
                            taxValue = Convert.ToDecimal(DR["Taxablevalue"]);
                            Unitprice = Convert.ToDecimal(DR["Unit Price"]);
                            uqc = Convert.ToString(DR["UQC"]);
                            Igst = Convert.ToDecimal(DR["IGST Amount"]);
                            Cgst = Convert.ToDecimal(DR["CGST Amount"]);
                            Sgst = Convert.ToDecimal(DR["SGST Amount"]);
                            Cess = Convert.ToDecimal(DR["CESS Amount"]);
                            #endregion

                            #region "Rate Calculation"
                            if (Cgst != 0 && Sgst != 0)
                            {
                                Decimal rate = rt / 2;
                                Cgst = rate;
                                Sgst = rate;
                                Igst = 0;
                            }
                            else
                            {
                                Igst = rt;
                            }
                            #endregion

                            int status = BusinessLayer.duplicatedocNoChecking(docNo, docDate, CustId);
                            if (command != "Confirm")
                            {
                                if (status == 1)
                                {
                                    ViewBag.docNoDialogue = "OPEN_docNoPOPUP";
                                    TempData["ConfirmMessage"] = "Document Number " + docNo + " Already Exist. Are You Sure Want to Continue?";
                                    DataTable dt1 = ((DataTable)Session["MasterdtlsEWAY"]);
                                    return View(ConvertToDictionary(dt1));
                                }
                            }
                            #region "inserting eway bill details"
                            EwaybillDataAccess.ewb_insert(strGSTINNo, supplyType, subsupplyType, documentType,
                              dispatchName, dispatchGstin, dispatchAdd1, dispatchAdd2, dispatchCity,
                              dispatchStateCode, actFromStateCode, dispatchPinCode, ShipToName, ShipToGstin, ShipToAdd1,
                              ShipToAdd2, ShipToCity, ShipToStateCode, actToStateCode, ShipToPinCode, transMode,
                              transName, transId, transdocNo, transdocDate, approDistance, vehicleType, ActDispatchFromGSTIN, ActDispatchFromTradeName, ActShipToGSTIN, ActShipToTradeName, TransactionType,
                            vehicleNo, docNo, docDate, hsn, itemDesc, productName, uqc, taxValue, Igst, Cgst,
                              Sgst, Cess, Qty, Unitprice, Convert.ToDecimal(Session["totaligstvalue"]),
                              Convert.ToDecimal(Session["totalcgstvalue"]), Convert.ToDecimal(Session["totalsgstvalue"]),
                              Convert.ToDecimal(Session["totalcessvalue"]), Convert.ToDecimal(Session["TotalTaxableValue"]),
                              Convert.ToDecimal(Session["totaltexablevalue"]), Convert.ToDecimal(Session["CessNonAdvoleway"]), Convert.ToDecimal(Session["Othereway"]), Refno, UserId.ToString(), branchId, invoiceType, subsupplyDesc);
                            #endregion

                        }

                        #region "Clearing Session Variables"
                        Session["supplyType"] = null;
                        Session["subsupplyType"] = null;
                        Session["documentType"] = null;
                        Session["transMode"] = null;
                        Session["invoiceType"] = null;
                        Session["strGSTINNo"] = null;
                        Session["docNo"] = null;
                        Session["docDate"] = null;
                        Session["totaligstvalue"] = null;
                        Session["totalcgstvalue"] = null;
                        Session["totalsgstvalue"] = null;
                        Session["totalcessvalue"] = null;
                        Session["totaltexablevalue"] = null;
                        Session["TotalTaxableValue"] = null;
                        Session["SnoEwb"] = null;
                        Session["invalueoeway"] = null;
                        Session["MasterdtlsEWAY"] = null;
                        Session["dispatchName"] = null;
                        Session["dispatchGstin"] = null;
                        Session["dispatchAdd1"] = null;
                        Session["dispatchAdd2"] = null;
                        Session["dispatchCity"] = null;
                        Session["dispatchStateCode"] = null;
                        Session["actfromStateCode"] = null;
                        Session["dispatchPinCode"] = null;
                        Session["ShipToName"] = null;
                        Session["ShipToGstin"] = null;
                        Session["ShipToAdd1"] = null;
                        Session["ShipToAdd2"] = null;
                        Session["ShipToCity"] = null;
                        Session["ShipToStateCode"] = null;
                        Session["acttoStateCode"] = null;
                        Session["ShipToPinCode"] = null;
                        Session["transMode"] = null;
                        Session["transName"] = null;
                        Session["transId"] = null;
                        Session["transdocNo"] = null;
                        Session["transdocDate"] = null;
                        Session["approDistance"] = null;
                        Session["vehicleType"] = null;
                        Session["vehicleNo"] = null;
                        Session["branchId"] = null;
                        Session["subsupplyDesc"] = null;
                        Session["ActDispatchFromGSTIN"] = null;
                        Session["ActDispatchFromTradeName"] = null;
                        Session["ActShipToGSTIN"] = null;
                        Session["ActShipToTradeName"] = null;
                        Session["TransactionType"] = null;
                        Session["CessNonAdvoleway"] = 0;
                        Session["Othereway"] = 0;
                        #endregion
                        ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                        ViewBag.GstinNo = null;
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                        ViewBag.SupplyType = Models.Common.LoadDropDowns.DropDownEwayBill("SupplyType");
                        ViewBag.SubSupplyType = Models.Common.LoadDropDowns.DropDownSubSupplyType("SubSupplyType", "");
                        ViewBag.DocumentType = Models.Common.LoadDropDowns.DropDownEwayBill("DocumentType");
                        ViewBag.InvoiceTypeList = WeP_BAL.EwayBill.LoadDropDowns.invoiceTypeList();
                        ViewBag.Units = Models.Common.LoadDropDowns.DropDownEwayBill("Units");
                        ViewBag.TransactionTypeList = WeP_BAL.EwayBill.LoadDropDowns.TransactionTypeList();

                        #region "eway bill Generation Api Calling
                        docNo = Form["docNo"];
                        docDate = Form["docDate"];
                        string OStatus = "0";
                        string OResponse = "";
                        string transdocno = Form["transporterDocNo"];
                        string trandocdt = Form["transdocDate"];
                        transMode = Form["transMode"];

                        string strJsonData = EwbJsonDataModel.GetJsonEWBGeneration(strGSTINNo, docNo, docDate);
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                        EwbGeneration.EWAYBILL_GEN(docNo, docDate, strJsonData, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OStatus, out OResponse, out ewbNo, out ewbDate);
                        if (OStatus == "0")
                        {
                            TempData["ErrorMessage"] = OResponse;
                        }

                        else
                        {
                            string Email = "";
                            EwbBusinessLayer.EWayBillEmail(CustId, branchId, out Email);
                            if (Email != string.Empty)
                            {
                                string UserEmail = "";
                                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                                {
                                    using (SqlCommand sqlcmd = new SqlCommand())
                                    {
                                        sqlcmd.Connection = sqlcon;
                                        sqlcmd.CommandText = "select Email from UserList where UserId= @UserId";
                                        sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                                        using (SqlDataAdapter da1 = new SqlDataAdapter(sqlcmd))
                                        {
                                            DataTable dt1 = new DataTable();
                                            da1.Fill(dt1);
                                            if (dt1.Rows.Count > 0)
                                            {
                                                UserEmail = dt1.Rows[0]["Email"].ToString();
                                            }
                                        }
                                    }
                                }

                                SendPDFEmail1(ewbNo, strGSTINNo, Email, UserEmail);
                            }
                            return RedirectToAction("EWB", "EWBPrint", new { ewbNo });
                        }

                        #endregion

                        break;
                    default:
                        #region "ON CHANGE EVENT Authentication Checking"

                        if (supplyType == "O" && subsupplyType == "3")
                        {
                            Session["dispatchGstin"] = strGSTINNo;
                            Session["dispatchStateCode"] = strGSTINNo.Substring(0, 2);
                            Session["actfromStateCode"] = strGSTINNo.Substring(0, 2);
                            Session["dispatchPinCode"] = null;
                            Session["ShipToGstin"] = "URP";
                            Session["ShipToStateCode"] = "00";
                            Session["acttoStateCode"] = "00";
                            Session["ShipToPinCode"] = "999999";
                        }
                        else if (supplyType == "I" && subsupplyType == "2")
                        {
                            Session["dispatchGstin"] = "URP";
                            Session["dispatchStateCode"] = "00";
                            Session["actfromStateCode"] = "00";
                            Session["dispatchPinCode"] = "999999";
                            Session["ShipToGstin"] = strGSTINNo;
                            Session["ShipToStateCode"] = strGSTINNo.Substring(0, 2);
                            Session["acttoStateCode"] = strGSTINNo.Substring(0, 2);
                            Session["ShipToPinCode"] = null;
                        }
                        else
                        {
                            if (supplyType == "O")
                            {
                                Session["dispatchGstin"] = strGSTINNo;
                                Session["dispatchStateCode"] = strGSTINNo.Substring(0, 2);
                                Session["actfromStateCode"] = strGSTINNo.Substring(0, 2);
                                Session["dispatchPinCode"] = null;
                                Session["ShipToGstin"] = null;
                                Session["ShipToStateCode"] = null;
                                Session["acttoStateCode"] = null;
                                Session["ShipToPinCode"] = null;
                            }
                            if (supplyType == "I")
                            {
                                Session["dispatchGstin"] = null;
                                Session["dispatchStateCode"] = null;
                                Session["actfromStateCode"] = null;
                                Session["dispatchPinCode"] = null;
                                Session["ShipToGstin"] = strGSTINNo;
                                Session["ShipToStateCode"] = strGSTINNo.Substring(0, 2);
                                Session["acttoStateCode"] = strGSTINNo.Substring(0, 2);
                                Session["ShipToPinCode"] = null;
                            }
                        }

                        strGSTINNo = Form["ddlGSTINNo"];
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion

                        if (Session["MasterdtlsEWAY"] != null)
                        {
                            DataTable dt1 = ((DataTable)Session["MasterdtlsEWAY"]);

                            return View(ConvertToDictionary(dt1));
                        }
                        break;

                }
                #endregion

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public int docNoChecking(string docNo, string docDate)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int status = BusinessLayer.duplicatedocNoChecking(docNo, docDate, CustId);
            return status;
        }
        [HttpPost]
        public JsonResult CustList(string Prefix)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            DataSet DS = EwaybillDataAccess.getCustomerList(Prefix, CustId);

            List<EwayAttributes.CustomerMgmt> CustMgmt = new List<EwayAttributes.CustomerMgmt>();

            #region "Customer Details for Auto Populate"

            foreach (DataRow dr in DS.Tables[0].Rows)
            {
                CustMgmt.Add(new EwayAttributes.CustomerMgmt
                {
                    gstinNo = dr["GSTINno"].ToString(),
                    Address = dr["Address"].ToString(),
                    buyerName = dr["BuyerName"].ToString(),
                    buyerId = Convert.ToInt32(dr["BuyerId"]),
                    custId = Convert.ToInt32(dr["CustomerId"])
                });

            }

            #endregion

            return Json(CustMgmt, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult SupplierList(string Prefix)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            DataSet DS = EwaybillDataAccess.getSupplierList(Prefix, CustId);

            List<EwayAttributes.SupplierMgmt> SupMgmt = new List<EwayAttributes.SupplierMgmt>();

            #region "Supplier Details for Auto Populate"

            foreach (DataRow dr in DS.Tables[0].Rows)
            {
                SupMgmt.Add(new EwayAttributes.SupplierMgmt
                {
                    gstinNo = dr["GSTINno"].ToString(),
                    Address = dr["Address"].ToString(),
                    supplierName = dr["SupplierName"].ToString(),
                    supplierId = Convert.ToInt32(dr["SupplierId"]),
                    custId = Convert.ToInt32(dr["CustomerId"])
                });

            }

            #endregion

            return Json(SupMgmt, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Delete(int id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                #region "Line Item delete of Eway Bill"
                DataTable dt = ((DataTable)Session["MasterdtlsEWAY"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CGST Amount"]) + Convert.ToDecimal(dtr["SGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invalueoeway"] = Convert.ToDecimal(Session["invalueoeway"]) - inv_value;

                        Session["TotalTaxableValue"] = Convert.ToDecimal(Session["TotalTaxableValue"]) - Convert.ToDecimal(dtr["Taxablevalue"]);
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsEWAY"] = null;
                }
                else
                {
                    Session["MasterdtlsEWAY"] = dt;
                }
                TempData["ItemDeletedMessage"] = "Item Deleted Successfully";

                return RedirectToAction("EWB");
                #endregion;
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("EWB");
            }

        }



        #endregion

        #region"get distance from pincodes"
        public JsonResult GetDist(string data, string data1)
        {
            int Distance = 0;
            PinDistanceApiCall.GetDistance(Convert.ToInt32(data), Convert.ToInt32(data1), out Distance);
            return Json(Distance, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region "Update Vechicle Number"

        [HttpGet]
        public ActionResult UPDVEHNO()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            try
            {
                ViewBag.TransDocDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ReasonList = Models.Common.LoadDropDowns.GetEWBReasonList();
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UPDVEHNO(FormCollection Form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            try
            {
                string strGSTINNo, ewbNo, vehicleNo, fromPlace, fromState, reasonCode, reasonRmrk, transDocNo, transDocDate, transMode;
                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ReasonList = Models.Common.LoadDropDowns.GetEWBReasonList();
                strGSTINNo = Form["ddlGSTINNo"];
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());

                switch (command)
                {
                    case "getDetails":
                        strGSTINNo = Form["ddlGSTINNo"];
                        ewbNo = Form["ewbNo"];
                        ViewBag.ewbNo = ewbNo;
                        ViewBag.gstinno = strGSTINNo;

                        #region "Retrieve Vechicle Details"

                        DataSet DS = EwaybillDataAccess.getVechicleDetails(strGSTINNo, ewbNo, CustId);
                        List<EwayAttributes.VehicleHistory> VechicleDetail = new List<EwayAttributes.VehicleHistory>();

                        foreach (DataRow dr in DS.Tables[0].Rows)
                        {
                            VechicleDetail.Add(new EwayAttributes.VehicleHistory
                            {
                                userGSTIN = dr.IsNull("userGSTIN") ? "" : dr["userGSTIN"].ToString(),
                                ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                                vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                                fromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                                fromState = dr.IsNull("FromState") ? "" : dr["FromState"].ToString(),
                                reasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                                reasonRemark = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                                transDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                                transDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                                transMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                                vehupddt = dr.IsNull("vehUpdDate") ? "" : dr["vehUpdDate"].ToString(),
                                errorCode = dr.IsNull("UPD_errorCodes") ? "" : dr["UPD_errorCodes"].ToString(),
                                errorDesc = dr.IsNull("UPD_errorDescription") ? "" : dr["UPD_errorDescription"].ToString()
                            });

                        }

                        EwayViewModel Model = new EwayViewModel();
                        Model.VehicleHistory = VechicleDetail;

                        #endregion

                        return View(Model);

                    case "updateDetails":

                        strGSTINNo = Form["ddlGSTINNo"];
                        ewbNo = Form["ewbNo"];
                        vehicleNo = Form["vehicleNo"];
                        fromPlace = Form["fromplace"];
                        fromState = Form["dispatchStateCode"];
                        reasonCode = Form["reasonCode"];
                        reasonRmrk = Form["reasonRmrk"];
                        transMode = Form["transMode"];
                        transDocNo = Form["transdocNo"];
                        transDocDate = Form["transdocDate"];
                        ViewBag.TransDocDate = transDocDate;
                        EwaybillDataAccess.Insert_Vechicle_Details(strGSTINNo, ewbNo, vehicleNo, fromPlace, fromState, reasonCode, reasonRmrk, transMode, transDocNo, transDocDate, Session["CustRefNo"].ToString(), UserId);
                        ViewBag.ewbNo = ewbNo;

                        #region "API Calling for Vechicle Updation"
                        string OStatus = "0";
                        string OResponse = "";

                        string strJsonData = EwbJsonDataModel.GetJsonEWBUpdateVehicleNo(strGSTINNo, ewbNo, vehicleNo);
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                        EwbGeneration.EWB_UPDATE_VEHICLENO(ewbNo, strJsonData, strGSTINNo,
                            Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(),
                            out OStatus, out OResponse);

                        TempData["SuccessMessage"] = OResponse;


                        #region "Retrieving Vechicle Details"

                        DataSet Ds = EwaybillDataAccess.getVechicleDetails(strGSTINNo, ewbNo, CustId);
                        List<EwayAttributes.VehicleHistory> VechicleList = new List<EwayAttributes.VehicleHistory>();

                        foreach (DataRow dr in Ds.Tables[0].Rows)
                        {
                            VechicleList.Add(new EwayAttributes.VehicleHistory
                            {
                                userGSTIN = dr.IsNull("userGSTIN") ? "" : dr["userGSTIN"].ToString(),
                                ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                                vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                                fromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                                fromState = dr.IsNull("FromState") ? "" : dr["FromState"].ToString(),
                                reasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                                reasonRemark = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                                transDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                                transDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                                transMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                                errorCode = dr.IsNull("UPD_errorCodes") ? "" : dr["UPD_errorCodes"].ToString(),
                                errorDesc = dr.IsNull("UPD_errorDescription") ? "" : dr["UPD_errorDescription"].ToString()

                            });

                        }

                        EwayViewModel model = new EwayViewModel();
                        model.VehicleHistory = VechicleList;

                        return View(model);
                    #endregion

                    #endregion

                    case "print":
                        #region
                        ewbNo = Form["ewbNo"];
                        string usergstin = strGSTINNo;
                        return RedirectToAction("EWB", "EWBPrint", new { ewbNo, usergstin });
                    #endregion

                    default:
                        #region "Authentication Checking"
                        strGSTINNo = Form["ddlGSTINNo"];
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion
                        break;
                }
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        #endregion

        #region "Consolidate Eway Bill Generation"
        public ActionResult CONSEWB()
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
                if (Session["transDocDate"] == null)
                {
                    Session["transDocDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                }
                #region "Eway Bill Dropdowns"
                if (Session["strGSTINNo"] != null)
                {

                    ViewBag.GstinNo = Session["strGSTINNo"].ToString();
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["strGSTINNo"].ToString(), Session["Role_Name"].ToString());
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(Session["strGSTINNo"].ToString());
                    if (Convert.ToInt32(Session["branchId"]) != 0)
                    {
                        ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(Session["branchId"]));
                    }
                    // ViewBag.StateCode = LoadGSTINNo.DropDownEwayBill("StateCode");
                    // ViewBag.TransportMode = LoadGSTINNo.DropDownEwayBill("TransportMode");
                }
                else
                {
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                if (Session["transMode"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transMode"].ToString());
                }
                else
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                }
                if (Session["statecode"] != null)
                {
                    ViewBag.StateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", Session["statecode"].ToString());
                }
                else
                {
                    ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                }
                #endregion

                if (Session["MasterdtlsCEWAY"] != null)
                {
                    DataTable dt1 = ((DataTable)Session["MasterdtlsCEWAY"]);
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transmode"].ToString());
                    ViewBag.StateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", Session["statecode"].ToString());
                    ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["strGSTINNo"].ToString(), Session["Role_Name"].ToString());
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(Session["strGSTINNo"].ToString());
                    ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(Session["branchId"]));
                    return View(ConvertToDictionary(dt1));
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();

        }

        [HttpPost]
        public ActionResult CONSEWB(FormCollection Form, string Command)
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
                int branchId = 0;
                string transMode, strGSTINNo, fromState, fromPlace, vehicleNo, transDocNo, transDocDate, ewbNo;
                strGSTINNo = Form["ddlGSTINNo"];
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                if (Form["branchId"] != null)
                {
                    branchId = Convert.ToInt32(Form["branchId"]);
                }
                ViewBag.GstinNo = strGSTINNo;
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                    if (branchId != 0)
                    {
                        ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), branchId);
                    }
                }
                Session["strGSTINNo"] = strGSTINNo;
                switch (Command)
                {
                    case "Add":
                        #region "Form Collection intialization to variables"
                        transMode = Form["transMode"];
                        fromState = Form["dispatchStateCode"];
                        Session["transmode"] = transMode;
                        Session["statecode"] = fromState;
                        Session["fromPlace"] = Form["fromPlace"];
                        Session["vehicleNo"] = Form["vehicleNo"];
                        Session["transDocNo"] = Form["transDocNo"];
                        Session["transDocDate"] = Form["transDocDate"];
                        Session["branchId"] = branchId;
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", transMode);
                        ViewBag.StateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", fromState);
                        #endregion

                        #region "Creating Datatable for Storing Line Item Details"

                        int count;
                        DataTable dt = new DataTable();

                        if (Session["SnoCEwb"] == null)
                        {
                            Session["SnoCEwb"] = 1;
                            count = (int)Session["SnoCEwb"];
                        }
                        else
                        {
                            count = (int)Session["SnoCEwb"];
                            count = count + 1;
                            Session["SnoCEwb"] = count;
                        }

                        if (Session["MasterdtlsCEWAY"] == null)
                        {

                            dt.Columns.Add("Srno", typeof(string));
                            dt.Columns.Add("EwbNo", typeof(string));
                            dt.Columns.Add("EwbDate", typeof(string));
                            dt.Columns.Add("GeneratedBy", typeof(string));
                            dt.Columns.Add("DocNo", typeof(string));
                            dt.Columns.Add("DocDate", typeof(string));
                            dt.Columns.Add("TaxVal", typeof(string));
                            dt.Columns.Add("FromPlace", typeof(string));
                            dt.Columns.Add("ToPlace", typeof(string));
                        }
                        else
                        {
                            dt = ((DataTable)Session["MasterdtlsCEWAY"]);
                        }
                        for (int i = 0; i < dt.Rows.Count; i++)
                        {
                            if (dt.Rows[i]["EwbNo"].ToString() == Form["ewbNo"].ToString())
                            {
                                TempData["ErrorMessage"] = "Eway Bill No Already Exist";
                                if (Session["MasterdtlsCEWAY"] != null)
                                {
                                    DataTable dt1 = ((DataTable)Session["MasterdtlsCEWAY"]);

                                    return View(ConvertToDictionary(dt1));
                                }
                            }
                        }
                        DataRow dr = dt.NewRow();

                        dr[0] = count;
                        dr[1] = Form["ewbNo"];
                        dr[2] = Form["ewbDate"];
                        dr[3] = Form["generatedBy"];
                        dr[4] = Form["docNo"];
                        dr[5] = Form["docDate"];
                        dr[6] = Form["taxVal"];
                        dr[7] = Form["fromPlaces"];
                        dr[8] = Form["toPlace"];
                        dt.Rows.Add(dr);
                        Session["MasterdtlsCEWAY"] = dt;

                        if (Session["MasterdtlsCEWAY"] != null)
                        {
                            DataTable dt1 = ((DataTable)Session["MasterdtlsCEWAY"]);

                            return View(ConvertToDictionary(dt1));
                        }
                        #endregion

                        break;
                    case "Save":
                        #region "Inserting FormCollection to Database"
                        int itemnum = 0;
                        transMode = Form["transMode"];
                        fromState = Form["dispatchStateCode"];
                        fromPlace = Form["fromPlace"];
                        vehicleNo = Form["vehicleNo"];
                        transDocNo = Form["transDocNo"];
                        transDocDate = Form["transDocDate"];
                        DataTable Dt = (DataTable)Session["MasterdtlsCEWAY"];
                        if (Dt.Select().Length > 1)
                        {
                            foreach (DataRow Dr in Dt.Rows)
                            {
                                itemnum = itemnum + 1;
                                ewbNo = Convert.ToString(Dr["EwbNo"]);
                                EwaybillDataAccess.cewb_insert(strGSTINNo, transMode, transDocNo, transDocDate, fromPlace, fromState, vehicleNo, ewbNo, Session["CustRefNo"].ToString(), Session["User_ID"].ToString(), branchId);

                            }
                            Session["fromPlace"] = null;
                            Session["vehicleNo"] = null;
                            Session["transDocNo"] = null;
                            Session["transDocDate"] = null;
                            Session["SnoCEwb"] = null;
                            Session["MasterdtlsCEWAY"] = null;
                            Session["branchId"] = null;

                            int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                            ViewBag.BranchList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

                            #region "API Calling for Generating Consolidated Eway Bill No"
                            string OStatus = "0";
                            string OResponse = "";
                            string cewbNo = "";

                            string strJsonData = EwbJsonDataModel.GetJsonCONSEWBGeneration(strGSTINNo, transDocNo, transDocDate);
                            strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                            EwbGeneration.CONS_EWB_GEN(transDocNo, transDocDate, strJsonData, strGSTINNo,
                                Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(),
                                out OStatus, out OResponse, out cewbNo);
                            // Session["MasterdtEWAY"] = null;
                            if (OStatus == "0")
                            {
                                TempData["ErrorMessage"] = OResponse;
                                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

                            }
                            else
                            {
                                return RedirectToAction("Index", "EWBPrint", new { cewbNo });
                            }
                            #endregion
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Please enter More than one Eway Bill No For Generating Consolidated Eway Bill No";
                            ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", transMode);
                            ViewBag.StateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", fromState);
                            if (Session["MasterdtlsCEWAY"] != null)
                            {
                                DataTable dt1 = ((DataTable)Session["MasterdtlsCEWAY"]);

                                return View(ConvertToDictionary(dt1));
                            }
                        }
                        #endregion
                        break;
                    default:
                        #region "Authentication Checking"
                        strGSTINNo = Form["ddlGSTINNo"];
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion
                        break;
                }

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();

        }

        public ActionResult CONSDelete(int id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                #region "Line Item delete of Consolidated Eway Bill"
                DataTable dt = ((DataTable)Session["MasterdtlsCEWAY"]);

                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == id)
                    {
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsCEWAY"] = null;
                }
                else
                {
                    Session["MasterdtlsCEWAY"] = dt;
                }
                TempData["EwayDeletedMessage"] = " Eway number Deleted Successfully";

                return RedirectToAction("CONSEWB");
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("CONSEWB");
            }

        }
        [HttpPost]
        public JsonResult ewbDetails(string Prefix, string Gstin)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            DataSet DS = EwaybillDataAccess.getEwbDetails(Prefix, Gstin, CustId);

            List<EwayAttributes.EwayDetailsAutoPopulate> ewbDetail = new List<EwayAttributes.EwayDetailsAutoPopulate>();

            #region "Customer Details for Auto Populate"

            foreach (DataRow dr in DS.Tables[0].Rows)
            {
                ewbDetail.Add(new EwayAttributes.EwayDetailsAutoPopulate
                {
                    ewbNo = dr["ewayBillNo"].ToString(),
                    ewbDate = dr["ewayBillDate"].ToString(),
                    generatedBy = dr["fromGstin"].ToString(),
                    docNo = dr["docNo"].ToString(),
                    docDate = dr["docDate"].ToString(),
                    taxValue = dr["totalValue"].ToString(),
                    fromPlace = dr["fromPlace"].ToString(),
                    toPlace = dr["toPlace"].ToString()
                });

            }

            #endregion

            return Json(ewbDetail, JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region "Regenerate Consolidated Eway Bill"

        public ActionResult REGCONSEWB(string cEwbNo)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                string vehicleNo = "", fromPlace = "", stateCode = "", transDocNo = "", transDocDate = "", transMode = "", userGstin = "";
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);

                #region "Getting Consolidate values Based on cEWbNo 
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = sqlcon;
                        sqlcmd.CommandText = "select TOP(1) * from TBL_EWB_GEN_CONSOLIDATED where cEwbNo = @cEwbNo";
                        sqlcmd.Parameters.AddWithValue("@cEwbNo", cEwbNo);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                vehicleNo = dt.Rows[0]["vehicleNo"].ToString();
                                fromPlace = dt.Rows[0]["fromPlace"].ToString();
                                stateCode = dt.Rows[0]["fromState"].ToString();
                                transDocNo = dt.Rows[0]["transDocNo"].ToString();
                                transDocDate = dt.Rows[0]["transDocDate"].ToString();
                                transMode = dt.Rows[0]["transMode"].ToString();
                                userGstin = dt.Rows[0]["userGSTIN"].ToString();
                            }
                        }
                    }
                }
                #endregion

                #region "Storing DB Values to Viewbag"
                ViewBag.cEwbNo = cEwbNo;
                ViewBag.vehicleNo = vehicleNo;
                ViewBag.fromPlace = fromPlace;
                ViewBag.stateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", stateCode);
                ViewBag.transDocNo = transDocNo;
                ViewBag.transDocdate = transDocDate;
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", transMode);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, userGstin, Session["Role_Name"].ToString());
                ViewBag.ReasonList = Models.Common.LoadDropDowns.GetEWBReasonList();
                #endregion

                #region "Authentication Checking"
                string strGSTINNo = userGstin;
                EwbGeneratingKeys.Autentication(strGSTINNo);
                #endregion

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();

        }

        [HttpPost]
        public ActionResult REGCONSEWB(FormCollection Form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            try
            {
                string vehicleNo = "", fromPlace = "", stateCode = "", transDocNo = "", transDocDate = "", transMode = "", userGstin = "", tripSheetNo = "", rsnCode = "", rsnRmrk = "";
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                string OStatus = "0";
                string OResponse = "";
                if (command != null)
                {
                    tripSheetNo = Form["cEwbNo"];
                    fromPlace = Form["fromplace"];
                    vehicleNo = Form["vehicleNo"];
                    stateCode = Form["dispatchStateCode"];
                    transDocNo = Form["transdocNo"];
                    transDocDate = Form["transdocDate"];
                    transMode = Form["transMode"];
                    rsnCode = Form["reasonCode"];
                    rsnRmrk = Form["reasonRmrk"];
                    userGstin = Form["ddlGSTINNo"];

                    string strJsonData = "{\"tripSheetNo\": " + Convert.ToInt64(tripSheetNo) + ",\"vehicleNo\":\"" + vehicleNo + "\",\"fromPlace\":\"" + fromPlace + "\",\"fromState\": " + Convert.ToInt32(stateCode) + ",\"transDocNo\":\"" + transDocNo + "\",\"transDocDate\":\"" + transDocDate + "\",\"transMode\":\"" + transMode + "\",\"reasonCode\": " + Convert.ToInt32(rsnCode) + ",\"reasonRem\":\"" + rsnRmrk + "\"}";
                    EwbGeneration.EWB_REGENERATE_CONS(userGstin, strJsonData, tripSheetNo, fromPlace, stateCode, transMode, transDocNo, transDocDate, vehicleNo, Session["User_ID"].ToString(),
                            Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OStatus, out OResponse);
                    if (OStatus == "0")
                    {
                        TempData["ErrorMessage"] = OResponse;
                    }
                    else
                    {
                        TempData["SuccessMessage"] = OResponse;
                    }
                    return RedirectToAction("CONSEWB", "EwayBill");
                    // return RedirectToAction("REGCONSEWB", new RouteValueDictionary(
                    //                     new { controller = "EWBGeneration", action = "REGCONSEWB",FormMethod="Post", cEwbNo = tripSheetNo }));

                }

                #region "Authentication Checking"
                string strGSTINNo = userGstin;
                EwbGeneratingKeys.Autentication(strGSTINNo);
                #endregion

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();

        }

        #endregion

        #region "extend Validity of Eway Bill"
        public ActionResult Validity()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            try
            {
                if (Session["transDocDate"] == null)
                {
                    Session["transDocDate"] = DateTime.Now.ToString("dd/MM/yyyy");
                }

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ExtendReasonList = Models.Common.LoadDropDowns.GetEWBExtendReasonList();

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }


            #region "Assing data to Model"
            DataSet Ds = BusinessLayer.Retrieve_Extend_Validty_History(CustId);
            List<ReportAttributes.ExtendValidity> ValidityMgmt = new List<ReportAttributes.ExtendValidity>();
            #region "Data Assign to Attributes"
            foreach (DataRow dr in Ds.Tables[0].Rows)
            {
                ValidityMgmt.Add(new ReportAttributes.ExtendValidity
                {
                    userGstin = dr.IsNull("UserGstin") ? "" : dr["EwbNo"].ToString(),
                    ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    vehicleNo = dr.IsNull("VehicleNo") ? "" : dr["VehicleNo"].ToString(),
                    fromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                    fromStateCode = dr.IsNull("FromStateCode") ? "" : dr["FromStateCode"].ToString(),
                    reaminingDist = dr.IsNull("RemainingDistance") ? "" : dr["RemainingDistance"].ToString(),
                    transDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                    transDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                    transMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                    extnRmrk = dr.IsNull("ExtnRsnCode") ? "" : dr["ExtnRsnCode"].ToString(),
                    extnRsnCode = dr.IsNull("ExtnRmrk") ? "" : dr["ExtnRmrk"].ToString(),
                    errorCode = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString(),
                    errorDesc = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString()

                });
            }

            ReportViewModel Model = new ReportViewModel();
            Model.ValidityMgmt = ValidityMgmt;
            #endregion

            #endregion

            return View(Model);
        }

        [HttpPost]
        public ActionResult Validity(FormCollection Form, string Command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string OStatus = "0";
            string OResponse = "";
            try
            {
                string strGSTINNo, ewbNo, vehicleNo, fromPlace, fromStateCode, remainingDistance, transMode, transdocNo, transdocDate, extRsnCode, extRsnRmrk;
                strGSTINNo = Form["ddlGSTINNo"];
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ExtendReasonList = Models.Common.LoadDropDowns.GetEWBExtendReasonList();

                switch (Command)
                {

                    case "extendValidity":
                        ewbNo = Form["ewbNo"];
                        ViewBag.ewbNo = ewbNo;
                        vehicleNo = Form["vehicleNo"];
                        fromPlace = Form["fromplace"];
                        fromStateCode = Form["dispatchStateCode"];
                        remainingDistance = Form["remainingDistance"];
                        transMode = Form["transMode"];
                        transdocNo = Form["transdocNo"];
                        transdocDate = Form["transdocDate"];
                        extRsnCode = Form["reasonCode"];
                        extRsnRmrk = Form["reasonRmrk"];


                        string strJsonData = "{\"ewbNo\":" + ewbNo + ",\"vehicleNo\":\"" + vehicleNo + "\",\"fromPlace\":\"" + fromPlace + "\",\"fromStateCode\": " + Convert.ToInt32(fromStateCode) + ",\"remainingDistance\": " + Convert.ToInt32(remainingDistance) + ",\"transDocNo\":\"" + transdocNo + "\",\"transDocDate\":\"" + transdocDate + "\",\"transMode\":\"" + transMode + "\",\"extnRsnCode\": " + Convert.ToInt32(extRsnCode) + ",\"extnRemarks\":\"" + extRsnRmrk + "\"}";
                        new EwbCancelReject(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).EWB_EXTEND_VALIDITY(strGSTINNo, ewbNo, vehicleNo, fromPlace, Convert.ToInt32(fromStateCode), remainingDistance, transdocNo, transdocDate,
                            Convert.ToInt32(transMode), extRsnCode, extRsnRmrk, strJsonData, out OStatus, out OResponse);
                        if (OStatus == "0")
                        {
                            TempData["ErrorMessage"] = OResponse;
                        }
                        else
                        {
                            TempData["SuccessMessage"] = OResponse;
                        }
                        break;
                    case "print":
                        #region                      
                        string ActionData = "ExtendValidity";
                        ewbNo = Form["ewbNo"];
                        string usergstin = strGSTINNo;
                        return RedirectToAction("EWB", "EWBPrint", new { ewbNo, usergstin, ActionData });
                    #endregion
                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion
                        break;
                }

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            #region "Assing data to Model"
            DataSet Ds = BusinessLayer.Retrieve_Extend_Validty_History(CustId);
            List<ReportAttributes.ExtendValidity> ValidityMgmt = new List<ReportAttributes.ExtendValidity>();

            #region "Data Assign to Attributes"
            foreach (DataRow dr in Ds.Tables[0].Rows)
            {
                ValidityMgmt.Add(new ReportAttributes.ExtendValidity
                {
                    userGstin = dr.IsNull("UserGstin") ? "" : dr["EwbNo"].ToString(),
                    ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    vehicleNo = dr.IsNull("VehicleNo") ? "" : dr["VehicleNo"].ToString(),
                    fromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                    fromStateCode = dr.IsNull("FromStateCode") ? "" : dr["FromStateCode"].ToString(),
                    reaminingDist = dr.IsNull("RemainingDistance") ? "" : dr["RemainingDistance"].ToString(),
                    transDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                    transDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                    transMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                    extnRmrk = dr.IsNull("ExtnRsnCode") ? "" : dr["ExtnRsnCode"].ToString(),
                    extnRsnCode = dr.IsNull("ExtnRmrk") ? "" : dr["ExtnRmrk"].ToString(),
                    errorCode = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString(),
                    errorDesc = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString()

                });
            }

            ReportViewModel Model = new ReportViewModel();
            Model.ValidityMgmt = ValidityMgmt;
            #endregion
            #endregion

            return View();
        }
        #endregion

        #region "Update Transporter"
        public ActionResult UPDTransPorter()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            try
            {
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            #region "Assing data to Model"
            DataSet Ds = BusinessLayer.Retrieve_Update_TransPorter_History(CustId);
            List<ReportAttributes.UpdateTransporter> TransMgmt = new List<ReportAttributes.UpdateTransporter>();
            #region "Data Assign to Attributes"
            foreach (DataRow dr in Ds.Tables[0].Rows)
            {
                TransMgmt.Add(new ReportAttributes.UpdateTransporter
                {
                    userGstin = dr.IsNull("UserGstin") ? "" : dr["UserGstin"].ToString(),
                    ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    transId = dr.IsNull("TransId") ? "" : dr["TransId"].ToString(),
                    updTransDate = dr.IsNull("UpdTransDate") ? "" : dr["UpdTransDate"].ToString(),
                    errorCode = dr.IsNull("ErrorCode") ? "" : dr["ErrorCode"].ToString(),
                    errorDesc = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString()
                });
            }

            ReportViewModel Model = new ReportViewModel();
            Model.updTransMgmt = TransMgmt;
            #endregion
            #endregion

            return View(Model);
        }

        [HttpPost]
        public ActionResult UPDTransPorter(FormCollection Form, string Command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);
            string OStatus = "";
            string OResponse = "";
            try
            {
                string strGSTINNo, ewbNo, transname, transId;
                strGSTINNo = Form["ddlGSTINNo"];
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(UserId, CustId, strGSTINNo, Session["Role_Name"].ToString());

                switch (Command)
                {
                    case "updTransporter":
                        ewbNo = Form["ewbNo"];
                        ViewBag.ewbno = ewbNo;
                        transname = Form["transName"];
                        transId = Form["transId"];

                        string strJsonData = "{\"ewbNo\":" + ewbNo + ",\"transporterId\":\"" + transId + "\"}";
                        new EwbCancelReject(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).EWB_UPDATE_TRANSPORTER(strGSTINNo, ewbNo, transId, transname, strJsonData, out OStatus, out OResponse);
                        Session["ewaybill"] = ewbNo;
                        Session["transporterid"] = transId;
                        if (OStatus == "0")
                        {
                            TempData["ErrorMessage"] = OResponse;
                        }
                        else
                        {
                            TempData["SuccessMessage"] = OResponse;
                        }
                        break;
                    case "print":
                        #region
                        string ActionData = "UpdateTransporter";
                        ewbNo = Form["ewbNo"];
                        string usergstin = strGSTINNo;
                        return RedirectToAction("EWB", "EWBPrint", new { ewbNo, usergstin, ActionData });
                    #endregion
                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion
                        break;
                }

            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            #region "Assing data to Model"
            DataSet Ds = BusinessLayer.Retrieve_Update_TransPorter_History(CustId);
            List<ReportAttributes.UpdateTransporter> TransMgmt = new List<ReportAttributes.UpdateTransporter>();

            #region "Data Assign to Attributes"
            foreach (DataRow dr in Ds.Tables[0].Rows)
            {
                TransMgmt.Add(new ReportAttributes.UpdateTransporter
                {
                    userGstin = dr.IsNull("UserGstin") ? "" : dr["UserGstin"].ToString(),
                    ewbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    transId = dr.IsNull("TransId") ? "" : dr["TransId"].ToString(),
                    updTransDate = dr.IsNull("UpdTransDate") ? "" : dr["UpdTransDate"].ToString(),
                    errorCode = dr.IsNull("ErrorCode") ? "" : dr["ErrorCode"].ToString(),
                    errorDesc = dr.IsNull("ErrorDesc") ? "" : dr["ErrorDesc"].ToString()
                });
            }

            ReportViewModel Model = new ReportViewModel();
            Model.updTransMgmt = TransMgmt;
            #endregion
            #endregion
            return View(Model);
        }
        #endregion

        #region "Coverting Dictionary"
        private object ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();

            return dictionaryList.ToList<IDictionary>();
        }
        #endregion

        private void SendPDFEmail1(string ewbNo, string gstin, string Email, string userEmail)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            using (StringWriter sw = new StringWriter())
            {
                using (HtmlTextWriter hw = new HtmlTextWriter(sw))
                {
                    DataSet ds1 = EwaybillDataAccess.getEWAY(ewbNo, gstin, "");

                    var edt = ds1.Tables[0].Rows[0]["ewayBillDate"].ToString();
                    var genby = ds1.Tables[0].Rows[0]["fromTrdName"].ToString();
                    var validuntil = ds1.Tables[0].Rows[0]["validUpto"].ToString();
                    var supp_gstin = ds1.Tables[0].Rows[0]["fromGstin"].ToString();
                    var frmadd1 = ds1.Tables[0].Rows[0]["fromAddr1"].ToString();
                    var frmadd2 = ds1.Tables[0].Rows[0]["fromAddr2"].ToString();
                    var dispatch_place = ds1.Tables[0].Rows[0]["fromPlace"].ToString();
                    var frmpincd = ds1.Tables[0].Rows[0]["fromPinCode"].ToString();
                    var rec_gstin = ds1.Tables[0].Rows[0]["toGstin"].ToString();
                    var toName = ds1.Tables[0].Rows[0]["toTrdName"].ToString();
                    var toadd1 = ds1.Tables[0].Rows[0]["toAddr1"].ToString();
                    var toadd2 = ds1.Tables[0].Rows[0]["toAddr2"].ToString();
                    var topincd = ds1.Tables[0].Rows[0]["toPincode"].ToString();
                    var pod = ds1.Tables[0].Rows[0]["toPlace"].ToString();
                    var docno = ds1.Tables[0].Rows[0]["docNo"].ToString();
                    var docdt = ds1.Tables[0].Rows[0]["docDate"].ToString();
                    var totval = ds1.Tables[0].Rows[0]["totalValue"].ToString();
                    var igst = ds1.Tables[0].Rows[0]["igstValue"].ToString();
                    var cgst = ds1.Tables[0].Rows[0]["cgstValue"].ToString();
                    var sgst = ds1.Tables[0].Rows[0]["sgstValue"].ToString();
                    var cess = ds1.Tables[0].Rows[0]["cessValue"].ToString();
                    var supplytyp = ds1.Tables[0].Rows[0]["supplyType"].ToString();

                    #region "Supplytype and SubSupplyType Validations"
                    if (supplytyp == "O")
                    {
                        supplytyp = "Outward";
                    }
                    else
                    {
                        supplytyp = "Inward";
                    }

                    var subsuptyp = ds1.Tables[0].Rows[0]["subSupplyType"].ToString();

                    switch (subsuptyp)
                    {
                        case "1":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Supply";
                            }
                            break;
                        case "2":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Line Sales";
                            }
                            break;
                        case "3":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Recipient Not Known";
                            }
                            break;
                        case "4":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Exhibition or Fares";
                            }
                            break;
                        case "5":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Export";
                            }
                            break;
                        case "6":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Job Work";
                            }
                            break;
                        case "7":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "For Own Use";
                            }
                            break;
                        case "8":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "Others";
                            }
                            break;
                        case "9":
                            if (supplytyp == "Outward")
                            {
                                subsuptyp = "SKD/CKD";
                            }
                            break;
                    }
                    #endregion

                    var trnsprtr = ds1.Tables[0].Rows[0]["transporterId"].ToString();
                    var trnsprtname = ds1.Tables[0].Rows[0]["transporterName"].ToString();

                    decimal val = Convert.ToDecimal(totval) + Convert.ToDecimal(igst) + Convert.ToDecimal(cgst) + Convert.ToDecimal(sgst) + Convert.ToDecimal(cess);
                    var valofgoods = Convert.ToString(val);

                    int ewbId = EwaybillDataAccess.getEwbId(ewbNo, CustId);
                    int hsnCode = EwaybillDataAccess.getHSNDetails(ewbId, CustId);
                    int hsncount = EwaybillDataAccess.getHSNCount(ewbId, CustId);

                    Document pdfDoc = new Document(PageSize.A4, 10f, 10f, 10f, 0f);

                    using (MemoryStream memoryStream = new MemoryStream())
                    {
                        PdfWriter writer = PdfWriter.GetInstance(pdfDoc, memoryStream);
                        pdfDoc.Open();
                        string strHTML = @"";

                        PdfPTable table = new PdfPTable(3);
                        table.DefaultCell.Border = Rectangle.NO_BORDER;
                        table.TotalWidth = 550f;
                        table.SpacingBefore = 10f;
                        table.SpacingAfter = 12.5f;
                        table.AddCell("");
                        table.AddCell("E-Way Bill");
                        table.AddCell("");
                        pdfDoc.Add(table);

                        PdfPTable table1 = new PdfPTable(3);
                        table1.DefaultCell.Border = Rectangle.NO_BORDER;
                        table1.TotalWidth = 550f;
                        table1.SpacingBefore = 10f;
                        table1.SpacingAfter = 12.5f;


                        PdfPCell pdfWordCell1 = new PdfPCell();
                        Phrase fromAdd = new Phrase("E-Way Bill No");


                        PdfPCell pdfWordCell2 = new PdfPCell();
                        Phrase toAdd = new Phrase(":");

                        PdfPCell pdfWordCell3 = new PdfPCell();
                        Phrase tooAdd = new Phrase(ewbNo);

                        table1.AddCell(fromAdd);
                        table1.AddCell(toAdd);
                        table1.AddCell(tooAdd);
                        table1.AddCell("E-Way Bill Date");
                        table1.AddCell(":");
                        table1.AddCell(edt);
                        table1.AddCell("Generated By");
                        table1.AddCell(":");
                        table1.AddCell(genby);
                        table1.AddCell("Valid From");
                        table1.AddCell(":");
                        table1.AddCell(edt);
                        table1.AddCell("Valid Until");
                        table1.AddCell(":");
                        table1.AddCell(validuntil);
                        pdfDoc.Add(table1);

                        PdfPTable table2 = new PdfPTable(3);
                        table2.DefaultCell.Border = Rectangle.NO_BORDER;
                        table2.TotalWidth = 550f;
                        table2.SpacingBefore = 10f;
                        table2.SpacingAfter = 12.5f;

                        table2.AddCell("");
                        table2.AddCell("PART-A");
                        table2.AddCell("");
                        table2.AddCell("GSTIN Of Supplier");
                        table2.AddCell(":");
                        table2.AddCell(supp_gstin + " " + genby);
                        table2.AddCell("Place Of Dispatch");
                        table2.AddCell(":");
                        table2.AddCell(frmadd1 + " " + frmadd2 + " " + dispatch_place + " " + frmpincd);
                        table2.AddCell("GSTIN Of Recipient");
                        table2.AddCell(":");
                        table2.AddCell(rec_gstin + " " + toName);
                        table2.AddCell("Place Of Delivery");
                        table2.AddCell(":");
                        table2.AddCell(toadd1 + " " + toadd2 + " " + pod + " " + topincd);
                        table2.AddCell("Document No.");
                        table2.AddCell(":");
                        table2.AddCell(docno);
                        table2.AddCell("Document Date");
                        table2.AddCell(":");
                        table2.AddCell(docdt);
                        table2.AddCell("Value Of Goods");
                        table2.AddCell(":");
                        table2.AddCell(valofgoods);
                        table2.AddCell("HSN Code");
                        table2.AddCell(":");
                        table2.AddCell(hsnCode + "-" + hsncount);
                        table2.AddCell("Reason for Transportation");
                        table2.AddCell(":");
                        table2.AddCell(supplytyp + "-" + subsuptyp);
                        table2.AddCell("Transporter");
                        table2.AddCell(":");
                        table2.AddCell(trnsprtr + " " + trnsprtname);
                        pdfDoc.Add(table2);

                        PdfPTable table4 = new PdfPTable(3);
                        table4.DefaultCell.Border = Rectangle.NO_BORDER;
                        table4.TotalWidth = 550f;
                        table4.SpacingBefore = 10f;
                        table4.SpacingAfter = 12.5f;

                        table4.AddCell("");
                        table4.AddCell("PART-B");
                        table4.AddCell("");
                        pdfDoc.Add(table4);

                        DataSet d2 = EwaybillDataAccess.getEWAYList(ewbNo);
                        var tab = d2.Tables[0];
                        if (tab.Rows.Count > 0)
                        {
                            var transmode = d2.Tables[0].Rows[0]["TransMode"].ToString();

                            #region "TransMode Validations"
                            switch (transmode)
                            {
                                case "1":
                                    transmode = "Road";
                                    break;
                                case "2":
                                    transmode = "Rail";
                                    break;
                                case "3":
                                    transmode = "Air";
                                    break;
                                case "4":
                                    transmode = "Ship";
                                    break;
                            }
                            #endregion

                            var vehclno = d2.Tables[0].Rows[0]["vehicleNo"].ToString();
                            var transdocno = d2.Tables[0].Rows[0]["TransDocNo"].ToString();
                            var transdocdt = d2.Tables[0].Rows[0]["transDocDate"].ToString();

                            PdfPTable table5 = new PdfPTable(5);
                            table5.TotalWidth = 550f;
                            table5.SpacingBefore = 10f;
                            table5.SpacingAfter = 12.5f;

                            PdfPCell pdfWordCell4 = new PdfPCell();
                            Phrase mode = new Phrase("Mode");
                            pdfWordCell4.Border = Rectangle.NO_BORDER;

                            PdfPCell pdfWordCell5 = new PdfPCell();
                            Phrase vehicleno = new Phrase("Vehicle No/Trans Doc No.& Dt.");
                            pdfWordCell5.Border = Rectangle.NO_BORDER;

                            PdfPCell pdfWordCell6 = new PdfPCell();
                            Phrase from = new Phrase("From");
                            pdfWordCell6.Border = Rectangle.NO_BORDER;

                            PdfPCell pdfWordCell7 = new PdfPCell();
                            Phrase Entereddt = new Phrase("Entered Date");
                            pdfWordCell7.Border = Rectangle.NO_BORDER;

                            PdfPCell pdfWordCell8 = new PdfPCell();
                            Phrase Enteredby = new Phrase("Entered By");
                            pdfWordCell8.Border = Rectangle.NO_BORDER;

                            table5.AddCell(mode);
                            table5.AddCell(vehicleno);
                            table5.AddCell(from);
                            table5.AddCell(Entereddt);
                            table5.AddCell(Enteredby);

                            table5.AddCell(transmode);
                            table5.AddCell(vehclno + " " + transdocno + " " + transdocdt);
                            table5.AddCell(dispatch_place);
                            table5.AddCell(docdt);
                            table5.AddCell(genby);
                            pdfDoc.Add(table5);
                        }
                        StringReader sr = new StringReader(strHTML.ToString());
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, pdfDoc, sr);

                        pdfDoc.Close();
                        byte[] bytes = memoryStream.ToArray();
                        memoryStream.Close();

                        string BodyMsg = string.Empty;
                        BodyMsg = "<html xmlns='http://www.w3.org/1999/xhtml'><body><div><strong>Dear Customer,<br />";

                        BodyMsg += "</strong></div><div style='margin-left: 40px'><br />Please find the attached e-Way Bill</div>";
                        BodyMsg += " <div style='margin-left: 40px'>Generated By : " + userEmail + "</div>";
                        BodyMsg += "<div><br /><br /><br /><strong><span style='color: #000000' > Thanks & Regards,</span><br /> Wep Digital Services</ span ><br/> ";
                        BodyMsg += "&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp;&nbsp; <img src='https://www.wepgst.com/Content/Images/icon-wep-logo.gif'></span></strong><span style='color: #CC0000'><br />Cashless, Paperless,<br />Omnipresent Solutions</span></div></body></html>";

                        MailMessage mm = new MailMessage("noreply@wepdigital.com", Email.Replace(";", ","));
                        mm.Subject = "Eway Bill Details";
                        mm.Body = BodyMsg;
                        mm.Attachments.Add(new Attachment(new MemoryStream(bytes), "EWBNO-" + ewbNo + ".pdf"));
                        mm.IsBodyHtml = true;
                        SmtpClient smtp = new SmtpClient();
                        smtp.Host = "smtp.office365.com";
                        smtp.EnableSsl = true;
                        NetworkCredential NetworkCred = new NetworkCredential();
                        NetworkCred.UserName = "noreply@wepdigital.com";
                        NetworkCred.Password = "wep@12345";
                        smtp.UseDefaultCredentials = true;
                        smtp.Credentials = NetworkCred;
                        smtp.Port = 587;
                        smtp.Send(mm);
                    }
                }

            }
        }
    }
}