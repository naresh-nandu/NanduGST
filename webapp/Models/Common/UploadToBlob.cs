using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.IO.Compression;
using System.Web;

namespace SmartAdminMvc.Models.Common
{
    public class UploadToBlob
    {
        private string containerName;
        private string containerUrl;                
        private string fileName;
        private readonly string createdBy;//UserId
        private readonly string customerId;
        private readonly string GSTRType;
        private readonly string strGSTIN;
        private readonly string strPeriod;
        private readonly string connectionString;
        private readonly string strToPeriod;
        public string DecryptedJsonData { get; set; }


        public UploadToBlob(string strGSTIN, string strPeriod, string GSTRType, string createdBy, string customerId)
        {
            this.strGSTIN = strGSTIN;
            this.strPeriod = strPeriod;
            this.GSTRType = GSTRType;
            this.createdBy = createdBy;
            this.customerId = customerId;
            string accountName = "wepdigital";
            string accessKey = "2PmhcsQ7++7SpzjnfiYj170zstfJ5Odjkzdnaek9o5WuvhrpTt5PTPhmS2Ux57rt/2hmXOo/IEeAS/yHvLmcXw==";
            connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
        }
        public UploadToBlob(string strGSTIN, string strPeriod,string strToPeriod, string GSTRType, string createdBy, string customerId)
        {
            this.strGSTIN = strGSTIN;
            this.strPeriod = strPeriod;
            this.strToPeriod = strToPeriod;
            this.GSTRType = GSTRType;
            this.createdBy = createdBy;
            this.customerId = customerId;
            string accountName = "wepdigital";
            string accessKey = "2PmhcsQ7++7SpzjnfiYj170zstfJ5Odjkzdnaek9o5WuvhrpTt5PTPhmS2Ux57rt/2hmXOo/IEeAS/yHvLmcXw==";
            connectionString = "DefaultEndpointsProtocol=https;AccountName=" + accountName + ";AccountKey=" + accessKey + ";EndpointSuffix=core.windows.net";
        }

        public void UploadBlob()
        {
            containerName = "wepgst";
            string folderName = "/Download";
            string subFolderName = "";

            if (GSTRType.Equals("GSTR1"))
            {
                subFolderName = "/GSTR1D";
            }

            string url = "";
            fileName = "GSTR1_Download" + this.strGSTIN + this.strPeriod + ".json";
            try
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(containerName + folderName + subFolderName);

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);
                url = cloudBlockBlob.Uri.ToString();

                cloudBlockBlob.UploadText(DecryptedJsonData);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            containerUrl = url.Substring(0, url.IndexOf(containerName) + containerName.Length);
            string filepath = url.Substring(url.IndexOf(containerName) + containerName.Length + 1);

            InsertdataIntoTable(containerName, containerUrl, filepath, fileName, url, customerId, createdBy, DateTime.Today, "Download", "1", GSTRType, "", null);
        }

        public string UploadReconcilationDataToBlob(string FileNameKey)
        {
            string link = null;
            containerName = "wepgst";
            string folderName = "/Reconcilation";
          string subFoldername = "/" + customerId + "_" +DateTime.Now.ToString("mm-dd-yyyy")+"_"+FileNameKey;
            
            string fileNameZip = "results.zip";
          
            string pathZip = Path.Combine(HttpContext.Current.Server.MapPath("~/App_Data/uploads"), fileNameZip);
            string path3 = string.Format("~/App_Data/uploads/Recon{0}", FileNameKey);
            string path = HttpContext.Current.Server.MapPath(path3);
                System.IO.File.Delete(pathZip);
                ZipFile.CreateFromDirectory(path, pathZip);
            
           
            try
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();
                var container = blobClient.GetContainerReference(containerName + folderName+subFoldername);

               
                    CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileNameZip);
                    

                    cloudBlockBlob.UploadFromFile(pathZip);
                link = cloudBlockBlob.Uri.ToString();

                string url = link.ToString();

                containerUrl = url.Substring(0, url.IndexOf(containerName) + containerName.Length);
                string filepath = url.Substring(url.IndexOf(containerName) + containerName.Length + 1);
                string GSTRRecord = "GSTR-2" + " Reconcilation Data";
                InsertdataIntoTable(containerName, containerUrl, filepath, "", link.ToString(), createdBy, customerId, DateTime.Now, "Reconcilation Data", "", GSTRRecord, "", "");

                return link;
            }
            catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                return null;
            }

        }

        public string UploadReportsToBlob(string path, string intials, string token)
        {
            containerName = "wepgst";
            string folderName = "/Reports";

            string url = "";
            fileName = intials + strPeriod +"_"+strToPeriod+"_"+customerId+"_"+createdBy + ".xlsx";
            try
            {
                var storageAccount = CloudStorageAccount.Parse(connectionString);
                var blobClient = storageAccount.CreateCloudBlobClient();

                var container = blobClient.GetContainerReference(containerName + folderName);

                CloudBlockBlob cloudBlockBlob = container.GetBlockBlobReference(fileName);
                url = cloudBlockBlob.Uri.ToString();

                cloudBlockBlob.UploadFromFile(path);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            containerUrl = url.Substring(0, url.IndexOf(containerName) + containerName.Length);
            string filepath = url.Substring(url.IndexOf(containerName) + containerName.Length + 1);

            InsertdataIntoTable(containerName, containerUrl, filepath, fileName, url, customerId, createdBy, DateTime.Now, "Download", "1", GSTRType, "", token);
            return url;
        }

        public void InsertdataIntoTable(string containername, string containerurl, string filePath, string filename, string bloburl, string Createdby, string CustomerId, DateTime Createddate, string TransType, string Rowstatus, string GSTRType, string TemplateType, string Token)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            conn.Open();
            string query = "INSERT INTO TBL_Blob_Transactions (Containername, Containerurl,Filepath,bloburl,FileName,Createdby,CustomerId,Createddate,TransType,RowStatus,GSTRType,TemplateType,Token)";
            query += " VALUES (@Containername, @Containerurl, @Filepath,@bloburl,@FileName, @Createdby, @CustomerId, @Createddate, @TransType, @RowStatus,@GSTRType,@TemplateType,@Token)";

            try
            {
                SqlCommand myCommand = new SqlCommand(query, conn);
                myCommand.Parameters.AddWithValue("@Containername", containername);
                myCommand.Parameters.AddWithValue("@Containerurl", containerurl);
                myCommand.Parameters.AddWithValue("@Filepath", filePath);
                myCommand.Parameters.AddWithValue("@bloburl", bloburl);
                myCommand.Parameters.AddWithValue("@FileName", filename);
                myCommand.Parameters.AddWithValue("@Createdby", Createdby);
                myCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                myCommand.Parameters.AddWithValue("@Createddate", Createddate);
                myCommand.Parameters.AddWithValue("@TransType", TransType);
                myCommand.Parameters.AddWithValue("@RowStatus", Rowstatus);
                myCommand.Parameters.AddWithValue("@GSTRType", GSTRType);
                myCommand.Parameters.AddWithValue("@TemplateType", TemplateType);
                myCommand.Parameters.AddWithValue("@Token", Token);

                myCommand.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

      static  public void InsertExceptiondataIntoTable(string containername, string containerurl, string filePath, string filename, string bloburl, string Createdby, string CustomerId, DateTime Createddate, string TransType, string Rowstatus, string GSTRType, string TemplateType, string Token)
        {
            SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
            conn.Open();
            string query = "INSERT INTO TBL_Blob_Transactions (Containername, Containerurl,Filepath,bloburl,FileName,Createdby,CustomerId,Createddate,TransType,RowStatus,GSTRType,TemplateType,Token,UploadStatus)";
            query += " VALUES (@Containername, @Containerurl, @Filepath,@bloburl,@FileName, @Createdby, @CustomerId, @Createddate, @TransType, @RowStatus,@GSTRType,@TemplateType,@Token,@UploadStatus)";

            try
            {
                SqlCommand myCommand = new SqlCommand(query, conn);
                myCommand.Parameters.AddWithValue("@Containername", containername);
                myCommand.Parameters.AddWithValue("@Containerurl", containerurl);
                myCommand.Parameters.AddWithValue("@Filepath", filePath);
                myCommand.Parameters.AddWithValue("@bloburl", bloburl);
                myCommand.Parameters.AddWithValue("@FileName", filename);
                myCommand.Parameters.AddWithValue("@Createdby", Createdby);
                myCommand.Parameters.AddWithValue("@CustomerId", CustomerId);
                myCommand.Parameters.AddWithValue("@Createddate", Createddate);
                myCommand.Parameters.AddWithValue("@TransType", TransType);
                myCommand.Parameters.AddWithValue("@RowStatus", Rowstatus);
                myCommand.Parameters.AddWithValue("@GSTRType", GSTRType);
                myCommand.Parameters.AddWithValue("@TemplateType", TemplateType);
                myCommand.Parameters.AddWithValue("@Token", Token);
                myCommand.Parameters.AddWithValue("@UploadStatus", "0");

                myCommand.ExecuteNonQuery();

                conn.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


    }
}