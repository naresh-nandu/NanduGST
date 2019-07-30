-- =============================================
-- Author:		Raja M
-- Create date: 28-Nov-2017
-- Description:	Procedure to Retrieve the GSTIN Details
-- exec usp_Retrieve_TRP_GSTIN_Details 1
-- =============================================

CREATE PROCEDURE [usp_Retrieve_TRP_GSTIN_Details]
	@TRPUserId int,
	@TRPId int,
	@CustId int,
	@RoleId int

AS
BEGIN
	
	Declare @RoleName varchar(150)

	select @RoleName = Role_Name from tbl_trp_roles where Role_Id = @RoleId

	if @RoleName like '%Admin%' or @RoleName like '%Super Admin%'
		Begin
			select * from TBL_Cust_GSTIN where CustId = @CustId and RowStatus = 1
		End
	Else
		Begin
			select * from TBL_Cust_GSTIN where  CustId = @CustId and   RowStatus = 1 and createdby = @TRPUserId
		End
END