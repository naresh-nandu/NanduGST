/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version


*/

/* Sample Procedure Call  

exec usp_Insert_TRP_New_Roles_Resourses 1

 */
Create procedure [usp_Insert_TRP_New_Roles_Resourses]
(
@ResourceId int
)
as
begin

	if exists(select 1 from TBL_TRP_Resources where Resource_ID=@ResourceId)
	begin
		declare @Roles table (index1 int identity(1,1),roleid int,rolename varchar(30))
		insert into @Roles select Role_ID,Role_Name from TBL_TRP_Roles
		declare @totalrecords int
		select  @totalrecords=count(*) from @Roles
		declare @i int
		set @i=1
			while @i<=@totalrecords
			begin
				declare @roleid int
				declare @rolename varchar(30)
				select @roleid=roleid,@rolename=rolename from @Roles where index1=@i
					if not exists(select 1 from TBL_TRP_Roles_Resources where FK_Role_Resource_Role_ID=@roleid and FK_Role_Resource_Resource_ID=@ResourceId)
					begin
					   
					   if (@rolename='Admin' or @rolename='Super Admin')
						insert into TBL_TRP_Roles_Resources values(@roleid,@ResourceId,1)
					   else
					    insert into TBL_TRP_Roles_Resources values(@roleid,@ResourceId,0)
					end
				set @i=@i+1
			end

	end

end