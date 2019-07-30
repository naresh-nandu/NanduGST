
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to process GSTR2 RETSTATUS
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
09/03/2017	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Process_GSTR2_RETSTATUS 

 */
 
CREATE PROCEDURE [usp_Process_GSTR2_RETSTATUS]
	@Gstin varchar(15),
	@Fp varchar(10)  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Return 0


End