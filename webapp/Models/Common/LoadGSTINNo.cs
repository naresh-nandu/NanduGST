using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System;
using System.Data.Entity.Core.Objects;
using System.IO;
using System.Web.UI;
using System.Net.Mail;
using System.Net;
using System.Text;
using System.Data;
using System.Configuration;
using System.Security.Cryptography.X509Certificates;

namespace SmartAdminMvc.Models.Common
{
    public partial class LoadGSTINNo
    {
        static WePGSPDBEntities db = new WePGSPDBEntities();

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
            //SelectList LstGSTINNo = null;
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
                //LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo");

                return GSTINNo;
            }
        }

        public static object Exist_GSTIN(int iUserId, int iCustId, string RoleName, string GSTIN)
        {
            //SelectList LstGSTINNo = null;
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
                //LstGSTINNo = new SelectList(GSTINNo, "GSTINNo", "GSTINNo", GSTIN);

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
                    throw ex;
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
                    throw ex;
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
                    throw ex;
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
                    throw ex;
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
        public static object GetEWayBillType(string mastertype)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
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

                    //LstSupplyType = new SelectList(Supplytype, "TypeName", "TypeName");
                    return Supplytype;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }


        public static object Exist_GetEWayBillType(string mastertype, string Selected_Value)
        {
            SelectList LstSupplyType = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
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

                    //LstSupplyType = new SelectList(Supplytype, "TypeName", "TypeName", Selected_Value);
                    return Supplytype;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static object DropoDown_Eway(string mastertype)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
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
                    return Supplytype;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
        public static SelectList LoadEwayCustGSTIN(int iCustId)
        {
            SelectList LstGSTIN = null;
            var GSTINo = (from ob in db.TBL_Customer
                          where (ob.CustId == iCustId && ob.RowStatus == true && ob.GSTINNo != null)
                          select new
                          {

                              GSTINNo = ob.GSTINNo
                          }
                       ).ToList();
            LstGSTIN = new SelectList(GSTINo, "GSTINNo");
            return LstGSTIN;
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList Exist_DropDownEwayBill(string mastertype, string selectedValue)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList DropDownSubSupplyType(string mastertype, string SupplyType)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '"+ SupplyType + "_" + mastertype + "' Order by TypeAbbr ASC", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList Exist_DropDownSubSupplyType(string mastertype, string SupplyType, string selectedValue)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + SupplyType + "_" + mastertype + "' Order by TypeAbbr ASC", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList PanList(int CustId)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Cust_PAN where CustId = '" + CustId + "'and rowstatus = 'true'", Con);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    Con.Close();
                    SelectList LstPanList = null;
                    var Pantype = (from ob in dt.AsEnumerable()
                                   select new
                                   {
                                       CompanyName = ob.Field<string>("CompanyName"),
                                       PANNo = ob.Field<string>("PANNo")
                                   }
                               ).ToList();

                    LstPanList = new SelectList(Pantype, "PANNo", "CompanyName");
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList Exist_PanList(int CustId, string Panno)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Cust_PAN where CustId = '" + CustId + "'and rowstatus = 'true'", Con);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    Con.Close();
                    SelectList LstPanList = null;
                    var Pantype = (from ob in dt.AsEnumerable()
                                   select new
                                   {
                                       CompanyName = ob.Field<string>("CompanyName"),
                                       PANNo = ob.Field<string>("PANNo")
                                   }
                               ).ToList();

                    LstPanList = new SelectList(Pantype, "PANNo", "CompanyName", Panno);
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList GSTINList(string Panno)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Cust_GSTIN where PANNo = '" + Panno + "'and rowstatus='true'", Con);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    Con.Close();
                    SelectList LstPanList = null;
                    var GSTIntype = (from ob in dt.AsEnumerable()
                                     select new
                                     {
                                         // GstinId = ob.Field<int>("GSTINId"),
                                         Gstinno = ob.Field<string>("GSTINNo")
                                     }
                               ).ToList();

                    LstPanList = new SelectList(GSTIntype, "Gstinno", "Gstinno");
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
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
                   
                   // LstPanList = new SelectList(null, "Gstinno", "Gstinno");
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList Exist_GSTINList(string Panno, string Gstin)
        {
            using (SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    Con.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Cust_GSTIN where PANNo = '" + Panno + "'and rowstatus='true'", Con);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    Con.Close();
                    SelectList LstPanList = null;
                    var GSTIntype = (from ob in dt.AsEnumerable()
                                     select new
                                     {
                                         // GstinId = ob.Field<int>("GSTINId"),
                                         Gstinno = ob.Field<string>("GSTINNo")
                                     }
                               ).ToList();

                    LstPanList = new SelectList(GSTIntype, "Gstinno", "Gstinno", Gstin);
                    return LstPanList;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList GetDSC_Certificates()
        {
            SelectList LstDSCType = null;
            List<SelectListItem> RecordsType = new List<SelectListItem>();
            X509Certificate2 certObject = null;
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
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName");
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }

        public static SelectList Exist_GetGSTRType(string mastertype, string selectedValue)
        {
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented
                    conn.Open();

                    SqlDataAdapter da = new SqlDataAdapter("select * from TBL_MAS_EWAYBILL where MasterType = '" + mastertype + "'", conn);
                    DataTable dt = new DataTable();

                    da.Fill(dt);
                    conn.Close();
                    SelectList LstSupplyType = null;
                    var Supplytype = (from ob in dt.AsEnumerable()
                                      select new
                                      {
                                          TypeAbbr = ob.Field<string>("TypeAbbr"),
                                          TypeName = ob.Field<string>("TypeName")
                                      }
                               ).ToList();

                    LstSupplyType = new SelectList(Supplytype, "TypeAbbr", "TypeName", selectedValue);
                    return LstSupplyType;
                    #endregion
                }
                catch (Exception ex)
                {
                    throw ex;
                }
            }
        }
    }
}