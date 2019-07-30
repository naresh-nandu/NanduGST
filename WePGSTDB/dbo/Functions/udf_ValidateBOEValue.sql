
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Bill Of Entry Value. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/08/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateBOEValue('18') 

 */

CREATE FUNCTION [udf_ValidateBOEValue](
 @BillOfEntryValue      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int
	
	Select @BillOfEntryValue = Ltrim(Rtrim(IsNull(@BillOfEntryValue,'')))
	Select @RetValue = 1

	if @BillOfEntryValue = '' Or IsNumeric(@BillOfEntryValue) <> 1 or Convert(dec(18,2),@BillOfEntryValue) <= 0	
	Begin
		Select @RetValue = -1  -- Bill Of Entry Value is either blank / not numeric / less than or equal to 0
		return @RetValue
	End 

	return @RetValue
End