
-- =============================================
-- Author:		Karthik
-- Create date: 28-nov-2017
-- Description:	Procedure to Retrieve the customer Details
-- exec usp_Retrieve_TRP_Customer_Details 
-- =============================================

CREATE PROCEDURE [usp_Retrieve_TRP_Customer_Details]
	@TRPUserId int,
	@TRPId int,
	@RoleId int

AS
BEGIN
	
	Declare @RoleName varchar(150)

	select @RoleName = Role_Name from tbl_trp_roles where Role_Id = @RoleId
	
	select custId as AssignedCustId into #AssignedCustIds from TBL_TRP_UserAccess_Customer where trpId = @TRPId and trpUserId = @TRPUserId and rowstatus =1

	if @RoleName like '%Admin%' or @RoleName like '%Super Admin%'
		Begin
			select custid,Company from tbl_customer where TRPId = @TRPId and RowStatus = 1
		End
	Else
		Begin
			select custid,Company from tbl_customer where  TRPId = @TRPId and RowStatus = 1 and (createdby = @TRPUserId or custId in (select AssignedCustId from #AssignedCustIds))
		End
END