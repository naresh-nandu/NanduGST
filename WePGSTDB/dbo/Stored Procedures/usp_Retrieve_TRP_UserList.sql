-- =============================================
-- Author:		RAJA M
-- Create date: 11-Nov-17
-- Description:	Procedure to Retrieve the TRP UserList
-- exec usp_Retrieve_TRP_UserList 1
-- =============================================
CREATE PROCEDURE [usp_Retrieve_TRP_UserList] 
	@TRPCustId int
AS
BEGIN
	select * from TBL_TRP_Userlist where trpCustId = @TRPCustId and rowstatus = 1
END