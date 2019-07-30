using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace WeP_DAL.GSTR9Attribute
{
    public class Gstr9
    {
        static SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        public Gstr9()
        {
            //
        }

        #region "Report"

        public static DataSet Retrieve_GSTR9_Report_Part2(string Reporttype, string userGSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Retrieve_GSTR9_MIS]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@GSTIN", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPeriod));
                cmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }

        #region "gstr9 "

        public static DataSet Retrieve_GSTR9_Template(string userGSTIN, string fromPeriod, string toPeriod)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_Template", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", userGSTIN));
                cmd.Parameters.Add(new SqlParameter("@fromPeriod", fromPeriod));              
                cmd.Parameters.Add(new SqlParameter("@toPeriod", toPeriod));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }
        #endregion

        #region "Table5"
        public static DataSet Table5(string StrGSTIN,string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table4"
        public static DataSet Table4(string StrGSTIN, string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table6"
        public static DataSet Table6(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;



        }
 
        #endregion

        #region "Table7"
        public static DataSet Table7(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table8"
        public static DataSet Table8(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table9"
        public static DataSet Table9(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table10"
        public static DataSet Table10(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;

        }
        #endregion

        #region "Table14"
        public static DataSet Table14(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;

        }
        #endregion

        #region "Table15"
        public static DataSet Table15(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table16"
        public static DataSet Table16(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }
        #endregion

        #region "Table17"
        public static DataSet Table17(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "Table18"
        public static DataSet Table18(string StrGSTIN, string ToDate)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.Add(new SqlParameter("@Gstin", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion

        #region "AllData"
        public static DataSet Gstr9Data(string StrGSTIN, string ToDate)
        {

             DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Retrieve_GSTR9_Summary_SA]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp ", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
        #endregion



        public static DataSet Retrieve_GSTR9_Log_Details(string DataMode,string GSTIN, string toPeriod,int CUSTID)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_EXT_LOG", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp ", toPeriod));
                cmd.Parameters.Add(new SqlParameter("@CustId ", CUSTID));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }

        public static DataSet Retrieve_GSTR9_Log_Details1(string DataMode, string GSTIN, string toPeriod, int CUSTID)
        {
            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Retrieve_GSTR9_SA_LOG", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", GSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp ", toPeriod));
                cmd.Parameters.Add(new SqlParameter("@CustId ", CUSTID));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);
                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;
        }


        public void GSTR9Delete(string GSTINNo, string fp, string type)
        {
            try
            {
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("usp_Delete_GSTR9_SA", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin", GSTINNo));
                cmd.Parameters.Add(new SqlParameter("@Fp", fp));
                cmd.Parameters.Add(new SqlParameter("@Type", type));
                cmd.ExecuteNonQuery();
                Con.Close();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }

        }

        public static DataSet Gstr9AutoCal(string StrGSTIN, string ToDate)
        {

            DataSet ds = new DataSet();
            try
            {
                #region commented
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand("[dbo].[usp_Retrieve_GSTR9_D_AutoCalculate]", Con);
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandTimeout = 0;
                cmd.Parameters.Add(new SqlParameter("@Gstin ", StrGSTIN));
                cmd.Parameters.Add(new SqlParameter("@Fp ", ToDate));
                SqlDataAdapter da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                Con.Close();
                #endregion
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
                throw;
            }
            return ds;


        }
    }
}
#endregion
