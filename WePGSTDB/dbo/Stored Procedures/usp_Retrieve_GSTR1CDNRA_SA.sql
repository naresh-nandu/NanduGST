
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 CDNRA Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/05/2018	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1CDNRA_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1CDNRA_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.cdnraid Desc) AS 'SNo',
		t2.cdnraid as cdnraid,
		t3.ntid as ntid,
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
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_CDNRA t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_CDNRA_NT t3 On t3.cdnraid = t2.cdnraid
	Inner Join TBL_GSTR1_CDNRA_NT_ITMS t4 On t4.ntid = t3.ntid
	Inner Join TBL_GSTR1_CDNRA_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t2.cdnraid Desc

	Return 0

End