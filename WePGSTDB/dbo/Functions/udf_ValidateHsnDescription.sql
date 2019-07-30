
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate HSN Desription
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
07/26/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateHsnDescription('NTasd')

 */

CREATE FUNCTION [udf_ValidateHsnDescription](
@HsnDesc	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10)

	Select @HsnDesc = Ltrim(Rtrim(IsNull(@HsnDesc,'')))
	Select @RetValue = 1

	if (len(@HsnDesc) <= 0 or len(@HsnDesc) > 30)
	Begin
		Select @RetValue = -1  -- Length of the HSN Description must be >= 1 and <= 30
		return @RetValue
	End

	if(@HsnDesc Not Like '[A-Za-z]')
	Begin
		Select @RetValue = -2  -- Invalid HSN Description
		return @RetValue
	End	
 
	return @RetValue

End