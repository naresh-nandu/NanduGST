

/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import GSTR - 6 Records
				
Written by  : muskan.garg@wepdigital.com

Date		Who			Decription 
05/21/2018	Muskan   	Initial Version

*/

/* Sample Procedure Call

exec usp_Import_CSV_GSTR6_EXT_PERF 'C:\Users\muskang\Desktop\TEST_DB APP\WePGST_Panel\webapp\App_Data\uploads\gstr6_120180521191019594.csv','raja.m@wepindia.com','WEP001'

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_GSTR6_EXT_PERF]  
	@FileName varchar(255),
	@UserId varchar(255),
	@ReferenceNo varchar(50),
	@TemplateTypeId tinyint = 1, -- 1 - Template A ;
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
	And gstrtypeid = 4
	And filestatus = 1

	Update TBL_RECVD_FILES
	Set filestatus = 2
	Where fileid = @FileId

	Create Table #TBL_CSV_GSTR6_RECS
	(
		fileid int,
		slno varchar(50),
		doctype varchar(50),
		gstin varchar(50),
		fp varchar(50),
		typ varchar(50),
		ctin varchar(50),
		isd_docty varchar(50),
		docnum varchar(50),
		docdt varchar(50),
		elglst varchar(50),
		iamti varchar(50),
		iamts varchar(50),
		iamtc varchar(50),
		samts varchar(50),
		samti varchar(50),
		camti varchar(50),
		camtc varchar(50),
		rt varchar(50),
		txval varchar(50),
		iamt varchar(50),
		camt varchar(50),
		samt varchar(50),
		csamt varchar(50),
		totval varchar(50),
		statecd varchar(50),
		ntty varchar(50),
		nt_num varchar(50),
		nt_dt varchar(50),
		rcpty varchar(50),
		odocnum varchar(50),
		odocdt varchar(50),
		ont_num varchar(50),
		ont_dt varchar(50),
		rstatecd varchar(50),		
		errorcode smallint,
		errormessage varchar(255)
	)

	Begin Try

		Insert Into #TBL_CSV_GSTR6_RECS
		Select t1.*
		From TBL_CSV_GSTR6_RECS t1 
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
	From #TBL_CSV_GSTR6_RECS

	if exists (Select 1 from #TBL_CSV_GSTR6_RECS)
	Begin

		Update #TBL_CSV_GSTR6_RECS -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_CSV_GSTR6_RECS -- Introduced due to Excel Format Issue
		Set Statecd = '0' + Ltrim(Rtrim(IsNull(Statecd,'')))
		Where Len(Ltrim(Rtrim(IsNull(Statecd,'')))) = 1

		Update #TBL_CSV_GSTR6_RECS
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Ctin = Upper(Ltrim(Rtrim(IsNull(Ctin,'')))),
			Ntty = Upper(Ltrim(Rtrim(IsNull(Ntty,'')))),
			Elglst = Lower(Ltrim(Rtrim(IsNull(Elglst,''))))

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Doc Type'
		Where dbo.udf_ValidateDocType(DocType) <> 1

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Ctin'
		Where (DocType In ('B2B','CDNR','B2BA','CDNRA') and
			   dbo.udf_ValidateGstin(Ctin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Invoice No'
		Where ( DocType In ('B2B','B2BA','CDNR','CDNRA') and
				dbo.udf_ValidateInvoiceNo(Docnum) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice Date'
		Where (	DocType In ('B2B','B2BA','CDNR','CDNRA') and
				dbo.udf_ValidateInvoiceDate(Docdt,Fp) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Rate'
		Where (DocType In ('B2B','B2BA','CDNR','CDNRA') and
			   dbo.udf_ValidateRate(Rt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -14,
			ErrorMessage = 'Invalid Taxable Value'
		Where (DocType In ('B2B','B2BA','CDNR','CDNRA') and
			   dbo.udf_ValidateTaxableValue(Txval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid Invoice Value'
		Where (DocType In ('B2B','B2BA','CDNR','CDNRA') and
			   dbo.udf_ValidateInvoiceValue(Totval) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -16,
			ErrorMessage = 'Place of Supply is mandatory'
		Where DocType In ('B2B','B2BA','CDNR','CDNRA')
		And Ltrim(Rtrim(IsNull(Statecd,''))) In ('','0')
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -17,
			ErrorMessage = 'Invalid Place Of Supply'
		Where ( DocType In ('B2B','B2BA','CDNR','CDNRA') and
				dbo.udf_ValidatePlaceOfSupply(Statecd) <> 1)
		And IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -25,
			ErrorMessage = 'Invalid Note Type'
		Where ( DocType In ('CDNR','CDNRA') and
				dbo.udf_ValidateNoteType(Ntty) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -26,
			ErrorMessage = 'Invalid Note No'
		Where ( DocType In ('CDNR','CDNRA') and
				dbo.udf_ValidateNoteNo(Nt_Num) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -27,
			ErrorMessage = 'Invalid Note Date'
		Where (	DocType In ('CDNR','CDNRA') and
				dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Docdt) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -39
		And IsNull(ErrorMessage,'') = ''
	
		Update #TBL_CSV_GSTR6_RECS 
		Set Ntty =
		Case Ntty
			When 'CREDIT' Then 'C'
			When 'DEBIT'  Then 'D'
			When 'REFUND' Then 'R'
			Else Ntty
		End
		Where ( DocType In ('CDNR','CDNRA') )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set Docdt = convert(varchar,(SELECT convert(datetime, docdt, 103)),105)
		Where ( DocType In ('B2B','B2BA','CDNR','CDNRA') )
		And Ltrim(Rtrim(IsNull(Docdt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_GSTR6_RECS 
		Set Nt_dt = convert(varchar,(SELECT convert(datetime, nt_dt, 103)),105)
		Where ( DocType In ('CDNR','CDNRA') )
		And Ltrim(Rtrim(IsNull(Nt_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with different invoice value

		Select distinct t.doctype,t.gstin,t.fp,t.ctin,t.docnum,t.docdt,count(*) as cnt
		Into #TmpInvoices
		From
		(
			Select distinct doctype,gstin,fp,ctin,docnum,docdt,totval
			From #TBL_CSV_GSTR6_RECS
			Where DocType in ('B2B','B2BA')
			group by doctype,gstin,fp,ctin,docnum,docdt,totval
		 ) t
		group by doctype,gstin,fp,ctin,docnum,docdt
		having count(*) > 1

		If Exists(Select 1 From #TmpInvoices)
		Begin
			Update #TBL_CSV_GSTR6_RECS 
			Set ErrorCode = -41,
				ErrorMessage = 'Invoice Value is different for line items.'
			From #TBL_CSV_GSTR6_RECS t1,
				 #TmpInvoices t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And t1.fp = t2.fp
			And IsNull(t1.ctin,'') = IsNull(t2.ctin,'') 
			And t1.docnum = t2.docnum
			And t1.docdt = t2.docdt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices with item errors

		Select distinct doctype,gstin,ctin,docnum,docdt
			Into #TmpRecords
		From #TBL_CSV_GSTR6_RECS  
		Where DocType in ('B2B','B2BA','CDNR','CDNRA')
		And IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #TBL_CSV_GSTR6_RECS 
			Set ErrorCode = -42,
				ErrorMessage = 'Error in Invoice Item.'
			From #TBL_CSV_GSTR6_RECS t1,
				 #TmpRecords t2
			Where t1.DocType = t2.doctype
			And t1.gstin = t2.gstin
			And IsNull(t1.Ctin,'') = IsNull(t2.Ctin,'') 
			And t1.docnum = t2.docnum
			And t1.docdt = t2.docdt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR6_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR6_B2B_INV 
								Where gstin = t1.gstin
								And fp = t1.fp 
								And inum = t1.docnum 
								And idt = t1.docdt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR6_RECS t1
		Where DocType = 'B2B' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR6_B2B_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.docnum 
								And idt = t1.docdt
								And rowstatus <> 0)

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_CSV_GSTR6_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR6_CDN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_CSV_GSTR6_RECS 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_CSV_GSTR6_RECS t1
		Where DocType = 'CDNR' 
		And IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR6_CDN 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)
		
		-- Import B2B Invoices

		if Exists (Select 1 From #TBL_CSV_GSTR6_RECS Where DocType = 'B2B')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR6_RECS
							 Where DocType = 'B2B'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR6_B2B_INV t1,
							#TBL_CSV_GSTR6_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.inum = t2.docnum
					And t1.idt = t2.docdt
					And DocType = 'B2B'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR6_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR6_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR6_B2B_INV
				(	gstin, fp, ctin,inum, idt, val, pos,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, ctin,docnum, docdt,
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totval,''),0)))),
					statecd, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId

				From #TBL_CSV_GSTR6_RECS t1
				Where DocType = 'B2B' 
				And IsNull(ErrorCode,0) = 0
					
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR6_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR6_RECS t1
					Where DocType = 'B2B' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch

		End

		-- Import CDNR

		if Exists (Select 1 From #TBL_CSV_GSTR6_RECS Where DocType = 'CDNR')
		Begin

			Begin Try

				if Exists(Select 1 From #TBL_CSV_GSTR6_RECS
							 Where DocType = 'CDNR'
							 And IsNull(ErrorCode,0) = -101)
				Begin

					Delete t1
					From	TBL_EXT_GSTR6_CDN t1,
							#TBL_CSV_GSTR6_RECS t2
					Where t1.gstin = t2.gstin
					And t1.fp = t2.fp
					And	t1.nt_num = t2.nt_num
					And t1.nt_dt = t2.nt_dt
					And DocType = 'CDNR'
					And IsNull(ErrorCode,0) = -101
					
					Update #TBL_CSV_GSTR6_RECS 
					Set ErrorCode = Null,
						ErrorMessage = Null
					From #TBL_CSV_GSTR6_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = -101	

				End

				Insert TBL_EXT_GSTR6_CDN
				(	gstin, fp, ctin, ntty, nt_num, nt_dt, 
					inum, idt,val,  
					rt, txval, iamt, camt, samt, csamt,
					rowstatus, sourcetype, referenceno, createddate,errormessage,fileid)
				Select 
					gstin, fp, ctin,ntty,nt_num, nt_dt, 				
					docnum, docdt, 
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(rt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(txval,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(iamt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(camt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(samt,''),0)))),
					Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(csamt,''),0)))),
					1 ,@SourceType ,@ReferenceNo,GetDate(),ErrorMessage,@FileId

				 From #TBL_CSV_GSTR6_RECS t1
				Where DocType = 'CDNR' 
				And IsNull(ErrorCode,0) = 0
			
			End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_GSTR6_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Invoice Data'
					From #TBL_CSV_GSTR6_RECS t1
					Where DocType = 'CDNR' 
					And IsNull(ErrorCode,0) = 0
				End				
			End Catch
	
		End

	End 

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_CSV_GSTR6_RECS fv
	Where IsNull(fv.ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_CSV_GSTR6_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Update TBL_RECVD_FILES
	Set filestatus = 0,
		totalrecordscount = @TotalRecordsCount,
		processedrecordscount = @ProcessedRecordsCount,
		errorrecordscount = @ErrorRecordsCount
	Where fileid = @FileId

	Insert Into TBL_RECVD_FILE_ERRORS
	Select @FileId,fv.Slno,fv.ErrorCode,fv.ErrorMessage
	From  #TBL_CSV_GSTR6_RECS fv
	Where IsNull(fv.ErrorCode,0) <> 0

	Select * from #TBL_CSV_GSTR6_RECS
	Where IsNull(ErrorCode,0) <> 0

	--Format the Output to enable reupload of Error Records

	If @TemplateTypeId = 1
	Begin
		Select slno  As  [Slno],
		 doctype  As  [Document Type],
		 gstin  As  [GSTIN (Supplier's GSTIN)],
		 fp  As  [Period  (MMYYYY)],   
		 typ As [ Type (Type of user registered/unregistered)],
		 ctin  As  [Ctin (Customer Gstin)],
		 isd_docty As [ISD Document Type], 
		 docnum As [Invoice  / Document No.], 
		 docdt As [Invoice/Document  Date (DD-MM-YYYY)],
		 elglst As [Type of Available ITC],  
		 iamti As [IamtI (IGST used as IGST)],
		 iamts As [IamtS (SGST used as IGST)],
		 iamtc As [IamtC (CGST used as IGST)],
		 samts As [SamtS (SGST used as SGST)],
		 samti As [SamtI (IGST used as SGST)],
		 camti As [CamtI (IGST used as CGST)],
		 camtc As [CamtC (CGST used as CGST)],
		 rt As [GST Rate(%)],
		 txval As [Item Taxable Value (excluding tax)],
		 iamt As [IGST Amount],
		 camt As [CGST Amount],
		 samt As [SGST/UTGST Amount],
		 csamt As [Cess Amount (If applicable)],
		 totval As [Total Invoice  or Note or Refund voucher Value(including taxes)],
		 statecd As [Place Of Supply (two digit state code)],
		 ntty As [Note Type],
		 nt_num As [Note Number],
		 nt_dt As [Note Date (DD-MM-YYYY)],
		 '' As ' ',
		 rcpty As [Original Ctin (RevisedCustomer Gstin)],
		 odocnum As [ Original Invoice  / Document No.],
		 odocdt As [Original Invoice/Document  Date (DD-MM-YYYY)],
		 ont_num As [Original Note Number],
		 ont_dt As [Original Note Date (DD-MM-YYYY)],
		 rstatecd As [Original Place Of Supply (two digit state code)],
		 errorcode,
		 errormessage
		 From #TBL_CSV_GSTR6_RECS 
		 	Where IsNull(ErrorCode,0) <> 0
	End
	--Else
	--Begin

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

	 Drop Table #TBL_CSV_GSTR6_RECS
	-- Delete From TBL_CSV_GSTR6_RECS Where fileid = @FileId
	
	 Return 0


End