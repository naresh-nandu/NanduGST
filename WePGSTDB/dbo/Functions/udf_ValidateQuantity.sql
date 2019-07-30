
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Quantity. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 	Initial Version
03/08/2018	Seshadri	Made Code Changes to allow negative values 
*/

/* Sample Function Call

 Select dbo.udf_ValidateQuantity('-12') 

 */

CREATE FUNCTION [udf_ValidateQuantity](
 @Quantity      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int,
	        @Qty dec(18,2)

	Select @Quantity = Ltrim(Rtrim(IsNull(@Quantity,'')))
	Select @RetValue = 1

	if IsNumeric(@Quantity) <> 1
	Begin
		Select @RetValue = -1  -- Quantity is not numeric
		return @RetValue
	End
	else
	Begin
		Select @Qty = Convert(dec(18,2),@Quantity) 
	End

	/*
	if @Qty <= 0
	Begin
		Select @RetValue = -2   -- Invalid Quantity
		return @RetValue
	End
	*/

	return @RetValue
End