using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SmartAdminMvc.Models;
using System.Data.Entity.Core.Objects;
using System.Collections;
using System.Data;
using SmartAdminMvc.Models.Common;
using System.Data.SqlClient;
using SmartAdminMvc.Models.EWAY;
using static SmartAdminMvc.Models.EWAY.ViewModelConso;
using System.Configuration;

namespace SmartAdminMvc.Controllers
{
    public class EwayCosoUpdateController : Controller
    {
        public WePGSPDBEntities db = new WePGSPDBEntities();
        // GET: EwayCosoUpdate  public WePGSPDBEntities db = new WePGSPDBEntities();
        Data_Access_Layer.GonsoGet DbLayer = new Data_Access_Layer.GonsoGet();
        EwayInsert DeleteEwayDetails = new EwayInsert();
        private int consewbid;
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);

        public ActionResult EwayConso(string docnum, string docdate)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            Session["transDocNo"] = docnum;
            Session["transDocDate"] = docdate;
            try
            {
                int custid = Convert.ToInt32(Session["Cust_ID"]);
                int userid = Convert.ToInt32(Session["User_ID"]);
                SqlDataAdapter da = new SqlDataAdapter("select TOP(1) * from TBL_EWB_GEN_CONSOLIDATED where transdocNo = '" + docnum + "'and transdocDate = '" + docdate + "'", con);
                DataTable dt = new DataTable();
                da.Fill(dt);
              
                var varstatecode = dt.Rows[0]["fromState"].ToString();
                var vartransmode = dt.Rows[0]["transMode"].ToString();
                ViewBag.TransportMode = LoadGSTINNo.Exist_DropDownEwayBill("TransportMode", vartransmode);
                ViewBag.StateCode = LoadGSTINNo.Exist_DropDownEwayBill("StateCode", varstatecode);
      
                if (Session["gstinnno"] != null)
                {
                    ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(userid, custid, Session["gstinnno"].ToString(), Session["Role_Name"].ToString());
                }
                else
                {
                    ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
                }
                DataSet ds = DbLayer.getEWAYCONSO(docnum, docdate);
                List<EwayConsoList> ewaylistmaster = new List<EwayConsoList>();
                foreach (DataRow dr in ds.Tables[0].Rows)
                {

                    consewbid = Convert.ToInt32(dr["consewbid"]);
                    ewaylistmaster.Add(new EwayConsoList
                    {
                        consewbid = Convert.ToInt32(dr["consewbid"]),
                        fromPlace = dr["fromPlace"].ToString(),
                        vehicleNo = dr["vehicleNo"].ToString(),
                        fromState = dr["fromState"].ToString(),
                        transMode = dr["transMode"].ToString(),
                        transDocNo = dr["transDocNo"].ToString(),
                        transDocDate = dr["transDocDate"].ToString(),
                    });
                }

                DataSet ds1 = DbLayer.getEWAYITEMSCONSO(consewbid);

                List<EwayConsoItems> ewayitem = new List<EwayConsoItems>();

                foreach (DataRow dr in ds1.Tables[0].Rows)
                {

                    ewayitem.Add(new EwayConsoItems
                    {
                        tripsheetid = Convert.ToInt32(dr["tripsheetid"]),
                        consewbid = Convert.ToInt32(dr["consewbid"]),
                        ewbNo = dr["ewbNo"].ToString()
                    });

                }

                ListViewmodel1 model = new ListViewmodel1();
                model.master = ewaylistmaster;
                model.items = ewayitem;

                return View(model);


            }
            catch (Exception e)
            {
                
                throw e;
            }

        }
        private List<IDictionary> ConvertToDictionary(DataTable dtObject)
        {
            var columns = dtObject.Columns.Cast<DataColumn>();

            var dictionaryList = dtObject.AsEnumerable()
                .Select(dataRow => columns
                    .Select(column =>
                        new { Column = column.ColumnName, Value = dataRow[column] })
                             .ToDictionary(data => data.Column, data => data.Value)).ToList().ToArray();

            return dictionaryList.ToList<IDictionary>();
        }

        [HttpPost]
        public ActionResult EwayConso(FormCollection Form, string command, string docnum, string docdate)
        {

            decimal consewbid;
            string ewbNo;
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int Userid = Convert.ToInt32(Session["User_ID"]);
            docnum = Convert.ToString(Session["transDocNo"]);
            docdate = Convert.ToString(Session["transDocDate"]);
            SqlDataAdapter da = new SqlDataAdapter("select TOP(1) * from TBL_EWB_GEN_CONSOLIDATED where transdocNo = '" + docnum + "'and transdocDate = '" + docdate + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);
            var consewb = dt.Rows[0]["consewbid"].ToString();
            ViewBag.StateCode = LoadGSTINNo.GetEWayBillType("StateCode");
            ViewBag.TransportMode = LoadGSTINNo.GetEWayBillType("TransportMode");
            string strGSTINNo = Form["ddlGSTINNo"];    
            Session["gstinnno"] = strGSTINNo;
            if (command == "updatedet")
            {

                string VehicleNumber, FromPlace, transMode, TransDocDate, FromState, transDocNo;

                int custid, userid, itemnum = 0;
                userid = Convert.ToInt32(Session["User_Id"]);
                custid = Convert.ToInt32(Session["Cust_ID"]);
                EwayInsert InsertEwayDetails2 = new EwayInsert();
                VehicleNumber = Form["VehicleNumber"];
                transDocNo = Form["transdocno"];
                transMode = Form["optiontransmode"];
                FromPlace = Form["frmplce"];
                FromState = Form["optionstatecode"];
                TransDocDate = Form["transporterdate"];
                DeleteEwayDetails.EwayConsoDetailsUpdate(Convert.ToInt32(consewb),VehicleNumber, FromPlace, transMode, TransDocDate, FromState, transDocNo);
                TempData["DetailsUpdated"] = "EwayBill Details Updated Successfully";
                return RedirectToAction("EwayConso", "EwayCosoUpdate", new { docnum = Session["transDocNo"].ToString(), docdate = Session["transDocDate"].ToString() });
             
            }
            if (command == "add")
            {
                consewbid = Convert.ToInt32(Form["consewbid"]);
                ewbNo = Convert.ToString(Form["ewaybillno"]);
                DeleteEwayDetails.EwayConsoEwayAdd(consewbid, ewbNo);
             
                return RedirectToAction("EwayConso", "EwayCosoUpdate", new { docnum = Session["transDocNo"].ToString(), docdate = Session["transDocDate"].ToString() });
            }
            return RedirectToAction("EwayConso", "EwayCosoUpdate", new { docnum = Session["transDocNo"].ToString(), docdate = Session["transDocDate"].ToString() });
        }
        public ActionResult EwayItemsUpdate(int id, string ewbno)
        {
            var message = id;
            int output = DeleteEwayDetails.EwayBillUpdate(id, ewbno);
            TempData["ItemsUpdatedd"] = "EwayBill Updated Successfully";


            return Json(message, JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult EwayItemsDeleteConso(int id)
        {
            var message = id;
            int output = DeleteEwayDetails.EwayBillDeleteConso(id);
            TempData["Deleted"] = "Eway Bill Number Deleted Successfully";
            return Json(message, JsonRequestBehavior.AllowGet);
        }


    }
}