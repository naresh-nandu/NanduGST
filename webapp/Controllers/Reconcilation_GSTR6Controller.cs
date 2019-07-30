#region Using
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.Reconcilation;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_DAL;

#endregion

namespace SmartAdminMvc.Controllers
{
    public class Reconcilation_GSTR6Controller : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        private readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult Index()
        {
            int id = Convert.ToInt32(Request.QueryString["InvId"]);
            string action = Convert.ToString(Request.QueryString["ActionType"]);
            string type = Convert.ToString(Request.QueryString["GSTRType"]);
            Session["id"] = id;
            Session["action"] = action;
            Session["type"] = type;
            return View();
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MatchedInvoices()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {
                    if (Session["CF_GSTIN_GSTR6"] != null)
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                       strGStin = Session["CF_GSTIN_GSTR6"].ToString();
                    }
                    else
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                    }
                    if (Session["CF_PERIOD_GSTR6"] != null)
                    {
                        ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                        fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                     }
                    if (Session["CF_TOPERIOD_GSTR6"] != null)
                    {
                        ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                     }
                    if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                    {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString(), Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(custid, userid, Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(custid, userid);
                        }
                    }
                Session["action"] = "B2B";
                ViewBag.MatchedInvoices = new ReconcilationDataModel(custid, userid).GetMatchedInvoicesGSTR6(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMatchedInvoicesGSTR6(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);

                TempData["ExportData"] = dt;
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;
                
            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MatchedInvoices(FormCollection frm, string Accept, string AcceptALL, string Reject, string Pending, string Trigger, string Export, string[] GSTR6Id)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strtoperiod = "";

            strGSTINNo = frm["ddlGSTINNo"];
            strCTIN = frm["ddlSupGSTIN"];
            strPeriod = frm["period"];
            strAction = frm["ddlActionType"];
            strSupplierName = frm["ddlSupplierName"];
            strtoperiod = frm["toperiod"];

            Session["GSTIN"] = strGSTINNo;
            Session["action"] = strAction;
            ViewBag.Period = strPeriod;
            ViewBag.ToPeriod = strtoperiod;
            ViewBag.ActionType = strAction;

            String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            String too = DateTime.ParseExact(strtoperiod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            DateTime from_dt = Convert.ToDateTime(fromm);
            DateTime to_dt = Convert.ToDateTime(too);

            String diff = (to_dt - from_dt).TotalDays.ToString();
            int diff_dt = Convert.ToInt32(diff);
            if (diff_dt < 0)
            {
                TempData["Message"] = "To Date should always be greater than or equal to from Date";
            }

            //Accept Invoice
            if (!string.IsNullOrEmpty(Accept))
            {
                string[] invid;
                if (GSTR6Id != null)
                {
                    foreach (var id in GSTR6Id)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        if (!string.IsNullOrEmpty(ids))
                        {
                            con.Open();
                            SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices_GSTR6", con);
                            dCmd.Parameters.Clear();
                            dCmd.CommandType = CommandType.StoredProcedure;
                            dCmd.CommandTimeout = 0;
                            dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                            dCmd.Parameters.Add(new SqlParameter("@RefIds", Convert.ToInt32(invid[0])));
                            dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                            dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                            dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                            dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                            dCmd.ExecuteNonQuery();
                            con.Close();

                        }
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                    TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";
                }
            }

            //Accept ALL Invoices
            if (!string.IsNullOrEmpty(AcceptALL))
            {
                if (Session["Matched_B2BRefIds"].ToString() != "")
                {
                    con.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices_GSTR6", con);
                    dCmd.Parameters.Clear();
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    if (strAction == "B2B")
                    {
                        dCmd.Parameters.Add(new SqlParameter("@RefIds", Session["Matched_B2BRefIds"].ToString()));
                    }
                    if (strAction == "CDNR" || strAction == "CDN")
                    {
                        dCmd.Parameters.Add(new SqlParameter("@RefIds", Session["Matched_CDNRefIds"].ToString()));
                    }
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.ExecuteNonQuery();
                    con.Close();

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "ALL Matched Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                    TempData["ReconcilationResponse"] = "ALL Invocies Accepted Successfully";
                }
            }

            //Reject Invoices
            else if (!string.IsNullOrEmpty(Reject))
            {
                string[] invid;
                if (GSTR6Id != null)
                {
                    foreach (var id in GSTR6Id)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices_GSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(invid[0])));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "R"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Rejected Successfully";
                }
            }

            //Pending Invoices
            else if (!string.IsNullOrEmpty(Pending))
            {
                string[] invid;
                if (GSTR6Id != null)
                {
                    foreach (var id in GSTR6Id)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Matched_Invoices_GSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(invid[0])));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "P"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies marked as 'Pending' Successfully";
                }
            }

            //Trigger Invoice
            else if (!string.IsNullOrEmpty(Trigger))
            {
                string[] sup_Gstin;
                string GSTINNO = string.Empty;
                string INVNO = string.Empty;
                string strEmail = string.Empty;
                string strMobileNo = string.Empty;
                string InvDate = string.Empty;
                string InvVal = string.Empty;
                string gstin_2 = string.Empty;
                string UserEmail = string.Empty;
                if (GSTR6Id != null)
                {
                    foreach (var id in GSTR6Id)
                    {
                        string ids = id.ToString();
                        sup_Gstin = ids.Split(',');
                        GSTINNO = sup_Gstin[1].ToString();
                        INVNO = sup_Gstin[2].ToString();
                        InvDate = sup_Gstin[3].ToString();
                        InvVal = sup_Gstin[4].ToString();
                        gstin_2 = sup_Gstin[7].ToString();
                        strEmail = (from lst in db.TBL_Supplier
                                    where lst.GSTINno == GSTINNO
                                    select lst.EmailId).FirstOrDefault();
                        strMobileNo = (from lst in db.TBL_Supplier
                                       where lst.GSTINno == GSTINNO
                                       select lst.MobileNo).FirstOrDefault();
                        UserEmail = (from lst in db.UserLists
                                     where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                     select lst.Email).FirstOrDefault();

                        string comp = Session["CompanyName"].ToString();
                        string mob = Session["MobileNo"].ToString();                        
                        string name = Session["Name"].ToString();

                        if (strEmail != null && strMobileNo != null)
                        {
                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Missing Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + InvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                            Notification.SendSMS(strMobileNo, string.Format("Please Review the Missing Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", Invoice Value : " + InvVal));
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Selected Invocies Triggered Successfully";
                        }
                        else
                        {
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Matched Invocies Not triggered because Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                        }

                    }
                }
            }
            else if (!string.IsNullOrEmpty(Export))
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["ExportData"] as DataTable;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Matched_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            }
            if (string.IsNullOrEmpty(strSupplierName))
            {
                strSupplierName = "ALL";
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");

            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
            }
            ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["ToPeriod"] = strtoperiod;

            if (!string.IsNullOrEmpty(strAction))
            {
                ViewBag.MatchedInvoices = new ReconcilationDataModel(custid, userid).GetMatchedInvoicesGSTR6(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMatchedInvoicesGSTR6(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                if (dt.Rows.Count > 0)
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Matched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                }
                TempData["ExportData"] = dt;
            }
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            return View();
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MissinginGSTR6A()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";

            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {
                    if (Session["CF_GSTIN_GSTR6"] != null)
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                        strGStin = Session["CF_GSTIN_GSTR6"].ToString();
                     }
                    else
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                    }
                    if (Session["CF_PERIOD_GSTR6"] != null)
                    {
                        ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                         fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                      }
                    if (Session["CF_TOPERIOD_GSTR6"] != null)
                    {
                        ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                     }
                    if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                    {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, Session["CF_SUPNAME_GSTR6"].ToString());

                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString(), Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(custid, userid, Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(custid, userid);
                        }
                    }

                Session["action"] = "B2B";
                ViewBag.MissinginGSTR6A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6A(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMissingInvoiceinGSTR6A(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);

                TempData["ExportData"] = dt;
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MissinginGSTR6A(FormCollection frm, string Accept, string Reject, string Pending, string Trigger, string Export, string[] RefIds)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strInvIds = "", strtoperiod = "";

            strGSTINNo = frm["ddlGSTINNo"];
            strCTIN = frm["ddlSupGSTIN"];
            strPeriod = frm["period"];
            strAction = frm["ddlActionType"];
            strSupplierName = frm["ddlSupplierName"];
            strInvIds = frm["InvIds"];
            strtoperiod = frm["toperiod"];

            Session["GSTIN"] = strGSTINNo;
            Session["action"] = strAction;
            ViewBag.Period = strPeriod;
            ViewBag.ToPeriod = strtoperiod;
            ViewBag.ActionType = strPeriod;

            String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            String too = DateTime.ParseExact(strtoperiod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            DateTime from_dt = Convert.ToDateTime(fromm);
            DateTime to_dt = Convert.ToDateTime(too);

            String diff = (to_dt - from_dt).TotalDays.ToString();
            int diff_dt = Convert.ToInt32(diff);
            if (diff_dt < 0)
            {
                TempData["Message"] = "To Date should always be greater than or equal to from Date";
            }

            //Accept Invoice
            if (!string.IsNullOrEmpty(Accept))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6A Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";
                }
            }
            //Reject Invoices
            else if (!string.IsNullOrEmpty(Reject))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "R"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6A Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Rejected Successfully";
                }
            }

            //Pending Invoices
            else if (!string.IsNullOrEmpty(Pending))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "P"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6A Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies marked as 'Pending' Successfully";
                }
            }

            //Trigger Invoice
            else if (!string.IsNullOrEmpty(Trigger))
            {
                string[] sup_Gstin;
                string GSTINNO = string.Empty;
                string INVNO = string.Empty;
                string strEmail = string.Empty;
                string strMobileNo = string.Empty;
                string InvDate = string.Empty;
                string InvVal = string.Empty;
                string gstin_2 = string.Empty;
                string UserEmail = string.Empty;
                if (RefIds != null)
                {
                    foreach (var id in RefIds)
                    {
                        string ids = id.ToString();
                        sup_Gstin = ids.Split(',');
                        GSTINNO = sup_Gstin[1].ToString();
                        INVNO = sup_Gstin[2].ToString();
                        InvDate = sup_Gstin[3].ToString();
                        InvVal = sup_Gstin[4].ToString();
                        gstin_2 = sup_Gstin[5].ToString();
                        strEmail = (from lst in db.TBL_Supplier
                                    where lst.GSTINno == GSTINNO
                                    select lst.EmailId).FirstOrDefault();
                        strMobileNo = (from lst in db.TBL_Supplier
                                       where lst.GSTINno == GSTINNO
                                       select lst.MobileNo).FirstOrDefault();
                        UserEmail = (from lst in db.UserLists
                                     where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                     select lst.Email).FirstOrDefault();
                        string comp = Session["CompanyName"].ToString();
                        string mob = Session["MobileNo"].ToString();
                        string name = Session["Name"].ToString();

                        if (strEmail != null && strMobileNo != null)
                        {
                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Missing Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + InvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                            Notification.SendSMS(strMobileNo, string.Format("Please Review the Missing Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", Invoice Value : " + InvVal));
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6A Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Selected Invocies Triggered Successfully";
                        }
                        else
                        {
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6A Invocies Not triggered because Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";

                        }

                    }
                }
            }
            else if (!string.IsNullOrEmpty(Export))
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["ExportData"] as DataTable;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Missing_in_GSTR6A_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(custid);
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                return View();
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }

            if (string.IsNullOrEmpty(strSupplierName))
            {
                strSupplierName = "ALL";
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
            }
            ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);


            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["ToPeriod"] = strtoperiod;

            if (!string.IsNullOrEmpty(strAction))
            {
                ViewBag.MissinginGSTR6A = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction,strtoperiod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMissingInvoiceinGSTR6A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                if (dt.Rows.Count > 0)
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Missing in GSTR6A Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                }
                TempData["ExportData"] = dt;
            }
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            return View();
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MissinginGSTR6()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {
                    if (Session["CF_GSTIN_GSTR6"] != null)
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                         strGStin = Session["CF_GSTIN_GSTR6"].ToString();
                     }
                    else
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                    }
                    if (Session["CF_PERIOD_GSTR6"] != null)
                    {
                        ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                        fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                    }
                    if (Session["CF_TOPERIOD_GSTR6"] != null)
                    {
                        ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                    }
                    if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                    {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString(), Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(custid, userid, Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(custid, userid);
                        }
                    }

                Session["action"] = "B2B";
                ViewBag.MissinginGSTR6 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMissingInvoiceinGSTR6(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);

                TempData["ExportData"] = dt;
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MissinginGSTR6(FormCollection frm, string Accept, string Reject, string Pending, string Trigger, string Export, string[] RefIds)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strInvIds = "", strtoperiod = "";

            strGSTINNo = frm["ddlGSTINNo"];
            strCTIN = frm["ddlSupGSTIN"];
            strPeriod = frm["period"];
            strAction = frm["ddlActionType"];
            strSupplierName = frm["ddlSupplierName"];
            strInvIds = frm["InvIds"];
            strtoperiod = frm["toperiod"];

            Session["GSTIN"] = strGSTINNo;
            Session["action"] = strAction;
            ViewBag.Period = strPeriod;
            ViewBag.ToPeriod = strtoperiod;

            String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            String too = DateTime.ParseExact(strtoperiod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            DateTime from_dt = Convert.ToDateTime(fromm);
            DateTime to_dt = Convert.ToDateTime(too);

            String diff = (to_dt - from_dt).TotalDays.ToString();
            int diff_dt = Convert.ToInt32(diff);
            if (diff_dt < 0)
            {
                TempData["Message"] = "To Date should always be greater than or equal to from Date";
            }

            //Accept invoices
            if (!string.IsNullOrEmpty(Accept))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "A"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6 Invocies Accepted Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invocies Accepted Successfully";
                }
            }
            //Reject Invoices
            else if (!string.IsNullOrEmpty(Reject))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "R"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6 Invocies Rejected Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices Rejected Successfully";
                }
            }
            //Pending Invoices
            else if (!string.IsNullOrEmpty(Pending))
            {
                string[] invid;
                invid = strInvIds.TrimStart(',').TrimEnd(',').Split(',');
                if (RefIds != null)
                {
                    foreach (var id in invid)
                    {
                        string ids = id;
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_MissingInGSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "P"));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6 Invocies Kept Pending Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices marked as 'Pending' Successfully";
                }
            }
            else if (!string.IsNullOrEmpty(Trigger))
            {
                string[] sup_Gstin;
                string GSTINNO = string.Empty;
                string INVNO = string.Empty;
                string strEmail = string.Empty;
                string strMobileNo = string.Empty;
                string InvDate = string.Empty;
                string InvVal = string.Empty;
                string gstin_2 = string.Empty;
                string UserEmail = string.Empty;
                if (RefIds != null)
                {
                    foreach (var id in RefIds)
                    {
                        string ids = id.ToString();
                        sup_Gstin = ids.Split(',');
                        GSTINNO = sup_Gstin[1].ToString();
                        INVNO = sup_Gstin[2].ToString();
                        InvDate = sup_Gstin[3].ToString();
                        InvVal = sup_Gstin[4].ToString();
                        gstin_2 = sup_Gstin[5].ToString();

                        strEmail = (from lst in db.TBL_Supplier
                                    where lst.GSTINno == GSTINNO
                                    select lst.EmailId).FirstOrDefault();
                        strMobileNo = (from lst in db.TBL_Supplier
                                       where lst.GSTINno == GSTINNO
                                       select lst.MobileNo).FirstOrDefault();
                        UserEmail = (from lst in db.UserLists
                                     where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                     select lst.Email).FirstOrDefault();
                        string comp = Session["CompanyName"].ToString();
                        string mob = Session["MobileNo"].ToString();
                        string name = Session["Name"].ToString();

                        if (strEmail != null && strMobileNo != null)
                        {
                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Missing Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + InvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                            Notification.SendSMS(strMobileNo, string.Format("Please Review the Missing Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", Invoice Value : " + InvVal));
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6 Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Seletced Invocies Triggered Successfully";
                        }
                        else
                        {
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Missing in GSTR6 Invocies Not triggered because Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                        }

                    }
                }
            }
            else if (!string.IsNullOrEmpty(Export))
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["ExportData"] as DataTable;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Missing_in_GSTR6_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(custid);
                return View();
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            }
            if (string.IsNullOrEmpty(strSupplierName))
            {
                strSupplierName = "ALL";
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");

            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
            }
            ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["ToPeriod"] = strtoperiod;

            if (!string.IsNullOrEmpty(strAction))
            {
                ViewBag.MissinginGSTR6 = new ReconcilationDataModel(custid, userid).GetMissingInvoiceinGSTR6(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMissingInvoiceinGSTR6(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                if (dt.Rows.Count > 0)
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Missing in GSTR6 Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                }
                TempData["ExportData"] = dt;
            }
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            return View();
        }

        [System.Web.Mvc.HttpGet]
        [System.Web.Mvc.AcceptVerbs(HttpVerbs.Get)]
        public ActionResult MismatchInvoices()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string fromPeriod = "", toPeriod = "", strGStin = "ALL", cName = "ALL", Ctin = "ALL";
            ViewBag.Period = DateTime.Now.ToString("MMyyyy");
            ViewBag.ToPeriod = DateTime.Now.ToString("MMyyyy");
            fromPeriod = DateTime.Now.ToString("MMyyyy");
            toPeriod = DateTime.Now.ToString("MMyyyy");
            try
            {

                    if (Session["CF_GSTIN_GSTR6"] != null)
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, Session["CF_GSTIN_GSTR6"].ToString(), Session["Role_Name"].ToString());
                         strGStin = Session["CF_GSTIN_GSTR6"].ToString();
                     }
                    else
                    {
                        ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                    }
                    if (Session["CF_PERIOD_GSTR6"] != null)
                    {
                        ViewBag.Period = Session["CF_PERIOD_GSTR6"].ToString();
                        fromPeriod = Session["CF_PERIOD_GSTR6"].ToString();
                      }
                    if (Session["CF_TOPERIOD_GSTR6"] != null)
                    {
                        ViewBag.ToPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                        toPeriod = Session["CF_TOPERIOD_GSTR6"].ToString();
                    }
                    if (Session["CF_SUPNAME_GSTR6"] != null && Session["CF_SUPNAME_GSTR6"].ToString() != "ALL")
                    {
                    cName = Session["CF_SUPNAME_GSTR6"].ToString();
                    ViewBag.SupplierName = LoadDropDowns.Exist_SupplierName(custid, Session["CF_SUPNAME_GSTR6"].ToString());

                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString(), Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.SupplierName_GSTIN(custid, Session["CF_SUPNAME_GSTR6"].ToString());
                        }
                    }
                    else
                    {
                        ViewBag.SupplierName = LoadDropDowns.SupplierName(custid);
                        if (Session["CF_CTIN_GSTR6"] != null)
                        {
                        Ctin = Session["CF_CTIN_GSTR6"].ToString();
                        ViewBag.CTINNoList = LoadDropDowns.Exist_GetGSTR6_6A_CTIN(custid, userid, Session["CF_CTIN_GSTR6"].ToString());
                        }
                        else
                        {
                            ViewBag.CTINNoList = LoadDropDowns.GetGSTR6_6A_CTIN(custid, userid);
                        }
                    }

                Session["action"] = "B2B";
                ViewBag.MismatchIncoices = new ReconcilationDataModel(custid, userid).GetMismatchInvoiceinGSTR6andGSTR6A(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMismatchInvoiceinGSTR6andGSTR6A(strGStin, cName, Ctin, fromPeriod, "B2B", toPeriod);

                TempData["ExportData"] = dt;
                SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", "B2B");
                ViewBag.ActionList = Actionlst;

            }
            catch (Exception ex)
            {
                // Info
                Console.Write(ex);
            }
            return this.View();
        }

        public ActionResult MismatchInvoices(FormCollection frm, string Accept, string Reject, string Pending, string Trigger, string Export, string Modify, string[] InvID)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string strGSTINNo = "", strPeriod = "", strSupplierName = "", strCTIN = "", strAction = "", strtoperiod = "";

            strGSTINNo = frm["ddlGSTINNo"];
            strCTIN = frm["ddlSupGSTIN"];
            strPeriod = frm["period"];
            strSupplierName = frm["ddlSupplierName"];
            strAction = frm["ddlActionType"];
            strtoperiod = frm["toperiod"];

            Session["GSTIN"] = strGSTINNo;
            Session["action"] = strAction;
            ViewBag.Period = strPeriod;
            ViewBag.ToPeriod = strtoperiod;

            String fromm = DateTime.ParseExact(strPeriod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            String too = DateTime.ParseExact(strtoperiod.ToString(), "MMyyyy", CultureInfo.InvariantCulture).ToString("dd'/'MM'/'yyyy");
            DateTime from_dt = Convert.ToDateTime(fromm);
            DateTime to_dt = Convert.ToDateTime(too);

            String diff = (to_dt - from_dt).TotalDays.ToString();
            int diff_dt = Convert.ToInt32(diff);
            if (diff_dt < 0)
            {
                TempData["Message"] = "To Date should always be greater than or equal to from Date";
            }

            //Accept Invoice
            if (!string.IsNullOrEmpty(Accept))
            {
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        //
                    }
                }
                TempData["ReconcilationResponse"] = "Invoice Accepted Successfully";

            }
            //Reject Invoice
            else if (!string.IsNullOrEmpty(Reject))
            {
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        //
                    }
                }
                TempData["ReconcilationResponse"] = "Invoice Rejected Successfully";

            }

            //Modify Invoice
            else if (!string.IsNullOrEmpty(Modify))
            {
                string[] invid;
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Mismatched_Invoices_GSTR6", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR6invid", Convert.ToInt32(invid[0])));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR6Ainvid", Convert.ToInt32(invid[6])));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }

                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices modified successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");

                    TempData["ReconcilationResponse"] = "Selected Invoices Modified Successfully";
                }

            }

            //Pending Invoices
            else if (!string.IsNullOrEmpty(Pending))
            {
                string[] invid;
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        string ids = id;
                        invid = ids.Split(',');
                        con.Open();
                        SqlCommand dCmd = new SqlCommand("usp_Update_Invoices_Mismatch_GSTR6and6A", con);
                        dCmd.Parameters.Clear();
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                        dCmd.Parameters.Add(new SqlParameter("@Activity", "P"));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR6invId", Convert.ToInt32(invid[0])));
                        dCmd.Parameters.Add(new SqlParameter("@GSTR6AinvId", Convert.ToInt32(invid[6])));
                        dCmd.Parameters.Add(new SqlParameter("@UserId", userid));
                        dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                        dCmd.ExecuteNonQuery();
                        con.Close();
                    }
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected  Mismatched Invoices Kept Pending successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                    TempData["ReconcilationResponse"] = "Selected Invoices marked as 'Pending' Successfully";
                }
            }

            //Trigger Invoice
            else if (!string.IsNullOrEmpty(Trigger))
            {
                string[] sup_Gstin;
                string GSTINNO = string.Empty;
                string INVNO = string.Empty;
                string strEmail = string.Empty;
                string strMobileNo = string.Empty;
                string InvDate = string.Empty;
                string GSTR6InvVal = string.Empty;
                string GSTR6AInvVal = string.Empty;
                string gstin_2 = string.Empty;
                string UserEmail = string.Empty;
                if (InvID != null)
                {
                    foreach (var id in InvID)
                    {
                        string ids = id.ToString();
                        sup_Gstin = ids.Split(',');
                        GSTINNO = sup_Gstin[1].ToString();
                        INVNO = sup_Gstin[2].ToString();
                        InvDate = sup_Gstin[3].ToString();
                        GSTR6InvVal = sup_Gstin[4].ToString();
                        GSTR6AInvVal = sup_Gstin[5].ToString();
                        gstin_2 = sup_Gstin[7].ToString();

                        strEmail = (from lst in db.TBL_Supplier
                                    where lst.GSTINno == GSTINNO
                                    select lst.EmailId).FirstOrDefault();
                        strMobileNo = (from lst in db.TBL_Supplier
                                       where lst.GSTINno == GSTINNO
                                       select lst.MobileNo).FirstOrDefault();
                        UserEmail = (from lst in db.UserLists
                                     where lst.UserId == userid && lst.CustId == custid && lst.rowstatus == true
                                     select lst.Email).FirstOrDefault();
                        string comp = Session["CompanyName"].ToString();
                        string mob = Session["MobileNo"].ToString();
                        string name = Session["Name"].ToString();

                        if (strEmail != null && strMobileNo != null)
                        {
                            Notification.SendEmail_Reconciliation(strEmail, UserEmail, string.Format(" Please Review the Mismatched Invoices For this GSTIN :" + gstin_2 + "."), string.Format("We on behalf of " + comp + " having GSTIN :" + gstin_2 + " would like to inform you that, we have observed the following discrepancies of supplies made or services provided to them.<br /><br /> <table border='1' cellpadding='0' cellspacing='0'><tr><th>CTIN</th><th>Invoice No</th><th>Invoice Date</th><th>GSTR6 Invoice Value</th><th>GSTR6A Invoice Value</th><th>Contact Details</th></tr><tr><td>" + GSTINNO + "</td><td>" + INVNO + "</td><td>" + InvDate + "</td><td>" + GSTR6InvVal + "</td><td>" + GSTR6AInvVal + "</td><td>" + name + " - " + mob + "</td></tr></table>"));
                            Notification.SendSMS(strMobileNo, string.Format("Please Review the Mismatched Invoices For this GSTIN :" + gstin_2 + ". Invoice No : " + INVNO + ", CTIN : " + GSTINNO + ", Invoice Date : " + InvDate + ", GSTR6 Invoice Value : " + GSTR6InvVal + ", GSTR6A Invoice Value : " + GSTR6AInvVal));
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Selected Invoices Triggered Successfully";
                        }
                        else
                        {
                            Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Selected Mismatched Invocies Triggered Successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                            TempData["ReconcilationResponse"] = "Email Id or Mobile No not updated for selected Supplier. Please Update and Trigger...";
                        }

                    }
                }
            }
            else if (!string.IsNullOrEmpty(Export))
            {
                GridView gv = new GridView();
                gv.DataSource = TempData["ExportData"] as DataTable;
                gv.DataBind();
                Response.ClearContent();
                Response.Buffer = true;
                Response.AddHeader("content-disposition", "attachment; filename=Mismatch_Invoices_" + strGSTINNo + "_" + strAction + "_" + strPeriod + "-" + strtoperiod + ".xls");
                Response.ContentType = "application/ms-excel";
                Response.Charset = "";
                StringWriter sw = new StringWriter();
                HtmlTextWriter htw = new HtmlTextWriter(sw);
                gv.RenderControl(htw);
                Response.Output.Write(sw.ToString());
                Response.Flush();
                Response.End();
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.CTINNoList = LoadDropDowns.SupplierGSTIN(custid);
                return View();
            }
            else
            {
                //
            }

            if (string.IsNullOrEmpty(strGSTINNo))
            {
                strGSTINNo = "ALL";
                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            }
            if (string.IsNullOrEmpty(strSupplierName))
            {
                strSupplierName = "ALL";
                ViewBag.SupplierName = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name");
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.SupplierName = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, "", "Supplier_Name", strSupplierName);
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }

            if (string.IsNullOrEmpty(strCTIN))
            {
                strCTIN = "ALL";
                ViewBag.CTINNoList = LoadDropDowns.Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "");
            }
            else
            {
                ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);
            }
            ViewBag.CTINNoList = LoadDropDowns.Existing_Get_Supplier_Name_CTIN_List(custid, userid, strGSTINNo, strPeriod, strtoperiod, strSupplierName, "", strCTIN);

            TempData["ActionType"] = strAction;
            TempData["GSTIN"] = strGSTINNo;
            TempData["SupplierName"] = strSupplierName;
            TempData["CTIN"] = strCTIN;
            TempData["Period"] = strPeriod;
            TempData["ToPeriod"] = strtoperiod;

            if (!string.IsNullOrEmpty(strAction))
            {
                ViewBag.MismatchIncoices = new ReconcilationDataModel(custid, userid).GetMismatchInvoiceinGSTR6andGSTR6A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                var dt = new ReconcilationDataModel(custid, userid).DT_GetMismatchInvoiceinGSTR6andGSTR6A(strGSTINNo, strSupplierName, strCTIN, strPeriod, strAction, strtoperiod);
                if (dt.Rows.Count > 0)
                {
                    Helper.InsertAuditLog(Convert.ToString(Session["User_ID"]), Convert.ToString(Session["UserName"]), "Mismatched Invoices data exported successfully for the GSTIN : " + strGSTINNo + " and Period : " + strPeriod, "");
                }
                TempData["ExportData"] = dt;
            }
            SelectList Actionlst = new SelectList(db.TBL_GSTR_ACTION_TYPE.Where(o => o.GSTRType == "RECONCILE"), "ActionName", "ActionName", Convert.ToString(strAction));
            ViewBag.ActionList = Actionlst;
            ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
            return View();
        }
    }
}