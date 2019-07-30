using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;

namespace WeP_BAL.GSTRDelete
{
   public class GSTRCommonDelete_BL
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public GSTRCommonDelete_BL()
        {
        }

        public static DataSet Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary(int custId,int userid,string GstrType,string ActionType,string Gstin,string fp,string RecordType,string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Delete_EXT_SA_Bulk_Summary", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@UserId", userid));
                cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp",fp));
                cmd.Parameters.Add(new SqlParameter("@RecordType",RecordType));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;

        }

        public static DataSet Retrieve_GSTR1_Delete_EXT_SA_Bulk_Rawdata(int custId, int userid, string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR1_Delete_EXT_SA_Bulk_RawData", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@UserId", userid));
                cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                cmd.Parameters.Add(new SqlParameter("@SourceType", "ALL"));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ds;

        }


        public static void Delete_GSTR1_Delete_EXT_SA_Bulk(int custId, int userid, string GstrType, string ActionType, string Gstin, string fp, string RecordType, string InvoiceNums)
        {
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_SA_Bulk", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@CustId", custId));
                cmd.Parameters.Add(new SqlParameter("@UserId", userid));
                cmd.Parameters.Add(new SqlParameter("@GSTRType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@RecordType", RecordType));
                cmd.Parameters.Add(new SqlParameter("@SourceType", "ALL"));
                cmd.Parameters.Add(new SqlParameter("@InvoiceNums", InvoiceNums));
                cmd.ExecuteNonQuery();
                Con.Close();
                
                #endregion
            }
            catch (Exception ex)
            {
                throw ex;
            }

        }


        public static SelectList GetRecordTypeList()
        {
            SelectList LstRecordType = null;
            List<SelectListItem> RecordType = new List<SelectListItem>();
            RecordType.Add(new SelectListItem
            {
                Text = "UPLOADED TO GSTN",
                Value = "UPLOADED TO GSTN"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "GSTN ERROR RECORDS",
                Value = "GSTN ERROR RECORDS"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "NOT UPLOADED TO GSTN",
                Value = "NOT UPLOADED TO GSTN"
            });
            LstRecordType = new SelectList(RecordType, "Value", "Text");
            return LstRecordType;
        }

        public static SelectList Exist_GetRecordTypeList(string strRecordType)
        {
            SelectList LstRecordType = null;
            List<SelectListItem> RecordType = new List<SelectListItem>();
            RecordType.Add(new SelectListItem
            {
                Text = "UPLOADED TO GSTN",
                Value = "UPLOADED TO GSTN"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "GSTN ERROR RECORDS",
                Value = "GSTN ERROR RECORDS"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "NOT UPLOADED TO GSTN",
                Value = "NOT UPLOADED TO GSTN"
            });

            LstRecordType = new SelectList(RecordType, "Value", "Text", strRecordType);
            return LstRecordType;
        }

        public static object LoadRecordType()
        {
            List<SelectListItem> RecordType = new List<SelectListItem>();
            RecordType.Add(new SelectListItem
            {
                Text = "Uploaded To Gstin",
                Value = "Uploaded To Gstin"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "Gstin Error Records",
                Value = "Gstin Error Records"
            });
            RecordType.Add(new SelectListItem
            {
                Text = "Uploaded To WeP",
                Value = "Uploaded To WeP"
            });
      
            return RecordType;
        }
    }
}
