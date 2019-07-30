
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 HSN Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/30/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code
12/28/2017	Karthik 	Fixed deleted Gstin Issue 
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.
*/

/* Sample Procedure Call

exec usp_Push_GSTR2HSN_EXT_SA 'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2HSN_EXT_SA]
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
	space(50) as hsnid,
	space(50) as num,
	t1.gstin,t2.gstinid,t1.fp,
	t1.hsn_sc,t1.descs,t1.uqc,t1.qty,t1.val,
	t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR2_HSN
	From
	(
		SELECT 
			gstin,fp,
			hsn_sc,descs,uqc,
			sum(qty) as qty,
			sum(val) as val,
			sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR2_HSN 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,hsn_sc,descs,uqc
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_HSN t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_HSN 
	SET #TBL_EXT_GSTR2_HSN.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_HSN t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_HSNSUM

	Insert TBL_GSTR2_HSNSUM(gstr2id,gstinid,custid,createdby,createddate) 
	Select	distinct gstr2Id,gstinid,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR2_HSN t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_HSNSUM t2 where t2.gstr2id = t1.gstr2id ) 

	Update #TBL_EXT_GSTR2_HSN
	SET #TBL_EXT_GSTR2_HSN.hsnid = t2.hsnsumid 
	FROM #TBL_EXT_GSTR2_HSN t1,
			TBL_GSTR2_HSNSUM t2 
	WHERE t1.gstr2id = t2.gstr2id 

	Update #TBL_EXT_GSTR2_HSN 
	SET #TBL_EXT_GSTR2_HSN.num = t3.num
	FROM #TBL_EXT_GSTR2_HSN t1,
			(Select slno,hsnid,Row_Number() OVER(Partition By t2.hsnid Order By t2.hsnid) as num
			FROM #TBL_EXT_GSTR2_HSN t2
	   		) t3
	Where t1.hsnid = t3.hsnid
	And t1.slno = t3.slno

	-- Insert Records into Table TBL_GSTR2_HSNSUM_DET

	Insert TBL_GSTR2_HSNSUM_DET
	(hsnsumid,num,hsn_sc,[desc],uqc,qty,val,
		txval,iamt,camt,samt,csamt,gstinid,gstr2id)
	Select	distinct t1.hsnid,t1.num,t1.hsn_sc,t1.descs,uqc,qty,val,
			txval,iamt,camt,samt,csamt,gstinid,gstr2id
	From #TBL_EXT_GSTR2_HSN t1
	Where	Not Exists (	SELECT 1 FROM 
							TBL_GSTR2_HSNSUM_DET t2,
							TBL_GSTR2_HSNSUM t3
							Where t2.hsnsumid = t1.hsnid 
							And t2.hsn_sc = t1.hsn_sc
							And t2.qty = t1.qty
							And t2.val = t1.val
							And t3.hsnsumid = t2.hsnsumid  
							And Isnull(t3.flag,'') = '')		 

	If Exists(Select 1 From #TBL_EXT_GSTR2_HSN)
	Begin
		Update TBL_EXT_GSTR2_HSN 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End