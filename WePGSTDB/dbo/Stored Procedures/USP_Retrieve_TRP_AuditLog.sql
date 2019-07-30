
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve TRP Audit Log
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec USP_Retrieve_TRP_AuditLog 12, '11/28/2017','11/28/2017'
*/

CREATE PROCEDURE [USP_Retrieve_TRP_AuditLog]
	
	@CustId    int,
	@StartDate	Datetime,
	@EndDate	Datetime

AS
BEGIN	

	if @CustId=1
		Begin

			select * from TBL_TRP_Audit_Log where convert(varchar, Audit_DateTime, 110) between @StartDate and @EndDate Order by Audit_DateTime desc
		End
	Else
		Begin
			select * from TBL_TRP_Audit_Log where  convert(varchar, Audit_DateTime, 110) between @StartDate and @EndDate
			and FK_Audit_User_ID in (Select trpUserId from TBL_TRP_userlist where trpCustId = @CustId)
			Order by Audit_DateTime desc
		End
END