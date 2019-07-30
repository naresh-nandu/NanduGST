using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;

namespace WeP_BAL.ViewAndTrack
{
    public class ViewAndTrackStatus
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public readonly int _userid;
        public readonly string _filename;
        public readonly string _gstin;
        public readonly string _sourcetype;
        public readonly string _financialyear;
        public ViewAndTrackStatus(int UserId, int CustId, string FileName, string Gstin,string SourceType)
        {
            this._userid = UserId;
            this._custid = CustId;
            this._filename = FileName;
            this._gstin = Gstin;
            this._sourcetype = SourceType;
        }

        public ViewAndTrackStatus(int CustId,string SourceType,string financialYear)
        {
            this._custid = CustId;
            this._sourcetype = SourceType;
            this._financialyear = financialYear;
        }

        public  DataSet GetFilingStatus()
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_ViewAndTrackStatus", con);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", this._sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@FinancialYear", this._financialyear));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);

                    ds.Clear();
                    da.Fill(ds);
                }

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }

        public DataSet GetFilingStatus_CSV(string FileName)

        {

            DataSet ds = new DataSet();



            try

            {

                #region commented

                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))

                {

                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_ViewAndTrackStatus_CSV", con);

                    dCmd.CommandType = CommandType.StoredProcedure;

                    dCmd.CommandTimeout = 0;

                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));

                    dCmd.Parameters.Add(new SqlParameter("@SourceType", this._sourcetype));

                    dCmd.Parameters.Add(new SqlParameter("@FinancialYear", this._financialyear));

                    dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));

                    SqlDataAdapter da = new SqlDataAdapter(dCmd);



                    ds.Clear();

                    da.Fill(ds);

                }



                #endregion

            }

            catch (Exception ex)

            {

                Console.WriteLine(ex);

                throw;

            }

            return ds;

        }

        public DataTable DT_GSTR1_GetViewAndTrackStatus()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_ViewAndTrackStatus", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId",this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@SourceType",this._sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@FinancialYear",this._financialyear));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds.Tables[0];
        }

        public DataTable DT_GSTR3B_GetViewAndTrackStatus()
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_ViewAndTrackStatus", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    dCmd.Parameters.Add(new SqlParameter("@SourceType", this._sourcetype));
                    dCmd.Parameters.Add(new SqlParameter("@FinancialYear", this._financialyear));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();

                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ds.Tables[1];
        }

        public string insert_View_and_Track(string DecryptedJson)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Insert_JSON_ViewAndTrackStatus", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", this._custid));
                cmd.Parameters.Add(new SqlParameter("@Gstin", this._gstin));
                cmd.Parameters.Add(new SqlParameter("@FileName", this._filename));
                cmd.Parameters.Add(new SqlParameter("@SourceType", this._sourcetype));
                cmd.Parameters.Add(new SqlParameter("@RecordContents", DecryptedJson));
                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return "";
        }

        public static DataSet getCtin(int CustId, string ActionData)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {

                if (ActionData == "1")
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select GSTINno From TBL_Buyer  where CustomerId = @CustomerId AND RowStatus = 1 ";
                        sqlcmd.Parameters.AddWithValue("@CustomerId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }
                else
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select GSTINno From TBL_Supplier  where CustomerId = @CustomerId AND RowStatus = 1 ";
                        sqlcmd.Parameters.AddWithValue("@CustomerId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            return ds;
                        }
                    }
                }

            }
        }
    }
}

