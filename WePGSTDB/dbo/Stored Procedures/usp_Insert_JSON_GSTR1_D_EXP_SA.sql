
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D EXP JSON Records to the corresponding statging area
			  tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri			Initial Version
08/10/2017	Seshadri	Included the Where Clause while inserting Invoice Items 
08/22/2017	Seshadri	Modified the code to correct insertion errors
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_EXP_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_EXP_SA]
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
			space(50) as expid,
			space(50) as invid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			exp_typ as exp_typ,
			chksum as chksum,
			inum as inum,
			idt as idt,
			val as val,
			sbpcode as sbpcode,
			sbnum as sbnum,
			sbdt as sbdt,
			txval as txval,
			rt as rt,
			iamt as iamt
	Into #TBL_GSTR1_D_EXP
	From OPENJSON(@RecordContents) 
	WITH
	(
	  [exp] nvarchar(max) as JSON
	) As Exps
	Cross Apply OPENJSON(Exps.[exp]) 
	WITH
	(
		exp_typ varchar(15),
		inv nvarchar(max) as JSON
	) As ExpTypes
	Cross Apply OPENJSON(ExpTypes.inv) 
	WITH
	(
		chksum varchar(64),
		inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
		sbpcode varchar(50),
		sbnum varchar(50),
		sbdt varchar(50),
    	itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		txval decimal(18,2),
		rt decimal(18,2),
		iamt decimal(18,2)
	) As Itm_det
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From  #TBL_GSTR1_D_EXP t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update  #TBL_GSTR1_D_EXP 
	SET  #TBL_GSTR1_D_EXP.gstr1did = t2.gstr1did 
	FROM  #TBL_GSTR1_D_EXP t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_EXP

	Insert TBL_GSTR1_D_EXP(gstr1did,ex_tp,gstinId) 
	Select	distinct gstr1did,exp_typ,gstinId
	From #TBL_GSTR1_D_EXP t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_EXP t2 where t2.gstr1did = t1.gstr1did and t2.ex_tp = t1.exp_typ) 

	Update #TBL_GSTR1_D_EXP 
	SET #TBL_GSTR1_D_EXP.expid = t2.expid
	FROM #TBL_GSTR1_D_EXP t1,
		TBL_GSTR1_D_EXP t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And t1.exp_typ = t2.ex_tp 

	-- Insert Records into Table TBL_GSTR1_D_EXP_INV

	Insert TBL_GSTR1_D_EXP_INV
	(expid,chksum,inum,idt,val,sbpcode,sbnum,sbdt,gstinId,gstr1did)
	Select	distinct t1.expid,t1.chksum,t1.inum,t1.idt,t1.val,
			t1.sbpcode,t1.sbnum,t1.sbdt,t1.gstinId,t1.gstr1did
	From #TBL_GSTR1_D_EXP t1
	Where	Not Exists (	SELECT 1 FROM TBL_GSTR1_D_EXP_INV t2
							Where t2.expid = t1.expid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And(t2.flag is Null Or t2.flag <> 'D')
						)		 

	Update #TBL_GSTR1_D_EXP 
	SET #TBL_GSTR1_D_EXP.invid = t2.invid 
	FROM #TBL_GSTR1_D_EXP t1,
			TBL_GSTR1_D_EXP_INV t2 
	WHERE t1.expid= t2.expid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And(t2.flag is Null Or t2.flag <> 'D')

	Insert TBL_GSTR1_D_EXP_INV_ITMS
	(invid,rt,txval,iamt,gstinId,gstr1did)
	Select invid,rt,txval,iamt,gstinId,gstr1did
	From #TBL_GSTR1_D_EXP t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_EXP_INV_ITMS t2
						Where t2.invid = t1.invid 
							And t2.rt = t1.rt
							And t2.txval = t1.txval)
	


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End