using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Web.Mvc;

namespace WeP_BAL.GSTRDelete
{
    public class GstrCommonDeleteBal
    {
        private static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public readonly int _userid;
        public GstrCommonDeleteBal(int CustId, int UserId)
        {
            this._custid = CustId;
            this._userid = UserId;
        }

        public static string GetJsonGSTR1_View(string strGSTINNo, string strFp, string strAction, string strFlag)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_GSTR1_Update", conn);
                    dCmd.CommandTimeout = 0;
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", strAction));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    var reader = dCmd.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            MyStringBuilder.Append(reader.GetValue(0));
                        }
                    }
                    reader.Close();
                    conn.Close();
                    returnJson = MyStringBuilder.ToString();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }

            return returnJson;
        }

        public DataSet Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary(string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;

        }

        public DataSet Retrieve_GSTR1_Delete_EXT_SA_Bulk_Rawdata(string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Delete_EXT_SA_Bulk_RawData", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                cmd.Parameters.Add(new SqlParameter("@SourceType", "ALL"));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;

        }


        public void Delete_GSTR1_Delete_EXT_SA_Bulk(string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_SA_Bulk", conn);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.CommandTimeout = 0;
                    cmd.Parameters.Add(new SqlParameter("@CustId", this._custid));
                    cmd.Parameters.Add(new SqlParameter("@UserId", this._userid));
                    cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                    cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                    cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                    cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                    cmd.Parameters.Add(new SqlParameter("@SourceType", "ALL"));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                    cmd.ExecuteNonQuery();
                    conn.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
    }
}
