/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Insert and Update Otherparty Eway Bill 
      
Written by  : nareshn@wepindia.com   
  
Date         Who         Decription   
13/05/2018  NARESH     Initial Version  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Insert_Update_EWB_OtherParty  
 */  
  
CREATE PROCEDURE [dbo].[usp_Insert_Update_EWB_OtherParty]    
 @ewbNo varchar(12)=Null,  
 @ewbDate varchar(30)=Null,  
 @genMode varchar(10)=Null,  
 @genGstin nvarchar(15)=NULL,  
 @docNo Nvarchar(30)=Null,  
 @docDate Nvarchar(30)=Null,
 @fromGstin nvarchar(15)=Null,
 @fromTrdName nvarchar(Max)=Null,
 @toGstin nvarchar(15)=Null,
 @toTrdName nvarchar(15)=Null,
 @totInvValue decimal(18,5)=Null,
 @hsnCode nvarchar(20)=Null,
 @hsnDesc nvarchar(Max)=Null,
 @status nvarchar(10)=Null,
 @rejectStatus nvarchar(10)=Null,  
 @CreatedBy int,  
 @CustId int 

As   
BEGIN  
  Set Nocount on  
    
 BEGIN
   Declare @ewbId int
   IF NOT EXISTS (SELECT 1 FROM TBL_EWB_GENERATION 
                   WHERE ewayBillNo = @ewbNo
				   AND flag = 'O' AND CustId=@CustId)
      BEGIN
         INSERT INTO TBL_EWB_GENERATION (ewayBillNo,ewayBillDate,genMode,userGSTIN,docNo,docDate,fromGstin,
	     fromTrdName,toGstin,toTrdName,totinvvalue,flag,[status],rejectStatus,CreatedBy,CreatedDate,CustId)
		 Values(@ewbNo,@ewbDate,@genMode,@genGstin,@docNo,convert(varchar,convert(date, @docDate),103),@fromGstin,@fromTrdName,@toGstin,@toTrdName,
		 @totInvValue,'O',@status,@rejectStatus,@CreatedBy,GETDATE(),@CustId)

	       IF @ewbId is Not Null
		    BEGIN
	         Select @ewbId= SCOPE_IDENTITY()

	         Insert INTO TBL_EWB_GENERATION_ITMS(ewbid,hsnCode,productDesc,createdby,createddate,CustId)
	         Values(@ewbId,@hsnCode,@hsnDesc,@CreatedBy,GETDATE(),@CustId)
            END
		   --IF EXISTS (SELECT 1 FROM TBL_EWB_GENERATION 
					--   WHERE ewayBillNo = @ewbNo
					--   AND (flag is Null OR flag = ''))
					--   BEGIN
					--   Update TBL_EWB_GENERATION set [status] = @status,rejectStatus = @rejectStatus,ModifiedBy = @CreatedBy,
					--   ModifiedDate = GETDATE() Where ewayBillNo = @ewbNo AND (flag is Null OR flag = '') AND CustId = @CustId
					--   END
     END


   IF EXISTS (SELECT 1 FROM TBL_EWB_GENERATION 
                   WHERE ewayBillNo = @ewbNo
				   AND flag = 'O' AND CustId=@CustId)	
			BEGIN
			  Update TBL_EWB_GENERATION set [status] = @status,rejectStatus = @rejectStatus,ModifiedBy = @CreatedBy,
			  ModifiedDate = GETDATE() Where ewayBillNo = @ewbNo AND CustId = @CustId AND flag = 'O'
			END

   END
    
 Return 0  
END