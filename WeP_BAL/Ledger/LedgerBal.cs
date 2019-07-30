using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.Ledger
{
   public class LedgerBal
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected LedgerBal()
        {
            //
        }

        #region "Inserting ITC and Ledger Details"
        public static void Insert_ITC_Ledger(string Gstin, string Fp, decimal Ledger_Sgst,
            decimal Ledger_Cgst, decimal Ledger_Igst, decimal Ledger_Cess, decimal Itc_Sgst, 
            decimal Itc_Cgst,decimal Itc_Igst, decimal Itc_Cess, int CustId, int UserId)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_insert_itc_ledger", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin",Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp",Fp));
                cmd.Parameters.Add(new SqlParameter("@Ledger_Sgst",Ledger_Sgst));
                cmd.Parameters.Add(new SqlParameter("@Ledger_Cgst",Ledger_Cgst));
                cmd.Parameters.Add(new SqlParameter("@Ledger_Igst", Ledger_Igst));
                cmd.Parameters.Add(new SqlParameter("@Ledger_Cess",Ledger_Cess));
                cmd.Parameters.Add(new SqlParameter("@Itc_Sgst", Itc_Sgst));
                cmd.Parameters.Add(new SqlParameter("@Itc_Cgst", Itc_Cgst));
                cmd.Parameters.Add(new SqlParameter("@Itc_Igst", Itc_Igst));
                cmd.Parameters.Add(new SqlParameter("@Itc_Cess", Itc_Cess));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }
        #endregion
    }
}
