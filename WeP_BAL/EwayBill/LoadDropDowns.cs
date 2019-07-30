using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WeP_BAL.EwayBill
{
    public partial class LoadDropDowns
    {
        protected LoadDropDowns()
        {
            //
        }

        public static SelectList branchList(int UserId, int CustId, int GstinId, string RoleName)
        {
            SelectList LstBranchList = null;
            try
            {
                #region commented
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId,t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And gstinId = @GSTINId And  t2.CustId = @CustId Group by t1.BranchId, t2.branch Order By 1 desc";
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        }
                        else
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId,t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And UserId = @UserId And gstinId = @GSTINId Order By 1 desc";
                            sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(dt);
                            var BranchList = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  BranchName = ob.Field<string>("BranchName"),
                                                  BranchId = ob.Field<int>("BranchId")
                                              }
                                   ).ToList();

                            LstBranchList = new SelectList(BranchList, "BranchId", "BranchName");
                        }
                    }
                }
                return LstBranchList;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public static SelectList Exist_branchList(int UserId, int CustId, int GstinId, string RoleName, int BranchId)
        {
            SelectList LstBranchList = null;
            try
            {
                #region commented
                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId,t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And gstinId = @GSTINId And  t2.CustId = @CustId Group by t1.BranchId, t2.branch Order By 1 desc";
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        }
                        else
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId,t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And UserId = @UserId And gstinId = @GSTINId Order By 1 desc";
                            sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(dt);
                            var BranchList = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  BranchName = ob.Field<string>("BranchName"),
                                                  BranchId = ob.Field<int>("BranchId")
                                              }
                                   ).ToList();

                            LstBranchList = new SelectList(BranchList, "BranchId", "BranchName", BranchId);
                        }
                    }
                }
                return LstBranchList;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }


        public static SelectList branchListForReport(int UserId, int CustId, string RoleName)
        {
            SelectList LstBranchList = null;
            try
            {
                #region commented

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
                        {
                            sqlcmd.CommandText = "Select branch as BranchName, branchId as BranchId from TBL_Cust_Location where rowstatus = 1 And custId = @CustId";
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);

                        }
                        else
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId, t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And UserId = @UserId";
                            sqlcmd.Parameters.AddWithValue("@UserId", UserId);

                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(dt);
                            var BranchList = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  BranchName = ob.Field<string>("BranchName"),
                                                  BranchId = ob.Field<int>("BranchId")
                                              }
                                       ).ToList();

                            LstBranchList = new SelectList(BranchList, "BranchId", "BranchName");
                        }
                    }
                }
                return LstBranchList;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public static SelectList Exist_branchListForReport(int UserId, int CustId, string RoleName, int BranchId)
        {
            SelectList LstBranchList = null;
            try
            {
                #region commented

                DataTable dt = new DataTable();
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        if (RoleName == "admin" || RoleName == "Admin" || RoleName == "Super Admin")
                        {
                            sqlcmd.CommandText = "Select branch as BranchName, branchId as BranchId from TBL_Cust_Location where rowstatus = 1 And custId = @CustId";
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);

                        }
                        else
                        {
                            sqlcmd.CommandText = "Select t1.BranchId as BranchId, t2.branch as BranchName from TBL_LocationAccess_Users t1 Inner Join TBL_Cust_Location t2 On t2.branchId = t1.BranchId where t1.rowstatus = 1 And t2.rowstatus = 1 And UserId = @UserId";
                            sqlcmd.Parameters.AddWithValue("@UserId", UserId);

                        }
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            da.Fill(dt);
                            var BranchList = (from ob in dt.AsEnumerable()
                                              select new
                                              {
                                                  BranchName = ob.Field<string>("BranchName"),
                                                  BranchId = ob.Field<int>("BranchId")
                                              }
                                       ).ToList();

                            LstBranchList = new SelectList(BranchList, "BranchId", "BranchName", BranchId);
                        }
                    }
                }
                return LstBranchList;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }


        public static SelectList GetEwayUserwise(int CustId)
        {
            SelectList Username = null;
            try
            {
                #region commented
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        sqlcmd.CommandText = "Select * from UserList where CustId = @CustId and rowstatus = 'true'";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            var name = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            TypeAbbr = ob.Field<int>("UserId"),
                                            TypeName = ob.Field<string>("Name")
                                        }
                                       ).ToList();

                            Username = new SelectList(name, "TypeAbbr", "TypeName");
                        }
                    }
                }
                return Username;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static SelectList Exist_GetEwayUserwise(int CustId, int UserId)
        {
            SelectList Username = null;
            try
            {
                #region commented
                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        sqlcmd.CommandText = "Select * from UserList where CustId = @CustId and rowstatus = 'true'";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            da.Fill(dt);
                            var name = (from ob in dt.AsEnumerable()
                                        select new
                                        {
                                            TypeAbbr = ob.Field<int>("UserId"),
                                            TypeName = ob.Field<string>("Name")
                                        }
                                       ).ToList();

                            Username = new SelectList(name, "TypeAbbr", "TypeName", UserId);
                        }
                    }
                }
                return Username;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public static int Get_GSTINId(string GstinNo)
        {
            int GSTINId = 0;
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where GSTINNo = @GSTINNo and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@GSTINNo", GstinNo);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            GSTINId = Convert.ToInt32(dt.Rows[0]["GSTINId"]);
                        }
                    }
                }
            }
            return GSTINId;
        }

        public static SelectList MyEWBList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Outward Supplies",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Inward Supplies",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Eway Bill Generated By Me",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Consolidate Eway Bill Generated By Me",
                Value = "4"
            });
            items.Add(new SelectListItem
            {
                Text = "Cancelled Eway Bill",
                Value = "5"
            });

            items.Add(new SelectListItem
            {
                Text = "Rejected Eway Bill",
                Value = "6"
            });
            items.Add(new SelectListItem
            {
                Text = "Vehicle Update History",
                Value = "7"
            });


            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_MyEWBList(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Outward Supplies",
                Value = "1"
            });
            items.Add(new SelectListItem
            {
                Text = "Inward Supplies",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Eway Bill Generated By Me",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Consolidate Eway Bill Generated By Me",
                Value = "4"
            });
            items.Add(new SelectListItem
            {
                Text = "Cancelled Eway Bill",
                Value = "5"
            });

            items.Add(new SelectListItem
            {
                Text = "Rejected Eway Bill",
                Value = "6"
            });
            items.Add(new SelectListItem
            {
                Text = "Vehicle Update History",
                Value = "7"
            });

            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList OtherEWBList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Eway Bill Generated By Others",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Rejected Eway Bill By Others",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_OtherEWBList(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Eway Bill Generated By Others",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Rejected Eway Bill By Others",
                Value = "2"
            });


            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }



        public static SelectList GSTR9Part()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Part-2",
                Value = "Part-2"
            });

            items.Add(new SelectListItem
            {
                Text = "Part-3",
                Value = "Part-3"
            });

            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_GSTR9Part(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Part-2",
                Value = "Part-2"
            });

            items.Add(new SelectListItem
            {
                Text = "Part-3",
                Value = "Part-3"
            });


            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }



        public static SelectList GetEWBList()
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "GET Eway Bill",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "GET Consolidate Eway Bill",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_GetEWBList(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GET Eway Bill",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "GET Consolidate Eway Bill",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList TransEWBList()
        {
            SelectList LstEWBType = null;

            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED FOR A TRANSPORTER",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED TO A TRANSPORTER BY A GSTIN",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text");
            return LstEWBType;
        }

        public static SelectList Exist_TransEWBList(string strType)
        {
            SelectList LstEWBType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED FOR A TRANSPORTER",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "GET EWAYBILL ASSIGNED TO A TRANSPORTER BY A GSTIN",
                Value = "2"
            });
            LstEWBType = new SelectList(items, "Value", "Text", strType);
            return LstEWBType;
        }

        public static SelectList invoiceTypeList()
        {
            SelectList LstinvoiceType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Regular Invoices",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Deemed Exports",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "SEZ Exports with payment",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "SEZ exports without payment",
                Value = "4"
            });
            LstinvoiceType = new SelectList(items, "Value", "Text");
            return LstinvoiceType;
        }

        public static SelectList Exist_invoiceTypeList(string strType)
        {
            SelectList LstinvoiceType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Regular Invoices",
                Value = "R"
            });

            items.Add(new SelectListItem
            {
                Text = "Deemed Exports",
                Value = "DE"
            });
            items.Add(new SelectListItem
            {
                Text = "SEZ Exports with payment",
                Value = "SEWP"
            });
            items.Add(new SelectListItem
            {
                Text = "SEZ exports without payment",
                Value = "SEWOP"
            });
            LstinvoiceType = new SelectList(items, "Value", "Text", strType);
            return LstinvoiceType;
        }
        public static SelectList MyStatusList()
        {
            SelectList LstStatusType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Active",
                Value = "Active"
            });
            items.Add(new SelectListItem
            {
                Text = "InActive",
                Value = "InActive"
            });
            items.Add(new SelectListItem
            {
                Text = "Error Records",
                Value = "ErrorRecords"
            });
            items.Add(new SelectListItem
            {
                Text = "Unique Number",
                Value = "UniqueNumber"
            });
            LstStatusType = new SelectList(items, "Value", "Text");
            return LstStatusType;
        }
        public static SelectList Exist_MyStatusList(string strType)
        {
            SelectList LstStatusType = null;
            List<SelectListItem> items = new List<SelectListItem>();
            items.Add(new SelectListItem
            {
                Text = "Active",
                Value = "Active"
            });
            items.Add(new SelectListItem
            {
                Text = "InActive",
                Value = "InActive"
            });
            items.Add(new SelectListItem
            {
                Text = "Error Records",
                Value = "ErrorRecords"
            });
            items.Add(new SelectListItem
            {
                Text = "Unique Number",
                Value = "UniqueNumber"
            });
            LstStatusType = new SelectList(items, "Value", "Text", strType);
            return LstStatusType;
        }
        public static SelectList TransactionTypeList()
        {
            SelectList LstTransactionType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Regular",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Bill To - Ship To",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Bill From - Dispatch From",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Combination of 2 and 3",
                Value = "4"
            });
            LstTransactionType = new SelectList(items, "Value", "Text");
            return LstTransactionType;
        }

        public static SelectList Exist_TransactionTypeList(string strType)
        {
            SelectList LstTransactionType = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Regular",
                Value = "1"
            });

            items.Add(new SelectListItem
            {
                Text = "Bill To - Ship To",
                Value = "2"
            });
            items.Add(new SelectListItem
            {
                Text = "Bill From - Dispatch From",
                Value = "3"
            });
            items.Add(new SelectListItem
            {
                Text = "Combination of 2 and 3",
                Value = "4"
            });
            LstTransactionType = new SelectList(items, "Value", "Text", strType);
            return LstTransactionType;
        }





        public static SelectList FinancialList()
        {
            SelectList LstFinancialList = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "2017-2018",
                Value = "2017-2018"
            });

            items.Add(new SelectListItem
            {
                Text = "2018-2019",
                Value = "2018-2019"
            });

            items.Add(new SelectListItem
            {
                Text = "2019-2020",
                Value = "2019-2020"
            });

            LstFinancialList = new SelectList(items, "Value", "Text");
            return LstFinancialList;
        }

        public static SelectList Exist_FinancialList(string strType)
        {
            SelectList LstFinancialList = null;
            List<SelectListItem> items = new List<SelectListItem>();


            items.Add(new SelectListItem
            {
                Text = "2017-2018",
                Value = "2017-2018"
            });

            items.Add(new SelectListItem
            {
                Text = "2018-2019",
                Value = "2018-2019"
            });
            items.Add(new SelectListItem
            {
                Text = "2019-2020",
                Value = "2019-2020"
            });
            LstFinancialList = new SelectList(items, "Value", "Text", strType);
            return LstFinancialList;
        }


        public static SelectList DataModeList()
        {
            SelectList LstFinancialList = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Data Uploaded to WeP",
                Value = "WeP"
            });

            items.Add(new SelectListItem
            {
                Text = "Data Uploaded to GSTIN",
                Value = "GSTIN"
            });
 

            LstFinancialList = new SelectList(items, "Value", "Text");
            return LstFinancialList;
        }

        public static SelectList Exist_DataModeList(string strType)
        {
            SelectList LstFinancialList = null;
            List<SelectListItem> items = new List<SelectListItem>();



            items.Add(new SelectListItem
            {
                Text = "Data Uploaded to WeP",
                Value = "WeP"
            });

            items.Add(new SelectListItem
            {
                Text = "Data Uploaded to GSTIN",
                Value = "GSTIN"
            });

            
            LstFinancialList = new SelectList(items, "Value", "Text", strType);
            return LstFinancialList;
        }

        public static SelectList GetGSTR9DataList()
        {
            SelectList GetGSTR9DataList = null;
            List<SelectListItem> items = new List<SelectListItem>();

            items.Add(new SelectListItem
            {
                Text = "Get Autocalculated Details",
                Value = "Autocalculated"
            });

            


            GetGSTR9DataList = new SelectList(items, "Value", "Text");
            return GetGSTR9DataList;
        }

        public static SelectList Exist_GetGSTR9DataList(string strType)
        {
            SelectList GetGSTR9DataList = null;
            List<SelectListItem> items = new List<SelectListItem>();



            items.Add(new SelectListItem
            {
                Text = "Get Autocalculated Details",
                Value = "Autocalculated"
            });

        

            GetGSTR9DataList = new SelectList(items, "Value", "Text", strType);
            return GetGSTR9DataList;
        }
    }
}
