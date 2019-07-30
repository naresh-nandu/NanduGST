
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 EXPA Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/05/2018	Seshadri 	Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1EXPA_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1EXPA_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	    ROW_NUMBER() OVER(ORDER BY t3.invid Desc) AS 'SNo',
		t3.invid as invid,
		ex_tp,
		inum,
		idt,
		val,
		sbnum,
		sbdt,
		txval, 
		rt,
		iamt,
		oinum,
		oidt,
		diff_percent
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_EXPA t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_EXPA_INV t3 On t3.expaid = t2.expaid
	Inner Join TBL_GSTR1_EXPA_INV_ITMS t4 On t4.invid = t3.invid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.invid Desc

	Return 0

End