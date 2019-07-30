
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Note Date
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepindia.com

Date		Who			Decription 
06/07/2017	Seshadri 		Initial Version
09/01/2017	Seshadri		Note Date Validation Fix
*/

/* Sample Function Call

select dbo.udf_ValidateNoteDate('22/05/2017','052017')

 */

CREATE FUNCTION [udf_ValidateNoteDate](
@NoteDate	 varchar(50),
@Period varchar(50),
@InvoiceDate varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@NtDate Date,
			@NtMonth int,
			@NtYear int,
			@PeriodMonth int,
			@PeriodYear int,
			@TmpValue varchar(10),
			@InvDate Date
		

	Select @NoteDate = Ltrim(Rtrim(IsNull(@NoteDate,'')))
	Select @RetValue = 1

	if len(@NoteDate) < 8 or len(@NoteDate) > 10
	Begin
		Select @RetValue = -1  -- Length of the Note Date should be between 8 and 10 
		return @RetValue
	End

	Select @NtDate = Try_Convert(date,@NoteDate,103)
	if @NtDate Is null
	Begin
		Select @RetValue = -2  -- Invalid Note Date Format
		return @RetValue
	End

	Select @NtMonth =  Month(@NtDate)
	Select @NtYear =  Year(@NtDate)


	Select @TmpValue =  Substring(@Period,1,2)
	Select @PeriodMonth = Convert(int,@TmpValue)
	Select @TmpValue =  Substring(@Period,3,4)
	Select @PeriodYear = Convert(int,@TmpValue)

	Select @InvDate = Try_Convert(date,@InvoiceDate,103)


	if @NtYear > @PeriodYear
	Begin
		Select @RetValue = -3  -- Note Date is greater than Return Period
		return @RetValue
	End
	else if (@NtYear = @PeriodYear and @NtMonth > @PeriodMonth)
	Begin
		Select @RetValue = -3  -- Note Date is greater than Return Period
		return @RetValue
	End	

	if @NtDate < @InvDate
	Begin
		Select @RetValue = -4  -- Note Date should not be less than Invoice date
		return @RetValue
	End	


	return @RetValue

End