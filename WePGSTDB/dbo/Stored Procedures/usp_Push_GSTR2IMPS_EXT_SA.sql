
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 IMPS Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code	
12/27/2017	Karthik 	Fixed deleted Gstin Issue 
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.

*/

/* Sample Procedure Call

exec usp_Push_GSTR2IMPS_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2IMPS_EXT_SA]
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
	space(50) as impsid,
	space(50) as num,
	t1.gstin,t2.gstinid,t1.fp,
	t1.inum,t1.idt,t1.pos,t1.ival,
	t1.rt,t1.elg,t1.txval, 
	t1.iamt,t1.csamt,
	t1.tx_i,t1.tx_cs
	Into #TBL_EXT_GSTR2_IMPS_INV
	From
	(
		SELECT 
			gstin,fp,
			inum,idt,pos,ival,
			rt,elg,sum(txval) as txval, 
			sum(iamt) as iamt, sum(csamt) as csamt,
			sum(tx_i) as tx_i, sum(tx_cs) as tx_cs
		FROM TBL_EXT_GSTR2_IMPS_INV
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,inum,idt,pos,ival,rt,elg
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_IMPS_INV t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_IMPS_INV
	SET #TBL_EXT_GSTR2_IMPS_INV.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_IMPS_INV t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_IMPS

	Insert TBL_GSTR2_IMPS(gstr2id,inum,idt,ival,pos,gstinid,custid,createdby,createddate)  
	Select	distinct gstr2id,inum,idt,ival,pos,gstinid,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR2_IMPS_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_IMPS t2 
						Where t2.gstr2id = t1.gstr2id
						And t2.inum = t1.inum
						And t2.idt = t1.idt
						And Isnull(t2.flag,'') = '') 

	Update #TBL_EXT_GSTR2_IMPS_INV
	SET #TBL_EXT_GSTR2_IMPS_INV.impsid = t2.impsid 
	FROM #TBL_EXT_GSTR2_IMPS_INV t1,
			TBL_GSTR2_IMPS t2 
	WHERE t1.gstr2id = t2.gstr2id 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR2_IMPS_INV 
	SET #TBL_EXT_GSTR2_IMPS_INV.num = t3.num
	FROM #TBL_EXT_GSTR2_IMPS_INV t1,
			(Select slno,impsid,Row_Number() OVER(Partition By t2.impsid Order By t2.impsid) as num
			FROM #TBL_EXT_GSTR2_IMPS_INV t2
	   		) t3
	Where t1.impsid = t3.impsid
	And t1.slno = t3.slno


	-- Insert Records into Table TBL_GSTR2_IMPS_ITMS

	Insert TBL_GSTR2_IMPS_ITMS
	(impsid,num,rt,txval,
		iamt,csamt,
		elg,tx_i,tx_cs,gstinid,gstr2id)
	Select	distinct t1.impsid,t1.num,t1.rt,t1.txval,
			t1.iamt,t1.csamt,
			t1.elg,t1.tx_i,t1.tx_cs,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_IMPS_INV t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_IMPS_ITMS t2
						Where t2.impsid = t1.impsid 
							And t2.rt = t1.rt
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR2_IMPS_INV)
	Begin
		Update TBL_EXT_GSTR2_IMPS_INV 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End