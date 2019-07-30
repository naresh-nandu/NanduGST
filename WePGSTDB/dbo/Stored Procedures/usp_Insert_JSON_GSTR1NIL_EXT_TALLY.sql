
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 Nil JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
10/8/2017	Karthik			Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
11/5/2017	Seshadri		Included Validation Framework

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1NIL_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1NIL_EXT_TALLY]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@Gt decimal(18,2),
	@Cur_Gt decimal(18,2),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
	--@ErrorRecords nvarchar(max) = NULL out

  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,
			@Gt as gt,
			@Cur_Gt as cur_gt,
			nil_amt as nil_amt,
			expt_amt as expt_amt,
			ngsup_amt as ngsup_amt,
			sply_ty as sply_ty,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR1_NIL
	From OPENJSON(@RecordContents) 
	WITH
	(	
		inv nvarchar(max) as JSON
	) As Nil
	Cross Apply OPENJSON(Nil.inv) 
	WITH
	(
		nil_amt decimal(18,2),
		expt_amt decimal(18,2),
		ngsup_amt decimal(18,2),
		sply_ty varchar(25)
	) As Inv
	
	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR1_NIL

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR1_NIL)
	Begin

		Update #TBL_EXT_GSTR1_NIL -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR1_NIL
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Nil Supply Type'
		Where dbo.udf_ValidateNilSupplyType(Sply_Ty) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Total Nil rated outward supplies'
		Where dbo.udf_ValidateNilAmount(Nil_Amt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Total Exempted outward supplies'
		Where dbo.udf_ValidateNilAmount(Expt_Amt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_NIL 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Total Non GST outward supplies'
		Where dbo.udf_ValidateNilAmount(Ngsup_Amt) <> 1
		And IsNull(ErrorCode,0) = 0

		-- Insert the Records Into External Tables

		Begin Try

			Insert TBL_EXT_GSTR1_NIL
			(	gstin, fp, gt, cur_gt, nil_amt, expt_amt, ngsup_amt, sply_ty,   
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, gt, cur_gt, nil_amt, expt_amt, ngsup_amt, sply_ty, 
				1 ,@SourceType ,@ReferenceNo,GetDate()
			 From #TBL_EXT_GSTR1_NIL t1
			 Where IsNull(ErrorCode,0) = 0

		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR1_NIL 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR1_NIL t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_NIL
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_NIL  
	Where IsNull(ErrorCode,0) <> 0

	If @ErrorRecordsCount > 0
	Begin
		Select 
		sply_ty as [Description],	
		nil_amt as [Nil Rated Supplies],	
		expt_amt as [Exempted (other than nil rated/non GST supply )],	
		ngsup_amt as [Non-GST supplies],
		ErrorCode,
		ErrorMessage
		From #TBL_EXT_GSTR1_NIL  Where IsNull(ErrorCode,0) <> 0
	End
	Else
	Begin
		Select 
		sply_ty as [Description],	
		nil_amt as [Nil Rated Supplies],	
		expt_amt as [Exempted (other than nil rated/non GST supply )],	
		ngsup_amt as [Non-GST supplies],
		ErrorCode,
		ErrorMessage
		From #TBL_EXT_GSTR1_NIL  Where IsNull(ErrorCode,0) <> 0
	End
						

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode

	
End