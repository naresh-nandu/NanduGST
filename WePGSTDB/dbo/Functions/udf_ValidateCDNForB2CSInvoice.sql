
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate CDN For B2CS Invoice. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/29/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateCDNForB2CSInvoice('07AAACD5108M1ZP','173604742','29-08-2017',11848)

 */

CREATE FUNCTION [udf_ValidateCDNForB2CSInvoice](
	@Gstin varchar(50),
	@Fp varchar(10),
	@InvNo	 varchar(50),
	@InvDate	 varchar(50),
	@TaxableValue      varchar(50)
)
Returns int
as
Begin
	Declare @RetValue int

	Select  @TaxableValue = Ltrim(Rtrim(IsNull(@TaxableValue,'')))
	Select @RetValue = 1

	if @TaxableValue = '' Or IsNumeric(@TaxableValue) <> 1 or Convert(dec(18,2),@TaxableValue) <= 0	
	Begin
		Select @RetValue = -1  -- Taxable Value is either blank / not numeric / less than or equal to 0
		return @RetValue
	End 

	return @RetValue

End