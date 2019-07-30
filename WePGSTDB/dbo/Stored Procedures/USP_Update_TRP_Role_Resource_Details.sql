
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to UPDATE TRP Roles and Resources Details
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec USP_Update_TRP_Role_Resource_Details
*/
CREATE Procedure [USP_Update_TRP_Role_Resource_Details]
(
@roleid int,
@resource int=0

)
As
Begin

	if(@resource!=0)
		Begin
			Update TBL_TRP_Roles_Resources set Role_Resource_IsAssigned=1
			where FK_Role_Resource_Role_ID=@roleid and FK_Role_Resource_Resource_ID=@resource
		End
	Else
		Begin 
			Update TBL_TRP_Roles_Resources set Role_Resource_IsAssigned=0
			where FK_Role_Resource_Role_ID=@roleid
		End
End