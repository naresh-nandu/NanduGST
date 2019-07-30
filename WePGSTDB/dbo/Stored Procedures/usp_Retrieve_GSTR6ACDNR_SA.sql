

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR6A CDNR Records from the corresponding staging area tables
				
Written by  : muskan.garg@wepdigital.com

Date		Who			    Decription 
05/15/2018	Muskan			Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR6ACDNR_SA '27AHQPA7588L1ZJ'



 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_GSTR6ACDNR_SA]  
	@Gstin varchar(15),
	@fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.cdnrid desc) AS 'SNo',
		--t2.cdnrid as cdnrid,
		ctin,
		Space(500) as 'CtinName',		
		chksum,
		ntty,
		nt_num,
		nt_dt,
		pos,
		inum,
		idt,	
		num,
		rt,
		txval,	
		iamt,
		camt,
		samt,
		csamt 
	Into #Tbl_Gstr6A_Bulk
	From TBL_GSTR6A t1
	Inner Join TBL_GSTR6A_CDNR t2 On t2.gstr6aid = t1.gstr6aid
	Inner Join TBL_GSTR6A_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
	Inner Join TBL_GSTR6A_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
	Inner Join TBL_GSTR6A_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
	Where gstin = @Gstin
	And fp = @Fp
	Order By t2.cdnrid Desc

	Update #Tbl_Gstr6A_Bulk 
	SET #Tbl_Gstr6A_Bulk.CtinName = t2.SupplierName 
	FROM #Tbl_Gstr6A_Bulk t1,
			TBL_Supplier t2 
	WHERE t1.ctin = t2.gstinno
	and t2.rowstatus =1
	And t1.CtinName =''


	Update #Tbl_Gstr6A_Bulk 
	SET #Tbl_Gstr6A_Bulk.CtinName = t2.LegalName 
	FROM #Tbl_Gstr6A_Bulk t1,
			TBL_ValidGSTIN t2 
	WHERE t1.ctin = t2.ValidGstin
	And t1.CtinName =''

	Select SNo,
		ctin as 'Counter Party GSTIN',
		CtinName as 'Supplier Name',		
		chksum as 'Check Sum',
		ntty as 'Note Type',
		nt_num as 'Note Number',
		nt_dt as 'Note Date',
		pos as 'Place of Supply',
		inum as 'Invoice Number',
		idt as 'Invoice Date',
		num as 'Item Number',
		rt as 'Tax Rate',
		txval as 'Taxable Value',		
		iamt as 'IGST Amount',
		camt as 'CGST Amount',
		samt as 'SGST Amount',
		csamt as 'CESS Amount'
	From #Tbl_Gstr6A_Bulk
	order by SNo
		
	Return 0

End