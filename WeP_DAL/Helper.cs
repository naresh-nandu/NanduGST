using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;

namespace WeP_DAL
{
    public static class Helper
    {
        public static string HMAC_SHA256(string data, string EK)
        {
            UTF8Encoding enc = new UTF8Encoding();
            byte[] secretKey = enc.GetBytes(EK);
            HMACSHA256 hmac = new HMACSHA256(secretKey);
            hmac.Initialize();
            byte[] bytes = enc.GetBytes(data);
            byte[] rawHmac = hmac.ComputeHash(bytes);
            string result = Convert.ToBase64String(rawHmac);
            return result;
        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string Base64Decode(string base64EncodedData)
        {
            var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
            return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
        }

        public static string openFileToString(byte[] _bytes)
        {
            string file_string = "";
            file_string = Encoding.UTF8.GetString(_bytes);
            return file_string;
        }

        public static string getHashSha256(string text)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(text);
            SHA256Managed hashstring = new SHA256Managed();
            byte[] hash = hashstring.ComputeHash(bytes);
            StringBuilder hashString = new StringBuilder();
            foreach (byte x in hash)
            {
                hashString.Append(String.Format("{0:x2}", x));
            }
            return hashString.ToString();
        }

        public static string Encrypt(string plainText, byte[] keyBytes)
        {
            byte[] dataToEncrypt = UTF8Encoding.UTF8.GetBytes(plainText);
            AesManaged tdes = new AesManaged();
            tdes.KeySize = 256;
            tdes.BlockSize = 128;
            tdes.Key = keyBytes;
            tdes.Mode = CipherMode.ECB;
            tdes.Padding = PaddingMode.PKCS7;

            ICryptoTransform crypt = tdes.CreateEncryptor();
            byte[] cipher = crypt.TransformFinalBlock(dataToEncrypt, 0, dataToEncrypt.Length);
            tdes.Clear();
            return Convert.ToBase64String(cipher, 0, cipher.Length);
        }

        public static string hmac(byte[] deCipher, string message)
        {
            string EK_result = Convert.ToBase64String(deCipher);
            Console.WriteLine(EK_result);
            var EK = Convert.FromBase64String(EK_result);
            string gen_hmac = "";
            using (var HMACSHA256 = new HMACSHA256(EK))
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                byte[] hashmessage = HMACSHA256.ComputeHash(data);
                gen_hmac = Convert.ToBase64String(hashmessage);
            }
            return gen_hmac;
        }

        public static string GetCustomerDetails(string Column, string TableName, string WhereClause)
        {
            DataTable dt = new DataTable();
            string returnColumn = "";
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                if (sqlcon.State == ConnectionState.Closed)
                {
                    sqlcon.Open();
                }
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "usp_Retrieve_Generic_Details";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.Parameters.AddWithValue("@TableName", TableName);
                    sqlcmd.Parameters.AddWithValue("@WhereClause", WhereClause);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            returnColumn = dt.Rows[0][Column].ToString();
                        }
                    }
                }
                sqlcon.Close();
            }
            return returnColumn;
        }

        public static void InsertGSTRSaveResponse(string strGSTINNo, string strGSTRType, string strTransNo, string strRefNo, string strPeriod, string strCustId, string strUserId)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@GSTINNo", SqlDbType.VarChar).Value = strGSTINNo;
                cmd.Parameters.Add("@GSTRName", SqlDbType.VarChar).Value = strGSTRType;
                cmd.Parameters.Add("@TransId", SqlDbType.VarChar).Value = strTransNo;
                cmd.Parameters.Add("@RefId", SqlDbType.VarChar).Value = strRefNo;
                cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = strPeriod;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = strCustId;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = strUserId;
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                SQLHelper.InsertIntoTable("TBL_GSTR_SAVE_Response", cmd, con);
                con.Close();
            }
        }

        public static void InsertGSTRFileResponse(string strGSTINNo, string strGSTRType, string strStatus, string strAckNo, string strPeriod, string strCustId, string strUserId)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@GSTINNo", SqlDbType.VarChar).Value = strGSTINNo;
                cmd.Parameters.Add("@GSTRName", SqlDbType.VarChar).Value = strGSTRType;
                cmd.Parameters.Add("@Status", SqlDbType.VarChar).Value = strStatus;
                cmd.Parameters.Add("@AckNo", SqlDbType.VarChar).Value = strAckNo;
                cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = strPeriod;
                cmd.Parameters.Add("@CustomerId", SqlDbType.Int).Value = strCustId;
                cmd.Parameters.Add("@UserId", SqlDbType.Int).Value = strUserId;
                cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                SQLHelper.InsertIntoTable("TBL_GSTR_FILE_Response", cmd, con);
                con.Close();
            }
        }

        public static void InsertAuditLog(string strUserId, string strUsername, string strMessage, string strException)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@FK_Audit_User_ID", SqlDbType.Int).Value = strUserId;
                cmd.Parameters.Add("@FK_Audit_Username", SqlDbType.VarChar).Value = strUsername;
                cmd.Parameters.Add("@Audit_DateTime", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@Audit_Message", SqlDbType.VarChar).Value = strMessage;
                cmd.Parameters.Add("@Audit_Exception", SqlDbType.VarChar).Value = strException;
                SQLHelper.InsertIntoTable("TBL_Audit_Log", cmd, con);
                con.Close();
            }
        }

        public static void InsertAPICountTransactions(string strGSTINNo, string strPERIOD, string strAPIName, string strUserId, string strCustId)
        {
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@Custid", SqlDbType.Int).Value = strCustId;
                cmd.Parameters.Add("@Gstinno", SqlDbType.VarChar).Value = strGSTINNo;
                cmd.Parameters.Add("@Period", SqlDbType.VarChar).Value = strPERIOD;
                cmd.Parameters.Add("@ApiName", SqlDbType.NVarChar).Value = strAPIName;
                cmd.Parameters.Add("@Createdby", SqlDbType.Int).Value = strUserId;
                cmd.Parameters.Add("@Createddate", SqlDbType.DateTime).Value = DateTime.Now;
                cmd.Parameters.Add("@rowstatus", SqlDbType.Bit).Value = 1;
                SQLHelper.InsertIntoTable("TBL_ASP_API_Transactions", cmd, con);
                con.Close();
            }
        }

        #region "WALLET PURPOSE METHODS"

        public static void UpdateWalletBalance(string strCustId, string strProductType, decimal dValue, string strGSTRType, string strGSTINNo, string strPeriod)
        {
            string strTRPId = "", strCustEmail = "", strMobileNo = "";
            decimal dTotalValue = 0;//dValue = 0, 
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select TOP 1 * from TBL_WEP_WALLET_TRANSACTIONS Where GSTRType = @GSTRType and GSTIN = @GSTINNo and FP = @Period";
                    sqlcmd.Parameters.AddWithValue("@GSTRType", strGSTRType);
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    sqlcmd.Parameters.AddWithValue("@Period", strPeriod);
                    using (SqlDataAdapter wt_adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable wt_dt = new DataTable();
                        wt_adt.Fill(wt_dt);
                        if (wt_dt.Rows.Count > 0)
                        {
                            // For that GSTIN and PERIOD Wallet Deduction already done....
                        }
                        else
                        {
                            #region "UPDATE WALLET BALANCE FOR WEPGST CUSTOERS"
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NULL";
                                sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                using (SqlDataAdapter w_adt = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable w_dt = new DataTable();
                                    w_adt.Fill(w_dt);
                                    if (w_dt.Rows.Count > 0)
                                    {
                                        using (SqlCommand sqlcmd2 = new SqlCommand())
                                        {
                                            sqlcmd2.Connection = con;
                                            sqlcmd2.CommandText = "Select * from TBL_WEP_WALLET where CustId = @CustId and TRPId IS NULL";
                                            sqlcmd2.Parameters.AddWithValue("@CustId", strCustId);
                                            using (SqlDataAdapter w_adt1 = new SqlDataAdapter(sqlcmd2))
                                            {
                                                DataTable w_dt1 = new DataTable();
                                                w_adt1.Fill(w_dt1);
                                                if (w_dt1.Rows.Count > 0)
                                                {
                                                    dTotalValue = Convert.ToDecimal(w_dt1.Rows[0]["TotalValue"].ToString());
                                                    strCustEmail = w_dt1.Rows[0]["CustEmail"].ToString();
                                                    strMobileNo = w_dt1.Rows[0]["MobileNo"].ToString();
                                                }
                                            }
                                        }

                                        dTotalValue = dTotalValue - dValue;

                                        // Deducting Wallet Balance
                                        SQLHelper.UpdateOrInsertTable("UPDATE TBL_WEP_WALLET SET TotalValue = '" + dTotalValue + "', ModifiedDate = '" + DateTime.Now + "' WHERE CustId = '" + strCustId + "' and TRPId IS NULL");

                                        // Wallet Transaction History
                                        SqlCommand w_cmd = new SqlCommand();
                                        w_cmd.Parameters.Clear();
                                        w_cmd.Parameters.Add("@CustEmail", SqlDbType.VarChar).Value = strCustEmail;
                                        w_cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = strMobileNo;
                                        w_cmd.Parameters.Add("@ProductType", SqlDbType.VarChar).Value = strProductType;
                                        w_cmd.Parameters.Add("@TransactionType", SqlDbType.VarChar).Value = "DEBIT";
                                        w_cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = dValue;
                                        w_cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                        w_cmd.Parameters.Add("@GSTRType", SqlDbType.NVarChar).Value = strGSTRType;
                                        w_cmd.Parameters.Add("@GSTIN", SqlDbType.NVarChar).Value = strGSTINNo;
                                        w_cmd.Parameters.Add("@FP", SqlDbType.NVarChar).Value = strPeriod;
                                        w_cmd.Parameters.Add("@CustId", SqlDbType.Int).Value = strCustId;
                                        SQLHelper.InsertIntoTable("TBL_WEP_WALLET_TRANSACTIONS", w_cmd, con);
                                    }
                                }
                            }
                            #endregion

                            #region "UPDATE WALLET BALANCE FOR TRP CUSTOERS"
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NOT NULL";
                                sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable dt = new DataTable();
                                    adt.Fill(dt);
                                    if (dt.Rows.Count > 0)
                                    {
                                        strTRPId = dt.Rows[0]["TRPId"].ToString();
                                        using (SqlCommand sqlcmd2 = new SqlCommand())
                                        {
                                            sqlcmd2.Connection = con;
                                            sqlcmd2.CommandText = "Select * from TBL_WEP_WALLET where TRPId = @TRPId and CustId IS NULL";
                                            sqlcmd2.Parameters.AddWithValue("@TRPId", strTRPId);
                                            using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd2))
                                            {
                                                DataTable dt1 = new DataTable();
                                                adt1.Fill(dt1);
                                                if (dt1.Rows.Count > 0)
                                                {
                                                    dTotalValue = Convert.ToDecimal(dt1.Rows[0]["TotalValue"].ToString());
                                                    strCustEmail = dt1.Rows[0]["CustEmail"].ToString();
                                                    strMobileNo = dt1.Rows[0]["MobileNo"].ToString();
                                                }
                                            }
                                        }

                                        dTotalValue = dTotalValue - dValue;

                                        // Deducting Wallet Balance
                                        SQLHelper.UpdateOrInsertTable("UPDATE TBL_WEP_WALLET SET TotalValue = '" + dTotalValue + "', ModifiedDate = '" + DateTime.Now + "' WHERE TRPId = '" + strTRPId + "' and CustId IS NULL");

                                        // Wallet Transaction History
                                        SqlCommand cmd = new SqlCommand();
                                        cmd.Parameters.Clear();
                                        cmd.Parameters.Add("@TRPId", SqlDbType.Int).Value = strTRPId;
                                        cmd.Parameters.Add("@CustEmail", SqlDbType.VarChar).Value = strCustEmail;
                                        cmd.Parameters.Add("@MobileNo", SqlDbType.VarChar).Value = strMobileNo;
                                        cmd.Parameters.Add("@ProductType", SqlDbType.VarChar).Value = strProductType;
                                        cmd.Parameters.Add("@TransactionType", SqlDbType.VarChar).Value = "DEBIT";
                                        cmd.Parameters.Add("@Amount", SqlDbType.Decimal).Value = dValue;
                                        cmd.Parameters.Add("@CreatedDate", SqlDbType.DateTime).Value = DateTime.Now;
                                        cmd.Parameters.Add("@GSTRType", SqlDbType.NVarChar).Value = strGSTRType;
                                        cmd.Parameters.Add("@GSTIN", SqlDbType.NVarChar).Value = strGSTINNo;
                                        cmd.Parameters.Add("@FP", SqlDbType.NVarChar).Value = strPeriod;
                                        SQLHelper.InsertIntoTable("TBL_WEP_WALLET_TRANSACTIONS", cmd, con);
                                    }
                                }
                            }
                            #endregion
                        }
                    }
                }
                con.Close();
            }
        }

        public static void GetWalletBalance(string strCustId, string strProductType, string strGSTRType, string strGSTINNo, string strPeriod, out decimal TotalValue,
            out string GSK, out bool isChecked)
        {
            string strTRPId = "", strGSK = "";
            bool check = false;
            decimal dTotalValue = 0;
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                #region "GET WALLET BALANCE FOR WePGST CUSTOMERS"
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NULL";
                    sqlcmd.Parameters.AddWithValue("@CustId", strCustId);
                    using (SqlDataAdapter w_adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable w_dt = new DataTable();
                        w_adt.Fill(w_dt);
                        if (w_dt.Rows.Count > 0)
                        {
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET where CustId = @CustId and CustId IS NOT NULL and TRPId IS NULL";
                                sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                using (SqlDataAdapter w_adt1 = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable w_dt1 = new DataTable();
                                    w_adt1.Fill(w_dt1);
                                    if (w_dt1.Rows.Count > 0)
                                    {
                                        dTotalValue = Convert.ToDecimal(w_dt1.Rows[0]["TotalValue"].ToString());
                                    }
                                    else
                                    {
                                        dTotalValue = 0;
                                    }
                                }
                            }
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_Customer where CustId = @CustId and RowStatus = 1 and WalletPack = 1";
                                sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                using (SqlDataAdapter w_adt2 = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable w_dt2 = new DataTable();
                                    w_adt2.Fill(w_dt2);
                                    if (w_dt2.Rows.Count > 0)
                                    {
                                        strGSK = w_dt2.Rows[0]["WalletPack"].ToString();
                                    }
                                    else
                                    {
                                        dTotalValue = 0;
                                    }
                                }
                            }


                            if (strProductType == "RETURN FILING")
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.Connection = con;
                                    sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET_TRANSACTIONS where CustId = @CustId and ProductType = @ProductType and GSTRType = @GSTRType and GSTIN = @GSTINNo and FP = @Period and TRPId IS NULL and CustId IS NOT NULL";
                                    sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                    sqlcmd1.Parameters.AddWithValue("@ProductType", strProductType);
                                    sqlcmd1.Parameters.AddWithValue("@GSTRType", strGSTRType);
                                    sqlcmd1.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                                    sqlcmd1.Parameters.AddWithValue("@Period", strPeriod);
                                    using (SqlDataAdapter w_adt3 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        DataTable w_dt3 = new DataTable();
                                        w_adt3.Fill(w_dt3);
                                        if (w_dt3.Rows.Count > 0)
                                        {
                                            check = true;
                                        }
                                        else
                                        {
                                            check = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.Connection = con;
                                    sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET_TRANSACTIONS where CustId = @CustId and ProductType = @ProductType and TRPId IS NULL and CustId IS NOT NULL";
                                    sqlcmd1.Parameters.AddWithValue("@CustId", strCustId);
                                    sqlcmd1.Parameters.AddWithValue("@ProductType", strProductType);
                                    using (SqlDataAdapter w_adt3 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        DataTable w_dt3 = new DataTable();
                                        w_adt3.Fill(w_dt3);
                                        check = false;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                #region "GET WALLET BALANCE FOR TRP CUSTOMERS"
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NOT NULL";
                    sqlcmd.Parameters.AddWithValue("@CustId", strCustId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            strTRPId = dt.Rows[0]["TRPId"].ToString();
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET where TRPId = @TRPId and CustId IS NULL";
                                sqlcmd1.Parameters.AddWithValue("@TRPId", strTRPId);
                                using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable dt1 = new DataTable();
                                    adt1.Fill(dt1);
                                    if (dt1.Rows.Count > 0)
                                    {
                                        dTotalValue = Convert.ToDecimal(dt1.Rows[0]["TotalValue"].ToString());
                                    }
                                    else
                                    {
                                        dTotalValue = 0;
                                    }
                                }
                            }

                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_TRP_Customer where TRPCustId = @TRPId and RowStatus = 1 and WalletPack = 1";
                                sqlcmd1.Parameters.AddWithValue("@TRPId", strTRPId);
                                using (SqlDataAdapter adt2 = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable dt2 = new DataTable();
                                    adt2.Fill(dt2);
                                    if (dt2.Rows.Count > 0)
                                    {
                                        strGSK = dt2.Rows[0]["WalletPack"].ToString();
                                    }
                                    else
                                    {
                                        dTotalValue = 0;
                                    }
                                }
                            }

                            if (strProductType == "RETURN FILING")
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.Connection = con;
                                    sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET_TRANSACTIONS where TRPId = @TRPId and ProductType = @ProductType and GSTRType = @GSTRType and GSTIN = @GSTINNo and FP = @Period and CustId IS NULL";
                                    sqlcmd1.Parameters.AddWithValue("@TRPId", strTRPId);
                                    sqlcmd1.Parameters.AddWithValue("@ProductType", strProductType);
                                    sqlcmd1.Parameters.AddWithValue("@GSTRType", strGSTRType);
                                    sqlcmd1.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                                    sqlcmd1.Parameters.AddWithValue("@Period", strPeriod);
                                    using (SqlDataAdapter adt3 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        DataTable dt3 = new DataTable();
                                        adt3.Fill(dt3);
                                        if (dt3.Rows.Count > 0)
                                        {
                                            check = true;
                                        }
                                        else
                                        {
                                            check = false;
                                        }
                                    }
                                }
                            }
                            else
                            {
                                using (SqlCommand sqlcmd1 = new SqlCommand())
                                {
                                    sqlcmd1.Connection = con;
                                    sqlcmd1.CommandText = "Select * from TBL_WEP_WALLET_TRANSACTIONS where TRPId = @TRPId and ProductType = @ProductType and TRPId IS NOT NULL and CustId IS NULL";
                                    sqlcmd1.Parameters.AddWithValue("@TRPId", strTRPId);
                                    sqlcmd1.Parameters.AddWithValue("@ProductType", strProductType);
                                    using (SqlDataAdapter adt3 = new SqlDataAdapter(sqlcmd1))
                                    {
                                        DataTable dt3 = new DataTable();
                                        adt3.Fill(dt3);
                                        check = false;
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                con.Close();
            }
            TotalValue = dTotalValue;
            isChecked = check;
            GSK = strGSK;
        }

        public static string UpdateWalletBalanceRequest(string CustId, decimal dValue, string strProductType)
        {
            string strCustEmail = "", strTRPId = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                #region "UPDATE WALLET BALANCE REQUEST"
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NULL";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter w_adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable w_dt = new DataTable();
                        w_adt.Fill(w_dt);
                        if (w_dt.Rows.Count > 0)
                        {
                            strCustEmail = w_dt.Rows[0]["Email"].ToString();
                        }
                    }
                }
                #endregion

                #region "UPDATE WALLET BALANCE REQUEST"
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId and TRPId IS NOT NULL";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            strTRPId = dt.Rows[0]["TRPId"].ToString();
                            using (SqlCommand sqlcmd1 = new SqlCommand())
                            {
                                sqlcmd1.Connection = con;
                                sqlcmd1.CommandText = "Select * from TBL_TRP_Customer where trpCustId = @TRPId";
                                sqlcmd1.Parameters.AddWithValue("@TRPId", strTRPId);
                                using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd1))
                                {
                                    DataTable dt1 = new DataTable();
                                    adt1.Fill(dt1);
                                    if (dt1.Rows.Count > 0)
                                    {
                                        strCustEmail = dt1.Rows[0]["Email"].ToString();
                                    }
                                }
                            }
                        }
                    }
                }
                #endregion

                con.Close();
            }
            WalletParmeters objWalletParams = new WalletParmeters();
            objWalletParams.cust = strCustEmail;
            objWalletParams.amt = dValue;
            objWalletParams.type = strProductType;
            string tempjsondata = new JavaScriptSerializer().Serialize(objWalletParams);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["WALLET_DEDUCT_API"]);
            httpWebRequest.ContentType = "application/json; charset=utf-8";
            httpWebRequest.Method = "POST";
            httpWebRequest.Accept = "application/json; charset=utf-8";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(tempjsondata);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    streamReader.ReadToEnd();
                }
            }
            return "";
        }

        #endregion

        public static void GSTR_Auth_DataAdapter(string strGSTINNo, out string strUsername, out string strEncryptedAppKey)
        {
            string Username = "", EncryptedAppKey = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString();
                            EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
                        }
                    }
                }
                con.Close();
            }
            strUsername = Username;
            strEncryptedAppKey = EncryptedAppKey;
        }

        public static void GSTR_AuthOTP_DataAdapter(string strGSTINNo, out string strUsername, out string strEncryptedAppKey, out string strEncryptedOtp)
        {
            string Username = "", EncryptedAppKey = "", EncryptedOTP = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString();
                            EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
                            EncryptedOTP = dt.Rows[0]["EncryptedOTP"].ToString();
                        }
                    }
                }
                con.Close();
            }
            strUsername = Username;
            strEncryptedAppKey = EncryptedAppKey;
            strEncryptedOtp = EncryptedOTP;
        }

        public static void GSTR_DataAdapter(string strGSTINNo, out string strUsername, out string strAppKey, out string strEncryptedSek, out string strAuthToken)
        {
            string Username = "", AppKey = "", EncryptedSEK = "", AuthToken = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS where AuthorizationToken = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString();
                            AppKey = dt.Rows[0]["AppKey"].ToString();
                            EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                            AuthToken = dt.Rows[0]["AuthToken"].ToString();
                        }
                    }
                }
                con.Close();
            }
            strUsername = Username;
            strAppKey = AppKey;
            strEncryptedSek = EncryptedSEK;
            strAuthToken = AuthToken;
        }

        public static void EWB_AUTH_DataAdapter(string strGSTINNo, out string strUsername, out string strEncAppKey, out string strEncpassword)
        {
            string Username = "", EncryptedAppKey = "", EncryptedPassword = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS_EWB where GSTIN = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString();
                            EncryptedAppKey = dt.Rows[0]["EncryptedAppKey"].ToString();
                            EncryptedPassword = dt.Rows[0]["EncryptedPassword"].ToString();
                        }
                    }
                }
                con.Close();
            }
            strUsername = Username;
            strEncAppKey = EncryptedAppKey;
            strEncpassword = EncryptedPassword;
        }

        public static void EWB_DataAdapter(string strGSTINNo, out string strUsername, out string strAppKey, out string strEncryptedSek, out string strAuthToken)
        {
            string Username = "", AppKey = "", EncryptedSEK = "", AuthToken = "";
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_AUTH_KEYS_EWB where GSTIN = @GSTINNo";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", strGSTINNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            Username = dt.Rows[0]["Username"].ToString();
                            AppKey = dt.Rows[0]["AppKey"].ToString();
                            EncryptedSEK = dt.Rows[0]["EncryptedSEK"].ToString();
                            AuthToken = dt.Rows[0]["AuthToken"].ToString();
                        }
                    }
                }
                con.Close();
            }
            strUsername = Username;
            strAppKey = AppKey;
            strEncryptedSek = EncryptedSEK;
            strAuthToken = AuthToken;
        }

        public static void EWB_Error_DataAdapter(string strErrorCode, out string strErrorDescription)
        {
            string ErrorDescription = "";
            StringBuilder myBuilder = new StringBuilder();
            strErrorCode = strErrorCode.Trim().TrimEnd(',');
            string connectionString = ConfigurationManager.AppSettings["ConnectionString"];
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "usp_Retrieve_EWB_ErrorDescription";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.Parameters.Add("@ErrorCodes", SqlDbType.NVarChar).Value = strErrorCode;
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            for (int i = 0; i < dt.Rows.Count; i++)
                            {
                                myBuilder.Append(dt.Rows[i]["ErrorDescription"].ToString().Trim() + ",");
                            }
                            ErrorDescription = myBuilder.ToString();
                        }
                    }
                }
                con.Close();
            }
            ErrorDescription = ErrorDescription.Trim().TrimEnd(',');
            strErrorDescription = ErrorDescription;
        }
    }

    public class WalletParmeters
    {
        public string cust { get; set; }
        public decimal amt { get; set; }
        public string type { get; set; }
    }
}