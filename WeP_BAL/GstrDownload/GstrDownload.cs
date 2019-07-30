using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace WeP_BAL.GstrDownload
{
   public class GstrDownload
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public readonly int _custid;
        public readonly int _userid;

        public GstrDownload(int CustId, int UserId)
        {
            this._userid = UserId;
            this._custid = CustId;
        }
        public string Insert_GSTRDownload_Filedet(string gstin, string fp, string action, string token, string GstrType, string ctin,string statecode,string username)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_GSTRDownload_Filedet", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin", gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@Action", action));
                cmd.Parameters.Add(new SqlParameter("@Token", token));
                cmd.Parameters.Add(new SqlParameter("@GSTRType",GstrType));
                cmd.Parameters.Add(new SqlParameter("@Ctin",ctin));
                cmd.Parameters.Add(new SqlParameter("@StateCode",statecode));
                cmd.Parameters.Add(new SqlParameter("@UserId",this._userid));
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserName",username));

                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return "";
        }

    }
}
