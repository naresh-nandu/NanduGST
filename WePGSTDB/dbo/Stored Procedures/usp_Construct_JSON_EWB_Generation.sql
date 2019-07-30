

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
01/03/2018	Raja		Initial Version
24/05/2018	Raja		UserGSTIN parameter included

*/

/* Sample Procedure Call
Select * from TBL_EWB_GENERATION where docNo = 'admin123545'
exec usp_Construct_JSON_EWB_Generation 'raja1212','15/05/2018'
 */

CREATE PROCEDURE [dbo].[usp_Construct_JSON_EWB_Generation] 
	@UserGstin varchar(15),
	@DocNo varchar(15),
	@DocDate varchar(10),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
as 
Begin

	Set Nocount on

		if exists (select 1 from TBL_EWB_GENERATION where userGSTIN = @userGSTIN and docNo = @DocNo and docDate = @DocDate 
			And isnull(ewayBillNo,'') = '' 
			And isnull(transporterId,'') <> ''
			And isnull(transDocNo,'') <> ''
			And isnull(transDocDate,'') <> ''
			And isnull(transMode,'') <> ''
			And isnull(vehicleNo,'') <> ''
			And isnull(vehicleType,'') <> '')
		Begin
			Select TOP 1 supplyType, subSupplyType, docType, docNo, docDate, --usertype, genmode, usergstin, userid, 
					fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPincode, fromStateCode, actFromStateCode,
					toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
					totalValue, cgstValue, sgstValue, igstValue, cessValue, totinvvalue, 
					transporterId, transporterName, transDocNo, transMode, transDistance, transDocDate, vehicleNo, vehicleType,
				( Select productName, productDesc, hsnCode, quantity, qtyUnit, cgstRate, sgstRate, igstRate, cessRate, cessAdvol, taxableAmount
					From TBL_EWB_GENERATION_ITMS
					Where ewbid = TBL_EWB_GENERATION.ewbid
					FOR JSON PATH	
				) As itemList
				From TBL_EWB_GENERATION 
				Where TBL_EWB_GENERATION.userGSTIN = @userGSTIN
				AND TBL_EWB_GENERATION.docNo = @DocNo
				AND TBL_EWB_GENERATION.docDate = @DocDate	
				AND TBL_EWB_GENERATION.ewayBillNo IS NULL
				Order By ewbid desc			
				FOR JSON AUTO
		End
		Else if exists (select 1 from TBL_EWB_GENERATION where userGSTIN = @userGSTIN and docNo = @DocNo and docDate = @DocDate 
			And isnull(ewayBillNo,'') = '' 
			And isnull(transporterId,'') = ''
			And isnull(transDocNo,'') = ''
			And isnull(transDocDate,'') = ''
			And isnull(transMode,'') = ''
			And isnull(vehicleNo,'') = ''
			And isnull(vehicleType,'') = '')			
		Begin
			Select TOP 1 supplyType, subSupplyType, docType, docNo, docDate, --usertype, genmode, usergstin, userid, 
					fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPincode, fromStateCode, actFromStateCode,
					toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
					totalValue, cgstValue, sgstValue, igstValue, cessValue, totinvvalue,transDistance,
				( Select productName, productDesc, hsnCode, quantity, qtyUnit, cgstRate, sgstRate, igstRate, cessRate, cessAdvol, taxableAmount
					From TBL_EWB_GENERATION_ITMS
					Where ewbid = TBL_EWB_GENERATION.ewbid
					FOR JSON PATH	
				) As itemList
				From TBL_EWB_GENERATION 
				Where TBL_EWB_GENERATION.userGSTIN = @userGSTIN
				AND TBL_EWB_GENERATION.docNo = @DocNo
				AND TBL_EWB_GENERATION.docDate = @DocDate	
				AND TBL_EWB_GENERATION.ewayBillNo IS NULL
				Order By ewbid desc			
				FOR JSON AUTO
		End

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End