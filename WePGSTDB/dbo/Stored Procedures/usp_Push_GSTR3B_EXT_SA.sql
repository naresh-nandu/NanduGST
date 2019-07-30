
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR3B Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/21/2017	Seshadri	Initial Version
12/11/2017	Karthik		Fixed Integration Testing defects
4/6/2018	Seshadri	Fixed deleted Gstin Issue 


*/

/* Sample Procedure Call

exec usp_Push_GSTR3B_EXT_SA 'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR3B_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	space(50) as gstr3bid,
	space(50) as sup_detid,
	space(50) as inter_supid,
	space(50) as inward_supid,
	space(50) as itc_elgid,
	space(50) as intr_ltfeeid,
	t1.gstin,t2.gstinid,t1.fp,
	t1.natureofsupplies,t1.supply_num,
	t1.pos,
	t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt,
	t1.interstatesupplies,
	t1.intrastatesupplies
	Into #TBL_EXT_GSTR3B
	From
	(
		SELECT 
			gstin,fp,
			natureofsupplies,supply_num,
			pos,
			sum(txval) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt,
			sum(interstatesupplies) as interstatesupplies,
			sum(intrastatesupplies) as intrastatesupplies
		FROM TBL_EXT_GSTR3B_DET
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,natureofsupplies,supply_num,pos
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR3B

	Insert TBL_GSTR3B (gstin,gstinId,ret_period)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR3B t1
	Where Not Exists(Select 1 From TBL_GSTR3B t2 Where t2.gstin = t1.gstin and t2.ret_period = t1.fp)

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.gstr3bid = t2.gstr3bid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.ret_period

	-- Insert Records into Table TBL_GSTR3B_sup_det

	Insert TBL_GSTR3B_sup_det(gstr3bid,gstinid) 
	Select	distinct gstr3bid,gstinid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num In (1,2,3,4,5)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det t2
					   Where t2.gstr3bid = t1.gstr3bid
		  		    ) 

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.sup_detid = t2.sup_detid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B_sup_det t2 
	WHERE t1.gstr3bid = t2.gstr3bid 

	-- Insert Records into Table TBL_GSTR3B_sup_det_osup_det

	Insert TBL_GSTR3B_sup_det_osup_det
	(sup_detid,
		txval,iamt,camt,samt,csamt,gstinid,gstr3bid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 1
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det_osup_det t2
						Where t2.sup_detid = t1.sup_detid 
						)
						
	-- Insert Records into Table TBL_GSTR3B_sup_det_osup_zero

	Insert TBL_GSTR3B_sup_det_osup_zero
	(sup_detid,
		txval,iamt,csamt,gstinid,gstr3bid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.csamt,t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 2
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det_osup_zero t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_sup_det_osup_nil_exmp

	Insert TBL_GSTR3B_sup_det_osup_nil_exmp
	(sup_detid,
		txval,gstinid,gstr3bid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 3
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det_osup_nil_exmp t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_sup_det_isup_rev

	Insert TBL_GSTR3B_sup_det_isup_rev
	(sup_detid,
		txval,iamt,camt,samt,csamt,gstinid,gstr3bid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 4
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det_isup_rev t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_sup_det_osup_nongst

	Insert TBL_GSTR3B_sup_det_osup_nongst
	(sup_detid,
		txval,gstinid,gstr3bid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 5
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_sup_det_osup_nongst t2
						Where t2.sup_detid = t1.sup_detid 
						)



	-- Insert Records into Table TBL_GSTR3B_inter_sup

	Insert TBL_GSTR3B_inter_sup(gstr3bid,gstinid) 
	Select	distinct gstr3bid,gstinid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num In (6,7,8)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_inter_sup t2
					   Where t2.gstr3bid = t1.gstr3bid
		  		    ) 

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.inter_supid = t2.inter_supid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B_inter_sup t2 
	WHERE t1.gstr3bid = t2.gstr3bid 

	-- Insert Records into Table TBL_GSTR3B_inter_sup_unreg_det

	Insert TBL_GSTR3B_inter_sup_unreg_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 6
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_inter_sup_unreg_det t2
						Where t2.inter_supid = t1.inter_supid 
						)

	-- Insert Records into Table TBL_GSTR3B_inter_sup_comp_det

	Insert TBL_GSTR3B_inter_sup_comp_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 7
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_inter_sup_comp_det t2
						Where t2.inter_supid = t1.inter_supid 
						)

	-- Insert Records into Table TBL_GSTR3B_inter_sup_uin_det

	Insert TBL_GSTR3B_inter_sup_uin_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 8
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_inter_sup_uin_det t2
						Where t2.inter_supid = t1.inter_supid 
						)


	-- Insert Records into Table TBL_GSTR3B_inward_sup

	Insert TBL_GSTR3B_inward_sup(gstr3bid,gstinid) 
	Select	distinct gstr3bid,gstinid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num In (19,20)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_inward_sup t2
					   Where t2.gstr3bid = t1.gstr3bid
		  		    ) 

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.inward_supid = t2.inward_supid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B_inward_sup t2 
	WHERE t1.gstr3bid = t2.gstr3bid 

	-- Insert Records into Table TBL_GSTR3B_inward_sup_isup_details

	Insert TBL_GSTR3B_inward_sup_isup_details
	(inward_supid,ty,inter,intra,
		gstinid,gstr3bid)
	Select	distinct t1.inward_supid,'GST',t1.interstatesupplies,t1.intrastatesupplies,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 19
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_inward_sup_isup_details t2
						Where t2.inward_supid = t1.inward_supid 
						And t2.ty = 'GST'
						)

	Insert TBL_GSTR3B_inward_sup_isup_details
	(inward_supid,ty,inter,intra,
		gstinid,gstr3bid)
	Select	distinct t1.inward_supid,'NONGST',t1.interstatesupplies,t1.intrastatesupplies,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 20
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_inward_sup_isup_details t2
						Where t2.inward_supid = t1.inward_supid 
						And t2.ty = 'NONGST'
						)

	-- Insert Records into Table TBL_GSTR3B_itc_elg

	Insert TBL_GSTR3B_itc_elg(gstr3bid,gstinid) 
	Select	distinct gstr3bid,gstinid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num In (9,10,11,12,13,14,15,16,17,18)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg t2
					   Where t2.gstr3bid = t1.gstr3bid
		  		    ) 

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.itc_elgid = t2.itc_elgid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B_itc_elg t2 
	WHERE t1.gstr3bid = t2.gstr3bid 

	-- Insert Records into Table TBL_GSTR3B_itc_elg_itc_avl

	Insert TBL_GSTR3B_itc_elg_itc_avl
	(itc_elgid,ty,iamt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'IMPG',t1.iamt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 9
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'IMPG'
						)

	Insert TBL_GSTR3B_itc_elg_itc_avl
	(itc_elgid,ty,iamt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'IMPS',t1.iamt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 10
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'IMPS'
						)

	Insert TBL_GSTR3B_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'ISRC',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 11
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'ISRC'
						)

	Insert TBL_GSTR3B_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'ISD',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 12
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'ISD'
						)

	Insert TBL_GSTR3B_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 13
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

	Insert TBL_GSTR3B_itc_elg_itc_rev
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'RUL',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 14
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_rev t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'RUL'
						)

	Insert TBL_GSTR3B_itc_elg_itc_rev
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 15
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_rev t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

	Insert TBL_GSTR3B_itc_elg_itc_net
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 16
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_net t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = ''
						)

	Insert TBL_GSTR3B_itc_elg_itc_inelg
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'RUL',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 17
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_inelg t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'RUL'
						)

	Insert TBL_GSTR3B_itc_elg_itc_inelg
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 18
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_itc_elg_itc_inelg t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

	-- Insert Records into Table TBL_GSTR3B_intr_ltfee

	Insert TBL_GSTR3B_intr_ltfee(gstr3bid,gstinid) 
	Select	distinct gstr3bid,gstinid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num In (21)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_intr_ltfee t2
					   Where t2.gstr3bid = t1.gstr3bid
		  		    ) 

	Update #TBL_EXT_GSTR3B
	SET #TBL_EXT_GSTR3B.intr_ltfeeid = t2.intr_ltfeeid 
	FROM #TBL_EXT_GSTR3B t1,
		TBL_GSTR3B_intr_ltfee t2 
	WHERE t1.gstr3bid = t2.gstr3bid 

	-- Insert Records into Table TBL_GSTR3B_intr_ltfee_intr_det

	Insert TBL_GSTR3B_intr_ltfee_intr_det
	(intr_ltfeeid,iamt,camt,samt,csamt,
		gstinid,gstr3bid)
	Select	distinct t1.intr_ltfeeid,t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bid
	From #TBL_EXT_GSTR3B t1
	Where t1.supply_num = 21
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_intr_ltfee_intr_det t2
						Where t2.intr_ltfeeid = t1.intr_ltfeeid 
						)

	If Exists(Select 1 From #TBL_EXT_GSTR3B )
	Begin
		Update TBL_EXT_GSTR3B_DET 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 
		
	Return 0

End