
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_OUTWARD_GSTR1_B2CL_EXT 
 */
 
CREATE PROCEDURE [usp_Insert_OUTWARD_GSTR1_B2CL_EXT_N]
		@gstin    varchar(15),
		@fp    varchar(10),
		@inum	nvarchar(50),
		@idt	varchar(50),
		@val	decimal(18,2),
		@POS	varchar(2),
		@etin	varchar(15),
		@rt		decimal(18,2),
		@txval	decimal(18,2),
		@iamt	decimal(18,2),
		@camt	decimal(18,2),
		@samt	decimal(18,2),
		@csamt	 decimal(18,2),
		@ReferenceNo	varchar(50),
		@hsncode	varchar(50),
		@hsndesc	varchar(250),
		@qty	decimal(18,2),
		@unitprice decimal(18,2),
		@discount decimal(18,2),
		@uqc varchar(50),
		@buyerid int,
		@Createdby int,
		@Addinfo varchar(MAX),
		@Retinum varchar(50)= null Out,
		@invid int = null

		

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @InvoiceNo varchar(50)
	Declare @gstinstr varchar(10),@fyyear varchar(10),@invCount varchar(10)
	Declare @InvoiceAmount decimal(18,2)
	if @invid <> NULL
		Begin

			if @inum = 'NA'
			Begin
				set @gstinstr = SUBSTRING(@gstin, 13, 15)
				--set @fyyear = convert(varchar(10),FORMAT(getdate(), 'MMyyyy'))
				select @invCount= convert(varchar(10),count(*)+1) from TBL_EXT_GSTR1_B2CL_INV where gstin = @gstin and sourcetype ='Manual'
				set @invCount = convert(varchar(10),FORMAT(convert(int,@invCount),'000000'))
				Set @InvoiceNo = 'INV' + @gstinstr +  @invCount
			End
			Else
			Begin
				Set @InvoiceNo = @inum
			End

				Insert into TBL_EXT_GSTR1_B2CL_INV
							(gstin, fp, gt, cur_gt, pos,inum, idt, val, etin,  
							rt, txval, iamt, csamt,
							rowstatus, sourcetype, referenceno,createddate,
							hsncode,hsndesc,qty,unitprice,discount,buyerid,uqc,createdby,camt,samt,Addinfo)
						values
							(UPPER(@gstin), @fp, null, null,@pos,@InvoiceNo, @idt, @val,UPPER(@etin), 
							@rt, @txval, @iamt,@csamt,
							1 ,'Manual' ,@ReferenceNo,GetDate(),
							@hsncode,@hsndesc,@qty,@unitprice,@discount,@buyerid,@uqc,@Createdby,@camt,@samt,@Addinfo)
			select @Retinum = @InvoiceNo
		End
	Else
		Begin

			Update TBL_EXT_GSTR1_B2CL_INV 
					Set pos=@pos,etin=@etin,hsncode =@hsncode,hsndesc=@hsndesc,qty=@qty,unitprice=@unitprice,discount=@discount,uqc=@uqc,
						rt=@rt,txval=@txval,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt,Addinfo=@Addinfo where invid = @invid and rowstatus =1
		
			select @InvoiceAmount = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) from TBL_EXT_GSTR1_B2CL_INV where inum= @inum and idt=@idt and val=@val
			update TBL_EXT_GSTR1_B2CL_INV set val= @InvoiceAmount where inum= @inum and idt=@idt and val=@val

				select @Retinum = '-1'
		End


End