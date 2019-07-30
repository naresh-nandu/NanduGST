
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 EXP JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
8/8/2017	Seshadri		Modified to include the column sbpcode
10/8/2017	Karthik			Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
09/26/2017	Seshadri		Added the code to delete all existing invoices which are not uploaded
11/5/2017	Seshadri		Included Validation Framework


*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1EXP_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1EXP_EXT_TALLY]
	@SourceType varchar(15), 
	@ReferenceNo varchar(50),
	@Gstin varchar(15),
	@Fp varchar(10),
	@Gt decimal(18,2),
	@Cur_Gt decimal(18,2),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
	--@ErrorRecords nvarchar(max) = NULL out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Select	@Gstin as gstin,
			@Fp as fp,
			@Gt as gt,
			@Cur_Gt as cur_gt,
			exp_typ as exp_typ,
			inum as inum,
			idt as idt,
			val as val,
			sbpcode as sbpcode,
			sbnum as sbnum,
			sbdt as sbdt,
			txval as txval,
			rt as rt,
			iamt as iamt,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR1_EXP_INV
	From OPENJSON(@RecordContents) 
	WITH
	(
		exp_typ varchar(15),
		inv nvarchar(max) as JSON
	) As Exps
	Cross Apply OPENJSON(Exps.inv) 
	WITH
	(
		inum varchar(50),
		idt varchar(50),
        val decimal(18,2),
		sbpcode varchar(6),
		sbnum varchar(50),
		sbdt varchar(50),
    	itms nvarchar(max) as JSON
	) As Inv
	Cross Apply OPENJSON(Inv.itms)
	WITH
	(
		txval decimal(18,2),
		rt decimal(18,2),
		iamt decimal(18,2)
	) As Itm_det

	
	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR1_EXP_INV

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR1_EXP_INV)
	Begin

		Update #TBL_EXT_GSTR1_EXP_INV -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR1_EXP_INV
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

		Update #TBL_EXT_GSTR1_EXP_INV
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
	
		/*
		Update #TBL_EXT_GSTR1_EXP_INV 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
							Where t1.custid = (Select custid From Userlist where email = @UserId)
							And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR1_EXP_INV 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Invoice No'
		Where dbo.udf_ValidateInvoiceNo(Inum) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Invoice Date'
		Where dbo.udf_ValidateInvoiceDate(Idt,Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV  
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Rate'
		Where dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV  
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR1_EXP_INV  
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Invoice Value'
		Where dbo.udf_ValidateInvoiceValue(Val) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV   
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid Export Type'
		Where dbo.udf_ValidateExportType(Exp_Typ) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV 
		Set ErrorCode = -10,
			ErrorMessage = dbo.udf_ValidateGstAmount('EXP',Gstin,'','','',Rt,Txval,Iamt,'','','','','','',Inum,Idt,'') 
		Where IsNull(Exp_Typ,'') = 'WPAY' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV   
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -10
		And IsNull(ErrorMessage,'') = ''

		Update #TBL_EXT_GSTR1_EXP_INV    
		Set ErrorCode = -11,
			ErrorMessage = 'Invalid Shipping Bill Date'
		Where (	Ltrim(Rtrim(IsNull(Sbdt,''))) <> ''  and 
				dbo.udf_ValidateDate(Sbdt) <> 1 )
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV   
		Set Idt = convert(varchar,(SELECT convert(datetime, idt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Idt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_EXP_INV 
		Set Sbdt = convert(varchar,(SELECT convert(datetime, sbdt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Sbdt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices that are already Imported / Uploaded

		--Update #TBL_EXT_GSTR1_EXP_INV
		--Set ErrorCode = -100,
		--	ErrorMessage = 'Invoice data already uploaded'
		--From  #TBL_EXT_GSTR1_EXP_INV t1
		--Where IsNull(ErrorCode,0) = 0
		--And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
		--						Where gstin = t1.gstin 
		--						And fp = t1.fp 
		--						And inum = t1.inum 
		--						And idt = t1.idt
		--						And rowstatus = 0)

		--Update  #TBL_EXT_GSTR1_EXP_INV 
		--Set ErrorCode = -101,
		--	ErrorMessage = 'Invoice data already imported'
		--From  #TBL_EXT_GSTR1_EXP_INV t1
		--Where IsNull(ErrorCode,0) = 0
		--And Exists(Select 1 From  TBL_EXT_GSTR1_EXP_INV 
		--						Where gstin = t1.gstin 
		--						And fp = t1.fp 
		--						And inum = t1.inum 
		--						And idt = t1.idt
		--						And rowstatus <> 0)

		-- Insert the Records Into External Tables

		Begin Try

			if Exists(Select 1 From #TBL_EXT_GSTR1_EXP_INV 
							 Where IsNull(ErrorCode,0) = -101)
			Begin

				Delete t1
				From	TBL_EXT_GSTR1_EXP_INV t1,
						#TBL_EXT_GSTR1_EXP_INV t2
				Where t1.gstin = t2.gstin
				And t1.fp = t2.fp
				And	t1.inum = t2.inum
				And t1.idt = t2.idt
				And IsNull(ErrorCode,0) = -101
					
				Update #TBL_EXT_GSTR1_EXP_INV 
				Set ErrorCode = Null,
					ErrorMessage = Null
				From #TBL_EXT_GSTR1_EXP_INV t1
				Where IsNull(ErrorCode,0) = -101	

			End

			Insert TBL_EXT_GSTR1_EXP_INV
			(	gstin, fp, gt, cur_gt,exp_typ,inum, idt, val,
				sbpcode,sbnum,sbdt,  
				txval,rt,iamt,
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, gt, cur_gt,exp_typ,inum, idt, val,
				sbpcode,sbnum,sbdt, 
				txval,rt,iamt,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			 From #TBL_EXT_GSTR1_EXP_INV t1
			Where IsNull(ErrorCode,0) = 0
			
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR1_EXP_INV  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR1_EXP_INV t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_EXP_INV
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_EXP_INV  
	Where IsNull(ErrorCode,0) <> 0

	if @ErrorRecordsCount > 0
	Begin
		Select 
		exp_typ as [Export Type],	
		inum as [Invoice Number],	
		idt as [Invoice date],	
		val as [Invoice Value],	
		sbpcode as [Port Code],	
		sbnum as [Shipping Bill Number],	
		sbdt as [Shipping Bill Date],	
		rt as [Rate],	
		txval as [Taxable Value],
		ErrorCode,
		ErrorMessage
		From #TBL_EXT_GSTR1_EXP_INV  Where IsNull(ErrorCode,0) <> 0
	End
	Else
	Begin
		Select 
		exp_typ as [Export Type],	
		inum as [Invoice Number],	
		idt as [Invoice date],	
		val as [Invoice Value],	
		sbpcode as [Port Code],	
		sbnum as [Shipping Bill Number],	
		sbdt as [Shipping Bill Date],	
		rt as [Rate],	
		txval as [Taxable Value]
		From #TBL_EXT_GSTR1_EXP_INV  Where IsNull(ErrorCode,0) <> 0
	End

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode

End