using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.GstrDownload
{
   public class Gstr7Download
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public readonly int _userid;
        public Gstr7Download(int CustId, int UserId)
        {
            this._custid = CustId;
            this._userid = UserId;
        }

        public DataSet Retrieve_GSTR7_Download_EXT_SA_Summary(string ActionType, string Gstin, string from, string to)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented

                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR7_Download_EXT_SA_Summary", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", from));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", to));
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

    }
}
