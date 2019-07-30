using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;


namespace SmartAdminMvc.Models.EditInvoice
{
    public class B2CsFunctions
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected B2CsFunctions()
        {
            //
        }

        public static int B2CSItemUpdate(int Invid,string HsnCode,string Itemdesc,decimal Qty,decimal Unitprice,decimal Discount,string Uqc,decimal Rate,decimal Txval,decimal Iamt,decimal Camt,decimal Samt,decimal Csamt,int Createdby)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_B2CS_Items", con);
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
                cmd.Parameters.Add(new SqlParameter("@Camt", Camt));
                cmd.Parameters.Add(new SqlParameter("@Samt", Samt));
                cmd.Parameters.Add(new SqlParameter("@Csamt", Csamt));
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

        public static int B2CSMasterUpdate(string Gstin,string Fp,string Inum,string Idt,decimal Val,string Pos,string Etin,string ReferenceNo,int BuyerId,int CreatedBy,string Addinfo, string serviceType, string transMode, string vechicleNo, string dateOfSupply, string cinNo)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("Usp_Update_OUTWARD_GSTR1_B2CS_Master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@Inum", Inum));
                cmd.Parameters.Add(new SqlParameter("@Idt", Idt));
                cmd.Parameters.Add(new SqlParameter("@Val", Val));
                cmd.Parameters.Add(new SqlParameter("@Pos", Pos));
                cmd.Parameters.Add(new SqlParameter("@Etin", Etin));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                cmd.Parameters.Add(new SqlParameter("@BuyerId", BuyerId));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                cmd.Parameters.Add(new SqlParameter("@Addinfo", Addinfo));
                cmd.Parameters.Add(new SqlParameter("@GoodsAndService", serviceType));
                cmd.Parameters.Add(new SqlParameter("@TransportMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vechicleNo));
                cmd.Parameters.Add(new SqlParameter("@DateOfSupply", dateOfSupply));
                cmd.Parameters.Add(new SqlParameter("@CINno", cinNo));
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
        public static string Insert(string Gstin,string fp,string Inum,string Idate,decimal Val,string Pos,string Etin, decimal Rate, decimal TaxVal, decimal iamt, decimal camt, decimal samt, decimal csamt,string Refno, string hsncode, string itemdesc, decimal qty, decimal unitprice, decimal discount, string uqc,int buyerid, int Createdby,string AddInfo,string serviceType,string transMode,string vechicleNo,string dateOfSupply,string cinNo)
        {
            string outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_Outward_GSTR1_B2CS_INV_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp ", fp));
                cmd.Parameters.Add(new SqlParameter("@Inum ", Inum));
                cmd.Parameters.Add(new SqlParameter("@Idt ", Idate));
                cmd.Parameters.Add(new SqlParameter("@Val ", Val));
                cmd.Parameters.Add(new SqlParameter("@Pos ", Pos));
                cmd.Parameters.Add(new SqlParameter("@Etin ", Etin));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Txval", TaxVal));
                cmd.Parameters.Add(new SqlParameter("@Iamt", iamt));
                cmd.Parameters.Add(new SqlParameter("@Camt", camt));
                cmd.Parameters.Add(new SqlParameter("@Samt", samt));
                cmd.Parameters.Add(new SqlParameter("@Csamt", csamt));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo ", Refno));
                cmd.Parameters.Add(new SqlParameter("@Hsncode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@Hsndesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@Qty", qty));
                cmd.Parameters.Add(new SqlParameter("@UnitPrice", unitprice));
                cmd.Parameters.Add(new SqlParameter("@Discount", discount));
                cmd.Parameters.Add(new SqlParameter("@Uqc", uqc));
                cmd.Parameters.Add(new SqlParameter("@BuyerId",buyerid));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add(new SqlParameter("@Addinfo", AddInfo));
                cmd.Parameters.Add(new SqlParameter("@GoodsAndService",serviceType));
                cmd.Parameters.Add(new SqlParameter("@TransportMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vechicleNo));
                cmd.Parameters.Add(new SqlParameter("@DateOfSupply", dateOfSupply));
                cmd.Parameters.Add(new SqlParameter("@CINno", cinNo));
                cmd.Parameters.Add("@RetInum", SqlDbType.VarChar,50).Direction = ParameterDirection.Output;
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
        public static int B2CSItemDelete(int B2CLid)
        {
            int outputparam;
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Delete_OUTWARD_GSTR1_B2CS_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Invid", B2CLid));
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

        public static void B2CSPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1B2CS_N_EXT_SA", con);
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

        public static void B2BPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1B2B_EXT_SA", con);
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

        public static void B2CLPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1B2CL_EXT_SA", con);
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

        public static void CDNRPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1CDNR_EXT_SA", con);
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