
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the CSV GSTR1 B2CS Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/16/2017	Seshadri		Initial Version
08/08/2017	Seshadri		Introduced Update Stmt to replace @ character with empty string
							for Gstin,Ctin and Etin


*/

/* Sample Procedure Call

exec usp_Insert_CSV_GSTR1B2CS_EXT  'BP','BP001','BP,BPDV100,13AMGOH2199X398,062017,13AMGOK2199X399,INV001,01-06-2017,16000,13,N,,1,5,800,,800,800'

 */

CREATE PROCEDURE [usp_Insert_CSV_GSTR1B2CS_EXT]
	@SourceType varchar(15), -- BP ; POS 
	@ReferenceNo varchar(50),
	@RecordContents nvarchar(max),
	@ErrorCode smallint = Null out,
	@ErrorMessage varchar(255) = Null out
  
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)

	Create Table #GstrRecordContents
	( 
		RecordContents nvarchar(max)
	)		

	Create Table #GstrRecordFldValues
	(	
		sourcetype varchar(50),
		referenceno varchar(50),
		gstin varchar(50),
		fp varchar(50),
		sply_ty varchar(50),
		etin varchar(50),
		pos varchar(50),
		hsncode varchar(50),
		hsndesc varchar(50),
		qty varchar(50),
		unitprice varchar(50),
		discount varchar(50),
		rt varchar(50),
		txval varchar(50),
		iamt varchar(50),
		camt varchar(50),
		samt varchar(50),
		csamt varchar(50)
	 )

	Select @Delimiter = ','

	 Begin Try

		Insert Into #GstrRecordContents
		Select @RecordContents

		If @SourceType = 'BP'
		Begin

			Insert Into #GstrRecordFldValues
			(	sourcetype,
				referenceno,
				gstin,
				fp,
				sply_ty,
				etin,
				pos,
				hsncode,
				hsndesc,
				qty,
				unitprice,
				discount,
				rt,
				txval,
				iamt,
				camt,
				samt,
				csamt
			)			
			Select 
				dbo.udf_SplitGSTRRecordData(RecordContents, 2,@Delimiter) as Sourcetype ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 3,@Delimiter) as Referenceno ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 4,@Delimiter) as Gstin ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 5,@Delimiter) as Fp ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 6,@Delimiter) as Sply_Ty,
				dbo.udf_SplitGSTRRecordData(RecordContents, 7,@Delimiter) as Etin,
				dbo.udf_SplitGSTRRecordData(RecordContents, 8,@Delimiter) as Pos,
				dbo.udf_SplitGSTRRecordData(RecordContents, 9,@Delimiter) as Hsncode ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 10,@Delimiter) as Hsndesc,
				dbo.udf_SplitGSTRRecordData(RecordContents, 11,@Delimiter) as Qty,
				dbo.udf_SplitGSTRRecordData(RecordContents, 12,@Delimiter) as Unitprice  ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 13,@Delimiter) as Discount,
				dbo.udf_SplitGSTRRecordData(RecordContents, 14,@Delimiter) as Rt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 15,@Delimiter) as Txval,
				dbo.udf_SplitGSTRRecordData(RecordContents, 16,@Delimiter) as Iamt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 17,@Delimiter) as Camt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 18,@Delimiter) as Samt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 19,@Delimiter) as Csamt
			From #GstrRecordContents

		End
	


	End Try
	Begin Catch

		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select	@ErrorCode = -1,
					@ErrorMessage = ERROR_MESSAGE()
			Return @ErrorCode
		End				

	End Catch

	-- Insert Records into Table TBL_EXT_GSTR1_B2CS

	if exists (Select 1 from #GstrRecordFldValues)
	Begin
		Update #GstrRecordFldValues
		Set Gstin = REPLACE(IsNull(Gstin,''),'@','') , 
			Etin = REPLACE(IsNull(Etin,''),'@','')
	End


	Begin Try

		Insert TBL_EXT_GSTR1_B2CS
			(	gstin, fp, gt, cur_gt, sply_ty,txval,
				typ,etin, pos, 
				hsncode,hsndesc,qty,unitprice,discount, 
				rt, iamt,camt,samt,csamt,
				rowstatus, sourcetype, referenceno, createddate,errormessage)
		Select 
			gstin, fp, null, null, sply_ty,txval,
			'OE',etin, pos, 
			hsncode,hsndesc,qty,unitprice,discount, 
			rt, iamt,camt,samt,csamt,
			1 ,@SourceType ,@ReferenceNo,GetDate(),null
		From #GstrRecordFldValues t1

	End Try
	Begin Catch

		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			Select	@ErrorCode = -2,
					@ErrorMessage = ERROR_MESSAGE()
			Return @ErrorCode
		End	
					
	End Catch
		
	
	-- Drop Temp Tables

	Drop Table #GstrRecordContents
	Drop Table #GstrRecordFldValues
		
	Select @ErrorCode = 1, @ErrorMessage = 'Success'

	Return @ErrorCode
End