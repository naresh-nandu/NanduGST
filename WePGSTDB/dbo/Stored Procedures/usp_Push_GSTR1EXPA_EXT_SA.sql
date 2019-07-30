
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 EXPA Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/06/2018	Seshadri	Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR1EXPA_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1EXPA_EXT_SA]
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
	space(50) as gstr1id,
	space(50) as expaid,
	space(50) as invid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,t1.exp_typ,
	t1.inum,t1.idt,t1.val,t1.sbpcode,t1.sbnum,t1.sbdt,
	t1.oinum,t1.oidt,t1.diff_percent,
	t1.rt, t1.txval, 
	t1.iamt, t1.csamt  
	Into #TBL_EXT_GSTR1_EXPA_INV 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,exp_typ,
			inum,idt,val,sbpcode,sbnum,sbdt,
			oinum,oidt,diff_percent,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(csamt) as csamt  
		FROM TBL_EXT_GSTR1_EXPA_INV 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,exp_typ,inum,idt,val,sbpcode,sbnum,sbdt,
				 oinum,oidt,diff_percent,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_EXPA_INV t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_EXPA_INV 
	SET #TBL_EXT_GSTR1_EXPA_INV.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_EXPA_INV t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_EXPA

	Insert TBL_GSTR1_EXPA(gstr1id,ex_tp,gstinId) 
	Select	distinct gstr1Id,exp_typ,gstinId
	From #TBL_EXT_GSTR1_EXPA_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_EXPA t2 where t2.gstr1id = t1.gstr1id and t2.ex_tp = t1.exp_typ) 

	Update #TBL_EXT_GSTR1_EXPA_INV 
	SET #TBL_EXT_GSTR1_EXPA_INV.expaid = t2.expaid
	FROM #TBL_EXT_GSTR1_EXPA_INV t1,
			TBL_GSTR1_EXPA t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And t1.exp_typ = t2.ex_tp 

	-- Insert Records into Table TBL_GSTR1_EXPA_INV

	Insert TBL_GSTR1_EXPA_INV
	(expaid,inum,idt,val,sbpcode,sbnum,sbdt,gstinId,gstr1id,
		custid,createdby,createddate,
		oinum,oidt,diff_percent)
	Select	distinct t1.expaid,t1.inum,t1.idt,t1.val,
			t1.sbpcode,t1.sbnum,t1.sbdt,t1.gstinId,t1.gstr1id,
			@Custid,@CreatedBy,GetDate(),
			t1.oinum,t1.oidt,t1.diff_percent
	From #TBL_EXT_GSTR1_EXPA_INV t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_EXPA_INV t2
						Where t2.expaid = t1.expaid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR1_EXPA_INV 
	SET #TBL_EXT_GSTR1_EXPA_INV.invid = t2.invid 
	FROM #TBL_EXT_GSTR1_EXPA_INV t1,
			TBL_GSTR1_EXPA_INV t2 
	WHERE t1.expaid= t2.expaid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And Isnull(t2.flag,'') = ''

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin			
		Update #TBL_EXT_GSTR1_EXPA_INV
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0
	End

	Insert TBL_GSTR1_EXPA_INV_ITMS
	(invid,rt,txval,iamt,csamt,gstinId,gstr1id)
	Select invid,rt,txval,iamt,csamt,gstinId,gstr1id
	From #TBL_EXT_GSTR1_EXPA_INV t1
	
	If Exists(Select 1 From #TBL_EXT_GSTR1_EXPA_INV)
	Begin
		Update TBL_EXT_GSTR1_EXPA_INV 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End