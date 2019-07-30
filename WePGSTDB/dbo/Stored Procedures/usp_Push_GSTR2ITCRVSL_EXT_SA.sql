
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 ITCRVSL Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.

*/

/* Sample Procedure Call

exec usp_Push_GSTR2ITCRVSL_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2ITCRVSL_EXT_SA]
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
	space(50) as gstr2id,
	space(50) as itcrvslid,
	t1.gstin,t2.gstinid,t1.fp,
	t1.ruletype,
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR2_ITCRVSL
	From
	(
		SELECT 
			gstin,fp,
			ruletype,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR2_ITCRVSL
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,ruletype
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_ITCRVSL
	SET #TBL_EXT_GSTR2_ITCRVSL.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_ITCRVSL t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_ITCRVSL

	Insert TBL_GSTR2_ITCRVSL(gstr2id,gstinid,custid,createdby,createddate)   
	Select	distinct gstr2id,gstinid,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_ITCRVSL t2 
					   Where t2.gstr2id = t1.gstr2id 
					   And Isnull(t2.flag,'') = '') 

	Update #TBL_EXT_GSTR2_ITCRVSL
	SET #TBL_EXT_GSTR2_ITCRVSL.itcrvslid = t2.itcrvslid 
	FROM #TBL_EXT_GSTR2_ITCRVSL t1,
			TBL_GSTR2_ITCRVSL t2 
	WHERE t1.gstr2id = t2.gstr2id 
	And Isnull(t2.flag,'') = ''


	-- Insert Records into Table TBL_GSTR2_ITCRVSL_ITMS

	Insert TBL_GSTR2_ITCRVSL_ITMS
	(itcrvslid,rulename,
		iamt,camt,samt,csamt,gstinid,gstr2id)
	Select	distinct t1.itcrvslid,t1.ruletype,
			t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_ITCRVSL_ITMS t2
						Where t2.itcrvslid = t1.itcrvslid 
							And t2.rulename = t1.ruletype
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR2_ITCRVSL)
	Begin
		Update TBL_EXT_GSTR2_ITCRVSL
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End