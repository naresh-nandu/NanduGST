
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 B2B Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter
09/21/2017	Seshadri	Fixed the retrieval issue because of Other Flag Parameters
10/17/2017	Seshadri	Changed the result set columns 
10/25/2017	Seshadri	Fixed the Flag Issue
10/26/2017	Seshadri	Introduced flag column in the result set
10/27/2017	Seshadri	Added the output parameters

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2B2B_SA '27AHQPA7588L1ZJ','052017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2B2B_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null,
	@TotalInvCnt int = Null out,
	@TotalInvAmt dec(18,2) = Null out,
	@TotalAcceptedInvCnt int = Null out,
	@TotalAcceptedInvAmt dec(18,2) = Null out,
	@TotalPendingInvCnt int = Null out,
	@TotalPendingInvAmt dec(18,2) = Null out,
	@TotalRejectedInvCnt int = Null out,
	@TotalRejectedInvAmt dec(18,2) = Null out


-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.invid desc) AS 'SNo',
		t3.invid as invid,
		ctin,
		rchrg,
		pos,
		inv_typ,
		inum,
		idt,
		val,
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
	Into #TBL_GSTR2_B2B
	From TBL_GSTR2 t1
	Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
	Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid
	Where gstin = @GstinNo
	And fp = @Fp
	And (	IsNull(flag,'') = IsNull(@Flag,'')
			Or
			IsNull(flag,'') In ('A','R','P','M')
		)
	Order By t3.invid Desc

	Select inum,idt,val,flag,count(*) as cnt
	Into #TBL_GSTR2_B2B_CNT
	From #TBL_GSTR2_B2B
	group by inum,idt,val,flag

	Select	@TotalInvCnt = Count(inum),
			@TotalInvAmt = Sum(val)
	From #TBL_GSTR2_B2B_CNT

	Select	@TotalAcceptedInvCnt = Count(inum),
			@TotalAcceptedInvAmt = Sum(val)
	From #TBL_GSTR2_B2B_CNT
	Where IsNull(flag,'') In ('A','')

	Select	@TotalPendingInvCnt = Count(inum),
			@TotalPendingInvAmt = Sum(val)
	From #TBL_GSTR2_B2B_CNT
	Where IsNull(flag,'') ='P'

	Select	@TotalRejectedInvCnt = Count(inum),
			@TotalRejectedInvAmt = Sum(val)
	From #TBL_GSTR2_B2B_CNT
	Where IsNull(flag,'') ='R'

	Select * From #TBL_GSTR2_B2B
		
	Return 0

End