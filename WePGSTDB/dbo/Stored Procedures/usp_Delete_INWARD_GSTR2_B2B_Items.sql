
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Delete data from table TBL_EXT_INWARD_GSTR2_B2B_INV Items
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Delete_OUTWARD_GSTR1_B2B_Items

 */
 
Create PROCEDURE [usp_Delete_INWARD_GSTR2_B2B_Items]  
	@invitemId int, 
	@RetVal int = NULL Out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on
	Declare @InvoiceAmount decimal(18,2)
	Declare @Inum varchar(50), @idt varchar(15),@Val decimal(18,2)

	if exists(select 1 from TBL_EXT_GSTR2_B2B_INV where b2bid = @invitemid and rowstatus =1)
	Begin
	select @Inum=inum,@idt=idt,@Val=val from TBL_EXT_GSTR2_B2B_INV where b2bid = @invitemid and rowstatus =1
		Delete from TBL_EXT_GSTR2_B2B_INV  where b2bid = @invitemid and rowstatus =1
		
		select @InvoiceAmount = (sum(txval)+sum(iamt)+sum(camt)+sum(samt)+sum(csamt)) from TBL_EXT_GSTR2_B2B_INV where inum= @Inum and idt=@idt and val=@Val
		update TBL_EXT_GSTR2_B2B_INV set val= @InvoiceAmount where inum= @Inum and idt=@idt and val=@Val

		select @Retval = 1 -- Item Deleted values updated in Invoice Value.
	End
	Else
	Begin
		select @Retval = -1 -- Item Not exist for Given Invoice.
	End

  End