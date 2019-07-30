

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Count Total Invoice and Api Calling Statistics
				
Written by  : nareshn@wepindia.com 

Date		Who			 Decription 
19/04/2018	Raja M		 Initial Version


*/

/* Sample Procedure Call


exec usp_Retrieve_GSTIN_Wise_Invoice_Count 1,'GSTR1'
 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_GSTIN_Wise_Invoice_Count]    
	@PartnerId int,
	@GSTRType varchar(20)
	--@Gstin varchar(15),
	--@Period varchar(10),
	--@GSTRType varchar(20),
	--@RetInvoiceCount varchar(50) Output,
	--@RetAPICount varchar(50) Output
as 
Begin

	Set Nocount on
			
		--Declare @PartnerId int
		--set @PartnerId = 1

		Select CustId into #CustIds from TBL_Customer Where PartnerId = @PartnerId and rowstatus = 1

		Select GSTINNo into #GSTINNos from TBL_Cust_Gstin Where CustId in (Select * from #CustIds) and rowstatus = 1

		Select gstin, fp, 0 as InvoiceCount, 0 as APICount into #GstinWiseCounts from TBL_GSTR1 where gstin in (Select GSTINNo from #GSTINNos) Group  by gstin, fp

		Select gstin, fp, SUM(Counts) as InvoiceCount into #InvCounts from (
		Select gstin, fp, count(t3.inum) as Counts from TBL_GSTR1 t1
		INNER JOIN TBL_GSTR1_B2B t2 ON t2.gstr1id = t1.gstr1id
		INNER JOIN TBL_GSTR1_B2B_INV t3 ON t3.b2bid = t2.b2bid
		GROUP BY gstin, fp
		UNION ALL
		Select gstin, fp, count(t3.inum) as Counts from TBL_GSTR1 t1
		INNER JOIN TBL_GSTR1_B2CL t2 ON t2.gstr1id = t1.gstr1id
		INNER JOIN TBL_GSTR1_B2CL_INV t3 ON t3.b2clid = t2.b2clid
		GROUP BY gstin, fp
		UNION ALL
		Select gstin, fp, count(t3.inum) as Counts from TBL_GSTR1 t1
		INNER JOIN TBL_GSTR1_EXP t2 ON t2.gstr1id = t1.gstr1id
		INNER JOIN TBL_GSTR1_EXP_INV t3 ON t3.expid = t2.expid
		GROUP BY gstin, fp
		UNION ALL
		Select gstin, fp, count(t3.nt_num) as Counts from TBL_GSTR1 t1
		INNER JOIN TBL_GSTR1_CDNR t2 ON t2.gstr1id = t1.gstr1id
		INNER JOIN TBL_GSTR1_CDNR_NT t3 ON t3.cdnrid = t2.cdnrid
		GROUP BY gstin, fp
		UNION ALL
		Select gstin, fp, count(t2.nt_num) as Counts from TBL_GSTR1 t1
		INNER JOIN TBL_GSTR1_CDNUR t2 ON t2.gstr1id = t1.gstr1id
		GROUP BY gstin, fp
		) as R1
		GROUP BY gstin, fp

		Select t1.gstin, t1.fp, t1.InvoiceCount into #InvoiceCounts from #InvCounts t1 INNER JOIN #GstinWiseCounts t2
		ON t1.gstin = t2.gstin and t1.fp = t2.fp
		--Group By t1.gstin, t1.fp

		Select t1.gstin, t1.fp, Count(*) as APICount into #APICounts from #GstinWiseCounts t1 INNER JOIN TBL_ASP_API_Transactions t2
		ON t1.gstin = t2.Gstinno and t1.fp = t2.Period and t2.ApiName like '%' + @GSTRType + '%'
		Group By t1.gstin, t1.fp

		Update #GstinWiseCounts 
		SET APICount = t2.APICount 
		FROM #GstinWiseCounts t1 INNER JOIN #APICounts t2 
		ON t1.gstin = t2.gstin and t1.fp = t2.fp

		Update #GstinWiseCounts 
		SET InvoiceCount = t2.InvoiceCount 
		FROM #GstinWiseCounts t1, #InvoiceCounts t2 
		WHERE t1.gstin = t2.gstin and t1.fp = t2.fp
		
		--Select * from #GstinWiseCounts --where gstin = '33GSPTN0802G1ZL' and fp = '032018'
		--Select * from #InvoiceCounts where gstin = '33GSPTN0802G1ZL' and fp = '032018'
		--Select * from #APICounts where gstin = '33GSPTN0802G1ZL' and fp = '032018'
		Select * from #GstinWiseCounts FOR JSON AUTO
End