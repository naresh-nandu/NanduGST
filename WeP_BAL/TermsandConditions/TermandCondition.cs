using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace WeP_BAL.TermsandConditions
{
   

    public class TermandCondition
    {
        readonly static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public string TermsandCondition { get; set; }
        public int termsid { get; set; }
        public Nullable<int> custid { get; set; }
        public Nullable<int> CreatedBy { get; set; }
        public Nullable<System.DateTime> CreatedDate { get; set; }
        public Nullable<int> Lastmodifiedby { get; set; }
        public Nullable<System.DateTime> LastModifiedDate { get; set; }
        public Nullable<bool> rowstatus { get; set; }

        #region "Retrieving Terms and Conditions"
        public static List<IDictionary> GetTermsAndConditions(int CustId, int UserId)
        {
            DataTable dtable = new DataTable();

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                try
                {
                    #region commented

                    if (con.State == ConnectionState.Closed)
                    {
                        con.Open();
                    }
                    SqlCommand dCmd = new SqlCommand("usp_Retrieve_TermsandCondition", conn);
                    dCmd.CommandType = CommandType.StoredProcedure;
                    dCmd.CommandTimeout = 0;
                    dCmd.Parameters.Add(new SqlParameter("@UserId",UserId));
                    dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                    SqlDataAdapter da = new SqlDataAdapter(dCmd);
                    da.Fill(dtable);
                    conn.Close();
                    #endregion
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                    throw;
                }
            }
            return ConvertToDictionary(dtable);
        }
        
        #endregion



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
    }
}
