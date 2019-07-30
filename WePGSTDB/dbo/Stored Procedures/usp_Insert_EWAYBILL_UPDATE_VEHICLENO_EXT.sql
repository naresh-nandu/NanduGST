CREATE PROCEDURE [dbo].[usp_Insert_EWAYBILL_UPDATE_VEHICLENO_EXT]	
	@userGSTIN nvarchar(15),
	@EwbNo nvarchar(15),
	@vehicleNo nvarchar(15),
	@fromPlace nvarchar(50),
	@fromState nvarchar(2),
	@reasonCode nvarchar(50),
	@reasonRemarks nvarchar(50),
	@transMode int,
	@transDocNo nvarchar(15),
	@transDocDate nvarchar(20),	
	@ReferenceNo varchar(50),
	@createdby int
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @SourceType varchar(15)
				
	Select @SourceType = 'Manual'
	
	Insert into TBL_EXT_EWB_UPDATE_VEHICLENO (userGSTIN, ewbno, vehicleno, fromplace, fromstate, 
			reasoncode, reasonremarks, transmode, transdocno, transdocdate, 
			rowstatus, sourcetype, referenceno, createdby, createddate) 
	Values (@userGSTIN, @EwbNo, @vehicleNo, @fromPlace, @fromState, 
			@reasonCode, @reasonRemarks, @transMode, @transDocNo, @transDocDate, 
			1, @SourceType, @ReferenceNo, @createdby, DATEADD (mi, 330, GETDATE()))	
					
	exec usp_Push_EWB_UPDATE_VEHICLENO_EXT_SA  @SourceType, @ReferenceNo

	Return 0

End