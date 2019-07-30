
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_EXT 'POS','POSDEV1','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR3B_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY detid Desc) AS 'SNo',
		detid,
		gstin,
		fp,
		natureofsupplies,
		supply_num,
		pos,
		txval,
		iamt,
		camt,
		samt,
		csamt,
		interstatesupplies,
		intrastatesupplies
	From TBL_EXT_GSTR3B_DET  
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By detid Desc

	Return 0

End