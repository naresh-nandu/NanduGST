
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve Reconciliation status details
				
Written by  : Karthik

Date		Who			Decription 
07/31/2017	Karthik			Initial Version


*/
/* Sample Procedure Call

exec usp_Retrieve_Logs_Reconciliation_Summary 1,1,'072017'
 */

CREATE PROCEDURE [usp_Retrieve_Logs_Reconciliation_Summary]
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

			SELECT	t1.gstin as gstin2, 
					t2.ctin as ctin2, 
					t3.inum as inum2, 
					t3.val as val2, 
					t3.idt as idt2,
					t3.pos as pos2,
					t3.inv_typ as inv_typ2,
					t3.Rchrg as Rchrg2,
					t3.flag as flag2,
					t3.chksum as chksum2,
					t3.uploadstatus as uploadstatus2,
					t4.Num as num2,
					t5.Rt as rt2,
					t5.TxVal as txval2,
					t5.Iamt as iamt2,
					t5.Camt as camt2,
					t5.Samt as samt2,
					t5.Csamt as csamt2,
					t6.Tx_I as tx_i2,
					t6.Tx_C as tx_c2,
					t6.Tx_S as tx_s2,
					t6.Tx_CS as tx_cs2,
					t6.Elg as elg2
			Into #TBL_B2B_Invs_T1  
			From TBL_GSTR2 t1
					Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
					Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
					Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 
			

			SELECT	t1.gstin as gstin2a, 
					t2.ctin as ctin2a, 
					t3.inum as inum2a, 
					t3.val as val2a, 
					t3.idt as idt2a,
					t3.pos as pos2a,
					t3.inv_typ as inv_typ2a,
					t3.Rchrg as Rchrg2a,
					t3.flag as flag2a,
					t3.chksum as chksum2a,
					t4.Num as num2a,
					t5.Rt as rt2a,
					t5.TxVal as txval2a,
					t5.Iamt as iamt2a,
					t5.Camt as camt2a,
					t5.Samt as samt2a,
					t5.Csamt as csamt2a
			Into #TBL_B2B_Invs_T2 
			From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_B2B t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_B2B_INV t3 On t3.b2bid = t2.b2bid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period

			Select t1.Gstin as Gstin2,
					t2.Ctin as Ctin2,
					t3.Nt_Num as Nt_Num2,
					t3.Nt_Dt as Nt_Dt2,
					t3.Ntty as Ntty2,
					t3.Inum as Inum2,
					t3.Idt as Idt2,
					t3.Rsn as Rsn2,
					t3.P_gst as P_gst2,
					t3.Val as Val2,
					t3.Flag as Flag2,
					t3.chksum as chksum2,
					t3.Uploadstatus as Uploadstatus2,
					t4.Num as num2,
					t5.Rt as rt2,
					t5.TxVal as txval2,
					t5.Iamt as iamt2,
					t5.Camt as camt2,
					t5.Samt as samt2,
					t5.Csamt as csamt2,
					t6.Tx_I as tx_i2,
					t6.Tx_C as tx_c2,
					t6.Tx_S as tx_s2,
					t6.Tx_CS as tx_cs2,
					t6.Elg as elg2
					Into #TBL_CDNR_Invs_T1
					From TBL_GSTR2 t1
					Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id
					Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t6 On t6.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 

			Select t1.Gstin as Gstin2a,
					t2.Ctin as Ctin2a,
					t3.Nt_Num as Nt_Num2a,
					t3.Nt_Dt as Nt_Dt2a,
					t3.Ntty as Ntty2a,
					t3.Inum as Inum2a,
					t3.Idt as Idt2a,
					t3.Rsn as Rsn2a,
					t3.P_gst as P_gst2a,
					t3.Val as Val2a,
					t3.Flag as Flag2a,
					t3.chksum as chksum2a,
					t4.Num as num2a,
					t5.Rt as rt2a,
					t5.TxVal as txval2a,
					t5.Iamt as iamt2a,
					t5.Camt as camt2a,
					t5.Samt as samt2a,
					t5.Csamt as csamt2a
					Into #TBL_CDNR_Invs_T2
					From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_CDNR t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t1.gstinid in( select gstinid from #GstinIds)
					And fp = @Period 

	-- Matched Invoices GSTR2 and 2A
		-- B2B
			Select t1.* into #tbl_b2bMatched_Invs
			From #TBL_B2B_Invs_T1 t1 
			Join #TBL_B2B_Invs_T2 t2
			ON	t1.gstin2=t2.gstin2a 
				And t1.ctin2=t2.ctin2a 
				And t1.inum2=t2.inum2a 
				And t1.idt2=t2.idt2a 
				And t1.pos2=t2.pos2a
				And t1.inv_typ2=t2.inv_typ2a
				And Not ( (t1.val2 < (t2.val2a - @AdjAmt))
						or
					  (t1.val2 > (t2.val2a + @AdjAmt))
					)
			Where t1.gstin2 is not NULL 
			And t2.gstin2a is not NULL 
			And (t1.flag2 is NULL or t1.flag2 ='A')

	    -- CDNR
			Select t1.* into #tbl_cdnrMatched_Invs
			From #TBL_CDNR_Invs_T1 t1 
			Join  #TBL_CDNR_Invs_T2 t2 
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2=t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And t1.nt_num2=t2.nt_num2a 
				And t1.nt_dt2=t2.nt_dt2a 
				And t1.ntty2=t2.ntty2a 
				And t1.p_gst2 = t2.p_gst2a 
				And Not ( (t1.val2 < (t2.val2a - @AdjAmt))
						or
					  (t1.val2 > (t2.val2a + @AdjAmt))
					)
			Where t1.gstin2 is not NULL 
			And t2.gstin2a is not NULL
		

	-- MissingInGSTR2A Invoices
	  -- B2B
			Select t1.* into #tbl_b2bMissing_GSTR2A_Invs 
			From  #TBL_B2B_Invs_T1 t1
			Left Outer Join #TBL_B2B_Invs_T2 t2
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2 =t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And t1.pos2=t2.pos2a 
				And t1.inv_typ2=t2.inv_typ2a
			Where t2.gstin2a is NULL 
	
	-- CDNR
			Select t1.* into #tbl_cdnrMissing_GSTR2A_Invs 
			From #TBL_CDNR_Invs_T1 t1
			Left Outer Join #TBL_CDNR_Invs_T2 t2
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2=t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And  t1.nt_num2=t2.nt_num2a 
				And t1.nt_dt2=t2.nt_dt2a 
				And t1.ntty2=t2.ntty2a 
				And t1.p_gst2 = t2.p_gst2a
			Where t2.gstin2a is NULL
		

	-- MissingInGSTR2 Invoices
	  -- B2B
			Select t2.* into #tbl_b2bMissing_GSTR2_Invs 
			From  #TBL_B2B_Invs_T1 t1
			Right Outer Join #TBL_B2B_Invs_T2 t2
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2 =t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And t1.pos2=t2.pos2a 
				And t1.inv_typ2=t2.inv_typ2a
			Where t1.gstin2 is NULL

	  -- CDNR
			Select t2.* into #tbl_cdnrMissing_GSTR2_Invs
			From  #TBL_CDNR_Invs_T1 t1
			Right Outer Join #TBL_CDNR_Invs_T2 t2
			ON  t1.gstin2 = t2.gstin2a 
				And t1.ctin2=t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And  t1.nt_num2=t2.nt_num2a 
				And t1.nt_dt2=t2.nt_dt2a 
				And t1.ntty2=t2.ntty2a 
				And t1.p_gst2 = t2.p_gst2a 	
		Where t1.gstin2 is NULL
		
	-- Mismatch Invoices
	  -- B2B
			Select t1.*,t2.* into #tbl_b2bMismatch_Invs
			From #TBL_B2B_Invs_T1 t1
			Join #TBL_B2B_Invs_T2 t2
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2 =t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And t1.pos2=t2.pos2a 
				And t1.inv_typ2=t2.inv_typ2a
				And ( (t1.val2 < (t2.val2a - @AdjAmt))
						or
					  (t1.val2 > (t2.val2a + @AdjAmt))
					)
			Where t1.gstin2 is not NULL 
			And t2.gstin2a is not NULL

	   -- CDNR
		   Select t1.*,t2.* into #tbl_cdnrMismatch_Invs 
			From  #TBL_CDNR_Invs_T1 t1
			Join #TBL_CDNR_Invs_T2 t2
			ON	t1.gstin2 = t2.gstin2a 
				And t1.ctin2=t2.ctin2a 
				And t1.inum2 = t2.inum2a 
				And t1.idt2=t2.idt2a 
				And  t1.nt_num2 = t2.nt_num2a 
				And t1.nt_dt2 = t2.nt_dt2a 
				And t1.ntty2 = t2.ntty2a 
				And t1.p_gst2 = t2.p_gst2a 
				And ( (t1.val2 < (t2.val2a - @AdjAmt))
						or
					  (t1.val2 > (t2.val2a + @AdjAmt))
					)
			Where t1.gstin2 is not NULL 
			And t2.gstin2a is not NULL


		select * from #tbl_b2bMatched_Invs
		select * from #tbl_cdnrMatched_Invs
		select * from #tbl_b2bMissing_GSTR2A_Invs
		select * from #tbl_cdnrMissing_GSTR2A_Invs
		select * from #tbl_b2bMissing_GSTR2_Invs
		select * from #tbl_cdnrMissing_GSTR2_Invs
		select * from #tbl_b2bMismatch_Invs
		select * from #tbl_cdnrMismatch_Invs

		Drop Table #tbl_b2bMatched_Invs
		Drop Table #tbl_b2bMissing_GSTR2A_Invs
		Drop Table #tbl_b2bMissing_GSTR2_Invs
		Drop Table #tbl_b2bMismatch_Invs
		Drop Table #tbl_cdnrMatched_Invs
		Drop Table #tbl_cdnrMissing_GSTR2A_Invs
		Drop Table #tbl_cdnrMissing_GSTR2_Invs
		Drop Table #tbl_cdnrMismatch_Invs
	
	Return 0

End