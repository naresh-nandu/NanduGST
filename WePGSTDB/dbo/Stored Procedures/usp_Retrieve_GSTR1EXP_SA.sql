
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 EXP Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter 


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1EXP_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1EXP_SA]  
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
		iamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
	Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.invid Desc

	Return 0

End