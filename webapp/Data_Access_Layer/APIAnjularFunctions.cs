using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Web;

namespace SmartAdminMvc.Data_Access_Layer
{
    public class APIAnjularFunctions
    {
        readonly SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value));

            return dictionaryList.ToList<IDictionary>();
        }

        public List<IDictionary> GetInvoiceCount(int CustId, int UserId, string FromDate, string ToDate, string PanNo, string GSTINNo)
        {
            DataSet ds = new DataSet();
            DataTable table = new DataTable();
            try
            {
                #region commented
                Con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_API_Invoice_Statistics", Con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@FromDate", FromDate));
                dCmd.Parameters.Add(new SqlParameter("@ToDate ", ToDate));
                dCmd.Parameters.Add(new SqlParameter("@Pan", PanNo));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);                
                ds.Clear();
                da.Fill(table);
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Con.Close();
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(table);
        }

        public List<IDictionary> GetAPICount(string FromDate, string ToDate, string GSTINNo,string Panno,int CustId)
        {
            DataSet ds = new DataSet();
            DataSet das = new DataSet();
            string RefIds = "";
            if (GSTINNo == "ALL")
            {
                if (Panno == "ALL")
                {
                    das = getGstinList(CustId);
                }
                else
                {
                    das = getGstin(Panno);
                }

                StringBuilder myBuilder = new StringBuilder();
                if (das.Tables[0].Rows.Count > 0)
                {
                    for (int i = 0; i < das.Tables[0].Rows.Count; i++)
                    {
                        myBuilder.Append(das.Tables[0].Rows[i]["GSTINNo"].ToString() + ",");
                    }
                    RefIds = myBuilder.ToString();
                    RefIds = RefIds.TrimEnd(',');
                }
                else
                {
                    RefIds = GSTINNo;
                }
            }            

            try
            {
                #region commented
                Con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_API_Statistics", Con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.CommandTimeout = 0;
                dCmd.Parameters.Add(new SqlParameter("@FromDate",FromDate));
                dCmd.Parameters.Add(new SqlParameter("@ToDate ", ToDate));
                dCmd.Parameters.Add(new SqlParameter("@Gstin", RefIds));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                ds.Clear();
                da.Fill(ds);
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Con.Close();
                Console.WriteLine(ex);
                throw;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        //Get Pan Number Records
        public DataSet getPan(int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Cust_PAN where CustId = @CustId and rowstatus='true'";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        public DataSet getGstinList(int CustId)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where CustId = @CustId and rowstatus='true'";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }

        //Get Gstin Number Records
        public DataSet getGstin(string Panno)
        {
            DataSet ds = new DataSet();
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Cust_GSTIN where PANNo = @PANNo and rowstatus='true'";
                    sqlcmd.Parameters.AddWithValue("@PANNo", Panno);
                    using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                    {
                        da.Fill(ds);
                    }
                }
            }
            return ds;
        }
    }
}