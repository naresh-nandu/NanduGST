/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Insert and Update Otherparty Eway Bill 
      
Written by  : nareshn@wepindia.com   
  
Date         Who         Decription   
15/05/2018  NARESH     Initial Version  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Insert_Update_EWB_Transporter  
 */  
  
CREATE PROCEDURE [dbo].[usp_Insert_Update_EWB_Transporter]    
 @ewbNo varchar(12)=Null,  
 @ewbDate varchar(30)=Null,  
 @genGstin nvarchar(15)=NULL,
 @userGstin nvarchar(15)=NULL,
 @docNo Nvarchar(30)=Null,  
 @docDate Nvarchar(30)=Null,
 @delPinCode int=Null,
 @delStateCode int=Null,
 @delPlace nvarchar(50)=Null,
 @validUpto nvarchar(30)=Null,
 @extendedTimes nvarchar(30)=Null, 
 @status nvarchar(10)=Null,
 @rejectStatus nvarchar(10)=Null,
 @flag nvarchar(10),  
 @CreatedBy int,  
 @CustId int 

As   
BEGIN  
  Set Nocount on  
    
 BEGIN
  
  IF @flag = 'G'
    IF NOT EXISTS (SELECT 1 FROM TBL_EWB_Transporter 
                   WHERE ewbNo = @ewbNo
				   AND flag = 'G' AND CustId=@CustId)
       BEGIN
          INSERT INTO TBL_EWB_Transporter (userGstin,ewbNo,ewbDate,genGSTIN,docNo,docDate,delPinCode,
	      delStateCode,delPlace,validUpto,extendedTimes,[status],rejectStatus,flag,CreatedBy,CreatedDate,CustId)
		  Values(@userGstin,@ewbNo,@ewbDate,@genGstin,@docNo,@docDate,@delPinCode,@delStateCode,@delPlace,@validUpto,
		  @extendedTimes,@status,@rejectStatus,'G',@CreatedBy,GETDATE(),@CustId)
       END


     IF EXISTS (SELECT 1 FROM TBL_EWB_Transporter 
                   WHERE ewbNo = @ewbNo  
				   AND flag = 'G' AND CustId=@CustId)	
		BEGIN
		  Update TBL_EWB_Transporter set userGstin = @userGstin,genGSTIN = @genGstin,
		  docNo = @docNo,docDate = @docDate,delPinCode = @delPinCode,delStateCode=@delStateCode,
		  delPlace = @delPlace,validUpto = @validUpto,extendedTimes = @extendedTimes, [status] = @status,
		  rejectStatus = @rejectStatus,ModifiedBy = @CreatedBy,ModifiedDate = GETDATE() 
		  Where ewbNo = @ewbNo AND CustId = @CustId AND flag = 'G'
		 END


  ELSE IF @flag = 'D'
    IF NOT EXISTS (SELECT 1 FROM TBL_EWB_Transporter 
                   WHERE ewbNo = @ewbNo
				   AND flag = 'D' AND CustId=@CustId)
       BEGIN
          INSERT INTO TBL_EWB_Transporter (userGstin,ewbNo,ewbDate,genGSTIN,docNo,docDate,delPinCode,
	      delStateCode,delPlace,validUpto,extendedTimes,[status],rejectStatus,flag,CreatedBy,CreatedDate,CustId)
		  Values(@userGstin,@ewbNo,@ewbDate,@genGstin,@docNo,@docDate,@delPinCode,@delStateCode,@delPlace,@validUpto,
		  @extendedTimes,@status,@rejectStatus,'D',@CreatedBy,GETDATE(),@CustId)
       END


     IF EXISTS (SELECT 1 FROM TBL_EWB_Transporter 
                   WHERE ewbNo = @ewbNo  
				   AND flag = 'D' AND CustId=@CustId)	
		BEGIN
		  Update TBL_EWB_Transporter set userGstin = @userGstin,genGSTIN = @genGstin,
		  docNo = @docNo,docDate = @docDate,delPinCode = @delPinCode,delStateCode=@delStateCode,
		  delPlace = @delPlace,validUpto = @validUpto,extendedTimes = @extendedTimes, [status] = @status,
		  rejectStatus = @rejectStatus,ModifiedBy = @CreatedBy,ModifiedDate = GETDATE() 
		  Where ewbNo = @ewbNo AND CustId = @CustId AND flag = 'D'
		 END


  END
    
 Return 0  
END