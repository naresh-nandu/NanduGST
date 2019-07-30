
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Date
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepindia.com

Date		Who			Decription 
09/04/2017	Seshadri 		Initial Version
*/

/* Sample Function Call

select dbo.udf_ValidateDate('22/05/2017')

 */

CREATE FUNCTION [udf_ValidateDate](
@Date	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpDate Date
		

	Select @Date = Ltrim(Rtrim(IsNull(@Date,'')))
	Select @RetValue = 1

	if len(@Date) < 8 or len(@Date) > 10
	Begin
		Select @RetValue = -1  -- Length of the Date should be between 8 and 10 
		return @RetValue
	End

	Select @TmpDate = Try_Convert(date,@Date,103)
	if @TmpDate Is null
	Begin
		Select @RetValue = -2  -- Invalid Date Format
		return @RetValue
	End

	return @RetValue

End