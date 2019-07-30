
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 CDNUR Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri			Initial Version
08/11/2017	Seshadri	Changed the logic for updating num value in Invoice Item table
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/04/2017	Seshadri	Fixed Flag Issue
12/13/2017	Seshadri	Modified the code to filter records with cdnurTyp = 'B2CS'
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 
03/01/2018	Seshadri	Fixed Tax value updation issue
07/03/2018	Raja		Pre GST column included for SA from EXT to SA Data Push

*/

/* Sample Procedure Call

exec usp_Push_GSTR1CDNUR_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1CDNUR_EXT_SA]
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
	space(50) as cdnurid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.typ,t1.ntty,t1.nt_num,t1.nt_dt,t1.inum,t1.idt,t1.val,t1.p_gst,
	t1.rt, t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt
	Into #TBL_EXT_GSTR1_CDNUR 
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			typ,ntty,nt_num,nt_dt,inum,idt,val,p_gst,
			rt,sum(txvaL) as txval,
			sum(iamt) as iamt, sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt
		FROM TBL_EXT_GSTR1_CDNUR 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
			And Ltrim(Rtrim(IsNull(typ,''))) <> 'B2CS'
		Group By gstin,fp,gt,cur_gt,typ,ntty,nt_num,nt_dt,inum,idt,val,rt,p_gst
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_CDNUR t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_CDNUR 
	SET #TBL_EXT_GSTR1_CDNUR.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_CDNUR t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_CDNUR

	Insert TBL_GSTR1_CDNUR
	(gstr1id,typ,ntty,nt_num,nt_dt,inum,idt,val,gstinId,p_gst,
		custid,createdby,createddate)
	Select	distinct t1.gstr1id,t1.typ,t1.ntty,t1.nt_num,t1.nt_dt,t1.inum,t1.idt,t1.val,t1.gstinId,t1.p_gst,
					@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_CDNUR t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_CDNUR t2
						Where t2.gstr1id = t1.gstr1id 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR1_CDNUR 
	SET #TBL_EXT_GSTR1_CDNUR.cdnurid = t2.cdnurid
	FROM #TBL_EXT_GSTR1_CDNUR t1,
			TBL_GSTR1_CDNUR t2 
	WHERE t1.gstr1id= t2.gstr1id 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And Isnull(t2.flag,'') = ''
	
	Update #TBL_EXT_GSTR1_CDNUR 
	SET #TBL_EXT_GSTR1_CDNUR.num = t3.num
	FROM #TBL_EXT_GSTR1_CDNUR t1,
			(Select slno,cdnurid,Row_Number()  OVER(Partition By t2.cdnurid Order By t2.cdnurid) as num
			FROM #TBL_EXT_GSTR1_CDNUR t2
	   		) t3
	Where t1.cdnurid = t3.cdnurid
	And t1.slno = t3.slno
	
	Insert TBL_GSTR1_CDNUR_ITMS
	(cdnurid,num,gstinId,gstr1id)
	Select	distinct t1.cdnurid,t1.num,t1.gstinId,t1.gstr1id
	From #TBL_EXT_GSTR1_CDNUR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_CDNUR_ITMS t2
						Where t2.cdnurid = t1.cdnurid 
						And t2.num = t1.num)
	
	Update #TBL_EXT_GSTR1_CDNUR 
	SET #TBL_EXT_GSTR1_CDNUR.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR1_CDNUR t1,
			TBL_GSTR1_CDNUR_ITMS t2 
	WHERE t1.cdnurid= t2.cdnurid 
	And t1.num = t2.num

	if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And TaxValCalnReqd = 1)
	Begin
		Update #TBL_EXT_GSTR1_CDNUR
		Set iamt=(convert(decimal(18,2),txval*rt/100))
		Where isnull(iamt,0) > 0

		Update #TBL_EXT_GSTR1_CDNUR 
		Set camt = (convert(decimal(18,2),(txval*rt/100)/2)), 
			samt = (convert(decimal(18,2),(txval*rt/100)/2)) 
		Where isnull(camt,0) > 0 and isnull(samt,0) > 0
	End

	Insert TBL_GSTR1_CDNUR_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinId,gstr1id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinId,gstr1id
	From #TBL_EXT_GSTR1_CDNUR t1
	
	If Exists(Select 1 From #TBL_EXT_GSTR1_CDNUR)
	Begin
		Update TBL_EXT_GSTR1_CDNUR 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End