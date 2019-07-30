--Created By: Ganesh Patil
--Purpose : To insert role detail and resource details

create procedure [ins_Role_Resource_Details]
(
@roleid int,
@resource int,
@status int
)
as
begin

insert into MAS_Roles_Resources values (@roleid,@resource,@status)

end