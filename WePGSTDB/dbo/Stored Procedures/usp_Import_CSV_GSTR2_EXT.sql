
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 2 Records
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who			Decription 
06/12/2017	Seshadri / Karthik			Initial Version
08/17/2017	Seshadri	Changed the code to insert only records without errors
						into respective external tables

*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR2_EXT 'GSTR2 Sample Live Data','raja.m@wepindia.com'

 */
 
CREATE PROCEDURE [usp_Import_CSV_GSTR2_EXT]  
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


	Select recordcontents Into #GstrRecordValues
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
		cname varchar(50),
		is_sez varchar(50),
		stin varchar(50),
		boe_num varchar(50),
		boe_dt varchar(50),
		boe_val varchar(50),
		ntty varchar(50),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(50),
		rtin varchar(50),
		nilsply_ty varchar(50),
		nilsply varchar(50),
		exptdsply varchar(50),
		ngsply varchar(50),
		cpddr varchar(50),
		sply_ty varchar(50),
		ad_paid_amt varchar(50),
		ad_adj_amt varchar(50),
		errorcode smallint,
		errormessage varchar(255)
	 )
 
	 Begin Try

		Insert Into #GstrRecordFldValues
		Select 
			dbo.udf_SplitGSTRRecordData(RecordContents, 1,@Delimiter) as Slno ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 2,@Delimiter) as Compcode,
			dbo.udf_SplitGSTRRecordData(RecordContents, 3,@Delimiter) as Unitcode,
			dbo.udf_SplitGSTRRecordData(RecordContents, 4,@Delimiter) as Doctype,
			dbo.udf_SplitGSTRRecordData(RecordContents, 5,@Delimiter) as Gstin ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 6,@Delimiter) as Fp,
			dbo.udf_SplitGSTRRecordData(RecordContents, 7,@Delimiter) as Ctin,
			dbo.udf_SplitGSTRRecordData(RecordContents, 8,@Delimiter) as Inum,
			dbo.udf_SplitGSTRRecordData(RecordContents, 9,@Delimiter) as Idt ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 10,@Delimiter) as Inv_Typ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 11,@Delimiter) as Hsncode ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 12,@Delimiter) as Hsndesc,
			dbo.udf_SplitGSTRRecordData(RecordContents, 13,@Delimiter) as Uqc ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 14,@Delimiter) as Qty,
			dbo.udf_SplitGSTRRecordData(RecordContents, 15,@Delimiter) as Unitprice ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 16,@Delimiter) as Rt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 17,@Delimiter) as Txval,
			dbo.udf_SplitGSTRRecordData(RecordContents, 18,@Delimiter) as Iamt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 19,@Delimiter) as Camt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 20,@Delimiter) as Samt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 21,@Delimiter) as Csamt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 22,@Delimiter) as TotVal,
			dbo.udf_SplitGSTRRecordData(RecordContents, 23,@Delimiter) as Val,
			dbo.udf_SplitGSTRRecordData(RecordContents, 24,@Delimiter) as Pos,
			dbo.udf_SplitGSTRRecordData(RecordContents, 25,@Delimiter) as Rchrg ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 26,@Delimiter) as Rrate,
			dbo.udf_SplitGSTRRecordData(RecordContents, 27,@Delimiter) as Cname,
			dbo.udf_SplitGSTRRecordData(RecordContents, 28,@Delimiter) as Is_Sez ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 29,@Delimiter) as Stin,
			dbo.udf_SplitGSTRRecordData(RecordContents, 30,@Delimiter) as Boe_Num ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 31,@Delimiter) as Boe_Dt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 32,@Delimiter) as Boe_Val ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 33,@Delimiter) as Ntty,
			dbo.udf_SplitGSTRRecordData(RecordContents, 34,@Delimiter) as Nt_Num ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 35,@Delimiter) as Nt_Dt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 36,@Delimiter) as Rsn,
			dbo.udf_SplitGSTRRecordData(RecordContents, 37,@Delimiter) as P_Gst,
			dbo.udf_SplitGSTRRecordData(RecordContents, 38,@Delimiter) as Rtin,
				-- Skip 39
			dbo.udf_SplitGSTRRecordData(RecordContents, 40,@Delimiter) as Nilsply_Ty,
			dbo.udf_SplitGSTRRecordData(RecordContents, 41,@Delimiter) as Nilsply,
			dbo.udf_SplitGSTRRecordData(RecordContents, 42,@Delimiter) as Exptdsply ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 43,@Delimiter) as Ngsply,
			dbo.udf_SplitGSTRRecordData(RecordContents, 44,@Delimiter) as Cpddr,
				-- Skip 45
			dbo.udf_SplitGSTRRecordData(RecordContents, 46,@Delimiter) as Sply_Ty ,
			dbo.udf_SplitGSTRRecordData(RecordContents, 47,@Delimiter) as Ad_Paid_Amt,
			dbo.udf_SplitGSTRRecordData(RecordContents, 48,@Delimiter) as Ad_Adj_Amt,
			null,
			null  
		From #GstrRecordValues

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
		Set Gstin = Upper(Gstin), 
			Ctin = Upper(Ctin)

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
			ErrorMessage = 'Invalid Invoice No'
		Where ( DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceNo(Inum) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('B2B','B2BUR','IMPS','CDNR','CDNUR') and
				dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Invoice Type'
		Where (	DocType In ('B2B') and
				dbo.udf_ValidateInvoiceType(Inv_Typ) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -8,
			ErrorMessage = 'HSN Code is mandatory'
		Where DocType In ('B2B','B2BUR','IMPG','IMPS')
		And (Ltrim(Rtrim(IsNull(Hsncode,''))) = '' )
		And IsNull(ErrorCode,0) = 0 

		Update #GstrRecordFldValues 
		Set ErrorCode = -9,
			ErrorMessage = 'Item Description is mandatory'
		Where DocType In ('B2B','B2BUR','IMPG','IMPS')
		And (Ltrim(Rtrim(IsNull(Hsndesc,''))) = '')
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -10,
			ErrorMessage = 'UQC is mandatory'
		Where DocType In ('B2B','B2BUR','IMPG','IMPS')
		And Ltrim(Rtrim(IsNull(Uqc,''))) = '' 
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -11,
			ErrorMessage = 'Invalid Quantity'
		Where (DocType In ('B2B','B2BUR','IMPG','IMPS') and
			   dbo.udf_ValidateQuantity(Qty) <> 1)
		And IsNull(ErrorCode,0) = 0 

		Update #GstrRecordFldValues 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Rate'
		Where (DocType In ('B2B','B2BUR','IMPG','IMPS','CDNR','CDNUR',
							'TXP','TXI') and
			   dbo.udf_ValidateRate(Rt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2B','B2BUR','IMPG','IMPS','CDNR','CDNUR') and
			   dbo.udf_ValidateTaxableValue(Txval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -14,
			ErrorMessage = 'Place of Supply is mandatory'
		Where DocType In ('B2B','IMPS','TXP','TXI')
		And Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Place Of Supply'
		Where ( DocType In ('B2B','IMPS','TXP','TXI') and
				dbo.udf_ValidatePlaceOfSupply(Pos) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -16,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2B','B2BUR','IMPS') and
			   dbo.udf_ValidateInvoiceValue(Val) <> 1)
		And IsNull(ErrorCode,0) = 0

	
		/*
		Update #GstrRecordFldValues 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Amount'
		Where (	DocType In ('B2B','B2CL','B2CS','CDNR',
				'EXP','CDNUR','TXP','AT') and
				dbo.udf_ValidateAmount(DocType,Gstin,Ctin,Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,Ad_Recvd_Amt,Ad_Adj_Amt) <> 1)
		And IsNull(ErrorCode,0) = 0 */

		Update #GstrRecordFldValues 
		Set ErrorCode = -21,
			ErrorMessage = 'Invalid Note Type'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteType(Ntty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -22,
			ErrorMessage = 'Invalid Note No'
		Where ( DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteNo(Nt_Num) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -23,
			ErrorMessage = 'Invalid Note Date'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Idt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #GstrRecordFldValues 
		Set ErrorCode = -24,
			ErrorMessage = 'Invalid Pre GST Regime'
		Where (	DocType In ('CDNR','CDNUR') and
				dbo.udf_ValidatePreGstRegime(P_Gst) <> 1)
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with item errors

		Select distinct doctype,gstin,inum,idt
			Into #TmpRecords
		From #GstrRecordFldValues  
		Where DocType in ('B2B','B2BUR','IMPS','CDNR','CDNUR')
		And IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #GstrRecordFldValues 
			Set ErrorCode = -25,
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2B_INV 
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2B_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'B2BUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2BUR_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'B2BUR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_B2BUR_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #GstrRecordFldValues t1
		Where DocType = 'IMPS' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
								Where gstin = t1.gstin 
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update #GstrRecordFldValues 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #GstrRecordFldValues t1
		Where DocType = 'IMPS' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDN 
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDN 
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDNUR 
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
		And Exists(Select 1 From  TBL_EXT_GSTR2_CDNUR 
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
					From	TBL_EXT_GSTR2_B2B_INV t1,
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

				Insert TBL_EXT_GSTR2_B2B_INV
				(	gstin, fp, ctin,inum, idt, val, pos, rchrg, inv_typ, 
					rt, txval, iamt, camt, samt, csamt,
					tx_i,tx_c,tx_s,tx_cs,elg,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
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
					null,null,null,null,null,
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
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
					select 'B2B -' +  error_message()
				End				
			End Catch

		End

		-- Import B2BUR Invoices

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'B2BUR')
		Begin
			
			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'B2BUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_B2BUR_INV t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'B2BUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'B2BUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_B2BUR_INV
				(	gstin, fp, inum, idt, val, cname,  
					rt, txval, camt, samt,csamt,
					tx_c,tx_s,tx_cs,elg,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice)
				Select 
					gstin, fp, inum, idt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(val,''),0)))),
					cname,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					null,null,null,null,
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From  #GstrRecordFldValues t1
				Where DocType = 'B2BUR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'B2BUR -' + error_message()
				End				
			End Catch

		End

		-- Import IMPG

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'IMPG')
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_IMPG_INV
				(	gstin, fp, is_sez,stin,boe_num,boe_dt,boe_val,  
					rt,txval,iamt,csamt,
					elg,tx_i,tx_cs,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid,
					hsncode,hsndesc,uqc,qty,unitprice
				)
				Select 
					gstin, fp, is_sez,stin,boe_num,boe_dt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(boe_val,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					null,null,null,
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From #GstrRecordFldValues t1
				Where DocType = 'IMPG' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'IMPG -' + error_message()
				End				
			End Catch
	
		End

		-- Import IMPS

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'IMPS')
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'IMPS'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_IMPS_INV t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'IMPS'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
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
		 			null,null,null,
					1 ,@SourceType , @ReferenceNo,GetDate(),ErrorMessage,@FileId,
					hsncode,hsndesc,uqc,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(qty,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(unitprice,''),0))))
				 From #GstrRecordFldValues t1
				Where DocType = 'IMPS' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'IMPS -' + error_message()
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
					From	TBL_EXT_GSTR2_CDN t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'CDNR'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_CDN
				(	gstin, fp, ctin,nt_num, nt_dt, inum, idt,ntty,  
					rt, txval, iamt, camt, samt, csamt,
					tx_i,tx_c,tx_s,tx_cs,elg,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, ctin,nt_num, nt_dt, inum, idt,ntty,  
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					null,null,null,null,null,
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From #GstrRecordFldValues t1
				Where DocType = 'CDNR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'CDNR -' + error_message()
				End				
			End Catch
	
		End

		-- Import CDNUR

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'CDNUR' )
		Begin

			Begin Try

				if Exists(Select 1 From #GstrRecordFldValues
							 Where DocType = 'CDNUR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR2_CDNUR t1,
							#GstrRecordFldValues t2
					Where t1.gstin = t2.gstin
					And	t1.inum = t2.inum
					And t1.idt = t2.idt
					And DocType = 'CDNUR'
					And IsNull(ErrorCode,0) = -101
					
					Update #GstrRecordFldValues 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #GstrRecordFldValues t1
					Where DocType = 'CDNUR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR2_CDNUR
				(	gstin, fp, rtin,ntty,nt_num,nt_dt,inum, idt,   
					txval, camt, samt, csamt,rt,
					tx_c,tx_s,tx_cs,elg,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, rtin,ntty,nt_num,nt_dt,inum, idt,  
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
		 			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					null,null,null,null,
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				 From #GstrRecordFldValues t1
				Where DocType = 'CDNUR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'CDNUR -' + error_message()
				End				
			End Catch
	
		End

		-- Import NIL

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'NIL' )
		Begin

			Begin Try
				Insert TBL_EXT_GSTR2_NIL
				(	gstin, fp,niltype,cpddr,exptdsply, ngsply,nilsply,   
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid
				)
				Select 
					gstin, fp,'Inter',
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cpddr,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(exptdsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ngsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(nilsply,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #GstrRecordFldValues t1
				Where DocType = 'NIL' 
				And nilsply_ty = 'Inter'
				Union All
				Select 
					gstin, fp,'Intra',
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cpddr,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(exptdsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(ngsply,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(nilsply,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId
				From #GstrRecordFldValues t1
				Where DocType = 'NIL' 
				And nilsply_ty = 'Intra'
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'NIL -' + error_message()
				End				
			End Catch
	
		End
		
		-- Import TXP

		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'TXP' )
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
				 From #GstrRecordFldValues t1
				Where DocType = 'TXP' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'TXP -' + error_message()
				End				
			End Catch
	
		End

		-- Import TXI
	
		if Exists (Select 1 From #GstrRecordFldValues Where DocType = 'TXI')
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
				 From  #GstrRecordFldValues t1
				Where DocType = 'TXI' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					select 'TXI -' + error_message()
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