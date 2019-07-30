
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the GSTR1_D Records from the corresponding GSTR1 Download tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/21/2017	Seshadri		Initial Version
08/24/2017	Seshadri	Modified the code to include CDNUR and DOCISSUE Action Type
*/

/* Sample Procedure Call

exec usp_Delete_GSTR1_D_SA 
 */

CREATE PROCEDURE [usp_Delete_GSTR1_D_SA]
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

	if @ActionType = 'B2B'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = invid
			From TBL_GSTR1_D_B2B_INV
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End

		Update TBL_GSTR1_D_B2B_INV
		Set flag = 'D'
		Where invid = @RefId 

	End
	else if @ActionType = 'B2CL'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = invid
			From TBL_GSTR1_D_B2CL_INV
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End

		Update TBL_GSTR1_D_B2CL_INV
		Set flag = 'D'
		Where invid = @RefId 

	End
	else if @ActionType = 'B2CS'
	Begin
		Update TBL_GSTR1_D_B2CS
		Set flag = 'D'
		Where b2csid = @RefId 
	End
	else if @ActionType = 'EXP'
	Begin
		Update TBL_GSTR1_D_EXP_INV
		Set flag = 'D'
		Where invid = @RefId 
	End
	else if @ActionType = 'CDNR'
	Begin

		If @RefId <= 0
		Begin
			Select @RefId = ntid
			From TBL_GSTR1_D_CDNR_NT
			Where gstinid = @GstinId
			And inum = @InvoiceNo
			And idt = @InvoiceDate
		End

		Update TBL_GSTR1_D_CDNR_NT
		Set flag = 'D'
		Where ntid = @RefId 

	End
	else if @ActionType = 'CDNUR'
	Begin
		Update TBL_GSTR1_D_CDNUR
		Set flag = 'D'
		Where cdnurid = @RefId 
	End
	else if @ActionType = 'HSN'
	Begin
		Update TBL_GSTR1_D_HSN
		Set flag = 'D'
		Where hsnid = @RefId 
	End
	else if @ActionType = 'NIL'
	Begin
		Update TBL_GSTR1_D_NIL
		Set flag = 'D'
		Where nilid = @RefId 
	End
	else if @ActionType = 'TXP'
	Begin
		Update TBL_GSTR1_D_TXPD
		Set flag = 'D'
		Where txpdid = @RefId
	End
	else if @ActionType = 'AT'
	Begin
		Update TBL_GSTR1_D_AT
		Set flag = 'D'
		Where atid = @RefId
	End	
	else if @ActionType = 'DOCISSUE'
	Begin
		Update TBL_GSTR1_D_DOC_ISSUE
		Set flag = 'D'
		Where docissueid = @RefId
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