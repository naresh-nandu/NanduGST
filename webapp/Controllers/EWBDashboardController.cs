#region "namespace"
using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_BAL;
using WeP_DAL;
#endregion

namespace SmartAdminMvc.Controllers
{
    public class EwbDashboardController : Controller
    {
        #region "Eway Bill DashBoard"
        // Testing purpose for Conflicts to Merge same file
        // GET: EwayDashboard
        public ActionResult Index()
        {

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                
                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                string GSTIN = "ALL";
                string FP = DateTime.Now.ToString("MMyyyy");
                ViewBag.Period = FP;
                ViewBag.GstinList= LoadDropDowns.GSTIN_No(UserId, CustId, Session["Role_Name"].ToString());
                DataSet DS = EwaybillDataAccess.getDaashboardSummary(FP, GSTIN, CustId, UserId);

                List<EwayAttributes.DashBoardSummary> Summary = new List<EwayAttributes.DashBoardSummary>();
                List<EwayAttributes.DashBoardCount> Count = new List<EwayAttributes.DashBoardCount>();

                #region "DashBoard Summary"

                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    Summary.Add(new EwayAttributes.DashBoardSummary
                    {
                        gstinNo = dr.IsNull("GSTINNo") ? "": dr["GSTINNo"].ToString(),
                        panNo = dr.IsNull("PANNo") ? "" : dr["PANNo"].ToString(),
                        companyName = dr.IsNull("CompanyName") ? "" : dr["CompanyName"].ToString(),
                        Generated = dr.IsNull("Generate") ? 0 : Convert.ToInt32(dr["Generate"]),
                        Received = dr.IsNull("Received") ? 0 : Convert.ToInt32(dr["Received"]),
                        Cancel = dr.IsNull("Cancelled") ? 0 : Convert.ToInt32(dr["Cancelled"]),
                        RejectedByMe = dr.IsNull("RejectedByMe") ? 0 : Convert.ToInt32(dr["RejectedByMe"]),
                        RejectedByCounterParty = dr.IsNull("RejectedByCounterParty") ? 0 : Convert.ToInt32(dr["RejectedByCounterParty"])
                        
                    });

                }

                #endregion

                #region "DashBoard Count"
                foreach (DataRow dr in DS.Tables[1].Rows)
                {
                    Count.Add(new EwayAttributes.DashBoardCount
                    {
                        Total_Generate = dr.IsNull("Total_Generate") ? 0:Convert.ToInt32(dr["Total_Generate"]),
                        Total_Received = dr.IsNull("Total_Received") ? 0:Convert.ToInt32(dr["Total_Received"]),
                        Total_Cancelled = dr.IsNull("Total_Cancelled") ? 0:Convert.ToInt32(dr["Total_Cancelled"]),
                        Total_RejectedByMe = dr.IsNull("Total_RejectedByMe") ? 0: Convert.ToInt32(dr["Total_RejectedByMe"]),
                        Total_RejectedByCounterParty = dr.IsNull("Total_RejectedByCounterParty") ? 0: Convert.ToInt32(dr["Total_RejectedByCounterParty"])

                    });

                }
                #endregion

                EwayViewModel Model = new EwayViewModel();
                Model.DashBoardCount = Count;
                Model.DashBoardSummary = Summary;

                return View(Model);
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            return View();
        }

        [HttpPost]
        public ActionResult Index(FormCollection Form)
        {

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                string GSTIN = "ALL";
                int UserId = Convert.ToInt32(Session["User_ID"]);
                int CustId = Convert.ToInt32(Session["Cust_ID"]);
                if (Form["gstin"] != "")
                {

                    GSTIN = Form["gstin"];
                }
                
                string FP = Form["period"];
                ViewBag.Period = FP;
                ViewBag.GstinList = LoadDropDowns.Exist_GSTIN_No(UserId, CustId, GSTIN, Session["Role_Name"].ToString());
                DataSet DS = EwaybillDataAccess.getDaashboardSummary(FP, GSTIN, CustId, UserId);

                List<EwayAttributes.DashBoardSummary> Summary = new List<EwayAttributes.DashBoardSummary>();
                List<EwayAttributes.DashBoardCount> Count = new List<EwayAttributes.DashBoardCount>();

                #region "DashBoard Summary"

                foreach (DataRow dr in DS.Tables[0].Rows)
                {
                    Summary.Add(new EwayAttributes.DashBoardSummary
                    {
                        gstinNo = dr.IsNull("GSTINNo") ? "" : dr["GSTINNo"].ToString(),
                        panNo = dr.IsNull("PANNo") ? "" : dr["PANNo"].ToString(),
                        companyName = dr.IsNull("CompanyName") ? "" : dr["CompanyName"].ToString(),
                        Generated = dr.IsNull("Generate") ? 0 : Convert.ToInt32(dr["Generate"]),
                        Received = dr.IsNull("Received") ? 0 : Convert.ToInt32(dr["Received"]),
                        Cancel = dr.IsNull("Cancelled") ? 0 : Convert.ToInt32(dr["Cancelled"]),
                        RejectedByMe = dr.IsNull("RejectedByMe") ? 0 : Convert.ToInt32(dr["RejectedByMe"]),
                        RejectedByCounterParty = dr.IsNull("RejectedByCounterParty") ? 0 : Convert.ToInt32(dr["RejectedByCounterParty"])

                    });

                }

                #endregion

                #region "DashBoard Count"
                foreach (DataRow dr in DS.Tables[1].Rows)
                {
                    Count.Add(new EwayAttributes.DashBoardCount
                    {
                        Total_Generate = dr.IsNull("Total_Generate") ? 0 : Convert.ToInt32(dr["Total_Generate"]),
                        Total_Received = dr.IsNull("Total_Received") ? 0 : Convert.ToInt32(dr["Total_Received"]),
                        Total_Cancelled = dr.IsNull("Total_Cancelled") ? 0 : Convert.ToInt32(dr["Total_Cancelled"]),
                        Total_RejectedByMe = dr.IsNull("Total_RejectedByMe") ? 0 : Convert.ToInt32(dr["Total_RejectedByMe"]),
                        Total_RejectedByCounterParty = dr.IsNull("Total_RejectedByCounterParty") ? 0 : Convert.ToInt32(dr["Total_RejectedByCounterParty"])

                    });

                }
                #endregion

                EwayViewModel Model = new EwayViewModel();
                Model.DashBoardCount = Count;
                Model.DashBoardSummary = Summary;

                return View(Model);
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            return View();
        }
        #endregion
    }
}