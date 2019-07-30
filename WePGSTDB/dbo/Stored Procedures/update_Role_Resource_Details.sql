--Created By: Ganesh Patil
--Purpose : To update role detail and resource details

CREATE procedure [update_Role_Resource_Details]
(
@roleid int,
@resource int=0

)
as
begin

	if(@resource!=0)
		begin
			update MAS_Roles_Resources set Role_Resource_IsAssigned=1
			where FK_Role_Resource_Role_ID=@roleid and FK_Role_Resource_Resource_ID=@resource
		end
	else
		begin 
		update MAS_Roles_Resources set Role_Resource_IsAssigned=0
			where FK_Role_Resource_Role_ID=@roleid
		end
end