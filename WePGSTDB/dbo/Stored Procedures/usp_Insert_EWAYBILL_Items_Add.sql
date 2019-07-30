CREATE PROCEDURE [dbo].[usp_Insert_EWAYBILL_Items_Add]	
  
   @ewbid int,
	
	@productDesc nvarchar(max),
	@productName nvarchar(max),
	@hsnCode nvarchar(50),
	@quantity decimal(18,2),
	@qtyUnit nvarchar(max),
	@taxableAmount decimal(18,2),
	@totaltaxablevalue decimal(18,2),
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
	(ewbid,productDesc,	productName, hsnCode, quantity,qtyUnit,taxableAmount,totaltaxablevalue, cgstRate, sgstRate, igstRate, cessRate,  
	 createdby) 
	Values
	(@ewbid, @productDesc,@productName, @hsnCode, @quantity,@qtyUnit,@taxableAmount,@totaltaxablevalue, @cgstRate, @sgstRate, @igstRate, @cessRate,  
 @createdby)	


					
	Return 0

End