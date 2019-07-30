  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Retrieve Invoices missing in GSTR2  
      
Written by  : Sheshadri.mk@wepdigital.com   
  
Date  Who   Decription   
07/31/2017 Seshadri   Initial Version  
10/18/2017 Seshadri Fine Tuned the Code  
10/24/2017 Seshadri Fixed the issue of mismatch in the total count displayed and   
      number of detail records displayed  
10/24/2017 Seshadri Modified the code to retrieve the statistics  
      based on the Tax Payer Gstin and Supplier Gstin  
10/26/2017 Seshadri Introduced SupplierName as the parameter  
10/30/2017 Seshadri Modified the code in CDNR section with repect to Value Column  
10/31/2017 Seshadri Introduced Note Num and Note Date in the result set  
11/07/2017  Karthik  Selecting required formatted columns from #tables for excel export  
11/07/2017  Karthik  Fixed the issue of POS in CDNR 
05/10/2018  Karthik   Included Supplier name 
  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Retrieve_Invoices_MissingInGSTR2  1,1,'ALL','ALL','ALL','052018','B2B'
 */  
  
CREATE PROCEDURE [dbo].[usp_Retrieve_Invoices_MissingInGSTR2]  
 @CustId int,  
 @UserId int,  
 @Gstin varchar(15), -- 'ALL' Supported  
 @SupplierName varchar(255), -- 'ALL' Supported  
 @Ctin varchar(15), -- 'ALL' Supported  
 @Fp varchar(10),  
 @ActionType varchar(15)  
    
-- /*mssx*/ With Encryption   
as   
Begin  
  
 Set Nocount on  
  
 Create Table #Ctins  
 (  
  Ctin varchar(15)  
 )  
  
 Select @Gstin = Ltrim(Rtrim(IsNull(@Gstin,'')))  
 Select @SupplierName = Ltrim(Rtrim(IsNull(@SupplierName,'')))  
 Select @Ctin = Ltrim(Rtrim(IsNull(@Ctin,'')))  
 Select @Fp = Ltrim(Rtrim(IsNull(@Fp,'')))  
 Select @ActionType = Ltrim(Rtrim(IsNull(@ActionType,'')))  
  
  
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
 else if(@SupplierName <> 'ALL' And @Ctin <> 'ALL')  
 Begin  
  Insert Into #Ctins  
  Select @Ctin  
 End  
  
 if @ActionType = 'B2B'  
 Begin  
  
  SELECT TBL_GSTR2.gstin as gstin2,  
    TBL_GSTR2_B2B.ctin as ctin2,  
    TBL_GSTR2_B2B_INV.invid as invid2,   
    TBL_GSTR2_B2B_INV.inum as inum2,   
    TBL_GSTR2_B2B_INV.idt as idt2,  
    TBL_GSTR2_B2B_INV.val as val2,  
    TBL_GSTR2_B2B_INV.pos as pos2,  
    TBL_GSTR2_B2B_INV.inv_typ as inv_typ2   
  Into #TBL_GSTR2_B2B   
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
  
  SELECT TBL_GSTR2A.gstin as gstin2a,   
    TBL_GSTR2A_B2B.ctin as ctin2a,   
    TBL_GSTR2A_B2B_INV.invid as invid2a,   
    TBL_GSTR2A_B2B_INV.inum as inum2a,   
    TBL_GSTR2A_B2B_INV.idt as idt2a,  
    TBL_GSTR2A_B2B_INV.val as val2a,  
    TBL_GSTR2A_B2B_INV.pos as pos2a,  
    TBL_GSTR2A_B2B_INV.inv_typ as inv_typ2a  
  Into #TBL_GSTR2A_B2B   
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
   
  Select  ROW_NUMBER() OVER(ORDER BY invid2a desc ) as 'SNo',   
    invid2a as invid,  
    Concat (invid2a,',',ctin2a,',',inum2a,',',idt2a,',',val2a) as ctin_inum,  
    ctin2a as ctin,      
    Space(500) as 'CtinName',  
    inum2a as inum,  
    idt2a as idt,  
    val2a as val,  
    pos2a as pos,  
    inv_typ2a as inv_typ   
  Into #TBL_GSTR2_B2B_M    
  From  #TBL_GSTR2_B2B  t1  
  Right Outer Join #TBL_GSTR2A_B2B t2  
  ON t1.gstin2 = t2.gstin2a   
   And t1.ctin2 =t2.ctin2a   
   And t1.inum2 = t2.inum2a   
   And t1.idt2=t2.idt2a   
   And t1.pos2=t2.pos2a   
   And t1.inv_typ2=t2.inv_typ2a  
  Where t1.gstin2 is NULL  
  Order By invid2a desc  
  
  Update #TBL_GSTR2_B2B_M   
  SET #TBL_GSTR2_B2B_M.CtinName = t2.SupplierName   
  FROM #TBL_GSTR2_B2B_M t1,  
    TBL_Supplier t2   
  WHERE t1.ctin = t2.gstinno  
  and t2.rowstatus =1  
  And t1.CtinName =''  
  
  
  Update #TBL_GSTR2_B2B_M   
  SET #TBL_GSTR2_B2B_M.CtinName = t2.LegalName   
  FROM #TBL_GSTR2_B2B_M t1,  
    TBL_ValidGSTIN t2   
  WHERE t1.ctin = t2.ValidGstin  
  And t1.CtinName =''  
  
  Select * from #TBL_GSTR2_B2B_M  
     
  Select ROW_NUMBER() OVER(ORDER BY #TBL_GSTR2_B2B_M.inum ASC) as 'S.No', ctin as 'Supplier GSTIN',CtinName as 'Supplier Name', inum as 'Invoice No-2', idt as 'Invoice Date', val as 'Invoice Value',pos as 'Place Of Supply',inv_typ as 'Invoice Type' from #TBL_GSTR2_B2B_M  
  
   
 End  
 else if @ActionType = 'CDNR' or  @ActionType = 'CDN'  
 Begin  
  
  SELECT TBL_GSTR2.gstin as gstin2,   
    TBL_GSTR2_CDNR.ctin as ctin2,   
    TBL_GSTR2_CDNR_NT.invid as invid2,  
    TBL_GSTR2_CDNR_NT.inum as inum2,   
    TBL_GSTR2_CDNR_NT.idt as idt2,   
    TBL_GSTR2_CDNR_NT.nt_num as nt_num2,   
    TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,    
    TBL_GSTR2_CDNR_NT.val as val2   
  Into #TBL_GSTR2_CDNR   
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
  
  SELECT TBL_GSTR2A.gstin as gstin2a,   
    TBL_GSTR2A_CDNR.ctin as ctin2a,   
    TBL_GSTR2A_CDNR_NT.ntid as invid2a,  
    TBL_GSTR2A_CDNR_NT.inum as inum2a,   
    TBL_GSTR2A_CDNR_NT.idt as idt2a,  
    TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a,   
    TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,    
    TBL_GSTR2A_CDNR_NT.val as val2a  
  Into #TBL_GSTR2A_CDNR   
  FROM TBL_GSTR2A  
  Inner JOIN TBL_GSTR2A_CDNR ON TBL_GSTR2A.gstr2aid = TBL_GSTR2A_CDNR.gstr2Aid   
  INNER JOIN TBL_GSTR2A_CDNR_NT ON TBL_GSTR2A_CDNR.cdnrid = TBL_GSTR2A_CDNR_NT.cdnrid  
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
  
  Select  ROW_NUMBER() OVER(ORDER BY invid2a desc ) as 'SNo',   
    invid2a as invid,  
    Concat (invid2a,',',ctin2a,',',nt_num2a,',',nt_dt2a,',',val2a) as ctin_inum,   
    ctin2a as ctin,   
    Space(500) as 'CtinName',  
    inum2a as inum,  
    idt2a as idt,  
    nt_num2a as nt_num,  
    nt_dt2a as nt_dt,  
    val2a as val  
  Into #TBL_GSTR2_CDNR_M     
  From  #TBL_GSTR2_CDNR  t1  
  Right Outer Join #TBL_GSTR2A_CDNR  t2  
  ON t1.gstin2 = t2.gstin2a   
   And t1.ctin2 =t2.ctin2a   
   And t1.nt_num2 = t2.nt_num2a   
   And t1.nt_dt2=t2.nt_dt2a   
  Where t1.gstin2 is NULL  
  Order By invid2a desc  
  
  Update #TBL_GSTR2_CDNR_M   
  SET #TBL_GSTR2_CDNR_M.CtinName = t2.SupplierName   
  FROM #TBL_GSTR2_CDNR_M t1,  
    TBL_Supplier t2   
  WHERE t1.ctin = t2.gstinno  
  and t2.rowstatus =1  
  And t1.CtinName =''  
  
  
  Update #TBL_GSTR2_CDNR_M   
  SET #TBL_GSTR2_CDNR_M.CtinName = t2.LegalName   
  FROM #TBL_GSTR2_CDNR_M t1,  
    TBL_ValidGSTIN t2   
  WHERE t1.ctin = t2.ValidGstin  
  And t1.CtinName =''  
    
  Select * from #TBL_GSTR2_CDNR_M  
  
    Select ROW_NUMBER() OVER(ORDER BY #TBL_GSTR2_CDNR_M.inum ASC) as 'S.No', ctin as 'Supplier GSTIN',CtinName as 'Supplier Name',nt_num as 'Note No',nt_dt as 'Note Date', inum as 'Invoice No', idt as 'Invoice Date', val as 'Invoice Value' from #TBL_GSTR2_CDNR_M  
  
  
 End  
 else  
 Begin  
  Select 'No data found' as 'Message'  
 End  
  
 -- Drop Temp Tables  
  
 Drop Table #Ctins  
  
  
 Return 0  
End