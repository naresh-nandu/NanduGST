
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Note Date
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
05/06/2018	Karthik 	Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateShippingBillDate('27/05/2017','052017','25/05/2017')

 */

CREATE FUNCTION [dbo].[udf_ValidateShippingBillDate](
@SBDate	 varchar(50),
@Period varchar(50),
@InvoiceDate varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@SDate Date,
			@SMonth int,
			@SYear int,
			@PeriodMonth int,
			@PeriodYear int,
			@TmpValue varchar(10),
			@InvDate Date
		

	Select @SBDate = Ltrim(Rtrim(IsNull(@SBDate,'')))
	Select @RetValue = 1

	if len(@SBDate) < 8 or len(@SBDate) > 10
	Begin
		Select @RetValue = -1  -- Length of the Note Date should be between 8 and 10 
		return @RetValue
	End

	Select @SDate = Try_Convert(date,@SBDate,103)
	if @SDate Is null
	Begin
		Select @RetValue = -2  -- Invalid Note Date Format
		return @RetValue
	End

	Select @SMonth =  Month(@SDate)
	Select @SYear =  Year(@SDate)


	Select @TmpValue =  Substring(@Period,1,2)
	Select @PeriodMonth = Convert(int,@TmpValue)
	Select @TmpValue =  Substring(@Period,3,4)
	Select @PeriodYear = Convert(int,@TmpValue)

	Select @InvDate = Try_Convert(date,@InvoiceDate,103)


	if @SYear > @PeriodYear
	Begin
		Select @RetValue = -3  -- Note Date is greater than Return Period
		return @RetValue
	End
	else if (@SYear = @PeriodYear and @SMonth > @PeriodMonth)
	Begin
		Select @RetValue = -3  -- Note Date is greater than Return Period
		return @RetValue
	End	

	if @SDate < @InvDate
	Begin
		Select @RetValue = -4  -- Note Date should not be less than Invoice date
		return @RetValue
	End	


	return @RetValue

End