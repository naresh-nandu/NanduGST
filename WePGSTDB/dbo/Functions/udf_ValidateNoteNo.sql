
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Note Number
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
07/26/2017	Seshadri 	Initial Version
08/29/2017	Seshadri	Modified the code related to length validation (Min : 1 ; Max : 16)  
08/31/2017	Seshadri	Removed AlphaNumeric Check
02/17/2017	Seshadri	Reintroduced AlphaNumeric Check along with 2 special characters (- & /)

*/

/* Sample Function Call

select dbo.udf_ValidateNoteNo('NT100001')

 */

CREATE FUNCTION [udf_ValidateNoteNo](
@NoteNo	 varchar(50)
)
Returns int
as
Begin
     
	Declare @Retvalue int,
			@TmpValue varchar(10)

	Select @NoteNo = Ltrim(Rtrim(IsNull(@NoteNo,'')))
	Select @RetValue = 1

	if (len(@NoteNo) < 1 or len(@NoteNo) > 16)
	Begin
		Select @RetValue = -1  -- Length of the NoteNo must be >= 1 and <= 16
		return @RetValue
	End

	if(Substring(@NoteNo, 0, Len(@NoteNo)) LIKE '%[^a-zA-Z0-9-/]%')
	Begin
		Select @RetValue = -2  -- NoteNo is not AlphaNumeric
		return @RetValue
	End					
								
	return @RetValue

End