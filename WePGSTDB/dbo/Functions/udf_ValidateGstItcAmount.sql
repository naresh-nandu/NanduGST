
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate GST ITC Amount. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/25/2017	Seshadri 	Initial Version
10/5/2017	Seshadri	Removed the validations for ITC Tax Amount
10/6/2017	Seshadri	Fixed Vaidation Issue related to Supply Type and POS in B2B and B2BUR
10/26/2017	Seshadri	Modified to support Deemed Exports / Supplies to SEZ
						for B2B Action Type		
10/27/2017	Seshadri	Modified to handle extra document types because of template defect

				
*/

/* Sample Function Call

 Select dbo.udf_ValidateGstItcAmount('') 

 */


CREATE FUNCTION [udf_ValidateGstItcAmount](
@ActionType varchar(15),
@Gstin varchar(50),
@Ctin varchar(50),
@Pos varchar(50),
@SplyType varchar(50),
@GstRate varchar(50),
@TaxableValue varchar(50),
@IgstAmount varchar(50),
@CgstAmount varchar(50),
@SgstAmount varchar(50),
@CessAmount varchar(50),
@TxIgstAmount varchar(50),
@TxCgstAmount varchar(50),
@TxSgstAmount varchar(50),
@TxCessAmount varchar(50),
@AdAdjAmount varchar(50),
@AdPaidAmount varchar(50),
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
	Select @Pos = Ltrim(Rtrim(IsNull(@Pos,'')))
	Select @SplyType = Ltrim(Rtrim(IsNull(@SplyType,'')))
	Select @GstRate = Ltrim(Rtrim(IsNull(@GstRate,'')))
	Select @TaxableValue = Ltrim(Rtrim(IsNull(@TaxableValue,'')))
	Select @IgstAmount = Ltrim(Rtrim(IsNull(@IgstAmount,'')))
	Select @CgstAmount = Ltrim(Rtrim(IsNull(@CgstAmount,'')))
	Select @SgstAmount = Ltrim(Rtrim(IsNull(@SgstAmount,'')))
	Select @CessAmount = Ltrim(Rtrim(IsNull(@CessAmount,'')))
	Select @TxIgstAmount = Ltrim(Rtrim(IsNull(@TxIgstAmount,'')))
	Select @TxCgstAmount = Ltrim(Rtrim(IsNull(@TxCgstAmount,'')))
	Select @TxSgstAmount = Ltrim(Rtrim(IsNull(@TxSgstAmount,'')))
	Select @TxCessAmount = Ltrim(Rtrim(IsNull(@TxCessAmount,'')))
	Select @AdAdjAmount  = Ltrim(Rtrim(IsNull(@AdAdjAmount,'')))
	Select @AdPaidAmount  = Ltrim(Rtrim(IsNull(@AdPaidAmount,'')))
	Select @InvType  = Ltrim(Rtrim(IsNull(@InvType,'')))
	Select @InvNo  = Ltrim(Rtrim(IsNull(@InvNo,'')))
	Select @InvDate  = Ltrim(Rtrim(IsNull(@InvDate,'')))
	Select @PreGstRegime  = Ltrim(Rtrim(IsNull(@PreGstRegime,'')))


	Select @RetValue = 1, @RetMessage = '' 

	-- Inter State / Intra State Supply Determination

	if @ActionType = 'B2B'
	Begin

		if @Ctin <> ''
		Begin
			Select @SupplierStateCode = Convert(int,Substring(@Ctin,1,2))
		End

		if @Pos <> ''
		Begin
			Select @RecipientStateCode = Convert(int,@Pos)
		End
		else if @Gstin <> '' 
		Begin
			Select @RecipientStateCode = Convert(int,Substring(@Gstin,1,2))
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

	End
	else if @ActionType in ('B2BUR','TXP','TXI','TXPD','TAXL') 
	Begin

		if @SplyType = 'INTER'
			Select @InterStateSupply = 1
		else
			Select @InterStateSupply = 0

	End

	-- Amount Validation (Generic)

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

	if @TxIgstAmount <> '' and IsNumeric(@TxIgstAmount) <> 1	
	Begin
		Select @RetValue = -8 , @RetMessage = 'Invalid ITC Tax Amount for IGST.'
		return @RetMessage
	End 

	if @TxCgstAmount <> '' and IsNumeric(@TxCgstAmount) <> 1	
	Begin
		Select @RetValue = -9 , @RetMessage = 'Invalid ITC Tax Amount for CGST.'
		return @RetMessage
	End 

	if @TxSgstAmount <> '' and IsNumeric(@TxSgstAmount) <> 1	
	Begin
		Select @RetValue = -10 , @RetMessage = 'Invalid ITC Tax Amount for SGST.'
		return @RetMessage
	End 

	if @TxCessAmount <> '' and IsNumeric(@TxCessAmount) <> 1	
	Begin
		Select @RetValue = -11 , @RetMessage = 'Invalid ITC Tax Amount for Cess.'
		return @RetMessage
	End 

	-- Amount Applicability Validation (Generic)

	if @ActionType in ('B2B','B2BUR','TXP','TXI','TXPD','TAXL') 
	Begin

		if @ActionType in ('B2B') and @InvType in ('SEWP','SEWOP','DE')
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

		End
		else if @InterStateSupply = 1
		Begin

			if @IgstAmount = ''
			Begin
				Select @RetValue = -12 , @RetMessage = 'Igst Amount is mandatory for Inter State Supply.'
				return @RetMessage
			End	
			if @CgstAmount <> '' and Convert(dec(18,2),@CgstAmount) > 0
			Begin
				Select @RetValue = -13 , @RetMessage = 'Cgst Amount is not applicable for Inter State Supply.'
				return @RetMessage
			End	
			if @SgstAmount <> '' and Convert(dec(18,2),@SgstAmount) > 0
			Begin
				Select @RetValue = -14 , @RetMessage = 'Sgst Amount is not applicable for Inter State Supply.'
				return @RetMessage
			End	

			/*
			if @ActionType in ('B2B','B2BUR')
			Begin
				if @TxIgstAmount = ''
				Begin
					Select @RetValue = -15 , @RetMessage = 'ITC Tax Amount for IGST is mandatory for Inter State Supply.'
					return @RetMessage
				End	
				if @TxCgstAmount <> '' and Convert(dec(18,2),@TxCgstAmount) > 0
				Begin
					Select @RetValue = -13 , @RetMessage = 'ITC Tax Amount for CGST is not applicable for Inter State Supply.'
					return @RetMessage
				End
				if @TxSgstAmount <> '' and Convert(dec(18,2),@TxSgstAmount) > 0
				Begin
					Select @RetValue = -14 , @RetMessage = 'ITC Tax Amount for SGST is not applicable for Inter State Supply.'
					return @RetMessage
				End	
			End */

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
			
			/*
			if @ActionType in ('B2B','B2BUR')
			Begin	

				if @TxIgstAmount <> '' and Convert(dec(18,2),@TxIgstAmount) > 0
				Begin
					Select @RetValue = -10 , @RetMessage = 'ITC Tax Amount for IGST is not applicable for Intra State Supply.'
					return @RetMessage
				End	
				if @TxCgstAmount = ''
				Begin
					Select @RetValue = -11 , @RetMessage = 'ITC Tax Amount for CGST is mandatory for Intra State Supply.'
					return @RetMessage
				End	
				if @TxSgstAmount = ''
				Begin
					Select @RetValue = -12 , @RetMessage = 'ITC Tax Amount for SGST is mandatory for Intra State Supply.'
					return @RetMessage
				End	

			End */

		End

	End
	Else if @ActionType in ('IMPG','IMPS')
	Begin

		if @IgstAmount = ''
		Begin
			Select @RetValue = -13 , @RetMessage = 'Igst Amount is mandatory.'
			return @RetMessage
		End	
		if @CgstAmount <> '' and Convert(dec(18,2),@CgstAmount) > 0
		Begin
			Select @RetValue = -14 , @RetMessage = 'Cgst Amount is not applicable.'
			return @RetMessage
		End	
		if @SgstAmount <> '' and Convert(dec(18,2),@SgstAmount) > 0
		Begin
			Select @RetValue = -15 , @RetMessage = 'Sgst Amount is not applicable.'
			return @RetMessage
		End	

		/*
		if @TxIgstAmount = ''
		Begin
			Select @RetValue = -15 , @RetMessage = 'ITC Tax Amount for IGST is mandatory.'
			return @RetMessage
		End	
		if @TxCgstAmount <> '' and Convert(dec(18,2),@TxCgstAmount) > 0
		Begin
			Select @RetValue = -13 , @RetMessage = 'ITC Tax Amount for CGST is not applicable.'
			return @RetMessage
		End
		if @TxSgstAmount <> '' and Convert(dec(18,2),@TxSgstAmount) > 0
		Begin
			Select @RetValue = -14 , @RetMessage = 'ITC Tax Amount for SGST is not applicable.'
			return @RetMessage
		End	*/


	End

	-- Amount Computation Validation (Generic)


	if @ActionType in ('B2B','B2BUR','TXP','TXI','TXPD','TAXL') 
	Begin

		Select @Rate = Convert(dec(18,2),@GstRate) 
		if  @ActionType In ('TXP','TXPD')
			Select @TxVal = Convert(dec(18,2),@AdAdjAmount) 
		else if @ActionType In ('TXI','TAXL')
			Select @TxVal = Convert(dec(18,2),@AdPaidAmount) 
		else
			Select @TxVal = Convert(dec(18,2),@TaxableValue) 


		if @ActionType in ('B2B') and @InvType in ('SEWP','SEWOP','DE')
		Begin
			Select @IAmt = (@Rate/100) * @TxVal
			if ((@IAmt < (Convert(dec(18,2),@IgstAmount)- 1.00)) or (@IAmt > (Convert(dec(18,2),@IgstAmount) + 1.00)))
			Begin
				Select @RetValue = -13 , @RetMessage = 'Igst Amount is wrong.'
				return @RetMessage
			End
		End
		else if @InterStateSupply = 1
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
	Else if @ActionType in ('IMPG','IMPS')
	Begin

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