
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 CDNUR Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
09/3/2017	Seshadri	Introduced ntty,nt_num,nt_dt in the result set

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1CDNUR_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR1CDNUR_EXT 'POS','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1CDNUR_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50), 
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	    ROW_NUMBER() OVER(ORDER BY cdnurid Desc) AS 'SNo',
		cdnurid,
		gstin,
		fp,
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
		csamt
	From TBL_EXT_GSTR1_CDNUR  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By cdnurid Desc

	Return 0

End