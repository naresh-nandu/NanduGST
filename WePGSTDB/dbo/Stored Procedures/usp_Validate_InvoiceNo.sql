
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Validate Invoice Number of Inward Register
				
Written by  : nareshn@wepindia.com 

Date		Who			 Decription 
12/02/2018	Naresh N	 Initial Version
15/02/2018  Naresh N     Note Number validation for CDNR & B2CL Changes


*/

/* Sample Procedure Call

declare @RetValue int
exec usp_Validate_InvoiceNo '27AWJPN5651K1Z6','12-02-2018','inv62','I',@retvalue out
select @retvalue

 */
 
CREATE PROCEDURE [usp_Validate_InvoiceNo]
	@Gstin varchar(15),
	@Idt varchar(10),  --In Case of CDNR Idt means Note Date
	@Inum varchar(16), --In Case of CDNR Inum means Note Number
	@Mode varchar(3), -- 'I' : Inward ; 'O' : Outward
	@RetValue int = Null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @InvDate Date,
			@InvMonth int,
			@InvYear int,
			@InvFinYear varchar(10),

			@PrevIdt varchar(10),
			@PrevInvDate Date,
			@PrevInvMonth int,
			@PrevInvYear int,
			@PrevInvFinYear varchar(10)

	Set @RetValue=-1 -- Invoice No doesn't exist (Default Value)
		

	if @Mode = 'I'
	Begin
		if Exists( Select 1 From TBL_EXT_GSTR2_B2B_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR2_B2B_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else if Exists( Select 1 From TBL_EXT_GSTR2_B2BUR_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR2_B2BUR_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else
		Begin
			return @RetValue
		End
	End
	else if @Mode = 'O'
	Begin
		if Exists( Select 1 From TBL_EXT_GSTR1_B2B_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR1_B2B_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else if Exists( Select 1 From TBL_EXT_GSTR1_B2CL_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR1_B2CL_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else if Exists( Select 1 From TBL_EXT_GSTR1_B2CS_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR1_B2CS_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else if Exists( Select 1 From TBL_EXT_GSTR1_EXP_INV 
					where gstin = @Gstin
					and inum = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR1_EXP_INV 
			Where gstin=@Gstin
			And inum=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else if Exists( Select 1 From TBL_EXT_GSTR1_CDNR 
					where gstin = @Gstin
					and nt_num = @Inum 
					and RowStatus <> 2 )
		Begin
			Select Top(1) @PrevIdt=idt 
			From TBL_EXT_GSTR1_CDNR 
			Where gstin=@Gstin
			And nt_num=@Inum 
			And RowStatus <> 2 
			Order by idt desc
		End
		else
		Begin
			return @RetValue
		End

	End

	Select @InvDate = Try_Convert(date,@Idt,103)
	Select @InvMonth =  Month(@InvDate)
	Select @InvYear =  Year(@InvDate)

	If @InvMonth < 4
	Begin
		Set @InvFinYear=Convert(varchar,@InvYear - 1) + '-' + Convert(varchar,@InvYear)
	End
	Else
	Begin
		Set @InvFinYear=Convert(varchar,@InvYear) + '-' + Convert(varchar,@InvYear + 1)
	End

	Select @PrevInvDate = Try_Convert(date,@PrevIdt,103)
	Select @PrevInvMonth =  Month(@PrevInvDate)
	Select @PrevInvYear =  Year(@PrevInvDate)

	If @PrevInvMonth  < 4
	Begin
		Set @PrevInvFinYear=Convert(varchar,@PrevInvYear - 1) + '-' + Convert(varchar,@PrevInvYear)
	End
	Else
	Begin
		Set @PrevInvFinYear=Convert(varchar,@PrevInvYear) + '-' + Convert(varchar,@PrevInvYear + 1)
	End

	Select @InvDate
	Select @InvFinYear
	Select @PrevInvDate
	Select @PrevInvFinYear


	if @InvFinYear = @PrevInvFinYear
	Begin
		Set @RetValue = 1 -- Invoice Number already exists
	End
	else
	Begin
		Set @RetValue = -1 -- Invoice Number do not exist
	End

	return @RetValue

End