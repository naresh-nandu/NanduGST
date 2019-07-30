

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Count Total Invoice and Api Calling Statistics
				
Written by  : nareshn@wepindia.com 

Date		Who			 Decription 
20/02/2018	Naresh N	 Initial Version


*/

/* Sample Procedure Call


exec usp_Retrieve_API_Invoice_Statistics 1,1,'03-01-2018','03-20-2018','AAACJ0248C','36AAACJ0248C1Z4'
 */
 
Create PROCEDURE [usp_Retrieve_API_Invoice_Statistics]
    @CustId int,
	@UserId int,
	@FromDate varchar(20),
	@ToDate varchar(20),  
	@Pan varchar(10),
	@Gstin varchar(15)

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	--Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	--Select @Pan = Ltrim(Rtrim(IsNull(@Pan,'')))
	if @Pan = 'ALL'
	Begin
		set @Pan ='%'
	End
	if @Gstin = 'ALL'
	Begin
		set @Gstin ='%'
	End
	
	select panid,panno,custid,Companyname into #tblpan from tbl_cust_pan 
	where custid =@CustId and panno like @Pan and rowstatus =1
	
	select custid,gstinid,Gstinno,panno,GSTINUserName,Space(500) as CompanyName
	into #tblgstin from tbl_cust_gstin where custid =@CustId and panno like @Pan and gstinno like @Gstin and rowstatus =1


	Update #tblgstin 
	SET #tblgstin.CompanyName = t2.CompanyName 
	FROM #tblgstin t1,
			#tblpan t2 
	WHERE t1.custid = t2.custid 
	And t1.panno = t2.panno 


	select gstinid,gstr1id,fp into #gstr1ids from tbl_gstr1 where gstinid in (select gstinid from #tblgstin)
	
	; with dates_CTE (date) as (
        select convert(datetime,@FromDate)
    Union ALL
        select DATEADD(day, 1, date)
        from dates_CTE
        where date < convert(datetime,@ToDate)
) select 
    * into #tbl
from dates_CTE 

select distinct FORMAT([date], 'MM-yyyy') as [Month],space(10) as B2B,space(10) as B2CL,
space(10) as[EXP],space(10)as CDNR,space(10) as CDNUR into #tblInvoiceDetails from #tbl

SELECT  #tblInvoiceDetails.*, #tblgstin.* into #TBL_Invoice_Count_Details FROM   #tblInvoiceDetails CROSS JOIN #tblgstin

	Select gstinid,FORMAT(Createddate, 'MM-yyyy') as [Month],count(inum) as Cnt into #B2B
	from TBL_GSTR1_B2B_INV 
	where createddate >= convert(datetime,@FromDate) and Createddate <= convert(datetime,@ToDate) and gstr1id in (select gstr1id from #gstr1ids)
	Group by gstinid,FORMAT(Createddate, 'MM-yyyy')

	Select gstinid,FORMAT(Createddate, 'MM-yyyy') as [Month],count(inum) as Cnt into #B2CL
	from TBL_GSTR1_B2CL_INV 
	where createddate >= convert(datetime,@FromDate) and Createddate <= convert(datetime,@ToDate) and gstr1id in (select gstr1id from #gstr1ids)
	Group by gstinid,FORMAT(Createddate, 'MM-yyyy')

	Select gstinid,FORMAT(Createddate, 'MM-yyyy') as [Month],count(inum) as Cnt into #EXP
	from TBL_GSTR1_EXP_INV 
	where createddate >= convert(datetime,@FromDate) and Createddate <= convert(datetime,@ToDate) and gstr1id in (select gstr1id from #gstr1ids)
	Group by gstinid,FORMAT(Createddate, 'MM-yyyy')
	
	Select gstinid,FORMAT(Createddate, 'MM-yyyy') as [Month],count(nt_num) as Cnt into #CDNR
	from TBL_GSTR1_CDNR_NT 
	where createddate >= convert(datetime,@FromDate) and Createddate <= convert(datetime,@ToDate) and gstr1id in (select gstr1id from #gstr1ids)
	Group by gstinid,FORMAT(Createddate, 'MM-yyyy')

	Select gstinid,FORMAT(Createddate, 'MM-yyyy') as [Month],count(nt_num) as Cnt into #CDNUR
	from TBL_GSTR1_CDNUR 
	where createddate >= convert(datetime,@FromDate) and Createddate <= convert(datetime,@ToDate) and gstr1id in (select gstr1id from #gstr1ids)
	Group by gstinid,FORMAT(Createddate, 'MM-yyyy')


	Update #TBL_Invoice_Count_Details 
	SET #TBL_Invoice_Count_Details.B2B = t2.Cnt 
	FROM #TBL_Invoice_Count_Details t1,
			#B2B t2 
	WHERE t1.[Month] = t2.[Month] 
	--And t1.panno = t2.panno
	And t1.gstinid = t2.gstinid

	Update #TBL_Invoice_Count_Details 
	SET #TBL_Invoice_Count_Details.B2CL = t2.Cnt 
	FROM #TBL_Invoice_Count_Details t1,
			#B2CL t2 
	WHERE t1.[Month] = t2.[Month] 
	--And t1.panno = t2.panno
	And t1.gstinid = t2.gstinid

	Update #TBL_Invoice_Count_Details 
	SET #TBL_Invoice_Count_Details.[EXP] = t2.Cnt 
	FROM #TBL_Invoice_Count_Details t1,
			#EXP t2 
	WHERE t1.[Month] = t2.[Month] 
	--And t1.panno = t2.panno
	And t1.gstinid = t2.gstinid

	Update #TBL_Invoice_Count_Details 
	SET #TBL_Invoice_Count_Details.CDNR = t2.Cnt 
	FROM #TBL_Invoice_Count_Details t1,
			#CDNR t2 
	WHERE t1.[Month] = t2.[Month] 
	--And t1.panno = t2.panno
	And t1.gstinid = t2.gstinid

	Update #TBL_Invoice_Count_Details 
	SET #TBL_Invoice_Count_Details.CDNUR = t2.Cnt 
	FROM #TBL_Invoice_Count_Details t1,
			#CDNUR t2 
	WHERE t1.[Month] = t2.[Month] 
	--And t1.panno = t2.panno
	And t1.gstinid = t2.gstinid

	Update #TBL_Invoice_Count_Details  set B2B =0 where IsNull(B2B,'')=''
	Update #TBL_Invoice_Count_Details  set B2CL =0 where IsNull(B2CL,'')=''
	Update #TBL_Invoice_Count_Details  set [EXP]=0 where IsNull([EXP],'')=''
	Update #TBL_Invoice_Count_Details  set CDNR =0 where IsNull(CDNR,'')=''
	Update #TBL_Invoice_Count_Details  set CDNUR =0 where IsNull(CDNUR,'')=''

	Select CompanyName,GSTINUserName,Panno,Gstinno,[Month],B2B,B2CL,[EXP],CDNR,CDNUR from #TBL_Invoice_Count_Details 
	
drop table if exists #tblgstin
drop table if exists #tblpan
drop table if exists #gstr1ids
drop table if exists #tbl
drop table if exists #tblInvoiceDetails
drop table if exists #TBL_Invoice_Count_Details
drop table if exists #B2B
drop table if exists #B2CL
drop table if exists #EXP
drop table if exists #CDNR
drop table if exists #CDNUR
End