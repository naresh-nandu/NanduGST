using System;
using System.Configuration;
using System.Collections;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Web.SessionState;
using System.IO;
using System.Web.Mail;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Net;
using System.Runtime.InteropServices;
using System.Globalization;
using System.Text;

namespace SmartAdminMvc.Models.Common
{
    public class Functions
    {
        [DllImport("iphlpapi.dll", ExactSpelling = true)]
        private static extern int SendARP(int DestIP, int SrcIP, [Out] byte[] pMacAddr, ref int PhyAddrLen);

        protected Functions()
        {
            //            
        }

        public static int FindStringPosition(string x, char FindChar, int NoofOccurance, int Direction) //Direction :0-leftToRight 1-RightToLeft
        {
            int Occurance = 1, CharPosition = 0;
            if (Direction == 0)
            {
                for (int i = 0; i <= x.Length - 1; i++)
                {
                    if ((x[i] == FindChar) && (Occurance <= NoofOccurance))
                    {
                        Occurance++;
                        CharPosition = (i + 1);
                    }
                }
            }
            else if (Direction == 1)
            {

                for (int i = x.Length - 1; i >= 0; i--)
                {
                    if ((x[i] == FindChar) && (Occurance <= NoofOccurance))
                    {
                        Occurance++;
                        CharPosition = (i + 1);
                    }
                }

            }
            return CharPosition;
        }


        public static string mid(string x, int startlength, int length)
        {
            StringBuilder s = new StringBuilder();
            for (int i = startlength - 1; i <= startlength - 1 + length - 1; i++)
                s.Append(x[i]);
            return s.ToString();
        }


        public static string left(string x, int length)
        {
            StringBuilder s = new StringBuilder();
            for (int i = 0; i <= length - 1; i++)
                s.Append(x[i]);
            return s.ToString();
        }


        public static string right(string x, int length)
        {
            StringBuilder s = new StringBuilder();
            for (int i = x.Length - length; i <= x.Length - 1; i++)
                s.Append(x[i]);
            return s.ToString();
        }


        public static string SqlDate(string IndianDate)
        {
            string day = "", Month = "", Year = "";
            day = Functions.left(IndianDate, Functions.FindStringPosition(IndianDate, '/', 1, 0) - 1);
            Month = Functions.mid(IndianDate, Functions.FindStringPosition(IndianDate, '/', 1, 0) + 1, Functions.FindStringPosition(IndianDate, '/', 2, 0) - (Functions.FindStringPosition(IndianDate, '/', 1, 0) + 1));
            Year = Functions.right(IndianDate, IndianDate.Length - Functions.FindStringPosition(IndianDate, '/', 2, 0));
            return Month + "/" + day + "/" + Year;
        }




        //public static string InsertIntoTable(string table, SqlCommand com, SqlConnection con)
        //{
        //    string IdentityColumnValue = "";
        //    if (con.State == ConnectionState.Closed)
        //    {
        //        con.Open();
        //    }
        //    com.Connection = con;
        //    StringBuilder ParameterNames = new StringBuilder();
        //    StringBuilder ParameterValues = new StringBuilder();

        //    foreach (SqlParameter sp in com.Parameters)
        //    {
        //        ParameterNames.Append(sp.ParameterName.Replace("@", "") + ",");
        //        ParameterValues.Append("'" + sp.Value + "',");
        //    }
        //    string query = "Insert into " + table + " (" + ParameterNames.Remove(ParameterNames.Length - 1, 1) + ") values (" + ParameterValues.Remove(ParameterValues.Length - 1, 1) + "); Select SCOPE_IDENTITY()";
        //    using (SqlCommand sqlcmd = new SqlCommand())
        //    {
        //        sqlcmd.Connection = con;
        //        sqlcmd.CommandText = "usp_Function_WePHelper";
        //        sqlcmd.CommandType = CommandType.StoredProcedure;
        //        sqlcmd.Parameters.Add("@SQLQuery", SqlDbType.NVarChar).Value = query;
        //        IdentityColumnValue = sqlcmd.ExecuteScalar().ToString();
        //    }
        //    return IdentityColumnValue;
        //}

        public static string InsertIntoTable(string table, SqlCommand com, SqlConnection con)
        {
            com.Connection = con;
            string ParameterNames = "", ParameterValues = "";
            foreach (SqlParameter sp in com.Parameters)
            {
                ParameterNames += sp.ParameterName.Replace("@", "") + ",";
                ParameterValues += sp.ParameterName + ",";
                // ParameterValues += sp.Value + ",";
            }
            com.CommandText = "Insert into " + table + " (" + ParameterNames.Remove(ParameterNames.Length - 1, 1) + ") values (" + ParameterValues.Remove(ParameterValues.Length - 1, 1) + "); Select SCOPE_IDENTITY()";
            string IdentityColumnValue = com.ExecuteScalar().ToString();
            return IdentityColumnValue;

            return com.CommandText;
        }

        public static void UpdateTable(string table, string fieldname, string fieldvalue, SqlCommand com, SqlConnection con)
        {
            com.Connection = con;
            string ParameterNames = "";
            foreach (SqlParameter sp in com.Parameters)
            {
                //   ParameterNames += sp.ParameterName.Replace("@", "") + " =" + sp.ParameterName.Replace(sp.ParameterName, "?") + ",";
                ParameterNames += sp.ParameterName.Replace("@", "") + " =" + sp.ParameterName + ",";
            }
            com.CommandText = "Update " + table + " set " + ParameterNames.Remove(ParameterNames.Length - 1, 1) + " where " + fieldname + "= '" + fieldvalue + "'";
            com.ExecuteNonQuery();
        }

        //public static void UpdateTable(string table, string fieldname, string fieldvalue, SqlCommand com, SqlConnection con)
        //{
        //    if (con.State == ConnectionState.Closed)
        //    {
        //        con.Open();
        //    }
        //    com.Connection = con;
        //    StringBuilder ParameterNames = new StringBuilder();
        //    foreach (SqlParameter sp in com.Parameters)
        //    {
        //        ParameterNames.Append(sp.ParameterName.Replace("@", "") + " ='" + sp.Value + "',");
        //    }
        //    string query = "Update " + table + " set " + ParameterNames.Remove(ParameterNames.Length - 1, 1) + " where " + fieldname + "= '" + fieldvalue + "'";
        //    using (SqlCommand sqlcmd = new SqlCommand())
        //    {
        //        sqlcmd.Connection = con;
        //        sqlcmd.CommandText = "usp_Function_WePHelper";
        //        sqlcmd.CommandType = CommandType.StoredProcedure;
        //        sqlcmd.Parameters.Add("@SQLQuery", SqlDbType.NVarChar).Value = query;
        //        sqlcmd.ExecuteNonQuery();
        //    }
        //}




        public static void UpdateOrInsertTable(string Query)
        {
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                sqlcon.Open();
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "usp_Function_WePHelper";
                    sqlcmd.CommandType = CommandType.StoredProcedure;
                    sqlcmd.CommandTimeout = 0;
                    sqlcmd.Parameters.Add("@SQLQuery", SqlDbType.NVarChar).Value = Query;
                    sqlcmd.ExecuteNonQuery();
                }
                sqlcon.Close();
            }
        }


        public static bool IsNumeric(object Expression)
        {
            // Variable to collect the Return value of the TryParse method.
            bool isNum;

            // Define variable to collect out parameter of the TryParse method. If the conversion fails, the out parameter is zero.
            double retNum;

            // The TryParse method converts a string in a specified style and culture-specific format to its double-precision floating point number equivalent.
            // The TryParse method does not generate an exception if the conversion fails. If the conversion passes, True is returned. If it does not, False is returned.
            isNum = Double.TryParse(Convert.ToString(Expression), System.Globalization.NumberStyles.Any, System.Globalization.NumberFormatInfo.InvariantInfo, out retNum);
            return isNum;
        }


        public static string NumberToText(string amount)
        {
            decimal numberrs = Convert.ToInt64(Math.Floor(Convert.ToDouble(amount)));
            CultureInfo ci = new CultureInfo("en-IN");
            string aaa = String.Format("{0:#,##0.##}", numberrs);
            aaa = aaa + " " + ci.NumberFormat.CurrencySymbol.ToString();
          
            string input = amount;
            string a = "";
            string b = "";
            string strWords1 = "";
            string strWords3 = "";
            // take decimal part of input. convert it to word. add it at the end of method.
            string decimals = "";

            if (input.Contains(".00"))
            {
                decimals = "";
                input = input.Remove(input.IndexOf("."));
                strWords1 = NumbersToWords(Convert.ToInt64(input));
            }

            else if (!input.Contains(".00"))
            {
                decimals = input.Substring(input.IndexOf(".") + 1);
                // remove decimal part from input
                input = input.Remove(input.IndexOf("."));
            }
            string strWords = NumbersToWords(Convert.ToInt64(input));

          
            
            if (amount.Contains(".00"))
            {
                a = strWords1 + "Rupees Only";
            }

            else if (numberrs == 0)
            {
                strWords3 = "";
                a = "";
              
            }
            else if(numberrs != 0)
            {
                a = strWords + " Rupees";
            }

            if (decimals == "")
            {
                b = "";
            }
            if (decimals.Length > 0)
            {
                // if there is any decimal part convert it to words and add it to strWords.
                string strwords2 = NumbersToWords(Convert.ToInt64(decimals));
                b = " and " + strwords2 + " Paisa Only ";
            }

            if( numberrs == 0)
            {
                string strwords2 = NumbersToWords(Convert.ToInt64(decimals));
                b = strwords2 + "Paisa Only";
            }
            string c = a + b;

            return c;
        }

        public static string NumbersToWords(long num)
        {

            if (num == 0)
                return "Zero";

            if (num < 0)
                return "Not supported";

            var words = "";
            string[] strones = { "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            string[] strtens = { "Twenty", "Thirty", "Fourty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };


            long trillion = 0, billion = 0, crore = 0, lakhs = 0, thousands = 0, hundreds = 0, tens = 0, single = 0;


            trillion = num / 100000000000;
            num = num - trillion * 100000000000;

            crore = num / 10000000;
            num = num - crore * 10000000;

            lakhs = num / 100000;
            num = num - lakhs * 100000;

            thousands = num / 1000;
            num = num - thousands * 1000;

            hundreds = num / 100;
            num = num - hundreds * 100;

            if (num > 19)
            {
                tens = num / 10; num = num - tens * 10;
            }
            single = num;
            if (trillion > 0)
            {
                if (billion > 19)
                    words += NumbersToWords(trillion) + " Crore";
                else
                    words += strones[trillion - 1] + " Crore";
            }
            if (crore > 0)
            {
                if (crore > 19)
                    words += NumbersToWords(crore) + "Crore ";
                else
                    words += strones[crore - 1] + " Crore ";
            }

            if (lakhs > 0)
            {
                if (lakhs > 19)
                    words += NumbersToWords(lakhs) + "Lakh ";
                else
                    words += strones[lakhs - 1] + " Lakh ";
            }

            if (thousands > 0)
            {
                if (thousands > 19)
                    words += NumbersToWords(thousands) + "Thousand ";
                else
                    words += strones[thousands - 1] + " Thousand ";
            }

            if (hundreds > 0)
                words += strones[hundreds - 1] + " Hundred ";

            if (tens > 0)
                words += strtens[tens - 2] + " ";

            if (single > 0)
                words += strones[single - 1] + " ";

            return words;
        }
                       

        public static void LogFile(string fil, string page, int NextLine)
        {
            string filename = fil + ".txt";
            FileInfo f = new FileInfo(ConfigurationManager.AppSettings["ServerName"] + filename);
            StreamWriter sw = f.AppendText();
            if (NextLine == 1)
            {
                sw.WriteLine("");
                sw.WriteLine("");
            }
            else
                sw.Write("\t");
            sw.Write(page + "---" + DateTime.Now);

            sw.Close();
        }

        public static void InternetLogFile(string IP, string fil, string page, int NextLine)
        {
            if (IP == ConfigurationManager.AppSettings["ServerInternetIP"].ToString())
            {
                string filename = fil + ".txt";
                FileInfo f = new FileInfo(ConfigurationManager.AppSettings["InternetLogPath"] + filename);

                StreamWriter sw = f.AppendText();
                if (NextLine == 1)
                {
                    sw.WriteLine("");
                    sw.WriteLine("");
                }
                else
                    sw.Write("\t");
                sw.Write(page + "---" + DateTime.Now);

                sw.Close();
            }
        }
    }
}