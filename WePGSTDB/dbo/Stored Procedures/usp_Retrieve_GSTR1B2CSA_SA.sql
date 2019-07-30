
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2CSA Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/05/2018	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1B2CSA_SA '27AHQPA7588L1ZJ' ,'052016'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1B2CSA_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.b2csaid Desc) AS 'SNo',
		t2.b2csaid,
		pos,
		sply_ty,
		etin, 
		'' as inum,
		'' as idt,
		0 as val,
		rt,
		txval, 
		iamt,
		camt,
		samt,
		csamt,
		omon,
		diff_percent
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_B2CSA t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_B2CSA_ITMS t3 On t3.b2csaid = t2.b2csaid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')
	Order By t2.b2csaid Desc

	Return 0

End