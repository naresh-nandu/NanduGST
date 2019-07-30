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

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class Gstr1DataModel
    {
        protected Gstr1DataModel()
        {
            //
        }

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

        
        public static List<IDictionary> GetB2CSInv(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_INV_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);
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
                    HttpContext.Current.Session["B2CSInvRefIds"] = RefIds;

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

        public static List<IDictionary> B2CSInvOther(string GSTINNo, string strFp, string strRefNo)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_Others_EXT", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GstinNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", strRefNo));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    ds.Clear();
                    da.Fill(ds);

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
                    HttpContext.Current.Session["B2CSInvOtherRefIds"] = RefIds;

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

        

        public static List<IDictionary> GetListGSTR1File(string strGSTINNo, string strPeriod)
        {
            DataTable dtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR1Summary where gstin = @GSTINNo and ret_period = @Period";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strPeriod);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.CommandTimeout = 0;
                                    sqlcmd1.Connection = conn;
                                    sqlcmd1.CommandText = "Select * from TBL_GSTR1SummaryAction where gstr1sumid = @GSTR1SumId";
                                    sqlcmd1.Parameters.AddWithValue("@GSTR1SumId", dt.Rows[0]["gstr1sumid"].ToString());
                                    using (SqlDataAdapter da1 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        da1.Fill(dtable);
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dtable);
        }

        public static DataTable GetDataTableListGSTR1Summary(string strGSTINNo, string strPeriod)
        {
            DataTable dtable = new DataTable();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.CommandTimeout = 0;
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR1Summary where gstin = @GSTINNo and ret_period = @Period";
                        sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                        sqlcmd.Parameters.AddWithValue("@Period", strPeriod);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.Connection = conn;
                                    sqlcmd1.CommandText = "Select sec_nm as 'Action', ttl_rec as 'Total Records', ttl_val as 'Total Value', ttl_tax as 'Total Taxable Amt', ttl_igst as 'Total IGST Amt', ttl_cgst as 'Total CGST Amt', ttl_sgst as 'Total SGST Amt', ttl_cess as 'Total CESS Amt', ttl_nilsup_amt as 'Total NIL Supply Amt', ttl_expt_amt as 'Total NIL Exempted Amt', ttl_ngsup_amt as 'Total NonGST Supply Amt', ttl_doc_issued as 'Total DOC Issued', ttl_doc_cancelled as 'Total DOC Cancelled', net_doc_issued as 'Total NET DOC Issued' from TBL_GSTR1SummaryAction where gstr1sumid = @GSTR1SumId";
                                    sqlcmd1.Parameters.AddWithValue("@GSTR1SumId", dt.Rows[0]["gstr1sumid"].ToString());
                                    using (SqlDataAdapter da1 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        da1.Fill(dtable);
                                    }
                                }
                            }
                        }
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return dtable;
        }

        public static string GetJsonGSTR1Save(string GSTINNo, string fp, string strAction, string strFlag)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_GSTR1_Save", conn);
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

        public static string GetJsonGSTR1File(string GSTINNo, string fp)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_GSTR1_RETFILE", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Fp", fp));
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

    }
}