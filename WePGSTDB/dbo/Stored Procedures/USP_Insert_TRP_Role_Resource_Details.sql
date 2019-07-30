
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert TRP Roles and Resources Details
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec USP_Insert_TRP_Role_Resource_Details
*/
CREATE Procedure [USP_Insert_TRP_Role_Resource_Details]
(
@roleid int,
@resource int,
@status int
)
As
begin

Insert into TBL_TRP_Roles_Resources values (@roleid,@resource,@status)

End