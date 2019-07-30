-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to insert the Audit Log details
-- exec [Ins_AuditLog] 7,'NewCustomer','Login','NA'
-- =============================================
CREATE PROCEDURE [Ins_AuditLog] 
	
	@FK_Audit_User_ID    int,
	@FK_Audit_Username    varchar(50),
	@Audit_Message    nvarchar(MAX),
	@Audit_Exception    nvarchar(MAX)

AS
BEGIN	
		insert into TBL_Audit_Log (FK_Audit_User_ID,
							FK_Audit_Username,
							Audit_DateTime,
							Audit_Message,
							Audit_Exception
							) values(@FK_Audit_User_ID,
							@FK_Audit_Username,
							GETDATE(),
							@Audit_Message,
							@Audit_Exception
							)
			
	END