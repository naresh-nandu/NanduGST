using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.GstrDownload
{
    public class Gstr6Download
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public readonly int _userid;
        public Gstr6Download(int CustId, int UserId)
        {
            this._custid = CustId;
            this._userid = UserId;
        }

        public DataSet Retrieve_GSTR6_Download_EXT_SA_Bulk_Summary(string ActionType, string Gstin, string from, string to, string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR6_Download_EXT_SA_Bulk_Summary", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", from));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", to));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
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

        public DataSet Retrieve_GSTR6_Download_EXT_SA_Bulk_Rawdata(string ActionType, string Gstin, string from, string to, string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR6_Download_EXT_SA_Bulk_RawData", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));

                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", from));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", to));

                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
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


        public void Delete_GSTR1_Delete_EXT_SA_Bulk(string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_SA_Bulk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                    cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                    cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                    cmd.Parameters.Add(new SqlParameter("@SourceType", "ALL"));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

      
    }
}
