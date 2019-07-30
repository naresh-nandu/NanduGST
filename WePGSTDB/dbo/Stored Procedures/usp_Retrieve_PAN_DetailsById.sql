-- =============================================
-- Author:		Naresh Nuthalapati
-- Create date: 13-Nov-17
-- Description:	Procedure to Retrieve the PAN Details based PANId
-- exec usp_Retrieve_PAN_DetailsById 220
-- =============================================

CREATE PROCEDURE [usp_Retrieve_PAN_DetailsById] 
	@PANId int
AS
BEGIN
	select * from TBL_Cust_PAN where PANId = @PANId 
END