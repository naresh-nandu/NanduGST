CREATE PROCEDURE [usp_Update_Conso_EWAYBILL_GENERATION_MASTER]
@consewbid int,
	@fromPlace nvarchar(max),
	@fromState nvarchar(max),
	@vehicleNo nvarchar(50),
	@transMode int,
	@transDocNo nvarchar(50),
	@transDocDate nvarchar(50)

as 
Begin

	Set Nocount on
	Update TBL_EWB_GEN_CONSOLIDATED 

SET         
	fromPlace=@fromPlace ,
	fromState=@fromState ,
	vehicleNo=@vehicleNo ,
	transMode=@transMode ,
    transDocNo=	@transDocNo ,
    transDocDate=@transDocDate
	Where 
 consewbid=@consewbid
end