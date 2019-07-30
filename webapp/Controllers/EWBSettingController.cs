using SmartAdminMvc.Models.Common;
using System;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WeP_DAL;

namespace SmartAdminMvc.Controllers
{
    public class EwbSettingController : Controller
    {
        private SqlConnection Con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        // GET: EWBSetting
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {

                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }

            return View();
        }

        [HttpPost]
        public ActionResult Home(FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            try
            {

                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                Nullable<bool> EwbEmail = null, EwbLocation = null;

                string strEwbEmail = Form["strEwbEmail"];
                string strLocation = Form["strLocation"];
                switch (strEwbEmail)
                {
                    case "Enable":
                        EwbEmail = true;
                        break;
                    case "Disable":
                        EwbEmail = false;
                        break;
                }
                switch (strLocation)
                {
                    case "Enable":
                        EwbLocation = true;
                        break;
                    case "Disable":
                        EwbLocation = false;
                        break;
                }
                #region "Updating Settings"
                if (Con.State == ConnectionState.Closed)
                {
                    Con.Open();
                }
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                cmd.Parameters.Add("@EwbEmailReqd", SqlDbType.Bit).Value = EwbEmail;
                cmd.Parameters.Add("@LocationReqd", SqlDbType.Bit).Value = EwbLocation;
                cmd.Parameters.Add("@ModifiedBy", SqlDbType.Int).Value = userid;
                cmd.Parameters.Add("@ModifiedDate", SqlDbType.DateTime).Value = DateTime.Now;
                Functions.UpdateTable("TBL_Cust_Settings", "CustId", Convert.ToString(custid), cmd, Con);
                Con.Close();
                Helper.InsertAuditLog(Session["User_ID"].ToString(), Session["UserName"].ToString(), "Eway Bill Settings Updated Successfully", "");
                TempData["SuccessMessage"] = "Settings Updated Successfully";

                #endregion

                #region "Updated Setting Assigned to Session"
                string Invoice_Setting = "", CustomerCTINSetting = "", SupplierCTINSetting = "", HSNCTINSetting = "", CreditNote_Setting = "", GSTR3B_Setting = "";
                string TaxValSettings = "", EwaytoGSTR1s = "", GSTR1toEways = "", EwayPrints = "", EwbEamil = "", Location = "", MakerCheckerApprover = "", AutoGenInvNo = "";
                string ReconSetting = "", InvFormat = "", GenerateGSTR1HSN = "", InvoicePrintLogo = "",  othFields = "", AutoGenInwardInvNo = "", TDS = "";

                OutwardFunctions.Settings(custid, out Invoice_Setting, out CustomerCTINSetting, out SupplierCTINSetting, out HSNCTINSetting, out CreditNote_Setting,
                    out GSTR3B_Setting, out TaxValSettings, out EwaytoGSTR1s, out GSTR1toEways, out EwayPrints, out EwbEamil, out Location, out MakerCheckerApprover, out AutoGenInvNo,
                    out ReconSetting, out InvFormat, out GenerateGSTR1HSN, out InvoicePrintLogo, out othFields, out AutoGenInwardInvNo, out TDS);
                Session["ewbEmailSetting"] = EwbEamil;
                Session["LocationSetting"] = Location;
                #endregion
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }

            return View();
        }
    }
}