
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D B2CS JSON Records to the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/01/2017	Seshadri			Initial Version
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_B2CS_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_B2CS_SA]
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
			space(50) as b2csid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			flag as flag,
			chksum as chksum,
			sply_ty as sply_ty,
			txval as txval,
			typ as typ,
			etin as etin,
			pos as pos,
			rt as rt,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_B2CS
	From OPENJSON(@RecordContents) 
	WITH
	(
		flag varchar(1),
		chksum varchar(64),
		sply_ty varchar(5),
		txval decimal(18,2),
		typ varchar(2),
		etin varchar(15),
		pos varchar(2),
		rt decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As B2cs
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_B2CS t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_B2CS 
	SET #TBL_GSTR1_D_B2CS.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_B2CS t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_B2CS

	Insert TBL_GSTR1_D_B2CS(gstr1did,flag,chksum,sply_ty,typ,etin,pos,rt,txval,iamt,camt,samt,csamt,gstinid) 
	Select	gstr1did,flag,chksum,sply_ty,typ,etin,pos,rt,txval,iamt,camt,samt,csamt,gstinid 
	From #TBL_GSTR1_D_B2CS t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_B2CS t2 
						Where t2.gstr1did = t1.gstr1did 
						And t2.pos = t1.pos
						And(t2.flag is Null Or t2.flag <> 'D')
					) 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End