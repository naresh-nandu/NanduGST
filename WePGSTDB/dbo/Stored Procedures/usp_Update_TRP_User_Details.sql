
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update TRP User Details
				
Written by  : Karthik

Date		Who			Decription 
11/11/2017	Karthik 	Initial Version
*/

/* Sample Procedure Call


exec usp_Update_TRP_User_Details 1,1,1,'D'
 */
CREATE PROCEDURE [usp_Update_TRP_User_Details] 
	@TrpCustId  int,
	@TrpUserId  int,
	@CreatedBy  int,
	@Action  varchar(1),
	@Name    nvarchar(150)= NULL,
	@Designation    varchar(50)= NULL,
	@Username    varchar(50) = NULL,
	@Email    nvarchar(250) = NULL,
	@MobileNo    varchar(50)= NULL,
	@RoleId    int = NULL,
	@RetValue	int= NULL Out,
	@RetMsg	varchar(250)=NULL Out
-- /*mssx*/ With Encryption 

AS
BEGIN
	
	If @Action = 'M'
		Begin
			If Ltrim(Rtrim(IsNull(@Email,''))) <> '' and Ltrim(Rtrim(IsNull(@MobileNo,''))) <> '' and Ltrim(Rtrim(IsNull(@RoleId,''))) <> ''
				Begin
					if exists(select 1 from TBL_TRP_Customer where trpCustId=@TrpCustId and Email = (select Email from TBL_TRP_UserList where TrpUserId =@TrpUserId and rowstatus =1) and rowstatus=1)
						Begin
							Update TBL_TRP_Customer set Email = @Email, MobileNo = @MobileNo where trpCustId=@TrpCustId and Email = (select Email from TBL_TRP_UserList where TrpUserId =@TrpUserId and rowstatus =1) and rowstatus=1
						End
							Update TBL_TRP_Userlist
								Set [Name] = @Name,
									Designation = @Designation,
									Username = @Username,
									Email = @Email,
									MobileNo = @MobileNo,
									RoleId = @RoleId
								Where TrpUserId = @TrpUserId and TrpCustId = @TrpCustId and rowstatus = 1
						Set @RetValue = 1
						Set @RetMsg = 'TRP User details Updated Successfully'

				End
			Else
				Begin
					Set @RetValue = -1
					Set @RetMsg = 'TRP User details Should not be empty'
				End
		End
	Else If @Action = 'D'
		Begin
			if Not exists(select 1 from TBL_TRP_Customer where trpCustId=@TrpCustId and Email = (select Email from TBL_TRP_UserList where TrpUserId =@TrpUserId and rowstatus =1) and rowstatus=1)
				Begin
						Update TBL_TRP_Userlist
								Set rowstatus = 0
							Where TrpUserId = @TrpUserId and TrpCustId = @TrpCustId and rowstatus = 1
						If @@RowCount >0
							Begin
								select * from TBL_TRP_UserAccess_Customer
								Update TBL_TRP_UserAccess_Customer set rowstatus = 0 
										Where TrpUserId = @TrpUserId and TrpId = @TrpCustId and rowstatus = 1
							End
								Set @RetValue = -2
								Set @RetMsg = 'TRP User Deleted Successfully'
				End
			Else
				Begin
					Set @RetValue = -3
					Set @RetMsg = 'Access Denied! Admin User details are not possible to delete.'
				End

		End
	Select convert(varchar(10),@RetValue) + ' : ' + @RetMsg as 'Output Result'
End