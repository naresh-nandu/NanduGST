using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.InteropServices;

namespace SmartAdminMvc.Models.ESign
{
    public enum CertStoreName
    {
        MY,
        ROOT,
        TRUST,
        CA
    }
    public class CertStoreReader
    {

        #region P/Invoke Interop
        private static int CERT_STORE_PROV_SYSTEM = 10;
        private static int CERT_SYSTEM_STORE_CURRENT_USER = (1 << 16);
        private static int CERT_SYSTEM_STORE_LOCAL_MACHINE = (2 << 16);

        [DllImport("CRYPT32", EntryPoint = "CertOpenStore", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CertOpenStore(int storeProvider, int encodingType, int hcryptProv, int flags, string pvPara);

        [DllImport("CRYPT32", EntryPoint = "CertEnumCertificatesInStore", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern IntPtr CertEnumCertificatesInStore(IntPtr storeProvider, IntPtr prevCertContext);

        [DllImport("CRYPT32", EntryPoint = "CertCloseStore", CharSet = CharSet.Unicode, SetLastError = true)]
        public static extern bool CertCloseStore(IntPtr storeProvider, int flags);

        #endregion


        public string ComputerName { get; set; }

        private readonly bool isLocalMachine;
        public CertStoreReader(string machineName)
        {
            ComputerName = machineName;
            if (machineName == string.Empty)
            {
                isLocalMachine = true;
            }
            else
            {
                isLocalMachine = string.Compare(ComputerName, Environment.MachineName, true) == 0 ? true : false;
            }
        }

        public X509Certificate2Collection GetCertificates(CertStoreName storeName)
        {
            X509Certificate2Collection collectionToReturn = null;
            string givenStoreName = GetStoreName(storeName);

            if (givenStoreName == string.Empty)
            {
                throw new Exception("Invalid Store Name");
            }

            if (isLocalMachine)
            {
                X509Store store = new X509Store(givenStoreName, StoreLocation.LocalMachine);
                try
                {
                    store.Open(OpenFlags.ReadOnly);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error opening certificate store", ex);
                }
                collectionToReturn = store.Certificates;
            }
            else
            {
                try
                {
                    IntPtr storeHandle = CertOpenStore(CERT_STORE_PROV_SYSTEM, 0, 0, CERT_SYSTEM_STORE_LOCAL_MACHINE, string.Format(@"\\{0}\{1}", ComputerName, givenStoreName));
                    if (storeHandle == IntPtr.Zero)
                    {
                        throw new Exception(string.Format("Cannot connect to remote machine: {0}", ComputerName));
                    }

                    IntPtr currentCertContext = IntPtr.Zero;
                    collectionToReturn = new X509Certificate2Collection();
                    do
                    {
                        currentCertContext = CertEnumCertificatesInStore(storeHandle, currentCertContext);
                        if (currentCertContext != IntPtr.Zero)
                        {
                            collectionToReturn.Add(new X509Certificate2(currentCertContext));
                        }
                    }
                    while (currentCertContext != (IntPtr)0);

                    CertCloseStore(storeHandle, 0);
                }
                catch (Exception ex)
                {
                    throw new Exception("Error opening Certificate Store", ex);
                }
            }

            return collectionToReturn;
        }

        private static string GetStoreName(CertStoreName certStoreName)
        {
            string storeName = string.Empty;
            switch (certStoreName)
            {
                case CertStoreName.MY:
                    storeName = "My";
                    break;

                case CertStoreName.ROOT:
                    storeName = "Root";
                    break;

                case CertStoreName.CA:
                    storeName = "CA";
                    break;

                case CertStoreName.TRUST:
                    storeName = "Trust";
                    break;
            }
            return storeName;
        }
    }
}