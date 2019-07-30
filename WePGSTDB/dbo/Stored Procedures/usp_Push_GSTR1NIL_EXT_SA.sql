
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 NIL Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/04/2017	Seshadri	Fixed Flag Issue
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 


*/

/* Sample Procedure Call

exec usp_Push_GSTR1NIL_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1NIL_EXT_SA]
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
	space(50) as nilid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.sply_ty,
	t1.nil_amt,t1.expt_amt,t1.ngsup_amt
	Into #TBL_EXT_GSTR1_NIL
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			sply_ty,
			sum(nil_amt) as nil_amt, sum(expt_amt) as expt_amt, sum(ngsup_amt) as ngsup_amt
		FROM TBL_EXT_GSTR1_NIL
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,sply_ty
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_NIL t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_NIL 
	SET #TBL_EXT_GSTR1_NIL.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_NIL t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_NIL

	Insert TBL_GSTR1_NIL(gstr1id,gstinId,custid,createdby,createddate) 
	Select	distinct gstr1Id,gstinId,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_NIL t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_NIL t2 where t2.gstr1id = t1.gstr1id
								And Isnull(t2.flag,'') = '' ) 

	Update #TBL_EXT_GSTR1_NIL
	SET #TBL_EXT_GSTR1_NIL.nilid = t2.nilid 
	FROM #TBL_EXT_GSTR1_NIL t1,
			TBL_GSTR1_NIL t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And Isnull(t2.flag,'') = ''

	-- Insert Records into Table TBL_GSTR1_NIL_INV

	Insert TBL_GSTR1_NIL_INV
	(nilid,sply_ty,
		nil_amt,expt_amt,ngsup_amt,gstinId,gstr1id)
	Select	distinct t1.nilid,t1.sply_ty,
			t1.nil_amt,t1.expt_amt,t1.ngsup_amt,t1.gstinId,t1.gstr1id
	From #TBL_EXT_GSTR1_NIL t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_NIL_INV t2
						Where t2.nilid = t1.nilid 
							And t2.sply_ty = t1.sply_ty
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR1_NIL)
	Begin
		Update TBL_EXT_GSTR1_NIL 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End