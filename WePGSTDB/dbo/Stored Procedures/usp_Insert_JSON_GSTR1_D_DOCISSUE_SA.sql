
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1_D DOC_Issue JSON Records to the corresponding statging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/22/2017	Seshadri		Initial Version
08/28/2017	Seshadri	Fixed Insertion issues for deleted use case


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1_D_DOCISSUE_SA  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1_D_DOCISSUE_SA]
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
			space(50) as docissueid,
			@Gstin as gstin,
			t1.gstinid as gstinid,
			@Fp as fp,
			chksum as chksum,
			doc_num as doc_num,
			num as num,
			[from] as [from],
			[to] as [to],
			totnum as totnum,
			cancel as cancel,
			net_issue as net_issue
	Into #TBL_GSTR1_D_DOC
	From OPENJSON(@RecordContents) 
	WITH
	(
	  doc_issue nvarchar(max) as JSON
	) As Doc_Issues
	Cross Apply OPENJSON(Doc_Issues.doc_issue) 
	WITH
	(	
		chksum varchar(64),
		doc_det nvarchar(max) as JSON
	) As Doc_Issue
	Cross Apply OPENJSON(Doc_Issue.doc_det) 
	WITH
	(
		doc_num int,
		docs nvarchar(max) as JSON
	) As Doc_Det
	Cross Apply OPENJSON(Doc_Det.docs) 
	WITH
	(
		num int,
		[from] varchar(50),
		[to] varchar(50),
		totnum int,
		cancel int,
		net_issue int
	) As Docs
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = @Gstin
	)t1

	-- Insert Records into Table TBL_GSTR1_D

	Insert TBL_GSTR1_D (gstin,gstinId,fp)
	Select	distinct gstin,gstinid,fp
	From #TBL_GSTR1_D_DOC t1
	Where Not Exists(Select 1 From TBL_GSTR1_D t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_GSTR1_D_DOC 
	SET #TBL_GSTR1_D_DOC.gstr1did = t2.gstr1did 
	FROM #TBL_GSTR1_D_DOC t1,
			TBL_GSTR1_D t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_D_DOC_ISSUE

	Insert TBL_GSTR1_D_DOC_ISSUE(gstr1did,chksum,doc_num,gstinId) 
	Select	distinct gstr1dId,chksum,doc_num,gstinId
	From #TBL_GSTR1_D_DOC t1
	Where Not Exists (	SELECT 1 FROM TBL_GSTR1_D_DOC_ISSUE t2 
						Where t2.gstr1did = t1.gstr1did
						And(t2.flag is Null Or t2.flag <> 'D')
					 ) 

	Update #TBL_GSTR1_D_DOC
	SET #TBL_GSTR1_D_DOC.docissueid = t2.docissueid 
	FROM #TBL_GSTR1_D_DOC t1,
			TBL_GSTR1_D_DOC_ISSUE t2 
	WHERE t1.gstr1did = t2.gstr1did 
	And(t2.flag is Null Or t2.flag <> 'D')

	-- Insert Records into Table TBL_GSTR1_D_DOCS

	Insert TBL_GSTR1_D_DOCS
	(docissueid,num,[from],[to],
		totnum,cancel,net_issue,gstinId,gstr1did)
	Select	distinct t1.docissueid,t1.num,t1.[from],t1.[to],
			t1.totnum,t1.cancel,t1.net_issue,t1.gstinId,t1.gstr1did
	From #TBL_GSTR1_D_DOC t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_D_DOCS t2
						Where t2.docissueid = t1.docissueid 
							And t2.[from] = t1.[from]
							And t2.[to] = t1.[to]
						)		 

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End