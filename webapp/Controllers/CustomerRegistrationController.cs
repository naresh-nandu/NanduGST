using SmartAdminMvc.Models.CustomerAPIFunction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using SmartAdmin.Models.CustomerAPIFunctions;
using System.Data.SqlClient;
using System.Configuration;
using System.Data;
using System.Web;

namespace SmartAdminMvc.Controllers
{
    public class CustomerRegistrationController : ApiController
    {

        #region "API For WHITE LABELING CUSTOMERS"

        [HttpPost]
        [Route("api/Customer/Registration")]
        public HttpResponseMessage SendRequest(Attributes objAttr)
        {
            string _referenceno;
            string _partnerid;
            Attributes OutputAttrObj = new Attributes();
            int retvalue;
            string retmessage;
            try
            {
                var req = Request.Headers;

                if (req.Contains("PartnerId"))
                {
                    _referenceno = string.IsNullOrEmpty(req.GetValues("PartnerId").First())
                                ? null :
                                req.GetValues("PartnerId").First();
                }
                else
                {
                    _referenceno = "";
                }

                if (!string.IsNullOrEmpty(_referenceno))
                {
                    using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                    {
                        con.Open();
                        using (SqlCommand sqlcmd = new SqlCommand())
                        {
                            sqlcmd.Connection = con;
                            sqlcmd.CommandText = "Select TOP 1 * from TBL_WePGST_PARTNER where referenceno = @RefNo";
                            sqlcmd.Parameters.AddWithValue("@RefNo", _referenceno);
                            using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                            {
                                DataTable dt = new DataTable();
                                adt.Fill(dt);
                                con.Close();
                                if (dt.Rows.Count > 0)
                                {
                                    _partnerid = dt.Rows[0]["PartnerId"].ToString();

                                    if (objAttr == null)
                                    {
                                        OutputAttrObj.Status = 0;
                                        OutputAttrObj.Message = "Please input proper value";
                                    }
                                    else
                                    {
                                        CustomerRegistration.SendRequest(objAttr, _partnerid, out retvalue, out retmessage);
                                        if (retvalue == 1)
                                        {
                                            OutputAttrObj.Status = retvalue;
                                            OutputAttrObj.Message = retmessage + ". Your Email is " + objAttr.Email + " and Password is " + objAttr.MobileNo;
                                        }
                                        else
                                        {
                                            OutputAttrObj.Status = retvalue;
                                            OutputAttrObj.Message = retmessage;
                                        }
                                    }
                                }
                                else
                                {
                                    OutputAttrObj.Status = 0;
                                    OutputAttrObj.Message = "Please provide valid Partner ID";
                                }
                            }
                        }
                        con.Close();
                    }
                }
                else
                {
                    OutputAttrObj.Status = 0;
                    OutputAttrObj.Message = "Please provide Partner ID";
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }


        [HttpGet]
        [Route("api/Customer/Login")]
        public HttpResponseMessage Login([FromUri]Attributes objAttr)
        {
            Attributes OutputAttrObj = new Attributes();
            int retvalue;
            string retmessage;
            try
            {
                if (objAttr == null)
                {
                    OutputAttrObj.Status = 0;
                    OutputAttrObj.Message = "Please input proper value";
                }
                else
                {
                    CustomerRegistration.GetRequest(objAttr, out retvalue, out retmessage);
                    OutputAttrObj.Status = retvalue;
                    OutputAttrObj.Message = retmessage;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }


        [HttpGet]
        [Route("api/GSTR1/Data")]
        public HttpResponseMessage GSTR1Data()
        {
            string _referenceno;
            Attributes OutputAttrObj = new Attributes();
            try
            {
                string _date;
                var req = Request.Headers;
                string strPartnerId = "";
                object objData;
                if (req.Contains("PartnerId"))
                {
                    _referenceno = string.IsNullOrEmpty(req.GetValues("PartnerId").First())
                                ? null :
                                req.GetValues("PartnerId").First();
                }
                else
                {
                    _referenceno = "";
                }

                _date = string.IsNullOrEmpty(HttpContext.Current.Request.QueryString["date"].ToString())
                               ? null
                               : HttpContext.Current.Request.QueryString["date"].ToString();

                using (SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
                {
                    using (SqlCommand sqlcmd = new SqlCommand())
                    {
                        sqlcmd.Connection = con;
                        sqlcmd.CommandText = "Select * from TBL_WePGST_PARTNER where ReferenceNo = @RefNo";
                        sqlcmd.Parameters.AddWithValue("@RefNo", _referenceno);
                        using (SqlDataAdapter adt = new SqlDataAdapter(sqlcmd))
                        {
                            DataTable dt = new DataTable();
                            adt.Fill(dt);
                            if (dt.Rows.Count > 0)
                            {
                                strPartnerId = dt.Rows[0]["PartnerId"].ToString();
                            }
                        }
                    }
                }

                CustomerRegistration.GetGSTR1DataRequest(strPartnerId, _date, out objData);

                return Request.CreateResponse(HttpStatusCode.OK, new { OutputResponse = objData }, Configuration.Formatters.JsonFormatter);

            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;

                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }

        #endregion


        #region "API For WeP DIGITAL PORTAL"

        [HttpPost]
        [Route("api/CustomerRegistration/SendRequest")]
        public HttpResponseMessage Portal_SendRequest(Attributes objAttr)
        {
            Attributes OutputAttrObj = new Attributes();
            int retvalue;
            string retmessage;
            try
            {
                if (objAttr == null)
                {
                    OutputAttrObj.Status = 0;
                    OutputAttrObj.Message = "Please input proper value";
                }
                else
                {
                    CustomerRegistration.Portal_SendRequest(objAttr, out retvalue, out retmessage);
                    if (retvalue == 1)
                    {
                        OutputAttrObj.Status = retvalue;
                        OutputAttrObj.Message = retmessage + ". Your Email is " + objAttr.Email + " and Password is " + objAttr.Password;
                    }
                    else
                    {
                        OutputAttrObj.Status = retvalue;
                        OutputAttrObj.Message = retmessage;
                    }
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }


        [HttpPost]
        [Route("api/Wallet/Balance")]
        public HttpResponseMessage WalletRequest(Attributes objAttr)
        {
            Attributes OutputAttrObj = new Attributes();
            int retvalue;
            string retmessage;
            decimal retbalance;
            try
            {
                if (objAttr == null)
                {
                    OutputAttrObj.Status = 0;
                    OutputAttrObj.Message = "Please input proper value";
                }
                else
                {
                    CustomerRegistration.Wallet_SendRequest(objAttr, out retvalue, out retmessage, out retbalance);
                    OutputAttrObj.Status = retvalue;
                    OutputAttrObj.Balance = retbalance;
                    OutputAttrObj.Message = retmessage;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Balance = OutputAttrObj.Balance, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }


        [HttpPost]
        [Route("api/Wallet/Pack")]
        public HttpResponseMessage WalletPackRequest(Attributes objAttr)
        {
            Attributes OutputAttrObj = new Attributes();
            int retvalue;
            string retmessage;
            try
            {
                if (objAttr == null)
                {
                    OutputAttrObj.Status = 0;
                    OutputAttrObj.Message = "Please input proper value";
                }
                else
                {
                    CustomerRegistration.WalletPack_SendRequest(objAttr, out retvalue, out retmessage);
                    OutputAttrObj.Status = retvalue;
                    OutputAttrObj.Message = retmessage;
                }
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
            catch (Exception ex)
            {
                OutputAttrObj.Status = 0;
                OutputAttrObj.Message = ex.Message;
                return Request.CreateResponse(HttpStatusCode.OK, new { Status = OutputAttrObj.Status, Message = OutputAttrObj.Message }, Configuration.Formatters.JsonFormatter);
            }
        }

        #endregion

    }
}
