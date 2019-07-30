CREATE PROCEDURE [dbo].[usp_Update_Taxpayer_Details_Reg_Admin]
    @RegId int,
    @Taxpayer nvarchar(10),

	@District nvarchar(50),
	@LegalName nvarchar(40),	
	@PanNo nvarchar(10),
	@MobNumber nvarchar(50),	
	@Mail nvarchar(60),
	@TradeName nvarchar(50),
	
	@ARN nvarchar(50),
	@TRN nvarchar(50),
	@gstinnum nvarchar(50)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Update TBL_MAS_TRP_TaxpayerDetails
SET  

    Taxpayer = @Taxpayer ,
	
	
	District =@District,
	LegalName =@LegalName,
	PanNo =@PanNo,
	MobNumber= @MobNumber,
	Mail =@Mail,
	TradeName= @TradeName,

	 ARN = @ARN,
     TRN =@TRN,
	gstinnum=  @gstinnum
Where 
RegId=@RegId
end