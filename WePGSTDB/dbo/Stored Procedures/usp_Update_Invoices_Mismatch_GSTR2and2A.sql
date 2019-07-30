  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Update Invoices mismatch in GSTR2A  
      
Written by  : Karthik   
  
Date  Who   Decription   
07/31/2017 Karthik   Initial Version  
*/  
  
/* Sample Procedure Call  
  
exec usp_Update_Invoices_Mismatch_GSTR2and2A  'B2B','33GSPTN0802G1ZL','A',64,1,1
 */  
  
Create PROCEDURE [usp_Update_Invoices_Mismatch_GSTR2and2A]  
 @ActionType varchar(15),  
 @Gstin varchar(15),  
 @Activity varchar(2), --P: 'Pending'  
 @GSTR2invId int,  
 @GSTR2AinvId int,  
 @UserId int,  
 @CustId int  
  
-- /*mssx*/ With Encryption   
as   
Begin  
  
 Set Nocount on  
  
 if @ActionType = 'B2B'  
 Begin  
  
	  if @Activity = 'P'  
	  Begin  
  
		   Update TBL_GSTR2_B2B_INV   
		   Set flag='P'   
		   Where invid= @GSTR2invId
		      
		  Update TBL_GSTR2A_B2B_INV   
		   Set flag='P'   
		   Where invid= @GSTR2AinvId		
		   if @@ROWCOUNT>0  
		   Begin  
			Insert into TBL_RECONCILIATION_LOGS  
			(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus)   
			Values(@Gstin,@GSTR2invId ,'GSTR2',@ActionType, @Activity,'',@UserId,@CustId,GETDATE(),1)  
		   End  
  
	  End   
  
   
 End  
 else if @ActionType = 'CDNR' or @ActionType = 'CDN' 
 Begin  
    
	 if @Activity = 'P'  
	  Begin  
  
		   Update TBL_GSTR2_CDNR_NT   
		   Set flag='P'   
		   Where invid= @GSTR2invId  

		   Update TBL_GSTR2A_CDNR_NT   
		   Set flag='P'   
		   Where ntid= @GSTR2AinvId  
  
		   if @@ROWCOUNT>0  
		   Begin  
			Insert into TBL_RECONCILIATION_LOGS  
			(Gstin,InvoiceId,GSTRType,ActionType,Flag,Chksum,CreatedBy,CustId,CreatedDate,Rowstatus)   
			Values(@Gstin,@GSTR2invId ,'GSTR2',@ActionType, @Activity,'',@UserId,@CustId,GETDATE(),1)  
		   End

	  End 
	 End 
  
 Return 0  
End