/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert Customer Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/11/2017	Karthik			Initial Version
04/05/2018  Karthik			Including Partner ID reference key for the API Calls


*/

/* Sample Procedure Call  

exec USP_Insert_CustomerRegistration 17,1,1

 */

CREATE PROCEDURE [dbo].[USP_Insert_CustomerRegistration]
	@Name    varchar(50),
	@Designation    varchar(50)= NULL,
	@Company    varchar(50),
	@GSTINNo    varchar(50)=NULL,
	@Email    varchar(50),
	@MobileNo    varchar(50),
	@PANNo    varchar(50)=NULL,
	@Statecode    varchar(50)=NULL,
	@ValidFrom    datetime = NULL,
	@ValidTo    datetime = NULL,
	@AadharNo	varchar(12) = NULL,
	@Referenceno varchar(50) = NULL,
	@Address varchar(255),
	@GSTINUserName varchar(50)=NULL,
	@PartnerId	int = NULL,
	@RetValue	int = Null out,
	@RetMessage	varchar(255)= NULL Out,
	@CustId		int = NULL out 
	

AS
BEGIN
	Declare @GSTINId int,@RoleId int,@AutoGSTINno varchar(15),@CustCount int

	Select @CustCount = convert(varchar(10),Max(CustId)+ 1) 
		From TBL_Customer 

	-- GSTINNO Check
			if ltrim(rtrim(isnull(@GSTINNo,''))) = ''
				Begin	
					Select  convert(varchar(10),FORMAT(convert(int,@CustCount),'NUM0000000'))			
					set @AutoGSTINno = '29GST'+ convert(varchar(10),FORMAT(convert(int,@CustCount),'NUM0000000'))
					Set @GSTINNo = @AutoGSTINno
				End
			-- PAN Check
			 If ltrim(rtrim(isnull(@PANNo,''))) = ''
				 Begin
					if ltrim(rtrim(isnull(@AutoGSTINno,''))) <> ''
						Begin
							Select @PANNO = convert(varchar(10),FORMAT(convert(int,@CustCount),'NUM0000000'))
						End
					Else
						Begin
							Select @PANNO = Substring(@GSTINNo, 3, 10)
						End
				 End
			-- State code check
			 If ltrim(rtrim(isnull(@Statecode,''))) = ''
				 Begin
					if ltrim(rtrim(isnull(@AutoGSTINno,''))) <> ''
						Begin
							Select @Statecode = '29'
						End
					Else
						Begin
							Select @Statecode =  Substring(@GSTINNo, 1, 2)
						End			
				End



	if not exists(select CustId from TBL_Customer where (GSTINNo=UPPER(@GSTINNo) or PANNO = UPPER(@PANNo) or MobileNo=@MobileNo or Email=@Email))
	begin

		if not exists(select 1 from Userlist where  MobileNo=@MobileNo or Email=@Email)
		Begin

		insert into TBL_Customer (Name,
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
							partnerid
							) values(
							@Name,
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
							@PartnerId
							)
			select @CustId = SCOPE_IDENTITY()

			if ltrim(rtrim(isnull(@Referenceno,''))) <> ''
				Begin
					update TBL_Customer set Referenceno=@Referenceno where custid =@CustId
				End
			Else
				Begin
					set @Referenceno = 'WEP00'+ CAST(@CustId AS VARCHAR(10))
					--set @Referenceno = 'WEP00'+ convert(varchar,10,@CustId)
					update TBL_Customer set Referenceno=@Referenceno where custid =@CustId
				End
			
				-- Adding GSTIN 
				if not exists(select GSTINNo from TBL_Cust_GSTIN where GSTINNo=@GSTINNo and Statecode=@Statecode and PANNo=@PANNo and CustId=@CustId and rowstatus =1)
				Begin
					insert into TBL_Cust_GSTIN(GSTINNo,Statecode,PANNo,CustId,CreatedBy,CreatedDate,GSTINUserName,Address,rowstatus)values(@GSTINNo,@Statecode,@PANNo,@CustId,1,GETDATE(),@GSTINUserName,@Address,1)
					select @GSTINId=SCOPE_IDENTITY()
						if(@@ROWCOUNT>0)
						Begin		
						-- Adding PAN No
							if not exists(select PANNO from TBL_Cust_PAN where PANNO = @PANNo and custid = @CustId and rowstatus =1)
							Begin
								insert into TBL_Cust_PAN(PANNo,CustId,CompanyName,CreatedBy,CreatedDate,rowstatus)values(@PANNo,@CustId,@Company,1,GETDATE(),1)
									if(@@ROWCOUNT>0)
									Begin
										if not exists(select Role_ID from MAS_Roles where Role_Name='Admin' and CustomerID=@CustId)
										Begin
										-- Creating default Role based on customer
											insert into MAS_Roles(Role_Name,CustomerID)values('Admin',@CustId)
											select @RoleId=SCOPE_IDENTITY()
											if(@@ROWCOUNT>0)
												Begin
													DECLARE @ResourceId INT
													DECLARE @getResourceID CURSOR
													SET @getResourceID = CURSOR FOR
													SELECT Resource_ID FROM MAS_Resources
													OPEN @getResourceID
													FETCH NEXT
													FROM @getResourceID INTO @ResourceId
													WHILE @@FETCH_STATUS = 0
														BEGIN
														-- Auto create the resources
															if (@ResourceId=1 or  @ResourceId=10 or @ResourceId=11 or @ResourceId=85)
																Begin
																	insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
																	values(@RoleId,@ResourceId,0)
																	End
																Else
																	Begin
																		insert into MAS_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
																	values(@RoleId,@ResourceId,1)
																	End
															PRINT @ResourceId
														FETCH NEXT
														FROM @getResourceID INTO @ResourceId
													END
												CLOSE @getResourceID
												DEALLOCATE @getResourceID
													Declare @UserId int
													-- Auto Register of User
													if not exists(select UserId from UserList where (Email=@Email or MobileNo=@MobileNo) and rowstatus=1)
															Begin
																insert into Userlist (CustId,Name,Designation,Username,[Password],Email,MobileNo,RoleId,
																			[Status],CreatedBy,CreatedDate,rowstatus) 
																			values(@CustId,@Name,@Designation,@Name,@MobileNo,@Email,@MobileNo,@RoleId,
																			1,1,GETDATE(),1)			
																	select @UserId=SCOPE_IDENTITY()
																	if(@@ROWCOUNT>0)
																		Begin
																		Insert into TBL_Cust_Settings(CustId,CreatedBy,CreatedDate)Values(@CustId,@UserId,GETDATE())
																		insert into UserAccess_GSTIN(GSTINId,CustId,UserId,CreatedDate)values(@GSTINId,@CustId,@UserId,GETDATE())
																		End
															End

															-- Inserting Audit Log for all above mentioned transactions
															declare @msg varchar(max)
															set @msg = 'Customer registration is done : '+ @Name
															exec [Ins_AuditLog] 1,@Name,@msg,'NA'

															set @msg = 'Admin Role is created for the customer : '+ @Name
															exec [Ins_AuditLog] 1,@Name,@msg,'NA'

															set @msg = 'Admin User creation is done : '+ @Name
															exec [Ins_AuditLog] 1,@Name,@msg,'NA'		

															set @msg = 'GSTIN Registration is done : '+ @GSTINNo
															exec [Ins_AuditLog] 1,@Name,@msg,'NA'

															set @msg = 'PAN Registration is done : '+ @PANNo
															exec [Ins_AuditLog] 1,@Name,@msg,'NA'

												End
										End
									End
								End
							End
End




			set @RetValue=1 -- GSTIN No not exist so inserting the customer details
			set @RetMessage = 'Customer Registration done successfully'
			print @RetValue	
		End
		Else
		Begin
			set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
			set @RetMessage = 'Customer GSTINno or Email or PhoneNo. already exists.'
			set @CustId=0
			print @RetValue
		End
	End
	else 
		Begin
			set @RetValue=2	-- GSTIN No or Email or phone no for this Customer already exists in the system.
			set @RetMessage = 'Customer GSTINno or Email or PhoneNo. already exists.'
			set @CustId=0
			print @RetValue
		End
	
	return @RetValue
	return @CustId
END