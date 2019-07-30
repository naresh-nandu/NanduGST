  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Retrieve the GSTR2 to update ITC details  
      
Written by  : Sheshadri.mk@wepdigital.com   
  
Date  Who   Decription   
10/29/2017 Karthik  Initial Version  
10/29/2017 Seshadri Fine tuned the code  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Retrieve_GSTR2_ITC_Updation   
  
  
 */  
   
CREATE PROCEDURE [usp_Retrieve_GSTR2_ITC_Updation]    
 @CustId int,  
 @UserId int,  
 @Gstin varchar(15), -- 'ALL' Supported  
 @SupplierName varchar(255), -- 'ALL' Supported  
 @Ctin varchar(15), -- 'ALL' Supported  
 @Fp varchar(10),  
 @ActionType varchar(15),  
 @Flag varchar(1) = Null  
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
 Select @Flag = Ltrim(Rtrim(IsNull(@Flag,'')))  
  
  
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
  
 Select  
  ROW_NUMBER() OVER(ORDER BY t3.invid desc) AS 'SNo',  
  t3.invid as invid,  
  ctin,  
  rchrg,  
  pos,  
  inv_typ,  
  inum,  
  idt,  
  val,  
  flag,  
  rt,  
  txval,    
  iamt,  
  camt,  
  samt,  
  csamt,   
  elg,  
  tx_i,  
  tx_c,  
  tx_s,  
  tx_cs  
 Into #TBL_GSTR2_B2B  
 From TBL_GSTR2 t1  
 Inner Join TBL_GSTR2_B2B t2 On t2.gstr2id = t1.gstr2id  
 Inner Join TBL_GSTR2_B2B_INV t3 On t3.b2bid = t2.b2bid  
 Inner Join TBL_GSTR2_B2B_INV_ITMS t4 On t4.invid = t3.invid  
 Inner Join TBL_GSTR2_B2B_INV_ITMS_DET t5 On t5.itmsid = t4.itmsid  
 Inner Join TBL_GSTR2_B2B_INV_ITMS_ITC t6 On t6.itmsid = t4.itmsid  
 Where t1.gstinid In (Select t1.gstinid   
       From UserAccess_GSTIN t1,  
       TBL_Cust_GSTIN t2   
       Where t1.gstinid = t2.gstinid  
       And t1.custid = @CustId   
       And t1.UserId = @UserId  
       And t1.rowstatus = 1  
       And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))  
 And t1.fp = @Fp  
 And ((@Ctin = 'ALL') Or (t2.ctin In (Select Ctin From #Ctins) ))  
 And IsNull(flag,'') = IsNull(@Flag,'')  
 Order By t3.invid Desc  
  
 Select * From #TBL_GSTR2_B2B  
    
 End  
 Else If @ActionType = 'CDNR' or  @ActionType = 'CDN'  
 Begin  
   
 Select  
  ROW_NUMBER() OVER(ORDER BY t3.invid desc) AS 'SNo',  
  t3.invid as invid,    
  ctin,  
  nt_num,  
  nt_dt,  
  ntty,  
  p_gst,  
  rsn,  
  val,  
  inum,  
  idt,  
  flag,  
  rt,  
  txval,    
  iamt,  
  camt,  
  samt,  
  csamt,   
  elg,  
  tx_i,  
  tx_c,  
  tx_s,  
  tx_cs  
 From TBL_GSTR2 t1  
 Inner Join TBL_GSTR2_CDNR t2 On t2.gstr2id = t1.gstr2id  
 Inner Join TBL_GSTR2_CDNR_NT t3 On t3.cdnrid = t2.cdnrid  
 Inner Join TBL_GSTR2_CDNR_NT_ITMS t4 On t4.invid = t3.invid  
 Inner Join TBL_GSTR2_CDNR_NT_ITMS_DET t5 On t5.itmsid = t4.itmsid  
 Inner Join TBL_GSTR2_CDNR_NT_ITMS_ITC t6 On t6.itmsid = t4.itmsid  
 Where t1.gstinid In (Select t1.gstinid   
       From UserAccess_GSTIN t1,  
         TBL_Cust_GSTIN t2   
       Where t1.gstinid = t2.gstinid  
       And t1.custid = @CustId   
       And t1.UserId = @UserId  
       And t1.rowstatus = 1  
       And ((@Gstin = 'ALL') Or (t2.gstinno = @Gstin)))  
 And t1.fp = @Fp  
 And ((@Ctin = 'ALL') Or (t2.ctin In (Select Ctin From #Ctins) ))  
 And IsNull(flag,'') = IsNull(@Flag,'')  
 Order By t2.cdnrid Desc  
  
 End  
 Else  
 Begin  
 Select 'No data found' as 'Message'  
 End  
  
End