
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 RETSUM Records from the corresponding tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/3/2017	Seshadri 	Initial Version
10/3/2017	Seshadri	Removed some of the columns from the result set based on testing feedback

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_RETSUM '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1_RETSUM]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t1.gstr1sumid Asc) AS 'SNo',
		-- t1.gstr1sumid as gstr1sumid,
		-- t1.summ_typ as summ_typ,
		-- t1.chksum as chksum,
		t2.sec_nm as sec_nm, 
		--t2.chksum as sec_chksum,
		t2.ttl_rec as sec_ttl_rec,
		t2.ttl_val as sec_ttl_val ,
		t2.ttl_tax as sec_ttl_tax,
		t2.ttl_igst as sec_ttl_igst,
		t2.ttl_cgst as sec_ttl_cgst,
		t2.ttl_sgst as sec_ttl_sgst,
		t2.ttl_cess as sec_ttl_cess,
		t2.ttl_nilsup_amt as sec_ttl_nilsup_amt,
		t2.ttl_expt_amt as sec_ttl_expt_amt,
		t2.ttl_ngsup_amt as sec_ttl_ngsup_amt,
		t2.ttl_doc_issued as sec_ttl_doc_issued,
		t2.ttl_doc_cancelled as sec_ttl_doc_cancelled,
		t2.net_doc_issued as sec_net_doc_issued,
		t3.ctin as ctin,
		t3.state_cd as state_cd,
		-- t3.chksum as cpst_chksum,
		t3.ttl_rec as cpst_ttl_rec,
		t3.ttl_val as cpst_ttl_val ,
		t3.ttl_tax as cpst_ttl_tax,
		t3.ttl_igst as cpst_ttl_igst,
		t3.ttl_cgst as cpst_ttl_cgst,
		t3.ttl_sgst as cpst_ttl_sgst,
		t3.ttl_cess as cpst_ttl_cess
	From TBL_GSTR1Summary t1
	Inner Join TBL_GSTR1SummaryAction t2 On t2.gstr1sumid = t1.gstr1sumid
	Left Join TBL_GSTR1SummaryActionList t3 On ((t3.gstr1sumActionid = t2.gstr1sumActionid) And (t3.sec_nm = t2.sec_nm) )  
	Where t1.gstin = @Gstin
	And t1.ret_period = @Fp
	Order By t1.gstr1sumid Asc

	Return 0

End