using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http;
using System.Security.Cryptography;
using System.IO;

namespace SmartAdminMvc.Models.ESign
{
    public class OldTCUtilsBuffer
    {
        private readonly string UPLOAD_BUFFER_URL = "https://wepindia.truecopy.in/services/corpservice/v2/uploadBuffer";
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
}
