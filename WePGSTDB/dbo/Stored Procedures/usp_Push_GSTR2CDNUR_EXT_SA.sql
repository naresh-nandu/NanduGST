
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR2 CDNUR Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/30/2017	Seshadri			Initial Version
09/12/2017	Seshadri	Fine tuned the code
12/28/2017	Karthik 	Fixed deleted Gstin Issue 
05/10/2018  Muskan      Added Custid,Createdby,Createddate in SA Table.

*/

/* Sample Procedure Call

exec usp_Push_GSTR2CDNUR_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [dbo].[usp_Push_GSTR2CDNUR_EXT_SA]
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
	space(50) as cdnurid,
	space(50) as num,
	space(50) as itmsid,
	t1.gstin,t2.gstinid,t1.fp,t1.rtin,
	t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,
	t1.inum,t1.idt,t1.inv_typ,t1.val,
	t1.rt,t1.elg,t1.txval, 
	t1.iamt,t1.camt,t1.samt,t1.csamt,
	t1.tx_i,t1.tx_c,t1.tx_s,t1.tx_cs
	Into #TBL_EXT_GSTR2_CDNUR 
	From
	(
		SELECT 
			gstin,fp,rtin,
			ntty,nt_num,nt_dt,rsn,p_gst,
			inum,idt,inv_typ,val,
			rt,elg,sum(txvaL) as txval,
			sum(iamt) as iamt,sum(camt) as camt, sum(samt) as samt, sum(csamt) as csamt,
			sum(tx_i) as tx_i,sum(tx_c) as tx_c, sum(tx_s) as tx_s, sum(tx_cs) as tx_cs
		FROM TBL_EXT_GSTR2_CDNUR 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,rtin,ntty,nt_num,nt_dt,rsn,p_gst,inum,idt,inv_typ,val,rt,elg
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2

	-- Insert Records into Table TBL_GSTR2

	Insert TBL_GSTR2 (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_EXT_GSTR2_CDNUR t1
	Where Not Exists(Select 1 From TBL_GSTR2 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR2_CDNUR 
	SET #TBL_EXT_GSTR2_CDNUR.gstr2id = t2.gstr2id 
	FROM #TBL_EXT_GSTR2_CDNUR t1,
			TBL_GSTR2 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2_CDNUR

	Insert TBL_GSTR2_CDNUR
	(gstr2id,rtin,ntty,nt_num,nt_dt,rsn,p_gst,
	inum,idt,inv_typ,val,gstinid,custid,createdby,createddate)
	Select	distinct t1.gstr2id,t1.rtin,t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,
					 t1.inum,t1.idt,t1.inv_typ,t1.val,t1.gstinid,@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR2_CDNUR t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNUR t2
						Where t2.gstr2id = t1.gstr2id 
							And t2.nt_num = t1.nt_num
							And t2.nt_dt = t1.nt_dt
							And Isnull(t2.flag,'') = '')		 


	Update #TBL_EXT_GSTR2_CDNUR 
	SET #TBL_EXT_GSTR2_CDNUR.cdnurid = t2.cdnurid
	FROM #TBL_EXT_GSTR2_CDNUR t1,
			TBL_GSTR2_CDNUR t2 
	WHERE t1.gstr2id= t2.gstr2id 
	And t1.nt_num = t2.nt_num
	And t1.nt_dt = t2.nt_dt
	And Isnull(t2.flag,'') = ''
	
	Update #TBL_EXT_GSTR2_CDNUR 
	SET #TBL_EXT_GSTR2_CDNUR.num = t3.num
	FROM #TBL_EXT_GSTR2_CDNUR t1,
			(Select slno,cdnurid,Row_Number()  OVER(Partition By t2.cdnurid Order By t2.cdnurid) as num
			FROM #TBL_EXT_GSTR2_CDNUR t2
	   		) t3
	Where t1.cdnurid = t3.cdnurid
	And t1.slno = t3.slno

	
	Insert TBL_GSTR2_CDNUR_ITMS
	(cdnurid,num,gstinid,gstr2id)
	Select	distinct t1.cdnurid,t1.num,t1.gstinid,t1.gstr2id
	From #TBL_EXT_GSTR2_CDNUR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2_CDNUR_ITMS t2
						Where t2.cdnurid = t1.cdnurid 
						And t2.num = t1.num)
	
	Update #TBL_EXT_GSTR2_CDNUR 
	SET #TBL_EXT_GSTR2_CDNUR.itmsid = t2.itmsid
	FROM #TBL_EXT_GSTR2_CDNUR t1,
			TBL_GSTR2_CDNUR_ITMS t2 
	WHERE t1.cdnurid= t2.cdnurid 
	And t1.num = t2.num
	
	Insert TBL_GSTR2_CDNUR_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2id
	From #TBL_EXT_GSTR2_CDNUR t1

	Insert TBL_GSTR2_CDNUR_ITMS_ITC
	(itmsid,elg,tx_i,tx_c,tx_s,tx_cs)
	Select itmsid,elg,tx_i,tx_c,tx_s,tx_cs
	From #TBL_EXT_GSTR2_CDNUR t1
	
	If Exists(Select 1 From #TBL_EXT_GSTR2_CDNUR)
	Begin
		Update TBL_EXT_GSTR2_CDNUR 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End