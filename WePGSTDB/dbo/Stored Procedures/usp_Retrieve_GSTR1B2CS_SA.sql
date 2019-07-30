
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2CS Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter
12/09/2017	Seshadri	Code Changes related to GSTR1 Template B 
12/12/2017	Seshadri	Commented the code changes related to GSTR1 Template B 

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1B2CS_SA '27AHQPA7588L1ZJ' ,'052016'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1B2CS_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY b2csid Desc) AS 'SNo',
		b2csid,
		pos,
		etin, 
		'' as inum,
		'' as idt,
		0 as val,
		rt,
		txval, 
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')

	/*
	Union

	Select
		ROW_NUMBER() OVER(ORDER BY t3.invid Desc) AS 'SNo',
		t3.invid as b2csid,
		pos,
		etin,
		inum,
		idt,
		val,
		rt,
		txval, 
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_B2CS_N t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_B2CS_INV t3 On t3.b2csid = t2.b2csid
	Inner Join TBL_GSTR1_B2CS_INV_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR1_B2CS_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'') */

	Order By t2.b2csid Desc

	Return 0

End