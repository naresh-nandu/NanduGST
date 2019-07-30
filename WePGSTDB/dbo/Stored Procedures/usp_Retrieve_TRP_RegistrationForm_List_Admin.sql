-- =============================================
-- Author:		Vishal S
-- Create date: 07-May-18
-- Description:	Procedure to Retrieve the TRP UserList
-- exec usp_Retrieve_TRP_UserList 1
-- =============================================
Create PROCEDURE [dbo].[usp_Retrieve_TRP_RegistrationForm_List_Admin] 
	
	
AS
BEGIN
select * From [TBL_MAS_TRP_TaxpayerDetails] 


END