using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SmartAdminMvc.Models.PANMgmt
{
    public class PanFunctions
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public int PANDelete(int PanID)
        {
            int ErrorCode;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_PAN", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@PanId",PanID));
                cmd.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                ErrorCode = Convert.ToInt32(cmd.Parameters["@ErrorCode"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ErrorCode;
        }
    }
}