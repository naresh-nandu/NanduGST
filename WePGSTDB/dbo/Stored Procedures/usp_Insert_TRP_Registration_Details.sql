
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to insert the TRP_Customer details into Tbl_TRPCustomer Table
				
Written by  : Karthik

Date		Who			Decription 
11/06/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call

-- exec usp_Insert_TRP_Registration_Details  'Naresh Soft Tech','Naresh','29AWJPN5651K1Z9','nareshn@wepindia.com','9553737452','AWJPN5651K',29,'04/01/2017','04/25/2017','888823304248',' ','MG Road'
*/
CREATE PROCEDURE [usp_Insert_TRP_Registration_Details] 
	@Company    varchar(50),
	@ContactName    varchar(50),
	@GSTINNo   varchar(15),
	@Email    varchar(50),
	@MobileNo    varchar(50),
	@PanNo    varchar(50),
	@Statecode    varchar(50),
	@ValidFrom    datetime,
	@ValidTo    datetime,
	@AadharNo	varchar(12),
	@Referenceno varchar(50),
	@Address varchar(255),
	@RetValue	int = Null out,
	@RetMessage	varchar(255)= NULL Out,
	@TrpCustId		int = NULL out 
	

AS
BEGIN
	
	if not exists(select trpCustId from TBL_TRP_Customer where ( MobileNo=@MobileNo or Email=@Email) and rowstatus=1 )
	begin

		if not exists(select trpUserId from TBL_TRP_Userlist where (Email=@Email or MobileNo=@MobileNo)  and rowstatus=1)
			Begin

			insert into TBL_TRP_Customer (Name,
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
							[Address]
							) values(
							@ContactName,
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
							@Address
							)
			select @TrpCustId = SCOPE_IDENTITY()

			if ltrim(rtrim(isnull(@Referenceno,''))) <> ''
			Begin
				update TBL_TRP_Customer set Referenceno=@Referenceno where trpCustId =@TrpCustId
			End
			Else
			Begin
				set @Referenceno = 'TRP00'+ CAST(@TrpCustId AS VARCHAR(10))
				--set @Referenceno = 'WEP00'+ convert(varchar,10,@TrpCustId)
				update TBL_TRP_Customer set Referenceno=@Referenceno where trpCustId =@TrpCustId
			End

			select * from  tbl_trp_roles
			declare @RoleId int
			if not exists(select Role_ID from tbl_trp_roles where Role_Name='Admin' and TRPId=@TrpCustId)
				Begin
				-- Creating default Role based on customer
					insert into tbl_trp_roles(Role_Name,TRPId)values('Admin',@TrpCustId)
					select @RoleId=SCOPE_IDENTITY()
				End
			Else
			Begin
				select @RoleId = Role_ID from tbl_trp_roles where Role_Name='Admin' and TRPId=@TrpCustId
			End
										
				DECLARE @ResourceId INT
				DECLARE @getResourceID CURSOR
				SET @getResourceID = CURSOR FOR
				SELECT Resource_ID FROM TBL_TRP_Resources
				OPEN @getResourceID
				FETCH NEXT
				FROM @getResourceID INTO @ResourceId
				WHILE @@FETCH_STATUS = 0
					BEGIN
					-- Auto create the resources
						if (@ResourceId=1 or  @ResourceId=7 or @ResourceId=8)
							Begin
								insert into TBL_TRP_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
								values(@RoleId,@ResourceId,0)
								End
							Else
								Begin
									insert into TBL_TRP_Roles_Resources (FK_Role_Resource_Role_ID,FK_Role_Resource_Resource_ID,Role_Resource_IsAssigned)
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
				if not exists(select TRPUserId from TBL_TRP_UserList where (Email=@Email or MobileNo=@MobileNo) and rowstatus=1)
					Begin
						insert into TBL_TRP_UserList (trpCustId,Name,Designation,Username,[Password],Email,MobileNo,RoleId,
									[Status],CreatedBy,CreatedDate,rowstatus) 
									values(@TrpCustId,@ContactName,'','',@MobileNo,@Email,@MobileNo,@RoleId,
									1,1,GETDATE(),1)
						select @UserId = SCOPE_IDENTITY()												
						
					End


					-- Inserting Audit Log for all above mentioned transactions
				declare @msg varchar(max)
				set @msg = 'TRP registration is done in TRP Portal: '+ @ContactName
				exec USP_Insert_TRP_AuditLog @UserId,@ContactName,@msg,'NA'

				set @msg = 'Admin Role is created for the TRP :'+ @ContactName+' in TRP Portal'
				exec USP_Insert_TRP_AuditLog @UserId,@ContactName,@msg,'NA'

				set @msg = 'Admin User '+ @ContactName +' creation in TRP Portal' 
				exec USP_Insert_TRP_AuditLog @UserId,@ContactName,@msg,'NA'		


			set @RetValue=1
			set @RetMessage = 'TRP Registration done successfully'
			print @RetValue	
		End
		Else
		Begin
			set @RetValue=2	
			set @RetMessage = 'TRP Customer already exists.'
			set @TrpCustId=0
			print @RetValue
		End
	End
	else 
		Begin
			set @RetValue=2	
			set @RetMessage = 'TRP Customer already exists.'
			set @TrpCustId=0
			print @RetValue
		End
	
	return @RetValue
	return @RetMessage
	return @TrpCustId
END