
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve TRP User Access
				
Written by  : Karthik

Date		Who			Decription 
11/06/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call
 exec usp_Retrieve_TRP_User_Details 'swarupa.anand@wepindia.com','wep12345', 0
*/
CREATE PROCEDURE [usp_Retrieve_TRP_User_Details]   
   
 @Email    nvarchar(250),  
 @Password    varchar(50),  
 @RetValue int NULL Output  
   
  
AS  
BEGIN  
  
Declare  @TRPId int, @UserStatus int,@CustName nvarchar(250),@CustStatus int,@CustRowstatus int,@RoleId int,@roleName nvarchar(100), @RetMsg nvarchar(max) ,@userrowstatus int
 if exists(select trpUserId from TBL_TRP_Userlist where Email=@Email and Password=@Password)  
 begin  
  select @TRPId= trpCustId,@UserStatus=[Status],@RoleId=RoleId,@userrowstatus=rowstatus from TBL_TRP_Userlist where  Email=@Email and [Password]=@Password  
  select @CustName=[Name], @CustRowstatus=RowStatus,@CustStatus=StatusCode from TBL_TRP_Customer where trpCustId=@TRPId  
  select @roleName=Role_Name from TBL_TRP_Roles where Role_ID=@RoleId and TRPID=@TRPId  
  
 --if exists( select CustId from TBL_Customer where CustId=@TRPId and RowStatus=1 and StatusCode=1)
 --Begin

  if @CustRowstatus=1  
  Begin 
  
	if @userrowstatus=1
	begin
	   if @CustStatus=1 or  @CustStatus=5
	   Begin   
				if @UserStatus=1  
				 Begin  
				  set @RetValue=-1--TRP Customer is available and status is Approved/Unblocked and user also acive. So User can able to Login  
				  set @RetMsg='TRP Customer is available and status is Approved/Unblocked and user also acive. So User can able to Login'  
				 End  
				Else  
				 Begin  
				  set @RetValue=-2  -- TRP Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.  
				  set @RetMsg='TRP Customer is available and status is Approved/Unblocked but User is not active so user cant able to login,.'  
				 End  
	   End  
	   Else  
	   Begin  
		set @RetValue=-3 -- TRP Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
		set @RetMsg='TRP Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
	   End
	 End
	 Else
		Begin
			 Set @RetValue=-5 -- User Email Id /Password is wrong.  
			 set @RetMsg='User Email Id /Password is wrong.' 
		End     
  End  
  Else  
	  Begin  
	   set @RetValue=-4 -- TRP Customer status is inactive, So User not allowed to Login  
	   set @RetMsg='TRP Customer status is inactive, So User not allowed to Login'  
	  End  
  
  --Else
  --Begin
	 -- set @RetValue=-3 -- TRP Customer is available but status is Pending approval/ Blocked, So User not allowed to Login  
  --  set @RetMsg='TRP Customer is available but status is Pending approval/ Blocked, So User not allowed to Login'  
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