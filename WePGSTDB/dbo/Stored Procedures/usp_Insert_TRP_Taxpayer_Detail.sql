
CREATE PROCEDURE [dbo].[usp_Insert_TRP_Taxpayer_Detail]		

	@Taxpayer nvarchar(10),
	@BusinessType nvarchar(100),
	@State nvarchar(100),
	@District nvarchar(50),
	@LegalName nvarchar(40),	
	@PanNo nvarchar(10),
	@MobNumber nvarchar(50),	
	@Mail nvarchar(60),
	@TradeName nvarchar(50),
	
	@custid nvarchar(50),
	@createdDate nvarchar(50)
					
	
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15)
				
	Select @SourceType = 'Manual'	
	Insert into TBL_MAS_TRP_TaxpayerDetails
	(Taxpayer,BusinessType, [State], District, LegalName, PanNo,MobNumber,Mail,TradeName,custid,createdDate)
	Values
	(@Taxpayer,@BusinessType, @State, @District, @LegalName, @PanNo, @MobNumber,@Mail,@TradeName,@custid,@createdDate)	
	Return 0

End