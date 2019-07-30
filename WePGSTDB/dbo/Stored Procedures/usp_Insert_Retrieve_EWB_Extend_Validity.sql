

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert and Retrieve Extend Validity of Eway Bill
				
Written by  : nareshn@wepindia.com 

Date		Who			Decription 
01/03/2018	NARESH		Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_Retrieve_EWB_Extend_Validity 1,1,'R'
 */

 CREATE PROCEDURE [dbo].[usp_Insert_Retrieve_EWB_Extend_Validity] 
	@CreatedBy int,
	@CustId int,
	@Mode Varchar(1), /* I-Insert,R-Retrieve */
    @UserGstin varchar(15)=Null,
	@EwbNo varchar(12)=Null,
	@VehicleNo varchar(20)=Null,
	@FromPlace varchar(50)=Null,
	@FromStateCode int=Null,
	@RemainingDistance nvarchar(20)=Null,
	@TransDocNo varchar(20)=Null,
	@TransDocDate varchar(30)=Null,
	@TransMode varchar(1)=Null,
	@ExtnRsnCode varchar(1)=Null,
	@ExtnRmrk Varchar(50)=Null,
	@Status bit=Null,
	@ErrorCode Nvarchar(Max)=Null,
	@ErrorDesc Nvarchar(Max)=Null
as 
Begin

	Set Nocount on
		
		If @Mode = 'I'
		 Begin

		 if @FromStateCode = 0
		 Begin
			 set @FromStateCode = NULL
		 End

		  Insert Into TBL_EWB_UPDATE_EXTEND_VALIDITY
		  (UserGstin,EwbNo,VehicleNo,FromPlace,FromStateCode,RemainingDistance,TransDocNo,TransDocDate,TransMode,[Status],ErrorCode,ErrorDesc,CreatedBy,CreatedDate,CustId)
		  Values
		  (@UserGstin,@EwbNo,@VehicleNo,@FromPlace,@FromStateCode,@RemainingDistance,@TransDocNo,@TransDocDate,@TransMode,@Status,@ErrorCode,@ErrorDesc,@CreatedBy,GETDATE(),@CustId)
		 End

		If @Mode = 'R'
		 Begin
		  Select * from TBL_EWB_UPDATE_EXTEND_VALIDITY Where CustId = @CustId
		 End
	Return 0
End