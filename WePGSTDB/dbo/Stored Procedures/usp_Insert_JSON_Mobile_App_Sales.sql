

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
09/04/2018	Raja	Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_Mobile_App_Sales  '33GSPTN0802G1ZL','WEP001','POS','{ "SalesData" : {
  "InvoiceDate": "09-04-2018",
  "Itms": [
    {
      "ItemId": "12345",
	  "ItemShortName": "RICE",
	  "ItemQty": "10",
	  "Amount": "1000"
    }
  ]
}
}'
exec usp_Insert_JSON_Mobile_App_Sales '33GSPTN0802G1ZL','WEP001','BP','BP,WEP001,33GSPTN0802G1ZL,12/05/2018,12345,BUTTER,100,10000'

 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_Mobile_App_Sales]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@SourceType varchar(50),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
as 
Begin

	Set Nocount on
	Declare @CreatedBy int, @CustId int, @RoleId int, @MobileAppId int, @Email nvarchar(250)
	IF @SourceType = 'POS'
	BEGIN
		--Declare @TotalRecordsCount int,@ProcessedRecordsCount int,@ErrorRecordsCount int		
		Select @MobileAppId = MobileAppId from TBL_MOBILE_APP Where GSTIN = @Gstin and ReferenceNo = @ReferenceNo and DeviceType = @SourceType
		
		Select	space(50) as SalesId,
				space(50) as SalesSubId,
				@Gstin as Gstin,
				@ReferenceNo as ReferenceNo,
				InvoiceDate as InvoiceDate,			
				ItemId as ItemId,
				ItemShortName as ItemShortName,
				ItemQty as ItemQty,
				Amount as Amount,			
				space(255) as errormessage,
				space(10) as errorcode 
				Into #TBL_MOBILE_APP_SALES
		From OPENJSON(@RecordContents) 	
		WITH
		(
			SalesData nvarchar(max) as JSON
		) as Sales
		Cross Apply OPENJSON(Sales.SalesData)
		WITH
		(		
			InvoiceDate nvarchar(50),
			Itms nvarchar(max) as JSON		
		) as Inv
		Cross Apply OPENJSON(Inv.Itms) 
		WITH
		(
			ItemId nvarchar(50),
			ItemShortName nvarchar(50),
			ItemQty nvarchar(50),
			Amount nvarchar(50)
		) As Itms
		

		Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
		Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1
		--Select * from #TBL_MOBILE_APP_SALES

		Select @TotalRecordsCount = count(*) from #TBL_MOBILE_APP_SALES
		
	
		Begin Try
			Begin
				Insert TBL_MOBILE_APP_SALES	(Gstin, ReferenceNo, InvoiceDate, createddate, MobileAppId) 
					Select Distinct Gstin, ReferenceNo, CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105), 
					DATEADD (mi, 330, GETDATE()), @MobileAppId from #TBL_MOBILE_APP_SALES t1
					Where Not Exists(Select 1 From TBL_MOBILE_APP_SALES t2 Where t2.Gstin  = t1.Gstin and t2.ReferenceNo = t1.ReferenceNo
					 and t2.InvoiceDate = CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105))

					Update #TBL_MOBILE_APP_SALES
						SET #TBL_MOBILE_APP_SALES.SalesId = t2.SalesId 
						FROM #TBL_MOBILE_APP_SALES t1,
						TBL_MOBILE_APP_SALES t2 
						WHERE CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105) = t2.InvoiceDate 

					if exists (Select 1 FROM #TBL_MOBILE_APP_SALES t1,
						TBL_MOBILE_APP_SALES t2,
						TBL_MOBILE_APP_SALES_ITMS t3
						WHERE t1.ItemShortName = t3.ItemShortName
						And CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105)  = t2.InvoiceDate)
						Begin
							Delete t3 
								FROM #TBL_MOBILE_APP_SALES t1,
								TBL_MOBILE_APP_SALES t2,
								TBL_MOBILE_APP_SALES_ITMS t3
								WHERE t1.ItemShortName = t3.ItemShortName
								And CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105)  = t2.InvoiceDate
						End

							Insert TBL_MOBILE_APP_SALES_ITMS( SalesId, ItemId, ItemShortName, ItemQty, Amount) 
									Select	distinct SalesId, ItemId, UPPER(ItemShortName), ItemQty, Amount
							From #TBL_MOBILE_APP_SALES t1
							--Where Not Exists ( SELECT 1 FROM TBL_MOBILE_APP_SALES t2,
							--						TBL_MOBILE_APP_SALES_ITMS t3
							--						WHERE t1.ItemId=t3.ItemId
							--						And CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105)  = t2.InvoiceDate)
			End
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_MOBILE_APP_SALES
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Data'
					From #TBL_MOBILE_APP_SALES t1
					Where IsNull(ErrorCode,0) = 0
				End				
			End Catch

			Select * from #TBL_MOBILE_APP_SALES

			Select @ProcessedRecordsCount = Count(*)
				From #TBL_MOBILE_APP_SALES
				Where IsNull(ErrorCode,0) = 0

			Select @ErrorRecordsCount = Count(*)
			From #TBL_MOBILE_APP_SALES
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
			ItemId varchar(50),
			ItemShortName varchar(50),
			ItemQty varchar(50),
			Amount varchar(50),
			SalesId varchar(10), 
			SalesSubId varchar(10),
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
				ItemId,
				ItemShortName,
				ItemQty,
				Amount,
				SalesId,
				SalesSubId,
				errormessage,
				errorcode 
			)			
			Select 
				dbo.udf_SplitGSTRRecordData(RecordContents, 1, @Delimiter) as SourceType,
				dbo.udf_SplitGSTRRecordData(RecordContents, 2, @Delimiter) as ReferenceNo,
				dbo.udf_SplitGSTRRecordData(RecordContents, 3, @Delimiter) as GSTIN,
				dbo.udf_SplitGSTRRecordData(RecordContents, 5, @Delimiter) as InvoiceDate,
				dbo.udf_SplitGSTRRecordData(RecordContents, 6, @Delimiter) as ItemId,
				dbo.udf_SplitGSTRRecordData(RecordContents, 7, @Delimiter) as ItemShortName,
				dbo.udf_SplitGSTRRecordData(RecordContents, 8, @Delimiter) as ItemQty,
				dbo.udf_SplitGSTRRecordData(RecordContents, 9, @Delimiter) as Amount,
				'' as SalesId,'' as SalesSubId,'' as errormessage,'' as errorcode
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
			Insert TBL_MOBILE_APP_SALES	(Gstin, ReferenceNo, InvoiceDate, createddate, MobileAppId) 
				Select Distinct GSTIN, ReferenceNo, CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105), 
				DATEADD (mi, 330, GETDATE()), @MobileAppId from #MobileAppRecordFldValues t1
				Where Not Exists(Select 1 From TBL_MOBILE_APP_SALES t2 Where t2.Gstin  = t1.Gstin and t2.ReferenceNo = t1.ReferenceNo
				 and t2.InvoiceDate = CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105))

				Update #MobileAppRecordFldValues
					SET #MobileAppRecordFldValues.SalesId = t2.SalesId 
					FROM #MobileAppRecordFldValues t1,
					TBL_MOBILE_APP_SALES t2 
					WHERE CONVERT(VARCHAR(50), CONVERT(DATETIME, t1.InvoiceDate, 103), 105) = t2.InvoiceDate 

				Insert TBL_MOBILE_APP_SALES_ITMS( SalesId, ItemId, ItemShortName, ItemQty, Amount) 
						Select	distinct SalesId, ItemId, UPPER(ItemShortName), ItemQty, Amount
				From #MobileAppRecordFldValues t1
				--Where Not Exists ( SELECT 1 FROM TBL_MOBILE_APP_SALES_ITMS t2
				--				   Where t2.SalesId = t1.SalesId
				--				   And t2.ItemId = t1.ItemId)
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