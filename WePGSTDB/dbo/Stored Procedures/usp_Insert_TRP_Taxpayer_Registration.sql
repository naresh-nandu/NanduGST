
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to insert the Customer details into Tbl_Customer Table
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who					Decription 
04/06/2017	Karthik				Initial Version
03/12/2018	Seshadri / Naresh	Inserted default values to TBL_Cust_Settings
*/

/* Sample Procedure Call

exec usp_Insert_TRP_Taxpayer_Registration 1,1, 'NewCustomer','Manager','New Customer','GST10001','kk5@gmail.com','9008906454','CJTPK10001',12,'04/01/2017','04/25/2017','AFDGFDHGJH77','','MG Road','GSTINName',1,0,0


 */
 

CREATE PROCEDURE [usp_Insert_TRP_Taxpayer_Registration]
	@TRPId	 int,
	@TrpUserId int,
	@Name    varchar(50),
	@Designation    varchar(50),
	@Company    varchar(50),
	@GSTINNo    varchar(50),
	@Email    varchar(50),
	@MobileNo    varchar(50),
	@PANNo    varchar(50),
	@Statecode    varchar(50),
	@ValidFrom    datetime,
	@ValidTo    datetime,
	@AadharNo	varchar(12),
	@Referenceno varchar(50),
	@Address varchar(255),
	@GSTINUserName varchar(50),
	@PackageId int,
	@RetValue	int Output,
	@CustId		int output
	

AS
BEGIN

	Declare @GSTINId int,@RoleId int

	If Not Exists(Select CustId from TBL_Customer 
					Where (GSTINNo=UPPER(@GSTINNo) 
					Or PANNO = UPPER(@PANNo) 
					Or MobileNo=@MobileNo 
					Or Email=@Email 
					And rowstatus =1)
				)
	Begin

		If Not Exists(Select 1 from Userlist 
						Where  MobileNo=@MobileNo 
						Or Email=@Email 
						And rowstatus =1)
		Begin

			Insert into TBL_Customer (
							Name,
							Designation,
							Company,
							GSTINNo,
							Email,
							MobileNo,
							PANNo,
							Statecode,
							ValidFrom,
							ValidTo,
							StatusCode,
							CreatedDate,
							RowStatus,
							AadharNo,
							[Address],
							TrpId,
							PackageId,
							CreatedBy
							) 
			Values(			@Name,
							@Designation,
							@Company,
							UPPER(@GSTINNo),
							@Email,
							@MobileNo,
							@PANNo,
							@Statecode,
							@ValidFrom,
							@ValidTo,
							1,
							getdate(),
							1,
							@AadharNo,
							@Address,
							@TRPId,
							@PackageId,
							@TrpUserId
							)
			Select @CustId = SCOPE_IDENTITY()

			If ltrim(rtrim(isnull(@Referenceno,''))) <> ''
			Begin
				Update TBL_Customer Set Referenceno=@Referenceno
				Where custid =@CustId
			End
			Else
			Begin
				Set @Referenceno = 'WEP00'+ CAST(@CustId AS VARCHAR(10))
				Update TBL_Customer set Referenceno=@Referenceno where custid =@CustId
			End


			-- Adding GSTIN 

			If Not Exists(Select GSTINNo from TBL_Cust_GSTIN
							where GSTINNo=@GSTINNo 
							and Statecode=@Statecode 
							and PANNo=@PANNo 
							and CustId=@CustId 
							and rowstatus =1 )
			Begin
			
				Insert Into TBL_Cust_GSTIN(GSTINNo,Statecode,PANNo,CustId,CreatedBy,
											CreatedDate,GSTINUserName,Address)
				Values(@GSTINNo,@Statecode,@PANNo,@CustId,@TrpUserId,
						GETDATE(),@GSTINUserName,@Address)
			
				Select @GSTINId=SCOPE_IDENTITY()

				if(@@ROWCOUNT>0)
				Begin		
				
					-- Adding PAN No
				
					If not exists(Select PANNO from TBL_Cust_PAN 
									where PANNO = @PANNo 
									And custid = @CustId 
									And rowstatus =1)
					Begin

						Insert into TBL_Cust_PAN(PANNo,CustId,CompanyName,CreatedBy,
												CreatedDate,rowstatus)
						values(@PANNo,@CustId,@Company,@TrpUserId,GETDATE(),1)
						if(@@ROWCOUNT>0)
						Begin

							If Not Exists(Select Role_ID From MAS_Roles 
											Where Role_Name='Admin' And CustomerID=@CustId)
							Begin

								-- Creating default Role based on customer
			
								Insert into MAS_Roles(Role_Name,CustomerID)values('Admin',@CustId)
								Select @RoleId=SCOPE_IDENTITY()
								if(@@ROWCOUNT>0)
								Begin

									-- Auto create the resources

									DECLARE @ResourceId INT
									DECLARE @getResourceID CURSOR

									SET @getResourceID = CURSOR FOR
									SELECT Resource_ID FROM MAS_Resources

									OPEN @getResourceID
									FETCH NEXT
									FROM @getResourceID INTO @ResourceId
									WHILE @@FETCH_STATUS = 0
									BEGIN

										If (@ResourceId=1 or  @ResourceId=10 or @ResourceId=11)
										Begin
											Insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
											Values(@RoleId,@ResourceId,0)
										End
										Else
										Begin
											Insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
											Values(@RoleId,@ResourceId,1)
										End
										PRINT @ResourceId
										FETCH NEXT
										FROM @getResourceID INTO @ResourceId

									END
									CLOSE @getResourceID
									DEALLOCATE @getResourceID

									
									-- Auto Register of User

									Declare @UserId int
									If not exists(Select UserId from UserList 
													Where (Email=@Email or MobileNo=@MobileNo) 
													And rowstatus=1)
									Begin
										Insert into Userlist (CustId,Name,Designation,Username,[Password],Email,MobileNo,RoleId,
													[Status],CreatedBy,CreatedDate,rowstatus) 
										Values(@CustId,@Name,@Designation,@Name,@MobileNo,@Email,@MobileNo,@RoleId,
													1,@TrpUserId,GETDATE(),1)			
										Select @UserId=SCOPE_IDENTITY()
										If(@@ROWCOUNT>0)
										Begin
											Insert into UserAccess_GSTIN(GSTINId,CustId,UserId,CreatedDate)
											Values(@GSTINId,@CustId,@UserId,GETDATE())
										End
									End

									-- Inserting Audit Log for all above mentioned transactions

									Declare @msg varchar(max)
									Set @msg = 'Customer registration is done from TRP Portal: '+ @Name
									Exec [Ins_AuditLog] 1,@Name,@msg,'NA'

									Set @msg = 'Admin Role is created for the customer from TRP Portal: '+ @Name
									Exec [Ins_AuditLog] 1,@Name,@msg,'NA'

									Set @msg = 'Admin User creation is done from TRP Portal: '+ @Name
									Exec [Ins_AuditLog] 1,@Name,@msg,'NA'		

									Set @msg = 'GSTIN Registration is done from TRP Portal: '+ @GSTINNo
									Exec [Ins_AuditLog] 1,@Name,@msg,'NA'

									Set @msg = 'PAN Registration is done from TRP Portal: '+ @PANNo
									Exec [Ins_AuditLog] 1,@Name,@msg,'NA'

								End
							End
						End
					End
				End
			End

			If Not Exists (Select 1 From TBL_Cust_Settings Where CustId = @CustId)
			Begin

				Insert Into TBL_Cust_Settings
				(CustId,InvoicePrintRequired,GSTR3BAutoPopulate,CtinValdnCustMgmtReqd,
				CdnValdnOrigInum,ReconcilationSetting,CtinValdnSupMgmtReqd,
				HsnValdnHsnMstrReqd,TaxValCalnReqd,Eway_To_GSTR1,GSTR1_to_Eway,
				EwayPrint,rowstatus,CreatedBy,CreatedDate)
				Values(@CustId,0,0,0,
				0,0,0,
				0,0,0,0,
				0,1,@TrpUserId,GETDATE())

			End

			Set @RetValue=1 -- GSTIN No does not exist so inserting the customer details
			Print @RetValue	
		End
		Else
		Begin
			Set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
			Set @CustId=0
			Print @RetValue
		End
	End
	else 
	Begin
		Set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
		Set @CustId=0
		Print @RetValue
	End
	
	Return @RetValue
	Return @CustId

END