
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
8/21/2017	Seshadri		Added the output parameter @JsonResult
11/5/2017	Seshadri	Included Validation Framework

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2_EXT]
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@JsonResult nvarchar(max) = NULL OUT

  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Gstin varchar(15),
			@Fp varchar(10),
			@B2b nvarchar(max),
			@B2bur nvarchar(max),
			@Cdn nvarchar(max),
			@Hsnsum nvarchar(max),
			@Imp_g nvarchar(max),
			@Imp_s nvarchar(max),
			@Nil_supplies nvarchar(max),
			@Txi nvarchar(max),
			@Txpd nvarchar(max),
			@Itc_rvsl nvarchar(max),
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
			@B2b = b2b,
			@B2bur = b2bur,
			@Cdn = cdn,
			@Hsnsum = hsnsum,
			@Imp_g = imp_g,
			@Imp_s = imp_s,
			@Nil_supplies = nil_supplies,
			@Txi = txi,
			@Txpd = txpd,
			@Itc_rvsl = itc_rvsl,
			@Cdnur = cdnur
	From OPENJSON(@RecordContents) 
	WITH
	(	gstin varchar(15),
		fp varchar(10),
		b2b nvarchar(max) as JSON,
		b2bur nvarchar(max) as JSON,
		cdn nvarchar(max) as JSON,
		hsnsum nvarchar(max) as JSON,
		imp_g nvarchar(max) as JSON,
		imp_s nvarchar(max) as JSON,
		nil_supplies nvarchar(max) as JSON,
		txi nvarchar(max) as JSON,
		txpd nvarchar(max) as JSON,
		itc_rvsl nvarchar(max) as JSON,
		cdnur nvarchar(max) as JSON
	)

	If Ltrim(Rtrim(IsNull(@B2b,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2B2B_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @B2b, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('B2B',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@B2bur,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2B2BUR_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @B2bur, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('B2BUR',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Cdn,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2CDN_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Cdn, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('CDN',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Hsnsum,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2HSN_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Hsnsum, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('HSNSUM',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End
	
	If Ltrim(Rtrim(IsNull(@Imp_g,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2IMPG_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Imp_g, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('IMPG',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End
	
	If Ltrim(Rtrim(IsNull(@Imp_s,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2IMPS_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Imp_s, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('IMPS',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Nil_supplies,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2NIL_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp,  @Nil_supplies, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('NIL',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	If Ltrim(Rtrim(IsNull(@Txi,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2TXI_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp,  @Txi, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('TXI',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End
	
	If Ltrim(Rtrim(IsNull(@Txpd,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2TXPD_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp,  @Txpd, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('TXPD',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

	/*
	If Ltrim(Rtrim(IsNull(@Itc_rvsl,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2ITCRVSL_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Itc_rvsl, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount)
		Values('ITCRVSL',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount)
	End */

	If Ltrim(Rtrim(IsNull(@Cdnur,''))) <> ''
	Begin

		exec usp_Insert_JSON_GSTR2CDNUR_EXT  @SourceType, @ReferenceNo, @Gstin, @Fp, @Cdnur, @ErrorCode output,@ErrorMessage output,@TotalRecordsCount output,@ProcessedRecordsCount output, @ErrorRecordsCount Output,@ErrorRecords Output
		Insert into #TBL_FinalOutput
		(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount,ErrorRecords)
		Values('CDNUR',@TotalRecordsCount,@ProcessedRecordsCount,@ErrorRecordsCount,@ErrorRecords)

	End

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

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End