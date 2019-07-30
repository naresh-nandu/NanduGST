

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
10/04/2018	Raja	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_JSON_Mobile_App_Sales_Bill  '33GSPTN0802G1ZL','WEP001','09-04-2018','10-04-2018'

*/

CREATE PROCEDURE [dbo].[usp_Retrieve_JSON_Mobile_App_Sales_Bill]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@StartDate nvarchar(20),
	@EndDate nvarchar(20)
as 
Begin
	
	Select InvoiceDate, JSON_QUERY
				((
					Select InvoiceNo, TotalItems, BillAmount, CustId, CustName 
					From TBL_MOBILE_APP_SALES_BILL_DET t2
					Where t2.SalesBillId = t1.SalesBillId
					FOR JSON PATH
				)) As Inv	
	from TBL_MOBILE_APP_SALES_BILL t1
	where GSTIN = @Gstin
	And ReferenceNo = @ReferenceNo
	AND (CONVERT(DATETIME, InvoiceDate, 105) 
	between CONVERT(DATETIME, @StartDate, 105) 
	And CONVERT(DATETIME, @EndDate, 105)) FOR JSON AUTO
	
End