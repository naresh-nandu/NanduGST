
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D B2B JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri			Initial Version
08/10/2017	Seshadri	Included the Where Clause while inserting Invoice Items 
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_B2B_SA 
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_B2B_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	Select	space(50) as gstr1did,
			space(50) as b2bid,
			space(50) as invid,
			space(50) as num,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			ctin as ctin ,
			cfs as cfs,
			flag as flag,
			updby as updby,
			chksum as chksum,
			inum as inum,
			idt as idt,
			val as val ,
			pos as pos,
			rchrg as rchrg,
			etin as etin,
			inv_typ as inv_typ,
			cflag as cflag,
			opd as opd,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_B2B
	From OPENJSON(@RecordContents) 	
	WITH
	(
		ctin varchar(15),
		cfs varchar(1),
		inv nvarchar(max) as JSON
	) As B2b
	Cross Apply OPENJSON(B2b.inv) 
	WITH
	(
		flag varchar(1),
		updby varchar(1),
		chksum varchar(64),
		inum varchar(50),
		idt varchar(50),
		val decimal(18,2),
		pos varchar(2),
		rchrg varchar(1),
		etin varchar(15),
		inv_typ varchar(1),
		cflag varchar(1),
		opd varchar(6),
		itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		num int,
		itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Itm_Det
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_B2B t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_B2B 
	SET #TBL_GSTR1_D_B2B.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_B2B t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_B2B

	Insert TBL_GSTR1_D_B2B(gstr1did,ctin,cfs,gstinid) 
	Select	distinct gstr1did,ctin,cfs,gstinid
	From #TBL_GSTR1_D_B2B t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_B2B t2 where t2.gstr1did = t1.gstr1did and t2.ctin = t1.ctin) 

	Update #TBL_GSTR1_D_B2B 
	SET  #TBL_GSTR1_D_B2B.b2bid = t2.b2bid 
	FROM  #TBL_GSTR1_D_B2B t1,
			TBL_GSTR1_D_B2B t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR1_D_B2B_INV

	Insert TBL_GSTR1_D_B2B_INV
	(b2bid,flag,updby,chksum,inum,idt,val,pos,rchrg,etin,inv_typ,cflag,opd,gstinid,gstr1did)
	Select	distinct t1.b2bid,t1.flag,t1.updby,t1.chksum,t1.inum,t1.idt,t1.val,
			t1.pos,t1.rchrg,t1.etin,t1.inv_typ,t1.cflag,t1.opd,t1.gstinid,t1.gstr1did
	From #TBL_GSTR1_D_B2B t1
	Where	Not Exists (	SELECT 1 FROM TBL_GSTR1_D_B2B_INV t2
							Where t2.b2bid = t1.b2bid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And(t2.flag is Null Or t2.flag <> 'D')
						)		 


	Update #TBL_GSTR1_D_B2B  
	SET #TBL_GSTR1_D_B2B.invid = t2.invid 
	FROM #TBL_GSTR1_D_B2B t1,
		TBL_GSTR1_D_B2B_INV t2 
	WHERE t1.b2bid= t2.b2bid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And(t2.flag is Null Or t2.flag <> 'D')
	

	Update #TBL_GSTR1_D_B2B  
	SET #TBL_GSTR1_D_B2B.num = t3.num
	FROM #TBL_GSTR1_D_B2B t1,
			(Select invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_GSTR1_D_B2B t2
	   		) t3
	Where t1.invid = t3.invid

	Insert TBL_GSTR1_D_B2B_INV_ITMS
	(invid,num,gstinid,gstr1did)
	Select	distinct t1.invid,t1.num,t1.gstinid,gstr1did
	From  #TBL_GSTR1_D_B2B t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_B2B_INV_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
		
	
	Update #TBL_GSTR1_D_B2B 
	SET #TBL_GSTR1_D_B2B.itmsid = t2.itmsid
	FROM #TBL_GSTR1_D_B2B t1,
		TBL_GSTR1_D_B2B_INV_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num
	
	Insert TBL_GSTR1_D_B2B_INV_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1did)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1did
	From #TBL_GSTR1_D_B2B t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_B2B_INV_ITMS_DET t2
						Where t2.itmsid = t1.itmsid
						and t2.rt = t1.rt)
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End