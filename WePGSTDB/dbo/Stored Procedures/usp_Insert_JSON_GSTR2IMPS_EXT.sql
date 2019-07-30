
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 IMPS JSON Records to the corresponding external tables
				
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

exec usp_Insert_JSON_GSTR2IMPS_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2IMPS_EXT]
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
			inum as inum,
			idt as idt,
			ival as val,
			pos as pos,
			txval as txval,
			rt as rt,
			iamt as iamt,
			csamt as csamt,
			elg as elg,
			tx_i as tx_i,
			tx_cs as tx_cs,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_IMPS_INV
	From OPENJSON(@RecordContents) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
        ival decimal(18,2),
		pos varchar(2),
    	itms nvarchar(max) as JSON
	) As Imps
	Cross Apply OPENJSON(Imps.itms)
	WITH
	(
		txval decimal(18,2),
		rt decimal(18,2),
		iamt decimal(18,2),
		csamt decimal(18,2),
		elg varchar(2),
		tx_i decimal(18,2),
		tx_cs decimal(18,2)
	) As Itms

	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_IMPS_INV

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_IMPS_INV)
	Begin

		Update #TBL_EXT_GSTR2_IMPS_INV -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_IMPS_INV  -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1


		Update #TBL_EXT_GSTR2_IMPS_INV
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Elg = Lower(Ltrim(Rtrim(IsNull(Elg,''))))

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV  
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Invoice No'
		Where dbo.udf_ValidateInvoiceNo(Inum) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV   
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Invoice Date'
		Where dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Rate'
		Where  dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV  
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Invoice Value'
		Where dbo.udf_ValidateInvoiceValue(Val) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV   
		Set ErrorCode = -7,
			ErrorMessage = 'Place of Supply is mandatory'
		Where Ltrim(Rtrim(IsNull(Pos,''))) In ('','0')
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Place Of Supply'
		Where dbo.udf_ValidatePlaceOfSupply(Pos) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid ITC Eligibility'
		Where dbo.udf_ValidateItcEligibility(Elg) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -10,
			ErrorMessage = dbo.udf_ValidateGstItcAmount('IMPS',Gstin,'',Pos,'',Rt,Txval,Iamt,'','',Csamt,Tx_I,'','',Tx_Cs,
								'','','',Inum,Idt,'') 
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -10
		And IsNull(ErrorMessage,'') = ''
	
		Update #TBL_EXT_GSTR2_IMPS_INV 
		Set Idt = convert(varchar,(SELECT convert(datetime, idt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Idt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices with item errors

		Select distinct gstin,inum,idt
			Into #TmpRecords
		From #TBL_EXT_GSTR2_IMPS_INV  
		Where IsNull(ErrorCode,0) <> 0

		If Exists(Select 1 From #TmpRecords)
		Begin
			Update #TBL_EXT_GSTR2_IMPS_INV  
			Set ErrorCode = -11,
				ErrorMessage = 'Error in Invoice Item.'
			From #TBL_EXT_GSTR2_IMPS_INV  t1,
				 #TmpRecords t2
			Where t1.gstin = t2.gstin
			And t1.inum = t2.inum
			And t1.idt = t2.idt
			And IsNull(ErrorCode,0) = 0
		End

		-- Tracking Invoices that are already Imported / Uploaded

		Update  #TBL_EXT_GSTR2_IMPS_INV  
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From  #TBL_EXT_GSTR2_IMPS_INV  t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus = 0)

		Update  #TBL_EXT_GSTR2_IMPS_INV 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From  #TBL_EXT_GSTR2_IMPS_INV t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPS_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And inum = t1.inum 
								And idt = t1.idt
								And rowstatus <> 0)

		-- Insert the Records Into External Tables

		Begin Try

			if Exists(Select 1 From #TBL_EXT_GSTR2_IMPS_INV 
							 Where IsNull(ErrorCode,0) = -101)
			Begin

				Delete t1
				From TBL_EXT_GSTR2_IMPS_INV t1,
					 #TBL_EXT_GSTR2_IMPS_INV t2
				Where t1.gstin = t2.gstin
				And t1.fp = t2.fp
				And	t1.inum = t2.inum
				And t1.idt = t2.idt
				And IsNull(ErrorCode,0) = -101
					
				Update #TBL_EXT_GSTR2_IMPS_INV 
				Set ErrorCode = Null,
					ErrorMessage = Null
				From #TBL_EXT_GSTR2_IMPS_INV t1
				Where IsNull(ErrorCode,0) = -101	

			End

			Insert TBL_EXT_GSTR2_IMPS_INV
			(	gstin, fp, inum, idt, pos, ival,  
				rt,txval,iamt,csamt,
				elg,tx_i,tx_cs,
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, inum, idt, pos, val,  
				rt,txval,iamt,csamt,
				elg,tx_i,tx_cs,
				1 ,@SourceType , @ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_IMPS_INV t1
			Where IsNull(ErrorCode,0) = 0
			
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_IMPS_INV  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR2_IMPS_INV t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch


	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_IMPS_INV  
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_IMPS_INV    
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_IMPS_INV  
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode


End