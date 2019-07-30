
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2A CDNR JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/28/2017	Seshadri			Initial Version
08/10/2017	Karthik			Included the Where Clause while inserting Invoice Items 
09/28/2017	Seshadri		Fixed unit testing defects
10/13/2017	Seshadri	Fixed the issue of duplicate item insertion
11/03/2017	Seshadri	Fixed the issue of duplicate item detail insertion
11/07/2017	Seshadri	Modified the code to insert the incoming data to the table
						TBL_GSTR_Download
11/07/2017	Seshadri	Resolved the mismatch error issue during GSTR2 Save
07/06/2018  Karthik     Resolved Duplication of items issue.


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2ACDNR_SA  
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2ACDNR_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Insert Into TBL_GSTR_Download
	Values('GSTR2A-CDNR',@Gstin,@RecordContents,null,null,GetDate())


	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			space(50) as gstr2aid,
			space(50) as cdnrid,
			space(50) as ntid,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			ctin as ctin,
			cfs as cfs,
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
	Into #TBL_GSTR2A_CDNR
	From OPENJSON(@RecordContents) 
	WITH
	(
		ctin varchar(15),
		cfs varchar(1),
		nt nvarchar(max) as JSON
	) As Cdnr
	Cross Apply OPENJSON(Cdnr.nt) 
	WITH
	(
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
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin and rowstatus =1
	)t1


	-- Insert Records into Table TBL_GSTR2A

	Insert TBL_GSTR2A (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR2A_CDNR t1
	Where Not Exists(Select 1 From TBL_GSTR2A t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR2A_CDNR 
	SET #TBL_GSTR2A_CDNR.gstr2aid = t2.gstr2aid 
	FROM #TBL_GSTR2A_CDNR t1,
			TBL_GSTR2A t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2A_CDNR

	Insert TBL_GSTR2A_CDNR(gstr2aid,ctin,cfs,gstinid) 
	Select	distinct gstr2aid,ctin,cfs,gstinid
	From #TBL_GSTR2A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_CDNR t2 where t2.gstr2aid = t1.gstr2aid and t2.ctin = t1.ctin) 

	Update #TBL_GSTR2A_CDNR 
	SET #TBL_GSTR2A_CDNR.cdnrid = t2.cdnrid 
	FROM #TBL_GSTR2A_CDNR t1,
			TBL_GSTR2A_CDNR t2 
	WHERE t1.gstr2aid = t2.gstr2aid 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR2A_CDNR_NT

	Insert TBL_GSTR2A_CDNR_NT
	(cdnrid,chksum,ntty,nt_num,nt_dt,rsn,p_gst,inum,idt,val,gstinid,gstr2aid)
	Select	distinct t1.cdnrid,t1.chksum,t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,t1.inum,t1.idt,t1.val,t1.gstinid,t1.gstr2aid
	From #TBL_GSTR2A_CDNR t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2A_CDNR_NT t2
						Where t2.cdnrid = t1.cdnrid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt)		 


	Update #TBL_GSTR2A_CDNR  
	SET #TBL_GSTR2A_CDNR.ntid = t2.ntid 
	FROM #TBL_GSTR2A_CDNR t1,
		 TBL_GSTR2A_CDNR_NT t2 
	WHERE t1.cdnrid= t2.cdnrid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt

	if exists (select 1 from  TBL_GSTR2A_CDNR_NT_ITMS where ntid in (select distinct ntid from #TBL_GSTR2A_CDNR where isnull(ntid,0) <> 0))
	Begin
		Select itmsid into #itmsid from TBL_GSTR2A_CDNR_NT_ITMS where ntid in (select distinct ntid from #TBL_GSTR2A_CDNR where isnull(ntid,0) <> 0)
		Delete from TBL_GSTR2A_CDNR_NT_ITMS_DET where itmsid in (select itmsid from  #itmsid)
		Delete from TBL_GSTR2A_CDNR_NT_ITMS where itmsid in (select itmsid from  #itmsid)
	End
	
	/*
	Update #TBL_GSTR2A_CDNR   
	SET #TBL_GSTR2A_CDNR.num = t3.num
	FROM #TBL_GSTR2A_CDNR t1,
			(Select slno,ntid,Row_Number()  OVER(Partition By t2.ntid Order By t2.ntid) as num
			FROM #TBL_GSTR2A_CDNR t2
	   		) t3
	Where t1.ntid = t3.ntid
	And t1.slno = t3.slno */

	
	Insert TBL_GSTR2A_CDNR_NT_ITMS
	(ntid,num,gstinid,gstr2aid)
	Select	distinct t1.ntid,t1.num,t1.gstinid,t1.gstr2aid
	From #TBL_GSTR2A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_CDNR_NT_ITMS t2
						Where t2.ntid = t1.ntid 
						And t2.num = t1.num)
		
	
	Update #TBL_GSTR2A_CDNR 
	SET #TBL_GSTR2A_CDNR.itmsid = t2.itmsid
	FROM #TBL_GSTR2A_CDNR t1,
		TBL_GSTR2A_CDNR_NT_ITMS t2 
	WHERE t1.ntid= t2.ntid 
	And t1.num = t2.num
	
	Insert TBL_GSTR2A_CDNR_NT_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2aid)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2aid
	From #TBL_GSTR2A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_CDNR_NT_ITMS_DET t2
						Where t2.itmsid = t1.itmsid)


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End

