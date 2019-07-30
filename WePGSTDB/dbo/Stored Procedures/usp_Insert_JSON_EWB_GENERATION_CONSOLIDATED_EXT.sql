

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
01/02/2018	Raja	Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_EWB_GENERATION_CONSOLIDATED_EXT  'ERP','WEP001','{
  "fromPlace": "BANGALORE SOUTH",
  "fromState": 29,
  "vehicleNo": "KA12AB1234",
  "transMode": "1",
  "TransDocNo": "57567",
  "TransDocDate": "18/12/2017",
  "tripSheetEwbBills": [
    {
      "ewbNo": 381000975546
    },
    {
      "ewbNo": 371000975530
    }
  ]
}'


 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_EWB_GENERATION_CONSOLIDATED_EXT]
	@userGSTIN varchar(15),
	@SourceType varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@Location varchar(50),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@JsonResult nvarchar(max) = NULL OUT,
	@ErrorRecords nvarchar(max) = NULL out,
	@TransDocNo nvarchar(50) = NULL out,
	@TransDocDate nvarchar(50) = NULL out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @TotalRecordsCount int,@ProcessedRecordsCount int,@ErrorRecordsCount int

	
	Select	@SourceType as Sourcetype,
			@ReferenceNo as ReferenceNo,
			fromPlace as fromPlace,
			fromState as fromState,
			vehicleNo as vehicleNo,
			transMode as transMode,
			TransDocNo as TransDocNo,
			TransDocDate as TransDocDate,
			tripSheetEwbBills as tripsheetEwbBills,
			ewbNo as ewbNo,
			space(255) as errormessage,
			space(10) as errorcode 
			Into #TBL_EXT_EWB_GENERATION_CONSOLIDATED
	From OPENJSON(@RecordContents) 	
	WITH
	(		
		fromPlace nvarchar(50),
		fromState nvarchar(2),
		vehicleNo nvarchar(20),
		transMode nvarchar(10),
		TransDocNo nvarchar(50),
		TransDocDate nvarchar(20),
		tripSheetEwbBills nvarchar(max) as JSON		
	) as ewaybill
	Cross Apply OPENJSON(ewaybill.tripSheetEwbBills) 
	WITH
	(
		ewbNo nvarchar(20)
	) As TripsheetEwbBills

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

	--Select * from #TBL_EXT_EWB_GENERATION_CONSOLIDATED
	Select @TotalRecordsCount = count(*) from #TBL_EXT_EWB_GENERATION_CONSOLIDATED

			Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED 
			Set ErrorCode = -1,
				ErrorMessage = 'All Parameters are Mandatory'
			Where (Ltrim(Rtrim(IsNull( fromPlace,''))) = ''
				Or  Ltrim(Rtrim(IsNull( fromState,''))) = ''
				Or  Ltrim(Rtrim(IsNull( vehicleNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( transMode,''))) = ''
				Or  Ltrim(Rtrim(IsNull( TransDocNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( TransDocDate,''))) = ''
				Or  Ltrim(Rtrim(IsNull( tripSheetEwbBills,''))) = ''
				Or  Ltrim(Rtrim(IsNull( ewbNo,''))) = ''
				)
				And IsNull(ErrorCode,0) = 0

			Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED  
			Set ErrorCode = -2,
				ErrorMessage = 'Invalid From State Code'
			Where dbo.udf_ValidatePlaceOfSupply(fromState) <> 1
			And IsNull(ErrorCode,0) = 0
		
		Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Trans Doc Date'
		Where dbo.udf_ValidateDate(TransDocDate) <> 1
		And IsNull(ErrorCode,0) = 0


	Begin Try
		Begin
			Insert into TBL_EXT_EWB_GENERATION_CONSOLIDATED
				(userGSTIN, vehicleNo, fromPlace, transMode, TransDocNo, TransDocDate, fromState, ewbNo, 
				rowstatus,createdby, sourcetype, referenceno, createddate, BranchId) 
				Select @userGSTIN, vehicleNo, fromPlace, transMode, TransDocNo, TransDocDate, fromState, ewbNo,
				 1, isnull(@CreatedBy,0),@SourceType, @ReferenceNo, DATEADD (mi, 330, GETDATE()), @BranchId
				 from #TBL_EXT_EWB_GENERATION_CONSOLIDATED


				 -- Push EWB CONSOLIDATED FOR EXT TO SA
				exec usp_Push_EWB_CONSOLIDATED_EXT_SA @SourceType, @ReferenceNo

				Select @TransDocNo = (Select TOP 1 TransDocNo from #TBL_EXT_EWB_GENERATION_CONSOLIDATED)
				Select @TransDocDate = (Select TOP 1 TransDocDate from #TBL_EXT_EWB_GENERATION_CONSOLIDATED)
			
		End
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Eway Bill Consolidated Data'
				From #TBL_EXT_EWB_GENERATION_CONSOLIDATED t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

		

		Select @ProcessedRecordsCount = Count(*)
			From #TBL_EXT_EWB_GENERATION_CONSOLIDATED
			Where IsNull(ErrorCode,0) = 0

			Select @ErrorRecordsCount = Count(*)
			From #TBL_EXT_EWB_GENERATION_CONSOLIDATED  
			Where IsNull(ErrorCode,0) <> 0

			Select @ErrorRecords = (Select * From #TBL_EXT_EWB_GENERATION_CONSOLIDATED
									Where IsNull(ErrorCode,0) <> 0
									FOR JSON AUTO)
	

		Select @JsonResult =  (Select @TotalRecordsCount as TotalRecordsCount,@ProcessedRecordscount as ProcessedRecordscount,@ErrorRecordsCount as ErrorRecordsCount, JSON_QUERY((@ErrorRecords)) as ErrorRecords FOR JSON PATH )
							-- RetStatus
			Select @JsonResult	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

End