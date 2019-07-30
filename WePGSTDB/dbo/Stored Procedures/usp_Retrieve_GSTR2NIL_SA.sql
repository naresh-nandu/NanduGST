
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 NIL Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2NIL_SA '27AHQPA7588L1ZJ','052017'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2NIL_SA]  
	@GstinNo varchar(15),
	@fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	ROW_NUMBER() OVER(ORDER BY nilid desc) AS 'S.No',
			nilid,
			[Description] ,
			[Composition Taxable Person],
			[Exempt Supply],
			[Nil_Rated Supply],
			[Non GST Supply]
	From ( 
		Select
			t3.nilid AS 'nilid',
			'INTER' as 'Description',
			cpddr as 'Composition Taxable Person',
			exptdsply as 'Exempt Supply',
			nilsply as 'Nil_Rated Supply',
			ngsply as 'Non GST Supply'
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_NIL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_NIL_INTER t3 On t3.nilid = t2.nilid
		Where gstin = @GstinNo
		And fp = @Fp
		And IsNull(flag,'') = IsNull(@Flag,'')

		Union all 

		Select
			t3.nilid AS 'nilid',
			'INTRA' as 'Description',
			cpddr as 'Composition Taxable Person',
			exptdsply as 'Exempt Supply',
			nilsply as 'Nil_Rated Supply',
			ngsply as 'Non GST Supply'
		From TBL_GSTR2 t1
		Inner Join TBL_GSTR2_NIL t2 On t2.gstr2id = t1.gstr2id
		Inner Join TBL_GSTR2_NIL_INTRA t3 On t3.nilid = t2.nilid
		Where gstin = @GstinNo
		And fp = @Fp
		And IsNull(flag,'') = IsNull(@Flag,'')

	) as t1

	Order By t1.nilid Desc

	Return 0

End