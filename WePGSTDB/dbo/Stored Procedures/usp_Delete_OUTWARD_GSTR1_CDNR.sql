
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete CDNR Invoice Line Item Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Delete_OUTWARD_GSTR1_CDNR 
 */

 
Create PROCEDURE [usp_Delete_OUTWARD_GSTR1_CDNR]  
	@Cdnrid int, 
	@RetVal int = NULL Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Gstin varchar(15),@Fp varchar(10)
	Declare @Inum varchar(50),@Idt varchar(15),@Val decimal(18,2),@Rt decimal(18,2) 
	Declare @Nt_Num varchar(50),@Nt_Dt varchar(15)
	Declare @Flag varchar(1)
	Declare @Nval decimal(18,2),@Ntxval decimal(18,2)
	Declare @Niamt decimal(18,2),@Ncamt decimal(18,2)
	Declare @Nsamt decimal(18,2),@Ncsamt decimal(18,2)


	if Exists (	Select 1 from TBL_EXT_GSTR1_CDNR
				Where cdnrid = @cdnrid
				And rowstatus in (0,1)
			  )
	Begin

		Select  @Gstin = gstin,
				@Fp = fp,
				@Inum=inum,
				@Idt=idt,
				@Val=val,
				@Rt=rt,
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
			Select @Retval = -2 -- Item details cannot be deleted.
			Return
		End

		Delete From TBL_EXT_GSTR1_CDNR 
		Where  cdnrid = @cdnrid 

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

		if Exists(Select 1 From TBL_EXT_GSTR1_CDNR   
					Where nt_num= @Nt_num 
					And nt_dt=@Nt_dt
					And rt = @Rt)
		Begin
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
		End
		else
		Begin
			Delete TBL_GSTR1_CDNR_NT_ITMS_DET
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
		End

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

		Select @Retval = 1 -- Item details deleted and also the Invoice Value updated for the entire Invoice

	End
	Else
	Begin
		Select @Retval = -1 -- Item does not exist for the given Invoice.
	End


End