


/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR6 CDNA Records from the corresponding external tables
				
Written by  : muskan.garg@wepdigital.com

Date		Who			Decription 
05/21/2018	Muskan  	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR6CDNA_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR6CDNA_EXT 'POS','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_GSTR6CDNA_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50), 
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY cdnaid desc) AS 'SNo',
		cdnaid,
		gstin,
		fp,
		ctin,
		nt_num,
		nt_dt,
		ntty,
		ont_num,
		ont_dt,	
		inum,
		idt,
		rt,
		txval,
		iamt,
		camt,
		samt,
		csamt
	From TBL_EXT_GSTR6_CDNA 
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By cdnaid Desc

	Return 0

End