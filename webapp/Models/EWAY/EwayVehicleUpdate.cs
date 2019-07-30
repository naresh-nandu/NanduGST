using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.EWAY
{
    public class EwayVehicleUpdate
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
    

        internal void EwayBillVehicleUpdate(string strEwbNo, string strVehicleNo, string strFromPlace, string strFromState, string strReasonCode,
            string strReasonRem, string strTransDocNo, string strTransDocDate, string strTransMode, string SessionUserId, string SessionCustId)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@EwbNo", SqlDbType.NVarChar).Value = strEwbNo;
                cmd.Parameters.Add("@vehicleNo", SqlDbType.NVarChar).Value = strVehicleNo;
                cmd.Parameters.Add("@FromPlace", SqlDbType.NVarChar).Value = strFromPlace;
                cmd.Parameters.Add("@FromState", SqlDbType.NVarChar).Value = strFromState;
                cmd.Parameters.Add("@ReasonCode", SqlDbType.NVarChar).Value = strReasonCode;
                cmd.Parameters.Add("@ReasonRem", SqlDbType.NVarChar).Value = strReasonRem;
                cmd.Parameters.Add("@transMode", SqlDbType.NVarChar).Value = strTransMode;
                cmd.Parameters.Add("@transDocNo", SqlDbType.NVarChar).Value = strTransDocNo;
                cmd.Parameters.Add("@transDocDate", SqlDbType.NVarChar).Value = strTransDocDate;                
                cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = SessionUserId;
                cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = SessionCustId;
                Functions.InsertIntoTable("TBL_EWB_UPDATE_VEHICLENO", cmd, con);
                con.Close();
            }

            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}