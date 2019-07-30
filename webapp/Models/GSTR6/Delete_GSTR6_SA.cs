using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR6
{
    public class Delete_GSTR6_SA
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
      
        public void GSTR6Delete(string strActionType, int[] ids)
        {
            try
            {
                con.Open();
                foreach (int invid in ids)
                {
                    SqlCommand cmd = new SqlCommand("usp_Delete_GSTR6_SA", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                    cmd.Parameters.Add(new SqlParameter("@RefId", invid));
                    //cmd.Parameters.Add(new SqlParameter("@GstinId", ""));
                    //cmd.Parameters.Add(new SqlParameter("@InvoiceNo", ""));
                    //cmd.Parameters.Add(new SqlParameter("@InvoiceDate", ""));
                    //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

     
    }
}