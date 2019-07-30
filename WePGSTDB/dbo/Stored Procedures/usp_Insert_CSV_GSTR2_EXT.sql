
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 CSV Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_CSV_GSTR2_EXT 'BP','BP0021','B2B','B2B,BP,11221122,04AABFN9870CMZT,062017,11XXPPHH5588HHH,1122,11-06-2017,675.00,4,0,R,1122,XXXX,5.00,120.00,10.00,10.00,540.00,0.00,27.00,27.00,81.00'
 */

CREATE PROCEDURE [usp_Insert_CSV_GSTR2_EXT] 
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
		exec usp_Insert_CSV_GSTR2B2B_EXT @SourceType, @ReferenceNo, @RecordContents,@ErrorCode,@ErrorMessage 
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