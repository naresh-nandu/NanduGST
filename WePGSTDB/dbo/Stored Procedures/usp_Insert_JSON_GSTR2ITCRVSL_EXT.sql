
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 ITC REVERSAL JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2ITCRVSL_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2ITCRVSL_EXT]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,

			Rule2_2.iamt as rule2_2_iamt,
			Rule2_2.camt as rule2_2_camt,
			Rule2_2.samt as rule2_2_samt,
			Rule2_2.csamt as rule2_2_csamt,

			Rule7_1_m.iamt as rule7_1_m_iamt,
			Rule7_1_m.camt as rule7_1_m_camt,
			Rule7_1_m.samt as rule7_1_m_samt,
			Rule7_1_m.csamt as rule7_1_m_csamt,

			Rule8_1_h.iamt as rule8_1_h_iamt,
			Rule8_1_h.camt as rule8_1_h_camt,
			Rule8_1_h.samt as rule8_1_h_samt,
			Rule8_1_h.csamt as rule8_1_h_csamt,

			Rule7_2_a.iamt as rule7_2_a_iamt,
			Rule7_2_a.camt as rule7_2_a_camt,
			Rule7_2_a.samt as rule7_2_a_samt,
			Rule7_2_a.csamt as rule7_2_a_csamt,

			Rule7_2_b.iamt as rule7_2_b_iamt,
			Rule7_2_b.camt as rule7_2_b_camt,
			Rule7_2_b.samt as rule7_2_b_samt,
			Rule7_2_b.csamt as rule7_2_b_csamt,

			Revitc.iamt as revitc_iamt,
			Revitc.camt as revitc_camt,
			Revitc.samt as revitc_samt,
			Revitc.csamt as revitc_csamt,

			Other.iamt as other_iamt,
			Other.camt as other_camt,
			Other.samt as other_samt,
			Other.csamt as other_csamt

	Into #TBL_EXT_GSTR2_ITCRVSL
	From OPENJSON(@RecordContents) 
	WITH
	(
		rule2_2 nvarchar(max) as JSON,
		rule7_1_m nvarchar(max) as JSON,
		rule8_1_h nvarchar(max) as JSON,
		rule7_2_a nvarchar(max) as JSON,
		rule7_2_b nvarchar(max) as JSON,
		revitc nvarchar(max) as JSON,
		other nvarchar(max) as JSON
	) As Itcrvsl
	Cross Apply OPENJSON(Itcrvsl.rule2_2)
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Rule2_2
	Outer Apply OPENJSON(Itcrvsl.rule7_1_m)
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Rule7_1_m
	Outer Apply OPENJSON(Itcrvsl.rule8_1_h )
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Rule8_1_h 
	Outer Apply OPENJSON(Itcrvsl.rule7_2_a )
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Rule7_2_a 
	Outer Apply OPENJSON(Itcrvsl.rule7_2_b )
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Rule7_2_b 
	Outer Apply OPENJSON(Itcrvsl.revitc  )
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Revitc  
	Outer Apply OPENJSON(Itcrvsl.other  )
	WITH
	(
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Other 

	-- Insert the Records Into External Tables

	Insert TBL_EXT_GSTR2_ITCRVSL
	(	gstin, fp,ruletype,iamt,camt,samt,csamt,   
		rowstatus, sourcetype, referenceno, createddate
	)
	Select 
		gstin, fp,'Rule2_2',rule2_2_iamt,rule2_2_camt,rule2_2_samt,rule2_2_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Rule7_1_m',rule7_1_m_iamt,rule7_1_m_camt,rule7_1_m_samt,rule7_1_m_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Rule8_1_h',rule8_1_h_iamt,rule8_1_h_camt,rule8_1_h_samt,rule8_1_h_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Rule7_2_a',rule7_2_a_iamt,rule7_2_a_camt,rule7_2_a_samt,rule7_2_a_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Rule7_2_b',rule7_2_b_iamt,rule7_2_b_camt,rule7_2_b_samt,rule7_2_b_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Revitc',revitc_iamt,revitc_camt,revitc_samt,revitc_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1
	Union All	
	Select 
		gstin, fp,'Other',other_iamt,other_camt,other_samt,other_csamt, 
		1 ,@SourceType ,@ReferenceNo,GetDate()
	From #TBL_EXT_GSTR2_ITCRVSL t1

	
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End