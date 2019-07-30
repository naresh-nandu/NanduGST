
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 AT Records from the corresponding staging area tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
08/02/2017	Seshadri	Introduced flag column in Where Clause
08/06/2017	Seshadri	Update the flag condition
08/31/2017	Seshadri	Introduced @Flag parameter 

*/

/* Sample Procedure Call

exec usp_Retrieve_GSTR1AT_SA '15BTBPL1540G1Z1','122017','U'


 */
 
CREATE PROCEDURE [usp_Retrieve_GSTR1AT_SA]  
	@GstinNo varchar(15),
	@Fp varchar(10),
	@Flag varchar(1) = Null
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select
		ROW_NUMBER() OVER(ORDER BY t3.atid Desc) AS 'SNo',
		t3.atid as atid,
		t2.pos, 
		t2.sply_ty,
		t3.rt,
		t3.ad_amt, 
		t3.iamt,
		t3.camt,
		t3.samt,
		t3.csamt
	From TBL_GSTR1 t1
	Inner Join TBL_GSTR1_AT t2 On t2.gstr1id = t1.gstr1id
	Inner Join TBL_GSTR1_AT_ITMS t3 On t3.atid = t2.atid
	Where gstin = @GstinNo
	And fp = @Fp
	And IsNull(flag,'') = @Flag
	Order By t3.atid Desc

	Return 0

End