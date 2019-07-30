
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 RETSUM JSON Records to the corresponding tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/3/2017	Seshadri			Initial Version


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_RETSUM
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_RETSUM]
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
	Values('Gstr1 - CDNR',@Gstin,@RecordContents,1,1,GetDate()) */


	Select	space(50) as gstr1sumActionid,
			space(50) as gstr1sumid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			Retsum.chksum as chksum,
			'L' as summ_typ,

			Secsum.sec_nm as sec_nm,
			Secsum.chksum as sec_chksum,
			Secsum.ttl_rec as sec_ttl_rec,
			Secsum.ttl_val as sec_ttl_val ,
			Secsum.ttl_tax as sec_ttl_tax,
			Secsum.ttl_igst as sec_ttl_igst,
			Secsum.ttl_cgst as sec_ttl_cgst,
			Secsum.ttl_sgst as sec_ttl_sgst,
			Secsum.ttl_cess as sec_ttl_cess,
			Secsum.ttl_nilsup_amt as sec_ttl_nilsup_amt,
			Secsum.ttl_expt_amt as sec_ttl_expt_amt,
			Secsum.ttl_ngsup_amt as sec_ttl_ngsup_amt,
			Secsum.ttl_doc_issued as sec_ttl_doc_issued,
			Secsum.ttl_doc_cancelled as sec_ttl_doc_cancelled,
			Secsum.net_doc_issued as sec_net_doc_issued,

			ctin as ctin,
			Cpty.chksum as cpty_chksum,
			Cpty.ttl_rec as cpty_ttl_rec,
			Cpty.ttl_val as cpty_ttl_val ,
			Cpty.ttl_tax as cpty_ttl_tax,
			Cpty.ttl_igst as cpty_ttl_igst,
			Cpty.ttl_cgst as cpty_ttl_cgst,
			Cpty.ttl_sgst as cpty_ttl_sgst,
			Cpty.ttl_cess as cpty_ttl_cess ,

			state_cd as state_cd,
			Stcd.chksum as stcd_chksum,
			Stcd.ttl_rec as stcd_ttl_rec,
			Stcd.ttl_val as stcd_ttl_val ,
			Stcd.ttl_tax as stcd_ttl_tax,
			Stcd.ttl_igst as stcd_ttl_igst,
			Stcd.ttl_cgst as stcd_ttl_cgst,
			Stcd.ttl_sgst as stcd_ttl_sgst,
			Stcd.ttl_cess as stcd_ttl_cess 

	Into #TBL_GSTR1_RETSUM
	From OPENJSON(@RecordContents) 
	WITH
	(
		gstin varchar(15),
		ret_period varchar(10),
		chksum varchar(64),
		--summ_typ varchar(1),
		sec_sum nvarchar(max) as JSON
	) As Retsum
	Cross Apply OPENJSON(Retsum.sec_sum) 
	WITH
	(
		sec_nm varchar(15),
		chksum varchar(64),
		ttl_rec int,
		ttl_val decimal(18,2) ,
	    ttl_tax decimal(18,2),
        ttl_igst decimal(18,2),
        ttl_cgst decimal(18,2),
        ttl_sgst decimal(18,2),
        ttl_cess decimal(18,2),
		ttl_nilsup_amt decimal(18,2),
		ttl_expt_amt decimal(18,2),
		ttl_ngsup_amt decimal(18,2),
		ttl_doc_issued decimal(18,2),
		ttl_doc_cancelled decimal(18,2),
		net_doc_issued decimal(18,2),
		cpty_sum nvarchar(max) as JSON,
		[state code summary] nvarchar(max) as JSON
	) As Secsum
	Outer Apply OPENJSON(Secsum.cpty_sum) 
	WITH
	(
		ctin varchar(15),
		chksum varchar(64),
		ttl_rec int,
        ttl_tax decimal(18,2),
		ttl_val decimal(18,2) ,
        ttl_igst decimal(18,2),
        ttl_cgst decimal(18,2),
        ttl_sgst decimal(18,2),
        ttl_cess decimal(18,2)
 	) As Cpty
	Outer Apply OPENJSON(Secsum.[state code summary]) 
	WITH
	(
		state_cd varchar(2),
		chksum varchar(64),
		ttl_rec int,
        ttl_tax decimal(18,2),
		ttl_val decimal(18,2) ,
        ttl_igst decimal(18,2),
        ttl_cgst decimal(18,2),
        ttl_sgst decimal(18,2),
        ttl_cess decimal(18,2)
 	) As Stcd
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Delete The Records

	Select gstr1sumid
	Into #TBL_GSTR1_SUM
	From TBL_GSTR1Summary
	Where gstin = @Gstin
	And ret_period = @Fp

	if Exists(Select 1 From #TBL_GSTR1_SUM)
	Begin

		Delete From TBL_GSTR1SummaryActionList
		Where gstr1sumActionid in
		(
			Select gstr1sumActionid
			From TBL_GSTR1SummaryAction
			Where gstr1sumid in (Select gstr1sumid From #TBL_GSTR1_SUM)
		)

		Delete From TBL_GSTR1SummaryAction
		Where gstr1sumid in (Select gstr1sumid From #TBL_GSTR1_SUM)

		Delete From TBL_GSTR1Summary
		Where gstr1sumid in (Select gstr1sumid From #TBL_GSTR1_SUM)

	End

	-- Insert Records into Table TBL_GSTR1Summary

	Insert TBL_GSTR1Summary (gstin,gstinId,ret_period,chksum,summ_typ)
	Select	distinct gstin,gstinid,fp,chksum,summ_typ
	From #TBL_GSTR1_RETSUM t1

	Update #TBL_GSTR1_RETSUM 
	SET #TBL_GSTR1_RETSUM.gstr1sumid = t2.gstr1sumid 
	FROM #TBL_GSTR1_RETSUM t1,
			TBL_GSTR1Summary t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid
	And t1.fp = t2.ret_period 
	And t1.chksum = t2.chksum
	And t1.summ_typ = t2.summ_typ

	-- Insert Records into Table TBL_GSTR1SummaryAction

	Insert TBL_GSTR1SummaryAction
	(	gstr1sumid,sec_nm,
		chksum,ttl_rec,ttl_val,ttl_tax,
		ttl_igst,ttl_cgst,ttl_sgst,ttl_cess,
		ttl_nilsup_amt,ttl_expt_amt,ttl_ngsup_amt,
		ttl_doc_issued,ttl_doc_cancelled,net_doc_issued
	)
	Select distinct gstr1sumid,sec_nm,
					sec_chksum,sec_ttl_rec,sec_ttl_val,sec_ttl_tax, 
					sec_ttl_igst,sec_ttl_cgst,sec_ttl_sgst,sec_ttl_cess, 
					sec_ttl_nilsup_amt,sec_ttl_expt_amt,sec_ttl_ngsup_amt,
					sec_ttl_doc_issued,sec_ttl_doc_cancelled,sec_net_doc_issued
	From #TBL_GSTR1_RETSUM t1

	Update #TBL_GSTR1_RETSUM 
	SET #TBL_GSTR1_RETSUM.gstr1sumActionid = t2.gstr1sumActionid 
	FROM #TBL_GSTR1_RETSUM t1,
		 TBL_GSTR1SummaryAction t2 
	WHERE t1.gstr1sumid = t2.gstr1sumid 
	And t1.sec_nm = t2.sec_nm

	-- Insert Records into Table TBL_GSTR1SummaryActionList

	Insert TBL_GSTR1SummaryActionList
	(	gstr1sumActionid,sec_nm,ctin,
		chksum,ttl_rec,ttl_val,ttl_tax,
		ttl_igst,ttl_cgst,ttl_sgst,ttl_cess
	)
	Select distinct gstr1sumActionid,sec_nm,ctin,
					cpty_chksum,cpty_ttl_rec,cpty_ttl_val,cpty_ttl_tax, 
					cpty_ttl_igst,cpty_ttl_cgst,cpty_ttl_sgst,cpty_ttl_cess 
	From #TBL_GSTR1_RETSUM t1
	Where IsNull(t1.ctin,'') <> ''

	Insert TBL_GSTR1SummaryActionList
	(	gstr1sumActionid,sec_nm,state_cd,
		chksum,ttl_rec,ttl_val,ttl_tax,
		ttl_igst,ttl_cgst,ttl_sgst,ttl_cess
	)
	Select distinct gstr1sumActionid,sec_nm,state_cd,
					stcd_chksum,stcd_ttl_rec,stcd_ttl_val,stcd_ttl_tax, 
					stcd_ttl_igst,stcd_ttl_cgst,stcd_ttl_sgst,stcd_ttl_cess 
	From #TBL_GSTR1_RETSUM t1
	Where IsNull(t1.state_cd,'') <> '' 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End