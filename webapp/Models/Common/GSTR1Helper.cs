using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public partial class GSTR1Helper
    {
        public static void Update_Invoice_Flag_GSTR1(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
                    conn.Close();
                }
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public static void Update_Invoice_Flag_GSTR1(string ActionType, string strGSTINNo, string strPeriod, string CurFlag, string NewFlag,
            string strMinInvId, string strMaxInvId)
        {
            try
            {
                #region commented
                using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1_Bulk", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@MinInvId", strMinInvId));
                    dCmd.Parameters.Add(new SqlParameter("@MaxInvId", strMaxInvId));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    dCmd.ExecuteNonQuery();
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