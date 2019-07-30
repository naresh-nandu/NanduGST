using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web.Mvc;

namespace SmartAdminMvc.Models.Common
{
    public partial class LoadDropDowns
    {
        private static WePGSPDBEntities db = new WePGSPDBEntities();

        protected LoadDropDowns()
        {

        }

        public static SelectList ServiceType()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Goods",
                Value = "Goods"
            });
            items.Add(new SelectListItem
            {
                Text = "Service",
                Value = "Service"
            });

            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_ServiceType(string strServiceType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Goods",
                Value = "Goods"
            });
            items.Add(new SelectListItem
            {
                Text = "Service",
                Value = "Service"
            });

            LstEWBType = new SelectList(items, "Value", "Text", strServiceType);
            return LstEWBType;
        }

        public static object LoadRates()
        {
            var rates = (from rt in db.TBL_RATE_MASTER
                         select new
                         {
                             ratevalue = rt.rate,
                             rateid = rt.rateId
                         }
                       ).ToList();
            return rates;
        }

        public static object LoadUQC()
        {
            var uqc = (from uq in db.TBL_UQC_MASTER
                       where (uq.rowstatus == true)
                       select new
                       {
                           uqc = uq.uqc,
                           uqcdesc = uq.uqcDesc

                       }
                       ).ToList();
            return uqc;
        }

        public static object GSTIN(int iUserId, int iCustId, string RoleName)
        {
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                return GSTINNo;
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList

                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();

                return GSTINNo;
            }
        }

        public static object Exist_GSTIN(int iUserId, int iCustId, string RoleName, string GSTIN)
        {
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                return GSTINNo;
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList

                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();

                return GSTINNo;
            }
        }

        public static SelectList GSTIN_No(int iUserId, int iCustId, string RoleName)
        {
            SelectList LstGSTINNo = null;
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo");
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList
                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo");
            }
            return LstGSTINNo;
        }

        public static SelectList Exist_GSTIN_No(int iUserId, int iCustId, string strGSTINNo, string RoleName)
        {
            SelectList LstGSTINNo = null;
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo", strGSTINNo);
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList
                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo", strGSTINNo);
            }
            return LstGSTINNo;
        }

        public static SelectList GSTIN_No_Based_On_Id(int iUserId, int iCustId, string RoleName)
        {
            SelectList LstGSTINNo = null;
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINId", "GSTINNo");
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList
                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINId", "GSTINNo");
            }
            return LstGSTINNo;
        }

        public static SelectList Exist_GSTIN_No_Based_On_Id(int iUserId, int iCustId, string strGSTINId, string RoleName)
        {
            SelectList LstGSTINNo = null;
            if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
            {
                var GSTINNo = (from p in db.TBL_Cust_GSTIN
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINId", "GSTINNo", strGSTINId);
            }
            else
            {
                var v = from p in db.UserAccess_GSTIN
                        where (p.UserId == iUserId && p.Rowstatus == true)
                        select new
                        {
                            p.TBL_Cust_GSTIN,
                            p.TBL_Customer,
                            p.UserList
                        };

                List<TBL_Cust_GSTIN> custgst = new List<TBL_Cust_GSTIN>();
                foreach (var a in v)
                {
                    custgst.Add(a.TBL_Cust_GSTIN);
                }

                var GSTINNo = (from p in custgst
                               where p.CustId == iCustId && p.rowstatus == true
                               select new
                               {
                                   GSTINId = p.GSTINId,
                                   GSTINNo = p.GSTINNo
                               }).ToList();
                LstGSTINNo = new SelectList(GSTINNo, "GSTINId", "GSTINNo", strGSTINId);
            }
            return LstGSTINNo;
        }

        public static object LoadHSN(int iCustId, string type)
        {
            var hsn = (from ob in db.TBL_HSN_MASTER
                       where (ob.CustomerId == iCustId && ob.rowstatus == true && ob.hsnType == type)
                       select new
                       {
                           hsnid = ob.hsnId,
                           hsncode = ob.hsnCode
                       }
                       ).ToList();
            return hsn;
        }

        public static object LoadDistinctHSN()
        {
            var hsn = (from ob in db.TBL_HSN_MASTER
                       where (ob.CustomerId == 1 && ob.rowstatus == true)
                       select new
                       {
                           hsncode = ob.hsnCode
                       }
                       ).Distinct();
            return hsn;
        }

        public static SelectList LoadCustomerGSTIN(int iCustId)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Buyer
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              BuyerId = ob.BuyerId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "BuyerId", "GSTINNo");
            return LstCTIN;
        }

        public static SelectList CustomerGSTIN(int iCustId)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Buyer
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              BuyerId = ob.BuyerId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo");
            return LstCTIN;
        }

        public static SelectList Exist_CustomerGSTIN(int iCustId, string strCTIN)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Buyer
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              BuyerId = ob.BuyerId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo", strCTIN);
            return LstCTIN;
        }

        public static SelectList LoadSupplierGSTIN(int iCustId)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Supplier
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              SupplierId = ob.SupplierId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "SupplierId", "GSTINNo");
            return LstCTIN;
        }

        public static SelectList SupplierGSTIN(int iCustId)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Supplier
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              SupplierId = ob.SupplierId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo");
            return LstCTIN;
        }

        public static SelectList Exist_SupplierGSTIN(int iCustId, string strCTIN)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Supplier
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                          select new
                          {
                              SupplierId = ob.SupplierId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo", strCTIN);
            return LstCTIN;
        }

        public static SelectList SupplierName_GSTIN(int iCustId, string strSupplierName)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Supplier.Distinct()
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null && ob.SupplierName == strSupplierName)
                          select new
                          {
                              SupplierId = ob.SupplierId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo");
            return LstCTIN;
        }

        public static SelectList Exist_SupplierName_GSTIN(int iCustId, string strSupplierName, string strCTIN)
        {
            SelectList LstCTIN = null;
            var CTINNo = (from ob in db.TBL_Supplier.Distinct()
                          where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null && ob.SupplierName == strSupplierName)
                          select new
                          {
                              SupplierId = ob.SupplierId,
                              GSTINNo = ob.GSTINno
                          }
                       ).ToList();
            LstCTIN = new SelectList(CTINNo, "GSTINNo", "GSTINNo", strCTIN);
            return LstCTIN;
        }

        public static SelectList SupplierName(int iCustId)
        {
            SelectList LstSupplierName = null;
            var SupplierName = (from ob in db.TBL_Supplier
                                where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                                select new
                                {
                                    SupplierId = ob.SupplierId,
                                    SupplierName = ob.SupplierName
                                }
                       ).ToList();

            LstSupplierName = new SelectList(SupplierName, "SupplierName", "SupplierName");
            return LstSupplierName;
        }

        public static SelectList FilterType()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "All Invoices",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Number",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Date",
                Value = "3"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_FilterTypet(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "All Invoices",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Number",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Date",
                Value = "3"
            });

            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList Amendment_FilterType()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2A",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Number",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Date",
                Value = "4"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_Amendment_FilterTypet(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2A",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Number",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Matched Invoices - Invoice Date",
                Value = "4"
            });

            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList FilterType_HoldUnhold()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2A",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Amendment to GSTR2",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_FilterType_HoldUnhold(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Missing in GSTR2A",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Missing in GSTR2",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Mismatch Invoices",
                Value = "3"
            });

            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList Exist_SupplierName(int iCustId, string strSupplierName)
        {
            SelectList LstSupplierName = null;
            var SupplierName = (from ob in db.TBL_Supplier
                                where (ob.CustomerId == iCustId && ob.RowStatus == true && ob.GSTINno != null)
                                select new
                                {
                                    SupplierId = ob.SupplierId,
                                    SupplierName = ob.SupplierName
                                }
                       ).ToList();

            LstSupplierName = new SelectList(SupplierName, "SupplierName", "SupplierName", strSupplierName);
            return LstSupplierName;
        }

        public static SelectList GetGSTR2_2A_CTIN(int CustId, int UserId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2_GSTR2A_Ctins", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstCTIN = null;
                    var SupplierName = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            //SNO = ob.Field<string>("slno"),
                                            CTIN = ob.Field<string>("ctin")
                                        }
                               ).ToList();

                    LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN");
                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Get_Supplier_Name_CTIN_List(int CustId, int UserId, string Gstin, string fromPriod, string toPeriod, string supplierName, string strType)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTINS", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", supplierName));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);
                    conn.Close();
                    SelectList LstCTIN = null;
                    if (strType == "Supplier_Name")
                    {
                        var SupplierName = (from ob in ds.Tables[1].AsEnumerable()
                                            select new
                                            {
                                                CTIN = ob.Field<string>("SupplierName")
                                            }
                              ).ToList();

                        LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN");
                    }
                    else
                    {
                        var SupplierName = (from ob in ds.Tables[0].AsEnumerable()
                                            select new
                                            {
                                                //SNO = ob.Field<string>("slno"),
                                                CTIN = ob.Field<string>("SupplierGstin")
                                            }
                              ).ToList();

                        LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN");
                    }


                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Existing_Get_Supplier_Name_CTIN_List(int CustId, int UserId, string Gstin, string fromPriod, string toPeriod,
            string supplierName, string strType, string strSelected_Value)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2andGSTR2A_Reconciliation_CTINS", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    dCmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                    dCmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPriod));
                    dCmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
                    dCmd.Parameters.Add(new SqlParameter("@SupplierName", supplierName));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataSet ds = new DataSet();
                    da.Fill(ds);

                    conn.Close();
                    SelectList LstCTIN = null;
                    if (strType == "Supplier_Name")
                    {
                        var SupplierName = (from ob in ds.Tables[1].AsEnumerable()
                                            select new
                                            {
                                                CTIN = ob.Field<string>("SupplierName")
                                            }
                              ).ToList();

                        LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN", strSelected_Value);
                    }
                    else
                    {
                        var SupplierName = (from ob in ds.Tables[0].AsEnumerable()
                                            select new
                                            {
                                                //SNO = ob.Field<string>("slno"),
                                                CTIN = ob.Field<string>("SupplierGstin")
                                            }
                              ).ToList();

                        LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN", strSelected_Value);
                    }


                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }


        public static SelectList Exist_GetGSTR2_2A_CTIN(int CustId, int UserId, string strCTIN)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR2_GSTR2A_Ctins", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstCTIN = null;
                    var SupplierName = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            //SNO = ob.Field<string>("slno"),
                                            CTIN = ob.Field<string>("ctin")
                                        }
                               ).ToList();

                    LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN", strCTIN);
                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GetGSTR6_6A_CTIN(int CustId, int UserId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_GSTR6A_Ctins", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstCTIN = null;
                    var SupplierName = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            //SNO = ob.Field<string>("slno"),
                                            CTIN = ob.Field<string>("ctin")
                                        }
                               ).ToList();

                    LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN");
                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_GetGSTR6_6A_CTIN(int CustId, int UserId, string strCTIN)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR6_GSTR6A_Ctins", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstCTIN = null;
                    var SupplierName = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            //SNO = ob.Field<string>("slno"),
                                            CTIN = ob.Field<string>("ctin")
                                        }
                               ).ToList();

                    LstCTIN = new SelectList(SupplierName, "CTIN", "CTIN", strCTIN);
                    return LstCTIN;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GetFlags()
        {
            SelectList LstFlags = null;
            List<SelectListItem> flags = new List<SelectListItem>();
            flags.Add(new SelectListItem
            {
                Text = "NEW INVOICES",
                Value = ""
            });
            flags.Add(new SelectListItem
            {
                Text = "ACCEPTED INVOICES",
                Value = "A"
            });
            flags.Add(new SelectListItem
            {
                Text = "MODIFIED INVOICES",
                Value = "M"
            });
            LstFlags = new SelectList(flags, "Value", "Text");
            return LstFlags;
        }

        public static SelectList Exist_GetFlags(string strFlag)
        {
            SelectList LstFlags = null;
            List<SelectListItem> flags = new List<SelectListItem>();
            flags.Add(new SelectListItem
            {
                Text = "NEW INVOICES",
                Value = ""
            });
            flags.Add(new SelectListItem
            {
                Text = "ACCEPTED INVOICES",
                Value = "A"
            });
            flags.Add(new SelectListItem
            {
                Text = "MODIFIED INVOICES",
                Value = "M"
            });
            LstFlags = new SelectList(flags, "Value", "Text", strFlag);
            return LstFlags;
        }

        public static SelectList ReconcilationRates()
        {
            SelectList LstRates = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "2",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "5",
                Value = "5"
            });
            items.Add(new SelectListItem
            {
                Text = "10",
                Value = "10"
            });

            LstRates = new SelectList(items, "Text", "Value");
            return LstRates;
        }

        public static SelectList Exist_ReconcilationRates(string strRates)
        {
            SelectList LstRates = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "1",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "2",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "5",
                Value = "5"
            });
            items.Add(new SelectListItem
            {
                Text = "10",
                Value = "10"
            });

            LstRates = new SelectList(items, "Text", "Value", strRates);
            return LstRates;
        }


        public static SelectList GetGSTR_RecordsType()
        {
            SelectList LstRecordsType = null;
            List<SelectListItem> RecordsType = new List<SelectListItem>();
            RecordsType.Add(new SelectListItem
            {
                Text = "Uploaded Records",
                Value = "Uploaded Records"
            });
            RecordsType.Add(new SelectListItem
            {
                Text = "Error Records",
                Value = "Error Records"
            });
            LstRecordsType = new SelectList(RecordsType, "Value", "Text");
            return LstRecordsType;
        }

        public static SelectList Exist_GetGSTR_RecordsType(string strRecordType)
        {
            SelectList LstRecordsType = null;
            List<SelectListItem> RecordsType = new List<SelectListItem>();
            RecordsType.Add(new SelectListItem
            {
                Text = "Uploaded Records",
                Value = "Uploaded Records"
            });
            RecordsType.Add(new SelectListItem
            {
                Text = "Error Records",
                Value = "Error Records"
            });
            LstRecordsType = new SelectList(RecordsType, "Value", "Text", strRecordType);
            return LstRecordsType;
        }

        public static SelectList DropDownEwayBill(string mastertype)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_MAS_EWAYBILL where MasterType = @MasterType";
                        sqlcmd.Parameters.AddWithValue("@MasterType", mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            conn.Close();
                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                        }
                    }
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_DropDownEwayBill(string mastertype, string selectedValue)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_MAS_EWAYBILL where MasterType = @MasterType";
                        sqlcmd.Parameters.AddWithValue("@MasterType", mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            conn.Close();

                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                        }
                    }
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList DropDownSubSupplyType(string mastertype, string SupplyType)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "select * from TBL_MAS_EWAYBILL where MasterType = @MasterType Order by TypeAbbr ASC";
                        sqlcmd.Parameters.AddWithValue("@MasterType", SupplyType + "_" + mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);

                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                        }
                    }
                    conn.Close();
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_DropDownSubSupplyType(string mastertype, string SupplyType, string selectedValue)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "select * from TBL_MAS_EWAYBILL where MasterType = @MasterType Order by TypeAbbr ASC";
                        sqlcmd.Parameters.AddWithValue("@MasterType", SupplyType + "_" + mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                        }
                    }
                    conn.Close();
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList PanList(int CustId)
        {
            SelectList LstPanList = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Cust_PAN where CustId = @CustId and rowstatus = 'true'";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var Pantype = (from ob in dt.AsEnumerable()
                                           select new
                                           {
                                               CompanyName = ob.Field<string>("CompanyName"),
                                               PANNo = ob.Field<string>("PANNo")
                                           }
                                       ).ToList();

                            LstPanList = new SelectList(Pantype, "PANNo", "CompanyName");
                        }
                    }
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_PanList(int CustId, string Panno)
        {
            SelectList LstPanList = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Cust_PAN where CustId = @CustId and rowstatus = 'true'";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var Pantype = (from ob in dt.AsEnumerable()
                                           select new
                                           {
                                               CompanyName = ob.Field<string>("CompanyName"),
                                               PANNo = ob.Field<string>("PANNo")
                                           }
                                       ).ToList();

                            LstPanList = new SelectList(Pantype, "PANNo", "CompanyName", Panno);
                        }
                    }
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GSTINList(string Panno)
        {
            SelectList LstPanList = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where PANNo = @PANNo and rowstatus='true'";
                        sqlcmd.Parameters.AddWithValue("@PANNo", Panno);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var GSTIntype = (from ob in dt.AsEnumerable()
                                             select new
                                             {
                                                 // GstinId = ob.Field<int>("GSTINId"),
                                                 Gstinno = ob.Field<string>("GSTINNo")
                                             }
                                       ).ToList();

                            LstPanList = new SelectList(GSTIntype, "Gstinno", "Gstinno");
                        }
                    }
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GSTINList()
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented                   
                    SelectList LstPanList = null;
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_GSTINList(string Panno, string Gstin)
        {
            SelectList LstPanList = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where PANNo = @PANNo and rowstatus='true'";
                        sqlcmd.Parameters.AddWithValue("@PANNo", Panno);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var GSTIntype = (from ob in dt.AsEnumerable()
                                             select new
                                             {
                                                 // GstinId = ob.Field<int>("GSTINId"),
                                                 Gstinno = ob.Field<string>("GSTINNo")
                                             }
                                       ).ToList();

                            LstPanList = new SelectList(GSTIntype, "Gstinno", "Gstinno", Gstin);
                        }
                    }
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GetDSC_Certificates()
        {
            SelectList LstDSCType = null;
            List<SelectListItem> RecordsType = new List<SelectListItem>();
            X509Store my = new X509Store(StoreName.My, StoreLocation.CurrentUser);
            my.Open(OpenFlags.ReadOnly);

            foreach (X509Certificate2 cert in my.Certificates)
            {
                string[] CNName = cert.Subject.ToString().Split(',');
                string[] DSCName = CNName[0].ToString().Split('=');

                string[] CNIssuerName = cert.Issuer.ToString().Split(',');
                string[] DSCIssuerName = CNIssuerName[0].ToString().Split('=');
                RecordsType.Add(new SelectListItem
                {
                    Text = DSCName[1].ToString() + " - " + DSCIssuerName[1].ToString(),
                    Value = CNName[0].ToString()
                });
            }

            LstDSCType = new SelectList(RecordsType, "Value", "Text");
            return LstDSCType;
        }

        public static SelectList GetEWBTypeList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> EWBType = new List<SelectListItem>();
            EWBType.Add(new SelectListItem
            {
                Text = "EWAYBILL",
                Value = "1"
            });
            EWBType.Add(new SelectListItem
            {
                Text = "EWAYBILL CONSOLIDATED",
                Value = "2"
            });
            EWBType.Add(new SelectListItem
            {
                Text = "UPDATE VEHICLE NO",
                Value = "3"
            });
            LstEWBType = new SelectList(EWBType, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_GetEWBTypeList(string strEWBType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> EWBType = new List<SelectListItem>();
            EWBType.Add(new SelectListItem
            {
                Text = "EWAYBILL",
                Value = "1"
            });
            EWBType.Add(new SelectListItem
            {
                Text = "EWAYBILL CONSOLIDATED",
                Value = "2"
            });
            EWBType.Add(new SelectListItem
            {
                Text = "UPDATE VEHICLE NO",
                Value = "3"
            });
            LstEWBType = new SelectList(EWBType, "Value", "Text", strEWBType);
            return LstEWBType;
        }

        public static SelectList GetEWBReasonList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Due To Break Down",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Due To Transhipment",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Others",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "First Time",
                Value = "4"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_GetEWBReasonList(string strReasonType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Due To Break Down",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Due To Transhipment",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Others",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "First Time",
                Value = "4"
            });
            LstEWBType = new SelectList(items, "Value", "Text", strReasonType);
            return LstEWBType;
        }

        public static SelectList GetEWBList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED FOR A TRANSPORTER",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED TO A TRANSPORTER BY A GSTIN",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL BY OTHER PARTY",
                Value = "4"
            });
            items.Add(new SelectListItem
            {
                Text = "GET CONSOLIDATED EWAYBILL",
                Value = "5"
            });
            items.Add(new SelectListItem
            {
                Text = "GET ERROR LIST",
                Value = "6"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_GetEWBList(string strReasonType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED FOR A TRANSPORTER",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED TO A TRANSPORTER BY A GSTIN",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL BY OTHER PARTY",
                Value = "4"
            });
            items.Add(new SelectListItem
            {
                Text = "GET CONSOLIDATED EWAYBILL",
                Value = "5"
            });
            items.Add(new SelectListItem
            {
                Text = "GET ERROR LIST",
                Value = "6"
            });
            LstEWBType = new SelectList(items, "Value", "Text", strReasonType);
            return LstEWBType;
        }

        public static SelectList GetEWBExtendReasonList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Natural Calamity",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Law and Order Situation",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Transhipment",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Accident",
                Value = "4"
            });
            items.Add(new SelectListItem
            {
                Text = "Other",
                Value = "4"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList GetFilingType()
        {
            SelectList LstFilingType = null;
            List<SelectListItem> FileType = new List<SelectListItem>();
            FileType.Add(new SelectListItem
            {
                Text = "DSC",
                Value = "DSC"
            });
            FileType.Add(new SelectListItem
            {
                Text = "EVC",
                Value = "EVC"
            });
            LstFilingType = new SelectList(FileType, "Value", "Text");
            return LstFilingType;
        }

        public static SelectList Exist_GetFilingType(string strFilingType)
        {
            SelectList LstFilingType = null;
            List<SelectListItem> FileType = new List<SelectListItem>();
            FileType.Add(new SelectListItem
            {
                Text = "DSC",
                Value = "DSC"
            });
            FileType.Add(new SelectListItem
            {
                Text = "EVC",
                Value = "EVC"
            });
            LstFilingType = new SelectList(FileType, "Value", "Text", strFilingType);
            return LstFilingType;
        }

        public static SelectList GetGSTRType(string mastertype)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "select * from TBL_MAS_EWAYBILL where MasterType = @MasterType";
                        sqlcmd.Parameters.AddWithValue("@MasterType", mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                        }
                    }
                    conn.Close();
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_GetGSTRType(string mastertype, string selectedValue)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "select * from TBL_MAS_EWAYBILL where MasterType = @MasterType";
                        sqlcmd.Parameters.AddWithValue("@MasterType", mastertype);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            var Supplytype = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  TypeAbbr = ob.Field<string>("TypeAbbr"),
                                                  TypeName = ob.Field<string>("TypeName")
                                              }
                                       ).ToList();

                            LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                        }
                    }
                    conn.Close();
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList GetMakerCheckerApproverList()
        {
            SelectList LstType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Approver",
                Value = "Approver"
            });
            items.Add(new SelectListItem
            {
                Text = "Checker",
                Value = "Checker"
            });
            items.Add(new SelectListItem
            {
                Text = "Maker",
                Value = "Maker"
            });
            LstType = new SelectList(items, "Value", "Text");
            return LstType;
        }

        public static SelectList Exist_GetMakerCheckerApproverList(string strType)
        {
            SelectList LstType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Approver",
                Value = "Approver"
            });
            items.Add(new SelectListItem
            {
                Text = "Checker",
                Value = "Checker"
            });
            items.Add(new SelectListItem
            {
                Text = "Maker",
                Value = "Maker"
            });
            LstType = new SelectList(items, "Value", "Text", strType);
            return LstType;
        }

        public static SelectList GetSourceType()
        {
            SelectList LstSourcelist = null;
            List<SelectListItem> Sourcelst = new List<SelectListItem>();
            Sourcelst.Add(new SelectListItem
            {
                Text = "1",
                Value = "WEP TEMPLATE"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "2",
                Value = "BP DEVICE API"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "3",
                Value = "POS DEVICE API"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "4",
                Value = "ERP DEVICE API"
            });
            LstSourcelist = new SelectList(Sourcelst, "Text", "Value");
            return LstSourcelist;
        }

        public static SelectList Exist_GetSourceType(string SourceType, string MCASetting, string MCAType, string PartnerType)
        {
            SelectList LstSourcelist = null;
            List<SelectListItem> Sourcelst = new List<SelectListItem>();
            Sourcelst.Add(new SelectListItem
            {
                Text = "1",
                Value = PartnerType + " TEMPLATE"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "2",
                Value = "BP DEVICE API"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "3",
                Value = "POS DEVICE API"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "4",
                Value = "ERP DEVICE API"
            });
            if (MCASetting == "True" && MCAType == "Checker")
            {
                Sourcelst.Add(new SelectListItem
                {
                    Text = "5",
                    Value = "OUTWARD/INWARD"
                });
            }
            LstSourcelist = new SelectList(Sourcelst, "Text", "Value", SourceType);
            return LstSourcelist;
        }

        public static SelectList GetViewAndTrackSourceType()
        {
            SelectList LstSourcelist = null;
            List<SelectListItem> Sourcelst = new List<SelectListItem>();
            Sourcelst.Add(new SelectListItem
            {
                Text = "1",
                Value = "Customer Management"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "2",
                Value = "Supplier Management"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "3",
                Value = "CSV Upload"
            });
            LstSourcelist = new SelectList(Sourcelst, "Text", "Value");
            return LstSourcelist;
        }

        public static SelectList Exist_GetViewAndTrackSourceType(string strSourceType)
        {
            SelectList LstSourcelist = null;
            List<SelectListItem> Sourcelst = new List<SelectListItem>();
            Sourcelst.Add(new SelectListItem
            {
                Text = "1",
                Value = "Customer Management"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "2",
                Value = "Supplier Management"
            });
            Sourcelst.Add(new SelectListItem
            {
                Text = "3",
                Value = "CSV Upload"
            });
            LstSourcelist = new SelectList(Sourcelst, "Text", "Value",strSourceType);
            return LstSourcelist;
        }


        public static SelectList GetFinancialYear()
        {
            SelectList Lstfinanciallist = null;
            List<SelectListItem> financiallst = new List<SelectListItem>();
            financiallst.Add(new SelectListItem
            {
                Text = "2017-18",
                Value = "2017-18"
            });
            financiallst.Add(new SelectListItem
            {
                Text = "2018-19",
                Value = "2018-19"
            });
            Lstfinanciallist = new SelectList(financiallst, "Text", "Value");
            return Lstfinanciallist;
        }

        public static SelectList Exist_GetFinancialYear(string strYear)
        {
            SelectList Lstfinanciallist = null;
            List<SelectListItem> financiallst = new List<SelectListItem>();
            financiallst.Add(new SelectListItem
            {
                Text = "2017-18",
                Value = "2017-18"
            });
            financiallst.Add(new SelectListItem
            {
                Text = "2018-19",
                Value = "2018-19"
            });
            Lstfinanciallist = new SelectList(financiallst, "Text", "Value", strYear);
            return Lstfinanciallist;
        }



        public static SelectList GetMakerUserlist(int CustId, int CheckerUserId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstUserlist = null;
                try
                {
                    #region commented

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    DataTable dt = new DataTable();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Cust_MakerCheckerApproverAccess_Users where CustId = @CustId and rowstatus = 1 and CheckerUserId = @CheckerUserId";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        sqlcmd.Parameters.AddWithValue("@CheckerUserId", CheckerUserId);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                    var userList = (from ob in dt.AsEnumerable()
                                    select new
                                    {
                                        MakerName = ob.Field<string>("MakerName"),
                                        MakerUserId = ob.Field<int>("MakerUserId")
                                    }
                                   ).ToList();
                    LstUserlist = new SelectList(userList, "MakerUserId", "MakerName");
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstUserlist;
            }
        }

        public static SelectList Exist_GetMakerUserlist(int CustId, int CheckerUserId, string MakerId)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstUserlist = null;
                try
                {
                    #region commented

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    DataTable dt = new DataTable();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_Cust_MakerCheckerApproverAccess_Users where CustId = @CustId and rowstatus = 1 and CheckerUserId = @CheckerUserId";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        sqlcmd.Parameters.AddWithValue("@CheckerUserId", CheckerUserId);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                    var userList = (from ob in dt.AsEnumerable()
                                    select new
                                    {
                                        MakerName = ob.Field<string>("MakerName"),
                                        MakerUserId = ob.Field<int>("MakerUserId")
                                    }
                                   ).ToList();
                    LstUserlist = new SelectList(userList, "MakerUserId", "MakerName", MakerId);
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstUserlist;
            }
        }

        public static SelectList GetRecordTypeList(string GSTRType)
        {
            SelectList LstRecordType = null;
            List<SelectListItem> RecordType = new List<SelectListItem>();
            RecordType.Add(new SelectListItem
            {
                Text = "NOT UPLOADED TO GSTN",
                Value = "NOT UPLOADED TO GSTN"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "GSTN ERROR RECORDS",
                Value = "GSTN ERROR RECORDS"
            });
            if (GSTRType == "GSTR DELETE")
            {
                RecordType.Add(new SelectListItem
                {
                    Text = "UPLOADED TO GSTN",
                    Value = "UPLOADED TO GSTN"
                });
            }
            LstRecordType = new SelectList(RecordType, "Value", "Text");
            return LstRecordType;
        }

        public static SelectList Exist_GetRecordTypeList(string strRecordType, string GSTRType)
        {
            SelectList LstRecordType = null;
            List<SelectListItem> RecordType = new List<SelectListItem>();
            RecordType.Add(new SelectListItem
            {
                Text = "NOT UPLOADED TO GSTN",
                Value = "NOT UPLOADED TO GSTN"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "GSTN ERROR RECORDS",
                Value = "GSTN ERROR RECORDS"
            });
            if (GSTRType == "GSTR DELETE")
            {
                RecordType.Add(new SelectListItem
                {
                    Text = "UPLOADED TO GSTN",
                    Value = "UPLOADED TO GSTN"
                });
            }
            LstRecordType = new SelectList(RecordType, "Value", "Text", strRecordType);
            return LstRecordType;
        }

        public static SelectList GetGSTRList(string strGSTRName)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstGSTRList = null;
                try
                {
                    #region commented                                        
                    string[] str = strGSTRName.Split(',');
                    DataTable dt = new DataTable();
                    dt.Columns.Add("GSTRName", typeof(string));
                    
                    for (int i = 0; i < str.Count(); i++)
                    {
                        dt.Rows.Add(new object[] { str[i].ToString() });
                    }
                    var GSTRList = (from ob in dt.AsEnumerable()
                                    select new
                                    {
                                        GSTRName = ob.Field<string>("GSTRName")
                                    }
                                   ).ToList();
                    LstGSTRList = new SelectList(GSTRList, "GSTRName", "GSTRName");
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstGSTRList;
            }
        }

        public static SelectList Exist_GetGSTRList(string strGSTRName, string SelectedValue)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstGSTRList = null;
                try
                {
                    #region commented
                    string[] str = strGSTRName.Split(',');
                    DataTable dt = new DataTable();
                    dt.Columns.Add("GSTRName", typeof(string));

                    for (int i = 0; i < str.Count(); i++)
                    {
                        dt.Rows.Add(new object[] { str[i].ToString() });
                    }
                    var GSTRList = (from ob in dt.AsEnumerable()
                                    select new
                                    {
                                        GSTRName = ob.Field<string>("GSTRName")
                                    }
                                   ).ToList();
                    LstGSTRList = new SelectList(GSTRList, "GSTRName", "GSTRName", SelectedValue);
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstGSTRList;
            }
        }

        public static SelectList GetGSTRActionList(string strGSTRType)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstActionList = null;
                try
                {
                    #region commented

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    DataTable dt = new DataTable();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR_ACTION_TYPE Where GSTRType = @GSTRType";
                        sqlcmd.Parameters.AddWithValue("@GSTRType", strGSTRType);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                    var ActionList = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          ActionName = ob.Field<string>("ActionName")
                                      }
                                   ).ToList();
                    LstActionList = new SelectList(ActionList, "ActionName", "ActionName");
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstActionList;
            }
        }

        public static SelectList Exist_GetGSTRActionList(string strGSTRType, string SelectedValue)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                SelectList LstActionList = null;
                try
                {
                    #region commented

                    if (conn.State == ConnectionState.Closed)
                    {
                        conn.Open();
                    }
                    DataTable dt = new DataTable();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = conn;
                        sqlcmd.CommandText = "Select * from TBL_GSTR_ACTION_TYPE Where GSTRType = @GSTRType";
                        sqlcmd.Parameters.AddWithValue("@GSTRType", strGSTRType);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            adt.Fill(dt);
                        }
                    }
                    var ActionList = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          ActionName = ob.Field<string>("ActionName")
                                      }
                                   ).ToList();
                    LstActionList = new SelectList(ActionList, "ActionName", "ActionName", SelectedValue);
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
                return LstActionList;
            }
        }

        public static SelectList Financial_year()
        {
            SelectList financialyear = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Financial_Year where rowstatus = 'true'";
                        //sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var f_year = (from ob in dt.AsEnumerable()
                                           select new
                                           {
                                               //CompanyName = ob.Field<string>("CompanyName"),
                                               FinancialYear = ob.Field<string>("FinancialYear")
                                           }
                                       ).ToList();

                            financialyear = new SelectList(f_year, "FinancialYear", "FinancialYear");
                        }
                    }
                    return financialyear;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static SelectList Exist_Financial_year(string year)
        {
            SelectList LstPanList = null;
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select * from TBL_Financial_Year where  rowstatus = 'true'";
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                            var f_year = (from ob in dt.AsEnumerable()
                                           select new
                                           {
                                               FinancialYear = ob.Field<string>("FinancialYear")
                                           }
                                       ).ToList();

                            LstPanList = new SelectList(f_year, "FinancialYear", "FinancialYear", year);
                        }
                    }
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }

        public static void Financial_year_Dates(out string startdate,out string enddate,string year)
        {
            string start = null;
            string end = null;

            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = Con;
                        sqlcmd.CommandText = "Select StartDate,Enddate from TBL_Financial_Year where rowstatus = 'true' and FinancialYear = @FinancialYear";
                        sqlcmd.Parameters.AddWithValue("@FinancialYear", year);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            Con.Close();

                           if(dt.Rows.Count > 0)
                            {
                                start = dt.Rows[0]["StartDate"].ToString();
                                end = dt.Rows[0]["Enddate"].ToString();
                            }
                        }
                    }

                    startdate = start;
                    enddate = end;
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
        }
    }
}