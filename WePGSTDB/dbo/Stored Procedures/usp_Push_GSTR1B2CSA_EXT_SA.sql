
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 B2CSA Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/15/2018	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR1B2CSA_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1B2CSA_EXT_SA]
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
	space(50) as b2csaid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.sply_ty,t1.typ,t1.etin,t1.pos,
	t1.omon,t1.diff_percent,
	t1.rt, t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_B2CSA 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			sply_ty,typ,etin,pos,
			omon,diff_percent,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt,sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_B2CSA 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,sply_ty,typ,etin,pos,
				omon,diff_percent,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_B2CSA t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_B2CSA 
	SET #TBL_EXT_GSTR1_B2CSA.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_B2CSA t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_B2CSA

	Insert TBL_GSTR1_B2CSA(gstr1id,sply_ty,typ,etin,pos,gstinid,
							custid,createdby,createddate,
							omon,diff_percent
							) 
	Select	gstr1Id,sply_ty,typ,etin,pos,gstinid,
			@Custid,@CreatedBy,GetDate(),
			omon,diff_percent 
	From #TBL_EXT_GSTR1_B2CSA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_B2CSA t2 
						where t2.gstr1id = t1.gstr1id 
						and t2.pos = t1.pos
						and t2.sply_ty = t1.sply_ty
						and Isnull(t2.flag,'') = '') 


	Update #TBL_EXT_GSTR1_B2CSA
	SET #TBL_EXT_GSTR1_B2CSA.b2csaid = t2.b2csaid 
	FROM #TBL_EXT_GSTR1_B2CSA t1,
		TBL_GSTR1_B2CSA t2 
	WHERE t1.gstr1id = t2.gstr1id 
    And t2.pos = t1.pos
    And t2.sply_ty = t1.sply_ty	  
	And Isnull(t2.flag,'') = ''

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin			
		Update #TBL_EXT_GSTR1_B2CSA 
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_B2CSA  
		Set camt = (convert(decimal(18,2),(txval*rt/100)/2)), 
			samt = (convert(decimal(18,2),(txval*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	-- Insert Records into Table TBL_GSTR1_B2CSA_ITMS

	Insert TBL_GSTR1_B2CSA_ITMS
	(b2csaid,rt,
		txval,iamt,camt,samt,csamt,gstinid,gstr1id)
	Select	distinct t1.b2csaid,t1.rt,
			t1.txval,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr1id
	From #TBL_EXT_GSTR1_B2CSA t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_B2CSA_ITMS t2
						Where t2.b2csaid = t1.b2csaid 
							And t2.rt = t1.rt
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR1_B2CSA )
	Begin
		Update TBL_EXT_GSTR1_B2CSA
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 
		
	Return 0

End