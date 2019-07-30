using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WeP_DAL.GSTRUpload;

namespace WeP_BAL.GSTRUpload
{
    public partial class GstrUploadBal
    {
        readonly static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public readonly int _userid;
        public readonly string _useremail;
        public readonly string _referenceno;
        public GstrUploadBal(int CustId, int UserId, string UserEmail, string ReferenceNo)
        {
            this._custid = CustId;
            this._userid = UserId;
            this._useremail = UserEmail;
            this._referenceno = ReferenceNo;
        }


        #region "Retrieve Procedure for File imported time and immediately need to call by passing file details"
        public List<IDictionary> Retrieve_CSV_Data(string GstrType, string FileName, string TemplateTypeId, string sourcetype)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_CSV_Imported", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                dCmd.Parameters.Add(new SqlParameter("@UserId", this._useremail));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", this._referenceno));
                dCmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", sourcetype));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", this._userid));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }
        
        #endregion


        #region "Retrieve Procedure for File imported time and immediately need to call by passing file details"
        public static List<IDictionary> Retrieve_TALLY_Data(string GstrType, string FileName, string UserId, string ReferenceNo, string TemplateTypeId, int CustId, int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_CSV_Imported", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion


        #region "Delete procedure for File Imported  summary data delete"
        public void Delete_CSV_Data(string GstrType, string FileName, string ActionType, string Gstin, string Fp, string TemplateTypeId, string sourcetype)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_CSV_Imported", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._useremail));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", this._referenceno));
                cmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", sourcetype));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", this._userid));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion


        #region "Delete procedure for File Imported  summary data delete Tally"
        public void Delete_TALLY_Data(string GstrType, string FileName, string ActionType, string Gstin, string Fp, string TemplateTypeId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_CSV_Imported", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._useremail));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", this._referenceno));
                cmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", this._userid));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion


        #region "Retrieve procedure for Displaying data after click on "View Complete summary" button in csv upload page itself, under the first grid"
        public static List<IDictionary> Retrieve_View_Summary(string GstrType, string UserId, string ReferenceNo, int CustId, string sourcetype, int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", sourcetype));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion


        #region "Retrieve procedure for Displaying data after click on "View Complete summary" button in GSTN/Tally page itself, under the first grid"
        public static List<IDictionary> Retrieve_View_Summary_Tallly(string GstrType, string UserId, string ReferenceNo, int CustId, int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion


        #region "Delete Procedure for Deleteing data which is displaying after click on "View Complete summary" button in csv upload page itself, under the first grid."
        public void Delete_View_Sammary_Data(string GstrType, string ActionType, string Gstin, string Fp, string sourcetype)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._useremail));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", this._referenceno));
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", sourcetype));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", this._userid));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion


        #region "Delete Procedure for Deleteing data which is displaying after click on "View Complete summary" button in tally upload page itself, under the first grid."
        public void Delete_View_Sammary_Tally_Data(string GstrType, string ActionType, string Gstin, string Fp)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._useremail));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", this._referenceno));
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", this._userid));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion



        #region "Retrieve procedure for Displaying data after click on "View Complete summary" button in csv upload page itself, under the first grid"
        public static List<IDictionary> Retrieve_View_Summary_GSTR9(string ReferenceNo, int CustId,int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR9_Summary_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;                           
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));              
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion

        #region "Delete Procedure for Deleteing data which is displaying after click on "View Complete summary" button in csv upload page itself, under the first grid."
        public void Delete_View_Sammary_Data_GSTR9(string Gstin, string Fp)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR9_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;                
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));                       
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        #endregion

        public static DataSet DT_Retrieve_TALLY_Data(string GstrType, string FileName, string UserId, string ReferenceNo, string TemplateTypeId, int CustId, int CreatedBy)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }

                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_CSV_Imported", con);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                    dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                    dCmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    con.Close();
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds;
        }

        public static DataTable DT_Retrieve_View_Summary_Tallly(string GstrType, string UserId, string ReferenceNo, int CustId, int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds.Tables[1];
        }

        public static DataTable DT_Retrieve_View_Summary_CSV(string GstrType, string UserId, string ReferenceNo, int CustId, string sourcetype, int CreatedBy)
        {
            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", sourcetype));
                dCmd.Parameters.Add(new SqlParameter("@CreatedBy", CreatedBy));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds.Tables[1];
        }



        #region "Push Procedures for GSTR1 and GSTR1 Amendments"

        public static void PushGSTR1_B2CS(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2CS_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2CS_INV(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2CS_N_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2B(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2B_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2CL(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2CL_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_EXP(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1EXP_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_CDNR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1CDNR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_CDNUR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1CDNUR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_AT(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1AT_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_TXP(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1TXP_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_NIL(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1NIL_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_HSN(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1HSNSUM_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_DOCISSUE(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1DOCISSUE_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2BA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2BA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2CLA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2CLA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_B2CSA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1B2CSA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_EXPA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1EXPA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_CDNRA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1CDNRA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_CDNURA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1CDNURA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_TXPA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1TXPA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR1_ATA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR1ATA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        
        #endregion


        #region "Push Procedures for GSTR2"

        public static void PushGSTR2_B2B(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2B2B_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_B2BUR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2B2BUR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_IMPG(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2IMPG_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_IMPS(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2IMPS_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_CDN(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2CDN_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_CDNUR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2CDNUR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_HSN(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2HSN_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_NIL(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2NIL_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_TXPD(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2TXPD_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_TXI(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2TXI_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR2_ITCRVSL(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR2ITCRVSL_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        #endregion


        #region "Push Procedures for GSTR6"

        public static void PushGSTR6_B2B(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR6B2B_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR6_CDN(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR6CDN_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR6_B2BA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR6B2BA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR6_CDNA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR6CDNA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR6_ISD(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR6ISD_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR9(string refno, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR9_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;                   
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));                  
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        #endregion

        #region "GSTR7 Push Procedures"
        public static void PushGSTR7_TDS(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR7TDS_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        public static void PushGSTR7_TDSA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR7TDSA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        #endregion

        #region "Push Procedures for GSTR4"
        public static void PushGSTR4_B2B(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4B2B_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_CDNR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4CDNR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        
        public static void PushGSTR4_CDNUR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4CDNUR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_AT(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4AT_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_TXP(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4TXP_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_B2BUR(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4B2BUR_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_IMPS(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4IMPS_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_TXOS(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4TXOS_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_B2BA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4B2BA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_CDNRA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4CDNRA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_CDNURA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4CDNURA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_ATA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4ATA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_TXPA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4TXPA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_B2BURA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4B2BURA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_IMPSA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4IMPSA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void PushGSTR4_TXOSA(string sourcetype, string refno, string GSTINNo, int custid, int userid)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Push_GSTR4TXOSA_EXT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refno));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", custid));
                    dCmd.Parameters.Add(new SqlParameter("@CreatedBy", userid));
                    dCmd.ExecuteNonQuery();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
        #endregion


        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
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
