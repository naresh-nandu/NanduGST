
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1_D CDNUR Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/23/2017	Seshadri 	Initial Version
08/30/2017	Seshadri	Modified the Where Condition to include the flag Value 0
05/22/2018	Karthik		Included nt_num and nt_dt


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_D_CDNUR_SA '27AHQPA7588L1ZJ','072017'

 */
 
Create PROCEDURE [usp_Retrieve_GSTR1_D_CDNUR_SA]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	    ROW_NUMBER() OVER(ORDER BY t2.cdnurid Desc) AS 'SNo',
		t2.cdnurid as cdnurid,
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
	From TBL_GSTR1_D t1
	Inner Join TBL_GSTR1_D_CDNUR t2 On t2.gstr1did = t1.gstr1did
	Inner Join TBL_GSTR1_D_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
	Inner Join TBL_GSTR1_D_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
	Where gstin = @Gstin
	And fp = @Fp
	And(t2.flag is Null Or (t2.flag <> 'D' And t2.flag <> '0'))
	Order By t2.cdnurid Desc

	Return 0

End