create procedure [sp_MSupd_dboMAS_Resources]
		@c1 int = NULL,
		@c2 nvarchar(100) = NULL,
		@c3 nvarchar(100) = NULL,
		@c4 nvarchar(100) = NULL,
		@c5 tinyint = NULL,
		@c6 int = NULL,
		@pkc1 int = NULL,
		@bitmap binary(1)
as
begin  
	declare @primarykey_text nvarchar(100) = ''
if (substring(@bitmap,1,1) & 1 = 1)
begin 
update [dbo].[MAS_Resources] set
		[Resource_ID] = case substring(@bitmap,1,1) & 1 when 1 then @c1 else [Resource_ID] end,
		[Resource_Name] = case substring(@bitmap,1,1) & 2 when 2 then @c2 else [Resource_Name] end,
		[Resource_Display_Name] = case substring(@bitmap,1,1) & 4 when 4 then @c3 else [Resource_Display_Name] end,
		[Resource_Page_Name] = case substring(@bitmap,1,1) & 8 when 8 then @c4 else [Resource_Page_Name] end,
		[Resource_MenuItem_ID] = case substring(@bitmap,1,1) & 16 when 16 then @c5 else [Resource_MenuItem_ID] end,
		[FK_Parent_Resource_ID] = case substring(@bitmap,1,1) & 32 when 32 then @c6 else [FK_Parent_Resource_ID] end
	where [Resource_ID] = @pkc1
if @@rowcount = 0
    if @@microsoftversion>0x07320000
		Begin
			
			set @primarykey_text = @primarykey_text + '[Resource_ID] = ' + convert(nvarchar(100),@pkc1,1)
			exec sp_MSreplraiserror @errorid=20598, @param1=N'[dbo].[MAS_Resources]', @param2=@primarykey_text, @param3=13233
		End
end  
else
begin 
update [dbo].[MAS_Resources] set
		[Resource_Name] = case substring(@bitmap,1,1) & 2 when 2 then @c2 else [Resource_Name] end,
		[Resource_Display_Name] = case substring(@bitmap,1,1) & 4 when 4 then @c3 else [Resource_Display_Name] end,
		[Resource_Page_Name] = case substring(@bitmap,1,1) & 8 when 8 then @c4 else [Resource_Page_Name] end,
		[Resource_MenuItem_ID] = case substring(@bitmap,1,1) & 16 when 16 then @c5 else [Resource_MenuItem_ID] end,
		[FK_Parent_Resource_ID] = case substring(@bitmap,1,1) & 32 when 32 then @c6 else [FK_Parent_Resource_ID] end
	where [Resource_ID] = @pkc1
if @@rowcount = 0
    if @@microsoftversion>0x07320000
		Begin
			
			set @primarykey_text = @primarykey_text + '[Resource_ID] = ' + convert(nvarchar(100),@pkc1,1)
			exec sp_MSreplraiserror @errorid=20598, @param1=N'[dbo].[MAS_Resources]', @param2=@primarykey_text, @param3=13233
		End
end 
end