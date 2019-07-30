CREATE PROCEDURE [Delete_Eway_Items]
       @itmsid    INT        
AS 
BEGIN 
     SET NOCOUNT ON 

     DELETE  FROM   dbo.TBL_EWB_GENERATION_ITMS
     WHERE  
     itmsid = @itmsid

END