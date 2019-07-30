﻿ 
CREATE PROCEDURE [usp_Update_EWAYBILL_GENERATION_Master_With_Items]
    @ewbid int,
    @supplyType  nvarchar(15),
	@subSupplyType nvarchar(15),
	@docType nvarchar(15),
	@docNo nvarchar(50),
	@docDate nvarchar(15),
	@fromGstin nvarchar(15),
	@fromTrdName nvarchar(100),
	@fromAddr1 nvarchar(max),
	@fromAddr2 nvarchar(max),
	@fromPlace nvarchar(50),
	@fromPinCode nvarchar(6),
	@fromStateCode nvarchar(2),
	@toGstin nvarchar(15),
	@toTrdName nvarchar(100),
	@toAddr1 nvarchar(max),
	@toAddr2 nvarchar(max),
	@toPlace nvarchar(100),
	@toPincode nvarchar(6),
	@toStateCode nvarchar(2),
	@transMode int,
	@transDistance decimal(18,2),
	@transporterId nvarchar(15),
	@transporterName nvarchar(100),
	@transDocNo nvarchar(15),
	@transDocDate nvarchar(10),
	@vehicleNo nvarchar(20),
	@totalValue decimal(18,2),
	@cgstValue decimal(18,2),
	@sgstValue decimal(18,2),
	@igstValue decimal(18,2),
	@cessValue decimal(18,2)


-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Update TBL_EWB_GENERATION
	Set supplyType=@supplyType,
		subSupplyType=@subSupplyType,
		docType=@docType,
		fromGStin=@fromGstin,
	    fromTrdName=@fromTrdName,
	    fromAddr1=@fromAddr1,
	    fromAddr2=@fromAddr2,
	    fromPlace=@fromPlace,
	    fromPinCode=@fromPinCode,
	    fromStateCode=@fromStateCode,
	    toGstin=@toGstin,
	    toTrdName=@toTrdName,
	    toAddr1=@toAddr1,
	    toAddr2=@toAddr2,
	    toPlace=@toPlace,
	    toPincode=@toPincode,
	    toStateCode=@toStateCode,
	    transMode=@transMode,
	    transDistance=@transDistance,
	    transporterId=@transporterId,
	    transporterName=@transporterName,
	    transDocNo=@transDocNo,
	    transDocDate=@transDocDate,
	    VehicleNo=@vehicleNo,	    
        totalValue = @totalValue,
        cgstValue=@cgstValue,
        sgstValue =@sgstValue,
	    igstValue=@igstValue,
	    cessValue =@cessValue
	Where 
	 ewbid=@ewbid
	
	
End