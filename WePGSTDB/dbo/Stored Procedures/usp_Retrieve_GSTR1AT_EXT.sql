
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 AT Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1AT_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR1AT_EXT 'POS','POSDEV1','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1AT_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY atid Desc) AS 'SNo',
		atid,
		gstin,
		fp,
		pos,
		sply_ty,
		rt,
		ad_amt,
		iamt,
		camt,
		samt,
		csamt
	From TBL_EXT_GSTR1_AT  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By atid Desc

	Return 0

End