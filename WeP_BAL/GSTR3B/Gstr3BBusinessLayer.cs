using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.GSTR3B
{
   public static class Gstr3BBusinessLayer
    {
        #region "Checking Either ewayBillNo exist or not "
        
        
        public static int Check_GStr3B_D_Data_Exist_Or_Not(string strGstin, string strPeriod)
        {
            int status = 0;
            try
            {
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                                sqlcmd.CommandText = "Select * from TBL_GSTR3B_D where gstin = @strGstin and ret_period = @strPeriod";
                                sqlcmd.Parameters.AddWithValue("@strGstin", strGstin);
                                sqlcmd.Parameters.AddWithValue("@strPeriod", strPeriod);

                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                status = 1;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return status;
        }
        #endregion


        #region
        #region "Report"
        public static DataSet Table4(string fromdate, string ToDate)
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
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", conn);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@fromDate", fromdate));
                cmd.Parameters.Add(new SqlParameter("@toDate", ToDate));
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
