

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete all the GSTR2 Records from the corresponding external tables
				
Written by  : muskan.garg@wepdigital.com

Date		Who			Decription 
17/05/2018	Muskan		Initial Version
21/05/2018	Muskan		Added ISD and ISDA for Deletion

*/

/* Sample Procedure Call

exec usp_Delete_All_GSTR2_EXT 
 */

CREATE PROCEDURE [dbo].[usp_Delete_All_GSTR6_EXT]
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

		Update TBL_EXT_GSTR6_B2B_INV
		Set rowstatus = 2
		Where b2bid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2BA'
		Begin

		Update TBL_EXT_GSTR6_B2BA_INV
		Set rowstatus = 2
		Where b2baid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End	
	else if @ActionType = 'CDN'
	Begin

		Update TBL_EXT_GSTR6_CDN
		Set rowstatus = 2
		Where cdnid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNA'
    Begin

		Update TBL_EXT_GSTR6_CDNA
		Set rowstatus = 2
		Where cdnaid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'ISD'
	Begin

		Update TBL_EXT_GSTR6_ISD
		Set rowstatus = 2
		Where isdid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'ISDA'
    Begin

		Update TBL_EXT_GSTR6_ISDA
		Set rowstatus = 2
		Where isdaid In (Select refid From @TBL_Values
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