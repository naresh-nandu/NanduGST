


/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate and convert the Invoice Date proper format
				Returns Valid date string
				
Written by  : Karthik.kanniyappan

Date		Who			Decription 
06/07/2017	Karthik 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateAndConvertDate('2/22/2017')

 */

CREATE FUNCTION [dbo].[udf_ValidateAndConvertDate](
@InvoiceDate	 varchar(50)
)
Returns varchar(50)
as
Begin
     
	Declare @Retvalue varchar(50)
		

	Select @InvoiceDate = Ltrim(Rtrim(IsNull(@InvoiceDate,'')))
	Select @RetValue = ''

	if(
	(
	CONVERT(INT,SUBSTRING(@InvoiceDate,0,CHARINDEX('/',@InvoiceDate,0))) >12 
	OR CONVERT(INT,SUBSTRING(@InvoiceDate,0,CHARINDEX('-',@InvoiceDate,0))) >12) 
	and 
	(CONVERT(INT,SUBSTRING(SUBSTRING(@InvoiceDate,CHARINDEX('/',@InvoiceDate)+1,LEN(@InvoiceDate)),0,CHARINDEX('/',SUBSTRING(@InvoiceDate,CHARINDEX('/',@InvoiceDate)+1,LEN(@InvoiceDate)),0))) <= 12 
	or CONVERT(INT,SUBSTRING(SUBSTRING(@InvoiceDate,CHARINDEX('-',@InvoiceDate)+1,LEN(@InvoiceDate)),0,CHARINDEX('-',SUBSTRING(@InvoiceDate,CHARINDEX('-',@InvoiceDate)+1,LEN(@InvoiceDate)),0))) <= 12))
		Begin

			Select @RetValue = @InvoiceDate
			Return @RetValue

		End
	Else if((CONVERT(INT,SUBSTRING(@InvoiceDate,0,CHARINDEX('/',@InvoiceDate,0))) <= 12 
	OR CONVERT(INT,SUBSTRING(@InvoiceDate,0,CHARINDEX('-',@InvoiceDate,0))) <= 12) 
	and (
	CONVERT(INT,SUBSTRING(SUBSTRING(@InvoiceDate,CHARINDEX('/',@InvoiceDate)+1,LEN(@InvoiceDate)),0,CHARINDEX('/',SUBSTRING(@InvoiceDate,CHARINDEX('/',@InvoiceDate)+1,LEN(@InvoiceDate)),0))
	) > 12 
	or CONVERT(INT,SUBSTRING(SUBSTRING(@InvoiceDate,CHARINDEX('-',@InvoiceDate)+1,LEN(@InvoiceDate)),0,CHARINDEX('-',SUBSTRING(@InvoiceDate,CHARINDEX('-',@InvoiceDate)+1,LEN(@InvoiceDate)),0))) > 12))
		Begin

			Select @RetValue = convert(varchar, convert(datetime,  Ltrim(Rtrim(@InvoiceDate)), 101), 103) 
			Return @RetValue

		End
		Else
		Begin

			Select @RetValue = convert(varchar, @InvoiceDate, 103)
			Return @RetValue

		End


	return @RetValue

End