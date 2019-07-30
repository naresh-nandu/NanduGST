using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EditInvoice
{
    public class InwardFunctions
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public int B2BItemUpdate(int invid, decimal Rate, decimal TaxVal, decimal iamt, decimal camt, decimal samt, decimal csamt, string hsncode, string itemdesc, decimal qty, decimal unitprice, decimal discount, string uqc, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_INWARD_GSTR2_B2B_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Invid ", invid));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Txval", TaxVal));
                cmd.Parameters.Add(new SqlParameter("@Iamt", iamt));
                cmd.Parameters.Add(new SqlParameter("@Camt", camt));
                cmd.Parameters.Add(new SqlParameter("@Samt", samt));
                cmd.Parameters.Add(new SqlParameter("@Csamt", csamt));
                cmd.Parameters.Add(new SqlParameter("@Hsncode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@Hsndesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@Qty", qty));
                cmd.Parameters.Add(new SqlParameter("@Unitprice", unitprice));
                cmd.Parameters.Add(new SqlParameter("@Discount", discount));
                cmd.Parameters.Add(new SqlParameter("@Uqc", uqc));
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

        public int B2BItemDelete(int B2Bid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_INWARD_GSTR2_B2B_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Invid", B2Bid));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);
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