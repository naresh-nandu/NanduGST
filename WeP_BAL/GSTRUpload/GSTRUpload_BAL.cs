using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using WeP_DAL.GSTRUpload;

namespace WeP_BAL.GSTRUpload
{

  
  public partial class GSTRUpload_BAL
    {
        public static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        
        #region "Retrieve Procedure for File imported time and immediately need to call by passing file details"
        public static List<IDictionary> Retrieve_CSV_Data(string GstrType,string FileName,string UserId, string ReferenceNo, string TemplateTypeId, int CustId)
        {


            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                  
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_CSV_Imported", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                 ds.Clear();

                da.Fill(ds);

                var das = ds.Tables[0].AsEnumerable();

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion

        #region "Retrieve Procedure for File imported time and immediately need to call by passing file details"
        public static List<IDictionary> Retrieve_TALLY_Data(string GstrType, string FileName, string UserId, string ReferenceNo, string TemplateTypeId, int CustId)
        {


            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_CSV_Imported", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                ds.Clear();

                da.Fill(ds);

                var das = ds.Tables[0].AsEnumerable();

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion

        #region "Delete procedure for File Imported  summary data delete"
        public static void Delete_CSV_Data(string GstrType, string FileName, string ActionType,string Gstin,string Fp, string UserId, string ReferenceNo, string TemplateTypeId, int CustId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_CSV_Imported", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                cmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Delete procedure for File Imported  summary data delete Tally"
        public static void Delete_TALLY_Data(string GstrType, string FileName, string ActionType, string Gstin, string Fp, string UserId, string ReferenceNo, string TemplateTypeId, int CustId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_CSV_Imported", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@FileName", FileName));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                cmd.Parameters.Add(new SqlParameter("@TemplateTypeId", TemplateTypeId));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Retrieve procedure for Displaying data after click on "View Complete summary" button in csv upload page itself, under the first grid"
        public static List<IDictionary> Retrieve_View_Summary(string GstrType, string UserId, string ReferenceNo,int CustId)
        {


            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                ds.Clear();

                da.Fill(ds);

                var das = ds.Tables[0].AsEnumerable();

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion

        #region "Retrieve procedure for Displaying data after click on "View Complete summary" button in GSTN/Tally page itself, under the first grid"
        public static List<IDictionary> Retrieve_View_Summary_Tallly(string GstrType, string UserId, string ReferenceNo, int CustId)
        {


            DataSet ds = new DataSet();
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }

                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                dCmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                dCmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                dCmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));

                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                DataTable table = new DataTable();
                ds.Clear();

                da.Fill(ds);

                var das = ds.Tables[0].AsEnumerable();

                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
            return ConvertToDictionary(ds.Tables[0]);
        }

        #endregion

        #region "Delete Procedure for Deleteing data which is displaying after click on "View Complete summary" button in csv upload page itself, under the first grid."
        public static void Delete_View_Sammary_Data(string GstrType, string ActionType,string Gstin,string Fp,string UserId, string ReferenceNo,int CustId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion

        #region "Delete Procedure for Deleteing data which is displaying after click on "View Complete summary" button in tally upload page itself, under the first grid."
        public static void Delete_View_Sammary_Tally_Data(string GstrType, string ActionType, string Gstin, string Fp, string UserId, string ReferenceNo, int CustId)
        {
            try
            {
                if (con.State == ConnectionState.Closed)
                {
                    con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_EXT_Not_Moved_To_SA_Summary", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@GstrType", GstrType));
                cmd.Parameters.Add(new SqlParameter("@ActionType", ActionType));
                cmd.Parameters.Add(new SqlParameter("@Gstin", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp", Fp));
                cmd.Parameters.Add(new SqlParameter("@UserId", UserId));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", ReferenceNo));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@IsTallyDoc", "Y"));
                cmd.ExecuteNonQuery();
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        #endregion


        private static List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();

            return dictionaryList.ToList<IDictionary>();
        }

        #region "DATATABLE DATA LOADING"

        public static void GetB2BCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();

                dataCount = dtable.Rows.Count; // Total DataCount
                HttpContext.Current.Session["dataCount"] = dataCount;

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        etin = dr.IsNull("etin") ? "" : dr["etin"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
               HttpContext.Current.Session["G1_B2BRefIds"] = RefIds;
               

                dList = dataList.ToList(); // Total DataList
                HttpContext.Current.Session["varName"] = dList;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetB2CLCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_B2CLRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetB2CSCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        b2csid = Convert.ToInt32(dr["b2csid"].ToString()),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        etin = dr.IsNull("etin") ? "" : dr["etin"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["b2csid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetCDNRCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        ntid = Convert.ToInt32(dr["ntid"].ToString()),
                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                        ntty = dr.IsNull("ntty") ? "" : dr["ntty"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["ntid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_CDNRRefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetCDNURCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        cdnurid = Convert.ToInt32(dr["cdnurid"].ToString()),
                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                        ntty = dr.IsNull("ntty") ? "" : dr["ntty"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["cdnurid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_CDNURRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetEXPCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        ex_tp = dr.IsNull("ex_tp") ? "" : dr["ex_tp"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        sbnum = dr.IsNull("sbnum") ? "" : dr["sbnum"].ToString(),
                        sbdt = dr.IsNull("sbdt") ? "" : dr["sbdt"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_EXPRefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetATCount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        atid = Convert.ToInt32(dr["atid"].ToString()),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        ad_amt = dr.IsNull("ad_amt") ? 0 : Convert.ToDecimal(dr["ad_amt"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["atid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
               
                    HttpContext.Current.Session["G1_ATRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetTXPCount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        txpid = Convert.ToInt32(dr["txpid"].ToString()),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        ad_amt = dr.IsNull("ad_amt") ? 0 : Convert.ToDecimal(dr["ad_amt"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["txpid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_TXPRefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }


        public static void GetHSNCount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        dataid = Convert.ToInt32(dr["dataid"].ToString()),
                        hsn_sc = dr.IsNull("hsn_sc") ? "" : dr["hsn_sc"].ToString(),
                        descs = dr.IsNull("descs") ? "" : dr["descs"].ToString(),
                        uqc = dr.IsNull("uqc") ? "" : dr["uqc"].ToString(),
                        qty = dr.IsNull("qty") ? 0 : Convert.ToDecimal(dr["qty"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["dataid"].ToString() + ",";

                }

                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_HSNRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetNILCount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        nilid = Convert.ToInt32(dr["nilid"].ToString()),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        nil_amt = dr.IsNull("nil_amt") ? 0 : Convert.ToDecimal(dr["nil_amt"]),
                        expt_amt = dr.IsNull("expt_amt") ? 0 : Convert.ToDecimal(dr["expt_amt"]),
                        ngsup_amt = dr.IsNull("ngsup_amt") ? 0 : Convert.ToDecimal(dr["ngsup_amt"])
                    });
                    RefIds += dr["nilid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_NILRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetDOCISSUECount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        docissueid = Convert.ToInt32(dr["docissueid"].ToString()),
                        doc_num = dr.IsNull("doc_num") ? "" : dr["doc_num"].ToString(),
                        froms = dr.IsNull("froms") ? "" : dr["froms"].ToString(),
                        tos = dr.IsNull("tos") ? "" : dr["tos"].ToString(),
                        totnum = dr.IsNull("totnum") ? "" : dr["totnum"].ToString(),
                        cancel = dr.IsNull("cancel") ? "" : dr["cancel"].ToString(),
                        net_issue = dr.IsNull("net_issue") ? "" : dr["net_issue"].ToString()
                    });
                    RefIds += dr["docissueid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
               
                    HttpContext.Current.Session["G1_DOCRefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetB2BACount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();

                dataCount = dtable.Rows.Count; // Total DataCount
                HttpContext.Current.Session["dataCount"] = dataCount;

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        oinum = dr.IsNull("oinum") ? "" : dr["oinum"].ToString(),
                        oidt = dr.IsNull("oidt") ? "" : dr["oidt"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        etin = dr.IsNull("etin") ? "" : dr["etin"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_B2BARefIds"] = RefIds;
               

                dList = dataList.ToList(); // Total DataList
                HttpContext.Current.Session["varName"] = dList;
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetB2CLACount_And_List(string refNo,
            out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        oinum = dr.IsNull("oinum") ? "" : dr["oinum"].ToString(),
                        oidt = dr.IsNull("oidt") ? "" : dr["oidt"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_B2CLARefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetB2CSACount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        b2csid = Convert.ToInt32(dr["b2csaid"].ToString()),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        omon = dr.IsNull("omon") ? "" : dr["omon"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        etin = dr.IsNull("etin") ? "" : dr["etin"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["b2csaid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_B2CSARefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetCDNRACount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        ntid = Convert.ToInt32(dr["ntid"].ToString()),
                        ctin = dr.IsNull("ctin") ? "" : dr["ctin"].ToString(),
                        ont_num = dr.IsNull("ont_num") ? "" : dr["ont_num"].ToString(),
                        ont_dt = dr.IsNull("ont_dt") ? "" : dr["ont_dt"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                        ntty = dr.IsNull("ntty") ? "" : dr["ntty"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["ntid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_CDNRARefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetCDNURACount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        cdnurid = Convert.ToInt32(dr["cdnuraid"].ToString()),
                        ont_num = dr.IsNull("ont_num") ? "" : dr["ont_num"].ToString(),
                        ont_dt = dr.IsNull("ont_dt") ? "" : dr["ont_dt"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        nt_num = dr.IsNull("nt_num") ? "" : dr["nt_num"].ToString(),
                        nt_dt = dr.IsNull("nt_dt") ? "" : dr["nt_dt"].ToString(),
                        ntty = dr.IsNull("ntty") ? "" : dr["ntty"].ToString(),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["cdnuraid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_CDNURARefIds"] = RefIds;
                
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetEXPACount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        invid = Convert.ToInt32(dr["invid"].ToString()),
                        ex_tp = dr.IsNull("ex_tp") ? "" : dr["ex_tp"].ToString(),
                        oinum = dr.IsNull("oinum") ? "" : dr["oinum"].ToString(),
                        oidt = dr.IsNull("oidt") ? "" : dr["oidt"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        inum = dr.IsNull("inum") ? "" : dr["inum"].ToString(),
                        idt = dr.IsNull("idt") ? "" : dr["idt"].ToString(),
                        val = dr.IsNull("val") ? 0 : Convert.ToDecimal(dr["val"]),
                        sbnum = dr.IsNull("sbnum") ? "" : dr["sbnum"].ToString(),
                        sbdt = dr.IsNull("sbdt") ? "" : dr["sbdt"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        txval = dr.IsNull("txval") ? 0 : Convert.ToDecimal(dr["txval"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"])
                    });
                    RefIds += dr["invid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_EXPARefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetATACount_And_List(string refNo,out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT",con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        atid = Convert.ToInt32(dr["ataid"].ToString()),
                        omon = dr.IsNull("omon") ? "" : dr["omon"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        ad_amt = dr.IsNull("ad_amt") ? 0 : Convert.ToDecimal(dr["ad_amt"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["ataid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
               
                    HttpContext.Current.Session["G1_ATARefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }

        public static void GetTXPACount_And_List(string refNo, out int dataCount, out List<GSTRAttribues_DAL> dList)
        {
            DataTable dtable = new DataTable();
            List<GSTRAttribues_DAL> dataList = new List<GSTRAttribues_DAL>();

            try
            {
                con.Open();
                SqlCommand dCmd = new SqlCommand("usp_Retrieve_GSTR1B2B_EXT", con);
                dCmd.CommandType = CommandType.StoredProcedure;
                dCmd.Parameters.Add(new SqlParameter("@SourceType", "CSV"));
                dCmd.Parameters.Add(new SqlParameter("@GstinNo", ""));
                dCmd.Parameters.Add(new SqlParameter("@ReferenceNo", refNo));
                SqlDataAdapter da = new SqlDataAdapter(dCmd);
                da.Fill(dtable);
                con.Close();
                dataCount = dtable.Rows.Count; // Total DataCount

                string RefIds = "";
                foreach (DataRow dr in dtable.Rows)
                {
                    dataList.Add(new GSTRAttribues_DAL
                    {
                        sno = Convert.ToInt32(dr["SNo"].ToString()),
                        txpid = Convert.ToInt32(dr["txpaid"].ToString()),
                        pos = dr.IsNull("pos") ? "" : dr["pos"].ToString(),
                        sply_ty = dr.IsNull("sply_ty") ? "" : dr["sply_ty"].ToString(),
                        omon = dr.IsNull("omon") ? "" : dr["omon"].ToString(),
                        diff_percent = dr.IsNull("diff_percent") ? 0 : Convert.ToDecimal(dr["diff_percent"]),
                        rt = dr.IsNull("rt") ? 0 : Convert.ToDecimal(dr["rt"]),
                        ad_amt = dr.IsNull("ad_amt") ? 0 : Convert.ToDecimal(dr["ad_amt"]),
                        iamt = dr.IsNull("iamt") ? 0 : Convert.ToDecimal(dr["iamt"]),
                        camt = dr.IsNull("camt") ? 0 : Convert.ToDecimal(dr["camt"]),
                        samt = dr.IsNull("samt") ? 0 : Convert.ToDecimal(dr["samt"]),
                        csamt = dr.IsNull("csamt") ? 0 : Convert.ToDecimal(dr["csamt"])
                    });
                    RefIds += dr["txpaid"].ToString() + ",";
                }
                RefIds = RefIds.TrimEnd(',');
                
                    HttpContext.Current.Session["G1_TXPARefIds"] = RefIds;
               
                dList = dataList.ToList(); // Total DataList
            }
            catch (Exception ex)
            {
                dataCount = 0;
                dList = dataList.ToList();
            }

        }


        #endregion
    }
}
