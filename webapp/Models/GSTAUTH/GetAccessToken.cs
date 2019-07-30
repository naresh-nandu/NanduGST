using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data;
using System.Data.SqlClient;
using System.Web.UI;
using System.IO;
using System.Reflection;
using System.Net.Http;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Threading.Tasks;
using RestSharp;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Models.GSTAUTH
{
    public partial class GetAccessToken
    {
        public static string getAuthToken()
        {
            string token = null;
            // Get OAuth token using client credentials 
            string authString = ConfigurationManager.AppSettings["GSP_OAUTHTOKEN"];

            // Config for OAuth client credentials  
            string subscriptionKey = ConfigurationManager.AppSettings["Azure_SubscriptionKey"];
            string clientId = ConfigurationManager.AppSettings["Azure_ClientId"];
            string clientSecret = ConfigurationManager.AppSettings["Azure_ClientSecret"];
            string resource = ConfigurationManager.AppSettings["Azure_Resource"];

            try
            {
                var client = new RestClient(authString);
                var request = new RestRequest(Method.POST);
                request.AddHeader("cache-control", "no-cache");
                request.AddHeader("client_secret", clientSecret);
                request.AddHeader("client_id", clientId);
                request.AddHeader("ocp-apim-subscription-key", subscriptionKey);
                IRestResponse response = client.Execute(request);
                if(response.StatusCode == System.Net.HttpStatusCode.OK)
                {
                    string resp = response.Content.ToString();
                    var root = JsonConvert.DeserializeObject<TokenModel>(resp);
                    token = "Bearer " + root.access_token;
                }
                
                return token;
            }
            catch (Exception ex)
            {
                return ex.InnerException.Message;
            }

        }
    }
}