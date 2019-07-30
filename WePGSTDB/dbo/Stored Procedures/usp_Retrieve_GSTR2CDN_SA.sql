
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 CDN Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter
10/25/2017	Seshadri	Fixed the Flag Issue
10/26/2017	Seshadri	Introduced flag column in the result set

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2CDN_SA '27AHQPA7588L1ZJ'



 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2CDN_SA]  
	@GstinNo varchar(15),
	@fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.cdnrid desc) AS 'SNo',
		t2.cdnrid as cdnrid,
		ctin,
		nt_num,
		nt_dt,
		ntty,
		p_gst,
		rsn,
		val,
		inum,
		idt,
		flag,
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
	Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
	Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR2_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t6 On t6.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And (	IsNull(flag,'') = IsNull(@Flag,'')
		Or
		IsNull(flag,'') In ('A','R','P','M')
	)
	Order By t2.cdnrid Desc
		
	Return 0

End