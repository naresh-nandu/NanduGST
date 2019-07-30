
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to iNSERT hsn DATA TO hsn tABLE
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com & nareshn@wepindia.com

Date		Who			                  Decription 
12/06/2017	Seshadri / Karthik			Initial Version
18/01/2018  Naresh                      Condition Checking and Introducing Output Parameters
*/

/* Sample Procedure Call

exec usp_Insert_HSN_Master '9954','Pure labour contract for beneficiary individual construction / Pradhan Mantri Awas Yojan/Housing for All Urban Mission',100.00,0.00,1,1,'OUTWARD'
 */
 
CREATE PROCEDURE [usp_Insert_HSN_Master]  

	@hsnCode   varchar(15),
	@hsnDescription   nvarchar(MAX),
	@unitPrice   decimal(18, 2),
	@rate   decimal(18, 2),
	@CustomerId   int,
	@UserId   int,
	@hsnType varchar(25),
	@RetVal int = NULL Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Username varchar(255)
	if not exists (select 1 from TBL_HSN_MASTER where hsnCode = @hsnCode and hsnDescription = @hsnDescription and CustomerId = @CustomerId and rate = @rate and rowstatus =1 and hsntype =@hsnType)
	Begin
	INSERT INTO TBL_HSN_MASTER (hsnCode,
								hsnDescription,
								unitPrice,
								rate,
								CustomerId,
								CreatedBy,
								CreatedDate,
								rowstatus,
								hsnType)Values
								(@hsnCode,
								@hsnDescription,
								@unitPrice,
								@rate,
								@CustomerId,
								@UserId,
								Getdate(),
								1,
								@hsnType)
		select @Username = username from userlist where userid = @UserId
		if @Username <> ''
		Begin
			declare @Msg varchar(max)
			set @Msg = 'New HSN : '+ @hsnCode +' details Added'
			exec [Ins_AuditLog] @UserId,@Username,@Msg,NULL
		End
		set @RetVal = 1 /*Success */
	End

 Else
 Begin
	set @RetVal = -1 /*Failure:-Already Exist */
 End

 select  @RetVal

End