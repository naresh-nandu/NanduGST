using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SmartAdminMvc.Models.CDNR
{
    public class CDNRFunctions
    {
        readonly SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public int CDNRMasterUpdate(string gstin, string period, string ctin, string notetype, string notenumber, string notedate, string invnumber, string invdate, string refno, int Modifiedby, string Addinfo, string serviceType, string transMode, string vechicleNo, string dateOfSupply, string cinNo)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_CDNR_Master", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@gstin", gstin));
                cmd.Parameters.Add(new SqlParameter("@fp", period));
                cmd.Parameters.Add(new SqlParameter("@ctin", ctin));
                cmd.Parameters.Add(new SqlParameter("@ntty", notetype));
                cmd.Parameters.Add(new SqlParameter("@nt_num", notenumber));
                cmd.Parameters.Add(new SqlParameter("@nt_dt", notedate));
                cmd.Parameters.Add(new SqlParameter("@inum", invnumber));
                cmd.Parameters.Add(new SqlParameter("@idt", invdate));
                cmd.Parameters.Add(new SqlParameter("@referenceno", refno));
                cmd.Parameters.Add(new SqlParameter("@ModifiedBy", Modifiedby));
                cmd.Parameters.Add(new SqlParameter("@Addinfo", Addinfo));
                cmd.Parameters.Add(new SqlParameter("@GoodsAndService", serviceType));
                cmd.Parameters.Add(new SqlParameter("@TransportMode", transMode));
                cmd.Parameters.Add(new SqlParameter("@vehicleNo", vechicleNo));
                cmd.Parameters.Add(new SqlParameter("@DateOfSupply", dateOfSupply));
                cmd.Parameters.Add(new SqlParameter("@CINno", cinNo));
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


        public int CDNRItemUpdate(int Cdnrid, decimal Rate, decimal TaxVal, decimal iamt, decimal camt, decimal samt, decimal csamt, string refno, string hsncode, string itemdesc, decimal qty, decimal unitprice, decimal discount, string uqc, int Createdby)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Update_OUTWARD_GSTR1_CDNR_Items", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Cdnrid", Cdnrid));
                cmd.Parameters.Add(new SqlParameter("@rt", Rate));
                cmd.Parameters.Add(new SqlParameter("@txval", TaxVal));
                cmd.Parameters.Add(new SqlParameter("@iamt", iamt));
                cmd.Parameters.Add(new SqlParameter("@camt", camt));
                cmd.Parameters.Add(new SqlParameter("@samt", samt));
                cmd.Parameters.Add(new SqlParameter("@csamt", csamt));
                cmd.Parameters.Add(new SqlParameter("@referenceno", refno));
                cmd.Parameters.Add(new SqlParameter("@hsncode", hsncode));
                cmd.Parameters.Add(new SqlParameter("@hsndesc", itemdesc));
                cmd.Parameters.Add(new SqlParameter("@qty", qty));
                cmd.Parameters.Add(new SqlParameter("@unitprice", unitprice));
                cmd.Parameters.Add(new SqlParameter("@discount", discount));
                cmd.Parameters.Add(new SqlParameter("@uqc", uqc));
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

        public int CDNRItemDelete(int Cdnrid)
        {
            int outputparam;
            try
            {
                con.Open();

                SqlCommand cmd = new SqlCommand("usp_Delete_OUTWARD_GSTR1_CDNR", con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@cdnrid", Cdnrid));
                cmd.Parameters.Add("@RetVal", SqlDbType.Int).Direction = ParameterDirection.Output;
                cmd.ExecuteNonQuery();

                con.Close();
                outputparam = Convert.ToInt32(cmd.Parameters["@RetVal"].Value);
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