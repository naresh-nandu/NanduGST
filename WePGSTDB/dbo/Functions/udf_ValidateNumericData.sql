
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Numeric Data. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who					Decription 
02/19/2018	Seshadri & Karthik 	Initial Version
				
*/

/* Sample Function Call

 Select dbo.udf_ValidateNumericData('') 

 */


CREATE FUNCTION [udf_ValidateNumericData](
@Gt varchar(50),
@CurGt varchar(50),
@Val varchar(50),
@GstRate varchar(50),
@TaxableValue varchar(50),
@IgstAmount varchar(50),
@CgstAmount varchar(50),
@SgstAmount varchar(50),
@CessAmount varchar(50),
@Qty varchar(50),
@UnitPrice varchar(50),
@TotVal varchar(50),
@AdAdjAmount varchar(50),
@AdRecvdAmount varchar(50),
@NilAmt varchar(50),
@ExptAmt varchar(50),
@NgsupAmt varchar(50)
 )
Returns varchar(255)
as
Begin

	Declare @RetValue int,
			@RetMessage varchar(255)
		
	Select @Gt = Ltrim(Rtrim(IsNull(@Gt,'')))
	Select @CurGt = Ltrim(Rtrim(IsNull(@CurGt,'')))
	Select @Val = Ltrim(Rtrim(IsNull(@Val,'')))
	Select @GstRate = Ltrim(Rtrim(IsNull(@GstRate,'')))
	Select @TaxableValue = Ltrim(Rtrim(IsNull(@TaxableValue,'')))
	Select @IgstAmount = Ltrim(Rtrim(IsNull(@IgstAmount,'')))
	Select @CgstAmount = Ltrim(Rtrim(IsNull(@CgstAmount,'')))
	Select @SgstAmount = Ltrim(Rtrim(IsNull(@SgstAmount,'')))
	Select @CessAmount = Ltrim(Rtrim(IsNull(@CessAmount,'')))
	Select @Qty  = Ltrim(Rtrim(IsNull(@Qty,'')))
	Select @UnitPrice  = Ltrim(Rtrim(IsNull(@UnitPrice,'')))
	Select @TotVal = Ltrim(Rtrim(IsNull(@TotVal,'')))
	Select @AdAdjAmount  = Ltrim(Rtrim(IsNull(@AdAdjAmount,'')))
	Select @AdRecvdAmount  = Ltrim(Rtrim(IsNull(@AdRecvdAmount,'')))
	Select @NilAmt  = Ltrim(Rtrim(IsNull(@NilAmt,'')))
	Select @ExptAmt = Ltrim(Rtrim(IsNull(@ExptAmt,'')))
	Select @NgsupAmt = Ltrim(Rtrim(IsNull(@NgsupAmt,'')))
	

	Select @RetValue = 1, @RetMessage = ''

	if @Gt <> '' and IsNumeric(@Gt) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Gross Turnover is not Numeric'
		return @RetMessage
	End
	else if @CurGt <> '' and IsNumeric(@CurGt) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Current Gross Turnover is not Numeric'
		return @RetMessage
	End
	else if @Val <> '' and IsNumeric(@Val) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Invoice Value is not Numeric'
		return @RetMessage
	End
	else if @GstRate <> '' and IsNumeric(@GstRate) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'GST Rate is not Numeric'
		return @RetMessage
	End
	else if @TaxableValue <> '' and IsNumeric(@TaxableValue) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Taxable Value is not Numeric'
		return @RetMessage
	End
	else if @IgstAmount <> '' and IsNumeric(@IgstAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'IGST Amount is Not Numeric '
		return @RetMessage
	End
	else if @CgstAmount <> '' and IsNumeric(@CgstAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'CGST Amount is not Numeric'
		return @RetMessage
	End
	else if @SgstAmount <> '' and IsNumeric(@SgstAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'SGST Amount is not Numeric'
		return @RetMessage
	End
	else if @CessAmount <> '' and IsNumeric(@CessAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Cess Amount is not Numeric'
		return @RetMessage
	End
	else if @Qty <> '' and IsNumeric(@Qty) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Quantity is not Numeric'
		return @RetMessage
	End
	else if @UnitPrice <> '' and IsNumeric(@UnitPrice) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Unit Price is not Numeric'
		return @RetMessage
	End
	else if @TotVal <> '' and IsNumeric(@TotVal) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Total Value is not Numeric'
		return @RetMessage
	End
	else if @AdAdjAmount <> '' and IsNumeric(@AdAdjAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Advance Adjusted Amount is not Numeric'
		return @RetMessage
	End
	else if @AdRecvdAmount <> '' and IsNumeric(@AdRecvdAmount) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Advance Received Amount is not Numeric'
		return @RetMessage
	End
	else if @NilAmt <> '' and IsNumeric(@NilAmt) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Total Nil rated outward supplies is not Numeric'
		return @RetMessage
	End
	else if @ExptAmt <> '' and IsNumeric(@ExptAmt) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Total Exempted outward supplies is not Numeric'
		return @RetMessage
	End
	else if @NgsupAmt <> '' and IsNumeric(@NgsupAmt) <> 1
	Begin
		Select @RetValue = -1 , @RetMessage = 'Total Non GST outward supplies is not Numeric'
		return @RetMessage
	End

	return @RetMessage

End