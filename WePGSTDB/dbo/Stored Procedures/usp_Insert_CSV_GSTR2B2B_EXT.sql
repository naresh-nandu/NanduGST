
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to Insert the CSV GSTR2 B2B Records to the corresponding external tables
				
Written by  : Sheshadri.mk@wepdigital.com 

Date		Who			Decription 
06/16/2017	Seshadri			Initial Version
07/19/2017	Seshadri		Modified the Where Cluase to include hsncode,rt & txval


*/

/* Sample Procedure Call

exec usp_Insert_CSV_GSTR2B2B_EXT  'BP','BP001','BP,BPDV100,13AMGOH2199X398,062017,13AMGOK2199X399,INV001,01-06-2017,16000,13,N,,1,5,800,,800,800'

 */

CREATE PROCEDURE [usp_Insert_CSV_GSTR2B2B_EXT]
	@SourceType varchar(15), -- BP 
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
		ctin varchar(50),
		inum varchar(50),
		idt varchar(50),
		val varchar(50),
		pos varchar(50),
		rchrg varchar(50),
		inv_typ varchar(50),
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
				ctin,
				inum,
				idt,
				val,
				pos,
				rchrg,
				inv_typ,
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
				dbo.udf_SplitGSTRRecordData(RecordContents, 6,@Delimiter) as Ctin,
				dbo.udf_SplitGSTRRecordData(RecordContents, 7,@Delimiter) as Inum,
				dbo.udf_SplitGSTRRecordData(RecordContents, 8,@Delimiter) as Idt ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 9,@Delimiter) as Val,
				dbo.udf_SplitGSTRRecordData(RecordContents, 10,@Delimiter) as Pos,
				dbo.udf_SplitGSTRRecordData(RecordContents, 11,@Delimiter) as Rchrg  ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 12,@Delimiter) as Inv_Typ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 13,@Delimiter) as Hsncode ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 14,@Delimiter) as Hsndesc,
				dbo.udf_SplitGSTRRecordData(RecordContents, 15,@Delimiter) as Qty,
				dbo.udf_SplitGSTRRecordData(RecordContents, 16,@Delimiter) as Unitprice  ,
				dbo.udf_SplitGSTRRecordData(RecordContents, 17,@Delimiter) as Discount,
				dbo.udf_SplitGSTRRecordData(RecordContents, 18,@Delimiter) as Rt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 19,@Delimiter) as Txval,
				dbo.udf_SplitGSTRRecordData(RecordContents, 20,@Delimiter) as Iamt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 21,@Delimiter) as Camt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 22,@Delimiter) as Samt,
				dbo.udf_SplitGSTRRecordData(RecordContents, 23,@Delimiter) as Csamt
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

	-- Insert Records into Table TBL_EXT_GSTR2_B2B_INV

	Begin Try
		Insert TBL_EXT_GSTR2_B2B_INV
		(	gstin, fp, ctin,inum, idt, val, pos, rchrg, inv_typ, 
			hsncode,hsndesc,qty,unitprice,discount,
			rt, txval, iamt, camt, samt, csamt,
			rowstatus, sourcetype, referenceno, createddate,errormessage)
		Select 
			gstin, fp, ctin,inum, idt, val, pos, rchrg, inv_typ, 
			hsncode,hsndesc,qty,unitprice,discount,
			rt, txval, iamt, camt, samt, csamt,
			1, @SourceType ,@ReferenceNo,GetDate(),null
		From #GstrRecordFldValues t1
		Where Not Exists(Select 1 From  TBL_EXT_GSTR2_B2B_INV 
							Where gstin = t1.gstin 
							And inum = t1.inum
							And idt = t1.idt
							And hsncode = t1.hsncode
							And rt = t1.rt
							And txval = t1.txval)
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