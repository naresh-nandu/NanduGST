using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_BAL;
using WeP_DAL.EwayBill;
using WeP_EWayBill;

namespace SmartAdminMvc.Controllers
{
    public class EwbReportController : Controller
    {
        public class EwbBusinessLayer : EwaybillDataAccess { }
        // GET: EWBReport
        public ActionResult MYEWB()
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
            try
            {
                ViewBag.Username = WeP_BAL.EwayBill.LoadDropDowns.GetEwayUserwise(custid);
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.MyEWBList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.strDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.strToDate = DateTime.Now.ToString("MM/dd/yyyy");
                ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchListForReport(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult MYEWB(FormCollection Form, string Command)
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
            try
            {
                string reportType, strGSTIN, strDate, branchId, UserId, strToDate;
                int locationId = 0;
                if (Form["branchId"] != "")
                {
                    locationId = Convert.ToInt32(Form["branchId"]);
                }

                reportType = Form["reportType"];
                strGSTIN = Form["strGSTIN"];
                strDate = Form["strDate"];
                strToDate = Form["strToDate"];
                UserId = Form["username"];
                if (string.IsNullOrEmpty(UserId))
                {
                    UserId = "0";
                }
                branchId = Form["branchId"];
                if (branchId == "")
                {
                    branchId = "ALL";
                }
                ViewBag.User = WeP_BAL.EwayBill.LoadDropDowns.Exist_GetEwayUserwise(custid, Convert.ToInt32(UserId));
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.Exist_MyEWBList(reportType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchListForReport(userid, custid, Session["Role_Name"].ToString(), locationId);
                ViewBag.strDate = strDate;
                ViewBag.strToDate = strToDate;
                switch (Command)
                {
                    case "getReport":
                        switch (reportType)
                        {
                            case "1":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }

                                DataSet Outward = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "OUTWARD", custid, Convert.ToInt32(UserId), branchId);
                                TempData["Outward"] = Outward;
                                List<ReportAttributes.EwbList> OReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (Outward.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in Outward.Tables[0].Rows)
                                    {
                                        OReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel OModel = new ReportViewModel();
                                OModel.ReportMgmt = OReportMgmt;
                                ViewBag.Outward = OReportMgmt.Count;
                                ViewBag.ReportName = "Outward Supplies";
                                return View(OModel);

                            case "2":

                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                DataSet Inward = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "INWARD", custid, Convert.ToInt32(UserId), branchId);
                                TempData["Inward"] = Inward;
                                List<ReportAttributes.EwbList> IReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (Inward.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in Inward.Tables[0].Rows)
                                    {
                                        IReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel IModel = new ReportViewModel();
                                IModel.ReportMgmt = IReportMgmt;
                                ViewBag.Inward = IReportMgmt.Count;
                                ViewBag.ReportName = "Inward Supplies";
                                return View(IModel);

                            case "3":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                DataSet GET = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "GENEWB", custid, Convert.ToInt32(UserId), branchId);
                                TempData["GetExport"] = GET;
                                List<ReportAttributes.EwbList> GETReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (GET.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in GET.Tables[0].Rows)
                                    {
                                        GETReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel GETModel = new ReportViewModel();
                                GETModel.ReportMgmt = GETReportMgmt;
                                ViewBag.GenEWB = GETReportMgmt.Count;
                                ViewBag.ReportName = "Generated By Me";
                                return View(GETModel);

                            case "4":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                DataSet ConsGet = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "CONSGENEWB", custid, Convert.ToInt32(UserId), branchId);
                                List<ReportAttributes.ConsewbList> ConsGetReportMgmt = new List<ReportAttributes.ConsewbList>();
                                TempData["ConsGet"] = ConsGet;
                                #region "Data Assign to Attributes"
                                if (ConsGet.Tables.Count > 0)
                                {

                                    foreach (DataRow dr in ConsGet.Tables[0].Rows)
                                    {
                                        ConsGetReportMgmt.Add(new ReportAttributes.ConsewbList
                                        {
                                            Consolidate_Ewaybill_Details = dr.IsNull("Consolidate_Ewaybill_Details") ? "" : dr["Consolidate_Ewaybill_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            Transporter_Details = dr.IsNull("Transporter_Details") ? "" : dr["Transporter_Details"].ToString(),
                                            Vehicle_No = dr.IsNull("Vehicle_No") ? "" : dr["Vehicle_No"].ToString(),
                                            Branch_Name = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel ConsGetModel = new ReportViewModel();
                                ConsGetModel.ConsReportMgmt = ConsGetReportMgmt;
                                ViewBag.ConsGenEWB = ConsGetReportMgmt.Count;
                                ViewBag.ReportName = "Generated Consolidated Eway Bill By Me";
                                return View(ConsGetModel);

                            case "5":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                DataSet Cancel = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "CANCEL", custid, Convert.ToInt32(UserId), branchId);
                                List<ReportAttributes.EwbList> CancelReportMgmt = new List<ReportAttributes.EwbList>();
                                TempData["Cancel"] = Cancel;
                                #region "Data Assign to Attributes"
                                if (Cancel.Tables.Count > 0)
                                {

                                    foreach (DataRow dr in Cancel.Tables[0].Rows)
                                    {
                                        CancelReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),
                                            status = dr.IsNull("Status") ? "" : dr["Status"].ToString(),
                                            rejectStatus = dr.IsNull("RejectStatus") ? "" : dr["RejectStatus"].ToString(),
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel CancelModel = new ReportViewModel();
                                CancelModel.ReportMgmt = CancelReportMgmt;
                                ViewBag.Cancel = CancelReportMgmt.Count;
                                ViewBag.ReportName = "Cancel";
                                return View(CancelModel);

                            case "6":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                DataSet Reject = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "REJECT", custid, Convert.ToInt32(UserId), branchId);
                                List<ReportAttributes.EwbList> RejectReportMgmt = new List<ReportAttributes.EwbList>();
                                TempData["Reject"] = Reject;
                                #region "Data Assign to Attributes"
                                if (Reject.Tables.Count > 0)
                                {

                                    foreach (DataRow dr in Reject.Tables[0].Rows)
                                    {
                                        RejectReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            status = dr.IsNull("Status") ? "" : dr["Status"].ToString(),
                                            rejectStatus = dr.IsNull("RejectStatus") ? "" : dr["RejectStatus"].ToString(),
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel RejectModel = new ReportViewModel();
                                RejectModel.ReportMgmt = RejectReportMgmt;
                                ViewBag.Reject = RejectReportMgmt.Count;
                                ViewBag.ReportName = "Reject";
                                return View(RejectModel);
                            case "7":
                                if (Session["Role_Name"].ToString() == "Super Admin" || Session["Role_Name"].ToString() == "Admin")
                                {
                                    UserId = Form["username"];
                                    if (string.IsNullOrEmpty(UserId))
                                    {
                                        UserId = "0";
                                    }

                                }
                                else
                                {
                                    UserId = Convert.ToString(Session["User_ID"]);
                                }
                                branchId = "ALL";
                                DataSet VehicleUpdate = EwbBusinessLayer.GetEWBListUserwise(strGSTIN, strDate, strToDate, "", "", "", "VEHICLEUPDATE", custid, Convert.ToInt32(UserId), branchId);
                                List<ReportAttributes.EwbList> RejectReportMgmt7 = new List<ReportAttributes.EwbList>();
                                TempData["VehicleUpdate"] = VehicleUpdate;
                                #region "Data Assign to Attributes"
                                if (VehicleUpdate.Tables.Count > 0)
                                {

                                    foreach (DataRow dr in VehicleUpdate.Tables[0].Rows)
                                    {
                                        RejectReportMgmt7.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                                            vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                                            fromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                                            ReasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                                            ReasonRem = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                                            transDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                                            transDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                                            vehUpdDate = dr.IsNull("vehUpdDate") ? "" : dr["vehUpdDate"].ToString(),
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel RejectModel7 = new ReportViewModel();
                                RejectModel7.ReportMgmt = RejectReportMgmt7;
                                ViewBag.VehicleUpdate = RejectReportMgmt7.Count;
                                ViewBag.VehicleUpdate = "VehicleUpdate";
                                return View(RejectModel7);



                        }

                        break;
                    case "getExport":
                        switch (reportType)
                        {
                            case "1":
                                GridView gv = new GridView();
                                gv.DataSource = TempData["Outward"];
                                gv.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-Outward.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw = new StringWriter();
                                HtmlTextWriter htw = new HtmlTextWriter(sw);
                                gv.RenderControl(htw);
                                Response.Output.Write(sw.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");

                            case "2":
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["Inward"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-Inward.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");
                            case "3":
                                GridView gv2 = new GridView();
                                gv2.DataSource = TempData["GetExport"];
                                gv2.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-Generaeted.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw2 = new StringWriter();
                                HtmlTextWriter htw2 = new HtmlTextWriter(sw2);
                                gv2.RenderControl(htw2);
                                Response.Output.Write(sw2.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");

                            case "4":
                                GridView gv3 = new GridView();
                                gv3.DataSource = TempData["ConsGet"];
                                gv3.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=ConsolidatedEwayBillReport-Generaeted.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw3 = new StringWriter();
                                HtmlTextWriter htw3 = new HtmlTextWriter(sw3);
                                gv3.RenderControl(htw3);
                                Response.Output.Write(sw3.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");

                            case "5":
                                GridView gv4 = new GridView();
                                gv4.DataSource = TempData["Cancel"];
                                gv4.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-Cancel.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw4 = new StringWriter();
                                HtmlTextWriter htw4 = new HtmlTextWriter(sw4);
                                gv4.RenderControl(htw4);
                                Response.Output.Write(sw4.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");

                            case "6":
                                GridView gv5 = new GridView();
                                gv5.DataSource = TempData["Reject"];
                                gv5.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-Reject.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw5 = new StringWriter();
                                HtmlTextWriter htw5 = new HtmlTextWriter(sw5);
                                gv5.RenderControl(htw5);
                                Response.Output.Write(sw5.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");
                            case "7":
                                GridView gv7 = new GridView();
                                gv7.DataSource = TempData["VehicleUpdate"];
                                gv7.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-vehicleupdlist.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw7 = new StringWriter();
                                HtmlTextWriter htw7 = new HtmlTextWriter(sw7);
                                gv7.RenderControl(htw7);
                                Response.Output.Write(sw7.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("MYEWB");

                        }
                        break;

                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTIN);
                        #endregion
                        break;
                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        public ActionResult OtherEWB()
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
            try
            {
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.OtherEWBList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.strDate = DateTime.Now.ToString("dd/MM/yyyy");
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult OtherEWB(FormCollection Form, string Command)
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
            try
            {
                string reportType, strGSTINNo, strDate, branchId = "ALL";
                string OStatus = "";
                string OResponse = "";
                string EWAYDate = "";
                reportType = Form["reportType"];
                strGSTINNo = Form["strGSTIN"];
                strDate = Form["strDate"];
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.Exist_OtherEWBList(reportType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTINNo, Session["Role_Name"].ToString());
                ViewBag.strDate = strDate;
                switch (Command)
                {
                    case "getReport":
                        switch (reportType)
                        {
                            case "1":
                                EwbGetApi.GET_EWAYBILL_BY_Other(strDate, strGSTINNo, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                 Session["UserName"].ToString(), out OStatus, out OResponse, out EWAYDate);

                                DataSet GenOther = EwbBusinessLayer.Retrieve_EWB_Report(strGSTINNo, strDate, "", "", "", "OTHERSEWB", custid, userid, branchId);
                                TempData["GenOther"] = GenOther;
                                List<ReportAttributes.EwbList> GenOtherReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (GenOther.Tables.Count > 0)
                                {

                                    foreach (DataRow dr in GenOther.Tables[0].Rows)
                                    {
                                        GenOtherReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                                            ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                                            docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                                            docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                                            fromGstin = dr.IsNull("fromGstin") ? "" : dr["fromGstin"].ToString(),
                                            toGstin = dr.IsNull("toGstin") ? "" : dr["toGstin"].ToString(),
                                            fromTrdName = dr.IsNull("fromTrdName") ? "" : dr["fromTrdName"].ToString(),
                                            toTrdName = dr.IsNull("toTrdName") ? "" : dr["toTrdName"].ToString(),
                                            totalinvValue = dr.IsNull("totinvvalue") ? 0 : Convert.ToDecimal(dr["totinvvalue"]),
                                            status = dr.IsNull("status") ? "" : dr["status"].ToString(),
                                            rejectStatus = dr.IsNull("rejectStatus") ? "" : dr["rejectStatus"].ToString()

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel OModel = new ReportViewModel();
                                OModel.ReportMgmt = GenOtherReportMgmt;
                                ViewBag.GenOther = GenOtherReportMgmt.Count;
                                ViewBag.ReportName = "Generated By Others";
                                return View(OModel);


                            case "2":
                                DataSet RejectOther = EwbBusinessLayer.Retrieve_EWB_Report(strGSTINNo, strDate, "", "", "", "OTHERSREJECT", custid, userid, branchId);
                                TempData["RejectOther"] = RejectOther;
                                List<ReportAttributes.EwbList> RejectOtherReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (RejectOther.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in RejectOther.Tables[0].Rows)
                                    {
                                        RejectOtherReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            supplyType = dr.IsNull("supplyType") ? "" : dr["supplyType"].ToString(),
                                            subSupplyType = dr.IsNull("subSupplyType") ? "" : dr["subSupplyType"].ToString(),
                                            ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                                            ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                                            docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                                            docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                                            cancelewbDate = dr.IsNull("ewbRejectedDate") ? "" : dr["ewbRejectedDate"].ToString(),
                                            status = dr.IsNull("status") ? "" : dr["status"].ToString(),
                                            rejectStatus = dr.IsNull("rejectStatus") ? "" : dr["rejectStatus"].ToString(),
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel RejectModel = new ReportViewModel();
                                RejectModel.ReportMgmt = RejectOtherReportMgmt;
                                ViewBag.RejectOthers = RejectOtherReportMgmt.Count;
                                ViewBag.ReportName = "Rejected By Others";
                                return View(RejectModel);

                        }

                        break;
                    case "getExport":
                        switch (reportType)
                        {
                            case "1":
                                GridView gv = new GridView();
                                gv.DataSource = TempData["GenOther"];
                                gv.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GetGeneratedOthersEwayBillReport.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw = new StringWriter();
                                HtmlTextWriter htw = new HtmlTextWriter(sw);
                                gv.RenderControl(htw);
                                Response.Output.Write(sw.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("OtherEWB");

                            case "2":
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["RejectOther"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GetRejectedOthersEwayBillReport.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("OtherEWB");
                        }
                        break;

                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTINNo);
                        #endregion
                        break;


                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        public ActionResult GETEWB()
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
            try
            {
                ViewBag.strDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.GetEWBList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult GETEWB(FormCollection Form, string Command)
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
            try
            {
                string reportType, strGSTIN, strewbNo, branchId = "ALL";
                string OStatus = "";
                string OResponse = "";
                reportType = Form["reportType"];
                strGSTIN = Form["strGSTIN"];
                strewbNo = Form["strewbNo"];
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.Exist_GetEWBList(reportType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.strewbNo = strewbNo;
                switch (Command)
                {
                    case "getReport":
                        switch (reportType)
                        {
                            case "1":
                                EwbGetApi.GET_EWAYBILL(strewbNo, strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                 Session["UserName"].ToString(), out OStatus, out OResponse);
                                DataSet GetEWB = EwbBusinessLayer.Retrieve_EWB_Report(strGSTIN, "", strewbNo, "", "", "GETEWB", custid, userid, branchId);
                                TempData["GetEWB"] = GetEWB;
                                List<ReportAttributes.EwbList> GetEWBReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (GetEWB.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in GetEWB.Tables[0].Rows)
                                    {
                                        GetEWBReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            supplyType = dr.IsNull("supplyType") ? "" : dr["supplyType"].ToString(),
                                            subSupplyType = dr.IsNull("subSupplyType") ? "" : dr["subSupplyType"].ToString(),
                                            ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                                            ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                                            docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                                            docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                                            docType = dr.IsNull("docType") ? "" : dr["docType"].ToString(),
                                            fromGstin = dr.IsNull("fromGstin") ? "" : dr["fromGstin"].ToString(),
                                            toGstin = dr.IsNull("toGstin") ? "" : dr["toGstin"].ToString(),
                                            transporterId = dr.IsNull("transporterId") ? "" : dr["transporterId"].ToString(),
                                            fromAddr1 = dr.IsNull("fromAddr1") ? "" : dr["fromAddr1"].ToString(),
                                            fromAddr2 = dr.IsNull("fromAddr2") ? "" : dr["fromAddr2"].ToString(),
                                            fromPinCode = dr.IsNull("fromPinCode") ? 0 : Convert.ToInt32(dr["fromPinCode"]),
                                            fromPlace = dr.IsNull("fromPlace") ? "" : dr["fromPlace"].ToString(),
                                            fromStateCode = dr.IsNull("fromStateCode") ? "" : dr["fromStateCode"].ToString(),
                                            fromTrdName = dr.IsNull("fromTrdName") ? "" : dr["fromTrdName"].ToString(),
                                            toAddr1 = dr.IsNull("toAddr1") ? "" : dr["toAddr1"].ToString(),
                                            toAddr2 = dr.IsNull("toAddr2") ? "" : dr["toAddr2"].ToString(),
                                            toPincode = dr.IsNull("toPinCode") ? 0 : Convert.ToInt32(dr["toPinCode"]),
                                            toPlace = dr.IsNull("toPlace") ? "" : dr["toPlace"].ToString(),
                                            toStateCode = dr.IsNull("toStateCode") ? "" : dr["toStateCode"].ToString(),
                                            toTrdName = dr.IsNull("toTrdName") ? "" : dr["toTrdName"].ToString(),
                                            totalinvValue = dr.IsNull("totinvvalue") ? 0 : Convert.ToDecimal(dr["totinvvalue"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel OModel = new ReportViewModel();
                                OModel.ReportMgmt = GetEWBReportMgmt;
                                ViewBag.GetEWB = GetEWBReportMgmt.Count;
                                ViewBag.ReportName = "Get Eway Bill";
                                return View(OModel);


                            case "2":
                                EwbGetApi.GET_Consolidated_EWAYBILL(strewbNo, strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                 Session["UserName"].ToString(), out OStatus, out OResponse);
                                DataSet GetConsEWB = EwbBusinessLayer.Retrieve_EWB_Report(strGSTIN, "", "", strewbNo, "", "CONSGETEWB", custid, userid, branchId);
                                TempData["GetConsEWB"] = GetConsEWB;
                                List<ReportAttributes.ConsewbList> ConsGetReportMgmt = new List<ReportAttributes.ConsewbList>();

                                #region "Data Assign to Attributes"
                                if (GetConsEWB.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in GetConsEWB.Tables[0].Rows)
                                    {
                                        ConsGetReportMgmt.Add(new ReportAttributes.ConsewbList
                                        {
                                            fromPlace = dr.IsNull("fromPlace") ? "" : dr["fromPlace"].ToString(),
                                            fromState = dr.IsNull("fromState") ? "" : dr["fromState"].ToString(),
                                            vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                                            transMode = dr.IsNull("transMode") ? "" : dr["transMode"].ToString(),
                                            transDocDate = dr.IsNull("transDocDate") ? "" : dr["transDocDate"].ToString(),
                                            transDocNo = dr.IsNull("transDocNo") ? "" : dr["transDocNo"].ToString(),
                                            cEwbNo = dr.IsNull("cEwbNo") ? "" : dr["cEwbNo"].ToString(),
                                            cEWBDate = dr.IsNull("cEWBDate") ? "" : dr["cEWBDate"].ToString()
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel ConsGetModel = new ReportViewModel();
                                ConsGetModel.ConsReportMgmt = ConsGetReportMgmt;
                                ViewBag.GetConsEWB = ConsGetReportMgmt.Count;
                                ViewBag.ReportName = "Get Consolidate Eway Bill";
                                return View(ConsGetModel);

                        }

                        break;
                    case "getExport":
                        switch (reportType)
                        {
                            case "1":
                                GridView gv = new GridView();
                                gv.DataSource = TempData["GetEWB"];
                                gv.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GetEwayBillReport.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw = new StringWriter();
                                HtmlTextWriter htw = new HtmlTextWriter(sw);
                                gv.RenderControl(htw);
                                Response.Output.Write(sw.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("GETEWB");

                            case "2":
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["GetConsEWB"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GetConsolidatedEwayBillReport.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("GETEWB");
                        }
                        break;

                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTIN);
                        #endregion
                        break;
                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        public ActionResult TRANSEWB()
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
            try
            {
                ViewBag.strDate = DateTime.Now.ToString("dd/MM/yyyy");
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.TransEWBList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());

            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult TRANSEWB(FormCollection Form, string Command)
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
            try
            {
                string reportType, strGSTIN, strDate, genGSTIN, branchId = "ALL";
                string OStatus = "";
                string OResponse = "";
                string EWAYDate = "";
                reportType = Form["reportType"];
                strGSTIN = Form["strGSTIN"];
                strDate = Form["strDate"];
                genGSTIN = Form["genGSTIN"];
                ViewBag.genGSTIN = genGSTIN;
                ViewBag.strDate = strDate;
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.Exist_TransEWBList(reportType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                switch (Command)
                {
                    case "getReport":
                        switch (reportType)
                        {
                            case "1":
                                EwbGetApi.GET_EWAYBILL_Transporter(strDate, strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                 Session["UserName"].ToString(), out OStatus, out OResponse, out EWAYDate);
                                DataSet TransDate = EwbBusinessLayer.Retrieve_EWB_Report(strGSTIN, strDate, "", "", "", "TRANSDATE", custid, userid, branchId);
                                TempData["TransDate"] = TransDate;
                                List<ReportAttributes.Transporter> TransMgmt = new List<ReportAttributes.Transporter>();

                                #region "Data Assign to Attributes"
                                if (TransDate.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in TransDate.Tables[0].Rows)
                                    {
                                        TransMgmt.Add(new ReportAttributes.Transporter
                                        {
                                            ewbNo = dr.IsNull("ewbNo") ? "" : dr["ewbNo"].ToString(),
                                            ewbDate = dr.IsNull("ewbDate") ? "" : dr["ewbDate"].ToString(),
                                            docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                                            docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                                            genGstin = dr.IsNull("genGstin") ? "" : dr["genGstin"].ToString(),
                                            delPlace = dr.IsNull("delPlace") ? "" : dr["delPlace"].ToString(),
                                            delPinCode = dr.IsNull("delPinCode") ? "" : dr["delPinCode"].ToString(),
                                            delStateCode = dr.IsNull("delStateCode") ? "" : dr["delStateCode"].ToString(),
                                            status = dr.IsNull("status") ? "" : dr["status"].ToString(),
                                            rejectStatus = dr.IsNull("rejectStatus") ? "" : dr["rejectStatus"].ToString(),
                                            validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                                            extendedTimes = dr.IsNull("extendedTimes") ? "" : dr["extendedTimes"].ToString()
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel OModel = new ReportViewModel();
                                OModel.TransMgmt = TransMgmt;
                                ViewBag.GetTrans = TransMgmt.Count;
                                ViewBag.ReportName = "Transpoter";
                                return View(OModel);


                            case "2":
                                EwbGetApi.GET_EWAYBILL_TRANSPORTER_GSTIN(strDate, genGSTIN, strGSTIN, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                Session["UserName"].ToString(), out OStatus, out OResponse, out EWAYDate);
                                DataSet TransEWB = EwbBusinessLayer.Retrieve_EWB_Report(strGSTIN, strDate, "", "", genGSTIN, "TRANSGSTIN", custid, userid, branchId);
                                TempData["TransEWB"] = TransEWB;
                                List<ReportAttributes.Transporter> TranMgmt = new List<ReportAttributes.Transporter>();

                                #region "Data Assign to Attributes"
                                if (TransEWB.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in TransEWB.Tables[0].Rows)
                                    {
                                        TranMgmt.Add(new ReportAttributes.Transporter
                                        {
                                            ewbNo = dr.IsNull("ewbNo") ? "" : dr["ewbNo"].ToString(),
                                            ewbDate = dr.IsNull("ewbDate") ? "" : dr["ewbDate"].ToString(),
                                            docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                                            docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                                            genGstin = dr.IsNull("genGstin") ? "" : dr["genGstin"].ToString(),
                                            delPlace = dr.IsNull("delPlace") ? "" : dr["delPlace"].ToString(),
                                            delPinCode = dr.IsNull("delPinCode") ? "" : dr["delPinCode"].ToString(),
                                            delStateCode = dr.IsNull("delStateCode") ? "" : dr["delStateCode"].ToString(),
                                            status = dr.IsNull("status") ? "" : dr["status"].ToString(),
                                            rejectStatus = dr.IsNull("rejectStatus") ? "" : dr["rejectStatus"].ToString(),
                                            validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                                            extendedTimes = dr.IsNull("extendedTimes") ? "" : dr["extendedTimes"].ToString()
                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel Model = new ReportViewModel();
                                Model.TransMgmt = TranMgmt;
                                ViewBag.GetTransGst = TranMgmt.Count;
                                ViewBag.ReportName = "Transpoter";
                                return View(Model);
                        }

                        break;
                    case "getExport":
                        switch (reportType)
                        {
                            case "1":
                                GridView gv = new GridView();
                                gv.DataSource = TempData["TransDate"];
                                gv.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GeTransporterReport_Date.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw = new StringWriter();
                                HtmlTextWriter htw = new HtmlTextWriter(sw);
                                gv.RenderControl(htw);
                                Response.Output.Write(sw.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("TRANSEWB");

                            case "2":
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["TransEWB"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=GetTransporterReport_GSTIN.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("TRANSEWB");
                        }
                        break;

                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTIN);
                        #endregion
                        break;
                }

            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }
        #region"EwayBill Reports Active,InActive and Error Records"
        public ActionResult EwayStatus()
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
            try
            {
                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.MyStatusList();
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.branchListForReport(userid, custid, Session["Role_Name"].ToString());
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult EwayStatus(FormCollection Form, string Command)
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
            try
            {
                string reportType, strGSTIN, branchId;
                int locationId = 0;
                if (Form["branchId"] != "")
                {
                    locationId = Convert.ToInt32(Form["branchId"]);
                }

                reportType = Form["reportType"];
                strGSTIN = Form["strGSTIN"];
                branchId = Form["branchId"];
                if (branchId == "")
                {
                    branchId = "ALL";
                }

                ViewBag.GetEwayList = WeP_BAL.EwayBill.LoadDropDowns.Exist_MyStatusList(reportType);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.LocationList = WeP_BAL.EwayBill.LoadDropDowns.Exist_branchListForReport(userid, custid, Session["Role_Name"].ToString(), locationId);

                switch (Command)
                {
                    case "getReport":
                        switch (reportType)
                        {
                            case "ErrorRecords":

                                DataSet ErrorRecords = EwbBusinessLayer.GetStatusList(strGSTIN, reportType, custid, userid, branchId);
                                TempData["ErrorRecords"] = ErrorRecords;
                                List<ReportAttributes.EwbList> ReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (ErrorRecords.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in ErrorRecords.Tables[0].Rows)
                                    {
                                        ReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel OModel = new ReportViewModel();
                                OModel.ReportMgmt = ReportMgmt;
                                ViewBag.ErrorRecords = ReportMgmt.Count;
                                ViewBag.ReportName = "Error Records";
                                return View(OModel);

                            case "Active":

                                DataSet Active = EwbBusinessLayer.GetStatusList(strGSTIN, reportType, custid, userid, branchId);
                                TempData["Active"] = Active;
                                List<ReportAttributes.EwbList> IReportMgmt = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (Active.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in Active.Tables[0].Rows)
                                    {
                                        IReportMgmt.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Eway_Bill_Validity = dr.IsNull("Eway_Bill_Validity") ? "" : dr["Eway_Bill_Validity"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel IModel = new ReportViewModel();
                                IModel.ReportMgmt = IReportMgmt;
                                ViewBag.Active = IReportMgmt.Count;
                                ViewBag.ReportName = "Active EwayBills";
                                return View(IModel);

                            case "InActive":

                                DataSet InActive = EwbBusinessLayer.GetStatusList(strGSTIN, reportType, custid, userid, branchId);
                                TempData["InActive"] = InActive;
                                List<ReportAttributes.EwbList> IReportMgmt1 = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (InActive.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in InActive.Tables[0].Rows)
                                    {
                                        IReportMgmt1.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Eway_Bill_Validity = dr.IsNull("Eway_Bill_Validity") ? "" : dr["Eway_Bill_Validity"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel IModel1 = new ReportViewModel();
                                IModel1.ReportMgmt = IReportMgmt1;
                                ViewBag.InActive = IReportMgmt1.Count;
                                ViewBag.ReportName = "InActive EwayBills";
                                return View(IModel1);

                            case "UniqueNumber":

                                DataSet UniqueNumber = EwbBusinessLayer.GetStatusList(strGSTIN, reportType, custid, userid, branchId);
                                TempData["InActive"] = UniqueNumber;
                                List<ReportAttributes.EwbList> IReportMgmt2 = new List<ReportAttributes.EwbList>();

                                #region "Data Assign to Attributes"
                                if (UniqueNumber.Tables.Count > 0)
                                {
                                    foreach (DataRow dr in UniqueNumber.Tables[0].Rows)
                                    {
                                        IReportMgmt2.Add(new ReportAttributes.EwbList
                                        {
                                            Eway_Bill_Details = dr.IsNull("Eway_Bill_Details") ? "" : dr["Eway_Bill_Details"].ToString(),
                                            Document_Details = dr.IsNull("Document_Details") ? "" : dr["Document_Details"].ToString(),
                                            From_Details = dr.IsNull("From_Details") ? "" : dr["From_Details"].ToString(),
                                            To_Details = dr.IsNull("To_Details") ? "" : dr["To_Details"].ToString(),
                                            Transported_Details = dr.IsNull("Transported_Details") ? "" : dr["Transported_Details"].ToString(),
                                            Vehicle_Details = dr.IsNull("Vehicle_Details") ? "" : dr["Vehicle_Details"].ToString(),
                                            BranchName = dr.IsNull("Branch_Name") ? "" : dr["Branch_Name"].ToString(),
                                            Taxable_Value = dr.IsNull("Taxable_Value") ? 0 : Convert.ToDecimal(dr["Taxable_Value"]),
                                            Total_Invoice_Amount = dr.IsNull("Total_Invoice_Amount") ? 0 : Convert.ToDecimal(dr["Total_Invoice_Amount"]),
                                            IGST_Amount = dr.IsNull("IGST_Amount") ? 0 : Convert.ToDecimal(dr["IGST_Amount"]),
                                            CGST_Amount = dr.IsNull("CGST_Amount") ? 0 : Convert.ToDecimal(dr["CGST_Amount"]),
                                            SGST_Amount = dr.IsNull("SGST_Amount") ? 0 : Convert.ToDecimal(dr["SGST_Amount"]),
                                            CESS_Amount = dr.IsNull("CESS_Amount") ? 0 : Convert.ToDecimal(dr["CESS_Amount"]),

                                        });
                                    }
                                }
                                #endregion
                                ReportViewModel IModel2 = new ReportViewModel();
                                IModel2.ReportMgmt = IReportMgmt2;
                                ViewBag.UniqueNumber = IReportMgmt2.Count;
                                ViewBag.ReportName = "Unique Number";
                                return View(IModel2);
                        }

                        break;
                    case "getActive":
                        switch (reportType)
                        {
                            case "Active":
                                GridView gv = new GridView();
                                gv.DataSource = TempData["Active"];
                                gv.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-ActiveEwayBill.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw = new StringWriter();
                                HtmlTextWriter htw = new HtmlTextWriter(sw);
                                gv.RenderControl(htw);
                                Response.Output.Write(sw.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("EwayStatus");

                            case "InActive":
                                GridView gv1 = new GridView();
                                gv1.DataSource = TempData["InActive"];
                                gv1.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-InActiveEwayBill.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw1 = new StringWriter();
                                HtmlTextWriter htw1 = new HtmlTextWriter(sw1);
                                gv1.RenderControl(htw1);
                                Response.Output.Write(sw1.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("EwayStatus");

                            case "ErrorRecords":
                                GridView gv2 = new GridView();
                                gv2.DataSource = TempData["ErrorRecords"];
                                gv2.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-ErrorRecordsEwayBill.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw2 = new StringWriter();
                                HtmlTextWriter htw2 = new HtmlTextWriter(sw2);
                                gv2.RenderControl(htw2);
                                Response.Output.Write(sw2.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("EwayStatus");

                            case "UniqueNumber":
                                GridView gv3 = new GridView();
                                gv3.DataSource = TempData["UniqueNumber"];
                                gv3.DataBind();
                                Response.ClearContent();
                                Response.Buffer = true;
                                Response.AddHeader("content-disposition", "attachment; filename=EwayBillReport-UniqueNumberEwayBill.xls");
                                Response.ContentType = "application/ms-excel";
                                Response.Charset = "";
                                StringWriter sw3 = new StringWriter();
                                HtmlTextWriter htw3 = new HtmlTextWriter(sw3);
                                gv3.RenderControl(htw3);
                                Response.Output.Write(sw3.ToString());
                                Response.Flush();
                                Response.End();
                                return RedirectToAction("EwayStatus");



                        }
                        break;

                    default:
                        #region "Authentication Checking"
                        EwbGeneratingKeys.Autentication(strGSTIN);
                        #endregion
                        break;
                }
            }
            catch (Exception EX)
            {
                TempData["ErrorMessage"] = EX.Message;
            }
            return View();
        }
        #endregion
    }
}