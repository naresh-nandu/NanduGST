
-- =============================================
-- Author:		KK
-- Create date: 11-Apr-17
-- Description:	Procedure to Approve the customer
-- exec Update_ApproveCustomer 1406,1,'fghdfgghf'
-- =============================================
CREATE PROCEDURE [dbo].[Update_ApproveCustomer]
@CustomerId	int,
@CreatedBy	int,
@GSTINUserName varchar(50)	
AS
BEGIN


	SET NOCOUNT ON;
	Declare @GSTINId int, @GSTINNo varchar(50), @Statecode varchar(50), @PANNo varchar(50),@Name nvarchar(50), @Designation varchar(50),@EmailId varchar(50),@MobileNo varchar(50) ,@RoleId int,
			@Address varchar(255)

	if exists (select * from TBL_Customer where CustId=@CustomerId and RowStatus=1)
	Begin
		select @GSTINNo=GSTINNo,@Statecode= Statecode,@PANNo=PANNo,@Name=Name,
		@Designation=Designation,@EmailId=Email,@MobileNo=MobileNo,
		@Address = Address
		from TBL_Customer where CustId=@CustomerId and RowStatus=1
		
		
		--Begin Transaction
		Update TBL_Customer set StatusCode=1,Approvedby=@CreatedBy,ApprovedDate=GETDATE(),ModifiedBy=@CreatedBy,ModifiedDate=GETDATE() where CustId=@CustomerId and RowStatus=1
		if (@@rowcount>0)
			Begin
				if not exists(select GSTINNo from TBL_Cust_GSTIN where GSTINNo=@GSTINNo and Statecode=@Statecode and PANNo=@PANNo and CustId=@CustomerId)
				Begin
				insert into TBL_Cust_GSTIN(GSTINNo,Statecode,PANNo,CustId,CreatedBy,CreatedDate,GSTINUserName,Address)values(@GSTINNo,@Statecode,@PANNo,@CustomerId,@CreatedBy,GETDATE(),@GSTINUserName,@Address)
				select @GSTINId=SCOPE_IDENTITY()
				if(@@ROWCOUNT>0)
				Begin

					if not exists(select Role_ID from MAS_Roles where Role_Name='Admin' and CustomerID=@CustomerId)
					Begin
						insert into MAS_Roles(Role_Name,CustomerID)values('Admin',@CustomerId)
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
								if not exists(select UserId from UserList where (Email=@EmailId or MobileNo=@MobileNo) and rowstatus=1)
										Begin
											insert into Userlist (CustId,Name,Designation,Username,[Password],Email,MobileNo,RoleId,
														[Status],CreatedBy,CreatedDate,rowstatus) 
														values(@CustomerId,@Name,@Designation,@Name,@MobileNo,@EmailId,@MobileNo,@RoleId,
														1,@CreatedBy,GETDATE(),1)			
												select @UserId=SCOPE_IDENTITY()


												if(@@ROWCOUNT>0)
													Begin
												
			                                          Insert into TBL_Cust_Settings(CustId,CreatedBy,CreatedDate)Values(@CustomerId,@UserId,GETDATE())
													   insert into UserAccess_GSTIN(GSTINId,CustId,UserId,CreatedDate)values(@GSTINId,@CustomerId,@UserId,GETDATE())
													End
										End
									End


					End
				End
			End
		End

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

END