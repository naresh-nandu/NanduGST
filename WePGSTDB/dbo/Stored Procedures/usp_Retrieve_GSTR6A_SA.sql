

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve  GSTR6A Records from the corresponding staging area tables
				
Written by  : muskan.garg@wepdigital.com

Date		Who			    Decription 
05/15/2018	Muskan			Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR6A_SA 'B2B','33GSPTN0802G1ZL','072017'
 */

CREATE PROCEDURE [dbo].[usp_Retrieve_GSTR6A_SA]
	@ActionType varchar(15),
	@Gstin varchar(15),
	@Fp varchar(10)
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @ActionType = 'B2B'
	Begin
		exec usp_Retrieve_GSTR6AB2B_SA @Gstin, @Fp
	End
	else if @ActionType = 'CDN'
	Begin
		exec usp_Retrieve_GSTR6ACDNR_SA @Gstin, @Fp
	End

	Return 0
End