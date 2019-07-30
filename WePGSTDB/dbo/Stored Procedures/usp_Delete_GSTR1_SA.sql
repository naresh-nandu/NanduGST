
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR1 Records from the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/01/2017	Seshadri		Initial Version
08/06/2017	Seshadri		Added Extra Parameters
08/30/2017	Seshadri	Fix the issue wrt HSN Delete
09/01/2017	Seshadri	Fix the delete issue wrt SA and External reference
09/03/2017	Seshadri	Fixed the imapct of SA Delete
*/

/* Sample Procedure Call

exec usp_Delete_GSTR1_SA 
 */

CREATE PROCEDURE [usp_Delete_GSTR1_SA]
	@ActionType varchar(15),
	@RefId int,
	@GstinId int = Null,
	@InvoiceNo varchar(50) = Null,
	@InvoiceDate varchar(50) = Null,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Gstin varchar(15),
			@Fp varchar(10),
			@Pos varchar(2),
			@Rt decimal(18,2),
			@NoteNo varchar(50),
			@NoteDate varchar(50),
			@HsnCode varchar(50),
			@HsnDesc varchar(50)


	if @ActionType = 'B2B'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = invid	
			From TBL_GSTR1_B2B_INV
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End
	
		Select @Gstin = gstin,
		       @Fp = fp,
   			   @InvoiceNo = inum,
			   @InvoiceDate = idt	
		From TBL_GSTR1_B2B_INV t1,
		     TBL_GSTR1 t2
		Where t1.invid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_B2B_INV
		Set flag = 'D'
		Where invid = @RefId 

		Update TBL_EXT_GSTR1_B2B_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvoiceNo
		And idt = @InvoiceDate

	End
	else if @ActionType = 'B2CL'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = invid	
			From TBL_GSTR1_B2CL_INV
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @InvoiceNo = inum,
			   @InvoiceDate = idt	
		From TBL_GSTR1_B2CL_INV t1,
		     TBL_GSTR1 t2
		Where t1.invid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_B2CL_INV
		Set flag = 'D'
		Where invid = @RefId 

		Update TBL_EXT_GSTR1_B2CL_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvoiceNo
		And idt = @InvoiceDate


	End
	else if @ActionType = 'B2CS'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
			   @Pos = pos,
			   @Rt = rt	
		From TBL_GSTR1_B2CS t1,
		     TBL_GSTR1 t2
		Where t1.b2csid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_B2CS
		Set flag = 'D'
		Where b2csid = @RefId
		
		Update TBL_EXT_GSTR1_B2CS
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And pos = @Pos
		And rt = @Rt 

	End
	else if @ActionType = 'EXP'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @InvoiceNo = inum,
			   @InvoiceDate = idt	
		From TBL_GSTR1_EXP_INV t1,
		     TBL_GSTR1 t2
		Where t1.invid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_EXP_INV
		Set flag = 'D'
		Where invid = @RefId

		Update TBL_EXT_GSTR1_EXP_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvoiceNo
		And idt = @InvoiceDate
		
	End
	else if @ActionType = 'CDNR'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = cdnrid
			From TBL_GSTR1_CDNR_NT
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @NoteNo = nt_num,
			   @NoteDate = nt_dt	
		From TBL_GSTR1_CDNR_NT t1,
		     TBL_GSTR1 t2
		Where t1.ntid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_CDNR_NT
		Set flag = 'D'
		Where ntid = @RefId 

		Update TBL_EXT_GSTR1_CDNR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo
		And nt_dt = @NoteDate

	End
	else if @ActionType = 'CDNUR'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @NoteNo = nt_num,
			   @NoteDate = nt_dt	
		From TBL_GSTR1_CDNUR t1,
		     TBL_GSTR1 t2
		Where t1.cdnurid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_CDNUR
		Set flag = 'D'
		Where cdnurid = @RefId 

		Update TBL_EXT_GSTR1_CDNUR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo
		And nt_dt = @NoteDate

	End
	else if @ActionType = 'HSN'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @HsnCode = hsn_sc,
			   @HsnDesc = [desc]	
		From TBL_GSTR1_HSN_DATA t1,
		     TBL_GSTR1 t2
		Where t1.dataid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_HSN_DATA
		Set flag = 'D'
		Where dataid = @RefId 

		Update TBL_EXT_GSTR1_HSN
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And hsn_sc = @HsnCode
		And descs = @HsnDesc

	End
	else if @ActionType = 'NIL'
	Begin

		Select gstin,fp,nil_amt,expt_amt,ngsup_amt,sply_ty
		Into #TBL_GSTR1_NIL
		From TBL_GSTR1_NIL_INV t1,
		     TBL_GSTR1 t2
		Where t1.nilid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_NIL
		Set flag = 'D'
		Where nilid = @RefId
		
		if Exists(Select 1 From #TBL_GSTR1_NIL)
		Begin
			Update TBL_EXT_GSTR1_NIL
			Set rowstatus = 2
			FROM TBL_EXT_GSTR1_NIL t1,
				 #TBL_GSTR1_NIL t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.nil_amt = t2.nil_amt
			And t1.expt_amt = t2.expt_amt
			And t1.ngsup_amt = t2.ngsup_amt
			And t1.sply_ty = t2.sply_ty
		End
	 

	End
	else if @ActionType = 'TXP'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_TXP
		From TBL_GSTR1_TXP_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.txpid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_TXP
		Set flag = 'D'
		Where txpid = @RefId

		if Exists(Select 1 From #TBL_GSTR1_TXP)
		Begin
			Update TBL_EXT_GSTR1_TXP
			Set rowstatus = 2
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
	
	
	End
	else if @ActionType = 'AT'
	Begin

		Select gstin,fp,rt,ad_amt,iamt,camt,samt,csamt
		Into #TBL_GSTR1_AT
		From TBL_GSTR1_AT_ITMS t1,
		     TBL_GSTR1 t2
		Where t1.atid = @RefId
		And t2.gstr1id = t1.gstr1id

	
		Update TBL_GSTR1_AT
		Set flag = 'D'
		Where atid = @RefId

		if Exists(Select 1 From #TBL_GSTR1_AT)
		Begin
			Update TBL_EXT_GSTR1_AT
			Set rowstatus = 2
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
	
	End
	else if @ActionType = 'DOCISSUE'
	Begin

		Select gstin,fp,num,[from],[to],totnum,cancel,net_issue
		Into #TBL_GSTR1_DOC
		From TBL_GSTR1_DOCS t1,
		     TBL_GSTR1 t2
		Where t1.docissueid = @RefId
		And t2.gstr1id = t1.gstr1id

		Update TBL_GSTR1_DOC_ISSUE
		Set flag = 'D'
		Where docissueid = @RefId

		if Exists(Select 1 From #TBL_GSTR1_DOC)
		Begin
			Update TBL_EXT_GSTR1_DOC
			Set rowstatus = 2
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