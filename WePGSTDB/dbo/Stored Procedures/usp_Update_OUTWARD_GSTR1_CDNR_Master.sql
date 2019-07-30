
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update CDNR Invoice Master Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_CDNR_Master 
 */
 
 
Create PROCEDURE [usp_Update_OUTWARD_GSTR1_CDNR_Master]
			@Gstin varchar(15),
			@Fp varchar(10),
			@Ctin varchar(15),
			@Ntty varchar(1),
			@Nt_Num varchar(50),
			@Nt_Dt varchar(50),
			@Inum varchar(50),
			@Idt varchar(50),
			@Referenceno varchar(50),
			@ModifiedBy int,
			@Addinfo varchar(MAX),
			@Retval int = null out
-- /*mssx*/ With Encryption 
as 
Begin

	
	Set Nocount on
	 
	if Exists(Select 1 from TBL_EXT_GSTR1_CDNR 
				Where inum= @Inum 
				And idt=@Idt 
				And nt_num= @Nt_Num 
				And nt_dt = @Nt_Dt 
				And gstin= @Gstin 
				And referenceno = @Referenceno)
	Begin
			
		Update TBL_EXT_GSTR1_CDNR 
		Set Addinfo = @Addinfo 
		Where inum= @Inum 
		And idt = @Idt
		And nt_num= @Nt_Num 
		And nt_dt = @Nt_Dt 
		And gstin = @Gstin
		And referenceno = @Referenceno
			
		Select @Retval = 1
	End
	Else
	Begin
		Select @Retval = -1
	End


End