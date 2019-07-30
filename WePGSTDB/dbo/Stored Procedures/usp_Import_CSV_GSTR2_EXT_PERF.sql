
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 2 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
09/06/2017	Seshadri 	Initial Version
10/01/2017	Seshadri	Tracking Invoices with different invoice values
10/05/2017	Seshadri	Fixed issues related to tracking invoices (No column name issue)
10/05/2017	Seshadri	Made code changes related to ItcEligibility Validation
10/6/2017	Seshadri	Fixed Vaidation Issue related to Supply Type and POS in B2B and B2BUR
10/13/2017	Seshadri	Tracking Invoices with different invoice values 
10/25/2017	Seshadri	Modified the code to store the ITC Eligibility flag in lower case
10/27/2017	Seshadri	Modified to handle extra document types because of template defect
02/01/2018	Seshadri	Modified the code to handle 2 newly added fields (User Id and Receipt Date)
05/11/2018  Karthik     'error in invoice item' - Fixed to check with in ctin same invoice no or not.

*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR2_EXT_PERF 'C:\Users\muskang\Desktop\TEST_DB APP\WePGST_Panel\webapp\App_Data\uploads\Updated Gstr2 Template with added 2 fields (2)_120180511181816466.csv','raja.m@wepindia.com','WEP001'

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_GSTR2_EXT_PERF]  
	@FileName varchar(255),
	@UserId varchar(255),
	@ReferenceNo varchar(50),
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1),
			@SourceType varchar(255),  
			@FileId int

	Select @Delimiter = ','
	Select @SourceType = 'CSV'

	Select @FileId = fileid
	From TBL_RECVD_FILES
	Where fileName = @FileName
	And createdby = @UserId
	And gstrtypeid = 2
	And filestatus = 1

	Update TBL_RECVD_FILES
	Set filestatus = 2
	Where fileid = @FileId

	Create Table #TBL_CSV_GSTR2_RECS
	(
		fileid int,
		slno varchar(50),
		compcode varchar(50),
		unitcode varchar(50),
		doctype varchar(50),
		gstin varchar(50),
		fp varchar(50),
		ctin varchar(50),
		inum varchar(50),
		idt varchar(50),
		inv_typ varchar(50),
		hsncode varchar(50),
		hsndesc varchar(50),
		uqc varchar(50),
		qty varchar(50),
		unitprice varchar(50),
		rt varchar(50),
		txval varchar(50),
		iamt varchar(50),
		camt varchar(50),
		samt varchar(50),
		csamt varchar(50),
		totval varchar(50),
		val varchar(50),
		pos varchar(50),
		rchrg varchar(50),
		rrate varchar(50),
		is_sez varchar(50),
		stin varchar(50),
		boe_num varchar(50),
		boe_dt varchar(50),
		boe_val varchar(50),
		port_code varchar(50),
		ntty varchar(50),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(50),
		rtin varchar(50),
		nil_sply_ty varchar(50),
		nilsply varchar(50),
		exptdsply varchar(50),
		ngsply varchar(50),
		cpddr varchar(50),
		sply_ty varchar(50),
		ad_paid_amt varchar(50),
		ad_adj_amt varchar(50),
		itc_elg varchar(50),
		tx_i varchar(50),
		tx_c varchar(50),
		tx_s varchar(50),
		tx_cs varchar(50),
		ruletype varchar(50),
		rvsl_iamt varchar(50),
		rvsl_camt varchar(50),
		rvsl_samt varchar(50),
		rvsl_csamt varchar(50),
		receivedby varchar(50),
		receiveddate varchar(50),
		errorcode smallint,
		errormessage varchar(255)
	)

	Begin Try

		Insert Into #TBL_CSV_GSTR2_RECS
		Select t1.*
		From TBL_CSV_GSTR2_RECS t1 
		Where t1.fileid = @FileId

	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch

	-- Processing Logic

	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_GSTR2_RECS

	if exists (Select 1 from #TBL_CSV_GSTR2_RECS)
	Begin

		Update #TBL_CSV_GSTR2_RECS -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_CSV_GSTR2_RECS -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #TBL_CSV_GSTR2_RECS
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Ctin = Upper(Ltrim(Rtrim(IsNull(Ctin,'')))),
			Ntty = Upper(Ltrim(Rtrim(IsNull(Ntty,'')))),
			Itc_Elg = Lower(Ltrim(Rtrim(IsNull(Itc_Elg,''))))

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Doc Type'
		Where dbo.udf_ValidateDocType(DocType) <> 1

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Ctin'
		Where (DocType In ('B2B','CDNR') and
			   dbo.udf_ValidateGstin(Ctin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Invoice No'
		Where ( DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceNo(Inum) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Invoice Type'
		Where (	DocType In ('B2B') and
				dbo.udf_ValidateInvoiceType(Inv_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Invoice Type'
		Where (	DocType = 'CDNUR' and
				(	Ltrim(Rtrim(IsNull(Inv_Typ,''))) <> '' and 
					Inv_Typ Not In ('B2BUR','IMPS') 
				)
			  )	
		And IsNull(ErrorCode,0) = 0

		/*
		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -8,
			ErrorMessage = 'HSN Code is mandatory'
		Where DocType In ('HSN')
		And Ltrim(Rtrim(IsNull(Hsncode,''))) = '' 
		And IsNull(ErrorCode,0) = 0  

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -9,
			ErrorMessage = 'Item Description is mandatory'
		Where DocType In ('HSN')
		And Ltrim(Rtrim(IsNull(Hsndesc,''))) = '' 
		And IsNull(ErrorCode,0) = 0 

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -10,
			ErrorMessage = 'Duplicate HSN Code'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType In ('HSN')
		And IsNull(ErrorCode,0) = 0 
		And Exists	(	Select Hsncode, count(*)
						From #TBL_CSV_GSTR2_RECS t2
						Where t2.Hsncode = t1.Hsncode
						And IsNull(ErrorCode,0) = 0 
						Group By Hsncode
						Having count(*) > 1
					)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -11,
			ErrorMessage = 'UQC is mandatory'
		Where DocType In ('HSN')
		And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
		And IsNull(ErrorCode,0) = 0

		*/

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Quantity'
		Where (DocType In ('HSN','HSN SUMMARY') and
			   dbo.udf_ValidateQuantity(Qty) <> 1)
		And IsNull(ErrorCode,0) = 0 

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Rate'
		Where (DocType In ('B2B','B2BUR','IMPG','IMPS','CDNR','CDNUR',
							'TXP','TXI',
							'TXPD','TAXL') and
			   dbo.udf_ValidateRate(Rt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -14,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2B','B2BUR','IMPG','IMPS','CDNR','CDNUR','HSN','HSN SUMMARY') and
			   dbo.udf_ValidateTaxableValue(Txval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') and
			   dbo.udf_ValidateInvoiceValue(Val) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -16,
			ErrorMessage = 'Place of Supply is mandatory'
		Where DocType In ('B2B','B2BUR','IMPS','TXP','TXI','TXPD','TAXL')
		And Ltrim(Rtrim(IsNull(Pos,''))) In ('','0')
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -17,
			ErrorMessage = 'Invalid Place Of Supply'
		Where ( DocType In ('B2B','B2BUR','IMPS','TXP','TXI','TXPD','TAXL') and
				dbo.udf_ValidatePlaceOfSupply(Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -18,
			ErrorMessage = 'Invalid Reverse Charge'
		Where ( DocType = 'B2B' and
				dbo.udf_ValidateReverseCharge(Rchrg) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -19,
			ErrorMessage = 'Invalid Elg Of Sez'
		Where ( DocType = 'IMPG' and
				dbo.udf_ValidateIsSEZ(is_sez) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -20,
			ErrorMessage = 'Invalid Stin(Sez Gstin)'
		Where ( DocType = 'IMPG' and
				Ltrim(Rtrim(IsNull(is_sez,''))) = 'Y' and
				dbo.udf_ValidateGstin(Stin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS  
		Set ErrorCode = -21,
			ErrorMessage = 'Invalid Bill of Entry Number'
		Where (	DocType = 'IMPG' and
				dbo.udf_ValidateBOENo(boe_num) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS  
		Set ErrorCode = -22,
			ErrorMessage = 'Invalid Bill Of Entry Date'
		Where (	DocType = 'IMPG' and
				dbo.udf_ValidateDate(boe_dt) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS  
		Set ErrorCode = -23,
			ErrorMessage = 'Invalid Bill of Entry Value'
		Where (	DocType = 'IMPG' and
				dbo.udf_ValidateAmount(boe_val) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS  
		Set ErrorCode = -24,
			ErrorMessage = 'Invalid Port Code'
		Where (	DocType = 'IMPG' and
				dbo.udf_ValidatePortCode(port_code) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -25,
			ErrorMessage = 'Invalid Note Type'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteType(Ntty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -26,
			ErrorMessage = 'Invalid Note No'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteNo(Nt_Num) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -27,
			ErrorMessage = 'Invalid Note Date'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Idt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -28,
			ErrorMessage = 'Invalid Pre GST Regime'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidatePreGstRegime(P_Gst) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -29,
			ErrorMessage = 'Invalid Receiver Gstin(Rtin)'
		Where ( DocType = 'CDNUR' and
				Ltrim(Rtrim(IsNull(Rtin,''))) <> '' and
				dbo.udf_ValidateGstin(Rtin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -30,
			ErrorMessage = 'Invalid Nil Rated Supply Type'
		Where (	DocType = 'NIL' and
				(	Ltrim(Rtrim(IsNull(Nil_Sply_Ty,''))) = '' Or 
					Nil_Sply_Ty Not In ('INTER','INTRA') 
				)
			  )	
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -31,
			ErrorMessage = 'Invalid Nil Rated Supply Amount'
		Where (	DocType = 'NIL' and
				Ltrim(Rtrim(IsNull(Nilsply,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Nilsply) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -32,
			ErrorMessage = 'Invalid Value of Exempted Supplies Received '
		Where (	DocType = 'NIL' and
				Ltrim(Rtrim(IsNull(Exptdsply,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Exptdsply) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -33,
			ErrorMessage = 'Invalid Total Non GST Outward Supplies'
		Where (	DocType = 'NIL' and
				Ltrim(Rtrim(IsNull(Ngsply,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Ngsply) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -34,
			ErrorMessage = 'Invalid Value of supplies received from Compounding Dealer'
		Where (	DocType = 'NIL' and
				Ltrim(Rtrim(IsNull(Cpddr,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Cpddr) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -35,
			ErrorMessage = 'Invalid Supply Type'
		Where ( DocType In ('B2BUR','TXP','TXI','TXPD','TAXL') and
				(	Ltrim(Rtrim(IsNull(Sply_Ty,''))) = '' Or 
					Sply_Ty Not In ('INTER','INTRA') 
				)
			  )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -36,
			ErrorMessage = 'Invalid Advance Amount to be Adjusted'
		Where (	DocType In ('TXP','TXPD') and
				dbo.udf_ValidateAmount(Ad_Adj_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -37,
			ErrorMessage = 'Invalid Gross Advance Paid'
		Where (	DocType In ('TXI','TAXL') and
				dbo.udf_ValidateAmount(Ad_Paid_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -38,
			ErrorMessage = 'Invalid ITC Eligibility'
		Where (	DocType In ('B2B','B2BUR','IMPG','IMPS','CDNR','CDNUR')  and
				dbo.udf_ValidateItcEligibility(Itc_Elg) <> 1
			  )	
		And IsNull(ErrorCode,0) = 0


		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -39,
			ErrorMessage = dbo.udf_ValidateGstItcAmount(DocType,Gstin,Ctin,Pos,Sply_Ty,Rt,Txval,Iamt,CAmt,Samt,Csamt,Tx_I,Tx_C,Tx_S,Tx_Cs,
								Ad_Adj_Amt,Ad_Paid_Amt,Inv_Typ,Inum,Idt,p_gst) 
		Where DocType In ('B2B','B2BUR','IMPG','IMPS','TXP','TXI','TXPD','TAXL') 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -39
		And IsNull(ErrorMessage,'') = ''
	
		Update #TBL_CSV_GSTR2_RECS 
		Set Ntty =
		Case Ntty
			When 'CREDIT' Then 'C'
			When 'DEBIT'  Then 'D'
			When 'REFUND' Then 'R'
			Else Ntty
		End
		Where ( DocType In ('CDNR','CDNUR') )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set rsn =
		Case rsn
			When '1.Sales Return' Then '01-Sales Return'
			When '2.Post Sale Discount'  Then '02-Post Sale Discount'
			When '3.Deficiency in services' Then '03-Deficiency in services'
			When '4.Correction in Invoice' Then '04-Correction in Invoice'
			When '5.Change in POS' Then '05-Change in POS'
			When '6.Finalization of Provisional assessment' Then '06-Finalization of Provisional assessment'
			When '7.Others' Then '07-Others'
			else rsn
		End
		Where ( DocType In ('CDNR','CDNUR') )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -40,
			ErrorMessage = 'Invalid Reason for Issuing Dr./ Cr. Notes'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteRsn(rsn) <> 1) 
		And IsNull(ErrorCode,0) = 0

	
		Update #TBL_CSV_GSTR2_RECS 
		Set Idt = convert(varchar,(SELECT convert(datetime, idt, 103)),105)
		Where ( DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') )
		And Ltrim(Rtrim(IsNull(Idt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set Nt_dt = convert(varchar,(SELECT convert(datetime, nt_dt, 103)),105)
		Where ( DocType In ('CDNR','CDNUR') )
		And Ltrim(Rtrim(IsNull(Nt_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR2_RECS 
		Set Boe_dt = convert(varchar,(SELECT convert(datetime, boe_dt, 103)),105)
		Where ( DocType In ('IMPG') )
		And Ltrim(Rtrim(IsNull(Boe_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with different invoice value

		Select distinct t.doctype,t.gstin,t.fp,t.ctin,t.inum,t.idt,count(*) as cnt
		Into #TmpInvoices
		From
		(
			Select distinct doctype,gstin,fp,ctin,inum,idt,val
			From #TBL_CSV_GSTR2_RECS
			Where DocType in ('B2B','B2BUR')
			group by doctype,gstin,fp,ctin,inum,idt,val
		 ) t
		group by doctype,gstin,fp,ctin,inum,idt
		having count(*) > 1

		If Exists(Select 1 From #TmpInvoices)
		Begin
			Update #TBL_CSV_GSTR2_RECS 
			Set ErrorCode = -41,
				ErrorMessage = 'Invoice Value is different for line items.'
			From #TBL_CSV_GSTR2_RECS t1,
				 #TmpInvoices t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And IsNull(t1.ctin,'') = IsNull(t2.ctin,'') 
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices with item errors

		Select distinct doctype,gstin,ctin,inum,idt
			Into #TmpRecords
		From #TBL_CSV_GSTR2_RECS  
		Where DocType in ('B2B','B2BUR','IMPS','CDNR','CDNUR')
		And IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #TBL_CSV_GSTR2_RECS 
			Set ErrorCode = -42,
				ErrorMessage = 'Error in Invoice Item.'
			From #TBL_CSV_GSTR2_RECS t1,
				 #TmpRecords t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And IsNull(t1.Ctin,'') = IsNull(t2.Ctin,'') 
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2B_INV 
								Where gstin = t1.gstin
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2B_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'B2BUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2BUR_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'B2BUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2BUR_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'IMPG' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPG_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And boe_num = t1.boe_num 
								And boe_dt = t1.boe_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'IMPG' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPG_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And boe_num = t1.boe_num 
								And boe_dt = t1.boe_dt
								And rowstatus <> 0)


		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'IMPS' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'IMPS' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)


		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)


		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'HSN data already uploaded'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType In ('HSN','HSN SUMMARY') 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_HSN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And hsn_sc = t1.hsncode 
								And descs = t1.hsndesc
								And rowstatus = 0)

		Update #TBL_CSV_GSTR2_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'HSN data already imported'
		From #TBL_CSV_GSTR2_RECS t1
		Where DocType In ('HSN','HSN SUMMARY') 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_HSN   
								Where gstin = t1.gstin 
								And fp = t1.fp
								And hsn_sc = t1.hsncode 
								And descs = t1.hsndesc
								And rowstatus <> 0)


		-- Import B2B Invoices

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'B2B')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType = 'B2B'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_B2B_INV t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2B'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_B2B_INV
				(	gstin, fp, ctin,inum, idt, val, pos, rchrg, inv_typ, 
					rt, txval, iamt, camt, samt, csamt,
					elg,tx_i,tx_c,tx_s,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice,
					compcode,unitcode,receivedby,receiveddate)
				Select 
					gstin, fp, ctin,inum, idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					pos, rchrg, inv_typ, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),

					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_c,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_s,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
					
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0)))),

					compcode,unitcode,receivedby,receiveddate

				From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'B2B' 
				And IsNull(ErrorCode,0) = 0
					
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End

		-- Import B2BUR Invoices

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'B2BUR')
		Begin
			
			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType = 'B2BUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_B2BUR_INV t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2BUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'B2BUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_B2BUR_INV
				(	gstin, fp, inum, idt, val,  
					sply_ty,pos, 
					rt, txval, iamt, camt, samt,csamt,
					elg,tx_i,tx_c,tx_s,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
				Select 
					gstin, fp, inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					sply_ty,pos,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),

					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_c,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_s,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
					
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From  #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'B2BUR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'B2BUR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End

		-- Import IMPG

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'IMPG')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType = 'IMPG'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_IMPG_INV t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.boe_num = t2.boe_num
					And t1.boe_dt = t2.boe_dt
					And DocType = 'IMPG'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'IMPG' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_IMPG_INV
				(	gstin, fp, is_sez,stin,port_code,boe_num,boe_dt,boe_val,  
					rt,txval,iamt,csamt,
					elg,tx_i,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice
				)
				Select 
					gstin, fp, is_sez,stin,port_code,boe_num,boe_dt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(boe_val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'IMPG' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'IMPG' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import IMPS

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'IMPS')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType = 'IMPS'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_IMPS_INV t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'IMPS'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'IMPS' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_IMPS_INV
				(	gstin, fp, inum, idt, pos, ival,  
					rt,txval,iamt,csamt,
					elg,tx_i,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice
				)
				Select 
					gstin, fp, inum, idt, pos,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
					1 ,@SourceType , @ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'IMPS' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'IMPS' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End


		-- Import CDNR

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'CDNR')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType = 'CDNR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_CDN t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_CDN
				(	gstin, fp, ctin, ntty, nt_num, nt_dt, 
					rsn,p_gst,
					inum, idt,val,  
					rt, txval, iamt, camt, samt, csamt,
					elg,tx_i,tx_c,tx_s,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					compcode,unitcode,receivedby,receiveddate)
				Select 
					gstin, fp, ctin,ntty,nt_num, nt_dt, 
					rsn,p_gst,
					inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),

					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_c,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_s,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
			
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,

					compcode,unitcode,receivedby,receiveddate

				 From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'CDNR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import CDNUR

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS  Where DocType = 'CDNUR' )
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS 
							 Where DocType = 'CDNUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_CDNUR t1,
							#TBL_CSV_GSTR2_RECS  t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.inum
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_CDNUR
				(	gstin, fp, rtin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt,inv_typ,val,   
					rt,txval, iamt, camt, samt, csamt,
					elg,tx_i,tx_c,tx_s,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, rtin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt,inv_typ, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
		 	
					itc_elg,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_i,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_c,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_s,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(tx_cs,''),0)))),
	
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'CDNUR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import HSN

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType In ('HSN','HSN SUMMARY'))
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR2_RECS
							 Where DocType In ('HSN','HSN SUMMARY')
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From TBL_EXT_GSTR2_HSN t1,
							#TBL_CSV_GSTR2_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.hsn_sc = t2.hsncode
					And t1.descs = t2.hsndesc
					And DocType In ('HSN','HSN SUMMARY')
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType In ('HSN','HSN SUMMARY') 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_HSN
				(	gstin, fp, hsn_sc, descs, uqc, qty, val,  
					txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp, hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR2_RECS t1
				Where DocType In ('HSN','HSN SUMMARY') 
				And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in HSN Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType In ('HSN','HSN SUMMARY') 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import NIL

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType = 'NIL' )
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_NIL
				(	gstin, fp,niltype,cpddr,exptdsply, ngsply,nilsply,   
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp,'INTER',
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cpddr,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(exptdsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ngsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(nilsply,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'NIL' 
				And nil_sply_ty = 'INTER'
				Union All
				Select 
					gstin, fp,'INTRA',
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cpddr,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(exptdsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ngsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(nilsply,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR2_RECS t1
				Where DocType = 'NIL' 
				And nil_sply_ty = 'INTRA'
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType = 'NIL' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End
		
		-- Import TXP

		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType In ('TXP','TXPD') )
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_TXPD
				(	gstin, fp, pos, sply_ty,   
					rt, adamt,camt, samt, csamt,iamt, 
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, pos, sply_ty, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_adj_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From #TBL_CSV_GSTR2_RECS t1
				Where DocType In ('TXP','TXPD') 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType In ('TXP','TXPD') 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import TXI
	
		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType In ('TXI','TAXL'))
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_TXI
				(	gstin, fp, pos, sply_ty,   
					rt, adamt,camt, samt, csamt,iamt, 
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, pos, sply_ty,   
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_paid_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From  #TBL_CSV_GSTR2_RECS t1
				Where DocType In ('TXI','TAXL') 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType In ('TXI','TAXL') 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import ITCRVSL
	
		if Exists (Select 1 From #TBL_CSV_GSTR2_RECS Where DocType In ('ITCRVSL','ITC REV'))
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_ITCRVSL
				(	gstin, fp, ruletype,   
					iamt,camt, samt, csamt, 
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp,ruletype,   
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rvsl_iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rvsl_camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rvsl_samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rvsl_csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From  #TBL_CSV_GSTR2_RECS t1
				Where DocType In ('ITCRVSL','ITC REV') 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR2_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR2_RECS t1
					Where DocType In ('ITCRVSL','ITC REV') 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End


	End 

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_CSV_GSTR2_RECS fv
	Where IsNull(fv.ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_CSV_GSTR2_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Update TBL_RECVD_FILES
	Set filestatus = 0,
		totalrecordscount = @TotalRecordsCount,
		processedrecordscount = @ProcessedRecordsCount,
		errorrecordscount = @ErrorRecordsCount
	Where fileid = @FileId

	Insert Into TBL_RECVD_FILE_ERRORS
	Select @FileId,fv.Slno,fv.ErrorCode,fv.ErrorMessage
	From  #TBL_CSV_GSTR2_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Select * from #TBL_CSV_GSTR2_RECS
	Where IsNull(ErrorCode,0) <> 0

	--Select fileid as 'Fileid', slno as 'Sno',compcode as 'Comp Code',unitcode as 'Unit Code',doctype as 'Document Type', gstin as 'GSTIN',
	--fp as 'Period', ctin as 'Ctin', inum as 'Invoice Number',idt as 'Invoice Date', inv_typ as 'Invoice Type',hsncode as 'HSN Code',
	--hsndesc as 'HSN Description', uqc as 'UQC',qty as 'Quantity', unitprice as 'Unit Price',rt as 'Rate', txval as 'Taxable Value',iamt as 'IGST Amount',
	--camt as 'CGST Amount', samt as 'SGST Amount', csamt as 'Cess Amount',totval as 'Total Value', val as 'Invoice Value',pos as 'Place of Supply',
	--rchrg as 'Reverse Charge',rrate as 'rrate',is_sez as 'Elg Of Sez', stin as 'Stin(Sez Gstin)', boe_num as 'Bill of Entry Number',
 --   boe_dt as 'Bill Of Entry Date',boe_val as 'Bill of Entry  Value',port_code as 'PORT CODE', ntty as 'Note Type', nt_num as 'Note Number',
 --   nt_dt as 'Note Date',rsn as 'Reason for Issuing Dr./ Cr. Notes',p_gst as 'Pre GST', rtin as 'Receiver Gstin(Rtin)',nil_sply_ty as 'Nil Rated Supply Type',
	--nilsply  as 'Nil Rated Supply Amount', exptdsply as 'exptdsply', ngsply as 'Non GST Supplies',cpddr as 'cpddr', sply_ty as 'Supply Type', 
	--ad_paid_amt as 'Gross Advance Paid', ad_adj_amt as 'Advance Amount to be Adjusted', itc_elg as 'ITC Eligibility', tx_i  as 'ITC Tax Amount for IGST',
	--tx_c as 'ITC Tax Amount for CGST',tx_s as 'ITC Tax Amount for SGST',tx_cs as 'ITC Tax Amount for Cess',ruletype as 'Rule Type',rvsl_iamt as 'RVCL IGST Amount',
	--rvsl_camt as 'RVCL CGST Amount',rvsl_samt as 'RVCL SGST Amount',rvsl_csamt as 'RVCL CESS Amount', receivedby as 'User Id',receiveddate as 'Received Date',
	--errorcode as 'errorcode ', errormessage as 'errormessage'
	--from #TBL_CSV_GSTR2_RECS
	--Where IsNull(ErrorCode,0) <> 0

	 -- Drop Temp Tables

	 Drop Table #TBL_CSV_GSTR2_RECS
	-- Delete From TBL_CSV_GSTR2_RECS Where fileid = @FileId
	
	 Return 0


End