
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the Invoice Flag for all GSTR1 Records in the staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/6/2017	Seshadri 	Initial Version
09/01/2017	Seshadri	Fixed Unit Testing Issues. Gstin and FP will not be considered
						as the Ids that are passed are unique. However the parameters will remain
09/04/2017	Seshadri	Fixed Action Type Issue
11/07/2017  Karthik     Added condition to update 'Z' flag after deletion where flag is 'D' and chksum <> NULL
3/21/2018	Seshadri	Made code changes to support amendments 

*/

/* Sample Procedure Call

usp_Update_Invoice_Flag_GSTR1


 */
 
CREATE PROCEDURE [usp_Update_Invoice_Flag_GSTR1]  
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@CurFlag varchar(1),
	@NewFlag varchar(1)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	DECLARE @TBL_Values TABLE 
	(
		Value int
	)

	Select @Delimiter = ','
	
	Insert Into @TBL_Values
	Select	Value 
	From string_split( @RefIds,@Delimiter) 

	if @ActionType = 'B2B'
	Begin

		if Exists(Select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2B_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2B_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag 
			And IsNull(chksum,'') <> ''
		End

	End
	else if @ActionType = 'B2CL'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2CL_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2CL_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End

	End
	else if @ActionType = 'B2CS'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2CS
			Set flag = @NewFlag
			Where b2csid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2CS
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''

		End
	End
	else if @ActionType = 'EXP'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_EXP_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_EXP_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'CDNR'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_CDNR_NT
			Set flag = @NewFlag
			Where ntid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_CDNR_NT
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'CDNUR'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_CDNUR
			Set flag = @NewFlag
			Where cdnurid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_CDNUR
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if (@ActionType In ('HSN','HSNSUM'))
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_HSN_DATA
			Set flag = @NewFlag
			Where dataid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_HSN_DATA
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'NIL'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_NIL
			Set flag = @NewFlag
			Where nilid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_NIL
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'TXP'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_TXP
			Set flag = @NewFlag
			Where txpid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_TXP
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'AT'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_AT
			Set flag = @NewFlag
			Where atid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_AT
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if (@ActionType In ('DOCISSUE','DOC'))
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_DOC_ISSUE
			Set flag = @NewFlag
			Where docissueid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_DOC_ISSUE
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'B2BA'
	Begin

		if Exists(Select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2BA_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2BA_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag 
			And IsNull(chksum,'') <> ''
		End

	End
	else if @ActionType = 'B2CLA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2CLA_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2CLA_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End

	End
	else if @ActionType = 'B2CSA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_B2CSA
			Set flag = @NewFlag
			Where b2csaid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_B2CSA
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''

		End
	End
	else if @ActionType = 'EXPA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_EXPA_INV
			Set flag = @NewFlag
			Where invid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_EXPA_INV
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'CDNRA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_CDNRA_NT
			Set flag = @NewFlag
			Where ntid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_CDNRA_NT
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'CDNURA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_CDNURA
			Set flag = @NewFlag
			Where cdnuraid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_CDNURA
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'TXPA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_TXPA
			Set flag = @NewFlag
			Where txpaid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_TXPA
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End
	else if @ActionType = 'ATA'
	Begin
		if Exists(select 1 from @TBL_Values)
		Begin
			Update TBL_GSTR1_ATA
			Set flag = @NewFlag
			Where ataid In (Select Value From @TBL_Values)
			And IsNull(flag,'') = @CurFlag
		End
		Else
		Begin
			Update TBL_GSTR1_ATA
			Set flag = @NewFlag
			Where IsNull(flag,'') = @CurFlag
			And IsNull(chksum,'') <> ''
		End
	End


	
	Return 0
End