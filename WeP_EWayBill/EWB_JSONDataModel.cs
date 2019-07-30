using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_EWayBill
{
    public class EWB_JSONDataModel
    {
        public static string GetJsonEWBGeneration(string strGSTINNo, string strDocNo, string strDocDate)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_EWB_Generation", conn);
                    dCmd.CommandTimeout = 7200;
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@DocNo", strDocNo));
                    dCmd.Parameters.Add(new SqlParameter("@DocDate", strDocDate));
                    //dCmd.Parameters.Add(new SqlParameter("@JsonResult", SqlDbType.NVarChar,1000000000)).Direction = ParameterDirection.Output;
                    
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

        public static string GetJsonCONSEWBGeneration(string strGSTINNo, string strtransDocNo, string strtransDocDate)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_CONS_EWB_Generation", conn);
                    dCmd.CommandTimeout = 7200;
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@TransDocNo", strtransDocNo));
                    dCmd.Parameters.Add(new SqlParameter("@TransDocDate", strtransDocDate));
                    //dCmd.Parameters.Add(new SqlParameter("@JsonResult", SqlDbType.NVarChar,1000000000)).Direction = ParameterDirection.Output;

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

        public static string GetJsonEWBUpdateVehicleNo(string strGSTINNo, string strEwbNo, string strVehicleNo)
        {
            DataSet ds = new DataSet();
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Construct_JSON_EWB_UPDATE_VEHICLENO", conn);
                    dCmd.CommandTimeout = 7200;
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.Parameters.Add(new SqlParameter("@userGSTIN", strGSTINNo));
                    dCmd.Parameters.Add(new SqlParameter("@EwbNo", strEwbNo));
                    dCmd.Parameters.Add(new SqlParameter("@VehicleNo", strVehicleNo));
                    //dCmd.Parameters.Add(new SqlParameter("@JsonResult", SqlDbType.NVarChar,1000000000)).Direction = ParameterDirection.Output;

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
