
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve  GSTR1_D Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri	Initial Version
08/18/2017	Seshadri	Modified to include HSNSUM Action Type
08/23/2017	Seshadri	Modified to include CDNUR and DOCISSUE Action Types	

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_D_SA
 */

CREATE PROCEDURE [usp_Retrieve_GSTR1_D_SA]
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10)
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		exec usp_Retrieve_GSTR1_D_B2B_SA @Gstin, @Fp
	End
	else if @ActionType = 'B2CL'
	Begin
		exec usp_Retrieve_GSTR1_D_B2CL_SA @Gstin, @Fp
	End
	else if @ActionType = 'B2CS'
	Begin
		exec usp_Retrieve_GSTR1_D_B2CS_SA @Gstin, @Fp
	End
	else if @ActionType = 'EXP'
	Begin
		exec usp_Retrieve_GSTR1_D_EXP_SA @Gstin, @Fp
	End
	else if @ActionType = 'CDNR'
	Begin
		exec usp_Retrieve_GSTR1_D_CDNR_SA @Gstin, @Fp
	End
	else if @ActionType = 'CDNUR'
	Begin
		exec usp_Retrieve_GSTR1_D_CDNUR_SA @Gstin, @Fp
	End
	else if @ActionType = 'HSNSUM'
	Begin
		exec usp_Retrieve_GSTR1_D_HSN_SA @Gstin, @Fp
	End
	else if @ActionType = 'HSN'
	Begin
		exec usp_Retrieve_GSTR1_D_HSN_SA @Gstin, @Fp
	End
	else if @ActionType = 'NIL'
	Begin
		exec usp_Retrieve_GSTR1_D_NIL_SA @Gstin, @Fp
	End
	else if @ActionType = 'TXP'
	Begin
		exec usp_Retrieve_GSTR1_D_TXP_SA @Gstin, @Fp
	End
	else if @ActionType = 'AT'
	Begin
		exec usp_Retrieve_GSTR1_D_AT_SA @Gstin, @Fp
	End
	else if @ActionType = 'DOCISSUE'
	Begin
		exec usp_Retrieve_GSTR1_D_DOCISSUE_SA @Gstin, @Fp
	End
	else
		Select ''

	Return 0
End