
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 HSN JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
10/21/2017	Seshadri		Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
10/05/2017	Seshadri		Added the code to delete all existing invoices which are not uploaded
11/5/2017	Seshadri		Included Validation Framework


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2HSN_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2HSN_EXT]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out,
	@ErrorRecords nvarchar(max) = NULL out


  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,
			hsn_sc as hsn_sc,
			[desc] as [desc],
			uqc as uqc,
			qty as qty,
			val as val,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_HSN
	From OPENJSON(@RecordContents) 
	WITH
	(	
		det nvarchar(max) as JSON
	) As Hsn
	Cross Apply OPENJSON(Hsn.det) 
	WITH
	(
		num int,
		hsn_sc varchar(50),
		[desc] varchar(50),
		uqc varchar(50),
		qty decimal(18,2),
		val decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As HsnData
	
	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_HSN

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_HSN)
	Begin

		Update #TBL_EXT_GSTR2_HSN -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_HSN
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

	
		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Quantity'
		Where dbo.udf_ValidateQuantity(Qty) <> 1
		And IsNull(ErrorCode,0) = 0 
	
		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0


		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -100,
			ErrorMessage = 'HSN data already uploaded'
		From #TBL_EXT_GSTR2_HSN t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_HSN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And	hsn_sc = t1.hsn_sc
								And isnull(descs,'') = isnull(t1.[desc],'')
								And rowstatus = 0)

		Update #TBL_EXT_GSTR2_HSN 
		Set ErrorCode = -101,
			ErrorMessage = 'HSN data already imported'
		From #TBL_EXT_GSTR2_HSN t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From TBL_EXT_GSTR2_HSN  
								Where gstin = t1.gstin 
								And fp = t1.fp
								And	hsn_sc = t1.hsn_sc
								And isnull(descs,'') = isnull(t1.[desc],'')
								And rowstatus <> 0)

		-- Insert the Records Into External Tables

		Begin Try

			if Exists(Select 1 From #TBL_EXT_GSTR2_HSN
						 Where IsNull(ErrorCode,0) = -101)
			Begin

				Delete t1
				From TBL_EXT_GSTR2_HSN t1,
					 #TBL_EXT_GSTR2_HSN t2
				Where t1.gstin = t2.gstin
				And t1.fp = t2.fp
				And	t1.hsn_sc = t2.hsn_sc
				And isnull(t1.descs,'') = isnull(t2.[desc],'')
				And IsNull(ErrorCode,0) = -101

				Update #TBL_EXT_GSTR2_HSN 
				Set ErrorCode = Null,
					ErrorMessage = Null
				From #TBL_EXT_GSTR2_HSN t1
				Where IsNull(ErrorCode,0) = -101	

			End

			Insert TBL_EXT_GSTR2_HSN
			(	gstin, fp, hsn_sc, descs, uqc, qty, val,  
				txval, iamt, camt, samt, csamt,
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, hsn_sc, [desc], uqc, qty, val,  
				txval, iamt, camt, samt, csamt,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_HSN t1
 			Where IsNull(ErrorCode,0) = 0


		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_HSN 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in HSN Data'
				From #TBL_EXT_GSTR2_HSN t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_HSN
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_HSN  
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_HSN
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode


End