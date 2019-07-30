using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Reconcilation
{
    public class GSRT1_EWB_Reconcilation
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public static DataSet Retrieve_Recon(string GSTIN,string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Reconcilation_Ewaybill_Gstr1", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fromdate ",fromPeriod));
                cmd.Parameters.Add(new SqlParameter("@Todate ", toPeriod));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }
        public static List<IDictionary> Retrieve_Data_Recon(string GSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Reconcilation_Ewaybill_Gstr1", Con);
                dCmd.CommandType = CommandType.StoredProcedure;

                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@Gstin ", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@Fromdate ", fromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@Todate ", toPeriod));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                ds.Clear();
                da.Fill(ds);
                var das = ds.Tables[0].AsEnumerable();
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ConvertToDictionary(ds.Tables[0]);

        }
        public static List<IDictionary> Retrieve_Data_Recon1(string GSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Reconcilation_Ewaybill_Gstr1", Con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@Gstin ", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@Fromdate ", fromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@Todate ", toPeriod));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                ds.Clear();
                da.Fill(ds);
                var das = ds.Tables[1].AsEnumerable();
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConvertToDictionary(ds.Tables[1]);
        }
        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();

            return dictionaryList.ToList<IDictionary>();
        }

    }
}