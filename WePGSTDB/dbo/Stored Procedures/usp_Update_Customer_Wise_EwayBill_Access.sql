
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the EMenu IDs for EWAY Bill Based oin the pAid customers
				
Written by  : Karthik.Kanniyappan@wepindia.com

Date		Who			Decription 
04/06/2018	Karthik			Initial Version

*/

/* Sample Procedure Call

usp_Update_Customer_Wise_EwayBill_Access 111

 */

CREATE PROCEDURE [dbo].[usp_Update_Customer_Wise_EwayBill_Access]
	@CustId int
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

		Declare @RoleId int
		Select @RoleId=roleid from userlist where custid =@Custid and 
		roleid in (select role_id from MAS_Roles where custid =@Custid and Role_Name like '%Admin%')
		insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned) 
		Select top(1) @RoleId,85,1 from MAS_Resources t1
		where Not Exists(Select 1 From MAS_Roles_Resources t2 Where t2.FK_Role_Resource_Role_ID = @RoleId 
							and t2.FK_Role_Resource_Resource_ID = 85)

		insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned) 
		Select @RoleId,Resource_ID,1 from MAS_Resources t1
		where Not Exists(Select 1 From MAS_Roles_Resources t2 Where t2.FK_Role_Resource_Role_ID = @RoleId 
							and t2.FK_Role_Resource_Resource_ID = t1.Resource_ID )
							And FK_Parent_Resource_ID = 85
		
	Return 0

End