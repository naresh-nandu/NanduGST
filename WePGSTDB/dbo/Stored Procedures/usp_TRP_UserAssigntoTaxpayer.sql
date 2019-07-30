
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Assign TRP User to Taxpayer
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call

usp_TRP_UserAssigntoTaxpayer 1,1,1,1,0
 */
CREATE PROCEDURE [usp_TRP_UserAssigntoTaxpayer] 
	@TrpId  int,
	@TrpUserId Int,
	@custId int,
	@AssignBy int,
	@RetValue	int Output
-- /*mssx*/ With Encryption 

AS
BEGIN

Declare @custrefno varchar(50),@pwrd varchar(15),@Email varchar(50)
	if not exists(select 1 from TBL_TRP_UserAccess_Customer where trpid = @TrpId and trpUserid = @TrpUserId and custid = @custId and rowstatus =1)
	begin
		if exists (select 1 from tbl_customer where custid = @custId and rowstatus =1)
		Begin
			set @custrefno = (select ReferenceNo from tbl_customer where custid = @custId and rowstatus =1) 
			if exists (select 1 from UserList where custid = @custId and rowstatus =1 )
				Begin
					select top(1) @pwrd = [Password],@Email = Email from UserList where custid = @custId and rowstatus =1  order by userid
					Begin
						insert into TBL_TRP_UserAccess_Customer (trpUserId, trpid,custid,custRefno,[password],Email,createdby,createdDate,rowstatus)
															Values(@TrpUserId,@TrpId,@custId,@custrefno,@pwrd,@Email,@AssignBy,getdate(),1)
						if @@rowcount >0
							Begin
								declare @msg varchar(max),@TRPUserName varchar(150)
								select @TRPUserName =[Name] from TBL_TRP_Userlist where  trpUserId=@TrpUserId
								set @msg = 'TRP : Customer is assgined to User : '+@TRPUserName +' Customer Ref No : '+ @custrefno
								exec [Ins_AuditLog] @AssignBy,'TRP-User Assign',@msg,'NA'
							End
						set @RetValue=1 -- User Assigned to customer
					End
				End
		End
	End
	Else
	Begin
		set @RetValue=2 -- User Already assinged to this customer
		-- Already Exists
	End
End