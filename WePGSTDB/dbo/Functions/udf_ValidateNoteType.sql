
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Note Type. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/06/2017	Seshadri 	Initial Version
08/28/2017	Seshadri	Included Values CREDIT , DEBIT & REFUND because of Template Changes
*/

/* Sample Function Call

 Select dbo.udf_ValidateNoteType('R') 

 */

CREATE FUNCTION [udf_ValidateNoteType](
 @NoteType      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @NoteType = Ltrim(Rtrim(IsNull(@NoteType,'')))
	   Select @RetValue = 1
	
	   if(@NoteType not in ('C','D','R','CREDIT','DEBIT','REFUND'))
	   Begin
			Select @RetValue = -1   -- Invalid Note Type
			return @RetValue
	   End

		return @RetValue
End