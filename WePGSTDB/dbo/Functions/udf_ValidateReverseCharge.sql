﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Reverse Charge. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/26/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateReverseCharge('Y') 

 */

CREATE FUNCTION [udf_ValidateReverseCharge](
 @ReverseCharge      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @ReverseCharge = Ltrim(Rtrim(IsNull(@ReverseCharge,'')))
	   Select @RetValue = 1
	
	   if(@ReverseCharge not in ('Y','N'))
	   Begin
			Select @RetValue = -1   -- Invalid Reverse Charge
			return @RetValue
	   End

		return @RetValue
End