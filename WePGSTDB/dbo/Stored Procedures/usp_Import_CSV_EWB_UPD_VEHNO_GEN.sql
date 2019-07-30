/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : raja.m@wepindia.com

Date		Who			Decription 
07/03/2018	Raja		Initial Version
03/04/2018  Karthik		Transformation logic and few additonal validations included


*/

/* Sample Procedure Call  

exec usp_Import_CSV_EWB_UPD_VEHNO_GEN 28,1,1

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_EWB_UPD_VEHNO_GEN]
	@FileId int,
	@UserId int, 
	@CustId int,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1),
			@ReferenceNo varchar(250),
			@SourceType varchar(255)

	Select @Delimiter = ','
	Select @SourceType = 'CSV'

	Select @ReferenceNo = ReferenceNo from TBL_Customer where CustId = @CustId

	--Update TBL_TRP_RECVD_FILES
	--Set filestatus = 2
	--Where fileid = @FileId

	Create Table #TBL_CSV_EWB_UPD_VEHNO_RECS
	(
		fileid int NULL,
		slno varchar(250),
		usergstin varchar(50) NULL,
		ewbno bigint NULL,
		vehicleno varchar(50) NULL,
		fromplace varchar(50) NULL,
		fromstate varchar(50) NULL,
		reasoncode varchar(50) NULL,
		reasonremarks varchar(250) NULL,
		transmode varchar(50) NULL,
		transdocno varchar(50) NULL,
		transdocdate varchar(50) NULL,		
		errorcode smallint NULL,
		errormessage varchar(255) NULL		
	 )

	 Begin Try

		Insert Into #TBL_CSV_EWB_UPD_VEHNO_RECS
		( fileid,
			slno,
			usergstin,
			ewbno,
			vehicleno,
			fromplace,
			fromstate,
			reasoncode,
			reasonremarks,
			transmode,
			transdocno,
			transdocdate )

		Select	fileid,
			slno,
			usergstin,
			ewbno,
			vehicleno,
			fromplace,
			fromstate,
			reasoncode,
			reasonremarks,
			transmode,
			transdocno,
			transdocdate
		From TBL_CSV_EWB_UPD_VEHNO_RECS t1 
		Where t1.fileid = @FileId
	
	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch
	
	-- Processing Logic
	--select * from #TBL_CSV_EWB_UPD_VEHNO_RECS

	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_EWB_UPD_VEHNO_RECS

	if exists (Select 1 from #TBL_CSV_EWB_UPD_VEHNO_RECS)
	Begin
	
		Update #TBL_CSV_EWB_UPD_VEHNO_RECS 
			Set ErrorCode = -1,
				ErrorMessage = 'All Parameters are Mandatory'
			Where (Ltrim(Rtrim(IsNull( ewbNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( vehicleNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( fromPlace,''))) = ''
				Or  Ltrim(Rtrim(IsNull( fromState,''))) = ''				
				Or  Ltrim(Rtrim(IsNull( reasoncode,''))) = ''
				Or  Ltrim(Rtrim(IsNull( reasonremarks,''))) = ''
				Or  Ltrim(Rtrim(IsNull( transMode,''))) = ''
				Or  Ltrim(Rtrim(IsNull( transDocNo,''))) = ''
				Or  Ltrim(Rtrim(IsNull( transDocDate,''))) = ''				
				)
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS 
			Set fromState =
			Case fromState
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
			else fromState
			End
			where IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS 
			Set transMode =
			Case convert(varchar(10),transMode)
			When  'Road'  Then  '1'
			When  'Rail'  Then  '2'
			When  'Air'  Then  '3'
			When  'Ship'  Then  '4'
			else transmode
			End
			where IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS -- Introduced due to Excel Format Issue
			Set fromState = '0' + Ltrim(Rtrim(IsNull(fromState,'')))
			Where Len(Ltrim(Rtrim(IsNull(fromState,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS
			Set ErrorCode = -2,
				ErrorMessage = 'Invalid From State Code'
			Where fromState not in ('00','01','02','03','04','05','06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
			And IsNull(ErrorCode,0) = 0		

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS  
			Set ErrorCode = -2,
				ErrorMessage = 'Invalid From State Code'
			Where dbo.udf_ValidatePlaceOfSupply(fromState) <> 1
			And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWB_UPD_VEHNO_RECS
			Set ErrorCode = -11,
				ErrorMessage = 'Invalid Transport mode'
			Where transmode not in ('1','2','3','4')
			And IsNull(ErrorCode,0) = 0

		-- Select * from #TBL_CSV_EWB_UPD_VEHNO_RECS
		--  Inserting Customer		 
		Insert into TBL_EXT_EWB_UPDATE_VEHICLENO (
			userGSTIN,
			ewbno,
			vehicleno,
			fromplace,
			fromstate,
			reasoncode,
			reasonremarks,
			transmode,
			transdocno,
			transdocdate,
			rowstatus, sourcetype, referenceno, createdby, createddate, fileid
			) 
		Select usergstin, ewbno, vehicleno, fromplace, fromstate, reasoncode, reasonremarks, transmode, transdocno, transdocdate, 
			1, @SourceType, @ReferenceNo, @UserId, @CustId, @FileId
		From #TBL_CSV_EWB_UPD_VEHNO_RECS t1 		
		Where Not Exists(Select 1 From TBL_EXT_EWB_UPDATE_VEHICLENO t2 Where t2.transdocno = t1.transdocno and t2.transdocdate = t1.transdocdate 
							and t2.ewbno = t1.ewbno and t2.vehicleno = t1.vehicleno)
		And IsNull(ErrorCode,0) = 0


		Select @ProcessedRecordsCount = Count(*)
		From #TBL_CSV_EWB_UPD_VEHNO_RECS fv
					Where IsNull(fv.ErrorCode,0) = 0

		Select @ErrorRecordsCount = Count(*)
		From #TBL_CSV_EWB_UPD_VEHNO_RECS fv
		Where IsNull(fv.ErrorCode,0) <> 0

		Update TBL_EWB_RECVD_FILES
				Set filestatus = 0,
				totalrecordscount = @TotalRecordsCount,
				processedrecordscount = @ProcessedRecordsCount,
				errorrecordscount = @ErrorRecordsCount
		Where fileid = @FileId

		exec usp_Push_EWB_UPDATE_VEHICLENO_EXT_SA  @SourceType, @ReferenceNo

		Select 'Total Record Count :' + convert(varchar(10), @TotalRecordsCount)
		Select 'Processed Record Count :' + convert(varchar(10), @ProcessedRecordsCount)
		Select 'Error Record Count :' + convert(varchar(10), @ErrorRecordsCount)
				
		select * from #TBL_CSV_EWB_UPD_VEHNO_RECS where IsNull(ErrorCode,0) <> 0
		Delete from TBL_CSV_EWB_UPD_VEHNO_RECS where fileid = @FileId

	End 
	 Return 0


End