using Newtonsoft.Json;
using SmartAdminMvc.Models.GSTAUTH;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Script.Serialization;

namespace SmartAdminMvc.Models.Common
{
    public partial class GspGetTokenResponseModel
    {
        private static string GSP_Token_Response = "";
        private static string GST_Path = "";
        static string URL = "";
        
        protected GspGetTokenResponseModel()
        {
            //
        }

        public static string GetGSPTokenResponse(string strGSPToken, string strGSTINNo)
        {
            GST_Path = ConfigurationManager.AppSettings["GSP_TOKENRESPONSE"];

            try
            {                
                string AuthorizeToken = GetAccessToken.getAuthToken();
                
                URL = GST_Path + "?gsptoken=" + strGSPToken;                

                var httpWebRequest = (HttpWebRequest)WebRequest.Create(URL);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";
                httpWebRequest.Headers.Add("Authorization", AuthorizeToken);
                httpWebRequest.Headers.Add("Ocp-Apim-Subscription-Key", ConfigurationManager.AppSettings["Azure_SubscriptionKey"]);
                httpWebRequest.Headers.Add("GSTINNO", strGSTINNo);

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    GSP_Token_Response = result.ToString();
                }
                                
                return GSP_Token_Response;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }
        
    }
}