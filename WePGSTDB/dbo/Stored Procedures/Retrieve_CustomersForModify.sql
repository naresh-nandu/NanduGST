-- =============================================
-- Author:		KK
-- Create date: 07-Apr-17
-- Description:	Procedure to Retrieve the Customer Details for modify
-- exec [Retrieve_CustomersForModify] 21
-- =============================================
CREATE PROCEDURE [Retrieve_CustomersForModify]
@CustomerId int 
	
AS
BEGIN
	select * from TBL_Customer where CustId=@CustomerId
END