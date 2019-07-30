-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 12-Nov-17
-- Description:	Procedure to Retrieve the PAN Details
-- exec usp_Retrieve_PAN_Details 1
-- =============================================

CREATE PROCEDURE [usp_Retrieve_PAN_Details] 
	@CustId int
AS
BEGIN
	select * from TBL_Cust_PAN where CustId = @CustId and RowStatus = 1
END