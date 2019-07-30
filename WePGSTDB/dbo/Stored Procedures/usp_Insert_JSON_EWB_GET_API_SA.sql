
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the Get Eway bill details in SA Tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
05/14/2018	Karthik	Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_EWB_GET_API_SA '05AAACC4214B1ZK', 'ERP','WEP001','{
  "ewbNo": 321001061411,
  "ewayBillDate": "13/05/2018 03:56:00 PM",
  "genMode": "API",
  "userGstin": "05AAACC4214B1ZK",
  "supplyType": "O",
  "subSupplyType": "1  ",
  "docType": "INV",
  "docNo": "raj123-1004",
  "docDate": "13/05/2018",
  "fromGstin": "05AAACC4214B1ZK",
  "fromTrdName": "welton",
  "fromAddr1": "2ND CROSS NO 59  19  A",
  "fromAddr2": "GROUND FLOOR OSBORNE ROAD",
  "fromPlace": "FRAZER TOWN",
  "fromPincode": 560042,
  "fromStateCode": 5,
  "toGstin": "05AAACC4309B1ZG",
  "toTrdName": "sthuthya",
  "toAddr1": "Shree Nilaya",
  "toAddr2": "Dasarahosahalli",
  "toPlace": "Beml Nagar",
  "toPincode": 689788,
  "toStateCode": 5,
  "totalValue": 5609889,
  "totInvValue": 5778185.7,
  "cgstValue": 84148.35,
  "sgstValue": 84148.35,
  "igstValue": 0,
  "cessValue": 224395.56,
  "transporterId": "05AAACC4421G1Z9",
  "transporterName": "Blue Dart",
  "status": "ACT",
  "actualDist": 656,
  "noValidDays": 7,
  "validUpto": "20/05/2018 11:59:00 PM",
  "extendedTimes": 0,
  "rejectStatus": "N",
  "vehicleType": "R",
  "actFromStateCode": 5,
  "actToStateCode": 5,
  "itemList": [
    {
      "itemNo": 1,
      "productId": 0,
      "productName": "Wheat",
      "productDesc": "Wheat",
      "hsnCode": 1001,
      "quantity": 4,
      "qtyUnit": "BOX",
      "cgstRate": 1.5,
      "sgstRate": 1.5,
      "igstRate": 0,
      "cessRate": 4,
      "cessAdvol": 0,
      "taxableAmount": 5609889
    }
  ],
  "VehiclListDetails": [
    {
      "updMode": "API",
      "vehicleNo": "ap09za1234",
      "fromPlace": "Banglore",
      "fromState": 29,
      "tripshtNo": 3210006596,
      "userGSTINTransin": "05AAACC4214B1ZK",
      "enteredDate": "13/05/2018 05:07:00 PM",
      "transMode": "1  ",
      "transDocNo": "TRANS7688",
      "transDocDate": "12/05/2018"
    },
    {
      "updMode": "API",
      "vehicleNo": "PVC1234",
      "fromPlace": "FRAZER TOWN",
      "fromState": 5,
      "tripshtNo": 0,
      "userGSTINTransin": "05AAACC4214B1ZK",
      "enteredDate": "13/05/2018 03:56:00 PM",
      "transMode": "1  ",
      "transDocNo": "3002",
      "transDocDate": "13/05/2018"
    }
  ]
}',1,1


 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_EWB_GET_API_SA]
	@userGSTIN varchar(15),
	@RecordContents nvarchar(max),
	@CreatedBy int,
	@CustId int,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@JsonResult nvarchar(max) = NULL OUT,
	@ErrorRecords nvarchar(max) = NULL out,
	@DocNo nvarchar(50) = NULL out,
	@DocDate nvarchar(50) = NULL out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @TotalRecordsCount int,@ProcessedRecordsCount int,@ErrorRecordsCount int

	Select	space(50) as ewbid,
			space(50) as itmsid, 
			space(50) as vehicleid,
			ewbNo as ewbNo,
			ewayBillDate as ewayBillDate,
			genMode as genMode,
			userGstin as userGstin,
			supplyType as supplyType,
			subSupplyType as subSupplyType,
			docType as docType,
			docNo as docNo,
			docDate as docDate,
			fromGstin as fromGstin,
			fromTrdName as fromTrdName,
			fromAddr1 as fromAddr1,
			fromAddr2 as fromAddr2,
			fromPlace as fromPlace,
			fromPincode as fromPincode,
			fromStateCode as fromStateCode,
			toGstin as toGstin,
			toTrdName as toTrdName,
			toAddr1 as toAddr1,
			toAddr2 as toAddr2,
			toPlace as toPlace,
			toPincode as toPincode,
			toStateCode as toStateCode,
			totalValue as totalValue,
			totinvvalue as totinvvalue,
			cgstValue as cgstValue,
			sgstValue as sgstValue,
			igstValue as igstValue,
			cessValue as cessValue,
			transporterId as transporterId,
			transporterName as transporterName,
			status as status,
			actualDist as actualDist,
			noValidDays as noValidDays,
			validUpto as validUpto,
			extendedTimes as extendedTimes,
			rejectStatus as rejectStatus,
			vehicleType as vehicleType,
			actFromStateCode as actFromStateCode,
			actToStateCode as actToStateCode,
			itemList as itemList,
			itemNo as itemNo,
			productId as productId,
			productName as productName,
			productDesc as productDesc,
			hsnCode as hsnCode,
			quantity as quantity,
			qtyUnit as qtyUnit,
			cgstRate as cgstRate,
			sgstRate as sgstRate,
			igstRate as igstRate,
			cessRate as cessRate,
			cessAdvol as cessAdvol,
			taxableAmount as taxableAmount,
			VehiclListDetails as VehiclListDetails,
			updMode as updMode,
			vehicleNo as vehicleNo,
			VfromPlace as VfromPlace,
			fromState as fromState,
			tripshtNo as tripshtNo,
			userGSTINTransin as userGSTINTransin,
			enteredDate as enteredDate,
			transMode as transMode,
			transDocNo as transDocNo,
			transDocDate as transDocDate,
			space(255) as errormessage,
			space(10) as errorcode 
			Into #TBL_EXT_EWB_GET_API_SA
	From OPENJSON(@RecordContents) 	
	WITH
	(
		ewbNo varchar(50),
		ewayBillDate varchar(50),
		genMode varchar(50),
		userGstin varchar(50),
		supplyType nvarchar(15),
		subSupplyType nvarchar(15),
		docType nvarchar(15),
		docNo nvarchar(50),
		docDate nvarchar(15),
		fromGstin nvarchar(15),
		fromTrdName nvarchar(100),
		fromAddr1 nvarchar(max),
		fromAddr2 nvarchar(max),
		fromPlace nvarchar(50),
		fromPincode int,
		fromStateCode int,
		toGstin nvarchar(15),
		toTrdName nvarchar(100),
		toAddr1 nvarchar(max),
		toAddr2 nvarchar(max),
		toPlace nvarchar(100),
		toPincode int,
		toStateCode int,
		totalValue decimal(18,2),
		totinvvalue decimal(18,2),
		cgstValue decimal(18,2),
		sgstValue decimal(18,2),
		igstValue decimal(18,2),
		cessValue decimal(18,2),
		transporterId nvarchar(15),
		transporterName nvarchar(100),
		status varchar(10),
		actualDist varchar(50),
		noValidDays int,
		validUpto varchar(50),
		extendedTimes int,
		rejectStatus varchar(10),
		vehicleType varchar(10),
		actFromStateCode int,
		actToStateCode int,
		itemList nvarchar(max) as JSON,
		VehiclListDetails nvarchar(max) as JSON			
	) as ewaybill
	Cross Apply OPENJSON(ewaybill.itemList) 
	WITH
	(
		itemNo int,
		productId int,
		productName nvarchar(100),
		productDesc nvarchar(max),
		hsnCode nvarchar(50),
		quantity decimal(18,2),
		qtyUnit nvarchar(10),
		cgstRate decimal(18,2),
		sgstRate decimal(18,2),
		igstRate decimal(18,2),
		cessRate decimal(18,2),
		cessAdvol decimal(18,2),
		taxableAmount decimal(18,2)
	) As ItemList
	Cross Apply OPENJSON(ewaybill.VehiclListDetails) 
	WITH
	(
		updMode varchar(50),
		vehicleNo nvarchar(20),
		VfromPlace nvarchar(50),
		fromState nvarchar(2),
		tripshtNo nvarchar(50),
		userGSTINTransin nvarchar(50),
		enteredDate varchar(50),
		transMode int,
		transDocNo nvarchar(15),
		transDocDate nvarchar(10)
	) As VehiclListDetails

		--Declare @CreatedBy int,@CustId int,@RoleId int,@Email nvarchar(250)
		--Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
		--Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1

	Select * from #TBL_EXT_EWB_GET_API_SA	

	If exists (Select 1 From #TBL_EXT_EWB_GET_API_SA t1
				Where Exists(Select 1 From TBL_EWB_GENERATION t2 Where t2.ewayBillNo = t1.ewbNo and t2.Custid = @CustId and t2.flag = ''))
			Begin

				Update TBL_EWB_GENERATION
				SET TBL_EWB_GENERATION.genMode = t2.genMode,
					TBL_EWB_GENERATION.Status = t2.status,  
					TBL_EWB_GENERATION.validUpto = t2.validUpto,
					TBL_EWB_GENERATION.extendedTimes = t2.extendedTimes,
					TBL_EWB_GENERATION.rejectStatus = t2.rejectStatus
				FROM TBL_EWB_GENERATION t1,
						#TBL_EXT_EWB_GET_API_SA t2 
				WHERE t1.ewayBillNo = t1.ewbNo
				And t1.Custid = @CustId
				And t1.flag <> 'O'

				Update #TBL_EXT_EWB_GET_API_SA
				SET #TBL_EXT_EWB_GET_API_SA.ewbid = t2.ewbid 
				FROM #TBL_EXT_EWB_GET_API_SA t1,
						TBL_EWB_GENERATION t2 
				WHERE t1.ewbNo = t2.ewayBillNo
				And t1.Custid = @CustId
				And t1.flag <> 'O'

				Update #TBL_EXT_EWB_GET_API_SA
				SET #TBL_EXT_EWB_GET_API_SA.itmsid = t2.itmsid 
				FROM #TBL_EXT_EWB_GET_API_SA t1,
						TBL_EWB_GENERATION_ITMS t2 
				WHERE t2.ewbid = t1.ewbid

				Update TBL_EWB_GENERATION_ITMS
				SET TBL_EWB_GENERATION_ITMS.productid = t2.productid,
					TBL_EWB_GENERATION_ITMS.itemNo = t2.itemNo  
				FROM TBL_EWB_GENERATION_ITMS t1,
						#TBL_EXT_EWB_GET_API_SA t2 
				WHERE t1.itmsid = t1.itmsid
				
				Update TBL_EWB_UPDATE_VEHICLENO
				SET TBL_EWB_UPDATE_VEHICLENO.updMode = t2.updMode ,
					TBL_EWB_UPDATE_VEHICLENO.tripshtNo = t2.tripshtNo
				FROM TBL_EWB_UPDATE_VEHICLENO t1,
						#TBL_EXT_EWB_GET_API_SA t2 
				WHERE t2.ewayBillNo = t1.ewbNo
				And t1.Custid = @CustId

				
			End

	If not exists (Select 1 From #TBL_EXT_EWB_GET_API_SA t1 
					Where Exists(Select 1 From TBL_EWB_GENERATION t2 
					Where t2.ewayBillNo = t1.ewbNo and t2.Custid = @CustId 
					and (isnull(t2.flag,'') ='' or t2.flag ='G')))
			Begin
			Insert TBL_EWB_GENERATION (ewayBillNo,ewayBillDate,genMode,userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
									fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, 
									toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, 
									totalValue,totinvvalue,  cgstValue, sgstValue,igstValue, cessValue, transporterId,
									 transporterName,status,transDistance,noValidDays,validUpto,extendedTimes,rejectStatus,
									 vehicleType,actFromStateCode,actToStateCode,flag,Custid, CreatedBy, CreatedDate)

				Select	ewbNo,ewayBillDate,genMode,userGstin,supplyType,subSupplyType,docType,docNo,docDate,
									fromGstin,fromTrdName,fromAddr1,fromAddr2,fromPlace,fromPincode,fromStateCode,
									toGstin,toTrdName,toAddr1,toAddr2,toPlace,toPincode,toStateCode,
									totalValue,totinvvalue,cgstValue,sgstValue,igstValue,cessValue,transporterId,
									transporterName,status,actualDist,noValidDays,validUpto,extendedTimes,rejectStatus,
									vehicleType,actFromStateCode,actToStateCode,'G', isnull(@CustId,0), isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE())
				From #TBL_EXT_EWB_GET_API_SA t1
				Where Not Exists(Select 1 From TBL_EWB_GENERATION t2 Where t2.ewayBillNo = t1.ewbNo and t2.docDate = t1.docDate) -- and t2.status in ('1','ACT'))

				Update #TBL_EXT_EWB_GET_API_SA
				SET #TBL_EXT_EWB_GET_API_SA.ewbid = t2.ewbid 
				FROM #TBL_EXT_EWB_GET_API_SA t1,
						TBL_EWB_GENERATION t2 
				WHERE t1.docNo = t2.docNo 
				And t1.docDate = t2.docDate 
				And t1.ewbNo = t2.ewayBillNo
				And t2.flag ='G'

				Insert TBL_EWB_GENERATION_ITMS( ewbid,itemNo,productid, productName, productDesc, hsnCode, quantity, qtyUnit, 
												 cgstRate, sgstRate, igstRate, cessRate, cessAdvol,taxableAmount, createdby, createddate, CustId) 
										Select	ewbid,itemNo,productId,productName,productDesc,hsnCode,quantity,qtyUnit,
												cgstRate,sgstRate,igstRate,cessRate,cessAdvol,taxableAmount, isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE()), isnull(@CustId,0)
				From #TBL_EXT_EWB_GET_API_SA t1
				Where Not Exists ( SELECT 1 FROM TBL_EWB_GENERATION_ITMS t2
								   Where t2.ewbid = t1.ewbid
								   And t2.itemNo = t1.itemNo
								   And t2.productid=t1.productid
								   And t2.productName = t1.productName
								   And t2.hsnCode = t1.hsnCode)


				Update #TBL_EXT_EWB_GET_API_SA
				SET #TBL_EXT_EWB_GET_API_SA.itmsid = t2.itmsid 
				FROM #TBL_EXT_EWB_GET_API_SA t1,
						TBL_EWB_GENERATION_ITMS t2 
				WHERE t2.ewbid = t1.ewbid
				And t2.itemNo = t1.itemNo
				And t2.productid=t1.productid
				And t2.productName = t1.productName
				And t2.hsnCode = t1.hsnCode

				 

				Insert TBL_EWB_UPDATE_VEHICLENO (updMode,vehicleNo,FromPlace,fromState,tripshtNo,
				userGSTIN,enteredDate,transMode,transDocNo,
				transDocDate,ewayBillNo,ewayBillDate,createdby, createddate, CustId)
				 
				 

				Select updMode,vehicleNo,VfromPlace,fromState,tripshtNo,
				userGSTINTransin,enteredDate,transMode,transDocNo,
				transDocDate,ewbNo,ewayBillDate,isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE()), isnull(@CustId,0)
				From #TBL_EXT_EWB_GET_API_SA t1
				Where Not Exists ( SELECT 1 FROM TBL_EWB_UPDATE_VEHICLENO t2
								   Where t2.ewayBillNo = t1.ewbNo
								   And t2.ewayBillDate = t1.ewayBillDate
								   And t2.vehicleNo=t1.vehicleNo
								   And t2.transDocNo = t1.transDocNo
								   And t2.transDocDate = t1.transDocDate)


				
				Update #TBL_EXT_EWB_GET_API_SA
				SET #TBL_EXT_EWB_GET_API_SA.vehicleid = t2.ewbVehUpdid 
				FROM #TBL_EXT_EWB_GET_API_SA t1,
						TBL_EWB_UPDATE_VEHICLENO t2 
				WHERE t2.ewayBillNo = t1.ewbNo
					And t2.ewayBillDate = t1.ewayBillDate
					And t2.vehicleNo=t1.vehicleNo
					And t2.transDocNo = t1.transDocNo
					And t2.transDocDate = t1.transDocDate

		End
		
			Select * from #TBL_EXT_EWB_GET_API_SA

End