
using SmartAdminMvc.Models.GSTR9API;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_DAL.GSTR9Attribute;

namespace SmartAdminMvc.Controllers
{
    public class Gstr9Controller : Controller
    {
        // GET: GSTR9
        #region"Get GSTR9 Home"
        public ActionResult GSTR9Home()
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
            if(Session["strGSTIN"]!=null|| Session["FinancialYear"]!=null)
            {
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, Convert.ToString(Session["strGSTIN"]), Session["Role_Name"].ToString());
                ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(Convert.ToString(Session["FinancialYear"]));
 
                Session.Remove("strGSTIN");
                Session.Remove("FinancialYear");           
            }
            else
            {
                ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            }

            return View();
        }
        #endregion

        #region "Get Summary gstr9 Details" 
        [HttpPost]
        public ActionResult GSTR9Home(string gstin,string fp,FormCollection form, string command, string OTPSubmit)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string FromDate, ToDate, strGSTIN;

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            string FinancialYear = form["FinancialYear"];
            Session["FinancialYear"] = FinancialYear;
            strGSTIN = form["strGSTIN"];
            Session["strGSTIN"] = strGSTIN;

            try
            {

                #region"Save and retrive"
                if (!String.IsNullOrEmpty(FinancialYear) && !String.IsNullOrEmpty(command))
                {
                    
                    if (command == "Savegstr9")
                    {
                        Session["Save"] = command;
                        string str = FinancialYear.Substring(0, 4);
                        FromDate = str.Insert(0, "04");
                        Session["Fromdate"] = FromDate;
                        var result = FinancialYear.Substring(FinancialYear.Length - 4);
                        ToDate = result.Insert(0, "03");
                        Session["Todate"] = ToDate;
                        string ReferenceId = "",Error ="";
                        string strJsonData = Models.GSTR9API.Gstr9Json.GetJsonGSTR9(strGSTIN,Session["Todate"].ToString());
                        strJsonData = strJsonData.TrimStart('[').TrimEnd(']');

                        Gstr9SaveApiCall Gstr9Api = new Gstr9SaveApiCall();                      
                        Gstr9Api.SendRequestGSTR9(strJsonData,strGSTIN, Session["Todate"].ToString(),userid, custid,"Gstr9Save",out ReferenceId,out Error);
                        if (ReferenceId != "")
                        {
                            TempData["SaveMessage"] = "Result :  " + ReferenceId + "";
                        }
                        else
                        {
                            TempData["error"] = "Result :  " + Error + "";
                        }
                        Session["Save"] = null;
                    }      
                    
                    else
                    {
                        Session["Save"] = command;
                        string str = FinancialYear.Substring(0, 4);
                        FromDate = str.Insert(0, "04");
                        Session["Fromdate"] = FromDate;
                        var result = FinancialYear.Substring(FinancialYear.Length - 4);
                        ToDate = result.Insert(0, "03");
                        Session["Todate"] = ToDate;

                        ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                        ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);

                        DataSet gstr9 = Gstr9.Gstr9Data(strGSTIN, ToDate);
                        TempData["gstr9"] = gstr9;
                        List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt1 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt2 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt3 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt4 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt5 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List1> OReportMgmt6 = new List<GSTR9Attributes.Gstr9List1>();
                        List<GSTR9Attributes.Gstr9List> OReportMgmt7 = new List<GSTR9Attributes.Gstr9List>();
                        List<GSTR9Attributes.Gstr9List> OReportMgmt8 = new List<GSTR9Attributes.Gstr9List>();
                        List<GSTR9Attributes.Gstr9List> OReportMgmt9 = new List<GSTR9Attributes.Gstr9List>();
                        List<GSTR9Attributes.Gstr9List> OReportMgmt10 = new List<GSTR9Attributes.Gstr9List>();
                        List<GSTR9Attributes.Gstr9List> OReportMgmt11 = new List<GSTR9Attributes.Gstr9List>();

                        #region "Data Assign to Attributes"
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[0].Rows)
                            {
                                OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[1].Rows)
                            {
                                OReportMgmt1.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[2].Rows)
                            {
                                OReportMgmt2.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[3].Rows)
                            {
                                OReportMgmt3.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[4].Rows)
                            {
                                OReportMgmt4.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[5].Rows)
                            {
                                OReportMgmt5.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[6].Rows)
                            {
                                OReportMgmt6.Add(new GSTR9Attributes.Gstr9List1
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[7].Rows)
                            {
                                OReportMgmt7.Add(new GSTR9Attributes.Gstr9List
                                {

                                    taxpayable = dr.IsNull("txpyble") ? 0 : Convert.ToDecimal(dr["txpyble"]),
                                    taxpaidthroughcash = dr.IsNull("txpaid") ? 0 : Convert.ToDecimal(dr["txpaid"]),



                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[8].Rows)
                            {
                                OReportMgmt8.Add(new GSTR9Attributes.Gstr9List
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[9].Rows)
                            {
                                OReportMgmt9.Add(new GSTR9Attributes.Gstr9List
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[10].Rows)
                            {
                                OReportMgmt10.Add(new GSTR9Attributes.Gstr9List
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }
                        if (gstr9.Tables.Count > 0)
                        {
                            foreach (DataRow dr in gstr9.Tables[11].Rows)
                            {
                                OReportMgmt11.Add(new GSTR9Attributes.Gstr9List
                                {

                                    iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                                    camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                                    samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                                    csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),


                                });
                            }
                        }


                        ReportViewModel Model = new ReportViewModel();

                        Model.ReportMgmt = OReportMgmt;
                        ViewBag.Table4 = OReportMgmt.Count;
                        Model.ReportMgmt1 = OReportMgmt1;
                        ViewBag.Table5 = OReportMgmt1.Count;
                        Model.ReportMgmt2 = OReportMgmt2;
                        ViewBag.Table6 = OReportMgmt2.Count;
                        Model.ReportMgmt3 = OReportMgmt3;
                        ViewBag.Table7 = OReportMgmt3.Count;
                        Model.ReportMgmt4 = OReportMgmt4;
                        ViewBag.Table8 = OReportMgmt4.Count;
                        Model.ReportMgmt5 = OReportMgmt5;
                        ViewBag.Table9 = OReportMgmt5.Count;
                        Model.ReportMgmt6 = OReportMgmt6;
                        ViewBag.Table10 = OReportMgmt6.Count;
                        Model.ReportMgmt7 = OReportMgmt7;
                        ViewBag.Table14 = OReportMgmt7.Count;
                        Model.ReportMgmt8 = OReportMgmt8;
                        ViewBag.Table15 = OReportMgmt8.Count;
                        Model.ReportMgmt9 = OReportMgmt9;
                        ViewBag.Table16 = OReportMgmt9.Count;
                        Model.ReportMgmt10 = OReportMgmt10;
                        ViewBag.Table17 = OReportMgmt10.Count;
                        Model.ReportMgmt11 = OReportMgmt11;
                        ViewBag.Table18 = OReportMgmt11.Count;

                        #endregion

                        return View(Model);

                    }
                }

                #endregion

                #region "OTP SUBMIT"
                else if (!string.IsNullOrEmpty(OTPSubmit))
                {
                    string strOTP = form["OTP"].ToString();
                    Models.GSTAUTH.GenerateKeys.GeneratingEncryptedOTPKeys(strOTP, strGSTIN);
                    string status = Models.GSTAUTH.GspGstAuthwithOtp.SendRequest(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString());
                    if (status == "1")
                    {
                        TempData["AuthMsg"] = "Authenticated Successfully";
                    }
                    else
                    {
                        TempData["AuthMsg"] = status;
                    }
                }
                #endregion

                #region "ONCHANGE EVENT"
                else
                {
                    string OTPPOPUPValue = "", OTPAUTHResponse = "";
                    Models.GSTAUTH.GspGstAuth.GSTAuthentication(strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(), Session["UserName"].ToString(), out OTPPOPUPValue, out OTPAUTHResponse);
                    Session["Save"] = null;
                    ViewBag.OTPSession = OTPPOPUPValue;
                    ViewBag.AUTH_Response = OTPAUTHResponse;
                    ViewBag.AUTH_GSTINNo = strGSTIN;
                }
                    #endregion

              
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);
            return View();
        }

        #endregion

        #region"Get table4"
        public ActionResult Table4()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table4(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[0].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.part2 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }


        [HttpGet]
        public ActionResult Table4Delete(string gstin, string fp)
        {
             
            
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL4");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        public ActionResult Canceltbl4(string gstinid, string fp)
        {
            return View();
        }


            #endregion

        #region"Get table5"
            public ActionResult Table5()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }      
            string Todate = Convert.ToString(Session["Todate"]);
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
            Session["strGSTIN"] = strGSTIN;
            DataSet gstr9 = Gstr9.Table5(strGSTIN,Todate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[1].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Part3 = OReportMgmt.Count;
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table5Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL5");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);

        }
        #endregion

        #region"Get table6"
            public ActionResult Table6()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table6(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[2].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        itc_typ = dr.IsNull("itc_typ") ? "" : dr["itc_typ"].ToString(),
                         iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table6 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table6Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL6");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table7"
        public ActionResult Table7()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table7(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[3].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {

                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        desc = dr.IsNull("desc") ? "" : dr["desc"].ToString(),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table7 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table7Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL7");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table8"
        public ActionResult Table8()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table8(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[4].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                       

                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table8 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table8Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL8");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table9"
        public ActionResult Table9()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table9(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[5].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        taxpayable = dr.IsNull("txpyble") ? 0 : Convert.ToDecimal(dr["txpyble"]),
                        iamt = dr.IsNull("tax_paid_itc_iamt") ? 0 : Convert.ToDecimal(dr["tax_paid_itc_iamt"]),
                        taxpaidthroughcash = dr.IsNull("txpaid_cash") ? 0 : Convert.ToDecimal(dr["txpaid_cash"]),
                        camt = dr.IsNull("tax_paid_itc_camt") ? 0 : Convert.ToDecimal(dr["tax_paid_itc_camt"]),
                        samt = dr.IsNull("tax_paid_itc_samt") ? 0 : Convert.ToDecimal(dr["tax_paid_itc_samt"]),
                        csamt = dr.IsNull("tax_paid_itc_csamt") ? 0 : Convert.ToDecimal(dr["tax_paid_itc_csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table9 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table9Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL9");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table10"
        public ActionResult Table10()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table10(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[6].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                       
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table10 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table10Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL10");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table14"
        public ActionResult Table14()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table14(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[7].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        taxpayable = dr.IsNull("txpyble") ? 0 : Convert.ToDecimal(dr["txpyble"]),
                        taxpaidthroughcash = dr.IsNull("txpaid") ? 0 : Convert.ToDecimal(dr["txpaid"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table14 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table14Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL14");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table15"
        public ActionResult Table15()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table15(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[8].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),             
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                        interest= dr.IsNull("intr") ? 0 : Convert.ToDecimal(dr["intr"]),
                        penalty= dr.IsNull("pen") ? 0 : Convert.ToDecimal(dr["pen"]),
                        latefee_oth = dr.IsNull("fee") ? 0 : Convert.ToDecimal(dr["fee"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table15 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table15Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL15");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table16"
        public ActionResult Table16()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table16(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[9].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),                       
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table16 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table16Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL16");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table17"
        public ActionResult Table17()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table17(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[10].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),                       
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                        totalquantity = dr.IsNull("qty") ? 0 : Convert.ToDecimal(dr["qty"]),
                        hsn = dr.IsNull("hsn_sc") ? "" : dr["hsn_sc"].ToString(),
                        uqc = dr.IsNull("uqc") ? "" : dr["uqc"].ToString(),
                        rate= dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        isconcesstional = dr.IsNull("isconcesstional") ? "" : dr["isconcesstional"].ToString(),
                        
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table17 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table17Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL17");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region"Get table18"
        public ActionResult Table18()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN = Convert.ToString(Session["strGSTIN"]);
             string ToDate = Convert.ToString(Session["Todate"]);

            DataSet gstr9 = Gstr9.Table18(strGSTIN, ToDate);
            TempData["gstr9"] = gstr9;
            List<GSTR9Attributes.Gstr9List> OReportMgmt = new List<GSTR9Attributes.Gstr9List>();
            #region "Data Assign to Attributes"
            if (gstr9.Tables.Count > 0)
            {
                foreach (DataRow dr in gstr9.Tables[11].Rows)
                {
                    OReportMgmt.Add(new GSTR9Attributes.Gstr9List
                    {
                        Action = dr.IsNull("Action") ? "" : dr["Action"].ToString(),
                        natureofsupplies = dr.IsNull("natureOfSupply") ? "" : dr["natureOfSupply"].ToString(),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        totalquantity = dr.IsNull("qty") ? 0 : Convert.ToDecimal(dr["qty"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"]),
                        hsn = dr.IsNull("hsn_sc") ? "" : dr["hsn_sc"].ToString(),
                        uqc = dr.IsNull("uqc") ? "" : dr["uqc"].ToString(),
                        rate = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        isconcesstional = dr.IsNull("isconcesstional") ? "" : dr["isconcesstional"].ToString(),
                    });
                }
            }
            #endregion
            ReportViewModel OModel = new ReportViewModel();
            OModel.ReportMgmt = OReportMgmt;
            ViewBag.Table18 = OReportMgmt.Count;
            ViewBag.ReportName = "Outward Supplies";
            return View(OModel);
        }
        [HttpGet]
        public ActionResult Table18Delete(string gstin, string fp)
        {
            Gstr9 del = new Gstr9();
            del.GSTR9Delete(gstin, Convert.ToString(fp), "TBL18");
            TempData["Delete"] = "Data Deleted Successfully";
            return Json(gstin, JsonRequestBehavior.AllowGet);
        }
        #endregion

         #region"DeleteAll"
        public ActionResult DeleteAllData()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }    
                    
              
               string strGSTIN = Convert.ToString(Session["strGSTIN"]);
               string fp1 = Convert.ToString(Session["Todate"]);

               Gstr9 del = new Gstr9();
               del.GSTR9Delete(strGSTIN, fp1, "ALL");
               TempData["Delete"] = "Data Deleted Successfully";
             

               return RedirectToAction("Gstr9Home");
        }
        #endregion

        [HttpGet]
        public ActionResult UploadFile()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }


            return View();
        }

    }
}