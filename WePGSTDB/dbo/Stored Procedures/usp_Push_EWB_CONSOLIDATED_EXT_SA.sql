

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External Eway Bill Records to the corresponding Staging
				Area tables
				
Written by  : Karthik.Kanniyappan@wepindia.com

Date		Who			Decription 
05/01/2018	RAJA M		Initial Version

*/

/* Sample Procedure Call

exec usp_Push_EWB_CONSOLIDATED_EXT_SA  'CSV','WEP001'

 */

CREATE PROCEDURE [dbo].[usp_Push_EWB_CONSOLIDATED_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50)
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

				Select space(50) as consewbid,space(50) as tripsheetid, userGSTIN,
								fromPlace, vehicleNo, transMode, transDocNo, transDocDate, fromState, ewbNo, 
								rowstatus, sourcetype, referenceno, createddate, BranchId, APIBulkFlag
				Into #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push
				From
				(
					SELECT userGSTIN, fromPlace, vehicleNo, transMode, transDocNo, transDocDate, fromState, ewbNo, 
						rowstatus, sourcetype, referenceno, createddate, BranchId, APIBulkFlag
						FROM TBL_EXT_EWB_GENERATION_CONSOLIDATED
						Where  sourcetype = @SourceType
							And referenceno = @ReferenceNo
							And rowstatus = 1
							And Ltrim(Rtrim(IsNull(errormessage,''))) = ''		
				)t1

				Declare @CreatedBy int,@CustId int,@RoleId int,@Email nvarchar(250)
				Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
				Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1

				-- Insert Records into Table TBL_EWB_GEN_CONSOLIDATED

				Insert TBL_EWB_GEN_CONSOLIDATED (userGSTIN, fromPlace, vehicleNo, transMode, transDocNo, transDocDate, fromState,
									createdby, createddate,custid, BranchId, APIBulkFlag)
				Select	distinct userGSTIN, fromPlace, vehicleNo, transMode, transDocNo, transDocDate, fromState, 
								isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE()),isnull(@CustId,0), BranchId, APIBulkFlag
				From #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push t1
				Where Not Exists(Select 1 From TBL_EWB_GEN_CONSOLIDATED t2 Where t2.transDocNo = t1.transDocNo and t2.transDocDate = t1.transDocDate)

				Update #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push
				SET #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push.consewbid = t2.consewbid 
				FROM #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push t1,
						TBL_EWB_GEN_CONSOLIDATED t2 
				WHERE t1.transDocNo = t2.transDocNo 
				And t1.transDocDate = t2.transDocDate 
				--And t2.Status = 1

				-- Insert Records into Table TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET

				Insert TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET( consewbid, ewbNo, createdby, createddate) 
						Select	distinct consewbid, ewbNo, 1, DATEADD (mi, 330, GETDATE())
				From #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push t1
				Where Not Exists ( SELECT 1 FROM TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET t2
								   Where t2.consewbid = t1.consewbid
								   And t2.ewbNo = t1.ewbNo)


				If Exists(Select 1 From #TBL_EXT_EWB_GENERATION_CONSOLIDATED_Push )
				Begin
					Update TBL_EXT_EWB_GENERATION_CONSOLIDATED
					SET rowstatus = 0 
					Where sourcetype = @SourceType
					And referenceno = @ReferenceNo
					And rowstatus = 1
					And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
				End 
		
				Return 0

End