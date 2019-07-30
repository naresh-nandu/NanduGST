/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2CS Other Sources Records 
				from the corresponding external tables which have been uploaded to SA
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Karthik 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1B2CS_Others_EXT 'WEP001','27AHQPA7588L1ZJ','122017'

 */
 
Create PROCEDURE [usp_Retrieve_GSTR1B2CS_Others_EXT]
	@ReferenceNo varchar(50),  
	@GstinNo varchar(15),
	@Fp varchar(10)
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
		etin,
		inum,
		idt,
		val,
		rt ,
		txval ,
		iamt,
		camt,
		samt,
		csamt,
		sourcetype
	From TBL_EXT_GSTR1_B2CS  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And fp = @Fp	
	And referenceno = @ReferenceNo
	And rowstatus = 0
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''


	Return 0

End