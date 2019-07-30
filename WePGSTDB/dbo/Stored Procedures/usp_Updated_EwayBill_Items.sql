CREATE PROCEDURE [dbo].[usp_Updated_EwayBill_Items]  
	
	@Itmsid int,
	@productDesc nvarchar(max),
	@hsnCode nvarchar(50),
	@quantity decimal(18,2),
	@taxableAmount decimal(18,2),
	@totaltaxablevalue decimal(18,2)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	-- Variable Declaration

	Declare @EwbId int,
			@IgstVal dec(18,2),
			@CgstVal dec(18,2),
			@SgstVal dec(18,2),
			@CessVal dec(18,2),
			@TotalVal dec(18,2),
			@TotalTaxVal dec(18,2)

	-- Delete Operation ---------

	
	Begin
		-- Fetch the corresponding Eway Bill Id

		Select @EwbId = ewbid 
		From TBL_EWB_GENERATION_ITMS
		Where itmsid = @ItmsId
	
		-- Delete the corresponding Eway Bill Item
Update TBL_EWB_GENERATION_ITMS 
SET  
productdesc = @productdesc, 
hsnCode = @hsnCode,  
quantity = @quantity ,
taxableAmount = @taxableAmount,
totaltaxablevalue = @totaltaxablevalue
Where itmsid=@itmsid

		-- Select the remaining Eway Bill Items for the corresponding
		--	Eway Bill Id and perform the calculations
 
		Select	itmsid,
				taxableAmount,
				totaltaxablevalue,
				igstRate,
				cgstRate,
				sgstRate,
				cessRate,
				@IgstVal as igstVal,
				@CgstVal as cgstVal,
				@SgstVal as sgstVal,
				@CessVal as cessVal
		Into #TBL_EWB_ITMS 
		From TBL_EWB_GENERATION_ITMS 
		Where ewbid = @EwbId


		Update #TBL_EWB_ITMS
		Set igstVal=(convert(decimal(18,2),taxableAmount*IsNull(igstRate,0)/100)),
			cgstVal = (convert(decimal(18,2),taxableAmount*IsNull(cgstRate,0)/200)),
			sgstVal = (convert(decimal(18,2),taxableAmount*IsNull(sgstRate,0)/200)),
			cessVal=(convert(decimal(18,2),taxableAmount*IsNull(cessRate,0)/100))

		Select @IgstVal = sum(IsNull(igstVal,0)),
			   @CgstVal = sum(IsNull(cgstVal,0)),
			   @SgstVal = sum(IsNull(sgstVal,0)),
			   @CessVal = sum(IsNull(cessVal,0)),
			   @TotalVal = sum(IsNull(taxableAmount,0)),
			   @TotalTaxVal = sum(IsNull(totaltaxablevalue,0))
		From #TBL_EWB_ITMS
	
		   	 
		Update TBL_EWB_GENERATION
		Set igstValue= @IgstVal,
			cgstValue= @CgstVal ,
			sgstValue= @SgstVal,
			cessValue=@CessVal,
			totalValue=@TotalVal,
			totinvvalue=@TotalVal + @IgstVal + @CgstVal + @SgstVal
		Where ewbid = @EwbId

	End

	
	Return 0


End