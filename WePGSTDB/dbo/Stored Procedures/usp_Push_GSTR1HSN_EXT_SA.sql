
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 HSN Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
08/11/2017	Seshadri	Changed the grouping logic
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/04/2017	Seshadri	Fixed Flag Issue
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 

*/

/* Sample Procedure Call

exec usp_Push_GSTR1HSN_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1HSN_EXT_SA]
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
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.hsn_sc,t1.descs,t1.uqc,t1.qty,t1.val,
	t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_HSN
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			hsn_sc,descs,uqc,
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
		Group By gstin,fp,gt,cur_gt,hsn_sc,descs,uqc,qty,val
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

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

	Insert TBL_GSTR1_HSN_DATA
	(hsnid,hsn_sc,[desc],uqc,qty,val,
		txval,iamt,camt,samt,csamt,gstinId,gstr1id,
		custid,createdby,createddate)
	Select	distinct t1.hsnid,t1.hsn_sc,t1.descs,uqc,qty,val,
			txval,iamt,camt,samt,csamt,gstinId,gstr1id,
			@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_HSN t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_HSN_DATA t2
						Where t2.hsnid = t1.hsnid 
							And t2.hsn_sc = t1.hsn_sc
							And t2.qty = t1.qty
							And t2.val = t1.val
							And Isnull(t2.flag,'') = '')		 

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