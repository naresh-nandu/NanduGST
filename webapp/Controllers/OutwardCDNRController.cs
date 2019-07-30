using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.EditInvoice;
using SmartAdminMvc.Models.Inward;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Linq;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardCDNRController : Controller
    {
        // GET: OutwardCDNR
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
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

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward CDNR Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                var buyname = db.TBL_Buyer.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { buyername = c.BuyerName, buyerid = c.BuyerId }).OrderBy(c => c.buyername).ToList();
                ViewBag.buyname = buyname;
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");

                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");


                if (Session["MasterdtlsOCDNR"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsOCDNR"]);
                    if (Session["transModecdnr"] != null)
                    {
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModecdnr"].ToString());
                    }
                    if (Session["serviceTypecdnr"] != null)
                    {
                        ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypecdnr"].ToString());
                    }
                    return View(ConvertToDictionary(dt));
                }
                else
                {
                    return View();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Home(FormCollection form, string command)
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
                if (command == "add")
                {

                    decimal invoicevalue, invalue;
                    string csval;

                    Session["name"] = form["name"];
                    Session["gstin"] = form["gstin"];
                    Session["pos"] = form["pos"];
                    Session["address"] = form["address"];
                    Session["cgstin"] = form["cgstin"];
                    Session["notetype"] = form["notetype"];
                    Session["isauto"] = form["isauto"];
                    Session["notenum"] = form["notenum"];
                    Session["notedate"] = form["notedate"];
                    Session["rns"] = form["rns"];
                    Session["pgst"] = form["pgst"];
                    string PreGST = form["pgst"];

                    Session["serviceTypecdnr"] = form["ddlserviceType"];
                    Session["transModecdnr"] = form["transMode"];
                    Session["vehicleNocdnr"] = form["vehicleNo"];
                    Session["dateOfSupplycdnr"] = form["dateOfSupply"];
                    Session["cinNocdnr"] = form["cinNo"];
                    if (Session["transModecdnr"] != null)
                    {
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModecdnr"].ToString());
                    }
                    if (Session["serviceTypecdnr"] != null)
                    {
                        ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypecdnr"].ToString());
                    }

                    if (PreGST == "N")
                    {
                        string Invno = form["invoice"];
                        string Invdate = form["invoicedate"];
                        string Refno = Convert.ToString(Session["CustRefNo"]);
                        string ctin = form["gstin"];
                        var invoicenumber = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == Invno && li.idt == Invdate && li.ctin == ctin && li.referenceno == Refno select li).ToList();
                        if (invoicenumber.Count > 0)
                        {
                            Session["invoice"] = form["invoice"];
                            Session["invoicedate"] = form["invoicedate"];

                        }
                        else
                        {
                            TempData["Message"] = "Invalid Invoice Number";
                            return RedirectToAction("Home");
                        }

                    }
                    else
                    {
                        Session["invoice"] = form["invoice"];
                        Session["invoicedate"] = form["invoicedate"];
                    }

                    Session["cfs"] = form["cfs"];
                    Session["comment1"] = form["comment1"];

                    if (Session["MasterdtlsOCDNR"] == null)
                    {
                        Session["discountreqocdnr"] = form["isDiscountreq"];
                    }

                    int count;
                    DataTable dt = new DataTable();


                    if (Session["Osrno"] == null)
                    {
                        Session["Osrno"] = 1;
                        count = (int)Session["Osrno"];
                    }
                    else
                    {
                        count = (int)Session["Osrno"];
                        count = count + 1;
                        Session["Osrno"] = count;
                    }

                    if (Session["MasterdtlsOCDNR"] == null)
                    {

                        dt.Columns.Add("Srno", typeof(int));
                        dt.Columns.Add("HSN", typeof(string));
                        dt.Columns.Add("HSNDesc", typeof(string));
                        dt.Columns.Add("Quantity", typeof(string));
                        dt.Columns.Add("Taxablevalue", typeof(string));
                        dt.Columns.Add("Rate", typeof(string));
                        dt.Columns.Add("IGST Amount", typeof(string));
                        dt.Columns.Add("CGST Amount", typeof(string));
                        dt.Columns.Add("SGST Amount", typeof(string));
                        dt.Columns.Add("CESS Amount", typeof(string));
                        dt.Columns.Add("Unit Price", typeof(string));
                        dt.Columns.Add("Discount", typeof(string));
                        dt.Columns.Add("UQC", typeof(string));
                        dt.Columns.Add("ItemDesc", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterdtlsOCDNR"]);
                    }

                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = form["HSN"];
                    dr[2] = form["HSNDesc"];
                    dr[3] = form["qty"];
                    dr[4] = form["taxablevalue"];
                    dr[5] = form["rate"];
                    dr[6] = form["iamount"];
                    dr[7] = form["camount"];
                    dr[8] = form["samount"];
                    dr["ItemDesc"] = form["ItemDesc"];
                    if (form["csamount"] == "")
                    {
                        dr[9] = 0;
                        csval = "0";
                    }
                    else
                    {
                        dr[9] = form["csamount"];
                        csval = form["csamount"];
                    }
                    dr[10] = form["unitprice"];
                    dr[11] = form["discount"];
                    dr["UQC"] = form["uqc"];



                    dt.Rows.Add(dr);
                    Session["MasterdtlsOCDNR"] = dt;
                    //
                    if (Session["invaluecdnr"] == null)
                    {
                        invalue = 0;
                    }
                    else
                    {
                        invalue = Convert.ToDecimal(form["invalue"]);
                    }
                    string GSTPre = Convert.ToString(Session["pgst"]);
                    if (GSTPre == "N")
                    {
                        string ino = form["invoice"];
                        string idt = form["invoicedate"];
                        string rno = Convert.ToString(Session["CustRefNo"]);
                        var invvalue = (from li in db.TBL_EXT_GSTR1_B2B_INV where li.inum == ino && li.idt == idt && li.referenceno == rno select li.val).FirstOrDefault();
                        invoicevalue = Convert.ToDecimal(invvalue) + Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(form["camount"]) + Convert.ToDecimal(form["samount"]) + Convert.ToDecimal(csval);
                        Session["invaluecdnr"] = invoicevalue;

                    }
                    else
                    {
                        invoicevalue = Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(form["camount"]) + Convert.ToDecimal(form["samount"]) + Convert.ToDecimal(csval) + invalue;
                        Session["invaluecdnr"] = invoicevalue;
                    }

                    return RedirectToAction("Home");

                }


                else if (command == "save1" || command == "save2")
                {

                    string hsn, pregst, gstin, cgstin, rns, invoice, invoicedate, notetype, notenum, notedate, refno, fp, pos, hsndesc, cfs, uqc, comment1, details;
                    decimal rate, qty, igsta, cgsta, sgsta, cessa, taxvalue, invalue, unitprice, discount;
                    int custid, userid, buyerid, itemnum = 0;
                    string serviceTpe = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";

                    serviceTpe = form["ddlserviceType"];
                    transMode = form["transMode"];
                    vechicleNo = form["vehicleNo"];
                    dateOfSupply = form["dateOfSupply"];
                    cinNo = form["cinNo"];

                    custid = Convert.ToInt32(Session["Cust_ID"]);
                    userid = Convert.ToInt32(Session["User_Id"]);
                    gstin = form["gstin"];
                    buyerid = Convert.ToInt32(form["name"]);
                    cgstin = form["cgstin"];
                    rns = form["rns"];
                    pos = form["pos"].ToString().Length > 1 ? form["pos"].ToString() : "0" + form["pos"].ToString();
                    cfs = form["cfs"];
                    pregst = form["pgst"];

                    invoice = form["invoice"];

                    invoicedate = form["invoicedate"];
                    notetype = form["notetype"];
                    notenum = form["notenum"];
                    notedate = form["notedate"];
                    if (notenum == "-")
                    {
                        notenum = "NA";
                    }
                    else
                    {
                        string Mode = "O";
                        int Res = InwardFunction.InvoiceValidation(cgstin, notenum, notedate, Mode);
                        if (Res == 1)
                        {
                            TempData["Message"] = "Invoice number " + notenum + " already exist";
                            Session["notenum"] = null;
                            return RedirectToAction("Home");
                        }

                    }
                    invalue = Convert.ToDecimal(form["invalue"]);
                    fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                    refno = Session["CustRefNo"].ToString();

                    comment1 = form["comment1"];
                    details = comment1;
                    DataTable dt = (DataTable)Session["MasterdtlsOCDNR"];
                    ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                    foreach (DataRow dr in dt.Rows)
                    {
                        itemnum = itemnum + 1;
                        hsn = Convert.ToString(dr["HSN"]);
                        hsndesc = Convert.ToString(dr["ItemDesc"]);
                        qty = Convert.ToDecimal(dr["Quantity"]);
                        rate = Convert.ToDecimal(dr["rate"]);
                        taxvalue = Convert.ToDecimal(dr["Taxablevalue"]);
                        igsta = Convert.ToDecimal(dr["IGST Amount"]);
                        cgsta = Convert.ToDecimal(dr["CGST Amount"]);
                        sgsta = Convert.ToDecimal(dr["SGST Amount"]);
                        cessa = Convert.ToDecimal(dr["CESS Amount"]);
                        unitprice = Convert.ToDecimal(dr["Unit Price"]);
                        discount = Convert.ToDecimal(dr["Discount"]);
                        uqc = Convert.ToString(dr["UQC"]);

                        db.usp_Insert_Outward_GSTR1_CDNR_EXT1(cgstin, fp, gstin, cfs, notetype, notenum, notedate, invoice, invoicedate, invalue, pos, rate, taxvalue, igsta, cgsta, sgsta, cessa, refno, hsn, hsndesc, qty, unitprice, discount, uqc, buyerid, userid, details, pregst, rns, serviceTpe, transMode, vechicleNo, dateOfSupply, cinNo, op);
                        if (op.Value == null)
                        {
                            notenum = "NA";
                        }
                        else
                        {
                            notenum = op.Value.ToString();
                        }

                    }
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                    {
                        //
                    }
                    else
                    {
                        B2CsFunctions.CDNRPush(refno, cgstin, custid, userid);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward CDNR invoice generated: " + notenum, "");

                    Session["name"] = null;
                    Session["gstin"] = null;
                    Session["pos"] = null;
                    Session["address"] = null;
                    Session["cgstin"] = null;
                    Session["notetype"] = null;
                    Session["isauto"] = null;
                    Session["notenum"] = null;
                    Session["notedate"] = null;
                    Session["rns"] = null;
                    Session["pgst"] = null;
                    Session["invoice"] = null;
                    Session["invoicedate"] = null;
                    Session["invaluecdnr"] = null;
                    Session["cfs"] = null;
                    Session["comment1"] = null;
                    Session["discountreqocdnr"] = null;
                    Session["Osrno"] = null;
                    Session["MasterdtlsOCDNR"] = null;

                    serviceTpe = null;
                    transMode = null;
                    vechicleNo = null;
                    dateOfSupply = null;
                    cinNo = null;
                    if (command == "save1")
                    {
                        TempData["Message"] = "Invoice Saved Successfully";
                        return RedirectToAction("Home");
                    }
                    else if (command == "save2")
                    {
                        return RedirectToAction("CDNR", "DownloadPdf", new { Invid = op.Value, Invdate = notedate });
                    }
                }
                return View();


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
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {

                DataTable dt = ((DataTable)Session["MasterdtlsOCDNR"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CGST Amount"]) + Convert.ToDecimal(dtr["SGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invaluecdnr"] = Convert.ToDecimal(Session["invaluecdnr"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsOCDNR"] = null;
                }
                else
                {
                    Session["MasterdtlsOCDNR"] = dt;
                }

                return RedirectToAction("Home");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
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
        public JsonResult load(int name)
        {
            var obj = (from buy in db.TBL_Buyer
                       where buy.BuyerId == name
                       select buy).SingleOrDefault();
            return Json(obj, JsonRequestBehavior.AllowGet);
        }
        public JsonResult AutoPopulate(int Prefix)
        {
            //Note : you can bind same list from database  
            var hsndetail = (from obj in db.TBL_HSN_MASTER
                             where obj.hsnId == Prefix
                             select new
                             {
                                 hsnDescription = obj.hsnDescription,
                                 unitPrice = obj.unitPrice,
                                 rate = obj.rate
                             }).SingleOrDefault();
            return Json(hsndetail, JsonRequestBehavior.AllowGet);
        }
    }
}