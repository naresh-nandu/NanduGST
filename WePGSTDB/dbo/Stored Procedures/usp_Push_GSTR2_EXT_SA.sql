
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
07/28/2017	Seshadri		Fixed the issue in GSTR2 - NIL Push functionality
09/12/2017	Seshadri	Fine tuned the code
05/10/2018  Muskan      Added Custid,Createdby in SA Table.
*/

/* Sample Procedure Call

exec usp_Push_GSTR2_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	exec usp_Push_GSTR2B2B_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2B2BUR_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2IMPG_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2IMPS_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2CDN_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2CDNUR_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2HSN_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2NIL_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2TXPD_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2TXI_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR2ITCRVSL_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy

	Return 0

End