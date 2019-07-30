
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version


*/

/* Sample Procedure Call  

exec usp_Import_CSV_TRP_Customer 113,190,101

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_TRP_Customer]
	@FileId int,
	@TRPUserId int, 
	@TRPId int,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1),
			@SourceType varchar(255)

	Select @Delimiter = ','
	Select @SourceType = 'CSV'

	--Update TBL_TRP_RECVD_FILES
	--Set filestatus = 2
	--Where fileid = @FileId

	Create Table #TBL_CSV_TRP_Customer_RECS
	(
		fileid int,
		slno varchar(50),
		[name] varchar(50),
		designation varchar(50),
		company varchar(250),
		gstin varchar(50),
		gstinusername varchar(50),
		email varchar(50),
		mobileno varchar(50),
		pan varchar(50),
		statecode varchar(50),
		package varchar(50),
		address Nvarchar(Max),
		custid int NULL,
		panid int NULL,
		gstinid int NULL,
		roleid int NULL,
		userid int NULL,
		useraccessid int NULL,
		packageId int NULL,
		errorcode smallint,
		errormessage varchar(255)
	 )

	 Begin Try

		Insert Into #TBL_CSV_TRP_Customer_RECS
		(	fileid,
			slno,
			[name],
			designation,
			company,
			gstin,
			gstinusername,
			email,
			mobileno,
			pan,
			statecode,
			package,
			[address] )

		Select	fileid,
				slno,
				[name],
				designation,
				company,
				gstin,
				gstinusername,
				email,
				mobileno,
				pan,
				statecode,
				package,
				[address]
		From TBL_CSV_TRP_Customer_RECS t1 
		Where t1.fileid = @FileId

		print '1.Inserted into # table'
	
	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch
	
	-- Processing Logic
	select * from #TBL_CSV_TRP_Customer_RECS

	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_TRP_Customer_RECS

	if exists (Select 1 from #TBL_CSV_TRP_Customer_RECS)
	Begin

		Update #TBL_CSV_TRP_Customer_RECS
		Set gstin = Upper(Ltrim(Rtrim(IsNull(gstin,'')))),
			[name] = Upper(Ltrim(Rtrim(IsNull([name],'')))),
			designation = Upper(Ltrim(Rtrim(IsNull(designation,'')))),
			company = Upper(Ltrim(Rtrim(IsNull(company,'')))),
			gstinusername = Upper(Ltrim(Rtrim(IsNull(gstinusername,'')))),
			email = Upper(Ltrim(Rtrim(IsNull(email,'')))),
			mobileno = Upper(Ltrim(Rtrim(IsNull(mobileno,'')))),
			pan = Upper(Ltrim(Rtrim(IsNull(pan,'')))),
			statecode = Upper(Ltrim(Rtrim(IsNull(statecode,'')))),
			package = Upper(Ltrim(Rtrim(IsNull(package,'')))),
			[address] = Upper(Ltrim(Rtrim(IsNull([address],''))))

		print '2.Removing Empty char in all fields'

		Update #TBL_CSV_TRP_Customer_RECS 
		SET #TBL_CSV_TRP_Customer_RECS.Packageid = t2.packageid 
		FROM #TBL_CSV_TRP_Customer_RECS t1,
					TBL_TRP_Package t2 
		WHERE Ltrim(Rtrim(IsNull(t1.package,''))) = Ltrim(Rtrim(IsNull(t2.package,'')))
		Print '3.Update Package'
				
		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		Print '4.Validate GStin'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Name is mandatory'
		Where Ltrim(Rtrim(IsNull([name],''))) = ''
		And IsNull(ErrorCode,0) = 0
		Print '5.Validation on Name'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -3,
			ErrorMessage = 'Company Name is mandatory'
		Where Ltrim(Rtrim(IsNull(company,''))) = ''
		And IsNull(ErrorCode,0) = 0
		print '6.Validation for Company Name'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -5,
			ErrorMessage = 'GSTIN User Name is mandatory and should be as per GST Site'
		Where Ltrim(Rtrim(IsNull(gstinusername,''))) = ''
		And IsNull(ErrorCode,0) = 0
		Print '7.Validation for GSTIN User Name'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -6,
			ErrorMessage = 'Email is mandatory'
		Where Ltrim(Rtrim(IsNull(email,''))) = ''
		And IsNull(ErrorCode,0) = 0
		Print '8.Validation for Mail'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -7,
			ErrorMessage = 'Mobile No is mandatory'
		Where Ltrim(Rtrim(IsNull(mobileno,''))) = ''
		And IsNull(ErrorCode,0) = 0
		print '9.Mobile No Mandatory'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -8,
			ErrorMessage = 'PAN No is mandatory'
		Where Ltrim(Rtrim(IsNull(pan,''))) = ''
		And IsNull(ErrorCode,0) = 0
		print '10.PAN No Mandatory'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -9,
			ErrorMessage = 'State Code is mandatory'
		Where Ltrim(Rtrim(IsNull(statecode,''))) = ''
		And IsNull(ErrorCode,0) = 0
		Print '11.State Code is Mandatory'

		Update #TBL_CSV_TRP_Customer_RECS 
		Set ErrorCode = -10,
			ErrorMessage = 'Invalid Package'
		Where Ltrim(Rtrim(IsNull(packageid,''))) = ''
		And IsNull(ErrorCode,0) = 0
		Print '12.Pakage Validation'

		---- Validating whether GSTIN is already exist or not in Wep GST
		--Update #TBL_CSV_TRP_Customer_RECS 
		--			SET #TBL_CSV_TRP_Customer_RECS.ErrorCode = -11,
		--				#TBL_CSV_TRP_Customer_RECS.ErrorMessage = 'GSTIN No Already Exists in WEP GST Portal'
		--FROM #TBL_CSV_TRP_Customer_RECS t1,
		--			TBL_Cust_Gstin t2 
		--WHERE t1.gstin = t2.gstinno
		--			And t2.rowstatus = 1
		--Print '13.Check GSTIN No is already Updated'

		--  Inserting Customer		 
		insert into TBL_Customer ([Name],
					Designation,
					Company,
					GSTINNo,
					Email,
					MobileNo,
					PANNo,
					Statecode,
					StatusCode,
					CreatedDate,
					RowStatus,
					AadharNo,
					[Address],
					TRPId,
					PackageId,
					CreatedBy
					) 
		Select [name],designation,company,gstin,
					email,mobileno,pan,statecode,1,getdate(),
					1,'',[address],@TRPId,Packageid,@TRPUserId
		From #TBL_CSV_TRP_Customer_RECS t1
					Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM TBL_Customer t2
										Where t2.Email = t1.email
										or t2.MobileNo = t1.mobileno
										And rowstatus=1 )
					And IsNull(ErrorCode,0) = 0
		Print '14. Bulk Insert Customer data'
		-- Updating Custid
		Update #TBL_CSV_TRP_Customer_RECS 
					SET #TBL_CSV_TRP_Customer_RECS.custid = t2.custid 
		FROM #TBL_CSV_TRP_Customer_RECS t1,
					TBL_Customer t2 
		WHERE t1.email = t2.Email 
					And t1.mobileno = t2.MobileNo
					and t2.rowstatus =1
		Print '15.Update customer id in # Table'

		-- Updating Customer Reference No
		Update TBL_Customer 
					SET TBL_Customer.referenceNo = 'WEP00' + (convert (varchar(15), t2.custid))
		FROM #TBL_CSV_TRP_Customer_RECS t1, TBL_Customer t2 
		WHERE t1.email = t2.Email 
				And t1.mobileno = t2.MobileNo
				And t1.custid =t2.custid and t2.referenceNo is NULL
		Print '16. Update Reference No'	
		-- Insert Pan				
		insert into tbl_cust_pan (
					PANNo,
					CustId,
					CreatedBy,
					createddate,
					rowstatus,
					CompanyName
					) 
		Select pan,custid,@TRPUserId,getdate(),1,company
					From #TBL_CSV_TRP_Customer_RECS t1
					Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM tbl_cust_pan t2
										Where t2.PANNo = t1.pan
										And rowstatus=1 )
					And IsNull(ErrorCode,0) = 0
		print '17.PAN No Bulk Insert'

		-- Updating CustiD 
		Update #TBL_CSV_TRP_Customer_RECS 
					SET #TBL_CSV_TRP_Customer_RECS.Panid = t2.Panid 
					FROM #TBL_CSV_TRP_Customer_RECS t1,
							tbl_cust_pan t2 
					WHERE t1.pan = t2.PANNo
					And t2.rowstatus=1 
		Print '18.Update PAN ID in # Table'
		 
		-- Inserting GSTIN
		insert into TBL_Cust_GSTIN (
					GSTINNo,
					StateCode,						
					PANNo,
					CustId,
					CreatedBy,
					createddate,
					rowstatus,
					GSTINUserName,
					Address
					) 
		Select		gstin,
					statecode,
					pan,
					custid,
					@TRPUserId,
					getdate(),
					1,
					gstinusername,
					[address]
		From #TBL_CSV_TRP_Customer_RECS t1
					Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM tbl_cust_GSTIN t2
										Where t2.PANNo = t1.pan
										And t2.GSTINno = t1.gstin
										And rowstatus=1 )
					And IsNull(ErrorCode,0) = 0
			Print '19.Bulk Insert GSTIN'

		-- Updating GSTINId 
		Update #TBL_CSV_TRP_Customer_RECS 
					SET #TBL_CSV_TRP_Customer_RECS.gstinid = t2.gstinid 
		FROM		#TBL_CSV_TRP_Customer_RECS t1,
					tbl_Cust_GStin t2 
		WHERE t1.pan = t2.PANNo
					And t1.gstin = t2.GstinNo
					And rowstatus=1 
		Print '20.Update GSTIN Id into # Table'
		-- Inserting Roles
		insert into MAS_Roles (
					Role_Name,
					CustomerId) 
		Select distinct 'Admin',custid
		From #TBL_CSV_TRP_Customer_RECS t1
		Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM MAS_Roles t2
										Where t2.CustomerId = t1.custid)
					And IsNull(ErrorCode,0) = 0
		Print '21. Insert Admin Role'

		-- Updating RoleId 
		Update #TBL_CSV_TRP_Customer_RECS 
					SET #TBL_CSV_TRP_Customer_RECS.RoleId = t2.Role_Id 
		FROM #TBL_CSV_TRP_Customer_RECS t1,
					MAS_Roles t2 
		WHERE t1.custid = t2.CustomerId
		Print '22.Update Role Id into # Table'

		-- Inserting User details to UserList
		insert into UserList (CustId,[Name],
					Designation,
					[Password],
					Email,
					MobileNo,
					RoleId,
					Status,
					CreatedBy,
					CreatedDate,
					RowStatus
					) 
		Select custId,[name],designation,mobileNo,email,mobileno,roleid,1,@TRPUserId,getdate(),1
		From #TBL_CSV_TRP_Customer_RECS t1
		Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.custid,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.roleid,''))) <> ''
					And Not Exists ( SELECT 1 FROM UserList t2
								Where t2.Email = t1.email
								And t2.MobileNo = t1.mobileno
								And t2.CustId = t1.CustId
								And rowstatus=1 ) 
					And IsNull(ErrorCode,0) = 0
		Group By custId,[name],designation,mobileno,email,mobileno,roleid

		Print '23.Bulk Insert User List'

		-- Updating UserId 
		Update #TBL_CSV_TRP_Customer_RECS 
					SET #TBL_CSV_TRP_Customer_RECS.UserId = t2.UserId 
		FROM #TBL_CSV_TRP_Customer_RECS t1,
					UserList t2 
		WHERE t1.email = t2.Email
					And t1.mobileno = t2.MobileNo
					And t1.custid = t2.CustId
					And rowstatus = 1
			Print '24.Update User Id'

		--Inserting Rol & Resources to MAS_Roles_Resources table.
		Insert into MAS_Roles_Resources (
					FK_Role_Resource_Role_ID,
					FK_Role_Resource_Resource_ID,
					Role_Resource_IsAssigned) 
		Select distinct t1.RoleId,t3.Resource_Id,1
		From #TBL_CSV_TRP_Customer_RECS t1,
					MAS_Resources t3
		Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM MAS_Roles_Resources t2
										Where t2.FK_Role_Resource_Role_ID = t1.RoleId
											And t2.FK_Role_Resource_Resource_ID =t3.Resource_Id)
					And t3.Resource_Id Not in (1,10,11)
					And IsNull(ErrorCode,0) = 0
			Print '25.Bulk Insert Role_Resources'

		-- Inserting Rol & Resources to MAS_Roles_Resources table
		Insert into MAS_Roles_Resources (
					FK_Role_Resource_Role_ID,
					FK_Role_Resource_Resource_ID,
					Role_Resource_IsAssigned) 
		Select distinct t1.roleid,t3.resource_id,0
		From #TBL_CSV_TRP_Customer_RECS t1,
					MAS_Resources t3
		Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM MAS_Roles_Resources t2
										Where t2.FK_Role_Resource_Role_ID = t1.RoleId
										And t2.FK_Role_Resource_Resource_ID =t3.Resource_Id)
					And t3.Resource_Id in (1,10,11)
					And IsNull(ErrorCode,0) = 0
			Print '25.a.Bulk Insert Role_Resources'
		-- Inserting User Access GSTIN
		Insert into UserAccess_GSTIN (
					GstinId,
					CustId,						
					UserId,
					createddate,
					CreatedBy,
					rowstatus) 
		Select gstinid,custid,userid,getdate(),@TRPUserId,1
		From #TBL_CSV_TRP_Customer_RECS t1
		Where Ltrim(Rtrim(IsNull(t1.email,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.gstin,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.mobileno,''))) <> ''
					And Ltrim(Rtrim(IsNull(t1.pan,''))) <> ''
					And Not Exists ( SELECT 1 FROM UserAccess_GSTIN t2
										Where t2.GstinId = t1.gstinid
										And t2.CustId = t1.custid
										And t2.UserId = t1.userid
										And rowstatus=1 )
					And IsNull(ErrorCode,0) = 0
		Group By gstinid,custid,userid
		Print '26.Bulk Insert User access GStin'

		-- Updating User Access GSTIN 
		Update #TBL_CSV_TRP_Customer_RECS 
		SET #TBL_CSV_TRP_Customer_RECS.UserAccessId = t2.UserAccessId 
		FROM #TBL_CSV_TRP_Customer_RECS t1,
					UserAccess_GSTIN t2 
		WHERE t1.GstinId = t2.GstinId
					And t1.custId = t2.CustId
					And t1.userId = t2.UserId
					And rowstatus=1 
		print '27.Update User ACcess Id'

		Select @ProcessedRecordsCount = Count(*)
		From #TBL_CSV_TRP_Customer_RECS fv
					Where IsNull(fv.ErrorCode,0) = 0
		print '28.Total  Processed Count'
		print @ProcessedRecordsCount
		Select @ErrorRecordsCount = Count(*)
		From #TBL_CSV_TRP_Customer_RECS fv
		Where IsNull(fv.ErrorCode,0) <> 0
		Print '29.Total Error Records'
		print @ErrorRecordsCount

		Update TBL_TRP_RECVD_FILES
				Set filestatus = 0,
				totalrecordscount = @TotalRecordsCount,
				processedrecordscount = @ProcessedRecordsCount,
				errorrecordscount = @ErrorRecordsCount
		Where fileid = @FileId

		Select 'Total Record Count :' + convert(varchar(10), @TotalRecordsCount)
		Select 'Processed Record Count :' + convert(varchar(10), @ProcessedRecordsCount)
		Select 'Error Record Count :' + convert(varchar(10), @ErrorRecordsCount)
				
		select * from #TBL_CSV_TRP_Customer_RECS

	End 
	 Return 0


End