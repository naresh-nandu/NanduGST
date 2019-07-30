using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.MISReports
{
    public class Reports
    {
        public static List<IDictionary> GSTINSalesSummary(string GSTINNo, string strFp,int custid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SalesSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
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
        public static DataTable GSTINSalesSummarydt(string GSTINNo, string strFp, int custid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SalesSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return dt;

        }


        public static List<IDictionary> GSTINPurchaseSummary(string GSTINNo, string strFp, int custid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_PurchaseSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
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

        public static DataTable GSTINPurchaseSummarydt(string GSTINNo, string strFp, int custid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_PurchaseSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return dt;

        }


        public static List<IDictionary> CustomerSalesReport(string GSTINNo, string strFp, int custid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_Customer_SalesSummary_B2B", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
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

        public static DataTable CustomerSalesReportdt(string GSTINNo, string strFp, int custid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_Customer_SalesSummary_B2B", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return dt;

        }
        public static List<IDictionary> InvoiceType(string GSTINNo, string strFp, int custid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SalesSummary_InvoiceType", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
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

        public static DataTable InvoiceTypedt(string GSTINNo, string strFp, int custid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SalesSummary_InvoiceType", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return dt;

        }
        public static List<IDictionary> SupplierInwardRegister(string GSTINNo, string strFp, int custid)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SupplierInward_Register", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
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

        public static DataTable SupplierInwardRegisterdt(string GSTINNo, string strFp, int custid)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_MIS_GSTIN_SupplierInward_Register", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstinno", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Period", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@Custid", custid));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return dt;
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

        public static List<IDictionary> GSTR6ISDSummary(string GSTINNo, string strFp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_Summary_After_Reconcil", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", strFp));
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
        public static DataTable GSTR6ISDSummarydt(string GSTINNo, string strFp)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_Summary_After_Reconcil", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", strFp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    dt.Clear();
                    da.Fill(dt);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return dt;
        }

    }
}