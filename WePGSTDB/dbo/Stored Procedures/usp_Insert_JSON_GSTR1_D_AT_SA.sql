
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D AT JSON Records to the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri			Initial Version
08/19/2017	Seshadri	Fixed Errors during record insertion
08/23/2017	Seshadri	Corrected Insertion Errors
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_AT_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_AT_SA]
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
	Values('Gstr1 - AT',@Gstin,@RecordContents,1,1,GetDate()) */

	Select	space(50) as gstr1did,
			space(50) as atid,
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
	Into #TBL_GSTR1_D_AT
	From OPENJSON(@RecordContents)
	/*
	WITH
	(
	  [at] nvarchar(max) as JSON
	) As Ats
	Cross Apply OPENJSON(Ats.[at]) */
	WITH
	(
		chksum varchar(64),
		pos varchar(5),
		sply_ty varchar(5),
		itms nvarchar(max) as JSON
	) As [At]
	Cross Apply OPENJSON([At].itms)
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
	From  #TBL_GSTR1_D_AT t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update  #TBL_GSTR1_D_AT 
	SET  #TBL_GSTR1_D_AT.gstr1did = t2.gstr1did 
	FROM  #TBL_GSTR1_D_AT t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_AT

	Insert TBL_GSTR1_D_AT(gstr1did,chksum,pos,sply_ty,gstinid) 
	Select	distinct gstr1did , chksum, pos, sply_ty,gstinid
	From  #TBL_GSTR1_D_AT t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_AT t2 
						Where t2.gstr1did = t1.gstr1did
						And(t2.flag is Null Or t2.flag <> 'D')
					 ) 

	Update  #TBL_GSTR1_D_AT
	SET  #TBL_GSTR1_D_AT.atid = t2.atid 
	FROM  #TBL_GSTR1_D_AT t1,
		TBL_GSTR1_D_AT t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And(t2.flag is Null Or t2.flag <> 'D')

	-- Insert Records into Table TBL_GSTR1_D_AT_ITMS

	Insert TBL_GSTR1_D_AT_ITMS
	(atid,rt,
		ad_amt,iamt,camt,samt,csamt,gstinid,gstr1did)
	Select	distinct t1.atid,t1.rt,
			t1.ad_amt,t1.iamt,t1.camt,t1.samt,t1.csamt,t1.gstinid,t1.gstr1did
	From  #TBL_GSTR1_D_AT t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_AT_ITMS t2
						Where t2.atid = t1.atid 
							And t2.rt = t1.rt
						)		 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End