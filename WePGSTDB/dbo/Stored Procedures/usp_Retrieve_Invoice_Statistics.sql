

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the statistics of invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who					Decription 
08/1/2017	Seshadri 			Initial Version
10/18/2017	Seshadri			Fine Tuned the Code
10/24/2017	Seshadri			Modified the code to retrieve the statistics
								based on the Tax Payer Gstin and Supplier Gstin
10/26/2017	Seshadri			Introduced SupplierName as the parameter
11/7/2017	Seshadri & Karthik	Introduced Reconciliation Statistics


*/

/* Sample Procedure Call

exec usp_Retrieve_Invoice_Statistics 1,1,'ALL','ALL','082017','ALL'

 */
 
CREATE PROCEDURE [usp_Retrieve_Invoice_Statistics]
	@CustId int,
	@UserId int,
	@Gstin varchar(15), -- 'ALL' Supported
	@SupplierName varchar(255), -- 'ALL' Supported
	@Ctin varchar(15), -- 'ALL' Supported
	@Fp varchar(10)

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @MatchedInvoices int,
			@MissingInGSTR2A int,
			@MissingInGSTR2 int,
			@MismatchInvoices int,
			@AcceptedInvoices int,
			@RejectedInvoices int,
			@PendingInvoices int,
			@ModifiedInvoices int


	Create Table #InvStatistics
	(
		StatName varchar(100),
		TotalCount int
	)

	Create Table #Ctins
	(
		Ctin varchar(15)
	)


	Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))
	Select @SupplierName = Ltrim(Rtrim(IsNull(@SupplierName,'')))
	Select @Ctin = Ltrim(Rtrim(IsNull(@Ctin,'')))
	Select @Fp = Ltrim(Rtrim(IsNull(@Fp,'')))

	Declare @AdjAmt decimal(18,2)

	Select @AdjAmt = Convert(dec(18,2),IsNull(ReconValueAdjust,0)) 
	From TBL_Settings_Reconciliation 
	Where gstinno = @Gstin
	And @Gstin <> 'ALL'

	if IsNull(@AdjAmt,0) <= 0
	Begin
		Set @AdjAmt = 1
	End

	-- Supplier Gstins

	/*
	if(@SupplierName = 'ALL' And @Ctin = 'ALL')
	Begin
		-- No Impact
	End
	*/
	if(@SupplierName = 'ALL' And @Ctin <> 'ALL')
	Begin
		Insert Into #Ctins
		Select @Ctin
	End
	else if(@SupplierName <> 'ALL' And @Ctin = 'ALL')
	Begin
		Select @Ctin = '' -- Deliberately Set the Value to Blank
		Insert Into #Ctins
		Select Ltrim(Rtrim(IsNull(gstinno,'')))
		From Tbl_Supplier
		Where suppliername = @SupplierName
		And  Ltrim(Rtrim(IsNull(gstinno,''))) <> ''
		And rowstatus = 1
	End
	if(@SupplierName <> 'ALL' And @Ctin <> 'ALL')
	Begin
		Insert Into #Ctins
		Select @Ctin
	End


	-- Matched Invoices

	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_B2B.ctin as ctin2, 
			TBL_GSTR2_B2B_INV.inum as inum2, 
			TBL_GSTR2_B2B_INV.val as val2, 
			TBL_GSTR2_B2B_INV.idt as idt2,
			TBL_GSTR2_B2B_INV.pos as pos2,
			TBL_GSTR2_B2B_INV.inv_typ as inv_typ2
	Into #TBL_Matched_Invs_T1  
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_B2B ON TBL_GSTR2.gstr2id = TBL_GSTR2_B2B.gstr2id 
	INNER JOIN TBL_GSTR2_B2B_INV ON TBL_GSTR2_B2B.b2bid = TBL_GSTR2_B2B_INV.b2bid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_B2B_INV.flag is NULL

	SELECT	TBL_GSTR2A.gstin as gstin2a,
			TBL_GSTR2A_B2B.ctin as ctin2a,
			TBL_GSTR2A_B2B_INV.inum as inum2a, 
			TBL_GSTR2A_B2B_INV.val as val2a, 
			TBL_GSTR2A_B2B_INV.idt as idt2a,
			TBL_GSTR2A_B2B_INV.pos as pos2a,
			TBL_GSTR2A_B2B_INV.inv_typ as inv_typ2a
	Into #TBL_Matched_Invs_T2  
	FROM TBL_GSTR2A
	Inner JOIN TBL_GSTR2A_B2B ON TBL_GSTR2A.gstr2aid = TBL_GSTR2A_B2B.gstr2aid 
	INNER JOIN TBL_GSTR2A_B2B_INV ON TBL_GSTR2A_B2B.b2bid = TBL_GSTR2A_B2B_INV.b2bid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_B2B_INV.flag is NULL

	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_CDNR.ctin as ctin2,
			TBL_GSTR2_CDNR_NT.inum as inum2, 
			TBL_GSTR2_CDNR_NT.idt as idt2,
			TBL_GSTR2_CDNR_NT.nt_num as nt_num2, 
			TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,  
			TBL_GSTR2_CDNR_NT.val as val2,
			TBL_GSTR2_CDNR_NT.ntty as ntty2,
			TBL_GSTR2_CDNR_NT.p_gst as p_gst2
	Into #TBL_Matched_Invs_T3  
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_CDNR ON TBL_GSTR2.gstr2id = TBL_GSTR2_CDNR.gstr2id 
	INNER JOIN TBL_GSTR2_CDNR_NT ON TBL_GSTR2_CDNR.cdnrid = TBL_GSTR2_CDNR_NT.cdnrid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_CDNR_NT.flag is NULL

	SELECT	TBL_GSTR2A.gstin as gstin2a, 
			TBL_GSTR2A_CDNR.ctin as ctin2a,
			TBL_GSTR2A_CDNR_NT.inum as inum2a, 
			TBL_GSTR2A_CDNR_NT.idt as idt2a,
			TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a, 
			TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,  
			TBL_GSTR2A_CDNR_NT.val as val2a,
			TBL_GSTR2A_CDNR_NT.ntty as ntty2a,
			TBL_GSTR2A_CDNR_NT.p_gst as p_gst2a
	Into #TBL_Matched_Invs_T4  
	FROM TBL_GSTR2A 
	Inner JOIN TBL_GSTR2A_CDNR ON TBL_GSTR2A_CDNR.gstr2aid = TBL_GSTR2A.gstr2aid 
	INNER JOIN TBL_GSTR2A_CDNR_NT ON TBL_GSTR2A_CDNR_NT.cdnrid = TBL_GSTR2A_CDNR.cdnrid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_CDNR_NT.flag is NULL
	

	Select @MatchedInvoices = SUM(Total) 
	FROM 
	(

		Select Count(*) as 'Total'
		From #TBL_Matched_Invs_T1 t1 
		Join #TBL_Matched_Invs_T2 t2
		ON	t1.gstin2=t2.gstin2a 
			And t1.ctin2=t2.ctin2a 
			And t1.inum2=t2.inum2a 
			And t1.idt2=t2.idt2a 
			And t1.pos2=t2.pos2a
			And t1.inv_typ2=t2.inv_typ2a
			And Not ( (t1.val2 < (t2.val2a - @AdjAmt))
					or
				  (t1.val2 > (t2.val2a + @AdjAmt))
				)
		Where t1.gstin2 is not NULL 
		And t2.gstin2a is not NULL
 
		UNION ALL
	
		Select Count(*) as 'Total' 
		From #TBL_Matched_Invs_T3 t1 
		Join  #TBL_Matched_Invs_T4 t2 
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2=t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And t1.nt_num2=t2.nt_num2a 
			And t1.nt_dt2=t2.nt_dt2a 
			And t1.ntty2=t2.ntty2a 
			And t1.p_gst2 = t2.p_gst2a 
			And Not ( (t1.val2 < (t2.val2a - @AdjAmt))
					or
				  (t1.val2 > (t2.val2a + @AdjAmt))
				)
		Where t1.gstin2 is not NULL 
		And t2.gstin2a is not NULL

	) t1

	-- MissingInGSTR2A Invoices

	SELECT	TBL_GSTR2.gstin as gstin2,
			TBL_GSTR2_B2B.ctin as ctin2, 
			TBL_GSTR2_B2B_INV.inum as inum2, 
			TBL_GSTR2_B2B_INV.idt as idt2,
			TBL_GSTR2_B2B_INV.val as val2, 
			TBL_GSTR2_B2B_INV.pos as pos2,
			TBL_GSTR2_B2B_INV.inv_typ as inv_typ2
	Into #TBL_MissingInGSTR2A_Invs_T1  
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_B2B ON TBL_GSTR2_B2B.gstr2id = TBL_GSTR2.gstr2id 
	INNER JOIN TBL_GSTR2_B2B_INV ON TBL_GSTR2_B2B_INV.b2bid = TBL_GSTR2_B2B.b2bid 
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_B2B_INV.flag is NULL

	SELECT	TBL_GSTR2A.gstin as gstin2a, 
			TBL_GSTR2A_B2B.ctin as ctin2a,
			TBL_GSTR2A_B2B_INV.inum as inum2a, 
			TBL_GSTR2A_B2B_INV.idt as idt2a,
			TBL_GSTR2A_B2B_INV.val as val2a, 
			TBL_GSTR2A_B2B_INV.pos as pos2a,
			TBL_GSTR2A_B2B_INV.inv_typ as inv_typ2a
	Into #TBL_MissingInGSTR2A_Invs_T2  
	FROM TBL_GSTR2A 
	Inner JOIN TBL_GSTR2A_B2B ON TBL_GSTR2A_B2B.gstr2aid = TBL_GSTR2A.gstr2aid 
	INNER JOIN TBL_GSTR2A_B2B_INV ON TBL_GSTR2A_B2B_INV.b2bid = TBL_GSTR2A_B2B.b2bid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_B2B.ctin In (Select Ctin From #Ctins) ))  

	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_CDNR.ctin as ctin2,
			TBL_GSTR2_CDNR_NT.inum as inum2, 
			TBL_GSTR2_CDNR_NT.idt as idt2,
			TBL_GSTR2_CDNR_NT.nt_num as nt_num2, 
			TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,  
			TBL_GSTR2_CDNR_NT.val as val2,
			TBL_GSTR2_CDNR_NT.ntty as ntty2,
			TBL_GSTR2_CDNR_NT.p_gst as p_gst2
	Into #TBL_MissingInGSTR2A_Invs_T3  
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_CDNR ON TBL_GSTR2_CDNR.gstr2id = TBL_GSTR2.gstr2id  
	INNER JOIN TBL_GSTR2_CDNR_NT ON TBL_GSTR2_CDNR_NT.cdnrid = TBL_GSTR2_CDNR.cdnrid 
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_CDNR_NT.flag is NULL
	
	SELECT	TBL_GSTR2A.gstin as gstin2a, 
			TBL_GSTR2A_CDNR.ctin as ctin2a,
			TBL_GSTR2A_CDNR_NT.inum as inum2a, 
			TBL_GSTR2A_CDNR_NT.idt as idt2a,
			TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a, 
			TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,  
			TBL_GSTR2A_CDNR_NT.val as val2a,
			TBL_GSTR2A_CDNR_NT.ntty as ntty2a,
			TBL_GSTR2A_CDNR_NT.p_gst as p_gst2a 
	Into #TBL_MissingInGSTR2A_Invs_T4 
	FROM TBL_GSTR2A 
	Inner JOIN TBL_GSTR2A_CDNR ON TBL_GSTR2A_CDNR.gstr2aid = TBL_GSTR2A.gstr2aid 
	INNER JOIN TBL_GSTR2A_CDNR_NT ON TBL_GSTR2A_CDNR_NT.cdnrid = TBL_GSTR2A_CDNR.cdnrid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_CDNR.ctin In (Select Ctin From #Ctins) ))  
	
	Select @MissingInGSTR2A = SUM(Total) 
	FROM
	(
		Select Count(*) as 'Total' 
		From  #TBL_MissingInGSTR2A_Invs_T1 t1
		Left Outer Join #TBL_MissingInGSTR2A_Invs_T2 t2
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2 =t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And t1.pos2=t2.pos2a 
			And t1.inv_typ2=t2.inv_typ2a
		Where t2.gstin2a is NULL
		
		UNION ALL

		Select Count(*) as 'Total' 
		From #TBL_MissingInGSTR2A_Invs_T3 t1
		Left Outer Join #TBL_MissingInGSTR2A_Invs_T4 t2
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2=t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And  t1.nt_num2=t2.nt_num2a 
			And t1.nt_dt2=t2.nt_dt2a 
			And t1.ntty2=t2.ntty2a 
			And t1.p_gst2 = t2.p_gst2a
		Where t2.gstin2a is NULL

	) t1

	-- MissingInGSTR2 Invoices

	SELECT	TBL_GSTR2.gstin as gstin2,
			TBL_GSTR2_B2B.ctin as ctin2,
			TBL_GSTR2_B2B_INV.inum as inum2,
			TBL_GSTR2_B2B_INV.idt as idt2,
			TBL_GSTR2_B2B_INV.val as val2,
			TBL_GSTR2_B2B_INV.pos as pos2,
			TBL_GSTR2_B2B_INV.inv_typ as inv_typ2
	Into #TBL_MissingInGSTR2_Invs_T1
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_B2B ON TBL_GSTR2.gstr2id = TBL_GSTR2_B2B.gstr2id 
	INNER JOIN TBL_GSTR2_B2B_INV ON TBL_GSTR2_B2B.b2bid = TBL_GSTR2_B2B_INV.b2bid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_B2B.ctin In (Select Ctin From #Ctins) ))  

	SELECT	TBL_GSTR2A.gstin as gstin2a,
			TBL_GSTR2A_B2B.ctin as ctin2a,
			TBL_GSTR2A_B2B_INV.inum as inum2a,
			TBL_GSTR2A_B2B_INV.idt as idt2a,
			TBL_GSTR2A_B2B_INV.val as val2a,
			TBL_GSTR2A_B2B_INV.pos as pos2a,
			TBL_GSTR2A_B2B_INV.inv_typ as inv_typ2a 
	Into #TBL_MissingInGSTR2_Invs_T2
	FROM TBL_GSTR2A
	Inner JOIN TBL_GSTR2A_B2B ON TBL_GSTR2A.gstr2aid = TBL_GSTR2A_B2B.gstr2aid 
	INNER JOIN TBL_GSTR2A_B2B_INV ON TBL_GSTR2A_B2B.b2bid = TBL_GSTR2A_B2B_INV.b2bid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_B2B_INV.flag is NULL

	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_CDNR.ctin as ctin2,
			TBL_GSTR2_CDNR_NT.inum as inum2, 
			TBL_GSTR2_CDNR_NT.idt as idt2,
			TBL_GSTR2_CDNR_NT.nt_num as nt_num2, 
			TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,  
			TBL_GSTR2_CDNR_NT.val as val2,
			TBL_GSTR2_CDNR_NT.ntty as ntty2,
			TBL_GSTR2_CDNR_NT.p_gst as p_gst2
	Into #TBL_MissingInGSTR2_Invs_T3
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_CDNR ON TBL_GSTR2.gstr2id = TBL_GSTR2_CDNR.gstr2id
	INNER JOIN TBL_GSTR2_CDNR_NT ON TBL_GSTR2_CDNR.cdnrid = TBL_GSTR2_CDNR_NT.cdnrid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_CDNR.ctin In (Select Ctin From #Ctins) ))  


	SELECT	TBL_GSTR2A.gstin as gstin2a, 
			TBL_GSTR2A_CDNR.ctin as ctin2a,
			TBL_GSTR2A_CDNR_NT.inum as inum2a, 
			TBL_GSTR2A_CDNR_NT.idt as idt2a,
			TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a, 
			TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,  
			TBL_GSTR2A_CDNR_NT.val as val2a,
			TBL_GSTR2A_CDNR_NT.ntty as ntty2a,
			TBL_GSTR2A_CDNR_NT.p_gst as p_gst2a
	Into #TBL_MissingInGSTR2_Invs_T4
	FROM TBL_GSTR2A 
	Inner JOIN TBL_GSTR2A_CDNR ON TBL_GSTR2A_CDNR.gstr2aid = TBL_GSTR2A.gstr2aid 
	INNER JOIN TBL_GSTR2A_CDNR_NT ON TBL_GSTR2A_CDNR_NT.cdnrid = TBL_GSTR2A_CDNR.cdnrid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_CDNR_NT.flag is NULL

	Select @MissingInGSTR2 = SUM(Total) 
	FROM
	(
		Select Count(*) as 'Total'
		From #TBL_MissingInGSTR2_Invs_T1 t1
		Right Outer Join  #TBL_MissingInGSTR2_Invs_T2 t2
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2 =t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And t1.pos2=t2.pos2a 
			And t1.inv_typ2=t2.inv_typ2a
		Where t1.gstin2 is NULL
 
		UNION ALL

		Select Count(*) as 'Total' 
		From  #TBL_MissingInGSTR2_Invs_T3 t1
		Right Outer Join #TBL_MissingInGSTR2_Invs_T4 t2
		ON  t1.gstin2 = t2.gstin2a 
			And t1.ctin2=t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And  t1.nt_num2=t2.nt_num2a 
			And t1.nt_dt2=t2.nt_dt2a 
			And t1.ntty2=t2.ntty2a 
			And t1.p_gst2 = t2.p_gst2a 
	
		Where t1.gstin2 is NULL

	) t1

	-- Mismatch Invoices

	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_B2B.ctin as ctin2,
			TBL_GSTR2_B2B_INV.inum as inum2,
			TBL_GSTR2_B2B_INV.idt as idt2,
			TBL_GSTR2_B2B_INV.val as val2,
			TBL_GSTR2_B2B_INV.pos as pos2,
			TBL_GSTR2_B2B_INV.inv_typ as inv_typ2 
	Into  #TBL_Mismatch_Invs_T1
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_B2B ON TBL_GSTR2.gstr2id = TBL_GSTR2_B2B.gstr2id 
	INNER JOIN TBL_GSTR2_B2B_INV ON TBL_GSTR2_B2B.b2bid = TBL_GSTR2_B2B_INV.b2bid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_B2B_INV.flag is NULL

	SELECT	TBL_GSTR2A.gstin as gstin2a,
			TBL_GSTR2A_B2B.ctin as ctin2a,
			TBL_GSTR2A_B2B_INV.inum as inum2a,
			TBL_GSTR2A_B2B_INV.idt as idt2a,
			TBL_GSTR2A_B2B_INV.val as val2a,
			TBL_GSTR2A_B2B_INV.pos as pos2a,
			TBL_GSTR2A_B2B_INV.inv_typ as inv_typ2a  
	Into  #TBL_Mismatch_Invs_T2
	FROM TBL_GSTR2A
	Inner JOIN TBL_GSTR2A_B2B ON TBL_GSTR2A.gstr2aid = TBL_GSTR2A_B2B.gstr2aid 
	INNER JOIN TBL_GSTR2A_B2B_INV ON TBL_GSTR2A_B2B.b2bid = TBL_GSTR2A_B2B_INV.b2bid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_B2B.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_B2B_INV.flag is NULL
	
	SELECT	TBL_GSTR2.gstin as gstin2, 
			TBL_GSTR2_CDNR.ctin as ctin2,
			TBL_GSTR2_CDNR_NT.inum as inum2, 
			TBL_GSTR2_CDNR_NT.idt as idt2,
			TBL_GSTR2_CDNR_NT.nt_num as nt_num2, 
			TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,  
			TBL_GSTR2_CDNR_NT.val as val2,
			TBL_GSTR2_CDNR_NT.ntty as ntty2,
			TBL_GSTR2_CDNR_NT.p_gst as p_gst2
	Into  #TBL_Mismatch_Invs_T3
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_CDNR ON TBL_GSTR2.gstr2id = TBL_GSTR2_CDNR.gstr2id 
	INNER JOIN TBL_GSTR2_CDNR_NT ON TBL_GSTR2_CDNR.cdnrid = TBL_GSTR2_CDNR_NT.cdnrid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2_CDNR_NT.flag is NULL 

	SELECT	TBL_GSTR2A.gstin as gstin2a, 
			TBL_GSTR2A_CDNR.ctin as ctin2a,
			TBL_GSTR2A_CDNR_NT.inum as inum2a, 
			TBL_GSTR2A_CDNR_NT.idt as idt2a,
			TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a, 
			TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,  
			TBL_GSTR2A_CDNR_NT.val as val2a,
			TBL_GSTR2A_CDNR_NT.ntty as ntty2a,
			TBL_GSTR2A_CDNR_NT.p_gst as p_gst2a
	Into  #TBL_Mismatch_Invs_T4
	FROM TBL_GSTR2A 
	Inner JOIN TBL_GSTR2A_CDNR ON TBL_GSTR2A_CDNR.gstr2aid = TBL_GSTR2A.gstr2aid 
	INNER JOIN TBL_GSTR2A_CDNR_NT ON TBL_GSTR2A_CDNR_NT.cdnrid = TBL_GSTR2A_CDNR.cdnrid
	Where TBL_GSTR2A.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2A.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2A_CDNR.ctin In (Select Ctin From #Ctins) ))  
	And TBL_GSTR2A_CDNR_NT.flag is NULL
	
	Select @MismatchInvoices = SUM(Total) 
	FROM 
	(
		Select Count(*) as 'Total'
		From #TBL_Mismatch_Invs_T1 t1
		Join #TBL_Mismatch_Invs_T2 t2
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2 =t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And t1.pos2=t2.pos2a 
			And t1.inv_typ2=t2.inv_typ2a
			And ( (t1.val2 < (t2.val2a - @AdjAmt))
					or
				  (t1.val2 > (t2.val2a + @AdjAmt))
				)
		Where t1.gstin2 is not NULL 
		And t2.gstin2a is not NULL
 
		UNION ALL
	
		Select Count(*) as 'Total' 
		From  #TBL_Mismatch_Invs_T3 t1
		Join #TBL_Mismatch_Invs_T4 t2
		ON	t1.gstin2 = t2.gstin2a 
			And t1.ctin2=t2.ctin2a 
			And t1.inum2 = t2.inum2a 
			And t1.idt2=t2.idt2a 
			And  t1.nt_num2 = t2.nt_num2a 
			And t1.nt_dt2 = t2.nt_dt2a 
			And t1.ntty2 = t2.ntty2a 
			And t1.p_gst2 = t2.p_gst2a 
			And ( (t1.val2 < (t2.val2a - @AdjAmt))
					or
				  (t1.val2 > (t2.val2a + @AdjAmt))
				)
		Where t1.gstin2 is not NULL 
		And t2.gstin2a is not NULL

	) t1

	-- Reconciliation Statistics

	SELECT flag,IsNull(Count(inum),0) as invcnt
	Into  #TBL_ReconStatistics 
	FROM TBL_GSTR2 
	INNER JOIN TBL_GSTR2_B2B ON TBL_GSTR2.gstr2id = TBL_GSTR2_B2B.gstr2id 
	INNER JOIN TBL_GSTR2_B2B_INV ON TBL_GSTR2_B2B.b2bid = TBL_GSTR2_B2B_INV.b2bid
	Where TBL_GSTR2.gstinid In (Select t1.gstinid 
								From UserAccess_GSTIN t1,
									 TBL_Cust_GSTIN t2	
								Where t1.gstinid = t2.gstinid
								And t1.custid = @CustId 
								And t1.UserId = @UserId
								And t1.rowstatus = 1
								And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))
	And TBL_GSTR2.fp = @Fp
	And ((@Ctin = 'ALL') Or (TBL_GSTR2_B2B.ctin In (Select Ctin From #Ctins) ))  
	And IsNull(TBL_GSTR2_B2B_INV.flag,'') In ('A','R','P','M')
	Group By flag

	Select @AcceptedInvoices = IsNull(invcnt,0) From #TBL_ReconStatistics Where flag = 'A'
	Select @RejectedInvoices = IsNull(invcnt,0) From #TBL_ReconStatistics Where flag = 'R'						
	Select @PendingInvoices = IsNull(invcnt,0) From #TBL_ReconStatistics Where flag = 'P'
	Select @ModifiedInvoices = IsNull(invcnt,0) From #TBL_ReconStatistics Where flag = 'M'

	Insert #InvStatistics values('Matched Invoices',IsNull(@MatchedInvoices,0))
	Insert #InvStatistics values('Missing In GSTR2A',IsNull(@MissingInGSTR2A,0))
	Insert #InvStatistics values('Missing In GSTR2',IsNull(@MissingInGSTR2,0))
	Insert #InvStatistics values('Mismatch Invoices',IsNull(@MismatchInvoices,0))
	Insert #InvStatistics values('Accepted Invoices',IsNull(@AcceptedInvoices,0))
	Insert #InvStatistics values('Rejected Invoices',IsNull(@RejectedInvoices,0))
	Insert #InvStatistics values('Pending Invoices', IsNull(@PendingInvoices,0))
	Insert #InvStatistics values('Modified Invoices',IsNull(@ModifiedInvoices,0))

	Select * from  #InvStatistics

	 -- Drop Temp Tables

	 Drop Table #InvStatistics
	 Drop Table #TBL_ReconStatistics 
	 Drop Table #Ctins

	Return 0

End