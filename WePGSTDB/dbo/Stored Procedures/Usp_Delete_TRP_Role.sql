

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete Roles
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Delete_Role 70

 */
 
Create PROCEDURE [Usp_Delete_TRP_Role]
	@RoleId int, 
	@RetVal int = NULL Out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	if exists (select 1 from TBL_TRP_userlist where roleid = @RoleId )
		begin
			print 'Users are there already'
			set @RetVal = -1
		End
	Else
		Begin
			 delete from TBL_TRP_Roles_Resources where FK_Role_Resource_Role_ID = @RoleId 
			 delete from TBL_TRP_Roles where role_id = @RoleId
			 print 'Users are not mapped, so can delete this role'
			 set @RetVal = 1
		End
	--print @RetVal
	select  @RetVal
  End