﻿create procedure [sp_MSdel_dboMAS_Resources]
		@pkc1 int
as
begin  
	declare @primarykey_text nvarchar(100) = ''
	delete [dbo].[MAS_Resources] 
	where [Resource_ID] = @pkc1
if @@rowcount = 0
    if @@microsoftversion>0x07320000
		Begin
			
			set @primarykey_text = @primarykey_text + '[Resource_ID] = ' + convert(nvarchar(100),@pkc1,1)
			exec sp_MSreplraiserror @errorid=20598, @param1=N'[dbo].[MAS_Resources]', @param2=@primarykey_text, @param3=13234
		End
end