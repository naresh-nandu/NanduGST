Create Procedure [sp_TestSendMail]           
(            
  @SenderAddress varchar(100)=null,            
  @RecipientAddress varchar(1000),            
  @Subject varchar(200),            
  @Body varchar(max),            
  @CC varchar(1000)=null,            
  @BCC varchar(1000)=null            
)            
AS       
    
Declare @oMail int --Object reference            
Declare @resultcode int            
if @SenderAddress is null or @SenderAddress=''            
begin            
 Set @SenderAddress='karthik.kanniyappan@wepindia.com'            
end            


EXEC @resultcode = sp_OACreate 'CDO.Message', @oMail OUT            
IF @resultcode = 0            
BEGIN            
   EXEC @resultcode = sp_OASetProperty @oMail, 'BodyFormat', 0            
   EXEC @resultcode = sp_OASetProperty @oMail, 'MailFormat', 0            
   EXEC @resultcode = sp_OASetProperty @oMail, 'Importance', 1            
   EXEC @resultcode = sp_OASetProperty @oMail, 'From',@SenderAddress            
   EXEC @resultcode = sp_OASetProperty @oMail, 'To',@RecipientAddress            
   EXEC @resultcode = sp_OASetProperty @oMail, 'Subject',@Subject            
   EXEC @resultcode = sp_OASetProperty @oMail, 'HTMLBody', @Body            
   EXEC @resultcode = sp_OASetProperty @oMail, 'MailFormat', 0             
   EXEC @resultcode = sp_OASetProperty @oMail, 'CC', @CC             
   EXEC @resultcode = sp_OASetProperty @oMail, 'BCC', @BCC             
   EXEC @resultcode = sp_OAMethod @oMail, 'Send', NULL            
   EXEC sp_OADestroy @oMail        
END