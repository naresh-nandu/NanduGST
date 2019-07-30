
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D Nil JSON Records to the corresponding staging area
			  tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/01/2017	Seshadri			Initial Version
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_NIL_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_NIL_SA]
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
			space(50) as nilid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			chksum as chksum,
			nil_amt as nil_amt,
			expt_amt as expt_amt,
			ngsup_amt as ngsup_amt,
			sply_ty as sply_ty
	Into #TBL_GSTR1_D_NIL
	From OPENJSON(@RecordContents) 
	WITH
	(	
		chksum varchar(64),
		inv nvarchar(max) as JSON
	) As Nil
	Cross Apply OPENJSON(Nil.inv) 
	WITH
	(
		nil_amt decimal(18,2),
		expt_amt decimal(18,2),
		ngsup_amt decimal(18,2),
		sply_ty varchar(25)
	) As Inv
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From  #TBL_GSTR1_D_NIL t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update  #TBL_GSTR1_D_NIL 
	SET  #TBL_GSTR1_D_NIL.gstr1did = t2.gstr1did 
	FROM  #TBL_GSTR1_D_NIL t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_NIL

	Insert TBL_GSTR1_D_NIL(gstr1did,chksum,gstinId) 
	Select	distinct gstr1did,chksum,gstinId
	From #TBL_GSTR1_D_NIL t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_NIL t2 
						Where t2.gstr1did = t1.gstr1did
						And(t2.flag is Null Or t2.flag <> 'D') 
					 ) 

	Update #TBL_GSTR1_D_NIL
	SET #TBL_GSTR1_D_NIL.nilid = t2.nilid 
	FROM #TBL_GSTR1_D_NIL t1,
			TBL_GSTR1_D_NIL t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And(t2.flag is Null Or t2.flag <> 'D')

	-- Insert Records into Table TBL_GSTR1_D_NIL_INV

	Insert TBL_GSTR1_D_NIL_INV
	(nilid,sply_ty,
		nil_amt,expt_amt,ngsup_amt,gstinId,gstr1did)
	Select	distinct t1.nilid,t1.sply_ty,
			t1.nil_amt,t1.expt_amt,t1.ngsup_amt,t1.gstinId,t1.gstr1did
	From #TBL_GSTR1_D_NIL t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_NIL_INV t2
						Where t2.nilid = t1.nilid 
							And t2.sply_ty = t1.sply_ty
						)		 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End