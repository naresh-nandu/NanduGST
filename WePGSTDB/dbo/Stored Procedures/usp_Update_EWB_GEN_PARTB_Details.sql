

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve EWAYBILL Data
				
Written by  : raja.m@wepdigital.com 

Date		Who						Decription 
18/05/2018	RAJA M					Initial Version

*/

/* Sample Procedure Call

exec usp_Update_EWB_GEN_PARTB_Details '05AAACC4214B1ZK','13/05/2018','18/05/2018',1,0,'ID'

 */
 
CREATE PROCEDURE [dbo].[usp_Update_EWB_GEN_PARTB_Details]
	@transMode int,
	@transporterId nvarchar(20),
	@transporterName nvarchar(50),
	@transDocNo nvarchar(20),
	@transDocDate nvarchar(20),
	@vehicleNo nvarchar(20),
	@vehicleType nvarchar(10),
	@CustId int,
	@EWBIds nvarchar(max)
as 
Begin

	Set Nocount on
	
	UPDATE TBL_EWB_GENERATION SET
		transMode = @transMode, transporterId = @transporterId, transporterName = @transporterName, transDocNo = @transDocNo,
		transDocDate = @transDocDate, vehicleNo = @vehicleNo, vehicleType = @vehicleType
		WHERE CustId = @CustId AND ewbid in (@EWBIds)
		
End