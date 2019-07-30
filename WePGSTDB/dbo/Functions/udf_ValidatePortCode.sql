
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Port Code
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
07/26/2017	Seshadri 	Initial Version

*/

/* Sample Function Call

select dbo.udf_ValidatePortCode('NT100001')

 */

CREATE FUNCTION [udf_ValidatePortCode](
@PortCode	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int

	Select @PortCode = Ltrim(Rtrim(IsNull(@PortCode,'')))
	Select @RetValue = 1

	if (len(@PortCode) < 1 or len(@PortCode) > 6)
	Begin
		Select @RetValue = -1  -- Length of the Port Code must be >= 1 and <= 6
		return @RetValue
	End

		
								
	return @RetValue

End