

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
14/05/2018	Raja		Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_BULK_JSON_EWB_GENERATION_EXT 'ERP','WEP001',''

 */

CREATE PROCEDURE [dbo].[usp_Insert_BULK_JSON_EWB_GENERATION_EXT]
	@userGSTIN varchar(15),
	@SourceType varchar(15),
	@ReferenceNo varchar(50),
	@Location varchar(50),
	@ReferenceId varchar(100),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@JsonResult nvarchar(max) = NULL OUT,
	@ErrorRecords nvarchar(max) = NULL out,
	@DocNo nvarchar(50) = NULL out,
	@DocDate nvarchar(50) = NULL out
as 
Begin

	Set Nocount on
	Declare @TotalRecordsCount int,@ProcessedRecordsCount int,@ErrorRecordsCount int

	Select	@SourceType as Sourcetype,
			@ReferenceNo as ReferenceNo,
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

	Declare @CreatedBy int, @CustId int, @RoleId int, @Email nvarchar(250), @LocationId int,@GstinId int,@Pan varchar(10),@PanId int,@BranchId int
	Select @CustId = Custid, @Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
	Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus = 1
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
	
	--Select * from #TBL_EXT_EWB_GENERATION
	Select @TotalRecordsCount = count(*) from #TBL_EXT_EWB_GENERATION

	--Update #TBL_EXT_EWB_GENERATION 
	--		Set ErrorCode = -1,
	--			ErrorMessage = 'Supply Type is mandatory'
	--		Where Ltrim(Rtrim(IsNull(supplyType,''))) = '' 
	--		And IsNull(ErrorCode,0) = 0

	Update #TBL_EXT_EWB_GENERATION 
			Set ErrorCode = -1,
				ErrorMessage = 'All Parameters are Mandatory'
			Where (Ltrim(Rtrim(IsNull( supplyType,''))) = ''
				Or  Ltrim(Rtrim(IsNull( subSupplyType,''))) = ''
				Or  Ltrim(Rtrim(IsNull( docType,''))) = ''
				Or  Ltrim(Rtrim(IsNull( docNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( docDate,''))) = ''
				Or  Ltrim(Rtrim(IsNull( fromGstin,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( fromTrdName,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( fromAddr1,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( fromAddr2,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( fromPlace,''))) = ''
				or  IsNull(fromPincode,0) = 0
				or  IsNull(fromStateCode,0)= 0
				Or  Ltrim(Rtrim(IsNull( toGstin,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( toTrdName,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( toAddr1,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( toAddr2,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( toPlace,''))) = ''
				Or  IsNull( toPincode,0) = 0
				Or  IsNull( toStateCode, 0)= 0
				or  IsNull(totalValue,0) = 0
				Or  IsNull( acttoStateCode, 0)= 0
				Or  IsNull( actfromStateCode, 0)= 0
				--Or  IsNull( cgstValue,0) = 0
				--Or  IsNull( sgstValue,0) = 0
				--Or  IsNull( igstValue,0) = 0
				--Or  IsNull( cessValue,0) = 0
				--Or  IsNull( transMode,0) = 0
				Or  IsNull( transDistance,0) = 0
				--Or  Ltrim(Rtrim(IsNull( transporterId,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( transporterName,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( transDocNo,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( transDocDate,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( vehicleNo,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( productName,''))) = ''
				--Or  Ltrim(Rtrim(IsNull( productDesc,''))) = ''
				Or  Ltrim(Rtrim(IsNull( hsnCode,''))) = ''
				Or  IsNull( quantity,0) = 0
				Or  Ltrim(Rtrim(IsNull( qtyUnit,''))) = ''
				----Or  Ltrim(Rtrim(IsNull( taxableAmount,''))) = ''
				----Or  Ltrim(Rtrim(IsNull( cgstRate,''))) = ''
				----Or  Ltrim(Rtrim(IsNull( sgstRate,''))) = ''
				----Or  Ltrim(Rtrim(IsNull( igstRate,''))) = ''
				----Or  Ltrim(Rtrim(IsNull( cessRate,''))) = ''
				)
				And IsNull(ErrorCode,0) = 0

		
		Update #TBL_EXT_EWB_GENERATION -- Introduced due to Excel Format Issue
		Set fromStateCode = '0' + Ltrim(Rtrim(IsNull(fromStateCode,'')))
		Where Len(Ltrim(Rtrim(IsNull(fromStateCode,'')))) = 1 And IsNull(ErrorCode,0) = 0

		--Update #TBL_EXT_EWB_GENERATION  
		--Set ErrorCode = -2,
		--	ErrorMessage = 'Invalid From State Code'
		--Where dbo.udf_ValidatePlaceOfSupply(fromStateCode) <> 1
		--And IsNull(ErrorCode,0) = 0

		--Update #TBL_EXT_EWB_GENERATION  
		--Set ErrorCode = -3,
		--	ErrorMessage = 'Invalid To State Code'
		--Where dbo.udf_ValidatePlaceOfSupply(toStateCode) <> 1
		--And IsNull(ErrorCode,0) = 0		

		Update #TBL_EXT_EWB_GENERATION
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid FromGstin'
		Where dbo.udf_ValidateGstin(fromGstin) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid ToGstin'
		Where dbo.udf_ValidateGstin(toGstin) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_EWB_GENERATION
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Doc Date'
		Where dbo.udf_ValidateDate(docDate) <> 1
		And IsNull(ErrorCode,0) = 0
		
		Update #TBL_EXT_EWB_GENERATION
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Trans Doc Date'
		Where dbo.udf_ValidateDate(transDocDate) <> 1
		And isnull(transDocDate,'') <> ''
		And IsNull(ErrorCode,0) = 0

	Begin Try
		Begin
			Insert into TBL_EXT_EWB_GENERATION
				(userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
					fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode,
					toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode,
					totalValue, cgstValue, sgstValue, igstValue, cessValue, totinvvalue, 
					transMode, transDistance, transporterId, transporterName, transDocNo, transDocDate, vehicleNo, vehicleType, 
					productName, productDesc, hsnCode, quantity, qtyUnit, taxableAmount, cgstRate, sgstRate, igstRate, cessRate, cessAdvol, 
					rowstatus, sourcetype, referenceno, createddate, BranchId, APIBulkFlag, ReferenceId) 
				Select @userGSTIN, supplyType, subSupplyType, docType, docNo, docDate,
						fromGstin,fromTrdName,fromAddr1,fromAddr2,fromPlace, fromPinCode, fromStateCode, actFromStateCode, 
						toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
						totalValue, IsNull(cgstValue,0), IsNull(sgstValue,0), IsNull(igstValue,0), isnull(cessValue,0), isnull(totinvvalue,0), 
						transMode, transDistance,transporterId,transporterName, transDocNo,transDocDate,vehicleNo,vehicleType, 
						productName, productDesc, hsnCode, quantity, qtyUnit, 
						taxableAmount, cgstRate, sgstRate, igstRate, cessRate, cessAdvol, 
						1, @SourceType, @ReferenceNo, DATEADD (mi, 330, GETDATE()), @BranchId, 1, @ReferenceId
						from #TBL_EXT_EWB_GENERATION where IsNull(ErrorCode,0) = 0

			--exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo

			--Select @DocNo = (Select TOP 1 docNo From #TBL_EXT_EWB_GENERATION)
			--Select @DocDate = (Select TOP 1 docDate From #TBL_EXT_EWB_GENERATION)
								
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
			End				
		End Catch

		exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo
		

		Select @ProcessedRecordsCount = Count(*)
		From #TBL_EXT_EWB_GENERATION
		Where IsNull(ErrorCode,0) = 0

		Select @ErrorRecordsCount = Count(*)
		From #TBL_EXT_EWB_GENERATION  
		Where IsNull(ErrorCode,0) <> 0

		--exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo

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