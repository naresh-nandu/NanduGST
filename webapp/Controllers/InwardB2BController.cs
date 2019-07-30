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

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class InwardB2BController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: InwardB2B
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            if (Session["MakerCheckerApproverSetting"].ToString() == "True" && Session["MakerCheckerApproverType"].ToString() == "Checker")
            {
                TempData["MakerCheckerMsg"] = "You are not authorized for GSTR1 Save Page.";
                return RedirectToAction("Summary", "Dashboard");
            }

            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                var supname = db.TBL_Supplier.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { suppliername = c.SupplierName, supplierid = c.SupplierId }).OrderBy(c => c.suppliername).ToList();
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "INWARD");
                ViewBag.supname = supname;
                if (Session["MasterdtlsB2B"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsB2B"]);

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

                DataTable dt = ((DataTable)Session["MasterdtlsB2B"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CGST Amount"]) + Convert.ToDecimal(dtr["SGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invalueb2b"] = Convert.ToDecimal(Session["invalueb2b"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }

                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsB2B"] = null;
                }
                else
                {
                    Session["MasterdtlsB2B"] = dt;
                }

                return RedirectToAction("Home");
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
                return RedirectToAction("Login", "Account");
            try
            {
                if (command == "add")
                {
                    decimal invoicevalue, invalue;
                    string csval;
                    Session["isautob2b"] = form["isauto"];
                    Session["gstinb2b"] = form["gstin"];
                    Session["nameb2b"] = form["name"];
                    Session["addressb2b"] = form["address"];
                    Session["invoiceb2b"] = form["invoice"];
                    Session["invoicedateb2b"] = form["invoicedate"];
                    Session["reversechargeb2b"] = form["reversecharge"];
                    Session["cgstinb2b"] = form["cgstin"];
                    Session["posb2b"] = form["pos"];
                    Session["invtypeb2b"] = form["invtype"];

                    if (Session["MasterdtlsB2B"] == null)
                    {
                        Session["discountreqb2b"] = form["isDiscountreq"];
                    }
                    int count;
                    DataTable dt = new DataTable();


                    if (Session["srno"] == null)
                    {
                        Session["srno"] = 1;
                        count = (int)Session["srno"];
                    }
                    else
                    {
                        count = (int)Session["srno"];
                        count = count + 1;
                        Session["srno"] = count;
                    }

                    if (Session["MasterdtlsB2B"] == null)
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
                        dt.Columns.Add("Eligibility", typeof(string));
                        dt.Columns.Add("Unit Price", typeof(string));
                        dt.Columns.Add("Discount", typeof(string));
                        dt.Columns.Add("UQC", typeof(string));
                        dt.Columns.Add("ItemDesc", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterdtlsB2B"]);
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
                    dr["UQC"] = form["uqc"];
                    dr["ItemDesc"] = form["itemdesc"];
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

                    dr[10] = form["eligibility"];
                    dr[11] = form["unitprice"];
                    dr[12] = form["discount"];

                    dt.Rows.Add(dr);
                    Session["MasterdtlsB2B"] = dt;
                    //
                    if (Session["invalueb2b"] == null)
                    {
                        invalue = 0;
                    }
                    else
                    {
                        invalue = Convert.ToDecimal(form["invalue"]);
                    }
                    invoicevalue = Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(form["camount"]) + Convert.ToDecimal(form["samount"]) + Convert.ToDecimal(csval) + invalue;
                    Session["invalueb2b"] = invoicevalue;
                    //
                    return RedirectToAction("Home");
                }
                else if (command == "save" || command == "save2")
                {
                    string HSN, HSNDesc, Eligibility, cgstin, gstin, invoice, reversecharge, pos, invoicedate, refno, fp, invtype, uqc, ItemDesc;
                    decimal invalue, Taxablevalue, rate, IGSTa, CGSTa, SGSTa, CESSa, unitprice, discount, qty;
                    int custid, itemnum = 0, userid, supid;

                    userid = Convert.ToInt32(Session["User_Id"]);
                    custid = Convert.ToInt32(Session["Cust_ID"]);

                    gstin = form["gstin"];
                    supid = Convert.ToInt32(form["name"]);
                    cgstin = form["cgstin"];
                    invoice = form["invoice"];
                    invoicedate = form["invoicedate"];
                    fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                    if (invoice == "-")
                    {
                        invoice = "NA";
                    }
                    else
                    {
                        string Mode = "I";
                        int Result = InwardFunction.InvoiceValidation(cgstin, invoice, invoicedate, Mode);
                        if (Result == 1)
                        {
                            TempData["Message"] = "Invoice number " + invoice + " already exist";
                            Session["invoiceb2b"] = null;
                            return RedirectToAction("Home");
                        }
                    }

                    invalue = Convert.ToDecimal(form["invalue"]);
                    reversecharge = form["reversecharge"];
                    pos = form["pos"];
                    invtype = form["invtype"];
                    refno = Session["CustRefNo"].ToString();


                    DataTable dt = (DataTable)Session["MasterdtlsB2B"];
                    ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                    foreach (DataRow dr in dt.Rows)
                    {

                        itemnum = itemnum + 1;
                        HSN = Convert.ToString(dr["HSN"]);
                        HSNDesc = Convert.ToString(dr["HSNDesc"]);
                        ItemDesc = Convert.ToString(dr["ItemDesc"]);
                        qty = Convert.ToDecimal(dr["Quantity"]);
                        rate = Convert.ToDecimal(dr["rate"]);
                        Taxablevalue = Convert.ToDecimal(dr["Taxablevalue"]);
                        IGSTa = Convert.ToDecimal(dr["IGST Amount"]);
                        CGSTa = Convert.ToDecimal(dr["CGST Amount"]);
                        SGSTa = Convert.ToDecimal(dr["SGST Amount"]);
                        CESSa = Convert.ToDecimal(dr["CESS Amount"]);
                        Eligibility = Convert.ToString(dr["Eligibility"]);
                        unitprice = Convert.ToDecimal(dr["Unit Price"]);
                        discount = Convert.ToDecimal(dr["Discount"]);
                        uqc = Convert.ToString(dr["UQC"]);

                        db.usp_Insert_INWARD_GSTR2_B2B_EXT(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, invtype, rate, Taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, refno, HSN, ItemDesc, qty, unitprice, discount, uqc, supid, userid, Eligibility, op);
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
                        InwardFunction.B2BPush(refno, cgstin, custid, userid);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice generated: " + invoice, "");

                    Session["invtypeb2b"] = null;
                    Session["addressb2b"] = null;
                    Session["gstinb2b"] = null;
                    Session["nameb2b"] = null;
                    Session["invoiceb2b"] = null;
                    Session["invoicedateb2b"] = null;
                    Session["invalueb2b"] = null;
                    Session["reversechargeb2b"] = null;
                    Session["cgstinb2b"] = null;
                    Session["posb2b"] = null;
                    Session["MasterdtlsB2B"] = null;
                    Session["srno"] = null;
                    Session["isautob2b"] = null;
                    Session["discountreqb2b"] = null;
                    //TempData["Message"] = "Inward B2B invoice details saved successfully..!";

                    if (command == "save1")
                    {
                        TempData["Message"] = "Invoice saved Successfully";
                        return RedirectToAction("Home");
                    }
                    else if (command == "save2")
                    {
                        return RedirectToAction("G2B2B", "DownloadPdf", new { Invid = invoice, Invdate = invoicedate });
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
        public JsonResult AutoPopulate1(int id)
        {

            var hsndetail = (from obj in db.TBL_HSN_MASTER
                             where obj.hsnId == id
                             select new
                             {
                                 hsncode = obj.hsnCode,
                                 hsnDescription = obj.hsnDescription,
                                 unitPrice = obj.unitPrice,
                                 rate = obj.rate
                             }).SingleOrDefault();
            return Json(hsndetail, JsonRequestBehavior.AllowGet);
        }
        public JsonResult load(int name)
        {
            var obj = (from sup in db.TBL_Supplier
                       where sup.SupplierId == name
                       select sup).SingleOrDefault();
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