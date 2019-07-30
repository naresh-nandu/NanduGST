
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 CDNURA Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/05/2018	Seshadri 	Initial Version
 

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1CDNURA_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1CDNURA_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	    ROW_NUMBER() OVER(ORDER BY t2.cdnuraid Desc) AS 'SNo',
		t2.cdnuraid as cdnuraid,
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
	Inner Join TBL_GSTR1_CDNURA t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_CDNURA_ITMS t3 On t3.cdnuraid = t2.cdnuraid
	Inner Join TBL_GSTR1_CDNURA_ITMS_DET t4 On t4.itmsid = t3.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t2.cdnuraid Desc

	Return 0

End