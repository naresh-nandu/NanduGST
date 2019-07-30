
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D CDNR JSON Records to the corresponding staging area
				tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri			Initial Version
08/10/2017	Seshadri	Included the Where Clause while inserting Invoice Items 
08/23/2017	Seshadri	Corrected Insertion Errors
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_CDNR_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_CDNR_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	/* Code Added to Debug
	Insert Into TBL_GSTR_Download
	Values('Gstr1 - CDNR',@Gstin,@RecordContents,1,1,GetDate()) */


	Select	space(50) as gstr1did,
			space(50) as cdnrid,
			space(50) as ntid,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			ctin as ctin,
			cfs as cfs,
			flag as flag,
			cflag as cflag,
			opd as opd,
			updby as updby,
			chksum as chksum,
			ntty as ntty,
			nt_num as nt_num,
			nt_dt as nt_dt,
			rsn as rsn,
			p_gst as p_gst,
			inum as inum,
			idt as idt,
			val as val,
			num as num,
			rt as rt,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_CDNR
	From OPENJSON(@RecordContents) 
	/*
	WITH
	(
	  cdnr nvarchar(max) as JSON
	) As Cdnrs 
	Cross Apply OPENJSON(Cdnrs.cdnr) 
	*/
	WITH
	(
		ctin varchar(15),
		cfs varchar(1),
		nt nvarchar(max) as JSON
	) As Cdnr
	Cross Apply OPENJSON(Cdnr.nt) 
	WITH
	(
		flag varchar(1),
		cflag varchar(1),
		opd varchar(10),
		updby varchar(1), 
		chksum varchar(64),
		ntty varchar(1),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(1),
    	inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
    	itms nvarchar(max) as JSON
	) As Nt
	Cross Apply OPENJSON(Nt.itms)
	WITH
	(
		num int,
        itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
        txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
        csamt decimal(18,2)
	) As Itm_Det
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_CDNR t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_CDNR 
	SET #TBL_GSTR1_D_CDNR.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_CDNR t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_CDNR

	Insert TBL_GSTR1_D_CDNR(gstr1did,ctin,cfs,gstinid) 
	Select	distinct gstr1did,ctin,cfs,gstinid 
	From #TBL_GSTR1_D_CDNR  t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_CDNR t2 where t2.gstr1did = t1.gstr1did and t2.ctin = t1.ctin) 

	Update #TBL_GSTR1_D_CDNR 
	SET #TBL_GSTR1_D_CDNR.cdnrid = t2.cdnrid 
	FROM #TBL_GSTR1_D_CDNR t1,
		TBL_GSTR1_D_CDNR t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR1_D_CDNR_NT

	Insert TBL_GSTR1_D_CDNR_NT
	(cdnrid,flag,cflag,opd,updby,chksum,
		ntty,nt_num,nt_dt,
		rsn,p_gst,inum,idt,val,gstinid,gstr1did)
	Select	distinct t1.cdnrid,t1.flag,t1.cflag,t1.opd,t1.updby,t1.chksum,
					 t1.ntty,t1.nt_num,t1.nt_dt,
					 t1.rsn,t1.p_gst,t1.inum,t1.idt,t1.val,t1.gstinid,t1.gstr1did
	From #TBL_GSTR1_D_CDNR  t1
	Where	Not Exists (	SELECT 1 FROM TBL_GSTR1_D_CDNR_NT t2
							Where t2.cdnrid = t1.cdnrid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt
							And(t2.flag is Null Or t2.flag <> 'D')
						)		 


	Update #TBL_GSTR1_D_CDNR 
	SET #TBL_GSTR1_D_CDNR.ntid = t2.ntid 
	FROM #TBL_GSTR1_D_CDNR t1,
			TBL_GSTR1_D_CDNR_NT t2 
	WHERE t1.cdnrid= t2.cdnrid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And(t2.flag is Null Or t2.flag <> 'D')
	
	Insert TBL_GSTR1_D_CDNR_NT_ITMS
	(ntid,num,gstinid,gstr1did)
	Select	distinct t1.ntid,t1.num,t1.gstinid,t1.gstr1did
	From #TBL_GSTR1_D_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_CDNR_NT_ITMS t2
						Where t2.ntid = t1.ntid 
						And t2.num = t1.num)
		
	
	Update #TBL_GSTR1_D_CDNR
	SET #TBL_GSTR1_D_CDNR.itmsid = t2.itmsid
	FROM #TBL_GSTR1_D_CDNR t1,
		TBL_GSTR1_D_CDNR_NT_ITMS t2 
	WHERE t1.ntid= t2.ntid 
	And t1.num = t2.num
	
	Insert TBL_GSTR1_D_CDNR_NT_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1did)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr1did
	From #TBL_GSTR1_D_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_CDNR_NT_ITMS_DET t2
						Where t2.itmsid = t1.itmsid 
						And t2.rt = t1.rt)
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End