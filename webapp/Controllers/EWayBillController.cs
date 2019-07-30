using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models.EWAY;
using WeP_EWayBill;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Threading;
using System.Threading.Tasks;
using WeP_BAL;
using WeP_BAL.EwayBill;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class EWayBillController : Controller
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        
        // GET: EWayBill

        #region "VIEW & PRINT OF EWb AND CONSEWB"
        [HttpGet]
        public ActionResult Home()
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

                ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection frm)
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

                string strGSTINNo = "", strFromDate = "", strToDate = "", strLocationId = "";

                strGSTINNo = frm["ddlGSTINNo"];
                strLocationId = frm["ddlLocation"];
                if(string.IsNullOrEmpty(strLocationId))
                {
                    strLocationId = "0";
                }
                strFromDate = frm["fromDate"];
                strToDate = frm["toDate"];
                ViewBag.FromDate = strFromDate;
                ViewBag.ToDate = strToDate;

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.GstinNo = strGSTINNo;
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                    if (Convert.ToInt32(strLocationId) != 0)
                    {
                        ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(strLocationId));
                    }
                }
                ViewBag.EWBList = EwaybillDataAccess.GetEWBList(strGSTINNo, strFromDate, strToDate, custid, Convert.ToInt32(strLocationId), "PD");
                ViewBag.CONSEWBList = EwaybillDataAccess.GetCONSEWBList(strGSTINNo, strFromDate, strToDate, custid, Convert.ToInt32(strLocationId), "PD");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "INVOICE DATA EWAYBILL GENERATION LIST"
        [HttpGet]
        public ActionResult InvoiceData()
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

                ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult InvoiceData(FormCollection frm, string btnEWBGEN, string btnEWBPopup)
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

                string strGSTINNo = "", strRefIds = "", strFromDate = "", strToDate = "", strLocationId = "";

                string strTransMode = "", strTransDocNo = "", strTransDocDate = "", strTransName = "", strTransId = "", strVehicleNo = "", strVehicleType = "";

                strGSTINNo = frm["ddlGSTINNo"];
                strLocationId = frm["ddlLocation"];
                if (string.IsNullOrEmpty(strLocationId))
                {
                    strLocationId = "0";
                }
                strFromDate = frm["fromDate"];
                strToDate = frm["toDate"];
                strRefIds = frm["EwbIds"];
                strRefIds = strRefIds.TrimStart(',').TrimEnd(',');
                ViewBag.EWBIds = strRefIds;
                strTransMode = frm["transmode"];
                strTransName = frm["transname"];
                strTransId = frm["transid"];
                strTransDocNo = frm["transdocno"];
                strTransDocDate = frm["transdocdate"];                
                strVehicleNo = frm["vehicleno"];
                strVehicleType = frm["vehicletype"];
                Session["vehicleType"] = strVehicleType;

                ViewBag.FromDate = strFromDate;
                ViewBag.ToDate = strToDate;

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.GstinNo = strGSTINNo;
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                    if (Convert.ToInt32(strLocationId) != 0)
                    {
                        ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(strLocationId));
                    }
                }
                ViewBag.EWBInvoiceDataList = EwaybillDataAccess.GetEWBList(strGSTINNo, strFromDate, strToDate, custid, Convert.ToInt32(strLocationId), "ID");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", strTransMode);                

                #region "GENERATE BULK EWAYBILL"
                if (!string.IsNullOrEmpty(btnEWBGEN))
                {
                    UpdateEWBPartBDetails(strTransMode, strTransId, strTransName, strTransDocNo, strTransDocDate, strVehicleNo, strVehicleType, Convert.ToString(custid), strRefIds);
                    
                    Task.Factory.StartNew(() => EwbGeneration.EWAYBILL_GEN_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "EWayBill Generation is in progress... Please check after sometime";
                }
                #endregion

                #region "EWB POPUP"
                else if (!string.IsNullOrEmpty(btnEWBPopup))
                {
                    ViewBag.EWBPOPUP = "OPEN_POPUP";
                }
                #endregion
                                
                #region "ONCHANGE EVENT"                
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public void UpdateEWBPartBDetails(string strTransMode, string strTransId, string strTransName, string strTransDocNo, string strTransDocDate,
            string strVehicleNo, string strVehicleType, string strCustId, string strEWBIds)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    conn.Open();
                    SqlCommand cmd = new SqlCommand("usp_Update_EWB_GEN_PARTB_Details", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@transMode", strTransMode));
                    cmd.Parameters.Add(new SqlParameter("@transporterId", strTransId));
                    cmd.Parameters.Add(new SqlParameter("@transporterName", strTransName));
                    cmd.Parameters.Add(new SqlParameter("@transDocNo", strTransDocNo));
                    cmd.Parameters.Add(new SqlParameter("@transDocDate", strTransDocDate));
                    cmd.Parameters.Add(new SqlParameter("@vehicleNo", strVehicleNo));
                    cmd.Parameters.Add(new SqlParameter("@vehicleType", strVehicleType));
                    cmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                    cmd.Parameters.Add(new SqlParameter("@EWBIds", strEWBIds));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                con.Close();
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion


        #region "EWAYBILL GENERATION LIST"
        [HttpGet]
        public ActionResult EWB()
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

                ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.TransDocDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.StateCode = Models.Common.LoadDropDowns.DropDownEwayBill("StateCode");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.DropDownEwayBill("TransportMode");
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EWB(FormCollection frm, string btnEWBGEN, string btnCONSEWBGEN, string btnCONSEWBPopup)
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

                string strGSTINNo = "", strRefIds = "", strFromDate = "", strToDate = "", strLocationId = "";
                
                string strTransMode = "", strTransDocNo = "", strTransDocDate = "", strFromPlace = "", strFromState = "", strVehicleNo = "", strEwbNo = "";
                string strStatus = "", strResponse = "", strcEWBNo = "";

                strGSTINNo = frm["ddlGSTINNo"];
                Session["gstin"] = strGSTINNo;
                strLocationId = frm["ddlLocation"];
                if(string.IsNullOrEmpty(strLocationId))
                {
                    strLocationId = "0";
                }
                strFromDate = frm["fromDate"];
                strToDate = frm["toDate"];
                strRefIds = frm["EwbIds"];
                strRefIds = strRefIds.TrimStart(',').TrimEnd(',');
                ViewBag.EWBIds = strRefIds;
                strTransMode = frm["transmode"];
                strTransDocNo = frm["transdocno"];
                strTransDocDate = frm["transdocdate"];
                ViewBag.TransDocDate = strTransDocDate;
                strFromPlace = frm["fromplace"];
                strFromState = frm["fromstate"];
                strVehicleNo = frm["vehicleno"];

                ViewBag.FromDate = strFromDate;
                ViewBag.ToDate = strToDate;

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());                
                ViewBag.GstinNo = strGSTINNo;
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                    if (Convert.ToInt32(strLocationId) != 0)
                    {
                        ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(strLocationId));
                    }
                }
                ViewBag.EWBList = EwaybillDataAccess.GetEWBList(strGSTINNo, strFromDate, strToDate, custid, Convert.ToInt32(strLocationId), "EG");
                ViewBag.TransportMode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("TransportMode", strTransMode);
                ViewBag.StateCode = Models.Common.LoadDropDowns.Exist_DropDownEwayBill("StateCode", strFromState);

                #region "GENERATE BULK EWAYBILL"
                if (!string.IsNullOrEmpty(btnEWBGEN))
                {
                    Task.Factory.StartNew(() => EwbGeneration.EWAYBILL_GEN_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "EWayBill Generation is in progress... Please check after sometime";
                }
                #endregion

                #region "CONSEWB POPUP"
                else if (!string.IsNullOrEmpty(btnCONSEWBPopup))
                {
                    ViewBag.CONSEWBPOPUP = "OPEN_POPUP";
                }
                #endregion

                #region "GENERATE BULK CONSOLIDATED EWAYBILL"
                else if (!string.IsNullOrEmpty(btnCONSEWBGEN))
                {
                    string[] strEWBIDS = strRefIds.Split(',');
                    for (int i = 0; i < strEWBIDS.Count(); i++)
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            if(conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            using (SqlCommand sqlcmd = new SqlCommand())
                            {
                                sqlcmd.Connection = conn;
                                sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION where ewbid = @EWBId";
                                sqlcmd.Parameters.AddWithValue("@EWBId", strEWBIDS[i]);
                                using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                                {
                                    DataTable dt = new DataTable();
                                    adt.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        //..
                                    }
                                    else
                                    {
                                        TempData["ErrorMessage"] = "One of the EwayBill is not Generated. So Genearte the EwayBill and do Consolidated.";
                                        return View();
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }

                    for (int j = 0; j < strEWBIDS.Count(); j++)
                    {
                        using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                        {
                            if (conn.State == ConnectionState.Closed)
                            {
                                conn.Open();
                            }
                            using (SqlCommand sqlcmd = new SqlCommand())
                            {
                                sqlcmd.Connection = conn;
                                sqlcmd.CommandText = "Select TOP 1 * from TBL_EWB_GENERATION where ewbid = @EWBId";
                                sqlcmd.Parameters.AddWithValue("@EWBId", strEWBIDS[j]);
                                using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                                {
                                    DataTable dt = new DataTable();
                                    adt.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        strEwbNo = dt.Rows[0]["ewayBillNo"].ToString();
                                        EwaybillDataAccess.cewb_insert(strGSTINNo, strTransMode, strTransDocNo, strTransDocDate, strFromPlace, strFromState, strVehicleNo, strEwbNo, Session["CustRefNo"].ToString(), Session["User_ID"].ToString(), 0);
                                    }
                                }
                            }
                            conn.Close();
                        }
                    }
                    string strJsonData = EwbJsonDataModel.GetJsonCONSEWBGeneration(strGSTINNo, strTransDocNo, strTransDocDate);
                    strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                    EwbGeneration.CONS_EWB_GEN(strTransDocNo, strTransDocDate, strJsonData, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out strStatus, out strResponse, out strcEWBNo);
                                        
                    if(strStatus == "0")
                    {
                        TempData["ErrorMessage"] = strResponse;
                    }
                    else
                    {
                        TempData["SuccessMessage"] = strResponse;
                    }                    
                }
                #endregion

                #region "ONCHANGE EVENT"                
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "CONSOLIDATED EWAYBILL GENERATION LIST"
        [HttpGet]
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

                ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult CONSEWB(FormCollection frm, string btnCONSEWBGEN)
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

                string strGSTINNo = "", strRefIds = "", strFromDate = "", strToDate = "", strLocationId = "";
                strGSTINNo = frm["ddlGSTINNo"];
                strLocationId = frm["ddlLocation"];
                if(string.IsNullOrEmpty(strLocationId))
                {
                    strLocationId = "0";
                }
                strRefIds = frm["CONSEwbIds"];
                strRefIds = strRefIds.TrimStart(',').TrimEnd(',');

                ViewBag.FromDate = strFromDate;
                ViewBag.ToDate = strToDate;

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.GstinNo = strGSTINNo;
                if (strGSTINNo != null)
                {
                    int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTINNo);
                    ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchList(userid, custid, GstinId, Session["Role_Name"].ToString());
                    if (Convert.ToInt32(strLocationId) != 0)
                    {
                        ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchList(userid, custid, GstinId, Session["Role_Name"].ToString(), Convert.ToInt32(strLocationId));
                    }
                }
                ViewBag.CONSEWBList = EwaybillDataAccess.GetCONSEWBList(strGSTINNo, "", "", custid, Convert.ToInt32(strLocationId), "EG");

                #region "GENERATE BULK CONSOLIDATED EWAYBILL"
                if (!string.IsNullOrEmpty(btnCONSEWBGEN))
                {
                    Task.Factory.StartNew(() => EwbGeneration.CONS_EWAYBILL_GEN_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "Consolidated EWayBill Generation is in progress... Please check after sometime";
                }
                #endregion

                #region "ONCHANGE EVENT"                
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion                
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "UPDATE VEHICLENO LIST"
        [HttpGet]
        public ActionResult UPDVEHNO()
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

                ViewBag.FromDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.ToDate = DateTime.Now.ToString("dd/MM/yyyy");

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult UPDVEHNO(FormCollection frm, string btnUPDVEHNO)
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

                string strGSTINNo = "", strRefIds = "";
                strGSTINNo = frm["ddlGSTINNo"];
                strRefIds = frm["UPDVEHNOIds"];
                strRefIds = strRefIds.TrimStart(',').TrimEnd(',');

                #region "GENERATE BULK CONSOLIDATED EWAYBILL"
                if (!string.IsNullOrEmpty(btnUPDVEHNO))
                {
                    Task.Factory.StartNew(() => EwbGeneration.EWB_UPDATE_VEHICLENO_THREAD(strRefIds, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    TempData["SuccessMessage"] = "VehicleNo Updation is in progress... Please check after sometime";
                }
                #endregion

                #region "ONCHANGE EVENT"                
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion

                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.UPDVEHNOList = EwaybillDataAccess.GetUPDVEHNOList(strGSTINNo, custid, "EG");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "EWB CANCEL"
        public ActionResult Cancel()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            Session["EWayType"] = "";
            ViewBag.ReasonList = Models.Common.LoadDropDowns.GetEWBReasonList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            return View();
        }

        [HttpPost]
        public ActionResult Cancel(FormCollection Form, string btnCancel)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string OStatus = "";
            string OResponse = "";
            try
            {
                string strBillNo = Form["billno"];
                string strReason = Form["ddlReasonCode"];
                string strRemarks = Form["remarks"];
                string strGSTINNo = Form["ddlGSTINNo"];
                Session["EWayType"] = Form["Type"];
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.ReasonList = Models.Common.LoadDropDowns.GetEWBReasonList();
                var CancelList = EwaybillDataAccess.Retrieve_EWAYBILL_CancelData(strGSTINNo, Convert.ToInt32(Session["Cust_ID"]));
                ViewBag.CancelList = CancelList;
                if (!string.IsNullOrEmpty(btnCancel))
                {
                    string strJsonData = "{\"ewbNo\":" + strBillNo + ",\"cancelRsnCode\": " + strReason + ",\"cancelRmrk\":\"" + strRemarks + "\"}";
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        conn.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = conn;
                            sqlcmd.CommandText = "Select * from TBL_EWB_GENERATION where ewayBillNo = @EwbNo";
                            sqlcmd.Parameters.AddWithValue("@EwbNo", strBillNo);
                            using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable ds = new DataTable();
                                da.Fill(ds);
                                if (ds.Rows.Count == 0)
                                {
                                    EwbGetApi.GET_EWAYBILL(strBillNo, strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                                 Session["UserName"].ToString(), out OStatus, out OResponse);
                                }
                            }
                        }
                        conn.Close();
                    }
                    new EwbCancelReject(Session["Cust_ID"].ToString(), Session["User_ID"].ToString(), Session["UserName"].ToString()).EWB_CANCEL(strBillNo, strReason, strRemarks, strJsonData, strGSTINNo,                        
                        out OStatus, out OResponse);

                    TempData["ViewResponse"] = OResponse;
                }
                #region "ONCHANGE EVENT"
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "EWB REJECT"
        public ActionResult Reject()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

            return View();
        }

        [HttpPost]
        public ActionResult Reject(FormCollection Form, string btnGetReject, string btnGet, string btnReject)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string OStatus = "";
            string OResponse = "";
            string EWAYDate = "";
            try
            {
                string strDate = Form["date"];
                string strGSTINNo = Form["ddlGSTINNo"];
                string strewbNo = Form["ewbNo"];
                ViewBag.Period = strDate;
                strewbNo = strewbNo.TrimStart(',').TrimEnd(',');
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                if (!string.IsNullOrEmpty(btnGetReject))
                {
                    Thread.Sleep(1000);
                    Task.Factory.StartNew(() => EwbCancelReject.EWAYBILL_Reject_THREAD(strewbNo, strGSTINNo, Session["User_ID"].ToString(),
                        Session["Cust_ID"].ToString(), Session["UserName"].ToString())
                    );
                    Thread.Sleep(1000);
                    var List = EwaybillDataAccess.Retrieve_EWAYBILL_BY_Other(strGSTINNo, strDate, "", "", "", "OTHERSEWB", custid);
                    ViewBag.GetReject = List;
                    TempData["ViewResponse"] = "EWayBill(S) Reject is in progress... Please check after sometime";
                }

                if (!string.IsNullOrEmpty(btnGet))
                {
                    EwbGetApi.GET_EWAYBILL_BY_Other(strDate, strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                            Session["UserName"].ToString(), out OStatus, out OResponse, out EWAYDate);

                    var List = EwaybillDataAccess.Retrieve_EWAYBILL_BY_Other(strGSTINNo, strDate, "", "", "", "OTHERSEWB", custid);
                    ViewBag.GetReject = List;
                }

                #region "ONCHANGE EVENT"            
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion


        #region "GET EWB"
        public ActionResult GetEWB()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            Session["EWayType"] = "";
            ViewBag.GetEwayList = Models.Common.LoadDropDowns.GetEWBList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

            return View();
        }
        [HttpPost]
        public ActionResult GetEWB(FormCollection Form, string btnSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string OResponse = "";

            try
            {
                string EWAYType = Form["ddlEWAY"];
                string strGSTINNo = Form["ddlGSTINNo"];
                Session["EWayType"] = Form["ddlEWAY"];

                ViewBag.GetEwayList = Models.Common.LoadDropDowns.Exist_GetEWBList(EWAYType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                if (!string.IsNullOrEmpty(btnSubmit))
                {
                    if (EWAYType == "6")
                    {
                        EwbGetApi.GET_ErrorList(strGSTINNo);
                        TempData["Date"] = "";
                        TempData["Gstin"] = "";
                    }

                    TempData["ViewResponse"] = OResponse;
                }
                #region "ONCHANGE EVENT"            
                else
                {
                    EwbGeneratingKeys.Autentication(strGSTINNo);
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }
        #endregion



        [HttpPost]
        public JsonResult ewbNoList(string Prefix, string Gstin)
        {
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            DataSet DS = EwaybillDataAccess.getewbNoList(Gstin, Prefix, CustId);

            List<EwayAttributes.EwaybillMgmt> ewbNoMgmt = new List<EwayAttributes.EwaybillMgmt>();

            #region "ewbno for Auto Populate"

            foreach (DataRow dr in DS.Tables[0].Rows)
            {
                ewbNoMgmt.Add(new EwayAttributes.EwaybillMgmt
                {
                    ewbNo = dr["ewayBillNo"].ToString()
                });

            }

            #endregion

            return Json(ewbNoMgmt, JsonRequestBehavior.AllowGet);
        }

    }
}