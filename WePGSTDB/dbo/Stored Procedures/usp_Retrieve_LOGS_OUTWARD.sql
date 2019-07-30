
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Retrieve the GSTR1 B2B Records from the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/15/2017	Seshadri 	Initial Version
*/

/* Sample Procedure Call

exec usp_Retrieve_LOGS_OUTWARD '072017','WEP0031'


 */
 
CREATE PROCEDURE [usp_Retrieve_LOGS_OUTWARD]
	@Period varchar(10),
	@ReferenceNo varchar(50)
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	-- Selecting AT records
	Select
		ROW_NUMBER() OVER(ORDER BY atid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt,pos, sply_ty, rt, ad_amt, iamt, camt, samt, csamt,referenceno,sourcetype
		From TBL_EXT_GSTR1_AT 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By atid Desc

	-- Selecting B2B records
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, ctin, inum, idt, val, pos, rchrg, etin, inv_typ, hsncode,hsndesc, qty,uqc, unitprice, discount, 
		rt, txval,iamt, camt, samt, csamt,sourcetype, referenceno,buyerid,space(250) as ReceiverName 
		Into  #TBL_EXT_GSTR1_B2B_INV
		From TBL_EXT_GSTR1_B2B_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By invid Desc

		
		Update #TBL_EXT_GSTR1_B2B_INV 
			SET #TBL_EXT_GSTR1_B2B_INV.ReceiverName = t2.BuyerName
			FROM #TBL_EXT_GSTR1_B2B_INV t1,
					TBL_Buyer t2 
			WHERE t1.BuyerId = t2.BuyerId

		Select [S.No],gstin, fp, gt, cur_gt, ctin, inum, idt, val, pos, rchrg, etin, inv_typ, hsncode,hsndesc, qty,uqc, unitprice, discount, 
		rt, txval,iamt, camt, samt, csamt,sourcetype, referenceno, ReceiverName  from #TBL_EXT_GSTR1_B2B_INV

	-- Selecting B2CL records
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, pos, inum, idt, val, etin,hsncode, hsndesc, qty,uqc, unitprice, discount, rt, txval, iamt, csamt,sourcetype,referenceno, 
		buyerid,space(250) as ReceiverName 
		into #TBL_EXT_GSTR1_B2CL_INV  
		From TBL_EXT_GSTR1_B2CL_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		

		
		Update #TBL_EXT_GSTR1_B2CL_INV 
			SET #TBL_EXT_GSTR1_B2CL_INV.ReceiverName = t2.BuyerName
			FROM #TBL_EXT_GSTR1_B2CL_INV t1,
					TBL_Buyer t2 
			WHERE t1.BuyerId = t2.BuyerId

	Select [S.No],gstin, fp, gt, cur_gt, pos, inum, idt, val, etin,hsncode, hsndesc, qty,uqc, unitprice, discount, rt, txval, iamt, csamt,sourcetype,referenceno, 
		buyerid,ReceiverName from #TBL_EXT_GSTR1_B2CL_INV
	-- Selecting B2CS records
	--Select
	--	ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
	--	gstin, fp, gt, cur_gt,sply_ty, typ, etin, pos,hsncode, hsndesc, qty,uqc, unitprice,discount, txval,rt, iamt, camt, samt, csamt,sourcetype,referenceno        
	--	From TBL_EXT_GSTR1_B2CS 
	--	Where referenceno = @ReferenceNo
	--	And fp = @Period
	--	And rowstatus in (0,1)
	--	And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
	--	Order By invid Desc
	Select * into #tmp_B2CS from(Select invid,Space(25) as inum, gstin, fp, gt, cur_gt,sply_ty, typ, etin, pos,hsncode, hsndesc, qty,uqc, unitprice,discount, txval,rt, iamt, camt, samt, csamt,sourcetype,referenceno,buyerid,space(250) as ReceiverName       
		From TBL_EXT_GSTR1_B2CS 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Union ALL
	Select invid,inum,gstin, fp, gt, cur_gt,sply_typ, typ, etin, pos,hsncode, hsndesc, qty,uqc, unitprice,discount,txval,rt, iamt, camt, samt, csamt,sourcetype,referenceno,buyerid,space(250) as ReceiverName       
		From TBL_EXT_GSTR1_B2CS_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = '')As AB
		

		Update #tmp_B2CS 
			SET #tmp_B2CS.ReceiverName = t2.BuyerName
			FROM #tmp_B2CS t1,
					TBL_Buyer t2 
			WHERE t1.BuyerId = t2.BuyerId

		Select ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',gstin, fp, gt, cur_gt,inum,sply_ty, typ, etin, pos,hsncode, hsndesc, qty,uqc, unitprice,discount, txval,rt, iamt, camt, samt, csamt,sourcetype,ReceiverName,referenceno  from #tmp_B2CS

	-- Selecting CDNR records
	Select
		ROW_NUMBER() OVER(ORDER BY cdnrid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, ctin, cfs, ntty, nt_num, nt_dt, inum, idt, val, pos,hsncode, hsndesc, qty,uqc, unitprice, discount, rt, txval, iamt, camt, samt, csamt,sourcetype,referenceno 
        From TBL_EXT_GSTR1_CDNR 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By cdnrid Desc
		
	-- Selecting CDNUR records
	Select
		ROW_NUMBER() OVER(ORDER BY cdnurid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, typ, ntty, nt_num, nt_dt, inum, idt, val,hsncode,hsndesc, qty, unitprice, discount, rt, txval, iamt, camt, samt, csamt,sourcetype,referenceno 
        From TBL_EXT_GSTR1_CDNUR 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By cdnurid Desc
		
			
	-- Selecting DOC records
	Select
		ROW_NUMBER() OVER(ORDER BY docid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, doc_num, doc_typ, num, froms, tos, totnum, cancel, net_issue,sourcetype, referenceno
		From TBL_EXT_GSTR1_DOC 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By docid Desc
		
				
	-- Selecting EXP records
	Select
		ROW_NUMBER() OVER(ORDER BY invid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, exp_typ, inum, idt, val, sbnum, sbdt,hsncode, hsndesc, qty,uqc, unitprice,discount, txval, rt, iamt,sourcetype,referenceno 
        From TBL_EXT_GSTR1_EXP_INV 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By invid Desc
		
					
	-- Selecting HSN records
	Select
		ROW_NUMBER() OVER(ORDER BY hsnid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, hsn_sc, descs, uqc, qty, val, txval, iamt, camt, samt, csamt,sourcetype,referenceno
		From TBL_EXT_GSTR1_HSN 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By hsnid Desc
		
						
	-- Selecting NIL records
	Select
		ROW_NUMBER() OVER(ORDER BY nilid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, nil_amt, expt_amt, ngsup_amt, sply_ty,sourcetype, referenceno
		From TBL_EXT_GSTR1_NIL 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus = 0
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By nilid Desc
		
							
	-- Selecting TXP records
	Select
		ROW_NUMBER() OVER(ORDER BY txpid Desc) AS 'S.No',
		gstin, fp, gt, cur_gt, pos, sply_ty, rt, ad_amt, iamt, camt, samt, csamt,sourcetype, referenceno
		From TBL_EXT_GSTR1_TXP 
		Where referenceno = @ReferenceNo
		And fp = @Period
		And rowstatus in (0,1)
		And Ltrim(Rtrim(IsNull(errormessage,''))) = ''
		Order By txpid Desc
		
		
	Return 0

End