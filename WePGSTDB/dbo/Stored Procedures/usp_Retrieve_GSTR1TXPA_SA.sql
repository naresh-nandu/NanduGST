﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 TXPA Records from the corresponding staging area tables tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
03/05/2018	Seshadri 	Initial Version

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1TXPA_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1TXPA_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.txpaid Desc) AS 'SNo',
		t3.txpaid as txpaid,
		pos, 
		sply_ty,
		rt,
		ad_amt, 
		iamt,
		camt,
		samt,
		csamt,
		omon,
		diff_percent
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_TXPA t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_TXPA_ITMS t3 On t3.txpaid = t2.txpaid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.txpaid Desc

	
	Return 0

End