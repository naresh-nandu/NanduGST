
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 CSV Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_CSV_GSTR1_EXT 'BP',
 */

CREATE PROCEDURE [usp_Insert_CSV_GSTR1_EXT]
	@SourceType varchar(15), -- BP  
	@ReferenceNo varchar(50),
	@ActionType varchar(15),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		exec usp_Insert_CSV_GSTR1B2B_EXT @SourceType, @ReferenceNo, @RecordContents,@ErrorCode,@ErrorMessage 
	End
	else if @ActionType = 'B2CS'
	Begin
		exec usp_Insert_CSV_GSTR1B2CS_EXT @SourceType, @ReferenceNo, @RecordContents,@ErrorCode,@ErrorMessage
	End
	else if @ActionType = 'CDNR'
	Begin
		exec usp_Insert_CSV_GSTR1CDNR_EXT @SourceType, @ReferenceNo, @RecordContents,@ErrorCode,@ErrorMessage
	End
	else
	Begin
		Select	@ErrorCode = -1,
				@ErrorMessage = 'Unsupported Action Type'
		Return @ErrorCode
	End


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End