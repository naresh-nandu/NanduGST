
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert TRP ROle Details
				
Written by  : Karthik

Date		Who			Decription 
11/13/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec Usp_Insert_TRP_Role_Details Admin,1
 */


CREATE procedure [Usp_Insert_TRP_Role_Details]
(
@name varchar(100),
@TRPId int,
@ret int output
)
as
begin

if Not Exists (select 1 from TBL_TRP_Roles where Upper(Role_Name)=Upper(@name) and TRPId=@TRPId)
Begin
	insert into TBL_TRP_Roles values (@name,@TRPId)
	select @ret=SCOPE_IDENTITY()
	return @ret
end 
else
begin
	select @ret=0
	return @ret
end
end