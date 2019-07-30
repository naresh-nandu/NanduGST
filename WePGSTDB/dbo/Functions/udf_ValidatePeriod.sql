
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Period. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/07/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidatePeriod('052017') 

 */


CREATE FUNCTION [udf_ValidatePeriod](
 @Period      varchar(50)
)
Returns int
as
Begin

	Declare @RetValue int,
			@GivenMonth int,
			@GivenYear int,
			@CurrMonth int,
			@CurrYear int,
			@TmpValue varchar(10)

	Select @Period = Ltrim(Rtrim(IsNull(@Period,'')))
	Select @RetValue = 1

	if len(@Period) != 6
	Begin
		Select @RetValue = -1  -- Length of the Period is not equal to 6
		return @RetValue
	End

	Select @TmpValue =  Substring(@Period,1,2)
	if IsNumeric(@TmpValue) <> 1
	Begin
		Select @RetValue = -2  -- Month Part of the Period is not numeric
		return @RetValue
	End
	else
	Begin
		Select @GivenMonth = Convert(int,@TmpValue)
	End

	Select @TmpValue =  Substring(@Period,3,4)
	if IsNumeric(@TmpValue) <> 1
	Begin
		Select @RetValue = -3  -- Year Part of the Period is not numeric
		return @RetValue
	End
	else
	Begin
		Select @GivenYear = Convert(int,@TmpValue)
	End


	if @GivenMonth <= 0 or @GivenMonth > 12
	Begin
		Select @RetValue = -4  -- Month Part of the Period is not Valid
		return @RetValue
	End

	if @GivenYear <= 0 
	Begin
		Select @RetValue = -5  -- Year Part of the Period is not Valid
		return @RetValue
	End

	Select @CurrMonth =  Month(GetDate())
	Select @CurrYear =  Year(GetDate())

	if @GivenYear > @CurrYear
	Begin
		Select @RetValue = -6  -- Given Year Cannot be greater than the Current Year
		return @RetValue
	End
	else if (@GivenYear = @CurrYear and @GivenMonth > @CurrMonth)
	Begin
		Select @RetValue = -7  -- Given Year and Month Cannot be greater than the Current Year and Month
		return @RetValue
	End	
	   		
	return @RetValue

End