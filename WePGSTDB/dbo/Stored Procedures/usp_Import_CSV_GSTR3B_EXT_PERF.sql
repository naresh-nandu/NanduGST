
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR3B Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/17/2017	Seshadri	Initial Version
12/11/2017	Karthik		Fixed Integration Testing defects
*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR3B_EXT_PERF

 */
 
CREATE PROCEDURE [usp_Import_CSV_GSTR3B_EXT_PERF]
	@FileName varchar(255),
	@UserId varchar(255),  
	@ReferenceNo varchar(50),
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1),
			@SourceType varchar(255),
			@FileId int

	Select @Delimiter = ','
	Select @SourceType = 'CSV'


	Select @FileId = fileid
	From TBL_RECVD_FILES
	Where fileName = @FileName
	And createdby = @UserId
	And gstrtypeid = 3
	And filestatus = 1

	Update TBL_RECVD_FILES
	Set filestatus = 2
	Where fileid = @FileId

	Create Table #TBL_CSV_GSTR3B_RECS
	(
		fileid int,
		slno varchar(50),
		gstin varchar(50),
		fp varchar(50),
		natureofsupplies varchar(100),
		supply_num varchar(50),
		pos varchar(50),
		txval varchar(50),
		iamt varchar(50),
		camt varchar(50),
		samt varchar(50),
		csamt varchar(50),
		interstatesupplies varchar(50) ,
	    intrastatesupplies varchar(50) ,
		errorcode smallint,
		errormessage varchar(255)
	 )

	 Begin Try

		Insert Into #TBL_CSV_GSTR3B_RECS
		Select t1.*
		From TBL_CSV_GSTR3B_RECS t1 
		Where t1.fileid = @FileId

	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch

	-- Processing Logic

	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_GSTR3B_RECS

	if exists (Select 1 from #TBL_CSV_GSTR3B_RECS)
	Begin

		Update #TBL_CSV_GSTR3B_RECS -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_CSV_GSTR3B_RECS -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #TBL_CSV_GSTR3B_RECS
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_CSV_GSTR3B_RECS
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_GSTR3B_RECS
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS 
		Set supply_num =
		Case natureofsupplies
			When '3.1-Outward taxable  supplies  (other than zero rated nil rated and exempted)' Then '1'
			When '3.1-Outward taxable supplies (zero rated)'  Then '2'
			When '3.1-Other outward supplies (Nil rated exempted)' Then '3'
			When '3.1-Inward supplies (liable to reverse charge)' Then '4'
			When '3.1-Non-GST outward supplies' Then '5' 
			When '3.2-Supplies made to Unregistered Persons' Then '6' 
			When '3.2-Supplies made to Composition Taxable Persons' Then '7'
			When '3.2-Supplies made to UIN holders' Then '8'
			When '4-ITC Available/Import of goods' Then '9'
			When '4-ITC Available/Import of services' Then '10'
			When '4-ITC Available/Inward supplies liable to reverse charge (other than 1 & 2 above)' Then '11'
			When '4-ITC Available/Inward supplies from ISD' Then '12'
			When '4-ITC Available/All other ITC' Then '13'
			When '4-ITC Reversed/As per Rules 42 & 43 of CGST/SGST rules' Then '14'
			When '4-ITC Reversed/Others' Then '15'
			When '4-Net ITC Available' Then '16' 
			When '4-Ineligible ITC/As per section 17(S)' Then '17'
			When '4-Ineligible ITC/Others' Then '18'
			When '5-From a supplier under composition scheme Exempt and Nil rated supply' Then '19'
			When '5-Non GST supply' Then '20'
			When '5.1-Interest late fee' Then '21'
			Else '0'
		End
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Nature of Supplies'
		Where Ltrim(Rtrim(IsNull(supply_num,''))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS 
		Set ErrorCode = -5,
			ErrorMessage = 'Place of Supply is mandatory'
		Where Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And supply_num in (6,7,8)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Place Of Supply'
		Where dbo.udf_ValidatePlaceOfSupply(Pos) <> 1
		And supply_num in (6,7,8)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And supply_num not in (9,10,11,12,13,14,15,16,17,18,19,20,21)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid IGST Amount'
		Where Ltrim(Rtrim(IsNull(iamt,''))) <> ''
		And dbo.udf_ValidateNilAmount(iamt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid CGST Amount'
		Where Ltrim(Rtrim(IsNull(camt,''))) <> ''
		And dbo.udf_ValidateNilAmount(camt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -10,
			ErrorMessage = 'Invalid SGST/UTGST Amount'
		Where Ltrim(Rtrim(IsNull(samt,''))) <> ''
		And dbo.udf_ValidateNilAmount(samt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -11,
			ErrorMessage = 'Invalid Cess Amount'
		Where Ltrim(Rtrim(IsNull(csamt,''))) <> ''
		And dbo.udf_ValidateNilAmount(csamt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Inter-State Supplies'
		Where Ltrim(Rtrim(IsNull(interstatesupplies,''))) <> ''
		And dbo.udf_ValidateNilAmount(interstatesupplies) <> 1 
		And supply_num in (19,20)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Intra-State Supplies'
		Where Ltrim(Rtrim(IsNull(intrastatesupplies,''))) <> ''
		And dbo.udf_ValidateNilAmount(intrastatesupplies) <> 1 
		And supply_num in (19,20)
		And IsNull(ErrorCode,0) = 0
	
		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR3B_RECS  t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From TBL_EXT_GSTR3B_DET 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And supply_num = t1.supply_num 
								And pos = t1.pos
								And rowstatus = 0)

		Update #TBL_CSV_GSTR3B_RECS  
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR3B_RECS t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From TBL_EXT_GSTR3B_DET 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And supply_num = t1.supply_num 
								And pos = t1.pos
								And rowstatus <> 0)

		-- Import GSTR3B Data

		if Exists (Select 1 From #TBL_CSV_GSTR3B_RECS )
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR3B_RECS
							 Where IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From TBL_EXT_GSTR3B_DET t1,
						 #TBL_CSV_GSTR3B_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And t1.supply_num = t2.supply_num 
					And t1.pos = t2.pos
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR3B_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR3B_RECS t1
					Where IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR3B_DET
				(	gstin, fp,
					natureofsupplies, supply_num, pos,  
					txval, iamt, camt, samt, csamt,
					interstatesupplies,intrastatesupplies,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp,
					natureofsupplies,
					Convert(int,Ltrim(Rtrim(IsNull(NullIf(supply_num,''),0)))),
					pos,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(interstatesupplies,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(intrastatesupplies,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId
				From #TBL_CSV_GSTR3B_RECS t1
				Where IsNull(ErrorCode,0) = 0

			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR3B_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR3B_RECS t1
					Where IsNull(ErrorCode,0) = 0
				End				
			End Catch
		End

	End 

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_CSV_GSTR3B_RECS
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_CSV_GSTR3B_RECS
	Where IsNull(ErrorCode,0) <> 0

	Update TBL_RECVD_FILES
	Set filestatus = 0,
		totalrecordscount = @TotalRecordsCount,
		processedrecordscount = @ProcessedRecordsCount,
		errorrecordscount = @ErrorRecordsCount
	Where fileid = @FileId

	Insert Into TBL_RECVD_FILE_ERRORS
	Select @FileId,t1.Slno,t1.ErrorCode,t1.ErrorMessage
	From  #TBL_CSV_GSTR3B_RECS t1
	Where IsNull(t1.ErrorCode,0) <> 0

	Select * from #TBL_CSV_GSTR3B_RECS
	Where IsNull(ErrorCode,0) <> 0

	Select distinct gstin from #TBL_CSV_GSTR3B_RECS
	Where IsNull(ErrorCode,0) = 0


	 -- Drop Temp Tables

	 Drop Table #TBL_CSV_GSTR3B_RECS
	 Delete From TBL_CSV_GSTR3B_RECS Where fileid = @FileId
	
	 Return 0


End