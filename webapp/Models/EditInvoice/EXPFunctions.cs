using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EditInvoice
{
    public class ExpFunctions
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected ExpFunctions()
        {
            //
        }

        public static string Insert(string Gstin, string fp,string Exp_Typ,string Inum, string Idate, decimal Val, string sbnum, string sbdt,string sbpcode, decimal Rate, decimal TaxVal, decimal iamt,string Refno, string hsncode, string itemdesc, decimal qty, decimal unitprice, decimal discount, string uqc,int Createdby, string AddInfo,string Address,int BuyerId,string SName)
        {
            string outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Insert_Outward_GSTR1_EXP_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp ", fp));
                cmd.Parameters.Add(new SqlParameter("@Exp_Typ ", Exp_Typ));
                cmd.Parameters.Add(new SqlParameter("@Inum ", Inum));
                cmd.Parameters.Add(new SqlParameter("@Idt ", Idate));
                cmd.Parameters.Add(new SqlParameter("@Val ", Val));
                cmd.Parameters.Add(new SqlParameter("@sbnum ", sbnum));
                cmd.Parameters.Add(new SqlParameter("@sbdt ", sbdt));
                cmd.Parameters.Add(new SqlParameter("@sbpcode ", sbpcode));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Txval", TaxVal));
                cmd.Parameters.Add(new SqlParameter("@Iamt", iamt));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo ", Refno));
                cmd.Parameters.Add(new SqlParameter("@Hsncode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@Hsndesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@Qty", qty));
                cmd.Parameters.Add(new SqlParameter("@UnitPrice", unitprice));
                cmd.Parameters.Add(new SqlParameter("@Discount", discount));
                cmd.Parameters.Add(new SqlParameter("@Uqc", uqc));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add(new SqlParameter("@Addinfo", AddInfo));
                cmd.Parameters.Add(new SqlParameter("@BuyerId", BuyerId));
                cmd.Parameters.Add(new SqlParameter("@ShippingAddress", Address));
                cmd.Parameters.Add(new SqlParameter("@ReceiverName", SName));
                cmd.Parameters.Add("@RetInum", SqlDbType.VarChar, 50).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToString(cmd.Parameters["@RetInum"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        public static void EXPPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1EXP_EXT_SA", con);
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

        public static int ItemUpdate(int Invid, string HsnCode, string Itemdesc, decimal Qty, decimal Unitprice, decimal Discount, string Uqc, decimal Rate, decimal Txval, decimal Iamt,int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_EXP_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Invid ", Invid));
                cmd.Parameters.Add(new SqlParameter("@Hsncode", HsnCode));
                cmd.Parameters.Add(new SqlParameter("@Hsndesc", Itemdesc));
                cmd.Parameters.Add(new SqlParameter("@Qty", Qty));
                cmd.Parameters.Add(new SqlParameter("@Unitprice", Unitprice));
                cmd.Parameters.Add(new SqlParameter("@Discount", Discount));
                cmd.Parameters.Add(new SqlParameter("@Uqc", Uqc));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Txval", Txval));
                cmd.Parameters.Add(new SqlParameter("@Iamt", Iamt));
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

        public static int MasterItemUpdate(string Gstin, string Fp, string Inum, string Idt, decimal Val, string sbnum, string sbdt, string sbpcode, string ShippingAddress,string Addinfo,string ReceiverName)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_EXP_Master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@Inum", Inum));
                cmd.Parameters.Add(new SqlParameter("@Idt", Idt));
                cmd.Parameters.Add(new SqlParameter("@Val", Val));
                cmd.Parameters.Add(new SqlParameter("@sbnum", sbnum));
                cmd.Parameters.Add(new SqlParameter("@sbdt",Convert.ToString(sbdt)));
                cmd.Parameters.Add(new SqlParameter("@sbpcode", sbpcode));
                cmd.Parameters.Add(new SqlParameter("@ShippingAddress", ShippingAddress));
                cmd.Parameters.Add(new SqlParameter("@Addinfo", Addinfo));
                cmd.Parameters.Add(new SqlParameter("@ReceiverName", ReceiverName));
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

        public static int ItemDelete(int ExpId)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_OUTWARD_GSTR1_EXP_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Invid", ExpId));
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