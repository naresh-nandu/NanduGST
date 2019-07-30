using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;
using Newtonsoft.Json;
using SmartAdmin.Models.CustomerAPIFunctions;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Models.CustomerAPIFunction
{
    public class CustomerRegistration
    {
        protected CustomerRegistration()
        {
            //
        }

        public static void SendRequest(Attributes objAttr, string StrPartnerID, out int RetValue, out string RetMessage)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand dCmd = new SqlCommand("USP_Insert_CustomerRegistration", conn))
                    {
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Name", objAttr.Name));
                        dCmd.Parameters.Add(new SqlParameter("@Designation", ""));
                        dCmd.Parameters.Add(new SqlParameter("@Company", objAttr.Company));
                        dCmd.Parameters.Add(new SqlParameter("@GSTINNo", objAttr.GSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@Email", objAttr.Email));
                        dCmd.Parameters.Add(new SqlParameter("@MobileNo", objAttr.MobileNo));
                        dCmd.Parameters.Add(new SqlParameter("@PANNo", objAttr.PANNo));
                        dCmd.Parameters.Add(new SqlParameter("@Statecode", objAttr.Statecode));
                        dCmd.Parameters.Add(new SqlParameter("@ValidFrom", ""));
                        dCmd.Parameters.Add(new SqlParameter("@ValidTo", ""));
                        dCmd.Parameters.Add(new SqlParameter("@AadharNo", ""));
                        dCmd.Parameters.Add(new SqlParameter("@Referenceno", ""));
                        dCmd.Parameters.Add(new SqlParameter("@Address", objAttr.Address));
                        dCmd.Parameters.Add(new SqlParameter("@GSTINUserName", objAttr.GSTINUserName));
                        dCmd.Parameters.Add(new SqlParameter("@PartnerId", StrPartnerID));
                        dCmd.Parameters.Add(new SqlParameter("@RetValue", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        dCmd.Parameters.Add(new SqlParameter("@RetMessage", SqlDbType.VarChar, 250)).Direction = ParameterDirection.Output;
                        dCmd.ExecuteNonQuery();

                        RetValue = Convert.ToInt32(dCmd.Parameters["@RetValue"].Value);
                        RetMessage = Convert.ToString(dCmd.Parameters["@RetMessage"].Value);
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void GetRequest(Attributes objAttr, out int RetValue, out string RetMessage)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented                    
                    conn.Open();
                    using (SqlCommand dCmd = new SqlCommand("Retrieve_UserAccess", conn))
                    {
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Email", objAttr.Email));
                        dCmd.Parameters.Add(new SqlParameter("@Password", objAttr.Password));
                        dCmd.Parameters.Add(new SqlParameter("@RetValue", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        dCmd.Parameters.Add(new SqlParameter("@RetMessage", SqlDbType.VarChar, 255)).Direction = ParameterDirection.Output;
                        dCmd.ExecuteNonQuery();

                        RetValue = Convert.ToInt32(dCmd.Parameters["@RetValue"].Value);
                        if(RetValue == -1)
                        {
                            RetValue = 1;
                        }
                        else
                        {
                            RetValue = 0;
                        }
                        RetMessage = Convert.ToString(dCmd.Parameters["@RetMessage"].Value);
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void GetGSTR1DataRequest(string strPartnerId, string strDate, out object objData)
        {
            string returnJson = "";
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    StringBuilder MyStringBuilder = new StringBuilder();
                    #region commented                    
                    conn.Open();
                    using (SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTIN_Wise_Invoice_Count", conn))
                    {
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@PartnerId", strPartnerId));
                        dCmd.Parameters.Add(new SqlParameter("@GSTRType", "GSTR1"));
                        dCmd.Parameters.Add(new SqlParameter("@Date", strDate));

                        var reader = dCmd.ExecuteReader();
                        if (reader.HasRows)
                        {
                            while (reader.Read())
                            {
                                MyStringBuilder.Append(reader.GetValue(0));
                            }
                        }
                        reader.Close();                        
                        returnJson = MyStringBuilder.ToString();
                        returnJson = returnJson.TrimStart('[').TrimEnd(']');
                        objData = JsonConvert.DeserializeObject<MasterParams>(returnJson);
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void Portal_SendRequest(Attributes objAttr, out int RetValue, out string RetMessage)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand dCmd = new SqlCommand("USP_Insert_CustomerRegistration", conn))
                    {
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@Name", objAttr.Name));
                        dCmd.Parameters.Add(new SqlParameter("@Designation", ""));
                        dCmd.Parameters.Add(new SqlParameter("@Company", objAttr.Company));
                        dCmd.Parameters.Add(new SqlParameter("@GSTINNo", objAttr.GSTINNo));
                        dCmd.Parameters.Add(new SqlParameter("@Email", objAttr.Email));
                        dCmd.Parameters.Add(new SqlParameter("@MobileNo", objAttr.MobileNo));
                        dCmd.Parameters.Add(new SqlParameter("@PANNo", objAttr.PANNo));
                        dCmd.Parameters.Add(new SqlParameter("@Statecode", objAttr.Statecode));
                        dCmd.Parameters.Add(new SqlParameter("@ValidFrom", ""));
                        dCmd.Parameters.Add(new SqlParameter("@ValidTo", ""));
                        dCmd.Parameters.Add(new SqlParameter("@AadharNo", ""));
                        dCmd.Parameters.Add(new SqlParameter("@Referenceno", objAttr.ReferenceNo));
                        dCmd.Parameters.Add(new SqlParameter("@Address", objAttr.Address));
                        dCmd.Parameters.Add(new SqlParameter("@GSTINUserName", objAttr.GSTINUserName));
                        dCmd.Parameters.Add(new SqlParameter("@RetValue", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        dCmd.Parameters.Add(new SqlParameter("@RetMessage", SqlDbType.VarChar, 250)).Direction = ParameterDirection.Output;
                        dCmd.ExecuteNonQuery();

                        RetValue = Convert.ToInt32(dCmd.Parameters["@RetValue"].Value);
                        RetMessage = Convert.ToString(dCmd.Parameters["@RetMessage"].Value);
                        string strCustRefNo = objAttr.ReferenceNo;

                        if (RetValue == 1)
                        {
                            Notification.SendSMS(objAttr.MobileNo.ToString(), "Greetings from WeP Digital!! Your Registration at WeP GST Panel is subject to verification of KYC documents. Your Login e - mail Id is " + objAttr.Email + " and Password is " + objAttr.MobileNo.ToString() + ". Customer Reference No " + strCustRefNo + ".");
                            Notification.SendEmail(objAttr.Email, "Your Registration is Approved By Wep Administrator.", "Greetings from WeP Digital!! Your Registration at WeP GST Panel is subject to verification of KYC documents. Your Login e - mail Id is " + objAttr.Email + " and Password is " + objAttr.MobileNo.ToString() + ". Customer Reference No " + strCustRefNo + ".");
                        }
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }


        public static void Wallet_SendRequest(Attributes objAttr, out int RetValue, out string RetMessage, out decimal RetBalance)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand dCmd = new SqlCommand("usp_Insert_Wallet_Balance", conn))
                    {
                        dCmd.CommandType = CommandType.StoredProcedure;
                        dCmd.CommandTimeout = 0;
                        dCmd.Parameters.Add(new SqlParameter("@CustEmail", objAttr.CustEmail));
                        dCmd.Parameters.Add(new SqlParameter("@MobileNo", objAttr.CustMobile));
                        dCmd.Parameters.Add(new SqlParameter("@TotalValue", objAttr.TotalValue));
                        dCmd.Parameters.Add(new SqlParameter("@CustomerType", objAttr.CustomerType));
                        dCmd.Parameters.Add(new SqlParameter("@RetValue", SqlDbType.Int)).Direction = ParameterDirection.Output;
                        dCmd.Parameters.Add(new SqlParameter("@RetMessage", SqlDbType.VarChar, 250)).Direction = ParameterDirection.Output;
                        dCmd.Parameters.Add(new SqlParameter("@RetBalance", SqlDbType.Decimal)).Direction = ParameterDirection.Output;
                        dCmd.ExecuteNonQuery();

                        RetValue = Convert.ToInt32(dCmd.Parameters["@RetValue"].Value);
                        RetMessage = Convert.ToString(dCmd.Parameters["@RetMessage"].Value);
                        RetBalance = Convert.ToDecimal(dCmd.Parameters["@RetBalance"].Value);
                    }
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void WalletPack_SendRequest(Attributes objAttr, out int RetValue, out string RetMessage)
        {
            string strCustId = "";
            try
            {
                #region commented
                if(objAttr.CustomerType == "WEPGST")
                {
                    Functions.UpdateOrInsertTable("UPDATE TBL_Customer SET WalletPack = " + objAttr.WalletPack + " Where Email = '" + objAttr.CustEmail + "' and MobileNo = '" + objAttr.CustMobile + "' and RowStatus = 1");
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = con;
                            sqlcmd.CommandText = "Select * from TBL_Customer Where Email = @Email and MobileNo = @Mobile and RowStatus = 1";
                            sqlcmd.Parameters.AddWithValue("@Email", objAttr.CustEmail);
                            sqlcmd.Parameters.AddWithValue("@Mobile", objAttr.CustMobile);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    strCustId = dt.Rows[0]["CustId"].ToString();
                                }
                            }
                        }
                    }
                }
                if (objAttr.CustomerType == "TRP" || objAttr.CustomerType == "GSK")
                {
                    Functions.UpdateOrInsertTable("UPDATE TBL_TRP_Customer SET WalletPack = " + objAttr.WalletPack + " Where Email = '" + objAttr.CustEmail + "' and MobileNo = '" + objAttr.CustMobile + "' and RowStatus = 1");
                }

                if (!string.IsNullOrEmpty(strCustId))
                {
                    using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        try
                        {
                            #region commented                        
                            conn.Open();
                            using (SqlCommand dCmd = new SqlCommand("usp_Update_Customer_Wise_EwayBill_Access", conn))
                            {
                                dCmd.CommandType = CommandType.StoredProcedure;
                                dCmd.CommandTimeout = 0;
                                dCmd.Parameters.Add(new SqlParameter("@CustId", strCustId));
                                dCmd.ExecuteNonQuery();
                            }
                            conn.Close();
                            #endregion
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine(ex);
                            throw;
                        }
                    }
                }
                #endregion
                RetValue = 1;
                RetMessage = "SUCCESS";
            }
            catch (Exception ex)
            {
                RetValue = 0;
                RetMessage = "FAILURE - " + ex.Message;
            }
        }
    }

    public class MasterParams
    {
        public string Date { get; set; }

        public List<Gstr1DataRequestParams> data { get; set; }
    }

    public class Gstr1DataRequestParams
    {
        public string GSTIN { get; set; }

        public string FP { get; set; }

        public string INVOICECOUNT { get; set; }

        public string APICOUNT { get; set; }
    }
}