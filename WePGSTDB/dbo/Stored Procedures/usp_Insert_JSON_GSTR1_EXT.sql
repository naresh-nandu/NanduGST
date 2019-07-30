
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri	Initial Version
8/10/2017	Karthik		Added the output parameter @JsonResult
11/5/2017	Seshadri	Included Validation Framework
03/29/2018	Seshadri	Modified the code to support XML output

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_EXT  'POS',
 */

CREATE PROCEDURE usp_Insert_JSON_GSTR1_EXT
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@RecordContents nvarchar(max),
	@OutputType varchar(15) = Null,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@JsonResult nvarchar(max) = NULL OUT
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Gstin varchar(15),
			@Fp varchar(10),
			@Gt decimal(18,2),
			@Cur_gt decimal(18,2),
			@B2b nvarchar(max),
			@B2cl nvarchar(max),
			@B2cs nvarchar(max),
			@Cdnr nvarchar(max),
			@Exp nvarchar(max),
			@Hsn nvarchar(max),
			@Nil nvarchar(max),
			@Txpd nvarchar(max),
			@At nvarchar(max),
			@Doc_issue nvarchar(max),
			@Cdnur nvarchar(max),

			@TotalRecordsCount int = Null,
			@ProcessedRecordsCount int = Null,
			@ErrorRecordsCount int = Null,
			@ErrorRecords nvarchar(max) = Null



	CREATE TABLE #TBL_FinalOutput
	(
		ActionType varchar(15) NULL,
		TotalRecordsCount int  NULL,
		ProcessedRecordsCount int  NULL,
		ErrorRecordsCount int  NULL,
		ErrorRecords nvarchar(max) NULL,
	)

	Select	@Gstin = gstin,
			@Fp = fp,
			@Gt = gt,
			@Cur_Gt = cur_gt,
			@B2b = b2b,
			@B2cl = b2cl,
			@B2cs = b2cs,
			@Cdnr = cdnr,
			@Exp = [exp],
			@Hsn = hsn,
			@Nil = nil,
			@Txpd = txpd,
			@At = [at],
			@Doc_Issue = doc_issue,
			@Cdnur = cdnur
	From OPENJSON(@RecordContents) 
	WITH
	(	gstin varchar(15),
		fp varchar(10),
		gt decimal(18,2),
		cur_gt decimal(18,2),
		b2b nvarchar(max) as JSON,
		b2cl nvarchar(max) as JSON,
		b2cs nvarchar(max) as JSON,
		cdnr nvarchar(max) as JSON,
		[exp] nvarchar(max) as JSON,
		hsn nvarchar(max) as JSON,
		nil nvarchar(max) as JSON,
		txpd nvarchar(max) as JSON,
		[at] nvarchar(max) as JSON,
		doc_issue nvarchar(max) as JSON,
		cdnur nvarchar(max) as JSON
	)

	If Ltrim(Rtrim(IsNull(@B2b,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1B2B_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @B2b,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert Into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('B2B',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End


	If Ltrim(Rtrim(IsNull(@B2cl,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1B2CL_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @B2cl,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('B2CL',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)
	
	End
	
	If Ltrim(Rtrim(IsNull(@B2cs,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1B2CS_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @B2cs,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('B2CS',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Cdnr,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1CDNR_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Cdnr,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('CDNR',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Exp,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1EXP_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Exp,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('EXP',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)
	
	End

	If Ltrim(Rtrim(IsNull(@Hsn,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1HSN_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Hsn,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('HSN',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Nil,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1NIL_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Nil,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('NIL',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Txpd,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1TXP_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Txpd,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('TXPD',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@At,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1AT_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @At,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('AT',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Doc_Issue,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1DOCISSUE_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Doc_Issue,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('DOC_ISSUE',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Cdnur,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR1CDNUR_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Gt, @Cur_Gt, @Cdnur,@ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('CDNUR',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End
	
	if Ltrim(Rtrim(IsNull(@OutputType,'JSON'))) = 'JSON'
	Begin

		Select @JsonResult = 
				JSON_QUERY	((	Select ActionType,
								 (		Select
										TotalRecordsCount,ProcessedRecordscount,ErrorRecordsCount,
										JSON_QUERY	((ErrorRecords)) as ErrorRecords
										From #TBL_FinalOutput t1
										Where t1.ActionType = #TBL_FinalOutput.ActionType
										FOR JSON PATH
								) Details
								From #TBL_FinalOutput
								Group By ActionType
								FOR JSON PATH , ROOT('Actions')
							)) 

	End
	else if Ltrim(Rtrim(IsNull(@OutputType,'JSON'))) = 'XML'
	Begin

		Select @JsonResult = 
				JSON_QUERY	((	Select ActionType,
								 (		Select
										TotalRecordsCount,ProcessedRecordscount,ErrorRecordsCount,
										JSON_QUERY	((ErrorRecords)) as ErrorRecords
										From #TBL_FinalOutput t1
										Where t1.ActionType = #TBL_FinalOutput.ActionType
										FOR JSON PATH
								) Details
								From #TBL_FinalOutput
								Group By ActionType
								FOR XML AUTO , ROOT('Actions')
							)) 
	End
				

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End