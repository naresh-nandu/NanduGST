
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete B2CL Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Delete_OUTWARD_GSTR1_B2CL_Items 
 */
 
Create PROCEDURE [usp_Delete_OUTWARD_GSTR1_B2CL_Items]
		@Invid    int,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	
	Set Nocount on
	
	Declare @Gstin varchar(15),@Fp varchar(10)
	Declare @Inum varchar(50),@Idt varchar(15),@Val decimal(18,2),@Rt decimal(18,2) 
	Declare @Flag varchar(1)
	Declare @Nval decimal(18,2),@Ntxval decimal(18,2)
	Declare @Niamt decimal(18,2),@Ncamt decimal(18,2)
	Declare @Nsamt decimal(18,2),@Ncsamt decimal(18,2)

	
	if Exists(Select 1 from TBL_EXT_GSTR1_B2CL_INV 
			  Where invid = @invid
			  And rowstatus in (0,1)
			 )
	Begin

		Select  @Gstin = gstin,
				@Fp = fp,
				@Inum=inum,
				@Idt=idt,
				@Val=val, 
				@Rt=rt
		From TBL_EXT_GSTR1_B2CL_INV
		Where invid = @Invid
		
		Select @Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CL t2,
			 TBL_GSTR1_B2CL_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2clid = t3.b2clid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt

		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be deleted.
			Return
		End

		Delete From TBL_EXT_GSTR1_B2CL_INV
		Where invid = @Invid

 		Select @Nval = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) 
		From TBL_EXT_GSTR1_B2CL_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val

		Select	@Ntxval = sum(txval),
				@Niamt = sum(iamt),
				@Ncamt = sum(camt),
				@Nsamt = sum(samt),
				@Ncsamt = sum(csamt)
		From TBL_EXT_GSTR1_B2CL_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val
		And rt = @Rt

		Update TBL_EXT_GSTR1_B2CL_INV 
		Set val= @Nval
		Where inum= @Inum 
		And idt=@Idt
		And val=@Val

		-- Update the Values in Staging Area

		if Exists(Select 1 From TBL_EXT_GSTR1_B2CL_INV 
					Where inum= @Inum 
					And idt=@Idt
					And rt = @Rt)
		Begin
			Update TBL_GSTR1_B2CL_INV_ITMS_DET
			SET TBL_GSTR1_B2CL_INV_ITMS_DET.txval = @Ntxval,
				TBL_GSTR1_B2CL_INV_ITMS_DET.iamt = @Niamt,
				TBL_GSTR1_B2CL_INV_ITMS_DET.csamt = @Ncsamt
			FROM TBL_GSTR1 t1,
				 TBL_GSTR1_B2CL t2,
				 TBL_GSTR1_B2CL_INV t3 ,
				 TBL_GSTR1_B2CL_INV_ITMS t4,
				 TBL_GSTR1_B2CL_INV_ITMS_DET t5
			Where t1.gstr1id = t2.gstr1id
			And t2.b2clid = t3.b2clid
			And t3.invid = t4.invid
			And t4.itmsid = t5.itmsid
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t3.inum = @Inum
			And t3.idt = @Idt
			And t5.rt = @Rt
		End
		else
		Begin
			Delete TBL_GSTR1_B2CL_INV_ITMS_DET
			FROM TBL_GSTR1 t1,
				 TBL_GSTR1_B2CL t2,
				 TBL_GSTR1_B2CL_INV t3 ,
				 TBL_GSTR1_B2CL_INV_ITMS t4,
				 TBL_GSTR1_B2CL_INV_ITMS_DET t5
			Where t1.gstr1id = t2.gstr1id
			And t2.b2clid = t3.b2clid
			And t3.invid = t4.invid
			And t4.itmsid = t5.itmsid
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t3.inum = @Inum
			And t3.idt = @Idt
			And t5.rt = @Rt
		End

		Update TBL_GSTR1_B2CL_INV
		SET TBL_GSTR1_B2CL_INV.val = @Nval
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CL t2,
			 TBL_GSTR1_B2CL_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2clid = t3.b2clid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt
	
	
		Select @Retval = 1 -- Item details deleted and also the Invoice Value updated for the entire Invoice

	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End


End