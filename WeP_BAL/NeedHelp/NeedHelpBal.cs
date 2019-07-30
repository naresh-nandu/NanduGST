using Newtonsoft.Json;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Web.Script.Serialization;

namespace WeP_BAL.NeedHelp
{
    public class NeedHelpBal
    {
        public readonly int _custid;
        public readonly int _userid;
        public readonly string _taskType;
        public readonly string _taskSummary;
        public readonly string _problemCode;
        public readonly string _mobileNo;
        public readonly string _emailId;
        public NeedHelpBal(int CustId, int UserId, string TaskType, string TaskSummary, string ProblemType, string MobileNo, string EmailId)
        {
            this._custid = CustId;
            this._userid = UserId;
            this._taskType = TaskType;
            this._taskSummary = TaskSummary;
            this._problemCode = ProblemType;
            this._mobileNo = MobileNo;
            this._emailId = EmailId;

        }

        public static string SerailNumberChecking(string refNo, int custId)
        {
            string serialNo = "";
            using (SqlConnection sqlcon = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = sqlcon;
                    sqlcmd.CommandText = "Select * from TBL_Customer where CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@CustId", custId);
                    using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt = new DataTable();
                        adt.Fill(dt);
                        if (dt.Rows.Count > 0)
                        {
                            serialNo = dt.Rows[0]["supportRefId"].ToString();
                        }

                    }
                    return serialNo;
                }
            }
        }
        public string needHelp_API(string serialNo,out string refNumber, out string status, out string statusCode)
        {
            string needHelp_Response = "";
            NeedHelpParameters objWalletParams = new NeedHelpParameters();
            objWalletParams.serial_num = serialNo;
            objWalletParams.task_type = this._taskType;
            objWalletParams.problem_code = this._problemCode;
            objWalletParams.task_summary = this._taskSummary;
            objWalletParams.contact_number = this._mobileNo;
            objWalletParams.contact_email = this._emailId;
            string tempjsondata = new JavaScriptSerializer().Serialize(objWalletParams);
            var httpWebRequest = (HttpWebRequest)WebRequest.Create(ConfigurationManager.AppSettings["Need_Help_API"]);
            httpWebRequest.Method = "POST";
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

            using (var streamWriter = new StreamWriter(httpWebRequest.GetRequestStream()))
            {
                streamWriter.Write(tempjsondata);
                streamWriter.Flush();
                streamWriter.Close();

                var httpResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var result = streamReader.ReadToEnd();
                    needHelp_Response = result.ToString();
                }

                Response Response = JsonConvert.DeserializeObject<Response>(needHelp_Response);
                refNumber = Response.ref_number;
                status = Response.status_code;
                statusCode = Response.status_desc;
                return needHelp_Response;
            }
        }

        public class NeedHelpParameters
        {
            public string serial_num { get; set; }
            public string task_type { get; set; }
            public string problem_code { get; set; }
            public string task_summary { get; set; }
            public string contact_number { get; set; }
            public string contact_email { get; set; }
        }
        public class Response
        {
            public string ref_number { get; set; }
            public string status_desc { get; set; }
            public string status_code { get; set; }
        }
    }
}
