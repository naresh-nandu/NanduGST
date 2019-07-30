  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Retrieve Matched Invoices  
      
Written by  : Sheshadri.mk@wepdigital.com   
  
Date  Who   Decription   
07/31/2017 Seshadri   Initial Version  
10/18/2017 Seshadri Fine Tuned the Code  
10/24/2017 Seshadri Modified the code to retrieve the statistics  
      based on the Tax Payer Gstin and Supplier Gstin  
10/26/2017 Seshadri Introduced SupplierName as the parameter  
10/27/2017 Seshadri Introduced invid2a in the concatentation string  
10/30/2017 Seshadri Inlcluded Note No & Date in result set of CDNR  
10/31/2017 Seshadri Fixed crashing issue in case of CDNR  
10/31/2017 Seshadri Introduced Note Num and Note Date in the result set  
11/07/2017  Karthik  Selecting required formatted columns from #tables for excel export
05/10/2018  Karthik  Included Unit code,Receivername,received date  
05/18/2018  MUSKAN      Changed Receieved Date Name from Reciep Date to RECEIVED DATE 
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Retrieve_Matched_Invoices  
 */  
  
CREATE PROCEDURE [dbo].[usp_Retrieve_Matched_Invoices]  
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
      TBL_GSTR2_B2B_INV.rchrg as rchrg2,        
      TBL_GSTR2_B2B_INV.inv_typ as inv_typ2,   
      TBL_GSTR2_B2B_INV.UnitCode as unit_code2,  
      TBL_GSTR2_B2B_INV.ReceivedBy as rec_by2,  
      TBL_GSTR2_B2B_INV.ReceivedDate as rec_dt2   
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
  And flag is NULL   
  
  
  SELECT TBL_GSTR2A.gstin as gstin2a,   
      TBL_GSTR2A_B2B.ctin as ctin2a,   
      TBL_GSTR2A_B2B_INV.invid as invid2a,   
      TBL_GSTR2A_B2B_INV.inum as inum2a,   
      TBL_GSTR2A_B2B_INV.idt as idt2a,  
      TBL_GSTR2A_B2B_INV.val as val2a,   
      TBL_GSTR2A_B2B_INV.pos as pos2a,  
      TBL_GSTR2A_B2B_INV.rchrg as rchrg2a,  
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
  And flag is NULL   
  
  
  -- Matched B2B Invoices  
   
  Select ROW_NUMBER() OVER(ORDER BY #TBL_GSTR2_B2B.gstin2 ASC) as 'S.No',   
    invid2,  
    Concat (invid2,',',ctin2,',',inum2,',',idt2,',',val2,',',val2a,',',invid2a) as ctin_inum,    
    ctin2,  
    inum2,   
    idt2,  
    val2,   
                unit_code2,  
                rec_by2,  
                rec_dt2,      
    Space(500) as 'CtinName',  
    invid2a,  
    ctin2a,  
    inum2a,  
    idt2a,  
    val2a  
  Into #TBL_Matched_B2B_Invs  
  From #TBL_GSTR2_B2B   
  Join #TBL_GSTR2A_B2B   
   On gstin2 = gstin2a   
   And ctin2 = ctin2a   
   And inum2 = inum2a   
   And idt2 = idt2a   
   And pos2 = pos2a   
   And inv_typ2 = inv_typ2a  
   And Not ( (val2 < (val2a - @AdjAmt))  
      or  
       (val2 > (val2a + @AdjAmt))  
     )  
  Where ((@Gstin = 'ALL') Or (gstin2 = @Gstin))  
  
  
  Update #TBL_Matched_B2B_Invs   
  SET #TBL_Matched_B2B_Invs.CtinName = t2.SupplierName   
  FROM #TBL_Matched_B2B_Invs t1,  
    TBL_Supplier t2   
  WHERE t1.ctin2 = t2.gstinno  
  and t2.rowstatus =1  
  And t1.CtinName =''  
  
  
  Update #TBL_Matched_B2B_Invs   
  SET #TBL_Matched_B2B_Invs.CtinName = t2.LegalName   
  FROM #TBL_Matched_B2B_Invs t1,  
    TBL_ValidGSTIN t2   
  WHERE t1.ctin2 = t2.ValidGstin  
  And t1.CtinName =''  
  Select * from #TBL_Matched_B2B_Invs  
  
  Select ROW_NUMBER() OVER(ORDER BY #TBL_Matched_B2B_Invs.inum2 ASC) as 'S.No', ctin2 as 'Supplier GSTIN',CtinName as 'Supplier Name', inum2 as 'Invoice No', idt2 as 'Invoice Date', val2 as 'Invoice Value',unit_code2 as 'UNIT CODE(Garden Code)', rec_by2 as 'USER ID', rec_dt2 as 'RECEIVED DATE' from #TBL_Matched_B2B_Invs  
  
   
   
 End  
 else if @ActionType = 'CDNR' or @ActionType = 'CDN'  
 Begin  
  
  SELECT TBL_GSTR2.gstin as gstin2,   
    TBL_GSTR2_CDNR.ctin as ctin2,  
    TBL_GSTR2_CDNR_NT.invid as invid2,  
    TBL_GSTR2_CDNR_NT.inum as inum2,   
    TBL_GSTR2_CDNR_NT.idt as idt2,  
    TBL_GSTR2_CDNR_NT.nt_num as nt_num2,   
    TBL_GSTR2_CDNR_NT.nt_dt as nt_dt2,    
    TBL_GSTR2_CDNR_NT.val as val2,  
    TBL_GSTR2_CDNR_NT.ntty as ntty2,  
    TBL_GSTR2_CDNR_NT.p_gst as p_gst2,  
    TBL_GSTR2_CDNR_NT.UnitCode as unit_code2,  
    TBL_GSTR2_CDNR_NT.ReceivedBy as rec_by2,  
    TBL_GSTR2_CDNR_NT.ReceivedDate as rec_dt2  
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
  And flag is NULL   
    
  
  SELECT TBL_GSTR2A.gstin as gstin2a,   
    TBL_GSTR2A_CDNR.ctin as ctin2a,  
    TBL_GSTR2A_CDNR_NT.ntid as invid2a,   
    TBL_GSTR2A_CDNR_NT.inum as inum2a,   
    TBL_GSTR2A_CDNR_NT.idt as idt2a,  
    TBL_GSTR2A_CDNR_NT.nt_num as nt_num2a,   
    TBL_GSTR2A_CDNR_NT.nt_dt as nt_dt2a,    
    TBL_GSTR2A_CDNR_NT.val as val2a,  
    TBL_GSTR2A_CDNR_NT.ntty as ntty2a,  
    TBL_GSTR2A_CDNR_NT.p_gst as p_gst2a  
  Into #TBL_GSTR2A_CDNR     
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
  And flag is NULL   
  
  
  Select ROW_NUMBER() OVER(ORDER BY #TBL_GSTR2_CDNR.gstin2 ASC) as 'S.No',   
    invid2,  
    Concat (invid2,',',ctin2,',',nt_num2,',',nt_dt2,',',val2,',',val2a,',',invid2a) as ctin_inum,    
    ctin2,  
    nt_num2,  
    nt_dt2,  
    inum2,   
    idt2,  
    val2,  
    unit_code2,  
    rec_by2,  
    rec_dt2,   
    Space(500) as 'CtinName',  
    invid2a,  
    ctin2a,  
    nt_num2a,  
    nt_dt2a,  
    inum2a,  
    idt2a,  
    val2a  
  Into #TBL_Matched_CDNR_Invs  
  From #TBL_GSTR2_CDNR   
  Join #TBL_GSTR2A_CDNR   
   On gstin2 = gstin2a   
   And ctin2 = ctin2a   
   And inum2 = inum2a   
   And idt2 = idt2a  
   And nt_num2 = nt_num2a   
   And nt_dt2 = nt_dt2a   
   And ntty2 = ntty2a   
   And p_gst2 = p_gst2a   
   And Not ( (val2 < (val2a - @AdjAmt))  
      or  
       (val2 > (val2a + @AdjAmt))  
     )  
  Where ((@Gstin = 'ALL') Or (gstin2 = @Gstin))  
  
  Update #TBL_Matched_CDNR_Invs   
  SET #TBL_Matched_CDNR_Invs.CtinName = t2.SupplierName   
  FROM #TBL_Matched_CDNR_Invs t1,  
    TBL_Supplier t2   
  WHERE t1.ctin2 = t2.gstinno  
  and t2.rowstatus =1  
  And t1.CtinName =''  
  
  
  Update #TBL_Matched_CDNR_Invs   
  SET #TBL_Matched_CDNR_Invs.CtinName = t2.LegalName   
  FROM #TBL_Matched_CDNR_Invs t1,  
    TBL_ValidGSTIN t2   
  WHERE t1.ctin2 = t2.ValidGstin  
  And t1.CtinName =''  
  
  Select * from #TBL_Matched_CDNR_Invs    
  
  Select ROW_NUMBER() OVER(ORDER BY #TBL_Matched_CDNR_Invs.inum2 ASC) as 'S.No', ctin2 as 'Supplier GSTIN',CtinName as 'Supplier Name',nt_num2 as 'Note No',nt_dt2 as 'Note Date', inum2 as 'Invoice No', idt2 as 'Invoice Date', val2 as 'Invoice Value', 
  unit_code2 as 'UNIT CODE(Garden Code)', rec_by2 as 'USER ID', rec_dt2 as 'RECEIVED DATE'  from #TBL_Matched_CDNR_Invs  
  
   
   
 End  
  
 -- Drop Temp Tables  
  
 Drop Table #Ctins  
  
  
  
 Return 0  
End