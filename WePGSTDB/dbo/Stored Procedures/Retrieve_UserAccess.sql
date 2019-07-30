-- =============================================  
-- Author:  KK  
-- Create date: 07-Apr-17  
-- Description: Procedure to Validate User Access  
-- exec [Retrieve_UserAccess] 'raja.m@wepindia.com','wep12345'  
-- =============================================  
CREATE PROCEDURE [Retrieve_UserAccess]   
   
 @Email    nvarchar(250),  
 @Password    varchar(50),  
 @RetValue int = NULL Out,
 @RetMessage	varchar(255)= NULL Out  
AS  

BEGIN  
  
Declare  @CustId int, @UserStatus int,@CustName nvarchar(250),@CustStatus int,@CustRowstatus int,@RoleId int,@roleName nvarchar(100), @userrowstatus int
 if exists(select UserId from UserList where Email=@Email and Password=@Password)  
 begin  
  select @CustId= CustId,@UserStatus=[Status],@RoleId=RoleId,@userrowstatus=rowstatus from UserList where  Email=@Email and [Password]=@Password  
  select @CustName=[Name], @CustRowstatus=RowStatus,@CustStatus=StatusCode from TBL_Customer where CustId=@CustId  
  select @roleName=Role_Name from MAS_Roles where Role_ID=@RoleId and CustomerID=@CustId  
  
 --if exists( select CustId from TBL_Customer where CustId=@CustId and RowStatus=1 and StatusCode=1)
 --Begin

  if @CustRowstatus=1  
  Begin 
  
	if @userrowstatus=1
	begin
	   if @CustStatus=1 or @CustStatus=4  or @CustStatus=5 
	   Begin   
				if @UserStatus=1  
				 Begin  
				  set @RetValue=-1--Customer is available and status is Approved/Unblocked and user also acive. So User can able to Login  
				  set @RetMessage='SUCCESS'  
				 End  
				Else  
				 Begin  
				  set @RetValue=-2  -- Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.  
				  set @RetMessage='Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.'  
				 End  
	   End  
	   Else  
	   Begin  
		set @RetValue=-3 -- Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
		set @RetMessage='Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
	   End
	 End
	 Else
		Begin
			 Set @RetValue=-5 -- User Email Id /Password is wrong.  
			 set @RetMessage='User is deleted.' 
		End     
  End  
  Else  
	  Begin  
	   set @RetValue=-4 -- Customer status is inactive, So User not allowed to Login  
	   set @RetMessage='Customer status is inactive, So User not allowed to Login'  
	  End  
  
  --Else
  --Begin
	 -- set @RetValue=-3 -- Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
  --  set @RetMessage='Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
  --End  
 End  
 Else  
	 Begin  
	  Set @RetValue=-5 -- User Email Id /Password is wrong.  
	  set @RetMessage='Invalid Login Credentials'  
	 End  
  
 print @RetValue  
 print @RetMessage  
 return @RetValue
 END