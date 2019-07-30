
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Invoice Value. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/08/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateInvoiceValue('18') 

 */

CREATE FUNCTION [udf_ValidateInvoiceValue](
 @InvoiceValue      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int
	
	Select @InvoiceValue = Ltrim(Rtrim(IsNull(@InvoiceValue,'')))
	Select @RetValue = 1

	if @InvoiceValue = '' Or IsNumeric(@InvoiceValue) <> 1 or Convert(dec(18,2),@InvoiceValue) <= 0	
	Begin
		Select @RetValue = -1  -- Invoice Value is either blank / not numeric / less than or equal to 0
		return @RetValue
	End 

	return @RetValue
End