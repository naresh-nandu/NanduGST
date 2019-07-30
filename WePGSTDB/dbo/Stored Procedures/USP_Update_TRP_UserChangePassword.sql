/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Change password for TRP Users
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version


*/

/* Sample Procedure Call  

exec USP_Update_TRP_UserChangePassword 17,1,1

 */
CREATE PROCEDURE [USP_Update_TRP_UserChangePassword] 
	@TrpId    int,
	@Email    nvarchar(250),
	@MobileNo	nvarchar(50),
	@OldPassword    varchar(50),
	@NewPassword    varchar(50),
	@ConfirmPassword    varchar(50),
	@ChangeBy	int
AS
BEGIN
	Declare @DBOldPassword nvarchar(50),@RetValue int
	if exists(select trpCustId from TBL_TRP_Customer where trpCustId=@TrpId and RowStatus=1)
		Begin
			if exists(select trpUserId from TBL_TRP_Userlist where trpCustId=@TrpId and  Email=@Email and MobileNo=@MobileNo and Status=1)
				Begin
					select @DBOldPassword=Password from TBL_TRP_Userlist where trpCustId=@TrpId and Email=@Email and MobileNo=@MobileNo and Status=1
						if @OldPassword=@DBOldPassword
							Begin
								if @NewPassword=@ConfirmPassword
									Begin
										update TBL_TRP_Userlist set Password=@NewPassword,LastModifiedBy=@ChangeBy,LastModifiedDate=GETDATE()  where trpCustId=@TrpId and trpUserId=@ChangeBy and Email=@Email and MobileNo=@MobileNo and Status=1 
										set @RetValue=-1 -- Password changed successfully
									End
								  Else
									Begin
										set @RetValue=-2 -- Newpassword and confirm pass word not matching
									End
							End
						Else
							Begin
								set @RetValue=-3  -- Old password is in correct.
							End

				End
				Else
				Begin
					set @RetValue=-4 --Logged in Person not authorized to change password. bacause may be wrong in mail Id, Phone no or user status may be blocked.
				End
		End
	Else
		Begin
			set @RetValue=-5 -- Customer is blocked so user is not allowing access or change the password.
		End
print @RetValue
return @RetValue
End