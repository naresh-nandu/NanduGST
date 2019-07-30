using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartAdminMvc.Models.GSTR1
{
    public class B2CS_Invoice_Delete
    {
      static  SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public static int Delete(string B2CSId,string StrGSTINNo,string strPeriod)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_B2CS_INV_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                cmd.Parameters.Add(new SqlParameter("@RefIds", B2CSId));
                cmd.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@ErrorCode"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outputparam;
        }

        public static int DeleteOther(string B2CSId, string StrGSTINNo, string strPeriod)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_B2CS_INV_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                cmd.Parameters.Add(new SqlParameter("@RefIds", B2CSId));
                cmd.Parameters.Add("@ErrorCode", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@ErrorCode"].Value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return outputparam;
        }
    }
}