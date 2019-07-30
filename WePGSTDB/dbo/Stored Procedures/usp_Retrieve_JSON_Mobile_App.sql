

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
28/04/2018	Raja		Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_JSON_Mobile_App  '33GSPTN0802G1ZL','WEP001','POS','09/04/2018','27/04/2018'

*/

CREATE PROCEDURE [dbo].[usp_Retrieve_JSON_Mobile_App]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@SourceType varchar(50),
	@StartDate nvarchar(20),
	@EndDate nvarchar(20)
as 
Begin
	
	Select FORMAT(CAST(CreatedDate AS DATETIME),'dd/MM/yyyy hh:mm tt') as LastUpdated,
		JSON_QUERY((
			Select TOP 10 ItemShortName as ItemName, SUM(CONVERT(DECIMAL, ItemQty)) as Qty 
			From TBL_MOBILE_APP_SALES t1
			INNER JOIN TBL_MOBILE_APP_SALES_ITMS t2 On t1.SalesId = t2.SalesId
			Where GSTIN = @Gstin
			And ReferenceNo = @ReferenceNo 
			And t1.MobileAppId = t3.MobileAppId
			And (CONVERT(DATETIME, InvoiceDate, 105) 
			between CONVERT(DATETIME, @StartDate, 105) AND CONVERT(DATETIME, @EndDate, 105))
			GROUP By ItemShortName 
			Order by Qty DESC 
			FOR JSON AUTO
		)) As Sales,
				
		JSON_QUERY((
			Select TOP 10 ItemShortName as ItemName, SUM(CONVERT(DECIMAL, ItemQty)) as Qty 
			From TBL_MOBILE_APP_PURCHASES t1
			INNER JOIN TBL_MOBILE_APP_PURCHASES_ITMS t2 On t1.PurchasesId = t2.PurchasesId
			Where GSTIN = @Gstin
			And ReferenceNo = @ReferenceNo 
			And t1.MobileAppId = t3.MobileAppId
			And (CONVERT(DATETIME, InvoiceDate, 105) 
			between CONVERT(DATETIME, @StartDate, 105) AND CONVERT(DATETIME, @EndDate, 105))
			GROUP By ItemShortName 
			Order by Qty DESC 
			FOR JSON AUTO
		)) As Purchases,

		JSON_QUERY((
			Select Count(InvoiceNo) as NoofBills, SUM(CONVERT(DECIMAL, BillAmount)) as TotalBillAmount 
			From TBL_MOBILE_APP_SALES_BILL t1
			INNER JOIN TBL_MOBILE_APP_SALES_BILL_DET t2 On t1.SalesBillId = t2.SalesBillId
			Where GSTIN = @Gstin
			And ReferenceNo = @ReferenceNo 
			And t1.MobileAppId = t3.MobileAppId
			And (CONVERT(DATETIME, InvoiceDate, 105) 
			between CONVERT(DATETIME, @StartDate, 105) AND CONVERT(DATETIME, @EndDate, 105))
			Order by TotalBillAmount DESC 
			FOR JSON AUTO
		)) As SalesBill,

		JSON_QUERY((
			Select Count(InvoiceNo) as NoofBills, Count(DISTINCT SupplierId) as TotalSuppliers, SUM(CONVERT(DECIMAL, BillAmount)) as TotalBillAmount
			From TBL_MOBILE_APP_PURCHASES_BILL t1
			INNER JOIN TBL_MOBILE_APP_PURCHASES_BILL_DET t2 On t1.PurchasesBillId = t2.PurchasesBillId
			Where GSTIN = @Gstin
			And ReferenceNo = @ReferenceNo 
			And t1.MobileAppId = t3.MobileAppId
			And (CONVERT(DATETIME, InvoiceDate, 105) 
			between CONVERT(DATETIME, @StartDate, 105) AND CONVERT(DATETIME, @EndDate, 105))
			Order by TotalBillAmount DESC 
			FOR JSON AUTO
		)) As PurchasesBill,
				
		JSON_QUERY((
			Select PaymentModeName, SUM(CONVERT(DECIMAL, Amount)) as Amount
			From TBL_MOBILE_APP_TRANSACTIONS t1
			INNER JOIN TBL_MOBILE_APP_TRANSACTIONS_DET t2 On t1.TransactionsId = t2.TransactionsId
			Where GSTIN = @Gstin
			And ReferenceNo = @ReferenceNo 
			And t1.MobileAppId = t3.MobileAppId
			And (CONVERT(DATETIME, InvoiceDate, 105) 
			between CONVERT(DATETIME, @StartDate, 105) AND CONVERT(DATETIME, @EndDate, 105))
			Group By PaymentModeName
			Order by Amount DESC 
			FOR JSON AUTO
		)) As Transactions

	from TBL_MOBILE_APP t3
	where GSTIN = @Gstin
	And ReferenceNo = @ReferenceNo
	And DeviceType = @SourceType
	FOR JSON AUTO
End