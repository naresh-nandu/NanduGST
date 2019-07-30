using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class delete3bdata
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        internal void GSTR3BDeleteALL(string gSTINNo, string fp,int mode)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR3B_Data", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@gstin", gSTINNo));
                cmd.Parameters.Add(new SqlParameter("@fp", fp));
                cmd.Parameters.Add(new SqlParameter("@Mode", mode));

                cmd.ExecuteNonQuery();
                con.Close();

            }
            catch(Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
      
        }

    }
}
