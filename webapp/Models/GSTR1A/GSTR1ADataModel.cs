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

#endregion

namespace SmartAdminMvc.Models.GSTR1A
{
    public partial class GSTR1ADataModel
    {
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

        public static List<IDictionary> GetListGSTR1ASave(string GSTINNo, string ActionType)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Retrieve_Upload_GSTR1_ByAction", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
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

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetListGSTR1AFile(string GSTINNo, string ActionType, string Period)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Retrieve_GSTR1_FilingSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", Period));
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
            return ConvertToDictionary(ds.Tables[0]);
        }

        public static string GetJsonGSTR1ASave(string ActionType, string GSTINNo, string fp)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Json_For_GSTR1_Save", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();
                    ds.Clear();

                    da.Fill(dt);
                    conn.Close();

                    if (dt.Rows.Count > 0)
                    {
                        returnJson = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        returnJson = "";
                    }

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

        public static string GetJsonGSTR1AFile(string ActionType, string GSTINNo, string fp)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Json_For_GSTR1_Save", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", fp));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();
                    ds.Clear();

                    da.Fill(dt);
                    conn.Close();

                    if (dt.Rows.Count > 0)
                    {
                        returnJson = dt.Rows[0][0].ToString();
                    }
                    else
                    {
                        returnJson = "";
                    }

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
    }
}