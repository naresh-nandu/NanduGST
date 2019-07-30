
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

exec [usp_Retrieve_GSTR2_RETSUM] '33GSPTN0802G1ZL','072017'


 */
 
Create PROCEDURE [usp_Retrieve_GSTR2_RETSUM]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t1.gstr2sumid Asc) AS 'SNo',
		-- t1.gstr1sumid as gstr1sumid,
		-- t1.summ_typ as summ_typ,
		-- t1.chksum as chksum,
		t2.section_name as sec_nm, 
		--t2.chksum as sec_chksum,
		t2.rc as sec_ttl_rec,
		t2.ttl_val as ttl_val ,
		t2.ttl_txpd_igst as ttl_txpd_igst,
		t2.ttl_txpd_cgst as ttl_txpd_cgst,
		t2.ttl_txpd_sgst as ttl_txpd_sgst,
		t2.ttl_txpd_cess as ttl_txpd_cess,
		t2.ttl_itcavld_igst as ttl_itcavld_igst,
		t2.ttl_itcavld_sgst as ttl_itcavld_sgst,
		t2.ttl_itcavld_cgst as ttl_itcavld_cgst,
		t2.ttl_itcavld_cess as ttl_itcavld_cess,
		t3.ctin as ctin,
		t3.rc as cpst_ttl_rec,
		t3.ttl_txpd_igst as cpst_ttl_txpd_igst,
		t3.ttl_txpd_cgst as cpst_ttl_txpd_cgst,
		t3.ttl_txpd_sgst as cpst_ttl_txpd_sgst,
		t3.ttl_txpd_cess as cpst_ttl_txpd_cess,
		t3.ttl_itcavld_igst as cpst_ttl_itcavld_igst,
		t3.ttl_itcavld_sgst as cpst_ttl_itcavld_sgst,
		t3.ttl_itcavld_cgst as cpst_ttl_itcavld_cgst,
		t3.ttl_itcavld_cess as cpst_ttl_itcavld_cess
	From TBL_GSTR2Summary t1
	Inner Join TBL_GSTR2SummaryAction t2 On t2.gstr2sumid = t1.gstr2sumid
	Left Join TBL_GSTR2SummaryActionList t3 On ((t3.gstr2sumActionid = t2.gstr2sumActionid))  
	Where t1.gstin = @Gstin
	And t1.ret_period = @Fp
	Order By t1.gstr2sumid Asc

	Return 0

End