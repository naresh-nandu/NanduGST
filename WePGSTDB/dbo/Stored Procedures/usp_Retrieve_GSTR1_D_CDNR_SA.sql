
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1_D CDNR Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri 	Initial Version
08/10/2017	Karthik		Introduced Aliases in the Result Set
08/22/2017	Seshadri	Removed the aliases in the result set
08/23/2017	Seshadri	Modified the Where Condition
08/30/2017	Seshadri	Modified the Where Condition to include the flag Value 0
05/22/2018	Karthik		Included nt_num and nt_dt


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_D_CDNR_SA


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1_D_CDNR_SA]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.cdnrid Desc) AS 'SNo',
		t2.cdnrid as cdnrid,
		ctin,
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
	Inner Join TBL_GSTR1_D_CDNR t2 On t2.gstr1did = t1.gstr1did
	Inner Join TBL_GSTR1_D_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
	Inner Join TBL_GSTR1_D_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
	Inner Join TBL_GSTR1_D_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @Gstin
	And fp = @Fp
	And(t3.flag is Null Or (t3.flag <> 'D' And t3.flag <> '0'))
	Order By t2.cdnrid Desc

	Return 0

End