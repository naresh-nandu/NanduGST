using Newtonsoft.Json;
using SmartAdminMvc.Models;
using SmartAdminMvc.Models.Common;
using SmartAdminMvc.Models.SearchTaxPayer;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using System.Web.UI.WebControls;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class GstinValidationController : Controller
    {
        // GET: GSTINValidation
        [HttpGet]
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
                        
            string startdate = DateTime.Now.ToString("dd-MM-yyyy");
            string enddate = DateTime.Now.AddDays(1).ToString("dd-MM-yyyy");
            ViewBag.StartDate = startdate;
            ViewBag.EndDate = enddate;
            return View();
        }

        [HttpPost]
        public ActionResult Index(HttpPostedFileBase FileUpload, FormCollection frm, string btnCheck, string btnCheckALL, string btnDownload, string btnAddToMaster)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());
            int iCustId = Convert.ToInt32(Session["Cust_ID"].ToString());
            string strUserName = Session["UserName"].ToString();
            string strGSTRType = "";
            strGSTRType = frm["ddlGSTR"].ToString();

            try
            {
                string strGSTIN = "", strData = "";
                int iStatus = 3;

                #region "SINGLE GSTIN CHECK"
                if (!string.IsNullOrEmpty(btnCheck))
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = con;
                            sqlcmd.CommandText = "Select * from TBL_ValidGstin where ValidGstin = @ValidGSTIN and Status = '1' and CustId = @CustId";
                            sqlcmd.Parameters.AddWithValue("@ValidGSTIN", strGSTIN);
                            sqlcmd.Parameters.AddWithValue("@CustId", iCustId);
                            using (SqlDataAdapter TP_adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable TP_dt = new DataTable();
                                TP_adt.Fill(TP_dt);
                                if (TP_dt.Rows.Count > 0)
                                {
                                    TempData["ErrorMessage"] = "GSTIN already Validated... Please check in the Report";
                                }
                                else
                                {
                                    GspSearchTaxPayerGstin.SendRequest(strGSTIN, Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName, out iStatus, out strData);
                                    InsertValidGSTIN(strGSTIN, iStatus, strData, iUserId, iCustId, "", strGSTRType);
                                    TempData["Message"] = "GSTIN Validated successfully... Please check in the list";
                                }
                            }
                        }
                    }

                }
                #endregion

                #region "BULK GSTIN CHECK"
                else if (!string.IsNullOrEmpty(btnCheckALL))
                {
                    DataTable dt = new DataTable();
                    DataRow row;
                    List<string> lstGSTINNo = new List<string>();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0];
                        if (file.ContentLength > 0 && !String.IsNullOrEmpty(file.FileName))
                        {
                            string fileName = Path.GetFileName(FileUpload.FileName);
                            fileName = string.Concat(Path.GetFileNameWithoutExtension(fileName), "_", iUserId.ToString(), DateTime.Now.ToString("yyyyMMddHHmmssfff"), Path.GetExtension(fileName));
                            Session["FileName"] = fileName.Trim();
                            string FileExtension = fileName.Substring(fileName.LastIndexOf('.') + 1).ToLower();
                            if (FileExtension.Trim() == "csv" && file.ContentLength > 0 && file.ContentLength < 5242880)
                            {
                                string path = Path.Combine(Server.MapPath("~/App_Data/uploads"), fileName);
                                FileUpload.SaveAs(path);

                                StreamReader sr = new StreamReader(path);
                                string line = sr.ReadLine();
                                string[] value = line.Split(',');

                                foreach (string dc in value)
                                {
                                    dt.Columns.Add(new DataColumn(dc));
                                }
                                int i = 0;
                                while (!sr.EndOfStream)
                                {
                                    line = sr.ReadLine();
                                    value = line.Split(',');
                                    if (value.Length == dt.Columns.Count)
                                    {
                                        row = dt.NewRow();
                                        row.ItemArray = value;
                                        dt.Rows.Add(row);
                                        if (dt.Rows.Count == 60000)
                                        {
                                            break;
                                        }
                                        if (dt.Columns.Count == 56) // Template A
                                        {
                                            lstGSTINNo.Add(dt.Rows[i][6].ToString().Trim());
                                        }
                                        else if (dt.Columns.Count == 52) // Template B
                                        {
                                            lstGSTINNo.Add(dt.Rows[i][4].ToString().Trim());
                                        }
                                        else if (dt.Columns.Count == 46) // Template B
                                        {
                                            lstGSTINNo.Add(dt.Rows[i][4].ToString().Trim());
                                        }
                                        else // Other Source
                                        {
                                            lstGSTINNo.Add(dt.Rows[i][0].ToString().Trim());
                                        }
                                    }
                                    i++;
                                }
                                sr.Close();

                                lstGSTINNo.RemoveAll(x => x == "");
                                lstGSTINNo = lstGSTINNo.Distinct().ToList();
                                if (lstGSTINNo.Count > 0)
                                {
                                    for (int j = 0; j < lstGSTINNo.Count; j++)
                                    {
                                        using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                                        {
                                            using (SqlCommand sqlcmd = new SqlCommand())
                                            {
                                                sqlcmd.Connection = con;
                                                sqlcmd.CommandText = "Select * from TBL_ValidGstin where ValidGstin = @ValidGSTIN and CustId = @CustId";
                                                sqlcmd.Parameters.AddWithValue("@ValidGSTIN", lstGSTINNo[j].Trim());
                                                sqlcmd.Parameters.AddWithValue("@CustId", iCustId);
                                                using (SqlDataAdapter TP_adt = new SqlDataAdapter(sqlcmd))
                                                {
                                                    DataTable TP_dt = new DataTable();
                                                    TP_adt.Fill(TP_dt);
                                                    if (TP_dt.Rows.Count > 0)
                                                    {
                                                        if (TP_dt.Rows[0]["Status"].ToString() == "1" && TP_dt.Rows[0]["Data"].ToString() != "Invalid GSTIN")
                                                        {
                                                            UpdateValidGSTINFileName(lstGSTINNo[j].Trim(), iUserId, iCustId, fileName, strGSTRType);
                                                        }
                                                        else
                                                        {
                                                            if (lstGSTINNo[j].Trim().Length == 15)
                                                            {
                                                                GspSearchTaxPayerGstin.SendRequest(lstGSTINNo[j].Trim(), Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName, out iStatus, out strData);
                                                            }
                                                            UpdateValidGSTIN(lstGSTINNo[j].Trim(), iStatus, strData, iUserId, iCustId, fileName, strGSTRType);
                                                        }                                                        
                                                    }
                                                    else
                                                    {
                                                        if (lstGSTINNo[j].Trim().Length == 15)
                                                        {
                                                            GspSearchTaxPayerGstin.SendRequest(lstGSTINNo[j].Trim(), Convert.ToString(iUserId), Convert.ToString(iCustId), strUserName, out iStatus, out strData);
                                                        }
                                                        InsertValidGSTIN(lstGSTINNo[j].Trim(), iStatus, strData, iUserId, iCustId, fileName, strGSTRType);
                                                    }
                                                }
                                            }
                                        }
                                    }
                                    TempData["Message"] = "GSTIN Validated successfully... Please check in the Report";
                                }

                            }
                        }
                    }
                }
                #endregion

                #region "EXPORT GSTIN LIST"
                else if (!string.IsNullOrEmpty(btnDownload))
                {
                    GridView gv = new GridView();
                    gv.DataSource = Get_GSTIN_List_Datatable(iCustId, Session["FileName"].ToString());
                    gv.DataBind();
                    Response.ClearContent();
                    Response.Buffer = true;
                    Response.AddHeader("content-disposition", "attachment; filename=GSTIN_Validated_List.xls");
                    Response.ContentType = "application/ms-excel";
                    Response.Charset = "";
                    StringWriter sw = new StringWriter();
                    HtmlTextWriter htw = new HtmlTextWriter(sw);
                    gv.RenderControl(htw);
                    Response.Output.Write(sw.ToString());
                    Response.Flush();
                    Response.End();
                    TempData["Message"] = "GSTIN List Exported Successfully...";
                }
                #endregion

                #region "ADD TO MASTER"
                else if (!string.IsNullOrEmpty(btnAddToMaster))
                {
                    Task.Factory.StartNew(() =>AddToMaster(strGSTRType, iCustId, iUserId));
                    Thread.Sleep(5000);
                    TempData["Message"] = "GSTIN Added to Master Successfully...";
                }
                #endregion

                else
                {
                    // ...
                }
                return View();
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message.ToString();
                return View();
            }
        }

        public void InsertValidGSTIN(string strGSTINNo, int iStatus, string strData, int UserId, int CustId, string strFileName, string strGSTRType)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@ValidGstin", SqlDbType.VarChar).Value = strGSTINNo;
                cmd.Parameters.Add("@Status", SqlDbType.Int).Value = iStatus;
                if (strGSTINNo.Trim().Length == 15)
                {
                    cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = strData;
                }
                else
                {
                    cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = "Invalid GSTIN";
                }
                cmd.Parameters.Add("@CreatedBy", SqlDbType.Int).Value = UserId;
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = CustId;
                if (iStatus == 1)
                {
                    string DecodedData = Helper.Base64Decode(strData);
                    var strTPAttr = JsonConvert.DeserializeObject<SearchTaxpayerAttributes>(DecodedData);

                    if (!string.IsNullOrEmpty(strTPAttr.lgnm))
                    {
                        cmd.Parameters.Add("@LegalName", SqlDbType.VarChar).Value = strTPAttr.lgnm.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.stj))
                    {
                        cmd.Parameters.Add("@State_jur", SqlDbType.VarChar).Value = strTPAttr.stj.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.ctj))
                    {
                        cmd.Parameters.Add("@Center_jur", SqlDbType.VarChar).Value = strTPAttr.ctj.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.rgdt))
                    {
                        cmd.Parameters.Add("@RegDate", SqlDbType.VarChar).Value = strTPAttr.rgdt.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.ctb))
                    {
                        cmd.Parameters.Add("@Business_Cont", SqlDbType.VarChar).Value = strTPAttr.ctb.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.dty))
                    {
                        cmd.Parameters.Add("@taxpayerType", SqlDbType.VarChar).Value = strTPAttr.dty.ToString();
                    }
                    if (strTPAttr.nba != null)
                    {
                        string strNBA = string.Join(",", strTPAttr.nba.ToArray());
                        strNBA = strNBA.Replace(',', ':').Replace('/', '-');
                        cmd.Parameters.Add("@NatureOfBusiness", SqlDbType.VarChar).Value = strNBA;
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.cxdt))
                    {
                        cmd.Parameters.Add("@CancellationDate", SqlDbType.VarChar).Value = strTPAttr.cxdt.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.lstupdt))
                    {
                        cmd.Parameters.Add("@LastUpdatedDate", SqlDbType.VarChar).Value = strTPAttr.lstupdt.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.stjCd))
                    {
                        cmd.Parameters.Add("@State_Jur_Code", SqlDbType.VarChar).Value = strTPAttr.stjCd.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.ctjCd))
                    {
                        cmd.Parameters.Add("@Center_Jur_Code", SqlDbType.VarChar).Value = strTPAttr.ctjCd.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.tradeNam))
                    {
                        cmd.Parameters.Add("@tradeName", SqlDbType.VarChar).Value = strTPAttr.tradeNam.ToString();
                    }
                    if (!string.IsNullOrEmpty(strTPAttr.sts))
                    {
                        cmd.Parameters.Add("@strStatus", SqlDbType.VarChar).Value = strTPAttr.sts.ToString();
                    }
                    //InsertCusSupGSTIN(DecodedData, strGSTINNo, CustId, UserId, strGSTRType);
                }
                cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = strFileName.Trim();
                cmd.Parameters.Add("@GSTRType", SqlDbType.NVarChar).Value = strGSTRType;
                Functions.InsertIntoTable("TBL_ValidGSTIN", cmd, con);
                con.Close();
            }
        }

        public void UpdateValidGSTIN(string strGSTINNo, int iStatus, string strData, int UserId, int CustId, string strFileName, string strGSTRType)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                using (SqlCommand command = new SqlCommand())
                {
                    command.Connection = con;
                    command.CommandText = "Select TOP 1 * from TBL_ValidGSTIN where ValidGstin = @ValidGSTIN and CustId = @CustId";
                    command.Parameters.AddWithValue("@ValidGSTIN", strGSTINNo);
                    command.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(command))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string strValidGSTINId = dt.Rows[0]["validGstinId"].ToString();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ValidGstin", SqlDbType.VarChar).Value = strGSTINNo;
                            cmd.Parameters.Add("@Status", SqlDbType.Int).Value = iStatus;
                            if (strGSTINNo.Trim().Length == 15)
                            {
                                cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = strData;
                            }
                            else
                            {
                                cmd.Parameters.Add("@Data", SqlDbType.NVarChar).Value = "Invalid GSTIN";
                            }
                            cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = UserId;
                            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            if (iStatus == 1)
                            {
                                string DecodedData = Helper.Base64Decode(strData);
                                var strTPAttr = JsonConvert.DeserializeObject<SearchTaxpayerAttributes>(DecodedData);

                                if (!string.IsNullOrEmpty(strTPAttr.lgnm))
                                {
                                    cmd.Parameters.Add("@LegalName", SqlDbType.VarChar).Value = strTPAttr.lgnm.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.stj))
                                {
                                    cmd.Parameters.Add("@State_jur", SqlDbType.VarChar).Value = strTPAttr.stj.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.ctj))
                                {
                                    cmd.Parameters.Add("@Center_jur", SqlDbType.VarChar).Value = strTPAttr.ctj.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.rgdt))
                                {
                                    cmd.Parameters.Add("@RegDate", SqlDbType.VarChar).Value = strTPAttr.rgdt.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.ctb))
                                {
                                    cmd.Parameters.Add("@Business_Cont", SqlDbType.VarChar).Value = strTPAttr.ctb.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.dty))
                                {
                                    cmd.Parameters.Add("@taxpayerType", SqlDbType.VarChar).Value = strTPAttr.dty.ToString();
                                }
                                if (strTPAttr.nba != null)
                                {
                                    string strNBA = string.Join(",", strTPAttr.nba.ToArray());
                                    strNBA = strNBA.Replace(',', ':').Replace('/', '-');
                                    cmd.Parameters.Add("@NatureOfBusiness", SqlDbType.VarChar).Value = strNBA;
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.cxdt))
                                {
                                    cmd.Parameters.Add("@CancellationDate", SqlDbType.VarChar).Value = strTPAttr.cxdt.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.lstupdt))
                                {
                                    cmd.Parameters.Add("@LastUpdatedDate", SqlDbType.VarChar).Value = strTPAttr.lstupdt.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.stjCd))
                                {
                                    cmd.Parameters.Add("@State_Jur_Code", SqlDbType.VarChar).Value = strTPAttr.stjCd.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.ctjCd))
                                {
                                    cmd.Parameters.Add("@Center_Jur_Code", SqlDbType.VarChar).Value = strTPAttr.ctjCd.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.tradeNam))
                                {
                                    cmd.Parameters.Add("@tradeName", SqlDbType.VarChar).Value = strTPAttr.tradeNam.ToString();
                                }
                                if (!string.IsNullOrEmpty(strTPAttr.sts))
                                {
                                    cmd.Parameters.Add("@strStatus", SqlDbType.VarChar).Value = strTPAttr.sts.ToString();
                                }
                                //InsertCusSupGSTIN(DecodedData, strGSTINNo, CustId, UserId, strGSTRType);
                            }
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = strFileName.Trim();
                            cmd.Parameters.Add("@GSTRType", SqlDbType.NVarChar).Value = strGSTRType;
                            Functions.UpdateTable("TBL_ValidGSTIN", "ValidGstinId", strValidGSTINId, cmd, con);
                        }
                    }
                }
                con.Close();
            }
        }
        
        public void UpdateValidGSTINFileName(string strGSTINNo, int UserId, int CustId, string strFileName, string strGSTRType)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_ValidGSTIN where ValidGstin = @ValidGSTIN and CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@ValidGSTIN", strGSTINNo);
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            string strValidGSTINId = dt.Rows[0]["validGstinId"].ToString();
                            //string strStatus = dt.Rows[0]["Status"].ToString();
                            //string strData = dt.Rows[0]["Data"].ToString();
                            SqlCommand cmd = new SqlCommand();
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = UserId;
                            cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                            cmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = strFileName.Trim();
                            cmd.Parameters.Add("@GSTRType", SqlDbType.NVarChar).Value = strGSTRType;
                            Functions.UpdateTable("TBL_ValidGSTIN", "ValidGstinId", strValidGSTINId, cmd, con);
                            //if(strStatus == "1")
                            //{
                            //    string DecodedData = Helper.Base64Decode(strData);
                            //    InsertCusSupGSTIN(DecodedData, strGSTINNo, CustId, UserId, strGSTRType);
                            //}
                        }
                    }
                }
                con.Close();
            }
        }

                       
        public static DataTable Get_GSTIN_List_Datatable(int iCustId, string strFileName)
        {
            DataTable dt = new DataTable();
            DataTable dt1 = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select validGstin as 'GSTIN',LegalName as 'LEGAL NAME', TradeName as 'TRADE NAME', taxpayerType as 'TAXPAYER TYPE', Case [status] When 1 Then 'Valid GSTIN' Else [Data] END as 'Message' from TBL_ValidGSTIN where CustId = @CustId and FileName = @FileName order by ValidGstin ASC";
                        sqlcmd.Parameters.AddWithValue("@CustId", iCustId);
                        sqlcmd.Parameters.AddWithValue("@FileName", strFileName);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            dt.Clear();
                            da.Fill(dt);
                        }
                    }
                    conn.Close();
                    if (dt.Rows.Count > 0)
                    {
                        return dt;
                    }
                    else
                    {
                        conn.Open();
                        SqlDataAdapter da1 = new SqlDataAdapter("Select 'The Uploaded file is already validated. The File has all Valid GSTIN.' as 'Message'", conn);
                        dt1.Clear();
                        da1.Fill(dt1);
                        conn.Close();
                        return dt1;
                    }
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public void AddToMaster(string strGSTRType, int iCustId, int iUserId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand("usp_Insert_ValidGstins_to_SupplierAndBuyer_Master", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@CustId", iCustId));
                    cmd.Parameters.Add(new SqlParameter("@UserId", iUserId));
                    cmd.Parameters.Add(new SqlParameter("@GstrType", strGSTRType));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

    }
}