
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the External GSTR1 DOC ISSUE Records to the corresponding Staging
				Area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
09/01/2017	Seshadri	Fixed the imapct of SA Delete
09/01/2017	Seshadri	Fixed GSTR1 Upload Issue
09/04/2017	Seshadri	Fixed Flag Issue
09/05/2017	Seshadri	Fixed DOC ISSUE Push Error 
09/07/2017	Seshadri	Fixed DOC ISSUE Push Error 
12/27/2017	Seshadri	Fixed deleted Gstin Issue 
02/19/2018	Seshadri	Added Custid, Created By , Created Date for tracking Invoice Count 


*/

/* Sample Procedure Call

exec usp_Push_GSTR1DOCISSUE_EXT_SA  'POS','POSDEV1', '27AHQPA7588L1ZJ'

 */

CREATE PROCEDURE [usp_Push_GSTR1DOCISSUE_EXT_SA]
	@SourceType varchar(15),
	@ReferenceNo varchar(50), 
	@Gstin varchar(15),
	@CustId int,
	@CreatedBy int
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
	Row_Number() OVER(Order by GSTIN ASC) as slno,
	space(50) as gstr1id,
	space(50) as docissueid,
	t1.gstin,t2.gstinid,t1.fp,t1.gt,t1.cur_gt,
	t1.doc_num,t1.doc_typ,num,
	t1.froms,t1.tos,
	t1.totnum,t1.cancel,t1.net_issue
	Into #TBL_EXT_GSTR1_DOC
	From
	(
		SELECT 
			gstin,fp,gt,cur_gt,
			doc_num,doc_typ,num,
			froms,tos,
			sum(totnum) as totnum, sum(cancel) as cancel, sum(net_issue) as net_issue
		FROM TBL_EXT_GSTR1_DOC
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
            And sourcetype = @SourceType
            And referenceno = @ReferenceNo
            And rowstatus = 1
			And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Group By gstin,fp,gt,cur_gt,doc_num,doc_typ,num,froms,tos
	)t1
	Cross Apply 
	(
		Select gstinid from TBL_Cust_GSTIN where gstinno = t1.gstin and rowstatus = 1
	)t2


	-- Insert Records into Table TBL_GSTR1

	Insert TBL_GSTR1 (gstin,gstinId,fp,gt,cur_gt)
	Select	distinct gstin,gstinid,fp,gt,cur_gt
	From #TBL_EXT_GSTR1_DOC t1
	Where Not Exists(Select 1 From TBL_GSTR1 t2 Where t2.gstin = t1.gstin and t2.fp = t1.fp)

	Update #TBL_EXT_GSTR1_DOC 
	SET #TBL_EXT_GSTR1_DOC.gstr1id = t2.gstr1id 
	FROM #TBL_EXT_GSTR1_DOC t1,
			TBL_GSTR1 t2 
	WHERE t1.gstin = t2.gstin 
	And t1.gstinid = t2.gstinid 
	And t1.fp = t2.fp

	-- Insert Records into Table TBL_GSTR1_DOC_ISSUE

	Insert TBL_GSTR1_DOC_ISSUE(gstr1id,doc_num,doc_typ,gstinId,
								custid,createdby,createddate) 
	Select	distinct gstr1Id,doc_num,doc_typ,gstinId,
					@Custid,@CreatedBy,GetDate()
	From #TBL_EXT_GSTR1_DOC t1
	Where Not Exists ( SELECT 1 FROM TBL_GSTR1_DOC_ISSUE t2 where t2.gstr1id = t1.gstr1id
						And t2.doc_num = t1.doc_num
						And Isnull(t2.flag,'') = '' ) 

	Update #TBL_EXT_GSTR1_DOC
	SET #TBL_EXT_GSTR1_DOC.docissueid = t2.docissueid 
	FROM #TBL_EXT_GSTR1_DOC t1,
			TBL_GSTR1_DOC_ISSUE t2 
	WHERE t1.gstr1id = t2.gstr1id 
	And t1.doc_num = t2.doc_num
	And Isnull(t2.flag,'') = ''

	Update #TBL_EXT_GSTR1_DOC 
	SET #TBL_EXT_GSTR1_DOC.num = t3.num
	FROM #TBL_EXT_GSTR1_DOC t1,
			(Select docissueid,doc_num,Row_Number()  OVER(Partition By t2.docissueid Order By t2.docissueid) as num
			FROM #TBL_EXT_GSTR1_DOC t2
	   		) t3
	Where t1.docissueid = t3.docissueid 
	And t1.doc_num = t3.doc_num


	-- Insert Records into Table TBL_GSTR1_DOCS

	Insert TBL_GSTR1_DOCS
	(docissueid,num,[from],[to],
		totnum,cancel,net_issue,gstinId,gstr1id)
	Select	distinct t1.docissueid,t1.num,t1.froms,t1.tos,
			t1.totnum,t1.cancel,t1.net_issue,t1.gstinId,t1.gstr1id
	From #TBL_EXT_GSTR1_DOC t1
	Where	Not Exists ( SELECT 1 FROM TBL_GSTR1_DOCS t2
						Where t2.docissueid = t1.docissueid 
							And t2.num = t1.num
							And t2.[from] = t1.froms
							And t2.[to] = t1.tos
						)		 

	If Exists(Select 1 From #TBL_EXT_GSTR1_DOC)
	Begin
		Update TBL_EXT_GSTR1_DOC 
		SET rowstatus = 0 
		Where (Ltrim(Rtrim(IsNull(@Gstin,''))) = '' or gstin = @Gstin)
		And sourcetype = @SourceType
		And referenceno = @ReferenceNo
		And rowstatus = 1
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	End   
		
	Return 0

End