
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2B Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_LOGS_GSTR1 'WeP0022'


 */
 
CREATE PROCEDURE [usp_Retrieve_LOGS_GSTR1]
	@Period varchar(10),
	@ReferenceNo varchar(50)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	-- Selecting AT records
	Select
		ROW_NUMBER() OVER(ORDER BY atid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt,pos, sply_ty, rt, ad_amt, iamt, camt, samt, csamt,referenceno
		From TBL_EXT_GSTR1_AT 
		Where sourcetype = 'CSV'
		And referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus = 0
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By atid Desc

	-- Selecting B2B records
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, ctin, inum, idt, val, pos, rchrg, etin, inv_typ, hsncode,hsndesc, qty,uqc, unitprice, discount, 
		rt, txval,iamt, camt, samt, csamt, referenceno
		From TBL_EXT_GSTR1_B2B_INV 
		Where sourcetype = 'CSV'
		And referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus = 0
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By invid Desc

	-- Selecting B2CL records
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, pos, inum, idt, val, etin,hsncode, hsndesc, qty,uqc, unitprice, discount, rt, txval, iamt, csamt,referenceno  
		From TBL_EXT_GSTR1_B2B_INV 
		Where sourcetype = 'CSV'
		And referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus = 0
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By invid Desc




		
	Return 0

End