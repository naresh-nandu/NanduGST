using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeP_DAL.BranchMgmt;
using System.Web.Mvc;

namespace WeP_BAL.BranchMgmt
{
    public class BranchBusinessLayer
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public readonly int _custid;
        public readonly int _userid;
        public readonly string _useraccessid;
        public readonly int _branchid;

        public BranchBusinessLayer(int CustId, int UserId)
        {
            this._custid = CustId;
            this._userid = UserId;
        }

        public BranchBusinessLayer(int CustId, int UserId, string UserAccessId, int BranchId)
        {
            this._custid = CustId;
            this._userid = UserId;
            this._useraccessid = UserAccessId;
            this._branchid = BranchId;
        }

        public static DataSet Branch_Retrieve(int custid)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Ins_Upd_Del_Retrieve_Location", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@panNo", ""));
                cmd.Parameters.Add(new SqlParameter("@gstinNo", ""));
                cmd.Parameters.Add(new SqlParameter("@branchName", ""));
                cmd.Parameters.Add(new SqlParameter("@email", ""));
                cmd.Parameters.Add(new SqlParameter("@custId", custid));
                cmd.Parameters.Add(new SqlParameter("@createdBy", ""));
                cmd.Parameters.Add(new SqlParameter("@userAccessId", ""));
                cmd.Parameters.Add(new SqlParameter("@AccessId", ""));
                cmd.Parameters.Add(new SqlParameter("@branchId", ""));
                cmd.Parameters.Add(new SqlParameter("@Mode", "R"));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;

        }

        public static DataSet Branch_Retrieve_BasedONBranchId(int branchId)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_Branch", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@branchId", branchId));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

            return ds;

        }
        public string Branch_insert_update_delete(string panNo, string gstinNo, string branchName, string email, int AccessId, string mode)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }

                SqlCommand cmd = new SqlCommand("usp_Ins_Upd_Del_Retrieve_Location", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@panNo", panNo));
                cmd.Parameters.Add(new SqlParameter("@gstinNo", gstinNo));
                cmd.Parameters.Add(new SqlParameter("@branchName", branchName));
                cmd.Parameters.Add(new SqlParameter("@email", email));
                cmd.Parameters.Add(new SqlParameter("@custId", this._custid));
                cmd.Parameters.Add(new SqlParameter("@createdBy", this._userid));
                cmd.Parameters.Add(new SqlParameter("@userAccessId", this._useraccessid));
                cmd.Parameters.Add(new SqlParameter("@AccessId", AccessId));
                cmd.Parameters.Add(new SqlParameter("@branchId", this._branchid));
                cmd.Parameters.Add(new SqlParameter("@Mode", mode));

                cmd.ExecuteNonQuery();
                Con.Close();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return "";
        }

        public static int BranchName_Duplicate(string BranchName, int CustId, int branchId, int GstinId, string Mode)
        {
            int status = 0;
            switch (Mode)
            {
                case "Entry":
                    DataTable dt = new DataTable();
                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        sqlcon.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = sqlcon;
                            sqlcmd.CommandText = "Select * from TBL_Cust_Location where branch = @BranchName and custId = @CustId and gstinid = @GSTINId and rowstatus = 1";
                            sqlcmd.Parameters.AddWithValue("@BranchName", BranchName);
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                adt.Fill(dt);
                                if (dt.Rows.Count > 0)
                                {
                                    status = 1;
                                }
                            }
                        }
                        sqlcon.Close();
                    }
                    break;
                case "Edit":
                    DataTable dt1 = new DataTable();
                    using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        sqlcon.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = sqlcon;
                            sqlcmd.CommandText = "Select * from TBL_Cust_Location where branch = @BranchName and custId = @CustId and gstinid = @GSTINId and branchId != @BranchId and rowstatus = 1";
                            sqlcmd.Parameters.AddWithValue("@BranchName", BranchName);
                            sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                            sqlcmd.Parameters.AddWithValue("@GSTINId", GstinId);
                            sqlcmd.Parameters.AddWithValue("@BranchId", branchId);
                            using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd))
                            {
                                adt1.Fill(dt1);
                                if (dt1.Rows.Count > 0)
                                {
                                    status = 1;
                                }
                            }
                        }
                        sqlcon.Close();
                    }
                    break;
            }

            return status;
        }

        public static object getUserlist(int CustId)
        {
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                DataTable dt = new DataTable();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = Con;
                    sqlcmd.CommandText = "select * from UserList where CustId = @CustId and rowstatus = 1";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        adt.Fill(dt);
                    }
                }
                SelectList LstUserlist = null;
                var userList = (from ob in dt.AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                               ).ToList();
                LstUserlist = new SelectList(userList, "UserId", "Name");

                return LstUserlist;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public static SelectList AddOnEdit_UserAcess(int branchId, int CustId)
        {
            try
            {
                #region commented
                Con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Allocate_LocationToUsers", Con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@branchId", branchId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataSet dt = new DataSet();

                da.Fill(dt);
                Con.Close();
                SelectList Lstusers = null;
                var Userlist = (from ob in dt.Tables[1].AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                           ).ToList();

                Lstusers = new SelectList(Userlist, "UserId", "Name");
                return Lstusers;
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }

        public static SelectList DeleteOnEdit_UserAcess(int branchId, int CustId)
        {

            try
            {
                #region commented
                Con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_Allocate_LocationToUsers", Con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@branchId", branchId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataSet dt = new DataSet();

                da.Fill(dt);
                Con.Close();
                SelectList Lstusers = null;
                var Userlist = (from ob in dt.Tables[0].AsEnumerable()
                                select new
                                {
                                    Name = ob.Field<string>("Name"),
                                    UserId = ob.Field<int>("UserId")
                                }
                           ).ToList();

                Lstusers = new SelectList(Userlist, "UserId", "Name");
                return Lstusers;
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
