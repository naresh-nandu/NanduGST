using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.GSTRUpload;
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
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class DeviceUploadController : Controller
    {
        // GET: DeviceUpload

        private readonly WePGSPDBEntities db = new WePGSPDBEntities();
        readonly SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        [HttpGet]
        public ActionResult Upload(int page = 0, string sort = null)
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

                ViewBag.GSTINNoList = LoadDropDowns.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                SelectList gsttypelst = new SelectList(db.TBL_GSTR_TYPE, "GSTRName", "GSTRName");
                ViewBag.gstrtypeList = gsttypelst;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "BP",
                    Value = "BP"
                });
                items.Add(new SelectListItem
                {
                    Text = "POS",
                    Value = "POS"
                });
                items.Add(new SelectListItem
                {
                    Text = "ERP",
                    Value = "ERP"
                });
                ViewBag.Device = new SelectList(items, "Text", "Value");
                return View();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        [HttpPost]
        public ActionResult Upload(FormCollection col, string command, int[] ids)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            string strInvIds = "";
            try
            {
                string gstnum = col["CategoryName"];
                string devicetype = col["DeviceList"];
                string gsttype = col["GSTRList"];
                strInvIds = col["InvIds"];
                strInvIds = strInvIds.TrimStart(',').TrimEnd(',');

                TempData["gstnum"] = gstnum;
                TempData["devicetype"] = devicetype;
                TempData["gsttype"] = gsttype;

                GstrExt objGSTREXT = new GstrExt();

                #region "Pushing data from EXT to SA"
                if (command == "save")
                {
                    string gstnum1 = col["CategoryName"];
                    string devicetype1 = col["DeviceList"];
                    string gsttype1 = col["GSTRList"];

                    if (gsttype1 == "GSTR1")
                    {
                        PushGSTR1(gstnum1, devicetype1);
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), " GSTR1 " + devicetype1 + " Data is Uploaded Successfully", "");

                    }
                    else if (gsttype1 == "GSTR2")
                    {
                        PushGSTR2(gstnum1, devicetype1);
                        db.Ins_AuditLog(Convert.ToInt32(Session["User_ID"]), Convert.ToString(Session["UserName"]), " GSTR2 " + devicetype1 + " Data is Uploaded Successfully", "");
                    }

                    TempData["UploadMessage"] = "Data Uploaded Successfully";
                }
                #endregion

                #region "GSTR1 EXT Delete ALL"
                // DELETE ALL
                //Deleting Invoices from GSTR1 B2B
                else if (command == "GSTR1B2B")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2B", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2B EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2CL
                else if (command == "GSTR1B2CL")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CL EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 AT
                else if (command == "GSTR1AT")
                {
                    objGSTREXT.GSTR1_Delete_ALL("AT", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 AT EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNUR
                else if (command == "GSTR1CDNUR")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNUR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNUR EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 CDNR
                else if (command == "GSTR1CDNR")
                {
                    objGSTREXT.GSTR1_Delete_ALL("CDNR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 CDNR EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 B2CS
                else if (command == "GSTR1B2CS")
                {
                    objGSTREXT.GSTR1_Delete_ALL("B2CS", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 B2CS EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 DOCISSUE
                else if (command == "GSTR1DOC")
                {
                    objGSTREXT.GSTR1_Delete_ALL("DOC", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 DOC EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 EXP
                else if (command == "GSTR1EXP")
                {
                    objGSTREXT.GSTR1_Delete_ALL("EXP", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 EXP EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                //Deleting Invoices from GSTR1 HSN
                else if (command == "GSTR1HSN")
                {
                    objGSTREXT.GSTR1_Delete_ALL("HSN", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 HSN EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR1 NIL
                else if (command == "GSTR1NIL")
                {
                    objGSTREXT.GSTR1_Delete_ALL("NIL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 NIL EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }


                //Deleting Invoices from GSTR1 TXPD
                else if (command == "GSTR1TXPD")
                {
                    objGSTREXT.GSTR1_Delete_ALL("TXP", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR1 TXP EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                #endregion

                #region "GSTR2 EXT Delete "
                //Deleting Invoices from GSTR2 B2B
                else if (command == "GSTR2B2B")
                {
                    objGSTREXT.GSTR2_Delete_ALL("B2B", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 B2B EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }


                //Deleting Invoices from GSTR2 IMPG
                else if (command == "GSTR2IMPG")
                {
                    objGSTREXT.GSTR2_Delete_ALL("IMPG", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 IMPG EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 IMPS
                else if (command == "GSTR2IMPS")
                {
                    objGSTREXT.GSTR2_Delete_ALL("IMPS", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 IMPS EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 CDN
                else if (command == "GSTR2CDN")
                {
                    objGSTREXT.GSTR2_Delete_ALL("CDN", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 CDN EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 Nil
                else if (command == "GSTR2NIL")
                {
                    objGSTREXT.GSTR2_Delete_ALL("NIL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 NIL EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 TXI
                else if (command == "GSTR2TXI")
                {
                    objGSTREXT.GSTR2_Delete_ALL("TXI", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 TXI EXT Deleted", ""); TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 TXPD
                else if (command == "GSTR2TXPD")
                {
                    objGSTREXT.GSTR2_Delete_ALL("TXPD", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 TXPD EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 HSN
                else if (command == "GSTR2HSN")
                {
                    objGSTREXT.GSTR2_Delete_ALL("HSN", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 HSN EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }


                //Deleting Invoices from GSTR2 B2BUR
                else if (command == "GSTR2B2BUR")
                {
                    objGSTREXT.GSTR2_Delete_ALL("B2BUR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 B2BUR EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 ITC
                else if (command == "GSTR2ITC")
                {
                    objGSTREXT.GSTR2_Delete_ALL("ITCRVSL", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 ITCRVSL EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }

                //Deleting Invoices from GSTR2 CDNUR
                else if (command == "GSTR2CDNUR")
                {
                    objGSTREXT.GSTR2_Delete_ALL("CDNUR", strInvIds);
                    Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "GSTR2 CDNUR EXT Deleted", "");
                    TempData["UploadMessage"] = "Selected Invoice(S) Deleted Successfully";
                }
                #endregion

                //Onchange Event Start Here
                if (gsttype == "GSTR1")
                {
                    var B2B = GetB2B(gstnum, devicetype);
                    ViewBag.B2B = B2B;

                    var B2CS = GetB2CS(gstnum, devicetype);
                    ViewBag.B2CS = B2CS;

                    var B2CL = GetB2CL(gstnum, devicetype);
                    ViewBag.B2CL = B2CL;

                    var CDNR = GetCDNR(gstnum, devicetype);
                    ViewBag.CDNR = CDNR;

                    var EXP = GetEXP(gstnum, devicetype);
                    ViewBag.EXP = EXP;

                    var HSN = GetHSN(gstnum, devicetype);
                    ViewBag.HSN = HSN;

                    var NIL = GetNIL(gstnum, devicetype);
                    ViewBag.NIL = NIL;

                    var TXPD = GetTXPD(gstnum, devicetype);
                    ViewBag.TXPD = TXPD;

                    var AT = GetAT(gstnum, devicetype);
                    ViewBag.AT = AT;

                    var DOC = GetDOC(gstnum, devicetype);
                    ViewBag.DOC = DOC;

                    var CDNUR = GetCDNUR(gstnum, devicetype);
                    ViewBag.CDNUR = CDNUR;
                    Session["ActionID"] = gsttype;
                }
                else if (gsttype == "GSTR2")
                {
                    var B2B = GetB2B2(gstnum, devicetype);
                    ViewBag.B2B = B2B;

                    var IMPG = GetIMPG(gstnum, devicetype);
                    ViewBag.IMPG = IMPG;

                    var IMPS = GetIMPS(gstnum, devicetype);
                    ViewBag.IMPS = IMPS;

                    var CDN = GetCDN(gstnum, devicetype);
                    ViewBag.CDN = CDN;

                    var TXI = GetTXI(gstnum, devicetype);
                    ViewBag.TXI = TXI;

                    var HSN = GetHSN2(gstnum, devicetype);
                    ViewBag.HSN = HSN;

                    var NIL = GetNIL2(gstnum, devicetype);
                    ViewBag.NIL = NIL;

                    var TXPD = GetTXPD2(gstnum, devicetype);
                    ViewBag.TXPD = TXPD;

                    var B2BUR = GetB2BUR(gstnum, devicetype);
                    ViewBag.B2BUR = B2BUR;

                    var ITC = GetITC(gstnum, devicetype);
                    ViewBag.ITC = ITC;

                    var CDNUR = GetCDNUR2(gstnum, devicetype);
                    ViewBag.CDNUR = CDNUR;
                    Session["ActionID"] = gsttype;
                }

                ViewBag.GSTINNoList = LoadDropDowns.Exist_GSTIN_No(userid, custid, gstnum, Session["Role_Name"].ToString());
                SelectList gsttypelst = new SelectList(db.TBL_GSTR_TYPE, "GSTRName", "GSTRName", gsttype);
                ViewBag.gstrtypeList = gsttypelst;
                List<SelectListItem> items = new List<SelectListItem>();
                items.Add(new SelectListItem
                {
                    Text = "BP",
                    Value = "BP"
                });
                items.Add(new SelectListItem
                {
                    Text = "POS",
                    Value = "POS"
                });
                items.Add(new SelectListItem
                {
                    Text = "ERP",
                    Value = "ERP"
                });
                ViewBag.Device = new SelectList(items, "Text", "Value", devicetype);

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


        public List<IDictionary> GetB2B(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetB2CS(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetB2CL(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CL_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetCDNR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNR_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetEXP(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1EXP_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetHSN(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1HSN_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetNIL(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1NIL_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetTXPD(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1TXP_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetAT(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1AT_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetDOC(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1DOCISSUE_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetCDNUR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNUR_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }


        public List<IDictionary> GetB2B2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2B_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetIMPG(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPG_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetIMPS(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetCDN(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDN_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetTXI(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXI_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetHSN2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2HSN_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetNIL2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2NIL_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetTXPD2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXPD_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetB2BUR(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2BUR_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        public List<IDictionary> GetITC(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2ITCRVSL_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetCDNUR2(string GSTINNo, string GSTDevice)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDNUR_EXT", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }


        public void PushGSTR1(string GSTINNo, string GSTDevice)
        {
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1_EXT_SA", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                dCmd.Parameters.Add(new SqlParameter("@CustId", Convert.ToInt32(Session["Cust_ID"])));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", Convert.ToInt32(Session["User_ID"])));
                dCmd.ExecuteNonQuery();
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public void PushGSTR2(string GSTINNo, string GSTDevice)
        {
            try
            {
                #region commented
                conn.Open();
                SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2_EXT_SA", conn);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", GSTDevice));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Session["CustRefNo"].ToString()));
                dCmd.Parameters.Add(new SqlParameter("@CustId", Convert.ToInt32(Session["Cust_ID"])));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", Convert.ToInt32(Session["User_ID"])));
                dCmd.ExecuteNonQuery();
                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        [HttpGet]
        public ActionResult Demo()
        {
            return View();
        }

        public ActionResult PDF()
        {
            return View();
        }
    }
}