
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 2 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_INWARD_GSTR2_B2B_EXT 
 */
 
CREATE PROCEDURE [usp_Insert_INWARD_GSTR2_B2B_EXT]
		@gstin    varchar(15),
		@fp    varchar(10),
		@ctin		varchar(15),
		@inum	nvarchar(50),
		@idt	varchar(50),
		@val	decimal(18,2),
		@POS	varchar(2),
		@rchrg	varchar(1),
		@inv_typ varchar(15),
		@rt		decimal(18,2),
		@txval	decimal(18,2),
		@iamt	decimal(18,2),
		@camt	decimal(18,2),
		@samt	decimal(18,2),
		@csamt	 decimal(18,2),
		@ReferenceNo	varchar(50),
		@hsncode	varchar(50),
		@hsndesc	varchar(50),
		@qty	decimal(18,2),
		@unitprice decimal(18,2),
		@discount decimal(18,2),
		@uqc varchar(50),
		@supplierid int,
		@Createdby int,
		@RetInum varchar(50)= NULL Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	
	--print @InvoiceNo

	Insert TBL_EXT_GSTR2_B2B_INV
	(	gstin, fp, ctin,inum, idt, val, pos, rchrg, inv_typ, 
		rt, txval, iamt, camt, samt, csamt,
		rowstatus, sourcetype, referenceno, createddate,
		hsncode,hsndesc,qty,unitprice,discount,supplierid,uqc,createdby)
	values(UPPER(@gstin), @fp,UPPER(@ctin),@inum, @idt, @val, @pos, @rchrg,@inv_typ, 
		@rt, @txval, @iamt, @camt, @samt, @csamt,
		1 ,'Manual' ,@ReferenceNo,GetDate(),
		@hsncode,@hsndesc,@qty,@unitprice,@discount,@supplierid,@uqc,@Createdby)
		
	select @RetInum = @inum

End