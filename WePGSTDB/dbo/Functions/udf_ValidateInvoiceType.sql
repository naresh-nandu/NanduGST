
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Invoice Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateInvoiceType('R') 

 */

CREATE FUNCTION [udf_ValidateInvoiceType](
 @InvoiceType      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @InvoiceType = Ltrim(Rtrim(IsNull(@InvoiceType,'')))
	   Select @RetValue = 1
	
	   if(@InvoiceType not in ('R','SEWP','SEWOP','DE'))
	   Begin
			Select @RetValue = -1   -- Invalid Invoice Type
			return @RetValue
	   End

		return @RetValue
End