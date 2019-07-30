CREATE PROCEDURE [usp_Update_EWAYBILL_CONSO_Items]
    @tripsheetid int,
	@ewbNo nvarchar(50)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Update TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET
SET  

ewbNo = @ewbNo
Where 
tripsheetid=@tripsheetid
end