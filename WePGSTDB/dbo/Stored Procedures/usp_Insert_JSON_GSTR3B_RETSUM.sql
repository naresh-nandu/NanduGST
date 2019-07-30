
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR3B RETSUM JSON Records to the corresponding tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/5/2017	Seshadri	Initial Version
12/12/2017	Seshadri	Fixed Integration Testing Defects

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR3B_RETSUM
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR3B_RETSUM]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Create Table #TBL_GSTR3B_RECS
	(
		gstin varchar(15),
		gstinid int,
		fp varchar(10),
		gstr3bdid int,
		sup_detid int,
		inter_supid int,
		inward_supid int,
		itc_elgid int,
		intr_ltfeeid int,
		supply_num int,
		ty varchar(10),
		pos varchar(2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2),
		interstatesupplies decimal(18,2),
	    intrastatesupplies decimal(18,2)
	)

	-- Supply_Num : 1

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 txval,iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,1 ,
			txval,
			iamt,
			camt,
			samt,
			csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		sup_details nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.sup_details) 
	WITH
	(
		osup_det nvarchar(max) as JSON
	) As Sup_details
	Cross Apply OPENJSON(Sup_details.osup_det)
	WITH
	(
		txval decimal(18,2),
	    iamt decimal(18,2),
		camt decimal(18,2) ,
        samt decimal(18,2),
        csamt decimal(18,2)
  	) As Osup_det
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 2

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 txval,iamt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,2 ,
			txval,
			iamt,
			csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		sup_details nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.sup_details) 
	WITH
	(
		osup_zero nvarchar(max) as JSON
	) As Sup_details
	Cross Apply OPENJSON(Sup_details.osup_zero)
	WITH
	(
		txval decimal(18,2),
	    iamt decimal(18,2),
        csamt decimal(18,2)
  	) As Osup_zero
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 3

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 txval)	
	Select	@Gstin,t1.gstinid ,@Fp,3 ,
			txval
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		sup_details nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.sup_details) 
	WITH
	(
		osup_nil_exmp nvarchar(max) as JSON
	) As Sup_details
	Cross Apply OPENJSON(Sup_details.osup_nil_exmp)
	WITH
	(
		txval decimal(18,2)
  	) As Osup_nil_exmp
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 4

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 txval,iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,4 ,
			txval,
			iamt,
			camt,
			samt,
			csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		sup_details nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.sup_details) 
	WITH
	(
		isup_rev nvarchar(max) as JSON
	) As Sup_details
	Cross Apply OPENJSON(Sup_details.isup_rev)
	WITH
	(
		txval decimal(18,2),
	    iamt decimal(18,2),
		camt decimal(18,2) ,
        samt decimal(18,2),
        csamt decimal(18,2)
  	) As Isup_rev
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 5

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 txval)	
	Select	@Gstin,t1.gstinid ,@Fp,5 ,
			txval
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		sup_details nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.sup_details) 
	WITH
	(
		osup_nongst nvarchar(max) as JSON
	) As Sup_details
	Cross Apply OPENJSON(Sup_details.osup_nongst)
	WITH
	(
		txval decimal(18,2)
  	) As Osup_nongst
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 6

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 pos,txval,iamt)	
	Select	@Gstin,t1.gstinid ,@Fp,6 ,
			pos,txval,iamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		inter_sup nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.inter_sup) 
	WITH
	(
		unreg_details nvarchar(max) as JSON
	) As Inter_sup
	Cross Apply OPENJSON(Inter_sup.unreg_details)
	WITH
	(
		pos varchar(2),
		txval decimal(18,2),
	    iamt decimal(18,2)
  	) As Unreg_details
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 7

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 pos,txval,iamt)	
	Select	@Gstin,t1.gstinid ,@Fp,7 ,
			pos,txval,iamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		inter_sup nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.inter_sup) 
	WITH
	(
		comp_details nvarchar(max) as JSON
	) As Inter_sup
	Cross Apply OPENJSON(Inter_sup.comp_details)
	WITH
	(
		pos varchar(2),
		txval decimal(18,2),
	    iamt decimal(18,2)
  	) As Comp_details
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 8

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 pos,txval,iamt)	
	Select	@Gstin,t1.gstinid ,@Fp,8 ,
			pos,txval,iamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		inter_sup nvarchar(max) as JSON
	) As Retsum
	Outer Apply OPENJSON(Retsum.inter_sup) 
	WITH
	(
		uin_details nvarchar(max) as JSON
	) As Inter_sup
	Cross Apply OPENJSON(Inter_sup.uin_details)
	WITH
	(
		pos varchar(2),
		txval decimal(18,2),
	    iamt decimal(18,2)
  	) As Uin_details
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 19, 20

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,ty,
	 interstatesupplies,intrastatesupplies)	
	Select	@Gstin,t1.gstinid ,@Fp,19 ,ty,
			inter,intra
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		inward_sup nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.inward_sup) 
	WITH
	(
		isup_details nvarchar(max) as JSON
	) As Inward_Sup
	Cross Apply OPENJSON(Inward_Sup.isup_details)
	WITH
	(
		ty varchar(10),
	    inter decimal(18,2),
		intra decimal(18,2)
  	) As Isup_Details
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	Update #TBL_GSTR3B_RECS
	Set supply_num = 19
	Where supply_num = 19
	And ty = 'GST'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 20
	Where supply_num = 19
	And ty = 'NONGST'

	-- Supply_Num : 9, 10,11,12,13

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,ty,
	 iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,9 ,ty,
			iamt,camt,samt,csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		itc_elg nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.itc_elg) 
	WITH
	(
		itc_avl nvarchar(max) as JSON
	) As Itc_Elg
	Cross Apply OPENJSON(Itc_Elg.itc_avl)
	WITH
	(
		ty varchar(10),
	    iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
  	) As Itc_Avl
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	Update #TBL_GSTR3B_RECS
	Set supply_num = 9
	Where supply_num = 9
	And ty = 'IMPG'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 10
	Where supply_num = 9
	And ty = 'IMPS'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 11
	Where supply_num = 9
	And ty = 'ISRC'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 12
	Where supply_num = 9
	And ty = 'ISD'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 13
	Where supply_num = 9
	And ty = 'OTH'

	-- Supply_Num : 14,15

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,ty,
	 iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,14 ,ty,
			iamt,camt,samt,csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		itc_elg nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.itc_elg) 
	WITH
	(
		itc_rev nvarchar(max) as JSON
	) As Itc_Elg
	Cross Apply OPENJSON(Itc_Elg.itc_rev)
	WITH
	(
		ty varchar(10),
	    iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
  	) As Itc_Rev
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	Update #TBL_GSTR3B_RECS
	Set supply_num = 14
	Where supply_num = 14
	And ty = 'RUL'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 15
	Where supply_num = 14
	And ty = 'OTH'

	-- Supply_Num : 16

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,16 ,
			iamt,camt,samt,csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		itc_elg nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.itc_elg) 
	WITH
	(
		itc_net nvarchar(max) as JSON
	) As Itc_Elg
	Cross Apply OPENJSON(Itc_Elg.itc_net)
	WITH
	(
	    iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
  	) As Itc_Net
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Supply_Num : 17,18

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,ty,
	 iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,17 ,ty,
			iamt,camt,samt,csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		itc_elg nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.itc_elg) 
	WITH
	(
		itc_inelg nvarchar(max) as JSON
	) As Itc_Elg
	Cross Apply OPENJSON(Itc_Elg.itc_inelg)
	WITH
	(
		ty varchar(10),
	    iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
  	) As Itc_Inelg
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	Update #TBL_GSTR3B_RECS
	Set supply_num = 17
	Where supply_num = 17
	And ty = 'RUL'

	Update #TBL_GSTR3B_RECS
	Set supply_num = 18
	Where supply_num = 17
	And ty = 'OTH'

	-- Supply_Num : 21

	Insert Into #TBL_GSTR3B_RECS
	(gstin,gstinid,fp,supply_num,
	 iamt,camt,samt,csamt)	
	Select	@Gstin,t1.gstinid ,@Fp,21 ,
			iamt,camt,samt,csamt
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		intr_ltfee nvarchar(max) as JSON
	) As  Retsum
	Outer Apply OPENJSON( Retsum.intr_ltfee) 
	WITH
	(
		intr_details nvarchar(max) as JSON
	) As Intr_Ltfee
	Cross Apply OPENJSON(Intr_Ltfee.intr_details)
	WITH
	(
	    iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
  	) As Intr_Details
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1


	-- Delete The Records

	Select gstr3bdid
	Into #TBL_GSTR3B_DET
	From TBL_GSTR3B_D
	Where gstin = @Gstin
	And ret_period = @Fp

	if Exists(Select 1 From #TBL_GSTR3B_DET)
	Begin

		Delete From TBL_GSTR3B_D_sup_det_osup_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_sup_det_osup_zero
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_sup_det_osup_nil_exmp
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_sup_det_isup_rev 
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_sup_det_osup_nongst
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_sup_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inter_sup_unreg_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inter_sup_comp_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inter_sup_uin_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inter_sup
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inward_sup_isup_details
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_inward_sup
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_itc_elg_itc_avl
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_itc_elg_itc_inelg
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_itc_elg_itc_net
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_itc_elg_itc_rev
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_itc_elg
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)


		Delete From TBL_GSTR3B_D_intr_ltfee_intr_det
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete From TBL_GSTR3B_D_intr_ltfee
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)

		Delete from TBL_GSTR3B_D
		Where gstr3bdid In (Select gstr3bdid From #TBL_GSTR3B_DET)


	End

	-- Insert Records into Table TBL_GSTR3B_D

	Insert TBL_GSTR3B_D (gstin,gstinId,ret_period)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR3B_RECS t1
	Where Not Exists(Select 1 From TBL_GSTR3B_D t2 Where t2.gstin = t1.gstin and t2.ret_period = t1.fp)

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.gstr3bdid = t2.gstr3bdid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.ret_period

	-- Insert Records into Table TBL_GSTR3B_D_sup_det

	Insert TBL_GSTR3B_D_sup_det(gstr3bdid,gstinid) 
	Select	distinct gstr3bdid,gstinid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num In (1,2,3,4,5)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det t2
					   Where t2.gstr3bdid = t1.gstr3bdid
		  		    ) 

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.sup_detid = t2.sup_detid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D_sup_det t2 
	WHERE t1.gstr3bdid = t2.gstr3bdid 

	-- Insert Records into Table TBL_GSTR3B_D_sup_det_osup_det

	Insert TBL_GSTR3B_D_sup_det_osup_det
	(sup_detid,
		txval,iamt,camt,samt,csamt,gstinid,gstr3bdid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 1
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det_osup_det t2
						Where t2.sup_detid = t1.sup_detid 
						)
						
	-- Insert Records into Table TBL_GSTR3B_D_sup_det_osup_zero

	Insert TBL_GSTR3B_D_sup_det_osup_zero
	(sup_detid,
		txval,iamt,csamt,gstinid,gstr3bdid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.csamt,t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 2
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det_osup_zero t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_sup_det_osup_nil_exmp
	
	Insert TBL_GSTR3B_D_sup_det_osup_nil_exmp
	(sup_detid,
		txval,gstinid,gstr3bdid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 3
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det_osup_nil_exmp t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_sup_det_isup_rev

	Insert TBL_GSTR3B_D_sup_det_isup_rev
	(sup_detid,
		txval,iamt,camt,samt,csamt,gstinid,gstr3bdid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 4
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det_isup_rev t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_sup_det_osup_nongst

	Insert TBL_GSTR3B_D_sup_det_osup_nongst
	(sup_detid,
		txval,gstinid,gstr3bdid)
	Select	distinct t1.sup_detid,
			t1.txval,t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 5
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_sup_det_osup_nongst t2
						Where t2.sup_detid = t1.sup_detid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_inter_sup

	Insert TBL_GSTR3B_D_inter_sup(gstr3bdid,gstinid) 
	Select	distinct gstr3bdid,gstinid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num In (6,7,8)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inter_sup t2
					   Where t2.gstr3bdid = t1.gstr3bdid
		  		    ) 

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.inter_supid = t2.inter_supid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D_inter_sup t2 
	WHERE t1.gstr3bdid = t2.gstr3bdid 

	-- Insert Records into Table TBL_GSTR3B_D_inter_sup_unreg_det

	Insert TBL_GSTR3B_D_inter_sup_unreg_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bdid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 6
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inter_sup_unreg_det t2
						Where t2.inter_supid = t1.inter_supid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_inter_sup_comp_det

	Insert TBL_GSTR3B_D_inter_sup_comp_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bdid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 7
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inter_sup_comp_det t2
						Where t2.inter_supid = t1.inter_supid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_inter_sup_uin_det

	Insert TBL_GSTR3B_D_inter_sup_uin_det
	(inter_supid,pos,txval,iamt,
		gstinid,gstr3bdid)
	Select	distinct t1.inter_supid,t1.pos,t1.txval,t1.iamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 8
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inter_sup_uin_det t2
						Where t2.inter_supid = t1.inter_supid 
						)

	-- Insert Records into Table TBL_GSTR3B_D_inward_sup

	Insert TBL_GSTR3B_D_inward_sup(gstr3bdid,gstinid) 
	Select	distinct gstr3bdid,gstinid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num In (19,20)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inward_sup t2
					   Where t2.gstr3bdid = t1.gstr3bdid
		  		    ) 

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.inward_supid = t2.inward_supid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D_inward_sup t2 
	WHERE t1.gstr3bdid = t2.gstr3bdid 

	-- Insert Records into Table TBL_GSTR3B_D_inward_sup_isup_details

	Insert TBL_GSTR3B_D_inward_sup_isup_details
	(inward_supid,ty,inter,intra,
		gstinid,gstr3bdid)
	Select	distinct t1.inward_supid,'GST',t1.interstatesupplies,t1.intrastatesupplies,
			t1.gstinid,t1.gstr3bdid
	From  #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 19
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inward_sup_isup_details t2
						Where t2.inward_supid = t1.inward_supid 
						And t2.ty = 'GST'
						)

	Insert TBL_GSTR3B_D_inward_sup_isup_details
	(inward_supid,ty,inter,intra,
		gstinid,gstr3bdid)
	Select	distinct t1.inward_supid,'NONGST',t1.interstatesupplies,t1.intrastatesupplies,
			t1.gstinid,t1.gstr3bdid
	From  #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 20
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_inward_sup_isup_details t2
						Where t2.inward_supid = t1.inward_supid 
						And t2.ty = 'NONGST'
						)

	-- Insert Records into Table TBL_GSTR3B_D_itc_elg

	Insert TBL_GSTR3B_D_itc_elg(gstr3bdid,gstinid) 
	Select	distinct gstr3bdid,gstinid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num In (9,10,11,12,13,14,15,16,17,18)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg t2
					   Where t2.gstr3bdid = t1.gstr3bdid
		  		    ) 

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.itc_elgid = t2.itc_elgid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D_itc_elg t2 
	WHERE t1.gstr3bdid = t2.gstr3bdid 

	-- Insert Records into Table TBL_GSTR3B_D_itc_elg_itc_avl

	Insert TBL_GSTR3B_D_itc_elg_itc_avl
	(itc_elgid,ty,iamt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'IMPG',t1.iamt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 9
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'IMPG'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_avl
	(itc_elgid,ty,iamt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'IMPS',t1.iamt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 10
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'IMPS'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'ISRC',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 11
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'ISRC'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'ISD',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 12
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'ISD'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_avl
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 13
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_avl t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

						
	Insert TBL_GSTR3B_D_itc_elg_itc_rev
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'RUL',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 14
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_rev t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'RUL'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_rev
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 15
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_rev t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_net
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 16
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_net t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = ''
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_inelg
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'RUL',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 17
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_inelg t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'RUL'
						)

	Insert TBL_GSTR3B_D_itc_elg_itc_inelg
	(itc_elgid,ty,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.itc_elgid,'OTH',t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 18
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_itc_elg_itc_inelg t2
						Where t2.itc_elgid = t1.itc_elgid 
						And t2.ty = 'OTH'
						)

	-- Insert Records into Table TBL_GSTR3B_D_intr_ltfee

	Insert TBL_GSTR3B_D_intr_ltfee(gstr3bdid,gstinid) 
	Select	distinct gstr3bdid,gstinid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num In (21)
	And  Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_intr_ltfee t2
					   Where t2.gstr3bdid = t1.gstr3bdid
		  		    ) 

	Update #TBL_GSTR3B_RECS
	SET #TBL_GSTR3B_RECS.intr_ltfeeid = t2.intr_ltfeeid 
	FROM #TBL_GSTR3B_RECS t1,
		TBL_GSTR3B_D_intr_ltfee t2 
	WHERE t1.gstr3bdid = t2.gstr3bdid 

	-- Insert Records into Table TBL_GSTR3B_D_intr_ltfee_intr_det

	Insert TBL_GSTR3B_D_intr_ltfee_intr_det
	(intr_ltfeeid,iamt,camt,samt,csamt,
		gstinid,gstr3bdid)
	Select	distinct t1.intr_ltfeeid,t1.iamt,t1.camt,t1.samt,t1.csamt,
			t1.gstinid,t1.gstr3bdid
	From #TBL_GSTR3B_RECS t1
	Where t1.supply_num = 21
	And	Not Exists ( SELECT 1 FROM TBL_GSTR3B_D_intr_ltfee_intr_det t2
						Where t2.intr_ltfeeid = t1.intr_ltfeeid 
						)

	Select * From  #TBL_GSTR3B_RECS

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End