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

namespace SmartAdminMvc.Models.GSTR6
{
    public partial class Gstr6DataModel
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
        

        #region "GSTR 6"

        public static List<IDictionary> GetB2B6(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G6B2BRefIds"] = RefIds;
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

        public static List<IDictionary> GetB2BA6(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6B2BA_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G6B2BARefIds"] = RefIds;
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

        public static List<IDictionary> GetCDNR6(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6CDNR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G6CDNRRefIds"] = RefIds;
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

        public static List<IDictionary> GetCDNRA6(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6CDNRA_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["invid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G6CDNRARefIds"] = RefIds;
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

        public static List<IDictionary> GetISD6(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6ISD_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
                    conn.Close();
                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    StringBuilder myBuilder = new StringBuilder();
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            myBuilder.Append(ds.Tables[0].Rows[i]["isdid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G6ISDRefIds"] = RefIds;
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
        #endregion


        public static List<IDictionary> GetListGSTR6File(string GSTINNo, string ActionType, string fp)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Retrieve_GSTR2_FilingSummary", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@fp", fp));
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


        public static string GetJsonGSTR6Save(string GSTINNo, string fp, string strAction, string strFlag)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_GSTR6_Save", conn);
                    dCmd.CommandTimeout = 0;
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
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

        public static string GetJsonGSTR6File(string ActionType, string GSTINNo, string fp)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("Json_For_GSTR2_Save", conn);
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