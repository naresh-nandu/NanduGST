


/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR6 B2BA Records to the corresponding Staging
				Area tables
				
Written by  : muskan.garg@wepdigital.com 

Date		Who			Decription 
05/21/2018	Muskan  	Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR6B2BA_EXT_SA  'CSV','33GSPTN0801G1ZM','WEP001',1,1 
Select * from TBL_EXT_GSTR6_B2BA_INV
 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR6B2BA_EXT_SA]
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
	space(50) as gstr6id,
	space(50) as b2baid,
	space(50) as invid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.ctin,
	t1.inum,t1.idt,t1.oinum,t1.oidt,t1.val,t1.pos,
	t1.rt,t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt	
	Into #TBL_EXT_GSTR6_B2BA_INV 
	From
	(
		SELECT 
			gstin,fp,ctin,
			inum,idt,oinum,oidt,val,pos,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR6_B2BA_INV 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,ctin,inum,idt,oinum,oidt,val,pos,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR6
	
	Insert TBL_GSTR6 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR6_B2BA_INV t1
	Where Not Exists(Select 1 From TBL_GSTR6 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR6_B2BA_INV 
	SET #TBL_EXT_GSTR6_B2BA_INV.gstr6id = t2.GSTR6id 
	FROM #TBL_EXT_GSTR6_B2BA_INV t1,
			TBL_GSTR6 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR6_B2BA
	Insert TBL_GSTR6_B2BA(gstr6id,ctin,gstinid) 
	Select	distinct gstr6Id,ctin,gstinid
	From #TBL_EXT_GSTR6_B2BA_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6_B2BA t2 where t2.GSTR6id = t1.GSTR6id and t2.ctin = t1.ctin) 

	Update #TBL_EXT_GSTR6_B2BA_INV 
	SET #TBL_EXT_GSTR6_B2BA_INV.b2baid = t2.b2baid 
	FROM #TBL_EXT_GSTR6_B2BA_INV t1,
			TBL_GSTR6_B2BA t2 
	WHERE t1.gstr6id = t2.gstr6id 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR6_B2BA_INV

	Insert TBL_GSTR6_B2BA_INV
	(b2baid,inum,idt,oinum,oidt,val,pos,gstinid,gstr6id,custid,createdby,createddate)
	Select	distinct t1.b2baid,t1.inum,t1.idt,t1.oinum,t1.oidt,t1.val,
			t1.pos,t1.gstinid,t1.gstr6id,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR6_B2BA_INV t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR6_B2BA_INV t2
						Where t2.b2baid = t1.b2baid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And t2.oinum = t1.oinum
							And t2.oidt = t1.oidt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR6_B2BA_INV 
	SET #TBL_EXT_GSTR6_B2BA_INV.invid = t2.invid 
	FROM #TBL_EXT_GSTR6_B2BA_INV t1,
			TBL_GSTR6_B2BA_INV t2 
	WHERE t1.b2baid= t2.b2baid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And t2.oinum = t1.oinum
	And t2.oidt = t1.oidt
	And Isnull(t2.flag,'') = ''
	
	Update #TBL_EXT_GSTR6_B2BA_INV 
	SET #TBL_EXT_GSTR6_B2BA_INV.num = t3.num
	FROM #TBL_EXT_GSTR6_B2BA_INV t1,
			(Select slno,invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_EXT_GSTR6_B2BA_INV t2
	   		) t3
	Where t1.invid = t3.invid
	And t1.slno = t3.slno

	Insert TBL_GSTR6_B2BA_INV_ITMS
	(invid,num,gstinid,gstr6id)
	Select	distinct t1.invid,t1.num,t1.gstinid,t1.gstr6id
	From #TBL_EXT_GSTR6_B2BA_INV t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6_B2BA_INV_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
		
	
	Update #TBL_EXT_GSTR6_B2BA_INV 
	SET #TBL_EXT_GSTR6_B2BA_INV.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR6_B2BA_INV t1,
			TBL_GSTR6_B2BA_INV_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num
	
	Insert TBL_GSTR6_B2BA_INV_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6id
	From #TBL_EXT_GSTR6_B2BA_INV t1

	If Exists(Select 1 From #TBL_EXT_GSTR6_B2BA_INV )
	Begin
		Update TBL_EXT_GSTR6_B2BA_INV 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End 
		
	Return 0

End