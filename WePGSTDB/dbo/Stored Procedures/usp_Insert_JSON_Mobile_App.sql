

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
28/04/2018	Raja		Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_Mobile_App '33GSPTN0802G1ZL','WEP001','POS',''
exec usp_Insert_JSON_Mobile_App '33GSPTN0802G1ZL','WEP001','BP','SALES','BP,WEP001,33GSPTN0802G1ZL,12/05/2018,12345,GHEE,100,10000'

 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_Mobile_App]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@SourceType varchar(50),
	@Type varchar(50) = NULL,
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null Out,
	@ErrorMessage varchar(255) = Null Out,
	@JsonResult nvarchar(max) = NULL OUT,
	@ErrorRecords nvarchar(max) = NULL Out
as 
Begin

	Set Nocount on

	IF @SourceType = 'POS'
	BEGIN

		Declare @SalesData nvarchar(max),
				@SalesBillData nvarchar(max),
				@PurchasesData nvarchar(max), 
				@PurchasesBillData nvarchar(max),
				@TransactionsData nvarchar(max),
				@TotalRecordsCount int, 
				@ProcessedRecordsCount int, 
				@ErrorRecordsCount int

		CREATE TABLE #TBL_FinalOutput
		(
			ActionType varchar(15) NULL,
			TotalRecordsCount int  NULL,
			ProcessedRecordsCount int  NULL,
			ErrorRecordsCount int  NULL
		)
	
		Select	@SalesData = SalesData,
				@SalesBillData = SalesBillData,
				@PurchasesData = PurchasesData,
				@PurchasesBillData = PurchasesBillData,
				@TransactionsData = TransactionsData
		From OPENJSON(@RecordContents) 	
		WITH
		(
			SalesData nvarchar(max) as JSON,
			SalesBillData nvarchar(max) as JSON,
			PurchasesData nvarchar(max) as JSON,
			PurchasesBillData nvarchar(max) as JSON,
			TransactionsData nvarchar(max) as JSON
		) 

		IF (Not Exists(Select 1 From TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType))
		BEGIN
			INSERT INTO TBL_MOBILE_APP(GSTIN, ReferenceNo, DeviceType, CreatedDate) 
				Values(@Gstin, @ReferenceNo, @SourceType, DATEADD (mi, 330, GETDATE())) 
		END
		ELSE
		BEGIN
			UPDATE TBL_MOBILE_APP SET CreatedDate = DATEADD (mi, 330, GETDATE())
				Where Exists(Select 1 From TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType)
		END

	
		If Ltrim(Rtrim(IsNull(@SalesData,''))) <> ''
		Begin		
			exec usp_Insert_JSON_Mobile_App_Sales  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		
			Insert Into #TBL_FinalOutput (ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount)
			Values('SALES', @TotalRecordsCount, @ProcessedRecordsCount, @ErrorRecordsCount)
		End

		If Ltrim(Rtrim(IsNull(@SalesBillData,''))) <> ''
		Begin
		
			exec usp_Insert_JSON_Mobile_App_Sales_Bill  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
			Insert Into #TBL_FinalOutput
			(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount)
			Values('SALES BILL', @TotalRecordsCount, @ProcessedRecordsCount, @ErrorRecordsCount)
		End

		If Ltrim(Rtrim(IsNull(@PurchasesData,''))) <> ''
		Begin
			exec usp_Insert_JSON_Mobile_App_Purchases  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
			Insert Into #TBL_FinalOutput
			(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount)
			Values('PURCHASES', @TotalRecordsCount, @ProcessedRecordsCount, @ErrorRecordsCount)
		End

		If Ltrim(Rtrim(IsNull(@PurchasesBillData,''))) <> ''
		Begin
			exec usp_Insert_JSON_Mobile_App_Purchases_Bill  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
			Insert Into #TBL_FinalOutput
			(ActionType,TotalRecordsCount ,ProcessedRecordsCount,ErrorRecordsCount)
			Values('PURCHASES BILL', @TotalRecordsCount, @ProcessedRecordsCount, @ErrorRecordsCount)
		End

		If Ltrim(Rtrim(IsNull(@TransactionsData,''))) <> ''
		Begin
			exec usp_Insert_JSON_Mobile_App_Transactions  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		
			Insert Into #TBL_FinalOutput (ActionType, TotalRecordsCount, ProcessedRecordsCount, ErrorRecordsCount)
			Values('TRANSACTIONS', @TotalRecordsCount, @ProcessedRecordsCount, @ErrorRecordsCount)
		End

		Select @JsonResult = 
					JSON_QUERY((Select ActionType,
							(Select
								TotalRecordsCount,ProcessedRecordscount,ErrorRecordsCount
								From #TBL_FinalOutput t1
								Where t1.ActionType = #TBL_FinalOutput.ActionType
								FOR JSON PATH
							) Details
							From #TBL_FinalOutput
							Group By ActionType
							FOR JSON PATH , ROOT('Actions')
						)) 
	END	
	IF @SourceType = 'BP'
	BEGIN

		IF (Not Exists(Select 1 From TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType))
		BEGIN
			INSERT INTO TBL_MOBILE_APP(GSTIN, ReferenceNo, DeviceType, CreatedDate) 
				Values(@Gstin, @ReferenceNo, @SourceType, DATEADD (mi, 330, GETDATE())) 
		END
		ELSE
		BEGIN
			UPDATE TBL_MOBILE_APP SET CreatedDate = DATEADD (mi, 330, GETDATE())
				Where Exists(Select 1 From TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType)
		END

		IF @Type = 'SALES'
		BEGIN
			exec usp_Insert_JSON_Mobile_App_Sales  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		END
		IF @Type = 'SALESBILL'
		BEGIN
			exec usp_Insert_JSON_Mobile_App_Sales_Bill  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		END
		IF @Type = 'PURCHASES'
		BEGIN
			exec usp_Insert_JSON_Mobile_App_Purchases  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		END
		IF @Type = 'PURCHASESBILL'
		BEGIN
			exec usp_Insert_JSON_Mobile_App_Purchases_Bill  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		END
		IF @Type = 'TRANSACTIONS'
		BEGIN
			exec usp_Insert_JSON_Mobile_App_Transactions  @GSTIN, @ReferenceNo, @SourceType, @RecordContents, @ErrorCode output, @ErrorMessage output, @TotalRecordsCount output, @ProcessedRecordsCount output, @ErrorRecordsCount Output
		END
	END
End