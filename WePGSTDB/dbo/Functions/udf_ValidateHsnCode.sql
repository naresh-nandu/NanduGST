
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate HSN Code
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
01/20/2018	Seshadri 	Initial Version

*/

/* Sample Function Call

select dbo.udf_ValidateHsnCode('20000A')

 */

CREATE FUNCTION [udf_ValidateHsnCode](
@HsnCode	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10)

	Select @HsnCode = Ltrim(Rtrim(IsNull(@HsnCode,'')))
	Select @RetValue = 1

	if (len(@HsnCode) < 2 or len(@HsnCode) > 10)
	Begin
		Select @RetValue = -1  -- Length of Hsn Code must be >= 2 and <= 10
		return @RetValue
	End

	if @HsnCode = '' Or IsNumeric(@HsnCode) <> 1 or Convert(int,@HsnCode) <= 0	
	Begin
		Select @RetValue = -2  -- Hsn Code is either blank / not numeric / less than or equal to 0
		return @RetValue
	End 
								
	return @RetValue

End