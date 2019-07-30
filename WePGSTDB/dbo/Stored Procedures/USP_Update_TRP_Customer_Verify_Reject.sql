-- =============================================
-- Author:		Naresh N
-- Create date: 19-Nov-17
-- Description:	Procedure to Retrieve the TRP UserList
-- exec USP_Update_TRP_Customer_Verify_Reject 9,5,1
-- =============================================
Create PROCEDURE [USP_Update_TRP_Customer_Verify_Reject] 
	@TRPCustId int,
	@Statuscode int,
	@ModifiedBy int
AS
BEGIN
	Update TBL_TRP_Customer set Statuscode=@Statuscode,ModifiedBy=@ModifiedBy,ModifiedDate=GETDATE() where trpCustId = @TRPCustId 
END