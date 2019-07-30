
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 B2BUR Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter
10/31/2017	Seshadri	Fixed B2BUR delete issue


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2B2BUR_SA '27AHQPA7588L1ZJ','052017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2B2BUR_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.invid desc) AS 'SNo',
		t3.invid as invid,
		t2.b2burid as b2burid, 
		pos,
		sply_ty,
		inum,
		idt,
		val,
		rt,
		txval,
		iamt,
		camt,
		samt,
		csamt, 
		elg,
		tx_i,
		tx_c,
		tx_s,
		tx_cs
	From TBL_GSTR2 t1
	Inner Join TBL_GSTR2_B2BUR t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_B2BUR_INV t3 On t3.b2burid = t2.b2burid
	Inner Join TBL_GSTR2_B2BUR_INV_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR2_B2BUR_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Inner Join TBL_GSTR2_B2BUR_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')
	Order By t3.invid Desc

		
	Return 0

End