
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Update the CTIN Validation Status
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
11/17/2017	Seshadri 	Initial Version

*/

/* Sample Procedure Call

usp_Update_Ctin_Validation_Status


 */
 
CREATE PROCEDURE [usp_Update_Ctin_Validation_Status]  
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@GstrType varchar(10), -- GSTR1 , GSTR2
	@Mode tinyint ,
	@Ctin varchar(15),
	@ErrorCode smallint ,
	@ErrorMessage varchar(255)
  

-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	if @Mode = 1 
	Begin
		if @GstrType = 'GSTR1' And @ErrorCode = 0
		Begin
			Update TBL_EXT_GSTR1_B2B_INV
			Set errorMessage = @ErrorMessage
			Where sourcetype = @SourceType
			And referenceno = @ReferenceNo
			And ctin = @Ctin
			And rowstatus = 1
		End
	End
	else if @Mode = 2
	Begin
		if @GstrType = 'GSTR1'
		Begin
			Select * From TBL_EXT_GSTR1_B2B_INV
			Where sourcetype = @SourceType
			And referenceno = @ReferenceNo
			And Ltrim(Rtrim(IsNull(errorMessage,''))) <> ''
			And rowstatus = 1
		End
	End

	Return 0
End