using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.Configuration;

namespace SmartAdminMvc
{
    public partial class Invoice_update : System.Web.UI.Page
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        protected void Page_Load(object sender, EventArgs e)
        {

            int invid = Convert.ToInt32(Session["id"].ToString());
            string action = Session["action"].ToString();
            string type = Session["type"].ToString();

            if (!IsPostBack)
            {
                LoadGSTR2_Items_ITC_data(invid, action);
            }
        }

        protected void LoadGSTR2_Items_ITC_data(int id, string action)
        {
            SqlCommand cmd = new SqlCommand("usp_RetrieveMismatchedInvoiceItemsInGSTR2andGSTR2A", con);
            cmd.Parameters.AddWithValue("@invid", id);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            con.Open();
            SqlDataAdapter da = new SqlDataAdapter(cmd);
            DataSet ds = new DataSet();
            da.Fill(ds);

            if (action == "B2B" || action == "CDNR" || action == "CDN")
            {
                grdInvoice.Visible = true;
                grdInvoice.DataSource = ds.Tables[0];
                grdInvoice.DataBind();                
            }
            con.Close();
        }

        public void Edit(object sender, EventArgs e)
        {
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            GridViewRow grdrw;
            int invid = Convert.ToInt32(Session["id"].ToString());
            string action = Session["action"].ToString();
            string type = Session["type"].ToString();

            decimal rt, iamt, camt, samt, csamt, txval, txi, txc, txs, txcs;
            string elg;
            grdrw = ((GridViewRow)((Button)sender).NamingContainer);
            // Item Details
            rt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtirt")).Text);
            iamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtiamt")).Text);
            camt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcamt")).Text);
            samt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtsamt")).Text);
            csamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcsamt")).Text);
            txval = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxval")).Text);

            // ITC Details
            txi = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxi")).Text);
            txc = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxc")).Text);
            txs = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxs")).Text);
            txcs = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxcs")).Text);
            elg = ((DropDownList)grdrw.FindControl("txtelg")).SelectedItem.Text;

            int itmsid = Convert.ToInt32(((Label)grdrw.FindControl("lblitmsid")).Text);

            if (elg == "Select")
            {
                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Please select Eligibility');", true);
            }
            else
            {
                SqlCommand cmd = new SqlCommand("usp_Update_GSTR2_Inv_Itms_ITC", con);
                cmd.Parameters.AddWithValue("@RefId", itmsid);
                cmd.Parameters.AddWithValue("@ActionType", action);
                cmd.Parameters.AddWithValue("@UpdateType", type);
                cmd.Parameters.AddWithValue("@TxVal", txval);
                cmd.Parameters.AddWithValue("@Rt", rt);
                cmd.Parameters.AddWithValue("@Iamt", iamt);
                cmd.Parameters.AddWithValue("@Camt", camt);
                cmd.Parameters.AddWithValue("@Samt", samt);
                cmd.Parameters.AddWithValue("@Csamt", csamt);

                cmd.Parameters.AddWithValue("@Elg", elg);
                cmd.Parameters.AddWithValue("@Tx_I", txi);
                cmd.Parameters.AddWithValue("@Tx_C", txc);
                cmd.Parameters.AddWithValue("@Tx_S", txs);
                cmd.Parameters.AddWithValue("@Tx_Cs", txcs);
                cmd.Parameters.AddWithValue("@UserId", userid);
                cmd.Parameters.AddWithValue("@CustId", custid);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                con.Open();
                cmd.ExecuteNonQuery();
                con.Close();

                LoadGSTR2_Items_ITC_data(invid, action);

                ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Items Updated Successfully');", true);
            }            
        }

        protected void grdInvoice_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                //Find the DropDownList in the Row
                DropDownList ddlelg = (e.Row.FindControl("txtelg") as DropDownList);
                Label elg = (e.Row.FindControl("lblelg") as Label);
                TextBox strtxval = (e.Row.FindControl("txttxval") as TextBox);
                TextBox striamt = (e.Row.FindControl("txtiamt") as TextBox);
                TextBox strcamt = (e.Row.FindControl("txtcamt") as TextBox);
                TextBox strsamt = (e.Row.FindControl("txtsamt") as TextBox);
                TextBox strcsamt = (e.Row.FindControl("txtcsamt") as TextBox);
                TextBox strtxi = (e.Row.FindControl("txttxi") as TextBox);
                TextBox strtxc = (e.Row.FindControl("txttxc") as TextBox);
                TextBox strtxs = (e.Row.FindControl("txttxs") as TextBox);
                TextBox strtxcs = (e.Row.FindControl("txttxcs") as TextBox);

                if (!string.IsNullOrEmpty(elg.Text.ToUpper()))
                {
                    ddlelg.Items.FindByValue(elg.Text.ToUpper()).Selected = true;
                }                
                
                if (string.IsNullOrEmpty(strtxval.Text))
                {
                    strtxval.Text = "0";
                }
                
                if (string.IsNullOrEmpty(striamt.Text))
                {
                    striamt.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strcamt.Text))
                {
                    strcamt.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strsamt.Text))
                {
                    strsamt.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strcsamt.Text))
                {
                    strcsamt.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strtxi.Text))
                {
                    strtxi.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strtxc.Text))
                {
                    strtxc.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strtxs.Text))
                {
                    strtxs.Text = "0";
                }
                
                if (string.IsNullOrEmpty(strtxcs.Text))
                {
                    strtxcs.Text = "0";
                }

                if (ddlelg.SelectedItem.Text == "no")
                {
                    strtxi.Text = "0.00";
                    strtxc.Text = "0.00";
                    strtxs.Text = "0.00";
                    strtxcs.Text = "0.00";

                    strtxi.Enabled = false;
                    strtxc.Enabled = false;
                    strtxs.Enabled = false;
                    strtxcs.Enabled = false;
                }
                else
                {
                    strtxi.Enabled = true;
                    strtxc.Enabled = true;
                    strtxs.Enabled = true;
                    strtxcs.Enabled = true;
                }

            }
        }

        public void EditCDNR(object sender, EventArgs e)
        {
            GridViewRow grdrw;
            int id = Convert.ToInt32(Session["id"].ToString());
            string action = Session["action"].ToString();
            string type = Session["type"].ToString();

            decimal rt, iamt, camt, samt, csamt;
            grdrw = ((GridViewRow)((Button)sender).NamingContainer);
            rt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtirt")).Text);
            iamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtiamt")).Text);
            camt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcamt")).Text);
            samt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtsamt")).Text);
            csamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcsamt")).Text);

            SqlCommand cmd = new SqlCommand("Update_Mismatch_InvoiceItemdata_GSTR2", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.Parameters.AddWithValue("@GstrType", type);
            cmd.Parameters.AddWithValue("@rt", rt);
            cmd.Parameters.AddWithValue("@iamt", iamt);
            cmd.Parameters.AddWithValue("@camt", camt);
            cmd.Parameters.AddWithValue("@samt", samt);
            cmd.Parameters.AddWithValue("@csamt", csamt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Items Updated Successfully');", true);
        }
        public void EditIMPG(object sender, EventArgs e)
        {
            GridViewRow grdrw;
            int id = Convert.ToInt32(Session["id"].ToString());
            string action = Session["action"].ToString();
            string type = Session["type"].ToString();

            decimal rt, iamt, camt, samt, csamt, txval;
            grdrw = ((GridViewRow)((Button)sender).NamingContainer);
            rt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtirt")).Text);
            iamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtiamt")).Text);
            camt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcamt")).Text);
            samt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtsamt")).Text);
            csamt = Convert.ToDecimal(((TextBox)grdrw.FindControl("txtcsamt")).Text);
            txval = Convert.ToDecimal(((TextBox)grdrw.FindControl("txttxval")).Text);
            id = Convert.ToInt32(grdrw.Cells[0].Text);

            SqlCommand cmd = new SqlCommand("Update_Mismatch_InvoiceItemdata_GSTR2", con);
            cmd.Parameters.AddWithValue("@id", id);
            cmd.Parameters.AddWithValue("@action", action);
            cmd.Parameters.AddWithValue("@GstrType", type);
            cmd.Parameters.AddWithValue("@txval", txval);
            cmd.Parameters.AddWithValue("@irt", rt);
            cmd.Parameters.AddWithValue("@iamt", iamt);
            cmd.Parameters.AddWithValue("@camt", camt);
            cmd.Parameters.AddWithValue("@samt", samt);
            cmd.Parameters.AddWithValue("@csamt", csamt);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.CommandTimeout = 0;
            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            ScriptManager.RegisterStartupScript(this, this.GetType(), "redirect", "alert('Items Updated Successfully');", true);
        }

        protected void txtelg_SelectedIndexChanged(object sender, EventArgs e)
        {
            var ddlelg = ((DropDownList)(grdInvoice.FindControl("txtelg")));
            ddlelg = (DropDownList)sender;
            GridViewRow gvr = (GridViewRow)ddlelg.NamingContainer;
            var strtxi = ((TextBox)(gvr.FindControl("txttxi")));
            var strtxc = ((TextBox)(gvr.FindControl("txttxc")));
            var strtxs = ((TextBox)(gvr.FindControl("txttxs")));
            var strtxcs = ((TextBox)(gvr.FindControl("txttxcs")));

            if (ddlelg.SelectedItem.Text == "no")
            {
                strtxi.Text = "0.00";
                strtxc.Text = "0.00";
                strtxs.Text = "0.00";
                strtxcs.Text = "0.00";

                strtxi.Enabled = false;
                strtxc.Enabled = false;
                strtxs.Enabled = false;
                strtxcs.Enabled = false;
            }
            else
            {
                strtxi.Enabled = true;
                strtxc.Enabled = true;
                strtxs.Enabled = true;
                strtxcs.Enabled = true;
            }
        }
    }
}