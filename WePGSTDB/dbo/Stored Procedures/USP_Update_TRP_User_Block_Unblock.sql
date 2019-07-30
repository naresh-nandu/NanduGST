/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Block/Unblock the TRP User
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version


*/

/* Sample Procedure Call  

exec USP_Update_TRP_User_Block_Unblock 17,1,1

 */
CREATE PROCEDURE [USP_Update_TRP_User_Block_Unblock]
	@TRPId    int,
	@TRPUserId	int,
	@Status	int,	
	@CreatedBy	int

AS
BEGIN
	
	update TBL_TRP_Userlist set Status=@Status,LastModifiedBy=@CreatedBy,LastModifiedDate=GETDATE() where trpCustId=@TRPId and trpUserId=@TRPUserId
		
End