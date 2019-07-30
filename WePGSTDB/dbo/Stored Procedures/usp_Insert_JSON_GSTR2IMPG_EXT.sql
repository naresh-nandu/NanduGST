
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 IMPG JSON Records to the corresponding external tables
				
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

exec usp_Insert_JSON_GSTR2IMPG_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2IMPG_EXT]
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
			is_sez as is_sez ,
			stin as stin,
			boe_num as boe_num,
			boe_dt as boe_dt,
			boe_val as boe_val,
			port_code as port_code,
			txval as txval,
			rt as rt,
			iamt as iamt,
			csamt as csamt,
			elg as elg,
			tx_i as tx_i,
			tx_cs as tx_cs,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_IMPG_INV
	From OPENJSON(@RecordContents) 
	WITH
	(
		is_sez varchar(2),
		stin varchar(15),
		boe_num varchar(50),
		boe_dt varchar(50),
		boe_val decimal(18,2),
		port_code varchar(6),
		itms nvarchar(max) as JSON
	) As Impg
	Cross Apply OPENJSON(Impg.itms)
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

	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_IMPG_INV

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_IMPG_INV)
	Begin

		Update #TBL_EXT_GSTR2_IMPG_INV -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_IMPG_INV
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Elg = Lower(Ltrim(Rtrim(IsNull(Elg,''))))

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Rate'
		Where  dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Elg Of Sez'
		Where dbo.udf_ValidateIsSEZ(is_sez) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Stin(Sez Gstin)'
		Where ( Ltrim(Rtrim(IsNull(is_sez,''))) = 'Y' and
				dbo.udf_ValidateGstin(Stin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV  
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Bill of Entry Number'
		Where dbo.udf_ValidateBOENo(boe_num) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV  
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid Bill Of Entry Date'
		Where dbo.udf_ValidateDate(boe_dt) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV  
		Set ErrorCode = -10,
			ErrorMessage = 'Invalid Bill of Entry Value'
		Where dbo.udf_ValidateAmount(boe_val) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV  
		Set ErrorCode = -11,
			ErrorMessage = 'Invalid Port Code'
		Where dbo.udf_ValidatePortCode(port_code) <> 1 
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -12,
			ErrorMessage = 'Invalid ITC Eligibility'
		Where dbo.udf_ValidateItcEligibility(Elg) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -13,
			ErrorMessage = dbo.udf_ValidateGstItcAmount('IMPG',Gstin,'','','',Rt,Txval,Iamt,'','',Csamt,Tx_I,'','',Tx_Cs,
								'','','','','','') 
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -13
		And IsNull(ErrorMessage,'') = ''
	
	
		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set Boe_dt = convert(varchar,(SELECT convert(datetime, boe_dt, 103)),105)
		Where Ltrim(Rtrim(IsNull(Boe_dt,''))) <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Tracking Invoices that are already Imported / Uploaded

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -100,
			ErrorMessage = 'Invoice data already uploaded'
		From #TBL_EXT_GSTR2_IMPG_INV t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPG_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And boe_num = t1.boe_num 
								And boe_dt = t1.boe_dt
								And rowstatus = 0)

		Update #TBL_EXT_GSTR2_IMPG_INV 
		Set ErrorCode = -101,
			ErrorMessage = 'Invoice data already imported'
		From #TBL_EXT_GSTR2_IMPG_INV t1
		Where IsNull(ErrorCode,0) = 0
		And Exists(Select 1 From  TBL_EXT_GSTR2_IMPG_INV 
								Where gstin = t1.gstin 
								And fp = t1.fp
								And boe_num = t1.boe_num 
								And boe_dt = t1.boe_dt
								And rowstatus <> 0)

		-- Insert the Records Into External Tables

		Begin Try

			if Exists(Select 1 From #TBL_EXT_GSTR2_IMPG_INV 
							 Where IsNull(ErrorCode,0) = -101)
			Begin

				Delete t1
				From TBL_EXT_GSTR2_IMPG_INV t1,
					 #TBL_EXT_GSTR2_IMPG_INV t2
				Where t1.gstin = t2.gstin
				And t1.fp = t2.fp
				And	t1.boe_num = t2.boe_num
				And t1.boe_dt = t2.boe_dt
				And IsNull(ErrorCode,0) = -101
					
				Update #TBL_EXT_GSTR2_IMPG_INV 
				Set ErrorCode = Null,
					ErrorMessage = Null
				From #TBL_EXT_GSTR2_IMPG_INV t1
				Where IsNull(ErrorCode,0) = -101	

			End

			Insert TBL_EXT_GSTR2_IMPG_INV
			(	gstin, fp, is_sez,stin,boe_num,boe_dt,boe_val,port_code,  
				rt,txval,iamt,csamt,
				elg,tx_i,tx_cs,
				rowstatus, sourcetype, referenceno, createddate
			)
			Select 
				gstin, fp, is_sez,stin,boe_num,boe_dt,boe_val,port_code,  
				rt,txval,iamt,csamt,
				elg,tx_i,tx_cs,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			 From #TBL_EXT_GSTR2_IMPG_INV t1
			Where IsNull(ErrorCode,0) = 0
			
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_IMPG_INV  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR2_IMPG_INV t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch


	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_IMPG_INV  
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_IMPG_INV    
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_IMPG_INV  
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode




End