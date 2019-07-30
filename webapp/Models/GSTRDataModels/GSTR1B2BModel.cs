using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartAdminMvc.Models.GSTRDataModel
{
    public class GSTR1B2BModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string BPInv_Response = "", BPInv_Itms_Response = "";
        SqlCommand cmd = new SqlCommand();

        public string SendRequest(DataTable data)
        {
            try
            {                
                con.Open();
                if (data.Rows.Count > 0)
                {
                    for (int i = 1; i < data.Rows.Count; i++)
                    {
                        SqlDataAdapter adt = new SqlDataAdapter("Select * from TBL_BP_GSTR1_B2B_INV where num = '" + data.Rows[i][2].ToString() + "' and dt = '" + data.Rows[i][3].ToString() + "'", con);
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@BPInvid", SqlDbType.Int).Value = dt.Rows[0]["BPInvid"].ToString();
                            cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = data.Rows[i][5].ToString();
                            cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = data.Rows[i][6].ToString();
                            cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = data.Rows[i][7].ToString();
                            cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = data.Rows[i][8].ToString();
                            cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = data.Rows[i][9].ToString();
                            cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = data.Rows[i][10].ToString();
                            cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = data.Rows[i][11].ToString();
                            cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = data.Rows[i][12].ToString();
                            cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = data.Rows[i][13].ToString();
                            cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = data.Rows[i][14].ToString();
                            cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = data.Rows[i][15].ToString();
                            BPInv_Itms_Response = Common.Functions.InsertIntoTable("TBL_BP_GSTR1_B2B_INV_ITMS", cmd, con);
                        }
                        else
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = data.Rows[i][0].ToString();
                            cmd.Parameters.Add("@ctin", SqlDbType.VarChar).Value = data.Rows[i][1].ToString();
                            cmd.Parameters.Add("@num", SqlDbType.VarChar).Value = data.Rows[i][2].ToString();
                            cmd.Parameters.Add("@dt", SqlDbType.VarChar).Value = data.Rows[i][3].ToString();
                            cmd.Parameters.Add("@val", SqlDbType.Decimal).Value = data.Rows[i][4].ToString();
                            BPInv_Response = Common.Functions.InsertIntoTable("TBL_BP_GSTR1_B2B_INV", cmd, con);

                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@BPInvid", SqlDbType.Int).Value = BPInv_Response;
                            cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = data.Rows[i][5].ToString();
                            cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = data.Rows[i][6].ToString();
                            cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = data.Rows[i][7].ToString();
                            cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = data.Rows[i][8].ToString();
                            cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = data.Rows[i][9].ToString();
                            cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = data.Rows[i][10].ToString();
                            cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = data.Rows[i][11].ToString();
                            cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = data.Rows[i][12].ToString();
                            cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = data.Rows[i][13].ToString();
                            cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = data.Rows[i][14].ToString();
                            cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = data.Rows[i][15].ToString();
                            BPInv_Itms_Response = Common.Functions.InsertIntoTable("TBL_BP_GSTR1_B2B_INV_ITMS", cmd, con);
                        }
                    }
                }
                con.Close();
                return "Data Transfer Success";
            }
            catch (Exception ex)
            {
                return ex.ToString();
            }
        }
    }
}