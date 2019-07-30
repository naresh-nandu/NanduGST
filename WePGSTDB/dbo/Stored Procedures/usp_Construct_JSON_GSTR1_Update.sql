
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 Modify / Delete
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/1/2017	Seshadri			Initial Version
9/3/2017	Seshadri	Fixed Truncation Issues
9/7/2017	Seshadri	Fixed Export Issue - sbpcode,sbnum,sbdt
9/7/2017	Seshadri	Fixed AT & TXP Issue 
10/9/2017	Seshadri	Fine Tuned the Code in line with usp_Construct_JSON_GSTR1_Save 
10/12/2017	Seshadri	Segregated JSON Construction based on Action Type
07/03/2018	Raja		Updated GSTR-1 Delete JSON Attributes based on Version 1.1 in (B2CS, CDNR, CDNUR)
03/10/2018	Seshadri	Made code changes related to Amendments

*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_Update '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Construct_JSON_GSTR1_Update] 
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
			(	Select inum, idt,flag -- ,val, pos,rchrg,etin,inv_typ,chksum,
				--(	Select num, JSON_QUERY
				--			((
				--				Select rt ,txval,iamt,camt,samt,csamt 
				--				From TBL_GSTR1_B2B_INV_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_B2B_INV_ITMS.itmsid
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det
				--	From TBL_GSTR1_B2B_INV_ITMS
				--	Where invid = TBL_GSTR1_B2B_INV.invid 
				--	And  TBL_GSTR1_B2B_INV.b2bid = TBL_GSTR1_B2B.b2bid
				--	And IsNull(TBL_GSTR1_B2B_INV.flag,'') = @Flag
				--	And IsNull(TBL_GSTR1_B2B_INV.chksum,'') <> ''
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_B2B_INV
				Where b2bid = TBL_GSTR1_B2B.b2bid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> '' Group by inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
	 				And IsNull(chksum,'') <> ''
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
			(	Select inum, idt,flag --,val,etin,flag,chksum,
				--(	Select num, JSON_QUERY
				--			((
				--				Select rt,txval,iamt,csamt 
				--				From TBL_GSTR1_B2CL_INV_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_B2CL_INV_ITMS.itmsid 
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det
				--	From TBL_GSTR1_B2CL_INV_ITMS
				--	Where invid = TBL_GSTR1_B2CL_INV.invid 
				--	And  TBL_GSTR1_B2CL_INV.b2clid = TBL_GSTR1_B2CL.b2clid
				--	And IsNull(TBL_GSTR1_B2CL_INV.flag,'') = @Flag
				--	And IsNull(TBL_GSTR1_B2CL_INV.chksum,'') <> ''
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_B2CL_INV
				Where b2clid = 	TBL_GSTR1_B2CL.b2clid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> '' Group By inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select	sply_ty,typ,pos,flag --rt,etin,txval,iamt,camt,samt,csamt,flag,chksum
			From TBL_GSTR1_B2CS
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)  
				Group By sply_ty,typ,pos,flag
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
			(	Select inum, idt,flag--,val,
				--(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
				--	 Else Null
				--End) as sbpcode ,
				--(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
				--	 Else Null
				--End) as sbnum ,
				--(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
				--	 Else Null
				--End) as sbdt ,
				--flag,chksum,
				--(	Select txval,rt,iamt
				--	From TBL_GSTR1_EXP_INV_ITMS
				--	Where invid = TBL_GSTR1_EXP_INV.invid 
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_EXP_INV
				Where expid = TBL_GSTR1_EXP.expid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By  inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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
			(	Select nt_num,nt_dt,inum,idt,flag --,ntty,p_gst,rsn,inum, idt,val,flag,chksum, 
				--(	Select num,JSON_QUERY
				--			((
				--				Select rt,txval,iamt,camt,samt,csamt 
				--				From TBL_GSTR1_CDNR_NT_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_CDNR_NT_ITMS.itmsid 
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det

				--	From TBL_GSTR1_CDNR_NT_ITMS
				--	Where ntid = TBL_GSTR1_CDNR_NT.ntid 
				--	And  TBL_GSTR1_CDNR_NT.cdnrid = TBL_GSTR1_CDNR.cdnrid
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_CDNR_NT
				Where cdnrid = 	TBL_GSTR1_CDNR.cdnrid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By nt_num,nt_dt,inum,idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select typ,nt_num,nt_dt,inum,idt,flag --,ntty,inum, idt,val, 
			--(	Select num,JSON_QUERY
			--			((	
			--				Select rt,txval,iamt,camt,samt,csamt 
			--				From TBL_GSTR1_CDNUR_ITMS_DET 
			--				Where itmsid = TBL_GSTR1_CDNUR_ITMS.itmsid 
			--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			--			)) As itm_det

			--	From TBL_GSTR1_CDNUR_ITMS
			--	Where cdnurid = TBL_GSTR1_CDNUR.cdnurid 
			--	FOR JSON PATH
			--) As itms
			From TBL_GSTR1_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR1_CDNUR 
								Where IsNull(flag,'') = @Flag
								And IsNull(chksum,'') <> ''
								And TBL_GSTR1_CDNUR.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)
				Group By typ,nt_num,nt_dt,inum,idt,flag	 
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
						flag,hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt
						From TBL_GSTR1_HSN_DATA 
						Where hsnid = TBL_GSTR1_HSN.hsnid
						And IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> ''
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
						Where IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> ''
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
				(	Select nil_amt,expt_amt, ngsup_amt,sply_ty,flag
					From TBL_GSTR1_NIL_INV 
					Where nilid = TBL_GSTR1_NIL.nilid
					FOR JSON PATH
				) AS inv
				From TBL_GSTR1_NIL 
				Where IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
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

		(	Select pos,sply_ty,flag
			--(	Select rt,ad_amt, iamt, camt, samt, csamt
			--	From TBL_GSTR1_TXP_ITMS 
			--	Where txpid = TBL_GSTR1_TXP.txpid
			--	FOR JSON PATH
			--) AS itms
			From TBL_GSTR1_TXP 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXP_ITMS Where TBL_GSTR1_TXP_ITMS.txpid = txpid)
				Group By pos,sply_ty,flag
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

		(	Select pos,sply_ty,flag
			--(	Select rt,ad_amt, iamt, camt, samt, csamt
			--	From TBL_GSTR1_AT_ITMS 
			--	Where atid = TBL_GSTR1_AT.atid
			--	FOR JSON PATH
			--) AS itms
			From TBL_GSTR1_AT 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_AT_ITMS Where TBL_GSTR1_AT_ITMS.atid = atid)
				Group By pos,sply_ty,flag
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
						 Select doc_num,flag,
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
						And IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> '' 
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
					And IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> ''
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
			(	Select inum, idt,flag 
				From TBL_GSTR1_B2BA_INV
				Where b2baid = TBL_GSTR1_B2BA.b2baid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> '' 
				Group by inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
	 				And IsNull(chksum,'') <> ''
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
			(	Select inum, idt,flag
				From TBL_GSTR1_B2CLA_INV
				Where b2claid = TBL_GSTR1_B2CLA.b2claid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> '' 
				Group By inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select	sply_ty,typ,pos,flag 
			From TBL_GSTR1_B2CSA
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				)  
				Group By sply_ty,typ,pos,flag
			FOR JSON PATH
		) As b2csa

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
			(	Select inum, idt,flag
				From TBL_GSTR1_EXPA_INV
				Where expaid = TBL_GSTR1_EXPA.expaid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By  inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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
			(	Select nt_num,nt_dt,inum,idt,flag 
				From TBL_GSTR1_CDNRA_NT
				Where cdnraid = TBL_GSTR1_CDNRA.cdnraid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By nt_num,nt_dt,inum,idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select typ,nt_num,nt_dt,inum,idt,flag
			From TBL_GSTR1_CDNURA
			Where cdnuraid in (	Select cdnuraid 
								From TBL_GSTR1_CDNURA 
								Where IsNull(flag,'') = @Flag
								And IsNull(chksum,'') <> ''
								And TBL_GSTR1_CDNURA.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)
				Group By typ,nt_num,nt_dt,inum,idt,flag	 
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

		(	Select pos,sply_ty,flag
			From TBL_GSTR1_TXPa 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXPA_ITMS Where TBL_GSTR1_TXPA_ITMS.txpaid = txpaid)
				Group By pos,sply_ty,flag
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

		(	Select pos,sply_ty,flag
			From TBL_GSTR1_ATA 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_ATA_ITMS Where TBL_GSTR1_ATA_ITMS.ataid = ataid)
				Group By pos,sply_ty,flag
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
			(	Select inum, idt,flag--,val, pos,rchrg,etin,inv_typ,flag,chksum,
				--(	Select num, JSON_QUERY
				--			((
				--				Select rt ,txval,iamt,camt,samt,csamt 
				--				From TBL_GSTR1_B2B_INV_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_B2B_INV_ITMS.itmsid
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det
				--	From TBL_GSTR1_B2B_INV_ITMS
				--	Where invid = TBL_GSTR1_B2B_INV.invid 
				--	And  TBL_GSTR1_B2B_INV.b2bid = TBL_GSTR1_B2B.b2bid
				--	And IsNull(TBL_GSTR1_B2B_INV.flag,'') = @Flag
				--	And IsNull(TBL_GSTR1_B2B_INV.chksum,'') <> ''
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_B2B_INV
				Where b2bid = TBL_GSTR1_B2B.b2bid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
	 				And IsNull(chksum,'') <> ''
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
			(	Select inum, idt,flag --,val,etin,flag,chksum,
				--(	Select num, JSON_QUERY
				--			((
				--				Select rt,txval,iamt,csamt 
				--				From TBL_GSTR1_B2CL_INV_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_B2CL_INV_ITMS.itmsid 
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det
				--	From TBL_GSTR1_B2CL_INV_ITMS
				--	Where invid = TBL_GSTR1_B2CL_INV.invid 
				--	And  TBL_GSTR1_B2CL_INV.b2clid = TBL_GSTR1_B2CL.b2clid
				--	And IsNull(TBL_GSTR1_B2CL_INV.flag,'') = @Flag
				--	And IsNull(TBL_GSTR1_B2CL_INV.chksum,'') <> ''
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_B2CL_INV
				Where b2clid = 	TBL_GSTR1_B2CL.b2clid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select	sply_ty,typ,pos,flag --rt,txval,etin,,iamt,camt,samt,csamt,flag,chksum
			From TBL_GSTR1_B2CS
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp
				) 
				Group By sply_ty,typ,pos,flag 
			FOR JSON PATH
		) As b2cs,

		(	Select	ex_tp as exp_typ,
			(	Select inum, idt,flag --,val,
				--(Case When Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' Then sbpcode
				--	 Else Null
				--End) as sbpcode ,
				--(Case When Ltrim(Rtrim(IsNull(sbnum,0))) <> 0 Then sbnum
				--	 Else Null
				--End) as sbnum ,
				--(Case When Ltrim(Rtrim(IsNull(sbdt,''))) <> '' Then sbdt
				--	 Else Null
				--End) as sbdt ,
				--flag,chksum,
				--(	Select txval,rt,iamt
				--	From TBL_GSTR1_EXP_INV_ITMS
				--	Where invid = TBL_GSTR1_EXP_INV.invid 
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_EXP_INV
				Where expid = TBL_GSTR1_EXP.expid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By inum, idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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
			(	Select nt_num,nt_dt,inum,idt,flag -- ,ntty,p_gst,rsn,inum, idt,val,flag,chksum, 
				--(	Select num,JSON_QUERY
				--			((
				--				Select rt,txval,iamt,camt,samt,csamt 
				--				From TBL_GSTR1_CDNR_NT_ITMS_DET 
				--				Where itmsid = TBL_GSTR1_CDNR_NT_ITMS.itmsid 
				--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				--			)) As itm_det

				--	From TBL_GSTR1_CDNR_NT_ITMS
				--	Where ntid = TBL_GSTR1_CDNR_NT.ntid 
				--	And  TBL_GSTR1_CDNR_NT.cdnrid = TBL_GSTR1_CDNR.cdnrid
				--	FOR JSON PATH
				--) As itms
				From TBL_GSTR1_CDNR_NT
				Where cdnrid = 	TBL_GSTR1_CDNR.cdnrid
				And IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
					Group By nt_num,nt_dt,inum,idt,flag
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
					Where IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> '' 
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

		(	Select typ,nt_num,nt_dt,inum,idt,flag --,ntty,inum, idt,val, 
			--(	Select num,JSON_QUERY
			--			((	
			--				Select rt,txval,iamt,camt,samt,csamt 
			--				From TBL_GSTR1_CDNUR_ITMS_DET 
			--				Where itmsid = TBL_GSTR1_CDNUR_ITMS.itmsid 
			--				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			--			)) As itm_det

			--	From TBL_GSTR1_CDNUR_ITMS
			--	Where cdnurid = TBL_GSTR1_CDNUR.cdnurid 
			--	FOR JSON PATH
			--) As itms
			From TBL_GSTR1_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR1_CDNUR 
								Where IsNull(flag,'') = @Flag
								And IsNull(chksum,'') <> ''
								And TBL_GSTR1_CDNUR.gstr1id in 
								(	Select gstr1id from TBL_GSTR1 
									Where TBL_GSTR1.gstin = @Gstin and TBL_GSTR1.fp = @Fp
								)
							)	
				Group By typ,nt_num,nt_dt,inum,idt,flag 
			FOR JSON PATH	
		) As cdnur,
	
		(	Select JSON_QUERY
			((
				Select 
				(	
						Select 
						Row_Number()  OVER(Partition By hsnid Order By hsnid) as num,
						hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt,flag
						From TBL_GSTR1_HSN_DATA 
						Where hsnid = TBL_GSTR1_HSN.hsnid
						And IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> ''
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
						Where IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> ''
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
			((  Select flag,
				(	Select nil_amt,expt_amt, ngsup_amt,sply_ty
					From TBL_GSTR1_NIL_INV 
					Where nilid = TBL_GSTR1_NIL.nilid
					FOR JSON PATH
				) AS inv
				From TBL_GSTR1_NIL 
				Where IsNull(flag,'') = @Flag
				And IsNull(chksum,'') <> ''
				And gstr1id in 
					(	Select gstr1id 
						From TBL_GSTR1 
						Where TBL_GSTR1.gstin = @Gstin
						And  TBL_GSTR1.fp = @Fp
					) 
				FOR JSON PATH,  WITHOUT_ARRAY_WRAPPER
			))
		) AS nil,
		
		(	Select pos,sply_ty,flag
			--(	Select rt,ad_amt, iamt, camt, samt, csamt
			--	From TBL_GSTR1_TXP_ITMS 
			--	Where txpid = TBL_GSTR1_TXP.txpid
			--	FOR JSON PATH
			--) AS itms
			From TBL_GSTR1_TXP 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_TXP_ITMS Where TBL_GSTR1_TXP_ITMS.txpid = txpid)
				Group By pos,sply_ty,flag
			FOR JSON PATH
		) AS txpd,
		
		(	Select pos,sply_ty,flag
			--(	Select rt,ad_amt, iamt, camt, samt, csamt
			--	From TBL_GSTR1_AT_ITMS 
			--	Where atid = TBL_GSTR1_AT.atid
			--	FOR JSON PATH
			--) AS itms
			From TBL_GSTR1_AT 
			Where IsNull(flag,'') = @Flag
			And IsNull(chksum,'') <> ''
			And gstr1id in 
				(	Select gstr1id 
					From TBL_GSTR1 
					Where TBL_GSTR1.gstin = @Gstin 
					And TBL_GSTR1.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR1_AT_ITMS Where TBL_GSTR1_AT_ITMS.atid = atid)
				Group By pos,sply_ty,flag
			FOR JSON PATH
		) AS 'at',
	
		(	Select JSON_QUERY 
			((	
			
				Select JSON_QUERY
					 ((
						 Select doc_num,flag,
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
						And IsNull(flag,'') = @Flag
						And IsNull(chksum,'') <> '' 
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
					And IsNull(flag,'') = @Flag
					And IsNull(chksum,'') <> ''
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

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End