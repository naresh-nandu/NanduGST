
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 TXI Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.
*/

/* Sample Procedure Call

exec usp_Push_GSTR2TXI_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2TXI_EXT_SA]
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
	space(50) as txiid,
	space(50) as num,
	t1.gstin,t2.gstinid,t1.fp,
	t1.pos,t1.sply_ty,
	t1.rt, t1.adamt, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR2_TXI
	From
	(
		SELECT 
			gstin,fp,
			pos,sply_ty,
			rt,sum(adamt) as adamt, 
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR2_TXI
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,pos,sply_ty,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_TXI t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_TXI
	SET #TBL_EXT_GSTR2_TXI.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_TXI t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_TXI

	Insert TBL_GSTR2_TXI(gstr2id,pos,sply_ty,gstinid,custid,createdby,createddate)  
	Select	distinct gstr2id,pos,sply_ty,gstinid,@Custid,@CreatedBy,GetDate() 
	From #TBL_EXT_GSTR2_TXI t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_TXI t2 
					   Where t2.gstr2id = t1.gstr2id 
					   And t2.pos = t1.pos
					   And t2.sply_ty = t1.sply_ty
					   And Isnull(t2.flag,'') = '') 

	Update #TBL_EXT_GSTR2_TXI
	SET #TBL_EXT_GSTR2_TXI.txiid = t2.txiid 
	FROM #TBL_EXT_GSTR2_TXI t1,
			TBL_GSTR2_TXI t2 
	WHERE t1.gstr2id = t2.gstr2id 
	And t2.pos = t1.pos
    And t2.sply_ty = t1.sply_ty	  
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR2_TXI 
	SET #TBL_EXT_GSTR2_TXI.num = t3.num
	FROM #TBL_EXT_GSTR2_TXI t1,
			(Select slno,txiid,Row_Number() OVER(Partition By t2.txiid Order By t2.txiid) as num
			FROM #TBL_EXT_GSTR2_TXI t2
	   		) t3
	Where t1.txiid = t3.txiid
	And t1.slno = t3.slno


	-- Insert Records into Table TBL_GSTR2_TXI_ITMS

	Insert TBL_GSTR2_TXI_ITMS
	(txiid,num,rt,
		adamt,iamt,camt,samt,csamt,gstinid,gstr2id)
	Select	distinct t1.txiid,t1.num,t1.rt,
			t1.adamt,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_TXI t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_TXI_ITMS t2
						Where t2.txiid = t1.txiid 
							And t2.rt = t1.rt
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR2_TXI)
	Begin
		Update TBL_EXT_GSTR2_TXI
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End