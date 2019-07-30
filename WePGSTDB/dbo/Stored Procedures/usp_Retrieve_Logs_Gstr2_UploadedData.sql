

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Log file for GSTR2 Uploaded data
				
Written by  : Karthik

Date		Who			Decription 
11/04/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Retrieve_Logs_Gstr2_UploadedData 1,1,'072017' 
*/
CREATE PROCEDURE [usp_Retrieve_Logs_Gstr2_UploadedData]
@CustId int,
@UserId int,
@Period varchar(10)  
   
  
AS  
BEGIN  

					Select a.* Into #GstinIds
					From
					(Select gstinid as gstinid from tbl_cust_gstin where custid = @CustId 
					) a
							-- B2B 
							Select  t1.gstin as 'GSTIN No', 
									t2.ctin as 'CTIN No',
									t3.inum as 'Invoice No', 
									t3.val as 'Invoice Value', 
									t3.idt as 'Invoice Date',
									t3.pos as 'Placve Of Supply',
									t3.inv_typ as 'Invoice Type',
									t3.Rchrg as 'Reverse Charge',
									t3.flag as 'Flag',
									t3.chksum as 'Check Sum',
									t4.Num as 'Num',
									t5.Rt as 'Rate',
									t5.TxVal as 'Taxable Value',
									t5.Iamt as 'IGST Amount',
									t5.Camt as 'CGST Amount',
									t5.Samt as 'SGST Amount',
									t5.Csamt as 'CESS Amount',
									t6.tx_i	as 'ITC IGST',
									t6.tx_c as 'ITC CGST',
									t6.tx_s as 'ITC SGST',
									t6.tx_cs as 'ITC CESS',
									t6.elg as 'Eligibility'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- B2CL
							Select  t1.gstin as 'GSTIN No', 
									t3.inum as 'Invoice No', 
									t3.val as 'Invoice Value', 
									t3.idt as 'Invoice Date',
									t3.sply_ty as 'Supply Type',
									t3.pos as 'Place Of Supply',
									t3.flag as 'Flag',
									t3.chksum as 'Check Sum',
									t4.Num as 'Num',
									t5.Rt as 'Rate',
									t5.TxVal as 'Taxable Value',
									t5.Iamt as 'IGST Amount',
									t5.Camt as 'CGST Amount',
									t5.Samt as 'SGST Amount',
									t5.Csamt as 'CESS Amount',
									t6.tx_i	as 'ITC IGST',
									t6.tx_c as 'ITC CGST',
									t6.tx_s as 'ITC SGST',
									t6.tx_cs as 'ITC CESS',
									t6.elg as 'Eligibility'
							from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2BUR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2BUR_INV t3 On t3.b2burid = t2.b2burid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U' 

							-- IMPG							
							Select gstin as 'GSTIN',
								   is_sez as 'IS SEZ', 
								   stin as 'GSTIN/UID of the Supplier', 
								   boe_num as 'Bill of Entry Number', 
								   boe_dt as 'Bill of Entry Date', 
								   boe_val as 'Bill of Entry Value',
								   flag as 'Flag', 
								   chksum as 'Check Sum', 
								   port_code as 'Port Code',
								   num as 'Serial No',
								   rt as 'Rate',
								   txval as 'Taxable Value',
								   iamt as 'IGST Amount',
								   csamt as 'CGST Amount',
								   elg as 'Eligibility',
								   tx_i as 'ITC IGST',
								   tx_cs as 'ITC CESS'
							from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_IMPG t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_IMPG_ITMS t3 On t3.impgid = t2.impgid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U' 

							-- IMPS 
							Select inum as 'Invoice No',
								   idt as 'Invoice Date',
								   pos as 'Place Of Supply',
								   ival as 'Invoice Value',
								   flag as 'Flag',
								   chksum as 'Check Sum',
								   num as 'Serial No',
								   rt as 'Rate',
								   txval as 'Taxable Value',
								   iamt as 'IGST Amount',
								   csamt as 'CGST Amount',
								   elg as 'Eligibility',
								   tx_i as 'ITC IGST',
								   tx_cs as 'ITC CESS'
							from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_IMPS t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_IMPS_ITMS t3 On t3.impsid = t2.impsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U' 
							
							-- CDNR
							Select Gstin as 'GSTIN',
							Ctin as 'CTIN',
							Nt_Num as 'Note No',
							Nt_Dt as 'Note Date',
							Ntty as 'Note Type',
							Inum as 'Invoice No',
							Idt as 'Invoice Date',
							Rsn as 'Reason',
							P_gst as 'Pre GST',
							Val as 'Invoice Value',
							Flag as 'Flag',
							Num as 'Num',
							Rt as 'Rate',
							TxVal as 'Taxable Value',
							Iamt as 'IGST Amount',
							Camt as 'CGST Amount',
							Samt as 'SGST Amount',
							Csamt as 'CESS Amount',
							tx_i	as 'ITC IGST',
							tx_c as 'ITC CGST',
							tx_s as 'ITC SGST',
							tx_cs as 'ITC CESS',
							elg as 'Eligibility'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t6 On t6.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U' 
							
							-- CDNUR
							Select Gstin as 'GSTIN',
							Nt_Num as 'Note No',
							Nt_Dt as 'Note Date',
							Ntty as 'Note Type',
							Inum as 'Invoice No',
							Idt as 'Invoice Date',
							Rsn as 'Reason',
							P_gst as 'Pre GST',
							Val as 'Invoice Value',
							Flag as 'Flag',
							Num as 'Num',
							Rt as 'Rate',
							TxVal as 'Taxable Value',
							Iamt as 'IGST Amount',
							Camt as 'CGST Amount',
							Samt as 'SGST Amount',
							Csamt as 'CESS Amount',
							tx_i	as 'ITC IGST',
							tx_c as 'ITC CGST',
							tx_s as 'ITC SGST',
							tx_cs as 'ITC CESS',
							elg as 'Eligibility'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_CDNUR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR2_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
							Inner Join TBL_GSTR2_CDNUR_ITMS_ITC t5 On t5.itmsid = t3.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'  
							
							-- HSN
							Select  gstin as 'GSTIN No', 
									num as 'Num', 
									hsn_sc as 'HSN Code', 
									[desc] as 'Description',
									uqc as 'Quantity Unit',  
									qty as 'Quantity',
									val as 'Value',
									flag as 'Flag',
									chksum as 'Check Sum',
									TxVal as 'Taxable Value',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_HSNSUM t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_HSNSUM_DET t3 On t3.hsnsumid = t2.hsnsumid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- NIL
							Select  gstin as 'GSTIN No', 
									t3.cpddr as 'Inter -Compounding Dealer', 
									t3.exptdsply as 'Inter -Exempted supplies received', 
									t3.ngsply as 'Inter -Non GST outward supplies', 
									t3.nilsply as 'Inter -Nil Rated Supply',
									t4.cpddr as 'Intra -Compounding Dealer', 
									t4.exptdsply as 'Intra -Exempted supplies received', 
									t4.ngsply as 'Intra -Non GST outward supplies', 
									t4.nilsply as 'Intra -Nil Rated Supply',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_NIL t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_NIL_INTER t3 On t3.nilid = t2.nilid
							Inner Join TBL_GSTR2_NIL_INTRA t4 On t4.nilid = t2.nilid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'
							
							-- TXPD
							Select  gstin as 'GSTIN No',
							        pos as 'Place Of Supply', 
									sply_ty as 'Supply Type',
									Rt as 'Rate',
									adamt as 'Advance to be adjusted',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_TXPD t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_TXPD_ITMS t3 On t3.txpdid = t2.txpdid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'


							-- TXI
							Select  gstin as 'GSTIN No',
							        pos as 'Place Of Supply', 
									sply_ty as 'Supply Type',
									Rt as 'Rate',
									adamt as 'Advance to be adjusted',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_TXI t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_TXI_ITMS t3 On t3.txiid = t2.txiid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'


							-- ITC RVSL
							Select  gstin as 'GSTIN No',
							        ruleName as 'Rule Name',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR2 t1
							Inner Join TBL_GSTR2_ITCRVSL t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_ITCRVSL_ITMS t3 On t3.itcrvslid = t2.itcrvslid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'
							

 END