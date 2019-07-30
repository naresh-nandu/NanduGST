
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
*/

/* Sample Procedure Call

exec usp_Update_INWARD_GSTR2_B2B_Master
 */
 
CREATE PROCEDURE [usp_Update_INWARD_GSTR2_B2B_Master]
		@gstin    varchar(15),
		@fp    varchar(10),
		@ctin		varchar(15),
		@inum	nvarchar(50),
		@idt	varchar(50),
		@val	decimal(18,2),
		@POS	varchar(2),
		@rchrg	varchar(1),
		@inv_typ varchar(15),		
		@supplierid int,
		@Retval int = null Out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @TPpos varchar(2)
	select @TPpos= convert(varchar(2),substring(@gstin,1,2))

	if @TPpos <> @POS
	Begin
	
		update TBL_EXT_GSTR2_B2B_INV set iamt = (camt+samt),CAMT=0,SAMT=0 where inum=@inum and idt=@idt and val=@val and rowstatus=1 and SourceType='Manual'
	End
	Else
	Begin
		update TBL_EXT_GSTR2_B2B_INV set camt = (iamt/2), samt = (iamt/2),iamt=0  where inum=@inum and idt=@idt and val=@val and rowstatus=1 and SourceType='Manual'
	End

	Update TBL_EXT_GSTR2_B2B_INV set gstin =@gstin,fp=@fp, ctin=@ctin,pos=@pos, rchrg=@rchrg,inv_typ=@inv_typ,supplierid=@supplierid
	where inum=@inum and idt=@idt  and rowstatus=1 and SourceType='Manual' 
	if(@@rowcount >0)
	Begin
		select @Retval = 1
	End							
	Else
	Begin
		select @Retval = -1
	End


End