
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Compute Checksum for GSTR1 Invoices
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/30/2017	Seshadri	Initial Version
10/03/2017	Seshadri	Modified the code to invoke the procedure for computing chksum 
						related to B2CS,EXP,CDNR Records

*/

/* Sample Procedure Call

exec usp_Compute_Chksum_GSTR1_Invoices '10BNAPG9890G5ZK','062017'
 */

CREATE PROCEDURE [usp_Compute_Chksum_GSTR1_Invoices] 
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RefIds nvarchar(MAX) = NULL,
	@Flag varchar(1) = 'U'

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		exec usp_Compute_Chksum_GSTR1_B2B @Gstin,@Fp,@RefIds,@Flag
	End
	else if @ActionType = 'B2CL'
	Begin
		exec usp_Compute_Chksum_GSTR1_B2CL @Gstin,@Fp,@RefIds,@Flag
	End
	else if @ActionType = 'B2CS'
	Begin
		exec usp_Compute_Chksum_GSTR1_B2CS @Gstin,@Fp,@RefIds,@Flag
	End
	else if @ActionType = 'EXP'
	Begin
		exec usp_Compute_Chksum_GSTR1_EXP @Gstin,@Fp,@RefIds,@Flag
	End
	else if @ActionType = 'CDNR'
	Begin
		exec usp_Compute_Chksum_GSTR1_CDNR @Gstin,@Fp,@RefIds,@Flag
	End
	/*
	else if @ActionType = 'CDNUR'
	Begin
	End
	else if @ActionType = 'HSNSUM'
	Begin
	End
	else if @ActionType = 'HSN'
	Begin
	End
	else if @ActionType = 'NIL'
	Begin
	End
	else if @ActionType = 'TXP'
	Begin
	End
	else if @ActionType = 'AT'
	Begin
	End
	else if @ActionType = 'DOCISSUE'
	Begin
	End
	*/

	Return 0

End