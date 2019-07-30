using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SmartAdminMvc.Models.GSTR4
{
    public class Gstr4DataModel
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


        #region "GSTR 4"

        public static List<IDictionary> GetB2B4(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4B2B_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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
                    HttpContext.Current.Session["G4B2BRefIds"] = RefIds;
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

        public static List<IDictionary> GetB2BA4(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4B2BA_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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
                    HttpContext.Current.Session["G4B2BARefIds"] = RefIds;
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

        public static List<IDictionary> GetCDNR4(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4CDNR_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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
                    HttpContext.Current.Session["G4CDNRRefIds"] = RefIds;
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

        public static List<IDictionary> GetCDNRA4(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4CDNRA_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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
                    HttpContext.Current.Session["G4CDNRARefIds"] = RefIds;
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

        public static List<IDictionary> GetTDS4(string GSTINNo, string strFp, string strFlag)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR4ISD_SA", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
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
                            myBuilder.Append(ds.Tables[0].Rows[i]["tdsid"].ToString() + ",");
                        }
                        RefIds = myBuilder.ToString();
                    }
                    RefIds = RefIds.TrimEnd(',');
                    HttpContext.Current.Session["G4TDSRefIds"] = RefIds;
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


        public static List<IDictionary> GetListGSTR4File(string GSTINNo, string ActionType, string fp)
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


        public static string GetJsonGSTR4Save(string GSTINNo, string fp, string strAction, string strFlag)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_GSTR4_Save", conn);
                    dCmd.CommandTimeout = 7200;
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

        //public static string GetJsonGSTRFile(string ActionType, string GSTINNo, string fp)
        //{
        //    DataSet ds = new DataSet();
        //    string returnJson = "";
        //    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
        //    {
        //        try
        //        {
        //            #region commented
        //            conn.Open();
        //            SqlCommand dCmd = new SqlCommand("Json_For_GSTR2_Save", conn);
        //            dCmd.CommandType = CommandType.StoredProcedure;
        //            dCmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
        //            dCmd.Parameters.Add(new SqlParameter("@GSTINNo", GSTINNo));
        //            dCmd.Parameters.Add(new SqlParameter("@fp", fp));
        //            SqlDataAdapter da = new SqlDataAdapter(dCmd);
        //            DataTable dt = new DataTable();
        //            ds.Clear();

        //            da.Fill(dt);
        //            conn.Close();

        //            if (dt.Rows.Count > 0)
        //            {
        //                returnJson = dt.Rows[0][0].ToString();
        //            }
        //            else
        //            {
        //                returnJson = "";
        //            }

        //            #endregion
        //        }
        //        catch (Exception ex)
        //        {
        //            Console.WriteLine(ex);
        //            throw;
        //        }
        //    }
        //    return returnJson;
        //}
    }
}