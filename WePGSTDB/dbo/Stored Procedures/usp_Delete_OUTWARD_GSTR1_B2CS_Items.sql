

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete B2CS Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
11/28/2017	Karthik		Initial Version
11/30/2017	Seshadri	Fine tuned the code
12/18/2017	Seshadri	Fixed the issue of invoice details not getting reflected
						in the consolidated B2CS dataset of SA area

*/

/* Sample Procedure Call

exec usp_Delete_OUTWARD_GSTR1_B2CS_Items 
 */
 
CREATE PROCEDURE [usp_Delete_OUTWARD_GSTR1_B2CS_Items]
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

	Declare @Gstinid int, @Gstr1id int, @B2csid int, @InvoiceId int
	Declare @PlaceOfSupply varchar(2), @SupplyType varchar(5), @InvoiceValue dec(18,2)


	
	if Exists(Select 1 from TBL_EXT_GSTR1_B2CS_INV 
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
		From TBL_EXT_GSTR1_B2CS_INV
		Where invid = @Invid
		
		Select @Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CS_N t2,
			 TBL_GSTR1_B2CS_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2csid = t3.b2csid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt

		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be deleted.
			Return
		End

		Delete From TBL_EXT_GSTR1_B2CS_INV
		Where invid = @Invid

 		Select @Nval = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) 
		From TBL_EXT_GSTR1_B2CS_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val

		Select	@Ntxval = sum(txval),
				@Niamt = sum(iamt),
				@Ncamt = sum(camt),
				@Nsamt = sum(samt),
				@Ncsamt = sum(csamt)
		From TBL_EXT_GSTR1_B2CS_INV 
		Where inum= @Inum 
		And idt=@Idt 
		And val=@Val
		And rt = @Rt

		Update TBL_EXT_GSTR1_B2CS_INV 
		Set val= @Nval
		Where inum= @Inum 
		And idt=@Idt
		And val=@Val

		-- Update the Values in Staging Area

		-- B2CS Consolidated DataSet Updation (Begin)

		Select  @Gstinid = t1.gstinid,
				@Gstr1id = t1.gstr1id,
				@B2CSid = t2.b2csid,
				@PlaceOfSupply = t2.pos,
				@SupplyType = t3.sply_ty,
				@InvoiceValue = t3.val,
				@InvoiceId = invid,
				@Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CS_N t2,
			TBL_GSTR1_B2CS_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2csid = t3.b2csid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.inum = @Inum
		And t3.idt = @Idt

		Select rt,txval,iamt,camt,samt,csamt
		Into #Tbl_Inv_Itms
		From TBL_GSTR1_B2CS_INV_ITMS_DET
		Where itmsid in (Select itmsid From TBL_GSTR1_B2CS_INV_ITMS
						 Where invid = @InvoiceId)

		if Exists(Select 1 From #Tbl_Inv_Itms)
		Begin

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
			And t3.rt = t2.rt
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t2.pos = @PlaceOfSupply
			And t2.sply_ty = @SupplyType
			And t3.rt = @Rt	

		End

		-- B2CS Consolidated DataSet Updation (End)

		if Exists(Select 1 From TBL_EXT_GSTR1_B2CS_INV 
					Where inum= @Inum 
					And idt=@Idt
					And rt = @Rt)
		Begin

			Update TBL_GSTR1_B2CS_INV_ITMS_DET
			SET TBL_GSTR1_B2CS_INV_ITMS_DET.txval = @Ntxval,
				TBL_GSTR1_B2CS_INV_ITMS_DET.iamt = @Niamt,
				TBL_GSTR1_B2CS_INV_ITMS_DET.camt = @Ncamt,
				TBL_GSTR1_B2CS_INV_ITMS_DET.samt = @Nsamt,
				TBL_GSTR1_B2CS_INV_ITMS_DET.csamt = @Ncsamt
			FROM TBL_GSTR1 t1,
				 TBL_GSTR1_B2CS_N t2,
				 TBL_GSTR1_B2CS_INV t3 ,
				 TBL_GSTR1_B2CS_INV_ITMS t4,
				 TBL_GSTR1_B2CS_INV_ITMS_DET t5
			Where t1.gstr1id = t2.gstr1id
			And t2.b2csid = t3.b2csid
			And t3.invid = t4.invid
			And t4.itmsid = t5.itmsid
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t3.inum = @Inum
			And t3.idt = @Idt
			And t5.rt = @Rt

			-- B2CS Consolidated DataSet Updation (Begin)

			Update TBL_GSTR1_B2CS 
				Set	txval = IsNull(t2.txval,0) + IsNull(@Ntxval,0), 
					iamt = IsNull(t2.iamt,0) + IsNull(@Niamt,0),
					camt = IsNull(t2.camt,0) + IsNull(@Ncamt,0),
					samt = IsNull(t2.samt,0) + IsNull(@Nsamt,0), 
					csamt = IsNull(t2.csamt,0) + IsNull(@Ncsamt,0) 
			From TBL_GSTR1 t1,
				 TBL_GSTR1_B2CS  t2
			Where t2.gstr1id = t1.gstr1id
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t2.pos = @PlaceOfSupply
			And t2.sply_ty = @SupplyType
			And t2.rt = @Rt	

			if Exists(	Select 1  
						From TBL_GSTR1 t1,
							 TBL_GSTR1_B2CS  t2
						Where t2.gstr1id = t1.gstr1id
						And t1.gstin = @Gstin
						And t1.fp = @Fp
						And t2.pos = @PlaceOfSupply
						And t2.sply_ty = @SupplyType
						And t2.rt = @Rt
						And IsNull(t2.txval,0) = 0
						And IsNull(t2.iamt,0) = 0
						And IsNull(t2.camt,0) = 0
						And IsNull(t2.samt,0) = 0
						And IsNull(t2.csamt,0) = 0)
			Begin
			
				Delete TBL_GSTR1_B2CS 
				From TBL_GSTR1 t1,
					 TBL_GSTR1_B2CS  t2
				Where t2.gstr1id = t1.gstr1id
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t2.pos = @PlaceOfSupply
				And t2.sply_ty = @SupplyType
				And t2.rt = @Rt	

			End	

			-- B2CS Consolidated DataSet Updation (End)
	
		End
		else
		Begin

			Delete TBL_GSTR1_B2CS_INV_ITMS_DET
			FROM TBL_GSTR1 t1,
				 TBL_GSTR1_B2CS_N t2,
				 TBL_GSTR1_B2CS_INV t3 ,
				 TBL_GSTR1_B2CS_INV_ITMS t4,
				 TBL_GSTR1_B2CS_INV_ITMS_DET t5
			Where t1.gstr1id = t2.gstr1id
			And t2.b2csid = t3.b2csid
			And t3.invid = t4.invid
			And t4.itmsid = t5.itmsid
			And t1.gstin = @Gstin
			And t1.fp = @Fp
			And t3.inum = @Inum
			And t3.idt = @Idt
			And t5.rt = @Rt

			-- B2CS Consolidated DataSet Updation (Begin)

			if Exists(	Select 1  
						From TBL_GSTR1 t1,
							 TBL_GSTR1_B2CS  t2
						Where t2.gstr1id = t1.gstr1id
						And t1.gstin = @Gstin
						And t1.fp = @Fp
						And t2.pos = @PlaceOfSupply
						And t2.sply_ty = @SupplyType
						And t2.rt = @Rt
						And IsNull(t2.txval,0) = 0
						And IsNull(t2.iamt,0) = 0
						And IsNull(t2.camt,0) = 0
						And IsNull(t2.samt,0) = 0
						And IsNull(t2.csamt,0) = 0)
			Begin
			
				Delete TBL_GSTR1_B2CS 
				From TBL_GSTR1 t1,
					 TBL_GSTR1_B2CS  t2
				Where t2.gstr1id = t1.gstr1id
				And t1.gstin = @Gstin
				And t1.fp = @Fp
				And t2.pos = @PlaceOfSupply
				And t2.sply_ty = @SupplyType
				And t2.rt = @Rt	

			End	

			-- B2CS Consolidated DataSet Updation (End)
	
		End

		Update TBL_GSTR1_B2CS_INV
		SET TBL_GSTR1_B2CS_INV.val = @Nval
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_B2CS_N t2,
			 TBL_GSTR1_B2CS_INV  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.b2csid = t3.b2csid
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