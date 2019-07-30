-- =============================================
-- Author:		KK
-- Create date: 06-Apr-17
-- Description:	Procedure to insert the User details into Userlist Table
-- exec [Ins_UserList] 20,'TestUser1','SSE','TestUser','kk','kk4@gmail.com','9008906454',1,'04/01/2017','04/25/2017',1,'TESTEsignedUser',0
-- =============================================
CREATE PROCEDURE [Ins_UserList] 
	@CustId    int,
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
	@EsignedUser    nvarchar(MAX),
	@RetValue	int Output,
	@UserId		int output

AS
BEGIN
	
	if exists(select CustId from TBL_Customer where CustId=@CustId and rowstatus=1 and (statuscode=1 or statuscode=5))
	begin

	if not exists(select UserId from UserList where (Email=@Email or MobileNo=@MobileNo) and  rowstatus=1 )
	Begin

		insert into Userlist (CustId,
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
							EsignedUser,rowstatus) values(
							@CustId,
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
							@EsignedUser,1
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
			set @RetValue=3 -- Customer is not valid(i.e. Customer status may be not yet approved or disabled)
			set @UserId=0
			print @RetValue
		End


	return @RetValue
	return @UserId
	END