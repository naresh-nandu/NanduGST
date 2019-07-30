
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the GSTR2 Invoice Data
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
10/25/2017	Seshadri	Initial Version
10/30/2017	Seshadri	Fixed unit testing defects

*/

/* Sample Procedure Call

exec usp_Update_GSTR2_Invoice_Data
 */

CREATE PROCEDURE [usp_Update_GSTR2_Invoice_Data]
	@ActionType varchar(15),
	@FromInvIds nvarchar(MAX) = NULL,
	@ToInvIds nvarchar(MAX) = NULL,
	@UpdateType varchar(15), -- Supported Value :'INUM','IVAL'
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out


-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	DECLARE @TBL_FromValues TABLE 
	(
		Slno int,
		RefId int
	)

	DECLARE @TBL_ToValues TABLE 
	(
		Slno int,
		RefId int
	)

	Select @ActionType = Ltrim(Rtrim(IsNull(@ActionType,'')))
	Select @FromInvIds = Ltrim(Rtrim(IsNull(@FromInvIds,'')))
	Select @ToInvIds  = Ltrim(Rtrim(IsNull(@ToInvIds,'')))
	Select @UpdateType = Ltrim(Rtrim(IsNull(@UpdateType,'')))
	Select @Delimiter = ','


	Insert Into @TBL_FromValues
	(	Slno ,
		RefId 
	)
	Select  Row_Number() OVER(Order by Value ASC) as slno,
			Value 
	From string_split(@FromInvIds,@Delimiter) 

	Insert Into @TBL_ToValues
	(	Slno ,
		RefId 
	)
	Select  Row_Number() OVER(Order by Value ASC) as slno,
			Value 
	From string_split(@ToInvIds,@Delimiter) 

	if @ActionType = 'B2B'
	Begin

		if @UpdateType = 'INUM'
		Begin

			Update TBL_GSTR2_B2B_INV
			Set inum = t1.inum,
			    idt = t1.idt
			From TBL_GSTR2A_B2B_INV t1,
				 TBL_GSTR2_B2B_INV t2,
				 @TBL_FromValues t3,
				 @TBL_ToValues t4,
				 TBL_GSTR2A_B2B t5,
				 TBL_GSTR2_B2B t6
			Where t3.slno = t4.slno
			And t2.invid = t4.refid
			And t2.b2bid = t6.b2bid
			And t1.invid = t3.refid
			And t1.b2bid = t5.b2bid
		 	And t1.pos = t2.pos
			And t1.inv_typ = t2.inv_typ
			And t5.ctin = t6.ctin  
	
			if @@rowcount = 0
			Begin
				Select	@ErrorCode = -2,
						@ErrorMessage = 'Unable to update the invoice number and date.'
				Return @ErrorCode
			End

		End
		else if @UpdateType = 'IVAL'
		Begin

			Update TBL_GSTR2_B2B_INV
			Set val = t1.val
			From TBL_GSTR2A_B2B_INV t1,
				 TBL_GSTR2_B2B_INV t2,
				 @TBL_FromValues t3,
				 @TBL_ToValues t4,
				 TBL_GSTR2A_B2B t5,
				 TBL_GSTR2_B2B t6
			Where t3.slno = t4.slno
			And t2.invid = t4.refid
			And t2.b2bid = t6.b2bid
			And t1.invid = t3.refid
			And t1.b2bid = t5.b2bid
			And t1.inum = t2.inum
			And t1.idt = t2.idt
		 	And t1.pos = t2.pos
			And t1.inv_typ = t2.inv_typ
			And t5.ctin = t6.ctin  

			if @@rowcount = 0
			Begin
				Select	@ErrorCode = -3,
						@ErrorMessage = 'Unable to update the invoice value.'
				Return @ErrorCode
			End


		End

	End
	else if @ActionType = 'CDNR' or  @ActionType = 'CDN'
	Begin

		if @UpdateType = 'INUM'
		Begin

			Update TBL_GSTR2_CDNR_NT
			Set nt_num = t1.nt_num,
			    nt_dt = t1.nt_dt
			From TBL_GSTR2A_CDNR_NT t1,
				 TBL_GSTR2_CDNR_NT t2,
				 @TBL_FromValues t3,
				 @TBL_ToValues t4,
				 TBL_GSTR2A_CDNR t5,
				 TBL_GSTR2_CDNR t6
			Where t3.slno = t4.slno
			And t2.invid = t4.refid
			And t2.cdnrid = t6.cdnrid
			And t1.ntid = t3.refid
			And t1.cdnrid = t5.cdnrid
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And t5.ctin = t6.ctin  


			if @@rowcount = 0
			Begin
				Select	@ErrorCode = -2,
						@ErrorMessage = 'Unable to update the note number and date.'
				Return @ErrorCode
			End


		End
		else if @UpdateType = 'IVAL'
		Begin

			Update TBL_GSTR2_CDNR_NT
			Set val = t1.val
			From TBL_GSTR2A_CDNR_NT t1,
				 TBL_GSTR2_CDNR_NT t2,
				 @TBL_FromValues t3,
				 @TBL_ToValues t4,
				 TBL_GSTR2A_CDNR t5,
				 TBL_GSTR2_CDNR t6
			Where t3.slno = t4.slno
			And t2.invid = t4.refid
			And t2.cdnrid = t6.cdnrid
			And t1.ntid = t3.refid
			And t1.cdnrid = t5.cdnrid
			And t1.nt_num = t2.nt_num
			And t1.nt_dt = t2.nt_dt
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And t5.ctin = t6.ctin  

			if @@rowcount = 0
			Begin
				Select	@ErrorCode = -3,
						@ErrorMessage = 'Unable to update the invoice value.'
				Return @ErrorCode
			End


		End

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