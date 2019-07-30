using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTRUpload
{
    public partial class GSTR_EXT
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public void GSTR1_Delete_ALL(string strActionType, string RefIds)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_All_GSTR1_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                cmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));

                //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GSTR2_Delete_ALL(string strActionType, string RefIds)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_All_GSTR2_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                cmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));

                //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GSTR6_Delete_ALL(string strActionType, string RefIds)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_All_GSTR6_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                cmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));

                //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}