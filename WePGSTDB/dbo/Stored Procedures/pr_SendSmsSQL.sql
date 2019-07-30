CREATE procedure [pr_SendSmsSQL] --'HELLO',''
    
    @smstext as varchar(300), 
    @sResponse varchar(1000) OUT 
as 
BEGIN 
   DECLARE @MobileNo varchar(12)
   Declare @iReq int,@hr int 
   Declare @sUrl as varchar(500) 
   DECLARE @errorSource VARCHAR(8000)
   DECLARE @errorDescription VARCHAR(8000) 

   -- Create Object for XMLHTTP 
   EXEC @hr = sp_OACreate 'Microsoft.XMLHTTP', @iReq OUT 

   print @hr 

   if @hr <> 0 
      Raiserror('sp_OACreate Microsoft.XMLHTTP FAILED!', 16, 1) 

   --set @sUrl='http://api.clickatell.com/http/sendmsg?user=devendar&password=csx19csx&api_id=3360313&to=#MobNo#&text=#Msg#' 
   set @sUrl = 'http://www.unicel.in/SendSMS/smspost.php?uname=WePCare&pass=f7dc8b&send=WePCare&dest=#MobNo#&msg=#Msg#&type=1'
   Select @MobileNo=MobileNo from TBL_Users
   set @sUrl=REPLACE(@sUrl,'#MobNo#',@MobileNo) 
   set @sUrl=REPLACE(@sUrl,'#Msg#',@smstext) 
      
   print @sUrl 

   -- sms code start 
   EXEC @hr = sp_OAMethod @iReq, 'Open', NULL, 'GET', @sUrl, true 
   print @hr 

   if @hr <> 0 
      Raiserror('sp_OAMethod Open FAILED!', 16, 1) 

   EXEC @hr = sp_OAMethod @iReq, 'send' 
   select @iReq
   print @hr 

   if @hr <> 0 
   Begin 
       EXEC sp_OAGetErrorInfo @iReq, @errorSource OUTPUT, @errorDescription OUTPUT

       SELECT [Error Source] = @errorSource, [Description] = @errorDescription

       Raiserror('sp_OAMethod Send FAILED!', 16, 1) 
   end 
else 
Begin
    EXEC @hr = sp_OAGetProperty @iReq,'responseText', @sResponse OUT 
    print @hr

    insert into send_log (mobile, sendtext, response, created, createddate) 
    values(@MobileNo, @smstext, @sResponse, 'System', GETDATE())
end
end