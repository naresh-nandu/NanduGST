
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 TXP Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/04/2017	Seshadri	Fixed Flag Issue
09/07/2017	Seshadri	Fixed TXP Push Error 
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 
03/01/2018	Seshadri	Fixed Tax value updation issue

*/

/* Sample Procedure Call

exec usp_Push_GSTR1TXP_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1TXP_EXT_SA]
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
	space(50) as txpid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.pos,t1.sply_ty,
	t1.rt, t1.ad_amt, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_TXP
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			pos,sply_ty,
			rt,sum(ad_amt) as ad_amt, 
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_TXP
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,pos,sply_ty,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2 

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_TXP t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_TXP
	SET #TBL_EXT_GSTR1_TXP.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_TXP t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_TXP

	Insert TBL_GSTR1_TXP(gstr1id,pos,sply_ty,gstinId,
						custid,createdby,createddate) 
	Select	distinct gstr1Id , pos, sply_ty,gstinId,
					@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_TXP t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_TXP t2 
						Where t2.gstr1id = t1.gstr1id 
						And t2.pos = t1.pos
					    And t2.sply_ty = t1.sply_ty
						And Isnull(t2.flag,'') = '') 

	Update #TBL_EXT_GSTR1_TXP
	SET #TBL_EXT_GSTR1_TXP.txpid = t2.txpid 
	FROM #TBL_EXT_GSTR1_TXP t1,
			TBL_GSTR1_TXP t2 
	WHERE t1.gstr1id = t2.gstr1id 
	 And t2.pos = t1.pos
    And t2.sply_ty = t1.sply_ty	  
	And Isnull(t2.flag,'') = ''

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin
		Update #TBL_EXT_GSTR1_TXP
		Set iamt=(convert(decimal(18,2),ad_amt*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_TXP 
		Set camt = (convert(decimal(18,2),(ad_amt*rt/100)/2)), 
			samt = (convert(decimal(18,2),(ad_amt*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	-- Insert Records into Table TBL_GSTR1_TXP_ITMS

	Insert TBL_GSTR1_TXP_ITMS
	(txpid,rt,
		ad_amt,iamt,camt,samt,csamt,gstinId,gstr1id)
	Select	distinct t1.txpid,t1.rt,
			t1.ad_amt,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinId,t1.gstr1id
	From #TBL_EXT_GSTR1_TXP t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_TXP_ITMS t2
						Where t2.txpid = t1.txpid 
							And t2.rt = t1.rt
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR1_TXP)
	Begin
		Update TBL_EXT_GSTR1_TXP 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End