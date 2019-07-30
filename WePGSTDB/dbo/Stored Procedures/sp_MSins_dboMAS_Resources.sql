create procedure [sp_MSins_dboMAS_Resources]
    @c1 int,
    @c2 nvarchar(100),
    @c3 nvarchar(100),
    @c4 nvarchar(100),
    @c5 tinyint,
    @c6 int
as
begin  
	insert into [dbo].[MAS_Resources] (
		[Resource_ID],
		[Resource_Name],
		[Resource_Display_Name],
		[Resource_Page_Name],
		[Resource_MenuItem_ID],
		[FK_Parent_Resource_ID]
	) values (
		@c1,
		@c2,
		@c3,
		@c4,
		@c5,
		@c6	) 
end