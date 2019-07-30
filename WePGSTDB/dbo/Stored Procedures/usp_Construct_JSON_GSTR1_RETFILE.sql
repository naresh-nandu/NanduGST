
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Construct the data in JSON format for GSTR1 RETFILE
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/03/2017	Seshadri			Initial Version
3/26/2018	Seshadri	Included chksum for each acion output 

*/

/* Sample Procedure Call

exec usp_Construct_JSON_GSTR1_RETFILE '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE usp_Construct_JSON_GSTR1_RETFILE 
	@Gstin varchar(15),
	@Fp varchar(10),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	gstin,ret_period,summ_typ,chksum,
	(	Select sec_nm,chksum, 
				ttl_rec,ttl_val,ttl_tax,
				ttl_igst,ttl_cgst,ttl_sgst,ttl_cess ,
				ttl_nilsup_amt,ttl_expt_amt,ttl_ngsup_amt,
				ttl_doc_issued,ttl_doc_cancelled,net_doc_issued,
				
			(	Select ctin,chksum,
						ttl_rec,ttl_val,ttl_tax,
						ttl_igst,ttl_cgst,ttl_sgst,ttl_cess
				From TBL_GSTR1SummaryActionList
				Where gstr1sumActionid In (	Select gstr1sumActionid
											From  TBL_GSTR1Summary t1,
												  TBL_GSTR1SummaryAction t2
											Where t2.gstr1sumid = t1.gstr1sumid
											And t1.gstin = @Gstin 
											And t1.ret_period = @fp
										)
				And  sec_nm = TBL_GSTR1SummaryAction.sec_nm
				And IsNull(ctin,'') <> ''
				FOR JSON PATH
			) As cpty_sum ,

			(	Select state_cd,chksum,
						ttl_rec,ttl_val,ttl_tax,
						ttl_igst,ttl_cgst,ttl_sgst,ttl_cess
				From TBL_GSTR1SummaryActionList
				Where gstr1sumActionid In (	Select gstr1sumActionid
											From  TBL_GSTR1Summary t1,
												  TBL_GSTR1SummaryAction t2
											Where t2.gstr1sumid = t1.gstr1sumid
											And t1.gstin = @Gstin 
											And t1.ret_period = @fp
										)
				And  sec_nm = TBL_GSTR1SummaryAction.sec_nm
				And IsNull(state_cd,'') <> ''
				FOR JSON PATH
			) As [state code summary] 

		From TBL_GSTR1SummaryAction
		Where gstr1sumid = (Select gstr1sumid From TBL_GSTR1Summary
							Where gstin = @Gstin
							And ret_period = @Fp)
		Group BY sec_nm,chksum, 
				ttl_rec,ttl_val,ttl_tax,
				ttl_igst,ttl_cgst,ttl_sgst,ttl_cess ,
				ttl_nilsup_amt,ttl_expt_amt,ttl_ngsup_amt,
				ttl_doc_issued,ttl_doc_cancelled,net_doc_issued
		FOR JSON PATH	
	) As sec_num

	From TBL_GSTR1Summary 
	Where gstin = @Gstin 
	And ret_period = @fp 
	Group by TBL_GSTR1Summary.gstin,TBL_GSTR1Summary.ret_period,
			 TBL_GSTR1Summary.summ_typ,TBL_GSTR1Summary.chksum 
	FOR JSON AUTO   

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End