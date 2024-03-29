﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1_D TXP Records from the corresponding staging area tables tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
08/1/2017	Seshadri 	Initial Version
08/10/2017	Karthik		Introduced Aliases in the Result Set
08/22/2017	Seshadri	Removed the aliases in the result set
08/23/2017	Seshadri	Modified the Where Condition
08/30/2017	Seshadri	Modified the Where Condition to include the flag Value 0


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1_D_TXP_SA


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1_D_TXP_SA]  
	@Gstin varchar(15),
	@Fp varchar(10)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.txpdid Desc) AS 'SNo',
		t3.txpdid as txpdid,
		pos, 
		sply_ty,
		rt,
		ad_amt, 
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR1_D t1
	Inner Join TBL_GSTR1_D_TXPD t2 On t2.gstr1did = t1.gstr1did
	Inner Join TBL_GSTR1_D_TXPD_ITMS t3 On t3.txpdid = t2.txpdid
	Where gstin = @Gstin
	And fp = @Fp
	And(t2.flag is Null Or (t2.flag <> 'D' And t2.flag <> '0'))
	Order By t3.txpdid Desc

	
	Return 0

End