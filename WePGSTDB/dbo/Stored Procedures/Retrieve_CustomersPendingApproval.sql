-- =============================================
-- Author:		KK
-- Create date: 07-Apr-17
-- Description:	Procedure to Retrieve the Customer list which is poending for approval
-- exec [Retrieve_CustomersPendingApproval]
-- =============================================
CREATE PROCEDURE [Retrieve_CustomersPendingApproval] 
	
AS
BEGIN
	select * from TBL_Customer where StatusCode=2 and RowStatus=1	
END