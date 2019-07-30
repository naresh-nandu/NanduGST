
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update EXT GSTR2 B2B Invoice Items
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Update_INWARD_GSTR2_B2B_Items 
 */
 
Create PROCEDURE [usp_Update_INWARD_GSTR2_B2B_Items]
		@invitemid    int,
		@hsncode	varchar(50),
		@hsndesc	varchar(50),
		@qty	decimal(18,2),
		@unitprice decimal(18,2),
		@discount decimal(18,2),
		@uqc varchar(50),
		@rt		decimal(18,2),
		@txval	decimal(18,2),
		@iamt	decimal(18,2),
		@camt	decimal(18,2),
		@samt	decimal(18,2),
		@csamt	 decimal(18,2),
		@Createdby int,
		@Retval int = null Out
-- /*mssx*/ With Encryption 
as 
Begin

	
	Set Nocount on
	 
	Declare @InvoiceAmount decimal(18,2)
	Declare @Inum varchar(50), @idt varchar(15),@Val decimal(18,2)
	


	if exists(select 1 from TBL_EXT_GSTR2_B2B_INV where b2bid = @invitemid and rowstatus =1)
	Begin
	select @Inum=inum,@idt=idt,@Val=val from TBL_EXT_GSTR2_B2B_INV where b2bid = @invitemid and rowstatus =1
		Update TBL_EXT_GSTR2_B2B_INV 
				Set hsncode =@hsncode,hsndesc=@hsndesc,qty=@qty,unitprice=@unitprice,discount=@discount,uqc=@uqc,
					rt=@rt,txval=@txval,iamt=@iamt,camt=@camt,samt=@samt,csamt=@csamt where b2bid = @invitemid and rowstatus =1
		
		select @InvoiceAmount = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) from TBL_EXT_GSTR2_B2B_INV where inum= @Inum and idt=@idt and val=@Val                      
		update TBL_EXT_GSTR2_B2B_INV set val= @InvoiceAmount where inum= @Inum and idt=@idt and val=@Val

		select @Retval = 1 -- Item details updated and Sum of all Items values updated in Invoice Value.
	End
	Else
	Begin
		select @Retval = -1 -- Item Not exist for Given Invoice.
	End


End