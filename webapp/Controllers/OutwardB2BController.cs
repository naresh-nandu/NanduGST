using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.Data;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Inward;
using SmartAdminMvc.Models.EditInvoice;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardB2BController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: OutwardB2B
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward B2B Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
               
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                var buyname = db.TBL_Buyer.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { buyername = c.BuyerName, buyerid = c.BuyerId }).OrderBy(c => c.buyername).ToList();
                ViewBag.buyname = buyname;
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "OUTWARD");
   
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");

                if (Session["transModeb2b"] != null)
                {
                    ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2b"].ToString());
                }
                if (Session["serviceTypeb2b"] != null)
                {
                    ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2b"].ToString());
                }
                if (Session["MasterdtlsOB2B"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsOB2B"]);                    
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

                DataTable dt = ((DataTable)Session["MasterdtlsOB2B"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CGST Amount"]) + Convert.ToDecimal(dtr["SGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invalueob2b"] = Convert.ToDecimal(Session["invalueob2b"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }
                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsOB2B"] = null;
                }
                else
                {
                    Session["MasterdtlsOB2B"] = dt;
                }

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Index(FormCollection form, string command)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.ServiceType = Models.Common.LoadDropDowns.DropDownEwayBill("ServiceType");
                if (command == "add")
                {
                    decimal invoicevalue, invalue;
                    string csval;
                    Session["isautoob2b"] = form["isauto"];
                    Session["gstinob2b"] = form["gstin"];
                    Session["nameob2b"] = form["name"];
                    Session["addressob2b"] = form["address"];
                    Session["saddressob2b"] = form["saddress"];
                    Session["invoiceob2b"] = form["invoice"];
                    Session["invoicedateob2b"] = form["invoicedate"];
                    Session["reversechargeob2b"] = form["reversecharge"];
                    Session["cgstinob2b"] = form["cgstin"];
                    Session["posob2b"] = form["pos"];
                    Session["invtypeob2b"] = form["invtype"];
                    Session["etinob2b"] = form["etin"];
                    Session["comment1ob2b"] = form["comment1"];

                    Session["serviceTypeb2b"] = form["ddlServiceType"];
                    Session["transModeb2b"] = form["transMode"];
                    Session["vehicleNob2b"] = form["vehicleNo"];
                    Session["dateOfSupplyb2b"] = form["dateOfSupply"];
                    Session["cinNob2b"] = form["cinNo"];
                    if (Session["transModeb2b"] != null)
                    {
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2b"].ToString());
                    }
                    if (Session["serviceTypeb2b"] != null)
                    {
                        ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2b"].ToString());
                    }

                    if (Session["MasterdtlsOB2B"] == null)
                    {
                        Session["discountreqob2b"] = form["isDiscountreq"];
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

                    if (Session["MasterdtlsOB2B"] == null)
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
                        dt = ((DataTable)Session["MasterdtlsOB2B"]);
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
                    Session["MasterdtlsOB2B"] = dt;
                   
                    if (Session["invalueob2b"] == null)
                    {
                        invalue = 0;
                    }
                    else
                    {
                        invalue = Convert.ToDecimal(form["invalue"]);
                    }
                    invoicevalue = Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(form["camount"]) + Convert.ToDecimal(form["samount"]) + Convert.ToDecimal(csval) + invalue;
                    Session["invalueob2b"] = invoicevalue;

                   
                    return RedirectToAction("Index");
                }
                else if (command == "save1" || command == "save2")
                {

                    string HSN, HSNDesc, cgstin, gstin, invoice, reversecharge, pos, invoicedate, etin, fp, invtype, refno, uqc, itemdesc,comment1,details,saddress;
                    string serviceTpe = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";
                    decimal invalue, Taxablevalue, rate, IGSTa, CGSTa, SGSTa, CESSa, unitprice, discount, qty;
                    int  custid, itemnum = 0, userid, buyerid;

                    serviceTpe = form["ddlServiceType"];
                    transMode = form["transMode"];
                    vechicleNo = form["vehicleNo"];
                    dateOfSupply = form["dateOfSupply"];
                    cinNo = form["cinNo"];

                    userid = Convert.ToInt32(Session["User_Id"]);
                    custid = Convert.ToInt32(Session["Cust_ID"]);
                    saddress = form["saddress"];
                    comment1 = form["comment1"];
                    details = comment1;
                    etin = form["etin"];
                    gstin = form["gstin"];
                    buyerid = Convert.ToInt32(form["name"]);
                    cgstin = form["cgstin"];
                    invoice = form["invoice"];
                    invoicedate = form["invoicedate"];
                    if (invoice == "-")
                    {
                        invoice = "NA";
                    }
                    else
                    {

                        string Mode = "O";
                        int Result = InwardFunction.InvoiceValidation(cgstin, invoice, invoicedate, Mode);
                        if (Result == 1)
                        {
                            TempData["Message"] = "Invoice number " + invoice + " already exist";
                            Session["invoiceob2b"] = null;
                            return RedirectToAction("Index");
                        }
                       
                    }
                    invalue = Convert.ToDecimal(form["invalue"]);
                    reversecharge = form["reversecharge"];
                    pos = form["pos"].ToString().Length > 1 ? form["pos"].ToString() : "0" + form["pos"].ToString();
                    invtype = form["invtype"];
                    fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                    refno = Session["CustRefNo"].ToString();
                   


                    DataTable dt = (DataTable)Session["MasterdtlsOB2B"];
                    ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                    foreach (DataRow dr in dt.Rows)
                    {
                        

                        itemnum = itemnum + 1;
                        HSN = Convert.ToString(dr["HSN"]);
                        HSNDesc = Convert.ToString(dr["HSNDesc"]);
                        itemdesc = Convert.ToString(dr["ItemDesc"]);
                        qty = Convert.ToDecimal(dr["Quantity"]);
                        rate = Convert.ToDecimal(dr["rate"]);
                        Taxablevalue = Convert.ToDecimal(dr["Taxablevalue"]);
                        IGSTa = Convert.ToDecimal(dr["IGST Amount"]);
                        CGSTa = Convert.ToDecimal(dr["CGST Amount"]);
                        SGSTa = Convert.ToDecimal(dr["SGST Amount"]);
                        CESSa = Convert.ToDecimal(dr["CESS Amount"]);
                        unitprice = Convert.ToDecimal(dr["Unit Price"]);
                        discount = Convert.ToDecimal(dr["Discount"]);
                        uqc = Convert.ToString(dr["UQC"]);
                       
                      db.usp_Insert_Outward_GSTR1_B2B_EXT1(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, etin, invtype, rate, Taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, refno, HSN, itemdesc, qty, unitprice, discount, uqc, buyerid, userid,details,saddress,serviceTpe,transMode,vechicleNo,dateOfSupply,cinNo,op);
                       
                        if (op.Value == null)
                        {
                            invoice = "NA";
                        }
                        else
                        {
                            invoice = op.Value.ToString();
                        }
                       
                    }
                    if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() != "Approver")
                    {
                        //
                    }
                    else
                    {
                        B2CsFunctions.B2BPush(refno, cgstin, custid, userid);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2B invoice generated: " + invoice, "");
                  
                    Session["etinob2b"] = null;
                    Session["invtypeob2b"] = null;
                    Session["addressob2b"] = null;
                    Session["gstinob2b"] = null;
                    Session["nameob2b"] = null;
                    Session["invoiceob2b"] = null;
                    Session["invoicedateob2b"] = null;
                    Session["invalueob2b"] = null;
                    Session["reversechargeob2b"] = null;
                    Session["cgstinob2b"] = null;
                    Session["posob2b"] = string.Empty;
                    Session["discountreqob2b"] = null;
                    Session["MasterdtlsOB2B"] = null;
                    Session["Osrno"] = null;
                    Session["isautoob2b"] = null;
                    Session["discountreqob2b"] = null;
                    Session["comment1ob2b"] = null;
                    Session["saddressob2b"] = null;

                    Session["serviceTypeb2b"] =null;
                    Session["transModeb2b"] = null;
                    Session["vehicleNob2b"] = null;
                    Session["dateOfSupplyb2b"] = null;
                    Session["cinNob2b"] = null;

                    if (command == "save1")
                    {
                        TempData["Message"] = "Invoice Saved Successfully";
                        return RedirectToAction("Index");
                    }
                    else if (command == "save2")
                    {

                        return RedirectToAction("Demo", "DownloadPDF", new { Invid = op.Value, invdate = invoicedate });
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

    
        public JsonResult AutoPopulate(string Prefix)
        {
            //Note : you can bind same list from database  
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            var hsndetail = (from obj in db.TBL_HSN_MASTER
                             where (obj.hsnCode.Contains(Prefix) && obj.hsnType=="OUTWARD" && obj.CustomerId== custid)
                             select new
                             {
                                 hsncode = obj.hsnCode,
                                 hsnid = obj.hsnId,
                                 hsndesc = obj.hsnDescription,
                                 rate=obj.rate
                             });
            return Json(hsndetail, JsonRequestBehavior.AllowGet);
        }

        public JsonResult HsnAutoPopulate(string Prefix, string Action)
        {
            //Note : you can bind same list from database  
            int custid = Convert.ToInt32(Session["Cust_ID"]);

            if (Action == "O")
            {
                var hsndetail = (from obj in db.TBL_HSN_MASTER
                                 where (obj.hsnCode.Contains(Prefix) && obj.hsnType == "OUTWARD" && obj.CustomerId == custid)
                                 select new
                                 {
                                     hsncode = obj.hsnCode,
                                     hsnid = obj.hsnId,
                                     hsndesc = obj.hsnDescription,
                                     rate = obj.rate
                                 });
                return Json(hsndetail, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var hsndetail = (from obj in db.TBL_HSN_MASTER
                                 where (obj.hsnCode.Contains(Prefix) && obj.hsnType == "INWARD" && obj.CustomerId == custid)
                                 select new
                                 {
                                     hsncode = obj.hsnCode,
                                     hsnid = obj.hsnId,
                                     hsndesc = obj.hsnDescription,
                                     rate = obj.rate
                                 });
                return Json(hsndetail, JsonRequestBehavior.AllowGet);
            }

        }
        public JsonResult AutoPopulate1(int id)
        {
          
             var hsndetail = (from obj in db.TBL_HSN_MASTER
                             where obj.hsnId == id
                             select new
                             {
                                 hsncode=obj.hsnCode,
                                 hsnDescription = obj.hsnDescription,
                                 unitPrice = obj.unitPrice,
                                 rate = obj.rate
                             }).SingleOrDefault();
            return Json(hsndetail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult load(int name)
        {
            var obj = (from buy in db.TBL_Buyer
                       where buy.BuyerId == name
                       select buy).SingleOrDefault();
            return Json(obj, JsonRequestBehavior.AllowGet);
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
    }
}

