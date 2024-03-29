﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 TXPD Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
09/12/2017	Seshadri	Added additional columns in the result set
09/21/2017	Seshadri	Fixed the retrieval issue because of @Flag Parameter


*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR2TXPD_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR2TXPD_SA]  
	@GstinNo varchar(15),
	@fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t2.txpdid desc) AS 'SNo', 
		t2.txpdid as txpdid,
		pos,
		sply_ty,		
		rt,
		adamt,
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR2 t1
	Inner Join TBL_GSTR2_TXPD t2 On t2.gstr2id = t1.gstr2id
	Inner Join TBL_GSTR2_TXPD_ITMS t3 On t3.txpdid = t2.txpdid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = IsNull(@Flag,'')
	Order By t2.txpdid Desc
		
	Return 0

End