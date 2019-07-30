

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
07/31/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_SA  
exec usp_Insert_JSON_GSTR1_D_SA  '29AAACB6820C1Z1','012018','{"cdnur": [
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 1495186
          }
        }
      ],
      "val": 1495186,
      "flag": "N",
      "idt": "26-12-2017",
      "ntty": "C",
      "nt_num": "C1Z11800018",
      "typ": "EXPWOP",
      "inum": "1Z11801091",
      "nt_dt": "04-01-2018",
      "chksum": "c0f070c38831022cc66fd3c76d00af78aa9795fbaa45a3a5b25ff938d96c46d7"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 739395.66
          }
        }
      ],
      "val": 739395.66,
      "flag": "N",
      "idt": "03-01-2018",
      "ntty": "C",
      "nt_num": "C1Z11800020",
      "typ": "EXPWOP",
      "inum": "1Z11801140",
      "nt_dt": "05-01-2018",
      "chksum": "640de7f32bc86a95573c20286d8f2c73f72a543a7555f2843ebcdfd1b42a1d57"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 1450493.1
          }
        }
      ],
      "val": 1450493.1,
      "flag": "N",
      "idt": "12-01-2018",
      "ntty": "C",
      "nt_num": "C1Z11800022",
      "typ": "EXPWOP",
      "inum": "1Z11801298",
      "nt_dt": "15-01-2018",
      "chksum": "892d8a3c421228f73b5280956c0741ffcae95001bbe51e82e5038575aafa6398"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 163804.2
          }
        }
      ],
      "val": 163804.2,
      "flag": "N",
      "idt": "07-08-2017",
      "ntty": "C",
      "nt_num": "C1Z11800023",
      "typ": "EXPWOP",
      "inum": "E1001800961",
      "nt_dt": "16-01-2018",
      "chksum": "d6fe5391cea322fa6d7099eb038ac1999f3d20b7840362bfb4a65158cde633f4"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 26500.35
          }
        }
      ],
      "val": 26500.35,
      "flag": "N",
      "idt": "31-12-2017",
      "ntty": "C",
      "nt_num": "C1Z1IC18-016",
      "typ": "EXPWOP",
      "inum": "1Z1IC18-0177",
      "nt_dt": "31-01-2018",
      "chksum": "891970bd094f95bb4d4c66071878118dadc2980e2322db8eaf56999f83f1f0ab"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 28661.05
          }
        }
      ],
      "val": 28661.05,
      "flag": "N",
      "idt": "31-12-2017",
      "ntty": "C",
      "nt_num": "C1Z1IC18-017",
      "typ": "EXPWOP",
      "inum": "1Z1IC18-0179",
      "nt_dt": "31-01-2018",
      "chksum": "e26f3d3febae5753f0fe4a37fdab6c37e689b08d4d8eaf669dab308c4b72627e"
    },
    {
      "itms": [
        {
          "num": 1,
          "itm_det": {
            "rt": 0,
            "txval": 72192.8
          }
        }
      ],
      "val": 72192.8,
      "flag": "N",
      "idt": "31-12-2017",
      "ntty": "C",
      "nt_num": "C1Z1IC18-018",
      "typ": "EXPWOP",
      "inum": "1Z1IC18-0181",
      "nt_dt": "31-01-2018",
      "chksum": "a30fbc08f3200f261a8bb30a6ef01630be3f5a2da2732b820146aa3e88f3bec4"
    }
  ]
  }'
 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_GSTR1_D_SA]
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
			@B2cl nvarchar(max),
			@B2cs nvarchar(max),
			@Cdnr nvarchar(max),
			@Cdnur nvarchar(max),
			@Exp nvarchar(max),
			@Hsn nvarchar(max),
			@Nil nvarchar(max),
			@Txpd nvarchar(max),
			@At nvarchar(max)

	Select	@B2b = b2b,
			@B2cl = b2cl,
			@B2cs = b2cs,
			@Cdnr = cdnr,
			@Cdnur = cdnur,
			@Exp = [exp],
			@Hsn = hsn,
			@Nil = nil,
			@Txpd = txpd,
			@At = [at]
	From OPENJSON(@RecordContents) 
	WITH
	(	
		b2b nvarchar(max) as JSON,
		b2cl nvarchar(max) as JSON,
		b2cs nvarchar(max) as JSON,
		cdnr nvarchar(max) as JSON,
		cdnur nvarchar(max) as JSON,
		[exp] nvarchar(max) as JSON,
		hsn nvarchar(max) as JSON,
		nil nvarchar(max) as JSON,
		txpd nvarchar(max) as JSON,
		[at] nvarchar(max) as JSON
	)

	print @Cdnur

	If Ltrim(Rtrim(IsNull(@B2b,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_B2B_SA  @Gstin, @Fp, @B2b
	End

	If Ltrim(Rtrim(IsNull(@B2cl,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_B2CL_SA @Gstin, @Fp, @B2cl
	End

	If Ltrim(Rtrim(IsNull(@B2cs,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_B2CS_SA @Gstin, @Fp,@B2cs
	End

	If Ltrim(Rtrim(IsNull(@Cdnr,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_CDNR_SA @Gstin, @Fp, @Cdnr
	End

	If Ltrim(Rtrim(IsNull(@Cdnur,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_CDNUR_SA @Gstin, @Fp, @Cdnur
	End

	If Ltrim(Rtrim(IsNull(@Exp,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_EXP_SA @Gstin, @Fp, @Exp
	End

	If Ltrim(Rtrim(IsNull(@Hsn,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_HSN_SA @Gstin, @Fp,@Hsn
	End

	If Ltrim(Rtrim(IsNull(@Nil,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_NIL_SA  @Gstin, @Fp,@Nil
	End

	If Ltrim(Rtrim(IsNull(@Txpd,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_TXP_SA @Gstin, @Fp, @Txpd
	End

	If Ltrim(Rtrim(IsNull(@At,''))) <> ''
	Begin
		exec usp_Insert_JSON_GSTR1_D_AT_SA @Gstin, @Fp,@At
	End
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End