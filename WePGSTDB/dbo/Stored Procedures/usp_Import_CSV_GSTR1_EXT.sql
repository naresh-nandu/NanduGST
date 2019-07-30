
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 1 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
08/08/2017	Seshadri	Included typ for B2CS Records ; rsn,p_gst for CDNR Records
08/10/2017	Seshadri	Reintroduced HSN Doc Type 
*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR1_EXT 'GSTR1 Sample Live Data','raja.m@wepindia.com'

 */
 
CREATE PROCEDURE [usp_Import_CSV_GSTR1_EXT]  
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
	And gstrtypeid = 1
	And filestatus = 1

	Update TBL_RECVD_FILES
	Set filestatus = 2
	Where fileid = @FileId


	Select recordid,recordcontents Into #GstrRecordValues
	From TBL_RECVD_FILE_RECORDS 
	Where fileid = @FileId

	Create Table #GstrRecordFldValues
	(
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
		doc_nature varchar(50),
		from_serial_number varchar(50),
		to_serial_number varchar(50),
		totnum varchar(50),
		cancel varchar(50),
		net_issue varchar(50),
		typ varchar(50),
		doc_num varchar(50),
		errorcode smallint,
		errormessage varchar(255)
	 )

	 IF Not EXISTS(SELECT 1 FROM sys.indexes WHERE object_id = object_id('tempdb..#GstrRecordFldValues') 
			AND NAME ='IDX_NC_TBL_GSTR_REC_FLD_VALUES')
	 Begin
		CREATE NONCLUSTERED INDEX IDX_NC_TBL_GSTR_REC_FLD_VALUES ON #GstrRecordFldValues(doctype)
		INCLUDE(errorcode,errormessage)
	 End

 
	 Begin Try

		/*
		Insert Into #GstrRecordFldValues
		Select 
			dbo.udf_SplitGSTRRecordData(RecordContents, 1,@Delimiter) as Slno ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 2,@Delimiter) as Compcode,
			dbo.udf_SplitGSTRRecordData(RecordContents, 3,@Delimiter) as Unitcode,
			dbo.udf_SplitGSTRRecordData(RecordContents, 4,@Delimiter) as Doctype,
			dbo.udf_SplitGSTRRecordData(RecordContents, 5,@Delimiter) as Gstin ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 6,@Delimiter) as Fp,
			dbo.udf_SplitGSTRRecordData(RecordContents, 7,@Delimiter) as Ctin,
			dbo.udf_SplitGSTRRecordData(RecordContents, 8,@Delimiter) as Etin,
			dbo.udf_SplitGSTRRecordData(RecordContents, 9,@Delimiter) as Inum,
			dbo.udf_SplitGSTRRecordData(RecordContents, 10,@Delimiter) as Idt ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 11,@Delimiter) as Inv_Typ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 12,@Delimiter) as Hsncode ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 13,@Delimiter) as Hsndesc,
			dbo.udf_SplitGSTRRecordData(RecordContents, 14,@Delimiter) as Uqc ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 15,@Delimiter) as Qty,
			dbo.udf_SplitGSTRRecordData(RecordContents, 16,@Delimiter) as Unitprice ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 17,@Delimiter) as Rt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 18,@Delimiter) as Txval,
			dbo.udf_SplitGSTRRecordData(RecordContents, 19,@Delimiter) as Iamt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 20,@Delimiter) as Camt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 21,@Delimiter) as Samt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 22,@Delimiter) as Csamt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 23,@Delimiter) as TotVal,
			dbo.udf_SplitGSTRRecordData(RecordContents, 24,@Delimiter) as Val,
			dbo.udf_SplitGSTRRecordData(RecordContents, 25,@Delimiter) as Pos,
			dbo.udf_SplitGSTRRecordData(RecordContents, 26,@Delimiter) as Rchrg ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 27,@Delimiter) as Rrate,
			dbo.udf_SplitGSTRRecordData(RecordContents, 28,@Delimiter) as Sply_Ty ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 29,@Delimiter) as Exp_Typ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 30,@Delimiter) as Sbpcode ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 31,@Delimiter) as Sbnum ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 32,@Delimiter) as Sbdt ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 33,@Delimiter) as Ntty,
			dbo.udf_SplitGSTRRecordData(RecordContents, 34,@Delimiter) as Nt_Num ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 35,@Delimiter) as Nt_Dt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 36,@Delimiter) as Rsn,
			dbo.udf_SplitGSTRRecordData(RecordContents, 37,@Delimiter) as P_Gst,
			dbo.udf_SplitGSTRRecordData(RecordContents, 38,@Delimiter) as Cdnur_Typ,
		
			-- Skip 39

			dbo.udf_SplitGSTRRecordData(RecordContents, 40,@Delimiter) as Gt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 41,@Delimiter) as Cur_Gt,
	
			-- Skip 42

			dbo.udf_SplitGSTRRecordData(RecordContents, 43,@Delimiter) as Ad_Recvd_Amt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 44,@Delimiter) as Ad_Adj_Amt,
		
			-- Skip 45
		
			dbo.udf_SplitGSTRRecordData(RecordContents, 46,@Delimiter) as Nil_Sply_Ty ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 47,@Delimiter) as Nil_Amt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 48,@Delimiter) as Expt_Amt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 49,@Delimiter) as Ngsup_Amt ,
		
			-- Skip 50
		
			dbo.udf_SplitGSTRRecordData(RecordContents, 51,@Delimiter) as Doc_Nature ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 52,@Delimiter) as From_Serial_Number,
			dbo.udf_SplitGSTRRecordData(RecordContents, 53,@Delimiter) as To_Serial_Number,
			dbo.udf_SplitGSTRRecordData(RecordContents, 54,@Delimiter) as Totnum,
			dbo.udf_SplitGSTRRecordData(RecordContents, 55,@Delimiter) as Cancel,
			dbo.udf_SplitGSTRRecordData(RecordContents, 56,@Delimiter) as Net_Issue,

			null,
			null,
			null,
			null  
		From #GstrRecordValues

		*/

		Insert Into #GstrRecordFldValues
		Select t2.*,null,null,null,null
		From #GstrRecordValues t1 
		Cross Apply dbo.udf_SplitString(t1.RecordContents,@Delimiter) t2 
		Where t1.RecordId = t2.Col_1

	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch

	-- Processing Logic

	if exists (Select 1 from #GstrRecordFldValues)
	Begin

		Update #GstrRecordFldValues -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #GstrRecordFldValues -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #GstrRecordFldValues
		Set Gt = t1.Gt, 
			Cur_Gt = t1.Cur_Gt
		From
		(Select Top(1) Slno,Gt, Cur_Gt From #GstrRecordFldValues ) t1
		Where #GstrRecordFldValues.Slno <> t1.Slno

		Update #GstrRecordFldValues
		Set Gstin = Upper(Gstin), 
			Ctin = Upper(Ctin),
			Etin = Upper(Etin)

		Update #GstrRecordFldValues 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Doc Type'
		Where dbo.udf_ValidateDocType(DocType) <> 1

		Update #GstrRecordFldValues 
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		Update #GstrRecordFldValues 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Ctin'
		Where (DocType In ('B2B','CDNR') and
			   dbo.udf_ValidateGstin(Ctin) <> 1)
		And IsNull(ErrorCode,0) = 0
	
		Update #GstrRecordFldValues 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Ecommerce Gstin'
		Where (DocType In ('B2B','B2CL','B2CS') and
			   Ltrim(Rtrim(IsNull(Etin,''))) <> '' and
			   dbo.udf_ValidateGstin(Etin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice No'
		Where ( DocType In ('B2B','B2CL','EXP','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceNo(Inum) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('B2B','B2CL','EXP','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Invoice Type'
		Where (	DocType In ('B2B') and
				dbo.udf_ValidateInvoiceType(Inv_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0

		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -9,
			ErrorMessage = 'HSN Code is mandatory'
		Where DocType In ('B2B','B2CL','B2CS','EXP')
		And (Ltrim(Rtrim(IsNull(Hsncode,''))) = '' )
		And IsNull(ErrorCode,0) = 0 
		*/

		Update #GstrRecordFldValues 
		Set ErrorCode = -9,
			ErrorMessage = 'HSN Code is mandatory'
		Where DocType In ('HSN')
		And (Ltrim(Rtrim(IsNull(Hsncode,''))) = '' )
		And IsNull(ErrorCode,0) = 0 

		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -10,
			ErrorMessage = 'Item Description is mandatory'
		Where DocType In ('B2B','B2CL','B2CS','EXP')
		And (Ltrim(Rtrim(IsNull(Hsndesc,''))) = '')
		And IsNull(ErrorCode,0) = 0
		*/

		Update #GstrRecordFldValues 
		Set ErrorCode = -10,
			ErrorMessage = 'Item Description is mandatory'
		Where DocType In ('HSN')
		And (Ltrim(Rtrim(IsNull(Hsndesc,''))) = '')
		And IsNull(ErrorCode,0) = 0

		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -11,
			ErrorMessage = 'UQC is mandatory'
		Where DocType In ('B2B','B2CL','B2CS','EXP')
		And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
		And IsNull(ErrorCode,0) = 0
		*/
		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -11,
			ErrorMessage = 'UQC is mandatory'
		Where DocType In ('HSN')
		And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
		And IsNull(ErrorCode,0) = 0
		*/
		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Quantity'
		Where (DocType In ('B2B','B2CL','B2CS','EXP') and
			   dbo.udf_ValidateQuantity(Qty) <> 1)
		And IsNull(ErrorCode,0) = 0 
		*/

		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Quantity'
		Where (DocType In ('HSN') and
			   dbo.udf_ValidateQuantity(Qty) <> 1)
		And IsNull(ErrorCode,0) = 0 
		*/

		Update #GstrRecordFldValues 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Rate'
		Where (DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR',
							'TXP','AT') and
			   dbo.udf_ValidateRate(Rt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -14,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR','HSN') and
			   dbo.udf_ValidateTaxableValue(Txval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -15,
			ErrorMessage = 'Place of Supply is mandatory'
		Where DocType In ('B2B','B2CL','B2CS','TXP','AT')
		And Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -16,
			ErrorMessage = 'Invalid Place Of Supply'
		Where ( DocType In ('B2B','B2CL','B2CS','TXP','AT') and
				dbo.udf_ValidatePlaceOfSupply(Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -17,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2B','B2CL','B2CS','EXP','CDNR','CDNUR') and
			   dbo.udf_ValidateInvoiceValue(Val) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -18,
			ErrorMessage = 'Invoice Value must be greater than 2.5 Lakh'
		Where DocType = 'B2CL'
		And (IsNumeric(Val) <> 1 Or Convert(dec,Val) <= 250000.00)
		And IsNull(ErrorCode,0) = 0
		
		Update #GstrRecordFldValues 
		Set ErrorCode = -19,
			ErrorMessage = 'Invoice Value must be less than 2.5 Lakh for Inter State Supply'
		Where DocType = 'B2CS'
		And (IsNumeric(Val) <> 1 Or Convert(dec,Val) >= 250000.00)
		And (Substring(Gstin,1,2) <> Pos)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -20,
			ErrorMessage = 'Invalid Advance Adjusted Amount'
		Where (	DocType In ('TXP') and
				dbo.udf_ValidateAmount(Ad_Adj_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -21,
			ErrorMessage = 'Invalid Advance Received Amount'
		Where (	DocType In ('AT') and
				dbo.udf_ValidateAmount(Ad_Recvd_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0


		Update #GstrRecordFldValues 
		Set ErrorCode = -22,
			ErrorMessage = dbo.udf_ValidateGstAmount(DocType,Gstin,Ctin,Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,Ad_Adj_Amt,Ad_Recvd_Amt) 
		Where DocType In ('B2B','B2CL','B2CS','CDNR',
							'EXP','CDNUR','TXP','AT') 
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -22
		And IsNull(ErrorMessage,'') = ''

		Update #GstrRecordFldValues 
		Set ErrorCode = -23,
			ErrorMessage = 'Invalid Reverse Charge'
		Where ( DocType = 'B2B' and
				dbo.udf_ValidateReverseCharge(Rchrg) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -24,
			ErrorMessage = 'Invalid Supply Type'
		Where ( DocType In ('B2CS','TXP','AT') and
				dbo.udf_ValidateSupplyType(Sply_Ty,Gstin,Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -25,
			ErrorMessage = 'Invalid Export Type'
		Where ( DocType = 'EXP' and
				dbo.udf_ValidateExportType(Exp_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -26,
			ErrorMessage = 'Invalid Note Type'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteType(Ntty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -27,
			ErrorMessage = 'Invalid Note No'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteNo(Nt_Num) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -28,
			ErrorMessage = 'Invalid Note Date'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Idt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -29,
			ErrorMessage = 'Invalid Pre GST Regime'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidatePreGstRegime(P_Gst) <> 1)
		And IsNull(ErrorCode,0) = 0


		Update #GstrRecordFldValues 
		Set ErrorCode = -30,
			ErrorMessage = 'Invalid Nil Supply Type'
		Where (	DocType In ('NIL') and
				dbo.udf_ValidateNilSupplyType(Nil_Sply_Ty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -31,
			ErrorMessage = 'Invalid Total Nil rated outward supplies'
		Where (	DocType In ('NIL') and
				dbo.udf_ValidateAmount(Nil_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -32,
			ErrorMessage = 'Invalid Total Exempted outward supplies'
		Where (	DocType In ('NIL') and
				dbo.udf_ValidateAmount(Expt_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -33,
			ErrorMessage = 'Invalid Total Non GST outward supplies'
		Where (	DocType In ('NIL') and
				dbo.udf_ValidateAmount(Ngsup_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set typ = 'OE'
		Where (DocType = 'B2CS')
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set typ = 'E'
		Where (DocType = 'B2CS' and
				IsNull(Etin,'') <> '' )
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set doc_num =
		Case doc_nature
			When 'Invoices for outward supply' Then '1'
			When 'Invoices for inward supply from unregistered person'  Then '2'
			When 'Revised Invoice' Then '3'
			When 'Debit Note' Then '4'
			When 'Credit Note' Then '5'
			When 'Receipt voucher' Then '6'
			When 'Payment Voucher' Then '7'
			When 'Refund voucher' Then '8'
			When 'Delivery Challan for job work' Then '9'
			When 'Delivery Challan for supply on approval' Then '10'
			When 'Delivery Challan in case of liquid gas' Then '11'
			When 'Delivery Challan in cases other than by way of supply (excluding at S no. 9 to 11)' Then '12'
			Else ''
		End
		Where (DocType = 'B2CS')
		And IsNull(ErrorCode,0) = 0



		-- Tracking Invoices with item errors

		Select distinct doctype,gstin,inum,idt
			Into #TmpRecords
		From #GstrRecordFldValues  
		Where DocType in ('B2B','B2CL','EXP','CDNR','CDNUR')
		And IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #GstrRecordFldValues 
			Set ErrorCode = -34,
				ErrorMessage = 'Error in Invoice Item.'
			From #GstrRecordFldValues t1,
				 #TmpRecords t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And IsNull(ErrorCode,0) = 0
		End

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2B_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2B_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'B2CL' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CL_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'B2CL' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_B2CL_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'EXP' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'EXP' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)


		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNR 
								Where gstin = t1.gstin 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNR 
								Where gstin = t1.gstin 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNUR 
								Where gstin = t1.gstin 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'CDNUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNR 
								Where gstin = t1.gstin 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)


		-- Import B2B Invoices

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'B2B')
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'B2B'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2B_INV t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2B'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
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
				From #GstrRecordFldValues t1
				Where DocType = 'B2B' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'B2B -' + 'Error in Invoice Data'
				End				
			End Catch
		End

		-- Import B2CL Invoices
		
		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'B2CL')
		Begin
			
			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'B2CL'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_B2CL_INV t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2CL'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
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
				From #GstrRecordFldValues t1
				Where DocType = 'B2CL' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'B2CL -' + 'Error in Invoice Data'
				End				
			End Catch

		End

		-- Import B2CS Invoices

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'B2CS')
		Begin

			Begin Try
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
				 From #GstrRecordFldValues t1
				 Where DocType = 'B2CS' 
				 And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'B2CS -' + 'Error in Invoice Data'
				End				
			End Catch

		End
	
		-- Import Exports

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'EXP')
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'EXP'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_EXP_INV t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'EXP'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'EXP' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_EXP_INV
				(	gstin, fp, gt, cur_gt,exp_typ,inum, idt, val,
					sbpcode,sbnum,sbdt,  
					txval,rt,iamt,
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
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				From #GstrRecordFldValues t1
				Where DocType = 'EXP' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'EXP -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End
		
		-- Import CDNR

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'CDNR')
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'CDNR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNR t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNR'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_CDNR
				(	gstin, fp, gt, cur_gt, ctin,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
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
					1 ,@SourceType ,@Referenceno,GetDate(),ErrorMessage,@FileId
				From #GstrRecordFldValues t1
				Where DocType = 'CDNR'
				And IsNull(ErrorCode,0) = 0
				 
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'CDNR -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

		-- Import CDNUR

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'CDNUR')
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'CDNUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR1_CDNUR t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR1_CDNUR
				(	gstin, fp, gt, cur_gt, typ,ntty,nt_num,nt_dt,
					rsn,p_gst,
					inum, idt, val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
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
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #GstrRecordFldValues t1
				Where DocType = 'CDNUR' 
				And IsNull(ErrorCode,0) = 0
	
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'CDNUR -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

		-- Import HSN

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'HSN' 
					And IsNull(ErrorCode,0) = 0)
		Begin

			Begin Try

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
				From #GstrRecordFldValues t1
				Where DocType = 'HSN' 
				And IsNull(ErrorCode,0) = 0
				And Not Exists(Select 1 From TBL_EXT_GSTR1_HSN   
									Where gstin = t1.gstin 
									And hsn_sc = t1.hsncode 
									And qty = Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(t1.qty,''),0))))
									And val=  Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totval,''),0))))
								)
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'HSN -' + ERROR_MESSAGE()
					select 'HSN -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

	

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'TXP')
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
				From #GstrRecordFldValues t1
				Where DocType = 'TXP' 
				And IsNull(ErrorCode,0) = 0
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'TXP -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'NIL')
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
					sply_ty, 
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #GstrRecordFldValues t1
				Where DocType = 'NIL' 
				And IsNull(ErrorCode,0) = 0
				And Not Exists(Select 1 From TBL_EXT_GSTR1_NIL   
										Where gstin = t1.gstin 
										And nil_amt = t1.nil_amt 
										And expt_amt = t1.expt_amt
										And sply_ty = t1.sply_ty)

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'NIL -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'AT')
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
				From #GstrRecordFldValues t1
				Where DocType = 'AT' 
				And IsNull(ErrorCode,0) = 0

			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'AT -' + 'Error in Invoice Data'
				End				
			End Catch
	
		End

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'DOCISSUE')
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
				From #GstrRecordFldValues t1
				Where DocType = 'DOCISSUE' 
				And IsNull(ErrorCode,0) = 0
				And Not Exists(Select 1 From TBL_EXT_GSTR1_DOC   
									Where gstin = t1.gstin 
									And doc_num = t1.doc_num)
		
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'DOCISSUE -' +  'Error in Invoice Data'
				End				
			End Catch
	
		End

	End 


	Select @TotalRecordsCount = Count(*)
	From #GstrRecordFldValues

	Select @ProcessedRecordsCount = Count(*)
	From #GstrRecordFldValues fv
	Where IsNull(fv.ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #GstrRecordFldValues fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Update TBL_RECVD_FILES
	Set filestatus = 0,
		totalrecordscount = @TotalRecordsCount,
		processedrecordscount = @ProcessedRecordsCount,
		errorrecordscount = @ErrorRecordsCount
	Where fileid = @FileId

	
	Insert Into TBL_RECVD_FILE_ERRORS
	Select @FileId,fv.Slno,fv.ErrorCode,fv.ErrorMessage
	From  #GstrRecordFldValues fv
	Where IsNull(fv.ErrorCode,0) <> 0


	Select * from #GstrRecordFldValues
	Where IsNull(ErrorCode,0) <> 0

	 -- Drop Temp Tables

	 Drop Table #GstrRecordValues
	 Drop Table #GstrRecordFldValues
	
	 Return 0


End