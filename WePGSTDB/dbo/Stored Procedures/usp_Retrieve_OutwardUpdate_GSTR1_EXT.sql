

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retreive AT,TXP,Nil,DocIssue for GSTR1 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/12/2017	Naresh 	Initial Version


*/

/* Sample Procedure Call

exec usp_Retrieve_OutwardUpdate_GSTR1_EXT 'Manual','27AHQPA7588L1ZJ'

 */
 
CREATE PROCEDURE [usp_Retrieve_OutwardUpdate_GSTR1_EXT]
	@SourceType varchar(15), -- Manual  
	@ReferenceNo varchar(50),
	@ActionType varchar(15),
	@CreatedBy int

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @RoleName varchar(150)

	DECLARE @TBL_Values TABLE 
	(
		UserIds int
	)
	if exists( select 1  from MAS_Roles where role_id = (select roleid from userlist where userid =@CreatedBy))
	Begin
		select @RoleName = Role_Name  from MAS_Roles where role_id = (select roleid from userlist where userid =@CreatedBy)		
	End

	If @RoleName = 'Admin'
		Begin
			Insert Into @TBL_Values
			(UserIds)
			Select	userid from userlist where CustId = (select Custid from userlist where userid =@CreatedBy)
		End
	Else
		Begin
			Insert Into @TBL_Values
			(UserIds)
			Select @CreatedBy
		End


	if @ActionType = 'AT'
	Begin
		Select
			gstin ,
			fp ,
			'AT' as'Action'  
		From TBL_EXT_GSTR1_AT 
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,fp
		Order By fp Desc
	End

	Else if @ActionType ='TXP'
	Begin
		Select
			gstin ,
			fp ,
			'TXP' as'Action' 
		From TBL_EXT_GSTR1_TXP  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,fp
		Order By fp Desc
	End

		Else if @ActionType ='NIL'
	Begin
		Select
			gstin ,
			fp ,
			sply_ty,
			'NIL' as'Action'
		From TBL_EXT_GSTR1_NIL  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,fp,sply_ty
		Order By fp Desc
	End

	Else if @ActionType ='DOCIssue'
	Begin
		Select
			gstin ,
			fp ,
			'DOCIssue' as'Action' 
		From TBL_EXT_GSTR1_DOC  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,fp
		Order By fp Desc
	End

	
	Return 0

End