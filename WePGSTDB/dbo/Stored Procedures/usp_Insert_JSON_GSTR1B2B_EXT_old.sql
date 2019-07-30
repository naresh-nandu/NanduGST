
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 B2B JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1B2B_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1B2B_EXT_old]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@Gt decimal(18,2),
	@Cur_Gt decimal(18,2),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	
	Select	@Gstin as gstin,
			@Fp as fp,
			@Gt as gt,
			@Cur_Gt as cur_gt,
			ctin as ctin ,
			inum as inum,
			idt as idt,
			val as val ,
			pos as pos,
			rchrg as rchrg,
			etin as etin,
			inv_typ as inv_typ,
			rt as rt ,
			txval as txval,
			iamt as iamt ,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_EXT_GSTR1_B2B_INV
	From OPENJSON(@RecordContents) 	
	WITH
	(
		ctin varchar(15),
		inv nvarchar(max) as JSON
	) As B2b
	Cross Apply OPENJSON(B2b.inv) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
		val decimal(18,2),
		pos varchar(2),
		rchrg varchar(1),
		etin varchar(15),
		inv_typ varchar(1),
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

	-- Insert the Records Into External Tables

	Insert TBL_EXT_GSTR1_B2B_INV
	(	gstin, fp, gt, cur_gt, ctin,inum, idt, val, pos, rchrg, etin, inv_typ, 
		rt, txval, iamt, camt, samt, csamt,
		rowstatus, sourcetype, referenceno, createddate)
	Select 
		gstin, fp, gt, cur_gt, ctin,inum, idt, val, pos, rchrg, etin, inv_typ, 
		rt, txval, iamt, camt, samt, csamt,
		1 ,@SourceType ,@ReferenceNo,GetDate()
	 From #TBL_EXT_GSTR1_B2B_INV t1
	Where Not Exists(Select 1 From  TBL_EXT_GSTR1_B2B_INV 
						Where gstin = t1.gstin 
						And inum = t1.inum 
						And idt = t1.idt)

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End