using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.EditInvoice;
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
using static javax.websocket.ClientEndpointConfig;

namespace SmartAdminMvc.Controllers
{
    public class InwardRegisterController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        #region "HOME"

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

            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int UserId = Convert.ToInt32(Session["User_ID"]);

            ViewBag.MakerList = LoadDropDowns.GetMakerUserlist(CustId, UserId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2BUR",
                Value = "B2BUR"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value");

            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection form)
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
            string RefNo = Session["CustRefNo"].ToString();

            string MakerId = "", strUserId = "";
            MakerId = form["ddlMaker"];
            string action = form["option"];
            TempData["option"] = form["option"];
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
            Session["op"] = action;

            var invoice = Getinvoice(action, strUserId, RefNo);
            ViewBag.invoice = invoice;

            ViewBag.MakerList = LoadDropDowns.Exist_GetMakerUserlist(CustId, UserId, MakerId);
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2BUR",
                Value = "B2BUR"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value", Convert.ToString(TempData["option"]));
            return View();
        }

        public List<IDictionary> Getinvoice(string action, string strUserId, string strRefNo)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Generate_PDF_GSTR2_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", strRefNo));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", action));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", strUserId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
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

        public ActionResult ListDelete(string InvId, string Invdate, string ActionType, string strUserId, string strRefNo)
        {
            string CustRefNo = Session["CustRefNo"].ToString();


            if (ActionType == "B2B")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR2_B2B_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.b2bid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR2_EXT(ActionType, Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 B2B Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR2 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR2 View. So Please Delete in GSTR View and Delete..!";
                    }
                }
            }


            else if (ActionType == "B2BUR")
            {
                var invoiceid = (from li in db.TBL_EXT_GSTR2_B2BUR_INV where li.inum == InvId && li.idt == Invdate && li.referenceno == CustRefNo select li.b2burid).ToList();

                ObjectParameter op = new ObjectParameter("ErrorCode", typeof(int));
                ObjectParameter op1 = new ObjectParameter("ErrorMessage", typeof(string));

                foreach (int Refid in invoiceid)
                {
                    db.usp_Delete_GSTR2_EXT(ActionType, Refid, op, op1);
                    int value = Convert.ToInt32(op.Value);
                    if (value == 1)
                    {
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "GSTR2 B2BUR Invoice: " + InvId + " is deleted", "");
                        TempData["Message"] = "Invoice Deleted Successfully";
                    }
                    else if (value == -2)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR2 Save. So Please Delete in GSTR View and Delete..!";
                    }
                    else if (value == -3)
                    {
                        TempData["Message"] = "Invoice already Uploaded to GSTR2 View. So Please Delete in GSTR View and Delete..!";
                    }
                }
            }

            
            var invoice = Getinvoice(ActionType, strUserId, strRefNo);
            ViewBag.invoice = invoice;


            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "B2B",
                Value = "B2B"
            });
            items.Add(new SelectListItem
            {
                Text = "B2BUR",
                Value = "B2BUR"
            });
            ViewBag.Actionlist = new SelectList(items, "Text", "Value", ActionType);

            return RedirectToAction("Home");
        }

        #endregion

        #region "INWARD B2B EDIT"

        public ActionResult B2B(string inum, string idt)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            Session["inum"] = inum;
            Session["idt"] = idt;

            string refnum;
            refnum = Session["CustRefNo"].ToString();

            var records = (from li in db.TBL_EXT_GSTR2_B2B_INV where li.inum == inum && li.idt == idt && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("Home", "InwardRegister");
            }
            else
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                var supname = db.TBL_Supplier.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { suppliername = c.SupplierName, supplierid = c.SupplierId }).OrderBy(c => c.suppliername).ToList();
                ViewBag.supname = supname;
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "INWARD");

                var master = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                              where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                              select b2b).First();

                object lineitms = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                                   where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                                   select b2b).ToList();
                string supid = (from b2b in db.TBL_EXT_GSTR2_B2B_INV
                                where b2b.inum == inum && b2b.idt == idt && b2b.referenceno == refnum
                                select b2b.supplierid).First().ToString();

                ViewBag.master = master;
                ViewBag.lineitms = lineitms;
                int supplier = Convert.ToInt32(supid);
                ViewBag.suppliername = db.TBL_Supplier.Where(b => b.SupplierId == supplier).Select(b => b.SupplierName).SingleOrDefault().ToString();
                ViewBag.supplieraddress = db.TBL_Supplier.Where(b => b.SupplierId == supplier).Select(b => b.Address).SingleOrDefault().ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult B2B(FormCollection form, string command)
        {
            string cgstin, gstin, fp, invoice, reversecharge, pos, invoicedate, etin, invtype, hsncode, itemdesc, uqc, elg;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, camt, samt, csamt;
            int supid, userid;

            userid = Convert.ToInt32(Session["User_Id"]);
            gstin = form["gstin"];
            supid = Convert.ToInt32(form["name"]);
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            invalue = Convert.ToDecimal(form["invalue"]);
            reversecharge = form["reversecharge"];
            pos = form["pos"];
            invtype = form["invtype"];
            fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);

            if (command == "save")
            {
                ObjectParameter op = new ObjectParameter("retval", typeof(int));

                db.usp_Update_INWARD_GSTR2_B2B_Master(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, invtype, supid, op);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice updated" + Session["inum"].ToString(), "");
                int ret = Convert.ToInt32(op.Value);
                if (ret == 1)
                {
                    TempData["Message"] = "Details updated successfully";
                }
            }
            else if (command == "add")
            {
                string hsnid = form["HSN"];
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                camt = Convert.ToDecimal(form["camount"]);
                samt = Convert.ToDecimal(form["samount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                elg = form["eligibility"];

                fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                string refno = Session["CustRefNo"].ToString();
                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                db.usp_Insert_INWARD_GSTR2_B2B_EXT(cgstin, fp, gstin, invoice, invoicedate, invalue, pos, reversecharge, invtype, rate, taxablevalue, iamt, camt, samt, csamt, refno, hsnid, itemdesc, qty, unitprice, discount, uqc, supid, userid, elg, op);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item added" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item added successfully";

            }
            return RedirectToAction("B2B", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
        }

        [HttpPost]
        public JsonResult B2BEdit(int id, string hsncode, string itemdesc, string qty, string uqc, string unitprice, string discount, string txval, string rate, string igsta, string cgsta, string sgsta, string cessa)
        {

            TBL_EXT_GSTR2_B2B_INV inv = db.TBL_EXT_GSTR2_B2B_INV.Where(s => s.b2bid == id).SingleOrDefault();
            var message = inv;

            decimal taxablevalue, rt, IGSTa, CGSTa, SGSTa, CESSa, price, disc, quantities;

            //string hsncd = (from hsn in db.TBL_HSN_MASTER
            //                where hsn.hsnId == hsncode
            //                select hsn.hsnCode).FirstOrDefault();

            price = Convert.ToDecimal(unitprice);
            disc = Convert.ToDecimal(discount);
            taxablevalue = Convert.ToDecimal(txval);
            rt = Convert.ToDecimal(rate);
            IGSTa = Convert.ToDecimal(igsta);
            CGSTa = Convert.ToDecimal(cgsta);
            SGSTa = Convert.ToDecimal(sgsta);
            CESSa = Convert.ToDecimal(cessa);
            quantities = Convert.ToDecimal(qty);
            ObjectParameter op = new ObjectParameter("retval", typeof(int));


            db.usp_Update_INWARD_GSTR2_B2B_Items(id, hsncode, itemdesc, quantities, price, disc, uqc, rt, taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, Convert.ToInt32(Session["User_ID"]), op);
            
            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item updated" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item updated successfully";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult B2BDelete(int Id)
        {
            TBL_EXT_GSTR2_B2B_INV inv = db.TBL_EXT_GSTR2_B2B_INV.Where(s => s.b2bid == Id).SingleOrDefault();
            var message = inv;
            ObjectParameter op = new ObjectParameter("retval", typeof(int));

            db.usp_Delete_INWARD_GSTR2_B2B_Items(Id, op);

            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item deleted" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item deleted successfully..";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
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

        #endregion

        #region "INWARD B2BUR EDIT"

        public ActionResult B2BUR(string inum, string idt)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            Session["inum"] = inum;
            Session["idt"] = idt;

            string refnum;
            refnum = Session["CustRefNo"].ToString();

            var records = (from li in db.TBL_EXT_GSTR2_B2BUR_INV where li.inum == inum && li.idt == idt && li.referenceno == refnum select li).ToList();
            if (records.Count <= 0)
            {
                TempData["Message"] = "Invoice Deleted Successfully..";
                return RedirectToAction("Home", "InwardRegister");
            }
            else
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                var supname = db.TBL_Supplier.Where(m => m.CustomerId == custid && m.GSTINno != null && m.RowStatus == true).Select(c => new { suppliername = c.SupplierName, supplierid = c.SupplierId }).OrderBy(c => c.suppliername).ToList();
                ViewBag.supname = supname;
                ViewBag.Categories = LoadDropDowns.GSTIN(userid, custid, Session["Role_Name"].ToString());
                ViewBag.Rates = LoadDropDowns.LoadRates();
                ViewBag.UQCList = LoadDropDowns.LoadUQC();
                ViewBag.HSNDetail = LoadDropDowns.LoadHSN(custid, "INWARD");

                var master = (from b2bur in db.TBL_EXT_GSTR2_B2BUR_INV
                              where b2bur.inum == inum && b2bur.idt == idt && b2bur.referenceno == refnum
                              select b2bur).First();

                object lineitms = (from b2bur in db.TBL_EXT_GSTR2_B2BUR_INV
                                   where b2bur.inum == inum && b2bur.idt == idt && b2bur.referenceno == refnum
                                   select b2bur).ToList();
                string supid = (from b2bur in db.TBL_EXT_GSTR2_B2BUR_INV
                                where b2bur.inum == inum && b2bur.idt == idt && b2bur.referenceno == refnum
                                select b2bur.supplierid).First().ToString();

                ViewBag.master = master;
                ViewBag.lineitms = lineitms;
                int supplier = Convert.ToInt32(supid);
                ViewBag.suppliername = db.TBL_Supplier.Where(b => b.SupplierId == supplier).Select(b => b.SupplierName).SingleOrDefault().ToString();
                ViewBag.supplieraddress = db.TBL_Supplier.Where(b => b.SupplierId == supplier).Select(b => b.Address).SingleOrDefault().ToString();
                return View();
            }
        }

        [HttpPost]
        public ActionResult B2BUR(FormCollection form, string command)
        {
            string cgstin, cname, fp, invoice, reversecharge, pos, invoicedate, etin, invtype, hsncode, itemdesc, uqc, elg;
            decimal invalue, qty, discount, unitprice, rate, taxablevalue, iamt, camt, samt, csamt;
            string supid; int userid;

            userid = Convert.ToInt32(Session["User_Id"]);
            cname = form["cname"];
            
            cgstin = form["cgstin"];
            invoice = form["invoice"];
            invoicedate = form["invoicedate"];
            invalue = Convert.ToDecimal(form["invalue"]);
            reversecharge = form["reversecharge"];
            pos = form["pos"];
            invtype = form["invtype"];
            fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
            string refnum = "";
            refnum = Session["CustRefNo"].ToString();

            supid = (from b2bur in db.TBL_EXT_GSTR2_B2BUR_INV
                     where b2bur.inum == invoice && b2bur.idt == invoicedate && b2bur.referenceno == refnum
                     select b2bur.supplierid).First().ToString();

            if (command == "save")
            {
                ObjectParameter op = new ObjectParameter("retval", typeof(int));
                db.usp_Update_INWARD_GSTR2_B2BUR_Master(cgstin, fp, cname, invoice, invoicedate, invalue, pos, "", Convert.ToInt32(supid), op);
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice updated" + Session["inum"].ToString(), "");
                int ret = Convert.ToInt32(op.Value);
                if (ret == 1)
                {
                    TempData["Message"] = "Details updated successfully";
                }
            }
            else if (command == "add")
            {
                string hsnid = form["HSN"];
                itemdesc = form["itemdesc"];
                taxablevalue = Convert.ToDecimal(form["taxablevalue"]);
                iamt = Convert.ToDecimal(form["iamount"]);
                camt = Convert.ToDecimal(form["camount"]);
                samt = Convert.ToDecimal(form["samount"]);
                csamt = Convert.ToDecimal(form["csamount"]);
                unitprice = Convert.ToDecimal(form["unitprice"]);
                discount = Convert.ToDecimal(form["discount"]);
                rate = Convert.ToDecimal(form["rate"]);
                qty = Convert.ToDecimal(form["qty"]);
                uqc = form["uqc"];
                elg = form["eligibility"];

                fp = invoicedate.Substring(3, 2) + invoicedate.Substring(6, 4);
                string refno = Session["CustRefNo"].ToString();
                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
                db.usp_Insert_INWARD_GSTR2_B2BUR_EXT(cgstin, fp, invoice, invoicedate, invalue, cname, rate, taxablevalue, iamt, camt, samt, csamt, refno, hsnid, itemdesc, uqc, qty, unitprice, discount, userid, Convert.ToInt32(supid), elg, pos, op);
                                
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item added" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item added successfully";

            }
            return RedirectToAction("B2BUR", new { inum = Session["inum"].ToString(), idt = Session["idt"].ToString() });
        }

        [HttpPost]
        public JsonResult B2BUREdit(int id, string hsncode, string itemdesc, string qty, string uqc, string unitprice, string discount, string txval, string rate, string igsta, string cgsta, string sgsta, string cessa)
        {

            TBL_EXT_GSTR2_B2BUR_INV inv = db.TBL_EXT_GSTR2_B2BUR_INV.Where(s => s.b2burid == id).SingleOrDefault();
            var message = inv;

            decimal taxablevalue, rt, IGSTa, CGSTa, SGSTa, CESSa, price, disc, quantities;

            //string hsncd = (from hsn in db.TBL_HSN_MASTER
            //                where hsn.hsnId == hsncode
            //                select hsn.hsnCode).FirstOrDefault();

            price = Convert.ToDecimal(unitprice);
            disc = Convert.ToDecimal(discount);
            taxablevalue = Convert.ToDecimal(txval);
            rt = Convert.ToDecimal(rate);
            IGSTa = Convert.ToDecimal(igsta);
            CGSTa = Convert.ToDecimal(cgsta);
            SGSTa = Convert.ToDecimal(sgsta);
            CESSa = Convert.ToDecimal(cessa);
            quantities = Convert.ToDecimal(qty);
            ObjectParameter op = new ObjectParameter("retval", typeof(int));


            db.usp_Update_INWARD_GSTR2_B2BUR_Items(id, hsncode, itemdesc, quantities, price, disc, uqc, rt, taxablevalue, IGSTa, CGSTa, SGSTa, CESSa, Convert.ToInt32(Session["User_ID"]), op);
            
            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2B invoice item updated" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item updated successfully";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }
            return Json(message, JsonRequestBehavior.AllowGet);
        }

        public JsonResult B2BURDelete(int Id)
        {
            TBL_EXT_GSTR2_B2BUR_INV inv = db.TBL_EXT_GSTR2_B2BUR_INV.Where(s => s.b2burid == Id).SingleOrDefault();
            var message = inv;
            ObjectParameter op = new ObjectParameter("retval", typeof(int));

            db.usp_Delete_INWARD_GSTR2_B2BUR_Items(Id, op);

            int ret = Convert.ToInt32(op.Value);

            if (ret == 1)
            {
                db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Inward B2BUR invoice item deleted" + Session["inum"].ToString(), "");
                TempData["Message"] = "Item deleted successfully..";
            }
            else if (ret == -1)
            {
                TempData["Message"] = "Item not available";
            }

            return Json(message, JsonRequestBehavior.AllowGet);
        }
        
        #endregion
    }
}