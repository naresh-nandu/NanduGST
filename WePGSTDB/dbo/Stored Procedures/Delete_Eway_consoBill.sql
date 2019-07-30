create PROCEDURE [Delete_Eway_consoBill]
       @tripsheetid    INT        
AS 
BEGIN 
     SET NOCOUNT ON 

     DELETE  FROM   dbo.TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET
     WHERE  
     tripsheetid = @tripsheetid

END
select * from TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET