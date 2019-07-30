

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve TRP Customers created by the respective TRP User 
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Raja 		Initial Version
11/24/2017	Seshadri	Fixed the qc reported bug
11/28/2017	Seshadri	Fixed the duplication issue while retrieving the
						customers in Customer Management Screen
11/28/2017	Seshadri	Fixed the QC reported issue on Admin Login in Customer Management Screen

*/

/* Sample Procedure Call

usp_Retrieve_TRP_TaxPayers 1,32

*/


CREATE PROCEDURE [usp_Retrieve_TRP_TaxPayers]
	@TrpId  int,
	@TrpUserId Int
-- /*mssx*/ With Encryption 

as 
Begin

	Select role_id,role_name 
	Into #TrpRoles 
	From Tbl_trp_roles 
	Where Trpid = @TrpId

	if Exists(Select 1 From TBL_TRP_Userlist t1,
							#TrpRoles t2		 
				Where t1.trpuserid = @TrpUserId
				And	t1.trpcustid = @TrpId
				And t1.roleid = t2.role_id
				And(t2.role_name = 'Admin' Or t2.role_name = 'Super Admin') )
	Begin

		Select * 
		Into #TBL_Customers_A 
		From Tbl_Customer t1
		Where t1.trpid = @TrpId
		And t1.statuscode Not In (6,3)
		And t1.rowstatus = 1

		Select * From #TBL_Customers_A

	End
	else
	Begin 	

		-- Assigned Customers

		Select distinct t2.*
		Into #TBL_Customers
		From TBL_TRP_UserAccess_Customer t1, 
				Tbl_customer t2
		Where t1.trpid = @TrpId
		And (t1.trpuserId = @TrpUserId Or t1.createdby = @TrpUserId)
		And t1.rowstatus = 1
		And t2.custid = t1.custid
		And t2.trpid = @TrpId
		And t1.rowstatus = 1
		And t2.statuscode Not In (6,3)
		And t2.rowstatus = 1

		if Exists (Select 1 From #TBL_Customers)
		Begin
		
			-- Created Customers

			Insert Into #TBL_Customers
			Select * From Tbl_Customer t1
			Where t1.trpid = @TrpId
			And t1.statuscode Not In (6,3)
			And t1.rowstatus = 1
			And t1.createdby = @TrpUserId
			And Not Exists(Select 1 From #TBL_Customers where custid = t1.custid)
		
		End 

		Select * From #TBL_Customers


	End


	Return 0

End