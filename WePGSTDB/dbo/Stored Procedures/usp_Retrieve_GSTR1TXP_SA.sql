
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 TXP Records from the corresponding staging area tables tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/26/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter 

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1TXP_SA '27AHQPA7588L1ZJ'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1TXP_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.txpid Desc) AS 'SNo',
		t3.txpid as txpid,
		pos, 
		sply_ty,
		rt,
		ad_amt, 
		iamt,
		camt,
		samt,
		csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_TXP t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_TXP_ITMS t3 On t3.txpid = t2.txpid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.txpid Desc

	
	Return 0

End