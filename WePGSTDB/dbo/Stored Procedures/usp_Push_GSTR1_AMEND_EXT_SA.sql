
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 Amendment Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/13/2018	Seshadri		Initial Version


*/

/* Sample Procedure Call

exec usp_Push_GSTR1_AMEND_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1_AMEND_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	exec usp_Push_GSTR1B2BA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1B2CLA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy

	exec usp_Push_GSTR1B2CSA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy

	exec usp_Push_GSTR1EXPA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1CDNRA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1CDNURA_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1TXPA_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy
	exec usp_Push_GSTR1ATA_EXT_SA @SourceType, @ReferenceNo, @Gstin, @CustId, @CreatedBy

	Return 0

End