
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update/Delete Assign TRP User to Taxpayer
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call

usp_Update_TRP_UserAssigntoTaxpayer 8,1,1,1,'D'
 */
CREATE PROCEDURE [usp_Update_TRP_UserAssigntoTaxpayer] 
	@UserAccessId int,
	@TrpId  int,
	@TrpUserId Int,
	@custId int,
	@Action varchar(1),
	@RetValue	int=NULL Out,
	@Retmsg	varchar(250)=NULL Out
-- /*mssx*/ With Encryption 

AS
BEGIN
	Declare @custrefno varchar(50)=NULL,@pwrd varchar(15)=NULL,@Email varchar(50)=NULL

	If @Action = 'M'
		Begin
				if exists (select 1 from tbl_customer where custid = @custId and rowstatus =1)
					Begin
						set @custrefno = (select ReferenceNo from tbl_customer where custid = @custId and rowstatus =1) 
					End
				if exists (select 1 from UserList where custid = @custId and rowstatus =1 )
					Begin
						select top(1) @pwrd = [Password],@Email = Email from UserList where custid = @custId and rowstatus =1  order by userid
					End
							if not exists(select 1 from TBL_TRP_UserAccess_Customer where trpid = @TrpId and CustId = @CustId and rowstatus =1)
								Begin
									Update TBL_TRP_UserAccess_Customer 
										set  trpid = @TrpId,
											trpUserId = @TrpUserId,
											CustId = @CustId,
											CustRefNo =@custrefno,
											[Password] = @pwrd,
											Email = @Email where UserAccessId = @UserAccessId and rowstatus =1
									Set @RetValue = 1
									Set @Retmsg = 'Customer Mapping updated successfully'
									
								End
								Else
								Begin
									Set @RetValue = -1
									Set @Retmsg = 'This Customer Mapping is already exist for the same User.'
								End
		End
	Else If @Action = 'D'
		Begin
			Update TBL_TRP_UserAccess_Customer set  rowstatus = 0 where UserAccessId = @UserAccessId and rowstatus =1
			Set @RetValue = -2
			Set @Retmsg = 'Customer mapping is deleted Successfully.'
		End

	Select convert(varchar(10),@RetValue) + ' : ' + @Retmsg as 'Output Message'

End