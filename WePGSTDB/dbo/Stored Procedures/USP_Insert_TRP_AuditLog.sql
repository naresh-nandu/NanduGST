
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert TRP Audit Log
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec USP_Insert_TRP_AuditLog
*/
CREATE PROCEDURE [USP_Insert_TRP_AuditLog] 
	
	@FK_Audit_User_ID    int,
	@FK_Audit_Username    varchar(50),
	@Audit_Message    nvarchar(MAX),
	@Audit_Exception    nvarchar(MAX)

AS
BEGIN	
		insert into TBL_TRP_Audit_Log (FK_Audit_User_ID,
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