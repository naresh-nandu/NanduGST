using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace SmartAdminMvc.Models.EditInvoice
{
    public class NilFunctions
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        protected NilFunctions()
        {
            //
        }
                
        public static int Insert(string Gstin, string fp,decimal NilAmount,decimal ExemptedAmount, decimal NongstAmont,string SupplyType, string Refno, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Insert_Outward_GSTR1_NIL_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp ", fp));
                cmd.Parameters.Add(new SqlParameter("@nil_amt", NilAmount));
                cmd.Parameters.Add(new SqlParameter("@expt_amt", ExemptedAmount));
                cmd.Parameters.Add(new SqlParameter("@ngsup_amt", NongstAmont));
                cmd.Parameters.Add(new SqlParameter("@sply_ty", SupplyType));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo ", Refno));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@Retval"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        public static void NilPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1NIL_EXT_SA", con);
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

        public static int Update(int nilid, decimal NilAmount, decimal ExemptedAmount, decimal NongstAmont, string SupplyType,int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_NIL_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@NILid ", nilid));
                cmd.Parameters.Add(new SqlParameter("@nil_amt", NilAmount));
                cmd.Parameters.Add(new SqlParameter("@expt_amt", ExemptedAmount));
                cmd.Parameters.Add(new SqlParameter("@ngsup_amt", NongstAmont));
                cmd.Parameters.Add(new SqlParameter("@sply_ty", SupplyType));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@Retval"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }
    }
}