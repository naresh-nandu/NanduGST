
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR2 Save
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri			Initial Version
10/14/2017	Seshadri	Segregated JSON Construction based on Action Type
10/25/2017	Seshadri	Modified to include the Flag Values A/R/P/M in case of B2B and CDNR
10/28/2017	Seshadri	Modified Flag level changes in case of B2B
11/02/2017	Seshadri	Modified Chksum changes in case of B2B
11/06/2017	Seshadri	Modified Chksum changes for B2B & Cdn in case of ALL
11/06/2017	Seshadri	Modified ITC details to handle Null Values
11/06/2017	Seshadri	Removed the commented section of the code
11/06/2017	Seshadri	Modified IMPG Section wrt stin
11/06/2017	Seshadri	Fixed Payload Issue in IMPG,IMPS,TXPD,TXI Section
11/13/2017	Seshadri	Fixed Payload Issue (Space Issue after Invoice No) in B2B
11/16/2017	Seshadri	Supported the flag CDN which is passed from front end
28/03/2018	Raja		For Mobile App 'Flag' changes done
*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR2_MOBILE_APP '18BNAPG9891G1ZM','072017',''		

 */

CREATE PROCEDURE [dbo].[usp_Construct_JSON_GSTR2_MOBILE_APP] 
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

	if (IsNull(@Flag,'') = '')
	Begin
		Select @Flag = ''
	End

	if @ActionType = 'B2B'
	Begin

		Select	gstin,fp,

		(	Select	ctin,
			(	Select Ltrim(Rtrim(IsNull(inum,''))) as inum, 
						idt,
						(Case When IsNull(flag,'') In ('','M') Then val
							Else Null
						End) as val ,
						(Case When IsNull(flag,'') In ('','M') Then pos
							Else Null
						End) as pos ,
						(Case When IsNull(flag,'') In ('','M') Then rchrg
							Else Null
						End) as rchrg ,
						(Case When IsNull(flag,'') In ('','M') Then inv_typ
							Else Null
						End) as inv_typ ,
						(Case When IsNull(flag,'') In ('') Then Null
							Else flag
						End) as flag ,
						(Case When IsNull(chksum,'') In ('') Then Null
							Else chksum
						End) as chksum,
				(	Select num, 

							JSON_QUERY
								((
									Select rt ,txval,iamt,camt,samt,csamt 
									From TBL_GSTR2_B2B_INV_ITMS_DET,
											TBL_GSTR2_B2B_INV 
									Where itmsid = TBL_GSTR2_B2B_INV_ITMS.itmsid
									And TBL_GSTR2_B2B_INV.invid = TBL_GSTR2_B2B_INV_ITMS.invid
									And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
										 (IsNull(TBL_GSTR2_B2B_INV.flag,'') = 'M' And
										  IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
										)
									FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
								)) as itm_det,
	
							JSON_QUERY
							((
								Select elg,
									IsNull(tx_i,0.00) as tx_i,
									IsNull(tx_c,0.00) as tx_c,
									IsNull(tx_s,0.00) as tx_s,
									IsNull(tx_cs,0.00) as tx_cs
								From TBL_GSTR2_B2B_INV_ITMS_ITC,
										TBL_GSTR2_B2B_INV  
								Where itmsid = TBL_GSTR2_B2B_INV_ITMS.itmsid
								And TBL_GSTR2_B2B_INV.invid = TBL_GSTR2_B2B_INV_ITMS.invid
								And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
										 (IsNull(TBL_GSTR2_B2B_INV.flag,'') In('M','A') And
										  IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
									)
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) as itc
								
					From TBL_GSTR2_B2B_INV_ITMS
					Where invid = TBL_GSTR2_B2B_INV.invid 
					And  TBL_GSTR2_B2B_INV.b2bid = TBL_GSTR2_B2B.b2bid
					And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
									(IsNull(TBL_GSTR2_B2B_INV.flag,'') In('M','A') And
									IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
						)
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_B2B_INV
				Where b2bid = TBL_GSTR2_B2B.b2bid
				And ((IsNull(flag,'') = '') Or
									(IsNull(flag,'') In('A','R','P','M') And
									IsNull(chksum,'') <> '' )
					)
				FOR JSON PATH	
			) As inv
			From TBL_GSTR2_B2B 
			Where TBL_GSTR2_B2B.gstr2id in 
				(	Select TBL_GSTR2.gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				) 
			And TBL_GSTR2_B2B.b2bid in
				(	Select b2bid
					From TBL_GSTR2_B2B_INV
					Where ((IsNull(flag,'') = '') Or
									(IsNull(flag,'') In('A','R','P','M') And
									IsNull(chksum,'') <> '' )
							)
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_B2B.b2bid,TBL_GSTR2_B2B.ctin 
			FOR JSON PATH
		) As b2b

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'B2BUR'
	Begin

		Select	gstin,fp,

		(	Select	
			(	Select inum,idt,val,pos,sply_ty,
				(	Select num, 
							JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt  
								From TBL_GSTR2_B2BUR_INV_ITMS_DET 
								Where itmsid = TBL_GSTR2_B2BUR_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det,

							JSON_QUERY
							((
								Select elg,tx_i,tx_c,tx_s,tx_cs 
								From TBL_GSTR2_B2BUR_INV_ITMS_ITC 
								Where itmsid = TBL_GSTR2_B2BUR_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itc


					From TBL_GSTR2_B2BUR_INV_ITMS
					Where invid = TBL_GSTR2_B2BUR_INV.invid 
					And  TBL_GSTR2_B2BUR_INV.b2burid = TBL_GSTR2_B2BUR.b2burid
					And IsNull(TBL_GSTR2_B2BUR_INV.flag,'') = @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_B2BUR_INV
				Where b2burid = TBL_GSTR2_B2BUR.b2burid
				And IsNull(flag,'') = @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR2_B2BUR  
			Where gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				) 
			And TBL_GSTR2_B2BUR.b2burid in
				(	Select b2burid
					From TBL_GSTR2_B2BUR_INV
					Where IsNull(flag,'') = @Flag 
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_B2BUR.b2burid 
			FOR JSON PATH
		) As b2bur

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'IMPG'
	Begin

		Select	gstin,fp,

		(	Select is_sez,
					(Case When Ltrim(Rtrim(IsNull(stin,''))) <> '' Then stin
								 Else Null
							End) as stin ,
				   boe_num,boe_dt,boe_val,port_code,
			(	Select num,rt,txval,iamt,csamt,elg,tx_i,tx_cs
				From TBL_GSTR2_IMPG_ITMS 
				Where impgid = TBL_GSTR2_IMPG.impgid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_IMPG 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_IMPG_ITMS Where TBL_GSTR2_IMPG_ITMS.impgid = TBL_GSTR2_IMPG.impgid)
			FOR JSON PATH
		) AS imp_g

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'IMPS'
	Begin

		Select	gstin,fp,

		(	Select inum,idt,ival,pos,
			(	Select num,rt,txval,iamt,csamt,elg,tx_i,tx_cs
				From TBL_GSTR2_IMPS_ITMS 
				Where impsid = TBL_GSTR2_IMPS.impsid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_IMPS 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_IMPS_ITMS Where TBL_GSTR2_IMPS_ITMS.impsid = TBL_GSTR2_IMPS.impsid)
			FOR JSON PATH
		) AS imp_s

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType In ('CDNR','CDN')
	Begin

		Select	gstin,fp,

		(	Select	ctin,
			(	Select ntty,nt_num,nt_dt,p_gst,rsn,inum, idt,val,flag,chksum, 
				(	Select num,
							JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR2_CDNR_NT_ITMS_DET 
								Where itmsid = TBL_GSTR2_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det,

							JSON_QUERY
							((
								Select elg,tx_i,tx_c,tx_s,tx_cs 
								From TBL_GSTR2_CDNR_NT_ITMS_ITC 
								Where itmsid = TBL_GSTR2_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itc


					From TBL_GSTR2_CDNR_NT_ITMS
					Where invid = TBL_GSTR2_CDNR_NT.invid 
					And  TBL_GSTR2_CDNR_NT.cdnrid = TBL_GSTR2_CDNR.cdnrid
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_CDNR_NT
				Where cdnrid = 	TBL_GSTR2_CDNR.cdnrid
				And (IsNull(flag,'') = @Flag 
						Or
					 (IsNull(flag,'') In ('A','R','P','M')  And
						IsNull(chksum,'') <> '')
					)
				FOR JSON PATH	
			) As nt
			From TBL_GSTR2_CDNR  
			Where gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				)
			And TBL_GSTR2_CDNR.cdnrid in
				(	Select cdnrid
					From TBL_GSTR2_CDNR_NT
					Where (IsNull(flag,'') = @Flag 
								Or
						   (IsNull(flag,'') In ('A','R','P','M')  And
							IsNull(chksum,'') <> '')
						  )
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_CDNR.cdnrid,TBL_GSTR2_CDNR.ctin 
			FOR JSON PATH
		) As cdn

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'CDNUR'
	Begin

		Select	gstin,fp,

		(	Select rtin,ntty,nt_num,nt_dt,p_gst,rsn,inum, idt,val,inv_typ, 
			(	Select num,
						JSON_QUERY
						((	
							Select rt,txval,iamt,camt,samt,csamt 
							From TBL_GSTR2_CDNUR_ITMS_DET 
							Where itmsid = TBL_GSTR2_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det,

						JSON_QUERY
						((	
							Select elg,tx_i,tx_c,tx_s,tx_cs 
							From TBL_GSTR2_CDNUR_ITMS_ITC 
							Where itmsid = TBL_GSTR2_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itc

				From TBL_GSTR2_CDNUR_ITMS
				Where cdnurid = TBL_GSTR2_CDNUR.cdnurid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR2_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR2_CDNUR 
								Where IsNull(flag,'') = @Flag
								And TBL_GSTR2_CDNUR.gstr2id in 
								(	Select gstr2id from TBL_GSTR2 
									Where TBL_GSTR2.gstin = @Gstin and TBL_GSTR2.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnur

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType In ('HSN','HSNSUM')
	Begin

		Select	gstin,fp,

		(	Select JSON_QUERY
			((
				Select 
				(	
						Select 
						num,hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt
						From TBL_GSTR2_HSNSUM_DET 
						Where hsnsumid = TBL_GSTR2_HSNSUM.hsnsumid
						And IsNull(flag,'') = @Flag
						FOR JSON PATH
				) AS 'det'		 
				From TBL_GSTR2_HSNSUM
				Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				And hsnsumid in
					(
						Select distinct hsnsumid From TBL_GSTR2_HSNSUM_DET
						Where IsNull(flag,'') = @Flag
						And gstr2id in 
							(	Select gstr2id 
								From TBL_GSTR2 
								Where TBL_GSTR2.gstin = @Gstin 
								And TBL_GSTR2.fp = @Fp
							)			
					) 
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		) AS hsnsum

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'NIL'
	Begin

		Select	gstin,fp,

		( Select JSON_QUERY
				((	Select
						(	Select	JSON_QUERY	
							((	
								(	Select cpddr,exptdsply,ngsply,nilsply
									From TBL_GSTR2_NIL_INTER  t1,
									TBL_GSTR2_NIL t2,
									TBL_GSTR2 t3
									Where t1.nilid = t2.nilid
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	 
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER	
								) 
							)) 
						) As inter,

						(	Select	JSON_QUERY	
							((	
								(	Select cpddr,exptdsply,ngsply,nilsply
									From TBL_GSTR2_NIL_INTRA  t1,
									TBL_GSTR2_NIL t2,
									TBL_GSTR2 t3
									Where t1.nilid = t2.nilid
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As intra

					From TBL_GSTR2_NIL 
					Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					) 
					And IsNull(flag,'') = @Flag
					Group by TBL_GSTR2_NIL.gstr2id,TBL_GSTR2_NIL.nilid  
					FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				))
		) As nil_supplies

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'TXP'
	Begin

		Select	gstin,fp,

		(	Select pos,sply_ty,
			(	Select num,rt,adamt, iamt, camt, samt, csamt
				From TBL_GSTR2_TXPD_ITMS 
				Where txpdid = TBL_GSTR2_TXPD.txpdid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_TXPD 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_TXPD_ITMS Where TBL_GSTR2_TXPD_ITMS.txpdid = TBL_GSTR2_TXPD.txpdid)
			FOR JSON PATH
		) AS txpd

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'TXI'
	Begin

		Select	gstin,fp,

		(	Select pos,sply_ty,
			(	Select num,rt,adamt, iamt, camt, samt, csamt
				From TBL_GSTR2_TXI_ITMS 
				Where txiid = TBL_GSTR2_TXI.txiid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_TXI
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_TXI_ITMS Where TBL_GSTR2_TXI_ITMS.txiid = TBL_GSTR2_TXI.txiid)
			FOR JSON PATH
		) AS txi

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'ITCRVSL'
	Begin

		Select	gstin,fp,

		( Select JSON_QUERY
				((	Select
						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule2_2'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	 
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER	
								) 
							)) 
						) As rule2_2,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_1_m'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_1_m,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule8_1_h'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule8_1_h,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_2_a'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_2_a,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_2_b'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_2_b,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'revitc'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As revitc,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'other'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As other


					From TBL_GSTR2_ITCRVSL 
					Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					) 
					And IsNull(flag,'') = @Flag
					Group by TBL_GSTR2_ITCRVSL.gstr2id,TBL_GSTR2_ITCRVSL.itcrvslid  
					FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				))
		) As itc_rvsl 
 


		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End
	else if @ActionType = 'ALL'
	Begin

		Select	gstin,fp,

		(	Select	ctin,
			(	Select Ltrim(Rtrim(IsNull(inum,''))) as inum,  
						idt,
						(Case When IsNull(flag,'') In ('','M') Then val
							Else Null
						End) as val ,
						(Case When IsNull(flag,'') In ('','M') Then pos
							Else Null
						End) as pos ,
						(Case When IsNull(flag,'') In ('','M') Then rchrg
							Else Null
						End) as rchrg ,
						(Case When IsNull(flag,'') In ('','M') Then inv_typ
							Else Null
						End) as inv_typ ,
						(Case When IsNull(flag,'') In ('') Then Null
							Else flag
						End) as flag ,
						(Case When IsNull(chksum,'') In ('') Then Null
							Else chksum
						End) as chksum,
				(	Select num, 

							JSON_QUERY
								((
									Select rt ,txval,iamt,camt,samt,csamt 
									From TBL_GSTR2_B2B_INV_ITMS_DET,
											TBL_GSTR2_B2B_INV 
									Where itmsid = TBL_GSTR2_B2B_INV_ITMS.itmsid
									And TBL_GSTR2_B2B_INV.invid = TBL_GSTR2_B2B_INV_ITMS.invid
									And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
										 (IsNull(TBL_GSTR2_B2B_INV.flag,'') = 'M' And
										  IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
										)
									FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
								)) as itm_det,
	
							JSON_QUERY
							((
								Select elg,
									IsNull(tx_i,0.00) as tx_i,
									IsNull(tx_c,0.00) as tx_c,
									IsNull(tx_s,0.00) as tx_s,
									IsNull(tx_cs,0.00) as tx_cs
								From TBL_GSTR2_B2B_INV_ITMS_ITC,
										TBL_GSTR2_B2B_INV  
								Where itmsid = TBL_GSTR2_B2B_INV_ITMS.itmsid
								And TBL_GSTR2_B2B_INV.invid = TBL_GSTR2_B2B_INV_ITMS.invid
								And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
										 (IsNull(TBL_GSTR2_B2B_INV.flag,'') In('M','A') And
										  IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
									)
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) as itc
								
					From TBL_GSTR2_B2B_INV_ITMS
					Where invid = TBL_GSTR2_B2B_INV.invid 
					And  TBL_GSTR2_B2B_INV.b2bid = TBL_GSTR2_B2B.b2bid
					And ((IsNull(TBL_GSTR2_B2B_INV.flag,'') = '') Or
									(IsNull(TBL_GSTR2_B2B_INV.flag,'') In('M','A') And
									IsNull(TBL_GSTR2_B2B_INV.chksum,'') <> '' )
						)
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_B2B_INV
				Where b2bid = TBL_GSTR2_B2B.b2bid
				And ((IsNull(flag,'') = '') Or
									(IsNull(flag,'') In('A','R','P','M') And
									IsNull(chksum,'') <> '' )
					)
				FOR JSON PATH	
			) As inv
			From TBL_GSTR2_B2B 
			Where TBL_GSTR2_B2B.gstr2id in 
				(	Select TBL_GSTR2.gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				) 
			And TBL_GSTR2_B2B.b2bid in
				(	Select b2bid
					From TBL_GSTR2_B2B_INV
					Where ((IsNull(flag,'') = '') Or
									(IsNull(flag,'') In('A','R','P','M') And
									IsNull(chksum,'') <> '' )
							)
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_B2B.b2bid,TBL_GSTR2_B2B.ctin 
			FOR JSON PATH
		) As b2b,


		(	Select	
			(	Select inum, idt,val,pos,sply_ty,
				(	Select num, 
							JSON_QUERY
							((
								Select rt ,txval,iamt,camt,samt,csamt  
								From TBL_GSTR2_B2BUR_INV_ITMS_DET 
								Where itmsid = TBL_GSTR2_B2BUR_INV_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det,

							JSON_QUERY
							((
								Select elg,tx_i,tx_c,tx_s,tx_cs 
								From TBL_GSTR2_B2BUR_INV_ITMS_ITC 
								Where itmsid = TBL_GSTR2_B2BUR_INV_ITMS.itmsid
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itc


					From TBL_GSTR2_B2BUR_INV_ITMS
					Where invid = TBL_GSTR2_B2BUR_INV.invid 
					And  TBL_GSTR2_B2BUR_INV.b2burid = TBL_GSTR2_B2BUR.b2burid
					And IsNull(TBL_GSTR2_B2BUR_INV.flag,'') = @Flag
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_B2BUR_INV
				Where b2burid = TBL_GSTR2_B2BUR.b2burid
				And IsNull(flag,'') = @Flag
				FOR JSON PATH	
			) As inv
			From TBL_GSTR2_B2BUR  
			Where gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				) 
			And TBL_GSTR2_B2BUR.b2burid in
				(	Select b2burid
					From TBL_GSTR2_B2BUR_INV
					Where IsNull(flag,'') = @Flag 
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_B2BUR.b2burid 
			FOR JSON PATH
		) As b2bur,

		(	Select is_sez,
					(Case When Ltrim(Rtrim(IsNull(stin,''))) <> '' Then stin
								 Else Null
							End) as stin ,
					boe_num,boe_dt,boe_val,port_code,
			(	Select num,rt,txval,iamt,csamt,elg,tx_i,tx_cs
				From TBL_GSTR2_IMPG_ITMS 
				Where impgid = TBL_GSTR2_IMPG.impgid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_IMPG 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_IMPG_ITMS Where TBL_GSTR2_IMPG_ITMS.impgid = TBL_GSTR2_IMPG.impgid)
			FOR JSON PATH
		) AS imp_g,

		(	Select inum,idt,ival,pos,
			(	Select num,rt,txval,iamt,csamt,elg,tx_i,tx_cs
				From TBL_GSTR2_IMPS_ITMS 
				Where impsid = TBL_GSTR2_IMPS.impsid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_IMPS 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_IMPS_ITMS Where TBL_GSTR2_IMPS_ITMS.impsid = TBL_GSTR2_IMPS.impsid)
			FOR JSON PATH
		) AS imp_s,

				(	Select	ctin,
			(	Select ntty,nt_num,nt_dt,p_gst,rsn,inum, idt,val,flag,chksum, 
				(	Select num,
							JSON_QUERY
							((
								Select rt,txval,iamt,camt,samt,csamt 
								From TBL_GSTR2_CDNR_NT_ITMS_DET 
								Where itmsid = TBL_GSTR2_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itm_det,

							JSON_QUERY
							((
								Select elg,tx_i,tx_c,tx_s,tx_cs 
								From TBL_GSTR2_CDNR_NT_ITMS_ITC 
								Where itmsid = TBL_GSTR2_CDNR_NT_ITMS.itmsid 
								FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
							)) As itc


					From TBL_GSTR2_CDNR_NT_ITMS
					Where invid = TBL_GSTR2_CDNR_NT.invid 
					And  TBL_GSTR2_CDNR_NT.cdnrid = TBL_GSTR2_CDNR.cdnrid
					FOR JSON PATH
				) As itms
				From TBL_GSTR2_CDNR_NT
				Where cdnrid = 	TBL_GSTR2_CDNR.cdnrid
				And (IsNull(flag,'') = @Flag 
						Or
					 (IsNull(flag,'') In ('A','R','P','M')  And
						IsNull(chksum,'') <> '')
					)
				FOR JSON PATH	
			) As nt
			From TBL_GSTR2_CDNR  
			Where gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp
				)
			And TBL_GSTR2_CDNR.cdnrid in
				(	Select cdnrid
					From TBL_GSTR2_CDNR_NT
					Where (IsNull(flag,'') = @Flag 
								Or
						   (IsNull(flag,'') In ('A','R','P','M')  And
							IsNull(chksum,'') <> '')
						  )
					And gstr2id In
					(
						Select TBL_GSTR2.gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				)
			Group by TBL_GSTR2_CDNR.cdnrid,TBL_GSTR2_CDNR.ctin 
			FOR JSON PATH
		) As cdn,

		(	Select rtin,ntty,nt_num,nt_dt,p_gst,rsn,inum, idt,val,inv_typ, 
			(	Select num,
						JSON_QUERY
						((	
							Select rt,txval,iamt,camt,samt,csamt 
							From TBL_GSTR2_CDNUR_ITMS_DET 
							Where itmsid = TBL_GSTR2_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itm_det,

						JSON_QUERY
						((	
							Select elg,tx_i,tx_c,tx_s,tx_cs 
							From TBL_GSTR2_CDNUR_ITMS_ITC 
							Where itmsid = TBL_GSTR2_CDNUR_ITMS.itmsid 
							FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
						)) As itc

				From TBL_GSTR2_CDNUR_ITMS
				Where cdnurid = TBL_GSTR2_CDNUR.cdnurid 
				FOR JSON PATH
			) As itms
			From TBL_GSTR2_CDNUR
			Where cdnurid in (	Select cdnurid 
								From TBL_GSTR2_CDNUR 
								Where IsNull(flag,'') = @Flag
								And TBL_GSTR2_CDNUR.gstr2id in 
								(	Select gstr2id from TBL_GSTR2 
									Where TBL_GSTR2.gstin = @Gstin and TBL_GSTR2.fp = @Fp
								)
							)	 
			FOR JSON PATH	
		) As cdnur,

		(	Select JSON_QUERY
			((
				Select 
				(	
						Select 
						num,hsn_sc,[desc],uqc,qty,
						val,txval,iamt,camt,samt,csamt
						From TBL_GSTR2_HSNSUM_DET 
						Where hsnsumid = TBL_GSTR2_HSNSUM.hsnsumid
						And IsNull(flag,'') = @Flag
						FOR JSON PATH
				) AS 'det'		 
				From TBL_GSTR2_HSNSUM
				Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2 
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					)
				And hsnsumid in
					(
						Select distinct hsnsumid From TBL_GSTR2_HSNSUM_DET
						Where IsNull(flag,'') = @Flag
						And gstr2id in 
							(	Select gstr2id 
								From TBL_GSTR2 
								Where TBL_GSTR2.gstin = @Gstin 
								And TBL_GSTR2.fp = @Fp
							)			
					) 
				FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
			))
		) AS hsnsum,

		(	Select pos,sply_ty,
			(	Select num,rt,adamt, iamt, camt, samt, csamt
				From TBL_GSTR2_TXPD_ITMS 
				Where txpdid = TBL_GSTR2_TXPD.txpdid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_TXPD 
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_TXPD_ITMS Where TBL_GSTR2_TXPD_ITMS.txpdid = TBL_GSTR2_TXPD.txpdid)
			FOR JSON PATH
		) AS txpd,

		(	Select pos,sply_ty,
			(	Select num,rt,adamt, iamt, camt, samt, csamt
				From TBL_GSTR2_TXI_ITMS 
				Where txiid = TBL_GSTR2_TXI.txiid
				FOR JSON PATH
			) AS itms
			From TBL_GSTR2_TXI
			Where IsNull(flag,'') = @Flag
			And gstr2id in 
				(	Select gstr2id 
					From TBL_GSTR2 
					Where TBL_GSTR2.gstin = @Gstin 
					And TBL_GSTR2.fp = @Fp 
				) 
			And Exists(Select 1 From TBL_GSTR2_TXI_ITMS Where TBL_GSTR2_TXI_ITMS.txiid = TBL_GSTR2_TXI.txiid)
			FOR JSON PATH
		) AS txi,

		( Select JSON_QUERY
				((	Select
						(	Select	JSON_QUERY	
							((	
								(	Select cpddr,exptdsply,ngsply,nilsply
									From TBL_GSTR2_NIL_INTER  t1,
									TBL_GSTR2_NIL t2,
									TBL_GSTR2 t3
									Where t1.nilid = t2.nilid
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	 
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER	
								) 
							)) 
						) As inter,

						(	Select	JSON_QUERY	
							((	
								(	Select cpddr,exptdsply,ngsply,nilsply
									From TBL_GSTR2_NIL_INTRA  t1,
									TBL_GSTR2_NIL t2,
									TBL_GSTR2 t3
									Where t1.nilid = t2.nilid
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As intra

					From TBL_GSTR2_NIL 
					Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					) 
					And IsNull(flag,'') = @Flag
					Group by TBL_GSTR2_NIL.gstr2id,TBL_GSTR2_NIL.nilid  
					FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				))
		) As nil_supplies,
	
			( Select JSON_QUERY
				((	Select
						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule2_2'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	 
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER	
								) 
							)) 
						) As rule2_2,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_1_m'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_1_m,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule8_1_h'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule8_1_h,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_2_a'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_2_a,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'rule7_2_b'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As rule7_2_b,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'revitc'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As revitc,

						(	Select	JSON_QUERY	
							((	
								(	Select iamt,camt,samt,csamt
									From TBL_GSTR2_ITCRVSL_ITMS t1,
									TBL_GSTR2_ITCRVSL t2,
									TBL_GSTR2 t3
									Where t1.itcrvslid = t2.itcrvslid
									And t1.rulename = 'other'
									And t2.gstr2id = t3.gstr2id
									And IsNull(t2.flag,'') = @Flag
									And t3.gstin = @Gstin
									And t3.fp = @Fp	   	
									FOR JSON PATH , WITHOUT_ARRAY_WRAPPER		
								) 
							)) 
						) As other


					From TBL_GSTR2_ITCRVSL 
					Where gstr2id in 
					(	Select gstr2id 
						From TBL_GSTR2
						Where TBL_GSTR2.gstin = @Gstin 
						And TBL_GSTR2.fp = @Fp
					) 
					And IsNull(flag,'') = @Flag
					Group by TBL_GSTR2_ITCRVSL.gstr2id,TBL_GSTR2_ITCRVSL.itcrvslid  
					FOR JSON PATH, WITHOUT_ARRAY_WRAPPER
				))
		) As itc_rvsl 
 

		From TBL_GSTR2 
		Where gstin = @Gstin 
		And TBL_GSTR2.fp = @fp 
		Group by TBL_GSTR2.gstin, TBL_GSTR2.fp 
		FOR JSON AUTO   

	End

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End