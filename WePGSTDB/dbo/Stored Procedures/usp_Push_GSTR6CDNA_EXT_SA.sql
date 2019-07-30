


/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR6 CDNRA Records to the corresponding Staging
				Area tables
				
Written by  : muskan.garg@wepdigital.com 

Date		Who			Decription 
05/21/2018	Muskan		Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR6CDNA_EXT_SA  'CSV','WEP001', '33GSPTN0801G1ZM',1,1

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR6CDNA_EXT_SA]
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
	space(50) as cdnraid,
	space(50) as invid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.ctin,
	t1.ntty,t1.nt_num,t1.nt_dt,t1.ont_num,t1.ont_dt,
	t1.inum,t1.idt,
	t1.rt,t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR6_CDNA
	From
	(
		SELECT 
			gstin,fp,ctin,
			ntty,nt_num,nt_dt,ont_num,ont_dt,
			inum,idt,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt		
		FROM TBL_EXT_GSTR6_CDNA 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,ctin,ntty,nt_num,nt_dt,ont_num,ont_dt,inum,idt,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR6

	Insert TBL_GSTR6 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR6_CDNA t1
	Where Not Exists(Select 1 From TBL_GSTR6 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR6_CDNA
	SET #TBL_EXT_GSTR6_CDNA.gstr6id = t2.gstr6id 
	FROM #TBL_EXT_GSTR6_CDNA t1,
			TBL_GSTR6 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR6_CDNRA

	Insert TBL_GSTR6_CDNRA(gstr6id,ctin,gstinid) 
	Select	distinct gstr6Id,ctin,gstinid
	From #TBL_EXT_GSTR6_CDNA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6_CDNRA t2 where t2.gstr6id = t1.gstr6id and t2.ctin = t1.ctin) 

	Update #TBL_EXT_GSTR6_CDNA 
	SET #TBL_EXT_GSTR6_CDNA.cdnraid = t2.cdnraid 
	FROM #TBL_EXT_GSTR6_CDNA t1,
			TBL_GSTR6_CDNRA t2 
	WHERE t1.gstr6id = t2.gstr6id 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR6_CDNR_NT

	Insert TBL_GSTR6_CDNRA_NT
	(cdnraid,ntty,nt_num,nt_dt,ont_num,ont_dt,
	inum,idt,gstinid,gstr6id,custid,createdby,createddate)
	Select	distinct t1.cdnraid,t1.ntty,t1.nt_num,t1.nt_dt,t1.ont_num,t1.ont_dt,
					 t1.inum,t1.idt,t1.gstinid,t1.gstr6id,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR6_CDNA t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR6_CDNRA_NT t2
						Where t2.cdnraid = t1.cdnraid 
							And t2.nt_num = t1.nt_num
							And t2.nt_dt = t1.nt_dt
							And t2.ont_dt = t1.ont_dt
							And t2.ont_dt = t1.ont_dt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR6_CDNA
	SET #TBL_EXT_GSTR6_CDNA.invid = t2.invid 
	FROM #TBL_EXT_GSTR6_CDNA t1,
			TBL_GSTR6_CDNRA_NT t2 
	WHERE t1.cdnraid= t2.cdnraid 
	And t1.nt_num = t2.nt_num
	And t1.nt_dt = t2.nt_dt
	And t1.ont_num = t2.ont_num
	And t1.ont_dt = t2.ont_dt
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR6_CDNA 
	SET #TBL_EXT_GSTR6_CDNA.num = t3.num
	FROM #TBL_EXT_GSTR6_CDNA t1,
			(Select slno,invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_EXT_GSTR6_CDNA t2
	   		) t3
	Where t1.invid = t3.invid
	And t1.slno = t3.slno
	
	Insert TBL_GSTR6_CDNRA_NT_ITMS
	(invid,num,gstinid,gstr6id)
	Select	distinct t1.invid,t1.num,t1.gstinid,t1.gstr6id
	From #TBL_EXT_GSTR6_CDNA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6_CDNRA_NT_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
		
	
	Update #TBL_EXT_GSTR6_CDNA 
	SET #TBL_EXT_GSTR6_CDNA.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR6_CDNA t1,
			TBL_GSTR6_CDNRA_NT_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num
	
	Insert TBL_GSTR6_CDNRA_NT_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6id
	From #TBL_EXT_GSTR6_CDNA t1


	If Exists(Select 1 From #TBL_EXT_GSTR6_CDNA)
	Begin
		Update TBL_EXT_GSTR6_CDNA
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End  
		
	Return 0

End