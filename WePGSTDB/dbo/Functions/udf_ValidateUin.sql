
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Embassy GSTIN / CTIN / Ecommerce GSTIN
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
02/28/2018	Seshadri		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateUin('33AABFA9A870CZq')

 */

CREATE FUNCTION [udf_ValidateUin](
@Uin	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@StateCode varchar(2),
			@YearCode varchar(2),
			@CountryCode varchar(3),
			@SerialNo varchar(5),
			@DefaultCode varchar(3),
			@TmpValue tinyint
		

	Select @Uin = Ltrim(Rtrim(IsNull(@Uin,'')))
	Select @RetValue = 1

	if len(@Uin) != 15
	Begin
		Select @RetValue = -1  -- Length of the UIN is not equal to 15
		return @RetValue
	End

	-- State Code

	Select @StateCode = Substring(@Uin,1,2)
	if(@StateCode Not Like '[0-9][0-9]')
	Begin
		Select @RetValue = -2  -- Invalid State Code
		return @RetValue
	End
	Select @TmpValue  = Convert(tinyint,@StateCode)
	if (@TmpValue <= 0 or @TmpValue  > 37) and (@TmpValue <> 97)
	Begin
		Select @RetValue = -2  -- Invalid State Code
		return @RetValue
	End

	-- Year Code

	Select @YearCode = Substring(@Uin,3,2)
	if(@YearCode Not Like '[0-9][0-9]')
	Begin
		Select @RetValue = -3  -- Invalid Year Code
		return @RetValue
	End

	-- Country Code

	Select @CountryCode = Substring(@Uin,5,3)
	if(@CountryCode Not Like '[A-Z][A-Z][A-Z]')
	Begin
		Select @RetValue = -4  -- Invalid Country Code
		return @RetValue
	End	
 
	-- Serial Number

	Select @SerialNo = Substring(@Uin,8,5)
	if(@SerialNo Not Like '[0-9][0-9][0-9][0-9][0-9]')
	Begin
		Select @RetValue = -5  -- Invalid Serial Number
		return @RetValue
	End	

	-- Last 3 Default Digits 
	
	Select @DefaultCode = Substring(@Uin,13,3)
	if(@DefaultCode Not Like '[A-Z0-9][A-Z0-9][A-Z0-9]')
	Begin
		Select @RetValue = -6  -- Invalid Last 3 Default Digits
		return @RetValue
	End	
 
	return @RetValue

End