
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 ATA Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/03/2018	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1ATA_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR1ATA_EXT 'POS','POSDEV1','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1ATA_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY ataid Desc) AS 'SNo',
		ataid,
		gstin,
		fp,
		pos,
		sply_ty,
		rt,
		ad_amt,
		iamt,
		camt,
		samt,
		csamt,
		omon,
		diff_percent
	From TBL_EXT_GSTR1_ATA  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By ataid Desc

	Return 0

End