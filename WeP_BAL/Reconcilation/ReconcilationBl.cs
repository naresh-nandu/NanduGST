using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.Reconcilation
{
  public static class ReconcilationBl
    {
       
        public readonly static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public static DataSet GetDataSetReconciliationLog(int strCustId,int strUserId,string strGstin,string strSupplierName,string strCtin,string strFromPeriod,string strToPeriod)
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
                dCmd.Parameters.Add(new SqlParameter("@CustId",strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId",strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin",strGstin));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName",strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin",strCtin));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod",strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@toPeriod",strToPeriod));
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

        public static DataSet GetDataSetReconciliationLog_IdtWise(int strCustId, int strUserId, string strGstin, string strSupplierName, string strCtin,string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Logs_Reconciliation_Summary_MultiPeriod_Idt_Wise", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", strGstin));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
               // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                dCmd.Parameters.Add(new SqlParameter("@fromdate", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@todate", strToPeriod));
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

       

        public static DataSet GetDataSetReconciliationLog_PanWise(int strCustId, int strUserId, string strSupplierName, string strCtin, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_DataCorrection_Based_Panno", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Panno", ""));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
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

        public static DataSet GetDataSetReconciliationLog_PanWise_IdtWise(int strCustId, int strUserId, string strSupplierName, string strCtin, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_RetrieveMismatchedInvoicesInGSTR2andGSTR2A_DataCorrection_Based_Panno_IdtWise", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Panno", ""));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
               // dCmd.Parameters.Add(new SqlParameter("@FY", year));
                dCmd.Parameters.Add(new SqlParameter("@fromdate", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@todate", strToPeriod));
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


        public static DataSet GetDataSetReconciliationLog_GSTR6(int strCustId, int strUserId, string strGstin, string strSupplierName, string strCtin, string strFromPeriod, string strToPeriod)
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
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", strGstin));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
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

        public static void ReconciliationReset(int strCustId, int strUserId, string strGstin, string strSupplierName, string strCtin, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Update_GStr2_Reconciliation_Reset", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", strGstin));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", "ALL"));
                dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
                
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
        }

        public static void ReconciliationReset_GSTR6(int strCustId, int strUserId, string strGstin, string strSupplierName, string strCtin, string strFromPeriod, string strToPeriod)
        {
            DataSet ds = new DataSet();

            try
            {
                #region commented
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand dCmd = new SqlCommand("usp_Update_GStr6_Reconciliation_Reset", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", strUserId));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", strGstin));
                dCmd.Parameters.Add(new SqlParameter("@SupplierName", strSupplierName));
                dCmd.Parameters.Add(new SqlParameter("@Ctin", strCtin));
                dCmd.Parameters.Add(new SqlParameter("@fromPeriod", strFromPeriod));
                dCmd.Parameters.Add(new SqlParameter("@toPeriod", strToPeriod));
                dCmd.Parameters.Add(new SqlParameter("@ActionType", "ALL"));
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
        }
    }
}
