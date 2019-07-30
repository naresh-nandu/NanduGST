CREATE PROCEDURE [dbo].[usp_Update_Taxpayer_Details_Reg]
    @RegId int,
    @Taxpayer nvarchar(10),
	@TRPMasterId nvarchar(10),
	@State nvarchar(100),
	@District nvarchar(50),
	@LegalName nvarchar(40),	
	@PanNo nvarchar(10),
	@MobNumber nvarchar(50),	
	@Mail nvarchar(60),
	@TradeName nvarchar(50),
	@custid nvarchar(50)
	

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Update TBL_MAS_TRP_TaxpayerDetails
SET  

    Taxpayer = @Taxpayer ,
	[State] =@State,
	District =@District,
	LegalName =@LegalName,
	PanNo =@PanNo,
	MobNumber= @MobNumber,
	Mail =@Mail,
	TradeName= @TradeName,
	custid=@custid
	
Where 
RegId=@RegId
end