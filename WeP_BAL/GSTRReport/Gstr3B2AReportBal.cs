using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace WeP_BAL.GSTRReport
{
    public class Gstr3B2AReportBal
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected Gstr3B2AReportBal()
        {

        }

        public static DataSet Retrieve_GSTR3BAndGSTR2A_Report(string strGSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR3BAndGSTR2A_Report", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPeriod));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
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
        #region
        #region "Report 3b"
        public static DataSet Report(string fromdate, string ToDate,int custid)
        {
            DataSet ds = new DataSet();
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            try
            {
                #region commented
                if (conn.State == ConnectionState.Closed)
                {
                    conn.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR3B_ITC_LEDGER_OUTPUTTAX", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", fromdate));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", ToDate));
                cmd.Parameters.Add(new SqlParameter("@CustId", custid));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                conn.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion



        #endregion
    }
}
