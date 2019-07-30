
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 CDNRA Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/3/2018	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1CDNRA_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR1CDNRA_EXT 'POS','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1CDNRA_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY cdnraid Desc) AS 'SNo',
		cdnraid,
		gstin,
		fp,
		ctin,
		ntty,
		nt_num,
		nt_dt,
		inum,
		idt,
		val,
		rt,
		txval,
		iamt,
		camt,
		samt,
		csamt,
		ont_num,
		ont_dt,
		diff_percent
	From TBL_EXT_GSTR1_CDNRA  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By cdnraid Desc

	Return 0

End