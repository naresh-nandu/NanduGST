
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Nil Supply Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/26/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateNilSupplyType() 

 */

CREATE FUNCTION [udf_ValidateNilSupplyType](
 @SupplyType      varchar(50)
)
Returns int
as
Begin
		Declare @RetValue int
	
		Select @SupplyType = Ltrim(Rtrim(IsNull(@SupplyType,'')))
   
		Select @RetValue = 1
	
		if(@SupplyType not in ('INTRB2B','INTRB2C','INTRAB2B','INTRAB2C'))
		Begin
			Select @RetValue = -1   -- Invalid Nil Supply Type
			return @RetValue
		End

		return @RetValue
End