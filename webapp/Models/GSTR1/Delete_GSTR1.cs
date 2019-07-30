using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class Delete_GSTR1
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        string RefIds = "";
        public void GSTR1_Checksum(string strActionType, string StrGSTINNo, string strPeriod, int[] ids)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                foreach (int invid in ids)
                {
                    RefIds += invid.ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');

                SqlCommand cmd = new SqlCommand("usp_Compute_Chksum_GSTR1_Invoices", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                cmd.Parameters.Add(new SqlParameter("@Flag", "U"));
                cmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));

                //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public void GSTR1DeleteALL(string strActionType, string strGstin, string strFp, string strRefIds)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand("usp_Delete_All_GSTR1_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", strGstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                cmd.Parameters.Add(new SqlParameter("@RefIds", strRefIds));
                //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }


        public void Update_Invoice_Flag_GSTR1(string ActionType, string strGSTINNo, string strPeriod, int[] RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    string ids = "";
                    foreach (int invid in RefIds)
                    {
                        ids += invid.ToString() + ",";
                    }
                    ids = ids.TrimEnd(',');

                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", ids));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }

        public void Update_Invoice_Flag_GSTR1_ALL(string ActionType, string strGSTINNo, string strPeriod, string RefIds, string CurFlag, string NewFlag)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Update_Invoice_Flag_GSTR1", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@RefIds", RefIds));
                    dCmd.Parameters.Add(new SqlParameter("@CurFlag", CurFlag));
                    dCmd.Parameters.Add(new SqlParameter("@NewFlag", NewFlag));
                    dCmd.Connection = conn;
                    int i = dCmd.ExecuteNonQuery();
                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }
        }
    }
}