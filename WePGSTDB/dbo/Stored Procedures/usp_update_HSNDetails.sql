
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update hsn DATA TO hsn tABLE
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com & nareshn@wepindia.com

Date		Who			                  Decription 
12/06/2017	Seshadri / Karthik			Initial Version
18/01/2018  Naresh                      Condition Checking and Introducing Output Parameter and Input parameter,
*/

/* Sample Procedure Call

exec usp_update_HSNDetails 
 */
 
CREATE PROCEDURE [usp_update_HSNDetails] 
		@hsnid		int,
		@action	nvarchar(50),
		@hsncode varchar(15) ,
		@hsndescription nvarchar(max) ,
		@unitprice decimal(18,2) ,
		@rate decimal(18,2) ,
		@CustomerId   int,
		@userId    int ,
		@RetVal int = NULL Out	
AS
BEGIN
Declare @Username varchar(255),@OldhsnCode varchar(15),@hsnType varchar(25)
declare @Msg varchar(max)
select @Username = username from userlist where userid = @UserId
select @OldhsnCode = hsncode from TBL_HSN_Master where hsnid= @hsnid and rowstatus=1
select @hsnType = hsntype from TBL_HSN_Master where hsnid= @hsnid and rowstatus=1

	
		if @Action = 'Edit'

		if not exists (select 1 from TBL_HSN_MASTER where hsnid !=@hsnid and hsnCode = @hsnCode and hsnDescription = @hsnDescription and CustomerId = @CustomerId and rate = @rate and rowstatus =1 and hsntype =@hsnType)
			Begin

				

				update TBL_HSN_Master set hsncode=@hsncode, 
										  hsndescription = @hsndescription,
										  unitprice=@unitprice, 
										  rate = @rate,
										  LastModifiedBy=@userId,
										  Lastmodifieddate=getdate() 
										   where hsnid= @hsnid and rowstatus=1

					set @Msg = 'HSN Details Modified : '+@OldhsnCode
					exec [Ins_AuditLog] @UserId,@Username,@Msg,NULL

					set @RetVal = 1 /*Success*/
			
			End

			Else
          Begin
	      set @RetVal = -1 /*Failure:-Already Exist */
          End

		Else if @action = 'Delete'

		if exists (select 1 from TBL_HSN_Master where hsnid=@hsnid and rowstatus=1)
			Begin
				update TBL_HSN_Master set rowstatus = 0,
										  LastModifiedBy=@userId,
										  Lastmodifieddate=getdate() 
										   where hsnid= @hsnid and rowstatus=1

				set @Msg = 'HSN Detail deleted : '+@OldhsnCode
					exec [Ins_AuditLog] @UserId,@Username,@Msg,NULL

					set @RetVal = 2 /*HSN Detail deleted */
			End

		Else
          Begin
	      set @RetVal = -2 /*Hsn code Doesn't Exist */
          End
	
	 select  @RetVal
END