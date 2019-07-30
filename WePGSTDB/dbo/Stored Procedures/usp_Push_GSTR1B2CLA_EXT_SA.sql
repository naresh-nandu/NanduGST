﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 B2CLA Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/06/2018	Seshadri	Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR1B2CLA_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1B2CLA_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	Row_Number() OVER(Order by GSTIN ASC) as slno,
	space(50) as gstr1id,
	space(50) as b2claid,
	space(50) as invid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,t1.pos,
	t1.inum,t1.idt,t1.val,t1.etin,
	t1.oinum,t1.oidt,t1.diff_percent,
	t1.rt, t1.txval, 
	t1.iamt,t1.csamt
	Into #TBL_EXT_GSTR1_B2CLA_INV 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,pos,
			inum,idt,val,etin,
			oinum,oidt,diff_percent,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_B2CLA_INV 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,pos,inum,idt,val,etin,
				 oinum,oidt,diff_percent,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_B2CLA_INV t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_B2CLA_INV 
	SET #TBL_EXT_GSTR1_B2CLA_INV.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_B2CLA_INV t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_B2CLA

	Insert TBL_GSTR1_B2CLA(gstr1id,pos,gstinid) 
	Select	distinct gstr1Id,pos,gstinid 
	From #TBL_EXT_GSTR1_B2CLA_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2CLA t2 where t2.gstr1id = t1.gstr1id and t2.pos = t1.pos) 

	Update #TBL_EXT_GSTR1_B2CLA_INV 
	SET #TBL_EXT_GSTR1_B2CLA_INV.b2claid = t2.b2claid 
	FROM #TBL_EXT_GSTR1_B2CLA_INV t1,
			TBL_GSTR1_B2CLA t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And t1.pos = t2.pos 

	-- Insert Records into Table TBL_GSTR1_B2CLA_INV

	Insert TBL_GSTR1_B2CLA_INV
	(b2claid,inum,idt,val,etin,gstinid,gstr1id,custid,createdby,createddate,
	oinum,oidt,diff_percent)
	Select	distinct t1.b2claid,t1.inum,t1.idt,t1.val,t1.etin,t1.gstinid,t1.gstr1id,
					@Custid,@CreatedBy,GetDate(),
					t1.oinum,t1.oidt,t1.diff_percent
	From #TBL_EXT_GSTR1_B2CLA_INV t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_B2CLA_INV t2
						Where t2.b2claid = t1.b2claid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR1_B2CLA_INV 
	SET #TBL_EXT_GSTR1_B2CLA_INV.invid = t2.invid 
	FROM #TBL_EXT_GSTR1_B2CLA_INV t1,
			TBL_GSTR1_B2CLA_INV t2 
	WHERE t1.b2claid= t2.b2claid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR1_B2CLA_INV 
	SET #TBL_EXT_GSTR1_B2CLA_INV.num = t3.num
	FROM #TBL_EXT_GSTR1_B2CLA_INV t1,
			(Select slno,invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_EXT_GSTR1_B2CLA_INV t2
	   		) t3
	Where t1.invid = t3.invid
	And t1.slno = t3.slno

	Insert TBL_GSTR1_B2CLA_INV_ITMS
	(invid,num,gstinid,gstr1id)
	Select	distinct t1.invid,t1.num,t1.gstinid,t1.gstr1id
	From #TBL_EXT_GSTR1_B2CLA_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2CLA_INV_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
		
	
	Update #TBL_EXT_GSTR1_B2CLA_INV 
	SET #TBL_EXT_GSTR1_B2CLA_INV.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR1_B2CLA_INV t1,
			TBL_GSTR1_B2CLA_INV_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin	
		Update #TBL_EXT_GSTR1_B2CLA_INV 
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0
	End

	Insert TBL_GSTR1_B2CLA_INV_ITMS_DET
	(itmsid,rt,txval,iamt,csamt,gstinid,gstr1id)
	Select itmsid,rt,txval,iamt,csamt,gstinid,gstr1id
	From #TBL_EXT_GSTR1_B2CLA_INV t1
	
	If Exists(Select 1 From #TBL_EXT_GSTR1_B2CLA_INV )
	Begin
		Update TBL_EXT_GSTR1_B2CLA_INV 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 

		
	Return 0

End