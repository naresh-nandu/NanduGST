-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 16-Nov-17
-- Description:	Procedure to Retrieve the TRP UserList Based on Primary key(Retrieve Data for Edit User)
-- exec usp_Retrieve_TRP_UserListByTRPUserId 1
-- =============================================
Create PROCEDURE [usp_Retrieve_TRP_UserListByTRPUserId] 
	@TRPUserId int
AS
BEGIN
	select * from TBL_TRP_Userlist where trpUserId = @TRPUserId 
END