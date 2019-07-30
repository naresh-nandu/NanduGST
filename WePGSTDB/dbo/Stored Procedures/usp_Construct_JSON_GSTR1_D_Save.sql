
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1_D Save
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/21/2017	Seshadri			Initial Version
8/24/2017	Seshadri	Modified the code related to Flag logic in
						all the Action Types
8/28/2017	Seshadri	Modified the code related to TXP Action Types
8/30/2017	Seshadri	Modified the code to include the Flag Value 0

*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_D_Save '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Construct_JSON_GSTR1_D_Save] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
    @JsonResult nvarchar(MAX) = NULL Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @Res nvarchar(Max)

	Set @Res = (
	Select	gstin,fp,gt,
	--cur_gt,
	(	Select	ctin,
		(	Select inum, idt,val, pos,rchrg,etin,inv_typ,flag,chksum,
			(	Select num, JSON_QUERY
						((
							Select rt ,txval,iamt,camt,samt,csamt 
							From TBL_GSTR1_D_B2B_INV_ITMS_DET 
							Where itmsid = TBL_GSTR1_D_B2B_INV_ITMS.itmsid
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det
				From TBL_GSTR1_D_B2B_INV_ITMS
				Where invid = TBL_GSTR1_D_B2B_INV.invid 
				And  TBL_GSTR1_D_B2B_INV.b2bid = TBL_GSTR1_D_B2B.b2bid
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_D_B2B_INV
			Where b2bid = TBL_GSTR1_D_B2B.b2bid
			AND flag Not In ('U','0')
			FOR JSON PATH	
		) As inv
		From TBL_GSTR1_D_B2B 
		Where TBL_GSTR1_D_B2B.gstr1did in 
			(	Select TBL_GSTR1_D.gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp
			) 
		And TBL_GSTR1_D_B2B.b2bid in
			(	Select b2bid
				From TBL_GSTR1_D_B2B_INV
				Where flag Not In ('U','0')
				And gstr1did In
				(
					Select TBL_GSTR1_D.gstr1did 
					From TBL_GSTR1_D 
					Where TBL_GSTR1_D.gstin = @Gstin 
					And TBL_GSTR1_D.fp = @Fp
				)
			)
		Group by TBL_GSTR1_D_B2B.b2bid,TBL_GSTR1_D_B2B.ctin 
		FOR JSON PATH
	) As b2b,

	(	Select	pos,
		(	Select inum, idt,val,etin,flag,chksum,
			(	Select num, JSON_QUERY
						((
							Select rt,txval,iamt,csamt 
							From TBL_GSTR1_D_B2CL_INV_ITMS_DET 
							Where itmsid = TBL_GSTR1_D_B2CL_INV_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det
				From TBL_GSTR1_D_B2CL_INV_ITMS
				Where invid = TBL_GSTR1_D_B2CL_INV.invid 
				And  TBL_GSTR1_D_B2CL_INV.b2clid = TBL_GSTR1_D_B2CL.b2clid
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_D_B2CL_INV
			Where b2clid = 	TBL_GSTR1_D_B2CL.b2clid
			And flag Not In ('U','0')
			FOR JSON PATH	
		) As inv
		From TBL_GSTR1_D_B2CL  
		Where gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp
			) 
		And TBL_GSTR1_D_B2CL.b2clid in
			(	Select b2clid
				From TBL_GSTR1_D_B2CL_INV
				Where flag Not In ('U','0')
				And gstr1did In
				(
					Select TBL_GSTR1_D.gstr1did 
					From TBL_GSTR1_D 
					Where TBL_GSTR1_D.gstin = @Gstin 
					And TBL_GSTR1_D.fp = @Fp
				)
			)
		Group by TBL_GSTR1_D_B2CL.b2clid,TBL_GSTR1_D_B2CL.pos 
		FOR JSON PATH
	) As b2cl,

	(	Select	sply_ty,txval,typ,etin,pos,rt,iamt,camt,samt,csamt,flag,chksum
		From TBL_GSTR1_D_B2CS
		Where flag Not In ('U','0')
		And gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp
			) 
		FOR JSON PATH
	) As b2cs,

	(	Select	ex_tp as exp_typ,
		(	Select inum, idt,val,sbpcode,sbnum,sbdt,flag,chksum,
			(	Select txval,rt,iamt
				From TBL_GSTR1_D_EXP_INV_ITMS
				Where invid = TBL_GSTR1_D_EXP_INV.invid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_D_EXP_INV
			Where expid = TBL_GSTR1_D_EXP.expid
			And flag Not In ('U','0')
			FOR JSON PATH	
		) As inv
		From TBL_GSTR1_D_EXP  
		Where gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp
			) 
		And TBL_GSTR1_D_EXP.expid in
			(	Select expid
				From TBL_GSTR1_D_EXP_INV
				Where flag Not In ('U','0')
				And gstr1did In
				(
					Select TBL_GSTR1_D.gstr1did 
					From TBL_GSTR1_D 
					Where TBL_GSTR1_D.gstin = @Gstin 
					And TBL_GSTR1_D.fp = @Fp
				)
			)
		Group by TBL_GSTR1_D_EXP.expid,TBL_GSTR1_D_EXP.ex_tp 
		FOR JSON PATH
	) As 'exp',

	(	Select	ctin,
		(	Select ntty,nt_num,nt_dt,p_gst,rsn,inum, idt,val,flag,chksum, 
			(	Select num,JSON_QUERY
						((
							Select rt,txval,iamt,camt,samt,csamt 
							From TBL_GSTR1_D_CDNR_NT_ITMS_DET 
							Where itmsid = TBL_GSTR1_D_CDNR_NT_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det

				From TBL_GSTR1_D_CDNR_NT_ITMS
				Where ntid = TBL_GSTR1_D_CDNR_NT.ntid 
				And  TBL_GSTR1_D_CDNR_NT.cdnrid = TBL_GSTR1_D_CDNR.cdnrid
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_D_CDNR_NT
			Where cdnrid = 	TBL_GSTR1_D_CDNR.cdnrid
			And flag Not In ('U','0')
			FOR JSON PATH	
		) As nt
		From TBL_GSTR1_D_CDNR  
		Where gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp
			)
		And TBL_GSTR1_D_CDNR.cdnrid in
			(	Select cdnrid
				From TBL_GSTR1_D_CDNR_NT
				Where flag Not In ('U','0')
				And gstr1did In
				(
					Select TBL_GSTR1_D.gstr1did 
					From TBL_GSTR1_D 
					Where TBL_GSTR1_D.gstin = @Gstin 
					And TBL_GSTR1_D.fp = @Fp
				)
			)
		Group by TBL_GSTR1_D_CDNR.cdnrid,TBL_GSTR1_D_CDNR.ctin 
		FOR JSON PATH
	) As cdnr,

	(	Select typ,ntty,nt_num,nt_dt,inum, idt,val,flag,chksum, 
		(	Select num,JSON_QUERY
					((	
						Select rt,txval,iamt,camt,samt,csamt 
						From TBL_GSTR1_D_CDNUR_ITMS_DET 
						Where itmsid = TBL_GSTR1_D_CDNUR_ITMS.itmsid 
						FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
					)) As itm_det

			From TBL_GSTR1_D_CDNUR_ITMS
			Where cdnurid = TBL_GSTR1_D_CDNUR.cdnurid 
			FOR JSON PATH
		) As itms
		From TBL_GSTR1_D_CDNUR
		Where cdnurid in (	Select cdnurid 
							From TBL_GSTR1_D_CDNUR 
							Where flag Not In ('U','0')
							And TBL_GSTR1_D_CDNUR.gstr1did in 
							(	Select gstr1did from TBL_GSTR1_D 
								Where TBL_GSTR1_D.gstin = @Gstin and TBL_GSTR1_D.fp = @Fp
							)
						)	 
		FOR JSON PATH	
	) As cdnur,
	
	(	Select JSON_QUERY
		((
			Select flag,chksum,
			(	
					Select 
					Row_Number()  OVER(Partition By hsnid Order By hsnid) as num,
					hsn_sc,descs as [desc],uqc,qty,
					--val,
					txval,iamt,camt,samt,csamt
					From TBL_GSTR1_D_HSN_DATA 
					Where hsnid = TBL_GSTR1_D_HSN.hsnid
					And flag Not In ('U','0')
					FOR JSON PATH
			) AS 'data'		 
			From TBL_GSTR1_D_HSN
			Where gstr1did in 
				(	Select gstr1did 
					From TBL_GSTR1_D 
					Where TBL_GSTR1_D.gstin = @Gstin 
					And TBL_GSTR1_D.fp = @Fp
				)
			And hsnid in
				(
					Select distinct hsnid From TBL_GSTR1_D_HSN_DATA
					Where flag Not In ('U','0')
					And gstr1did in 
						(	Select gstr1did 
							From TBL_GSTR1_D 
							Where TBL_GSTR1_D.gstin = @Gstin 
							And TBL_GSTR1_D.fp = @Fp
						)			
				) 
			FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		))
	) AS hsn, 
	
	(	Select flag,chksum, 
		(	Select nil_amt,expt_amt, ngsup_amt,sply_ty
			From TBL_GSTR1_D_NIL_INV 
			Where nilid = TBL_GSTR1_D_NIL.nilid
			FOR JSON PATH
		) AS inv
		From TBL_GSTR1_D_NIL 
		Where flag Not In ('U','0')
		And gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin
				And  TBL_GSTR1_D.fp = @Fp
			) 
		FOR JSON PATH
	) AS nil,
	
	(	Select pos,sply_ty,flag,chksum,
		(	Select rt,ad_amt, iamt, camt, samt, csamt
			From TBL_GSTR1_D_TXPD_ITMS 
			Where txpdid = TBL_GSTR1_D_TXPD.txpdid
			FOR JSON PATH
		) AS itms
		From TBL_GSTR1_D_TXPD 
		Where flag Not In ('U','0')
		And gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp 
			) 
		FOR JSON PATH
	) AS txpd,
		
	(	Select pos,sply_ty,flag,chksum,
		(	Select rt,ad_amt, iamt, camt, samt, csamt
			From TBL_GSTR1_D_AT_ITMS 
			Where atid = TBL_GSTR1_D_AT.atid
			FOR JSON PATH
		) AS itms
		From TBL_GSTR1_D_AT 
		Where flag Not In ('U','0')
		And gstr1did in 
			(	Select gstr1did 
				From TBL_GSTR1_D 
				Where TBL_GSTR1_D.gstin = @Gstin 
				And TBL_GSTR1_D.fp = @Fp 
			) 
		FOR JSON PATH
	) AS 'at',

	(			

			Select flag,chksum,JSON_QUERY
				 ((
					 Select doc_num,
						(	Select num,[from],[to],totnum,cancel,net_issue
							From TBL_GSTR1_D_DOCS
							Where docissueid = TBL_GSTR1_D_DOC_ISSUE. docissueid
							FOR JSON PATH
						) As docs
					From TBL_GSTR1_D_DOC_ISSUE
					Where gstr1did in 
						(	Select gstr1did 
							From TBL_GSTR1_D 
							Where TBL_GSTR1_D.gstin = @Gstin 
							And TBL_GSTR1_D.fp = @Fp 
						) 
					And flag Not In ('U','0')
					FOR JSON PATH	
				)) As doc_det

				From TBL_GSTR1_D_DOC_ISSUE
				Where gstr1did in 
					(	Select gstr1did 
						From TBL_GSTR1_D 
						Where TBL_GSTR1_D.gstin = @Gstin 
						And TBL_GSTR1_D.fp = @Fp 
					)
				And flag Not In ('U','0')
			
			FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
		
	)   As doc_issue 
	

	From TBL_GSTR1_D 
	Where gstin = @Gstin 
	And TBL_GSTR1_D.fp = @fp 
	Group by TBL_GSTR1_D.gstin, TBL_GSTR1_D.fp,TBL_GSTR1_D.gt
	--,TBL_GSTR1_D.cur_gt 
	FOR JSON AUTO   

	)

	Select @JsonResult = @Res

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End