
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete all the GSTR1 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/06/2017	Seshadri		Initial Version
09/07/2017	Seshadri	Introduced the Action Type HSNSUM & DOC
03/16/2018	Seshadri	Modified the code to support Amendments
06/06/2018  Karthik     Implementing Permanent Delete

*/

/* Sample Procedure Call

exec usp_Delete_All_GSTR1_EXT 
 */

CREATE PROCEDURE [usp_Delete_All_GSTR1_EXT]
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

		--Update TBL_EXT_GSTR1_B2B_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_B2B_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2CL'
	Begin

		--Update TBL_EXT_GSTR1_B2CL_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0
		Delete from TBL_EXT_GSTR1_B2CL_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2CS'
	Begin

		--Update TBL_EXT_GSTR1_B2CS
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_B2CS
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'EXP'
	Begin
		
		--Update TBL_EXT_GSTR1_EXP_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_EXP_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNR'
	Begin

		--Update  TBL_EXT_GSTR1_CDNR
		--Set rowstatus = 2
		--Where cdnrid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_CDNR
		Where cdnrid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNUR'
	Begin

		--Update  TBL_EXT_GSTR1_CDNUR
		--Set rowstatus = 2
		--Where cdnurid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_CDNUR
		Where cdnurid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if (@ActionType In ('HSN','HSNSUM'))
	Begin

		--Update TBL_EXT_GSTR1_HSN
		--Set rowstatus = 2
		--Where hsnid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_HSN
		Where hsnid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'NIL'
	Begin

		--Update TBL_EXT_GSTR1_NIL
		--Set rowstatus = 2
		--Where nilid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_NIL
		Where nilid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'TXP'
	Begin

		--Update TBL_EXT_GSTR1_TXP
		--Set rowstatus = 2
		--Where txpid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_TXP
		Where txpid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'AT'
	Begin

		--Update TBL_EXT_GSTR1_AT
		--Set rowstatus = 2
		--Where atid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_AT
		Where atid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if (@ActionType In ('DOCISSUE','DOC'))
	Begin

		--Update TBL_EXT_GSTR1_DOC
		--Set rowstatus = 2
		--Where docid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0
		Delete from TBL_EXT_GSTR1_DOC
		Where docid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0


	End
	else if @ActionType = 'B2BA'
	Begin

		--Update TBL_EXT_GSTR1_B2BA_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_B2BA_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2CLA'
	Begin

		--Update TBL_EXT_GSTR1_B2CLA_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_B2CLA_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'B2CSA'
	Begin

		--Update TBL_EXT_GSTR1_B2CSA
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_B2CSA
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'EXPA'
	Begin

		--Update TBL_EXT_GSTR1_EXPA_INV
		--Set rowstatus = 2
		--Where invid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_EXPA_INV
		Where invid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNRA'
	Begin

		--Update  TBL_EXT_GSTR1_CDNRA
		--Set rowstatus = 2
		--Where cdnraid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from  TBL_EXT_GSTR1_CDNRA
		Where cdnraid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'CDNURA'
	Begin

		--Update  TBL_EXT_GSTR1_CDNURA
		--Set rowstatus = 2
		--Where cdnuraid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from  TBL_EXT_GSTR1_CDNURA
		Where cdnuraid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'TXPA'
	Begin

		--Update TBL_EXT_GSTR1_TXPA
		--Set rowstatus = 2
		--Where txpaid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_TXPA
		Where txpaid In (Select refid From @TBL_Values
						Where actiontype = @ActionType)
		And rowstatus <> 0

	End
	else if @ActionType = 'ATA'
	Begin

		--Update TBL_EXT_GSTR1_ATA
		--Set rowstatus = 2
		--Where ataid In (Select refid From @TBL_Values
		--				Where actiontype = @ActionType)
		--And rowstatus <> 0

		Delete from TBL_EXT_GSTR1_ATA
		Where ataid In (Select refid From @TBL_Values
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