
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate IsSEZ. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/20/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateIsSEZ('Y') 

 */

CREATE FUNCTION [udf_ValidateIsSEZ](
 @IsSEZ      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @IsSEZ = Ltrim(Rtrim(IsNull(@IsSEZ,'')))
	   Select @RetValue = 1
	
	   if(@IsSEZ not in ('Y','N'))
	   Begin
			Select @RetValue = -1   -- Invalid IsSEZ
			return @RetValue
	   End

		return @RetValue
End