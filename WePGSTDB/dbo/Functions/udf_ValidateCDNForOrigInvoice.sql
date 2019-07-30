
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate CDN For Original Invoice. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/02/2018	Seshadri 		Initial Version
07/06/2018  Karthik         Invluced EXP Invoice existance for CDNUR validation
*/

/* Sample Function Call

 Select dbo.udf_ValidateCDNForOrigInvoice('1') 

 */

CREATE FUNCTION [udf_ValidateCDNForOrigInvoice](
	@ActionType varchar(15),
	@TemplateTypeId tinyint = 1, -- 1 - Template A ; 2 - Template B
	@Gstin varchar(15),
	@InvNo	 varchar(50),
	@InvDate	 varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int

	Select @ActionType = Ltrim(Rtrim(IsNull(@ActionType,'')))
	Select @TemplateTypeId = IsNull(@TemplateTypeId,0)
	Select @InvNo = Ltrim(Rtrim(IsNull(@InvNo,'')))
	Select @InvDate = Ltrim(Rtrim(IsNull(@InvDate,'')))
	Select @RetValue = 1

	if @ActionType = 'CDNR' Or @ActionType = 'CDNUR'
	Begin

		if Exists(Select 1 From TBL_GSTR1_B2B_INV
					Where inum = @InvNo
					And idt = @InvDate
					And isnull(flag,'') = 'U'
					And gstr1id in (Select gstr1id From TBL_GSTR1 Where gstin = @Gstin)
				)
		Begin
			Select @RetValue = 1 
			return @RetValue
		End
		else if Exists(Select 1 From TBL_GSTR1_B2CL_INV
					Where inum = @InvNo
					And idt = @InvDate
					And isnull(flag,'') = 'U'
					And gstr1id in (Select gstr1id From TBL_GSTR1 Where gstin = @Gstin)
				)
		Begin
			Select @RetValue = 1 
			return @RetValue
		End
		else if Exists(Select 1 From TBL_GSTR1_B2CS_INV
					Where @TemplateTypeId = 2
					And inum = @InvNo
					And idt = @InvDate
					And isnull(flag,'') = 'U'
					And gstr1id in (Select gstr1id From TBL_GSTR1 Where gstin = @Gstin)
				)
		Begin
			Select @RetValue = 1 
			return @RetValue
		End
		-- KK on 7/6/2018
		else if Exists(Select 1 From TBL_GSTR1_EXP_INV
					Where inum = @InvNo
					And idt = @InvDate
					And isnull(flag,'') = 'U'
					And gstr1id in (Select gstr1id From TBL_GSTR1 Where gstin = @Gstin)
				)
		Begin
			Select @RetValue = 1 
			return @RetValue
		End
		else
		Begin
			Select @RetValue = -1 -- Invoice Number does not exist
			return @RetValue
		End

	End
	else
	Begin
		Select @RetValue = -2 -- Unsupported Action Type
		return @RetValue
	End

	return @RetValue

End