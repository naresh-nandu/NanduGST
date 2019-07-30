-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Update the User details into Userlist Table
-- exec [Update_User_Block_Unblock] 21,7,0,1
-- =============================================
CREATE PROCEDURE [Update_User_Block_Unblock] 
	@CustomerId    int,
	@UserId	int,
	@Status	int,	
	@CreatedBy	int

AS
BEGIN
	
	update UserList set Status=@Status,LastModifiedBy=@CreatedBy,LastModifiedDate=GETDATE() where CustId=@CustomerId and UserId=@UserId
		
End