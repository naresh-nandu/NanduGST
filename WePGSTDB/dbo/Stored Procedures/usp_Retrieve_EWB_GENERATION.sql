

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve EWAYBILL Data
				
Written by  : raja.m@wepdigital.com 

Date		Who						Decription 
06/02/2018	Raja M	 				Initial Version
18/05/2018	RAJA M					Branch ID Included

*/

/* Sample Procedure Call

exec usp_Retrieve_EWB_GENERATION '05AAACC4214B1ZK','1/3/2018','18/05/2018',1,32,'ID'

 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_EWB_GENERATION]
	@userGSTIN nvarchar(15),
	@FromDate nvarchar(20),
	@ToDate nvarchar(20),
	@CustId int,
	@BranchId int = NULL,
	@Mode varchar(5) /*PD-Print and Downoad
	                   EG-Edit and Generate 
					   ID-Invocie Data*/

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	IF @Mode = 'PD'
	BEGIN
		IF @BranchId <> 0 AND ISNULL(@BranchId, NULL) <> NULL
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = '' 
			AND ewayBillNo IS NOT NULL AND ewayBillDate IS NOT NULL AND validUpto IS NOT NULL AND BranchId = @BranchId		
			AND (CONVERT(DATETIME, ewayBillDate, 105) between CONVERT(DATETIME, @FromDate, 105) 
													And CONVERT(DATETIME, DATEADD(day, 2, CONVERT(DATETIME, @ToDate, 105)), 105))
			Order By 1 Desc
		END
		ELSE
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = '' 
			AND ewayBillNo IS NOT NULL AND ewayBillDate IS NOT NULL AND validUpto IS NOT NULL --AND BranchId = @BranchId		
			AND (CONVERT(DATETIME, ewayBillDate, 105) between CONVERT(DATETIME, @FromDate, 105) 
													And CONVERT(DATETIME, DATEADD(day, 2, CONVERT(DATETIME, @ToDate, 105)), 105))
			Order By 1 Desc
		END
	END
    IF @Mode = 'EG'
	BEGIN
		IF @BranchId <> 0 AND ISNULL(@BranchId, NULL) <> NULL
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = ''
			AND (CONVERT(DATETIME, docDate, 105) between CONVERT(DATETIME, @FromDate, 105) 
													And CONVERT(DATETIME, @ToDate, 105))
			AND BranchId = @BranchId
			--AND ISNULL(transMode,'') = ''
			--AND ISNULL(transDistance,'') = ''
			--AND ISNULL(transporterId,'') = ''
			--AND ISNULL(transporterName,'') = ''
			--AND ISNULL(transDocNo,'') = ''
			--AND ISNULL(transDocDate,'') = ''
			--AND ISNULL(vehicleNo,'') = ''
			--AND ISNULL(vehicleType,'') = ''
			Order By 1 Desc
		END
		ELSE
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = ''
			AND (CONVERT(DATETIME, docDate, 105) between CONVERT(DATETIME, @FromDate, 105) 
													And CONVERT(DATETIME, @ToDate, 105))
			--AND BranchId = @BranchId
			--AND ISNULL(transMode,'') = ''
			--AND ISNULL(transDistance,'') = ''
			--AND ISNULL(transporterId,'') = ''
			--AND ISNULL(transporterName,'') = ''
			--AND ISNULL(transDocNo,'') = ''
			--AND ISNULL(transDocDate,'') = ''
			--AND ISNULL(vehicleNo,'') = ''
			--AND ISNULL(vehicleType,'') = ''
			Order By 1 Desc
		END
	END
	IF @Mode = 'ID'
	BEGIN
		IF @BranchId <> 0 AND ISNULL(@BranchId, NULL) <> NULL
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = '' AND BranchId = @BranchId
			AND (CONVERT(DATETIME, docDate, 105) between CONVERT(DATETIME, @FromDate, 105) And CONVERT(DATETIME, @ToDate, 105))
			--AND ISNULL(transMode, 0) = 0
			--AND ISNULL(transporterId,'') = ''
			--AND ISNULL(transporterName,'') = ''
			--AND ISNULL(transDocNo,'') = ''
			--AND ISNULL(transDocDate,'') = ''
			--AND ISNULL(vehicleNo,'') = ''
			--AND ISNULL(vehicleType,'') = ''
			Order By 1 Desc
		END
		ELSE
		BEGIN
			Select * From TBL_EWB_GENERATION 
			Where userGSTIN = @userGSTIN AND CustId = @CustId AND ISNULL(flag,'') = '' --AND BranchId = @BranchId
			AND (CONVERT(DATETIME, docDate, 105) between CONVERT(DATETIME, @FromDate, 105) And CONVERT(DATETIME, @ToDate, 105))
			--AND ISNULL(transMode, 0) = 0
			--AND ISNULL(transporterId,'') = ''
			--AND ISNULL(transporterName,'') = ''
			--AND ISNULL(transDocNo,'') = ''
			--AND ISNULL(transDocDate,'') = ''
			--AND ISNULL(vehicleNo,'') = ''
			--AND ISNULL(vehicleType,'') = ''
			Order By 1 Desc
		END
	END
End