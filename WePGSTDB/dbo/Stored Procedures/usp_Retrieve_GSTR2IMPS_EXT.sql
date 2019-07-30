
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 IMPS Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
09/12/2017	Seshadri	Added additional columns in the result set

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2IMPS_EXT 'BP','27AHQPA7588L1ZJ'
exec usp_Retrieve_GSTR2IMPS_EXT 'POS','27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_GSTR2IMPS_EXT]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50), 
	@GstinNo varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY impsid desc) AS 'SNo',
		impsid,
		gstin,
		fp,
		pos,
		inum,
		idt,
		ival,
		rt,
		txval,
		iamt,
		csamt,
		elg,
		tx_i,
		tx_cs,
		ReceivedBy,
		ReceivedDate
	From TBL_EXT_GSTR2_IMPS_INV 
	Where (Ltrim(Rtrim(IsNull(@GstinNo,''))) = '' or gstin = @GstinNo)
	And sourcetype = @SourceType
	And referenceno = @ReferenceNo
	And rowstatus = 1
	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	Order By impsid Desc

	Return 0

End