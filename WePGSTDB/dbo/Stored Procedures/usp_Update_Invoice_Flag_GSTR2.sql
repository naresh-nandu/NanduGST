
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the Invoice Flag for all GSTR2 Records in the staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/27/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

usp_Update_Invoice_Flag_GSTR2


 */
 
CREATE PROCEDURE [usp_Update_Invoice_Flag_GSTR2]  
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

		Update TBL_GSTR2_B2B_INV
		Set flag = @NewFlag
		Where invid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'B2BUR'
	Begin

		Update TBL_GSTR2_B2BUR_INV
		Set flag = @NewFlag
		Where invid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'IMPG'
	Begin

		Update TBL_GSTR2_IMPG
		Set flag = @NewFlag
		Where impgid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'IMPS'
	Begin

		Update TBL_GSTR2_IMPS
		Set flag = @NewFlag
		Where impsid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'CDNR'
	Begin

		Update TBL_GSTR2_CDNR_NT
		Set flag = @NewFlag
		Where invid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'CDNUR'
	Begin

		Update TBL_GSTR2_CDNUR
		Set flag = @NewFlag
		Where cdnurid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag
	End
	else if (@ActionType In ('HSN','HSNSUM'))
	Begin

		Update TBL_GSTR2_HSNSUM
		Set flag = @NewFlag
		Where hsnsumid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'NIL'
	Begin

		Update TBL_GSTR2_NIL
		Set flag = @NewFlag
		Where nilid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'TXP'
	Begin

		Update TBL_GSTR2_TXPD
		Set flag = @NewFlag
		Where txpdid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if @ActionType = 'TXI'
	Begin

		Update TBL_GSTR2_TXI
		Set flag = @NewFlag
		Where txiid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	else if (@ActionType In ('ITCRVSL'))
	Begin

		Update TBL_GSTR2_ITCRVSL
		Set flag = @NewFlag
		Where itcrvslid In (Select Value From @TBL_Values)
		And IsNull(flag,'') = @CurFlag

	End
	
	Return 0

End