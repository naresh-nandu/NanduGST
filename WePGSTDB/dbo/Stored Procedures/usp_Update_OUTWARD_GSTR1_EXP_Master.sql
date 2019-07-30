
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update EXP Invoice Master Details
				
Written by  : Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
12/05/2017	Karthik		Initial Version

*/

/* Sample Procedure Call

exec usp_Update_OUTWARD_GSTR1_EXP_Master 
 */
 
CREATE PROCEDURE [usp_Update_OUTWARD_GSTR1_EXP_Master] 
		@Gstin    varchar(15),
		@Fp    varchar(10),
		@Inum	nvarchar(50),
		@Idt	varchar(50),
		@Val	decimal(18,2),
		@sbnum	varchar(50),
		@sbdt   datetime,
		@sbpcode  varchar(6),
		@ShippingAddress varchar(max)=NULL,
		@Addinfo varchar(MAX)=NULL,
		@ReceiverName nvarchar(250)=NULL,
		@Retval int = null Out

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on


	Update TBL_EXT_GSTR1_EXP_INV 
	Set gstin=@Gstin,
		fp=@Fp,
		inum = @Inum,
		idt = @Idt,
		val = @Val,
		sbnum = @Sbnum,
		sbdt = @Sbdt,
		sbpcode = @sbpcode,
		ShippingAddress = @ShippingAddress,
		Addinfo=@Addinfo,
		ReceiverName = @ReceiverName
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