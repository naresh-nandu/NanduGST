
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Log file for GSTR1 Uploaded data
				
Written by  : Karthik

Date		Who			Decription 
11/04/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Retrieve_Logs_Gstr1_UploadedData 1,1,'082017'
*/
CREATE PROCEDURE [usp_Retrieve_Logs_Gstr1_UploadedData]   
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
									t1.gt as 'Grant Total',
									t1.cur_gt as 'Current Grand Total', 
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
									t5.Csamt as 'CESS Amount'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR1_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- B2CL
							Select GSTIN as 'GSTIN No', 
								   inum as 'Invoice No', 
								   idt as 'Invoice Date', 
								   val as 'Invoice Value',
								   pos as 'Place Of Supply',
								   Flag as 'Flag', 
								   chksum as 'Check Sum', 
								   etin as 'Ecommerce CTIN',
								   num as 'Num',
								   Rt as 'Rate',
							       TxVal as 'Taxable Value',
								   Iamt as 'IGST Amount',
								   Csamt as 'CESS Amount'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'
							 
							-- B2CS
							Select GSTIN as 'GSTIN No', 
								   pos as 'Place Of Supply',
								   Flag as 'Flag', 
								   chksum as 'Check Sum', 								    
								   etin as 'Ecommerce CTIN',
								   Rt as 'Rate',
							       TxVal as 'Taxable Value',
								   Iamt as 'IGST Amount',
								   Camt as 'CGST Amount',
								   Samt as 'SGST Amount',
								   Csamt as 'CESS Amount'
							from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'
							 
							-- EXP
							Select  gstin as 'GSTIN No',
									ex_tp as 'Export Type',
									flag as 'Flag',
									chksum as 'Check Sum', 
									inum as 'Invoice No', 
									idt as 'Invoice Date', 
									val as 'Invoice Value', 
									sbnum as 'Shipping Bill No' , 
									sbdt as 'Shipping Bill Date', 
									sbpcode as 'Shipping Bill Port Code',
									txval as 'Taxable Value', 
									rt as 'Rate', 
									iamt as 'IGST Amount'
							from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
							Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'  
							
							-- CDNR
							Select  gstin as 'GSTIN No', 
									ctin as 'CTIN No', 
									cfs as 'GSTR2 Counter party Filing Status',
									ntty as 'Note Type', 
									nt_num as 'Note No', 
									nt_dt as 'Note Date',
									inum as 'Invoice No',  
									idt as 'Invoice Date',
									val as 'Invoice Value',
									p_gst as 'Pre GST', 
									rsn as 'Reason',
									flag as 'Flag',
									chksum as 'Check Sum',
									Num as 'Num',
									Rt as 'Rate',
									TxVal as 'Taxable Value',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- CDNUR
							Select  gstin as 'GSTIN No',
									typ as 'Type',
									ntty as 'Note Type', 
									nt_num as 'Note No', 
									nt_dt as 'Note Date',
									inum as 'Invoice No',  
									idt as 'Invoice Date',
									val as 'Invoice Value',
									p_gst as 'Pre GST', 
									rsn as 'Reason',
									flag as 'Flag',
									chksum as 'Check Sum',
									Num as 'Num',
									Rt as 'Rate',
									TxVal as 'Taxable Value',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
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
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_HSN t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_HSN_DATA t3 On t3.hsnid = t2.hsnid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- NIL
							Select  gstin as 'GSTIN No', 
									nil_amt as 'Total Nil rated outward supplies', 
									expt_amt as 'Total Exempted outward supplies', 
									ngsup_amt as 'Total Non GST outward supplies', 
									sply_ty as 'Nature of Supply Type',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_NIL t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_NIL_INV t3 On t3.nilid = t2.nilid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- TXP
							Select  gstin as 'GSTIN No',
							        pos as 'Place Of Supply', 
									sply_ty as 'Supply Type',
									Rt as 'Rate',
									ad_amt as 'Advance to be adjusted',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_TXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_TXP_ITMS t3 On t3.txpid = t2.txpid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- AT
							Select  gstin as 'GSTIN No',
							        pos as 'Place Of Supply', 
									sply_ty as 'Supply Type',
									Rt as 'Rate',
									ad_amt as 'Advance received',
									Iamt as 'IGST Amount',
									Camt as 'CGST Amount',
									Samt as 'SGST Amount',
									Csamt as 'CESS Amount',
									flag as 'Flag',
									chksum as 'Check Sum'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_AT t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_AT_ITMS t3 On t3.atid = t2.atid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

							-- Doc Issue
							Select  gstin as 'GSTIN No',
									doc_num as 'Document No', 
									doc_typ as 'Document Type',
									flag as 'Flag',
									chksum as 'Check Sum',
									num as 'Serial Number', 
									[from] as 'From serial number', 
									[to] as 'To serial number', 
									totnum as 'Total Number', 
									cancel as 'Cancelled', 
									net_issue as 'Net issued'
							From TBL_GSTR1 t1
							Inner Join TBL_GSTR1_DOC_ISSUE t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_DOCS t3 On t3.docissueid = t2.docissueid
							Where t1.gstinid in( select gstinid from #GstinIds)
							And fp = @Period 
							And flag = 'U'

 END