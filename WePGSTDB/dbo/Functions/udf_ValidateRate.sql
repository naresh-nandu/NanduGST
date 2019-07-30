
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Rate. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateRate('18') 

 */

CREATE FUNCTION [udf_ValidateRate](
 @Rate      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int,
	        @IntRate dec(18,2)

	Select @Rate = Ltrim(Rtrim(IsNull(@Rate,'')))
	Select @RetValue = 1

	if IsNumeric(@Rate) <> 1
	Begin
		Select @RetValue = -1  -- Rate is not numeric
		return @RetValue
	End
	else
	Begin
		Select @IntRate = Convert(dec(18,2),@Rate) 
	End

	if @IntRate not in (0,0.1,0.25,3,5,12,18,28)
	Begin
		Select @RetValue = -2   -- Invalid Rate
		return @RetValue
	End

	return @RetValue
End