

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
09/04/2018	Raja	Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_Mobile_App_Sales_Bill  '33GSPTN0802G1ZL','WEP001','{
  "InvoiceDate": "09-04-2018",
  "Itms": [
    {
      "ItemId": "12345",
	  "ItemShortName": "RICE",
	  "ItemQty": "10",
	  "Amount": "1000"
    }
  ]
}'


 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_Mobile_App_Sales_Bill]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@SourceType varchar(50),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	--@JsonResult nvarchar(max) = NULL OUT,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
	--@ErrorRecords nvarchar(max) = NULL out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	Declare @CreatedBy int, @CustId int, @RoleId int, @MobileAppId int, @Email nvarchar(250)
	IF @SourceType = 'POS'
	BEGIN
		Select @MobileAppId = MobileAppId from TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType
		
		Select	space(50) as SalesBillId,
				space(50) as SalesBillSubId,
				@Gstin as Gstin,
				@ReferenceNo as ReferenceNo,
				InvoiceDate as InvoiceDate,
				InvoiceNo as InvoiceNo,
				TotalItems as TotalItems,
				BillAmount as BillAmount,
				CustId as CustId,
				CustName as CustName,
				space(255) as errormessage,
				space(10) as errorcode 
				Into #TBL_MOBILE_APP_SALES_BILL
		From OPENJSON(@RecordContents) 	
		WITH
		(
			SalesBillData nvarchar(max) as JSON
		) as SalesBill
		Cross Apply OPENJSON(SalesBill.SalesBillData)
		WITH
		(		
			InvoiceDate nvarchar(50),
			Inv nvarchar(max) as JSON		
		) as AllInv
		Cross Apply OPENJSON(AllInv.Inv) 
		WITH
		(
			InvoiceNo nvarchar(50),
			TotalItems nvarchar(50),
			BillAmount nvarchar(50),
			CustId nvarchar(50),
			CustName nvarchar(50)
		) As Inv
		
		Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
		Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1
		--Select * from #TBL_MOBILE_APP_SALES_BILL
		Select @TotalRecordsCount = count(*) from #TBL_MOBILE_APP_SALES_BILL

			
		Begin Try
			Begin
				Insert TBL_MOBILE_APP_SALES_BILL (Gstin, ReferenceNo, InvoiceDate, createddate, MobileAppId) 
					Select Distinct Gstin, ReferenceNo, CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105), 
						DATEADD (mi, 330, GETDATE()), @MobileAppId from #TBL_MOBILE_APP_SALES_BILL t1
						Where Not Exists(Select 1 From TBL_MOBILE_APP_SALES_BILL t2 Where t2.Gstin  = t1.Gstin and t2.ReferenceNo = t1.ReferenceNo
						 and t2.InvoiceDate = CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105))

					Update #TBL_MOBILE_APP_SALES_BILL
						SET #TBL_MOBILE_APP_SALES_BILL.SalesBillId = t2.SalesBillId 
						FROM #TBL_MOBILE_APP_SALES_BILL t1,
						TBL_MOBILE_APP_SALES_BILL t2 
						WHERE CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105) = t2.InvoiceDate 

					if exists (Select 1 FROM #TBL_MOBILE_APP_SALES_BILL t1,
						TBL_MOBILE_APP_SALES_BILL t2,
						TBL_MOBILE_APP_SALES_BILL_DET t3
						WHERE t1.InvoiceNo = t3.InvoiceNo
						And CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105)  = t2.InvoiceDate)
						Begin
							Delete t3 
								FROM #TBL_MOBILE_APP_SALES_BILL t1,
								TBL_MOBILE_APP_SALES_BILL t2,
								TBL_MOBILE_APP_SALES_BILL_DET t3
								WHERE t1.InvoiceNo = t3.InvoiceNo
								And CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105)  = t2.InvoiceDate
						End

					Insert TBL_MOBILE_APP_SALES_BILL_DET( SalesBillId, InvoiceNo, TotalItems, BillAmount, CustId, CustName) 
							Select	distinct SalesBillId, InvoiceNo, TotalItems, BillAmount, CustId, CustName
					From #TBL_MOBILE_APP_SALES_BILL t1
					--Where Not Exists ( SELECT 1 FROM TBL_MOBILE_APP_SALES_BILL_DET t2
					--				   Where t2.SalesBillId = t1.SalesBillId
					--				   And t2.InvoiceNo = t1.InvoiceNo)
			End
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_MOBILE_APP_SALES_BILL
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Eway Bill Consolidated Data'
					From #TBL_MOBILE_APP_SALES_BILL t1
					Where IsNull(ErrorCode,0) = 0
				End				
			End Catch

			Select * from #TBL_MOBILE_APP_SALES_BILL

			Select @ProcessedRecordsCount = Count(*)
			From #TBL_MOBILE_APP_SALES_BILL
			Where IsNull(ErrorCode,0) = 0

			Select @ErrorRecordsCount = Count(*)
			From #TBL_MOBILE_APP_SALES_BILL
			Where IsNull(ErrorCode,0) <> 0

		Select @ErrorCode = 1, @ErrorMessage = 'Success'
	END
	IF @SourceType = 'BP'
	BEGIN
	
		Declare @Delimiter char(1)
		Select @MobileAppId = MobileAppId from TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType
		
		Create Table #MobileAppRecordContents
		( 
			RecordContents nvarchar(max)
		)
		Create Table #MobileAppRecordFldValues
		(
			SourceType varchar(50),
			ReferenceNo varchar(50),
			GSTIN varchar(50),
			InvoiceDate varchar(50),
			InvoiceNo varchar(50),
			TotalItems varchar(50),
			BillAmount varchar(50),
			CustId varchar(50),
			CustName varchar(10), 
			SalesBillId varchar(10),
			SalesBillSubId varchar(10),
			errormessage varchar(max),
			errorcode varchar(10)
		 )

		Select @Delimiter = ','

		Begin Try

		Insert Into #MobileAppRecordContents
		Select @RecordContents

		If @SourceType = 'BP'
		Begin

			Insert Into #MobileAppRecordFldValues
			(	
				SourceType,
				ReferenceNo,
				GSTIN,
				InvoiceDate,
				InvoiceNo,
				TotalItems,
				BillAmount,
				CustId,
				CustName,
				SalesBillId,
				SalesBillSubId,
				errormessage,
				errorcode 
			)			
			Select 
				dbo.udf_SplitGSTRRecordData(RecordContents, 1, @Delimiter) as SourceType,
				dbo.udf_SplitGSTRRecordData(RecordContents, 2, @Delimiter) as ReferenceNo,
				dbo.udf_SplitGSTRRecordData(RecordContents, 3, @Delimiter) as GSTIN,
				dbo.udf_SplitGSTRRecordData(RecordContents, 5, @Delimiter) as InvoiceDate,
				dbo.udf_SplitGSTRRecordData(RecordContents, 6, @Delimiter) as InvoiceNo,
				dbo.udf_SplitGSTRRecordData(RecordContents, 7, @Delimiter) as TotalItems,
				dbo.udf_SplitGSTRRecordData(RecordContents, 8, @Delimiter) as BillAmount,
				dbo.udf_SplitGSTRRecordData(RecordContents, 9, @Delimiter) as CustId,
				dbo.udf_SplitGSTRRecordData(RecordContents, 10, @Delimiter) as CustName,
				'' as SalesBillId,'' as SalesBillSubId,'' as errormessage,'' as errorcode
			From #MobileAppRecordContents
		End
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Select	@ErrorCode = -1,
						@ErrorMessage = ERROR_MESSAGE()
				Return @ErrorCode
			End
		End Catch

		Begin Try
		Begin
			Insert TBL_MOBILE_APP_SALES_BILL(Gstin, ReferenceNo, InvoiceDate, createddate, MobileAppId) 
				Select Distinct GSTIN, ReferenceNo, CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105), 
				DATEADD (mi, 330, GETDATE()), @MobileAppId from #MobileAppRecordFldValues t1
				Where Not Exists(Select 1 From TBL_MOBILE_APP_SALES_BILL t2 Where t2.Gstin  = t1.Gstin and t2.ReferenceNo = t1.ReferenceNo
				 and t2.InvoiceDate = CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105))

				Update #MobileAppRecordFldValues
					SET #MobileAppRecordFldValues.SalesBillId = t2.SalesBillId 
					FROM #MobileAppRecordFldValues t1,
					TBL_MOBILE_APP_SALES_BILL t2 
					WHERE CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105) = t2.InvoiceDate 

				Insert TBL_MOBILE_APP_SALES_BILL_DET(SalesBillId, InvoiceNo, TotalItems, BillAmount, CustId, CustName) 
						Select	distinct SalesBillId, InvoiceNo, TotalItems, BillAmount, CustId, CustName
				From #MobileAppRecordFldValues t1
		End
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #MobileAppRecordFldValues
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Data'
				From #MobileAppRecordFldValues t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch
	END
End