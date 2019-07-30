
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate HSN Desc
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
01/20/2018	Seshadri 	Initial Version

*/

/* Sample Function Call

select dbo.udf_ValidateHsnDesc('INVWEP200007')

 */

CREATE FUNCTION [udf_ValidateHsnDesc](
@HsnDesc	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10)

	Select @HsnDesc = Ltrim(Rtrim(IsNull(@HsnDesc,'')))
	Select @RetValue = 1

	if (len(@HsnDesc) < 1 or len(@HsnDesc) > 30)
	Begin
		Select @RetValue = -1  -- Length of the Hsn Description must be >= 1 and <= 30
		return @RetValue
	End

								
	return @RetValue

End