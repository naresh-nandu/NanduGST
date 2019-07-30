--Created By: Ganesh Patil
--Purpose : To insert role detail and resource details

CREATE procedure [ins_Role_Details]
(
@name varchar(100),
@cust_id int,
@ret int output
)
as
begin

if Not Exists (select 1 from MAS_Roles where Upper(Role_Name)=Upper(@name) and CustomerID=@cust_id)
Begin
	insert into MAS_Roles values (@name,@cust_id)
	select @ret=SCOPE_IDENTITY()
	return @ret
end 
else
begin
	select @ret=0;
	return @ret
end
end