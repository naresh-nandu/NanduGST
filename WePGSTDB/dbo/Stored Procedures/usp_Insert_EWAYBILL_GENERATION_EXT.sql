
  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure to Insert EWAYBILL GENERATION Details  
      
Written by  : raja.m@wepindia.com  
  
Date  Who   Decription   
30/01/2018 Raja  Initial Version  
03/05/2018  Naresh      Adding new parameter as per new version  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Insert_EWAYBILL_GENERATION_EXT   
 */  
  
CREATE PROCEDURE [dbo].[usp_Insert_EWAYBILL_GENERATION_EXT]   
 @usergstin nvarchar(15),  
 @supplyType nvarchar(15),  
 @subSupplyType nvarchar(15),  
 @docType nvarchar(15),  
 @docNo nvarchar(50),  
 @docDate nvarchar(15),  
 @fromGstin nvarchar(15),  
 @fromTrdName nvarchar(100),  
 @fromAddr1 nvarchar(max),  
 @fromAddr2 nvarchar(max),  
 @fromPlace nvarchar(50),  
 @fromPinCode nvarchar(6),  
 @fromStateCode nvarchar(2),  
 @actfromStateCode nvarchar(2),  
 @toGstin nvarchar(15),  
 @toTrdName nvarchar(100),  
 @toAddr1 nvarchar(max),  
 @toAddr2 nvarchar(max),  
 @toPlace nvarchar(100),  
 @toPincode nvarchar(6),  
 @toStateCode nvarchar(2),  
 @acttoStateCode nvarchar(2),  
 @totalValue decimal(18,2),  
 @totalInvValue decimal(18,2),  
 @cgstValue decimal(18,2),  
 @sgstValue decimal(18,2),  
 @igstValue decimal(18,2),  
 @cessValue decimal(18,2),  
 @transMode int,  
 @transDistance nvarchar(50),  
 @transporterId nvarchar(15),  
 @transporterName nvarchar(100),  
 @transDocNo nvarchar(15),  
 @transDocDate nvarchar(10),  
 @vehicleNo nvarchar(20),  
 @vehicleType nvarchar(250),  
 @productName nvarchar(100),  
 @productDesc nvarchar(max),  
 @hsnCode nvarchar(50),  
 @quantity decimal(18,2),  
 @qtyUnit nvarchar(10),  
 @taxableAmount decimal(18,2),  
 @cgstRate decimal(18,2),  
 @sgstRate decimal(18,2),  
 @igstRate decimal(18,2),  
 @cessRate decimal(18,2),   
 @Referenceno varchar(50),  
 @createdby int,  
 @branchId int = Null,
 @invType nvarchar(10)=Null  
-- /*mssx*/ With Encryption   
as   
Begin  
  
 Set Nocount on  
  
 Declare @SourceType varchar(15)  
      
 Select @SourceType = 'Manual'  
   
 Insert into TBL_EXT_EWB_GENERATION  
 (supplyType, subSupplyType, docType, docNo, docDate, fromGstin, fromTrdName, fromAddr1, fromAddr2, fromPlace, fromPinCode, fromStateCode, actFromStateCode,   
  toGstin, toTrdName, toAddr1, toAddr2, toPlace, toPincode, toStateCode,actToStateCode,totalValue,totinvvalue, cgstValue, cessValue, transMode, transDistance, transporterId, transporterName,   
  transDocNo, transDocDate, vehicleNo, vehicleType ,productName, productDesc, hsnCode, quantity, qtyUnit, cgstRate, sgstRate, igstRate, cessRate, cessAdvol,  
  rowstatus, sourcetype, referenceno, createdby, createddate, taxableAmount, sgstValue, igstValue, usergstin,BranchId,InvoiceType)   
 Values  
 (@supplyType, @subSupplyType, @docType, @docNo, @docDate, @fromGstin, @fromTrdName, @fromAddr1, @fromAddr2, @fromPlace, @fromPinCode, @fromStateCode,@actfromStateCode,   
  @toGstin, @toTrdName, @toAddr1, @toAddr2, @toPlace, @toPincode, @toStateCode,@acttoStateCode, @totalValue,@totalInvValue, @cgstValue, @cessValue, @transMode, @transDistance, @transporterId, @transporterName,   
  @transDocNo, @transDocDate, @vehicleNo, @vehicleType, @productName, @productDesc, @hsnCode, @quantity, @qtyUnit, @cgstRate, @sgstRate, @igstRate, @cessRate, 0,  
  1, @sourcetype, @Referenceno, @createdby, DATEADD (mi, 330, GETDATE()), @taxableAmount, @sgstValue, @igstValue, @usergstin,@branchId,@invType)   
  
 exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo  
       
 Return 0  
  
End