using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Core.Objects;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models.Inward;

namespace SmartAdminMvc.Controllers
{
    public class InwardUnregisteredController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: InwardUnregistered
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
                if (Session["MasterdtlsB2BUR"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsB2BUR"]);

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

                DataTable dt = ((DataTable)Session["MasterdtlsB2BUR"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CGST Amount"]) + Convert.ToDecimal(dtr["SGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invalueb2bur"] = Convert.ToDecimal(Session["invalueb2bur"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }

                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsB2BUR"] = null;
                }
                else
                {
                    Session["MasterdtlsB2BUR"] = dt;
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
                if (command == "add")
                {
                    decimal invoicevalue, invalue;
                    string csval;
                    Session["gstinb2bur"] = form["gstin"];
                    Session["nameb2bur"] = form["name"];
                    Session["addressb2bur"] = form["address"];
                    Session["invoiceb2bur"] = form["invoice"];
                    Session["invoicedateb2bur"] = form["invoicedate"];
                    Session["cgstinb2bur"] = form["cgstin"];
                    Session["posb2bur"] = form["pos"];

                    if (Session["MasterdtlsB2BUR"] == null)
                    {
                        Session["discountreqb2bur"] = form["isDiscountreq"];
                    }
                    int count;
                    DataTable dt = new DataTable();


                    if (Session["srnour"] == null)
                    {
                        Session["srnour"] = 1;
                        count = (int)Session["srnour"];
                    }
                    else
                    {
                        count = (int)Session["srnour"];
                        count = count + 1;
                        Session["srnour"] = count;
                    }

                    if (Session["MasterdtlsB2BUR"] == null)
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
                        dt = ((DataTable)Session["MasterdtlsB2BUR"]);
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
                    Session["MasterdtlsB2BUR"] = dt;
                    //
                    if (Session["invalueb2bur"] == null)
                    {
                        invalue = 0;
                    }
                    else
                    {
                        invalue = Convert.ToDecimal(form["invalue"]);
                    }
                    invoicevalue = Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(form["camount"]) + Convert.ToDecimal(form["samount"]) + Convert.ToDecimal(csval) + invalue;
                    Session["invalueb2bur"] = invoicevalue;
                    //
                    return RedirectToAction("Index");
                }
                else if (command == "save1" || command == "save2")
                {
                    string HSN, HSNDesc, Eligibility, cgstin, gstin, invoice, reversecharge, pos, invoicedate, refno, fp, invtype, uqc, ItemDesc, supname, address;
                    decimal invalue, Taxablevalue, rate, IGSTa, CGSTa, SGSTa, CESSa, unitprice, discount, qty;
                    int custid, itemnum = 0, userid;

                    userid = Convert.ToInt32(Session["User_Id"]);
                    custid = Convert.ToInt32(Session["Cust_ID"]);

                    supname = form["name"];
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
                            Session["invoiceb2bur"] = null;
                            return RedirectToAction("Index");
                        }
                    }

                    invalue = Convert.ToDecimal(form["invalue"]);
                    pos = form["pos"].ToString();

                    refno = Session["CustRefNo"].ToString();
                    address = form["address"];
                    con.Open();
                    SqlCommand cmd = new SqlCommand();
                    cmd.Parameters.Clear();
                    cmd.Parameters.Add("@SupplierName", SqlDbType.VarChar).Value = supname;
                    cmd.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = pos;
                    cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = address;
                    cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = custid;
                    cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userid;
                    cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                    string supid = Models.Common.Functions.InsertIntoTable("TBL_Supplier", cmd, con);
                    con.Close();

                    //ObjectParameter obj = new ObjectParameter("InvoiceId", typeof(int));
                    //db.Ins_Inward_Gstr2_B2B(name, gstin, custid, cgstin, revdate, revinvoice, taxvalue, pos, reversecharge, userid, obj);
                    //retInvoiceId = Convert.ToInt32(obj.Value);


                    DataTable dt = (DataTable)Session["MasterdtlsB2BUR"];
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


                        db.usp_Insert_INWARD_GSTR2_B2BUR_EXT(cgstin, fp, invoice, invoicedate, invalue, supname, rate, Taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, refno, HSN, ItemDesc, uqc, qty, unitprice, discount, userid, Convert.ToInt32(supid), Eligibility, pos, op);
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
                        InwardFunction.B2BURPush(refno, cgstin, custid, userid);
                    }
                    db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Reversecharge invoice generated: " + invoice, "");

                    Session["invtypeb2bur"] = null;
                    Session["addressb2bur"] = null;
                    Session["gstinb2bur"] = null;
                    Session["nameb2bur"] = null;
                    Session["invoiceb2bur"] = null;
                    Session["invoicedateb2bur"] = null;
                    Session["invalueb2bur"] = null;
                    Session["reversechargeb2bur"] = null;
                    Session["cgstinb2bur"] = null;
                    Session["posb2bur"] = null;
                    Session["MasterdtlsB2BUR"] = null;
                    Session["srnour"] = null;
                    Session["discountreqb2bur"] = null;
                    if (command == "save1")
                    {
                        TempData["Message"] = "Invoice saved Successfully";
                        return RedirectToAction("Index");
                    }
                    else if (command == "save2")
                    {
                        return RedirectToAction("Reversecharge", "DownloadPdf", new { Invid = invoice, Invdate = invoicedate });
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