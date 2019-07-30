create PROCEDURE [dbo].[Delete_Eway_Record]
       @ewbid   INT        
AS 
BEGIN 
     SET NOCOUNT ON 

     DELETE  FROM   TBL_EWB_GENERATION
     WHERE  
     ewbid = @ewbid

END