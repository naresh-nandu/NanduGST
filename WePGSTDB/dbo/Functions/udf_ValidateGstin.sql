
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate GSTIN / CTIN / Ecommerce GSTIN
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/07/2017	Karthik 		Initial Version
09/12/2017	Seshadri	Corrected the state code for Other Territory
*/

/* Sample Function Call

select dbo.udf_ValidateGstin('33AABFA9A870CZq')

 */

CREATE FUNCTION [udf_ValidateGstin](
@Gstin	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10),
			@StateCode tinyint,
			@Pan varchar(10)


	Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	Select @RetValue = 1

	if len(@Gstin) != 15
	Begin
		Select @RetValue = -1  -- Length of the Gstin is not equal to 15
		return @RetValue
	End

	Select @TmpValue =  Substring(@Gstin,1,2)
	if IsNumeric(@TmpValue) <> 1
	Begin
		Select @RetValue = -2  -- State Code of Gstin is not numeric
		return @RetValue
	End
	else
	Begin
		Select @StateCode = Convert(tinyint,@TmpValue)
	End

	if (@StateCode <= 0 or @StateCode  > 37) and (@StateCode <> 97)
	Begin
		Select @RetValue = -3  -- State Code is not Valid
		return @RetValue
	End

	Select @Pan = Substring(@Gstin,3,10)
	if(@Pan Not Like '[A-Z][A-Z][A-Z][A-Z][A-Z][0-9][0-9][0-9][0-9][A-Z]')
	Begin
		Select @RetValue = -4  -- Invalid PAN Number
		return @RetValue
	End	
	
	if(Substring(@Gstin,13,1) Not Like '[A-Z0-9]')
	Begin
		Select @RetValue = -5  -- Invalid Number of Registration
		return @RetValue
	End	
 
	if(Substring(@Gstin,14,1) <> 'Z')
	Begin
		Select @RetValue = -6  -- Invalid Default Digit
		return @RetValue
	End	
	
	if(Substring(@Gstin,15,1) Not Like '[A-Z0-9]')
	Begin
		Select @RetValue = -7  -- Invalid Check Code
		return @RetValue
	End				
								
	return @RetValue

End