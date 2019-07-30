
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 CDNUR Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter
09/3/2017	Seshadri	Introduced ntty,nt_num,nt_dt in the result set
 

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1CDNUR_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1CDNUR_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	    ROW_NUMBER() OVER(ORDER BY t2.cdnurid Desc) AS 'SNo',
		t2.cdnurid as cdnurid,
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
		csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
	Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t2.cdnurid Desc

	Return 0

End