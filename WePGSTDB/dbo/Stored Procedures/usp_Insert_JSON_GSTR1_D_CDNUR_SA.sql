

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D CDNUR JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/23/2017	Seshadri			Initial Version
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_CDNUR_SA  '29AAACB6820C1Z1','012018','[
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
  ]'
 */

CREATE PROCEDURE [dbo].[usp_Insert_JSON_GSTR1_D_CDNUR_SA]
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	space(50) as gstr1did,
			space(50) as cdnurid,
			space(50) as itmsid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			chksum as chksum,
			flag as flag,
			typ as typ,
			ntty as ntty,
			nt_num as nt_num,
			nt_dt as nt_dt,
			rsn as rsn,
			p_gst as p_gst,
			inum as inum,
			idt as idt,
			val as val,
			num as num,
			rt as rt,
			txval as txval,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt
	Into #TBL_GSTR1_D_CDNUR
	From OPENJSON(@RecordContents) 
	/*
	WITH
	(
	  cdnur nvarchar(max) as JSON
	) As Cdnurs
	Cross Apply OPENJSON(Cdnurs.cdnur) */
	WITH
	(
		chksum varchar(64),
		flag varchar(2),
		typ varchar(6),
		ntty varchar(1),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(1),
    	inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
    	itms nvarchar(max) as JSON
	) As Cdnur
	Cross Apply OPENJSON(Cdnur.itms)
	WITH
	(
		num int,
        itm_det nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
        txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
        csamt decimal(18,2)
	) As Itm_Det
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D
	
	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_CDNUR t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_CDNUR 
	SET #TBL_GSTR1_D_CDNUR.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_CDNUR t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_CDNUR

	Insert TBL_GSTR1_D_CDNUR
	(gstr1did,chksum,flag,typ,ntty,nt_num,nt_dt,rsn,p_gst,inum,idt,val,gstinid)
	Select	distinct t1.gstr1did,t1.chksum,t1.flag,t1.typ,t1.ntty,t1.nt_num,t1.nt_dt,t1.rsn,t1.p_gst,
					  t1.inum,t1.idt,t1.val,t1.gstinId
	From #TBL_GSTR1_D_CDNUR t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_CDNUR t2
						Where t2.gstr1did = t1.gstr1did 
						And t2.inum = t1.inum
						And t2.idt = t1.idt
						And(t2.flag is Null Or t2.flag <> 'D')
					)		 


	Update #TBL_GSTR1_D_CDNUR 
	SET #TBL_GSTR1_D_CDNUR.cdnurid = t2.cdnurid
	FROM #TBL_GSTR1_D_CDNUR t1,
			TBL_GSTR1_D_CDNUR t2 
	WHERE t1.gstr1did= t2.gstr1did 
	And t1.inum = t2.inum
	And t1.idt = t2.idt
	And(t2.flag is Null Or t2.flag <> 'D')
	
	
	Insert TBL_GSTR1_D_CDNUR_ITMS
	(cdnurid,num,gstinId,gstr1did)
	Select	distinct t1.cdnurid,t1.num,t1.gstinId,t1.gstr1did
	From #TBL_GSTR1_D_CDNUR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_CDNUR_ITMS t2
						Where t2.cdnurid = t1.cdnurid 
						And t2.num = t1.num)
	
	Update #TBL_GSTR1_D_CDNUR 
	SET #TBL_GSTR1_D_CDNUR.itmsid = t2.itmsid
	FROM #TBL_GSTR1_D_CDNUR t1,
		TBL_GSTR1_D_CDNUR_ITMS t2 
	WHERE t1.cdnurid= t2.cdnurid 
	And t1.num = t2.num
	
	Insert TBL_GSTR1_D_CDNUR_ITMS_DET
	(itmsid,rt,txval,iamt,camt,samt,csamt,gstinId,gstr1did)
	Select itmsid,rt,txval,iamt,camt,samt,csamt,gstinId,gstr1did
	From #TBL_GSTR1_D_CDNUR t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_D_CDNUR_ITMS_DET t2
						Where t2.itmsid = t1.itmsid 
						And t2.rt = t1.rt)
	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End