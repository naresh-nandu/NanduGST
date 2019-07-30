using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR7
{
    public class DeleteGstr7Sa
    {              
        public void GSTR7Delete(string strActionType, string ids)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    con.Open();
                    //foreach (string invid in ids)
                    //{
                        SqlCommand cmd = new SqlCommand("usp_Delete_GSTR7_SA", con);
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                        cmd.Parameters.Add(new SqlParameter("@RefId", Convert.ToInt32(ids)));
                        cmd.ExecuteNonQuery();
                    //}
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

     
    }
}