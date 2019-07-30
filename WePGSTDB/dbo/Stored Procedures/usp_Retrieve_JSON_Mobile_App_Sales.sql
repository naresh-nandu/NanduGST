

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
10/04/2018	Raja	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_JSON_Mobile_App_Sales  '33GSPTN0802G1ZL','WEP001','09-04-2018','10-04-2018'

*/

CREATE PROCEDURE [dbo].[usp_Retrieve_JSON_Mobile_App_Sales]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@StartDate nvarchar(20),
	@EndDate nvarchar(20)
as 
Begin
	
	Select InvoiceDate, JSON_QUERY
				((
					Select ItemId, ItemShortName, ItemQty, Amount 
					From TBL_MOBILE_APP_SALES_ITMS t2
					Where t2.SalesId = t1.SalesId
					FOR JSON PATH
				)) As Itms	
	from TBL_MOBILE_APP_SALES t1
	where GSTIN = @Gstin
	And ReferenceNo = @ReferenceNo
	AND (CONVERT(DATETIME, InvoiceDate, 105) 
	between CONVERT(DATETIME, @StartDate, 105) 
	And CONVERT(DATETIME, @EndDate, 105)) FOR JSON AUTO
	
End