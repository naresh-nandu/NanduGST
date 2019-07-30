---Created By : Ganesh Patil (27/07/2017)
---Purpose : To insert new resource in MAS_Roles_Resourses and assigning the same to all roles present in MAS_Roles
CREATE procedure [usp_insert_new_roles_resourses] --50
(
@ResourceId int
)
as
begin

	if exists(select 1 from MAS_Resources where Resource_ID=@ResourceId)
	begin
		declare @Roles table (index1 int identity(1,1),roleid int,rolename varchar(30))
		insert into @Roles select Role_ID,Role_Name from MAS_Roles
		declare @totalrecords int
		select  @totalrecords=count(*) from @Roles
		declare @i int
		set @i=1
			while @i<=@totalrecords
			begin
				declare @roleid int
				declare @rolename varchar(30)
				select @roleid=roleid,@rolename=rolename from @Roles where index1=@i
					if not exists(select 1 from MAS_Roles_Resources where FK_Role_Resource_Role_ID=@roleid and FK_Role_Resource_Resource_ID=@ResourceId)
					begin
					   
					   if (@rolename='Admin' or @rolename='Super Admin')
						insert into MAS_Roles_Resources values(@roleid,@ResourceId,1)
					   else
					    insert into MAS_Roles_Resources values(@roleid,@ResourceId,0)
					end
				set @i=@i+1
			end

	end

end

--select * from [dbo].[MAS_Resources] where  resource_name like '%hsn%'

--select  * from [dbo].[MAS_Roles]
 
--select  * from [dbo].[MAS_Roles_Resources] where FK_Role_Resource_Resource_ID=50