
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Check whether it is a Valid Ctin. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/17/2017	Seshadri 		Initial Version


*/

/* Sample Function Call

 Select dbo.udf_IsValidCtin('B2B') 

 */

CREATE FUNCTION [udf_IsValidCtin](
 @Ctin      varchar(15)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @Ctin  = Ltrim(Rtrim(IsNull(@Ctin,'')))
	   Select @RetValue = 1

	   If Not Exists(Select 1 From TBL_ValidGSTIN Where ValidGstin = @Ctin and Status = 1)
	   Begin
			Select @RetValue = -1   -- Invalid Ctin
			return @RetValue
	   End

		return @RetValue
End