using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SmartAdminMvc.Models.Reconcilation
{
    public partial class ReconcilationDataModel
    {
        public readonly int _custid;
        public readonly int _userid;
        public ReconcilationDataModel(int CustId, int UserId)
        {
            this._custid = CustId;
            this._userid = UserId;
        }

        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }

        public DataSet GetMatchedInvoices(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            HttpContext.Current.Session["Matched_B2BRefIds"] = "";
            HttpContext.Current.Session["Matched_CDNRefIds"] = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid2"].ToString() + ",");
                        }
                    }
                    RefIds = myBuilder.ToString();
                    RefIds = RefIds.TrimEnd(',');

                    if (ActionType == "B2B")
                    {
                        HttpContext.Current.Session["Matched_B2BRefIds"] = RefIds;
                    }
                    if (ActionType == "CDNR" || ActionType == "CDN")
                    {
                        HttpContext.Current.Session["Matched_CDNRefIds"] = RefIds;
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds;
        }

        public DataSet GetMatchedInvoices_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            HttpContext.Current.Session["Matched_B2BRefIds"] = "";
            HttpContext.Current.Session["Matched_CDNRefIds"] = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid2"].ToString() + ",");
                        }
                    }
                    RefIds = myBuilder.ToString();
                    RefIds = RefIds.TrimEnd(',');

                    if (ActionType == "B2B")
                    {
                        HttpContext.Current.Session["Matched_B2BRefIds"] = RefIds;
                    }
                    if (ActionType == "CDNR" || ActionType == "CDN")
                    {
                        HttpContext.Current.Session["Matched_CDNRefIds"] = RefIds;
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds;
        }

        public List<IDictionary> GetMatchedInvoicesGSTR6(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            HttpContext.Current.Session["Matched_B2BRefIds"] = "";
            HttpContext.Current.Session["Matched_CDNRefIds"] = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices_GSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid2"].ToString() + ",");
                        }
                    }
                    RefIds = myBuilder.ToString();
                    RefIds = RefIds.TrimEnd(',');

                    if (ActionType == "B2B")
                    {
                        HttpContext.Current.Session["Matched_B2BRefIds"] = RefIds;
                    }
                    if (ActionType == "CDNR" || ActionType == "CDN")
                    {
                        HttpContext.Current.Session["Matched_CDNRefIds"] = RefIds;
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public DataSet GetMissingInvoiceinGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetReconcileCount(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoice_Statistics", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }

            return ds;
        }

        public DataSet GetReconcileCount_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string fromdate, string todate)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoice_Statistics_Idt_wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", fromdate));
                    dCmd.Parameters.Add(new SqlParameter("@todate", todate));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();
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
            }

            return ds;
        }



        public DataSet GetMissingInvoiceinGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MissingInvoiceinGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_Invoices_MissingInGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MissingInvoiceinGSTR2_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_Invoices_MissingInGSTR2_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }


        public List<IDictionary> I_GetMissingInvoiceinGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> I__GetMissingInvoiceinGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string year, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }


        public DataSet GetMissingInvoiceinGSTR2(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetMissingInvoiceinGSTR2_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2_idt_wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                   // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public List<IDictionary> I_GetMissingInvoiceinGSTR2(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> I_GetMissingInvoiceinGSTR2_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string year, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2_idt_wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public DataSet GetMissingInvoiceinGSTR2and2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Idt_DataCorrection_FP", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetMissingInvoiceinGSTR6and6A_INUM(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR6andGSTR6A_Inum_DataCorrection_FP", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetMissingInvoiceinGSTR6and6A_IDT(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR6andGSTR6A_Idt_DataCorrection_FP", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetMissingInvoiceinGSTR2and2A_INUM(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Inum_DataCorrection_FP", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendmentInvoiceinGSTR2A_Amed(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2A_Reg_and_Amend_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendmentInvoiceinGSTR2A_Amed_Idt_Wise(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp,  string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2A_Reg_and_Amend_DataCorrection_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendment_inum(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Amend_Inum_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendment_inum_Idt_Wise(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Amend_Inum_DataCorrection_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendment_idt(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Amend_Idt_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendment_idt_Idt_Wise(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp,  string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Amend_Idt_DataCorrection_Idt_wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }


        public DataSet GetAmendmentInvoiceinGSTR2_2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2_and_GSTR2A_Amend_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetAmendmentInvoiceinGSTR2_2A_IdtWise(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2_and_GSTR2A_Amend_DataCorrection_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MissingInvoiceinGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_Invoices_MissingInGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MissingInvoiceinGSTR2(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_Invoices_MissingInGSTR2", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MismatchInvoices(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_MismatchedInvoicesInGSTR2andGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }


        public DataSet GetMissingInvoiceinGSTR2and2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("[usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Idt_DataCorrection_Idt_Wise]", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                   // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }


        public DataSet GetMissingInvoiceinGSTR2and2A_inum_idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Inum_DataCorrection_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }


        public List<IDictionary> GetMissingInvoiceinGSTR6and6A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public DataSet GetMissingInvoiceinGSTR6A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR6A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }


        public DataSet GetMissingInvoiceinGSTR6(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public List<IDictionary> GetMismatchInvoice(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Mismatched_Invoices", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public DataSet GetMissingInvoiceinGSTR2and2A_CTIN_wise(string strGSTINNo, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTIN_wise_Summary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
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
            }
            return ds;
        }

        public DataSet GetMissingInvoiceinGSTR2and2A_CTIN_wise_idt_wise(string strGSTINNo, string strFromdt, string strTodt)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTIN_wise_Summary_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                   // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFromdt));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strTodt));
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
            }
            return ds;
        }


        public DataTable DT_GetMissingInvoiceinGSTR2and2A_CTIN_wise(string strGSTINNo, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTIN_wise_Summary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
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
            }
            return ds.Tables[0];
        }

        public DataTable DT_GetMismatchInvoice(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Mismatched_Invoices", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR2and2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_DataCorrection", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR2and2A_CTIN_wiese(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTIN_wise_Summary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataSet GetMismatchInvoiceinGSTR2andGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds;
        }

        public DataSet GetMismatchInvoiceinGSTR2andGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                   // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }

        public DataSet Get_Hold_MismatchInvoiceinGSTR2andGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Hold_MismatchedInvoicesInGSTR2andGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds;
        }


        public List<IDictionary> GetMismatchInvoiceinGSTR6andGSTR6A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR6andGSTR6A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetUpdateITCData(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2_ITC_Updation", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
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
            }
            return ConvertToDictionary(ds.Tables[0]);
        }


        public DataTable DT_GetMatchedInvoices(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMatchedInvoices_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string year, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMatchedInvoicesGSTR6(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Matched_Invoices_GSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR6A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR6A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR2(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR2", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMissingInvoiceinGSTR6(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_Invoices_MissingInGSTR6", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMismatchInvoiceinGSTR2andGSTR2A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }
            return ds.Tables[1];
        }

        public DataTable DT_GetMismatchInvoiceinGSTR2andGSTR2A_Idt(string strGSTINNo, string strSupplierName, string strCTIN, string ActionType, string year, string strFp, string strtofp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strtofp));
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
            }
            return ds.Tables[1];
        }


        public DataTable DT_GetMismatchInvoiceinGSTR6andGSTR6A(string strGSTINNo, string strSupplierName, string strCTIN, string strFp, string ActionType, string strtofp)
        {
            DataSet ds = new DataSet();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR6andGSTR6A", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                    dCmd.Parameters.Add(new SqlParameter("@Ctin", strCTIN));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strtofp));
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
            }

            return ds.Tables[1];
        }

        public int ctin_Validation(string strGSTINNo, string strFromPeriod, string strToPeriod, string oldctin, string newctin)
        {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_GSTR2and2AandSupplier_with_newCtin", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@oldctin", oldctin));
                    dCmd.Parameters.Add(new SqlParameter("@newctin", newctin));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

                return 1;
            }

        }

        public int ctin_Validation_IdtWise(string strGSTINNo, string strFromPeriod, string strToPeriod, string oldctin, string newctin)
        {

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_GSTR2and2AandSupplier_with_newCtin_Idt_Wise", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    //dCmd.Parameters.Add(new SqlParameter("@FY", year));
                    dCmd.Parameters.Add(new SqlParameter("@fromdate", strFromPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@todate", strToPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@oldctin", oldctin));
                    dCmd.Parameters.Add(new SqlParameter("@newctin", newctin));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }

                return 1;
            }

        }
    }
}