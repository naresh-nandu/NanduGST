
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR2 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/24/2017	Seshadri			Initial Version
08/01/2017	Seshadri		Included Rowstatus Updation and removed the Delete Statement 
08/02/2017	Seshadri		Changed @InvoiceId to @RefId
08/29/2017	Seshadri		Modified the code to handle multiple line items in case of
							B2B,B2BUR,IMPG,IMPS,CDNR,CDNUR Action Types

*/

/* Sample Procedure Call

exec usp_Delete_GSTR2_EXT 
 */

CREATE PROCEDURE [usp_Delete_GSTR2_EXT]
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
		@BoeNo varchar(50),
		@BoeDate varchar(50),
		@NoteNo varchar(50),
		@NoteDate varchar(50),
		@RefNo varchar(50)

	if @ActionType = 'B2B'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno
		From TBL_EXT_GSTR2_B2B_INV
		Where b2bid = @RefId

		Update TBL_EXT_GSTR2_B2B_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'B2BUR'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno
		From TBL_EXT_GSTR2_B2BUR_INV
		Where b2burid = @RefId

		Update TBL_EXT_GSTR2_B2BUR_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'IMPG'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@BoeNo = boe_num,
				@BoeDate = boe_dt,
				@RefNo = referenceno
		From TBL_EXT_GSTR2_IMPG_INV
		Where impgid = @RefId

		Update TBL_EXT_GSTR2_IMPG_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And boe_num = @BoeNo
		And boe_dt = @BoeDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'IMPS'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno
		From TBL_EXT_GSTR2_IMPS_INV
		Where impsid = @RefId

		Update TBL_EXT_GSTR2_IMPS_INV
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
				@RefNo = referenceno
		From TBL_EXT_GSTR2_CDN
		Where cdnid = @RefId

		Update TBL_EXT_GSTR2_CDN
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
				@RefNo = referenceno
		From TBL_EXT_GSTR2_CDNUR
		Where cdnurid = @RefId

		Update TBL_EXT_GSTR2_CDNUR
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo 
		And nt_dt = @NoteDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'HSN'
	Begin

		Update TBL_EXT_GSTR2_HSN
		Set rowstatus = 2
		Where hsnid = @RefId

	End
	else if @ActionType = 'NIL'
	Begin

		Update TBL_EXT_GSTR2_NIL
		Set rowstatus = 2
		Where nilid = @RefId

	End
	else if @ActionType = 'TXP'
	Begin

		Update TBL_EXT_GSTR2_TXPD
		Set rowstatus = 2
		Where txpdid = @RefId

	End
	else if @ActionType = 'TXI'
	Begin

		Update TBL_EXT_GSTR2_TXI
		Set rowstatus = 2
		Where txiid = @RefId

	End
	else if @ActionType = 'ITCRVSL'
	Begin

		Update TBL_EXT_GSTR2_ITCRVSL
		Set rowstatus = 2
		Where itcrvslid = @RefId

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