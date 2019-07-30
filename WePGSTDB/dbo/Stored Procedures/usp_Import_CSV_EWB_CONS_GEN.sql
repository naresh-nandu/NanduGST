/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : raja.m@wepindia.com

Date		Who			Decription 
07/03/2018	Raja		Initial Version
03/04/2018  Karthik		Transformation logic and few additonal validations included
31/05/2018  Karthik		Vehicle No Validation,TransMode validations included.


*/

/* Sample Procedure Call  

exec usp_Import_CSV_EWB_CONS_GEN 227,1,1

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_EWB_CONS_GEN]
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

	Create Table #TBL_CSV_CONSEWBGEN_RECS
	(
		fileid int NULL,
		slno varchar(250),
		branch varchar(250) NULL,
		usergstin varchar(50) NULL,
		vehicleno varchar(50) NULL,
		fromplace varchar(50) NULL,
		transmode varchar(50) NULL,
		transdocno varchar(50) NULL,
		transdocdate varchar(50) NULL,
		fromstate varchar(50) NULL,
		ewbno varchar(50) NULL,
		AllowDuplication varchar(10) NULL,
		gstinid int NULL,
		errorcode smallint NULL,
		errormessage varchar(255) NULL		
	 )

	 Begin Try

		Insert Into #TBL_CSV_CONSEWBGEN_RECS
		( fileid,
			slno,
			branch,
			usergstin,
			vehicleno,
			fromplace,
			transmode,
			transdocno,
			transdocdate,
			fromstate,
			ewbno,
			AllowDuplication )
		Select	fileid,
			slno,
			branch,
			usergstin,
			vehicleno,
			fromplace,			
			transmode,
			transdocno,
			transdocdate,
			fromstate,
			ewbno,
			AllowDuplication
		From TBL_CSV_EWB_GEN_CONS_RECS t1 
		Where t1.fileid = @FileId
	
	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch
	
	-- Processing Logic
	--select * from #TBL_CSV_CONSEWBGEN_RECS

	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_CONSEWBGEN_RECS

	if exists (Select 1 from #TBL_CSV_CONSEWBGEN_RECS)
	Begin

			Update #TBL_CSV_CONSEWBGEN_RECS Set fromPlace = Ltrim(Rtrim(IsNull(fromPlace,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set fromState = Ltrim(Rtrim(IsNull(fromState,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set vehicleNo = Ltrim(Rtrim(IsNull(vehicleNo,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set transMode = Ltrim(Rtrim(IsNull(transMode,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set transDocNo = Ltrim(Rtrim(IsNull(transDocNo,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set transDocDate = Ltrim(Rtrim(IsNull(transDocDate,'')))
			Update #TBL_CSV_CONSEWBGEN_RECS Set ewbNo = Ltrim(Rtrim(IsNull(ewbNo,'')))

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

			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -1,
					ErrorMessage = 'From Place is Mandatory'
				Where (Ltrim(Rtrim(IsNull( fromPlace,''))) = '')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -1,
					ErrorMessage = 'From State is Mandatory'
				Where (Ltrim(Rtrim(IsNull( fromState,''))) = '')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -1,
					ErrorMessage = 'Trans Mode is Mandatory'
				Where (Ltrim(Rtrim(IsNull( transMode,''))) = '')
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -1,
					ErrorMessage = 'Eway Bill No is Mandatory'
				Where (Ltrim(Rtrim(IsNull( ewbNo,''))) = '')
				And IsNull(ErrorCode,0) = 0

				Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -1,
					ErrorMessage = 'Eway Bill No is Not Numeric'
				Where isnumeric(ewbNo) <> 1
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS 
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

			-- TransMode Validation
			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set transmode =
				Case transmode
				When  'Road'  Then  '1'
				When  'Rail'  Then  '2'
				When  'Air'  Then  '3'
				When  'Ship'  Then  '4'
				else transmode
				End
				where Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0


			Update #TBL_CSV_CONSEWBGEN_RECS
				Set ErrorCode = -11,
					ErrorMessage = 'Invalid Transport mode'
				Where transmode not in ('','1','2','3','4')
				And Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS
				Set ErrorCode = -12,
					ErrorMessage = 'Vehicle No is Mandatory'
				Where isnull(vehicleno,'')='' and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS
				Set ErrorCode = -13,
					ErrorMessage = 'Invalid Vehicle No'
				Where dbo.udf_ValidateVehicleNo(vehicleno) <> 1 and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS
				Set ErrorCode = -12,
					ErrorMessage = 'Trans Doc No /Doc Date is Mandatory'
				Where (isnull(transDocNo,'')='' or isnull(transDocDate,'')='') and Ltrim(Rtrim(IsNull(transmode,''))) <> '1'
				And IsNull(ErrorCode,0) = 0

			
			Update #TBL_CSV_CONSEWBGEN_RECS -- Introduced due to Excel Format Issue
				Set fromState = '0' + Ltrim(Rtrim(IsNull(fromState,'')))
				Where Len(Ltrim(Rtrim(IsNull(fromState,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS
				Set ErrorCode = -2,
					ErrorMessage = 'Invalid From State Code'
				Where fromState not in ('00','01','02','03','04','05','06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0		

			Update #TBL_CSV_CONSEWBGEN_RECS  
				Set ErrorCode = -2,
					ErrorMessage = 'Invalid From State Code'
				Where dbo.udf_ValidatePlaceOfSupply(fromState) <> 1
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS 
			Set ErrorCode = -1,
				ErrorMessage = 'Eway Bill No is Mandatory'
			Where (Ltrim(Rtrim(IsNull( ewbNo,''))) = '')
			And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_CONSEWBGEN_RECS 
				Set ErrorCode = -12,
					ErrorMessage = 'Eway Bill Number is not Numeric'
				Where isnumeric(Ltrim(Rtrim(IsNull(ewbno,0)))) <> 1
				And IsNull(ErrorCode,0) = 0 


				Update #TBL_CSV_CONSEWBGEN_RECS
				Set #TBL_CSV_CONSEWBGEN_RECS.gstinid = t2.gstinid
				FROM #TBL_CSV_CONSEWBGEN_RECS t1,
						tbl_cust_gstin t2 
				WHERE upper(t1.usergstin) = upper(t2.Gstinno)
				And t2.Custid = @CustId
				And t2.rowstatus =1
				and  IsNull(ErrorCode,0) = 0
		
			if (@LocationReq = 1)
			Begin
			
				Update #TBL_CSV_CONSEWBGEN_RECS
					Set #TBL_CSV_CONSEWBGEN_RECS.branch = t2.branchid
					FROM #TBL_CSV_CONSEWBGEN_RECS t1,
							tbl_Cust_Location t2 
					WHERE upper(ltrim(rtrim(t2.branch))) = upper(ltrim(rtrim(t1.branch)))
					And t2.Custid = @CustId
					And (t2.gstinid = t1.gstinid or isnull(t1.gstinid,0) = 0)
					And t2.rowstatus =1
					and  IsNull(ErrorCode,0) = 0

				Update #TBL_CSV_CONSEWBGEN_RECS
					Set ErrorCode = -54,
						ErrorMessage = 'Invalid Branch name'
					Where  isnumeric(ltrim(rtrim(branch))) <> 1
					And IsNull(ErrorCode,0) = 0
			End
			Else
			Begin
				Update #TBL_CSV_CONSEWBGEN_RECS
					Set branch = 0
					WHERE  IsNull(ErrorCode,0) = 0
			End

			--  Inserting Customer
		Begin Try	 		 
			insert into TBL_EXT_EWB_GENERATION_CONSOLIDATED (
					userGSTIN,
					vehicleno,
					fromplace,
					transmode,
					transdocno,
					transdocdate,
					fromstate,
					ewbno,
					rowstatus, sourcetype, referenceno, createdby, createddate, fileid,BranchId
					) 
			Select usergstin, vehicleno, fromplace, transmode, transdocno, transdocdate, fromstate, 
			Convert(bigint,Ltrim(Rtrim(IsNull(NullIf(ewbno,''),0)))), 
				1, @SourceType, @ReferenceNo, @UserId, getdate(), @FileId,isnull(try_convert(int,branch),NULL)
			From #TBL_CSV_CONSEWBGEN_RECS t1 
			Where IsNull(ErrorCode,0) = 0
		End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_CONSEWBGEN_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Eway bill Consolidated Data'
					From #TBL_CSV_CONSEWBGEN_RECS t1
					Where IsNull(ErrorCode,0) = 0
				End				
			End Catch
		

			Select @ProcessedRecordsCount = Count(*)
			From #TBL_CSV_CONSEWBGEN_RECS fv
						Where IsNull(fv.ErrorCode,0) = 0

			Select @ErrorRecordsCount = Count(*)
			From #TBL_CSV_CONSEWBGEN_RECS fv
			Where IsNull(fv.ErrorCode,0) <> 0

			Update TBL_EWB_RECVD_FILES
					Set filestatus = 0,
					totalrecordscount = @TotalRecordsCount,
					processedrecordscount = @ProcessedRecordsCount,
					errorrecordscount = @ErrorRecordsCount
			Where fileid = @FileId
	

			if exists (select 1 from #TBL_CSV_CONSEWBGEN_RECS where  IsNull(ErrorCode,0) = 0)
				Begin
					exec usp_Push_EWB_CONSOLIDATED_EXT_SA  @SourceType, @ReferenceNo
				End
				select * from #TBL_CSV_CONSEWBGEN_RECS where IsNull(ErrorCode,0) <> 0

				Select 'Total Record Count :' + convert(varchar(10), @TotalRecordsCount)
				Select 'Processed Record Count :' + convert(varchar(10), @ProcessedRecordsCount)
				Select 'Error Record Count :' + convert(varchar(10), @ErrorRecordsCount)
				
				
				--Delete from TBL_CSV_EWB_GEN_CONS_RECS where fileid = @FileId

		End 
	 Return 0


End