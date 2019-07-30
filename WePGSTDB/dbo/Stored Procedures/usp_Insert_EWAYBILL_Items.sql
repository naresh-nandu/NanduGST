create PROCEDURE [usp_Insert_EWAYBILL_Items]	
   @itmsid int,
   @ewbid int,
	@productName nvarchar(100),
	@productDesc nvarchar(max),
	@hsnCode nvarchar(50),
	@quantity decimal(18,2),
	@qtyUnit nvarchar(10),
	@taxableAmount decimal(18,2),
	@cgstRate decimal(18,2),
	@sgstRate decimal(18,2),
	@igstRate decimal(18,2),
	@cessRate decimal(18,2),	
	@Referenceno varchar(50),
	@createdby int
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15)
				
	Select @SourceType = 'Manual'
	
	Insert into TBL_EWB_GENERATION_ITMS
	(itmsid,ewbid,productName, productDesc, hsnCode, quantity, qtyUnit, cgstRate, sgstRate, igstRate, cessRate
	) 
	Values
	(@itmsid,@ewbid,@productName, @productDesc, @hsnCode, @quantity, @qtyUnit, @cgstRate, @sgstRate, @igstRate, @cessRate)	

	exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo
					
	Return 0

End

select*from TBL_EWB_GENERATION_ITMS