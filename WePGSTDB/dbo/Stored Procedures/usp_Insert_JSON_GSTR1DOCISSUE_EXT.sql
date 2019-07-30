
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 DOC_Issue JSON Records to the corresponding external tables
				
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

exec usp_Insert_JSON_GSTR1DOCISSUE_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1DOCISSUE_EXT]
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
	@ErrorRecordsCount int = Null out,
	@ErrorRecords nvarchar(max) = NULL out

  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,
			@Gt as gt,
			@Cur_Gt as cur_gt,
			doc_num as doc_num,
			doc_typ as doc_typ,
			num as num,
			[from] as [from],
			[to] as [to],
			totnum as totnum,
			cancel as cancel,
			net_issue as net_issue,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR1_DOC
	From OPENJSON(@RecordContents) 
	WITH
	(
		doc_det nvarchar(max) as JSON
	) As Doc_Issue
	Cross Apply OPENJSON(Doc_Issue.doc_det) 
	WITH
	(	
		doc_num int,
		doc_typ varchar(50),
		docs nvarchar(max) as JSON
	) As Doc_Det
	Cross Apply OPENJSON(Doc_Det.docs) 
	WITH
	(
		num int,
		[from] varchar(50),
		[to] varchar(50),
		totnum int,
		cancel int,
		net_issue int
	) As Docs
	
	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR1_DOC

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR1_DOC)
	Begin

		Update #TBL_EXT_GSTR1_DOC -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR1_DOC
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_EXT_GSTR1_DOC 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR1_DOC 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR1_DOC 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_DOC 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Document Number'
		Where IsNull(doc_num,0) Not In (1,2,3,4,5,6,7,8,9,10,11,12)
		And IsNull(ErrorCode,0) = 0

		-- Insert the Records Into External Tables

		Begin Try

			Insert TBL_EXT_GSTR1_DOC
			(	gstin, fp, gt, cur_gt, doc_num, doc_typ, num, froms, tos, totnum, cancel, net_issue,  
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, gt, cur_gt, doc_num, doc_typ, num, [from], [to], totnum, cancel, net_issue, 
				1 ,@SourceType ,@ReferenceNo,GetDate()
			 From #TBL_EXT_GSTR1_DOC t1
			 Where IsNull(ErrorCode,0) = 0

		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR1_DOC 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR1_DOC t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_DOC
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_DOC  
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR1_DOC
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode


End