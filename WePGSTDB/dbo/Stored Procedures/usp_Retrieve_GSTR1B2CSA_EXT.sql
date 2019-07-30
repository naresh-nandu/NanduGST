
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2CSA Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/03/2018	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1B2CSA_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR1B2CSA_EXT 'POS','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1B2CSA_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),  
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'SNo',
		invid,
		gstin ,
		fp,
		pos,
		sply_ty,
		etin,
		'' as inum,
		'' as idt,
		0 as val,
		rt ,
		txval ,
		iamt,
		camt,
		samt,
		csamt,
		omon,
		diff_percent
	From TBL_EXT_GSTR1_B2CSA  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	
	Union 
	
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'SNo',
		invid,
		gstin ,
		fp,
		pos,
		sply_ty,
		etin,
		inum,
		idt,
		val,
		rt,
		txval, 
		iamt,
		camt,
		samt,
		csamt,
		omon,
		diff_percent
	From TBL_EXT_GSTR1_B2CSA_INV  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''

	Order By invid Desc	

	

	Return 0

End