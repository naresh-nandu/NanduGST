
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert TRP UIser Details
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec usp_Insert_TRP_User_Details
 */
CREATE PROCEDURE [usp_Insert_TRP_User_Details] 
	@TrpCustId  int,
	@Name    nvarchar(150),
	@Designation    varchar(50),
	@Username    varchar(50),
	@Password    varchar(50),
	@Email    nvarchar(250),
	@MobileNo    varchar(50),
	@RoleId    int,
	@ValidFrom    datetime,
	@ValidTo    datetime,
	@CreatedBy    int,
	@RetValue	int Output,
	@UserId		int output
-- /*mssx*/ With Encryption 

AS
BEGIN
	
	if exists(select trpCustId from TBL_TRP_Customer where trpCustId=@TrpCustId and rowstatus=1)
	begin

	if not exists(select trpUserId from TBL_TRP_Userlist where (Email=@Email or MobileNo=@MobileNo)  and rowstatus=1)
	Begin

		insert into TBL_TRP_Userlist (trpCustId,
							Name,
							Designation,
							Username,
							[Password],
							Email,
							MobileNo,
							RoleId,
							ValidFrom,
							ValidTo,
							[Status],
							CreatedBy,
							CreatedDate,
							rowstatus) 
							values(
							@TrpCustId,
							@Name,
							@Designation,
							@Username,
							@Password,
							@Email,
							@MobileNo,
							@RoleId,
							@ValidFrom,
							@ValidTo,
							1,
							@CreatedBy,
							GETDATE(),
							1
							)
			select @UserId=SCOPE_IDENTITY()
			set @RetValue=1 -- User Not exists under given customer so Inserting the details
			
			print @RetValue	
	End
	else 
		Begin
			set @RetValue=2	-- User name or email Id already Exist under given customer.
			set @UserId=0
			print @RetValue
		End
	
	End
	else
		Begin
			set @RetValue=3 -- TRP Customer is not valid(i.e. TRP Customer status may be not yet approved or disabled)
			set @UserId=0
			print @RetValue
		End


	return @RetValue
	return @UserId
	END