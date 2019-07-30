

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTIN Sales Summary
				
Written by  : Karthik

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
05/11/2018  Karthik     Included Credit and debit data for the summary
05/14/2018  Karthik     Included All option for Gstin Selection
*/

/* Sample Procedure Call

exec usp_MIS_GSTIN_SalesSummary 'ALL','082017',47


 */
 
CREATE PROCEDURE [dbo].[usp_MIS_GSTIN_SalesSummary]  
	@Gstinno varchar(15),
	@Period varchar(10),
	@Custid int
-- /*mssx*/ With Encryption 
as 
Begin

			Create Table #Gstins(Gstinno nvarchar(15))

			if(@Gstinno = 'ALL')
			Begin
				insert into #Gstins (Gstinno) Select Gstinno from tbl_Cust_Gstin where custid = @Custid and rowstatus =1		
			End
			Else
			Begin
				insert into #Gstins (Gstinno) Select Gstinno from tbl_Cust_Gstin where gstinno =  @Gstinno and custid = @Custid and rowstatus =1
			End

				select AB.gstin,sum(Otaxval) as Otaxval,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #tmpInvoices from (
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2B t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2B_INV t3 On t3.b2bid = t2.b2bid
							Inner Join TBL_GSTR1_B2B_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							group by t1.gstin
							union ALL
				select t1.gstin,sum(txval) as Otaxval,0 as OIGST,0 as OCGST,0 as OSGST,sum(csamt)as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CL t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_B2CL_INV t3 On t3.b2clid = t2.b2clid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS t4 On t4.invid = t3.invid
							Inner Join TBL_GSTR1_B2CL_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin 
							union ALL
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin
							union ALL
				select t1.gstin,sum(txval) as Otaxval,sum(iamt)as OIGST,0 as OCGST,0 as OSGST,0 as OCESS  
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
							Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin)  as AB
				group by AB.gstin


					select ACD.gstin,ACD.ntty,sum(Otaxval) as Otaxval,sum(OIGST) as OIGST,sum(OCGST) as OCGST,sum(OSGST) as OSGST,sum(OCESS)as OCESS into #tmpCDN from (
					select t1.gstin,t3.ntty,sum(txval) as Otaxval,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							group by t1.gstin,t3.ntty
							Union All
							select t1.gstin,t2.ntty,sum(txval) as Otaxval,sum(iamt)as OIGST,sum(camt)as OCGST,sum(samt) as OSGST,sum(csamt)as OCESS  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							group by t1.gstin,t2.ntty
							)as ACD
				group by ACD.gstin,ACD.ntty

				-- Add/Minus Credit/Debit data in output tax liability based on 'ntty- note Type'
				Update #tmpInvoices 
				SET #tmpInvoices.Otaxval =  t1.Otaxval + t2.Otaxval, 
					#tmpInvoices.OIGST =  t1.OIGST + t2.OIGST,
					#tmpInvoices.OCGST =  t1.OCGST + t2.OCGST,
					#tmpInvoices.OSGST =  t1.OSGST + t2.OSGST,
					#tmpInvoices.OCESS =  t1.OCESS + t2.OCESS
				FROM #tmpInvoices t1,
						#tmpCDN t2 
				WHERE t1.gstin = t2.gstin 
				And t2.ntty = 'D'

				Update #tmpInvoices 
				SET #tmpInvoices.Otaxval =  t1.Otaxval - t2.Otaxval, 
					#tmpInvoices.OIGST =  t1.OIGST - t2.OIGST, 
					#tmpInvoices.OCGST =  t1.OCGST - t2.OCGST,
					#tmpInvoices.OSGST =  t1.OSGST - t2.OSGST,
					#tmpInvoices.OCESS =  t1.OCESS - t2.OCESS
				FROM #tmpInvoices t1,
						#tmpCDN t2 
				WHERE t1.gstin = t2.gstin 
				And t2.ntty = 'C'


		select gstin,@Period as [Period],
		Otaxval as 'Total Taxable Amount',
		OIGST as 'Total IGST Amount',
		OCGST as 'Total CGST Amount',
		OSGST as 'Total SGST Amount',
		OCESS as 'Total CESS Amount' from #tmpInvoices  order by gstin



End