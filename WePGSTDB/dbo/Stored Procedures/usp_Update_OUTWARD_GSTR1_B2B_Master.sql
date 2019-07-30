
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update B2B Invoice Master Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_B2B_Master 
 */
 
Create PROCEDURE [usp_Update_OUTWARD_GSTR1_B2B_Master] 
		@Gstin    varchar(15),
		@Fp    varchar(10),
		@Ctin		varchar(15),
		@Inum	nvarchar(50),
		@Idt	varchar(50),
		@Val	decimal(18,2),
		@Pos	varchar(2),
		@Rchrg	varchar(1),
		@Etin	varchar(15),
		@Inv_Typ varchar(15),		
		@Buyerid int,
		@Addinfo varchar(MAX),
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @TP_Pos varchar(2) -- Tax Payer POS
	Select @TP_Pos= convert(varchar(2),substring(@Gstin,1,2))

	if @TP_Pos <> @Pos
	Begin

		Update TBL_EXT_GSTR1_B2B_INV 
		Set iamt = (camt+samt),
			camt=0,
			samt=0 
		Where inum=@Inum 
		And idt=@Idt 
		And val=@Val
		And (rowstatus= 0 or rowstatus= 1) 
		And SourceType='Manual'
	
	End
	Else
	Begin
		
		Update TBL_EXT_GSTR1_B2B_INV 
		Set camt = (iamt/2), 
			samt = (iamt/2),
			iamt=0  
		Where inum=@Inum 
		And idt=@Idt 
		And val=@Val 
		And (rowstatus= 0 or rowstatus= 1)
		And SourceType='Manual'
		
	End

	Update TBL_EXT_GSTR1_B2B_INV 
	Set gstin=@Gstin,
		fp=@Fp,
		ctin=@Ctin,
		pos=@Pos,
		rchrg=@Rchrg,
		etin=@Etin,
		inv_typ=@Inv_Typ,
		buyerid=@Buyerid,
		Addinfo=@Addinfo
	Where inum=@Inum 
	And idt=@Idt 
	And val=@Val 
	And (rowstatus= 0 or  rowstatus= 1)
	And SourceType='Manual'
	
	if(@@rowcount > 0 )
	Begin
		Select @Retval = 1
	End							
	Else
	Begin
		Select @Retval = -1
	End
		

End