CREATE PROCEDURE [usp_Update_EWAYBILL_GENERATION_Items]
    @itmsid int,
	@productDesc nvarchar(max),
	@productName nvarchar(max),
	@hsnCode nvarchar(50),
	@quantity decimal(18,2),
	@taxableAmount decimal(18,2),
	@cgstRate decimal(18,2),
	@sgstRate decimal(18,2),
	@igstRate decimal(18,2),
	@cessRate decimal(18,2)	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Update TBL_EWB_GENERATION_ITMS 

SET  
productdesc = @productdesc, 
productName=@productName,
hsnCode = @hsnCode,  
quantity = @quantity ,
taxableAmount = @taxableAmount,
cgstRate=@cgstRate,
sgstRate =@sgstRate,
	igstRate=@igstRate,
	cessRate =@cessRate
Where itmsid=@itmsid
	
	
end