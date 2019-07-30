
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR1 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/24/2017	Seshadri		Initial Version
08/01/2017	Seshadri		Included Rowstatus Updation and removed the Delete Statement 
08/02/2017	Seshadri		Changed @InvoiceId to @RefId
08/29/2017	Seshadri		Modified the code to handle multiple line items in case of
							B2B,B2CL,EXP,CDNR,CDNUR Action Types
09/03/2017	Seshadri		Modified the code to handle staging area deletion
12/18/2017	Seshadri		Modified the code to handle B2CS Invoice deletion

*/

/* Sample Procedure Call

exec usp_Delete_GSTR1_EXT 
 */

CREATE PROCEDURE [usp_Delete_GSTR1_EXT]
	@ActionType varchar(15),
	@RefId int,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare	@Gstin varchar(15),
			@Fp varchar(10),
			@InvNo varchar(50),
			@InvDate varchar(50),
			@NoteNo varchar(50),
			@NoteDate varchar(50),
			@RefNo varchar(50),
			@RowStatus tinyint,
			@Flag varchar(1),

			@NilAmt decimal(18,2),
			@ExptAmt decimal(18,2),
			@NgsupAmt decimal(18,2),
			@SplyTyp varchar(50)

	if @ActionType = 'B2B'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_B2B_INV
		Where invid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_B2B_INV 
			Where inum = @InvNo
			And	idt = @InvDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_B2B_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'B2CL'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_B2CL_INV
		Where invid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_B2CL_INV 
			Where inum = @InvNo
			And	idt = @InvDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_B2CL_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'B2CS'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_B2CS_INV
		Where invid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_B2CS_INV 
			Where inum = @InvNo
			And	idt = @InvDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_B2CS_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	
	End
	else if @ActionType = 'EXP'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_EXP_INV
		Where invid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_EXP_INV 
			Where inum = @InvNo
			And	idt = @InvDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_EXP_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo


	End
	else if @ActionType = 'CDNR'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@NoteNo = nt_num,
				@NoteDate = nt_dt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_CDNR
		Where cdnrid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_CDNR_NT 
			Where nt_num = @NoteNo
			And	nt_dt = @NoteDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_CDNR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo 
		And nt_dt = @NoteDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'CDNUR'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@NoteNo = nt_num,
				@NoteDate = nt_dt,
				@RefNo = referenceno,
				@RowStatus = rowstatus
		From TBL_EXT_GSTR1_CDNUR
		Where cdnurid = @RefId

		if @RowStatus = 0 
		Begin

			Set @Flag = 'Z' -- Initialization

			Select @Flag = IsNull(flag,'')
			From TBL_GSTR1_CDNUR 
			Where nt_num = @NoteNo
			And	nt_dt = @NoteDate
			And gstr1id = (Select gstr1id From TBL_GSTR1 
							Where gstin = @Gstin
							And fp = @Fp) 
			if @Flag = ''
			Begin
				Select @ErrorCode = -2, @ErrorMessage = 'Invoice Uploaded to Staging Area'
				Return @ErrorCode
			End
			else if @Flag = 'U'
			Begin
				Select @ErrorCode = -3, @ErrorMessage = 'Invoice Uploaded to GSTN Server'
				Return @ErrorCode
			End 

		End

		Update TBL_EXT_GSTR1_CDNUR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo 
		And nt_dt = @NoteDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'HSN'
	Begin

		Update TBL_EXT_GSTR1_HSN
		Set rowstatus = 2
		Where hsnid = @RefId

	End
	else if @ActionType = 'NIL'
	Begin

		Update TBL_EXT_GSTR1_NIL
		Set rowstatus = 2
		Where nilid = @RefId
	
	End
	else if @ActionType = 'TXP'
	Begin

		Update TBL_EXT_GSTR1_TXP
		Set rowstatus = 2
		Where txpid = @RefId

	End
	else if @ActionType = 'AT'
	Begin

		Update TBL_EXT_GSTR1_AT
		Set rowstatus = 2
		Where atid = @RefId

	End
	else if @ActionType = 'DOCISSUE'
	Begin

		Update TBL_EXT_GSTR1_DOC
		Set rowstatus = 2
		Where docid = @RefId
	
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