

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve EWAYBILL Data
				
Written by  : raja.m@wepdigital.com 

Date		Who			Decription 
15/05/2018	Raja M	 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_BULK_JSON_EWB_GENERATION 'MMSQjmKwDEmr1LBV7jyk+oDbHKJ8eIK/8xzMSR6T1F0=','WEP001',''

 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_BULK_JSON_EWB_GENERATION]
	@ReferenceId nvarchar(100),
	@ReferenceNo nvarchar(50),	
	@JsonResult nvarchar(max) = NULL OUT
as 
Begin

	Set Nocount on

	Declare @CustId int
	Select @CustId = CustId from TBL_Customer WHERE ReferenceNo = @ReferenceNo and rowstatus = 1
	
	Select @JsonResult = (Select status, ewayBillNo, ewayBillDate, validUpto, EWB_errorCodes as errorCodes, EWB_errorDescription as errorDescription, docNo 
	From TBL_EWB_GENERATION 
	Where ReferenceId = @ReferenceId
	AND APIBulkFlag = 1
	Order By 1 Desc
	FOR JSON AUTO)

	Select @JsonResult

	Delete FROM TBL_EWB_GENERATION WHERE APIBulkFlag = 2 and CustId = @CustId and ReferenceID = @ReferenceId
	
	--Select status, ewayBillNo, ewayBillDate, validUpto, EWB_errorCodes as errorCodes, EWB_errorDescription as errorDescription, docNo 
	--From TBL_EWB_GENERATION 
	--Where ReferenceId = @ReferenceId
	--AND APIBulkFlag = 1
	--Order By 1 Desc
	--FOR JSON AUTO
End