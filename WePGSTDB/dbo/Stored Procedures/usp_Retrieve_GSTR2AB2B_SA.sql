
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2A B2B Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/10/2017	Karthik		Introduced Aliases in the Result Set
02/01/2018	Karthik		Included the Supplier Name based on ctin from Suppliermaster table/ValidGstin Table.

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2AB2B_SA '27AHQPA7588L1ZJ','052017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2AB2B_SA] 
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.b2bid desc) AS 'SNo',
		ctin ,
		Space(500) as 'CtinName',
		inum,
		idt,
		val,
		rchrg,
		pos,
		inv_typ,
		num,
		rt,
		txval,	
		iamt,
		camt,
		samt,
		csamt
  	Into #Tbl_Gstr2A_Bulk
	From TBL_GSTR2A t1
	Inner Join TBL_GSTR2A_B2B t2 On t2.gstr2aid = t1.gstr2aid
	Inner Join TBL_GSTR2A_B2B_INV t3 On t3.b2bid = t2.b2bid
	Inner Join TBL_GSTR2A_B2B_INV_ITMS t4 On t4.invid = t3.invid
	Inner Join TBL_GSTR2A_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @Gstin
	And fp = @Fp
	Order By t2.b2bid Desc


	Update #Tbl_Gstr2A_Bulk 
	SET #Tbl_Gstr2A_Bulk.CtinName = t2.SupplierName 
	FROM #Tbl_Gstr2A_Bulk t1,
			TBL_Supplier t2 
	WHERE t1.ctin = t2.gstinno
	and t2.rowstatus =1
	And t1.CtinName =''


	Update #Tbl_Gstr2A_Bulk 
	SET #Tbl_Gstr2A_Bulk.CtinName = t2.LegalName 
	FROM #Tbl_Gstr2A_Bulk t1,
			TBL_ValidGSTIN t2 
	WHERE t1.ctin = t2.ValidGstin
	And t1.CtinName =''
	

	Select
		SNo,
		ctin as 'CTIN',
		CtinName as 'Supplier Name',
		inum as 'Invoice Number',
		idt as 'Invoice Date',
		val as 'Invoice Value',
		rchrg as 'Reverse Charge',
		pos as 'Place Of Supply',
		inv_typ as 'Invoice Type',
		num as 'Item Number',
		rt as 'Tax Rate',
		txval as 'Taxable Value',		
		iamt as 'IGST Amount',
		camt as 'CGST Amount',
		samt as 'SGST Amount',
		csamt as 'CESS Amount'
	From #Tbl_Gstr2A_Bulk
	order by SNo
		
	Return 0
		

End