using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Security.Cryptography.Pkcs;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;

namespace SmartAdminMvc.Models.ESign
{
    public partial class DscDataModel
    {
        protected DscDataModel()
        {

        }

        public static string SignData(string dataToSign, string strDSCSubjectName)
        {
            byte[] data = Encoding.UTF8.GetBytes(dataToSign);
            //load certificate 
            X509Certificate2 certObject = null;
            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);
            foreach (X509Certificate2 cert in my.Certificates)
            {
                //if (cert.Subject.Contains("CN=SANDEEP GOYAL"))//change the subject name
                if (cert.Subject.Contains(strDSCSubjectName))//change the subject name
                {
                    certObject = cert;
                    break;
                }
            }
            //
            ContentInfo content = new ContentInfo(data);
            SignedCms signedCms = new SignedCms(content);
            CmsSigner signer = new CmsSigner(certObject);
            signer.DigestAlgorithm = new Oid("SHA256");
            signer.IncludeOption = X509IncludeOption.EndCertOnly;
            //compute signature and encode and convert to base64 string 
            signedCms.ComputeSignature(signer, false);
            string sign = Convert.ToBase64String(signedCms.Encode());
            return sign;
        }
    }
}