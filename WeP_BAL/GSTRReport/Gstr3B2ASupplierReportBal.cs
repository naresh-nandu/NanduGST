using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using WeP_DAL;

namespace WeP_BAL.GSTRReport
{
    public class Gstr3B2ASupplierReportBal
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public readonly int _custid;
        public Gstr3B2ASupplierReportBal(int CustId)
        {
            this._custid = CustId;
        }

        public void ImportCSV(string fileName, string userEmail, string RefNo, out int TotalRecords, out int ProcessRecords, out int ErrorRecords, out DataSet ErrorReport)
        {
            int fileId = 0;
            DataSet dsErrorRecords = new DataSet();
            string tableName = string.Empty;
            DataTable dt = new DataTable();
            DataRow row;
            int totalRecords = 0, processedRecords = 0, errorRecords = 0;

            StreamReader sr = new StreamReader(fileName);
            string line = sr.ReadLine();
            string[] value = line.Split(',');

            foreach (string dc in value)
            {
                dt.Columns.Add(new DataColumn(dc));
            }

            while (!sr.EndOfStream)
            {
                line = sr.ReadLine();
                value = line.Split(',');
                if (value.Length == dt.Columns.Count)
                {
                    row = dt.NewRow();
                    row.ItemArray = value;
                    dt.Rows.Add(row);
                    if (dt.Rows.Count == 60000)
                    {
                        break;
                    }
                }

            }
            sr.Close();

            if (Con.State == ConnectionState.Closed)
            {
                Con.Open();
            }

            SqlCommand cmd = new SqlCommand();
            cmd.Parameters.Clear();
            cmd.Parameters.Add("@filename", SqlDbType.VarChar).Value = fileName;
            cmd.Parameters.Add("@filestatus", SqlDbType.TinyInt).Value = 1;
            cmd.Parameters.Add("@gstrtypeid", SqlDbType.TinyInt).Value = 5;
            cmd.Parameters.Add("@createdby", SqlDbType.NVarChar).Value = userEmail;
            cmd.Parameters.Add("@createddate", SqlDbType.DateTime).Value = DateTime.Now.ToString();
            fileId = Convert.ToInt32(SQLHelper.InsertIntoTable("TBL_RECVD_FILES", cmd, Con));

            dt.Columns.Add(new DataColumn("fileid", typeof(System.Int32)));

            foreach (DataRow dr in dt.Rows)
            {
                dr[10] = fileId;
            }

            SqlBulkCopy copy = new SqlBulkCopy(Con);
            copy.BulkCopyTimeout = 0;
            copy.BatchSize = 5000;

            tableName = "TBL_CSV_GSTR3B_Report_RECS";
            copy.DestinationTableName = tableName;

            copy.ColumnMappings.Add(0, 1); // S.No
            copy.ColumnMappings.Add(1, 2); // GSTIN
            copy.ColumnMappings.Add(2, 3); // FP
            copy.ColumnMappings.Add(3, 4); // CTIN
            copy.ColumnMappings.Add(4, 5); // TaxVal
            copy.ColumnMappings.Add(5, 6); // SGST
            copy.ColumnMappings.Add(6, 7); // CGST
            copy.ColumnMappings.Add(7, 8); // IGST
            copy.ColumnMappings.Add(8, 9); // CESS
            copy.ColumnMappings.Add(9, 10); // Total
            copy.ColumnMappings.Add(10, 0);//Fileid
            copy.WriteToServer(dt);
            copy.Close();

            using (SqlCommand sqlcmd = new SqlCommand())
            {

                sqlcmd.CommandText = "usp_Import_CSV_GSTR3B_Report_PERF";
                sqlcmd.CommandType = CommandType.StoredProcedure;
                sqlcmd.CommandTimeout = 0;
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                sqlcmd.Connection = Con;

                sqlcmd.Parameters.Add("@FileName", SqlDbType.NVarChar).Value = fileName;
                sqlcmd.Parameters.Add("@UserId", SqlDbType.NVarChar).Value = userEmail;
                sqlcmd.Parameters.Add("@ReferenceNo", SqlDbType.NVarChar).Value = RefNo;
                sqlcmd.Parameters.Add("@CustId", SqlDbType.Int).Value = this._custid;


                SqlParameter totalRecordsCount = new SqlParameter();
                totalRecordsCount.ParameterName = "@TotalRecordsCount";
                totalRecordsCount.IsNullable = true;
                totalRecordsCount.DbType = DbType.Int32;
                totalRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(totalRecordsCount);

                SqlParameter processedRecordsCount = new SqlParameter();
                processedRecordsCount.ParameterName = "@ProcessedRecordsCount";
                processedRecordsCount.IsNullable = true;
                processedRecordsCount.DbType = DbType.Int32;
                processedRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(processedRecordsCount);

                SqlParameter errorRecordsCount = new SqlParameter();
                errorRecordsCount.ParameterName = "@ErrorRecordsCount";
                errorRecordsCount.IsNullable = true;
                errorRecordsCount.DbType = DbType.Int32;
                errorRecordsCount.Direction = ParameterDirection.Output;
                sqlcmd.Parameters.Add(errorRecordsCount);

                using (SqlDataAdapter adp = new SqlDataAdapter(sqlcmd))
                {
                    adp.Fill(dsErrorRecords);
                    totalRecords = Convert.ToInt32(sqlcmd.Parameters["@TotalRecordsCount"].Value);
                    processedRecords = Convert.ToInt32(sqlcmd.Parameters["@ProcessedRecordsCount"].Value);
                    errorRecords = Convert.ToInt32(sqlcmd.Parameters["@ErrorRecordsCount"].Value);
                }
            }
            Con.Close();
            TotalRecords = totalRecords;
            ProcessRecords = processedRecords;
            ErrorRecords = errorRecords;
            ErrorReport = dsErrorRecords;
        }

        public static void getUserEmail(int UserId, out string Email)
        {
            string emailId = "";
            using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = con;
                    sqlcmd.CommandText = "Select * from UserList Where UserId = @UserId and RowStatus = 1";
                    sqlcmd.Parameters.AddWithValue("@UserId", UserId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            emailId = dt.Rows[0]["Email"].ToString();
                        }
                    }
                }
            }
            Email = emailId;
        }

        public static DataSet Retrieve_GSTR3BAndGSTR2A_Supplier_Report(string strGSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR3BAndGSTR2A_Supplier_Report", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin", strGSTIN));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPeriod));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
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

        public static SelectList GetReportTypeList()
        {
            SelectList LstReportType = null;
            List<SelectListItem> ReportType = new List<SelectListItem>();
            ReportType.Add(new SelectListItem
            {
                Text = "GSTR3B Supplier Reconciliation",
                Value = "GSTR3B Supplier Reconciliation"
            });
            LstReportType = new SelectList(ReportType, "Value", "Text");
            return LstReportType;
        }

        public static SelectList Existing_GetReportTypeList(string strReportType)
        {
            SelectList LstReportType = null;
            List<SelectListItem> ReportType = new List<SelectListItem>();
            ReportType.Add(new SelectListItem
            {
                Text = "GSTR3B Supplier Reconciliation",
                Value = "GSTR3B Supplier Reconciliation"
            });
            LstReportType = new SelectList(ReportType, "Value", "Text", strReportType);
            return LstReportType;
        }
    }
}
