
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete the PAN from the corresponding tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/20/2017	Seshadri		Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_PAN 
 */

CREATE PROCEDURE [usp_Delete_PAN]
	@PanId int,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @RowStatus bit, @Pan varchar(10),@custid int

	if Exists(Select 1 From TBL_Cust_PAN Where panid = @PanId)
	Begin

		Select @RowStatus = rowstatus,
			   @Pan = panno,
			   @custid = custid
		From TBL_Cust_PAN Where panid = @PanId
		if @RowStatus = 0
		Begin
			Select	@ErrorCode = -2,
					@ErrorMessage = 'PAN already deleted.'
			Return @ErrorCode
		End

		if not exists(select 1 from Tbl_Customer where custid =@custid and PANNo = @Pan and rowstatus =1)
		Begin
			Update UserAccess_Gstin
			Set rowstatus = 0
			Where GstinId In (Select gstinid From Tbl_Cust_Gstin Where panno = @Pan)

			Update TBL_Cust_GSTIN
			Set rowstatus = 0
			Where panno = @Pan

			Update TBL_Cust_PAN_Docs
			Set rowstatus = 0
			Where panid = @PanId

			Update TBL_Cust_PAN
			Set rowstatus = 0
			Where panid = @PanId
			Select	@ErrorCode = 1,
				@ErrorMessage = 'PAN Deleted Successfully.'
		Return @ErrorCode
		End
		Else
		Begin
			Select	@ErrorCode = -2,
				@ErrorMessage = 'Master PAN No cannot be deleted.'
		Return @ErrorCode
		End

	End
	else
	Begin
		Select	@ErrorCode = -1,
				@ErrorMessage = 'Invalid PAN ID.'
		Return @ErrorCode
	End


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End