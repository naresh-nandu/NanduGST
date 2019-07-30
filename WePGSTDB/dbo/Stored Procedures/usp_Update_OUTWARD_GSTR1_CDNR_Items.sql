
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update CDNR Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_CDNR_Items 
 */
 
Create PROCEDURE [usp_Update_OUTWARD_GSTR1_CDNR_Items]
			@Cdnrid	int,
			@Rt decimal(18, 2),
			@Txval decimal(18, 2),
			@Iamt decimal(18, 2),
			@Camt decimal(18, 2),
			@Samt decimal(18, 2),
			@Csamt decimal(18, 2),
			@Referenceno varchar(50),
			@Hsncode varchar(50),
			@Hsndesc varchar(255),
			@Qty decimal(18, 2),
			@Unitprice decimal(18, 2),
			@Discount decimal(18, 2),
			@Uqc varchar(50),
			@Createdby int,
			@Retval int = null out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	 
	Declare @Gstin varchar(15),@Fp varchar(10)
	Declare @Inum varchar(50),@Idt varchar(15),@Val decimal(18,2) 
	Declare @Nt_Num varchar(50),@Nt_Dt varchar(15)
	Declare @Flag varchar(1)
	Declare @Nval decimal(18,2),@Ntxval decimal(18,2)
	Declare @Niamt decimal(18,2),@Ncamt decimal(18,2)
	Declare @Nsamt decimal(18,2),@Ncsamt decimal(18,2)

	if Exists(	Select 1 from TBL_EXT_GSTR1_CDNR 
				Where cdnrid = @Cdnrid
				And rowstatus in (0,1)
			 )
	Begin

		Select  @Gstin = gstin,
				@Fp = fp,
				@Inum=inum,
				@Idt=idt,
				@Val=val,
				@Nt_Num = nt_num,
				@Nt_Dt = nt_dt 
		From TBL_EXT_GSTR1_CDNR
		Where cdnrid= @Cdnrid 

		Select @Flag = flag
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_CDNR t2,
			 TBL_GSTR1_CDNR_NT  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.cdnrid = t3.cdnrid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.nt_num = @Nt_num
		And t3.nt_dt = @Nt_dt
	
		If IsNull(@Flag,'') <> ''
		Begin
			Select @Retval = -2 -- Item details cannot be updated.
			Return
		End

		Update TBL_EXT_GSTR1_CDNR 
		Set rt= @Rt, 
			txval=@Txval, 
			iamt=@Iamt, 
			camt=@Camt, 
			samt=@Samt,
			csamt=@Csamt,
			hsncode=@Hsncode,
			hsndesc=@Hsndesc,
			qty=@Qty,
			unitprice=@Unitprice,
			discount=@Discount,
			uqc=@Uqc 
		Where cdnrid= @Cdnrid 
			
		Select @Nval = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) 
		From TBL_EXT_GSTR1_CDNR 
		Where nt_num= @Nt_Num 
		And nt_dt=@Nt_Dt 
		And val=@Val

		Select	@Ntxval = sum(txval),
				@Niamt = sum(iamt),
				@Ncamt = sum(camt),
				@Nsamt = sum(samt),
				@Ncsamt = sum(csamt)
		From TBL_EXT_GSTR1_CDNR  
		Where nt_num= @Nt_Num  
		And nt_dt=@Nt_Dt 
		And val=@Val
		And rt = @Rt

		Update TBL_EXT_GSTR1_CDNR  
		Set val= @Nval
		Where nt_num= @Nt_Num  
		And nt_dt=@Nt_Dt 
		And val=@Val

		-- Update the Values in Staging Area

		Update TBL_GSTR1_CDNR_NT_ITMS_DET
		SET TBL_GSTR1_CDNR_NT_ITMS_DET.txval = @Ntxval,
			TBL_GSTR1_CDNR_NT_ITMS_DET.iamt = @Niamt,
			TBL_GSTR1_CDNR_NT_ITMS_DET.camt = @Ncamt,
			TBL_GSTR1_CDNR_NT_ITMS_DET.samt = @Nsamt,
			TBL_GSTR1_CDNR_NT_ITMS_DET.csamt = @Ncsamt
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_CDNR t2,
			 TBL_GSTR1_CDNR_NT t3 ,
			 TBL_GSTR1_CDNR_NT_ITMS t4,
			 TBL_GSTR1_CDNR_NT_ITMS_DET t5
		Where t1.gstr1id = t2.gstr1id
		And t2.cdnrid = t3.cdnrid
		And t3.ntid = t4.ntid
		And t4.itmsid = t5.itmsid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.nt_num = @Nt_Num
		And t3.nt_dt = @Nt_Dt
		And t5.rt = @Rt

		Update TBL_GSTR1_CDNR_NT
		SET TBL_GSTR1_CDNR_NT.val = @Nval
		FROM TBL_GSTR1 t1,
			 TBL_GSTR1_CDNR t2,
			 TBL_GSTR1_CDNR_NT  t3 
		Where t1.gstr1id = t2.gstr1id
		And t2.cdnrid = t3.cdnrid
		And t1.gstin = @Gstin
		And t1.fp = @Fp
		And t3.nt_num = @Nt_Num
		And t3.nt_dt = @Nt_Dt

		Select @Retval = 1 -- Item details updated and also the Invoice Value updated for the entire Invoice
			
	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End

End