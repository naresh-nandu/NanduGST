﻿
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the GSTR2 TXI JSON Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/21/2017	Seshadri		Initial Version
10/21/2017	Seshadri		Modified to include the below mentioned Output parameters to return to generic procedure
							@TotalRecordsCount int = Null out,
							@ProcessedRecordsCount int = Null out,
							@ErrorRecordsCount int = Null out
11/5/2017	Seshadri		Included Validation Framework

*/

/* Sample Procedure Call

exec usp_Insert_JSON_GSTR2TXI_EXT  'POS',
 */

CREATE PROCEDURE [usp_Insert_JSON_GSTR2TXI_EXT]
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
			pos as pos,
			sply_ty as sply_ty,
			rt as rt,
			adamt as adamt,
			iamt as iamt,
			camt as camt,
			samt as samt,
			csamt as csamt,
			space(255) as errormessage,
			space(10) as errorcode
	Into #TBL_EXT_GSTR2_TXI
	From OPENJSON(@RecordContents) 
	WITH
	(
		pos varchar(5),
		sply_ty varchar(5),
		itms nvarchar(max) as JSON
	) As Txi
	Cross Apply OPENJSON(Txi.itms)
	WITH
	(
		rt decimal(18,2),
		adamt decimal(18,2),
		iamt decimal(18,2),
		camt decimal(18,2),
		samt decimal(18,2),
		csamt decimal(18,2)
	) As Itms

	Select @TotalRecordsCount = count(*) from #TBL_EXT_GSTR2_TXI

		-- Validation Framework

	if Exists(Select 1 From #TBL_EXT_GSTR2_TXI)
	Begin

		Update #TBL_EXT_GSTR2_TXI -- Introduced due to Excel Format Issue
		Set Fp = '0' + Ltrim(Rtrim(IsNull(Fp,'')))
		Where Len(Ltrim(Rtrim(IsNull(Fp,'')))) = 5

		Update #TBL_EXT_GSTR2_TXI -- Introduced due to Excel Format Issue
		Set Pos = '0' + Ltrim(Rtrim(IsNull(Pos,'')))
		Where Len(Ltrim(Rtrim(IsNull(Pos,'')))) = 1

		Update #TBL_EXT_GSTR2_TXI
		Set Gstin = Upper(Ltrim(Rtrim(IsNull(Gstin,''))))

	
		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -1,
			ErrorMessage = 'Invalid Gstin'
		Where dbo.udf_ValidateGstin(Gstin) <> 1
		And IsNull(ErrorCode,0) = 0
		
		/*
		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -2,
			ErrorMessage = 'Gstin is not registered'
		Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = (Select custid From Userlist where email = @UserId)
						And t1.GstinNo = Gstin) 
		And IsNull(ErrorCode,0) = 0 */

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -3,
			ErrorMessage = 'Invalid Period'
		Where dbo.udf_ValidatePeriod(Fp) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -4,
			ErrorMessage = 'Invalid Rate'
		Where dbo.udf_ValidateRate(Rt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -5,
			ErrorMessage = 'Place of Supply is mandatory'
		Where Ltrim(Rtrim(IsNull(Pos,'0'))) = '0'
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -6,
			ErrorMessage = 'Invalid Place Of Supply'
		Where dbo.udf_ValidatePlaceOfSupply(Pos) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -7,
			ErrorMessage = 'Invalid Supply Type'
		Where	(	Ltrim(Rtrim(IsNull(Sply_Ty,''))) = '' Or 
					Sply_Ty Not In ('INTER','INTRA') 
				)
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -8,
			ErrorMessage = 'Invalid Gross Advance Paid'
		Where dbo.udf_ValidateAmount(Adamt) <> 1
		And IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = -9,
			ErrorMessage = dbo.udf_ValidateGstItcAmount('TXI',Gstin,'',Pos,Sply_Ty,Rt,'',Iamt,CAmt,Samt,Csamt,'','','','',
								'',Adamt,'','','','') 
	
		Where IsNull(ErrorCode,0) = 0

		Update #TBL_EXT_GSTR2_TXI 
		Set ErrorCode = null,
			ErrorMessage = null 
		Where IsNull(ErrorCode,0) = -9
		And IsNull(ErrorMessage,'') = ''

		-- Insert the Records Into External Tables

		Begin Try

			Insert TBL_EXT_GSTR2_TXI
			(	gstin, fp, pos, sply_ty,   
				rt, adamt,camt, samt, csamt,iamt, 
				rowstatus, sourcetype, referenceno, createddate)
			Select 
				gstin, fp, pos, sply_ty,   
				rt, adamt,camt, samt, csamt,iamt,
				1 ,@SourceType ,@ReferenceNo,GetDate()
			From #TBL_EXT_GSTR2_TXI t1
			Where IsNull(ErrorCode,0) = 0
	
		End Try
		Begin Catch
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				Update #TBL_EXT_GSTR2_TXI 
				Set ErrorCode = -102,
					ErrorMessage = 'Error in Invoice Data'
				From #TBL_EXT_GSTR2_TXI t1
				Where IsNull(ErrorCode,0) = 0
			End				
		End Catch
	
	End

	Select @ProcessedRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_TXI
	Where IsNull(ErrorCode,0) = 0

	Select @ErrorRecordsCount = Count(*)
	From #TBL_EXT_GSTR2_TXI  
	Where IsNull(ErrorCode,0) <> 0

	Select @ErrorRecords = (Select * From #TBL_EXT_GSTR2_TXI
							Where IsNull(ErrorCode,0) <> 0
							FOR JSON AUTO)
	

	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode

	
End