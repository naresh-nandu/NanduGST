CREATE PROCEDURE [usp_Insert_CONSO_EWAYBILL_Add]	
  
   @Consewbid int,
	@ewbNo nvarchar(50)
	
		
	
	
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15)
				
	Select @SourceType = 'Manual'
	
	Insert into dbo.TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET
	( Consewbid,ewbNo
	 ) 
	Values
	(@Consewbid,@ewbNo)	


					
	Return 0

End

select * from TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET