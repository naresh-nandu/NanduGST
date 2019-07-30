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
    public class AdvanceTaxFunctions
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public static int Insert(string Gstin, string fp, string Action, string Pos, decimal Rate, decimal AdvanceAmount, decimal Igst,decimal Cgst,decimal Sgst,decimal Cess, string Refno, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Insert_Outward_GSTR1_AT_TXP_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp ", fp));
                cmd.Parameters.Add(new SqlParameter("@Action", Action));
                cmd.Parameters.Add(new SqlParameter("@Pos", Pos));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@AdvanceAmount", AdvanceAmount));
                cmd.Parameters.Add(new SqlParameter("@Igst", Igst));
                cmd.Parameters.Add(new SqlParameter("@Cgst", Cgst));
                cmd.Parameters.Add(new SqlParameter("@Sgst", Sgst));
                cmd.Parameters.Add(new SqlParameter("@Cess", Cess));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo ", Refno));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.Output;
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

        public static void ATPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1AT_EXT_SA", con);
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

        public static int ATUpdate(int atid,string Mode,string Pos, decimal Rate, decimal AdvanceAmount, decimal Igst, decimal Cgst, decimal Sgst, decimal Cess,int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_AT_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ATid", atid));
                cmd.Parameters.Add(new SqlParameter("@Mode", Mode));
                cmd.Parameters.Add(new SqlParameter("@Pos", Pos));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Ad_amt", AdvanceAmount));
                cmd.Parameters.Add(new SqlParameter("@Iamt", Igst));
                cmd.Parameters.Add(new SqlParameter("@Camt", Cgst));
                cmd.Parameters.Add(new SqlParameter("@Samt", Sgst));
                cmd.Parameters.Add(new SqlParameter("@Csamt", Cess));
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

        public static int ATDelete(int atid, string Mode, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_AT_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@ATid", atid));
                cmd.Parameters.Add(new SqlParameter("@Mode", Mode));
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

        public static int TXPUpdate(int txpid, string Mode, string Pos, decimal Rate, decimal AdvanceAmount, decimal Igst, decimal Cgst, decimal Sgst, decimal Cess, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_TXP_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@TXPid", txpid));
                cmd.Parameters.Add(new SqlParameter("@Mode", Mode));
                cmd.Parameters.Add(new SqlParameter("@Pos", Pos));
                cmd.Parameters.Add(new SqlParameter("@Rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@Ad_amt", AdvanceAmount));
                cmd.Parameters.Add(new SqlParameter("@Iamt", Igst));
                cmd.Parameters.Add(new SqlParameter("@Camt", Cgst));
                cmd.Parameters.Add(new SqlParameter("@Samt", Sgst));
                cmd.Parameters.Add(new SqlParameter("@Csamt", Cess));
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

        public static int TXPDelete(int txpid, string Mode, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_TXP_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@TXPid", txpid));
                cmd.Parameters.Add(new SqlParameter("@Mode ", Mode));
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

        public static void TXPPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1TXP_EXT_SA", con);
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
        public  List<IDictionary> Getinvoice(string GSTIN,string FP,int CreatedBy)
        {
            DataSet ds = new DataSet();

            try
            {

                #region commented
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Outward_AT_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GSTIN",GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@FP", FP));
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();

                da.Fill(ds);
                con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetTXPinvoice(string GSTIN, string FP, int CreatedBy)
        {
            DataSet ds = new DataSet();

            try
            {

                #region commented
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Outward_TXP_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GSTIN", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@FP", FP));
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();

                da.Fill(ds);
                con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetDOCinvoice(string GSTIN, string FP, int CreatedBy)
        {
            DataSet ds = new DataSet();

            try
            {

                #region commented
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Outward_DocIssue_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GSTIN", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@FP", FP));
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();

                da.Fill(ds);
                con.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        public List<IDictionary> GetNilinvoice(string GSTIN, string FP, int CreatedBy,string Type)
        {
            DataSet ds = new DataSet();

            try
            {

                #region commented
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Outward_NilRated_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GSTIN", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@FP", FP));
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                dCmd.Parameters.Add(new SqlParameter("@SupplyType", Type));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();

                da.Fill(ds);
                con.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }
    }
}