using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace SmartAdminMvc.Models.GSTR3
{
    public class SendGSTR3BSaveDataModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        DataSet result = new DataSet();
        private static string DecryptedJsonData = "", DecryptedJsonData1 = "";
        private static string Username = "";
        private static string AppKey = "";
        private static string EncryptedSEK = "";
        private static string AuthToken = "";
        private static string GSTR3BResponse = "";
        //public string SendRequest(string JsonData, string GSTINNo, string Period, string SessionUserId, string SessionCustId, string SessionUsername)
        //{
        //    try
        //    {
        //        SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_AUTH_KEYS where AuthorizationToken = '" + GSTINNo + "'", con);
        //        DataTable dt = new DataTable();
        //        adt.Fill(dt);
        //        if (dt.Rows.Count > 0)
        //        {
        //            Username = dt.Rows[0]["Username"].ToString();
        //            AppKey = dt.Rows[0]["AppKey"].ToString();
        //            EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
        //            AuthToken = dt.Rows[0]["AuthToken"].ToString();
        //        }
        //    }
        //    }
    }
}