
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 CDNRA Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/06/2018	Seshadri			Initial Version

*/

/* Sample Procedure Call

exec usp_Push_GSTR1CDNRA_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1CDNRA_EXT_SA]
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
	space(50) as gstr1id,
	space(50) as cdnraid,
	space(50) as ntid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,t1.ctin,t1.cfs,
	t1.ntty,t1.nt_num,t1.nt_dt,t1.p_gst,
	t1.inum,t1.idt,t1.val,t1.pos,
	t1.ont_num,t1.ont_dt,t1.diff_percent,
	t1.rt, t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_CDNRA 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,ctin,cfs,
			ntty,nt_num,nt_dt,p_gst,
			inum,idt,val,pos,
			ont_num,ont_dt,diff_percent,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_CDNRA 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,ctin,cfs,ntty,nt_num,nt_dt,p_gst,inum,idt,val,pos,
				ont_num,ont_dt,diff_percent,rt
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_CDNRA t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_CDNRA 
	SET #TBL_EXT_GSTR1_CDNRA.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_CDNRA t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_CDNRA

	Insert TBL_GSTR1_CDNRA(gstr1id,ctin,cfs,gstinid) 
	Select	distinct gstr1Id,ctin,cfs,gstinid 
	From #TBL_EXT_GSTR1_CDNRA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_CDNRA t2 where t2.gstr1id = t1.gstr1id and t2.ctin = t1.ctin) 

	Update #TBL_EXT_GSTR1_CDNRA 
	SET #TBL_EXT_GSTR1_CDNRA.cdnraid = t2.cdnraid 
	FROM #TBL_EXT_GSTR1_CDNRA t1,
			TBL_GSTR1_CDNRA t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR1_CDNRA_NT

	Insert TBL_GSTR1_CDNRA_NT
	(	cdnraid,ntty,nt_num,nt_dt,p_gst,
		inum,idt,val,gstinid,gstr1id,
		custid,createdby,createddate,
		ont_num,ont_dt,diff_percent)
	Select	distinct t1.cdnraid,t1.ntty,t1.nt_num,t1.nt_dt,t1.p_gst,
					 t1.inum,t1.idt,t1.val,t1.gstinid,t1.gstr1id,
					 @Custid,@CreatedBy,GetDate(),
					 t1.ont_num,t1.ont_dt,t1.diff_percent
	From #TBL_EXT_GSTR1_CDNRA t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_CDNRA_NT t2
						Where t2.cdnraid = t1.cdnraid 
							And t2.nt_num = t1.nt_num
							And t2.nt_dt = t1.nt_dt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR1_CDNRA 
	SET #TBL_EXT_GSTR1_CDNRA.ntid = t2.ntid 
	FROM #TBL_EXT_GSTR1_CDNRA t1,
			TBL_GSTR1_CDNRA_NT t2 
	WHERE t1.cdnraid= t2.cdnraid 
	And t1.nt_num = t2.nt_num
	And t1.nt_dt = t2.nt_dt
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR1_CDNRA 
	SET #TBL_EXT_GSTR1_CDNRA.num = t3.num
	FROM #TBL_EXT_GSTR1_CDNRA t1,
			(Select slno,ntid,Row_Number()  OVER(Partition By t2.ntid Order By t2.ntid) as num
			FROM #TBL_EXT_GSTR1_CDNRA t2
	   		) t3
	Where t1.ntid = t3.ntid
	And t1.slno = t3.slno
	
	Insert TBL_GSTR1_CDNRA_NT_ITMS
	(ntid,num,gstinid,gstr1id)
	Select	distinct t1.ntid,t1.num,t1.gstinid,t1.gstr1id
	From #TBL_EXT_GSTR1_CDNRA t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_CDNRA_NT_ITMS t2
						Where t2.ntid = t1.ntid 
						And t2.num = t1.num)
		
	
	Update #TBL_EXT_GSTR1_CDNRA 
	SET #TBL_EXT_GSTR1_CDNRA.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR1_CDNRA t1,
			TBL_GSTR1_CDNRA_NT_ITMS t2 
	WHERE t1.ntid= t2.ntid 
	And t1.num = t2.num

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin	
		Update #TBL_EXT_GSTR1_CDNRA 
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_CDNRA 
		Set camt = (convert(decimal(18,2),(txval*rt/100)/2)), 
			samt = (convert(decimal(18,2),(txval*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	
	Insert TBL_GSTR1_CDNRA_NT_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1id
	From #TBL_EXT_GSTR1_CDNRA t1
	
	If Exists(Select 1 From #TBL_EXT_GSTR1_CDNRA)
	Begin
		Update TBL_EXT_GSTR1_CDNRA 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End  
		
	Return 0

End