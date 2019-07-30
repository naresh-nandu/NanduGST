using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.IO;
using System.Data.SqlClient;
using System.Data;
using WeP_DAL;

namespace WeP_EWayBill
{
    public class EWB_GeneratingKeys
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        static string Certificate_Path = "", AppKey = "", public_key = "", strPassword = "";

        public static void GeneratingEncryptedKeys(string strGSTINNo)
        {
            Certificate_Path = binPath + "\\" + ConfigurationManager.AppSettings["EWB_Certificate"].ToString();
            // reading public key string from file
            using (var reader = File.OpenText(Certificate_Path))
            {
                public_key = reader.ReadToEnd().Replace("-----BEGIN PUBLIC KEY-----", "").Replace("-----END PUBLIC KEY-----", "").Replace("\n", "");
            }

            //Generation of app key.
            byte[] _aeskey = EncryptionUtils.generateSecureKey();            
            AppKey = Convert.ToBase64String(_aeskey);

            //Encrypt AppKey with EWayBill public key
            string encAppKey = EncryptionUtils.Encrypt(_aeskey, public_key);
            con.Open();
            SqlDataAdapter adt = new SqlDataAdapter("Select TOP 1 * from TBL_AUTH_KEYS_EWB where GSTIN = '" + strGSTINNo + "'", con);
            DataTable dt = new DataTable();
            adt.Fill(dt);
            if(dt.Rows.Count > 0)
            {
                strPassword = dt.Rows[0]["Password"].ToString();
            }

            //Encrypt Password with EWayBill public key
            string encPassword = EncryptionUtils.Encrypt(strPassword, public_key);
                        
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();            
            cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = AppKey.Trim();
            cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = encAppKey.Trim();
            cmd.Parameters.Add("@EncryptedPassword", SqlDbType.NVarChar).Value = encPassword.Trim();
            //cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
            SQLHelper.UpdateTable("TBL_AUTH_KEYS_EWB", "GSTIN", strGSTINNo, cmd, con);
            con.Close();
        }

        #region "eway bill Authentication Checking"

        public static void Autentication(string strGSTINNo)
        {
            SqlDataAdapter ewbadt = new SqlDataAdapter("Select TOP 1 * from TBL_AUTH_KEYS_EWB where GSTIN = '" + strGSTINNo + "'", con);
            DataTable ewbdt = new DataTable();
            ewbadt.Fill(ewbdt);
            if (ewbdt.Rows.Count > 0)
            {
                var varFinish = DateTime.Now;
                var varValue = ewbdt.Rows[0]["CreatedDate"];
                TimeSpan varTime = (DateTime)varFinish - (DateTime)varValue;
                int intMinutes = (int)varTime.TotalMinutes;

                if (300 >= intMinutes) // 5 Hours
                {
                    //ViewBag.OTPSession = "CLOSE_POPUP";
                    //EWB_GeneratingKeys.GeneratingEncryptedKeys(strGSTINNo);
                    //EWB_Auth.SendRequest(strGSTINNo);
                }
                else
                {
                    EWB_GeneratingKeys.GeneratingEncryptedKeys(strGSTINNo);
                    EWB_Auth.SendRequest(strGSTINNo);
                }
            }
            else
            {

            }
        }
        #endregion  
    }
}
