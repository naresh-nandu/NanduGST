
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 ATA Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/06/2018	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR1ATA_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1ATA_EXT_SA]
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
	space(50) as ataid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.pos,t1.sply_ty,
	t1.omon,t1.diff_percent,
	t1.rt, t1.ad_amt, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_ATA
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			pos,sply_ty,
			omon,diff_percent,
			rt,sum(ad_amt) as ad_amt, 
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_ATA
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,pos,sply_ty,
				 omon,diff_percent,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_ATA t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_ATA
	SET #TBL_EXT_GSTR1_ATA.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_ATA t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_ATA

	Insert TBL_GSTR1_ATA(gstr1id,pos,sply_ty,gstinid,
						custid,createdby,createddate,
						omon,diff_percent) 
	Select	distinct gstr1Id , pos, sply_ty,gstinid,
					@Custid,@CreatedBy,GetDate(),
					omon,diff_percent
	From #TBL_EXT_GSTR1_ATA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_ATA t2
					   Where t2.gstr1id = t1.gstr1id
					   And t2.pos = t1.pos
					   And t2.sply_ty = t1.sply_ty
					   And Isnull(t2.flag,'') = '' ) 

	Update #TBL_EXT_GSTR1_ATA
	SET #TBL_EXT_GSTR1_ATA.ataid = t2.ataid 
	FROM #TBL_EXT_GSTR1_ATA t1,
			TBL_GSTR1_ATA t2 
	WHERE t1.gstr1id = t2.gstr1id 
    And t2.pos = t1.pos
    And t2.sply_ty = t1.sply_ty	  
	And Isnull(t2.flag,'') = ''

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin
		Update #TBL_EXT_GSTR1_ATA
		Set iamt=(convert(decimal(18,2),ad_amt*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_ATA 
		Set camt = (convert(decimal(18,2),(ad_amt*rt/100)/2)), 
			samt = (convert(decimal(18,2),(ad_amt*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	-- Insert Records into Table TBL_GSTR1_AT_ITMS

	Insert TBL_GSTR1_ATA_ITMS
	(ataid,rt,
		ad_amt,iamt,camt,samt,csamt,gstinid,gstr1id)
	Select	distinct t1.ataid,t1.rt,
			t1.ad_amt,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr1id
	From #TBL_EXT_GSTR1_ATA t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_ATA_ITMS t2
						Where t2.ataid = t1.ataid 
							And t2.rt = t1.rt
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR1_ATA )
	Begin
		Update TBL_EXT_GSTR1_ATA 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 
		
	Return 0

End