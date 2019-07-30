

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External Eway Bill Records to the corresponding Staging
				Area tables
				
Written by  : raja.m@wepindia.com

Date		Who			Decription 
25/03/2018	RAJA M		Initial Version

*/

/* Sample Procedure Call

exec usp_Push_EWB_UPDATE_VEHICLENO_EXT_SA  'CSV','WEP001'

 */

CREATE PROCEDURE [dbo].[usp_Push_EWB_UPDATE_VEHICLENO_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50)
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

				Select space(50) as consewbid,space(50) as tripsheetid, usergstin,
								ewbno, vehicleno, fromplace, fromstate, reasoncode, reasonremarks, 
								transMode, transDocNo, transDocDate, 
								rowstatus, sourcetype, referenceno, createddate
				Into #TBL_EXT_EWB_UPDATE_VEHICLENO_Push
				From
				(
					SELECT userGSTIN, EwbNo, vehicleNo, fromPlace, fromState, reasonCode, reasonremarks, transMode, transDocNo, transDocDate,
								rowstatus, sourcetype, referenceno, createddate
					FROM TBL_EXT_EWB_UPDATE_VEHICLENO
					Where  sourcetype = @SourceType
						And referenceno = @ReferenceNo
						And rowstatus = 1
						And Ltrim(Rtrim(IsNull(errormessage,''))) = ''		
				)t1

				Declare @CreatedBy int,@CustId int,@RoleId int,@Email nvarchar(250)
				Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus = 1
				Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus = 1

	-- Insert Records into Table TBL_EWB_GEN_CONSOLIDATED

				Insert TBL_EWB_UPDATE_VEHICLENO (userGSTIN, EwbNo, vehicleNo, fromPlace, fromState, reasonCode, reasonRem, 
							transMode, transDocNo, transDocDate, CustId, CreatedBy, CreatedDate)
				Select	distinct userGSTIN, EwbNo, vehicleNo, fromPlace, fromState, reasonCode, reasonremarks, 
							transMode, transDocNo, transDocDate, isnull(@CustId,0), isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE())
				From #TBL_EXT_EWB_UPDATE_VEHICLENO_Push t1
				Where Not Exists(Select 1 From TBL_EWB_UPDATE_VEHICLENO t2 Where t2.EwbNo = t1.ewbno and t2.vehicleNo = t1.vehicleno)

				--Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push
				--SET #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push.consewbid = t2.consewbid 
				--FROM #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push t1,
				--		TBL_EWB_GEN_CONSOLIDATED t2 
				--WHERE t1.transDocNo = t2.transDocNo 
				--And t1.transDocDate = t2.transDocDate 
				--And t2.Status = 1

				---- Insert Records into Table TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET

				--Insert TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET( consewbid, ewbNo, createdby, createddate) 
				--		Select	distinct consewbid, ewbNo, 1, DATEADD (mi, 330, GETDATE())
				--From #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push t1
				--Where Not Exists ( SELECT 1 FROM TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET t2
				--				   Where t2.consewbid = t1.consewbid
				--				   And t2.ewbNo = t1.ewbNo)


				If Exists(Select 1 From #TBL_EXT_EWB_UPDATE_VEHICLENO_Push)
				Begin
					Update TBL_EXT_EWB_UPDATE_VEHICLENO
					SET rowstatus = 0 
					Where sourcetype = @SourceType
					And referenceno = @ReferenceNo
					And rowstatus = 1
					And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
				End 
		
				Return 0

End