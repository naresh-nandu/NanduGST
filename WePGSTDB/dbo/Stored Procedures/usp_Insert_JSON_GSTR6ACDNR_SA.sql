
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR6A CDNR JSON Records to the corresponding statging area tables
				
Written by  : muskan.garg@wepdigital.com

Date		Who			Decription 
05/15/2018	Muskan			Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR6ACDNR_SA  
 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_GSTR6ACDNR_SA]
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
	Values('GSTR6A-CDNR',@Gstin,@RecordContents,null,null,GetDate())


	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			space(50) as gstr6aid,
			space(50) as cdnrid,
			space(50) as ntid,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			ctin as ctin,			
			chksum as chksum,
			ntty as ntty,
			nt_num as nt_num,
			nt_dt as nt_dt,
		    pos as pos,			
			inum as inum,
			idt as idt,		
			num as num,
			rt as rt,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR6A_CDNR
	From OPENJSON(@RecordContents) 
	WITH
	(
		ctin varchar(15),	
		nt nvarchar(max) as JSON
	) As Cdnr
	Cross Apply OPENJSON(Cdnr.nt) 
	WITH
	(
		chksum varchar(64),
		ntty varchar(1),
		nt_num varchar(50),
		nt_dt varchar(50),
		pos varchar(2),
    	inum varchar(50),
		idt varchar(50),
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


	-- Insert Records into Table TBL_GSTR6A

	Insert TBL_GSTR6A (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR6A_CDNR t1
	Where Not Exists(Select 1 From TBL_GSTR6A t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR6A_CDNR 
	SET #TBL_GSTR6A_CDNR.gstr6aid = t2.gstr6aid 
	FROM #TBL_GSTR6A_CDNR t1,
			TBL_GSTR6A t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR6A_CDNR

	Insert TBL_GSTR6A_CDNR(gstr6aid,ctin,gstinid) 
	Select	distinct gstr6aid,ctin,gstinid
	From #TBL_GSTR6A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6A_CDNR t2 where t2.gstr6aid = t1.gstr6aid and t2.ctin = t1.ctin) 

	Update #TBL_GSTR6A_CDNR 
	SET #TBL_GSTR6A_CDNR.cdnrid = t2.cdnrid 
	FROM #TBL_GSTR6A_CDNR t1,
			TBL_GSTR6A_CDNR t2 
	WHERE t1.gstr6aid = t2.gstr6aid 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR6A_CDNR_NT

	Insert TBL_GSTR6A_CDNR_NT
	(cdnrid,chksum,ntty,nt_num,nt_dt,pos,inum,idt,gstinid,gstr6aid)
	Select	distinct t1.cdnrid,t1.chksum,t1.ntty,t1.nt_num,t1.nt_dt,t1.pos,t1.inum,t1.idt,t1.gstinid,t1.gstr6aid
	From #TBL_GSTR6A_CDNR t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR6A_CDNR_NT t2
						Where t2.cdnrid = t1.cdnrid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt)		 


	Update #TBL_GSTR6A_CDNR  
	SET #TBL_GSTR6A_CDNR.ntid = t2.ntid 
	FROM #TBL_GSTR6A_CDNR t1,
		 TBL_GSTR6A_CDNR_NT t2 
	WHERE t1.cdnrid= t2.cdnrid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	
	/*
	Update #TBL_GSTR6A_CDNR   
	SET #TBL_GSTR6A_CDNR.num = t3.num
	FROM #TBL_GSTR6A_CDNR t1,
			(Select slno,ntid,Row_Number()  OVER(Partition By t2.ntid Order By t2.ntid) as num
			FROM #TBL_GSTR6A_CDNR t2
	   		) t3
	Where t1.ntid = t3.ntid
	And t1.slno = t3.slno */

	
	Insert TBL_GSTR6A_CDNR_NT_ITMS
	(ntid,num,gstinid,gstr6aid)
	Select	distinct t1.ntid,t1.num,t1.gstinid,t1.gstr6aid
	From #TBL_GSTR6A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6A_CDNR_NT_ITMS t2
						Where t2.ntid = t1.ntid 
						And t2.num = t1.num)
		
	
	Update #TBL_GSTR6A_CDNR 
	SET #TBL_GSTR6A_CDNR.itmsid = t2.itmsid
	FROM #TBL_GSTR6A_CDNR t1,
		TBL_GSTR6A_CDNR_NT_ITMS t2 
	WHERE t1.ntid= t2.ntid 
	And t1.num = t2.num
	
	Insert TBL_GSTR6A_CDNR_NT_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6aid)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr6aid
	From #TBL_GSTR6A_CDNR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR6A_CDNR_NT_ITMS_DET t2
						Where t2.itmsid = t1.itmsid)


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End