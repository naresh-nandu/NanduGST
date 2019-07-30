
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 B2B Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
08/11/2017	Seshadri	Changed the logic for updating num value in Invoice Item table
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/04/2017	Seshadri	Fixed Flag Issue
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 
03/01/2018	Seshadri	Fixed Tax value updation issue

*/

/* Sample Procedure Call

exec usp_Push_GSTR1B2B_EXT_SA  'CSV','CSV001', 

 */

CREATE PROCEDURE [usp_Push_GSTR1B2B_EXT_SA]
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
	space(50) as b2bid,
	space(50) as invid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,t1.ctin,
	t1.inum,t1.idt,t1.val,t1.pos,t1.rchrg,t1.etin,t1.inv_typ,
	t1.rt, t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_B2B_INV 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,ctin,
			inum,idt,val,pos,rchrg,etin,inv_typ,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_B2B_INV 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,ctin,inum,idt,val,pos,rchrg,etin,inv_typ,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_B2B_INV t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_B2B_INV 
	SET #TBL_EXT_GSTR1_B2B_INV.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_B2B_INV t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_B2B

	Insert TBL_GSTR1_B2B(gstr1id,ctin,gstinid) 
	Select	distinct gstr1Id,ctin,gstinid
	From #TBL_EXT_GSTR1_B2B_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2B t2 where t2.gstr1id = t1.gstr1id and t2.ctin = t1.ctin) 

	Update #TBL_EXT_GSTR1_B2B_INV 
	SET #TBL_EXT_GSTR1_B2B_INV.b2bid = t2.b2bid 
	FROM #TBL_EXT_GSTR1_B2B_INV t1,
			TBL_GSTR1_B2B t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR1_B2B_INV

	Insert TBL_GSTR1_B2B_INV
	(b2bid,inum,idt,val,pos,rchrg,etin,inv_typ,gstinid,gstr1id,custid,createdby,createddate)
	Select	distinct t1.b2bid,t1.inum,t1.idt,t1.val,
			t1.pos,t1.rchrg,t1.etin,t1.inv_typ,t1.gstinid,t1.gstr1id,
			@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_B2B_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2B_INV t2
						Where t2.b2bid = t1.b2bid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And Isnull(t2.flag,'') = '' )		 


	Update #TBL_EXT_GSTR1_B2B_INV 
	SET #TBL_EXT_GSTR1_B2B_INV.invid = t2.invid 
	FROM #TBL_EXT_GSTR1_B2B_INV t1,
			TBL_GSTR1_B2B_INV t2 
	WHERE t1.b2bid= t2.b2bid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And Isnull(t2.flag,'') = ''
	
	Update #TBL_EXT_GSTR1_B2B_INV 
	SET #TBL_EXT_GSTR1_B2B_INV.num = t3.num
	FROM #TBL_EXT_GSTR1_B2B_INV t1,
			(Select slno,invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_EXT_GSTR1_B2B_INV t2
	   		) t3
	Where t1.invid = t3.invid 
	And t1.slno = t3.slno

	Insert TBL_GSTR1_B2B_INV_ITMS
	(invid,num,gstinid,gstr1id)
	Select	distinct t1.invid,t1.num,t1.gstinid,gstr1id
	From #TBL_EXT_GSTR1_B2B_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2B_INV_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
	
	Update #TBL_EXT_GSTR1_B2B_INV 
	SET #TBL_EXT_GSTR1_B2B_INV.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR1_B2B_INV t1,
			TBL_GSTR1_B2B_INV_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin			
		Update #TBL_EXT_GSTR1_B2B_INV
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_B2B_INV 
		Set camt = (convert(decimal(18,2),(txval*rt/100)/2)), 
			samt = (convert(decimal(18,2),(txval*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	Insert TBL_GSTR1_B2B_INV_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1id
	From #TBL_EXT_GSTR1_B2B_INV t1

	
	If Exists(Select 1 From #TBL_EXT_GSTR1_B2B_INV )
	Begin
		Update TBL_EXT_GSTR1_B2B_INV 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 

	Return 0

End