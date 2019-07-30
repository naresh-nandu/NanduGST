
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 NIL Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.

*/

/* Sample Procedure Call

exec usp_Push_GSTR2NIL_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2NIL_EXT_SA]
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
	space(50) as gstr2id,
	space(50) as nilid,
	t1.gstin,t2.gstinid,t1.fp,niltype,
	t1.cpddr,t1.exptdsply,t1.ngsply,t1.nilsply
	Into #TBL_EXT_GSTR2_NIL
	From
	(
		SELECT 
			gstin,fp,niltype,
			sum(cpddr) as cpddr, sum(exptdsply) as exptdsply, sum(ngsply) as ngsply, sum(nilsply) as nilsply
		FROM TBL_EXT_GSTR2_NIL
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,niltype
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_NIL t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_NIL 
	SET #TBL_EXT_GSTR2_NIL.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_NIL t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_NIL

	Insert TBL_GSTR2_NIL(gstr2id,gstinid,custid,createdby,createddate) 
	Select	distinct gstr2Id,gstinid,@Custid,@CreatedBy,GetDate() 
	From #TBL_EXT_GSTR2_NIL t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_NIL t2 where t2.gstr2id = t1.gstr2id ) 

	Update #TBL_EXT_GSTR2_NIL
	SET #TBL_EXT_GSTR2_NIL.nilid = t2.nilid 
	FROM #TBL_EXT_GSTR2_NIL t1,
			TBL_GSTR2_NIL t2 
	WHERE t1.gstr2id = t2.gstr2id 

	-- Insert Records into Table TBL_GSTR1_NIL_INTER

	Insert TBL_GSTR2_NIL_INTER
	(nilid,cpddr,exptdsply,ngsply,nilsply,gstinid,gstr2id)
	Select distinct t1.nilid,t1.cpddr,t1.exptdsply,t1.ngsply,t1.nilsply,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_NIL t1
	Where t1.niltype = 'INTER'

	Insert TBL_GSTR2_NIL_INTRA
	(nilid,cpddr,exptdsply,ngsply,nilsply,gstinid,gstr2id)
	Select distinct t1.nilid,t1.cpddr,t1.exptdsply,t1.ngsply,t1.nilsply,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_NIL t1
	Where t1.niltype = 'INTRA'

	If Exists(Select 1 From #TBL_EXT_GSTR2_NIL)
	Begin
		Update TBL_EXT_GSTR2_NIL 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End