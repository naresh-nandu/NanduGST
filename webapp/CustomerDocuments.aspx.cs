using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SmartAdminMvc
{
    public partial class CustomerDocuments : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                int CustId = Convert.ToInt32(Session["SesCustDocsId"]);
                
                using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    sqlcon.Open();
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = sqlcon;
                        sqlcmd.CommandText = "Select * from TBL_Cust_Docs where CustomerId = @CustId";
                        sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                        using (SqlDataAdapter da = new SqlDataAdapter(sqlcmd))
                        {
                            DataSet ds = new DataSet();
                            da.Fill(ds);
                            GridView1.DataSource = ds;
                            GridView1.DataBind();
                        }
                    }
                    sqlcon.Close();
                }
            }
        }

        protected void GridView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                sqlcon.Open();
                SqlCommand com = new SqlCommand("select FileName, FileContentType, FileData from TBL_Cust_Docs where UploadFileId = @UploadFileId", sqlcon);
                com.Parameters.AddWithValue("UploadFileId", GridView1.SelectedRow.Cells[1].Text);
                SqlDataReader dr = com.ExecuteReader();
                if (dr.Read())
                {
                    Response.Clear();
                    Response.Buffer = true;
                    Response.ContentType = dr["FileContentType"].ToString();
                    Response.AddHeader("content-disposition", "attachment;filename=" + dr["FileName"].ToString());     // to open file prompt Box open or Save file         
                    Response.Charset = "";
                    Response.Cache.SetCacheability(HttpCacheability.NoCache);
                    Response.BinaryWrite((byte[])dr["FileData"]);
                    Response.End();
                }
                sqlcon.Close();
            }
        }
    }
}