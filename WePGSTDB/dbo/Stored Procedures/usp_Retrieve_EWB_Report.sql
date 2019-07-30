  
/*  
(c) Copyright 2018 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Retrieve the EWAYBill Dashboard PIE  
      
Written by  : nareshn@wepindia.com   

Date			Who				Decription   
06/05/2018		Naresh N		Initial Version 
16/05/2018      Karthik			Included branch concept using # tables for both generation and consolidation 
18/05/2018		RAJA M			@UserId parameter made it as default allow NULL
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Retrieve_EWB_Report '05AAACC5909K1ZP','REJECT',1,1,'ALL','30/05/2018'
  
  
 */  
   
CREATE PROCEDURE [dbo].[usp_Retrieve_EWB_Report]  
 @userGSTIN varchar(15), 
 @Type varchar(20),   
 @CustId int,
 @UserId int = NULL,
 @BranchId varchar(10)=NULL,
 @Period varchar(10)=NULL,  
 @EwbNo varchar(12)=NULL,  
 @CEwbNo varchar(10)=NULL,  
 @genGSTIN varchar(15)=NULL   
  
-- /*mssx*/ With Encryption   
AS  
Begin  

	CREATE TABLE #TBL_EWB_GENERATION
	(
	ewbid int,
	userGSTIN nvarchar(50) NULL,
	supplyType nvarchar(100) NULL,
	subSupplyType nvarchar(100) NULL,
	docType nvarchar(50) NULL,
	docNo nvarchar(50) NULL,
	docDate varchar(30) NULL,
	fromGstin nvarchar(50) NULL,
	fromTrdName nvarchar(100) NULL,
	fromAddr1 nvarchar(500) NULL,
	fromAddr2 nvarchar(500) NULL,
	fromPlace nvarchar(50) NULL,
	fromPinCode int NULL,
	fromStateCode varchar(50) NULL,
	actFromStateCode  varchar(50) NULL,
	toGstin nvarchar(50) NULL,
	toTrdName nvarchar(100) NULL,
	toAddr1 nvarchar(500) NULL,
	toAddr2 nvarchar(500) NULL,
	toPlace nvarchar(50) NULL,
	toPincode int NULL,
	toStateCode  varchar(50) NULL,
	actToStateCode  varchar(50) NULL,
	totalValue decimal(18, 2) NULL,
	igstValue decimal(18, 2) NULL,
	cgstValue decimal(18, 2) NULL,
	sgstValue decimal(18, 2) NULL,
	cessValue decimal(18, 2) NULL,
	totinvvalue decimal(18, 2) NULL,
	transMode  varchar(50) NULL,
	transDistance nvarchar(50) NULL,
	transporterId nvarchar(50) NULL,
	transporterName nvarchar(100) NULL,
	transDocNo nvarchar(50) NULL,
	transDocDate varchar(50) NULL,
	vehicleNo nvarchar(50) NULL,
	vehicleType nvarchar(50) NULL,
	CustId int NULL,
	CreatedBy int NULL,
	CreatedDate datetime NULL,
	ModifiedBy int NULL,
	ModifiedDate datetime NULL,
	flag nvarchar(1) NULL,
	EWB_status int NULL,
	ewayBillNo nvarchar(50) NULL,
	ewayBillDate nvarchar(50) NULL,
	validUpto nvarchar(50) NULL,
	EWB_errorCodes nvarchar(50) NULL,
	EWB_errorDescription nvarchar(max) NULL,
	cancelRsnCode int NULL,
	cancelRmrk nvarchar(50) NULL,
	cancelDate nvarchar(50) NULL,
	CAN_status int NULL,
	CAN_errorCodes nvarchar(50) NULL,
	CAN_errorDescription nvarchar(max) NULL,
	status nvarchar(50) NULL,
	genMode nvarchar(50) NULL,
	extendedTimes int NULL,
	rejectStatus nvarchar(50) NULL,
	ewbRejectedDate nvarchar(50) NULL,
	BranchId int NULL,
	APIBulkFlag varchar(1) NULL,
	noValidDays int NULL,
	BranchName nvarchar(250)
	)

	CREATE TABLE #TBL_EWB_GEN_CONSOLIDATED
	(
	consewbid int ,
	userGSTIN varchar(15) NULL,
	fromPlace nvarchar(50) NULL,
	fromState  nvarchar(50) NULL,
	vehicleNo nvarchar(20) NULL,
	transMode  nvarchar(50) NULL,
	transDocNo nvarchar(15) NULL,
	transDocDate nvarchar(10) NULL,
	CustId int NULL,
	CreatedBy int NULL,
	CreatedDate datetime NULL,
	flag nvarchar(1) NULL,
	CEWB_status nvarchar(1) NULL,
	cEwbNo nvarchar(50) NULL,
	cEWBDate nvarchar(50) NULL,
	CEWB_errorCodes nvarchar(max) NULL,
	CEWB_errorDescription nvarchar(max) NULL,
	BranchId int NULL,
	APIBulkFlag varchar(1) NULL,
	BranchName nvarchar(250)
	)


	Insert into #TBL_EWB_GENERATION(ewbid,userGSTIN,supplyType,subSupplyType,docType,docNo,docDate,fromGstin,fromTrdName,fromAddr1,
	fromAddr2,fromPlace,fromPinCode,fromStateCode,actFromStateCode,toGstin,toTrdName,toAddr1,toAddr2,toPlace,toPincode,toStateCode,
	actToStateCode,totalValue,igstValue,cgstValue,sgstValue,cessValue,totinvvalue,transMode,transDistance,transporterId,
	transporterName,transDocNo,transDocDate,vehicleNo,vehicleType,CustId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,
	flag,EWB_status,ewayBillNo,ewayBillDate,validUpto,EWB_errorCodes,EWB_errorDescription,cancelRsnCode,cancelRmrk,cancelDate,
	CAN_status,CAN_errorCodes,CAN_errorDescription,status,genMode,extendedTimes,rejectStatus,ewbRejectedDate,BranchId,APIBulkFlag,
	noValidDays,BranchName)
	Select ewbid,userGSTIN,supplyType,subSupplyType,docType,docNo,docDate,fromGstin,fromTrdName,fromAddr1,
	fromAddr2,fromPlace,fromPinCode,fromStateCode,actFromStateCode,toGstin,toTrdName,toAddr1,toAddr2,toPlace,toPincode,toStateCode,
	actToStateCode,totalValue,igstValue,cgstValue,sgstValue,cessValue,totinvvalue,transMode,transDistance,transporterId,
	transporterName,transDocNo,transDocDate,vehicleNo,vehicleType,CustId,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,
	flag,EWB_status,ewayBillNo,ewayBillDate,validUpto,EWB_errorCodes,EWB_errorDescription,cancelRsnCode,cancelRmrk,cancelDate,
	CAN_status,CAN_errorCodes,CAN_errorDescription,status,genMode,extendedTimes,rejectStatus,ewbRejectedDate,BranchId,APIBulkFlag,
	noValidDays,NULL
  From TBL_EWB_GENERATION 
			Where (userGSTIN = @userGSTIN or toGstin = @userGSTIN)
			AND ewayBillDate like '%' +@Period +'%' 
			AND CustId= @CustId


	Insert into #TBL_EWB_GEN_CONSOLIDATED(consewbid,userGSTIN,fromPlace,fromState,vehicleNo,transMode,transDocNo,transDocDate,CustId,CreatedBy,CreatedDate,
		flag,CEWB_status,cEwbNo,cEWBDate,CEWB_errorCodes,CEWB_errorDescription,BranchId,APIBulkFlag)
			Select consewbid,userGSTIN,fromPlace,fromState,vehicleNo,transMode,transDocNo,transDocDate,CustId,CreatedBy,CreatedDate,
			flag,CEWB_status,cEwbNo,cEWBDate,CEWB_errorCodes,CEWB_errorDescription,BranchId,APIBulkFlag
		From TBL_EWB_GEN_CONSOLIDATED
			Where userGSTIN = @userGSTIN 
				AND cEWBDate like '%' +@Period +'%'
				AND CustId=@CustId

		--		    Select * From #TBL_EWB_Generation Where flag='G'  
  --AND userGSTIN=@userGSTIN AND ewayBillNo=@EwbNo AND CustId=@CustId  

	Update #TBL_EWB_GENERATION 
	SET #TBL_EWB_GENERATION.BranchName = t2.Branch 
	FROM #TBL_EWB_GENERATION t1,
			TBL_Cust_LOCATION t2 
	WHERE t1.Branchid = t2.Branchid 
	And t2.Rowstatus =1

	Update #TBL_EWB_GEN_CONSOLIDATED 
	SET #TBL_EWB_GEN_CONSOLIDATED.BranchName = t2.Branch 
	FROM #TBL_EWB_GEN_CONSOLIDATED t1,
			TBL_Cust_LOCATION t2 
	WHERE t1.Branchid = t2.Branchid 
	And t2.Rowstatus =1

	

	Update #TBL_EWB_GENERATION 
	Set docType =
	Case docType
		When   'INV'  Then  'Tax Invoice'
		When   'BOL'  Then  'Bill of Supply'
		When   'BOE'  Then  'Bill of Entry'
		When   'CHL'  Then  'Delivery Challan'
		When   'CNT'  Then  'Credit Note'
		When   'OTH'  Then  'Others'
		else docType
		End
		where isnull(docType,'') <>''

		Update #TBL_EWB_GENERATION 
		Set fromstatecode =
			Case fromstatecode
			When   '0'  Then  'OTHER COUNTRIES'
			When   '1'  Then  'JAMMU AND KASHMIR'
			When   '22'  Then  'CHHATTISGARH'
			When   '23'  Then  'MADHYA PRADESH'
			When   '24'  Then  'GUJARAT'
			When   '25'  Then  'DAMAN AND DIU'
			When   '26'  Then  'DADAR AND NAGAR HAVELI'
			When   '27'  Then  'MAHARASTRA'
			When   '37'  Then  'ANDHRA PRADESH'
			When   '29'  Then  'KARNATAKA'
			When   '30'  Then  'GOA'
			When   '31'  Then  'LAKSHADWEEP'
			When   '32'  Then  'KERALA'
			When   '33'  Then  'TAMIL NADU'
			When   '34'  Then  'PONDICHERRY'
			When   '35'  Then  'ANDAMAN AND NICOBAR'
			When   '36'  Then  'TELENGANA'
			When   '2'  Then  'HIMACHAL PRADESH'
			When   '3'  Then  'PUNJAB'
			When   '4'  Then  'CHANDIGARH'
			When   '5'  Then  'UTTARAKHAND'
			When   '6'  Then  'HARYANA'
			When   '7'  Then  'DELHI'
			When   '8'  Then  'RAJASTHAN'
			When   '9'  Then  'UTTAR PRADESH'
			When   '10'  Then  'BIHAR'
			When   '11'  Then  'SIKKIM'
			When   '12'  Then  'ARUNACHAL PRADESH'
			When   '13'  Then  'NAGALAND'
			When   '14'  Then  'MANIPUR'
			When   '15'  Then  'MIZORAM'
			When   '16'  Then  'TRIPURA'
			When   '17'  Then  'MEGHALAYA'
			When   '18'  Then  'ASSAM'
			When   '19'  Then  'WEST BENGAL'
			When   '20'  Then  'JHARKHAND'
			When   '21'  Then  'ORISSA'

			else fromstatecode
			End
			where IsNull(fromstatecode,'') <> ''

			Update #TBL_EWB_GEN_CONSOLIDATED 
		Set fromstate =
			Case fromstate
			When   '0'  Then  'OTHER COUNTRIES'
			When   '1'  Then  'JAMMU AND KASHMIR'
			When   '22'  Then  'CHHATTISGARH'
			When   '23'  Then  'MADHYA PRADESH'
			When   '24'  Then  'GUJARAT'
			When   '25'  Then  'DAMAN AND DIU'
			When   '26'  Then  'DADAR AND NAGAR HAVELI'
			When   '27'  Then  'MAHARASTRA'
			When   '37'  Then  'ANDHRA PRADESH'
			When   '29'  Then  'KARNATAKA'
			When   '30'  Then  'GOA'
			When   '31'  Then  'LAKSHADWEEP'
			When   '32'  Then  'KERALA'
			When   '33'  Then  'TAMIL NADU'
			When   '34'  Then  'PONDICHERRY'
			When   '35'  Then  'ANDAMAN AND NICOBAR'
			When   '36'  Then  'TELENGANA'
			When   '2'  Then  'HIMACHAL PRADESH'
			When   '3'  Then  'PUNJAB'
			When   '4'  Then  'CHANDIGARH'
			When   '5'  Then  'UTTARAKHAND'
			When   '6'  Then  'HARYANA'
			When   '7'  Then  'DELHI'
			When   '8'  Then  'RAJASTHAN'
			When   '9'  Then  'UTTAR PRADESH'
			When   '10'  Then  'BIHAR'
			When   '11'  Then  'SIKKIM'
			When   '12'  Then  'ARUNACHAL PRADESH'
			When   '13'  Then  'NAGALAND'
			When   '14'  Then  'MANIPUR'
			When   '15'  Then  'MIZORAM'
			When   '16'  Then  'TRIPURA'
			When   '17'  Then  'MEGHALAYA'
			When   '18'  Then  'ASSAM'
			When   '19'  Then  'WEST BENGAL'
			When   '20'  Then  'JHARKHAND'
			When   '21'  Then  'ORISSA'

			else fromstate
			End
			where IsNull(fromstate,'') <> ''

			Update #TBL_EWB_GENERATION 
			Set toStateCode =
			Case toStateCode
			When   '0'  Then  'OTHER COUNTRIES'
			When   '1'  Then  'JAMMU AND KASHMIR'
			When   '22'  Then  'CHHATTISGARH'
			When   '23'  Then  'MADHYA PRADESH'
			When   '24'  Then  'GUJARAT'
			When   '25'  Then  'DAMAN AND DIU'
			When   '26'  Then  'DADAR AND NAGAR HAVELI'
			When   '27'  Then  'MAHARASTRA'
			When   '37'  Then  'ANDHRA PRADESH'
			When   '29'  Then  'KARNATAKA'
			When   '30'  Then  'GOA'
			When   '31'  Then  'LAKSHADWEEP'
			When   '32'  Then  'KERALA'
			When   '33'  Then  'TAMIL NADU'
			When   '34'  Then  'PONDICHERRY'
			When   '35'  Then  'ANDAMAN AND NICOBAR'
			When   '36'  Then  'TELENGANA'
			When   '2'  Then  'HIMACHAL PRADESH'
			When   '3'  Then  'PUNJAB'
			When   '4'  Then  'CHANDIGARH'
			When   '5'  Then  'UTTARAKHAND'
			When   '6'  Then  'HARYANA'
			When   '7'  Then  'DELHI'
			When   '8'  Then  'RAJASTHAN'
			When   '9'  Then  'UTTAR PRADESH'
			When   '10'  Then  'BIHAR'
			When   '11'  Then  'SIKKIM'
			When   '12'  Then  'ARUNACHAL PRADESH'
			When   '13'  Then  'NAGALAND'
			When   '14'  Then  'MANIPUR'
			When   '15'  Then  'MIZORAM'
			When   '16'  Then  'TRIPURA'
			When   '17'  Then  'MEGHALAYA'
			When   '18'  Then  'ASSAM'
			When   '19'  Then  'WEST BENGAL'
			When   '20'  Then  'JHARKHAND'
			When   '21'  Then  'ORISSA'
			else toStateCode
			End
			where IsNull(toStateCode,'') <> ''

			Update #TBL_EWB_GENERATION 
			Set actfromstatecode =
			Case actfromstatecode
			When   '0'  Then  'OTHER COUNTRIES'
			When   '1'  Then  'JAMMU AND KASHMIR'
			When   '22'  Then  'CHHATTISGARH'
			When   '23'  Then  'MADHYA PRADESH'
			When   '24'  Then  'GUJARAT'
			When   '25'  Then  'DAMAN AND DIU'
			When   '26'  Then  'DADAR AND NAGAR HAVELI'
			When   '27'  Then  'MAHARASTRA'
			When   '37'  Then  'ANDHRA PRADESH'
			When   '29'  Then  'KARNATAKA'
			When   '30'  Then  'GOA'
			When   '31'  Then  'LAKSHADWEEP'
			When   '32'  Then  'KERALA'
			When   '33'  Then  'TAMIL NADU'
			When   '34'  Then  'PONDICHERRY'
			When   '35'  Then  'ANDAMAN AND NICOBAR'
			When   '36'  Then  'TELENGANA'
			When   '2'  Then  'HIMACHAL PRADESH'
			When   '3'  Then  'PUNJAB'
			When   '4'  Then  'CHANDIGARH'
			When   '5'  Then  'UTTARAKHAND'
			When   '6'  Then  'HARYANA'
			When   '7'  Then  'DELHI'
			When   '8'  Then  'RAJASTHAN'
			When   '9'  Then  'UTTAR PRADESH'
			When   '10'  Then  'BIHAR'
			When   '11'  Then  'SIKKIM'
			When   '12'  Then  'ARUNACHAL PRADESH'
			When   '13'  Then  'NAGALAND'
			When   '14'  Then  'MANIPUR'
			When   '15'  Then  'MIZORAM'
			When   '16'  Then  'TRIPURA'
			When   '17'  Then  'MEGHALAYA'
			When   '18'  Then  'ASSAM'
			When   '19'  Then  'WEST BENGAL'
			When   '20'  Then  'JHARKHAND'
			When   '21'  Then  'ORISSA'
			else actfromstatecode
			End
			where IsNull(actfromstatecode,'') <> ''

  	Update #TBL_EWB_GENERATION 
			Set acttoStateCode =
			Case acttoStateCode
			When   '0'  Then  'OTHER COUNTRIES'
			When   '1'  Then  'JAMMU AND KASHMIR'
			When   '22'  Then  'CHHATTISGARH'
			When   '23'  Then  'MADHYA PRADESH'
			When   '24'  Then  'GUJARAT'
			When   '25'  Then  'DAMAN AND DIU'
			When   '26'  Then  'DADAR AND NAGAR HAVELI'
			When   '27'  Then  'MAHARASTRA'
			When   '37'  Then  'ANDHRA PRADESH'
			When   '29'  Then  'KARNATAKA'
			When   '30'  Then  'GOA'
			When   '31'  Then  'LAKSHADWEEP'
			When   '32'  Then  'KERALA'
			When   '33'  Then  'TAMIL NADU'
			When   '34'  Then  'PONDICHERRY'
			When   '35'  Then  'ANDAMAN AND NICOBAR'
			When   '36'  Then  'TELENGANA'
			When   '2'  Then  'HIMACHAL PRADESH'
			When   '3'  Then  'PUNJAB'
			When   '4'  Then  'CHANDIGARH'
			When   '5'  Then  'UTTARAKHAND'
			When   '6'  Then  'HARYANA'
			When   '7'  Then  'DELHI'
			When   '8'  Then  'RAJASTHAN'
			When   '9'  Then  'UTTAR PRADESH'
			When   '10'  Then  'BIHAR'
			When   '11'  Then  'SIKKIM'
			When   '12'  Then  'ARUNACHAL PRADESH'
			When   '13'  Then  'NAGALAND'
			When   '14'  Then  'MANIPUR'
			When   '15'  Then  'MIZORAM'
			When   '16'  Then  'TRIPURA'
			When   '17'  Then  'MEGHALAYA'
			When   '18'  Then  'ASSAM'
			When   '19'  Then  'WEST BENGAL'
			When   '20'  Then  'JHARKHAND'
			When   '21'  Then  'ORISSA'
			else acttoStateCode
			End
			where IsNull(acttoStateCode,'') <> ''

			Update #TBL_EWB_GENERATION 
			Set subsupplytype =
			Case subsupplytype
			When   '1'  Then  'Supply'
			When   '2'  Then  'Import'
			When   '3'  Then  'Export'
			When   '4'  Then  'Job Work'
			When   '5'  Then  'For Own Use'
			When   '6'  Then  'Job work Returns'
			When   '7'  Then  'Sales Return'
			When   '8'  Then  'Others'
			When   '9'  Then  'SKD/CKD'
			When   '10'  Then  'Line Sales'
			When   '11'  Then  'Recipient Not Known'
			When   '12'  Then  'Exhibition or Fairs'
			else subsupplytype
			End
			where IsNull(subsupplytype,'') <> ''

			Update #TBL_EWB_GENERATION 
			Set SupplyType =
			Case SupplyType
			When   'O'  Then  'Outward'
			When   'I'  Then  'Inward'
			else SupplyType
			End
			where IsNull(SupplyType,'') <> ''
			
			Update #TBL_EWB_GENERATION 
			Set transmode =
			Case transmode
			When   '1'  Then  'Road'
			When   '2'  Then  'Rail'
			When   '3'  Then  'Air'
			When   '4'  Then  'Ship'
			else transmode
			End
			where IsNull(transmode,'') <> ''
			
			
			Update #TBL_EWB_GEN_CONSOLIDATED 
			Set transmode =
			Case transmode
			When   '1'  Then  'Road'
			When   '2'  Then  'Rail'
			When   '3'  Then  'Air'
			When   '4'  Then  'Ship'
			else transmode
			End
			where IsNull(transmode,'') <> ''
			--Update #TBL_EWB_GENERATION 
			--Set qtyunit =
			--Case qtyunit
			--When   'BGS'  Then  'BAGS'
			--When   'BND'  Then  'BUNDLES'
			--When   'BOX'  Then  'BOXES'
			--When   'CMS'  Then  'CENTIMETERS'
			--When   'DZN'  Then  'DOZENS'
			--When   'GMS'  Then  'GRAMS'
			--When   'HKM'  Then  'HUNDRED KILOMETERS'
			--When   'HNO'  Then  'HUNDRED NUMBERS/UNITS'
			--When   'KGS'  Then  'KILOGRAMS'
			--When   'KMS'  Then  'KILOMETERS'
			--When   'LTS'  Then  'LITRES'
			--When   'MLS'  Then  'MILLILITRES'
			--When   'MTR'  Then  'METERS'
			--When   'MTS'  Then  'METRIC TONNES'
			--When   'NOS'  Then  'NUMBERS/UNITS'
			--When   'PAR'  Then  'PAIRS'
			--When   'QTS'  Then  'QUINTALS'
			--When   'SNO'  Then  'THOUSAND NUMBERS/UNITS'
			--When   'TKM'  Then  'THOUSAND KILOMETERS'
			--When   'TLT'  Then  'THOUSAND LITRES'
			--When   'TNO'  Then  'TEN NUMBERS/UNITS'
			--When   'TON'  Then  'TONNES'
			--else qtyunit
			--End
			--where IsNull(qtyunit,'') <> ''

			
			Update #TBL_EWB_GENERATION 
			Set vehicletype =
			Case vehicletype
			When   'R'  Then  'Regular'
			When   'O'  Then  'Over dimensional cargo'
			else vehicletype
			End
			where IsNull(vehicletype,'') <> ''
						
    
	Declare @LocationReq int
	select @LocationReq=LocationReqd from tbl_cust_settings where custid =@Custid and rowstatus =1
	If @LocationReq <> 1
		Begin
			Set @LocationReq = 0
		End

	--Create Table #branchids 
	--			(
	--			Branchid int
	--			)
	--if @LocationReq = 1
	--Begin 
	--	if @Branchid <> 'ALL' and (isnumeric(@Branchid)=1)
	--	Begin
	--		Insert into #branchids (Branchid) Select Convert(int,@Branchid)
	--	End
	--End

  If @Type='OUTWARD'  
   Begin  
		if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
					DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',					
					fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
					toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
					transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
					VehicleNo+' & '+VehicleType as 'Vehicle_Details',
					totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
					cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE supplyType in ('Outward','O')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
					And branchid =convert(int,@Branchid)

			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE  supplyType in ('Outward','O')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
		
		Else if(@LocationReq = 0)
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation  
					WHERE  supplyType in ('Outward','O')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End  
   End  
       
  If @Type='INWARD'  
   Begin  
			if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
					DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',					
					fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
					toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
					transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
					VehicleNo+' & '+VehicleType as 'Vehicle_Details',
					totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
					cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE supplyType in ('Inward','I')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
					And branchid =convert(int,@Branchid)

			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE   supplyType in ('Inward','I')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
		
		Else if(@LocationReq = 0)
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation  
					WHERE   supplyType in ('Inward','I')
					AND (flag is null OR flag = 'G') 
					AND userGSTIN=@userGSTIN  
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End   
   End  
  
  If @Type='GENEWB'  
   Begin  		
		if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
					DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',					
					fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
					toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
					transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
					VehicleNo+' & '+VehicleType as 'Vehicle_Details',
					totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
					cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE flag is null
					AND userGSTIN = @userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
					And branchid =convert(int,@Branchid)

			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation 
					WHERE flag is null
					AND userGSTIN = @userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
		
		Else if(@LocationReq = 0)
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name
					FROM #TBL_EWB_Generation  
					WHERE flag is null
					AND userGSTIN = @userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
   End  
  
  If @Type='CONSGENEWB'  
   Begin				
		if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select cEwbNo+' & '+cEWBDate as 'Consolidate_Ewaybill_Details',
							fromplace+ '& '+fromState as 'From_Details',
							transDocNo+' '+TransDocDate+' '+TransMode as 'Transporter_Details',
							vehicleNo as 'Vehicle_No',
							BranchName as 'Branch_Name'
						FROM #TBL_EWB_GEN_CONSOLIDATED
						WHERE flag is null
							AND userGSTIN = @userGSTIN 
							AND cEWBDate like '%' +@Period +'%'
							AND CustId=@CustId
							And branchid =convert(int,@Branchid)
			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select cEwbNo+' & '+cEWBDate as 'Consolidate_Ewaybill_Details',
							fromplace+ '& '+fromState as 'From_Details',
							transDocNo+' '+TransDocDate+' '+TransMode as 'Transporter_Details',
							vehicleNo as 'Vehicle_No',
							BranchName as 'Branch_Name'
					FROM #TBL_EWB_GEN_CONSOLIDATED
					WHERE flag is null
						AND userGSTIN = @userGSTIN 
						AND cEWBDate like '%' +@Period +'%'
						AND CustId=@CustId
			End		
	    Else if(@LocationReq = 0)
			Begin			
				Select cEwbNo+' & '+cEWBDate as 'Consolidate_Ewaybill_Details',
							fromplace+ '& '+fromState as 'From_Details',
							transDocNo+' '+TransDocDate+' '+TransMode as 'Transporter_Details',
							vehicleNo as 'Vehicle_No',
							BranchName as 'Branch_Name'
					FROM #TBL_EWB_GEN_CONSOLIDATED
					WHERE flag is null
						AND userGSTIN = @userGSTIN 
						AND cEWBDate like '%' +@Period +'%'
						AND CustId=@CustId
			End
   End  
  
  If @Type='CANCEL'  
   Begin
  if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
					DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',					
					fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
					toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
					transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
					VehicleNo+' & '+VehicleType as 'Vehicle_Details',
					totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
					cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
					Status,RejectStatus
					FROM #TBL_EWB_Generation 
					WHERE (flag is null OR flag='G')  
				    AND userGSTIN=@userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId  
					AND status='CNL'  
					And branchid =convert(int,@Branchid)

			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
				Status,RejectStatus
					FROM #TBL_EWB_Generation 
					WHERE flag is null
					AND userGSTIN = @userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
		
		Else if(@LocationReq = 0)
			Begin
				Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
				DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',
				fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
				toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
				transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
				VehicleNo+' & '+VehicleType as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
				Status,RejectStatus
					FROM #TBL_EWB_Generation  
					WHERE flag is null
					AND userGSTIN = @userGSTIN 
					AND ewayBillDate like '%' +@Period +'%' 
					AND CustId=@CustId
			End
   End  
  
   If @Type='CAN_Retrieve'  
   Begin  
     Select * From #TBL_EWB_Generation Where (flag is null OR flag='G') AND CustId=@CustId  
   End  
  
  
  If @Type='REJECT'
  Begin 
  if (@LocationReq = 1 and  @Branchid <> 'ALL' and isnumeric(@Branchid) = 1)
			Begin
					Select ewayBillNo+' & '+isnull(ewayBillDate,'') as 'Eway_Bill_Details',
					isnull(DocNo,'')+' & '+isnull(DocDate,'')+' & '+isnull(DocType,'') as 'Document_Details',
					fromGstin as 'From_Details',
					toGstin as 'To_Details',
					totinvValue as Total_Invoice_Amount,
					Status,RejectStatus
					FROM #TBL_EWB_Generation 
					WHERE flag='O'  
					  AND userGSTIN=@userGSTIN 
					  AND ewayBillDate like '%' +@Period +'%' 
					  AND CustId=@CustId  
					  AND rejectStatus='Y'    
					  And branchid =convert(int,@Branchid)

			End
		if (@LocationReq = 1 and  @Branchid = 'ALL')
			Begin
				Select ewayBillNo+' & '+isnull(ewayBillDate,'') as 'Eway_Bill_Details',
				isnull(DocNo,'')+' & '+isnull(DocDate,'')+' & '+isnull(DocType,'') as 'Document_Details',
				fromGstin as 'From_Details',
			    toGstin as 'To_Details',
				transporterId+' '+isnull(transDocNo,'')+' '+isnull(transdocDate,'')+' '+isnull(transMode,'')+' '+isnull(transDistance,'')+' '+isnull(transporterName,'') as 'Transported_Details',
				VehicleNo+' & '+isnull(VehicleType,'') as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
				Status,RejectStatus
					FROM #TBL_EWB_Generation 
					Where flag='O'  
					  AND userGSTIN=@userGSTIN 
					  AND ewayBillDate like '%' +@Period +'%' 
					  AND CustId=@CustId  
					  AND rejectStatus='Y'  
			End
		
		Else if(@LocationReq = 0)
			Begin
				Select ewayBillNo+' & '+isnull(ewayBillDate,'') as 'Eway_Bill_Details',
				isnull(DocNo,'')+' & '+isnull(DocDate,'')+' & '+isnull(DocType,'') as 'Document_Details',
				fromGstin as 'From_Details',
			     toGstin as 'To_Details',
				 transporterId+' '+isnull(transDocNo,'')+' '+isnull(transdocDate,'')+' '+isnull(transMode,'')+' '+isnull(transDistance,'')+' '+isnull(transporterName,'') as 'Transported_Details',
				VehicleNo+' & '+isnull(VehicleType,'') as 'Vehicle_Details', 
				totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
				cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
				Status,RejectStatus
					FROM #TBL_EWB_Generation  
					Where flag='O'  
					  AND userGSTIN=@userGSTIN 
					  AND ewayBillDate like '%' +@Period +'%' 
					  AND CustId=@CustId  
					  AND rejectStatus='Y'  
			End
   End  
   
   --Begin  
   --  Select ewayBillNo+' & '+ewayBillDate as 'Eway_Bill_Details',
			--		DocNo+' & '+DocDate+' & '+DocType as 'Document_Details',					
			--		fromGstin+' '+fromTRDName+' '+fromAddr1+' '+fromAddr2+' '+fromPlace+' '+Convert(varchar(50),fromStateCode)+' '+Convert(varchar(50),actFromStateCode)+' '+Convert(varchar(50),fromPinCode) as 'From_Details',
			--		toGstin+' '+toTRDName+' '+toAddr1+' '+toAddr2+' '+toPlace+' '+Convert(varchar(50),toStateCode)+' '+Convert(varchar(50),acttoStateCode)+' '+Convert(varchar(50),toPinCode) as 'To_Details',
			--		transporterId+' '+transDocNo+' '+transdocDate+' '+transMode+' '+transDistance+' '+transporterName as 'Transported_Details',
			--		VehicleNo+' & '+VehicleType as 'Vehicle_Details',
			--		totalValue as Taxable_Value,totinvValue as Total_Invoice_Amount,igstValue as IGST_Amount,
			--		cgstValue as CGST_Amount,sgstValue as SGST_Amount,CessValue as CESS_Amount,BranchName as Branch_Name,
			--		Status,RejectStatus	 From #TBL_EWB_Generation 
			--		Where flag='O'  
	  --AND userGSTIN=@userGSTIN AND ewayBillDate like '%' +@Period +'%' AND CustId=@CustId  
	  --AND rejectStatus='Y'  
   --End  
  
   If @Type='OTHERSREJECT'  
   Begin  
     Select * From #TBL_EWB_Generation Where flag='G'  
  AND userGSTIN=@userGSTIN AND ewayBillDate like '%' +@Period +'%' AND CustId=@CustId  
  AND rejectStatus='Y'  
   End  
  
  If @Type='OTHERSEWB'  
   Begin  
     Select * From #TBL_EWB_Generation Where flag='O'  
     AND toGstin=@userGSTIN AND ewayBillDate like '%' +@Period +'%' AND CustId=@CustId  
   End  
  
  If @Type='GETEWB'  
   Begin  
     Select * From #TBL_EWB_Generation Where flag='G'  
  AND userGSTIN=@userGSTIN AND ewayBillNo=@EwbNo AND CustId=@CustId  
   End  
  
  
  If @Type='CONSGETEWB'  
   Begin  
     Select * From #TBL_EWB_GEN_CONSOLIDATED Where flag = 'G'  
  AND userGSTIN=@userGSTIN AND cEwbNo=@CEwbNo AND CustId=@CustId  
   End  
  
   If @Type='TRANSDATE'  
   Begin  
     Select * From TBL_EWB_Transporter Where flag='D' AND ewbDate like '%' +@Period +'%'  
     AND userGSTIN=@userGSTIN AND custid=@CustId  
   End  
  
   If @Type='TRANSGSTIN'  
   Begin  
     Select * From TBL_EWB_Transporter Where flag = 'G' AND genGstin=@genGSTIN  
  AND userGSTIN=@userGSTIN AND ewbDate like '%' +@Period +'%' AND custid=@CustId  
   End  
   
 Return 0  
End