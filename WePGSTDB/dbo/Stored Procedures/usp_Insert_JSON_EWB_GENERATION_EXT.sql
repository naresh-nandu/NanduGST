

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
01/02/2018	Raja	Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_EWB_GENERATION_EXT '05AAACC4214B1ZK','ERP','WEP001','Madiwala','{
"ewbValue": 
{
"supplyType": "O",
"subSupplyType": "1",
"docType": "INV",
"docNo": "erwr23434",
"docDate": "23/04/2018",
"fromGstin": "05AAACC4214B1ZK",
"fromTrdName": "Microtek International",
"fromAddr1": "Pvt. Ltd.",
"fromAddr2": "",
"fromPlace": "Delhi",
"fromPincode": 110006,
"fromStateCode": 22,
"actFromStateCode": 22,
"toGstin": "05AAACC4309B1ZG",
"toTrdName": "Saatvik Green Energy Pvt Ltd.",
"toAddr1": "",
"toAddr2": "Village Dubli, Tehsil Barara",
"toPlace": "Ambala, Haryana",
"toPincode": 133101,
"toStateCode": 6,
"actToStateCode": 30,
"totalValue": 30850.5,
"cgstValue": 0,
"sgstValue": 0,
"igstValue": 1542.52,
"cessValue": 0,
"totinvvalue": 30850.5,
"transporterId": "",
"transporterName": "",
"transDocNo": "",
"transMode": "",
"transDistance": "650",
"transDocDate": "",
"vehicleNo": "",
"vehicleType": "",
"itemList": [{
"productName": "solar",
"productDesc": "Solar Panel 75W",
"hsnCode": 85414011,
"quantity": 6,
"qtyUnit": "BOX",
"cgstRate": 0,
"sgstRate": 0,
"igstRate": 5,
"cessRate": 0,
"cessAdvol": 0,
"taxableAmount": 14850.24
}, {
"productName": "solar",
"productDesc": "Solar Panel 50W",
"hsnCode": 85414011,
"quantity": 10,
"qtyUnit": "BOX",
"cgstRate": 0,
"sgstRate": 0,
"igstRate": 5,
"cessRate": 0,
"cessAdvol": 0,
"taxableAmount": 16000.26
}
]
}
}'

 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_EWB_GENERATION_EXT]
	@userGSTIN varchar(15),
	@SourceType varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@Location varchar(50),
	@RecordContents nvarchar(max),
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

	Select	@SourceType as Sourcetype,
			@ReferenceNo as ReferenceNo,
			space(50) as usergstin,
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
			actFromStateCode as actFromStateCode,
			toGstin as toGstin,
			toTrdName as toTrdName,
			toAddr1 as toAddr1,
			toAddr2 as toAddr2,
			toPlace as toPlace,
			toPincode as toPincode,
			toStateCode as toStateCode,
			actToStateCode as actToStateCode,
			totalValue as totalValue,
			cgstValue as cgstValue,
			sgstValue as sgstValue,
			igstValue as igstValue,
			cessValue as cessValue,
			totinvvalue as totinvvalue,
			transMode as transMode,
			transDistance as transDistance,
			transporterId as transporterId,
			transporterName as transporterName,
			transDocNo as transDocNo,
			transDocDate as transDocDate,
			vehicleNo as vehicleNo,
			vehicleType as vehicleType,
			itemList as itemList,
			productName as productName,
			productDesc as productDesc,
			hsnCode as hsnCode,
			quantity as quantity,
			qtyUnit as qtyUnit,
			taxableAmount as taxableAmount,
			cgstRate as cgstRate,
			sgstRate as sgstRate,
			igstRate as igstRate,
			cessRate as cessRate,
			cessAdvol as cessAdvol,
			space(50) as gstinid,
			space(50) as branch,
			space(50) as InvoiceType,
			Space(50) as AllowDuplication,
			space(255) as errormessage,
			space(10) as errorcode 
			Into #TBL_EXT_EWB_GENERATION
	From OPENJSON(@RecordContents) 	
	WITH
	(
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
		actFromStateCode int,
		toGstin nvarchar(15),
		toTrdName nvarchar(100),
		toAddr1 nvarchar(max),
		toAddr2 nvarchar(max),
		toPlace nvarchar(100),
		toPincode int,
		toStateCode int,
		actToStateCode int,
		totalValue decimal(18,2),
		cgstValue decimal(18,2),
		sgstValue decimal(18,2),
		igstValue decimal(18,2),
		cessValue decimal(18,2),
		totinvvalue decimal(18,2),
		transMode int,
		transDistance nvarchar(15),
		transporterId nvarchar(15),
		transporterName nvarchar(100),
		transDocNo nvarchar(15),
		transDocDate nvarchar(10),
		vehicleNo nvarchar(20),
		vehicleType nvarchar(10),
		itemList nvarchar(max) as JSON		
	) as ewaybill
	Cross Apply OPENJSON(ewaybill.itemList) 
	WITH
	(
		productName nvarchar(100),
		productDesc nvarchar(max),
		hsnCode nvarchar(50),
		quantity decimal(18,2),
		qtyUnit nvarchar(10),
		taxableAmount decimal(18,2),
		cgstRate decimal(18,2),
		sgstRate decimal(18,2),
		igstRate decimal(18,2),
		cessRate decimal(18,2),
		cessAdvol decimal(18,2)
	) As ItemList

	Select * from #TBL_EXT_EWB_GENERATION

	Declare @CreatedBy int, @CustId int, @RoleId int, @Email nvarchar(250), @LocationId int, @GstinId int, @Pan varchar(10), @PanId int, @BranchId int
	Select @CustId = Custid, @Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
	Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1
	Select @GstinId = GStinid, @Pan= Panno from tbl_Cust_gstin where Custid = @CustId and Gstinno = @userGSTIN  and rowstatus = 1
	Select @PanId = PanId from tbl_Cust_Pan where Custid = @CustId and Panno = @Pan  and rowstatus =1
	
	if not exists (Select 1 from tbl_cust_Location where UPPER(branch) = (UPPER(@Location)) and Custid = @CustId and Gstinid = @GstinId And PanId = @PanId and rowstatus = 1)
	Begin
		Insert into tbl_cust_Location (PanId, Gstinid, Branch, emailId, Custid, CreatedBy, Createddate, rowstatus)
		Select @PanId, @GstinId, UPPER(@Location), @Email, @CustId, @CreatedBy, Getdate(), 1
	End
	Select @BranchId = BranchId from tbl_cust_Location where UPPER(branch) = (UPPER(@Location)) and Custid = @CustId and Gstinid = @GstinId And PanId = @PanId and rowstatus = 1

	If Not exists(Select 1 from TBL_LocationAccess_Users where userid = @CreatedBy and BranchId = @BranchId and Custid = @CustId and rowstatus = 1)
	Begin
		Insert into TBL_LocationAccess_Users(Branchid, UserId, Custid, CreatedBy, Createddate, rowstatus)
		Select @BranchId, @CreatedBy, @CustId, @CreatedBy, Getdate(), 1
	End

	Select @TotalRecordsCount = count(*) from #TBL_EXT_EWB_GENERATION

		Update #TBL_EXT_EWB_GENERATION Set totalvalue = Ltrim(Rtrim(IsNull(totalvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set igstvalue = Ltrim(Rtrim(IsNull(igstvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set cgstvalue = Ltrim(Rtrim(IsNull(cgstvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set sgstvalue = Ltrim(Rtrim(IsNull(sgstvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set cessvalue = Ltrim(Rtrim(IsNull(cessvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set totinvvalue = Ltrim(Rtrim(IsNull(totinvvalue,'')))
		Update #TBL_EXT_EWB_GENERATION Set quantity = Ltrim(Rtrim(IsNull(quantity,'')))
		Update #TBL_EXT_EWB_GENERATION Set taxableamount = Ltrim(Rtrim(IsNull(taxableamount,'')))
		Update #TBL_EXT_EWB_GENERATION Set igstrate = Ltrim(Rtrim(IsNull(igstrate,'')))
		Update #TBL_EXT_EWB_GENERATION Set cgstrate = Ltrim(Rtrim(IsNull(cgstrate,'')))
		Update #TBL_EXT_EWB_GENERATION Set sgstrate = Ltrim(Rtrim(IsNull(sgstrate,'')))
		Update #TBL_EXT_EWB_GENERATION Set cessrate = Ltrim(Rtrim(IsNull(cessrate,'')))
		Update #TBL_EXT_EWB_GENERATION Set docNo = Ltrim(Rtrim(IsNull(docNo,'')))
		Update #TBL_EXT_EWB_GENERATION Set transDistance = Ltrim(Rtrim(IsNull(transDistance,'')))
		
		Declare @LocationReq int
		select @LocationReq=LocationReqd from tbl_cust_settings where custid = @Custid and rowstatus =1
			If @LocationReq <> 1
				Begin
					Set @LocationReq = 0
				End
			Else
				Begin
					Set @LocationReq = 1
				End

		-- Supply Type Validation
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -1,
				ErrorMessage = 'Supply Type is mandatory'
			Where Ltrim(Rtrim(IsNull(supplyType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_EXT_EWB_GENERATION 
			Set SupplyType =
			Case SupplyType
			When  'Inward'  Then  'I'
			When  'Outward'  Then  'O'
			else SupplyType
			End
			where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -2,
				ErrorMessage = 'Invalid Supply Type'
			Where SupplyType not in ('O','I')
			And IsNull(ErrorCode,0) = 0
			
		-- Sub Supply Type Validation
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -3,
				ErrorMessage = 'Sub Supply Type is mandatory'
			Where Ltrim(Rtrim(IsNull(subSupplyType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
		
		Update #TBL_EXT_EWB_GENERATION 
			Set subsupplytype =
				Case subsupplytype
				When  'Supply'  Then  '1'--o,I
				When  'Import'  Then  '2'--I
				When  'Export'  Then  '3'--o
				When  'Job Work'  Then  '4'--o
				When  'For Own Use'  Then  '5'--o,I
				When  'Job work Returns'  Then  '6'--I
				When  'Sales Return'  Then  '7'--I
				When  'Others'  Then  '8'--o,I
				When  'SKD/CKD'  Then  '9'--o,I
				When  'Line Sales'  Then  '10'--o
				When  'Recipient Not Known'  Then  '11'--o
				When  'Exhibition or Fairs'  Then  '12'--o,I
				else subsupplytype
				End
				where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -4,
				ErrorMessage = 'Invalid Outward Sub Supply Type'
			Where subsupplytype not in ('1','3','4','5','8','9','10','11','12')
			And SupplyType in ('O')
			And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -5,
				ErrorMessage = 'Invalid Inward Sub Supply Type'
			Where subsupplytype not in ('1','2','5','6','7','8','9','12')
			And SupplyType in ('I')
			And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION 
			Set usergstin = fromgstin 
			from #TBL_EXT_EWB_GENERATION 
			Where supplytype = 'O' 
			And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION 
			Set usergstin =  togstin 
			from #TBL_EXT_EWB_GENERATION 
			Where supplytype = 'I' 
			And IsNull(ErrorCode,0) = 0
		
		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -6,
				ErrorMessage = 'From Gstin is Not Applicable'
			Where subsupplytype in ('2')
			And SupplyType in ('I')
			And fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -7,
				ErrorMessage = 'To Gstin is Not Applicable'
			Where subsupplytype in ('3')
			And SupplyType in ('O')
			And togstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		-- Document Type Validation	
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -8,
				ErrorMessage = 'DocType is mandatory'
			Where Ltrim(Rtrim(IsNull(docType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
		
		Update #TBL_EXT_EWB_GENERATION 
			Set docType =
			Case docType
				When  'Bill of Entry'  Then  'BOE'
				When  'Bill of Supply'  Then  'BOL'
				When  'Challan'  Then  'CHL'
				When  'Credit Note'  Then  'CNT'
				When  'Tax Invoice'  Then  'INV'
				When  'Others'  Then  'OTH'
				else docType
				End
			where IsNull(ErrorCode,0) = 0
		
		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -9,
				ErrorMessage = 'Invalid Document Type'
			Where docType not in ('INV','BOL','BOE','CHL','CNT','OTH')
			And IsNull(ErrorCode,0) = 0

		-- Document No Validation	
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -10,
				ErrorMessage = 'DocNo is mandatory'
			Where Ltrim(Rtrim(IsNull(docNo,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -11,
				ErrorMessage = 'Invalid Document No'
			Where dbo.udf_ValidateInvoiceNo(docNo) <> 1
			And IsNull(ErrorCode,0) = 0

		-- Document Date Validation
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -12,
				ErrorMessage = 'DocDate is mandatory'
			Where Ltrim(Rtrim(IsNull(docDate,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
					
		-- From GSTIN Validation
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -13,
				ErrorMessage = 'From Gstin is mandatory'
			Where Ltrim(Rtrim(IsNull(fromGstin,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
			
		Update #TBL_EXT_EWB_GENERATION
			Set ErrorCode = -14,
				ErrorMessage = 'Invalid From Gstin'
			Where dbo.udf_ValidateGstin(fromgstin) <> 1 
			And subsupplytype not in ('2')
			And fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		-- From Pin Code Validation
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -15,
				ErrorMessage = 'From Pin Code is mandatory'
			Where   Ltrim(Rtrim(IsNull(fromPincode,0))) = 0
			And subsupplytype not in ('2') and fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_EXT_EWB_GENERATION 
			Set  ErrorCode = -16,
				ErrorMessage = 'Invalid From PinCode'
			Where subsupplytype in ('2') and fromgstin = 'URP'
			And fromPincode <> '999999'
			And IsNull(ErrorCode,0) = 0 

		-- From State Code Validation	
		Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -17,
				ErrorMessage = 'From State Code is mandatory'
			Where Ltrim(Rtrim(IsNull(fromStateCode,''))) = '' 
			And subsupplytype not in ('2') and fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0 
					
		Update #TBL_EXT_EWB_GENERATION 
			Set fromstatecode =
				Case fromstatecode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else fromstatecode
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -18,
						ErrorMessage = 'Invalid From State Code'
				Where subsupplytype in ('2') and fromgstin = 'URP'
				And (fromStateCode <> '00' or fromStateCode <> '0')
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -19,
					ErrorMessage = 'Invalid From State Code'
				Where fromstatecode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0	
				
			Update #TBL_EXT_EWB_GENERATION 
				Set fromstatecode = '0' + Ltrim(Rtrim(IsNull(fromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(fromstatecode,'')))) = 1	

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -20,
					ErrorMessage = 'Invalid From State Code'
				Where dbo.udf_ValidatePlaceOfSupply(fromstatecode) <> 1
				And IsNull(ErrorCode,0) = 0

		-- To GSTIN Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -21,
					ErrorMessage = 'To Gstin is mandatory'
				Where Ltrim(Rtrim(IsNull(toGstin,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -22,
					ErrorMessage = 'Invalid To Gstin'
				Where dbo.udf_ValidateGstin(togstin) <> 1
				And subsupplytype not in ('3') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0
			
		-- To PinCode Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -23,
					ErrorMessage = 'To Pin Code is mandatory'
				Where   Ltrim(Rtrim(IsNull(toPincode,0))) = 0
				And subsupplytype not in ('3') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -24,
					ErrorMessage = 'Invalid To Pin Code'
				Where subsupplytype in ('3') and togstin = 'URP'
				And toPincode <> '999999'
				And IsNull(ErrorCode,0) = 0 
		
		-- To State Code Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -25,
					ErrorMessage = 'To State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(toStateCode,''))) = ''  
				And subsupplytype not in ('2') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_EXT_EWB_GENERATION 
			Set toStateCode =
				Case toStateCode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else toStateCode
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -26,
						ErrorMessage = 'Invalid To State Code'
				Where subsupplytype in ('3') and togstin = 'URP'
				And (toStateCode <> '00' or toStateCode <> '0')
				And IsNull(ErrorCode,0) = 0 
				
			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -27,
					ErrorMessage = 'Invalid To State Code'
				Where toStateCode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0			
				
			Update #TBL_EXT_EWB_GENERATION 
				Set toStateCode = '0' + Ltrim(Rtrim(IsNull(toStateCode,'')))
				Where Len(Ltrim(Rtrim(IsNull(toStateCode,'')))) = 1	

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -28,
					ErrorMessage = 'Invalid To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(toStateCode) <> 1
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -29,
					ErrorMessage = 'From Gstin is not registered'
					Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = @custid
						And t1.GstinNo = fromgstin
						And t1.rowstatus = 1) 
					And (isnull(fromgstin,'') <> '' or isnull(fromgstin,'') <> 'URP')
					And SupplyType in ('O') 
					And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -30,
					ErrorMessage = 'TO Gstin is not registered'
					Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = @custid
						And t1.GstinNo = togstin
						And t1.rowstatus = 1) 
					And (isnull(togstin,'') <> '' or isnull(togstin,'') <> 'URP')
					And SupplyType in ('I')
					And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -31,
					ErrorMessage = 'User Gstin is not registered'
					Where Not Exists(Select 1 From Tbl_Cust_Gstin t1
						Where t1.custid = @custid And t1.GstinNo = usergstin And t1.rowstatus = 1) 
					And (isnull(usergstin,'') <> '' or isnull(usergstin,'') <> 'URP')
					And IsNull(ErrorCode,0) = 0

			-- Actual From State Code Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -32,
					ErrorMessage = 'Actual From State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(actfromstatecode,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
			Set actfromstatecode =
				Case actfromstatecode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else actfromstatecode
				End
				where IsNull(ErrorCode,0) = 0
									
			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -33,
					ErrorMessage = 'Invalid Actual From State Code'
				Where actfromstatecode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0			
				
			Update #TBL_EXT_EWB_GENERATION 
				Set actfromstatecode = '0' + Ltrim(Rtrim(IsNull(actfromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(actfromstatecode,'')))) = 1		

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -34,
					ErrorMessage = 'Invalid Actual From State Code'
				Where dbo.udf_ValidatePlaceOfSupply(actfromstatecode) <> 1
				And IsNull(ErrorCode,0) = 0

			-- Actual To State Code Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -35,
					ErrorMessage = 'Actual From State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(acttoStateCode,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
			Set acttoStateCode =
				Case acttoStateCode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else acttoStateCode
				End
				where IsNull(ErrorCode,0) = 0

									
			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -36,
					ErrorMessage = 'Invalid Actual To State Code'
				Where acttoStateCode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0	
				
			Update #TBL_EXT_EWB_GENERATION 
				Set acttoStateCode = '0' + Ltrim(Rtrim(IsNull(acttoStateCode,'')))
				Where Len(Ltrim(Rtrim(IsNull(acttoStateCode,'')))) = 1		

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -37,
					ErrorMessage = 'Invalid Actual To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(acttoStateCode) <> 1
				And IsNull(ErrorCode,0) = 0

			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -38,
					ErrorMessage = 'Total Value is mandatory'
				Where totalValue = ''
				And IsNull(ErrorCode,0) = 0 
				
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -39,
					ErrorMessage = 'Total Value is Not Numeric'
				Where isnumeric(totalValue) <> 1
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -40,
					ErrorMessage = 'Invalid CGST/SGST Rate'
				Where dbo.udf_ValidateRate(try_convert(Decimal(18,2),cgstRate) + try_convert(Decimal(18,2),sgstRate)) <> 1
				And cgstRate <> '' And sgstRate <> ''
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -41,
					ErrorMessage = 'Invalid IGST Rate'
				Where dbo.udf_ValidateRate(igstRate) <> 1
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -42,
					ErrorMessage = 'Invalid CESS Rate'
				Where cessrate <> '' and isnumeric(try_convert(Decimal(18,2),cessrate)) <> 1
				And IsNull(ErrorCode,0) = 0	
				   
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -43,
					ErrorMessage = 'Tax Amount is mandatory'				
					Where  (
						Convert(Decimal(18,2),cgstRate) > 0
					Or   Convert(Decimal(18,2),sgstRate) > 0
					Or   Convert(Decimal(18,2),igstRate) > 0
					) 
					And   (
						  Convert(Decimal(18,2),cgstValue) = 0
					And   Convert(Decimal(18,2),sgstValue) = 0
					And   Convert(Decimal(18,2),igstValue) = 0
					)
					And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -44,
					ErrorMessage = 'CGST Value is Not Numeric'
				Where isnumeric(cgstValue) <> 1
				And Convert(Decimal(18,2),cgstValue) <> 0
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -45,
					ErrorMessage = 'SGST Value is Not Numeric'
				Where isnumeric(sgstValue) <> 1
				And  Convert(Decimal(18,2),sgstValue) <> 0
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -46,
					ErrorMessage = 'IGST Value is Not Numeric'
				Where isnumeric(igstValue) <> 1
				And  Convert(Decimal(18,2),igstValue) <> 0
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -47,
					ErrorMessage = 'CESS Value is Not Numeric'
				Where IsNumeric(cessValue) <> 1
				And  Convert(Decimal(18,2),cessValue) <> 0
				And IsNull(ErrorCode,0) = 0		
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -48,
					ErrorMessage = 'Taxable Amount is mandatory'
				Where  taxableAmount = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -49,
					ErrorMessage = 'Igst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) > 0
				And Convert(Decimal(18,2),cgstRate) = 0
				And Convert(Decimal(18,2),sgstrate) = 0
				And  (igstValue < ((Convert(dec(18,2),igstRate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (igstValue > ((Convert(dec(18,2),igstRate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0	
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -50,
					ErrorMessage = 'Cgst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) = 0
				And Convert(Decimal(18,2),cgstRate) > 0
				And Convert(Decimal(18,2),sgstrate) > 0
				And  (cgstvalue < ((Convert(dec(18,2),cgstRate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (cgstvalue > ((Convert(dec(18,2),cgstRate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -51,
					ErrorMessage = 'Sgst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) = 0
				And Convert(Decimal(18,2),cgstRate) > 0
				And Convert(Decimal(18,2),sgstrate) > 0
				And  (sgstValue < ((Convert(dec(18,2),sgstrate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (sgstValue > ((Convert(dec(18,2),sgstrate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -52,
					ErrorMessage = 'Cess Amount is wrong.'
				Where Convert(Decimal(18,2),cessrate) > 0
				And  (cessvalue < ((Convert(dec(18,2),cessrate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (cessvalue > ((Convert(dec(18,2),cessrate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -53,
					ErrorMessage = 'Transport Distance is mandatory'
				Where transDistance = ''
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -54,
					ErrorMessage = 'Transporter Id is mandatory'
				Where  Ltrim(Rtrim(IsNull( transporterId,''))) = ''
				And IsNull(ErrorCode,0) = 0
			 
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -55,
					ErrorMessage = 'Transporter Name is mandatory'
				Where  Ltrim(Rtrim(IsNull( transporterName,''))) = ''
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -56,
					ErrorMessage = 'Invalid Transporter Date'
				Where dbo.udf_Validate_EWB_DocDate(docdate,transDocDate) <> 1
				And isnull(docdate,'')<>'' 
				And isnull(transDocDate,'')<>''
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -57,
					ErrorMessage = 'HSN Code is mandatory'
				Where  Ltrim(Rtrim(IsNull( hsnCode,''))) = ''
				And IsNull(ErrorCode,0) = 0

			-- TransMode Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set transmode =
				Case convert(varchar(10),transmode)
				When  'Road'  Then  '1'
				When  'Rail'  Then  '2'
				When  'Air'  Then  '3'
				When  'Ship'  Then  '4'
				else transmode
				End
				where Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0

			
			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -58,
					ErrorMessage = 'Invalid Transport mode'
				Where transmode not in ('','1','2','3','4')
				And Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0

			-- Vehicle Type Validation
			Update #TBL_EXT_EWB_GENERATION 
				Set vehicletype =
				Case vehicletype
				When  'Regular'  Then  'R'
				When  'Over dimensional cargo'  Then  'O'
				else vehicletype
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -59,
					ErrorMessage = 'Vehicle No is Mandatory'
				Where isnull(vehicleno,'')='' and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -60,
					ErrorMessage = 'Invalid Vehicle No'
				Where dbo.udf_ValidateVehicleNo(vehicleno) <> 1 and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -61,
					ErrorMessage = 'Invalid Vehicle Type'
				Where vehicletype not in ('','R','O')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set qtyunit =
				Case qtyunit
				When  'BAGS'  Then  'BGS'
				When  'BUNDLES'  Then  'BND'
				When  'BOXES'  Then  'BOX'
				When  'CENTIMETERS'  Then  'CMS'
				When  'DOZENS'  Then  'DZN'
				When  'GRAMS'  Then  'GMS'
				When  'HUNDRED KILOMETERS'  Then  'HKM'
				When  'HUNDRED NUMBERS/UNITS'  Then  'HNO'
				When  'KILOGRAMS'  Then  'KGS'
				When  'KILOMETERS'  Then  'KMS'
				When  'LITRES'  Then  'LTS'
				When  'MILLILITRES'  Then  'MLS'
				When  'METERS'  Then  'MTR'
				When  'METRIC TONNES'  Then  'MTS'
				When  'NUMBERS/UNITS'  Then  'NOS'
				When  'PAIRS'  Then  'PAR'
				When  'QUINTALS'  Then  'QTS'
				When  'THOUSAND NUMBERS/UNITS'  Then  'SNO'
				When  'THOUSAND KILOMETERS'  Then  'TKM'
				When  'THOUSAND LITRES'  Then  'TLT'
				When  'TEN NUMBERS/UNITS'  Then  'TNO'
				When  'TONNES'  Then  'TON'
				else qtyunit
				End
				where IsNull(ErrorCode,0) = 0	
			
			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = 62,
					ErrorMessage = 'Invalid Quantity Unit'
				Where qtyunit not in ('BGS','BND','BOX','CMS','DZN','GMS','HKM','HNO','KGS','KMS','LTS','MLS','MTR','MTS','NOS','PAR','QTS','SNO','TKM','TLT','TNO','TON')
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_EXT_EWB_GENERATION -- Introduced due to Excel Format Issue
				Set fromstatecode = '0' + Ltrim(Rtrim(IsNull(fromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(fromstatecode,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION -- Introduced due to Excel Format Issue
				Set tostatecode = '0' + Ltrim(Rtrim(IsNull(tostatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(tostatecode,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION
				Set ErrorCode = -63,
					ErrorMessage = 'Invalid To State Code'
				Where tostatecode not in ('00','01','02','03','04','05','06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0		

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -64,
					ErrorMessage = 'Invalid To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(tostatecode) <> 1
				And IsNull(ErrorCode,0) = 0	
					
			Update #TBL_EXT_EWB_GENERATION
				Set #TBL_EXT_EWB_GENERATION.gstinid = t2.gstinid
				FROM #TBL_EXT_EWB_GENERATION t1,
						tbl_cust_gstin t2 
				WHERE upper(t1.usergstin) = upper(t2.Gstinno)
				And t2.Custid = @CustId
				And t2.rowstatus =1
				and  IsNull(ErrorCode,0) = 0
		
			if (@LocationReq = 1)
			Begin
			
				Update #TBL_EXT_EWB_GENERATION
					Set #TBL_EXT_EWB_GENERATION.branch = t2.branchid
					FROM #TBL_EXT_EWB_GENERATION t1,
							tbl_Cust_Location t2 
					WHERE upper(ltrim(rtrim(t2.branch))) = upper(ltrim(rtrim(t1.branch)))
					And t2.Custid = @CustId
					And (t2.gstinid = t1.gstinid or isnull(t1.gstinid,0) = 0)
					And t2.rowstatus =1
					and  IsNull(ErrorCode,0) = 0

				Update #TBL_EXT_EWB_GENERATION
					Set ErrorCode = -54,
						ErrorMessage = 'Invalid Branch name'
					Where  isnumeric(ltrim(rtrim(branch))) <> 1
					And IsNull(ErrorCode,0) = 0
			End
			Else
			Begin
				Update #TBL_EXT_EWB_GENERATION
					Set branch = 0
					WHERE  IsNull(ErrorCode,0) = 0
			End

			
			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -65,
					ErrorMessage = 'Invalid Invoice Type'
				Where ltrim(rtrim(InvoiceType)) not in ('Regular','SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -66,
					ErrorMessage = 'Igst Amount is mandatory for Deemed Exports / Supplies to SEZ.'
				Where igstValue ='' And InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0	
			
			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -67,
					ErrorMessage = 'Cgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				Where (cgstValue <> '' And Convert(dec(18,2),cgstValue) > 0) and  InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0	

			Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -68,
					ErrorMessage = 'Sgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				Where (sgstValue <> '' And Convert(dec(18,2),sgstValue) > 0) and  InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -69,
					ErrorMessage = 'Invalid Transporter Id'
				From #TBL_EXT_EWB_GENERATION t1
				Where len(ltrim(rtrim(transporterid))) > 15 
			
			Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -100,
					ErrorMessage = 'Eway Bill data already uploaded'
				From #TBL_EXT_EWB_GENERATION t1
				Where upper(AllowDuplication) = 'YES' 
				And  IsNull(ErrorCode,0) = 0
				And Exists(Select 1 From  TBL_EWB_GENERATION 
								Where userGstin = t1.UserGstin 
								And docNo = t1.DocNo 
								And DocDate = t1.DocDate 
								And isnull(ewaybillNo,'') = ''
								And isnull(ewayBillDate,'') = '')

		    Update #TBL_EXT_EWB_GENERATION 
				Set ErrorCode = -101,
					ErrorMessage = 'Eway Bill data already Generated.'
				From #TBL_EXT_EWB_GENERATION t1
				Where upper(AllowDuplication) = 'NO' 
				And  IsNull(ErrorCode,0) = 0
				And Exists(Select 1 From  TBL_EWB_GENERATION 
								Where userGstin = t1.UserGstin 
								And docNo = t1.DocNo 
								And DocDate = t1.DocDate)



	Begin Try
		Begin
			Insert into TBL_EXT_EWB_GENERATION
				(userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
					fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode,
					toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode,
					totalValue, cgstValue, sgstValue, igstValue, cessValue, totinvvalue, transMode, 
					transDistance, transporterId, transporterName, transDocNo, transDocDate, vehicleNo, vehicleType, productName, productDesc, hsnCode, quantity, 
					qtyUnit, taxableAmount, cgstRate, sgstRate, igstRate, cessRate, cessAdvol, rowstatus, sourcetype, referenceno, createddate, BranchId) 
				Select @userGSTIN, supplyType, subSupplyType, docType, docNo, docDate,
						fromGstin,fromTrdName,fromAddr1,fromAddr2,fromPlace, fromPinCode, fromStateCode, actFromStateCode, 
						toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
						totalValue, IsNull(cgstValue,0), IsNull(sgstValue,0), IsNull(igstValue,0), isnull(cessValue,0), isnull(totinvvalue,0), transMode,
						transDistance,transporterId,transporterName, transDocNo,transDocDate,vehicleNo,vehicleType,productName,productDesc,hsnCode,quantity,
						qtyUnit,taxableAmount,cgstRate,sgstRate,igstRate,cessRate, cessAdvol, 1, @SourceType, @ReferenceNo, DATEADD (mi, 330, GETDATE()), @BranchId 
						from #TBL_EXT_EWB_GENERATION where IsNull(ErrorCode,0) = 0

		

			exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo

			Select @DocNo = (Select TOP 1 docNo From #TBL_EXT_EWB_GENERATION)
			Select @DocDate = (Select TOP 1 docDate From #TBL_EXT_EWB_GENERATION)
								
		End
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_EWB_GENERATION  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Eway Bill Data'
				From #TBL_EXT_EWB_GENERATION t1
				Where IsNull(ErrorCode,0) = 0


					Delete t1
					From	TBL_EXT_EWB_GENERATION t1,
							#TBL_EXT_EWB_GENERATION t2
					Where t1.userGSTIN = @userGSTIN
					And t1.docNo = t2.docNo
					And	t1.docDate = t2.docDate
					And t1.sourcetype = @SourceType
					And t1.referenceno = @ReferenceNo
					And IsNull(ErrorCode,0) = -102

			End				
		End Catch

		

		Select @ProcessedRecordsCount = Count(*)
		From #TBL_EXT_EWB_GENERATION
		Where IsNull(ErrorCode,0) = 0

		Select @ErrorRecordsCount = Count(*)
		From #TBL_EXT_EWB_GENERATION  
		Where IsNull(ErrorCode,0) <> 0

		Select @ErrorRecords = (Select * From #TBL_EXT_EWB_GENERATION
								Where IsNull(ErrorCode,0) <> 0
								FOR JSON AUTO)
	

		Select @JsonResult = (Select @TotalRecordsCount as TotalRecordsCount,@ProcessedRecordscount as ProcessedRecordscount,@ErrorRecordsCount as ErrorRecordsCount, JSON_QUERY((@ErrorRecords)) as ErrorRecords FOR JSON PATH )
							-- RetStatus
			Select @JsonResult	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	-- Push Procedure for EWB EXT TO SA
	--exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo

End