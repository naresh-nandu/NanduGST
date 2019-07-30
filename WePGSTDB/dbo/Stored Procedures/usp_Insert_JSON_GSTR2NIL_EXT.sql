
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 Nil JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
10/21/2017	Seshadri		Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
11/5/2017	Seshadri		Included Validation Framework


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2NIL_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2NIL_EXT]
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
			inter.cpddr as inter_cpddr,
			inter.exptdsply as inter_exptdsply,
			inter.ngsply as inter_ngsply,
			inter.nilsply as inter_nilsply,
			intra.cpddr as intra_cpddr,
			intra.exptdsply as intra_exptdsply,
			intra.ngsply as intra_ngsply,
			intra.nilsply as intra_nilsply,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_NIL
	From OPENJSON(@RecordContents) 
	WITH
	(	
		inter nvarchar(max) as JSON,
		intra nvarchar(max) as JSON
	) As Nil
	Cross Apply OPENJSON(Nil.inter) 
	WITH
	(
		cpddr decimal(18,2),
		exptdsply decimal(18,2),
		ngsply decimal(18,2),
		nilsply decimal(18,2)
	) As Inter
	Cross Apply OPENJSON(Nil.intra) 
	WITH
	(
		cpddr decimal(18,2),
		exptdsply decimal(18,2),
		ngsply decimal(18,2),
		nilsply decimal(18,2)
	) As Intra	

	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_NIL

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_NIL)
	Begin

		Update #TBL_EXT_GSTR2_NIL -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_NIL
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0
		*/

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Nil Rated Supply Amount - Inter'
		Where IsNull(Inter_Nilsply,0.00) <> 0.00   
		And dbo.udf_ValidateNilAmount(Inter_Nilsply) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Nil Rated Supply Amount - Intra'
		Where	IsNull(Intra_Nilsply,0.00) <> 0.00   
		And dbo.udf_ValidateNilAmount(Intra_Nilsply) <> 1 
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Value of Exempted Supplies Received - Inter '
		Where IsNull(Inter_Exptdsply,0.00) <> 0.00  
		And	dbo.udf_ValidateNilAmount(Inter_Exptdsply) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Value of Exempted Supplies Received - Intra '
		Where IsNull(Intra_Exptdsply,0.00) <> 0.00  
		And	dbo.udf_ValidateNilAmount(Intra_Exptdsply) <> 1
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Total Non GST Outward Supplies - Inter'
		Where IsNull(Inter_Ngsply,0.00) <> 0.00   
		And	dbo.udf_ValidateNilAmount(Inter_Ngsply) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Total Non GST Outward Supplies - Intra'
		Where IsNull(Intra_Ngsply,0.00) <> 0.00   
		And	dbo.udf_ValidateNilAmount(Intra_Ngsply) <> 1
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Value of supplies received from Compounding Dealer - Inter'
		Where IsNull(Inter_Cpddr,0.00) <> 0.00  
		And	dbo.udf_ValidateNilAmount(Inter_Cpddr) <> 1
		And IsNull(ErrorCode,0) = 0

		
		Update #TBL_EXT_GSTR2_NIL 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Value of supplies received from Compounding Dealer - Intra'
		Where IsNull(Intra_Cpddr,0.00) <> 0.00  
		And	dbo.udf_ValidateNilAmount(Intra_Cpddr) <> 1
		And IsNull(ErrorCode,0) = 0


		-- Insert the Records Into External Tables

		Begin Try

			Insert TBL_EXT_GSTR2_NIL
			(	gstin, fp,niltype,cpddr,exptdsply, ngsply,nilsply,   
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp,'Inter',inter_cpddr,inter_exptdsply, inter_ngsply,inter_nilsply, 
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_NIL t1
			Union All
			Select 
				gstin, fp,'Intra',intra_cpddr,intra_exptdsply, intra_ngsply,intra_nilsply, 
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_NIL t1
			Where IsNull(ErrorCode,0) = 0
	
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_NIL 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR2_NIL t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_NIL
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_NIL  
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_NIL
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode

End