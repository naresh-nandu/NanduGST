-- =============================================
-- Author:		KK
-- Create date: 07-Apr-17
-- Description:	Procedure to Retrieve the User Details for modify
-- exec [Retrieve_UserDetailsForModify] 21,7
-- =============================================
CREATE PROCEDURE [Retrieve_UserDetailsForModify]
@CustomerId int,
@UserId int 
	
AS
BEGIN
	select * from UserList where CustId=@CustomerId and UserId=@UserId
END