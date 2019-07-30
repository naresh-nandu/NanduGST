
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Function to Validate Note Reason. 
				Returns 1 - Valid ; Any other Value  - Invalid
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/01/2017	Seshadri 	Initial Version
*/

/* Sample Function Call

 Select dbo.udf_ValidateNoteRsn('01-Sales Return') 

 */

CREATE FUNCTION [udf_ValidateNoteRsn](
 @NoteRsn      varchar(50)
)
Returns int
as
Begin
       Declare @RetValue int
	
	   Select @NoteRsn = Ltrim(Rtrim(IsNull(@NoteRsn,'')))
	   Select @RetValue = 1
	
	   if(@NoteRsn not in (
			'01-Sales Return',
			'02-Post Sale Discount',
			'03-Deficiency in services',
			'04-Correction in Invoice',
			'05-Change in POS',
			'06-Finalization of Provisional assessment',
			'07-Others'
			  )	)
	   Begin
			Select @RetValue = -1   -- Invalid Note Reason
			return @RetValue
	   End

		return @RetValue
End