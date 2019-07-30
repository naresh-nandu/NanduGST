


/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve GSTIN Sales Summary
				
Written by  : Karthik

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
05/11/2018  Karthik     Included CDNR and CDNUR action type for summary
05/14/2018  Karthik     Included All option for Gstin Selection
*/

/* Sample Procedure Call

exec usp_MIS_GSTIN_SalesSummary_InvoiceType 'ALL','082017',47


 */
 
CREATE PROCEDURE [dbo].[usp_MIS_GSTIN_SalesSummary_InvoiceType]  
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

				select A.gstin,A.ActionType as'Action Type',txval as  'Total Taxable Amount',iamt as  'Total IGST Amount',camt as  'Total CGST Amount',samt as  'Total SGST Amount',csamt as  'Total CESS Amount' into #tbl_Summary from (
				select t1.gstin,'B2B' as ActionType ,sum(txval) as  txval,sum(iamt)as  iamt,sum(camt)as  camt,sum(samt) as  samt,sum(csamt)as  csamt 
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
				select t1.gstin,'B2CL' as ActionType ,sum(txval) as txval,sum(iamt) as  iamt,0 as  camt,0 as  samt,sum(csamt)as  csamt
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
				select t1.gstin,'B2CS' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,sum(camt)as camt,sum(samt) as samt,sum(csamt)as csamt
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_B2CS t2 On t2.gstr1id = t1.gstr1id
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin
							union ALL
				select t1.gstin,'EXP' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,0 as camt,0 as samt,0 as csamt
						from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_EXP t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_EXP_INV t3 On t3.expid = t2.expid
							Inner Join TBL_GSTR1_EXP_INV_ITMS t4 On t4.invid = t3.invid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'  
							group by t1.gstin
							Union All
			select t1.gstin,'CDN-Debit' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,sum(camt)as camt,sum(samt) as samt,sum(csamt)as csamt  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							And t3.ntty ='D'
							group by t1.gstin
							Union All
			select t1.gstin,'CDN-Credit' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,sum(camt)as camt,sum(samt) as samt,sum(csamt)as csamt  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNR_NT t3 On t3.cdnrid = t2.cdnrid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS t4 On t4.ntid = t3.ntid
							Inner Join TBL_GSTR1_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							And t3.ntty ='C'
							group by t1.gstin
							Union All
		   select t1.gstin,'CDNUR-Debit' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,sum(camt)as camt,sum(samt) as samt,sum(csamt)as csamt   
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							And t2.ntty ='D'
							group by t1.gstin
							Union All
		   select t1.gstin,'CDNUR-Credit' as ActionType ,sum(txval) as txval,sum(iamt)as iamt,sum(camt)as camt,sum(samt) as samt,sum(csamt)as csamt  
					   from TBL_GSTR1 t1
							Inner Join TBL_GSTR1_CDNUR t2 On t2.gstr1id = t1.gstr1id
							Inner Join TBL_GSTR1_CDNUR_ITMS t3 On t3.cdnurid = t2.cdnurid
							Inner Join TBL_GSTR1_CDNUR_ITMS_DET t4 On t4.itmsid = t3.itmsid
							Where t1.gstin in (Select Gstinno from #Gstins)
							And fp = @Period 
							And flag = 'U'
							And t2.ntty ='C'
							group by t1.gstin
			)as A
		group by A.gstin,A.ActionType,A.txval,A.iamt,A.camt,A.samt,A.csamt


		Select @Period as 'Period',* from #tbl_Summary order by gstin,[Action Type]

End