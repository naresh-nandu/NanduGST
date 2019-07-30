
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure for Retrieve Data For Edit Branch
				
Written by  : nareshn@wepindia.com

Date		Who			Decription 
08.05.2018	Naresh		Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_Branch 1
 */

Create PROCEDURE [dbo].[usp_Retrieve_Branch]		
    @branchId int
	
-- /*mssx*/ With Encryption 
AS

Set Nocount on

   Begin

   Select 
      t2.CompanyName as company,
	  t2.PANNo as pan,
	  t3.GSTINNo as gstinNo,
	  branch as branchName,
	  emailId as email,
	  branchId
      from TBL_Cust_LOCATION t1
      Inner Join TBL_Cust_PAN t2 On t2.PANId = t1.panId
	  Inner Join TBL_Cust_GSTIN t3 On t3.GSTINId = t1.gstinId
      where t1.branchId=@branchId And t1.rowstatus=1
   
   End
	
  
Return 0