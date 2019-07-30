
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
08/08/2017	Seshadri	Included typ for B2CS Records ; rsn,p_gst for CDNR Records
08/10/2017	Seshadri	Reintroduced HSN Doc Type 
08/17/2017	Seshadri	Performance Related Changes
8/18/2017	Seshadri	Modified the code related to HSN
8/19/2017	Seshadri	Removed Mandatory Check for Invoice Value in case of B2CS
8/28/2017	Seshadri	Made Changes related to Note Type
8/28/2017	Seshadri	Modified to support Deemed Exports / Supplies to SEZ
						for B2B Action Type		
8/28/2017	Seshadri	Modified the code to support re-import HSN records
8/29/2017	Seshadri	Increased the length of Doc Nature
8/29/2017	Seshadri	Fixed Issues related to support re-import HSN records	
8/31/2017	Seshadri	Introduced Validation For Nature of Document	
8/31/2017	Seshadri	Modified the code related to Nil Action Type Validation
9/2/2017	Seshadri	Fixed DocIssue Section
9/3/2017	Seshadri	Fixed Note Reason Validation Issue
9/4/2017	Karthik		Modified the code to store that Date(idt,nt_dt,sbdt) in dd-mm-yyyy format
9/7/2017	Seshadri	Fixed Invalid Nature of Document Issue
9/8/2017	Seshadri	Modified the function call - udf_ValidateGstAmount 
9/9/2017	Seshadri	Made code changes to trim Gstin,Ctin,Etin & Ntty
10/3/2017	Seshadri	Introduced the period in the where clause while deleting already imported data for HSN
10/4/2017	Seshadri	Bypassed the Tax Amount Validation for Exp (WOPAY)
10/13/2017	Seshadri	Tracking Invoices with different invoice values 
11/17/2017	Seshadri	Made Code Changes for CTIN Validation
11/27/2017	Seshadri	Included Validation for From and To Serial Number
11/30/2017	Seshadri	Commented CTIN API Validation Section
12/2/2017	Seshadri	Code Changes related to GSTR1 New Template (Template B)
12/6/2017	Seshadri	Introduced Validation for Shipping Bill Port Code &
						Shipping Bill Number
12/10/2017	Seshadri	Bug fixes related to GSTR1 New Template (Template B)
12/11/2017	Seshadri	Bug fixes related to GSTR1 New Template (Template B)
12/12/2017	Seshadri	Bug fixes related to GSTR1 New Template (Template B)
12/15/2017	Seshadri	Fixed B2B Invoice Identification Logic
12/21/2017	Seshadri	Made code changes related to Invoice Date Validation wrt CDNR and CDNUR
12/28/2017	Seshadri	Bypassed Amount Validation for CDNUR Records in case of Template B
12/29/2017	Seshadri	Invoked the function udf_ValidateCDNForB2CSInvoice for CDNUR Records in case of Template B
12/29/2017  Karthik     Fixed update the CDNUR_TYP(B2CL/B2CS) for CDNUR Action Type data for template B Automatically
01/05/2018  Karthik		Fixed Included rowstatus of Userlist and GSTIN both table fetching query.
1/8/2018	Seshadri	Allow negative taxable value for HSN 
2/1/2018	Karthik		Fixed for Based on Template A or Template B Import error records will be download in the respective format.
2/7/2018	Seshadri	Made Code Changes for CTIN Validation as per new design
2/7/2018	Seshadri	Commented the Code Changes for CTIN Validation as per new design
2/7/2018	Seshadri	Made Code Changes for HSN Inclusion in case of Template B
2/8/2018	Seshadri	Made Code Changes for CDNUR - B2CS Inclusion in case of Template B
2/9/2018	Seshadri	Made Code Changes for CDNUR_TYP Determination.Removed extra check points
2/9/2018	Seshadri	Made Temp Code Changes for CDNUR_TYP Determination.
2/22/2018	Karthik		Made code changes to EXP Doc Type as per Ver 1.1 Changes (Added csamt)
2/28/2018	Seshadri	Made code changes related to HSN Code and Desc Validation.
3/1/2018	Seshadri	Made code changes related to UIN (Embassy GSTIN) format.
3/6/2018	Seshadri	Made code changes related to Numeric Data Vaidation Check
3/6/2018	Seshadri	Made code changes related to Settings (Ctin,HSN)
3/6/2018	Seshadri	Made code changes to support amendments
3/8/2018	Seshadri	Made code changes to track Invoices with different invoice dates &
						values for line items
3/12/2018	Seshadri	Made code changes to support amendments (Fixed Unit Testing defects)
3/13/2018	Seshadri	Made code changes to support amendments (Fixed Unit Testing defects)
3/15/2018	Seshadri	Made code changes to support B2CS amendments 
3/21/2018	Seshadri	Fixed QC defect related to HSN & CTIN Settings
3/23/2018	Seshadri	Fixed QC defect related to UQC Mandatory Check wrt Template A
3/23/2018	Seshadri	Removed commented code
3/26/2018	Seshadri	Made code changes to support 0 taxable Value & invoice value for Amendments
3/27/2018	Seshadri	Made code changes to support amendments for Template B
4/3/2018	Seshadri	Allow negative taxable value for B2CS
04/04/2018	Seshadri	Modified the code to store HSN details for CDNR and CDNUR records
04/04/2018	Seshadri	Modified the code to store Pos and Supply type for CDNUR records
04/09/2018	Seshadri	Modified the code to trim leading / trailing spaces for Invoice and Note Nos
04/13/2018	Seshadri	Modified the code to include validations for CDNR and CDNUR (Note Num - Invoice Mapping)
04/16/2018	Seshadri	Modified the code to include Cdnur Type Validations for CDNUR & CDNURA  
*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR1_EXT_PERF 'GSTR1 Sample Live Data','raja.m@wepindia.com'

 */
 
CREATE PROCEDURE usp_Import_CSV_GSTR1_EXT_PERF
	@FileName varchar(255),
	@UserId varchar(255),  
	@ReferenceNo varchar(50),
	@TemplateTypeId tinyint = 1, -- 1 - Template A ; 2 - Template B
	@CustId int,
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
	And gstrtypeid = 1
	And filestatus = 1

	Update TBL_RECVD_FILES
	Set filestatus = 2
	Where fileid = @FileId

	Create Table #TBL_CSV_GSTR1_RECS
	(
		fileid int,
		slno varchar(50),
		compcode varchar(50),
		unitcode varchar(50),
		doctype varchar(50),
		gstin varchar(50),
		fp varchar(50),
		ctin varchar(50),
		etin varchar(50),
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
		sply_ty varchar(50),
		exp_typ varchar(50),
		sbpcode varchar(50),
		sbnum varchar(50),
		sbdt varchar(50),
		ntty varchar(50),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(50),
		cdnur_typ varchar(50),
		gt varchar(50),
		cur_gt varchar(50),
		ad_recvd_amt varchar(50),
		ad_adj_amt varchar(50),
		nil_sply_ty varchar(50),
		nil_amt varchar(50),
		expt_amt varchar(50),
		ngsup_amt varchar(50),
		doc_nature varchar(255),
		from_serial_number varchar(50),
		to_serial_number varchar(50),
		totnum varchar(50),
		cancel varchar(50),
		net_issue varchar(50),
		omon varchar(50),
		oinum varchar(50),
		oidt varchar(50),
		ont_num varchar(50),
		ont_dt varchar(50),
		diff_percent varchar(50),
		typ varchar(50),
		doc_num varchar(50),
		errorcode smallint,
		errormessage varchar(255)
	 )

	 Create Table #TBL_EXT_GSTR1_HSN
	 (
	 	gstin varchar(50),
		fp varchar(50),
	 	gt decimal(18,2),
		cur_gt decimal(18,2),
		hsncode varchar(50),
		hsndesc varchar(50),
		uqc varchar(50),
		qty decimal(18,2),
		val decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	 )

	 Create Table #TBL_EXT_GSTR1_CDN_B2CS
	 (
	 	gstin varchar(50),
		fp varchar(50),
		pos varchar(2),
		sply_ty varchar(5),
		ntty varchar(1),
		rt decimal(18,2),
		txval decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	 )



	 Begin Try

		Insert Into #TBL_CSV_GSTR1_RECS
		Select t1.*
		From TBL_CSV_GSTR1_RECS t1 
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
	From #TBL_CSV_GSTR1_RECS

	if exists (Select 1 from #TBL_CSV_GSTR1_RECS)
	Begin

		Update #TBL_CSV_GSTR1_RECS -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_CSV_GSTR1_RECS -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #TBL_CSV_GSTR1_RECS
		Set Gt = t1.Gt, 
			Cur_Gt = t1.Cur_Gt
		From
		(Select Top(1) Slno,Gt, Cur_Gt From #TBL_CSV_GSTR1_RECS ) t1
		Where #TBL_CSV_GSTR1_RECS.Slno <> t1.Slno

		Update #TBL_CSV_GSTR1_RECS
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Ctin = Upper(Ltrim(Rtrim(IsNull(Ctin,'')))),
			Etin = Upper(Ltrim(Rtrim(IsNull(Etin,'')))),
			Ntty = Upper(Ltrim(Rtrim(IsNull(Ntty,''))))

		Update #TBL_CSV_GSTR1_RECS -- Introduced due to Excel Format Issue
		Set Omon = '0' + Ltrim(Rtrim(IsNull(Omon,'')))
		Where Len(Ltrim(Rtrim(IsNull(Omon,'')))) = 5

		Update #TBL_CSV_GSTR1_RECS
		Set Inum = Ltrim(Rtrim(IsNull(Inum,''))), 
			Nt_Num = Ltrim(Rtrim(IsNull(Nt_Num,''))),
			Oinum = Ltrim(Rtrim(IsNull(Oinum,''))),
			Ont_Num = Ltrim(Rtrim(IsNull(Ont_Num,'')))


		if @TemplateTypeId = 2
		Begin

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2B'
			Where DocType = 'INVOICE'
			And Ltrim(Rtrim(IsNull(Ctin,''))) <> '' 

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CL'
			Where DocType = 'INVOICE'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And dbo.udf_ValidateInvoiceValue(Val) = 1
			And (Convert(int,Substring(Gstin,1,2)) != Convert(int,Pos))
			And Convert(dec,Val) > 250000.00

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CS'
			Where DocType = 'INVOICE'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And dbo.udf_ValidateInvoiceValue(Val) = 1
			And (Convert(int,Substring(Gstin,1,2)) != Convert(int,Pos))
			And Convert(dec,Val) <= 250000.00

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CS'
			Where DocType = 'INVOICE'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And (Convert(int,Substring(Gstin,1,2)) = Convert(int,Pos))

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'EXP'
			Where DocType = 'EXPORT'
		
			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'CDNR'
			Where DocType = 'CREDIT/DEBIT'
			And dbo.udf_ValidateGstin(Ctin) = 1

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'CDNUR'
			Where DocType = 'CREDIT/DEBIT'
			And Ltrim(Rtrim(IsNull(Ctin,''))) = ''

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'TXP'
			Where DocType = 'ADVANCE ADJUSTED'
			
			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'AT'
			Where DocType = 'ADVANCE TAX'

			-- Amendments (Begin)

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2BA'
			Where DocType = 'INVOICE AMENDMENT'
			And Ltrim(Rtrim(IsNull(Ctin,''))) <> '' 

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CLA'
			Where DocType = 'INVOICE AMENDMENT'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And dbo.udf_ValidateInvoiceValue(Val) = 1
			And (Convert(int,Substring(Gstin,1,2)) != Convert(int,Pos))
			And Convert(dec,Val) > 250000.00

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CSA'
			Where DocType = 'INVOICE AMENDMENT'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And dbo.udf_ValidateInvoiceValue(Val) = 1
			And (Convert(int,Substring(Gstin,1,2)) != Convert(int,Pos))
			And Convert(dec,Val) <= 250000.00

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'B2CSA'
			Where DocType = 'INVOICE AMENDMENT'
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And (Convert(int,Substring(Gstin,1,2)) = Convert(int,Pos))

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'EXPA'
			Where DocType = 'EXPORT AMENDMENT'
		
			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'CDNRA'
			Where DocType = 'CREDIT/DEBIT AMENDMENT'
			And dbo.udf_ValidateGstin(Ctin) = 1

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'CDNURA'
			Where DocType = 'CREDIT/DEBIT AMENDMENT'
			And Ltrim(Rtrim(IsNull(Ctin,''))) = ''

			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'TXPA'
			Where DocType = 'ADVANCE ADJUSTED AMENDMENT'
			
			Update #TBL_CSV_GSTR1_RECS
			Set DocType = 'ATA'
			Where DocType = 'ADVANCE TAX AMENDMENT'

			-- Amendments (End) 
			
			Update #TBL_CSV_GSTR1_RECS
			Set Sply_Ty = 'INTRA'
			Where DocType In ('B2CS','TXP','AT','B2CSA','TXPA','ATA')
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And (Convert(int,Substring(Gstin,1,2)) = Convert(int,Pos))

			Update #TBL_CSV_GSTR1_RECS
			Set Sply_Ty = 'INTER'
			Where DocType In ('B2CS','TXP','AT','B2CSA','TXPA','ATA')
			And dbo.udf_ValidateGstin(Gstin) = 1
			And Ltrim(Rtrim(IsNull(Pos,''))) <> '0'
			And Ltrim(Rtrim(IsNull(Pos,''))) <> ''
			And dbo.udf_ValidatePlaceOfSupply(Pos) = 1
			And (Convert(int,Substring(Gstin,1,2)) != Convert(int,Pos))

		End

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Doc Type'
		Where dbo.udf_ValidateDocType(DocType) <> 1

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -99,
			ErrorMessage = dbo.udf_ValidateNumericData(Gt,Cur_Gt,Val,Rt,Txval,Iamt,CAmt,Samt,Csamt,Qty,Unitprice,Totval,Ad_Adj_Amt,Ad_Recvd_Amt,Nil_Amt,Expt_Amt,Ngsup_Amt) 
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -99
		And IsNull(ErrorMessage,'') = ''


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId and rowstatus =1)
						And t1.GstinNo = Gstin
						And t1.rowstatus = 1) 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Ctin'
		Where (DocType In ('B2B','CDNR','B2BA','CDNRA') and
			   dbo.udf_ValidateGstin(Ctin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = null,
			ErrorMessage = null
		Where (DocType In ('B2B','CDNR','B2BA','CDNRA') and
			   dbo.udf_ValidateUin(Ctin) = 1)
		And IsNull(ErrorCode,0) = -4

		if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And CtinValdnCustMgmtReqd = 1)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -4,
				ErrorMessage = 'Ctin is not available in Customer Management.'
			From  #TBL_CSV_GSTR1_RECS t1 
			Where  DocType In ('B2B','CDNR','B2BA','CDNRA') 
			And IsNull(ErrorCode,0) = 0
			And Not Exists(Select 1 From Tbl_Buyer t2
								Where t2.GstinNo = t1.Ctin
								And t2.CustomerId = @CustId
								And t2.rowstatus = 1) 
		End

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Ecommerce Gstin'
		Where (DocType In ('B2B','B2CL','B2CS','B2BA','B2CLA','B2CSA') and
			   Ltrim(Rtrim(IsNull(Etin,''))) <> '' and
			   dbo.udf_ValidateGstin(Etin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice No'
		Where ( DocType In ('B2B','B2CL','EXP','CDNR','CDNUR',
							'B2BA','B2CLA','EXPA','CDNRA','CDNURA'
						   ) and
				dbo.udf_ValidateInvoiceNo(Inum) <> 1)
		And IsNull(ErrorCode,0) = 0

		if Exists (Select 1 From TBL_Cust_Settings 
			Where CustId = @CustId
			And CdnValdnOrigInum = 1)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -6,
				ErrorMessage = 'Invoice No does not exist.'
			Where ( DocType In ('CDNR','CDNUR') and
					dbo.udf_ValidateCDNForOrigInvoice(DocType,@TemplateTypeId,Gstin,Inum,Idt) <> 1)
			And IsNull(ErrorCode,0) = 0
		End

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('B2B','B2CL','EXP',
							'B2BA','B2CLA','EXPA'
						   ) and
				dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Invoice Type'
		Where (	DocType In ('B2B','B2BA') and
				dbo.udf_ValidateInvoiceType(Inv_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0


		if @TemplateTypeId = 1
		Begin

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -9,
				ErrorMessage = 'HSN Code is mandatory'
			Where DocType In ('HSN')
			And Ltrim(Rtrim(IsNull(Hsncode,''))) = '' 
			And IsNull(ErrorCode,0) = 0  

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -9,
				ErrorMessage = 'Invalid HSN Code'
			Where ( DocType In ('HSN') and
					dbo.udf_ValidateHsnCode(Hsncode) <> 1)
			And IsNull(ErrorCode,0) = 0

			if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And HsnValdnHsnMstrReqd = 1)
			Begin
				Update #TBL_CSV_GSTR1_RECS 
				Set ErrorCode = -9,
					ErrorMessage = 'HSN Code is not available in HSN Master List.'
				From  #TBL_CSV_GSTR1_RECS t1 
				Where  DocType In ('HSN') 
				And IsNull(ErrorCode,0) = 0
				And Not Exists(Select 1 From TBL_HSN_MASTER t2
									Where t2.hsnCode = t1.Hsncode
									And t2.hsntype = 'OUTWARD'
									And t2.CustomerId = @CustId
									And t2.rowstatus = 1) 
			End

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -10,
				ErrorMessage = 'Item Description is mandatory'
			Where DocType In ('HSN')
			And Ltrim(Rtrim(IsNull(Hsndesc,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -10,
				ErrorMessage = 'Invalid Item Description'
			Where ( DocType In ('HSN') and
					dbo.udf_ValidateHsnDesc(Hsndesc) <> 1)
			And IsNull(ErrorCode,0) = 0

			
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -12,
				ErrorMessage = 'UQC is mandatory'
			Where DocType In ('HSN')
			And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
			And IsNull(ErrorCode,0) = 0


		End
		else if @TemplateTypeId = 2
		Begin

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -9,
				ErrorMessage = 'HSN Code is mandatory'
			Where DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR')
			And Ltrim(Rtrim(IsNull(Hsncode,''))) = '' 
			And IsNull(ErrorCode,0) = 0  

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -9,
				ErrorMessage = 'Invalid HSN Code'
			Where ( DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR') and
					dbo.udf_ValidateHsnCode(Hsncode) <> 1)
			And IsNull(ErrorCode,0) = 0

			if Exists (Select 1 From TBL_Cust_Settings 
				Where CustId = @CustId
				And HsnValdnHsnMstrReqd = 1)
			Begin
				Update #TBL_CSV_GSTR1_RECS 
				Set ErrorCode = -9,
					ErrorMessage = 'HSN Code is not available in HSN Master List.'
				From  #TBL_CSV_GSTR1_RECS t1 
				Where  DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR') 
				And IsNull(ErrorCode,0) = 0
				And Not Exists(Select 1 From TBL_HSN_MASTER t2
									Where t2.hsnCode = t1.Hsncode
									And t2.hsntype = 'OUTWARD'
									And t2.CustomerId = @CustId
									And t2.rowstatus = 1) 
			End

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -10,
				ErrorMessage = 'Item Description is mandatory'
			Where DocType In ('B2B','B2CL','B2CS','EXP''CDNR','CDNUR')
			And Ltrim(Rtrim(IsNull(Hsndesc,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -10,
				ErrorMessage = 'Invalid Item Description'
			Where ( DocType In ('B2B','B2CL','B2CS','EXP''CDNR','CDNUR') and
					dbo.udf_ValidateHsnDesc(Hsndesc) <> 1)
			And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -12,
				ErrorMessage = 'UQC is mandatory'
			Where DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR')
			And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
			And IsNull(ErrorCode,0) = 0

		End

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Quantity'
		Where (DocType In ('HSN') and
			   dbo.udf_ValidateQuantity(Qty) <> 1)
		And IsNull(ErrorCode,0) = 0 
	
		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -14,
			ErrorMessage = 'Invalid Rate'
		Where (DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR',
							'TXP','AT',
							'B2BA','B2CLA','B2CSA','EXPA','CDNRA','CDNURA',
							'TXPA','ATA') and
			   dbo.udf_ValidateRate(Rt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2B','B2CL','EXP','CDNR','CDNUR') and
			   dbo.udf_ValidateTaxableValue(Txval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2BA','B2CLA','EXPA','CDNRA','CDNURA') and
				(Ltrim(Rtrim(IsNull(Txval,''))) = ''
					Or IsNumeric(Txval) <> 1
					Or Convert(dec,Txval) < 0
				)
			   )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('HSN','B2CS','B2CSA') and
				(Ltrim(Rtrim(IsNull(Txval,''))) = ''
					Or IsNumeric(Txval) <> 1
				)
			   )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -16,
			ErrorMessage = 'Place of Supply is mandatory'
		Where DocType In ('B2B','B2CL','B2CS','TXP','AT',
						  'B2BA','B2CLA','B2CSA','TXPA','ATA')
		And Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -17,
			ErrorMessage = 'Invalid Place Of Supply'
		Where ( DocType In ('B2B','B2CL','B2CS','TXP','AT',
							'B2BA','B2CLA','B2CSA','TXPA','ATA') and
				dbo.udf_ValidatePlaceOfSupply(Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -18,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2B','B2CL','EXP','CDNR','CDNUR') and
			   dbo.udf_ValidateInvoiceValue(Val) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -18,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2BA','B2CLA','EXPA','CDNRA','CDNURA') and
				(Ltrim(Rtrim(IsNull(Val,''))) = ''
					Or IsNumeric(Val) <> 1
					Or Convert(dec,Val) < 0
				)
			   )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -19,
			ErrorMessage = 'Invoice Value must be greater than 2.5 Lakh'
		Where DocType In ('B2CL','B2CLA')
		And (IsNumeric(Val) <> 1 Or Convert(dec,Val) <= 250000.00)
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -19,
			ErrorMessage = 'Invoice Value must be less than 2.5 Lakh for Inter State Supply'
		Where DocType = 'B2CS'
		And (IsNumeric(Val) <> 1 Or Convert(dec,Val) >= 250000.00)
		And (Substring(Gstin,1,2) <> Pos)
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -20,
			ErrorMessage = 'Invalid Advance Adjusted Amount'
		Where (	DocType In ('TXP','TXPA') and
				dbo.udf_ValidateAmount(Ad_Adj_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -21,
			ErrorMessage = 'Invalid Advance Received Amount'
		Where (	DocType In ('AT','ATA') and
				dbo.udf_ValidateAmount(Ad_Recvd_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -22,
			ErrorMessage = 'Invalid Reverse Charge'
		Where ( DocType In ('B2B','B2BA') and
				dbo.udf_ValidateReverseCharge(Rchrg) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -23,
			ErrorMessage = 'Invalid Supply Type'
		Where ( DocType In ('B2CS','TXP','AT','B2CSA','TXPA','ATA') and
				dbo.udf_ValidateSupplyType(Sply_Ty,Gstin,Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -24,
			ErrorMessage = 'Invalid Export Type'
		Where ( DocType In ('EXP','EXPA') and
				dbo.udf_ValidateExportType(Exp_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS  
		Set ErrorCode = -25,
			ErrorMessage = 'Invalid Shipping Bill Port Code'
		Where (	DocType In ('EXP','EXPA') 
				And Ltrim(Rtrim(IsNull(sbpcode,''))) <> '' 
				And dbo.udf_ValidatePortCode(sbpcode) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS  
		Set ErrorCode = -26,
			ErrorMessage = 'Invalid Shipping Bill Number'
		Where (	DocType In ('EXP','EXPA') 
				And Ltrim(Rtrim(IsNull(sbnum,''))) <> '' 
				And dbo.udf_ValidateBOENo(sbnum) <> 1 )
		And IsNull(ErrorCode,0) = 0


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -27,
			ErrorMessage = 'Invalid Note Type'
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') and
				dbo.udf_ValidateNoteType(Ntty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -28,
			ErrorMessage = 'Invalid Note No'
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') and
				dbo.udf_ValidateNoteNo(Nt_Num) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -29,
			ErrorMessage = 'Invalid Note Date'
		Where (	DocType In ('CDNR','CDNUR','CDNRA','CDNURA') and
				dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Idt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -30,
			ErrorMessage = 'Invalid Pre GST Regime'
		Where (	DocType In ('CDNR','CDNUR','CDNRA','CDNURA') and
				dbo.udf_ValidatePreGstRegime(P_Gst) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -31,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('CDNR','CDNUR','CDNRA','CDNURA') 
				And Ltrim(Rtrim(IsNull(P_Gst,''))) = 'N' 
				And dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -32,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('CDNR','CDNUR','CDNRA','CDNURA') 
				And Ltrim(Rtrim(IsNull(P_Gst,''))) = 'Y' 
				And dbo.udf_ValidateDate(Idt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -33,
			ErrorMessage = 'Invalid Nil Supply Type'
		Where (	DocType In ('NIL') and
				dbo.udf_ValidateNilSupplyType(Nil_Sply_Ty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -34,
			ErrorMessage = 'Invalid Total Nil rated outward supplies'
		Where (	DocType In ('NIL') and
				Ltrim(Rtrim(IsNull(Nil_Amt,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Nil_Amt) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -35,
			ErrorMessage = 'Invalid Total Exempted outward supplies'
		Where (	DocType In ('NIL') and
				Ltrim(Rtrim(IsNull(Expt_Amt,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Expt_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -36,
			ErrorMessage = 'Invalid Total Non GST outward supplies'
		Where (	DocType In ('NIL') and
				Ltrim(Rtrim(IsNull(Ngsup_Amt,''))) <> ''  and 
				dbo.udf_ValidateNilAmount(Ngsup_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -37,
			ErrorMessage = dbo.udf_ValidateGstAmount(DocType,Gstin,Ctin,Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,Ad_Adj_Amt,Ad_Recvd_Amt,Inv_Typ,Inum,Idt,P_Gst) 
		Where DocType In ('B2B','B2CL','B2CS','CDNR',
							'TXP','AT',
						  'B2BA','B2CLA','B2CSA','CDNRA',
							'TXPA','ATA'	) 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -37,
			ErrorMessage = dbo.udf_ValidateGstAmount(DocType,Gstin,Ctin,Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,Ad_Adj_Amt,Ad_Recvd_Amt,Inv_Typ,Inum,Idt,P_Gst) 
		Where DocType In ('EXP','EXPA')
		And IsNull(Exp_Typ,'') = 'WPAY' 
		And IsNull(ErrorCode,0) = 0

		if @TemplateTypeId = 1
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -37,
				ErrorMessage = dbo.udf_ValidateGstAmount(DocType,Gstin,Ctin,Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,Ad_Adj_Amt,Ad_Recvd_Amt,Inv_Typ,Inum,Idt,P_Gst) 
			Where DocType In ('CDNUR','CDNURA')
			And IsNull(ErrorCode,0) = 0
		End

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -37
		And IsNull(ErrorMessage,'') = ''

		Update #TBL_CSV_GSTR1_RECS 
		Set typ = 'OE'
		Where (DocType In ('B2CS','B2CSA'))
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set typ = 'E'
		Where (DocType In ('B2CS','B2CSA') and
				IsNull(Etin,'') <> '' )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set Ntty =
		Case Ntty
			When 'CREDIT' Then 'C'
			When 'DEBIT'  Then 'D'
			When 'REFUND' Then 'R'
			Else Ntty
		End
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
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
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -38,
			ErrorMessage = 'Invalid Reason for Issuing Dr./ Cr. Notes'
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') and
				dbo.udf_ValidateNoteRsn(rsn) <> 1) 
		And IsNull(ErrorCode,0) = 0


		Update #TBL_CSV_GSTR1_RECS 
		Set doc_num =
		Case doc_nature
			When '1.Invoices for outward supply' Then '1'
			When '2.Invoices for inward supply from unregistered person'  Then '2'
			When '3.Revised Invoice' Then '3'
			When '4.Debit Note' Then '4'
			When '5.Credit Note' Then '5'
			When '6.Receipt voucher' Then '6'
			When '7.Payment Voucher' Then '7'
			When '8.Refund voucher' Then '8'
			When '9.Delivery Challan for job work' Then '9'
			When '10.Delivery Challan for supply on approval' Then '10'
			When '11.Delivery Challan in case of liquid gas' Then '11'
			When '12.Delivery Challan in cases other than by way of supply (excluding at S.no. 9 to 11)' Then '12'
			Else '0'
		End
		Where (DocType = 'DOCISSUE')
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -39,
			ErrorMessage = 'Invalid Nature of Document'
		Where (DocType = 'DOCISSUE') 
		And Ltrim(Rtrim(IsNull(doc_num,''))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -40,
			ErrorMessage = 'Invalid From Serial Number'
		Where ( DocType In ('DOCISSUE') and
				dbo.udf_ValidateSerialNo(from_serial_number) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -41,
			ErrorMessage = 'Invalid To Serial Number'
		Where ( DocType In ('DOCISSUE') and
				dbo.udf_ValidateSerialNo(to_serial_number) <> 1)
		And IsNull(ErrorCode,0) = 0


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -42,
			ErrorMessage = 'Invalid Shipping Bill Date'
		Where (	DocType In ('EXP','EXPA') and
				Ltrim(Rtrim(IsNull(Sbdt,''))) <> ''  and 
				dbo.udf_ValidateDate(Sbdt) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -43,
			ErrorMessage = 'Invalid Original Invoice No.'
		Where DocType In ('B2BA','B2CLA','EXPA') 
		And	dbo.udf_ValidateInvoiceNo(Oinum) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -44,
			ErrorMessage = 'Invalid Original Inv Date'
		Where DocType In ('B2BA','B2CLA','EXPA')
		And	dbo.udf_ValidateDate(Oidt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -45,
			ErrorMessage = 'Invalid Original Note No.'
		Where DocType In ('CDNRA','CDNURA') 
		And dbo.udf_ValidateNoteNo(Ont_Num) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -46,
			ErrorMessage = 'Invalid Original Note Date'
		Where DocType In ('CDNRA','CDNURA')
		And	dbo.udf_ValidateDate(Ont_Dt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -47,
			ErrorMessage = 'Invalid Original Period'
		Where DocType In ('B2CSA','TXPA','ATA') 
		And dbo.udf_ValidatePeriod(Omon) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set Idt = convert(varchar,(SELECT convert(datetime, idt, 103)),105)
		Where ( DocType In ('B2B','B2CL','EXP','CDNR','CDNUR','B2BA','B2CLA','EXPA','CDNRA','CDNURA') )
		And Ltrim(Rtrim(IsNull(Idt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set Nt_dt = convert(varchar,(SELECT convert(datetime, nt_dt, 103)),105)
		Where ( DocType In ('CDNR','CDNUR','CDNRA','CDNURA') )
		And Ltrim(Rtrim(IsNull(Nt_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR1_RECS 
		Set Sbdt = convert(varchar,(SELECT convert(datetime, sbdt, 103)),105)
		Where ( DocType In ('EXP','EXPA') )
		And Ltrim(Rtrim(IsNull(Sbdt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with different invoice dates for line items

		Select t.doctype,t.gstin,t.fp,t.inum,count(*) as cnt
		Into #TmpInvoices_Dt
		From
		(
			Select distinct doctype,gstin,fp,inum,idt
			From #TBL_CSV_GSTR1_RECS
			Where ( DocType in ('B2B','B2CL','B2BA','B2CLA') Or
					(@TemplateTypeId = 2 And DocType = 'B2CS')
				  )
		 ) t
		group by doctype,gstin,fp,inum
		having count(*) > 1

		If Exists(Select 1 From #TmpInvoices_Dt)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -48,
				ErrorMessage = 'Invoice Date is different for line items.'
			From #TBL_CSV_GSTR1_RECS t1,
				 #TmpInvoices_Dt t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.inum = t2.inum
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices with different invoice values for line items

		Select t.doctype,t.gstin,t.fp,t.inum,count(*) as cnt
		Into #TmpInvoices_Val
		From
		(
			Select distinct doctype,gstin,fp,inum,val
			From #TBL_CSV_GSTR1_RECS
			Where ( DocType in ('B2B','B2CL','B2BA','B2CLA') Or
					(@TemplateTypeId = 2 And DocType = 'B2CS')
				  )
		 ) t
		group by doctype,gstin,fp,inum
		having count(*) > 1

		If Exists(Select 1 From #TmpInvoices_Val)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -49,
				ErrorMessage = 'Invoice Value is different for line items.'
			From #TBL_CSV_GSTR1_RECS t1,
				 #TmpInvoices_Val t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.inum = t2.inum
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking CDN with different note dates for line items

		Select t.doctype,t.gstin,t.fp,t.nt_num,count(*) as cnt
		Into #TmpCdn_Dt
		From
		(
			Select distinct doctype,gstin,fp,nt_num,nt_dt
			From #TBL_CSV_GSTR1_RECS
			Where DocType in ('CDNR','CDNRA')
			Or (DocType in ('CDNUR','CDNURA') And Cdnur_Typ <> 'B2CS')
		 ) t
		group by doctype,gstin,fp,nt_num
		having count(*) > 1

		If Exists(Select 1 From #TmpCdn_Dt)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -50,
				ErrorMessage = 'Note Date is different for line items.'
			From #TBL_CSV_GSTR1_RECS t1,
				 #TmpCdn_Dt t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.nt_num = t2.nt_num
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking CDN with different note values for line items

		Select t.doctype,t.gstin,t.fp,t.nt_num,count(*) as cnt
		Into #TmpCdn_Val
		From
		(
			Select distinct doctype,gstin,fp,nt_num,val
			From #TBL_CSV_GSTR1_RECS
			Where DocType in ('CDNR','CDNRA')
			Or (DocType in ('CDNUR','CDNURA') And Cdnur_Typ <> 'B2CS')
		 ) t
		group by doctype,gstin,fp,nt_num
		having count(*) > 1

		If Exists(Select 1 From #TmpCdn_Val)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -51,
				ErrorMessage = 'Note Value is different for line items.'
			From #TBL_CSV_GSTR1_RECS t1,
				  #TmpCdn_Val t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.nt_num = t2.nt_num
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking CDN with different Invoices

		Select t.doctype,t.gstin,t.fp,t.nt_num,count(*) as cnt
		Into #TmpCdn_Inv
		From
		(
			Select distinct doctype,gstin,fp,nt_num,inum
			From #TBL_CSV_GSTR1_RECS
			Where DocType in ('CDNR','CDNRA')
			Or (DocType in ('CDNUR','CDNURA') And Cdnur_Typ <> 'B2CS')
		 ) t
		group by doctype,gstin,fp,nt_num
		having count(*) > 1

		If Exists(Select 1 From #TmpCdn_Inv)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -52,
				ErrorMessage = 'Note Number is same for different invoices.'
			From #TBL_CSV_GSTR1_RECS t1,
				  #TmpCdn_Inv t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.nt_num = t2.nt_num
			And IsNull(ErrorCode,0) = 0
		End


		-- Tracking Invoices with item errors

		Select distinct doctype,gstin,fp,inum
			Into #TmpRecords
		From #TBL_CSV_GSTR1_RECS  
		Where ( DocType in ('B2B','B2CL','EXP','CDNR','CDNUR',
							'B2BA','B2CLA','EXPA','CDNRA','CDNURA') Or
					(@TemplateTypeId = 2 And DocType = 'B2CS')
			  )
		And IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -53,
				ErrorMessage = 'Error in Invoice Item.'
			From #TBL_CSV_GSTR1_RECS t1,
				 #TmpRecords t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And t1.inum = t2.inum
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2B_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2B_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2CL' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CL_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2CL' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CL_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		if @TemplateTypeId = 2
		Begin

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -100,
				ErrorMessage = 'Invoice data already uploaded'
			From #TBL_CSV_GSTR1_RECS t1
			Where DocType = 'B2CS' 
			And IsNull(ErrorCode,0) = 0
			And Exists(Select 1 From  TBL_EXT_GSTR1_B2CS_INV 
									Where gstin = t1.gstin 
									And fp = t1.fp 
									And inum = t1.inum 
									And idt = t1.idt
									And rowstatus = 0)

			Update #TBL_CSV_GSTR1_RECS 
			Set ErrorCode = -101,
				ErrorMessage = 'Invoice data already imported'
			From #TBL_CSV_GSTR1_RECS t1
			Where DocType = 'B2CS' 
			And IsNull(ErrorCode,0) = 0
			And Exists(Select 1 From  TBL_EXT_GSTR1_B2CS_INV 
									Where gstin = t1.gstin 
									And fp = t1.fp 
									And inum = t1.inum 
									And idt = t1.idt
									And rowstatus <> 0)

		End


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'EXP' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'EXP' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'HSN data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'HSN' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_HSN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And hsn_sc = t1.hsncode 
								And descs = t1.hsndesc
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'HSN data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'HSN' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From TBL_EXT_GSTR1_HSN  
								Where gstin = t1.gstin 
								And fp = t1.fp
								And hsn_sc = t1.hsncode 
								And descs = t1.hsndesc
								And rowstatus <> 0)


		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2BA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2BA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2BA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2BA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2CLA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CLA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'B2CLA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CLA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'EXPA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXPA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'EXPA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXPA_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNRA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNRA 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNRA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNRA 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNURA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNURA 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR1_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR1_RECS t1
		Where DocType = 'CDNURA' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNURA 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)


		-- Import B2B Invoices

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2B')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'B2B'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2B_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2B'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_B2B_INV
				(	gstin, fp, gt, cur_gt, ctin,inum, idt, val, pos, rchrg, etin, inv_typ, 
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
				Select 
					gstin, fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					ctin,inum, idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					pos, rchrg,etin, inv_typ, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'B2B' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
		End

		-- Import B2CL Invoices
		
		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2CL')
		Begin
			
			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'B2CL'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2CL_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2CL'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CL' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_B2CL_INV
				(	gstin, fp, gt, cur_gt, pos,inum, idt, val, etin,  
					rt, txval, iamt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
				Select 
					gstin, fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos,inum,idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					etin,  
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'B2CL' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CL' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End

		-- Import B2CS Invoices

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2CS')
		Begin

			Begin Try

				if @TemplateTypeId = 2
				Begin

					if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							Where DocType = 'B2CS'
							And IsNull(ErrorCode,0) = -101)
					Begin

						Delete t1
						From	TBL_EXT_GSTR1_B2CS_INV t1,
								#TBL_CSV_GSTR1_RECS t2
						Where t1.gstin = t2.gstin
						And t1.fp = t2.fp
						And	t1.inum = t2.inum
						And t1.idt = t2.idt
						And DocType = 'B2CS'
						And IsNull(ErrorCode,0) = -101
					
						Update #TBL_CSV_GSTR1_RECS 
						Set ErrorCode = Null,
							ErrorMessage = Null
						From #TBL_CSV_GSTR1_RECS t1
						Where DocType = 'B2CS' 
						And IsNull(ErrorCode,0) = -101	

					End


					Insert TBL_EXT_GSTR1_B2CS_INV
					(	gstin, fp, gt, cur_gt, 
						sply_typ,typ,pos,
						inum, idt, val, etin,  
						rt, txval, iamt,camt,samt, csamt,
						rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
						hsncode,hsndesc,uqc,qty,unitprice)
					Select 
						gstin, fp,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						sply_ty,typ,pos,
						inum,idt,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
						etin,  
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
						1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
						hsncode,hsndesc,uqc,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CS' 
					And IsNull(ErrorCode,0) = 0

				End
				else
				Begin

					Insert TBL_EXT_GSTR1_B2CS
					(	gstin, fp, gt, cur_gt, sply_ty,txval,typ,etin, pos,  
						rt, iamt,camt,samt,csamt,
						rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
						hsncode,hsndesc,uqc,qty,unitprice)
					Select 
						gstin, fp, 
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						sply_ty,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						typ,etin,pos,  
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
						1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
						hsncode,hsndesc,uqc,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
					 From #TBL_CSV_GSTR1_RECS t1
					 Where DocType = 'B2CS' 
					 And IsNull(ErrorCode,0) = 0

				End

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CS' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End
	
		-- Import Exports

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'EXP')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'EXP'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_EXP_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'EXP'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'EXP' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_EXP_INV
				(	gstin, fp, gt, cur_gt,exp_typ,inum, idt, val,
					sbpcode,sbnum,sbdt,  
					txval,rt,iamt,csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice
				)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					exp_typ,inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					sbpcode,sbnum,sbdt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
				    Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'EXP' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'EXP' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End
		
		-- Import CDNR

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'CDNR')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'CDNR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNR t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_CDNR
				(	gstin, fp, gt, cur_gt, ctin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					ctin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@Referenceno,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'CDNR'
				And IsNull(ErrorCode,0) = 0
				 
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import CDNUR

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'CDNUR')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'CDNUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNUR t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				if @TemplateTypeId = 2
				Begin

					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'EXPWP'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR'
					And Exists (Select 1 From TBL_EXT_GSTR1_EXP_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.exp_typ = 'WPAY' 
						 And t2.rowstatus <> 2)
					And IsNull(ErrorCode,0) = 0
	
					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'EXPWOP'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR'
					And Exists (Select 1 From TBL_EXT_GSTR1_EXP_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.exp_typ = 'WOPAY'
						 And t2.rowstatus <> 2 )
					And IsNull(ErrorCode,0) = 0

					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'B2CL'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR'
					And Exists (Select 1 From TBL_EXT_GSTR1_B2CL_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.rowstatus <> 2)
					And IsNull(ErrorCode,0) = 0

					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'B2CS'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR'
					And Exists (Select 1 From TBL_EXT_GSTR1_B2CS_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.rowstatus <> 2)
					And IsNull(ErrorCode,0) = 0

					Update #TBL_CSV_GSTR1_RECS 
					Set Cdnur_Typ = 'B2CS'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(Cdnur_Typ,'') = ''
					And IsNull(ErrorCode,0) = 0	

					/*
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -43,
						ErrorMessage = 'Invoice No does not exist'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(Cdnur_Typ,'') = ''
					And IsNull(ErrorCode,0) = 0	

					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -44,
						ErrorMessage = 'No B2CS Invoices exist for the current period'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(Cdnur_Typ,'') = 'B2CS'
					And Not Exists(Select 1 From TBL_EXT_GSTR1_B2CS_INV t2 
									Where t2.gstin = t1.gstin
									And t2.fp = t1.fp
									And t2.rowstatus <> 2) 
					And IsNull(ErrorCode,0) = 0
					*/	

					--Update #TBL_CSV_GSTR1_RECS 
					--Set ErrorCode = -45,
					--	ErrorMessage = ''
					--From #TBL_CSV_GSTR1_RECS t1
					--Where DocType = 'CDNUR' 
					--And IsNull(Cdnur_Typ,'') = 'B2CS'
					--And dbo.udf_ValidateCDNForB2CSInvoice(Gstin,Fp,Inum,Idt,Txval) = 1
					--And IsNull(ErrorCode,0) = 0	


				End


				Insert TBL_EXT_GSTR1_CDNUR
				(	gstin, fp, gt, cur_gt, typ,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice,
					pos,sply_ty)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					cdnur_typ,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					Case When @TemplateTypeId = 2 And IsNull(Cdnur_Typ,'') = 'B2CS' Then 0 Else 1 End,
					@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0)))),
					pos,
					Case pos When convert(varchar(2),substring(gstin,1,2)) Then 'INTRA' Else 'INTER' END
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'CDNUR' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		if (@TemplateTypeId = 2  
			And Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'CDNUR'
							And Cdnur_Typ = 'B2CS' )
			)
		Begin

			Insert #TBL_EXT_GSTR1_CDN_B2CS
				(	gstin, fp, pos, sply_ty, ntty, rt,  
					txval, iamt, camt, samt, csamt
				)
			Select	gstin,fp,
					pos,
					Case pos When convert(varchar(2),substring(gstin,1,2)) Then 'INTRA' Else 'INTER' END,
					ntty,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
			FROM #TBL_CSV_GSTR1_RECS
			Where DocType In ('CDNUR')
			And Cdnur_Typ = 'B2CS'
			And Ntty = 'C'
			And IsNull(ErrorCode,0) = 0

			Insert #TBL_EXT_GSTR1_CDN_B2CS
				(	gstin, fp, pos, sply_ty, ntty, rt,  
					txval, iamt, camt, samt, csamt
				)
			Select	gstin,fp,
					pos,
					Case pos When convert(varchar(2),substring(gstin,1,2)) Then 'INTRA' Else 'INTER' END,
					ntty,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
			FROM #TBL_CSV_GSTR1_RECS
			Where DocType In ('CDNUR')
			And Cdnur_Typ = 'B2CS'
			And Ntty = 'D'
			And IsNull(ErrorCode,0) = 0

			if Exists(Select 1 From #TBL_EXT_GSTR1_CDN_B2CS)
			Begin

				Insert Into TBL_EXT_GSTR1_CDN_B2CS
				Select @FileName,t1.*
				From #TBL_EXT_GSTR1_CDN_B2CS t1

				Update TBL_GSTR1_B2CS 
				Set 
					txval = IsNull(t3.txval,0) + IsNull(t1.txval,0),
					iamt =  IsNull(t3.iamt,0) + IsNull(t1.iamt,0), 
					camt =  IsNull(t3.camt,0) + IsNull(t1.camt,0), 
					samt =  IsNull(t3.samt,0) + IsNull(t1.samt,0), 
					csamt =  IsNull(t3.csamt,0) + IsNull(t1.csamt,0) 
				From #TBL_EXT_GSTR1_CDN_B2CS t1,
					 TBL_GSTR1 t2,
					 TBL_GSTR1_B2CS  t3
				Where t2.gstin = t1.gstin
				And t2.fp = t1.fp
				And t3.gstr1id = t2.gstr1id
				And t3.rt = t1.rt
				And t3.pos = t1.pos
				And t3.sply_ty = t1.sply_ty
				And IsNull(t3.flag,'') = ''
			
			End
		
		End

		-- Import HSN

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'HSN')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'HSN'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From TBL_EXT_GSTR1_HSN t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.hsn_sc = t2.hsncode
					And t1.descs = t2.hsndesc
					And DocType = 'HSN'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'HSN' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt, hsn_sc, descs, uqc, qty, val,  
					txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'HSN' 
				And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in HSN Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'HSN' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		if @TemplateTypeId = 2
		Begin

			Begin Try

				Insert #TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt,hsncode,hsndesc, uqc, qty, val,  
					txval, iamt, camt, samt, csamt
				)
				Select	gstin,fp,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						hsncode,hsndesc,uqc,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
				FROM #TBL_CSV_GSTR1_RECS
				Where DocType In ('B2B','B2CL','B2CS','EXP')
				And IsNull(ErrorCode,0) = 0

				Insert #TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt,hsncode,hsndesc, uqc, qty, val,  
					txval, iamt, camt, samt, csamt
				)
				Select	gstin,fp,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						hsncode,hsndesc,uqc,
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
				FROM #TBL_CSV_GSTR1_RECS
				Where DocType In ('CDNR')
				And Ntty = 'C'
				And IsNull(ErrorCode,0) = 0

				Insert #TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt,hsncode,hsndesc, uqc, qty, val,  
					txval, iamt, camt, samt, csamt
				)
				Select	gstin,fp,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						hsncode,hsndesc,uqc,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
				FROM #TBL_CSV_GSTR1_RECS
				Where DocType In ('CDNR')
				And Ntty = 'D'
				And IsNull(ErrorCode,0) = 0

				Insert #TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt,hsncode,hsndesc, uqc, qty, val,  
					txval, iamt, camt, samt, csamt
				)
				Select	gstin,fp,
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
						Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
						hsncode,hsndesc,uqc,
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
						-Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
				FROM #TBL_CSV_GSTR1_RECS
				Where DocType In ('CDNUR')
				And Ntty = 'C'
				And IsNull(ErrorCode,0) = 0

				Insert #TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt,hsncode,hsndesc, uqc, qty, val,  
					txval, iamt, camt, samt, csamt
				)
				Select	gstin,fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0))))
				FROM #TBL_CSV_GSTR1_RECS
				Where DocType In ('CDNUR')
				And Ntty = 'D'
				And IsNull(ErrorCode,0) = 0

				Insert TBL_EXT_GSTR1_HSN
				(	gstin, fp, gt, cur_gt, hsn_sc, descs, uqc, qty, val,  
					txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				SELECT  gstin,fp,gt,cur_gt,
					hsncode,hsndesc,uqc,sum(qty), sum(val),  
					sum(txval), sum(iamt), sum(camt), sum(samt), sum(csamt),
					1 ,@SourceType ,@ReferenceNo,GetDate(),'',@FileId
				FROM #TBL_EXT_GSTR1_HSN
				Group By gstin,fp,gt,cur_gt,hsncode,hsndesc,uqc

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in HSN Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType In ('B2B','B2CL','B2CS','EXP')
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch


		End

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'TXP')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_TXP
				(	gstin, fp, gt, cur_gt, pos, sply_ty,   
					rt, ad_amt,iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos, sply_ty, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_adj_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'TXP' 
				And IsNull(ErrorCode,0) = 0
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'TXP' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'NIL')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_NIL
				(	gstin, fp, gt, cur_gt, nil_amt, expt_amt, ngsup_amt, sply_ty,   
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(nil_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(expt_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ngsup_amt,''),0)))),
					Nil_Sply_Ty, 
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'NIL' 
				And IsNull(ErrorCode,0) = 0
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'NIL' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'AT')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_AT
				(	gstin, fp, gt, cur_gt, pos, sply_ty,   
					rt, ad_amt,iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos, sply_ty, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_recvd_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'AT' 
				And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'AT' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'DOCISSUE')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_DOC
				(	gstin, fp, gt, cur_gt, doc_num, doc_typ, num, 
					froms, tos, totnum, cancel, net_issue,  
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					Convert(int,Ltrim(Rtrim(IsNull(NullIf(doc_num,''),0)))),
					null, null, 
					from_serial_number, to_serial_number,
					Convert(int,Ltrim(Rtrim(IsNull(NullIf(totnum,''),0)))),
					Convert(int,Ltrim(Rtrim(IsNull(NullIf(cancel,''),0)))),
					Convert(int,Ltrim(Rtrim(IsNull(NullIf(net_issue,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'DOCISSUE' 
				And IsNull(ErrorCode,0) = 0
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'DOCISSUE' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import Amendments - B2BA 

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2BA')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'B2BA'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2BA_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2BA'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2BA' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_B2BA_INV
				(	gstin, fp, gt, cur_gt, ctin,inum, idt, val, pos, rchrg, etin, inv_typ, 
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice,
					oinum,oidt,diff_percent)
				Select 
					gstin, fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					ctin,inum, idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					pos, rchrg,etin, inv_typ, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0)))),
					oinum,oidt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'B2BA' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2BA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
		End

		-- Import Amendments - B2CLA
		
		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2CLA')
		Begin
			
			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'B2CLA'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2CLA_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2CLA'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CLA' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_B2CLA_INV
				(	gstin, fp, gt, cur_gt, pos,inum, idt, val, etin,  
					rt, txval, iamt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice,
					oinum,oidt,diff_percent)
				Select 
					gstin, fp,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos,inum,idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					etin,  
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0)))),
					oinum,oidt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'B2CLA' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CLA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End

		-- Import Amendments - B2CSA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'B2CSA')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_B2CSA
				(	gstin, fp, gt, cur_gt, sply_ty,txval,typ,etin, pos,  
					rt, iamt,camt,samt,csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					omon,diff_percent)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					sply_ty,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					typ,etin,pos,  
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),errorMessage,@FileId,
					omon,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CSA' 
					And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'B2CSA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End


		-- Import Amendments - EXPA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'EXPA')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'EXPA'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_EXPA_INV t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'EXPA'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'EXPA' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_EXPA_INV
				(	gstin, fp, gt, cur_gt,exp_typ,inum, idt, val,
					sbpcode,sbnum,sbdt,  
					txval,rt,iamt,csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice,
					oinum,oidt,diff_percent
				)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					exp_typ,inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					sbpcode,sbnum,sbdt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
				    Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0)))),
					oinum,oidt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'EXPA' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'EXPA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import Amendments - CDNRA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'CDNRA')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'CDNRA'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNRA t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNRA'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNRA' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_CDNRA
				(	gstin, fp, gt, cur_gt, ctin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					ont_num,ont_dt,diff_percent)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					ctin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@Referenceno,GetDate(),ErrorMessage,@FileId,
					ont_num,ont_dt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'CDNRA'
				And IsNull(ErrorCode,0) = 0
				 
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNRA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import Amendments - CDNURA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'CDNURA')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR1_RECS
							 Where DocType = 'CDNURA'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNURA t1,
							#TBL_CSV_GSTR1_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNURA'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNURA' 
					And IsNull(ErrorCode,0) = -101	

				End

				if @TemplateTypeId = 2
				Begin

					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'EXPWP'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNURA'
					And Exists (Select 1 From TBL_EXT_GSTR1_EXP_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.exp_typ = 'WPAY' 
						 And t2.rowstatus <> 2)
					And IsNull(ErrorCode,0) = 0
	
					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'EXPWOP'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNURA'
					And Exists (Select 1 From TBL_EXT_GSTR1_EXP_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.exp_typ = 'WOPAY'
						 And t2.rowstatus <> 2 )
					And IsNull(ErrorCode,0) = 0

					Update #TBL_CSV_GSTR1_RECS
					Set Cdnur_Typ = 'B2CL'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNURA'
					And Exists (Select 1 From TBL_EXT_GSTR1_B2CL_INV t2
						 Where t2.gstin = t1.gstin
						 And t2.inum = t1.inum
						 And t2.idt = t1.idt
						 And t2.rowstatus <> 2)
					And IsNull(ErrorCode,0) = 0

				End


				Insert TBL_EXT_GSTR1_CDNURA
				(	gstin, fp, gt, cur_gt, typ,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					ont_num,ont_dt,diff_percent)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					cdnur_typ,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					Case @TemplateTypeId When 2 Then 0 Else 1 End,
					@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					ont_num,ont_dt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'CDNURA' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'CDNURA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End


		-- Import Amendments - TXPA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'TXPA')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_TXPA
				(	gstin, fp, gt, cur_gt, pos, sply_ty,   
					rt, ad_amt,iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					omon,diff_percent)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos, sply_ty, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_adj_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					omon,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'TXPA' 
				And IsNull(ErrorCode,0) = 0
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'TXPA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

		-- Import Amendments - ATA

		if Exists (Select 1 From #TBL_CSV_GSTR1_RECS Where DocType = 'ATA')
		Begin

			Begin Try

				Insert TBL_EXT_GSTR1_ATA
				(	gstin, fp, gt, cur_gt, pos, sply_ty,   
					rt, ad_amt,iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					omon,diff_percent)
				Select 
					gstin, fp, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(gt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cur_gt,''),0)))),
					pos, sply_ty, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ad_recvd_amt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					omon,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(diff_percent,''),0))))
				From #TBL_CSV_GSTR1_RECS t1
				Where DocType = 'ATA' 
				And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR1_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR1_RECS t1
					Where DocType = 'ATA' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

	End 

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_CSV_GSTR1_RECS fv
	Where IsNull(fv.ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_CSV_GSTR1_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Update TBL_RECVD_FILES
	Set filestatus = 0,
		totalrecordscount = @TotalRecordsCount,
		processedrecordscount = @ProcessedRecordsCount,
		errorrecordscount = @ErrorRecordsCount
	Where fileid = @FileId

	Insert Into TBL_RECVD_FILE_ERRORS
	Select @FileId,fv.Slno,fv.ErrorCode,fv.ErrorMessage
	From  #TBL_CSV_GSTR1_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	--Format the Output to enable reupload of Error Records

	If @TemplateTypeId = 1
	Begin
		Select slno  As  [Slno],
				compcode  As  [Comp Code (Company code)],
				unitcode  As  [UnitCode (branch code)],
				doctype  As  [Document Type],
				gstin  As  [GSTIN (Supplier's GSTIN)],
				fp  As  [Period  (MMYYYY)],
				ctin  As  [Ctin (Customer Gstin)],
				etin  As  [Ecom GSTIN (if applicable)],
				inum  As  [Invoice No],
				idt  As  [Inv Date (DD-MM-YYYY)],
				inv_typ  As  [Invoice Type],
				hsncode  As  [HSN/SAC],
				hsndesc  As  [Item Description],
				uqc  As  [UQC],
				qty  As  [Quantity Of Goods Sold],
				unitprice  As  [Unit Price],
				rt  As  [GST Rate(%)],
				txval  As  [Item Taxable Value  (excluding tax)],
				iamt  As  [IGST Amount],
				camt  As  [CGST Amount],
				samt  As  [SGST/UTGST Amount],
				csamt  As  [Cess Amount       (If applicable)],
				totval  As  [Item Total Value(Including Tax)],
				val  As  [Total Invoice  or Note or Refund voucher Value(including taxes)],
				pos  As  [Place Of Supply (two digit state code)],
				rchrg  As  [Reverse Charge],
				rrate  As  [Rev Rate],
				sply_ty  As  [Supply Type],
				exp_typ  As  [Export Type],
				sbpcode  As  [Shipping Bill Port Code],
				sbnum  As  [ShippingBillNo],
				sbdt  As  [ShippingBillDate          (DD-MM-YYYY)],
				ntty  As  [Note Type],
				nt_num  As  [Note Number],
				nt_dt  As  [Note Date    (DD-MM-YYYY)],
				rsn  As  [Reason for Issuing Dr./ Cr. Notes],
				p_gst  As  [Pre GST Regime Dr./ Cr. Notes],
				cdnur_typ  As  [Type(cdnur)],
				'' As ' ',
				gt  As  [Gross Turnover],
				cur_gt  As  [Current Gross Turnover],
				'' As '  ',
				ad_recvd_amt  As  [Advance Received Amount],
				ad_adj_amt  As  [Advance Adjusted Amount],
				'' As '   ',
				nil_sply_ty  As  [Nil Supply Type],
				nil_amt  As  [Total Nil rated outward supplies],
				expt_amt  As  [Total Exempted outward supplies],
				ngsup_amt  As  [Total Non GST outward supplies],
				''As '    ',
				doc_nature  As  [Nature Of Document],
				from_serial_number  As  [From Serial Number   (For invoice/document)],
				to_serial_number  As  [To Serial Number   (For invoice/document)],
				totnum  As  [Total Number               (Of invoice/document)],
				cancel  As  [Cancelled  (Invoice/document)],
				net_issue  As  [Net Issued (Total number - Cancelled)],
				''As '      ',
				omon AS [Original Period  (MMYYYY)],
				oinum AS [Original Invoice No],
				oidt AS [ Original Inv Date (DD-MM-YYYY)],
				ont_num AS [Original Note Number],
				ont_dt AS [Original  Note Date (DD-MM-YYYY)],
				''As '      ',
				diff_percent AS [Applicable Tax Rate/Differential Tax Rate], 
				errorcode,
				errormessage
			From #TBL_CSV_GSTR1_RECS
			Where IsNull(ErrorCode,0) <> 0
	End
	Else
	Begin
		Select doctype  AS  [Document Type],
				slno  AS  [Slno],
				gstin  AS  [GSTIN      (Supplier's GSTIN)],
				fp  AS  [Period  (MMYYYY)],
				ctin  AS  [Ctin       (Customer Gstin)],
				etin  AS  [Ecom GSTIN     (if applicable)],
				inum  AS  [Invoice No.],
				idt  AS  [Inv Date     (DD-MM-YYYY)],
				inv_typ  AS  [Invoice Type],
				rt  AS  [GST Rate(%)],
				txval  AS  [Item Taxable Value  (excluding tax)],
				iamt  AS  [IGST Amount],
				camt  AS  [CGST Amount],
				samt  AS  [SGST/UTGST Amount],
				csamt  AS  [Cess Amount       (If applicable)],
				totval  AS  [Item Total Value],
				val  AS  [Total Invoice  or Note or Refund voucher Value(including taxes)],
				pos  AS  [Place Of Supply (two digit state code)],
				rchrg  AS  [Reverse Charge],
				hsncode  AS  [HSN/SAC (If applicable)],
				hsndesc  AS  [Item Description],
				uqc  AS  [UQC],
				qty  AS  [Quantity Of Goods Sold],
				gt  AS  [Gross Turnover (Only One Time Required)],
				cur_gt  AS  [Current Gross Turnover (Only One Time Required)],
				'' As ' ',
				exp_typ  AS  [Export Type],
				sbpcode  AS  [Shipping Bill Port Code],
				sbnum  AS  [ShippingBillNo],
				sbdt  AS  [ShippingBillDate (DD-MM-YYYY)],
				''As '  ',
				nt_num  AS  [Note No],
				nt_dt  AS  [Note Date],
				ntty  AS  [Note Type],
				rsn  AS  [Reason for Issuing Dr./ Cr. Notes],
				p_gst  AS  [Pre GST Regime Dr./ Cr. Notes],
				''As '   ',
				nil_sply_ty  AS  [Nil Supply Type],
				nil_amt  AS  [Total Nil rated outward supplies],
				expt_amt  AS  [Total Exempted outward supplies],
				ngsup_amt  AS  [Total Non GST outward supplies],
				'' As '    ',
				ad_recvd_amt  AS  [Advance Received Amount],
				''As '     ',
				ad_adj_amt  AS  [Advance Adjusted Amount],
				''As '      ',
				doc_nature  AS  [Nature Of Document],
				from_serial_number  AS  [From Serial Number   (For invoice/document)],
				to_serial_number  AS  [To Serial Number   (For invoice/document)],
				totnum  AS  [Total Number  (Of invoice/document)],
				cancel  AS  [Cancelled  (Invoice/document)],
				net_issue  AS  [Net Issued (Total number - cancelled)],
				''As '      ',
				omon AS [Original Period  (MMYYYY)],
				oinum AS [Original Invoice No],
				oidt AS [ Original Inv Date (DD-MM-YYYY)],
				ont_num AS [Original Note Number],
				ont_dt AS [Original  Note Date (DD-MM-YYYY)],
				''As '      ',
				diff_percent AS [Applicable Tax Rate/Differential Tax Rate], 
				errorcode,
				errormessage			
			From #TBL_CSV_GSTR1_RECS
			Where IsNull(ErrorCode,0) <> 0

	End


	 -- Drop Temp Tables

	 Drop Table #TBL_CSV_GSTR1_RECS
	 Drop Table #TBL_EXT_GSTR1_HSN
	 Delete From TBL_CSV_GSTR1_RECS Where fileid = @FileId
	
	 Return 0


End