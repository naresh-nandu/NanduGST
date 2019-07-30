using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;
using SmartAdminMvc.Models.Common;
using System.Configuration;
using SmartAdminMvc.Models.Inward;
using SmartAdminMvc.Models.EditInvoice;

namespace SmartAdminMvc.Controllers
{
    [System.Web.Mvc.Authorize]
    public class OutwardB2CLController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: OutwardB2CL
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
                TempData["MakerCheckerMsg"] = "You are not authorized for Outward B2CL Page.";
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

                if (Session["MasterdtlsOB2CL"] != null)
                {
                    DataTable dt = ((DataTable)Session["MasterdtlsOB2CL"]);
                    if (Session["transModeb2cl"] != null)
                    {
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cl"].ToString());
                    }
                    if (Session["serviceTypeb2cl"] != null)
                    {
                        ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cl"].ToString());
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

                DataTable dt = ((DataTable)Session["MasterdtlsOB2CL"]);
                decimal inv_value;
                foreach (DataRow dtr in dt.Rows)
                {
                    if (Convert.ToInt32(dtr[0]) == Id)
                    {
                        inv_value = Convert.ToDecimal(dtr["Taxablevalue"]) + Convert.ToDecimal(dtr["IGST Amount"]) + Convert.ToDecimal(dtr["CESS Amount"]);
                        Session["invalueob2cl"] = Convert.ToDecimal(Session["invalueob2cl"]) - inv_value;
                        dt.Rows.Remove(dtr);
                        break;
                    }

                }


                if (dt.Rows.Count == 0)
                {
                    Session["MasterdtlsOB2CL"] = null;
                }
                else
                {
                    Session["MasterdtlsOB2CL"] = dt;
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
                    Session["isautoob2cl"] = form["isauto"];
                    Session["buyernameob2cl"] = form["buyername"];
                    Session["gstinob2cl"] = form["gstin"];
                    Session["nameob2cl"] = form["name"];
                    Session["addressob2cl"] = form["address"];
                    Session["invoiceob2cl"] = form["invoice"];
                    Session["invoicedateob2cl"] = form["invoicedate"];
                    Session["cgstinob2cl"] = form["cgstin"];
                    Session["posob2cl"] = form["pos"];
                    Session["etinob2cl"] = form["etin"];
                    Session["comment1ob2cl"] = form["comment1"];

                    Session["serviceTypeb2cl"] = form["ddlServiceType"];
                    Session["transModeb2cl"] = form["transMode"];
                    Session["vehicleNob2cl"] = form["vehicleNo"];
                    Session["dateOfSupplyb2cl"] = form["dateOfSupply"];
                    Session["cinNob2cl"] = form["cinNo"];
                    if (Session["transModeb2cl"] != null)
                    {
                        ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", Session["transModeb2cl"].ToString());
                    }
                    if (Session["serviceTypeb2cl"] != null)
                    {
                        ViewBag.ServiceTypeList = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("ServiceType", Session["serviceTypeb2cl"].ToString());
                    }
                    if (Session["MasterdtlsOB2CL"] == null)
                    {
                        Session["discountreqob2cl"] = form["isDiscountreq"];
                    }
                    int count;
                    DataTable dt = new DataTable();


                    if (Session["no"] == null)
                    {
                        Session["no"] = 1;
                        count = (int)Session["no"];
                    }
                    else
                    {
                        count = (int)Session["no"];
                        count = count + 1;
                        Session["no"] = count;
                    }

                    if (Session["MasterdtlsOB2CL"] == null)
                    {

                        dt.Columns.Add("Srno", typeof(int));
                        dt.Columns.Add("HSN", typeof(string));
                        dt.Columns.Add("HSNDesc", typeof(string));
                        dt.Columns.Add("Quantity", typeof(string));
                        dt.Columns.Add("Taxablevalue", typeof(string));
                        dt.Columns.Add("Rate", typeof(string));
                        dt.Columns.Add("IGST Amount", typeof(string));
                        dt.Columns.Add("CESS Amount", typeof(string));
                        dt.Columns.Add("Unit Price", typeof(string));
                        dt.Columns.Add("Discount", typeof(string));
                        dt.Columns.Add("UQC", typeof(string));
                        dt.Columns.Add("ItemDesc", typeof(string));
                    }
                    else
                    {
                        dt = ((DataTable)Session["MasterdtlsOB2CL"]);
                    }
                    
                    DataRow dr = dt.NewRow();
                    dr[0] = count;
                    dr[1] = form["HSN"];
                    dr[2] = form["HSNDesc"];
                    dr[3] = form["qty"];
                    dr[4] = form["taxablevalue"];
                    dr[5] = form["rate"];
                    dr[6] = form["iamount"];

                    if (form["csamount"] == "")
                    {
                        dr[7] = 0;
                        csval = "0";
                    }
                    else
                    {
                        dr[7] = form["csamount"];
                        csval = form["csamount"];
                    }
                    dr[8] = form["unitprice"];
                    dr[9] = form["discount"];
                    dr["UQC"] = form["uqc"];
                    dr["ItemDesc"] = form["itemdesc"];

                   

                    dt.Rows.Add(dr);
                    Session["MasterdtlsOB2CL"] = dt;
                    //
                    if (Session["invalueob2cl"] == null)
                    {
                        invalue = 0;
                    }
                    else
                    {
                        invalue = Convert.ToDecimal(form["invalue"]);
                    }
                    invoicevalue = Convert.ToDecimal(form["taxablevalue"]) + Convert.ToDecimal(form["iamount"]) + Convert.ToDecimal(csval) + invalue;
                    Session["invalueob2cl"] = invoicevalue;
                    //
                    return RedirectToAction("Index");
                }
                else if (command == "save1" || command=="save2")
                {
                    string HSN, HSNDesc, cgstin, gstin, invoice, pos, invoicedate, etin, fp, refno, uqc, itemdesc, comment1,details;
                    decimal invalue, Taxablevalue, rate, IGSTa, CESSa, unitprice, discount, CGST, SGST, qty;
                    int  custid, itemnum = 0, userid, buyerid;
                    string serviceTpe = "", transMode = "", vechicleNo = "", dateOfSupply = "", cinNo = "";

                    serviceTpe = form["ddlServiceType"];
                    transMode = form["transMode"];
                    vechicleNo = form["vehicleNo"];
                    dateOfSupply = form["dateOfSupply"];
                    cinNo = form["cinNo"];

                    invalue = Convert.ToDecimal(form["invalue"]);
                    string strStateCode = form["cgstin"].ToString().Substring(0, 2);
                    string cpos = form["pos"].ToString();
                    if (invalue >= 250000 && strStateCode != cpos)
                    {
                        userid = Convert.ToInt32(Session["User_Id"]);
                        custid = Convert.ToInt32(Session["Cust_ID"]);
                        string companyname = form["buyername"].ToString();
                       
                        string caddress = form["address"].ToString();
                        con.Open();
                        SqlCommand cmd = new SqlCommand();
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@BuyerName", SqlDbType.VarChar).Value = companyname;
                        cmd.Parameters.Add("@StateCode", SqlDbType.VarChar).Value = cpos;
                        cmd.Parameters.Add("@Address", SqlDbType.VarChar).Value = caddress;
                        cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = custid;
                        cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = userid;
                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                        string bid = Models.Common.Functions.InsertIntoTable("TBL_Buyer", cmd, con);
                        con.Close();

                        comment1 = form["comment1"];
                        details = comment1;
                        
                        etin = form["etin"];
                        gstin = form["gstin"];
                        buyerid = Convert.ToInt32(bid);
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
                                Session["invoiceob2cl"] = null;
                                return RedirectToAction("Index");
                            }
                        }

                        pos = form["pos"].ToString().Length > 1 ? form["pos"].ToString() : "0" + form["pos"].ToString();
                        fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                        refno = Session["CustRefNo"].ToString();
                                                
                        DataTable dt = (DataTable)Session["MasterdtlsOB2CL"];
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
                            CGST = 0;
                            SGST = 0;
                            CESSa = Convert.ToDecimal(dr["CESS Amount"]);
                            unitprice = Convert.ToDecimal(dr["Unit Price"]);
                            discount = Convert.ToDecimal(dr["Discount"]);
                            uqc = Convert.ToString(dr["UQC"]);
                            
                           db.usp_Insert_Outward_GSTR1_B2CL_EXT1(cgstin,fp, invoice, invoicedate, invalue, pos, etin, rate, Taxablevalue, IGSTa, CGST, SGST, CESSa, refno, HSN, itemdesc, qty, unitprice, discount, uqc, buyerid, userid,details, serviceTpe, transMode, vechicleNo, dateOfSupply, cinNo,op);
                            db.SaveChanges();
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
                            B2CsFunctions.B2CLPush(refno, cgstin, custid, userid);
                        }
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Outward B2CL invoice generated: " + invoice, "");
                        Session["buyernameob2cl"] = null;
                        Session["etinob2cl"] = null;
                        Session["addressob2cl"] = null;
                        Session["gstinob2cl"] = null;
                        Session["nameob2cl"] = null;
                        Session["invoiceob2cl"] = null;
                        Session["invoicedateob2cl"] = null;
                        Session["invalueob2cl"] = null;
                        Session["cgstinob2cl"] = null;
                        Session["posob2cl"] = null;
                        Session["MasterdtlsOB2CL"] = null;
                        Session["no"] = null;
                        Session["comment1ob2cl"] = null;

                        Session["serviceTypeb2cl"] = null;
                        Session["transModeb2cl"] = null;
                        Session["vehicleNob2cl"] = null;
                        Session["dateOfSupplyb2cl"] = null;
                        Session["cinNob2cl"] = null;
                        if (command == "save1")
                        {
                            TempData["Message"] = "Invoice Saved Successfully";
                            return RedirectToAction("Index");
                        }
                        else if (command == "save2")
                        {
                            return RedirectToAction("B2CL", "DownloadPdf", new { Invid = op.Value, Invdate = invoicedate });
                        }
                    }
                    else
                    {
                        TempData["Message"] = "B2CL invoice format for interstate with invoice value more than 2.5 Lakhs.";
                        return RedirectToAction("Index");
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