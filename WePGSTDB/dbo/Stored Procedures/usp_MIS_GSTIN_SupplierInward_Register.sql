
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTIN Supplier Inward Register
				
Written by  : Karthik

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_MIS_GSTIN_SupplierInward_Register '33GSPTN0801G1ZM','072017'


 */
 
CREATE PROCEDURE [usp_MIS_GSTIN_SupplierInward_Register]  
	@Gstinno varchar(15),
	@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

				select t1.gstin,t2.ctin as 'Supplier GSTIN',sum(txval) as 'Total Taxable Amount',sum(iamt) as 'Total IGST Amount',sum(camt) as 'Total CGST Amount',sum(samt) as 'Total SGST Amount',sum(csamt)as 'Total CESS Amount'
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin = @Gstinno
							And fp = @Period 
							And flag = 'U'
							group by t1.gstin,t2.ctin

End