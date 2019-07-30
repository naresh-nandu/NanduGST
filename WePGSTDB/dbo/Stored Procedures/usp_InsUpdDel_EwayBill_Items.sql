
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert / Edit / Delete EwayBill Item
				
Written by  : vishal.singh@wepdigital.com

Date		Who			Decription 
03/08/2018	Vishal			Initial Version
05/23/2018  Muskan       Added Totaltaxvalue and corresponding calculations
*/

/* Sample Procedure Call

exec usp_InsUpdDel_EwayBill_Items 2,1

 */
 
CREATE PROCEDURE [dbo].[usp_InsUpdDel_EwayBill_Items]  
	@Mode tinyint, -- 0 : Insert , 1 - Edit / Update , 2 - Delete 
	@Itmsid int
	
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

	if @Mode = 2
	Begin
		-- Fetch the corresponding Eway Bill Id

		Select @EwbId = ewbid 
		From TBL_EWB_GENERATION_ITMS
		Where itmsid = @ItmsId
	
		-- Delete the corresponding Eway Bill Item
 
		Delete From TBL_EWB_GENERATION_ITMS
		Where itmsid = @Itmsid

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
			cgstVal = (convert(decimal(18,2),(taxableAmount*IsNull(cgstRate,0)/100)/2)),
			sgstVal = (convert(decimal(18,2),(taxableAmount*IsNull(sgstRate,0)/100)/2)),
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