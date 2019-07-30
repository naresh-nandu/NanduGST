
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate GST Amount. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/25/2017	Seshadri 	Initial Version
08/28/2017	Seshadri	Modified to support Deemed Exports / Supplies to SEZ
						for B2B Action Type		
08/29/2017	Seshadri	Modified to support Deemed Exports / Supplies to SEZ
						for CDNR Action Type	
08/30/2017	Seshadri	Modified to support Deemed Exports / Supplies to SEZ
						for CDNR Action Type
08/31/2017	Seshadri	Fixed SubQuery Issue
09/08/2017	Seshadri	Removed Amount Validation For Inter / Intra State in case of CDNR &
						Pre-Gst = 'Y' 
09/12/2017	Seshadri	Corrected the state code for Other Territory
02/05/2018	Seshadri	Removed the Validation for SEWOP in case of B2B & CDNR
02/06/2018	Seshadri	Removed Amount Validation in case of CDNR &
						Pre-Gst = 'Y' 
02/08/2018	Seshadri	Made Code Changes for MindTree with respect to Invoice Type in CDNR
03/10/2018	Seshadri	Made code changes related to Amendments

				
*/

/* Sample Function Call

 Select dbo.udf_ValidateGstAmount('') 

 */


CREATE FUNCTION [udf_ValidateGstAmount](
@ActionType varchar(15),
@Gstin varchar(50),
@Ctin varchar(50),
@Etin varchar(50),
@Pos varchar(50),
@GstRate varchar(50),
@TaxableValue varchar(50),
@IgstAmount varchar(50),
@CgstAmount varchar(50),
@SgstAmount varchar(50),
@CessAmount varchar(50),
@AdAdjAmount varchar(50),
@AdRecvdAmount varchar(50),
@InvType varchar(50),
@InvNo	 varchar(50),
@InvDate	 varchar(50),
@PreGstRegime      varchar(50)
 )
Returns varchar(255)
as
Begin

	Declare @RetValue int,
			@RetMessage varchar(255),
			@SupplierStateCode tinyint,
			@RecipientStateCode tinyint,
			@InterStateSupply bit,
			@Rate dec(18,2),
			@TxVal dec(18,2),
			@IAmt dec(18,2),
			@CAmt dec(18,2),
			@SAmt dec(18,2),
			@CsAmt dec(18,2)
			 


	Select @ActionType = Ltrim(Rtrim(IsNull(@ActionType,'')))
	Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	Select @Ctin = Ltrim(Rtrim(IsNull(@Ctin,'')))
	Select @Etin = Ltrim(Rtrim(IsNull(@Etin,'')))
	Select @Pos = Ltrim(Rtrim(IsNull(@Pos,'')))
	Select @GstRate = Ltrim(Rtrim(IsNull(@GstRate,'')))
	Select @TaxableValue = Ltrim(Rtrim(IsNull(@TaxableValue,'')))
	Select @IgstAmount = Ltrim(Rtrim(IsNull(@IgstAmount,'')))
	Select @CgstAmount = Ltrim(Rtrim(IsNull(@CgstAmount,'')))
	Select @SgstAmount = Ltrim(Rtrim(IsNull(@SgstAmount,'')))
	Select @CessAmount = Ltrim(Rtrim(IsNull(@CessAmount,'')))
	Select @AdAdjAmount  = Ltrim(Rtrim(IsNull(@AdAdjAmount,'')))
	Select @AdRecvdAmount  = Ltrim(Rtrim(IsNull(@AdRecvdAmount,'')))
	Select @InvType  = Ltrim(Rtrim(IsNull(@InvType,'')))
	Select @InvNo  = Ltrim(Rtrim(IsNull(@InvNo,'')))
	Select @InvDate  = Ltrim(Rtrim(IsNull(@InvDate,'')))
	Select @PreGstRegime  = Ltrim(Rtrim(IsNull(@PreGstRegime,'')))


	Select @RetValue = 1, @RetMessage = ''

	if @ActionType In ('CDNR','CDNRA') and @PreGstRegime = 'Y'
	Begin
		return @RetMessage
	End


	if @ActionType = 'CDNR'
	Begin

		Select distinct @InvType = inv_typ
		From TBL_EXT_GSTR1_B2B_INV
		Where inum = @InvNo
		And idt = @InvDate
		And gstin = @Gstin

		if Ltrim(Rtrim(IsNull(@InvType,''))) = ''
		Begin
			Select @InvType = inv_typ
			From TBL_GSTR1_B2B_INV
			Where inum = @InvNo
			And idt = @InvDate
			And gstinid = (Select gstinid From TBL_Cust_GSTIN Where GstinNo = @Gstin And rowstatus = 1)
		End

		if Exists (Select 1 from Tbl_cust_gstin 
					Where custid = 149
					And GstinNo = @Gstin And rowstatus = 1)
		Begin
			Select @InvType = 'SEWP'
		End

	End
	else if @ActionType = 'CDNRA'
	Begin

		Select distinct @InvType = inv_typ
		From TBL_EXT_GSTR1_B2BA_INV
		Where inum = @InvNo
		And idt = @InvDate
		And gstin = @Gstin

		if Ltrim(Rtrim(IsNull(@InvType,''))) = ''
		Begin
			Select @InvType = inv_typ
			From TBL_GSTR1_B2BA_INV
			Where inum = @InvNo
			And idt = @InvDate
			And gstinid = (Select gstinid From TBL_Cust_GSTIN Where GstinNo = @Gstin And rowstatus = 1)
		End

		if Exists (Select 1 from Tbl_cust_gstin 
					Where custid = 149
					And GstinNo = @Gstin And rowstatus = 1)
		Begin
			Select @InvType = 'SEWP'
		End

	End

	if @ActionType in ('B2B','CDNR','B2BA','CDNRA') and @InvType in ('SEWP','SEWOP','DE')
	Begin

	    if @InvType in ('SEWP','DE')
		Begin

			if @IgstAmount = ''
			Begin
				Select @RetValue = -1 , @RetMessage = 'Igst Amount is mandatory for Deemed Exports / Supplies to SEZ.'
				return @RetMessage
			End	
			if @CgstAmount <> '' and Convert(dec(18,2),@CgstAmount) > 0
			Begin
				Select @RetValue = -2 , @RetMessage = 'Cgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				return @RetMessage
			End	
			if @SgstAmount <> '' and Convert(dec(18,2),@SgstAmount) > 0
			Begin
				Select @RetValue = -3 , @RetMessage = 'Sgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				return @RetMessage
			End	
			if @IgstAmount <> '' and IsNumeric(@IgstAmount) <> 1	
			Begin
				Select @RetValue = -4 , @RetMessage = 'Invalid Igst Amount.'
				return @RetMessage
			End 

			Select @Rate = Convert(dec(18,2),@GstRate) 
			Select @TxVal = Convert(dec(18,2),@TaxableValue) 

			Select @IAmt = (@Rate/100) * @TxVal
			if ((@IAmt < (Convert(dec(18,2),@IgstAmount)- 1.00)) or (@IAmt > (Convert(dec(18,2),@IgstAmount) + 1.00)))
			Begin
				Select @RetValue = -5 , @RetMessage = 'Igst Amount is wrong.'
				return @RetMessage
			End

		End
	End
	else if @ActionType in ('B2B','B2CL','B2CS','CDNR','TXP','AT',
							'B2BA','B2CLA','B2CSA','CDNRA','TXPA','ATA') -- B2B (Regular Invoice)
	Begin

		if @Gstin <> ''
		Begin
			Select @SupplierStateCode = Convert(int,Substring(@Gstin,1,2))
		End

		if @Pos <> ''
		Begin
			Select @RecipientStateCode = Convert(int,@Pos)
		End
		else if @Ctin <> '' 
		Begin
			Select @RecipientStateCode = Convert(int,Substring(@Ctin,1,2))
		End
		else if @Etin <> '' 
		Begin
			Select @RecipientStateCode = Convert(int,Substring(@Etin,1,2))
		End

		if (@SupplierStateCode <= 0 or @SupplierStateCode  > 37) and (@SupplierStateCode <> 97)
		Begin
			Select @RetValue = -1 , @RetMessage = 'Invalid Supplier State Code.'
			return @RetMessage
		End

		if (@RecipientStateCode <= 0 or @RecipientStateCode  > 37) and (@RecipientStateCode <> 97)
		Begin
			Select @RetValue = -2, @RetMessage = 'Invalid Recipient State Code.'
			return @RetMessage
		End 
	
		if @SupplierStateCode != @RecipientStateCode
			Select @InterStateSupply = 1
		else
			Select @InterStateSupply = 0

		if  @ActionType In ('B2CL','B2CLA') and  @InterStateSupply = 0
		Begin
			Select @RetValue = -3 , @RetMessage = 'Not applicable for Intra State Supply.'
			return @RetMessage
		End

		if @IgstAmount <> '' and IsNumeric(@IgstAmount) <> 1	
		Begin
			Select @RetValue = -3 , @RetMessage = 'Invalid Igst Amount.'
			return @RetMessage
		End 

		if @CgstAmount <> '' and IsNumeric(@CgstAmount) <> 1	
		Begin
			Select @RetValue = -4 , @RetMessage = 'Invalid Cgst Amount.'
			return @RetMessage
		End 

		if @SgstAmount <> '' and IsNumeric(@SgstAmount) <> 1	
		Begin
			Select @RetValue = -5 , @RetMessage = 'Invalid Sgst Amount.'
			return @RetMessage
		End 

		if @CessAmount <> '' and IsNumeric(@CessAmount) <> 1	
		Begin
			Select @RetValue = -6 , @RetMessage = 'Invalid Cess Amount.'
			return @RetMessage
		End 

		if (@ActionType Not In ('CDNR','CDNRA')) Or (@ActionType In ('CDNR','CDNRA') and @PreGstRegime = 'N')
		Begin

			if @InterStateSupply = 1
			Begin
				if @IgstAmount = ''
				Begin
					Select @RetValue = -7 , @RetMessage = 'Igst Amount is mandatory for Inter State Supply.'
					return @RetMessage
				End	
				if @CgstAmount <> '' and Convert(dec(18,2),@CgstAmount) > 0
				Begin
					Select @RetValue = -8 , @RetMessage = 'Cgst Amount is not applicable for Inter State Supply.'
					return @RetMessage
				End	
				if @SgstAmount <> '' and Convert(dec(18,2),@SgstAmount) > 0
				Begin
					Select @RetValue = -9 , @RetMessage = 'Sgst Amount is not applicable for Inter State Supply.'
					return @RetMessage
				End	
			End
			else
			Begin
				if @IgstAmount <> '' and Convert(dec(18,2),@IgstAmount) > 0
				Begin
					Select @RetValue = -10 , @RetMessage = 'Igst Amount is not applicable for Intra State Supply.'
					return @RetMessage
				End	
				if @CgstAmount = ''
				Begin
					Select @RetValue = -11 , @RetMessage = 'Cgst Amount is mandatory for Intra State Supply.'
					return @RetMessage
				End	
				if @SgstAmount = ''
				Begin
					Select @RetValue = -12 , @RetMessage = 'Sgst Amount is mandatory for Intra State Supply.'
					return @RetMessage
				End	
			End

		End

		Select @Rate = Convert(dec(18,2),@GstRate) 
		if  @ActionType In('TXP','TXPA')
			Select @TxVal = Convert(dec(18,2),@AdAdjAmount) 
		else if @ActionType In ('AT','ATA')
			Select @TxVal = Convert(dec(18,2),@AdRecvdAmount) 
		else
			Select @TxVal = Convert(dec(18,2),@TaxableValue) 

		if @InterStateSupply = 1
		Begin
			Select @IAmt = (@Rate/100) * @TxVal
			if ((@IAmt < (Convert(dec(18,2),@IgstAmount)- 1.00)) or (@IAmt > (Convert(dec(18,2),@IgstAmount) + 1.00)))
			Begin
				Select @RetValue = -13 , @RetMessage = 'Igst Amount is wrong.'
				return @RetMessage
			End
		End
		else
		Begin
			Select @CAmt = (@Rate/200) * @TxVal
			Select @SAmt = (@Rate/200) * @TxVal
			if ((@CAmt < (Convert(dec(18,2),@CgstAmount)- 1.00)) or (@CAmt > (Convert(dec(18,2),@CgstAmount) + 1.00)))
			Begin
				Select @RetValue = -14 , @RetMessage = 'Cgst Amount is wrong.'
				return @RetMessage
			End
			if ((@SAmt < (Convert(dec(18,2),@SgstAmount)- 1.00)) or (@SAmt > (Convert(dec(18,2),@SgstAmount) + 1.00))) 
			Begin
				Select @RetValue = -15  , @RetMessage = 'Sgst Amount is wrong.'
				return @RetMessage
			End
		End

	End
	Else if @ActionType in ('EXP','CDNUR','EXPA','CDNURA')
	Begin

		if @IgstAmount = ''
		Begin
			Select @RetValue = -1 , @RetMessage = 'Igst Amount is mandatory.'
			return @RetMessage
		End	

		if @CgstAmount <> '' and Convert(dec(18,2),@CgstAmount) > 0
		Begin
			Select @RetValue = -2 , @RetMessage = 'Cgst Amount is not applicable.'
			return @RetMessage
		End	
		if @SgstAmount <> '' and Convert(dec(18,2),@SgstAmount) > 0
		Begin
			Select @RetValue = -3 , @RetMessage = 'Sgst Amount is not applicable.'
			return @RetMessage
		End	

		if @IgstAmount <> '' and IsNumeric(@IgstAmount) <> 1	
		Begin
			Select @RetValue = -4 , @RetMessage = 'Invalid Igst Amount.'
			return @RetMessage
		End 

		Select @Rate = Convert(dec(18,2),@GstRate) 
		Select @TxVal = Convert(dec(18,2),@TaxableValue) 

		Select @IAmt = (@Rate/100) * @TxVal
		if ((@IAmt < (Convert(dec(18,2),@IgstAmount)- 1.00)) or (@IAmt > (Convert(dec(18,2),@IgstAmount) + 1.00)))
		Begin
			Select @RetValue = -5 , @RetMessage = 'Igst Amount is wrong.'
			return @RetMessage
		End

	End

	return @RetMessage

End