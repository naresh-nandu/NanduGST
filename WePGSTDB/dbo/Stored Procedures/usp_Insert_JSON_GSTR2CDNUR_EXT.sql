
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 CDNUR JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri		Initial Version
10/21/2017	Seshadri		Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
10/5/2017	Seshadri		Added the code to delete all existing invoices which are not uploaded
11/5/2017	Seshadri		Included Validation Framework


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2CDNUR_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2CDNUR_EXT]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out,
	@ErrorRecords nvarchar(max) = NULL out


  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,
			rtin as rtin,
			ntty as ntty,
			nt_num as nt_num,
			nt_dt as nt_dt,
			rsn as rsn,
			p_gst as p_gst,
			inum as inum,
			idt as idt,
			val as val,
			inv_typ as inv_typ,
			rt as rt,
			txval as txval,
			camt as camt,
			samt as samt,
			csamt as csamt,
			elg as elg,
			tx_s as tx_s,
			tx_c as tx_c,
			tx_cs as tx_cs,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_CDNUR
	From OPENJSON(@RecordContents) 
	WITH
	(
		rtin varchar(15),
		ntty varchar(1),
		nt_num varchar(50),
		nt_dt varchar(50),
		rsn varchar(50),
		p_gst varchar(1),
    	inum varchar(50),
		idt varchar(50),
		val decimal(18,2),
		inv_typ varchar(5),
     	itms nvarchar(max) as JSON
	) As Cdnur
	Cross Apply OPENJSON(Cdnur.itms)
	WITH
	(
		num int,
        itm_det nvarchar(max) as JSON,
		itc nvarchar(max) as JSON
	) As Itms
	Cross Apply OPENJSON(Itms.itm_det)
	WITH
	(
		rt decimal(18,2),
        txval decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
        csamt decimal(18,2)
	) As Itm_Det
	Cross Apply OPENJSON(Itms.itc)
	WITH
	(
		elg varchar(2),
		tx_s decimal(18,2),
		tx_c decimal(18,2),
		tx_cs decimal(18,2)
	) As Itc

	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_CDNUR

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_CDNUR)
	Begin

		Update #TBL_EXT_GSTR2_CDNUR -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_CDNUR
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Ntty = Upper(Ltrim(Rtrim(IsNull(Ntty,'')))),
			Elg = Lower(Ltrim(Rtrim(IsNull(Elg,''))))

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Invoice No'
		Where dbo.udf_ValidateInvoiceNo(Inum) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Invoice Date'
		Where dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR  
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice Type'
		Where	(	Ltrim(Rtrim(IsNull(Inv_Typ,''))) <> '' and 
					Inv_Typ Not In ('B2BUR','IMPS') 
				)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Rate'
		Where dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0

	
		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid Invoice Value'
		Where dbo.udf_ValidateInvoiceValue(Val) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -10,
			ErrorMessage = 'Invalid Note Type'
		Where dbo.udf_ValidateNoteType(Ntty) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -11,
			ErrorMessage = 'Invalid Note No'
		Where dbo.udf_ValidateNoteNo(Nt_Num) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid Note Date'
		Where dbo.udf_ValidateNoteDate(Nt_Dt,Fp,Idt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -13,
			ErrorMessage = 'Invalid Pre GST Regime'
		Where dbo.udf_ValidatePreGstRegime(P_Gst) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR  
		Set ErrorCode = -14,
			ErrorMessage = 'Invalid Receiver Gstin(Rtin)'
		Where ( Ltrim(Rtrim(IsNull(Rtin,''))) <> '' and
				dbo.udf_ValidateGstin(Rtin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR   
		Set ErrorCode = -15,
			ErrorMessage = 'Invalid ITC Eligibility'
		Where dbo.udf_ValidateItcEligibility(Elg) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set Ntty =
		Case Ntty
			When 'CREDIT' Then 'C'
			When 'DEBIT'  Then 'D'
			When 'REFUND' Then 'R'
			Else Ntty
		End
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
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
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -16,
			ErrorMessage = 'Invalid Reason for Issuing Dr./ Cr. Notes'
		Where dbo.udf_ValidateNoteRsn(rsn) <> 1
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR2_CDNUR 
		Set Idt = convert(varchar,(SELECT convert(datetime, idt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Idt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_CDNUR 
		Set Nt_dt = convert(varchar,(SELECT convert(datetime, nt_dt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Nt_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with item errors

		Select distinct gstin,inum,idt
			Into #TmpRecords
		From #TBL_EXT_GSTR2_CDNUR  
		Where IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #TBL_EXT_GSTR2_CDNUR 
			Set ErrorCode = -17,
				ErrorMessage = 'Error in Invoice Item.'
			From #TBL_EXT_GSTR2_CDNUR t1,
				 #TmpRecords t2
			Where t1.gstin = t2.gstin
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_EXT_GSTR2_CDNUR t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus = 0)

		Update #TBL_EXT_GSTR2_CDNUR 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_EXT_GSTR2_CDNUR t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR1_CDNUR 
								Where gstin = t1.gstin 
								And fp = t1.fp 
								And nt_num = t1.nt_num 
								And nt_dt = t1.nt_dt
								And rowstatus <> 0)

		-- Insert the Records Into External Tables

		Begin Try

			if Exists(Select 1 From #TBL_EXT_GSTR2_CDNUR 
							 Where IsNull(ErrorCode,0) = -101)
	
			Begin

				Delete t1
				From	TBL_EXT_GSTR2_CDNUR t1,
						#TBL_EXT_GSTR2_CDNUR t2
				Where t1.gstin = t2.gstin
				And t1.fp = t2.fp
				And	t1.nt_num = t2.nt_num
				And t1.nt_dt = t2.nt_dt
				And IsNull(ErrorCode,0) = -101

				Update #TBL_EXT_GSTR2_CDNUR 
				Set ErrorCode = Null,
					ErrorMessage = Null
				From #TBL_EXT_GSTR2_CDNUR t1
				Where IsNull(ErrorCode,0) = -101	

			End

			Insert TBL_EXT_GSTR2_CDNUR
			(	gstin, fp, rtin,ntty,nt_num,nt_dt,
				rsn,p_gst,
				inum, idt,inv_typ,val,   
				txval, camt, samt, csamt,rt,
				tx_c,tx_s,tx_cs,elg,
				rowstatus, sourcetype, referenceno, createddate)
			Select 
				gstin, fp, rtin,ntty,nt_num,nt_dt,
				rsn,p_gst,
				inum, idt,inv_typ,val,     
				txval, camt, samt, csamt,rt,
				tx_c,tx_s,tx_cs,elg,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_CDNUR t1
			Where IsNull(ErrorCode,0) = 0
				 
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_CDNUR 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR2_CDNUR t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch
	
	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_CDNUR
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_CDNUR  
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_CDNUR
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode


End