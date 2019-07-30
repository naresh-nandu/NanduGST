using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_DAL.GSTR9Attribute;

namespace SmartAdminMvc.Controllers
{
    public class Gstr9LogDataController : Controller
    {
        // GET: Gstr9LogData
        public ActionResult Log()
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
            ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.FinancialList();
            ViewBag.GSTINNoList = Models.Common.LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            ViewBag.GetDataModeList = WeP_BAL.EwayBill.LoadDropDowns.DataModeList();
            return View();
        }

        [HttpPost]
        public ActionResult Log(string command, FormCollection form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }
            string strGSTIN,FinancialYear, DataMode, FromDate = "", ToDate = "";
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                FinancialYear = form["FinancialList"];
                DataMode = form["DataMode"];
                strGSTIN = form["strGSTIN"];
                if (!String.IsNullOrEmpty(FinancialYear))
                {
                    strGSTIN = "ALL";
                }


                 ViewBag.GetFinancialList = WeP_BAL.EwayBill.LoadDropDowns.Exist_FinancialList(FinancialYear);
                ViewBag.GSTINNoList = Models.Common.LoadDropDowns.Exist_GSTIN_No(userid, custid, strGSTIN, Session["Role_Name"].ToString());
                ViewBag.GetDataModeList = WeP_BAL.EwayBill.LoadDropDowns.Exist_DataModeList(DataMode);
                if (!String.IsNullOrEmpty(FinancialYear))
                {
                    string str = FinancialYear.Substring(0, 4);
                    FromDate = str.Insert(0, "04");
                    var result = FinancialYear.Substring(FinancialYear.Length - 4);
                    ToDate = result.Insert(0, "03");
                }
                if (!String.IsNullOrEmpty(command))
                {
                    if (DataMode == "WeP")
                    {
                        var das = Gstr9.Retrieve_GSTR9_Log_Details(DataMode, strGSTIN, ToDate,custid);

                        using (DataSet ds = das)
                        {
                            //Set Name of DataTables.
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].TableName = "GSTR9 Log";
                            }
                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                foreach (DataTable dt in ds.Tables)
                                {
                                    //Add DataTable as Worksheet.
                                    if (dt.Rows.Count > 0)
                                    {
                                        wb.Worksheets.Add(dt);
                                    }
                                }

                                //Export the Excel file.
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment; filename=GSTR9Report_For_" + FinancialYear + ".xls");

                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {

                                    if (wb.Worksheets.Count > 0)
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                    }

                                    Response.Flush();
                                    Response.End();

                                }
                            }
                        }
                    }
                    else
                    {
                        var das1 = Gstr9.Retrieve_GSTR9_Log_Details1(DataMode, strGSTIN, ToDate,custid);
                        using (DataSet ds = das1)
                        {
                            //Set Name of DataTables.
                            if (ds.Tables[0].Rows.Count > 0)
                            {
                                ds.Tables[0].TableName = "GSTR9 Log";
                            }

                            using (XLWorkbook wb = new XLWorkbook())
                            {
                                foreach (DataTable dt in ds.Tables)
                                {
                                    //Add DataTable as Worksheet.
                                    if (dt.Rows.Count > 0)
                                    {
                                        wb.Worksheets.Add(dt);
                                    }
                                }
                                Response.Clear();
                                Response.Buffer = true;
                                Response.Charset = "";
                                Response.ContentType = "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                                Response.AddHeader("content-disposition", "attachment; filename=GSTR9Report_For_" + FinancialYear + ".xls");
                                using (MemoryStream MyMemoryStream = new MemoryStream())
                                {

                                    if (wb.Worksheets.Count > 0)
                                    {
                                        wb.SaveAs(MyMemoryStream);
                                        MyMemoryStream.WriteTo(Response.OutputStream);
                                    }

                                    Response.Flush();
                                    Response.End();

                                }
                            }


                        }
                    }

                }
            }
            catch (Exception ex)
            {
                TempData["error"] = ex.Message;
            }

            return View();
        }
    }
}