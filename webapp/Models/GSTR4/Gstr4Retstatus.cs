using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR4
{
    public class Gstr4Retstatus
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);


        public static void Update_GSTR4_RetStatus(string GSTINNo, string Period, string RefNo, string Action, string RefIds, string Status, string error_msg, string CustId, string UserId)
        {
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = GSTINNo;
            cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = Period;
            cmd.Parameters.Add("@referenceno", SqlDbType.VarChar).Value = RefNo;
            cmd.Parameters.Add("@actiontype", SqlDbType.VarChar).Value = Action;
            cmd.Parameters.Add("@refids", SqlDbType.NVarChar).Value = RefIds;
            cmd.Parameters.Add("@status", SqlDbType.NVarChar).Value = Status;
            cmd.Parameters.Add("@errorreport", SqlDbType.NVarChar).Value = error_msg;
            cmd.Parameters.Add("@customerid", SqlDbType.Int).Value = CustId;
            cmd.Parameters.Add("@createdby", SqlDbType.Int).Value = UserId;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now;

            if (Status == "1")
            {
                Common.Functions.InsertIntoTable("TBL_GSTR4_SAVE_RETSTATUS", cmd, con);
            }
            else
            {
                Common.Functions.UpdateTable("TBL_GSTR4_SAVE_RETSTATUS", "referenceno", RefNo, cmd, con);
            }
            con.Close();
        }

    //    internal static void Update_GSTR6_RetStatus(string gSTINNo, string period, string refid_response, string v1, string v2, string v3, string v4, string sessionCustId, string sessionUserId)
    //    {
    //        throw new NotImplementedException();
    //    }
    }
}