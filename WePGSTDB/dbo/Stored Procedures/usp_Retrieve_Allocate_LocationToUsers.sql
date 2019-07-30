  
/*  
(c) Copyright 2017 WEP Solutions Pvt. Ltd..  
All rights reserved  
  
Description : Procedure for Dropdown of Allocate Location to Users  
      
Written by  : nareshn@wepindia.com  
  
Date  Who   Decription   
11.05.2018 Naresh  Initial Version  
  
*/  
  
/* Sample Procedure Call  
  
exec usp_Retrieve_Allocate_LocationToUsers 26  
 */  
  
CREATE PROCEDURE [dbo].[usp_Retrieve_Allocate_LocationToUsers]    
    @CustId int = NULL,
	@branchId int
AS  
  
Set Nocount on  
  
Begin  
	Select   
		t1.Name as Name,  
		t2.UserId as UserId into #userdet  
		from UserList t1  
		Inner Join TBL_LocationAccess_Users t2 
		On t2.UserId = t1.UserId AND t1.CustId = t2.CustId
		Inner Join TBL_Cust_Location t3 
		On t3.branchId = t2.BranchId  
		Where t1.rowstatus = 1 And t2.rowstatus = 1 And t3.rowstatus = 1 
		And t2.BranchId = @branchId
  
		Select * from #userdet  
		Select Name, UserId from userlist where CustId = @CustId AND UserId Not In (select userid from #userdet) and rowstatus = 1
  
End