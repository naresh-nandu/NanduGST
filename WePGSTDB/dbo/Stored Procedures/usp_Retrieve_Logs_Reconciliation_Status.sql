
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve Reconciliation status details
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri			Initial Version


*/
/* Sample Procedure Call

exec usp_Retrieve_Logs_Reconciliation_Status 1,1,'072017'
 */

CREATE PROCEDURE [usp_Retrieve_Logs_Reconciliation_Status]
	@CustId	int,
	@UserId int,  
	@Period varchar(10)	
-- /*mssx*/ With Encryption 
as 
BEGIN

	Select a.* Into #GstinIds
					From
					(Select gstinid as gstinid from tbl_cust_gstin where custid = @CustId 
					) a
					
					
					-- B2B 
					Select Gstin,Ctin,Inum,Idt,Val,Flag,Rchrg,POS,Inv_Typ,Num,Rt,TxVal,Iamt,Camt,Samt,Csamt,Tx_I,Tx_C,Tx_S,Tx_CS,Elg
					Into #temp1
					From TBL_GSTR2 t1
					Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
					Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
					Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 
					And t3.flag in ('A','R','M','P')
		
					--select * from TBL_RECONCILIATION_LOGS
					Select * into #t2_b2bAccept from #temp1 where flag='A'
					Select * into #t2_b2bReject from #temp1 where flag='R'
					Select * into #t2_b2bModify from #temp1 where flag='M'
					Select * into #t2_b2bPending from #temp1 where flag='P'

					
					-- CDNR
					Select Gstin,Ctin,Nt_Num,Nt_Dt,Ntty,Inum,Idt,Rsn,P_gst,Val,Flag,Num,Rt,TxVal,Iamt,Camt,Samt,Csamt,Tx_I,Tx_C,Tx_S,Tx_CS,Elg
					Into #temp2
					From TBL_GSTR2 t1
					Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id
					Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t6 On t6.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 
					And t3.flag in ('A','R','M','P')
		
					--select * from TBL_RECONCILIATION_LOGS
					Select * into #t2_cdnrAccept from #temp2 where flag='A'
					Select * into #t2_cdnrReject from #temp2 where flag='R'
					Select * into #t2_cdnrModify from #temp2 where flag='M'
					Select * into #t2_cdnrPending from #temp2 where flag='P'

					-- OUtput of B2B and CDNR
					Select * from #t2_b2bAccept
					Select * from #t2_cdnrAccept
					Select * from #t2_b2bReject
					Select * from #t2_cdnrReject
					Select * from #t2_b2bModify
					Select * from #t2_cdnrModify
					Select * from #t2_b2bPending
					Select * from #t2_cdnrPending

					-- Dropping all Temp Tables
					Drop table #t2_b2bAccept
					Drop table #t2_b2bReject
					Drop table #t2_b2bModify
					Drop table #t2_b2bPending

					Drop table #t2_cdnrAccept
					Drop table #t2_cdnrReject
					Drop table #t2_cdnrModify
					Drop table #t2_cdnrPending
		
END