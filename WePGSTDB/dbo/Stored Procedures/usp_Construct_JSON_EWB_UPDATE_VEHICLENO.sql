

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
25/03/2018	Raja		Initial Version

*/

/* Sample Procedure Call

exec usp_Construct_JSON_EWB_UPDATE_VEHICLENO '301000988409','AP01DF1234'
 */

CREATE PROCEDURE [dbo].[usp_Construct_JSON_EWB_UPDATE_VEHICLENO] 
	--@UserGstin varchar(15),
	@EwbNo varchar(20),
	@VehicleNo varchar(20),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
as 
Begin

	Set Nocount on


		Select TOP 1 Cast([EwbNo] as bigint) as EwbNo, VehicleNo, FromPlace, Cast([FromState] as int) as FromState,
				ReasonCode, ReasonRem, TransDocNo, TransDocDate, TransMode
			From TBL_EWB_UPDATE_VEHICLENO 
			Where TBL_EWB_UPDATE_VEHICLENO.EwbNo = @EwbNo and TBL_EWB_UPDATE_VEHICLENO.VehicleNo = @VehicleNo
			Order By ewbVehUpdid DESC
			FOR JSON AUTO

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End