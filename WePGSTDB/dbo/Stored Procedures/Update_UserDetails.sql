-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Update the User details into Userlist Table
-- exec [Update_UserDetails] 21,7,'Karthik','Software Dev','KarthikK','kksoft@gmail.com','9008906454'
-- =============================================
Create PROCEDURE [Update_UserDetails] 
	@CustId    int,
	@UserId	int,
	@Name    nvarchar(150),
	@Designation    varchar(50),
	@Username    varchar(50),
	@Email    nvarchar(250),
	@MobileNo    varchar(50),
	@CreatedBy	int

AS
BEGIN
	
	
	if exists(select UserId from UserList where CustId=@CustId and UserId =@UserId and [status]=1)
	Begin
		
		Update Userlist set Name=@Name,Designation=@Designation,Username=@Username,Email=@Email,MobileNo=@MobileNo,LastModifiedBy=@CreatedBy,LastModifiedDate=GETDATE()
		 where CustId=@CustId and UserId =@UserId  and [status]=1
	END

End