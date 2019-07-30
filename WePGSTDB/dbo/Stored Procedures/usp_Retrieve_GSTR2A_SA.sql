
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve  GSTR2A Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2A_SA 'B2B','33GSPTN0802G1ZL','072017'
 */

CREATE PROCEDURE [usp_Retrieve_GSTR2A_SA]
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10)
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		exec usp_Retrieve_GSTR2AB2B_SA @Gstin, @Fp
	End
	else if @ActionType = 'CDN'
	Begin
		exec usp_Retrieve_GSTR2ACDNR_SA @Gstin, @Fp
	End

	Return 0
End