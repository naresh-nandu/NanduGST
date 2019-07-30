
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Invoice Date
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepindia.com

Date		Who			Decription 
06/07/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateInvoiceDate('22/05/2017','052017')

 */

CREATE FUNCTION [udf_ValidateInvoiceDate](
@InvoiceDate	 varchar(50),
@Period varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@InvDate Date,
			@InvMonth int,
			@InvYear int,
			@PeriodMonth int,
			@PeriodYear int,
			@TmpValue varchar(10)
		

	Select @InvoiceDate = Ltrim(Rtrim(IsNull(@InvoiceDate,'')))
	Select @RetValue = 1

	if len(@InvoiceDate) < 8 or len(@InvoiceDate) > 10
	Begin
		Select @RetValue = -1  -- Length of the Invoice Date should be between 8 and 10 
		return @RetValue
	End

	Select @InvDate = Try_Convert(date,@InvoiceDate,103)
	if @InvDate Is null
	Begin
		Select @RetValue = -2  -- Invalid Invoice Date Format
		return @RetValue
	End

	Select @InvMonth =  Month(@InvDate)
	Select @InvYear =  Year(@InvDate)


	Select @TmpValue =  Substring(@Period,1,2)
	Select @PeriodMonth = Convert(int,@TmpValue)
	Select @TmpValue =  Substring(@Period,3,4)
	Select @PeriodYear = Convert(int,@TmpValue)

	if @InvYear > @PeriodYear
	Begin
		Select @RetValue = -3  -- Invoice Date is greater than Return Period
		return @RetValue
	End
	else if (@InvYear = @PeriodYear and @InvMonth > @PeriodMonth)
	Begin
		Select @RetValue = -3  -- Invoice Date is greater than Return Period
		return @RetValue
	End	

	if(DATEDIFF(MM,@InvDate,GETDATE()) > 18)
	Begin
		Select @RetValue = -4  -- Invoice Date is Older than 18 Months
		return @RetValue
	End	


	return @RetValue

End