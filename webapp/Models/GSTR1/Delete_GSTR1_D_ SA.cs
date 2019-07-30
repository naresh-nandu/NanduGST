using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;

namespace SmartAdminMvc.Models.GSTR1
{
    public partial class Delete_GSTR1_D_SA
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public void GSTR1_D_Delete(string strActionType, int[] ids)
        {
            try
            {
                con.Open();
                foreach (int invid in ids)
                {
                    SqlCommand cmd = new SqlCommand("usp_Delete_GSTR1_D_SA", con);
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.Add(new SqlParameter("@ActionType", strActionType));
                    cmd.Parameters.Add(new SqlParameter("@RefId", invid));
                    cmd.Parameters.Add(new SqlParameter("@GstinId", ""));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceNo", ""));
                    cmd.Parameters.Add(new SqlParameter("@InvoiceDate", ""));
                    //cmd.Parameters.Add(new SqlParameter("@ErrorMessage", SqlDbType.NVarChar)).Direction = ParameterDirection.Output;
                    cmd.ExecuteNonQuery();
                }
                con.Close();
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
    }
}