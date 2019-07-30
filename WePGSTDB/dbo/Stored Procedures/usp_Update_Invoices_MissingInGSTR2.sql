
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update Invoices missing in GSTR2
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri	Initial Version
10/15/2017  Karthik		Added code for include flag 'P' and for all activity data will be inserted into GSTR2 and respective activity flag will be updated in GSTR2.
11/2/2017	Seshadri	Fixed the crash issue regarding CDNR
11/03/2017	Seshadri	Fixed the issue of duplicate item detail insertion & ITC issue in CDNR

*/

/* Sample Procedure Call

exec usp_Update_Invoices_MissingInGSTR2
 */

CREATE PROCEDURE [usp_Update_Invoices_MissingInGSTR2]
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Activity varchar(2), -- A : 'Accept' R : 'Reject' P : 'Pending'
	@RefId int,
	@UserId int,
	@CustId int
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		
		if Exists(Select 1 From TBL_GSTR2A_B2B_INV Where invid = @RefId)
		Begin
		  	Select
				space(50) as gstr2id,
				space(50) as b2bid,
				space(50) as invid,
				space(50) as itmsid,
				t1.gstin,t2.gstinid,t1.fp,t1.ctin,
				t1.chksum,t1.inum,t1.idt,t1.val,t1.flag,t1.rchrg,t1.pos,t1.inv_typ,
				t1.num,t1.rt,t1.txval, 
				t1.iamt,t1.camt,t1.samt,t1.csamt
				Into #TBL_GSTR2A_B2B 
				From
				(
					SELECT 
						gstin,fp,ctin,
						chksum,inum,idt,val,flag,rchrg,pos,inv_typ,
						num,rt,txval,
						iamt,camt,samt,csamt
					From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_B2B t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_B2B_INV t3 On t3.b2bid = t2.b2bid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS t4 On t4.invid = t3.invid
					Inner Join TBL_GSTR2A_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t3.invid = @RefId

				)t1
				Cross Apply 
				(
					Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin
				)t2

				-- Insert Records into Table TBL_GSTR2

				Insert TBL_GSTR2 (gstin,gstinId,fp)
				Select	distinct gstin,gstinid,fp
				From #TBL_GSTR2A_B2B t1
				Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

				Update #TBL_GSTR2A_B2B 
				SET #TBL_GSTR2A_B2B.gstr2id = t2.GSTR2id 
				FROM #TBL_GSTR2A_B2B t1,
					TBL_GSTR2 t2 
				WHERE t1.gstin = t2.gstin 
				And t1.gstinid = t2.gstinid 
				And t1.fp = t2.fp

				-- Insert Records into Table TBL_GSTR2_B2B

				Insert TBL_GSTR2_B2B(gstr2id,ctin,gstinid) 
				Select	distinct gstr2id,ctin,gstinid
				From  #TBL_GSTR2A_B2B t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_B2B t2 where t2.GSTR2id = t1.GSTR2id and t2.ctin = t1.ctin) 

				Update #TBL_GSTR2A_B2B
				SET #TBL_GSTR2A_B2B.b2bid = t2.b2bid 
				FROM #TBL_GSTR2A_B2B t1,
					TBL_GSTR2_B2B t2 
				WHERE t1.gstr2id = t2.gstr2id 
				And t1.ctin = t2.ctin 

				-- Insert Records into Table TBL_GSTR2_B2B_INV

				Insert TBL_GSTR2_B2B_INV
				(b2bid,chksum,inum,idt,val,flag,rchrg,pos,inv_typ,gstinid,gstr2id)
				Select	distinct t1.b2bid,t1.chksum,t1.inum,t1.idt,t1.val,
						@Activity,t1.rchrg,t1.pos,t1.inv_typ,t1.gstinid,t1.gstr2id
				From #TBL_GSTR2A_B2B t1
				Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_B2B_INV t2
									Where t2.b2bid = t1.b2bid 
										And t2.inum = t1.inum
										And t2.idt = t1.idt) 		 


				Update #TBL_GSTR2A_B2B 
				SET #TBL_GSTR2A_B2B.invid = t2.invid 
				FROM #TBL_GSTR2A_B2B t1,
						TBL_GSTR2_B2B_INV t2 
				WHERE t1.b2bid= t2.b2bid 
				And t1.inum = t2.inum
				And t1.idt = t2.idt

				Insert TBL_GSTR2_B2B_INV_ITMS
				(invid,num,gstinid,gstr2id)
				Select	distinct t1.invid,t1.num,t1.gstinid,t1.gstr2id
				From #TBL_GSTR2A_B2B t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_B2B_INV_ITMS t2
									Where t2.invid = t1.invid 
									And t2.num = t1.num)
		
	
				Update #TBL_GSTR2A_B2B 
				SET #TBL_GSTR2A_B2B.itmsid = t2.itmsid
				FROM #TBL_GSTR2A_B2B t1,
					TBL_GSTR2_B2B_INV_ITMS t2 
				WHERE t1.invid= t2.invid 
				And t1.num = t2.num
	
					
				Insert TBL_GSTR2_B2B_INV_ITMS_DET
				(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id)
				Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id
				From #TBL_GSTR2A_B2B t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_B2B_INV_ITMS_DET t2
									Where t2.itmsid = t1.itmsid)


				Insert TBL_GSTR2_B2B_INV_ITMS_ITC
				(itmsid,tx_i,tx_c,tx_s,tx_cs,elg,gstinid,gstr2id)
				Select itmsid,iamt,camt,samt,csamt,NULL,gstinid,gstr2id
				From #TBL_GSTR2A_B2B t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_B2B_INV_ITMS_ITC t2
									Where t2.itmsid = t1.itmsid)


				Update TBL_GSTR2A_B2B_INV 
				Set flag= @Activity 
				Where invid= @RefId 


				Insert into TBL_RECONCILIATION_LOGS
				(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
				Values(@Gstin,@RefId ,'GSTR2A',@ActionType, @Activity,'',@UserId,@CustId,GETDATE(),1)
	
		End
		
	End
	else if @ActionType = 'CDNR' or @ActionType = 'CDN'
	Begin
		if Exists(Select 1 From TBL_GSTR2A_CDNR_NT Where ntid = @RefId)
		Begin
		  	Select
				space(50) as gstr2id,
				space(50) as cdnrid,
				space(50) as ntid,
				space(50) as itmsid,
				t1.gstin,t2.gstinid,t1.fp,t1.ctin,
				t1.chksum,t1.flag,t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,
				t1.inum,t1.idt,t1.num,
				t1.rt,t1.txval, 
				t1.iamt,t1.camt,t1.samt,t1.csamt
				Into #TBL_GSTR2A_CDNR
				From
				(
					SELECT 
						gstin,fp,ctin,
						chksum,flag,ntty,nt_num,nt_dt,rsn,p_gst,
						inum,idt,num,
						rt,txval,iamt,camt,samt,csamt
					From TBL_GSTR2A t1
					Inner Join TBL_GSTR2A_CDNR t2 On t2.gstr2aid = t1.gstr2aid
					Inner Join TBL_GSTR2A_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
					Inner Join TBL_GSTR2A_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
					Where t3.ntid = @RefId

				)t1
				Cross Apply 
				(
					Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin
				)t2

				-- Insert Records into Table TBL_GSTR2

				Insert TBL_GSTR2 (gstin,gstinId,fp)
				Select	distinct gstin,gstinid,fp
				From #TBL_GSTR2A_CDNR t1
				Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

				Update #TBL_GSTR2A_CDNR 
				SET #TBL_GSTR2A_CDNR.gstr2id = t2.GSTR2id 
				FROM #TBL_GSTR2A_CDNR t1,
					TBL_GSTR2 t2 
				WHERE t1.gstin = t2.gstin 
				And t1.gstinid = t2.gstinid 
				And t1.fp = t2.fp

				-- Insert Records into Table TBL_GSTR2_CDNR

				Insert TBL_GSTR2_CDNR(gstr2id,ctin,gstinid) 
				Select	distinct gstr2id,ctin,gstinid
				From #TBL_GSTR2A_CDNR t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNR t2 where t2.gstr2id = t1.gstr2id and t2.ctin = t1.ctin) 

				Update #TBL_GSTR2A_CDNR 
				SET #TBL_GSTR2A_CDNR.cdnrid = t2.cdnrid 
				FROM #TBL_GSTR2A_CDNR t1,
					TBL_GSTR2_CDNR t2 
				WHERE t1.gstr2id = t2.gstr2id 
				And t1.ctin = t2.ctin 

				-- Insert Records into Table TBL_GSTR2_CDNR_NT

				Insert TBL_GSTR2_CDNR_NT
				(cdnrid,flag,chksum,ntty,nt_num,nt_dt,rsn,p_gst,inum,idt,gstinid,gstr2id)
				Select	distinct t1.cdnrid,@Activity,t1.chksum,t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,
								t1.inum,t1.idt,t1.gstinid,t1.gstr2id
				From  #TBL_GSTR2A_CDNR  t1
				Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNR_NT t2
									Where t2.cdnrid = t1.cdnrid 
										And t2.inum = t1.inum
										And t2.idt = t1.idt)		 


				Update #TBL_GSTR2A_CDNR 
				SET #TBL_GSTR2A_CDNR.ntid = t2.invid 
				FROM  #TBL_GSTR2A_CDNR  t1,
					TBL_GSTR2_CDNR_NT t2 
				WHERE t1.cdnrid= t2.cdnrid 
				And t1.inum = t2.inum
				And t1.idt = t2.idt
	
				Insert TBL_GSTR2_CDNR_NT_ITMS
				(invid,num,gstinid,gstr2id)
				Select	distinct t1.ntid,t1.num,t1.gstinid,t1.gstr2id
				From #TBL_GSTR2A_CDNR  t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNR_NT_ITMS t2
									Where t2.invid = t1.ntid 
									And t2.num = t1.num)
		
	
				Update #TBL_GSTR2A_CDNR  
				SET #TBL_GSTR2A_CDNR.itmsid = t2.itmsid
				FROM #TBL_GSTR2A_CDNR  t1,
						TBL_GSTR2_CDNR_NT_ITMS t2 
				WHERE t1.ntid= t2.invid 
				And t1.num = t2.num
	
				Insert TBL_GSTR2_CDNR_NT_ITMS_DET
				(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id)
				Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id
				From  #TBL_GSTR2A_CDNR  t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNR_NT_ITMS_DET t2
									Where t2.itmsid = t1.itmsid)

				Insert TBL_GSTR2_CDNR_NT_ITMS_ITC
				(itmsid,tx_i,tx_c,tx_s,tx_cs,elg,gstinid,gstr2id)
				Select itmsid,iamt,camt,samt,csamt,NULL,gstinid,gstr2id
				From  #TBL_GSTR2A_CDNR  t1
				Where Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNR_NT_ITMS_ITC t2
									Where t2.itmsid = t1.itmsid)

				Update TBL_GSTR2A_CDNR_NT 
				Set flag= @Activity 
				Where ntid= @RefId

				Insert into TBL_RECONCILIATION_LOGS
				(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus) 
				Values(@Gstin,@RefId ,'GSTR2A',@ActionType, @Activity,'',@UserId,@CustId,GETDATE(),1)

		End
	End

	Return 0
End