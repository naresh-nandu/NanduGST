
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate BOE Number
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/07/2017	Seshadri 	Initial Version

*/

/* Sample Function Call

select dbo.udf_ValidateBOENo('20000A')

 */

CREATE FUNCTION [udf_ValidateBOENo](
@BillOfEntryNo	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10)

	Select @BillOfEntryNo = Ltrim(Rtrim(IsNull(@BillOfEntryNo,'')))
	Select @RetValue = 1

	if (len(@BillOfEntryNo) < 1 or len(@BillOfEntryNo) > 7)
	Begin
		Select @RetValue = -1  -- Length of Bill of Entry Number must be >= 1 and <= 7
		return @RetValue
	End

	if @BillOfEntryNo = '' Or IsNumeric(@BillOfEntryNo) <> 1 or Convert(int,@BillOfEntryNo) <= 0	
	Begin
		Select @RetValue = -2  -- Bill Of Entry Number is either blank / not numeric / less than or equal to 0
		return @RetValue
	End 
								
	return @RetValue

End