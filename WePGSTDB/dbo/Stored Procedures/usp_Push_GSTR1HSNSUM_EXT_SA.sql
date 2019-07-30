
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 HSN Summary Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/14/2017	Seshadri	Initial Version
12/20/2017	Seshadri	Fixed the issue of duplicate HSN Issue
12/24/2017	Seshadri	Fixed Integration Testing Defects
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 


*/

/* Sample Procedure Call

exec usp_Push_GSTR1HSNSUM_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1HSNSUM_EXT_SA]
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
	space(50) as hsnid,
	space(50) as descs,
	space(50) as uqc,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.hsn_sc,
	t1.qty,t1.val,
	t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_HSN
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			hsn_sc,
			sum(qty) as qty,
			sum(val) as val,
			sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_HSN 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,hsn_sc
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Update HSN Description and UQC

	Update #TBL_EXT_GSTR1_HSN 
	SET descs = (Select Top 1 descs From TBL_EXT_GSTR1_HSN t2 
				 Where t2.gstin = t1.gstin
				 And t2.fp = t1.fp
				 And t2.hsn_sc = t1.hsn_sc)
	FROM #TBL_EXT_GSTR1_HSN t1

	Update #TBL_EXT_GSTR1_HSN 
	SET uqc = (Select Top 1 uqc From TBL_EXT_GSTR1_HSN t2 
				 Where t2.gstin = t1.gstin
				 And t2.fp = t1.fp
				 And t2.hsn_sc = t1.hsn_sc)
	FROM #TBL_EXT_GSTR1_HSN t1

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_HSN t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_HSN 
	SET #TBL_EXT_GSTR1_HSN.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_HSN t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_HSN

	Insert TBL_GSTR1_HSN(gstr1id,gstinId) 
	Select	distinct gstr1Id,gstinId
	From #TBL_EXT_GSTR1_HSN t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_HSN t2 where t2.gstr1id = t1.gstr1id ) 

	Update #TBL_EXT_GSTR1_HSN
	SET #TBL_EXT_GSTR1_HSN.hsnid = t2.hsnid 
	FROM #TBL_EXT_GSTR1_HSN t1,
			TBL_GSTR1_HSN t2 
	WHERE t1.gstr1id = t2.gstr1id 

	-- Insert Records into Table TBL_GSTR1_HSN_DATA

	Update TBL_GSTR1_HSN_DATA 
	Set	qty = IsNull(t1.qty,0) + IsNull(t2.qty,0),
		val = IsNull(t1.val,0) + IsNull(t2.val,0),
		txval = IsNull(t1.txval,0) + IsNull(t2.txval,0), 
		iamt = IsNull(t1.iamt,0) + IsNull(t2.iamt,0),
		camt = IsNull(t1.camt,0) + IsNull(t2.camt,0),
		samt = IsNull(t1.samt,0) + IsNull(t2.samt,0), 
		csamt = IsNull(t1.csamt,0) + IsNull(t2.csamt,0) 
	From TBL_GSTR1_HSN_DATA t1,
		 #TBL_EXT_GSTR1_HSN t2
	Where t2.gstr1id = t1.gstr1id
	And t2.hsnid = t1.hsnid
	And t2.hsn_sc = t1.hsn_sc
	And Isnull(t1.flag,'') = ''

	Insert TBL_GSTR1_HSN_DATA
	(hsnid,hsn_sc,[desc],uqc,qty,val,
		txval,iamt,camt,samt,csamt,gstinId,gstr1id,
		custid,createdby,createddate)
	Select	distinct t1.hsnid,t1.hsn_sc,t1.descs,uqc,qty,val,
			txval,iamt,camt,samt,csamt,gstinId,gstr1id,
			@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_HSN t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_HSN_DATA t2
						Where t2.gstr1id = t1.gstr1id
						And t2.hsnid = t1.hsnid 
						And t2.hsn_sc = t1.hsn_sc
						And Isnull(t2.flag,'') = '')	

	
	/*
	Insert TBL_GSTR1_HSN_DATA
	(hsnid,hsn_sc,[desc],uqc,qty,val,
		txval,iamt,camt,samt,csamt,gstinId,gstr1id)
	Select	distinct t1.hsnid,t1.hsn_sc,t1.descs,uqc,qty,val,
			txval,iamt,camt,samt,csamt,gstinId,gstr1id
	From #TBL_EXT_GSTR1_HSN t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_HSN_DATA t2
						Where t2.hsnid = t1.hsnid 
							And t2.hsn_sc = t1.hsn_sc
							And t2.qty = t1.qty
							And t2.val = t1.val
							And Isnull(t2.flag,'') = '')	
							
	*/	 

	If Exists(Select 1 From #TBL_EXT_GSTR1_HSN)
	Begin
		Update TBL_EXT_GSTR1_HSN 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End