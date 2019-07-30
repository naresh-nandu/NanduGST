

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert and Retrieve Update Transporter Details
				
Written by  : nareshn@wepindia.com 

Date		Who			Decription 
01/03/2018	NARESH		Initial Version

*/

/* Sample Procedure Call

exec usp_Insert_Retrieve_EWB_Update_Transporter
 */

 CREATE PROCEDURE [dbo].[usp_Insert_Retrieve_EWB_Update_Transporter]
    @UserGstin varchar(15)=Null,
	@EwbNo varchar(12)=Null,
	@TransId varchar(15)=Null,
	@UpdTransDate varchar(30)=Null,
	@Status bit,
	@ErrorCode Nvarchar(Max)=Null,
	@ErrorDesc Nvarchar(Max)=Null,
	@CreatedBy int,
	@CustId int,
	@Mode Varchar(1) /* I-Insert,R-Retrieve */
as 
Begin

	Set Nocount on
		
		If @Mode = 'I'
		 Begin
		  Insert Into TBL_EWB_UPDATE_TRANSPORTER
		  (UserGstin,EwbNo,TransId,UpdTransDate,[Status],ErrorCode,ErrorDesc,CreatedBy,CreatedDate,CustId)
		  Values
		  (@UserGstin,@EwbNo,@TransId,@UpdTransDate,@Status,@ErrorCode,@ErrorDesc,@CreatedBy,GETDATE(),@CustId)
		 End

		If @Mode = 'R'
		 Begin
		  Select * from TBL_EWB_UPDATE_TRANSPORTER Where CustId = @CustId
		 End
	Return 0
End