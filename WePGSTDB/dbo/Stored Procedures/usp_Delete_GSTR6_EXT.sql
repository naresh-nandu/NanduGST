
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR2 Records from the corresponding external tables
				
Written by  : muskan.garg@wepdigital.com 

Date		Who			Decription 
05/17/2017	Muskan      Initial Version


*/

/* Sample Procedure Call

exec usp_Delete_GSTR2_EXT 
 */

CREATE PROCEDURE [dbo].[usp_Delete_GSTR6_EXT]
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
		From TBL_EXT_GSTR6_B2B_INV
		Where b2bid = @RefId

		Update TBL_EXT_GSTR6_B2B_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'B2BA'
	Begin

		Select	@Gstin = gstin,
				@Fp = fp,
				@InvNo = inum,
				@InvDate = idt,
				@RefNo = referenceno
		From TBL_EXT_GSTR6_B2BA_INV
		Where b2baid = @RefId

		Update TBL_EXT_GSTR6_B2BA_INV
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And inum = @InvNo
		And idt = @InvDate
		And referenceno = @RefNo

	End

	else if @ActionType = 'CDN'
	Begin

			Select	@Gstin = gstin,
				@Fp = fp,
				@NoteNo = nt_num,
				@NoteDate = nt_dt,
				@RefNo = referenceno
		From TBL_EXT_GSTR6_CDN
		Where cdnid = @RefId

		Update TBL_EXT_GSTR6_CDN
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo 
		And nt_dt = @NoteDate
		And referenceno = @RefNo

	End
	else if @ActionType = 'CDNA'
	Begin

			Select	@Gstin = gstin,
				@Fp = fp,
				@NoteNo = nt_num,
				@NoteDate = nt_dt,
				@RefNo = referenceno
		From TBL_EXT_GSTR6_CDNA
		Where cdnaid = @RefId

		Update TBL_EXT_GSTR6_CDNA
		Set rowstatus = 2
		Where gstin = @Gstin
		And fp = @Fp
		And nt_num = @NoteNo 
		And nt_dt = @NoteDate
		And referenceno = @RefNo

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