
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update B2CL Invoice Master Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Karthik		Initial Version
10/6/2017	Seshadri	Fine tuned the code 

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_B2CL_Master 
 */
 
 
Create PROCEDURE [usp_Update_OUTWARD_GSTR1_B2CL_Master]
	@Gstin varchar(15),
	@Fp varchar(10),
	@Inum varchar(50),
	@Idt varchar(50),
	@Val decimal(18,2),
	@Pos varchar(2),
	@Etin varchar(15),
	@ReferenceNo varchar(50),	
	@BuyerId int,
	@CreatedBy int,
	@Addinfo varchar(MAX),
	@Retval int= null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @TP_Pos varchar(2) -- Tax Payer POS
	Declare @Nval decimal(18,2)

	Select @TP_Pos= convert(varchar(2),substring(@Gstin,1,2))

	if @TP_Pos <> @Pos and @Val >= 250000.00
	Begin
	
		Update TBL_EXT_GSTR1_B2CL_INV 
		Set gstin=@Gstin,
			fp=@Fp,
			pos =@Pos,
			etin=@Etin,
			Addinfo=@AddInfo 
		Where inum=@Inum 
		And idt=@Idt 
		And val=@Val
		And SourceType='Manual' 
		And ReferenceNo=@ReferenceNo 
		And Buyerid=@BuyerId
		
		Select @Nval = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) 
		From TBL_EXT_GSTR1_B2CL_INV 
		Where inum=@Inum 
		And idt=@Idt 
		And val=@Val
		And SourceType='Manual'
		And ReferenceNo=@ReferenceNo 
		And Buyerid=@BuyerId
		
		Update TBL_EXT_GSTR1_B2CL_INV 
		Set val= @Nval
		Where inum=@Inum
		And idt=@Idt 
		And val=@Val
		And SourceType='Manual'
		And ReferenceNo=@ReferenceNo 
		And Buyerid=@BuyerId
		
		if(@@rowcount >0)
		Begin
			Select @Retval = 1
		End							
		Else
		Begin
			Select @Retval = -1
		End

	End
	Else
	Begin
		select @Retval = -2
	End

End