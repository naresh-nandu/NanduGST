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
    public partial class GSTR1ViewDataModel
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUB2BRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetB2CL(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUB2CLRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDNR(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["ntid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUCDNRRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetB2CS(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["b2csid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUB2CSRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetEXP(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1EXP_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUEXPRefIds"] = RefIds;

                    conn.Close();

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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1HSN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["dataid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUHSNRefIds"] = RefIds;

                    conn.Close();

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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1NIL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["nilid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUNILRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXP(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1TXP_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txpid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUTXPRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }


        public static List<IDictionary> GetAT(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1AT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["atid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUATRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetDOC(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1DOCISSUE_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["docissueid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUDOCRefIds"] = RefIds;

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
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnurid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAUCDNURRefIds"] = RefIds;

                    conn.Close();

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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEB2BRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetB2CL_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEB2CLRefIds"] = RefIds;

                    conn.Close();
                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetCDNR_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1CDNR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["ntid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAECDNRRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetB2CS_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2CS_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["b2csid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEB2CSRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetEXP_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1EXP_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["invid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEEXPRefIds"] = RefIds;

                    conn.Close();

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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1HSN_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["dataid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEHSNRefIds"] = RefIds;

                    conn.Close();

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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1NIL_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["nilid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAENILRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetTXP_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1TXP_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["txpid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAETXPRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }


        public static List<IDictionary> GetAT_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1AT_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["atid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEATRefIds"] = RefIds;

                    conn.Close();

                    #endregion
                }
                catch
                {

                }
            }

            return ConvertToDictionary(ds.Tables[0]);

        }

        public static List<IDictionary> GetDOC_ER(string GSTINNo, string strFp, string strFlag)
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
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1DOCISSUE_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@Fp", strFp));
                    dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@Flag", strFlag));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable table = new DataTable();
                    ds.Clear();

                    da.Fill(ds);
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["docissueid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAEDOCRefIds"] = RefIds;

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
                    var das = ds.Tables[0].AsEnumerable();

                    // Taking RefIds for Update Flag Procedure
                    string RefIds = "";
                    if (ds.Tables[0].Rows.Count > 0)
                    {
                        for (int i = 0; i < ds.Tables[0].Rows.Count; i++)
                        {
                            RefIds += ds.Tables[0].Rows[i]["cdnurid"].ToString() + ",";
                        }
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["SAECDNURRefIds"] = RefIds;

                    conn.Close();

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