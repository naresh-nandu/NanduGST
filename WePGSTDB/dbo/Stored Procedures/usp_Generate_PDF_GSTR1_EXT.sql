

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Generate the PDF for GSTR1 Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/10/2017	Karthik 	Initial Version
08/1/2017	Seshadri	Removed rowstatus check in the Where Clause
08/15/2017	Karthik		Modified to include RowStatus in the Where Clause
08/28/2017	Naresh		Modified the result set of CDNR

*/

/* Sample Procedure Call

exec usp_Generate_PDF_GSTR1_EXT 'Manual','27AHQPA7588L1ZJ'

 */
 
CREATE PROCEDURE [usp_Generate_PDF_GSTR1_EXT]
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

	if @ActionType = 'B2B'
	Begin
		Select
			gstin ,
			inum ,
			idt ,
			val ,
			'B2B' as'Action'  
		From TBL_EXT_GSTR1_B2B_INV 
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,inum,idt,val
		Order By idt Desc
	End
	Else if @ActionType ='B2CL'
	Begin
		Select
			gstin,
			inum ,
			idt ,
			val ,
			'B2CL' as'Action' 
		From TBL_EXT_GSTR1_B2CL_INV  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		--AND (val >= 250000 and pos <> convert(varchar(2),substring(gstin,1,2)))
		Group By gstin,inum,idt,val
		Order By idt Desc
	End
		Else if @ActionType ='B2CS'
	Begin
		Select
			gstin,
			inum ,
			idt ,
			val ,
			'B2CS' as'Action' 
		From TBL_EXT_GSTR1_B2CS_INV  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		--AND  (val < 250000 or pos = convert(varchar(2),substring(gstin,1,2)))
		Group By gstin,inum,idt,val
		--Union 
		--Select
		--	gstin,
		--	inum ,
		--	idt ,
		--	val ,
		--	'B2CS' as'Action' 
		--From TBL_EXT_GSTR1_B2CL_INV  
		--Where sourcetype = @SourceType
		--And referenceno = @ReferenceNo
		--And rowstatus <> 2
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		----And createdby = @CreatedBy
		--AND  (val < 250000 or pos = convert(varchar(2),substring(gstin,1,2)))
		----And Createddate <='12/01/2017'
		--And SourceType = 'Manual'
		----And Inum like '%CS%'
		--Group By gstin,inum,idt,val

	End
	Else if @ActionType ='CDNR'
	Begin
		Select
			gstin,
			inum ,
			idt ,
			val ,
			nt_num,
			nt_dt,
			'CDNR' as'Action'  
		From TBL_EXT_GSTR1_CDNR  
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,inum,idt,val,nt_num,nt_dt
		Order By nt_dt Desc
	End
	Else if @ActionType = 'EXP'
	Begin
		Select
			gstin ,
			inum ,
			idt ,
			val ,
			'EXP' as'Action'  
		From TBL_EXT_GSTR1_EXP_INV 
		Where sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus <> 2
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		--And createdby = @CreatedBy
		And createdby in (Select UserIds From @TBL_Values)
		Group By gstin,inum,idt,val
		Order By idt Desc
	End
		
	Return 0

End