

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Assign TRP User to Taxpayer
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
11/23/2017	Seshadri	Fined Tuned the Code
11/24/2017	Seshadri	Fixed the qc reported bug
11/28/2017	Seshadri	Fixed the QC reported issue on Admin Login in Customer Mapping Screen
*/

/* Sample Procedure Call

usp_Retrieve_TRPUser_Assignto_Taxpayer 1,32,2

*/


CREATE PROCEDURE [usp_Retrieve_TRPUser_Assignto_Taxpayer] 
	@TrpId  int,
	@TrpUserId Int,
	@Mode int -- 1. Customer Mapping Screen ; 2 - Your Customer Screen
-- /*mssx*/ With Encryption 

as 
Begin

	Set Nocount on

	Create Table #TBL_Customers
	(
		custid int,
		companyname varchar(50),
		gstin varchar(15),
		packageid int,
		custrefno varchar(150),
		email varchar(50),
		[password] varchar(50),
		username varchar(150),
		useraccessid int
	 )

	 Select role_id,role_name 
	 Into #TrpRoles 
	 From Tbl_trp_roles 
	 Where Trpid = @TrpId

	 if @Mode = 1
	 Begin

	 	 if Exists(Select 1 From TBL_TRP_Userlist t1,
								#TrpRoles t2		 
					Where t1.trpuserid = @TrpUserId
					And	t1.trpcustid = @TrpId
					And t1.roleid = t2.role_id
					And(t2.role_name = 'Admin' Or t2.role_name = 'Super Admin') )
		 Begin

	 		Insert Into #TBL_Customers
			Select t2.custid,
					t2.company,
					t2.gstinno,
					t2.packageid,
					t2.referenceno,
					t1.email,
					t1.[password],
					t3.[name],
					t1.useraccessid
			From TBL_TRP_UserAccess_Customer t1, 
					Tbl_customer t2,
					TBL_TRP_Userlist t3
			Where t1.trpid = @TrpId
			And t1.rowstatus = 1
			And t2.custid = t1.custid
			And t2.trpid = @TrpId
			And t3.trpuserid = t1.trpuserid
			And t2.statuscode Not In (6,3)
			And t2.rowstatus = 1
			And t3.[status] = 1
			And t3.rowstatus = 1

		End
		else 
		Begin

			Insert Into #TBL_Customers
			Select t2.custid,
					t2.company,
					t2.gstinno,
					t2.packageid,
					t2.referenceno,
					t1.email,
					t1.[password],
					t3.[name],
					t1.useraccessid
			From TBL_TRP_UserAccess_Customer t1, 
					Tbl_customer t2,
					TBL_TRP_Userlist t3
			Where t1.trpid = @TrpId
			And (t1.trpuserId = @TrpUserId Or t1.createdby = @TrpUserId)
			And t1.rowstatus = 1
			And t2.custid = t1.custid
			And t2.trpid = @TrpId
			And t3.trpuserid = t1.trpuserid
			And t2.statuscode Not In (6,3)
			And t2.rowstatus = 1
			And t3.[status] = 1
			And t3.rowstatus = 1

		End

	 End
	 else if @Mode = 2 
	 Begin
		 if Exists(Select 1 From TBL_TRP_Userlist t1,
								#TrpRoles t2		 
					Where t1.trpuserid = @TrpUserId
					And	t1.trpcustid = @TrpId
					And t1.roleid = t2.role_id
					And(t2.role_name = 'Admin' Or t2.role_name = 'Super Admin') )
		 Begin

			Insert Into #TBL_Customers
			Select t1.custid,
				   t1.company,
				   t1.gstinno,
				   t1.packageid,
				   t1.referenceno,
				   space(250) as email,
				   space(50) as [password],
				   t2.[name] as [name],
				   0 as useraccessid
			From  Tbl_customer t1,
				  TBL_TRP_Userlist t2
			Where t1.trpid = @TrpId
			And t2.trpcustid = t1.trpid
			And t2.trpUserId = @TrpUserId
			And t1.statuscode Not In (6,3)
			And t1.rowstatus = 1
			And t2.[status] = 1
			And t2.rowstatus = 1

			Select  t1.custid,t1.email,t1.[password]
			Into #TBL_EmailIds 
			From UserList t1,
				 #TBL_Customers t2
			Where t1.custid = t2.custid and rowstatus = 1  
			Order By userid

			UPDATE #TBL_Customers
			Set  #TBL_Customers.email = t2.email,
				 #TBL_Customers.[password] = t2.[password]
			FROM #TBL_Customers t1,
				 #TBL_EmailIds t2
			Where t1.custid = t2.custid

		 End
		 else
		 Begin

	 		Insert Into #TBL_Customers
			Select t2.custid,
				   t2.company,
				   t2.gstinno,
				   t2.packageid,
				   t2.referenceno,
				   t1.email,
				   t1.[password],
				   t3.[name],
				   t1.useraccessid
			From TBL_TRP_UserAccess_Customer t1, 
				 Tbl_customer t2,
				 TBL_TRP_Userlist t3
			Where t1.trpid = @TrpId
			And (t1.trpuserId = @TrpUserId Or t1.createdby = @TrpUserId)
			And t1.rowstatus = 1
			And t2.custid = t1.custid
			And t2.trpid = @TrpId
			And t3.trpuserid = t1.trpuserid
			And t2.statuscode Not In (6,3)
			And t2.rowstatus = 1
			And t3.[status] = 1
			And t3.rowstatus = 1

	 
		 End

	 End
	 
	 Select custid as Cust_Id,
			companyname as Company_Name,
			gstin as GSTIN_No,
			packageid as Package_Id,
			custrefno as Cust_Ref_No,
			email as Email,
			[password] as [Password],
			username as [User_Name],
			useraccessid as User_Access_Id
	From #TBL_Customers

    -- Drop Temp Tables

	 Drop Table #TBL_Customers


	Return 0


End