using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;
using System.Net;
using RestSharp;
using Newtonsoft.Json;

namespace SmartAdminMvc.Models.ESign
{
    public class TCUtilsBuffer
    {
        private readonly string UPLOAD_BUFFER_URL = "https://wepindia.truecopy.in/services/corpservice/v2/uploadBuffer";

        private readonly string MARK_FOR_DSC_SIGNING_URL = "https://wepindia.truecopy.in/services/corpservice/v2/markforsigncorpfiledsc";

        private readonly string CHECK_SIGNED_STATUS_URL = "https://wepindia.truecopy.in/services/corpservice/v2/getstatus/";

        private readonly string API_KEY = "PEB63H9QPJHBT8B9"; // Add the APIKey shared with you

        /// <summary>
        /// Uploads buffer to be signed identified by the unique uuid
        /// </summary>
        /// <param name="uuid">UniqueID given by the partner</param>
        /// <param name="email">Email address</param>
        /// <param name="filename">filepath which contains the data that will be passed as buffer</param>
        public async Task<string> uploadBuffer(string uuid, string email, string filename)
        {
            // Compute the checksum
            string cs = GetCS(API_KEY, uuid);
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(uuid), "uuid");
                content.Add(new StringContent(cs), "cs");
                content.Add(new StringContent(email), "email");
                content.Add(new ByteArrayContent(File.ReadAllBytes(filename)), "uploadbuffer");
                Task<HttpResponseMessage> result = client.PostAsync(UPLOAD_BUFFER_URL, content);
                string data = await result.Result.Content.ReadAsStringAsync();
                return data.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public FileUploadResponse uploadFile(string uuid, string signpxy, string description, string fileNamePath, string fileMimeType)
        {
            FileUploadResponse fileUploadResponse = null;
            String cs = GetCS(API_KEY, uuid);
            var client = new RestClient(UPLOAD_BUFFER_URL);
            var request = new RestRequest(Method.POST);
            request.AddHeader("cache-control", "no-cache");
            request.AddHeader("accept", "application/json");
            request.AddHeader("content-type", "multipart/form-data; boundary=----WebKitFormBoundary7MA4YWxkTrZu0gW");
            request.AddParameter("uuid", uuid);
            request.AddParameter("cs", cs);
            request.AddParameter("signpxy", signpxy);
            request.AddParameter("description", description);
            request.AddFile("uploadfile", fileNamePath);

            IRestResponse response = client.Execute(request);
            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                fileUploadResponse = JsonConvert.DeserializeObject<FileUploadResponse>(response.Content);
            }
            else
            {
                fileUploadResponse = null;
            }
            return fileUploadResponse;
        }

        public async Task<string> uploadHash(string uuid, string email, string hashdata)
        {
            // Compute the checksum
            string cs = GetCS(API_KEY, uuid);
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(uuid), "uuid");
                content.Add(new StringContent(cs), "cs");
                content.Add(new StringContent(email), "email");
                content.Add(new StringContent(hashdata), "uploadhash");
                Task<HttpResponseMessage> result = client.PostAsync("https://wepindia.truecopy.in/services/corpservice/v2/uploadHash", content);
                string data = await result.Result.Content.ReadAsStringAsync();
                return data.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> getSignedStatus(string uuid)
        {
            // Compute the checksum
            string cs = GetDownloadCS(API_KEY, uuid);
            Console.WriteLine("Checking Status UUID: " + uuid + ", Chksum: " + cs);

            try
            {
                HttpClient client = new HttpClient();
                Task<HttpResponseMessage> result = client.GetAsync(CHECK_SIGNED_STATUS_URL + uuid + "/" + cs + "/OTHER");
                string data = await result.Result.Content.ReadAsStringAsync();
                Console.WriteLine(data);
                return data;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        public string DownloadESignedData(string uuid, string downloadCS)
        {            
            try
            {
                string url = "https://wepindia.truecopy.in/services/corpservice/v2/fetchsignbuffer/" + uuid + "/" + downloadCS;                
                var httpWebRequest = (HttpWebRequest)WebRequest.Create(url);
                httpWebRequest.ContentType = "application/json; charset=utf-8";
                httpWebRequest.Method = "GET";
                httpWebRequest.Accept = "application/json; charset=utf-8";

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    return result.ToString();
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public async Task<string> markForDSCSigning(string uuid, string email)
        {
            // Compute the checksum
            string cs = GetCS(API_KEY, uuid);
            Console.WriteLine("Processing markForDSCSigning UUID: " + uuid + ", Chksum: " + cs);

            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(uuid), "uuid");
                content.Add(new StringContent(cs), "cs");
                content.Add(new StringContent("OTHER"), "doc_category");
                content.Add(new StringContent(email), "emailid");
                content.Add(new StringContent("YES"), "sendemail");
                Task<HttpResponseMessage> result = client.PostAsync(MARK_FOR_DSC_SIGNING_URL, content);
                string data = await result.Result.Content.ReadAsStringAsync();
                Console.WriteLine(data);
                return data.ToString();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return e.Message;
            }
        }

        public async Task<string> markfile(string uuid, string email, string hashdata)
        {
            // Compute the checksum
            string cs = GetCS(API_KEY, uuid);
            try
            {
                HttpClient client = new HttpClient();
                MultipartFormDataContent content = new MultipartFormDataContent();
                content.Add(new StringContent(uuid), "uuid");
                content.Add(new StringContent(cs), "cs");
                content.Add(new StringContent(email), "email");
                content.Add(new StringContent("OTHER"), "doc_category");
                content.Add(new StringContent(hashdata), "uploadhash");
                Task<HttpResponseMessage> result = client.PostAsync("https://wepindia.truecopy.in/services/corpservice/v2/markforsigncorpfileetoken", content);
                string data = await result.Result.Content.ReadAsStringAsync();
                return data.ToString();
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        public string GetCS(string apiKey, string uuid)
        {
            return GetChecksum(apiKey + uuid);
        }

        public string GetCS(string uuid)
        {
            return GetChecksum(API_KEY + uuid);
        }

        public string GetDownloadCS(string apiKey, string uuid)
        {
            return GetChecksum(apiKey + GetChecksum(apiKey + uuid));
        }

        public string GetChecksum(String input)
        {
            MD5 md5 = System.Security.Cryptography.MD5.Create();

            byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
            byte[] hash = md5.ComputeHash(inputBytes);

            StringBuilder sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }
            return sb.ToString().ToUpper().Substring(0, 16);
        }
    }

    public class FileUploadResponse
    {
        public string status { get; set; }
        public string message { get; set; }
        public string did { get; set; }
        public string url { get; set; }
        public string fileid { get; set; }
        public string cs { get; set; }
    }
}
