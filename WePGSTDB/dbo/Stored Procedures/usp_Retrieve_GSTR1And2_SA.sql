
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B_D Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/05/2017	Seshadri 	Initial Version
12/12/2017	Seshadri	Fixed Integration Testing defects

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_D_SA


 */
 
Create  PROCEDURE [usp_Retrieve_GSTR1And2_SA]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Create Table #TBL_GSTR1And2_RECS
	(
		supply_num int,
		natureofsupplies varchar(100),
		slno int,
		detid int,
		pos varchar(2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
		interstatesupplies decimal(18,2),
	    intrastatesupplies decimal(18,2)
	)

	Insert Into #TBL_GSTR1And2_RECS (supply_num,slno,detid,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t.detid Desc) as slno,
		detid,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
             1 as supply_num,
		     ROW_NUMBER() OVER(ORDER BY t2.b2bid Desc) as slno,
		     t2.b2bid as detid,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid
		Inner Join TBL_GSTR1_B2B_INV_ITMS t4 On t4.invid = t3.invid
		Inner Join TBL_GSTR1_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')
		group by t2.b2bid

		/*
		Union 

		Select
             1 as supply_num,
		     ROW_NUMBER() OVER(ORDER BY t3.invid Desc) as slno,
		     t3.invid as detid,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
		Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid
		Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')

		Union 

		Select
		 1 as supply_num,
		     ROW_NUMBER() OVER(ORDER BY t3.itmsid Desc) as slno,
		     t3.itmsid as detid,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')

		*/

	) t
	group by detid

	Return 0


End