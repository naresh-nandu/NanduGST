  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Insert EWAYBILL GENERATION Details  
      
Written by  : raja.m@wepindia.com  
  
Date  Who   Decription   
31/01/2018 Raja  Initial Version  
03/05/2018  Naresh N    Added userGSTIN as per new version 
16/05/2018  Naresh N    Added Branch Id  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Insert_EWAYBILL_GENERATION_CONSOLIDATED_EXT  
 */  
  
CREATE PROCEDURE [dbo].[usp_Insert_EWAYBILL_GENERATION_CONSOLIDATED_EXT]    
 @userGSTIN nvarchar(15),  
 @vehicleNo nvarchar(20),  
 @fromPlace nvarchar(50),  
 @transMode int,  
 @transDocNo nvarchar(15),  
 @transDocDate nvarchar(10),   
 @fromState nvarchar(2),  
 @ewbNo nvarchar(20),     
 @Referenceno varchar(50),  
 @createdby int,
 @BranchId int=Null  
   
-- /*mssx*/ With Encryption   
as   
Begin  
  
 Set Nocount on  
  
 Declare @SourceType varchar(15)  
      
 Select @SourceType = 'Manual'  
   
 Insert into TBL_EXT_EWB_GENERATION_CONSOLIDATED  
 (userGSTIN,vehicleNo, fromPlace, transMode, transDocNo, transDocDate, fromState, ewbNo, rowstatus, sourcetype, referenceno, createdby, createddate,BranchId)   
 Values  
 (@userGSTIN,@vehicleNo, @fromPlace, @transMode, @transDocNo, @transDocDate, @fromState, @ewbNo, 1, @sourcetype, @Referenceno, @createdby, DATEADD (mi, 330, GETDATE()),@BranchId)   
       
 exec usp_Push_EWB_CONSOLIDATED_EXT_SA @SourceType, @ReferenceNo  
  
 Return 0  
  
End