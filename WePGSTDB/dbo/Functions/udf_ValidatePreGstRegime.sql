
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Pre Gst Regime. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/26/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidatePreGstRegime('Y') 

 */

CREATE FUNCTION [udf_ValidatePreGstRegime](
 @PreGstRegime      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @PreGstRegime = Ltrim(Rtrim(IsNull(@PreGstRegime,'')))
	   Select @RetValue = 1
	
	   if(@PreGstRegime not in ('Y','N'))
	   Begin
			Select @RetValue = -1   -- Invalid Pre Gst Regime
			return @RetValue
	   End

		return @RetValue
End