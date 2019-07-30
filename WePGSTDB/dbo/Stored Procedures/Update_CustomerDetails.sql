-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Modify the Customer details into Tbl_Customer Table
-- exec [Update_CustomerDetails] 1,'raja.m@wepindia.com','8050012205',7
-- =============================================
CREATE PROCEDURE [Update_CustomerDetails] 
	@CustomerId int,	
	@Email    varchar(50),
	@MobileNo    varchar(50),
	@UserId int	
AS
BEGIN


Declare @OldEmailId nvarchar(100), @OldMobileNo nvarchar(50)

	if exists (select CustId from TBL_Customer where CustId=@CustomerId)
	Begin 
		select @OldEmailId=Email,@OldMobileNo=MobileNo from TBL_Customer where CustId=@CustomerId
		Begin Transaction
			Update TBL_Customer set Email=@Email,MobileNo=@MobileNo,ModifiedBy=@UserId,ModifiedDate=GETDATE() where CustId=@CustomerId and Email=@OldEmailId and MobileNo=@OldMobileNo

			if @@ROWCOUNT>0
				Begin
					Update UserList set Email=@Email,MobileNo=@MobileNo,LastModifiedBy=@UserId,LastModifiedDate=GETDATE() where CustId=@CustomerId and Email=@OldEmailId and MobileNo=@OldMobileNo

					if @@ROWCOUNT>0
						Begin
							Commit
						End
						Else
						Begin
							RollBack
						End

				End
	End

	
	
END