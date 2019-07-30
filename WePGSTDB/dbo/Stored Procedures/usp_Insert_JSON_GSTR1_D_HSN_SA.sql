
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D HSN JSON Records to the corresponding statging
				area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri	Initial Version
08/18/2017	Seshadri	Modified the Insert Statment related to the table TBL_GSTR1_D_HSN_DATA
08/19/2017	Seshadri	Fixed Errors during record insertion
08/20/2017	Seshadri	Fixed Errors during record retrieval
08/23/2017	Seshadri	Corrected Insertion Errors
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_HSN_SA
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_HSN_SA]
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
	Values('Gstr1 - HSN',@Gstin,@RecordContents,1,1,GetDate()) 
	 */
	 

	Select	space(50) as gstr1did,
			space(50) as hsnid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			chksum as chksum,
			hsn_sc as hsn_sc,
			[desc] as descs,
			uqc as uqc,
			qty as qty,
			val as val,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_HSN
	From OPENJSON(@RecordContents) 
	WITH
	(	
		chksum varchar(64),
		data nvarchar(max) as JSON
	) As Hsn
	Cross Apply OPENJSON(Hsn.data) 
	WITH
	(
		num int,
		hsn_sc varchar(50),
		[desc] varchar(50),
		uqc varchar(50),
		qty decimal(18,2),
		val decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As HsnData
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	
	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_HSN t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update  #TBL_GSTR1_D_HSN 
	SET  #TBL_GSTR1_D_HSN.gstr1did = t2.gstr1did 
	FROM  #TBL_GSTR1_D_HSN t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_HSN

	Insert TBL_GSTR1_D_HSN(gstr1did,chksum,gstinId) 
	Select	distinct gstr1did,chksum,gstinId
	From #TBL_GSTR1_D_HSN t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_HSN t2 
						Where t2.gstr1did = t1.gstr1did
						And(t2.flag is Null Or t2.flag <> 'D')
					 ) 

	Update #TBL_GSTR1_D_HSN
	SET #TBL_GSTR1_D_HSN.hsnid = t2.hsnid 
	FROM #TBL_GSTR1_D_HSN t1,
		TBL_GSTR1_D_HSN t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And(t2.flag is Null Or t2.flag <> 'D')
	

	-- Insert Records into Table TBL_GSTR1_HSN_DATA

	Insert TBL_GSTR1_D_HSN_DATA
	(hsnid,hsn_sc,descs,uqc,qty,
		txval,iamt,camt,samt,csamt,gstinId,gstr1did)
	Select	distinct t1.hsnid,t1.hsn_sc,t1.descs,uqc,qty,
			txval,iamt,camt,samt,csamt,gstinId,gstr1did
	From #TBL_GSTR1_D_HSN t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_HSN_DATA t2
						Where t2.hsnid = t1.hsnid 
							And t2.hsn_sc = t1.hsn_sc
							And t2.qty = t1.qty
							And t2.txval = t1.txval)		 


	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End