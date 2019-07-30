-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Update the User details into Userlist Table
-- exec [Update_UserChangePassword] 1,'kk3@gmail.com','9008906453','kk','kknew','kknew',3
-- =============================================
CREATE PROCEDURE [Update_UserChangePassword] 
	@CustId    int,
	@Email    nvarchar(250),
	@MobileNo	nvarchar(50),
	@OldPassword    varchar(50),
	@NewPassword    varchar(50),
	@ConfirmPassword    varchar(50),
	@ChangeBy	int
AS
BEGIN
	Declare @DBOldPassword nvarchar(50),@RetValue int
	if exists(select CustId from TBL_Customer where CustId=@CustId and RowStatus=1)
		Begin
			if exists(select UserId from UserList where CustId=@CustId and UserId=@ChangeBy and Email=@Email and MobileNo=@MobileNo and Status=1)
				Begin
					select @DBOldPassword=Password from UserList where CustId=@CustId and UserId=@ChangeBy and Email=@Email and MobileNo=@MobileNo and Status=1
						if @OldPassword=@DBOldPassword
							Begin
								if @NewPassword=@ConfirmPassword
									Begin
										update UserList set Password=@NewPassword,LastModifiedBy=@ChangeBy,LastModifiedDate=GETDATE()  where CustId=@CustId and UserId=@ChangeBy and Email=@Email and MobileNo=@MobileNo and Status=1 
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