
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTR2A Downloaded Data
				
Written by  : Karthik

Date		Who			Decription 
07/31/2017	Karthik			Initial Version


*/
/* Sample Procedure Call

exec usp_Retrieve_Logs_GSTR2A_DownloadedData 1,1,'072017'
 */

CREATE PROCEDURE usp_Retrieve_Logs_GSTR2A_DownloadedData
	@CustId	int,
	@UserId int,  
	@Period varchar(10)	
-- /*mssx*/ With Encryption 
as 
BEGIN

	Set Nocount on

	Declare @AdjAmt int
	set @AdjAmt = 1
		Select a.* Into #GstinIds
					From
					(Select gstinid as gstinid from tbl_cust_gstin where custid = @CustId 
					) a

		SELECT	t1.gstin as 'GSTIN No', 
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
					t5.Csamt as 'CESS Amount'
			Into #TBL_GSTR2A_B2B_Invs 
			From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_B2B t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_B2B_INV t3 On t3.b2bid = t2.b2bid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period

			Select t1.gstin as 'GSTIN No', 
					t2.ctin as 'CTIN No', 
					t3.Nt_Num as 'Note No',
					t3.Nt_Dt as 'Note Date',
					t3.Ntty as 'Note Type',
					t3.inum as 'Invoice No', 
					t3.val as 'Invoice Value', 
					t3.idt as 'Invoice Date',
					t3.flag as 'Flag',
					t3.chksum as 'Check Sum',					
					t3.Rsn as 'Reason',
					t3.P_gst as 'Pre GST',					
					t4.Num as 'Num',
					t5.Rt as 'Rate',
					t5.TxVal as 'Taxable Value',
					t5.Iamt as 'IGST Amount',
					t5.Camt as 'CGST Amount',
					t5.Samt as 'SGST Amount',
					t5.Csamt as 'CESS Amount'
					Into #TBL_GSTR2A_CDNR_Invs
					From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_CDNR t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 

	

		select * from #TBL_GSTR2A_B2B_Invs
		select * from #TBL_GSTR2A_CDNR_Invs

		Drop Table #TBL_GSTR2A_B2B_Invs
		Drop Table #TBL_GSTR2A_CDNR_Invs
	
	Return 0

End