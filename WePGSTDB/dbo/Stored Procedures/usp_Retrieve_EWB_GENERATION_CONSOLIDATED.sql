

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve EWAYBILL Data
				
Written by  : nareshn@wepindia.com

Date		Who						Decription 
06/02/2018	Naresh Nuthalapati	 	Initial Version
18/05/2018	RAJA M					Branch ID Included
*/

/* Sample Procedure Call

exec usp_Retrieve_EWB_GENERATION_CONSOLIDATED 1,1

 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_EWB_GENERATION_CONSOLIDATED]
	@userGSTIN nvarchar(15),
	@FromDate nvarchar(20),
	@ToDate nvarchar(20),
	@CustId int,
	@BranchId int = NULL,
    @Mode varchar(5)	

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	IF @Mode = 'PD'
	BEGIN
		Select * From TBL_EWB_GEN_CONSOLIDATED 
		Where userGSTIN = @userGSTIN
		AND CustId = @CustId
		AND cEWBNo is not null
		AND cEWBDate is not null
		AND BranchId = @BranchId
		AND (CONVERT(DATETIME, cEWBDate, 105) 
			between CONVERT(DATETIME, @FromDate, 105) 
			And CONVERT(DATETIME, DATEADD(day, 2, CONVERT(DATETIME, @ToDate, 105)), 105))
		 Order By 1 Desc
	END
	ELSE IF @Mode = 'EG'
	BEGIN
	    Select * From TBL_EWB_GEN_CONSOLIDATED 
		Where userGSTIN = @userGSTIN
		AND CustId = @CustId
		AND BranchId = @BranchId
		--AND (CONVERT(DATETIME, docDate, 105) 
		--	between CONVERT(DATETIME, @FromDate, 105) 
		--	And CONVERT(DATETIME, @ToDate, 105)) 
		 Order By 1 Desc
	END
End