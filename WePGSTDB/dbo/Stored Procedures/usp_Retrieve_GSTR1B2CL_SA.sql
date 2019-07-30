
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2CL Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter 


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1B2CL_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1B2CL_SA]  
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
		pos,
		inum,
		idt,
		val,
		rt,
		txval, 
		iamt,
		csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
	Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.invid Desc

	Return 0

End