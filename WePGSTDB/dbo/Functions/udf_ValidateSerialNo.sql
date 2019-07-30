
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Serial Number
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/27/2017	Seshadri 	Initial Version
02/28/2018	Seshadri	Introduced AlphaNumeric Check along with 2 special characters (- & /)


*/

/* Sample Function Call

select dbo.udf_ValidateSerialNo('INVWEP200007')

 */

CREATE FUNCTION [udf_ValidateSerialNo](
@SerialNo	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int

	Select @SerialNo = Ltrim(Rtrim(IsNull(@SerialNo,'')))
	Select @RetValue = 1

	if (len(@SerialNo) < 1 or len(@SerialNo) > 16)
	Begin
		Select @RetValue = -1  -- Length of the SerialNo must be >= 1 and <= 16
		return @RetValue
	End

	if(Substring(@SerialNo, 0, Len(@SerialNo)) LIKE '%[^a-zA-Z0-9-/]%')
	Begin
		Select @RetValue = -2  -- SerialNo is not AlphaNumeric
		return @RetValue
	End		
								
	return @RetValue

End