
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Save
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/12/2017	Seshadri			Initial Version
8/8/2017	Karthik		Add @JsonResult parameter to resolve truncation issue
8/18/2017	Seshadri	Modified the code related to HSN
8/18/2017	Seshadri	Modified the code related to Exp
8/20/2017	Seshadri	Modified the code related to Reason Code
8/24/2017	Seshadri	Modified the code related to Flag logic in
						B2B,B2CL,EXP,CDNR,HSN,DOCISSUE
9/3/2017	Seshadri	Fixed Truncation Issues
9/4/2017    Seshadri	Made flag Changes to all Sections
9/5/2017	Seshadri	Fixed Json Error For Nil Record Type
9/5/2017    Sheshadri/Karthik Added Flag parameter to use the reupload option
9/7/2017	Seshadri	Fixed Export Issue - sbpcode,sbnum,sbdt
9/7/2017	Seshadri	Fixed AT & TXP Issue 
10/12/2017	Seshadri	Segregated JSON Construction based on Action Type
22/02/2017	Raja		In EXP "csamt" added and CDNR, CDNUR "rsn" removed
07/03/2017	Raja		In CDNUR "camt & samt" columns removed
03/10/2018	Seshadri	Made code changes related to Amendments
03/23/2018	Seshadri	Made code changes for diff_percent for all actions
28/03/2018	Raja		For Mobile APP 'Flag' changes done
*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_Save '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [dbo].[usp_Construct_JSON_GSTR1_MOBILE_APP] 
	@Gstin varchar(15),
	@Fp varchar(10),
	@ActionType varchar(15) = Null,
	@Flag varchar(1),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if (IsNull(@ActionType,'') = '')
	Begin
		Select @ActionType = 'ALL'
	End

	if @ActionType = 'B2B'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ctin,
			(	Select inum, idt,val, pos,rchrg,etin,inv_typ,
				(	Select num, JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_B2B_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2B_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2B_INV_ITMS
					Where invid = TBL_GSTR1_B2B_INV.invid 
					And  TBL_GSTR1_B2B_INV.b2bid = TBL_GSTR1_B2B.b2bid
					And IsNull(TBL_GSTR1_B2B_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2B_INV
				Where b2bid = TBL_GSTR1_B2B.b2bid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2B 
			Where TBL_GSTR1_B2B.gstr1id in 
				(	Select TBL_GSTR1.gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2B.b2bid in
				(	Select b2bid
					From TBL_GSTR1_B2B_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2B.b2bid,TBL_GSTR1_B2B.ctin 
			FOR JSON PATH
		) As b2b

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  
		
	End
	else if @ActionType = 'B2CL'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	pos,
			(	Select inum, idt,val,etin,
				(	Select num, JSON_QUERY
							((
								Select rt,txval,iamt,csamt 
								From TBL_GSTR1_B2CL_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2CL_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2CL_INV_ITMS
					Where invid = TBL_GSTR1_B2CL_INV.invid 
					And  TBL_GSTR1_B2CL_INV.b2clid = TBL_GSTR1_B2CL.b2clid
					And IsNull(TBL_GSTR1_B2CL_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2CL_INV
				Where b2clid = 	TBL_GSTR1_B2CL.b2clid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2CL  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2CL.b2clid in
				(	Select b2clid
					From TBL_GSTR1_B2CL_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2CL.b2clid,TBL_GSTR1_B2CL.pos 
			FOR JSON PATH
		) As b2cl

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  


	End
	else if @ActionType = 'B2CS'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select	sply_ty,txval,typ,etin,pos,rt,iamt,camt,samt,csamt
			From TBL_GSTR1_B2CS
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			FOR JSON PATH
		) As b2cs

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	
	End
	else if @ActionType = 'EXP'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ex_tp as exp_typ,
			(	Select inum, idt,val,
				(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
					 Else Null
				End) as sbpcode ,
				(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
					 Else Null
				End) as sbnum ,
				(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
					 Else Null
				End) as sbdt ,
				(	Select txval,rt,iamt,csamt
					From TBL_GSTR1_EXP_INV_ITMS
					Where invid = TBL_GSTR1_EXP_INV.invid 
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_EXP_INV
				Where expid = TBL_GSTR1_EXP.expid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_EXP  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_EXP.expid in
				(	Select expid
					From TBL_GSTR1_EXP_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_EXP.expid,TBL_GSTR1_EXP.ex_tp 
			FOR JSON PATH
		) As 'exp'

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'CDNR'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ctin,
			(	Select ntty,nt_num,nt_dt,p_gst,inum, idt,val, --rsn,
				(	Select num,JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_CDNR_NT_ITMS_DET 
								Where itmsid = TBL_GSTR1_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det

					From TBL_GSTR1_CDNR_NT_ITMS
					Where ntid = TBL_GSTR1_CDNR_NT.ntid 
					And  TBL_GSTR1_CDNR_NT.cdnrid = TBL_GSTR1_CDNR.cdnrid
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_CDNR_NT
				Where cdnrid = 	TBL_GSTR1_CDNR.cdnrid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As nt
			From TBL_GSTR1_CDNR  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)
			And TBL_GSTR1_CDNR.cdnrid in
				(	Select cdnrid
					From TBL_GSTR1_CDNR_NT
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_CDNR.cdnrid,TBL_GSTR1_CDNR.ctin 
			FOR JSON PATH
		) As cdnr

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  


	End
	else if @ActionType = 'CDNUR'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select typ,ntty,nt_num,nt_dt,inum, idt,val,p_gst,--rsn,
			(	Select num,JSON_QUERY
						((	
							Select rt,txval,iamt,csamt --camt,samt
							From TBL_GSTR1_CDNUR_ITMS_DET 
							Where itmsid = TBL_GSTR1_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det

				From TBL_GSTR1_CDNUR_ITMS
				Where cdnurid = TBL_GSTR1_CDNUR.cdnurid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR1_CDNUR 
								Where IsNull(flag,'') <> @Flag
								And TBL_GSTR1_CDNUR.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnur

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType In ('HSN','HSNSUM')
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select JSON_QUERY
			((
				Select 
				(	
						Select 
						Row_Number()  OVER(Partition By hsnid Order By hsnid) as num,
						hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt
						From TBL_GSTR1_HSN_DATA 
						Where hsnid = TBL_GSTR1_HSN.hsnid
						And IsNull(flag,'') <> @Flag
						FOR JSON PATH
				) AS 'data'		 
				From TBL_GSTR1_HSN
				Where gstr1id in 
					(	Select gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				And hsnid in
					(
						Select distinct hsnid From TBL_GSTR1_HSN_DATA
						Where IsNull(flag,'') <> @Flag
						And gstr1id in 
							(	Select gstr1id 
								From TBL_GSTR1 
								Where TBL_GSTR1.gstin = @Gstin 
								And TBL_GSTR1.fp = @Fp
							)			
					) 
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		) AS hsn

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	
	End
	else if @ActionType = 'NIL'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select JSON_QUERY
			((  Select 
				(	Select nil_amt,expt_amt, ngsup_amt,sply_ty
					From TBL_GSTR1_NIL_INV 
					Where nilid = TBL_GSTR1_NIL.nilid
					FOR JSON PATH
				) AS inv
				From TBL_GSTR1_NIL 
				Where IsNull(flag,'') <> @Flag
				And gstr1id in 
					(	Select gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin
						And  TBL_GSTR1.fp = @Fp
					) 
				FOR JSON PATH,  WITHOUT_ARRAY_WRAPPER
			))
		) AS nil

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	
	End
	else if @ActionType in('TXP','TXPD')
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_TXP_ITMS 
				Where txpid = TBL_GSTR1_TXP.txpid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_TXP 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXP_ITMS Where TBL_GSTR1_TXP_ITMS.txpid = txpid)
			FOR JSON PATH
		) AS txpd

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'AT'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_AT_ITMS 
				Where atid = TBL_GSTR1_AT.atid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_AT 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_AT_ITMS Where TBL_GSTR1_AT_ITMS.atid = atid)
			FOR JSON PATH
		) AS 'at'

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'DOCISSUE'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select JSON_QUERY 
			((	

				Select JSON_QUERY
					 ((
						 Select doc_num,
							(	Select num,[from],[to],totnum,cancel,net_issue
								From TBL_GSTR1_DOCS
								Where docissueid = TBL_GSTR1_DOC_ISSUE. docissueid
								FOR JSON PATH
							) As docs
						From TBL_GSTR1_DOC_ISSUE
						Where gstr1id in 
							(	Select gstr1id 
								From TBL_GSTR1 
								Where TBL_GSTR1.gstin = @Gstin 
								And TBL_GSTR1.fp = @Fp 
							)
						And IsNull(flag,'') <> @Flag
						And Exists(Select 1 From TBL_GSTR1_DOCS Where TBL_GSTR1_DOCS.docissueid = docissueid)
						FOR JSON PATH	
					)) As doc_det

					From TBL_GSTR1_DOC_ISSUE
					Where gstr1id in 
						(	Select gstr1id 
							From TBL_GSTR1 
							Where TBL_GSTR1.gstin = @Gstin 
							And TBL_GSTR1.fp = @Fp 
						)
					And IsNull(flag,'') <> @Flag
					And Exists(Select 1 From TBL_GSTR1_DOCS Where TBL_GSTR1_DOCS.docissueid = docissueid)

				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		
		)   As doc_issue 

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End

	else if @ActionType = 'B2BA'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ctin,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val, pos,rchrg,etin,inv_typ,
				(	Select num, JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_B2BA_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2BA_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2BA_INV_ITMS
					Where invid = TBL_GSTR1_B2BA_INV.invid 
					And  TBL_GSTR1_B2BA_INV.b2baid = TBL_GSTR1_B2BA.b2baid
					And IsNull(TBL_GSTR1_B2BA_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2BA_INV
				Where b2baid = TBL_GSTR1_B2BA.b2baid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2BA 
			Where TBL_GSTR1_B2BA.gstr1id in 
				(	Select TBL_GSTR1.gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2BA.b2baid in
				(	Select b2baid
					From TBL_GSTR1_B2BA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2BA.b2baid,TBL_GSTR1_B2BA.ctin 
			FOR JSON PATH
		) As b2ba

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  
		
	End
	else if @ActionType = 'B2CLA'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	pos,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val,etin,
				(	Select num, JSON_QUERY
							((
								Select rt,txval,iamt,csamt 
								From TBL_GSTR1_B2CLA_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2CLA_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2CLA_INV_ITMS
					Where invid = TBL_GSTR1_B2CLA_INV.invid 
					And  TBL_GSTR1_B2CLA_INV.b2claid = TBL_GSTR1_B2CLA.b2claid
					And IsNull(TBL_GSTR1_B2CLA_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2CLA_INV
				Where b2claid = 	TBL_GSTR1_B2CLA.b2claid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2CLA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2CLA.b2claid in
				(	Select b2claid
					From TBL_GSTR1_B2CLA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2CLA.b2claid,TBL_GSTR1_B2CLA.pos 
			FOR JSON PATH
		) As b2cla

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  


	End
	else if @ActionType = 'B2CSA'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			pos,sply_ty,typ,etin,
			(	Select rt,txval, iamt, camt, samt, csamt
				From TBL_GSTR1_B2CSA_ITMS 
				Where b2csaid = TBL_GSTR1_B2CSA.b2csaid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_B2CSA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_B2CSA_ITMS Where TBL_GSTR1_B2CSA_ITMS.b2csaid = b2csaid)
			FOR JSON PATH
		) AS b2csa

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'EXPA'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ex_tp as exp_typ,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val,
				(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
					 Else Null
				End) as sbpcode ,
				(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
					 Else Null
				End) as sbnum ,
				(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
					 Else Null
				End) as sbdt ,
				(	Select txval,rt,iamt,csamt
					From TBL_GSTR1_EXPA_INV_ITMS
					Where invid = TBL_GSTR1_EXPA_INV.invid 
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_EXPA_INV
				Where expaid = TBL_GSTR1_EXPA.expaid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_EXPA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_EXPA.expaid in
				(	Select expaid
					From TBL_GSTR1_EXPA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_EXPA.expaid,TBL_GSTR1_EXPA.ex_tp 
			FOR JSON PATH
		) As 'expa'

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'CDNRA'
	Begin

		Select	gstin,fp,gt,cur_gt,

		(	Select	ctin,
			(	Select ont_num,ont_dt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				ntty,nt_num,nt_dt,p_gst,inum, idt,val, --rsn,
				(	Select num,JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_CDNRA_NT_ITMS_DET 
								Where itmsid = TBL_GSTR1_CDNRA_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det

					From TBL_GSTR1_CDNRA_NT_ITMS
					Where ntid = TBL_GSTR1_CDNRA_NT.ntid 
					And  TBL_GSTR1_CDNRA_NT.cdnraid = TBL_GSTR1_CDNRA.cdnraid
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_CDNRA_NT
				Where cdnraid = 	TBL_GSTR1_CDNRA.cdnraid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As nt
			From TBL_GSTR1_CDNRA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)
			And TBL_GSTR1_CDNRA.cdnraid in
				(	Select cdnraid
					From TBL_GSTR1_CDNRA_NT
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_CDNRA.cdnraid,TBL_GSTR1_CDNRA.ctin 
			FOR JSON PATH
		) As cdnra

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  


	End
	else if @ActionType = 'CDNURA'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select ont_num,ont_dt,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			typ,ntty,nt_num,nt_dt,inum, idt,val,p_gst,--rsn,
			(	Select num,JSON_QUERY
						((	
							Select rt,txval,iamt,csamt --camt,samt
							From TBL_GSTR1_CDNURA_ITMS_DET 
							Where itmsid = TBL_GSTR1_CDNURA_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det

				From TBL_GSTR1_CDNURA_ITMS
				Where cdnuraid = TBL_GSTR1_CDNURA.cdnuraid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_CDNURA
			Where cdnuraid in (	Select cdnuraid 
								From TBL_GSTR1_CDNURA 
								Where IsNull(flag,'') <> @Flag
								And TBL_GSTR1_CDNURA.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnura

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType in('TXPA','TXPDA')
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_TXPA_ITMS 
				Where txpaid = TBL_GSTR1_TXPA.txpaid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_TXPA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXPA_ITMS Where TBL_GSTR1_TXPA_ITMS.txpaid = txpaid)
			FOR JSON PATH
		) AS txpda

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End
	else if @ActionType = 'ATA'
	Begin
	
		Select	gstin,fp,gt,cur_gt,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_ATA_ITMS 
				Where ataid = TBL_GSTR1_ATA.ataid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_ATA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_ATA_ITMS Where TBL_GSTR1_ATA_ITMS.ataid = ataid)
			FOR JSON PATH
		) AS 'ata'

		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  

	End

	else if @ActionType = 'ALL'
	Begin

		Select	gstin,fp,gt,cur_gt,
		(	Select	ctin,
			(	Select inum, idt,val, pos,rchrg,etin,inv_typ,
				(	Select num, JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_B2B_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2B_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2B_INV_ITMS
					Where invid = TBL_GSTR1_B2B_INV.invid 
					And  TBL_GSTR1_B2B_INV.b2bid = TBL_GSTR1_B2B.b2bid
					And IsNull(TBL_GSTR1_B2B_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2B_INV
				Where b2bid = TBL_GSTR1_B2B.b2bid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2B 
			Where TBL_GSTR1_B2B.gstr1id in 
				(	Select TBL_GSTR1.gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2B.b2bid in
				(	Select b2bid
					From TBL_GSTR1_B2B_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2B.b2bid,TBL_GSTR1_B2B.ctin 
			FOR JSON PATH
		) As b2b,

		(	Select	pos,
			(	Select inum, idt,val,etin,
				(	Select num, JSON_QUERY
							((
								Select rt,txval,iamt,csamt 
								From TBL_GSTR1_B2CL_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2CL_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2CL_INV_ITMS
					Where invid = TBL_GSTR1_B2CL_INV.invid 
					And  TBL_GSTR1_B2CL_INV.b2clid = TBL_GSTR1_B2CL.b2clid
					And IsNull(TBL_GSTR1_B2CL_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2CL_INV
				Where b2clid = 	TBL_GSTR1_B2CL.b2clid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2CL  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2CL.b2clid in
				(	Select b2clid
					From TBL_GSTR1_B2CL_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2CL.b2clid,TBL_GSTR1_B2CL.pos 
			FOR JSON PATH
		) As b2cl,

		(	Select	sply_ty,txval,typ,etin,pos,rt,iamt,camt,samt,csamt
			From TBL_GSTR1_B2CS
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			FOR JSON PATH
		) As b2cs,

		(	Select	ex_tp as exp_typ,
			(	Select inum, idt,val,
				(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
					 Else Null
				End) as sbpcode ,
				(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
					 Else Null
				End) as sbnum ,
				(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
					 Else Null
				End) as sbdt ,
				(	Select txval,rt,iamt,csamt
					From TBL_GSTR1_EXP_INV_ITMS
					Where invid = TBL_GSTR1_EXP_INV.invid 
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_EXP_INV
				Where expid = TBL_GSTR1_EXP.expid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_EXP  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_EXP.expid in
				(	Select expid
					From TBL_GSTR1_EXP_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_EXP.expid,TBL_GSTR1_EXP.ex_tp 
			FOR JSON PATH
		) As 'exp',

		(	Select	ctin,
			(	Select ntty,nt_num,nt_dt,p_gst,inum, idt,val, --rsn,
				(	Select num,JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_CDNR_NT_ITMS_DET 
								Where itmsid = TBL_GSTR1_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det

					From TBL_GSTR1_CDNR_NT_ITMS
					Where ntid = TBL_GSTR1_CDNR_NT.ntid 
					And  TBL_GSTR1_CDNR_NT.cdnrid = TBL_GSTR1_CDNR.cdnrid
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_CDNR_NT
				Where cdnrid = 	TBL_GSTR1_CDNR.cdnrid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As nt
			From TBL_GSTR1_CDNR  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)
			And TBL_GSTR1_CDNR.cdnrid in
				(	Select cdnrid
					From TBL_GSTR1_CDNR_NT
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_CDNR.cdnrid,TBL_GSTR1_CDNR.ctin 
			FOR JSON PATH
		) As cdnr,

		(	Select typ,ntty,nt_num,nt_dt,inum, idt,val,p_gst,--rsn,
			(	Select num,JSON_QUERY
						((	
							Select rt,txval,iamt,csamt --camt,samt
							From TBL_GSTR1_CDNUR_ITMS_DET 
							Where itmsid = TBL_GSTR1_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det

				From TBL_GSTR1_CDNUR_ITMS
				Where cdnurid = TBL_GSTR1_CDNUR.cdnurid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR1_CDNUR 
								Where IsNull(flag,'') <> @Flag
								And TBL_GSTR1_CDNUR.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnur,
	
		(	Select JSON_QUERY
			((
				Select 
				(	
						Select 
						Row_Number()  OVER(Partition By hsnid Order By hsnid) as num,
						hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt
						From TBL_GSTR1_HSN_DATA 
						Where hsnid = TBL_GSTR1_HSN.hsnid
						And IsNull(flag,'') <> @Flag
						FOR JSON PATH
				) AS 'data'		 
				From TBL_GSTR1_HSN
				Where gstr1id in 
					(	Select gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				And hsnid in
					(
						Select distinct hsnid From TBL_GSTR1_HSN_DATA
						Where IsNull(flag,'') <> @Flag
						And gstr1id in 
							(	Select gstr1id 
								From TBL_GSTR1 
								Where TBL_GSTR1.gstin = @Gstin 
								And TBL_GSTR1.fp = @Fp
							)			
					) 
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		) AS hsn, 
	
		(	Select JSON_QUERY
			((  Select 
				(	Select nil_amt,expt_amt, ngsup_amt,sply_ty
					From TBL_GSTR1_NIL_INV 
					Where nilid = TBL_GSTR1_NIL.nilid
					FOR JSON PATH
				) AS inv
				From TBL_GSTR1_NIL 
				Where IsNull(flag,'') <> @Flag
				And gstr1id in 
					(	Select gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin
						And  TBL_GSTR1.fp = @Fp
					) 
				FOR JSON PATH,  WITHOUT_ARRAY_WRAPPER
			))
		) AS nil,
	
		(	Select pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_TXP_ITMS 
				Where txpid = TBL_GSTR1_TXP.txpid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_TXP 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXP_ITMS Where TBL_GSTR1_TXP_ITMS.txpid = txpid)
			FOR JSON PATH
		) AS txpd,
		
		(	Select pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_AT_ITMS 
				Where atid = TBL_GSTR1_AT.atid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_AT 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_AT_ITMS Where TBL_GSTR1_AT_ITMS.atid = atid)
			FOR JSON PATH
		) AS 'at',
	
		(	Select JSON_QUERY 
			((	

				Select JSON_QUERY
					 ((
						 Select doc_num,
							(	Select num,[from],[to],totnum,cancel,net_issue
								From TBL_GSTR1_DOCS
								Where docissueid = TBL_GSTR1_DOC_ISSUE. docissueid
								FOR JSON PATH
							) As docs
						From TBL_GSTR1_DOC_ISSUE
						Where gstr1id in 
							(	Select gstr1id 
								From TBL_GSTR1 
								Where TBL_GSTR1.gstin = @Gstin 
								And TBL_GSTR1.fp = @Fp 
							)
						And IsNull(flag,'') <> @Flag
						And Exists(Select 1 From TBL_GSTR1_DOCS Where TBL_GSTR1_DOCS.docissueid = docissueid)
						FOR JSON PATH	
					)) As doc_det

					From TBL_GSTR1_DOC_ISSUE
					Where gstr1id in 
						(	Select gstr1id 
							From TBL_GSTR1 
							Where TBL_GSTR1.gstin = @Gstin 
							And TBL_GSTR1.fp = @Fp 
						)
					And IsNull(flag,'') <> @Flag
					And Exists(Select 1 From TBL_GSTR1_DOCS Where TBL_GSTR1_DOCS.docissueid = docissueid)

				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		
		)   As doc_issue ,


		(	Select	ctin,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val, pos,rchrg,etin,inv_typ,
				(	Select num, JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_B2BA_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2BA_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2BA_INV_ITMS
					Where invid = TBL_GSTR1_B2BA_INV.invid 
					And  TBL_GSTR1_B2BA_INV.b2baid = TBL_GSTR1_B2BA.b2baid
					And IsNull(TBL_GSTR1_B2BA_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2BA_INV
				Where b2baid = TBL_GSTR1_B2BA.b2baid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2BA 
			Where TBL_GSTR1_B2BA.gstr1id in 
				(	Select TBL_GSTR1.gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2BA.b2baid in
				(	Select b2baid
					From TBL_GSTR1_B2BA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2BA.b2baid,TBL_GSTR1_B2BA.ctin 
			FOR JSON PATH
		) As b2ba,

		(	Select	pos,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val,etin,
				(	Select num, JSON_QUERY
							((
								Select rt,txval,iamt,csamt 
								From TBL_GSTR1_B2CLA_INV_ITMS_DET 
								Where itmsid = TBL_GSTR1_B2CLA_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det
					From TBL_GSTR1_B2CLA_INV_ITMS
					Where invid = TBL_GSTR1_B2CLA_INV.invid 
					And  TBL_GSTR1_B2CLA_INV.b2claid = TBL_GSTR1_B2CLA.b2claid
					And IsNull(TBL_GSTR1_B2CLA_INV.flag,'') <> @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_B2CLA_INV
				Where b2claid = 	TBL_GSTR1_B2CLA.b2claid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_B2CLA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_B2CLA.b2claid in
				(	Select b2claid
					From TBL_GSTR1_B2CLA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_B2CLA.b2claid,TBL_GSTR1_B2CLA.pos 
			FOR JSON PATH
		) As b2cla,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			pos,sply_ty,typ,etin,
			(	Select rt,txval, iamt, camt, samt, csamt
				From TBL_GSTR1_B2CSA_ITMS 
				Where b2csaid = TBL_GSTR1_B2CSA.b2csaid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_B2CSA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_B2CSA_ITMS Where TBL_GSTR1_B2CSA_ITMS.b2csaid = b2csaid)
			FOR JSON PATH
		) AS b2csa,

		(	Select	ex_tp as exp_typ,
			(	Select oinum,oidt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				inum, idt,val,
				(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
					 Else Null
				End) as sbpcode ,
				(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
					 Else Null
				End) as sbnum ,
				(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
					 Else Null
				End) as sbdt ,
				(	Select txval,rt,iamt,csamt
					From TBL_GSTR1_EXPA_INV_ITMS
					Where invid = TBL_GSTR1_EXPA_INV.invid 
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_EXPA_INV
				Where expaid = TBL_GSTR1_EXPA.expaid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR1_EXPA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
			And TBL_GSTR1_EXPA.expaid in
				(	Select expaid
					From TBL_GSTR1_EXPA_INV
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_EXPA.expaid,TBL_GSTR1_EXPA.ex_tp 
			FOR JSON PATH
		) As 'expa',

		(	Select	ctin,
			(	Select ont_num,ont_dt,
				(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
				ntty,nt_num,nt_dt,p_gst,inum, idt,val, --rsn,
				(	Select num,JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR1_CDNRA_NT_ITMS_DET 
								Where itmsid = TBL_GSTR1_CDNRA_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det

					From TBL_GSTR1_CDNRA_NT_ITMS
					Where ntid = TBL_GSTR1_CDNRA_NT.ntid 
					And  TBL_GSTR1_CDNRA_NT.cdnraid = TBL_GSTR1_CDNRA.cdnraid
					FOR JSON PATH
				) As itms
				From TBL_GSTR1_CDNRA_NT
				Where cdnraid = 	TBL_GSTR1_CDNRA.cdnraid
				And IsNull(flag,'') <> @Flag
				FOR JSON PATH	
			) As nt
			From TBL_GSTR1_CDNRA  
			Where gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)
			And TBL_GSTR1_CDNRA.cdnraid in
				(	Select cdnraid
					From TBL_GSTR1_CDNRA_NT
					Where IsNull(flag,'') <> @Flag
					And gstr1id In
					(
						Select TBL_GSTR1.gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin 
						And TBL_GSTR1.fp = @Fp
					)
				)
			Group by TBL_GSTR1_CDNRA.cdnraid,TBL_GSTR1_CDNRA.ctin 
			FOR JSON PATH
		) As cdnra,

		(	Select ont_num,ont_dt,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			typ,ntty,nt_num,nt_dt,inum, idt,val,p_gst,--rsn,
			(	Select num,JSON_QUERY
						((	
							Select rt,txval,iamt,csamt --camt,samt
							From TBL_GSTR1_CDNURA_ITMS_DET 
							Where itmsid = TBL_GSTR1_CDNURA_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det

				From TBL_GSTR1_CDNURA_ITMS
				Where cdnuraid = TBL_GSTR1_CDNURA.cdnuraid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR1_CDNURA
			Where cdnuraid in (	Select cdnuraid 
								From TBL_GSTR1_CDNURA 
								Where IsNull(flag,'') <> @Flag
								And TBL_GSTR1_CDNURA.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnura,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
			End) as diff_percent ,
			pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_TXPA_ITMS 
				Where txpaid = TBL_GSTR1_TXPA.txpaid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_TXPA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXPA_ITMS Where TBL_GSTR1_TXPA_ITMS.txpaid = txpaid)
			FOR JSON PATH
		) AS txpda,

		(	Select omon,
			(Case When IsNull(diff_percent,0) <> 0 Then diff_percent
					 Else Null
				End) as diff_percent ,
			pos,sply_ty,
			(	Select rt,ad_amt, iamt, camt, samt, csamt
				From TBL_GSTR1_ATA_ITMS 
				Where ataid = TBL_GSTR1_ATA.ataid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR1_ATA 
			Where IsNull(flag,'') <> @Flag
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_ATA_ITMS Where TBL_GSTR1_ATA_ITMS.ataid = ataid)
			FOR JSON PATH
		) AS 'ata'


		From TBL_GSTR1 
		Where gstin = @Gstin 
		And TBL_GSTR1.fp = @fp 
		Group by TBL_GSTR1.gstin, TBL_GSTR1.fp,TBL_GSTR1.gt,TBL_GSTR1.cur_gt 
		FOR JSON AUTO  
		
	End 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End