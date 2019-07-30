/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTIN Sales Summary
				
Written by  : Karthik

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec [usp_MIS_InvoiceCountandDetails]


 */
 
Create PROCEDURE [usp_MIS_InvoiceCountandDetails]  
	--@custid varchar(15)
	--@Period varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

				select GstinNo as GstinNo into #GstinNos from tbl_cust_gstin where rowstatus =1 --and custid = @custid

				select AB.gstin,fp,ActionType,sum(InvoiceCount) as TotalInvCount from (
				select t1.gstin,fp,'B2B' as ActionType,count(inum) as InvoiceCount
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid
							Where t1.gstin in (select GstinNo from #GstinNos)
							--And fp = @Period 
							And flag = 'U'
							group by t1.gstin,fp
							union ALL
				select t1.gstin,fp,'B2CL' as ActionType,count(inum) as InvoiceCount
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
							Where t1.gstin in (select GstinNo from #GstinNos)
							--And fp = @Period 
							And flag = 'U'  
							group by t1.gstin,fp 
							union ALL
				select t1.gstin,fp,'CDNR' as ActionType,count(inum) as InvoiceCount
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Where t1.gstin in (select GstinNo from #GstinNos)
							--And fp = @Period 
							And flag = 'U'  
							group by t1.gstin,fp 
				union ALL
				select t1.gstin,fp,'CDNUR' as ActionType,count(inum) as InvoiceCount
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Where t1.gstin in (select GstinNo from #GstinNos)
							--And fp = @Period 
							And flag = 'U'  
							group by t1.gstin,fp 
							union ALL
				select t1.gstin,fp,'EXP' as ActionType,count(inum) as InvoiceCount
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
							Where t1.gstin in (select GstinNo from #GstinNos)
							--And fp = @Period 
							And flag = 'U'  
							group by t1.gstin,fp)  as AB
				group by AB.gstin,AB.fp,AB.ActionType


End