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
using System.Configuration;
using SmartAdminMvc.Models.EWAY;
using static SmartAdminMvc.Models.EWAY.ViewModel;

namespace SmartAdminMvc.Controllers
{
   
    public class EwayUpdateBillController : Controller
    {
        SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        EwayInsert DeleteEwayDetails = new EwayInsert();
        public WePGSPDBEntities db = new WePGSPDBEntities();
        Data_Access_Layer.EwayGetDetails DbLayer = new Data_Access_Layer.EwayGetDetails();

        private int ewbid;
        private string qtyUt;
        decimal totalvalue;
        public object GridFill { get; private set; }

        // GET: EwayConsManualEntry
        public ActionResult EwayUpdate(string docnum, string docdate,FormCollection Form)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            Session["docNo"] = docnum;
            Session["docDate"] = docdate;
            //ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(userid, custid, Session["Role_Name"].ToString());
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int userid = Convert.ToInt32(Session["User_ID"]);
            EwayInsert InsertEwayDetails1 = new EwayInsert();
            string docNo = Convert.ToString(Session["docNo"]);
            string docDate = Convert.ToString(Session["docDate"]);
            SqlDataAdapter da = new SqlDataAdapter("select TOP(1) *  from TBL_EWB_GENERATION where docNo = '" + docNo + "'and docDate = '" + docDate + "'", con);
            DataTable dt = new DataTable();
            da.Fill(dt);         
            var varsupplytype = dt.Rows[0]["supplyType"].ToString();
            var varsubsupplytype = dt.Rows[0]["subSupplyType"].ToString();
            var vardocumenttype = dt.Rows[0]["docType"].ToString();
            var transmode = dt.Rows[0]["transMode"].ToString();
            var gstin_state = dt.Rows[0]["fromStateCode"].ToString();
            var cgstin_state = dt.Rows[0]["toStateCode"].ToString();
            ViewBag.gstin_state = gstin_state;
            ViewBag.cgstin_state = cgstin_state;
            ViewBag.SupplyType = LoadGSTINNo.Exist_DropDownEwayBill("SupplyType", varsupplytype);
            ViewBag.DocumentType = LoadGSTINNo.Exist_DropDownEwayBill("DocumentType", vardocumenttype);
            ViewBag.SubSupplyType = LoadGSTINNo.Exist_DropDownEwayBill("SubSupplyType", varsubsupplytype);
            ViewBag.TransportMode = LoadGSTINNo.Exist_DropDownEwayBill("TransportMode", transmode);
            ViewBag.Units = LoadGSTINNo.DropDownEwayBill("Units");
            
            if (Session["gstinno"] != null)
            {
                ViewBag.GSTINNoList = LoadGSTINNo.Exist_GSTIN_No(userid, CustId, Session["gstinno"].ToString(), Session["Role_Name"].ToString());
            }
            else
            {
                ViewBag.GSTINNoList = LoadGSTINNo.GSTIN_No(userid, CustId, Session["Role_Name"].ToString());
            }

            Session["docNo"] = docnum;
            Session["docDate"] = docdate;
            SqlDataAdapter ada = new SqlDataAdapter("select *  from TBL_EWB_GENERATION where docNo = '" + docNo + "'and docDate = '" + docDate + "'", con);
            DataTable ds4 = new DataTable();
            ada.Fill(ds4);
            var taxavalValue = dt.Rows[0]["totalValue"].ToString();
            Session["totalvaluue"] = taxavalValue;



            DataSet ds = DbLayer.getEWAY(docnum, docdate);
            List<EwayList> ewaylistmaster = new List<EwayList>();
            foreach (DataRow dr in ds.Tables[0].Rows)
            {
                ewbid = Convert.ToInt32(dr["ewbid"]);
               
                ewaylistmaster.Add(new EwayList
                {
                    ewbid = Convert.ToInt32(dr["ewbid"]),          
                    docNo = dr["docNo"].ToString(),
                    supplyType = dr["supplyType"].ToString(),
                    subSupplyType = dr["subSupplyType"].ToString(),
                    docType = dr["docType"].ToString(),
                    docDate = dr["docDate"].ToString(),      
                    fromGstin = dr["fromGstin"].ToString(),
                    fromTrdName = dr["fromTrdName"].ToString(),
                    fromAddr1 = dr["fromAddr1"].ToString(),
                    fromAddr2 = dr["fromAddr2"].ToString(),
                    fromPlace = dr["fromPlace"].ToString(),
                    fromPinCode = Convert.ToInt32(dr["fromPinCode"]),
                    fromStateCode = Convert.ToInt32(dr["fromStateCode"]),
                    toGstin = dr["toGstin"].ToString(),
                    toTrdName = dr["toTrdName"].ToString(),
                    toAddr1 = dr["toAddr1"].ToString(),
                    toAddr2 = dr["toAddr2"].ToString(),
                    toPlace = dr["toPlace"].ToString(),
                    toPincode = Convert.ToInt32(dr["toPincode"]),
                    toStateCode = Convert.ToInt32(dr["toStateCode"]),                 
                    transMode = Convert.ToInt32(dr["transMode"]),
                    transDistance = dr["transDistance"].ToString(),
                    transporterId = dr["transporterId"].ToString(),
                    transporterName = dr["transporterName"].ToString(),
                    transDocNo = dr["transDocNo"].ToString(),
                    transDocDate = dr["transDocDate"].ToString(),
                    vehicleNo = dr["vehicleNo"].ToString()

                });
            }
           // test = 
          // int  ewbid = Convert.ToInt32(Form["ewbid"]);
            DataSet ds1 = DbLayer.getEWAYITEMS (ewbid);
            List<EwayItems> ewayitem = new List<EwayItems>();
           
            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                qtyUt = dr["qtyUnit"].ToString();
                ViewBag.qtyUt = qtyUt;
                if (Convert.ToDecimal(dr["cgstRate"]) != Convert.ToDecimal(0) && Convert.ToDecimal(dr["sgstRate"]) != Convert.ToDecimal(0))
                {
                    decimal rate = Convert.ToDecimal(dr["cgstRate"]) + Convert.ToDecimal(dr["sgstRate"]);
                }
                else
                {
                    decimal rate = Convert.ToDecimal(dr["igstRate"]);
                }



                ewayitem.Add(new EwayItems
                {
                    ewbid = Convert.ToInt32(dr["ewbid"]),
                    itmsid = Convert.ToInt32(dr["itmsid"]),
                    quantity = Convert.ToInt32(dr["quantity"]),
                    taxableAmount = Convert.ToDecimal(dr["taxableAmount"]),
                    cgstRate = Convert.ToDecimal(dr["cgstRate"]),
                    igstRate = Convert.ToDecimal(dr["igstRate"]),
                    sgstRate = Convert.ToDecimal(dr["sgstRate"]),
                    cessRate = Convert.ToDecimal(dr["cessRate"]),
                    productName = dr["productName"].ToString(),
                    productDesc = dr["productDesc"].ToString(),
                    hsnCode = dr["hsnCode"].ToString(),
                    qtyUnit = dr["qtyUnit"].ToString(),
                    rate = Convert.ToDecimal(dr["cgstRate"]) + Convert.ToDecimal(dr["sgstRate"])

            });

            }           
           ListViewmodel model = new ListViewmodel();
            model.master = ewaylistmaster;
            model.items = ewayitem;          
            return View(model);          
        }

        [HttpPost]
        public ActionResult EwayUpdate(FormCollection Form, string command, string docnumber, string docdate)
        {
            docnumber = Convert.ToString(Session["docNo"]);
            docdate = Convert.ToString(Session["docDate"]);
            string transactiontype, usid, genmode,productname, usermode, subtype, documenttype, tranportmode, transportdate, documnumber, fname, gstin, address1, Documentdate, address2, state, pincode, city, name1, gstin1, address11, address22, state1, city1, pincode1, transportername, transporterid, transporterDocNo, vehiclenumber, approximatedistance, HSN, itemdesc;
            decimal rate, totalvaluee, Taxablevalue, IGSTa,CESSa, CGSTa, SGSTa, qty, unitprice, igstval;
            int ewbid,custid, userid,id, itemnum = 0;
            string supplytype = Form["options"];
            string subsupplyType = Form["optionssub"];
            string documentType = Form["optiondoc"];
            string transMode = Form["optiontrans"];
            string units = Form["optionunit"];
            string strGSTINNo = Form["ddlGSTINNo"];
            
            int CustId = Convert.ToInt32(Session["Cust_ID"]);
            int Userid = Convert.ToInt32(Session["User_ID"]);
            Session["gstinno"] = strGSTINNo;
            EwayInsert InsertEwayDetails1 = new EwayInsert();
            string docNo = Convert.ToString(Session["docNo"]);
            string docDate = Convert.ToString(Session["docDate"]);
            ViewBag.SupplyType = LoadGSTINNo.Exist_DropDownEwayBill("SupplyType", supplytype);
            ViewBag.DocumentType = LoadGSTINNo.Exist_DropDownEwayBill("DocumentType", documentType);
            ViewBag.SubSupplyType = LoadGSTINNo.Exist_DropDownEwayBill("SubSupplyType", subsupplyType);
            ViewBag.TransportMode = LoadGSTINNo.Exist_DropDownEwayBill("TransportMode", transMode);
            ViewBag.Units = LoadGSTINNo.Exist_DropDownEwayBill("Units", units);
           
            ewbid = Convert.ToInt32(Form["ewbid"]);         
            transactiontype = Form["options"];
            subtype = Form["optionssub"];
            documenttype = Form["optiondoc"];
            documnumber = Form["docnumber"];
           
            Documentdate = Form["Documentdate"];
            fname = Form["name"];
            gstin = Form["gst"];
            address1 = Form["adress1"];
            address2 = Form["adress2"];
            state = Form["statecode"];
            pincode = Form["pincode"];
            city = Form["city"];
            name1 = Form["name1"];
            gstin1 = Form["gstin1"];
            address11 = Form["adress11"];
            address22 = Form["adress22"];
            state1 = Form["state1"];
            city1 = Form["city1"];
            pincode1 = Form["pincode"];
            transportername = Form["transportername"];
            transporterid = Form["transporterid"];
            transporterDocNo = Form["transporterDocNo"];
            tranportmode = Form["optiontrans"];
            transportdate = Form["transporterdate"];
            vehiclenumber = Form["Vehicleno"];
            approximatedistance = Form["AppDis"];
            if (command == "update")
            {                       
                InsertEwayDetails1.InsertEwayDetailsUpdated(transportdate, tranportmode, transactiontype, subtype, documenttype, Documentdate, documnumber, fname, gstin, address1, address2, state, pincode, city, name1, gstin1, address11, address22, state1, city1, pincode1, transportername, transporterid, transporterDocNo, vehiclenumber,approximatedistance);
                TempData["Update"] = "Data Updated Successfully";
                return RedirectToAction("EwayUpdate", "EwayUpdateBill", new { docnum = Session["docNo"].ToString(), docdate = Session["docDate"].ToString() });
            }
            if (command == "add")
            {
                decimal invalue, totalval;
                ewbid = Convert.ToInt32(Form["ewbid"]);
                docnumber = Convert.ToString(Session["docNo"]);
                docdate = Convert.ToString(Session["docDate"]);


                string uqc;
                custid = Convert.ToInt32(Session["Cust_ID"]);
                userid = Convert.ToInt32(Session["User_ID"]);

                //foreach (DataRow dr in ds1.Rows)
                //{
                HSN = Convert.ToString(Form["hsn"]);

                itemdesc = Convert.ToString(Form["itemdesc"]);
                productname = Convert.ToString(Form["productname"]);
                qty = Convert.ToDecimal(Form["qty"]);
                rate = Convert.ToDecimal(Form["rate"]);
                Taxablevalue = Convert.ToDecimal(Form["taxablevalue"]);

                unitprice = Convert.ToDecimal(Form["unitprice"]);

               
              
                 uqc = Convert.ToString(Form["optionunit"]);
                 uqc= uqc.TrimEnd(',');
                IGSTa = Convert.ToDecimal(Form["iamount"]);

                CGSTa = Convert.ToDecimal(Form["camount"]);
                SGSTa = Convert.ToDecimal(Form["samount"]);
                CESSa = Convert.ToDecimal(Form["cesssamount"]);
                if (CGSTa != 0 && SGSTa != 0)
                {
                    Decimal rt = rate / 2;
                    CGSTa = rt;
                    SGSTa = rt;
                    IGSTa = 0;

                }

                else
                {
                    IGSTa = rate;
                }
                  if (Session["totalvaluue"] == null || Session["totalvaluue"] == "")
                {
                    invalue = 0;
                }
                    else
                    {
                    invalue = Convert.ToDecimal(Form["invalue"]);
                }
                totalval = Convert.ToDecimal(Form["iamount"]) + Convert.ToDecimal(Form["samount"])+ Convert.ToDecimal(Form["camount"]) + Taxablevalue+ Convert.ToDecimal(Form["cesssamount"])+invalue;
              //  totalval = Convert.ToDecimal(Form["taxablevalue"]) + Convert.ToDecimal(Form["camount"]) + Convert.ToDecimal(Form["samount"]) + Convert.ToDecimal(Form["iamount"]) + Convert.ToDecimal(Form["invalue"]);
              Session["totalvaluue"] = totalval;

                con.Open();
                SqlCommand cmd = new SqlCommand();
                cmd.Parameters.Clear();
                //cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = "WeP.TN.2";
                cmd.Parameters.Add("@totalValue", SqlDbType.Decimal).Value = totalval;
                Models.Common.Functions.UpdateTable("TBL_EWB_GENERATION", "ewbid", Convert.ToString(ewbid), cmd, con);
                con.Close();
                InsertEwayDetails1.InsertEwayItems1(productname,ewbid,  HSN,  itemdesc,  qty,  uqc,  unitprice, rate, Taxablevalue, IGSTa, CESSa, CGSTa, SGSTa, Session["CustRefNo"].ToString(), Session["User_ID"].ToString());

                ObjectParameter op = new ObjectParameter("retinum", typeof(string));
               
            }
            return RedirectToAction("EwayUpdate", "EwayUpdateBill", new { docnum = Session["docNo"].ToString(), docdate = Session["docDate"].ToString() });
            
           
        }
       [HttpGet]
        public ActionResult DeleteEway(int id)
        {
            var message = id;
            int mode = 2;
            //SqlDataAdapter aada = new SqlDataAdapter("select  * from TBL_EWB_GENERATION_ITMS  where itmsid = '" + id + "'", con);
            //DataTable dss4 = new DataTable();
            //aada.Fill(dss4);
            //var ewbd = dss4.Rows[0]["ewbid"].ToString();
            int output = DeleteEwayDetails.EwayItemDelete(id,mode);
            TempData["Delete"] = "Item Deleted Successfully";
            //SqlDataAdapter asda = new SqlDataAdapter("select  * from TBL_EWB_GENERATION_ITMS  where ewbid = '" + ewbd + "'", con);
            //DataTable dss = new DataTable();
            //asda.Fill(dss);
            //if (dss.Rows.Count ==0)
            //{
            //    DeleteEwayDetails.EwayRecordDelete(Convert.ToInt32(ewbd));

            //    message = 2;
              
            //}

           
            return Json(message, JsonRequestBehavior.AllowGet);

          

        }
        [HttpPost]
        public ActionResult UpdateItems(int id,string productname,string hsncode,string itemdesc, decimal qty,string uqc, decimal unitprice,decimal txamt, decimal igsta,decimal cgsta, decimal sgsta, decimal cessa)
        { 
            int ewbid = id;
            var message = id;
            //decimal  igstvalue=0, cgstvalue=0, sgstvalue=0, cessvalue=0, totalvalue=0;
              int output = DeleteEwayDetails.EwayItemUpdate(productname ,id,  hsncode,  itemdesc,  qty,  uqc,  unitprice, txamt,  igsta,  cgsta,  sgsta,  cessa);
         
            //if (cgsta != 0 && sgsta != 0)
            //{
            //    cgstvalue = txamt * cgsta / 100;
            //    sgstvalue = txamt * sgsta / 100;
            //}
            //else
            //{
            //    igstvalue = txamt * igsta / 100;
                
            //}
            //totalvalue = txamt + igstvalue + sgstvalue + cgstvalue+cessvalue;
            //Session["totalvaluue"] = totalvalue;
            //con.Open();
            //SqlCommand cmd = new SqlCommand();
            //cmd.Parameters.Clear();          
            ////cmd.Parameters.Add("@Username", SqlDbType.NVarChar).Value = "WeP.TN.2";
            //cmd.Parameters.Add("@cgstValue", SqlDbType.Decimal).Value = cgstvalue;
            //cmd.Parameters.Add("@igstValue", SqlDbType.Decimal).Value = igstvalue;
            //cmd.Parameters.Add("@sgstValue", SqlDbType.Decimal).Value = sgstvalue;
            //cmd.Parameters.Add("@cessValue", SqlDbType.Decimal).Value = cessvalue;
            //cmd.Parameters.Add("@totalValue", SqlDbType.Decimal).Value = totalvalue;
            //Models.Common.Functions.UpdateTable("TBL_EWB_GENERATION", "ewbid",Convert.ToString(ewbid), cmd, con);
            //con.Close();
          
     
            TempData["ItemsUpdated"] = "Item Data Updated Successfully";

            return Json(message, JsonRequestBehavior.AllowGet);

        }

    }
  


}







