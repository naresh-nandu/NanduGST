
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B Summary Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/24/2017	Seshadri 	Initial Version
12/11/2017	Karthik		Fixed Integration Testing defects


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_Summary_SA


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR3B_Summary_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Create Table #TBL_GSTR3B_RECS
	(
		supply_num int,
		slno int,
		pos varchar(2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
		interstatesupplies decimal(18,2),
	    intrastatesupplies decimal(18,2)
	)

	Create Table #TBL_SUMMARY_DATA
	(
		slno int,
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
		interstatesupplies decimal(18,2),
	    intrastatesupplies decimal(18,2)
	)

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_detid Desc) as slno,
		txval, 
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_det t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,txval,iamt,csamt)
	Select
		2 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_zeroid Desc) as slno,
		txval, 
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_zero t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_zeroid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,txval)
	Select
		3 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_nil_exmpid Desc) as slno,
		txval
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_nil_exmp t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_nil_exmpid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,txval,iamt,camt,samt,csamt)
	Select
		4 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_rev_id Desc) as slno,
		txval,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_isup_rev t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.isup_rev_id Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,txval)
	Select
		5 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_nongstid Desc) as slno,
		txval
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_nongst t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_nongstid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,pos,txval,iamt)
	Select
		6 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.unreg_detid Desc) as slno,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_unreg_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.unreg_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,pos,txval,iamt)
	Select
		7 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.comp_detid Desc) as slno,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_comp_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.comp_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,pos,txval,iamt)
	Select
		8 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.uin_detid Desc) as slno,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_uin_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.uin_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,interstatesupplies,intrastatesupplies)
	Select
		19 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_detid Desc) as slno,
		inter,
		intra
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'GST'
	Order By t3.isup_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,interstatesupplies,intrastatesupplies)
	Select
		20 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_detid Desc) as slno,
		inter,
		intra
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'NONGST'
	Order By t3.isup_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,csamt)
	Select
		9 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'IMPG'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,csamt)
	Select
		10 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'IMPS'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'ISRC'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		12 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'ISD'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'RUL'
	Order By t3.itc_revid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_rev t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_revid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_netid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_net t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = ''
	Order By t3.itc_netid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		17 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_inelg t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'RUL'
	Order By t3.itc_inelgid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		18 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_inelg t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'OTH'
	Order By t3.itc_inelgid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,iamt,camt,samt,csamt)
	Select
		21 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.intr_ltfeeid Desc) as slno,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_intr_ltfee t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_intr_ltfee_intr_det t3 On t3.intr_ltfeeid = t2.intr_ltfeeid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.intr_ltfeeid Desc

	Insert Into #TBL_SUMMARY_DATA(slno,txval,iamt,camt,samt,csamt)
	Select 1,
		   sum(IsNull(txval,0)) as txval,
	   	   sum(IsNull(iamt,0)) as iamt,
		   sum(IsNull(camt,0)) as camt,
		   sum(IsNull(samt,0)) as samt,
		   sum(IsNull(csamt,0)) as csamt
	From #TBL_GSTR3B_RECS
	Where supply_num in (1,2,3,4,5)

	Insert Into #TBL_SUMMARY_DATA(slno,txval,iamt)
	Select 2,
		   sum(IsNull(txval,0)) as txval,
	   	   sum(IsNull(iamt,0)) as iamt
	From #TBL_GSTR3B_RECS
	Where supply_num in (6,7,8)

	Insert Into #TBL_SUMMARY_DATA(slno,txval,iamt,camt,samt,csamt)
	Select 3,
		   sum(IsNull(txval,0)) as txval,
	   	   sum(IsNull(iamt,0)) as iamt,
		   sum(IsNull(camt,0)) as camt,
		   sum(IsNull(samt,0)) as samt,
		   sum(IsNull(csamt,0)) as csamt
	From #TBL_GSTR3B_RECS
	Where supply_num in (9,10,11,12,13,14,15,16,17,18)

	Insert Into #TBL_SUMMARY_DATA(slno,interstatesupplies,intrastatesupplies)
	Select 4,
		   sum(IsNull(interstatesupplies,0)) as interstatesupplies,
	   	   sum(IsNull(intrastatesupplies,0)) as intrastatesupplies
	From #TBL_GSTR3B_RECS
	Where supply_num in (19,20)

	Insert Into #TBL_SUMMARY_DATA(slno,iamt,camt,samt,csamt)
	Select 5,
	   	   sum(IsNull(iamt,0)) as iamt,
		   sum(IsNull(camt,0)) as camt,
		   sum(IsNull(samt,0)) as samt,
		   sum(IsNull(csamt,0)) as csamt
	From #TBL_GSTR3B_RECS
	Where supply_num in (21)

	Select * From #TBL_SUMMARY_DATA

    -- Drop Temp Tables

	Drop Table #TBL_GSTR3B_RECS
	Drop Table #TBL_SUMMARY_DATA


	Return 0

End