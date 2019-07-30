

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
01/03/2018	Raja		Initial Version

*/

/* Sample Procedure Call

exec usp_Construct_JSON_Update_Vehicle_No '12345','02-02-2018'
 */

CREATE PROCEDURE [usp_Construct_JSON_Update_Vehicle_No] 
	@TransDocNo varchar(15),
	@TransDocDate varchar(10),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
as 
Begin

	Set Nocount on


		Select EwbNo, vehicleNo, FromPlace, FromState, 
				ReasonCode, ReasonRem, TransDocNo, TransDocDate, TransMode
			From TBL_EWB_UPDATE_VEHICLENO
			Where TBL_EWB_UPDATE_VEHICLENO.TransDocNo = @TransDocNo
			AND TBL_EWB_UPDATE_VEHICLENO.TransDocDate = @TransDocDate				
			FOR JSON AUTO
			
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End