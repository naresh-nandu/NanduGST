using SmartAdminMvc.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using WeP_BAL.BranchMgmt;
using WeP_BAL.EwayBill;
using WeP_DAL.BranchMgmt;

namespace SmartAdminMvc.Controllers
{
    public class BranchMgmtController : Controller
    {
        [HttpGet]
        public ActionResult Home()
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            try
            {
                ViewBag.Panlist = Models.Common.LoadDropDowns.PanList(custid);
                ViewBag.GstinList = Models.Common.LoadDropDowns.GSTINList();
                ViewBag.UserList = BranchBusinessLayer.getUserlist(custid);

                #region "Retrieve Branch Details"
                DataSet ds = BranchBusinessLayer.Branch_Retrieve(custid);
                List<BranchDataAccess.BrachAttributes> BranchList = new List<BranchDataAccess.BrachAttributes>();
                List<BranchDataAccess.UserAccessLocation> UALList = new List<BranchDataAccess.UserAccessLocation>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BranchList.Add(new BranchDataAccess.BrachAttributes
                    {
                        branchId = dr.IsNull("branchId") ? 0 : Convert.ToInt32(dr["branchId"]),
                        company = dr.IsNull("company") ? "" : dr["company"].ToString(),
                        gstinNo = dr.IsNull("gstinNo") ? "" : dr["gstinNo"].ToString(),
                        branchName = dr.IsNull("branchName") ? "" : dr["branchName"].ToString(),
                        email = dr.IsNull("email") ? "" : dr["email"].ToString()
                    });

                }

                BranchViewModel Model = new BranchViewModel();
                Model.BranchList = BranchList;
                Model.UALList = UALList;
                #endregion

                return View(Model);
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }


        [HttpPost]
        public ActionResult Home(FormCollection Form, string Command)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            try
            {
                string panNo, strGSTIN, branchName, email, Mode, userAccessId;
                int branchId = 0, GstinId = 0;

                panNo = Form["ddlPanNo"];
                strGSTIN = Form["ddlGSTIN"];
                branchName = Form["branchName"].ToUpper();
                email = Form["email"];
                Mode = "I";
                userAccessId = Form["UserId"];
                ViewBag.email = email;

                #region"Email Validation"
                if (!string.IsNullOrEmpty(email))
                {
                    //Count total no of Email in the list
                    int totalEmail = email.Split(';').Count();
                    Regex regex = new Regex(@"[A-Za-z0-9._%-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}");
                    int regexMatch = regex.Matches(email).Count;
                    //Count no of email with which regex matches
                    if (totalEmail == regexMatch)
                    {
                        //Emails are correctly formatted
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter the valid Email Address";//Raise error on one or more email is incorrect in the list
                        return RedirectToAction("Home");
                    }
                }
                #endregion

                ViewBag.UserList = BranchBusinessLayer.getUserlist(custid);
                if (!string.IsNullOrEmpty(strGSTIN))
                {
                    GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTIN);
                }

                #region "Dropdown Loading"
                ViewBag.Panno = panNo;
                ViewBag.Panlist = Models.Common.LoadDropDowns.Exist_PanList(custid, panNo);
                if (panNo != null)
                {
                    ViewBag.GstinList = Models.Common.LoadDropDowns.GSTINList(panNo);
                    if (strGSTIN != null)
                    {
                        ViewBag.GstinList = Models.Common.LoadDropDowns.Exist_GSTINList(panNo, strGSTIN);
                    }
                }

                #endregion;

                switch (Command)
                {
                    case "BranchMgmt":
                        int status = BranchBusinessLayer.BranchName_Duplicate(branchName, custid, branchId, GstinId, "Entry");
                        if (status == 1)
                        {
                            TempData["ErrorMessage"] = "Location Name " + branchName + " Already Exist";
                        }
                        else
                        {
                            new BranchBusinessLayer(custid, userid, userAccessId, branchId).Branch_insert_update_delete(panNo, strGSTIN, branchName, email, 0, Mode);
                            TempData["SuccessMessage"] = "Location Successfully Created";
                        }

                        break;
                }

                #region "Retrieve Branch Details"
                DataSet ds = BranchBusinessLayer.Branch_Retrieve(custid);
                List<BranchDataAccess.BrachAttributes> BranchList = new List<BranchDataAccess.BrachAttributes>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BranchList.Add(new BranchDataAccess.BrachAttributes
                    {
                        branchId = dr.IsNull("branchId") ? 0 : Convert.ToInt32(dr["branchId"]),
                        company = dr.IsNull("company") ? "" : dr["company"].ToString(),
                        gstinNo = dr.IsNull("gstinNo") ? "" : dr["gstinNo"].ToString(),
                        branchName = dr.IsNull("branchName") ? "" : dr["branchName"].ToString(),
                        email = dr.IsNull("email") ? "" : dr["email"].ToString()
                    });
                }
                BranchViewModel Model = new BranchViewModel();
                Model.BranchList = BranchList;
                ViewBag.Panlist = Models.Common.LoadDropDowns.Exist_PanList(custid, panNo);
                return View(Model);
                #endregion

            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        public int Delete(int Id)
        {
            int status = 0;
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            if (Id != 0)
            {
                new BranchBusinessLayer(custid, userid, "", Id).Branch_insert_update_delete("", "", "", "", 0, "D");
                status = 1;
            }
            ViewBag.Panlist = Models.Common.LoadDropDowns.PanList(custid);
            ViewBag.GstinList = Models.Common.LoadDropDowns.GSTINList();
            return status;
        }

        public ActionResult Edit(int Id)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            try
            {

                string panNo = "", strGSTIN = "";
                ViewBag.Panlist = Models.Common.LoadDropDowns.PanList(custid);
                ViewBag.GstinList = Models.Common.LoadDropDowns.GSTINList();
                ViewBag.AddUserList = BranchBusinessLayer.AddOnEdit_UserAcess(Id, custid);
                ViewBag.DeleteUserList = BranchBusinessLayer.DeleteOnEdit_UserAcess(Id, custid);

                #region "Retrieve Branch Details"
                DataSet ds = BranchBusinessLayer.Branch_Retrieve_BasedONBranchId(Id);
                if (ds.Tables[0].Rows.Count > 0)
                {
                    panNo = ds.Tables[0].Rows[0]["pan"].ToString();
                    strGSTIN = ds.Tables[0].Rows[0]["gstinNo"].ToString();
                    ViewBag.Panlist = Models.Common.LoadDropDowns.Exist_PanList(custid, panNo);
                    ViewBag.GstinList = Models.Common.LoadDropDowns.Exist_GSTINList(panNo, strGSTIN);
                }

                List<BranchDataAccess.BrachAttributes> BranchList = new List<BranchDataAccess.BrachAttributes>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {
                    BranchList.Add(new BranchDataAccess.BrachAttributes
                    {
                        branchId = dr.IsNull("branchId") ? 0 : Convert.ToInt32(dr["branchId"]),
                        company = dr.IsNull("company") ? "" : dr["company"].ToString(),
                        gstinNo = dr.IsNull("gstinNo") ? "" : dr["gstinNo"].ToString(),
                        branchName = dr.IsNull("branchName") ? "" : dr["branchName"].ToString(),
                        email = dr.IsNull("email") ? "" : dr["email"].ToString()
                    });

                }

                BranchViewModel Model = new BranchViewModel();
                Model.BranchList = BranchList;
                #endregion

                return View(Model);
            }

            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();

        }

        [HttpPost]
        public ActionResult Edit(FormCollection Form, string Command)
        {
            if (Session["User_ID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            int custid = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            int branchId = 0;
            try
            {
                string panNo, strGSTIN, branchName, email, Mode, addUserId, deleteUserId;

                branchId = Convert.ToInt32(Form["branchId"]);
                panNo = Form["ddlPanNo"];
                strGSTIN = Form["ddlGSTIN"];
                branchName = Form["branchName"].ToUpper();
                email = Form["email"];
                #region"Email Validation"
                if (!string.IsNullOrEmpty(email))
                {
                    //Count total no of Email in the list
                    int totalEmail = email.Split(';').Count();
                    Regex regex = new Regex(@"[A-Za-z0-9._%-]+@[a-zA-Z0-9.-]+\.[A-Za-z]{2,4}");
                    int regexMatch = regex.Matches(email).Count;
                    //Count no of email with which regex matches
                    if (totalEmail == regexMatch)
                    {
                        //Emails are correctly formatted
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "Please enter the valid Email Address";//Raise error on one or more email is incorrect in the list
                        return RedirectToAction("Edit");
                    }
                }
                #endregion
                addUserId = Form["addUser"];
                deleteUserId = Form["deleteUser"];
                int GstinId = WeP_BAL.EwayBill.LoadDropDowns.Get_GSTINId(strGSTIN);
                switch (Command)
                {
                    case "Update":
                        int status = BranchBusinessLayer.BranchName_Duplicate(branchName, custid, branchId, GstinId, "Edit");
                        if (status == 1)
                        {
                            TempData["ErrorMessage"] = "Location Name " + branchName + " Already Exist";
                        }
                        else
                        {
                            Mode = "U";
                            new BranchBusinessLayer(custid, userid, "", branchId).Branch_insert_update_delete(panNo, strGSTIN, branchName, email, 0, Mode);
                            TempData["SuccessMessage"] = "Branch Updated Successfully";
                            return RedirectToAction("Home");
                        }
                        break;

                    case "Add":
                        Mode = "AU";
                        new BranchBusinessLayer(custid, userid, "", branchId).Branch_insert_update_delete(panNo, strGSTIN, branchName, email, Convert.ToInt32(addUserId), Mode);
                        TempData["SuccessMessage"] = "User linked to particular Location Successfully";
                        break;

                    case "Delete":
                        Mode = "DU";
                        new BranchBusinessLayer(custid, userid, "", branchId).Branch_insert_update_delete(panNo, strGSTIN, branchName, email, Convert.ToInt32(deleteUserId), Mode);
                        TempData["SuccessMessage"] = "User deleted from particular Location Successfully";
                        break;
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return RedirectToAction("Edit/" + branchId);
        }

    }
}