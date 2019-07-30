
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete all the GSTR2 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/02/2017	Seshadri		Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_All_GSTR2_EXT 
 */

CREATE PROCEDURE [usp_Delete_All_GSTR2_EXT]
	@ActionType varchar(15),
	@RefIds nvarchar(MAX) = NULL,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	DECLARE @TBL_Values TABLE 
	(
		ActionType varchar(15),
		RefId int
	)

	Select @Delimiter = ','
	
	Insert Into @TBL_Values
	(ActionType,RefId)
	Select	@ActionType,Value 
	From string_split( @RefIds,@Delimiter) 


	if @ActionType = 'B2B'
	Begin

		Update TBL_EXT_GSTR2_B2B_INV
		Set rowstatus = 2
		Where b2bid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2BUR'
	Begin

		Update TBL_EXT_GSTR2_B2BUR_INV
		Set rowstatus = 2
		Where b2burid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'IMPG'
	Begin

		Update TBL_EXT_GSTR2_IMPG_INV
		Set rowstatus = 2
		Where impgid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'IMPS'
	Begin

		Update TBL_EXT_GSTR2_IMPS_INV
		Set rowstatus = 2
		Where impsid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNR'
	Begin

		Update TBL_EXT_GSTR2_CDN
		Set rowstatus = 2
		Where cdnid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNUR'
	Begin

		Update TBL_EXT_GSTR2_CDNUR
		Set rowstatus = 2
		Where cdnurid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if (@ActionType In ('HSN','HSNSUM'))
	Begin

		Update TBL_EXT_GSTR2_HSN
		Set rowstatus = 2
		Where hsnid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'NIL'
	Begin

		Update TBL_EXT_GSTR2_NIL
		Set rowstatus = 2
		Where nilid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'TXP'
	Begin

		Update TBL_EXT_GSTR2_TXPD
		Set rowstatus = 2
		Where txpdid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'TXI'
	Begin

		Update TBL_EXT_GSTR2_TXI
		Set rowstatus = 2
		Where txiid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'ITCRVSL'
	Begin

		Update TBL_EXT_GSTR2_ITCRVSL
		Set rowstatus = 2
		Where itcrvslid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

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