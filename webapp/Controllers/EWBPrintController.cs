# region "namespaces"
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web.Mvc;
using WeP_BAL;
using WeP_DAL;
using Rotativa;
using System.Web;
using System.Collections;
using System.IO;
using System.Text.RegularExpressions;
using ClosedXML.Excel;
using System.Web.UI.WebControls;
using WeP_EWayBill;
using System.Web.UI;
using iTextSharp.text.pdf;
using iTextSharp.text.html.simpleparser;
using iTextSharp.text;
using System.Net.Mail;
using System.Net;
using iTextSharp.tool.xml;
using QRCoder;
using System.Drawing;
using System.Drawing.Imaging;

#endregion

namespace SmartAdminMvc.Controllers
{
    public class EwbPrintController : Controller
    {

        #region "Static Variable Declaration For Eway Bill and Consolidated Eway Bill Print and Download"
        static int i;
        static string EWBNO;
        static string DEWBNO;
        static string CEWBNO;
        static string CDEWBNO;
        static string ACTIONDATA;
        static string USERGSIN;
        static string DEWGSTIN;
        #endregion

        #region "EWAY BILL"
        public ActionResult EWB(string ewbNo, string usergstin,string ActionData)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                if (Session["Cust_ID"] != null)
                {
                    i = Convert.ToInt32(Session["Cust_ID"]);
                }
               
                string OStatus = "0";
                string OResponse = "";
                EWBNO = ewbNo;
                USERGSIN = usergstin;
                Session["EwayBillNo"] = ewbNo;
                Session["userGSTIN"] = usergstin;
                ACTIONDATA = ActionData;
                int hsnCode = 0, hsnCount = 0;
                string gstinnum = "", ewaybilldate = "";
                DataSet ds1 = EwaybillDataAccess.getEWAY(ewbNo, usergstin,ActionData);
                EwaybillDataAccess.getGSTIN(EWBNO, out gstinnum, out ewaybilldate);
                using (MemoryStream ms = new MemoryStream())
                {
                    QRCodeGenerator qrGenerator = new QRCodeGenerator();
                    var qrCodedata = qrGenerator.CreateQrCode("" + EWBNO + "|" + gstinnum + "|" + ewaybilldate + "", QRCodeGenerator.ECCLevel.H);
                    var qrcode = new QRCoder.QRCode(qrCodedata);
                    var BarcodeImage = qrcode.GetGraphic(150);
                    ViewBag.BarcodeImage = BarcodeImage;
                    using (Bitmap bitMap = qrcode.GetGraphic(3))
                    {
                        bitMap.Save(ms, ImageFormat.Png);
                        ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                    }
                }

            
                if (ds1.Tables[0].Rows.Count == 0)
                      {
                    #region "Authentication Checking"
                    EwbGeneratingKeys.Autentication(usergstin);
                    #endregion

                    EwbGetApi.GET_EWAYBILL(ewbNo, usergstin, Session["User_ID"].ToString(), Session["Cust_ID"].ToString(),
                                 Session["UserName"].ToString(), out OStatus, out OResponse);
                    ds1 = EwaybillDataAccess.getEWAY(ewbNo, usergstin,ActionData);
                }
                
                DataSet ds2 = EwaybillDataAccess.getEWAYList(ewbNo);
                
                List<EwayAttributes.Master> Master = new List<EwayAttributes.Master>();
                List<EwayAttributes.MasterList> MasterList = new List<EwayAttributes.MasterList>();
                List<EwayAttributes.HsnList> hsnList = new List<EwayAttributes.HsnList>();

                #region "TOP1 Record" 

                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    Master.Add(new EwayAttributes.Master
                    {
                        ewbid = Convert.ToInt32(dr["ewbid"]),
                        docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                        supplyType = dr.IsNull("supplyType") ? "" : dr["supplyType"].ToString(),
                        subSupplyType = dr.IsNull("subSupplyType") ? "" : dr["subSupplyType"].ToString(),
                        docType = dr.IsNull("docType") ? "" : dr["docType"].ToString(),
                        docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                        fromGstin = dr.IsNull("fromGstin") ? "" : dr["fromGstin"].ToString(),
                        fromTrdName = dr.IsNull("fromTrdName") ? "" : dr["fromTrdName"].ToString(),
                        fromAddr1 = dr.IsNull("fromAddr1") ? "" : dr["fromAddr1"].ToString(),
                        fromAddr2 = dr.IsNull("fromAddr2") ? "" : dr["fromAddr2"].ToString(),
                        fromPlace = dr.IsNull("fromPlace") ? "" : dr["fromPlace"].ToString(),
                        fromPinCode = dr.IsNull("fromPinCode") ? 0 : Convert.ToInt32(dr["fromPinCode"]),
                        fromStateCode = dr.IsNull("fromStateCode") ? 0 : Convert.ToInt32(dr["fromStateCode"]),
                        toGstin = dr.IsNull("toGstin") ? "" : dr["toGstin"].ToString(),
                        toTrdName = dr.IsNull("toTrdName") ? "" : dr["toTrdName"].ToString(),
                        toAddr1 = dr.IsNull("toAddr1") ? "" : dr["toAddr1"].ToString(),
                        toAddr2 = dr.IsNull("toAddr2") ? "" : dr["toAddr2"].ToString(),
                        toPlace = dr.IsNull("toPlace") ? "" : dr["toPlace"].ToString(),
                        toPincode = dr.IsNull("toPincode") ? 0 : Convert.ToInt32(dr["toPincode"]),
                        toStateCode = dr.IsNull("toStateCode") ? 0 : Convert.ToInt32(dr["toStateCode"]),
                        totalValue = dr.IsNull("totalValue") ? 0 : Convert.ToDecimal(dr["totalValue"]),
                        cgstValue = dr.IsNull("cgstValue") ? 0 : Convert.ToDecimal(dr["cgstValue"]),
                        igstValue = dr.IsNull("igstValue") ? 0 : Convert.ToDecimal(dr["igstValue"]),
                        sgstValue = dr.IsNull("sgstValue") ? 0 : Convert.ToDecimal(dr["sgstValue"]),
                        cessValue = dr.IsNull("cessValue") ? 0 : Convert.ToDecimal(dr["cessValue"]),
                        transMode = dr.IsNull("transMode") ? 0 : Convert.ToInt32(dr["transMode"]),
                        transDistance = dr.IsNull("transDistance") ? "" : dr["transDistance"].ToString(),
                        transporterId = dr.IsNull("transporterId") ? "" : dr["transporterId"].ToString(),
                        transporterName = dr.IsNull("transporterName") ? "" : dr["transporterName"].ToString(),
                        transDocNo = dr.IsNull("transDocNo") ? "" : dr["transDocNo"].ToString(),
                        transDocDate = dr.IsNull("transDocDate") ? "" : dr["transDocDate"].ToString(),
                        vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                        validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                        ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                        ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                        CompanyName = dr.IsNull("CompanyName") ? "" : dr["CompanyName"].ToString(),
                        transactionType = dr.IsNull("transactionType") ? "" : dr["transactionType"].ToString()
                    });

                }

                #endregion

                #region "All Records"
          
           foreach (DataRow dr in ds2.Tables[0].Rows)
                {
                    MasterList.Add(new EwayAttributes.MasterList
                    {
                        ewbVehUpdid = Convert.ToInt32(dr["ewbVehUpdid"]),
                        userGSTIN = dr.IsNull("userGSTIN") ? "" : dr["userGSTIN"].ToString(),
                        EwbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                        validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                        vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                        FromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                        FromState = dr.IsNull("FromState") ? "" : dr["FromState"].ToString(),
                        TransMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                        TransDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                        TransDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                        UPD_status = dr.IsNull("UPD_status") ? "" : dr["UPD_status"].ToString(),
                        ReasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                        ReasonRem = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                        UPD_errorCodes = dr.IsNull("UPD_errorCodes") ? "" : dr["UPD_errorCodes"].ToString(),
                        UPD_errorDescription = dr.IsNull("UPD_errorDescription") ? "" : dr["UPD_errorDescription"].ToString()
                    });

                }
            
                #endregion

                #region "HSN Details"
                int ewbId = EwaybillDataAccess.getEwbId(ewbNo, i);
                 hsnCode = EwaybillDataAccess.getHSNDetails(ewbId, i);
                int hsn_Count = EwaybillDataAccess.getHSNCount(ewbId, i);
                hsnCount = hsn_Count - 1;
                hsnList.Add(new EwayAttributes.HsnList
                {
                    hsnCode = hsnCode,
                    hsnCount=hsnCount
                });
                #endregion

                EwayViewModel Model = new EwayViewModel();
                Model.Master = Master;
                Model.MasterList = MasterList;
                Model.hsnList = hsnList;

                return View(Model);
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            return View();
        }
     
        public ActionResult ExportPdf()
        {

            return new ActionAsPdf("Print")
            {
                FileName = Session["EwayBillNo"].ToString() + ".pdf"
            };

        }

        public ActionResult Print()
        {
            int hsnCode = 0, hsnCount = 0;
            string gstinnum = "", ewaybilldate = "";
            DataSet ds1 = EwaybillDataAccess.getEWAY(EWBNO, USERGSIN, ACTIONDATA);
            DataSet ds2 = EwaybillDataAccess.getEWAYList(EWBNO);
            EwaybillDataAccess.getGSTIN(EWBNO,out gstinnum,out ewaybilldate);
             
            using (MemoryStream ms = new MemoryStream())
            {
                QRCodeGenerator qrGenerator = new QRCodeGenerator();
                var qrCodedata = qrGenerator.CreateQrCode("" + EWBNO + "|" + gstinnum + "|"+ ewaybilldate + "", QRCodeGenerator.ECCLevel.H);
                var qrcode = new QRCoder.QRCode(qrCodedata);
                var BarcodeImage = qrcode.GetGraphic(150);
                ViewBag.BarcodeImage = BarcodeImage;
                using (Bitmap bitMap = qrcode.GetGraphic(3))
                {
                    bitMap.Save(ms, ImageFormat.Png);
                    ViewBag.QRCodeImage = "data:image/png;base64," + Convert.ToBase64String(ms.ToArray());
                }
            }
            List<EwayAttributes.Master> Master = new List<EwayAttributes.Master>();
            List<EwayAttributes.MasterList> MasterList = new List<EwayAttributes.MasterList>();
            List<EwayAttributes.HsnList> hsnList = new List<EwayAttributes.HsnList>();

            #region "TOP1 Record" 

            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                Master.Add(new EwayAttributes.Master
                {
                    ewbid = Convert.ToInt32(dr["ewbid"]),
                    docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                    supplyType = dr.IsNull("supplyType") ? "" : dr["supplyType"].ToString(),
                    subSupplyType = dr.IsNull("subSupplyType") ? "" : dr["subSupplyType"].ToString(),
                    docType = dr.IsNull("docType") ? "" : dr["docType"].ToString(),
                    docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                    fromGstin = dr.IsNull("fromGstin") ? "" : dr["fromGstin"].ToString(),
                    fromTrdName = dr.IsNull("fromTrdName") ? "" : dr["fromTrdName"].ToString(),
                    fromAddr1 = dr.IsNull("fromAddr1") ? "" : dr["fromAddr1"].ToString(),
                    fromAddr2 = dr.IsNull("fromAddr2") ? "" : dr["fromAddr2"].ToString(),
                    fromPlace = dr.IsNull("fromPlace") ? "" : dr["fromPlace"].ToString(),
                    fromPinCode = dr.IsNull("fromPinCode") ? 0 : Convert.ToInt32(dr["fromPinCode"]),
                    fromStateCode = dr.IsNull("fromStateCode") ? 0 : Convert.ToInt32(dr["fromStateCode"]),
                    toGstin = dr.IsNull("toGstin") ? "" : dr["toGstin"].ToString(),
                    toTrdName = dr.IsNull("toTrdName") ? "" : dr["toTrdName"].ToString(),
                    toAddr1 = dr.IsNull("toAddr1") ? "" : dr["toAddr1"].ToString(),
                    toAddr2 = dr.IsNull("toAddr2") ? "" : dr["toAddr2"].ToString(),
                    toPlace = dr.IsNull("toPlace") ? "" : dr["toPlace"].ToString(),
                    toPincode = dr.IsNull("toPincode") ? 0 : Convert.ToInt32(dr["toPincode"]),
                    toStateCode = dr.IsNull("toStateCode") ? 0 : Convert.ToInt32(dr["toStateCode"]),
                    totalValue = dr.IsNull("totalValue") ? 0 : Convert.ToDecimal(dr["totalValue"]),
                    cgstValue = dr.IsNull("cgstValue") ? 0 : Convert.ToDecimal(dr["cgstValue"]),
                    igstValue = dr.IsNull("igstValue") ? 0 : Convert.ToDecimal(dr["igstValue"]),
                    sgstValue = dr.IsNull("sgstValue") ? 0 : Convert.ToDecimal(dr["sgstValue"]),
                    cessValue = dr.IsNull("cessValue") ? 0 : Convert.ToDecimal(dr["cessValue"]),
                    transMode = dr.IsNull("transMode") ? 0 : Convert.ToInt32(dr["transMode"]),
                    transDistance = dr.IsNull("transDistance") ? "" : dr["transDistance"].ToString(),
                    transporterId = dr.IsNull("transporterId") ? "" : dr["transporterId"].ToString(),
                    transporterName = dr.IsNull("transporterName") ? "" : dr["transporterName"].ToString(),
                    transDocNo = dr.IsNull("transDocNo") ? "" : dr["transDocNo"].ToString(),
                    transDocDate = dr.IsNull("transDocDate") ? "" : dr["transDocDate"].ToString(),
                    vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                    validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                    ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                    ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                    CompanyName = dr.IsNull("CompanyName") ? "" : dr["CompanyName"].ToString(),
                    transactionType = dr.IsNull("transactionType") ? "" : dr["transactionType"].ToString()
                });

            }

            #endregion

            #region "All Records"
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {


                MasterList.Add(new EwayAttributes.MasterList
                {
                    ewbVehUpdid = Convert.ToInt32(dr["ewbVehUpdid"]),
                    userGSTIN = dr.IsNull("userGSTIN") ? "" : dr["userGSTIN"].ToString(),
                    EwbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                    vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                    FromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                    FromState = dr.IsNull("FromState") ? "" : dr["FromState"].ToString(),
                    TransMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                    TransDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                    TransDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                    UPD_status = dr.IsNull("UPD_status") ? "" : dr["UPD_status"].ToString(),
                    ReasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                    ReasonRem = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                    UPD_errorCodes = dr.IsNull("UPD_errorCodes") ? "" : dr["UPD_errorCodes"].ToString(),
                    UPD_errorDescription = dr.IsNull("UPD_errorDescription") ? "" : dr["UPD_errorDescription"].ToString()
                });

            }

            #endregion

            #region "HSN Details"
            int ewbId = EwaybillDataAccess.getEwbId(EWBNO, i);
            hsnCode = EwaybillDataAccess.getHSNDetails(ewbId, i);
            int hsn_Count = EwaybillDataAccess.getHSNCount(ewbId, i);
            hsnCount = hsn_Count - 1;
            hsnList.Add(new EwayAttributes.HsnList
            {
                hsnCode = hsnCode,
                hsnCount = hsnCount
            });
            #endregion

            EwayViewModel Model = new EwayViewModel();
            Model.Master = Master;
            Model.MasterList = MasterList;
            Model.hsnList = hsnList;


            return PartialView("_PrintPdf", Model);
        }

        public ActionResult StandartPDF(string ewbNo)
        {

            string fileName =ewbNo.ToString() + ".pdf";
            var makeCvSession = Session["makeCV"];

            var root = Server.MapPath("~/App_Data/EWBPdf");
            var pdfname = String.Format("{0}.pdf", Guid.NewGuid().ToString());
            var path = Path.Combine(root, pdfname);
            path = Path.GetFullPath(path);
            
            var something = new Rotativa.ViewAsPdf("StandartPDF", makeCvSession) { FileName = fileName, SaveOnServerPath = fileName };
            return something;

        }


        public ActionResult DownloadPdf(string ewbNo, string usergstin)
        {
            DEWBNO = ewbNo;
            DEWGSTIN = usergstin;
            return new ActionAsPdf("Download")
            {
                FileName = ewbNo.ToString() + ".pdf"

            };
        }

        public ActionResult Download()
        {
            int hsnCode = 0, hsnCount = 0;
            DataSet ds1 = EwaybillDataAccess.getEWAY(DEWBNO, DEWGSTIN, ACTIONDATA);
            DataSet ds2 = EwaybillDataAccess.getEWAYList(DEWBNO);

            List<EwayAttributes.Master> Master = new List<EwayAttributes.Master>();
            List<EwayAttributes.MasterList> MasterList = new List<EwayAttributes.MasterList>();
            List<EwayAttributes.HsnList> hsnList = new List<EwayAttributes.HsnList>();

            #region "TOP1 Record" 

            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                Master.Add(new EwayAttributes.Master
                {
                    ewbid = Convert.ToInt32(dr["ewbid"]),
                    docNo = dr.IsNull("docNo") ? "" : dr["docNo"].ToString(),
                    supplyType = dr.IsNull("supplyType") ? "" : dr["supplyType"].ToString(),
                    subSupplyType = dr.IsNull("subSupplyType") ? "" : dr["subSupplyType"].ToString(),
                    docType = dr.IsNull("docType") ? "" : dr["docType"].ToString(),
                    docDate = dr.IsNull("docDate") ? "" : dr["docDate"].ToString(),
                    fromGstin = dr.IsNull("fromGstin") ? "" : dr["fromGstin"].ToString(),
                    fromTrdName = dr.IsNull("fromTrdName") ? "" : dr["fromTrdName"].ToString(),
                    fromAddr1 = dr.IsNull("fromAddr1") ? "" : dr["fromAddr1"].ToString(),
                    fromAddr2 = dr.IsNull("fromAddr2") ? "" : dr["fromAddr2"].ToString(),
                    fromPlace = dr.IsNull("fromPlace") ? "" : dr["fromPlace"].ToString(),
                    fromPinCode = dr.IsNull("fromPinCode") ? 0 : Convert.ToInt32(dr["fromPinCode"]),
                    fromStateCode = dr.IsNull("fromStateCode") ? 0 : Convert.ToInt32(dr["fromStateCode"]),
                    toGstin = dr.IsNull("toGstin") ? "" : dr["toGstin"].ToString(),
                    toTrdName = dr.IsNull("toTrdName") ? "" : dr["toTrdName"].ToString(),
                    toAddr1 = dr.IsNull("toAddr1") ? "" : dr["toAddr1"].ToString(),
                    toAddr2 = dr.IsNull("toAddr2") ? "" : dr["toAddr2"].ToString(),
                    toPlace = dr.IsNull("toPlace") ? "" : dr["toPlace"].ToString(),
                    toPincode = dr.IsNull("toPincode") ? 0 : Convert.ToInt32(dr["toPincode"]),
                    toStateCode = dr.IsNull("toStateCode") ? 0 : Convert.ToInt32(dr["toStateCode"]),
                    totalValue = dr.IsNull("totalValue") ? 0 : Convert.ToDecimal(dr["totalValue"]),
                    cgstValue = dr.IsNull("cgstValue") ? 0 : Convert.ToDecimal(dr["cgstValue"]),
                    igstValue = dr.IsNull("igstValue") ? 0 : Convert.ToDecimal(dr["igstValue"]),
                    sgstValue = dr.IsNull("sgstValue") ? 0 : Convert.ToDecimal(dr["sgstValue"]),
                    cessValue = dr.IsNull("cessValue") ? 0 : Convert.ToDecimal(dr["cessValue"]),
                    transMode = dr.IsNull("transMode") ? 0 : Convert.ToInt32(dr["transMode"]),
                    transDistance = dr.IsNull("transDistance") ? "" : dr["transDistance"].ToString(),
                    transporterId = dr.IsNull("transporterId") ? "" : dr["transporterId"].ToString(),
                    transporterName = dr.IsNull("transporterName") ? "" : dr["transporterName"].ToString(),
                    transDocNo = dr.IsNull("transDocNo") ? "" : dr["transDocNo"].ToString(),
                    transDocDate = dr.IsNull("transDocDate") ? "" : dr["transDocDate"].ToString(),
                    vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                    validUpto = dr.IsNull("validUpto") ? "" : dr["validUpto"].ToString(),
                    ewayBillNo = dr.IsNull("ewayBillNo") ? "" : dr["ewayBillNo"].ToString(),
                    ewayBillDate = dr.IsNull("ewayBillDate") ? "" : dr["ewayBillDate"].ToString(),
                    CompanyName = dr.IsNull("CompanyName") ? "" : dr["CompanyName"].ToString(),
                    transactionType = dr.IsNull("transactionType") ? "" : dr["transactionType"].ToString()
                });

            }

            #endregion

            #region "All Records"
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {


                MasterList.Add(new EwayAttributes.MasterList
                {
                    ewbVehUpdid = Convert.ToInt32(dr["ewbVehUpdid"]),
                    userGSTIN = dr.IsNull("userGSTIN") ? "" : dr["userGSTIN"].ToString(),
                    EwbNo = dr.IsNull("EwbNo") ? "" : dr["EwbNo"].ToString(),
                    vehicleNo = dr.IsNull("vehicleNo") ? "" : dr["vehicleNo"].ToString(),
                    FromPlace = dr.IsNull("FromPlace") ? "" : dr["FromPlace"].ToString(),
                    FromState = dr.IsNull("FromState") ? "" : dr["FromState"].ToString(),
                    TransMode = dr.IsNull("TransMode") ? "" : dr["TransMode"].ToString(),
                    TransDocNo = dr.IsNull("TransDocNo") ? "" : dr["TransDocNo"].ToString(),
                    TransDocDate = dr.IsNull("TransDocDate") ? "" : dr["TransDocDate"].ToString(),
                    UPD_status = dr.IsNull("UPD_status") ? "" : dr["UPD_status"].ToString(),
                    ReasonCode = dr.IsNull("ReasonCode") ? "" : dr["ReasonCode"].ToString(),
                    ReasonRem = dr.IsNull("ReasonRem") ? "" : dr["ReasonRem"].ToString(),
                    UPD_errorCodes = dr.IsNull("UPD_errorCodes") ? "" : dr["UPD_errorCodes"].ToString(),
                    UPD_errorDescription = dr.IsNull("UPD_errorDescription") ? "" : dr["UPD_errorDescription"].ToString()
                });

            }

            #endregion

            #region "HSN Details"
            int ewbId = EwaybillDataAccess.getEwbId(EWBNO, i);
            hsnCode = EwaybillDataAccess.getHSNDetails(ewbId, i);
            int hsn_Count = EwaybillDataAccess.getHSNCount(ewbId, i);
            hsnCount = hsn_Count - 1;
            hsnList.Add(new EwayAttributes.HsnList
            {
                hsnCode = hsnCode,
                hsnCount = hsnCount
            });
            #endregion

            EwayViewModel Model = new EwayViewModel();
            Model.Master = Master;
            Model.MasterList = MasterList;
            Model.hsnList = hsnList;



            return PartialView("_Print", Model);
        }

        #endregion

        #region "Consolidated EWAY BILL"
        public ActionResult CONSEWB(string cewbNo)
        {
            if (!Request.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account");
            }
            if (Session["User_ID"] == null)
                return RedirectToAction("Login", "Account");
            try
            {
                if (Session["Cust_ID"] != null)
                {
                    i = Convert.ToInt32(Session["Cust_ID"]);
                }
                CEWBNO = cewbNo;
                Session["CEwayBillNo"] = cewbNo;
                int conewbid = 0;
                EwaybillDataAccess.getConsewbid(cewbNo, out conewbid);
                int cewbid = conewbid;
                DataSet ds1 = EwaybillDataAccess.getCEWAY(cewbNo);
                DataSet ds2 = EwaybillDataAccess.getCEWAYList(cewbid);

                List<EwayAttributes.ConsolidatedMaster> CMaster = new List<EwayAttributes.ConsolidatedMaster>();
                List<EwayAttributes.ConsolidatedMasterList> CMasterList = new List<EwayAttributes.ConsolidatedMasterList>();

                #region "TOP1 Record" 

                foreach (DataRow dr in ds1.Tables[0].Rows)
                {
                    CMaster.Add(new EwayAttributes.ConsolidatedMaster
                    {
                        consewbid = Convert.ToInt32(dr["consewbid"]),
                        cewbNo = dr["cEwbNo"].ToString(),
                        cewbDate = dr["cEWBDate"].ToString(),
                        vechileNo = dr["vehicleNo"].ToString(),
                        fromPlace = dr["fromPlace"].ToString()
                    });

                }

                #endregion

                #region "All Records"
                foreach (DataRow dr in ds2.Tables[0].Rows)
                {

                    CMasterList.Add(new EwayAttributes.ConsolidatedMasterList
                    {
                        docNo = dr["docNo"].ToString(),
                        docDate = dr["docDate"].ToString(),
                        ewbNo = dr["ewayBillNo"].ToString(),
                        ewbDate = dr["ewayBillDate"].ToString(),
                        generatedBy = dr["fromPlace"].ToString(),
                        toAddress = dr["toPlace"].ToString()
                    });

                }

                #endregion

                EwayViewModel CModel = new EwayViewModel();
                CModel.ConsolidatedMaster = CMaster;
                CModel.ConsolidatedMasterList = CMasterList;

                return View(CModel);
            }
            catch (Exception Ex)
            {
                TempData["ErrorMessage"] = Ex.Message;
            }
            return View();
        }

        public ActionResult CExportPdf()
        {

            return new ActionAsPdf("CPrint")
            {
                FileName = Session["CEwayBillNo"].ToString() + ".pdf"
            };

        }
        public ActionResult CPrint()
        {
            int conewbid = 0;
            EwaybillDataAccess.getConsewbid(CEWBNO, out conewbid);
            int cewbid = conewbid;
            DataSet ds1 = EwaybillDataAccess.getCEWAY(CEWBNO);
            DataSet ds2 = EwaybillDataAccess.getCEWAYList(cewbid);

            List<EwayAttributes.ConsolidatedMaster> CMaster = new List<EwayAttributes.ConsolidatedMaster>();
            List<EwayAttributes.ConsolidatedMasterList> CMasterList = new List<EwayAttributes.ConsolidatedMasterList>();

            #region "TOP1 Record" 

            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                CMaster.Add(new EwayAttributes.ConsolidatedMaster
                {
                    consewbid = Convert.ToInt32(dr["consewbid"]),
                    cewbNo = dr["cEwbNo"].ToString(),
                    cewbDate = dr["cEWBDate"].ToString(),
                    vechileNo = dr["vehicleNo"].ToString(),
                    fromPlace = dr["fromPlace"].ToString()
                });

            }

            #endregion

            #region "All Records"
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {

                CMasterList.Add(new EwayAttributes.ConsolidatedMasterList
                {
                    docNo = dr["docNo"].ToString(),
                    docDate = dr["docDate"].ToString(),
                    ewbNo = dr["ewayBillNo"].ToString(),
                    ewbDate = dr["ewayBillDate"].ToString(),
                    generatedBy = dr["fromPlace"].ToString(),
                    toAddress = dr["toPlace"].ToString()
                });

            }

            #endregion
            EwayViewModel CModel = new EwayViewModel();
            CModel.ConsolidatedMaster = CMaster;
            CModel.ConsolidatedMasterList = CMasterList;
            return PartialView("_CPrint", CModel);
        }

        public ActionResult CDownloadPdf(string cewbNo)
        {
            CDEWBNO = cewbNo;
            return new ActionAsPdf("CDownload")
            {
                FileName = cewbNo.ToString() + ".pdf"
            };

        }
        public ActionResult CDownload()
        {
            int conewbid = 0;
            EwaybillDataAccess.getConsewbid(CDEWBNO, out conewbid);
            int cewbid = conewbid;
            DataSet ds1 = EwaybillDataAccess.getCEWAY(CDEWBNO);
            DataSet ds2 = EwaybillDataAccess.getCEWAYList(cewbid);

            List<EwayAttributes.ConsolidatedMaster> CMaster = new List<EwayAttributes.ConsolidatedMaster>();
            List<EwayAttributes.ConsolidatedMasterList> CMasterList = new List<EwayAttributes.ConsolidatedMasterList>();

            #region "TOP1 Record" 

            foreach (DataRow dr in ds1.Tables[0].Rows)
            {
                CMaster.Add(new EwayAttributes.ConsolidatedMaster
                {
                    consewbid = Convert.ToInt32(dr["consewbid"]),
                    cewbNo = dr["cEwbNo"].ToString(),
                    cewbDate = dr["cEWBDate"].ToString(),
                    vechileNo = dr["vehicleNo"].ToString(),
                    fromPlace = dr["fromPlace"].ToString()
                });

            }

            #endregion

            #region "All Records"
            foreach (DataRow dr in ds2.Tables[0].Rows)
            {

                CMasterList.Add(new EwayAttributes.ConsolidatedMasterList
                {
                    docNo = dr["docNo"].ToString(),
                    docDate = dr["docDate"].ToString(),
                    ewbNo = dr["ewayBillNo"].ToString(),
                    ewbDate = dr["ewayBillDate"].ToString(),
                    generatedBy = dr["fromPlace"].ToString(),
                    toAddress = dr["toPlace"].ToString()
                });

            }

            #endregion

            EwayViewModel CModel = new EwayViewModel();
            CModel.ConsolidatedMaster = CMaster;
            CModel.ConsolidatedMasterList = CMasterList;
            return PartialView("_CPrint", CModel);
        }

        #endregion
    }
}