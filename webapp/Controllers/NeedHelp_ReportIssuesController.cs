using SmartAdminMvc.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models.Common;
using System.Configuration;
using WeP_BAL;
using WeP_BAL.NeedHelp;

namespace SmartAdminMvc.Controllers
{
    public class NeedHelp_ReportIssuesController : Controller
    {
        private readonly WePGSPDBEntities db = new WePGSPDBEntities();

        // GET: NeedHelp_ReportIssues
        public ActionResult Index()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            return View();
        }

        [System.Web.Mvc.HttpPost]
        public ActionResult Index(FormCollection frm, string btnNeedHelp, string btnReportIssue)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");

            int iUserId = Convert.ToInt32(Session["User_ID"].ToString());

            string strController = frm["controllername"];
            string strAction = frm["actionname"];

            if (!string.IsNullOrEmpty(btnNeedHelp))
            {
                string strEmail = (from user in db.UserLists
                                   where user.UserId == iUserId
                                   select user.Email).SingleOrDefault();

                string strMessage = frm["message"];

                Notification.SendEmailtoASPPanel(strEmail, Session["UserName"].ToString(), "Need Help", strMessage);
            }
            else if (!string.IsNullOrEmpty(btnReportIssue))
            {
                string strEmail = (from user in db.UserLists
                                   where user.UserId == iUserId
                                   select user.Email).SingleOrDefault();

                string strMessage = frm["message"];

                Notification.SendEmailtoASPPanel(strEmail, Session["UserName"].ToString(), "Report Issues", strMessage);
            }

            return RedirectToAction(strAction, strController);
        }

        #region "Need Help for Customer Issues"
        [HttpGet]
        public ActionResult Home()
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                //
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
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
                return RedirectToAction("Login", "Account");

            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);

            try
            {
                #region "variableDeclaration and Intialization"
                string emailId = "", mobileNo = "", taskType = "", problemType = "", taskSummary = "";
                string refNum = "", status = "", status_desc = "";
                #endregion

                #region "Fetching Form Collection Values"
                emailId = Form["emailId"];
                mobileNo = Form["mobileNo"];
                taskType = Form["taskType"];
                problemType = Form["problemType"];
                taskSummary = Form["taskSummary"];
                #endregion

                #region "API Calling"
                string serailNo = NeedHelpBal.SerailNumberChecking(Session["CustRefNo"].ToString(), custid);
                if (!string.IsNullOrEmpty(serailNo))
                {
                    new NeedHelpBal(custid, userid, taskType, taskSummary, problemType, mobileNo, emailId).needHelp_API(serailNo,out refNum, out status, out status_desc);
                    if (status == "S")
                    {
                        TempData["SuccessMessage"] = "Your Request for help done Successfully.Shorthly You will Get Email And SMS.";
                    }
                    else
                    {
                        TempData["SuccessMessage"] = "Something went wrong,please contact administrator.";
                    }
                    
                }
                else
                {
                    TempData["ErrorMessage"] = "Please Buy Our Product to avail this service";
                }
                #endregion
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        #endregion
    }
}