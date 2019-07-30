using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR2
{
    public class DeleteGstr2Sa
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public void GSTR2Delete(string strActionType, int[] ids)
        {
            try
            {
                con.Open();
                foreach (int invid in ids)
                {
                    SqlCommand cmd = new SqlCommand("usp_Delete_GSTR2_SA", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                    cmd.Parameters.Add(new SqlParameter("@RefId", invid));
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
    }
}