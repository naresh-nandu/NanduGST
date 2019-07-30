using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR1BulkData
{
    public partial class GSTR1BulkDataModel
    {
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

        public static List<IDictionary> GetInputData(string GSTINNo, string strFp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Metro_Summary_Input_Data where entityid = @GSTINNo and fp = @Period";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", GSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strFp);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(ds);
                        }
                    }
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

        public static List<IDictionary> GetOutputData(string GSTINNo, string strFp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Metro_Summary_Output_Data where gstin = @GSTINNo and fp = @Period";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", GSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strFp);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(ds);
                        }
                    }
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

        public static List<IDictionary> GetBulkDataProcessed(string GSTINNo, string strFp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_Bulkdata_Processed_Summary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
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

        public static List<IDictionary> GetBulkDataErrorRecords(string GSTINNo, string strFp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_Bulkdata_Error_Summary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();
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

        public static DataTable GetGSTR1_Data(string strGSTINNo, string strPeriod, string Action)
        {
            DataTable newdt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlCommand cmd = new SqlCommand();
                cmd.Connection = con;
                if (Action == "B2B")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1B2B_SA";
                }
                if (Action == "B2CL")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1B2CL_SA";
                }
                if (Action == "B2CS")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1B2CS_SA";
                }
                if (Action == "CDNR")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1CDNR_SA";
                }
                if (Action == "CDNUR")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1CDNUR_SA";
                }
                if (Action == "EXP")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1EXP_SA";
                }
                if (Action == "HSN")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1HSN_SA";
                }
                if (Action == "TXPD")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1TXP_SA";
                }
                if (Action == "AT")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1AT_SA";
                }
                if (Action == "NIL")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1NIL_SA";
                }
                if (Action == "DOCISSUE")
                {
                    cmd.CommandText = "usp_Retrieve_GSTR1DOCISSUE_SA";
                }

                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GSTINNo", strGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                cmd.Parameters.Add(new SqlParameter("@Flag", ""));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(newdt);
                con.Close();
                return newdt;
            }
        }

        public static DataTable GetGSTR1_Error_Data(string strGSTINNo, string strPeriod)
        {
            DataTable newdt = new DataTable();
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Bulkdata_Error_Records", con);
                cmd.CommandTimeout = 0;
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(newdt);
                con.Close();
                return newdt;
            }
        }

        public static void GetGSTR1_Error_Data_Count(string strGSTINNo, string strPeriod, out int totalCount, out int totprocessedcount, out int toterrorcount)
        {
            DataTable newdt = new DataTable();
            int totcount = 0, totprocessed = 0, toterror = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Bulkdata_Error_Records", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                cmd.Parameters.Add(new SqlParameter("@TotalRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@ProcessedRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                cmd.Parameters.Add(new SqlParameter("@ErrorRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                con.Open();
                cmd.ExecuteNonQuery();

                totcount = Convert.ToInt32(cmd.Parameters["@TotalRecordsCount"].Value);
                totprocessed = Convert.ToInt32(cmd.Parameters["@ProcessedRecordsCount"].Value);
                toterror = Convert.ToInt32(cmd.Parameters["@ErrorRecordsCount"].Value);
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(newdt);
                con.Close();
            }
            totalCount = totcount;
            totprocessedcount = totprocessed;
            toterrorcount = toterror;
        }
    }
}