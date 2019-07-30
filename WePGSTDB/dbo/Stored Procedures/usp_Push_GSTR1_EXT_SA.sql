
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
12/13/2017	Seshadri	Invoked the Push Procedure for B2CS Invoices
12/14/2017	Seshadri	Invoked the Push Procedure for HSN Summary	
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 
03/01/2018	Seshadri	Fixed Tax value updation issue


*/

/* Sample Procedure Call

exec usp_Push_GSTR1_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	exec usp_Push_GSTR1B2B_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1B2CL_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1B2CS_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId
	exec usp_Push_GSTR1B2CS_N_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1EXP_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1CDNR_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1CDNUR_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	-- exec usp_Push_GSTR1HSN_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1HSNSUM_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1NIL_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1TXP_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1AT_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1DOCISSUE_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy

	Return 0

End