

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 JSON Records to the corresponding external tables
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
10/04/2018	Raja	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_JSON_Mobile_App_Purchases  '33GSPTN0802G1ZL','WEP001','09-04-2018','10-04-2018'


 */

CREATE PROCEDURE [dbo].[usp_Retrieve_JSON_Mobile_App_Purchases]
	@Gstin varchar(15), -- ERP 
	@ReferenceNo varchar(50),
	@StartDate varchar(20),
	@EndDate varchar(20)
as 
Begin

	Select InvoiceDate, JSON_QUERY
				((
					Select ItemId, ItemShortName, ItemQty, Amount 
					From TBL_MOBILE_APP_PURCHASES_ITMS t2
					Where t2.PurchasesId = t1.PurchasesId
					FOR JSON PATH
				)) As Itms	
	from TBL_MOBILE_APP_PURCHASES t1
	where GSTIN = @Gstin
	And ReferenceNo = @ReferenceNo
	AND (CONVERT(DATETIME, InvoiceDate, 105) 
	between CONVERT(DATETIME, @StartDate, 105) 
	And CONVERT(DATETIME, @EndDate, 105)) FOR JSON AUTO

End