﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartAdminMvc.Models.GSTRDataModel
{
    public class GSTR1B2CSModel
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        private static string BPInv_Response = "";
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
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = data.Rows[i][0].ToString();
                        cmd.Parameters.Add("@state_cd", SqlDbType.VarChar).Value = data.Rows[i][1].ToString();
                        cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = data.Rows[i][2].ToString();
                        cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = data.Rows[i][3].ToString();
                        cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = data.Rows[i][4].ToString();
                        cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = data.Rows[i][5].ToString();
                        cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = data.Rows[i][6].ToString();
                        cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = data.Rows[i][7].ToString();
                        cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = data.Rows[i][8].ToString();
                        cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = data.Rows[i][9].ToString();
                        cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = data.Rows[i][10].ToString();
                        cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = data.Rows[i][11].ToString();
                        cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = data.Rows[i][12].ToString();

                        BPInv_Response = Common.Functions.InsertIntoTable("TBL_BP_GSTR1_B2CS", cmd, con);
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