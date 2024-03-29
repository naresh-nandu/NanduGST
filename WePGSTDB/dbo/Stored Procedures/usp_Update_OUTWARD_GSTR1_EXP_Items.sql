﻿/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update EXP Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_EXP_Items 
 */
 
CREATE PROCEDURE [usp_Update_OUTWARD_GSTR1_EXP_Items]
		@Invid    int,
		@Hsncode	varchar(50),
		@Hsndesc	varchar(250),
		@Qty	decimal(18,2),
		@Unitprice decimal(18,2),
		@Discount decimal(18,2),
		@Uqc varchar(50),
		@Rt		decimal(18,2),
		@Txval	decimal(18,2),
		@Iamt	decimal(18,2),
		@Createdby int,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	
	Set Nocount on
	
	Declare @Gstin varchar(15),@Fp varchar(10)
	Declare @Inum varchar(50),@Idt varchar(15),@Val decimal(18,2) 
	Declare @Flag varchar(1)
	Declare @Nval decimal(18,2),@Ntxval decimal(18,2)
	Declare @Niamt decimal(18,2)

	
	if Exists(Select 1 from TBL_EXT_GSTR1_EXP_INV 
			  Where invid = @invid
			  And rowstatus in (0,1)
			 )
	Begin

		Select  @Gstin = gstin,
				@Fp = fp,
				@Inum=inum,
				@Idt=idt,
				@Val=val 
		From TBL_EXT_GSTR1_EXP_INV
		Where invid = @Invid 

		Select @Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_EXP t2,
			 TBL_GSTR1_EXP_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.expid = t3.expid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt
	
		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be updated.
			Return
		End

		Update TBL_EXT_GSTR1_EXP_INV 
		Set hsncode = @Hsncode,
			hsndesc = @Hsndesc,
			qty = @Qty,
			unitprice = @Unitprice,
			discount = @Discount,
			uqc= @Uqc,
			rt = @Rt,
			txval = @Txval,
			iamt= @Iamt
		where invid = @Invid 
		
		Select @Nval = (sum(txval)+sum(iamt)) 
		From TBL_EXT_GSTR1_EXP_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val

		Select	@Ntxval = sum(txval),
				@Niamt = sum(iamt)
		From TBL_EXT_GSTR1_EXP_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val
		And rt = @Rt

		Update TBL_EXT_GSTR1_EXP_INV 
		Set val= @Nval
		Where inum= @Inum 
		And idt=@Idt
		And val=@Val

		-- Update the Values in Staging Area

		Update TBL_GSTR1_EXP_INV_ITMS
		SET TBL_GSTR1_EXP_INV_ITMS.txval = @Ntxval,
			TBL_GSTR1_EXP_INV_ITMS.iamt = @Niamt
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_EXP t2,
			 TBL_GSTR1_EXP_INV t3 ,
			 TBL_GSTR1_EXP_INV_ITMS t4			 
		Where t1.gstr1id = t2.gstr1id
		And t2.expid = t3.expid
		And t3.invid = t4.invid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt
		And t4.rt = @Rt

		Update TBL_GSTR1_EXP_INV
		SET TBL_GSTR1_EXP_INV.val = @Nval
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_EXP t2,
			 TBL_GSTR1_EXP_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.expid = t3.expid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt
	
	
		Select @Retval = 1 -- Item details updated and also the Invoice Value updated for the entire Invoice

	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End


End