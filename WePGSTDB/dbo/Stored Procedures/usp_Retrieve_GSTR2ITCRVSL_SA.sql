
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 ITCRVSL Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/22/2017	Seshadri 	Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2ITCRVSL_SA '27AHQPA7588L1ZJ','052017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2ITCRVSL_SA]  
	@GstinNo varchar(15),
	@fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.itcrvslid desc) AS 'SNo',
		t2.itcrvslid as itcrvslid,
		rulename,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR2 t1
	Inner Join TBL_GSTR2_ITCRVSL t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_ITCRVSL_ITMS t3 On t3.itcrvslid = t2.itcrvslid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')
	Order By t2.itcrvslid Desc
		
	Return 0

End