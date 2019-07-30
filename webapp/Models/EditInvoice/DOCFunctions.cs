using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Collections;

namespace SmartAdminMvc.Models.EditInvoice
{
    public class DocFunctions
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected DocFunctions()
        {
            //
        }

        public static int Insert(string Gstin, string fp, int DOCNum,int Num,string FromNo,string ToNo,int TotalNo,int Cancel,int NetIssue,string Refno, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Insert_Outward_GSTR1_DOC_EXT", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", Gstin));
                cmd.Parameters.Add(new SqlParameter("@Fp ", fp));
                cmd.Parameters.Add(new SqlParameter("@doc_num", DOCNum));
                cmd.Parameters.Add(new SqlParameter("@doc_typ",""));
                cmd.Parameters.Add(new SqlParameter("@num", Num));
                cmd.Parameters.Add(new SqlParameter("@froms",FromNo));
                cmd.Parameters.Add(new SqlParameter("@tos", ToNo));
                cmd.Parameters.Add(new SqlParameter("@totnum", TotalNo));
                cmd.Parameters.Add(new SqlParameter("@cancel", Cancel));
                cmd.Parameters.Add(new SqlParameter("@net_issue",NetIssue));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo ", Refno));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@Retval"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        public static void DOCPush(string RefNo, string GSTIN, int CustId, int UserId)
        {
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Push_GSTR1DOCISSUE_EXT_SA", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@SourceType", "Manual"));
                cmd.Parameters.Add(new SqlParameter("@ReferenceNo", RefNo));
                cmd.Parameters.Add(new SqlParameter("@Gstin", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@CustId", CustId));
                cmd.Parameters.Add(new SqlParameter("@CreatedBy", UserId));
                cmd.ExecuteNonQuery();

                con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
        }


        public static int DocUpdate(int docid, string Mode, int DocNum, string FromNo, string ToNo, int TotalNum, int Cancel, int NetIssue,int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_Doc_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@docid", docid));
                cmd.Parameters.Add(new SqlParameter("@Mode", Mode));
                cmd.Parameters.Add(new SqlParameter("@doc_num", DocNum));
                cmd.Parameters.Add(new SqlParameter("@froms", FromNo));
                cmd.Parameters.Add(new SqlParameter("@tos", ToNo));
                cmd.Parameters.Add(new SqlParameter("@totnum", TotalNum));
                cmd.Parameters.Add(new SqlParameter("@cancel", Cancel));
                cmd.Parameters.Add(new SqlParameter("@net_issue", NetIssue));
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@Retval"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }

        public static int DocDelete(int docid, string Mode, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_Doc_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@docid", docid));
                cmd.Parameters.Add(new SqlParameter("@Mode", Mode));                
                cmd.Parameters.Add(new SqlParameter("@Createdby", Createdby));
                cmd.Parameters.Add("@Retval", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@Retval"].Value);
            }

            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return outputparam;
        }
    }
}