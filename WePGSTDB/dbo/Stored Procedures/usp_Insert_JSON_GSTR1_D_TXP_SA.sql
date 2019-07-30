
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D TXP JSON Records to the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri			Initial Version
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_TXP_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_TXP_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	space(50) as gstr1did,
			space(50) as txpdid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			chksum as chksum,
			pos as pos,
			sply_ty as sply_ty,
			rt as rt,
			ad_amt as ad_amt,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_TXP
	From OPENJSON(@RecordContents) 
	WITH
	(
	  txpd nvarchar(max) as JSON
	) As Txpds
	Cross Apply OPENJSON(Txpds.txpd) 
	WITH
	(
		chksum varchar(64),
		pos varchar(5),
		sply_ty varchar(5),
		itms nvarchar(max) as JSON
	) As Txpd
	Cross Apply OPENJSON(Txpd.itms)
	WITH
	(
		rt decimal(18,2),
		ad_amt decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Itms
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_TXP t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_TXP 
	SET #TBL_GSTR1_D_TXP.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_TXP t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_TXPD

	Insert TBL_GSTR1_D_TXPD(gstr1did,chksum,pos,sply_ty,gstinid) 
	Select	distinct gstr1did , chksum, pos, sply_ty,gstinid
	From #TBL_GSTR1_D_TXP t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_TXPD t2 
						Where t2.gstr1did = t1.gstr1did
						And(t2.flag is Null Or t2.flag <> 'D')
					 ) 

	Update #TBL_GSTR1_D_TXP
	SET #TBL_GSTR1_D_TXP.txpdid = t2.txpdid 
	FROM #TBL_GSTR1_D_TXP t1,
		TBL_GSTR1_D_TXPD t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And(t2.flag is Null Or t2.flag <> 'D')

	-- Insert Records into Table TBL_GSTR1_D_TXPD_ITMS

	Insert TBL_GSTR1_D_TXPD_ITMS
	(txpdid,rt,
		ad_amt,iamt,camt,samt,csamt,gstinid,gstr1did)
	Select	distinct t1.txpdid,t1.rt,
			t1.ad_amt,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr1did
	From #TBL_GSTR1_D_TXP t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_TXPD_ITMS t2
						Where t2.txpdid = t1.txpdid 
							And t2.rt = t1.rt
						)		 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End