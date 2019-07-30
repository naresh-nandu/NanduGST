-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 18-Nov-17
-- Description:	Procedure to Retrieve the TRP UserListById(Based On Primary Key)
-- exec usp_Retrieve_TRP_UserListById 1
-- =============================================
Create PROCEDURE [usp_Retrieve_TRP_UserListById] 
	@TrpUserId int
AS
BEGIN
	select * from TBL_TRP_Userlist where trpUserId = @TrpUserId
END