-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 13-Nov-17
-- Description:	Procedure to Retrieve the Customer GSTIN 
-- exec usp_Retrieve_CustomerGSTIN_Details 30
-- =============================================

Create PROCEDURE [usp_Retrieve_CustomerGSTIN_Details] 
	@CustId int
AS
BEGIN
	select * from TBL_Cust_GSTIN where CustId = @CustId and rowstatus=1
END