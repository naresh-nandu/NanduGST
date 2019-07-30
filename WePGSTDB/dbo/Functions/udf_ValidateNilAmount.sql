
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Nil Amount. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/31/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateNilAmount('1') 

 */

CREATE FUNCTION [udf_ValidateNilAmount](
 @Amount      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int

	Select  @Amount = Ltrim(Rtrim(IsNull(@Amount,'')))
	Select @RetValue = 1

	if IsNumeric(@Amount) <> 1 or Convert(dec(18,2),@Amount) < 0	
	Begin
		Select @RetValue = -1  -- Amount is not numeric / less than  0
		return @RetValue
	End 

	return @RetValue

End