
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR2 all action Records(Inward Data) from the corresponding external tables for LOG files
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_LOGS_INWARD '062017','WeP0022'


 */
 
CREATE PROCEDURE [usp_Retrieve_LOGS_INWARD]
	@Period varchar(10),
	@ReferenceNo varchar(50)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	
	-- Selecting B2B records
	Select
		ROW_NUMBER() OVER(ORDER BY b2bid Desc) AS 'S.No',
		gstin, fp, ctin, inum, idt, val, pos, rchrg, inv_typ,hsncode, hsndesc, qty,uqc, unitprice, discount, rt, txval, iamt, camt, samt, csamt,sourcetype,referenceno
        From TBL_EXT_GSTR2_B2B_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By b2bid Desc

	-- Selecting B2BUR records
	select * from (Select
		ROW_NUMBER() OVER(ORDER BY b2burid Desc) AS 'S.No',
		gstin, fp, inum, idt, val, cname, hsncode, hsndesc, uqc, qty, unitprice, discount, rt, txval, camt, samt, csamt, sourcetype, referenceno                        
		From TBL_EXT_GSTR2_B2BUR_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		) As B2CL 


	-- Selecting CDNR records
	Select
		ROW_NUMBER() OVER(ORDER BY cdnid Desc) AS 'S.No',
		gstin, fp, ctin, nt_num, nt_dt, inum, idt, ntty, rt, txval, iamt, camt, samt, csamt,sourcetype, referenceno
		From TBL_EXT_GSTR2_CDN 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By cdnid Desc
		
	-- Selecting CDNUR records
	Select
		ROW_NUMBER() OVER(ORDER BY cdnurid Desc) AS 'S.No',
		gstin, fp, rtin, ntty, nt_num, nt_dt, inum, idt, txval, camt, samt, csamt, rt,sourcetype,referenceno
		From TBL_EXT_GSTR2_CDNUR 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By cdnurid Desc
		
					
	-- Selecting HSN records
	Select
		ROW_NUMBER() OVER(ORDER BY hsnid Desc) AS 'S.No',
		gstin, fp,hsn_sc, uqc, qty, val, txval, iamt, camt, samt, csamt, descs,sourcetype,referenceno
		From TBL_EXT_GSTR2_HSN 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By hsnid Desc
		
					
	-- Selecting IMPG records
	Select
		ROW_NUMBER() OVER(ORDER BY impgid Desc) AS 'S.No',
		gstin, fp,is_sez, stin, boe_num, boe_dt, boe_val,hsncode,hsndesc, uqc, qty, unitprice, discount, rt, txval, iamt, csamt,  num,sourcetype, referenceno
		From TBL_EXT_GSTR2_IMPG_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By impgid Desc
		
			
	-- Selecting IMPS records
	Select
		ROW_NUMBER() OVER(ORDER BY impsid Desc) AS 'S.No',
		impsid, gstin, fp, inum, idt, pos, ival, hsncode, hsndesc, uqc, qty,unitprice, discount, rt, txval, iamt, csamt, num,sourcetype, referenceno 
		From TBL_EXT_GSTR2_IMPS_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By impsid Desc
		
			
			
	-- Selecting ITCRVSL records
	Select
		ROW_NUMBER() OVER(ORDER BY itcrvslid Desc) AS 'S.No',
		gstin, fp, ruletype, iamt, camt, samt, csamt,sourcetype, referenceno
		From TBL_EXT_GSTR2_ITCRVSL 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By itcrvslid Desc
		
		
	
						
	-- Selecting NIL records
	Select
		ROW_NUMBER() OVER(ORDER BY nilid Desc) AS 'S.No',
		gstin, fp, niltype, cpddr, exptdsply, ngsply, nilsply,sourcetype, referenceno
		From TBL_EXT_GSTR2_NIL 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By nilid Desc
		
							
	-- Selecting TXI records
	Select
		ROW_NUMBER() OVER(ORDER BY txiid Desc) AS 'S.No',
		gstin, fp, pos, sply_ty, rt, adamt, camt, samt, csamt, iamt, num,sourcetype, referenceno
		From TBL_EXT_GSTR2_TXI 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By txiid Desc
		
								
	-- Selecting TXPD records
	Select
		ROW_NUMBER() OVER(ORDER BY txpdid Desc) AS 'S.No',
		gstin, fp, pos, sply_ty, rt, adamt, camt, samt, csamt, iamt, num,sourcetype, referenceno
		From TBL_EXT_GSTR2_TXPD 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		--And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By txpdid Desc
		
		
	Return 0

End