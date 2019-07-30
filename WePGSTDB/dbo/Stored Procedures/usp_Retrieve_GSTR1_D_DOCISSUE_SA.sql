
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1_D DOC ISSUED Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/23/2017	Seshadri 	Initial Version
08/30/2017	Seshadri	Modified the Where Condition to include the flag Value 0


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_D_DOCISSUE_SA '27AHQPA7588L1ZJ'


 */
 
Create PROCEDURE [usp_Retrieve_GSTR1_D_DOCISSUE_SA]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.docissueid Desc) AS 'SNo',
		t2.docissueid as docissueid,
		doc_num, 
		doc_typ, 
		[from] as froms,
		[to] as tos,
		totnum ,
		cancel, 
		net_issue
	From TBL_GSTR1_D t1
	Inner Join TBL_GSTR1_D_DOC_ISSUE t2 On t2.gstr1did = t1.gstr1did
	Inner Join TBL_GSTR1_D_DOCS t3 On t3.docissueid = t2.docissueid
	Where gstin = @Gstin
	And fp = @Fp
	And(t2.flag is Null Or (t2.flag <> 'D' And t2.flag <> '0'))
	Order By t2.docissueid Desc

	Return 0

End