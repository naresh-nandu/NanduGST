
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR2 Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/01/2017	Seshadri	Initial Version
10/17/2017	Seshadri	Fixed the imapct of SA Delete
10/27/2017	Seshadri	Modified the code to delete IMPS Invoices
10/28/2017	Seshadri	Modified the code to delete CDNR,CDNUR,HSN,TXP,TXI,NIL,ITCRVSL Invoices
10/31/2017	Seshadri	Fixed B2BUR delete issue
11/29/2017	Seshadri	Fixed the CDNR issue
*/

/* Sample Procedure Call

exec usp_Delete_GSTR2_SA 
 */

CREATE PROCEDURE [usp_Delete_GSTR2_SA]
	@ActionType varchar(15),
	@RefId int,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Gstin varchar(15),
			@Fp varchar(10),
			@Ctin varchar(15),
			@Inum varchar(50),
			@Idt varchar(50),
			@Boe_Num varchar(50),
			@Boe_Dt varchar(50),
			@Nt_Num varchar(50),
			@Nt_Dt varchar(50),
			@HsnCode varchar(50),
			@HsnDesc varchar(50)


	if @ActionType = 'B2B'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
			   @Ctin = ctin,
   			   @Inum = inum,
			   @Idt = idt	
		From TBL_GSTR2_B2B_INV t1,
			 TBL_GSTR2_B2B t2,
		     TBL_GSTR2 t3
		Where t1.invid = @RefId
		And t2.b2bid = t1.b2bid
		And t3.gstr2id = t2.gstr2id

		Update TBL_GSTR2_B2B_INV
		Set flag = 'D'
		Where invid = @RefId

		Update TBL_EXT_GSTR2_B2B_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And ctin = @Ctin
		And inum = @Inum
		And idt = @Idt


	End
	else if @ActionType = 'B2BUR'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
			   @Inum = inum,
			   @Idt = idt	
		From TBL_GSTR2_B2BUR_INV t1,
			 TBL_GSTR2_B2BUR t2,
		     TBL_GSTR2 t3
		Where t1.invid = @RefId
		And t2.b2burid = t1.b2burid
		And t3.gstr2id = t2.gstr2id

		Update TBL_GSTR2_B2BUR_INV
		Set flag = 'D'
		Where invid = @RefId

		Update TBL_EXT_GSTR2_B2BUR_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @Inum
		And idt = @Idt


	End
	else if @ActionType = 'IMPG'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @Boe_Num = boe_num,
			   @Boe_Dt = boe_dt	
		From TBL_GSTR2_IMPG t1,
		     TBL_GSTR2 t2
		Where t1.impgid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_IMPG
		Set flag = 'D'
		Where impgid = @RefId

		Update TBL_EXT_GSTR2_IMPG_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And boe_num = @Boe_Num
		And boe_dt = @Boe_Dt

	End
	else if @ActionType = 'IMPS'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
			   @Inum = inum,
			   @Idt = idt	
		From TBL_GSTR2_IMPS t1,
		     TBL_GSTR2 t2
		Where t1.impsid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_IMPS
		Set flag = 'D'
		Where impsid = @RefId

		Update TBL_EXT_GSTR2_IMPS_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @Inum
		And idt = @Idt

	End
	else if @ActionType = 'CDNR'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @Nt_Num = nt_num,
			   @Nt_Dt = nt_dt	
		From TBL_GSTR2_CDNR_NT t1,
		     TBL_GSTR2 t2
		Where t1.invid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_CDNR_NT
		Set flag = 'D'
		Where invid = @RefId 

		Update TBL_EXT_GSTR2_CDN
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @Nt_Num
		And nt_dt = @Nt_Dt

	End
	else if @ActionType = 'CDNUR'
	Begin

		Select @Gstin = gstin,
		       @Fp = fp,
   			   @Nt_Num = nt_num,
			   @Nt_Dt = nt_dt		
		From TBL_GSTR2_CDNUR t1,
		     TBL_GSTR2 t2
		Where t1.cdnurid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_CDNUR
		Set flag = 'D'
		Where cdnurid = @RefId 

		Update TBL_EXT_GSTR2_CDNUR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @Nt_Num
		And nt_dt = @Nt_Dt

	End
	else if @ActionType = 'HSN'
	Begin

		Select gstin,fp,hsn_sc,[desc]
		Into #TBL_GSTR2_HSN
		From TBL_GSTR2_HSNSUM_DET t1,
		     TBL_GSTR2 t2
		Where t1.hsnsumid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_HSNSUM
		Set flag = 'D'
		Where hsnsumid = @RefId

		if Exists(Select 1 From #TBL_GSTR2_HSN)
		Begin
			Update TBL_EXT_GSTR2_HSN
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_HSN t1,
				 #TBL_GSTR2_HSN t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.hsn_sc = t2.hsn_sc
			And t1.descs = t2.[desc]
		End

	End
	else if @ActionType = 'NIL'
	Begin

		Select gstin,fp,cpddr,exptdsply,ngsply,nilsply
		Into #TBL_GSTR2_NIL_INTER
		From TBL_GSTR2_NIL_INTER t1,
		     TBL_GSTR2 t2
		Where t1.nilid = @RefId
		And t2.gstr2id = t1.gstr2id

		Select gstin,fp,cpddr,exptdsply,ngsply,nilsply
		Into #TBL_GSTR2_NIL_INTRA
		From TBL_GSTR2_NIL_INTRA t1,
		     TBL_GSTR2 t2
		Where t1.nilid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_NIL
		Set flag = 'D'
		Where nilid = @RefId

		if Exists(Select 1 From #TBL_GSTR2_NIL_INTER)
		Begin
			Update TBL_EXT_GSTR2_NIL
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_NIL t1,
				 #TBL_GSTR2_NIL_INTER t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.cpddr = t2.cpddr
			And t1.exptdsply = t2.exptdsply
			And t1.ngsply = t2.ngsply
			And t1.nilsply = t2.nilsply
			And t1.niltype = 'INTER'
		End

		if Exists(Select 1 From #TBL_GSTR2_NIL_INTRA)
		Begin
			Update TBL_EXT_GSTR2_NIL
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_NIL t1,
				 #TBL_GSTR2_NIL_INTRA t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.cpddr = t2.cpddr
			And t1.exptdsply = t2.exptdsply
			And t1.ngsply = t2.ngsply
			And t1.nilsply = t2.nilsply
			And t1.niltype = 'INTRA'
		End

	End
	else if @ActionType = 'TXP'
	Begin

		Select gstin,fp,rt,adamt,iamt,camt,samt,csamt
		Into #TBL_GSTR2_TXPD
		From TBL_GSTR2_TXPD_ITMS t1,
		     TBL_GSTR2 t2
		Where t1.txpdid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_TXPD
		Set flag = 'D'
		Where txpdid = @RefId

		if Exists(Select 1 From #TBL_GSTR2_TXPD)
		Begin
			Update TBL_EXT_GSTR2_TXPD
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_TXPD t1,
				 #TBL_GSTR2_TXPD t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.adamt = t2.adamt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End
	
	End
	else if @ActionType = 'TXI'
	Begin

		Select gstin,fp,rt,adamt,iamt,camt,samt,csamt
		Into #TBL_GSTR2_TXI
		From TBL_GSTR2_TXI_ITMS t1,
		     TBL_GSTR2 t2
		Where t1.txiid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_TXI
		Set flag = 'D'
		Where txiid = @RefId

		if Exists(Select 1 From #TBL_GSTR2_TXI)
		Begin
			Update TBL_EXT_GSTR2_TXI
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_TXI t1,
				 #TBL_GSTR2_TXI t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.rt = t2.rt
			And t1.adamt = t2.adamt
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
		End

	End
	else if @ActionType = 'ITCRVSL'
	Begin

		Select gstin,fp,rulename,iamt,camt,samt,csamt
		Into #TBL_GSTR2_ITCRVSL
		From TBL_GSTR2_ITCRVSL_ITMS t1,
		     TBL_GSTR2 t2
		Where t1.itcrvslid = @RefId
		And t2.gstr2id = t1.gstr2id

		Update TBL_GSTR2_ITCRVSL
		Set flag = 'D'
		Where itcrvslid = @RefId

		if Exists(Select 1 From #TBL_GSTR2_ITCRVSL)
		Begin
			Update TBL_EXT_GSTR2_ITCRVSL
			Set rowstatus = 2
			FROM TBL_EXT_GSTR2_ITCRVSL t1,
				 #TBL_GSTR2_ITCRVSL t2 
			WHERE t1.gstin= t2.gstin 
			And t1.fp = t2.fp
			And t1.ruletype = t2.rulename
			And t1.iamt = t2.iamt
			And t1.camt = t2.camt
			And t1.samt = t2.samt
			And t1.csamt = t2.csamt
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