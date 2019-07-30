
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B Report from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/05/2017	Seshadri 	Initial Version
17/05/2018  Karthik     Included 1)3.1.A EWP,CDN,CDNUR included. 2)3.1.B EWOP and rt =0 Included
21/05/2018  Karthik		Included 3.1 and 3.2 added together, 4.ITC ISD details Added.  5.NIL(Supplies received as NIL,Exmptd, Non GST

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_Report '33GSPTN0801G1ZM','022018'


 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_GSTR3B_Report]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Create Table #TBL_GSTR3B_RECS
	(
		supply_num int,
		natureofsupplies varchar(500),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
	)

	Create Table #TBL_tmp_GSTR3B_RECS
	(
		supply_num int,
		natureofsupplies varchar(500),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
	)

	Create Table #TBL_GSTR1And2_RECS
	(
		supply_num int,
		natureofsupplies varchar(500),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
	)

	Create Table #TBL_GSTRDiff_RECS
	(
		supply_num int,
		natureofsupplies varchar(500),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
	)
	Create Table #TBL_GSTR1And2_CDN_RECS
	(
		Cdn_Type nvarchar(50),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
	)
	

	-- Supply_Num : 1 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_det t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @Gstin
	And ret_period = @Fp

	-- Including 3.2 with 3.1 only for GSTR3B Report
	Insert Into #TBL_tmp_GSTR3B_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
			Select
				1 as supply_num,
				sum(IsNull(txval,0)) as txval,
				sum(IsNull(iamt,0)) as iamt,
				0 as camt,
				0 as samt,
				0 as csamt 
			From TBL_GSTR3B t1
			Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
			Inner Join TBL_GSTR3B_inter_sup_unreg_det t3 On t3.inter_supid = t2.inter_supid
			Where gstin = @Gstin
			And ret_period = @Fp

			Union 

			Select
				1 as supply_num,
				sum(IsNull(txval,0))as txval,
				sum(IsNull(iamt,0)) as iamt,
				0 as camt,
				0 as samt,
				0 as csamt 
			From TBL_GSTR3B t1
			Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
			Inner Join TBL_GSTR3B_inter_sup_comp_det t3 On t3.inter_supid = t2.inter_supid
			Where gstin = @Gstin
			And ret_period = @Fp

			Union 

			Select
				1 as supply_num,
				sum(IsNull(txval,0))as txval,
				sum(IsNull(iamt,0)) as iamt,
				0 as camt,
				0 as samt,
				0 as csamt 
			From TBL_GSTR3B t1
			Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
			Inner Join TBL_GSTR3B_inter_sup_uin_det t3 On t3.inter_supid = t2.inter_supid
			Where gstin = @Gstin
			And ret_period = @Fp
		)B


		Update #TBL_GSTR3B_RECS 
			SET #TBL_GSTR3B_RECS.txval =  t1.txval + t2.txval, 
				#TBL_GSTR3B_RECS.iamt =  t1.iamt + t2.iamt,
				#TBL_GSTR3B_RECS.camt =  t1.camt + t2.camt,
				#TBL_GSTR3B_RECS.samt =  t1.samt + t2.samt,
				#TBL_GSTR3B_RECS.csamt =  t1.csamt + t2.csamt
			FROM #TBL_GSTR3B_RECS t1,
					#TBL_tmp_GSTR3B_RECS t2 
			WHERE t1.supply_num = t2.supply_num 
			And t1.supply_num = 1
	---


	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
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

		Union 

		Select
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

		
		Union 

		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
		Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') And t2.ex_tp ='WPAY'
		
	) t1

	Insert Into #TBL_GSTR1And2_CDN_RECS (Cdn_Type,txval,iamt,camt,samt,csamt)
	Select
		Cdn_Type,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			'CDNR-C' as Cdn_Type,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
		Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
		Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') and ntty='C'

		Union 

		Select
			'CDNR-D' as Cdn_Type,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
		Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
		Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') and ntty='D'

		Union 

		Select
			'CDNUR-C' as Cdn_Type,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
		Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') and ntty='C' and typ in ('EXPWOP','B2CL','EXPWP')

		Union 

		Select
			'CDNUR-D' as Cdn_Type,
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
		Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') and ntty='D' and typ in ('EXPWOP','B2CL','EXPWP')
		) A
		Group by A.Cdn_Type

	
				Update #TBL_GSTR1And2_RECS 
				SET #TBL_GSTR1And2_RECS.txval =  t1.txval + t2.txval, 
					#TBL_GSTR1And2_RECS.iamt =  t1.iamt + t2.iamt,
					#TBL_GSTR1And2_RECS.camt =  t1.camt + t2.camt,
					#TBL_GSTR1And2_RECS.samt =  t1.samt + t2.samt,
					#TBL_GSTR1And2_RECS.csamt =  t1.csamt + t2.csamt
				FROM #TBL_GSTR1And2_RECS t1,
						#TBL_GSTR1And2_CDN_RECS t2 
				WHERE t2.Cdn_Type ='CDNR-D'
				And t1.supply_num = 1
				
				Update #TBL_GSTR1And2_RECS 
				SET #TBL_GSTR1And2_RECS.txval =  t1.txval + t2.txval, 
					#TBL_GSTR1And2_RECS.iamt =  t1.iamt + t2.iamt,
					#TBL_GSTR1And2_RECS.camt =  t1.camt + t2.camt,
					#TBL_GSTR1And2_RECS.samt =  t1.samt + t2.samt,
					#TBL_GSTR1And2_RECS.csamt =  t1.csamt + t2.csamt
				FROM #TBL_GSTR1And2_RECS t1,
						#TBL_GSTR1And2_CDN_RECS t2 
				WHERE t2.Cdn_Type ='CDNUR-D'
				And t1.supply_num = 1

				Update #TBL_GSTR1And2_RECS 
				SET #TBL_GSTR1And2_RECS.txval =  t1.txval - t2.txval, 
					#TBL_GSTR1And2_RECS.iamt =  t1.iamt - t2.iamt,
					#TBL_GSTR1And2_RECS.camt =  t1.camt - t2.camt,
					#TBL_GSTR1And2_RECS.samt =  t1.samt - t2.samt,
					#TBL_GSTR1And2_RECS.csamt =  t1.csamt - t2.csamt
				FROM #TBL_GSTR1And2_RECS t1,
						#TBL_GSTR1And2_CDN_RECS t2 
				WHERE t2.Cdn_Type ='CDNR-C'
				And t1.supply_num = 1
				
				Update #TBL_GSTR1And2_RECS 
				SET #TBL_GSTR1And2_RECS.txval =  t1.txval - t2.txval, 
					#TBL_GSTR1And2_RECS.iamt =  t1.iamt - t2.iamt,
					#TBL_GSTR1And2_RECS.camt =  t1.camt - t2.camt,
					#TBL_GSTR1And2_RECS.samt =  t1.samt - t2.samt,
					#TBL_GSTR1And2_RECS.csamt =  t1.csamt - t2.csamt
				FROM #TBL_GSTR1And2_RECS t1,
						#TBL_GSTR1And2_CDN_RECS t2 
				WHERE t2.Cdn_Type ='CDNUR-C'
				And t1.supply_num = 1


		Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
		Select
			1 as supply_num,
			IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
			IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
			IsNull(t1.camt,0) - IsNull(t2.camt,0),
			IsNull(t1.samt,0) - IsNull(t2.samt,0),
			IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
		From #TBL_GSTR3B_RECS t1,
			#TBL_GSTR1And2_RECS t2
		Where t1.supply_num = t2.supply_num
		And t1.supply_num = 1

	-- Supply_Num : 1 (End)

	-- Supply_Num : 2 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt,csamt)
	Select
		2 as supply_num,
		sum(IsNull(txval,0)),  
		sum(IsNull(iamt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_zero t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @Gstin
	And ret_period = @Fp

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		2 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			0 as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
		Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D') And t2.ex_tp ='WOPAY' and t4.rt = 0
		
	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		2 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 2

	-- Supply_Num : 2 (End)

	-- Supply_Num : 3 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval)
	Select
		3 as supply_num,
		sum(IsNull(txval,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_nil_exmp t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @Gstin
	And ret_period = @Fp

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		3 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(nil_amt,0) + IsNull(expt_amt,0) + IsNull(ngsup_amt,0))  as txval, 
			0 as iamt, 
			0 as camt,
			0 as samt,
			0 as csamt
		From TBL_GSTR1 t1
		Inner Join TBL_GSTR1_NIL t2 On t2.gstr1id = t1.gstr1id
		Inner Join TBL_GSTR1_NIL_INV t3 On t3.nilid = t2.nilid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')

	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		3 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 3


	-- Supply_Num : 3 (End)

	-- Supply_Num : 4 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		4 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_isup_rev t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @Gstin
	And ret_period = @Fp


	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		4 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_B2BUR t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_B2BUR_INV t3 On t3.b2burid = t2.b2burid
		Inner Join TBL_GSTR2_B2BUR_INV_ITMS t4 On t4.invid = t3.invid
		Inner Join TBL_GSTR2_B2BUR_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')


	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		4 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 4

	-- Supply_Num : 4 (End)

	Insert Into #TBL_GSTR3B_RECS (supply_num,natureofsupplies,txval,iamt,camt,samt,csamt)
	Select
		5 as supply_num,
		'3-Total Outward Tax Liability' , 
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From #TBL_GSTR3B_RECS 
	Where supply_num in (1,2,3,4)

	Insert Into #TBL_GSTR1And2_RECS (supply_num,natureofsupplies,txval,iamt,camt,samt,csamt)
	Select
		5 as supply_num,
		'3-Total Outward Tax Liability' , 
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From  #TBL_GSTR1And2_RECS 
	Where supply_num in (1,2,3,4)

	Insert Into #TBL_GSTRDiff_RECS (supply_num,natureofsupplies,txval,iamt,camt,samt,csamt)
	Select
		5 as supply_num,
		'3-Total Outward Tax Liability' , 
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From  #TBL_GSTRDiff_RECS 
	Where supply_num in (1,2,3,4)

	-- Supply_Num : 9 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,csamt)
	Select
		9 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'IMPG'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		9 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_IMPG t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_IMPG_ITMS t3 On t3.impgid = t2.impgid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')


	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		9 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 9

	-- Supply_Num : 9 (End)

	-- Supply_Num : 10 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,csamt)
	Select
		10 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'IMPS'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		10 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			0 as camt,
			0 as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_IMPS t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_IMPS_ITMS t3 On t3.impsid = t2.impsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')

	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		10 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 10

	-- Supply_Num : 10 (End)

	-- Supply_Num : 11 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'ISRC'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
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
		And IsNull(rchrg,'') = 'Y'


	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 11

	-- Supply_Num : 11 (End)

	-- Supply_Num : 13 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'OTH'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			sum(IsNull(txval,0)) as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
		Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
		Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')
		And IsNull(rchrg,'') Not In ('Y')

	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 13

	-- Supply_Num : 13 (End)

	-- Supply_Num : 14 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'RUL'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			0 as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_ITCRVSL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_ITCRVSL_ITMS t3 On t3.itcrvslid = t2.itcrvslid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')
		And rulename In ('rule7_1_m','rule8_1_h')
	
	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 14

	-- Supply_Num : 14 (End)

	-- Supply_Num : 15 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'OTH'

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			0 as txval, 
			sum(IsNull(iamt,0)) as iamt, 
			sum(IsNull(camt,0)) as camt,
			sum(IsNull(samt,0)) as samt,
			sum(IsNull(csamt,0)) as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_ITCRVSL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_ITCRVSL_ITMS t3 On t3.itcrvslid = t2.itcrvslid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')
		And rulename Not In ('rule7_1_m','rule8_1_h')
	
	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 15

	-- Supply_Num : 15 (End)

	-- Supply_Num : 16 (Begin)

	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_net t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = ''

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		sum(IsNull(t1.txval,0)) , 
		sum(IsNull(t1.iamt,0)),
		sum(IsNull(t1.camt,0)),
		sum(IsNull(t1.samt,0)),
		sum(IsNull(t1.csamt,0))  
	From #TBL_GSTR1And2_RECS t1
	Where t1.supply_num In (9,10,11,13)

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 16

	-- Supply_Num : 16 (End)

	---- Supply_Num : 17 (Start)
	--	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	--Select
	--	17 as supply_num,
	--	sum(IsNull(t1.txval,0)) , 
	--	sum(IsNull(t1.iamt,0)),
	--	sum(IsNull(t1.camt,0)),
	--	sum(IsNull(t1.samt,0)),
	--	sum(IsNull(t1.csamt,0))  
	--From #TBL_GSTR1And2_RECS t1
	--Where t1.supply_num In (5,10,11,13)
	---- Supply_Num : 17 (End)


	/*
	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt)
	Select
		6 as supply_num,
		sum(IsNull(txval,0)),
		sum(IsNull(iamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_unreg_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @Gstin
	And ret_period = @Fp

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt)
	Select
		7 as supply_num,
		sum(IsNull(txval,0)),
		sum(IsNull(iamt,0))
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_comp_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @Gstin
	And ret_period = @Fp

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt)
	Select
		8 as supply_num,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_uin_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @Gstin
	And ret_period = @Fp */


	Select
		19 as supply_num,
		sum(inter) as inter,
		sum(intra) as intra into #tbl_tmp_19
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty in ('GST','NONGST')

	
	Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	Select
		19 as supply_num,
		inter as iamt,
		 convert(Decimal(18,2),intra)/2 as camt,
		 convert(Decimal(18,2),intra)/2 as samt,
		 0 as csamt
	From #tbl_tmp_19 

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		19 as supply_num,
		sum(IsNull(txval,0)), 
		sum(IsNull(iamt,0)),
		sum(IsNull(camt,0)),
		sum(IsNull(samt,0)),
		sum(IsNull(csamt,0))
	From
	(
		Select
			0 as txval, 
			sum(IsNull(exptdsply,0)+IsNull(ngsply,0)+IsNull(nilsply,0)) as iamt, 
			0 as camt,
			0 as samt,
			0 as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_NIL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_NIL_INTER t3 On t3.nilid = t2.nilid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')

		Union

		Select
			0 as txval, 
			0 as iamt, 
			sum(IsNull(exptdsply,0)+IsNull(ngsply,0)+IsNull(nilsply,0))/2 as camt,
			sum(IsNull(exptdsply,0)+IsNull(ngsply,0)+IsNull(nilsply,0))/2 as samt,
			0 as csamt
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_NIL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_NIL_INTRA t3 On t3.nilid = t2.nilid
		Where gstin = @Gstin
		And fp = @Fp
		And IsNull(flag,'') Not In ('','D')
	
	) t1

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		19 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 19

	

	--Select
	--	20 as supply_num,
	--	t3.isup_detid as detid,
	--	inter,
	--	intra  into #tbl_tmp_20
	--From TBL_GSTR3B t1
	--Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	--Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	--Where gstin = @Gstin
	--And ret_period = @Fp
	--And t3.ty = 'NONGST'
	--Order By t3.isup_detid Desc

	--Insert Into #TBL_GSTR3B_RECS (supply_num,iamt,camt,samt,csamt)
	--Select
	--	20 as supply_num,
	--	inter as iamt,
	--	 convert(Decimal(18,2),intra)/2 as camt,
	--	 convert(Decimal(18,2),intra)/2 as samt,
	--	 0 as csamt
	--From #tbl_tmp_20

	/* Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,csamt)
	Select
		9 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'MPG'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,csamt)
	Select
		10 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'IMPS'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'ISRC'
	Order By t3.itc_avlid Desc */

	Insert Into #TBL_GSTR3B_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		12 as supply_num,
		0 as txval,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @Gstin
	And ret_period = @Fp
	And t3.ty = 'ISD'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR1And2_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		12 as supply_num,
		0 as txval, 
		0 as iamt,
		0 as camt,
		0 as samt,
		0 as csamt
	-- GSTR2 - ISD Table data needs to include here once tables created and data stored.

	Insert Into #TBL_GSTRDiff_RECS (supply_num,txval,iamt,camt,samt,csamt)
	Select
		12 as supply_num,
		IsNull(t1.txval,0) - IsNull(t2.txval,0) , 
		IsNull(t1.iamt,0) - IsNull(t2.iamt,0),
		IsNull(t1.camt,0) - IsNull(t2.camt,0),
		IsNull(t1.samt,0) - IsNull(t2.samt,0),
		IsNull(t1.csamt,0) - IsNull(t2.csamt,0)  
	From #TBL_GSTR3B_RECS t1,
		#TBL_GSTR1And2_RECS t2
	Where t1.supply_num = t2.supply_num
	And t1.supply_num = 12

	/* Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		t3.itc_revid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'RUL'
	Order By t3.itc_revid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		t3.itc_revid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_revid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_netid Desc) as slno,
		t3.itc_netid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_net t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = ''
	Order By t3.itc_netid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		17 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		t3.itc_inelgid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_inelg t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'RUL'
	Order By t3.itc_inelgid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		18 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		t3.itc_inelgid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_itc_elg_itc_inelg t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_inelgid Desc

	Insert Into #TBL_GSTR3B_D_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		21 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.intr_ltfeeid Desc) as slno,
		t3.intr_ltfeeid as detid,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B_D t1
	Inner Join TBL_GSTR3B_D_intr_ltfee t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_D_intr_ltfee_intr_det t3 On t3.intr_ltfeeid = t2.intr_ltfeeid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.intr_ltfeeid Desc

	*/

	Update #TBL_GSTR3B_RECS 
	Set natureofsupplies =
	Case supply_num
		When 1 Then '3.1 & 3.2-Outward taxable  supplies  (other than zero rated, nil rated and exempted) & Supplies made to Unregistered Persons & Supplies made to Composition Taxable Persons & Supplies made to UIN holders' 
		When 2 Then '3.1-Outward taxable  supplies  (zero rated )' 
		When 3 Then '3.1-Other outward supplies (Nil rated, exempted)' 
		When 4 Then '3.1-Inward supplies (liable to reverse charge)' 
		--When 5 Then '3.1-Non-GST outward supplies'  
		When 6 Then '3.2-Supplies made to Unregistered Persons'  
		When 7 Then '3.2-Supplies made to Composition Taxable Persons' 
		When 8 Then '3.2-Supplies made to UIN holders' 
		When 9 Then '4-ITC Available/Import of goods' 
		When 10	Then '4-ITC Available/Import of services' 
		When 11 Then '4-ITC Available/Inward supplies liable to reverse charge (other than 1 & 2 above)' 
		When 12 Then '4-ITC Available/Inward supplies from ISD' 
		When 13 Then '4-ITC Available/All other ITC' 
		When 14 Then '4-ITC Reversed/As per Rules 42 & 43 of CGST/SGST rules' 
		When 15 Then '4-ITC Reversed/Others' 
		When 16 Then '4-Net ITC Available'  
		When 17 Then '4-Ineligible ITC/As per section 17(S)' 
		When 18 Then '4-Ineligible ITC/Others' 
		When 19 Then '5-From a supplier under composition scheme, Exempt and Nil rated supply & Non GST supply' 
		When 20 Then '5-Non GST supply' 
		When 21 Then '5.1-Interest late fee' 
		When 5 Then natureofsupplies
		Else ''
	End

	Update #TBL_GSTR1And2_RECS 
	Set natureofsupplies =
	Case supply_num
		When 1 Then '3.1 & 3.2-Outward taxable  supplies  (other than zero rated, nil rated and exempted) & Supplies made to Unregistered Persons & Supplies made to Composition Taxable Persons & Supplies made to UIN holders' 
		When 2 Then '3.1-Outward taxable  supplies  (zero rated )' 
		When 3 Then '3.1-Other outward supplies (Nil rated, exempted)' 
		When 4 Then '3.1-Inward supplies (liable to reverse charge)' 
		--When 5 Then '3.1-Non-GST outward supplies'  
		When 6 Then '3.2-Supplies made to Unregistered Persons'  
		When 7 Then '3.2-Supplies made to Composition Taxable Persons' 
		When 8 Then '3.2-Supplies made to UIN holders' 
		When 9 Then '4-ITC Available/Import of goods' 
		When 10	Then '4-ITC Available/Import of services' 
		When 11 Then '4-ITC Available/Inward supplies liable to reverse charge (other than 1 & 2 above)' 
		When 12 Then '4-ITC Available/Inward supplies from ISD' 
		When 13 Then '4-ITC Available/All other ITC' 
		When 14 Then '4-ITC Reversed/As per Rules 42 & 43 of CGST/SGST rules' 
		When 15 Then '4-ITC Reversed/Others' 
		When 16 Then '4-Net ITC Available'  
		When 17 Then '4-Ineligible ITC/As per section 17(S)' 
		When 18 Then '4-Ineligible ITC/Others' 
		When 19 Then '5-From a supplier under composition scheme, Exempt and Nil rated supply & Non GST supply' 
		When 20 Then '5-Non GST supply' 
		When 21 Then '5.1-Interest late fee' 
		When 5 Then natureofsupplies
		Else ''
	End

	Update #TBL_GSTRDiff_RECS 
	Set natureofsupplies =
	Case supply_num
		When 1 Then '3.1 & 3.2-Outward taxable  supplies  (other than zero rated, nil rated and exempted) & Supplies made to Unregistered Persons & Supplies made to Composition Taxable Persons & Supplies made to UIN holders' 
		When 2 Then '3.1-Outward taxable  supplies  (zero rated )' 
		When 3 Then '3.1-Other outward supplies (Nil rated, exempted)' 
		When 4 Then '3.1-Inward supplies (liable to reverse charge)' 
		--When 5 Then '3.1-Non-GST outward supplies'  
		When 6 Then '3.2-Supplies made to Unregistered Persons'  
		When 7 Then '3.2-Supplies made to Composition Taxable Persons' 
		When 8 Then '3.2-Supplies made to UIN holders' 
		When 9 Then '4-ITC Available/Import of goods' 
		When 10	Then '4-ITC Available/Import of services' 
		When 11 Then '4-ITC Available/Inward supplies liable to reverse charge (other than 1 & 2 above)' 
		When 12 Then '4-ITC Available/Inward supplies from ISD' 
		When 13 Then '4-ITC Available/All other ITC' 
		When 14 Then '4-ITC Reversed/As per Rules 42 & 43 of CGST/SGST rules' 
		When 15 Then '4-ITC Reversed/Others' 
		When 16 Then '4-Net ITC Available'  
		When 17 Then '4-Ineligible ITC/As per section 17(S)' 
		When 18 Then '4-Ineligible ITC/Others' 
		When 19 Then '5-From a supplier under composition scheme, Exempt and Nil rated supply & Non GST supply' 
		When 20 Then '5-Non GST supply' 
		When 21 Then '5.1-Interest late fee' 
		When 5 Then natureofsupplies
		Else ''
	End

	/*
	Select 
		natureofsupplies as 'Nature of Supplies',
		txval as 'Total Taxable Value',
		iamt as 'Integrated Tax',
		camt as 'Central Tax',
		samt as 'State / UT Tax',
		csamt as 'CESS'
	From #TBL_GSTR3B_RECS
	Order By supply_num

	Select 
		natureofsupplies as 'Nature of Supplies',
		txval as 'Total Taxable Value',
		iamt as 'Integrated Tax',
		camt as 'Central Tax',
		samt as 'State / UT Tax',
		csamt as 'CESS'
	From #TBL_GSTR1And2_RECS
	Order By supply_num


	Select 
		natureofsupplies as 'Nature of Supplies',
		txval as 'Total Taxable Value',
		iamt as 'Integrated Tax',
		camt as 'Central Tax',
		samt as 'State / UT Tax',
		csamt as 'CESS'
	From #TBL_GSTRDiff_RECS
	Order By supply_num
	*/

	Select 
		
		t1.natureofsupplies as 'Nature of Supplies',
		IsNull(t1.txval,0) as 'Total Taxable Value',
		IsNull(t1.iamt,0) as 'Integrated Tax',
		IsNull(t1.camt,0) as 'Central Tax',
		IsNull(t1.samt,0) as 'State / UT Tax',
		IsNull(t1.csamt,0) as 'CESS',

		IsNull(t2.txval,0) as 'Total Taxable Value',
		IsNull(t2.iamt,0) as 'Integrated Tax',
		IsNull(t2.camt,0) as 'Central Tax',
		IsNull(t2.samt,0) as 'State / UT Tax',
		IsNull(t2.csamt,0) as 'CESS',

		IsNull(t3.txval,0) as 'Total Taxable Value',
		IsNull(t3.iamt,0) as 'Integrated Tax',
		IsNull(t3.camt,0) as 'Central Tax',
		IsNull(t3.samt,0) as 'State / UT Tax',
		IsNull(t3.csamt,0) as 'CESS'


	From #TBL_GSTR3B_RECS t1,
		 #TBL_GSTR1And2_RECS t2,
		 #TBL_GSTRDiff_RECS t3
	Where t2.supply_num = t1.supply_num
	And t3.supply_num = t1.supply_num
	Order By t1.supply_num

	--Select * from  #TBL_GSTR3B_RECS
	--Select * from  #TBL_GSTR1And2_RECS
	--Select * from  #TBL_GSTRDiff_RECS

    -- Drop Temp Tables

	Drop Table #TBL_GSTR3B_RECS
	Drop Table #TBL_GSTR1And2_RECS
	Drop Table #TBL_GSTRDiff_RECS

	Return 0

End