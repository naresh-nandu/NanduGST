

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR6 Records to the corresponding Staging
				Area tables
				
Written by  : muskan.garg@wepdigital.com 

Date		Who			Decription 
05/21/2018	Muskan		Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR6_EXT_SA  'CSV','WEP001', '33GSPTN0801G1ZM',1,1

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR6_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	exec usp_Push_GSTR6B2B_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR6CDN_EXT_SA @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR6B2BA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	exec usp_Push_GSTR6CDNA_EXT_SA  @SourceType, @ReferenceNo, @Gstin, @Custid,@CreatedBy
	Return 0

End