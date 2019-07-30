
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2A JSON Records to the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/28/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2A_SA  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2A_SA]
	@Gstin varchar(15),
	@Fp varchar(10), 
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @B2b nvarchar(max),
			@Cdnr nvarchar(max)
	
	Select	@B2b = b2b,
			@Cdnr = cdn
	From OPENJSON(@RecordContents) 
	WITH
	(	b2b nvarchar(max) as JSON,
		cdn nvarchar(max) as JSON
	)

	If Ltrim(Rtrim(IsNull(@B2b,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2AB2B_SA @Gstin, @Fp, @B2b
	End

	If Ltrim(Rtrim(IsNull(@Cdnr,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2ACDNR_SA @Gstin, @Fp, @Cdnr
	End


	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End