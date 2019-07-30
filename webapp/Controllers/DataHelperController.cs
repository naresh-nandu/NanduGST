using SmartAdminMvc.Models.GSTR1;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WeP_BAL.GSTRUpload;

namespace SmartAdminMvc.Controllers
{
    public class DataHelperController : Controller
    {
        // DataTable purpose
        int dataCount = 0;
        List<Retrieve_GSTR1_Attributes> dList = null;

        #region "GSTR1 DATA RETRIEVE" 
        public ActionResult GetGSTR1Data(string strGSTINNo, string strFp, string strFlag, string strAction)
        {
            var draw = Request.Form.GetValues("draw").FirstOrDefault();
            var start = Request.Form.GetValues("start").FirstOrDefault();
            var length = Request.Form.GetValues("length").FirstOrDefault();
            var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
            var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
            var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

            //Paging Size (10,20,50,100)
            int pageSize = length != null ? Convert.ToInt32(length) : 0;
            int skip = start != null ? Convert.ToInt32(start) : 0;

            #region "GSTR1 SAVE DATA ASSIGNING LIST TO TEMPLIST"
            if (strFlag == "")
            {
                if (strAction == "B2B" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2B"]) == 0)
                    {
                        GSTR1DataModel.GetB2BCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2B"] = dataCount;
                        Session["dataList_G1B2B"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2B"]);
                        dList = Session["dataList_G1B2B"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "B2BA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2BA"]) == 0)
                    {
                        GSTR1DataModel.GetB2BACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2BA"] = dataCount;
                        Session["dataList_G1B2BA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2BA"]);
                        dList = Session["dataList_G1B2BA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "B2CL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2CL"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2CL"] = dataCount;
                        Session["dataList_G1B2CL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2CL"]);
                        dList = Session["dataList_G1B2CL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "B2CLA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2CLA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2CLA"] = dataCount;
                        Session["dataList_G1B2CLA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2CLA"]);
                        dList = Session["dataList_G1B2CLA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CS" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2CS"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2CS"] = dataCount;
                        Session["dataList_G1B2CS"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2CS"]);
                        dList = Session["dataList_G1B2CS"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "B2CSA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1B2CSA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1B2CSA"] = dataCount;
                        Session["dataList_G1B2CSA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1B2CSA"]);
                        dList = Session["dataList_G1B2CSA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1CDNR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1CDNR"] = dataCount;
                        Session["dataList_G1CDNR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1CDNR"]);
                        dList = Session["dataList_G1CDNR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNRA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1CDNRA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1CDNRA"] = dataCount;
                        Session["dataList_G1CDNRA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1CDNRA"]);
                        dList = Session["dataList_G1CDNRA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNUR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1CDNUR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1CDNUR"] = dataCount;
                        Session["dataList_G1CDNUR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1CDNUR"]);
                        dList = Session["dataList_G1CDNUR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNURA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1CDNURA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1CDNURA"] = dataCount;
                        Session["dataList_G1CDNURA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1CDNURA"]);
                        dList = Session["dataList_G1CDNURA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1EXP"]) == 0)
                    {
                        GSTR1DataModel.GetEXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1EXP"] = dataCount;
                        Session["dataList_G1EXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1EXP"]);
                        dList = Session["dataList_G1EXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1EXPA"]) == 0)
                    {
                        GSTR1DataModel.GetEXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1EXPA"] = dataCount;
                        Session["dataList_G1EXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1EXPA"]);
                        dList = Session["dataList_G1EXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "AT" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1AT"]) == 0)
                    {
                        GSTR1DataModel.GetATCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1AT"] = dataCount;
                        Session["dataList_G1AT"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1AT"]);
                        dList = Session["dataList_G1AT"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "ATA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1ATA"]) == 0)
                    {
                        GSTR1DataModel.GetATACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1ATA"] = dataCount;
                        Session["dataList_G1ATA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1ATA"]);
                        dList = Session["dataList_G1ATA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "TXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1TXP"]) == 0)
                    {
                        GSTR1DataModel.GetTXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1TXP"] = dataCount;
                        Session["dataList_G1TXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1TXP"]);
                        dList = Session["dataList_G1TXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "TXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1TXPA"]) == 0)
                    {
                        GSTR1DataModel.GetTXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1TXPA"] = dataCount;
                        Session["dataList_G1TXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1TXPA"]);
                        dList = Session["dataList_G1TXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "DOC" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1DOC"]) == 0)
                    {
                        GSTR1DataModel.GetDOCISSUECount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1DOC"] = dataCount;
                        Session["dataList_G1DOC"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1DOC"]);
                        dList = Session["dataList_G1DOC"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "NIL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1NIL"]) == 0)
                    {
                        GSTR1DataModel.GetNILCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1NIL"] = dataCount;
                        Session["dataList_G1NIL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1NIL"]);
                        dList = Session["dataList_G1NIL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "HSN" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1HSN"]) == 0)
                    {
                        GSTR1DataModel.GetHSNCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1HSN"] = dataCount;
                        Session["dataList_G1HSN"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1HSN"]);
                        dList = Session["dataList_G1HSN"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
            }
            #endregion

            #region "GSTR1 ERROR DATA ASSIGNING LIST TO TEMPLIST"
            if (strFlag == "1")
            {
                if (strAction == "B2B" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2B"]) == 0)
                    {
                        GSTR1DataModel.GetB2BCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2B"] = dataCount;
                        Session["dataList_G1SAEB2B"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2B"]);
                        dList = Session["dataList_G1SAEB2B"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2BA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2BA"]) == 0)
                    {
                        GSTR1DataModel.GetB2BACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2BA"] = dataCount;
                        Session["dataList_G1SAEB2BA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2BA"]);
                        dList = Session["dataList_G1SAEB2BA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2CL"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2CL"] = dataCount;
                        Session["dataList_G1SAEB2CL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2CL"]);
                        dList = Session["dataList_G1SAEB2CL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CLA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2CLA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2CLA"] = dataCount;
                        Session["dataList_G1SAEB2CLA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2CLA"]);
                        dList = Session["dataList_G1SAEB2CLA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CS" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2CS"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2CS"] = dataCount;
                        Session["dataList_G1SAEB2CS"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2CS"]);
                        dList = Session["dataList_G1SAEB2CS"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CSA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEB2CSA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEB2CSA"] = dataCount;
                        Session["dataList_G1SAEB2CSA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEB2CSA"]);
                        dList = Session["dataList_G1SAEB2CSA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAECDNR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAECDNR"] = dataCount;
                        Session["dataList_G1SAECDNR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAECDNR"]);
                        dList = Session["dataList_G1SAECDNR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNRA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAECDNRA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAECDNRA"] = dataCount;
                        Session["dataList_G1SAECDNRA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAECDNRA"]);
                        dList = Session["dataList_G1SAECDNRA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNUR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAECDNUR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAECDNUR"] = dataCount;
                        Session["dataList_G1SAECDNUR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAECDNUR"]);
                        dList = Session["dataList_G1SAECDNUR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNURA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAECDNURA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAECDNURA"] = dataCount;
                        Session["dataList_G1SAECDNURA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAECDNURA"]);
                        dList = Session["dataList_G1SAECDNURA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEEXP"]) == 0)
                    {
                        GSTR1DataModel.GetEXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEEXP"] = dataCount;
                        Session["dataList_G1SAEEXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEEXP"]);
                        dList = Session["dataList_G1SAEEXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEEXPA"]) == 0)
                    {
                        GSTR1DataModel.GetEXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEEXPA"] = dataCount;
                        Session["dataList_G1SAEEXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEEXPA"]);
                        dList = Session["dataList_G1SAEEXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "AT" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEAT"]) == 0)
                    {
                        GSTR1DataModel.GetATCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEAT"] = dataCount;
                        Session["dataList_G1SAEAT"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEAT"]);
                        dList = Session["dataList_G1SAEAT"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "ATA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEATA"]) == 0)
                    {
                        GSTR1DataModel.GetATACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEATA"] = dataCount;
                        Session["dataList_G1SAEATA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEATA"]);
                        dList = Session["dataList_G1SAEATA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "TXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAETXP"]) == 0)
                    {
                        GSTR1DataModel.GetTXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAETXP"] = dataCount;
                        Session["dataList_G1SAETXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAETXP"]);
                        dList = Session["dataList_G1SAETXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "TXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAETXP"]) == 0)
                    {
                        GSTR1DataModel.GetTXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAETXPA"] = dataCount;
                        Session["dataList_G1SAETXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAETXPA"]);
                        dList = Session["dataList_G1SAETXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "DOC" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEDOC"]) == 0)
                    {
                        GSTR1DataModel.GetDOCISSUECount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEDOC"] = dataCount;
                        Session["dataList_G1SAEDOC"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEDOC"]);
                        dList = Session["dataList_G1SAEDOC"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "NIL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAENIL"]) == 0)
                    {
                        GSTR1DataModel.GetNILCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAENIL"] = dataCount;
                        Session["dataList_G1SAENIL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAENIL"]);
                        dList = Session["dataList_G1SAENIL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "HSN" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAEHSN"]) == 0)
                    {
                        GSTR1DataModel.GetHSNCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAEHSN"] = dataCount;
                        Session["dataList_G1SAEHSN"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAEHSN"]);
                        dList = Session["dataList_G1SAEHSN"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
            }
            #endregion

            #region "GSTR1 UPLOADED DATA ASSIGNING LIST TO TEMPLIST"
            if (strFlag == "U")
            {
                if (strAction == "B2B" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2B"]) == 0)
                    {
                        GSTR1DataModel.GetB2BCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2B"] = dataCount;
                        Session["dataList_G1SAUB2B"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2B"]);
                        dList = Session["dataList_G1SAUB2B"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2BA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2BA"]) == 0)
                    {
                        GSTR1DataModel.GetB2BACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2BA"] = dataCount;
                        Session["dataList_G1SAUB2BA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2BA"]);
                        dList = Session["dataList_G1SAUB2BA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2CL"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2CL"] = dataCount;
                        Session["dataList_G1SAUB2CL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2CL"]);
                        dList = Session["dataList_G1SAUB2CL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CLA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2CLA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CLACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2CLA"] = dataCount;
                        Session["dataList_G1SAUB2CLA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2CLA"]);
                        dList = Session["dataList_G1SAUB2CLA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "B2CS" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2CS"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2CS"] = dataCount;
                        Session["dataList_G1SAUB2CS"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2CS"]);
                        dList = Session["dataList_G1SAUB2CS"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "B2CSA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUB2CSA"]) == 0)
                    {
                        GSTR1DataModel.GetB2CSACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUB2CSA"] = dataCount;
                        Session["dataList_G1SAUB2CSA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUB2CSA"]);
                        dList = Session["dataList_G1SAUB2CSA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }

                if (strAction == "CDNR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUCDNR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUCDNR"] = dataCount;
                        Session["dataList_G1SAUCDNR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUCDNR"]);
                        dList = Session["dataList_G1SAUCDNR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNRA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUCDNRA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNRACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUCDNRA"] = dataCount;
                        Session["dataList_G1SAUCDNRA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUCDNRA"]);
                        dList = Session["dataList_G1SAUCDNRA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNUR" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUCDNUR"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUCDNUR"] = dataCount;
                        Session["dataList_G1SAUCDNUR"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUCDNUR"]);
                        dList = Session["dataList_G1SAUCDNUR"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "CDNURA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUCDNURA"]) == 0)
                    {
                        GSTR1DataModel.GetCDNURACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUCDNURA"] = dataCount;
                        Session["dataList_G1SAUCDNURA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUCDNURA"]);
                        dList = Session["dataList_G1SAUCDNURA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUEXP"]) == 0)
                    {
                        GSTR1DataModel.GetEXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUEXP"] = dataCount;
                        Session["dataList_G1SAUEXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUEXP"]);
                        dList = Session["dataList_G1SAUEXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "EXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUEXPA"]) == 0)
                    {
                        GSTR1DataModel.GetEXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUEXPA"] = dataCount;
                        Session["dataList_G1SAUEXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUEXPA"]);
                        dList = Session["dataList_G1SAUEXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "AT" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUAT"]) == 0)
                    {
                        GSTR1DataModel.GetATCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUAT"] = dataCount;
                        Session["dataList_G1SAUAT"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUAT"]);
                        dList = Session["dataList_G1SAUAT"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "ATA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUATA"]) == 0)
                    {
                        GSTR1DataModel.GetATACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUATA"] = dataCount;
                        Session["dataList_G1SAUATA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUATA"]);
                        dList = Session["dataList_G1SAUATA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "TXP" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUTXP"]) == 0)
                    {
                        GSTR1DataModel.GetTXPCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUTXP"] = dataCount;
                        Session["dataList_G1SAUTXP"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUTXP"]);
                        dList = Session["dataList_G1SAUTXP"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "TXPA" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUTXPA"]) == 0)
                    {
                        GSTR1DataModel.GetTXPACount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUTXPA"] = dataCount;
                        Session["dataList_G1SAUTXPA"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUTXPA"]);
                        dList = Session["dataList_G1SAUTXPA"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "DOC" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUDOC"]) == 0)
                    {
                        GSTR1DataModel.GetDOCISSUECount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUDOC"] = dataCount;
                        Session["dataList_G1SAUDOC"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUDOC"]);
                        dList = Session["dataList_G1SAUDOC"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "NIL" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUNIL"]) == 0)
                    {
                        GSTR1DataModel.GetNILCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUNIL"] = dataCount;
                        Session["dataList_G1SAUNIL"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUNIL"]);
                        dList = Session["dataList_G1SAUNIL"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
                if (strAction == "HSN" || strAction == "ALL")
                {
                    if (Convert.ToInt32(Session["dataCount_G1SAUHSN"]) == 0)
                    {
                        GSTR1DataModel.GetHSNCount_And_List(strGSTINNo, strFp, strFlag, out dataCount, out dList);
                        Session["dataCount_G1SAUHSN"] = dataCount;
                        Session["dataList_G1SAUHSN"] = dList;
                    }
                    else
                    {
                        dataCount = Convert.ToInt32(Session["dataCount_G1SAUHSN"]);
                        dList = Session["dataList_G1SAUHSN"] as List<Retrieve_GSTR1_Attributes>;
                    }
                }
            }
            #endregion


            #region "APPLY SEARCH"
            if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
            {
                switch (strAction)
                {
                    case "B2BA":
                        dList = dList.Where(p =>
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "B2CLA":
                        dList = dList.Where(p =>
                        p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "B2CSA":
                        dList = dList.Where(p =>
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "EXPA":
                        dList = dList.Where(p =>
                        p.ex_tp.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.sbnum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sbdt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "CDNRA":
                        dList = dList.Where(p =>
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ont_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ont_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "CDNURA":
                        dList = dList.Where(p =>
                        p.ont_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ont_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "ATA":
                        dList = dList.Where(p =>
                        p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
                        p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "TXPA":
                        dList = dList.Where(p =>
                        p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
                        p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "B2B":
                        dList = dList.Where(p =>
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "B2CL":
                        dList = dList.Where(p =>
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "B2CS":
                        dList = dList.Where(p =>
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "EXP":
                        dList = dList.Where(p =>
                        p.ex_tp.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.sbnum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sbdt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "CDNR":
                        dList = dList.Where(p =>
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "CDNUR":
                        dList = dList.Where(p =>
                        p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "AT":
                        dList = dList.Where(p =>
                        p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "TXP":
                        dList = dList.Where(p =>
                        p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "NIL":
                        dList = dList.Where(p =>
                        p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.nil_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.expt_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.ngsup_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "HSNSUM":
                        dList = dList.Where(p =>
                        p.hsn_sc.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.descs.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.qty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.uqc.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    case "DOCISSUE":
                        dList = dList.Where(p =>
                        p.doc_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.froms.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.tos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.totnum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.cancel.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.net_issue.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                    default:
                        dList = dList.Where(p =>
                        p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.val.ToString().Contains(searchValue ?? string.Empty) ||
                        p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
                        p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
                        break;

                }

            }
            #endregion

            //Paging     
            var data = dList.Skip(skip).Take(pageSize).ToList();
            //Returning Json Data    
            return Json(new { draw = draw, recordsFiltered = dataCount, recordsTotal = dataCount, data = data });
        }
        #endregion

        //#region "GSTR1 DATA RETRIEVE" 
        //public ActionResult GetGSTR1Upload(string strAction)
        //{
        //    var draw = Request.Form.GetValues("draw").FirstOrDefault();
        //    var start = Request.Form.GetValues("start").FirstOrDefault();
        //    var length = Request.Form.GetValues("length").FirstOrDefault();
        //    var sortColumn = Request.Form.GetValues("columns[" + Request.Form.GetValues("order[0][column]").FirstOrDefault() + "][name]").FirstOrDefault();
        //    var sortColumnDir = Request.Form.GetValues("order[0][dir]").FirstOrDefault();
        //    var searchValue = Request.Form.GetValues("search[value]").FirstOrDefault();

        //    //Paging Size (10,20,50,100)
        //    int pageSize = length != null ? Convert.ToInt32(length) : 0;
        //    int skip = start != null ? Convert.ToInt32(start) : 0;

        //    #region "GSTR1 SAVE DATA ASSIGNING LIST TO TEMPLIST"

        //    if (strAction == "B2B")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2B"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2BCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1B2B"] = dataCount;
        //            Session["dataList_G1B2B"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2B"]);
        //            dList = Session["dataList_G1B2B"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "B2BA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2BA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2BACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1B2BA"] = dataCount;
        //            Session["dataList_G1B2BA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2BA"]);
        //            dList = Session["dataList_G1B2BA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "B2CL")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2CL"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2CLCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1B2CL"] = dataCount;
        //            Session["dataList_G1B2CL"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2CL"]);
        //            dList = Session["dataList_G1B2CL"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "B2CLA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2CLA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2CLACount_And_List(Session["CustRefNo"].ToString(),  out dataCount, out dList);
        //            Session["dataCount_G1B2CLA"] = dataCount;
        //            Session["dataList_G1B2CLA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2CLA"]);
        //            dList = Session["dataList_G1B2CLA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "B2CS")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2CS"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2CSCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1B2CS"] = dataCount;
        //            Session["dataList_G1B2CS"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2CS"]);
        //            dList = Session["dataList_G1B2CS"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "B2CSA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1B2CSA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetB2CSACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1B2CSA"] = dataCount;
        //            Session["dataList_G1B2CSA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1B2CSA"]);
        //            dList = Session["dataList_G1B2CSA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "CDNR")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1CDNR"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetCDNRCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1CDNR"] = dataCount;
        //            Session["dataList_G1CDNR"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1CDNR"]);
        //            dList = Session["dataList_G1CDNR"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "CDNRA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1CDNRA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetCDNRACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1CDNRA"] = dataCount;
        //            Session["dataList_G1CDNRA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1CDNRA"]);
        //            dList = Session["dataList_G1CDNRA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "CDNUR")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1CDNUR"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetCDNURCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1CDNUR"] = dataCount;
        //            Session["dataList_G1CDNUR"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1CDNUR"]);
        //            dList = Session["dataList_G1CDNUR"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    if (strAction == "CDNURA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1CDNURA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetCDNURACount_And_List(Session["CustRefNo"].ToString(),out dataCount, out dList);
        //            Session["dataCount_G1CDNURA"] = dataCount;
        //            Session["dataList_G1CDNURA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1CDNURA"]);
        //            dList = Session["dataList_G1CDNURA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "EXP")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1EXP"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetEXPCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1EXP"] = dataCount;
        //            Session["dataList_G1EXP"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1EXP"]);
        //            dList = Session["dataList_G1EXP"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "EXPA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1EXPA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetEXPACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1EXPA"] = dataCount;
        //            Session["dataList_G1EXPA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1EXPA"]);
        //            dList = Session["dataList_G1EXPA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "AT")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1AT"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetATCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1AT"] = dataCount;
        //            Session["dataList_G1AT"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1AT"]);
        //            dList = Session["dataList_G1AT"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "ATA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1ATA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetATACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1ATA"] = dataCount;
        //            Session["dataList_G1ATA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1ATA"]);
        //            dList = Session["dataList_G1ATA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "TXP")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1TXP"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetTXPCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1TXP"] = dataCount;
        //            Session["dataList_G1TXP"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1TXP"]);
        //            dList = Session["dataList_G1TXP"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "TXPA")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1TXPA"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetTXPACount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1TXPA"] = dataCount;
        //            Session["dataList_G1TXPA"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1TXPA"]);
        //            dList = Session["dataList_G1TXPA"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "DOC")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1DOC"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetDOCISSUECount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1DOC"] = dataCount;
        //            Session["dataList_G1DOC"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1DOC"]);
        //            dList = Session["dataList_G1DOC"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "NIL")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1NIL"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetNILCount_And_List(Session["CustRefNo"].ToString(), out dataCount, out dList);
        //            Session["dataCount_G1NIL"] = dataCount;
        //            Session["dataList_G1NIL"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1NIL"]);
        //            dList = Session["dataList_G1NIL"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }
        //    if (strAction == "HSN")
        //    {
        //        if (Convert.ToInt32(Session["dataCount_G1HSN"]) == 0)
        //        {
        //            GSTRUpload_BAL.GetHSNCount_And_List(Session["CustRefNo"].ToString(),out dataCount, out dList);
        //            Session["dataCount_G1HSN"] = dataCount;
        //            Session["dataList_G1HSN"] = dList;
        //        }
        //        else
        //        {
        //            dataCount = Convert.ToInt32(Session["dataCount_G1HSN"]);
        //            dList = Session["dataList_G1HSN"] as List<Retrieve_GSTR1_Attributes>;
        //        }
        //    }

        //    #endregion


        //    #region "APPLY SEARCH"
        //    if (!string.IsNullOrEmpty(searchValue) && !string.IsNullOrWhiteSpace(searchValue))
        //    {
        //        switch (strAction)
        //        {
        //            case "B2BA":
        //                dList = dList.Where(p =>
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "B2CLA":
        //                dList = dList.Where(p =>
        //                p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "B2CSA":
        //                dList = dList.Where(p =>
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "EXPA":
        //                dList = dList.Where(p =>
        //                p.ex_tp.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.oinum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.oidt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.sbnum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sbdt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "CDNRA":
        //                dList = dList.Where(p =>
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ont_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ont_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "CDNURA":
        //                dList = dList.Where(p =>
        //                p.ont_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ont_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "ATA":
        //                dList = dList.Where(p =>
        //                p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "TXPA":
        //                dList = dList.Where(p =>
        //                p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.diff_percent.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.omon.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "B2B":
        //                dList = dList.Where(p =>
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "B2CL":
        //                dList = dList.Where(p =>
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "B2CS":
        //                dList = dList.Where(p =>
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "EXP":
        //                dList = dList.Where(p =>
        //                p.ex_tp.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.sbnum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sbdt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "CDNR":
        //                dList = dList.Where(p =>
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "CDNUR":
        //                dList = dList.Where(p =>
        //                p.ntty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nt_dt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "AT":
        //                dList = dList.Where(p =>
        //                p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "TXP":
        //                dList = dList.Where(p =>
        //                p.pos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ad_amt.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "NIL":
        //                dList = dList.Where(p =>
        //                p.sply_ty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.nil_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.expt_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.ngsup_amt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "HSNSUM":
        //                dList = dList.Where(p =>
        //                p.hsn_sc.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.descs.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.qty.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.uqc.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            case "DOCISSUE":
        //                dList = dList.Where(p =>
        //                p.doc_num.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.froms.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.tos.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.totnum.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.cancel.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.net_issue.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //            default:
        //                dList = dList.Where(p =>
        //                p.ctin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.inum.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.idt.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.val.ToString().Contains(searchValue ?? string.Empty) ||
        //                p.pos.ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.etin.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.rt.ToString().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.txval.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.iamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.samt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.camt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty) ||
        //                p.csamt.ToString().ToLower().Contains(searchValue.ToLower() ?? string.Empty)).ToList();
        //                break;

        //        }

        //    }
        //    #endregion

        //    //Paging     
        //    var data = dList.Skip(skip).Take(pageSize).ToList();
        //    //Returning Json Data    
        //    return Json(new { draw = draw, recordsFiltered = dataCount, recordsTotal = dataCount, data = data });
        //}
        //#endregion

        #region "EWAYBILL & CONSOLIDATED EWAYBILL DATA RETRIEVE"

        #endregion

    }
}