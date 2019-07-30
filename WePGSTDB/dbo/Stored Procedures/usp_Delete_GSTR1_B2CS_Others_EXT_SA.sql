/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete all B2CS GSTR1 Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
12/18/2017	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Delete_GSTR1_B2CS_Others_EXT_SA 
 */

Create PROCEDURE [usp_Delete_GSTR1_B2CS_Others_EXT_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption  
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	DECLARE @TBL_Values TABLE 
	(
		RefId int,
		Gstin varchar(15),
		Fp varchar(10),
		Pos varchar(2),
		Sply_Ty varchar(5)
	)

	Select @Delimiter = ','
	
	Insert Into @TBL_Values
	(RefId)
	Select	Value 
	From string_split( @RefIds,@Delimiter) 

	
	Update @TBL_Values
	Set Gstin = @Gstin,
		Fp = @Fp,
		Pos = t1.pos,
		Sply_Ty = t1.sply_ty
	From TBL_EXT_GSTR1_B2CS t1, 
		 @TBL_Values t2
	Where t2.refid = t1.invid 
	And t1.gstin = @Gstin
	And t1.fp = @Fp 

	Select gstin,fp,invid,sply_ty,pos,rt,txval,iamt,camt,samt,csamt
	Into #Tbl_Inv_Itms
	From TBL_EXT_GSTR1_B2CS 
	Where invid In (Select RefId From @TBL_Values )
	And gstin = @Gstin
	And fp = @Fp 

	if Exists(Select 1 From #Tbl_Inv_Itms)
	Begin
	
		-- Updating B2CS Consolidated Data Set - SA 
	
		Update TBL_GSTR1_B2CS 
		Set	txval = IsNull(t2.txval,0) - IsNull(t3.txval,0), 
			iamt = IsNull(t2.iamt,0) - IsNull(t3.iamt,0),
			camt = IsNull(t2.camt,0) - IsNull(t3.camt,0),
			samt = IsNull(t2.samt,0) - IsNull(t3.samt,0), 
			csamt = IsNull(t2.csamt,0) - IsNull(t3.csamt,0) 
		From TBL_GSTR1 t1,
			 TBL_GSTR1_B2CS  t2,
			 #Tbl_Inv_Itms t3
		Where t2.gstr1id = t1.gstr1id
		And t3.gstr1id = t2.gstr1id
		And t3.pos = t2.pos
		And t3.sply_ty = t2.sply_ty 
		And t3.rt = t2.rt
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		
		if @@rowcount > 0
		Begin
			
			-- Deleting from Ext Area

			Update TBL_EXT_GSTR1_B2CS
			Set rowstatus = 2
			From  TBL_EXT_GSTR1_B2CS t1,
	  			  @TBL_Values t2
			Where t1.gstin = t2.Gstin
			And t1.fp = t2.Fp
			And t1.pos = t2.Pos
			And t1.sply_ty = t2.Sply_Ty
		End

	End


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End