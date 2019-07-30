
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_OUTWARD_GSTR1_CDNR_EXT 
 */
 
CREATE PROCEDURE [usp_Insert_OUTWARD_GSTR1_CDNR_EXT_N]
		
	@gstin varchar(15),
	@fp varchar(10),
	@ctin varchar(15),
	@cfs varchar(1),
	@ntty varchar(1),
	@nt_num varchar(50),
	@nt_dt varchar(50),
	@inum varchar(50),
	@idt varchar(50),
	@val decimal(18, 2),
	@pos varchar(2),
	@rt decimal(18, 2),
	@txval decimal(18, 2),
	@iamt decimal(18, 2),
	@camt decimal(18, 2),
	@samt decimal(18, 2),
	@csamt decimal(18, 2),
	@referenceno varchar(50),
	@hsncode varchar(50),
	@hsndesc varchar(255),
	@qty decimal(18, 2),
	@unitprice decimal(18, 2),
	@discount decimal(18, 2),
	@uqc varchar(50),
	@buyerid int,
	@Createdby int,
	@Addinfo varchar(MAX),
	@Retinum varchar(50) = null out,
	@Cdnrid	int = NULL

	

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
			
	Declare @InvoiceNo varchar(50)
	Declare @gstinstr varchar(10),@fyyear varchar(10),@invCount varchar(10)
	if @Cdnrid <> NULL
		Begin
				if @nt_num = 'NA'
				Begin
					set @gstinstr = SUBSTRING(@gstin, 13, 15)
					--set @fyyear = convert(varchar(10),FORMAT(getdate(), 'MMyyyy'))
					select @invCount= convert(varchar(10),count(*)+1) from TBL_EXT_GSTR1_CDNR where gstin = @gstin and sourcetype ='Manual'
					set @invCount = convert(varchar(10),FORMAT(convert(int,@invCount),'000000'))
					Set @InvoiceNo = 'INV' + @gstinstr +  @invCount
				End
				Else
				Begin
					Set @InvoiceNo = @nt_num
				End

						Insert into TBL_EXT_GSTR1_CDNR
							(	gstin, fp, gt, cur_gt, ctin,cfs,ntty,nt_num,nt_dt,inum, idt, val,pos, 
								rt, txval, iamt, camt, samt, csamt,
								rowstatus, sourcetype, referenceno, createddate,
								hsncode,hsndesc,qty,unitprice,discount,buyerid,createdby,uqc,Addinfo)
							values
								(UPPER(@gstin), @fp, null, null,UPPER(@ctin),@cfs,@ntty,@InvoiceNo,@nt_dt,@inum, @idt,@val,@pos,
								@rt,@txval,@iamt,@camt,@samt,@csamt,
								1 ,'Manual' ,@ReferenceNo,GetDate(),
								@hsncode,@hsndesc,@qty,@unitprice,@discount,@buyerid,@Createdby,@uqc,@Addinfo)

					select @Retinum = @InvoiceNo
		End
	Else
		Begin
			Update TBL_EXT_GSTR1_CDNR set gstin=@gstin, fp=@fp, ctin=@ctin,cfs=@cfs,val=@val,pos=@pos, 
								rt=@rt, txval=@txval, iamt=@iamt, camt=@camt, samt=@samt, csamt=@csamt,hsncode=@hsncode,hsndesc=@hsndesc,
								qty=@qty,unitprice=@unitprice,discount=@discount,buyerid=@buyerid,uqc=@uqc,Addinfo=@Addinfo where cdnrid= @Cdnrid and rowstatus =1
						select @Retinum = '-1'
		End


End