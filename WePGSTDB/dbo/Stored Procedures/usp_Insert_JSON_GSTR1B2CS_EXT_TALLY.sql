
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR1 B2CS JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/20/2017	Seshadri			Initial Version
10/8/2017	Karthik			Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
11/5/2017	Seshadri		Included Validation Framework

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR1B2CS_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR1B2CS_EXT_TALLY]
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
			sply_ty as sply_ty,
			txval as txval,
			typ as typ,
			etin as etin,
			pos as pos,
			rt as rt,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR1_B2CS_INV
	From OPENJSON(@RecordContents) 
	WITH
	(	sply_ty varchar(5),
		txval decimal(18,2),
		typ varchar(2),
		etin varchar(15),
		pos varchar(2),
		rt decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As B2cs
	
	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR1_B2CS_INV

	-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR1_B2CS_INV)
	Begin

		Update #TBL_EXT_GSTR1_B2CS_INV -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR1_B2CS_INV  -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #TBL_EXT_GSTR1_B2CS_INV
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,'')))), 
			Etin = Upper(Ltrim(Rtrim(IsNull(Etin,''))))

		Update #TBL_EXT_GSTR1_B2CS_INV
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
	
		/*
		Update #TBL_EXT_GSTR1_B2CS_INV 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
							Where t1.custid = (Select custid From Userlist where email = @UserId)
							And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR1_B2CS_INV 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0


		Update #TBL_EXT_GSTR1_B2CS_INV
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Ecommerce Gstin'
		Where (Ltrim(Rtrim(IsNull(Etin,''))) <> '' and
			   dbo.udf_ValidateGstin(Etin) <> 1)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV  
		Set ErrorCode = -5,
			ErrorMessage = 'Invalid Rate'
		Where dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV  
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Taxable Value'
		Where dbo.udf_ValidateTaxableValue(Txval) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV   
		Set ErrorCode = -7,
			ErrorMessage = 'Place of Supply is mandatory'
		Where Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV  
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Place Of Supply'
		Where dbo.udf_ValidatePlaceOfSupply(Pos) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV   
		Set ErrorCode = -9,
			ErrorMessage = 'Invalid Supply Type'
		Where dbo.udf_ValidateSupplyType(Sply_Ty,Gstin,Pos) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV   
		Set ErrorCode = -10,
			ErrorMessage = dbo.udf_ValidateGstAmount('B2CS',Gstin,'',Etin,Pos,Rt,Txval,Iamt,CAmt,Samt,Csamt,'','','','','','') 
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV   
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -10
		And IsNull(ErrorMessage,'') = ''

		Update #TBL_EXT_GSTR1_B2CS_INV    
		Set typ = 'OE'
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR1_B2CS_INV    
		Set typ = 'E'
		Where IsNull(Etin,'') <> '' 
		And IsNull(ErrorCode,0) = 0

		-- Insert the Records Into External Tables

		Begin Try

			Insert TBL_EXT_GSTR1_B2CS
			(	gstin, fp, gt, cur_gt, sply_ty,txval,typ,etin, pos,  
				rt, iamt,camt,samt,csamt,
				rowstatus, sourcetype, referenceno, createddate)
			Select 
				gstin, fp, gt, cur_gt, sply_ty,txval,typ,etin, pos,  
				rt, iamt,camt,samt,csamt,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR1_B2CS_INV t1
			Where IsNull(ErrorCode,0) = 0
			
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR1_B2CS_INV  
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR1_B2CS_INV t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch

	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_B2CS_INV
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR1_B2CS_INV  
	Where IsNull(ErrorCode,0) <> 0
		if @ErrorRecordsCount > 0
		Begin 
				Select 
				sply_ty as [Type],	
				pos as [Place Of Supply],	
				rt as [Rate],	
				txval as [Taxable Value],	
				csamt as [Cess Amount],	
				etin as [E-Commerce GSTIN],
				ErrorCode,
				ErrorMessage
				From #TBL_EXT_GSTR1_B2CS_INV  Where IsNull(ErrorCode,0) <> 0
		End
		Else
		Begin
			Select 
				sply_ty as [Type],	
				pos as [Place Of Supply],	
				rt as [Rate],	
				txval as [Taxable Value],	
				csamt as [Cess Amount],	
				etin as [E-Commerce GSTIN]
				From #TBL_EXT_GSTR1_B2CS_INV  Where IsNull(ErrorCode,0) <> 0
		End
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode

End