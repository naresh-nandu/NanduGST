

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External Eway Bill Records to the corresponding Staging
				Area tables
				
Written by  : Karthik.Kanniyappan@wepindia.com

Date		Who			Decription 
02/01/2018	Karthik			Initial Version

*/

/* Sample Procedure Call

exec usp_Push_EWB_EXT_SA  'ERP','WEP001'

 */

CREATE PROCEDURE [dbo].[usp_Push_EWB_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50)
	
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

		Declare @CreatedBy int,@CustId int,@RoleId int,@Email nvarchar(250)
		Select @CustId = Custid,@Email = email from tbl_customer where referenceno = @ReferenceNo and rowstatus =1
		Select @CreatedBy = Userid from  Userlist where email = @Email and  Custid = @CustId and rowstatus =1

				Select space(50) as ewbid,space(50) as itmsid, userGSTIN,
								supplyType, subSupplyType, docType, docNo, docDate, 
								fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode, 
								toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
								totalValue, cgstValue, sgstValue, igstValue, cessValue, totinvvalue, 
								transMode, transDistance, transporterId, transporterName, 
								transDocNo, transDocDate, vehicleNo, vehicleType, productName, productDesc, hsnCode, quantity, qtyUnit,taxableAmount, cgstRate, sgstRate, igstRate, cessRate, cessAdvol,
								rowstatus, sourcetype, referenceno, createddate, BranchId, APIBulkFlag, ReferenceId
				Into #TBL_EXT_EWB_GENERATION_Push
				From
				(
					SELECT userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
						fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode, 
						toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode, 
						totalValue, igstValue, cgstValue, sgstValue, cessValue, totinvvalue, 
						transMode, transDistance, transporterId, transporterName, 
						transDocNo, transDocDate, vehicleNo, vehicleType, productName, productDesc, hsnCode, quantity, qtyUnit, taxableAmount, cgstRate, sgstRate, igstRate, cessRate, cessAdvol,
						rowstatus, sourcetype, referenceno, createddate, BranchId, APIBulkFlag, ReferenceId
					FROM TBL_EXT_EWB_GENERATION
					Where  sourcetype = @SourceType
						And referenceno = @ReferenceNo
						And rowstatus = 1
						And Ltrim(Rtrim(IsNull(errormessage,''))) = ''		
				)t1

	-- Insert Records into Table TBL_EWB_GENERATION

					Insert TBL_EWB_GENERATION (userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
									fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode,
									toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode,
									totalValue, igstValue, cgstValue, sgstValue, cessValue, totinvvalue,
									transMode, transDistance, transporterId, transporterName, transDocNo, transDocDate, 
									vehicleNo, vehicleType, Custid, CreatedBy, CreatedDate, BranchId, APIBulkFlag, ReferenceId)
				Select	distinct userGSTIN, supplyType, subSupplyType, docType, docNo, docDate, 
								fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode,
								toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode, actToStateCode,
								totalValue, igstValue, cgstValue,sgstValue,  cessValue, totinvvalue,
								transMode, transDistance, transporterId, transporterName, transDocNo, transDocDate, 
								vehicleNo, vehicleType, isnull(@CustId,0), isnull(@CreatedBy,0), DATEADD (mi, 330, GETDATE()),
								BranchId, APIBulkFlag, ReferenceId
				From #TBL_EXT_EWB_GENERATION_Push t1
				Where Not Exists(Select 1 From TBL_EWB_GENERATION t2 Where t2.docNo = t1.docNo and t2.docDate = t1.docDate and t2.ewayBillNo IS NULL)

				Update #TBL_EXT_EWB_GENERATION_Push
				SET #TBL_EXT_EWB_GENERATION_Push.ewbid = t2.ewbid 
				FROM #TBL_EXT_EWB_GENERATION_Push t1,
						TBL_EWB_GENERATION t2 
				WHERE t1.docNo = t2.docNo 
				And t1.docDate = t2.docDate AND t2.ewayBillNo IS NULL
				--And t2.Status in ('1','ACT')

				-- Insert Records into Table TBL_EWB_GENERATION_ITMS

				Insert TBL_EWB_GENERATION_ITMS( ewbid, productName, productDesc, hsnCode, quantity, qtyUnit, taxableAmount, 
												igstRate, cgstRate, sgstRate, cessRate, cessAdvol, createdby, createddate, CustId) 
						Select	distinct ewbid,productName, productDesc, hsnCode, quantity, qtyUnit,taxableAmount, 
												igstRate,cgstRate, sgstRate, cessRate, cessAdvol, 1, DATEADD (mi, 330, GETDATE()), isnull(@CustId,0)
				From #TBL_EXT_EWB_GENERATION_Push t1
				Where Not Exists ( SELECT 1 FROM TBL_EWB_GENERATION_ITMS t2
								   Where t2.ewbid = t1.ewbid
								   And t2.productName = t1.productName
								   And t2.productDesc = t1.productDesc
								   And t2.hsnCode = t1.hsnCode)
								   --And t2.igstRate = t1.igstRate)


				If Exists(Select 1 From #TBL_EXT_EWB_GENERATION_Push )
				Begin
					Update TBL_EXT_EWB_GENERATION 
					SET rowstatus = 0 
					Where sourcetype = @SourceType
					And referenceno = @ReferenceNo
					And rowstatus = 1
					And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
				End 
		--Select * from #TBL_EXT_EWB_GENERATION_Push
				Return 0

End