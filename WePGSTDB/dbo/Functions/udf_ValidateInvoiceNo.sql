
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Invoice Number
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/07/2017	Karthik 	Initial Version
08/29/2017	Seshadri	Modified the code related to length validation (Min : 1 ; Max : 16)  
08/31/2017	Seshadri	Removed AlphaNumeric Check
02/17/2017	Seshadri	Reintroduced AlphaNumeric Check along with 2 special characters (- & /)

*/

/* Sample Function Call

select dbo.udf_ValidateInvoiceNo('INVWEP200007')

 */

CREATE FUNCTION [udf_ValidateInvoiceNo](
@InvoiceNo	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@StateCode tinyint,
			@TmpValue varchar(10)

	Select @InvoiceNo = Ltrim(Rtrim(IsNull(@InvoiceNo,'')))
	Select @RetValue = 1

	if (len(@InvoiceNo) < 1 or len(@InvoiceNo) > 16)
	Begin
		Select @RetValue = -1  -- Length of the InvoiceNo must be >= 1 and <= 16
		return @RetValue
	End

	if(Substring(@InvoiceNo, 0, Len(@InvoiceNo)) LIKE '%[^a-zA-Z0-9-/]%')
	Begin
		Select @RetValue = -2  -- InvoiceNo is not AlphaNumeric
		return @RetValue
	End					
								
	return @RetValue

End