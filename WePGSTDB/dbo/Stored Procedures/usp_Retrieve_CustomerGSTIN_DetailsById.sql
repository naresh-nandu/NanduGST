-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 13-Nov-17
-- Description:	Procedure to Retrieve the Customer GSTIN Based on GSTINId
-- exec usp_Retrieve_CustomerGSTIN_DetailsById 220
-- =============================================

CREATE PROCEDURE [usp_Retrieve_CustomerGSTIN_DetailsById] 
	@GSTINId int
AS
BEGIN
	select * from TBL_Cust_GSTIN where GSTINId = @GSTINId 
END