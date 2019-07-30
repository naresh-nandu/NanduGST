
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 CDNUR Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2CDNUR_SA '12AWJPN5651K2Z9','062017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2CDNUR_SA]  
	@GstinNo varchar(15),
	@fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select		
		ROW_NUMBER() OVER(ORDER BY t2.cdnurid desc) AS 'SNo',
		t2.cdnurid as cdnurid,
		rtin,
		nt_num,
		nt_dt,
		ntty,
		p_gst,
		rsn,
		val,
		inum,
		idt,
		inv_typ,
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
	Inner Join TBL_GSTR2_CDNUR t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
	Inner Join TBL_GSTR2_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
	Inner Join TBL_GSTR2_CDNUR_ITMS_ITC t5 On t5.itmsid = t3.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')
	Order By t2.cdnurid Desc
		
	Return 0

End