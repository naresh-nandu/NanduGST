using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.IO;
using System.Net;
using System.Text;
using System.Web.UI.WebControls;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Xml;
using System.Runtime.InteropServices;
using System.Reflection;
using GSTR2Download;
using SmartAdminMvc.Models.Common;

namespace SmartAdminMvc.Models.GSTR2
{
    public class GSTR2DownloadDataModel
    {
        static SqlConnection con = new SqlConnection(ConfigurationManager.AppSettings["ConnectionString"]);
        //string DownloadPath = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + @"\Downloads";
        private static string GSTR2_Download_Response = "";
        
        public static string SaveJsonDatatoDB(string DecryptedJsonData, string GSTINNo, string Period)
        {
            try
            {
                con.Open();
                SqlCommand cmd = new SqlCommand();
                dynamic root = JsonConvert.DeserializeObject(DecryptedJsonData, typeof(object));
                //var root = JsonConvert.DeserializeObject<Main>(jsondata);

                cmd.Parameters.Clear();                
                cmd.Parameters.Add("@gstin", SqlDbType.VarChar).Value = GSTINNo;
                cmd.Parameters.Add("@gstinid", SqlDbType.Int).Value = "30";
                cmd.Parameters.Add("@fp", SqlDbType.VarChar).Value = Period;
                string gstr2aid = Functions.InsertIntoTable("TBL_GSTR2_D", cmd, con);

                if (root.b2b != null)
                {
                    foreach (var B2B in root.b2b)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (B2B.ctin != null)
                            cmd.Parameters.Add("@ctin", SqlDbType.VarChar).Value = B2B.ctin;
                        if (B2B.cfs != null)
                            cmd.Parameters.Add("@cfs", SqlDbType.VarChar).Value = B2B.cfs;
                        string b2bid = Functions.InsertIntoTable("TBL_GSTR2_D_B2B", cmd, con);
                        foreach (var INV in B2B.inv)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@b2bid", SqlDbType.Int).Value = b2bid;
                            if (INV.flag != null)
                                cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = INV.flag;
                            if (INV.chksum != null)
                                cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = INV.chksum;
                            if (INV.inum != null)
                                cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = INV.inum;
                            if (INV.idt != null)
                                cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = INV.idt;
                            if (INV.val != null)
                                cmd.Parameters.Add("@val", SqlDbType.Decimal).Value = INV.val;
                            if (INV.pos != null)
                                cmd.Parameters.Add("@pos", SqlDbType.VarChar).Value = INV.pos;
                            if (INV.rchrg != null)
                                cmd.Parameters.Add("@rchrg", SqlDbType.VarChar).Value = INV.rchrg;
                            if (INV.updby != null)
                                cmd.Parameters.Add("@updby", SqlDbType.VarChar).Value = INV.updby;
                            string invid = Functions.InsertIntoTable("TBL_GSTR2_D_B2B_INV", cmd, con);
                            foreach (var ITMS in INV.itms)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@invid", SqlDbType.Int).Value = invid;
                                if (ITMS.num != null)
                                    cmd.Parameters.Add("@num", SqlDbType.NVarChar).Value = ITMS.num;
                                string itmsid = Functions.InsertIntoTable("TBL_GSTR2_D_B2B_INV_ITMS", cmd, con);

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@itmsid", SqlDbType.Int).Value = itmsid;
                                if (ITMS.itm_det.ty != null)
                                    cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = ITMS.itm_det.ty;
                                if (ITMS.itm_det.hsn_sc != null)
                                    cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = ITMS.itm_det.hsn_sc;
                                if (ITMS.itm_det.txval != null)
                                    cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = ITMS.itm_det.txval;
                                if (ITMS.itm_det.irt != null)
                                    cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = ITMS.itm_det.irt;
                                if (ITMS.itm_det.iamt != null)
                                    cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITMS.itm_det.iamt;
                                if (ITMS.itm_det.crt != null)
                                    cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = ITMS.itm_det.crt;
                                if (ITMS.itm_det.camt != null)
                                    cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITMS.itm_det.camt;
                                if (ITMS.itm_det.srt != null)
                                    cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = ITMS.itm_det.srt;
                                if (ITMS.itm_det.samt != null)
                                    cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITMS.itm_det.samt;
                                if (ITMS.itm_det.csrt != null)
                                    cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = ITMS.itm_det.csrt;
                                if (ITMS.itm_det.csamt != null)
                                    cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITMS.itm_det.csamt;
                                if (ITMS.itm_det.elg != null)
                                    cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = ITMS.itm_det.elg;
                                Functions.InsertIntoTable("TBL_GSTR2_D_B2B_INV_ITMS_DET", cmd, con);

                                if (ITMS.itc != null)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@itmsid", SqlDbType.Int).Value = itmsid;
                                    if (ITMS.itc.tx_i != null)
                                        cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = ITMS.itc.tx_i;
                                    if (ITMS.itc.tx_c != null)
                                        cmd.Parameters.Add("@tx_c", SqlDbType.Decimal).Value = ITMS.itc.tx_c;
                                    if (ITMS.itc.tx_s != null)
                                        cmd.Parameters.Add("@tx_s", SqlDbType.Decimal).Value = ITMS.itc.tx_s;
                                    if (ITMS.itc.tx_cs != null)
                                        cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = ITMS.itc.tx_cs;
                                    if (ITMS.itc.tc_i != null)
                                        cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = ITMS.itc.tc_i;
                                    if (ITMS.itc.tc_c != null)
                                        cmd.Parameters.Add("@tc_c", SqlDbType.Decimal).Value = ITMS.itc.tc_c;
                                    if (ITMS.itc.tc_s != null)
                                        cmd.Parameters.Add("@tc_s", SqlDbType.Decimal).Value = ITMS.itc.tc_s;
                                    //if (ITMS.itc.tc_cs != null)
                                    //    cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = ITMS.itc.tc_cs;
                                    Functions.InsertIntoTable("TBL_GSTR2_D_B2B_INV_ITMS_ITC", cmd, con);
                                }
                            }
                        }
                    }
                }
                if (root.b2ba != null)
                {
                    foreach (var B2BA in root.b2ba)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (B2BA.ctin != null)
                            cmd.Parameters.Add("@ctin", SqlDbType.VarChar).Value = B2BA.ctin;
                        if (B2BA.cfs != null)
                            cmd.Parameters.Add("@cfs", SqlDbType.VarChar).Value = B2BA.cfs;
                        string b2baid = Functions.InsertIntoTable("TBL_GSTR2_D_B2BA", cmd, con);
                        foreach (var INV in B2BA.inv)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@b2baid", SqlDbType.Int).Value = b2baid;
                            if (INV.flag != null)
                                cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = INV.flag;
                            if (INV.chksum != null)
                                cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = INV.chksum;
                            if (INV.oinum != null)
                                cmd.Parameters.Add("@oinum", SqlDbType.VarChar).Value = INV.oinum;
                            if (INV.oidt != null)
                                cmd.Parameters.Add("@oidt", SqlDbType.VarChar).Value = INV.oidt;
                            if (INV.inum != null)
                                cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = INV.inum;
                            if (INV.idt != null)
                                cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = INV.idt;
                            if (INV.val != null)
                                cmd.Parameters.Add("@val", SqlDbType.Decimal).Value = INV.val;
                            if (INV.pos != null)
                                cmd.Parameters.Add("@pos", SqlDbType.VarChar).Value = INV.pos;
                            if (INV.rchrg != null)
                                cmd.Parameters.Add("@rchrg", SqlDbType.VarChar).Value = INV.rchrg;
                            string invid = Functions.InsertIntoTable("TBL_GSTR2_D_B2BA_INV", cmd, con);
                            foreach (var ITMS in INV.itms)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@invid", SqlDbType.Int).Value = invid;
                                if (ITMS.num != null)
                                    cmd.Parameters.Add("@num", SqlDbType.NVarChar).Value = ITMS.num;
                                string itmsid = Functions.InsertIntoTable("TBL_GSTR2_D_B2BA_INV_ITMS", cmd, con);

                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@itmsid", SqlDbType.Int).Value = itmsid;
                                if (ITMS.itm_det.ty != null)
                                    cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = ITMS.itm_det.ty;
                                if (ITMS.itm_det.hsn_sc != null)
                                    cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = ITMS.itm_det.hsn_sc;
                                if (ITMS.itm_det.txval != null)
                                    cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = ITMS.itm_det.txval;
                                if (ITMS.itm_det.irt != null)
                                    cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = ITMS.itm_det.irt;
                                if (ITMS.itm_det.iamt != null)
                                    cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITMS.itm_det.iamt;
                                if (ITMS.itm_det.crt != null)
                                    cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = ITMS.itm_det.crt;
                                if (ITMS.itm_det.camt != null)
                                    cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITMS.itm_det.camt;
                                if (ITMS.itm_det.srt != null)
                                    cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = ITMS.itm_det.srt;
                                if (ITMS.itm_det.samt != null)
                                    cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITMS.itm_det.samt;
                                if (ITMS.itm_det.csrt != null)
                                    cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = ITMS.itm_det.csrt;
                                if (ITMS.itm_det.csamt != null)
                                    cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITMS.itm_det.csamt;
                                Functions.InsertIntoTable("TBL_GSTR2_D_B2BA_INV_ITMS_DET", cmd, con);

                                if (ITMS.itc != null)
                                {
                                    cmd.Parameters.Clear();
                                    cmd.Parameters.Add("@itmsid", SqlDbType.Int).Value = itmsid;
                                    if (ITMS.itc.tx_i != null)
                                        cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = ITMS.itc.tx_i;
                                    if (ITMS.itc.tx_c != null)
                                        cmd.Parameters.Add("@tx_c", SqlDbType.Decimal).Value = ITMS.itc.tx_c;
                                    if (ITMS.itc.tx_s != null)
                                        cmd.Parameters.Add("@tx_s", SqlDbType.Decimal).Value = ITMS.itc.tx_s;
                                    if (ITMS.itc.tx_cs != null)
                                        cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = ITMS.itc.tx_cs;
                                    if (ITMS.itc.tc_i != null)
                                        cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = ITMS.itc.tc_i;
                                    if (ITMS.itc.tc_c != null)
                                        cmd.Parameters.Add("@tc_c", SqlDbType.Decimal).Value = ITMS.itc.tc_c;
                                    if (ITMS.itc.tc_s != null)
                                        cmd.Parameters.Add("@tc_s", SqlDbType.Decimal).Value = ITMS.itc.tc_s;

                                    Functions.InsertIntoTable("TBL_GSTR2_D_B2BA_INV_ITMS_ITC", cmd, con);
                                }
                            }
                        }
                    }
                }
                if (root.cdn != null)
                {
                    foreach (var CDNR in root.cdn)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (CDNR.ctin != null)
                            cmd.Parameters.Add("@ctin", SqlDbType.VarChar).Value = CDNR.ctin;
                        if (CDNR.cfs != null)
                            cmd.Parameters.Add("@cfs", SqlDbType.VarChar).Value = CDNR.cfs;
                        string cdnrid = Functions.InsertIntoTable("TBL_GSTR2_D_CDN", cmd, con);
                        foreach (var NT in CDNR.nt)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@cdnid", SqlDbType.Int).Value = cdnrid;
                            if (NT.flag != null)
                                cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = NT.flag;
                            if (NT.chksum != null)
                                cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = NT.chksum;
                            if (NT.ntty != null)
                                cmd.Parameters.Add("@ntty", SqlDbType.VarChar).Value = NT.ntty;
                            if (NT.nt_num != null)
                                cmd.Parameters.Add("@nt_num", SqlDbType.VarChar).Value = NT.nt_num;
                            if (NT.nt_dt != null)
                                cmd.Parameters.Add("@nt_dt", SqlDbType.VarChar).Value = NT.nt_dt;
                            if (NT.rsn != null)
                                cmd.Parameters.Add("@rsn", SqlDbType.VarChar).Value = NT.rsn;
                            if (NT.inum != null)
                                cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = NT.inum;
                            if (NT.idt != null)
                                cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = NT.idt;
                            if (NT.rchrg != null)
                                cmd.Parameters.Add("@rchrg", SqlDbType.VarChar).Value = NT.rchrg;
                            if (NT.val != null)
                                cmd.Parameters.Add("@val", SqlDbType.Decimal).Value = NT.val;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.crt != null)
                                cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = NT.crt;
                            if (NT.camt != null)
                                cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = NT.camt;
                            if (NT.srt != null)
                                cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = NT.srt;
                            if (NT.samt != null)
                                cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = NT.samt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.updby != null)
                                cmd.Parameters.Add("@updby", SqlDbType.VarChar).Value = NT.updby;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            string ntid = Functions.InsertIntoTable("TBL_GSTR2_D_CDN_CDN", cmd, con);

                            if (NT.itc != null)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@cdnntid", SqlDbType.Int).Value = ntid;
                                if (NT.itc.tx_i != null)
                                    cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.itc.tx_i;
                                if (NT.itc.tx_c != null)
                                    cmd.Parameters.Add("@tx_c", SqlDbType.Decimal).Value = NT.itc.tx_c;
                                if (NT.itc.tx_s != null)
                                    cmd.Parameters.Add("@tx_s", SqlDbType.Decimal).Value = NT.itc.tx_s;
                                if (NT.itc.tx_cs != null)
                                    cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.itc.tx_cs;
                                if (NT.itc.tc_i != null)
                                    cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.itc.tc_i;
                                if (NT.itc.tc_c != null)
                                    cmd.Parameters.Add("@tc_c", SqlDbType.Decimal).Value = NT.itc.tc_c;
                                if (NT.itc.tc_s != null)
                                    cmd.Parameters.Add("@tc_s", SqlDbType.Decimal).Value = NT.itc.tc_s;
                                if (NT.itc.tc_cs != null)
                                    cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.itc.tc_cs;
                                Functions.InsertIntoTable("TBL_GSTR2_D_CDN_CDN_ITC", cmd, con);
                            }
                        }
                    }
                }
                if (root.cdna != null)
                {
                    foreach (var CDNA in root.cdna)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (CDNA.ctin != null)
                            cmd.Parameters.Add("@ctin", SqlDbType.VarChar).Value = CDNA.ctin;
                        if (CDNA.cfs != null)
                            cmd.Parameters.Add("@cfs", SqlDbType.VarChar).Value = CDNA.cfs;
                        string cdnraid = Functions.InsertIntoTable("TBL_GSTR2_D_CDNA", cmd, con);
                        foreach (var NT in CDNA.nt)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@cdnaid", SqlDbType.Int).Value = cdnraid;
                            if (NT.flag != null)
                                cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = NT.flag;
                            if (NT.chksum != null)
                                cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = NT.chksum;
                            if (NT.ntty != null)
                                cmd.Parameters.Add("@ntty", SqlDbType.VarChar).Value = NT.ntty;
                            if (NT.rsn != null)
                                cmd.Parameters.Add("@rsn", SqlDbType.VarChar).Value = NT.rsn;
                            if (NT.ont_num != null)
                                cmd.Parameters.Add("@ont_num", SqlDbType.VarChar).Value = NT.ont_num;
                            if (NT.ont_dt != null)
                                cmd.Parameters.Add("@ont_dt", SqlDbType.VarChar).Value = NT.ont_dt;
                            if (NT.nt_num != null)
                                cmd.Parameters.Add("@nt_num", SqlDbType.VarChar).Value = NT.nt_num;
                            if (NT.nt_dt != null)
                                cmd.Parameters.Add("@nt_dt", SqlDbType.VarChar).Value = NT.nt_dt;
                            if (NT.inum != null)
                                cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = NT.inum;
                            if (NT.idt != null)
                                cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = NT.idt;
                            if (NT.rchrg != null)
                                cmd.Parameters.Add("@rchrg", SqlDbType.VarChar).Value = NT.rchrg;
                            if (NT.val != null)
                                cmd.Parameters.Add("@val", SqlDbType.Decimal).Value = NT.val;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.crt != null)
                                cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = NT.crt;
                            if (NT.camt != null)
                                cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = NT.camt;
                            if (NT.srt != null)
                                cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = NT.srt;
                            if (NT.samt != null)
                                cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = NT.samt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            if (NT.updby != null)
                                cmd.Parameters.Add("@updby", SqlDbType.VarChar).Value = NT.updby;
                            string ntid = Functions.InsertIntoTable("TBL_GSTR2_D_CDNA_CDN", cmd, con);

                            if (NT.itc != null)
                            {
                                cmd.Parameters.Clear();
                                cmd.Parameters.Add("@cdnantid", SqlDbType.Int).Value = ntid;
                                if (NT.itc.tx_i != null)
                                    cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.itc.tx_i;
                                if (NT.itc.tx_c != null)
                                    cmd.Parameters.Add("@tx_c", SqlDbType.Decimal).Value = NT.itc.tx_c;
                                if (NT.itc.tx_s != null)
                                    cmd.Parameters.Add("@tx_s", SqlDbType.Decimal).Value = NT.itc.tx_s;
                                if (NT.itc.tx_cs != null)
                                    cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.itc.tx_cs;
                                if (NT.itc.tc_i != null)
                                    cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.itc.tc_i;
                                if (NT.itc.tc_c != null)
                                    cmd.Parameters.Add("@tc_c", SqlDbType.Decimal).Value = NT.itc.tc_c;
                                if (NT.itc.tc_s != null)
                                    cmd.Parameters.Add("@tc_s", SqlDbType.Decimal).Value = NT.itc.tc_s;
                                if (NT.itc.tc_cs != null)
                                    cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.itc.tc_cs;
                                Functions.InsertIntoTable("TBL_GSTR2_D_CDNA_CDN_ITC", cmd, con);
                            }
                        }
                    }
                }
                //if (root.hsnsum != null)
                //{
                //    foreach (var HSN in root.hsnsum)
                //    {
                //        cmd.Parameters.Clear();
                //        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                //        if (HSN.num != null)
                //            cmd.Parameters.Add("@num", SqlDbType.Int).Value = HSN.num;
                //        if (HSN.ty != null)
                //            cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = HSN.ty;
                //        if (HSN.hsn_sc != null)
                //            cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = HSN.hsn_sc;
                //        if (HSN.desc != null)
                //            cmd.Parameters.Add("@descs", SqlDbType.VarChar).Value = HSN.desc;
                //        if (HSN.uqc != null)
                //            cmd.Parameters.Add("@uqc", SqlDbType.VarChar).Value = HSN.uqc;
                //        if (HSN.qty != null)
                //            cmd.Parameters.Add("@qty", SqlDbType.Decimal).Value = HSN.qty;
                //        if (HSN.sup_ty != null)
                //            cmd.Parameters.Add("@sup_ty", SqlDbType.VarChar).Value = HSN.sup_ty;
                //        if (HSN.txval != null)
                //            cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = HSN.txval;
                //        if (HSN.irt != null)
                //            cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = HSN.irt;
                //        if (HSN.iamt != null)
                //            cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = HSN.iamt;
                //        if (HSN.crt != null)
                //            cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = HSN.crt;
                //        if (HSN.camt != null)
                //            cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = HSN.camt;
                //        if (HSN.srt != null)
                //            cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = HSN.srt;
                //        if (HSN.samt != null)
                //            cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = HSN.samt;
                //        if (HSN.csrt != null)
                //            cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = HSN.csrt;
                //        if (HSN.csamt != null)
                //            cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = HSN.csamt;
                //        string hsnid = Functions.InsertIntoTable("TBL_GSTR2_D_HSNSUM", cmd, con); // HSN SUB NEED TO ADD

                //    }
                //}
                if (root.imp_g != null)
                {
                    foreach (var IMPG in root.imp_g)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (IMPG.boe_num != null)
                            cmd.Parameters.Add("@boe_num", SqlDbType.VarChar).Value = IMPG.boe_num;
                        if (IMPG.boe_dt != null)
                            cmd.Parameters.Add("@boe_dt", SqlDbType.VarChar).Value = IMPG.boe_dt;
                        if (IMPG.boe_val != null)
                            cmd.Parameters.Add("@boe_val", SqlDbType.Decimal).Value = IMPG.boe_val;
                        if (IMPG.flag != null)
                            cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = IMPG.flag;
                        if (IMPG.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = IMPG.chksum;
                        string impgid = Functions.InsertIntoTable("TBL_GSTR2_D_IMPG", cmd, con);
                        foreach (var NT in IMPG.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@impgid", SqlDbType.Int).Value = impgid;
                            if (NT.num != null)
                                cmd.Parameters.Add("@num", SqlDbType.VarChar).Value = NT.num;
                            if (NT.hsn_sc != null)
                                cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = NT.hsn_sc;
                            if (NT.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.VarChar).Value = NT.txval;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            if (NT.tx_i != null)
                                cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.tx_i;
                            if (NT.tx_cs != null)
                                cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.tx_cs;
                            if (NT.tc_i != null)
                                cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.tc_i;
                            if (NT.tc_cs != null)
                                cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.tc_cs;
                            Functions.InsertIntoTable("TBL_GSTR2_D_IMPG_ITMS", cmd, con);
                        }
                    }
                }
                if (root.imp_ga != null)
                {
                    foreach (var IMPGA in root.imp_ga)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (IMPGA.flag != null)
                            cmd.Parameters.Add("@flag", SqlDbType.VarChar).Value = IMPGA.flag;
                        if (IMPGA.boe_num != null)
                            cmd.Parameters.Add("@boe_num", SqlDbType.VarChar).Value = IMPGA.boe_num;
                        if (IMPGA.boe_dt != null)
                            cmd.Parameters.Add("@boe_dt", SqlDbType.VarChar).Value = IMPGA.boe_dt;
                        if (IMPGA.boe_val != null)
                            cmd.Parameters.Add("@boe_val", SqlDbType.Decimal).Value = IMPGA.boe_val;
                        if (IMPGA.port_code != null)
                            cmd.Parameters.Add("@port_code", SqlDbType.VarChar).Value = IMPGA.port_code;
                        if (IMPGA.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = IMPGA.chksum;
                        if (IMPGA.oboe_num != null)
                            cmd.Parameters.Add("@oboe_num", SqlDbType.VarChar).Value = IMPGA.oboe_num;
                        if (IMPGA.oboe_dt != null)
                            cmd.Parameters.Add("@oboe_dt", SqlDbType.VarChar).Value = IMPGA.oboe_dt;
                        string impgaid = Functions.InsertIntoTable("TBL_GSTR2_D_IMPGA", cmd, con);
                        foreach (var NT in IMPGA.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@impgaid", SqlDbType.Int).Value = impgaid;
                            if (NT.num != null)
                                cmd.Parameters.Add("@num", SqlDbType.VarChar).Value = NT.num;
                            if (NT.hsn_sc != null)
                                cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = NT.hsn_sc;
                            if (NT.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.VarChar).Value = NT.txval;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            if (NT.tx_i != null)
                                cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.tx_i;
                            if (NT.tx_cs != null)
                                cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.tx_cs;
                            if (NT.tc_i != null)
                                cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.tc_i;
                            if (NT.tc_cs != null)
                                cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.tc_cs;
                            Functions.InsertIntoTable("TBL_GSTR2_D_IMPGA_ITMS", cmd, con);
                        }
                    }
                }
                if (root.imp_s != null)
                {
                    foreach (var IMPS in root.imp_s)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (IMPS.i_num != null)
                            cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = IMPS.i_num;
                        if (IMPS.i_dt != null)
                            cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = IMPS.i_dt;
                        if (IMPS.i_val != null)
                            cmd.Parameters.Add("@ival", SqlDbType.Decimal).Value = IMPS.i_val;
                        if (IMPS.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = IMPS.chksum;
                        string impsid = Functions.InsertIntoTable("TBL_GSTR2_D_IMPS", cmd, con);
                        foreach (var NT in IMPS.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@impsid", SqlDbType.Int).Value = impsid;
                            if (NT.num != null)
                                cmd.Parameters.Add("@num", SqlDbType.VarChar).Value = NT.num;
                            if (NT.sac != null)
                                cmd.Parameters.Add("@sac", SqlDbType.VarChar).Value = NT.sac;
                            if (NT.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.VarChar).Value = NT.txval;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            if (NT.tx_i != null)
                                cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.tx_i;
                            if (NT.tx_cs != null)
                                cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.tx_cs;
                            if (NT.tc_i != null)
                                cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.tc_i;
                            if (NT.tc_cs != null)
                                cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.tc_cs;
                            Functions.InsertIntoTable("TBL_GSTR2_D_IMPS_ITMS", cmd, con);
                        }
                    }
                }
                if (root.imp_sa != null)
                {
                    foreach (var IMPSA in root.imp_sa)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (IMPSA.i_num != null)
                            cmd.Parameters.Add("@inum", SqlDbType.VarChar).Value = IMPSA.inum;
                        if (IMPSA.i_dt != null)
                            cmd.Parameters.Add("@idt", SqlDbType.VarChar).Value = IMPSA.idt;
                        if (IMPSA.i_val != null)
                            cmd.Parameters.Add("@ival", SqlDbType.Decimal).Value = IMPSA.ival;
                        if (IMPSA.oi_num != null)
                            cmd.Parameters.Add("@oinum", SqlDbType.VarChar).Value = IMPSA.oinum;
                        if (IMPSA.oi_dt != null)
                            cmd.Parameters.Add("@oi_dt", SqlDbType.VarChar).Value = IMPSA.oidt;
                        if (IMPSA.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = IMPSA.chksum;
                        string impgaid = Functions.InsertIntoTable("TBL_GSTR2_D_IMPSA", cmd, con);
                        foreach (var NT in IMPSA.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@impsaid", SqlDbType.Int).Value = impgaid;
                            if (NT.num != null)
                                cmd.Parameters.Add("@num", SqlDbType.VarChar).Value = NT.num;
                            if (NT.sac != null)
                                cmd.Parameters.Add("@sac", SqlDbType.VarChar).Value = NT.sac;
                            if (NT.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.VarChar).Value = NT.txval;
                            if (NT.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = NT.irt;
                            if (NT.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = NT.iamt;
                            if (NT.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = NT.csrt;
                            if (NT.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = NT.csamt;
                            if (NT.elg != null)
                                cmd.Parameters.Add("@elg", SqlDbType.VarChar).Value = NT.elg;
                            if (NT.tx_i != null)
                                cmd.Parameters.Add("@tx_i", SqlDbType.Decimal).Value = NT.tx_i;
                            if (NT.tx_cs != null)
                                cmd.Parameters.Add("@tx_cs", SqlDbType.Decimal).Value = NT.tx_cs;
                            if (NT.tc_i != null)
                                cmd.Parameters.Add("@tc_i", SqlDbType.Decimal).Value = NT.tc_i;
                            if (NT.tc_cs != null)
                                cmd.Parameters.Add("@tc_cs", SqlDbType.Decimal).Value = NT.tc_cs;
                            Functions.InsertIntoTable("TBL_GSTR2_D_IMPSA_ITMS", cmd, con);
                        }
                    }
                }
                if (root.isd != null)
                {
                    foreach (var ITCRVSL in root.isd)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (ITCRVSL.i_num != null)
                            cmd.Parameters.Add("@i_num", SqlDbType.VarChar).Value = ITCRVSL.typ;
                        if (ITCRVSL.i_dt != null)
                            cmd.Parameters.Add("@i_dt", SqlDbType.VarChar).Value = ITCRVSL.stin;
                        if (ITCRVSL.ig_cr != null)
                            cmd.Parameters.Add("@ig_cr", SqlDbType.Decimal).Value = ITCRVSL.ig_cr;
                        if (ITCRVSL.cg_cr != null)
                            cmd.Parameters.Add("@cg_cr", SqlDbType.Decimal).Value = ITCRVSL.cg_cr;
                        if (ITCRVSL.sg_cr != null)
                            cmd.Parameters.Add("@sg_cr", SqlDbType.Decimal).Value = ITCRVSL.sg_cr;
                        if (ITCRVSL.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = ITCRVSL.chksum;

                        string invupdid = Functions.InsertIntoTable("TBL_GSTR2_D_ISD", cmd, con);

                    }
                }
                //if (root.nil_supplies != null)
                //{
                //    foreach (var NIL in root.nil_supplies)
                //    {
                //        cmd.Parameters.Clear();
                //        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                //        if (NIL.sply_ty != null)
                //            cmd.Parameters.Add("@SPLY_TY", SqlDbType.VarChar).Value = NIL.sply_ty;
                //        string nilid = Functions.InsertIntoTable("TBL_GSTR2_D_NIL", cmd, con);
                //        foreach (var G in NIL.nil_data)
                //        {
                //            cmd.Parameters.Clear();
                //            cmd.Parameters.Add("@nilid", SqlDbType.Int).Value = nilid;
                //            if (G.ty != null)
                //                cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = G.ty;
                //            if (G.hsn_sc != null)
                //                cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = G.hsn_sc;
                //            if (G.cpddr != null)
                //                cmd.Parameters.Add("@cpddr", SqlDbType.VarChar).Value = G.cpddr;
                //            if (G.uredr != null)
                //                cmd.Parameters.Add("@uredr", SqlDbType.VarChar).Value = G.uredr;
                //            if (G.exptdsply != null)
                //                cmd.Parameters.Add("@exptdsply", SqlDbType.VarChar).Value = G.exptdsply;
                //            if (G.ngsply != null)
                //                cmd.Parameters.Add("@ngsply", SqlDbType.VarChar).Value = G.ngsply;
                //            if (G.nilsply != null)
                //                cmd.Parameters.Add("@nilsply", SqlDbType.VarChar).Value = G.nilsply;
                //            string gid = Functions.InsertIntoTable("TBL_GSTR2_D_NIL_NIL", cmd, con);
                //        }
                //    }
                //}
                if (root.itc != null)
                {
                    foreach (var ITC in root.itc)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (ITC.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = ITC.chksum;
                        if (ITC.typ != null)
                            cmd.Parameters.Add("@typ", SqlDbType.VarChar).Value = ITC.typ;
                        if (ITC.stin != null)
                            cmd.Parameters.Add("@stin", SqlDbType.VarChar).Value = ITC.stin;
                        if (ITC.inv_doc_num != null)
                            cmd.Parameters.Add("@inv_doc_num", SqlDbType.VarChar).Value = ITC.inv_doc_num;
                        if (ITC.inv_doc_dt != null)
                            cmd.Parameters.Add("@inv_doc_dt", SqlDbType.VarChar).Value = ITC.inv_doc_dt;
                        if (ITC.des != null)
                            cmd.Parameters.Add("@o_ig", SqlDbType.VarChar).Value = ITC.des;
                        if (ITC.iamt != null)
                            cmd.Parameters.Add("@n_ig", SqlDbType.Decimal).Value = ITC.iamt;
                        if (ITC.camt != null)
                            cmd.Parameters.Add("@o_cg", SqlDbType.Decimal).Value = ITC.camt;
                        if (ITC.samt != null)
                            cmd.Parameters.Add("@n_cg", SqlDbType.Decimal).Value = ITC.samt;
                        if (ITC.csamt != null)
                            cmd.Parameters.Add("@o_sg", SqlDbType.Decimal).Value = ITC.csamt;
                        if (ITC.csamt != null)
                            cmd.Parameters.Add("@n_sg", SqlDbType.Decimal).Value = ITC.csamt;
                        if (ITC.csamt != null)
                            cmd.Parameters.Add("@o_cs", SqlDbType.Decimal).Value = ITC.csamt;
                        if (ITC.csamt != null)
                            cmd.Parameters.Add("@n_cs", SqlDbType.Decimal).Value = ITC.csamt;
                        string invupdid = Functions.InsertIntoTable("TBL_GSTR2_D_ITCRVSL", cmd, con);

                    }
                }
                if (root.TXLI != null)
                {
                    foreach (var TXI in root.TXLI)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (TXI.cpty != null)
                            cmd.Parameters.Add("@cpty", SqlDbType.VarChar).Value = TXI.cpty;
                        if (TXI.chksum != null)
                            cmd.Parameters.Add("@chksum", SqlDbType.VarChar).Value = TXI.chksum;
                        if (TXI.reg_type != null)
                            cmd.Parameters.Add("@reg_type", SqlDbType.VarChar).Value = TXI.reg_type;
                        if (TXI.state_cd != null)
                            cmd.Parameters.Add("@state_cd", SqlDbType.VarChar).Value = TXI.state_cd;
                        if (TXI.dnum != null)
                            cmd.Parameters.Add("@dnum", SqlDbType.VarChar).Value = TXI.dnum;
                        if (TXI.dt != null)
                            cmd.Parameters.Add("@dt", SqlDbType.VarChar).Value = TXI.dt;
                        string txiid = Functions.InsertIntoTable("TBL_GSTR2_D_TXLI", cmd, con);

                        foreach (var ITMS in TXI.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@txiid", SqlDbType.Int).Value = txiid;
                            if (ITMS.ty != null)
                                cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = ITMS.ty;
                            if (ITMS.hsn_sc != null)
                                cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = ITMS.hsn_sc;
                            if (ITMS.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = ITMS.txval;
                            if (ITMS.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = ITMS.irt;
                            if (ITMS.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITMS.iamt;
                            if (ITMS.crt != null)
                                cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = ITMS.crt;
                            if (ITMS.camt != null)
                                cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITMS.camt;
                            if (ITMS.srt != null)
                                cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = ITMS.srt;
                            if (ITMS.samt != null)
                                cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITMS.samt;
                            if (ITMS.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = ITMS.csrt;
                            if (ITMS.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITMS.csamt;
                            string hsnid = Functions.InsertIntoTable("TBL_GSTR2_D_TXLI_ITMS", cmd, con);

                        }
                    }
                }
                if (root.atxi != null)
                {
                    foreach (var TXIA in root.atxi)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (TXIA.cpty != null)
                            cmd.Parameters.Add("@cpty", SqlDbType.VarChar).Value = TXIA.cpty;
                        if (TXIA.reg_type != null)
                            cmd.Parameters.Add("@reg_type", SqlDbType.VarChar).Value = TXIA.reg_type;
                        if (TXIA.state_cd != null)
                            cmd.Parameters.Add("@state_cd", SqlDbType.VarChar).Value = TXIA.state_cd;
                        if (TXIA.dnum != null)
                            cmd.Parameters.Add("@dnum", SqlDbType.VarChar).Value = TXIA.dnum;
                        if (TXIA.dt != null)
                            cmd.Parameters.Add("@dt", SqlDbType.VarChar).Value = TXIA.dt;
                        if (TXIA.oreg_type != null)
                            cmd.Parameters.Add("@oreg_type", SqlDbType.VarChar).Value = TXIA.oreg_type;
                        if (TXIA.ocpty != null)
                            cmd.Parameters.Add("@ocpty", SqlDbType.VarChar).Value = TXIA.ocpty;
                        if (TXIA.odnum != null)
                            cmd.Parameters.Add("@odnum", SqlDbType.VarChar).Value = TXIA.odnum;
                        if (TXIA.otdt != null)
                            cmd.Parameters.Add("@otdt", SqlDbType.VarChar).Value = TXIA.otdt;
                        string txiaid = Functions.InsertIntoTable("TBL_GSTR2_D_TXLIA", cmd, con);

                        foreach (var ITMS in TXIA.itms)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@txliaid", SqlDbType.Int).Value = txiaid;
                            if (ITMS.ty != null)
                                cmd.Parameters.Add("@ty", SqlDbType.VarChar).Value = ITMS.ty;
                            if (ITMS.hsn_sc != null)
                                cmd.Parameters.Add("@hsn_sc", SqlDbType.VarChar).Value = ITMS.hsn_sc;
                            if (ITMS.txval != null)
                                cmd.Parameters.Add("@txval", SqlDbType.Decimal).Value = ITMS.txval;
                            if (ITMS.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = ITMS.irt;
                            if (ITMS.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITMS.iamt;
                            if (ITMS.crt != null)
                                cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = ITMS.crt;
                            if (ITMS.camt != null)
                                cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITMS.camt;
                            if (ITMS.srt != null)
                                cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = ITMS.srt;
                            if (ITMS.samt != null)
                                cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITMS.samt;
                            if (ITMS.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = ITMS.csrt;
                            if (ITMS.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITMS.csamt;
                            string hsnid = Functions.InsertIntoTable("TBL_GSTR2_D_TXLIA_ITMS", cmd, con);

                        }
                    }
                }
                //if (root.itc_rvsl != null)
                //{
                //    foreach (var ITCRVSL in root.itc_rvsl)
                //    {
                //        cmd.Parameters.Clear();
                //        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                //        if (ITCRVSL.typ != null)
                //            cmd.Parameters.Add("@typ", SqlDbType.VarChar).Value = ITCRVSL.typ;
                //        if (ITCRVSL.stin != null)
                //            cmd.Parameters.Add("@stin", SqlDbType.VarChar).Value = ITCRVSL.stin;
                //        if (ITCRVSL.inv_doc_num != null)
                //            cmd.Parameters.Add("@inv_doc_num", SqlDbType.VarChar).Value = ITCRVSL.inv_doc_num;
                //        if (ITCRVSL.inv_doc_dt != null)
                //            cmd.Parameters.Add("@inv_doc_dt", SqlDbType.VarChar).Value = ITCRVSL.inv_doc_dt;
                //        if (ITCRVSL.des != null)
                //            cmd.Parameters.Add("@des", SqlDbType.VarChar).Value = ITCRVSL.des;
                //        if (ITCRVSL.iamt != null)
                //            cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITCRVSL.iamt;
                //        if (ITCRVSL.camt != null)
                //            cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITCRVSL.camt;
                //        if (ITCRVSL.samt != null)
                //            cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITCRVSL.samt;
                //        if (ITCRVSL.csamt != null)
                //            cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITCRVSL.csamt;
                //        string invupdid = Functions.InsertIntoTable("TBL_GSTR2_D_ITCRVSL", cmd, con);

                //    }
                // }
                //if (root.itc_rvsla != null)
                //{
                //    foreach (var ITCRVSLA in root.itc_rvsla)
                //    {
                //        cmd.Parameters.Clear();
                //        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                //        if (ITCRVSLA.typ != null)
                //            cmd.Parameters.Add("@typ", SqlDbType.VarChar).Value = ITCRVSLA.typ;
                //        if (ITCRVSLA.stin != null)
                //            cmd.Parameters.Add("@stin", SqlDbType.VarChar).Value = ITCRVSLA.stin;
                //        if (ITCRVSLA.inv_doc_num != null)
                //            cmd.Parameters.Add("@inv_doc_num", SqlDbType.VarChar).Value = ITCRVSLA.inv_doc_num;
                //        if (ITCRVSLA.inv_doc_dt != null)
                //            cmd.Parameters.Add("@inv_doc_dt", SqlDbType.VarChar).Value = ITCRVSLA.inv_doc_dt;
                //        if (ITCRVSLA.des != null)
                //            cmd.Parameters.Add("@des", SqlDbType.VarChar).Value = ITCRVSLA.des;
                //        if (ITCRVSLA.iamt != null)
                //            cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITCRVSLA.iamt;
                //        if (ITCRVSLA.camt != null)
                //            cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITCRVSLA.camt;
                //        if (ITCRVSLA.samt != null)
                //            cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITCRVSLA.samt;
                //        if (ITCRVSLA.csamt != null)
                //            cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITCRVSLA.csamt;
                //        string invupdid = Functions.InsertIntoTable("TBL_GSTR2_D_ITCRVSL", cmd, con);

                //    }
                //}
                if (root.txpd != null)
                {
                    foreach (var INVPD in root.txpd)
                    {
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add("@gstr2aid", SqlDbType.Int).Value = gstr2aid;
                        if (INVPD.inum != null)
                            cmd.Parameters.Add("@i_num", SqlDbType.Int).Value = INVPD.i_num;
                        if (INVPD.idt != null)
                            cmd.Parameters.Add("@i_dt", SqlDbType.VarChar).Value = INVPD.i_dt;
                        string invpdid = Functions.InsertIntoTable("TBL_GSTR2_D_TXPD", cmd, con);

                        foreach (var ITMS in INVPD.doc)
                        {
                            cmd.Parameters.Clear();
                            cmd.Parameters.Add("@txpdid", SqlDbType.Int).Value = invpdid;
                            if (ITMS.doc_num != null)
                                cmd.Parameters.Add("@doc_num", SqlDbType.VarChar).Value = ITMS.doc_num;
                            if (ITMS.irt != null)
                                cmd.Parameters.Add("@irt", SqlDbType.Decimal).Value = ITMS.irt;
                            if (ITMS.iamt != null)
                                cmd.Parameters.Add("@iamt", SqlDbType.Decimal).Value = ITMS.iamt;
                            if (ITMS.crt != null)
                                cmd.Parameters.Add("@crt", SqlDbType.Decimal).Value = ITMS.crt;
                            if (ITMS.camt != null)
                                cmd.Parameters.Add("@camt", SqlDbType.Decimal).Value = ITMS.camt;
                            if (ITMS.srt != null)
                                cmd.Parameters.Add("@srt", SqlDbType.Decimal).Value = ITMS.srt;
                            if (ITMS.samt != null)
                                cmd.Parameters.Add("@samt", SqlDbType.Decimal).Value = ITMS.samt;
                            if (ITMS.csrt != null)
                                cmd.Parameters.Add("@csrt", SqlDbType.Decimal).Value = ITMS.csrt;
                            if (ITMS.csamt != null)
                                cmd.Parameters.Add("@csamt", SqlDbType.Decimal).Value = ITMS.csamt;
                            Functions.InsertIntoTable("TBL_GSTR2_D_TXPD_ITMS", cmd, con);
                        }
                    }
                }
                con.Close();
                return "";
            }
            catch (Exception ex)
            {
                con.Close();
                return ex.Message;
            }
        }
    }
}