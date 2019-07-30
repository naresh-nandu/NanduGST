

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : raja.m@wepindia.com 

Date		Who			Decription 
01/03/2018	Raja		Initial Version
24/05/2018	Raja		UserGSTIN parameter included

*/

/* Sample Procedure Call

exec usp_Construct_JSON_CONS_EWB_Generation 'D12','30/03/2018'
 */

CREATE PROCEDURE [dbo].[usp_Construct_JSON_CONS_EWB_Generation] 
	@userGSTIN varchar(15),
	@TransDocNo varchar(15),
	@TransDocDate varchar(10),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
as 
Begin

	Set Nocount on


		Select TOP 1 fromPlace, fromState, vehicleNo,				
				transMode, TransDocNo, TransDocDate, 
			( Select ewbNo
				From TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET
				Where consewbid = TBL_EWB_GEN_CONSOLIDATED.consewbid
				FOR JSON PATH	
			) As tripSheetEwbBills
			From TBL_EWB_GEN_CONSOLIDATED
			Where TBL_EWB_GEN_CONSOLIDATED.userGSTIN = @userGSTIN
			AND TBL_EWB_GEN_CONSOLIDATED.TransDocNo = @TransDocNo
			AND TBL_EWB_GEN_CONSOLIDATED.TransDocDate = @TransDocDate	
			Order By consewbid desc			
			FOR JSON AUTO
			
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End