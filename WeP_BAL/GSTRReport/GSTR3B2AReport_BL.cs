using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.GSTRReport
{
  public  class GSTR3B2AReport_BL
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

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
                throw ex;
            }

            return ds;

        }

    }
}
