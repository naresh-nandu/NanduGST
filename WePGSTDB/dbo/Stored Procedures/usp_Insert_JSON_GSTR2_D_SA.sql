
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
09/01/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2_D_SA  
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2_D_SA]
	@Gstin varchar(15),
	@Fp varchar(10), 
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare	@B2b nvarchar(max),
			@B2bur nvarchar(max),
			@Cdnr nvarchar(max),
			@Cdnur nvarchar(max),
			@Hsnsum nvarchar(max),
			@Impg nvarchar(max),
			@Imps nvarchar(max),
			@Nil nvarchar(max),
			@Txli nvarchar(max),
			@Txpd nvarchar(max),
			@Itcrvsl nvarchar(max)

	Select	@B2b = b2b,
			@B2bur = b2bur,
			@Cdnr = cdn,
			@Cdnur = cdnur,
			@Hsnsum = hsnsum,
			@Impg = Imp_g,
			@Imps = Imp_s,
			@Nil = nil_supplies,
			@Txli = txli,
			@Txpd = txpd,
			@Itcrvsl = itc_rvsl
			
	From OPENJSON(@RecordContents) 
	WITH
	(	
		b2b nvarchar(max) as JSON,
		b2bur nvarchar(max) as JSON,
		cdn nvarchar(max) as JSON,
		cdnur nvarchar(max) as JSON,
		hsnsum nvarchar(max) as JSON,
		imp_g nvarchar(max) as JSON,
		imp_s nvarchar(max) as JSON,
		nil_supplies nvarchar(max) as JSON,
		txli nvarchar(max) as JSON,
		txpd nvarchar(max) as JSON,
		itc_rvsl nvarchar(max) as JSON
	)

	If Ltrim(Rtrim(IsNull(@B2b,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_B2B_SA  @Gstin, @Fp, @B2b
	End

	If Ltrim(Rtrim(IsNull(@B2bur,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_B2BUR_SA @Gstin, @Fp, @B2bur
	End

	If Ltrim(Rtrim(IsNull(@Cdnr,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_CDNR_SA @Gstin, @Fp, @Cdnr
	End
	
	If Ltrim(Rtrim(IsNull(@Cdnur,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_CDNUR_SA @Gstin, @Fp, @Cdnur
	End
		
	If Ltrim(Rtrim(IsNull(@Hsnsum,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_HSNSUM_SA @Gstin, @Fp,@Hsnsum
	End
	
	If Ltrim(Rtrim(IsNull(@Impg,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_IMPG_SA @Gstin, @Fp,@Impg
	End
	
	If Ltrim(Rtrim(IsNull(@Imps,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_IMPS_SA @Gstin, @Fp,@Imps
	End

	If Ltrim(Rtrim(IsNull(@Nil,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_NIL_SA  @Gstin, @Fp,@Nil
	End

	If Ltrim(Rtrim(IsNull(@Txli,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_TXLI_SA @Gstin, @Fp, @Txli
	End
	
	If Ltrim(Rtrim(IsNull(@Txpd,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR2_D_TXPD_SA @Gstin, @Fp, @Txpd
	End
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End