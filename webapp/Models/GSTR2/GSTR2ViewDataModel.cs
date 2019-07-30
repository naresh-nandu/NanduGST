#region Using

using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web.Mvc;
using System.Web.Security;
using Microsoft.Ajax.Utilities;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.Owin.Security;
using System.Web.Http;
using Microsoft.Owin.Host.SystemWeb;
using System.Web;
using System.Data.SqlClient;
using System.Collections;
using System.Data;
using System.Configuration;
using System.Text;

#endregion

namespace SmartAdminMvc.Models.GSTR2
{
    public partial class GSTR2ViewDataModel
    {
        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();

            return dictionaryList.ToList<IDictionary>();
        }

        public static List<IDictionary> GetB2B(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetIMPG(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPG_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetIMPS(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDN(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetNIL(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2NIL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXI(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXI_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXPD(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXPD_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetHSN(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2HSN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }


        public static List<IDictionary> GetB2BUR(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2BUR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetITC(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_SA", conn);// need to add sp name here " usp_Retrieve_GSTR2ITCRVSL_SA"
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDNUR(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNUR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetB2B_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetIMPG_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPG_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetIMPS_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDN_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2CDN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetNIL_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2NIL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXI_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXI_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXPD_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2TXPD_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetHSN_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2HSN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }


        public static List<IDictionary> GetB2BUR_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2B2BUR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetITC_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2IMPS_SA", conn);// need to add sp name here " usp_Retrieve_GSTR2ITCRVSL_SA"
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDNUR_ER(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            //var modelList = new List<User>();
            // var result = new List<dynamic>();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNUR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    conn.Close();

                    var das = ds.Tables[0].AsEnumerable();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

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
                    dCmd.CommandTimeout = 7200;
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
                    throw ex;
                }
            }

            return returnJson;
        }
        
    }
}