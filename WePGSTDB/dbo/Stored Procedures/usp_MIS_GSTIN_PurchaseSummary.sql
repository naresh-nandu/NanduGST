
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTIN Purchase Summary
				
Written by  : Karthik

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_MIS_GSTIN_PurchaseSummary '33GSPTN0801G1ZM','072017'


 */
 
CREATE PROCEDURE [usp_MIS_GSTIN_PurchaseSummary]  
	@Gstinno varchar(15),
	@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

				select AB.gstin,sum(Otaxval) as 'Total Taxable Amount',sum(OIGST) as 'Total IGST Amount',sum(OCGST) as 'Total CGST Amount',sum(OSGST) as 'Total SGST Amount',sum(OCESS)as 'Total CESS Amount' from (
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin = @Gstinno
							And fp = @Period 
							And flag = 'U'
							group by t1.gstin
							union ALL
				select t1.gstin,sum(txval) as Otaxval,0 as OIGST,0 as OCGST,0 as OSGST,sum(csamt)as OCESS  
						from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_B2BUR t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_B2BUR_INV t3 On t3.b2burid = t2.b2burid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR2_B2BUR_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin = @Gstinno
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin
							union ALL
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,sum(csamt) as OCESS  
						from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_IMPG t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_IMPG_ITMS t3 On t3.impgid = t2.impgid
							Where t1.gstin = @Gstinno
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin
							union ALL
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,sum(csamt) as OCESS  
						from TBL_GSTR2 t1
							Inner Join TBL_GSTR2_IMPS t2 On t2.gstr2id = t1.gstr2id
							Inner Join TBL_GSTR2_IMPS_ITMS t3 On t3.impsid = t2.impsid
							Where t1.gstin = @Gstinno
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin)  as AB
				group by AB.gstin

End