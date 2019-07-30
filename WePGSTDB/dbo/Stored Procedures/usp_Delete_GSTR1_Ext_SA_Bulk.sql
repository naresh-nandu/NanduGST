
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete all the GSTR1 Records from the corresponding staging area tables based on filters
				
Written by  : karthik.kanniyappan@wepdigital.com 

Date		Who			Decription 
07/06/2018	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_All_GSTR1_SA 
 */

CREATE PROCEDURE [usp_Delete_GSTR1_Ext_SA_Bulk]
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	DECLARE @TBL_Values TABLE 
	(
		ActionType varchar(15),
		RefId int,
		Gstin varchar(15),
		Fp varchar(10),
		InvNo varchar(50),
		InvDate varchar(50),
		Pos varchar(2),
		Rt decimal(18,2),
		NoteNo varchar(50),
		NoteDate varchar(50),
		HsnCode varchar(50),
		HsnDesc varchar(50)
	)

	Select @Delimiter = ','
	
	Insert Into @TBL_Values
	(ActionType,RefId)
	Select	@ActionType,Value 
	From string_split( @RefIds,@Delimiter) 


	if @ActionType = 'B2B'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_B2B_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		select itmsid into #b2b_itmsid from TBL_GSTR1_B2B_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_B2B_INV_ITMS_DET 
		where itmsid in (select itmsid from #b2b_itmsid)

		Delete from TBL_GSTR1_B2B_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_B2B_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_B2B_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_B2B_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_B2B_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_B2B_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType
	

	End
	else if @ActionType = 'B2CL'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_B2CL_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 
		
		select itmsid into #b2cl_itmsid from TBL_GSTR1_B2CL_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_B2CL_INV_ITMS_DET 
		where itmsid in (select itmsid from #b2cl_itmsid)

		Delete from TBL_GSTR1_B2CL_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_B2CL_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_B2CL_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_B2CL_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_B2CL_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_B2CL_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'B2CS'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			Pos = t2.pos,
			Rt = t2.rt	
		From @TBL_Values t1,
			 TBL_GSTR1_B2CS t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.b2csid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		Delete from TBL_GSTR1_B2CS
		Where b2csid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_B2CS t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.pos = t2.pos
		And t1.rt = t2.rt
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_B2CS
		--Set flag = 'D'
		--Where b2csid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_B2CS
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_B2CS t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.pos = t2.pos
		--And t1.rt = t2.rt
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'EXP'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_EXP_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 
		
		Delete from TBL_GSTR1_EXP_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_EXP_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_EXP_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_EXP_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_EXP_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_EXP_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType


	End
	else if @ActionType = 'CDNR'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			NoteNo = t2.nt_num,
			NoteDate = t2.nt_dt
		From @TBL_Values t1,
			 TBL_GSTR1_CDNR_NT t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.ntid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 


		select itmsid into #cdnr_itmsid from TBL_GSTR1_CDNR_NT_ITMS 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_CDNR_NT_ITMS_DET 
		where itmsid in (select itmsid from #cdnr_itmsid)

		Delete from TBL_GSTR1_CDNR_NT_ITMS 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_CDNR_NT 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_CDNR t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.nt_num = t2.NoteNo
		And t1.nt_dt = t2.NoteDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_CDNR_NT
		--Set flag = 'D'
		--Where ntid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_CDNR
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_CDNR t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.nt_num = t2.NoteNo
		--And t1.nt_dt = t2.NoteDate
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'CDNUR'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			NoteNo = t2.nt_num,
			NoteDate = t2.nt_dt
		From @TBL_Values t1,
			 TBL_GSTR1_CDNUR t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.cdnurid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

	    select itmsid into #cdnur_itmsid from TBL_GSTR1_CDNUR_ITMS 
		where cdnurid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_CDNUR_ITMS_DET 
		where itmsid in (select itmsid from #cdnur_itmsid)

								
		Delete from TBL_GSTR1_CDNUR 
		where cdnurid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_CDNUR t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.nt_num = t2.NoteNo
		And t1.nt_dt = t2.NoteDate
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_CDNUR
		--Set flag = 'D'
		--Where cdnurid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_CDNUR
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_CDNUR t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.nt_num = t2.NoteNo
		--And t1.nt_dt = t2.NoteDate
		--And t2.ActionType = @ActionType


	End
	else if (@ActionType In ('HSN','HSNSUM'))
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			HsnCode = t2.hsn_sc,
			HsnDesc = t2.[desc]
		From @TBL_Values t1,
			 TBL_GSTR1_HSN_DATA t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.dataid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		Delete from TBL_GSTR1_HSN_DATA
		Where dataid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_HSN t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.hsn_sc = t2.HsnCode
		And t1.descs = t2.HsnDesc
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_HSN_DATA
		--Set flag = 'D'
		--Where dataid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_HSN
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_HSN t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.hsn_sc = t2.HsnCode
		--And t1.descs = t2.HsnDesc
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'NIL'
	Begin

	 	Select gstin,fp,nil_amt,expt_amt,ngsup_amt,sply_ty
		Into #TBL_GSTR1_NIL
		From TBL_GSTR1_NIL_INV t1,
		     TBL_GSTR1 t2
		Where t1.nilid In (Select refid From @TBL_Values
							Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_NIL_INV 
		where nilid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_NIL 
		where nilid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_NIL)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_NIL t1,
				 #TBL_GSTR1_NIL t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.nil_amt = t2.nil_amt
			And t1.expt_amt = t2.expt_amt
			And t1.ngsup_amt = t2.ngsup_amt
			And t1.sply_ty = t2.sply_ty
		End

		--Update TBL_GSTR1_NIL
		--Set flag = 'D'
		--Where nilid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		
		--if Exists(Select 1 From #TBL_GSTR1_NIL)
		--Begin
		--	Update TBL_EXT_GSTR1_NIL
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_NIL t1,
		--		 #TBL_GSTR1_NIL t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.nil_amt = t2.nil_amt
		--	And t1.expt_amt = t2.expt_amt
		--	And t1.ngsup_amt = t2.ngsup_amt
		--	And t1.sply_ty = t2.sply_ty
		--End


	End
	else if @ActionType = 'TXP'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_TXP
		From TBL_GSTR1_TXP_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.txpid In (Select refid From @TBL_Values
							Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_TXP_ITMS
		Where txpid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		Delete from TBL_GSTR1_TXP
		Where txpid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		
		if Exists(Select 1 From #TBL_GSTR1_TXP)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_TXP t1,
				 #TBL_GSTR1_TXP t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.ad_amt = t2.ad_amt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End

		--Update TBL_GSTR1_TXP
		--Set flag = 'D'
		--Where txpid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_TXP)
		--Begin
		--	Update TBL_EXT_GSTR1_TXP
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_TXP t1,
		--		 #TBL_GSTR1_TXP t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.rt = t2.rt
		--	And t1.ad_amt = t2.ad_amt
		--	And t1.iamt = t2.iamt
		--	And t1.camt = t2.camt
		--	And t1.samt = t2.samt
		--	And t1.csamt = t2.csamt
		--End

	
	End
	else if @ActionType = 'AT'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_AT
		From TBL_GSTR1_AT_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.atid In (Select refid From @TBL_Values
						  Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_AT_ITMS
		Where atid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_AT
		Where atid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_AT)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_AT t1,
				 #TBL_GSTR1_AT t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.ad_amt = t2.ad_amt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End
		--Update TBL_GSTR1_AT
		--Set flag = 'D'
		--Where atid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_AT)
		--Begin
		--	Update TBL_EXT_GSTR1_AT
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_AT t1,
		--		 #TBL_GSTR1_AT t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.rt = t2.rt
		--	And t1.ad_amt = t2.ad_amt
		--	And t1.iamt = t2.iamt
		--	And t1.camt = t2.camt
		--	And t1.samt = t2.samt
		--	And t1.csamt = t2.csamt
		--End

	
	End
	else if (@ActionType In ('DOCISSUE','DOC'))
	Begin

		Select gstin,fp,num,[from],[to],totnum,cancel,net_issue
		Into #TBL_GSTR1_DOC
		From TBL_GSTR1_DOCS t1,
		     TBL_GSTR1 t2
		Where t1.docissueid In (Select refid From @TBL_Values
								Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_DOCS
		Where docissueid In (Select refid From @TBL_Values
							 Where actiontype = @ActionType)

		Delete from TBL_GSTR1_DOC_ISSUE
		Where docissueid In (Select refid From @TBL_Values
							 Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_DOC)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_DOC t1,
				 #TBL_GSTR1_DOC t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.num = t2.num
			And t1.froms = t2.[from]
			And t1.tos = t2.[to]
			And t1.totnum = t2.totnum
			And t1.cancel = t2.cancel
			And t1.net_issue = t2.net_issue
		End

		--Update TBL_GSTR1_DOC_ISSUE
		--Set flag = 'D'
		--Where docissueid In (Select refid From @TBL_Values
		--					 Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_DOC)
		--Begin
		--	Update TBL_EXT_GSTR1_DOC
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_DOC t1,
		--		 #TBL_GSTR1_DOC t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.num = t2.num
		--	And t1.froms = t2.[from]
		--	And t1.tos = t2.[to]
		--	And t1.totnum = t2.totnum
		--	And t1.cancel = t2.cancel
		--	And t1.net_issue = t2.net_issue
		--End
	
	End
	else if @ActionType = 'B2BA'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_B2BA_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		select itmsid into #b2ba_itmsid from TBL_GSTR1_B2BA_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_B2BA_INV_ITMS_DET 
		where itmsid in (select itmsid from #b2ba_itmsid)

		Delete from TBL_GSTR1_B2BA_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_B2BA_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_B2BA_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_B2BA_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_B2BA_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_B2BA_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType
	
	End
	else if @ActionType = 'B2CLA'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_B2CLA_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		select itmsid into #b2bcla_itmsid from TBL_GSTR1_B2CLA_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_B2CLA_INV_ITMS_DET 
		where itmsid in (select itmsid from #b2bcla_itmsid)

		Delete from TBL_GSTR1_B2CLA_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_B2CLA_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_B2CLA_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_B2CLA_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_B2CLA_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_B2CLA_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'B2CSA'
	Begin

		Select gstin,fp,rt,txval,iamt,camt,samt,csamt
		Into #TBL_GSTR1_B2CSA
		From TBL_GSTR1_B2CSA_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.b2csaid In (Select refid From @TBL_Values
						  Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_B2CSA
		Where b2csaid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_B2CSA)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_B2CSA t1,
				 #TBL_GSTR1_B2CSA t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.txval = t2.txval
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End
		--Update TBL_GSTR1_B2CSA
		--Set flag = 'D'
		--Where b2csaid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_B2CSA)
		--Begin
		--	Update TBL_EXT_GSTR1_B2CSA
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_B2CSA t1,
		--		 #TBL_GSTR1_B2CSA t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.rt = t2.rt
		--	And t1.txval = t2.txval
		--	And t1.iamt = t2.iamt
		--	And t1.camt = t2.camt
		--	And t1.samt = t2.samt
		--	And t1.csamt = t2.csamt
		--End

	End
	else if @ActionType = 'EXPA'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			InvNo = t2.inum,
			InvDate = t2.idt
		From @TBL_Values t1,
			 TBL_GSTR1_EXPA_INV t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.invid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		Delete from TBL_GSTR1_EXPA_INV_ITMS 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_EXPA_INV 
		where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		--Update TBL_GSTR1_EXPA_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		Delete t1
		From  TBL_EXT_GSTR1_EXPA_INV t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.inum = t2.InvNo
		And t1.idt = t2.InvDate
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_EXPA_INV
		--Set flag = 'D'
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_EXPA_INV
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_EXPA_INV t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.inum = t2.InvNo
		--And t1.idt = t2.InvDate
		--And t2.ActionType = @ActionType


	End
	else if @ActionType = 'CDNRA'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			NoteNo = t2.nt_num,
			NoteDate = t2.nt_dt
		From @TBL_Values t1,
			 TBL_GSTR1_CDNRA_NT t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.ntid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 
		
		select itmsid into #cdnra_itmsid from TBL_GSTR1_CDNRA_NT_ITMS 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_CDNRA_NT_ITMS_DET 
		where itmsid in (select itmsid from #cdnra_itmsid)

		Delete from TBL_GSTR1_CDNRA_NT_ITMS 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
								
		Delete from TBL_GSTR1_CDNRA_NT 
		where ntid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_CDNRA t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.nt_num = t2.NoteNo
		And t1.nt_dt = t2.NoteDate
		And t2.ActionType = @ActionType

		--Update TBL_GSTR1_CDNRA_NT
		--Set flag = 'D'
		--Where ntid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_CDNRA
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_CDNRA t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.nt_num = t2.NoteNo
		--And t1.nt_dt = t2.NoteDate
		--And t2.ActionType = @ActionType

	End
	else if @ActionType = 'CDNURA'
	Begin

		Update @TBL_Values
		Set Gstin = @Gstin,
			Fp = @Fp,
			NoteNo = t2.nt_num,
			NoteDate = t2.nt_dt
		From @TBL_Values t1,
			 TBL_GSTR1_CDNURA t2,
		     TBL_GSTR1 t3
		Where t1.actiontype = @ActionType
		And t2.cdnuraid = t1.refid
		And t3.gstr1id = t2.gstr1id
		And t3.gstin = @Gstin
		And t3.fp = @Fp 

		select itmsid into #cdnura_itmsid from TBL_GSTR1_CDNURA_ITMS 
		where cdnuraid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_CDNURA_ITMS_DET 
		where itmsid in (select itmsid from #cdnura_itmsid)

								
		Delete from TBL_GSTR1_CDNURA
		where cdnuraid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete t1
		From  TBL_EXT_GSTR1_CDNURA t1,
			  @TBL_Values t2
		Where t1.gstin = t2.Gstin
		And t1.fp = t2.Fp
		And t1.nt_num = t2.NoteNo
		And t1.nt_dt = t2.NoteDate
		And t2.ActionType = @ActionType
	
		--Update TBL_GSTR1_CDNURA
		--Set flag = 'D'
		--Where cdnuraid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
	
		--Update TBL_EXT_GSTR1_CDNURA
		--Set rowstatus = 2
		--From  TBL_EXT_GSTR1_CDNURA t1,
		--	  @TBL_Values t2
		--Where t1.gstin = t2.Gstin
		--And t1.fp = t2.Fp
		--And t1.nt_num = t2.NoteNo
		--And t1.nt_dt = t2.NoteDate
		--And t2.ActionType = @ActionType


	End
	else if @ActionType = 'TXPA'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_TXPA
		From TBL_GSTR1_TXPA_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.txpaid In (Select refid From @TBL_Values
							Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_TXPA_ITMS
		Where txpaid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		Delete from TBL_GSTR1_TXPA
		Where txpaid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_TXPA)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_TXPA t1,
				 #TBL_GSTR1_TXPA t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.ad_amt = t2.ad_amt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End

		--Update TBL_GSTR1_TXPA
		--Set flag = 'D'
		--Where txpaid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_TXPA)
		--Begin
		--	Update TBL_EXT_GSTR1_TXPA
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_TXPA t1,
		--		 #TBL_GSTR1_TXPA t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.rt = t2.rt
		--	And t1.ad_amt = t2.ad_amt
		--	And t1.iamt = t2.iamt
		--	And t1.camt = t2.camt
		--	And t1.samt = t2.samt
		--	And t1.csamt = t2.csamt
		--End

	
	End
	else if @ActionType = 'ATA'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_ATA
		From TBL_GSTR1_ATA_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.ataid In (Select refid From @TBL_Values
						  Where actiontype = @ActionType)
		And t2.gstr1id = t1.gstr1id

		Delete from TBL_GSTR1_ATA_ITMS
		Where ataid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		Delete from TBL_GSTR1_ATA
		Where ataid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)

		if Exists(Select 1 From #TBL_GSTR1_ATA)
		Begin
			Delete t1
			FROM TBL_EXT_GSTR1_ATA t1,
				 #TBL_GSTR1_ATA t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.ad_amt = t2.ad_amt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End

		--Update TBL_GSTR1_ATA
		--Set flag = 'D'
		--Where ataid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)

		--if Exists(Select 1 From #TBL_GSTR1_ATA)
		--Begin
		--	Update TBL_EXT_GSTR1_ATA
		--	Set rowstatus = 2
		--	FROM TBL_EXT_GSTR1_ATA t1,
		--		 #TBL_GSTR1_ATA t2 
		--	WHERE t1.gstin= t2.gstin 
		--	And t1.fp = t2.fp
		--	And t1.rt = t2.rt
		--	And t1.ad_amt = t2.ad_amt
		--	And t1.iamt = t2.iamt
		--	And t1.camt = t2.camt
		--	And t1.samt = t2.samt
		--	And t1.csamt = t2.csamt
		--End

	
	End


	else
	Begin
		Select	@ErrorCode = -1,
				@ErrorMessage = 'Unsupported Action Type'
		Return @ErrorCode
	End


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End