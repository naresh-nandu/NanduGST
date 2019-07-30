using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Controllers
{
    public class DiaController : Controller
    {
        // GET: Dia
        public ActionResult Index()
        {
            return View();
        }


        [HttpPost]
        public string GetDashboardDetails ()
        {
            string Url = ConfigurationManager.AppSettings["DIA_DASHBOARD_URL"];
            return SendRequest(Url);
        }

        [HttpPost]
        public string GetContentDetails (string ContentId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_CONTENT_DETAILS"] + ContentId;
            return SendRequest(Url);
        }

        [HttpPost]
        public string GetContentInfo(string ContentId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_CONTENT_INFO"] + ContentId;
            return SendRequest(Url);
        }

        [HttpPost]
        public string GetRelatedContentDetails(string ContentId, string CategoryId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_RELATED_CONTENT"] + ContentId + "?categoryid=" + CategoryId + "&limit=7&manualOffset=0&sysOffset=0&domainid=32";
            return SendRequest(Url);
        }

        [HttpPost]
        public string GetContentAuthorTeamDetails(string ContentId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_CONTENT_AUTH_TEAM_DETAILS"] + ContentId;
            return SendRequest(Url);
        }

        /// <summary>
        /// Get Author Image Data in base64 encoded string
        /// </summary>
        /// <param name="AuthorId">Author ID</param>
        /// <returns></returns>
        [HttpPost]
        public string GetAuthorImage(string AuthorId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_GET_AUTHOR_IMAGE"] + AuthorId + "/image/";
            return SendRequest(Url);
        }

        /// <summary>
        /// Get PDF Attachment fiile data in bas64 encoded string
        /// </summary>
        /// <param name="ContentId">Content ID</param>
        /// <returns></returns>
        [HttpPost]
        public string GetPdfAttachmentData(string ContentId)
        {
            string Url = ConfigurationManager.AppSettings["DIA_CONTENT_DETAILS"] + ContentId + "/download/";
            return SendRequest(Url);
        }

        [HttpPost]
        public string SearchContent(string SearchString, string RelatedCategoryID)
        {
            string jsonData = "{ \"skip\":0,\"limit\":10,\"categoryId\":" + RelatedCategoryID + ",\"spaceId\":32,\"query\" :\"" + SearchString + "\"}";
            
            string Url = ConfigurationManager.AppSettings["DIA_SEARCH_CONTENT"];
            return SendPostRequest(Url, jsonData);
        }



        private string SendRequest (string url)
        {
            string lvResponseText = String.Empty;
            try
            {
                string Credentials = ConfigurationManager.AppSettings["DIA_USER"] + ":" + ConfigurationManager.AppSettings["DIA_PWD"];
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);
                WebRequest req = WebRequest.Create(url);
                req.Method = "GET";
                req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Credentials));
                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                var encoding = ASCIIEncoding.UTF8;
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
                {
                    lvResponseText = reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                return "";
            }
            return lvResponseText;
        }


        private string SendPostRequest(string url, string data)
        {
            string lvResponseText = String.Empty;
            try
            {
                string Credentials = ConfigurationManager.AppSettings["DIA_USER"] + ":" + ConfigurationManager.AppSettings["DIA_PWD"];
                ServicePointManager.ServerCertificateValidationCallback = ((sender, certificate, chain, sslPolicyErrors) => true);//This line included for https
                WebRequest req = WebRequest.Create(url);
                req.Method = "POST";
                req.Headers["Authorization"] = "Basic " + Convert.ToBase64String(Encoding.Default.GetBytes(Credentials));// This is Username : password

                byte[] byteArray = Encoding.UTF8.GetBytes(data);
                req.ContentType = "application/json";
                req.ContentLength = byteArray.Length;
                Stream dataStream = req.GetRequestStream();
                dataStream.Write(byteArray, 0, byteArray.Length);
                dataStream.Close();

                HttpWebResponse resp = req.GetResponse() as HttpWebResponse;

                var encoding = ASCIIEncoding.UTF8;
                using (var reader = new System.IO.StreamReader(resp.GetResponseStream(), encoding))
                {
                    lvResponseText = reader.ReadToEnd();
                }

            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return lvResponseText;
        }
    }
}