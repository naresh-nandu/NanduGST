-- =============================================  
-- Author:  KK  
-- Create date: 07-Apr-17  
-- Description: Procedure to Validate User Access  
-- exec [Retrieve_UserAccess_ByHash] 'PriyabratP@wepindia.com','test123',0  
-- =============================================  
CREATE PROCEDURE [Retrieve_UserAccess_ByHash]   
   
 @Email    nvarchar(250),  
 @Password    varchar(50),  
 @RetValue int NULL Output  
   
  
AS  
BEGIN  
  
Declare  @CustId int, @UserStatus int,@CustName nvarchar(250),@CustStatus int,@CustRowstatus int,@RoleId int,@roleName nvarchar(100), @RetMsg nvarchar(max)  
 if exists(select UserId from UserList where Email=@Email and HashPassword=HASHBYTES('SHA2_512', @Password))  
 begin  
  select @CustId= CustId,@UserStatus=[Status],@RoleId=RoleId from UserList where  Email=@Email and HashPassword=HASHBYTES('SHA2_512', @Password)
  select @CustName=[Name], @CustRowstatus=RowStatus,@CustStatus=StatusCode from TBL_Customer where CustId=@CustId  
  select @roleName=Role_Name from MAS_Roles where Role_ID=@RoleId and CustomerID=@CustId  
  
 --if exists( select CustId from TBL_Customer where CustId=@CustId and RowStatus=1 and StatusCode=1)
 --Begin

  if @CustRowstatus=1  
  Begin  
   if @CustStatus=1 or @CustStatus=4  
   Begin   
    if @UserStatus=1  
     Begin  
      set @RetValue=-1--Customer is available and status is Approved/Unblocked and user also acive. So User can able to Login  
      set @RetMsg='Customer is available and status is Approved/Unblocked and user also acive. So User can able to Login'  
     End  
    Else  
     Begin  
      set @RetValue=-2  -- Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.  
      set @RetMsg='Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.'  
     End  
   End  
   Else  
   Begin  
    set @RetValue=-3 -- Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
    set @RetMsg='Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
   End     
  End  
  Else  
  Begin  
   set @RetValue=-4 -- Customer status is inactive, So User not allowed to Login  
   set @RetMsg='Customer status is inactive, So User not allowed to Login'  
  End  
  
  --Else
  --Begin
	 -- set @RetValue=-3 -- Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
  --  set @RetMsg='Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
  --End  
 End  
 Else  
 Begin  
  Set @RetValue=-5 -- User Email Id /Password is wrong.  
  set @RetMsg='User Email Id /Password is wrong.'  
 End  
  
 print @RetValue  
 print @RetMsg  
 return @RetValue
 END