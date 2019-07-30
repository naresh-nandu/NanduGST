
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR3B Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/22/2017	Seshadri 	Initial Version
11/29/2017	Seshadri	Added detid column to the temporary table #TBL_GSTR3B_RECS
12/11/2017	Karthik		Fixed Integration Testing defects

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR3B_SA


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR3B_SA]  
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,txval,iamt,camt,samt,csamt)
	Select
		1 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_detid Desc) as slno,
		t3.osup_detid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,txval,iamt,csamt)
	Select
		2 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_zeroid Desc) as slno,
		t3.osup_zeroid as detid,
		txval, 
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_zero t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_zeroid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,txval)
	Select
		3 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_nil_exmpid Desc) as slno, 
		t3.osup_nil_exmpid as detid,
		txval
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_nil_exmp t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_nil_exmpid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,txval,iamt,camt,samt,csamt)
	Select
		4 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_rev_id Desc) as slno,
		t3.isup_rev_id as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,txval)
	Select
		5 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.osup_nongstid Desc) as slno,
		t3.osup_nongstid as detid,
		txval
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_sup_det t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_sup_det_osup_nongst t3 On t3.sup_detid = t2.sup_detid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.osup_nongstid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,pos,txval,iamt)
	Select
		6 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.unreg_detid Desc) as slno,
		t3.unreg_detid as detid,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_unreg_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.unreg_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,pos,txval,iamt)
	Select
		7 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.comp_detid Desc) as slno,
		t3.comp_detid as detid,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_comp_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.comp_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,pos,txval,iamt)
	Select
		8 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.uin_detid Desc) as slno,
		t3.uin_detid as detid,
		pos,
		txval,
		iamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inter_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inter_sup_uin_det t3 On t3.inter_supid = t2.inter_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	Order By t3.uin_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,interstatesupplies,intrastatesupplies)
	Select
		19 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_detid Desc) as slno,
		t3.isup_detid as detid,
		inter,
		intra
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'GST'
	Order By t3.isup_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,interstatesupplies,intrastatesupplies)
	Select
		20 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.isup_detid Desc) as slno,
		t3.isup_detid as detid,
		inter,
		intra
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_inward_sup t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_inward_sup_isup_details t3 On t3.inward_supid = t2.inward_supid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'NONGST'
	Order By t3.isup_detid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,csamt)
	Select
		9 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'IMPG'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,csamt)
	Select
		10 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
		iamt,
		csamt
	From TBL_GSTR3B t1
	Inner Join TBL_GSTR3B_itc_elg t2 On t2.gstr3bid = t1.gstr3bid
	Inner Join TBL_GSTR3B_itc_elg_itc_avl t3 On t3.itc_elgid = t2.itc_elgid
	Where gstin = @GstinNo
	And ret_period = @Fp
	And t3.ty = 'IMPS'
	Order By t3.itc_avlid Desc

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		11 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		12 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		13 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_avlid Desc) as slno,
		t3.itc_avlid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		14 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		t3.itc_revid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		15 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_revid Desc) as slno,
		t3.itc_revid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		16 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_netid Desc) as slno,
		t3.itc_netid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		17 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		t3.itc_inelgid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		18 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.itc_inelgid Desc) as slno,
		t3.itc_inelgid as detid,
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

	Insert Into #TBL_GSTR3B_RECS (supply_num,slno,detid,iamt,camt,samt,csamt)
	Select
		21 as supply_num,
		ROW_NUMBER() OVER(ORDER BY t3.intr_ltfeeid Desc) as slno,
		t3.intr_ltfeeid as detid,
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

	Select * from #TBL_GSTR3B_RECS

    -- Drop Temp Tables

	Drop Table #TBL_GSTR3B_RECS


	Return 0

End