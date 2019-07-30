using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.Report
{
    public partial class LogFilesDataModel
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected LogFilesDataModel()
        {
            //
        }

        public static DataSet GetDataSetOutwardLog(string CustRefNo, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_LOGS_OUTWARD", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));
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
            return ds;
        }
        public static DataSet GetDataSetInwardLog(string CustRefNo, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_LOGS_INWARD", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR6Import(string CustRefNo, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_LOGS_GSTR6", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR1UploadedData(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_UploadedData", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR1AmendUploadedData(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_Amendments_UploadedData", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR1Summary(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_SummaryReport", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR2ADownloadedData(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr2A_DownloadedData", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetReconciliationSummary(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetReconciliationStatus(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Status", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetReconciliationStatusGSTR6(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Status_GSTR6", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR6ReconciliationSummary(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_GSTR6_Reconciliation_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }
       
        public static DataSet GetDataSetGSTR2UploadedData(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr2_UploadedData", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }
       
        public static DataSet GetDataSetGSTR6UploadData(string CustId, string UserId, string Period)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr6_UploadedData", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Period", Period));
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
            return ds;
        }


        public static DataSet GetDataSetTally(string SourceType, string Refno, string records)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Insert_JSON_GSTR1_EXT_TALLY", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", SourceType));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", Refno));
                dCmd.Parameters.Add(new SqlParameter("@RecordContents", records));
                dCmd.Parameters.Add(new SqlParameter("@TotalRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                dCmd.Parameters.Add(new SqlParameter("@ProcessedRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                dCmd.Parameters.Add(new SqlParameter("@ErrorRecordsCount", SqlDbType.Int, 10)).Direction = System.Data.ParameterDirection.Output;
                dCmd.Parameters.Add(new SqlParameter("@ErrorRecords", SqlDbType.NVarChar, 1000)).Direction = System.Data.ParameterDirection.Output;
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
            return ds;
        }

        #region "MultiPeriod and Multi GSTIN"

        public static DataSet GetDataSetGSTR6UploadDataMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr6_UploadedData_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR2UploadedDataMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr2_UploadedData_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR6ReconciliationSummaryMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_GSTR6_Reconciliation_Summary_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetReconciliationStatusGSTR6MultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Status_GSTR6MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetReconciliationStatusMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Status_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetReconciliationSummaryMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Summary_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetGSTR2ADownloadedDataMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr2A_DownloadedData_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetGSTR1SummaryMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_SummaryReport_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetGSTR1AmendUploadedDataMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_Amendments_UploadedData_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetGSTR1UploadedDataMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Gstr1_UploadedData_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR1DownloadLogMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_Download_LOG", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR6DownloadLogMultiPeriod(string CustId, string UserId, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_Download_LOG", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR6ImportMultiPeriod(string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_GSTR6_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetInwardLogMultiPeriod(string custid,string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Inward_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@CustId", custid));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR3BDownloadMultiPeriod(string custid,string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Download_Log", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
               
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@CustId", custid));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }

        public static DataSet GetDataSetGSTR3BUploadMultiPeriod(string custid,string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR3B_Uploaded_Log", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
               
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                dCmd.Parameters.Add(new SqlParameter("@CustId", custid));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
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
            return ds;
        }


        public static DataSet GetDataSetOutwardLogMultiPeriod(string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Outward_MultiPeriod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));

                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);

                ds.Clear();
                da.Fill(ds);
                con.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return ds;
        }

        public static DataSet GetDataSetOutwardAmendmentLogMultiPeriod(string CustRefNo, string FromPeriod, string ToPeriod, string GSTIN)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_LOGS_OUTWARD_Amendments_Multiperiod", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", CustRefNo));

                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));

                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", FromPeriod));

                dCmd.Parameters.Add(new SqlParameter("@toPeriod", ToPeriod));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);

                ds.Clear();
                da.Fill(ds);
                con.Close();

                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);

            }
            return ds;
        }



        #endregion

    }
}