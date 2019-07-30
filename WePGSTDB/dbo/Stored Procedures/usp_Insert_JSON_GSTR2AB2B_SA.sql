﻿

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2A B2B JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/28/2017	Seshadri			Initial Version
08/10/2017	Karthik			Included the Where Clause while inserting Invoice Items 
10/13/2017	Seshadri	Fixed the issue of duplicate item insertion
11/03/2017	Seshadri	Fixed the issue of duplicate item detail insertion
11/07/2017	Seshadri	Modified the code to insert the incoming data to the table
						TBL_GSTR_Download
11/07/2017	Seshadri	Resolved the mismatch error issue during GSTR2 Save
07/06/2018  Karthik     Resolved Duplication of items issue.
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2AB2B_SA  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2AB2B_SA]
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
	Values('GSTR2A-B2B',@Gstin,@RecordContents,null,null,GetDate())

	Select	Row_Number() OVER(Order by @Gstin ASC) as slno,
			space(50) as gstr2aid,
			space(50) as b2bid,
			space(50) as invid,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			ctin as ctin ,
			cfs as cfs,
			chksum as chksum,
			inum as inum,
			idt as idt,
			val as val ,
			pos as pos,
			rchrg as rchrg,
			inv_typ as inv_typ,
			num as num,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR2A_B2B
	From OPENJSON(@RecordContents) 	
	WITH
	(
		ctin varchar(15),
		cfs varchar(1),
		inv nvarchar(max) as JSON
	) As B2b
	Cross Apply OPENJSON(B2b.inv) 
	WITH
	(
		chksum varchar(64),
		inum varchar(50),
		idt varchar(50),
		val decimal(18,2),
		pos varchar(2),
		rchrg varchar(1),
		inv_typ varchar(5),
		itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
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

	if Not Exists(Select 1 From #TBL_GSTR2A_B2B)
	Begin
		Select @ErrorCode = -1, @ErrorMessage = 'Error In JSON Structure'
		Return @ErrorCode
	End
	
	-- Insert Records into Table TBL_GSTR2A

	Insert TBL_GSTR2A (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR2A_B2B t1
	Where Not Exists(Select 1 From TBL_GSTR2A t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR2A_B2B 
	SET #TBL_GSTR2A_B2B.gstr2aid = t2.gstr2aid 
	FROM #TBL_GSTR2A_B2B t1,
			TBL_GSTR2A t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR2A_B2B

	Insert TBL_GSTR2A_B2B(gstr2aid,ctin,cfs,gstinid) 
	Select	distinct gstr2aid,ctin,cfs,gstinid
	From #TBL_GSTR2A_B2B t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_B2B t2 where t2.gstr2aid = t1.gstr2aid and t2.ctin = t1.ctin) 

	Update #TBL_GSTR2A_B2B 
	SET #TBL_GSTR2A_B2B.b2bid = t2.b2bid 
	FROM #TBL_GSTR2A_B2B t1,
			TBL_GSTR2A_B2B t2 
	WHERE t1.gstr2aid = t2.gstr2aid 
	And t1.ctin = t2.ctin 

	-- Insert Records into Table TBL_GSTR2A_B2B_INV

	Insert TBL_GSTR2A_B2B_INV
	(b2bid,chksum,inum,idt,val,pos,rchrg,inv_typ,gstinid,gstr2aid)
	Select	distinct t1.b2bid,t1.chksum,t1.inum,t1.idt,t1.val,
			t1.pos,t1.rchrg,t1.inv_typ,t1.gstinid,t1.gstr2aid
	From #TBL_GSTR2A_B2B t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR2A_B2B_INV t2
						Where t2.b2bid = t1.b2bid 
							And t2.inum = t1.inum
							And t2.idt = t1.idt)		 


	Update #TBL_GSTR2A_B2B 
	SET #TBL_GSTR2A_B2B.invid = t2.invid 
	FROM #TBL_GSTR2A_B2B t1,
			TBL_GSTR2A_B2B_INV t2 
	WHERE t1.b2bid= t2.b2bid 
	And t1.inum = t2.inum
	And t1.idt = t2.idt

	if exists (select 1 from  TBL_GSTR2A_B2B_INV_ITMS where invid in (select distinct invid from #TBL_GSTR2A_B2B where isnull(invid,0) <> 0))
	Begin
		Select itmsid into #itmsid from TBL_GSTR2A_B2B_INV_ITMS where invid in (select distinct invid from #TBL_GSTR2A_B2B where isnull(invid,0) <> 0)
		Delete from TBL_GSTR2A_B2B_INV_ITMS_DET where itmsid in (select itmsid from  #itmsid)
		Delete from TBL_GSTR2A_B2B_INV_ITMS where itmsid in (select itmsid from  #itmsid)
	End
	
	/*
	Update #TBL_GSTR2A_B2B 
	SET #TBL_GSTR2A_B2B.num = t3.num
	FROM #TBL_GSTR2A_B2B t1,
			(Select slno,invid,Row_Number()  OVER(Partition By t2.invid Order By t2.invid) as num
			FROM #TBL_GSTR2A_B2B t2
	   		) t3
	Where t1.invid = t3.invid
	And t1.slno = t3.slno */

	Insert TBL_GSTR2A_B2B_INV_ITMS
	(invid,num,gstinid,gstr2aid)
	Select	distinct t1.invid,t1.num,t1.gstinid,gstr2aid
	From #TBL_GSTR2A_B2B  t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_B2B_INV_ITMS t2
						Where t2.invid = t1.invid 
						And t2.num = t1.num)
		
	
	Update #TBL_GSTR2A_B2B 
	SET #TBL_GSTR2A_B2B.itmsid = t2.itmsid
	FROM #TBL_GSTR2A_B2B t1,
			TBL_GSTR2A_B2B_INV_ITMS t2 
	WHERE t1.invid= t2.invid 
	And t1.num = t2.num
	
	Insert TBL_GSTR2A_B2B_INV_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2aid)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinid,gstr2aid
	From #TBL_GSTR2A_B2B t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR2A_B2B_INV_ITMS_DET t2
						Where t2.itmsid = t1.itmsid)
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End

