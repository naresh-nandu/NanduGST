using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SmartAdminMvc.Models.Inward
{
    public class InwardFunction
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public static int InvoiceValidation(string gstin,string Inum,string Idt,string Mode)
        {
            int result;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Validate_InvoiceNo", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin",gstin));
                cmd.Parameters.Add(new SqlParameter("@Idt", Idt));
                cmd.Parameters.Add(new SqlParameter("@Inum",Inum));
                cmd.Parameters.Add(new SqlParameter("@Mode",Mode));
                cmd.Parameters.Add("@RetValue", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                result = Convert.ToInt32(cmd.Parameters["@RetValue"].Value);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return result;
        }
        public static void B2BPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR2B2B_EXT_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", RefNo));
                cmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", UserId));

                cmd.ExecuteNonQuery();

                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        public static void B2BURPush(string RefNo, string GSTIN,int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR2B2BUR_EXT_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", RefNo));
                cmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", UserId));
                cmd.ExecuteNonQuery();
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