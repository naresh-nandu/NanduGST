

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve EWAYBILL Data
				
Written by  : raja.m@wepdigital.com 

Date		Who			Decription 
28/04/2018	Raja M	 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_EWB_UPD_VEHICLENO '05AAACC4214B1ZK', 1, 'EG'

 */
 
CREATE PROCEDURE [dbo].[usp_Retrieve_EWB_UPD_VEHICLENO]
	@userGSTIN nvarchar(15),
	@CustId int,
	@Mode varchar(5) /*PD-Print and Downoad
	                   EG-Edit and Generate */

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	--Declare @RoleName varchar(150)

	--DECLARE @TBL_Values TABLE 
	--(
	--	UserIds int
	--)
	--if exists( select 1  from MAS_Roles where role_id = (select roleid from userlist where userid = @CreatedBy))
	--Begin
	--	select @RoleName = Role_Name  from MAS_Roles where role_id = (select roleid from userlist where userid = @CreatedBy)		
	--End

	--If @RoleName = 'Admin' or @RoleName = 'Super Admin'
	--	Begin
	--		Insert Into @TBL_Values
	--		(UserIds)
	--		Select	userid from userlist where CustId = (select Custid from userlist where userid =@CreatedBy)
	--	End
	--Else

	--	Begin
	--		Insert Into @TBL_Values
	--		(UserIds)
	--		Select @CreatedBy
	--	End

	
	If(@Mode='PD')

		Select * From TBL_EWB_UPDATE_VEHICLENO 
		Where CustId = @CustId and EwbNo is not null
		--And createdby in (Select UserIds From @TBL_Values)
		Order By 1 Desc

    Else If(@Mode='EG')	
		Select * From TBL_EWB_UPDATE_VEHICLENO 
		Where userGSTIN = @userGSTIN
		AND CustId = @CustId
		Order By 1 Desc

	Return 0

End