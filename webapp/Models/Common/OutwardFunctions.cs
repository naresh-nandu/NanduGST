using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SmartAdminMvc.Models.Common
{
    public class OutwardFunctions
    {
        public static void Settings(int CustId, out string InvoiceSetting, out string CustomerCTINSetting, out string SupplierCTINSetting, out string HSNCTINSetting, 
            out string CreditNoteSetting, out string GSTR3BSetting, out string TaxValSettings, out string EwaytoGSTR1s, out string GSTR1toEways, out string EwayPrints, 
            out string EwbEmail, out string Location, out string MakerCheckerApprover, out string AutoGenInvNo, out string ReconSetting,out string InvFormat, 
            out string GenerateGSTR1HSN, out string InvoicePrintLogo, out string Rec_Oth_Fields, out string AutoGenInwardInvNo, out string TDS)
        {
            string LstInvoiceSetting = null;
            string LstCustomerCTINSetting = null;
            string LstSupplierCTINSetting = null;
            string LstHSNCTINSetting = null;
            string LstCreditNoteSetting = null;
            string LstGSTR3BSetting = null;
            string LstInvValSetting = null;
            string LstEwaytoGSTR1 = null;
            string LstGSTR1toEway = null;
            string LstEwayPrint = null;
            string LstEwbEmail = null;
            string LstLocation = null;
            string LstMakerCheckerApprover = null;
            string LstAutoGenInvNo = null;
            string LstReconSetting = null;
            string LstInvFormat = null;
            string LstGenerateGSTR1HSN = null;
            string LstInvoicePrintLogo = null;
            string LstReconOtherFields = null;
            string LstAutoGenInwardInvNo = null;
            string LstTDS = null;

            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                using (SqlCommand sqlcmd = new SqlCommand())
                {
                    sqlcmd.Connection = conn;
                    sqlcmd.CommandText = "Select * from TBL_Cust_Settings where CustId = @CustId";
                    sqlcmd.Parameters.AddWithValue("@CustId", CustId);
                    using (SqlDataAdapter adt1 = new SqlDataAdapter(sqlcmd))
                    {
                        DataTable dt1 = new DataTable();
                        adt1.Fill(dt1);
                        if (dt1.Rows.Count > 0)
                        {
                            LstInvoiceSetting = dt1.Rows[0]["InvoicePrintRequired"].ToString();
                            LstCustomerCTINSetting = dt1.Rows[0]["CtinValdnCustMgmtReqd"].ToString();
                            LstSupplierCTINSetting = dt1.Rows[0]["CtinValdnSupMgmtReqd"].ToString();
                            LstHSNCTINSetting = dt1.Rows[0]["HsnValdnHsnMstrReqd"].ToString();
                            LstCreditNoteSetting = dt1.Rows[0]["CdnValdnOrigInum"].ToString();
                            LstGSTR3BSetting = dt1.Rows[0]["GSTR3BAutoPopulate"].ToString();
                            LstInvValSetting = dt1.Rows[0]["TaxValCalnReqd"].ToString();
                            LstEwaytoGSTR1 = dt1.Rows[0]["Eway_To_GSTR1"].ToString();
                            LstGSTR1toEway = dt1.Rows[0]["GSTR1_to_Eway"].ToString();
                            LstEwayPrint = dt1.Rows[0]["EwayPrint"].ToString();
                            LstEwbEmail = dt1.Rows[0]["EwbEmailReqd"].ToString();
                            LstLocation = dt1.Rows[0]["LocationReqd"].ToString();
                            LstMakerCheckerApprover = dt1.Rows[0]["MakerCheckerApproverReqd"].ToString();
                            LstAutoGenInvNo = dt1.Rows[0]["AutoGenInvNoSettingReqd"].ToString();
                            LstReconSetting = dt1.Rows[0]["AdjustAmount"].ToString();
                            LstInvFormat= dt1.Rows[0]["InvoiceFormat"].ToString();
                            LstGenerateGSTR1HSN = dt1.Rows[0]["GenerateGSTR1HSNReqd"].ToString();
                            LstInvoicePrintLogo = dt1.Rows[0]["LogoPath"].ToString();
                            LstReconOtherFields = dt1.Rows[0]["Recon_Accept_OtherFields"].ToString();
                            LstAutoGenInwardInvNo = dt1.Rows[0]["AutoGenInwardInvNoSettingReqd"].ToString();
                            LstTDS = dt1.Rows[0]["TDSSettingReqd"].ToString();
                        }
                    }
                }
            }
            InvoiceSetting = LstInvoiceSetting;
            CustomerCTINSetting = LstCustomerCTINSetting;
            SupplierCTINSetting = LstSupplierCTINSetting;
            HSNCTINSetting = LstHSNCTINSetting;
            CreditNoteSetting = LstCreditNoteSetting;
            GSTR3BSetting = LstGSTR3BSetting;
            TaxValSettings = LstInvValSetting;
            EwaytoGSTR1s = LstEwaytoGSTR1;
            GSTR1toEways = LstGSTR1toEway;
            EwayPrints = LstEwayPrint;
            EwbEmail = LstEwbEmail;
            Location = LstLocation;
            MakerCheckerApprover = LstMakerCheckerApprover;
            AutoGenInvNo = LstAutoGenInvNo;
            ReconSetting = LstReconSetting;
            InvFormat = LstInvFormat;
            GenerateGSTR1HSN = LstGenerateGSTR1HSN;
            InvoicePrintLogo = LstInvoicePrintLogo;
            Rec_Oth_Fields = LstReconOtherFields;
            AutoGenInwardInvNo = LstAutoGenInwardInvNo;
            TDS = LstTDS;
        }
        public static SelectList GetExpType()
        {
            SelectList LstTypes = null;
            List<SelectListItem> Types = new List<SelectListItem>();
            Types.Add(new SelectListItem
            {
                Text = "WPAY",
                Value = "WPAY"
            });
            Types.Add(new SelectListItem
            {
                Text = "WOPAY",
                Value = "WOPAY"
            });

            LstTypes = new SelectList(Types, "Value", "Text");
            return LstTypes;
        }

        public static SelectList GetActionType()
        {
            SelectList LstTypes = null;
            List<SelectListItem> Types = new List<SelectListItem>();
            Types.Add(new SelectListItem
            {
                Text = "AT",
                Value = "AT"
            });
            Types.Add(new SelectListItem
            {
                Text = "TXP",
                Value = "TXP"
            });

            LstTypes = new SelectList(Types, "Value", "Text");
            return LstTypes;
        }

        public static SelectList GetNilSupplyType()
        {
            SelectList LstTypes = null;
            List<SelectListItem> Types = new List<SelectListItem>();
            Types.Add(new SelectListItem
            {
                Text = "INTRB2B",
                Value = "INTRB2B"
            });
            Types.Add(new SelectListItem
            {
                Text = "INTRB2C",
                Value = "INTRB2C"
            });
            Types.Add(new SelectListItem
            {
                Text = "INTRAB2B",
                Value = "INTRAB2B"
            });
            Types.Add(new SelectListItem
            {
                Text = "INTRAB2C",
                Value = "INTRAB2C"
            });

            LstTypes = new SelectList(Types, "Value", "Text");
            return LstTypes;
        }

        public static object GetDocType()
        {
            SelectList LstTypes = null;
            using (SqlConnection conn = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]))
            {
                conn.Open();
                SqlDataAdapter da = new SqlDataAdapter("select * from TBL_Nature_Of_Document", conn);
                DataTable dt = new DataTable();
                da.Fill(dt);
                
                if (dt.Rows.Count > 0)
                {
                    var listdoc = (from ob in dt.AsEnumerable()
                                   select new
                                   {
                                       Value = ob.Field<int>("DocId"),
                                       Text = ob.Field<string>("DocDesc")
                                   }
                               ).ToList();

                    LstTypes = new SelectList(listdoc, "Value", "Text");
                }
                conn.Close();
            }
            return LstTypes;

        }

        public static SelectList GetActionList()
        {
            SelectList LstTypes = null;
            List<SelectListItem> Types = new List<SelectListItem>();
            Types.Add(new SelectListItem
            {
                Text = "AT",
                Value = "AT"
            });
            Types.Add(new SelectListItem
            {
                Text = "TXP",
                Value = "TXP"
            });
            Types.Add(new SelectListItem
            {
                Text = "NIL",
                Value = "NIL"
            });
            Types.Add(new SelectListItem
            {
                Text = "DOCIssue",
                Value = "DOCIssue"
            });

            LstTypes = new SelectList(Types, "Value", "Text");
            return LstTypes;
        }

        public static SelectList Exist_GetActionList(string Action)
        {
            SelectList LstTypes = null;
            List<SelectListItem> Types = new List<SelectListItem>();
            Types.Add(new SelectListItem
            {
                Text = "AT",
                Value = "AT"
            });
            Types.Add(new SelectListItem
            {
                Text = "TXP",
                Value = "TXP"
            });
            Types.Add(new SelectListItem
            {
                Text = "NIL",
                Value = "NIL"
            });
            Types.Add(new SelectListItem
            {
                Text = "DOCIssue",
                Value = "DOCIssue"
            });

            LstTypes = new SelectList(Types, "Value", "Text", Action);
            return LstTypes;
        }

    }
}