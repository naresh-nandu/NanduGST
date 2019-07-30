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
    public class EwbGeneratingKeys
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);

        protected EwbGeneratingKeys()
        {
            //
        }

        public static void GeneratingEncryptedKeys(string strGSTINNo)
        {
            string Certificate_Path = "", AppKey = "", public_key = "", strPassword = "";
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
            
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_AUTH_KEYS_EWB where GSTIN = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            strPassword = dt.Rows[0]["Password"].ToString();
                        }
                    }
                }
            }

            //Encrypt Password with EWayBill public key
            string encPassword = EncryptionUtils.Encrypt(strPassword, public_key);
            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();            
            cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = AppKey.Trim();
            cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = encAppKey.Trim();
            cmd.Parameters.Add("@EncryptedPassword", SqlDbType.NVarChar).Value = encPassword.Trim();
            SQLHelper.UpdateTable("TBL_AUTH_KEYS_EWB", "GSTIN", strGSTINNo, cmd, con);
            con.Close();
        }

        #region "EWAYBILL Authentication Checking"

        public static void Autentication(string strGSTINNo)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {                
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_AUTH_KEYS_EWB where GSTIN = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter ewbadt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable ewbdt = new DataTable();
                        ewbadt.Fill(ewbdt);
                        if (ewbdt.Rows.Count > 0)
                        {
                            var varFinish = DateTime.Now;
                            var varValue = ewbdt.Rows[0]["CreatedDate"];
                            TimeSpan varTime = varFinish - (DateTime)varValue;
                            int intMinutes = (int)varTime.TotalMinutes;

                            if (300 >= intMinutes) // 5 Hours
                            {
                                //..
                            }
                            else
                            {
                                EwbGeneratingKeys.GeneratingEncryptedKeys(strGSTINNo);
                                EwbAuth.SendRequest(strGSTINNo);
                            }
                        }
                    }
                }
            }
        }
        #endregion  
    }
}
