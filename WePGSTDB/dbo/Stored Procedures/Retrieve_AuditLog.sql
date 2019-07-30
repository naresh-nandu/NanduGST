
-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Retrieve the Audit Log details
-- exec [Retrieve_AuditLog] 1,'05/08/2017','05/08/2017'
-- =============================================
CREATE PROCEDURE [Retrieve_AuditLog] 
	
	@CustId    int,
	@StartDate	Datetime,
	@EndDate	Datetime

AS
BEGIN	

	if @CustId=1
		Begin

			select * from TBL_Audit_Log where convert(varchar, Audit_DateTime, 110) between @StartDate and @EndDate Order by Audit_DateTime desc
		End
	Else
		Begin
			select * from TBL_Audit_Log where  convert(varchar, Audit_DateTime, 110) between @StartDate and @EndDate
			and FK_Audit_User_ID in (Select UserId from userlist where CustId = @CustId)
			Order by Audit_DateTime desc
		End
END