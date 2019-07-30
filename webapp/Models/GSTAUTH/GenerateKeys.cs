using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Reflection;
using System.IO;
using WeP_DAL;

namespace SmartAdminMvc.Models.GSTAUTH
{
    public partial class GenerateKeys
    {
        private static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string binPath = Path.GetDirectoryName(new Uri(Assembly.GetExecutingAssembly().CodeBase).LocalPath);
        static string Certificate_Path = "", AppKey = "", EncryptedAppKey = "", Otp = "", EncryptedOtp = "";

        protected GenerateKeys()
        {
            //
        }
        public static void GeneratingEncryptedKeys(string strGSTINNo)
        {
            Certificate_Path = binPath + "\\" + ConfigurationManager.AppSettings["GST_Certificate"].ToString();
            //Generation of app key. this will be in encoded.
            AppKey = AESEncryption.generateSecureKey();

            //Encrypt with GSTN public key
            EncryptedAppKey = EncryptionUtil.generateEncAppkey(AESEncryption.decodeBase64StringTOByte(AppKey), Certificate_Path);

            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@AppKey", SqlDbType.NVarChar).Value = AppKey.Trim();
            cmd.Parameters.Add("@EncryptedAppKey", SqlDbType.NVarChar).Value = EncryptedAppKey.Trim();
            Common.Functions.UpdateTable("TBL_AUTH_KEYS", "AuthorizationToken", strGSTINNo, cmd, con);
            con.Close();
        }

        public static void GeneratingEncryptedOTPKeys(string strOTP, string strGSTINNo)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                sqlcon.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            AppKey = dt.Rows[0]["AppKey"].ToString();
                            EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
                        }
                    }
                }
                sqlcon.Close();
            }

            //Generation of OTP with appkey
            Otp = strOTP;
            EncryptedOtp = AESEncryption.encryptEK(Convert.FromBase64String(Helper.Base64Encode(Otp)), AESEncryption.decodeBase64StringTOByte(AppKey));

            con.Open();
            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@EncryptedOTP", SqlDbType.NVarChar).Value = EncryptedOtp.Trim();
            Common.Functions.UpdateTable("TBL_AUTH_KEYS", "AuthorizationToken", strGSTINNo, cmd, con);
            con.Close();
        }
    }
}