
/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to upload Master Data From CSV File
				
Written by  : Sheshadri.mk@wepdigital.com & Karthik.kanniyappan@wepindia.com

Date		Who					Decription 
06/12/2017	Seshadri/Karthik	Initial Version
*/

/* Sample Procedure Call

exec usp_Import_CSV_Master 94,40,'HSN','C:\inetpub\wwwroot\GSPtest\App_Data\uploads\HSN Template-Copy2_20170719190548341.csv' 

 */
 
Create PROCEDURE [usp_Import_CSV_Master]  
	@UserId int,
	@custId int,
	@MasterType varchar(255),
	@FileName varchar(max),
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1)
	Select @Delimiter = ','

	Select Uploadcontent Into #MasterRecordValues
	From TBL_TMP_CSV_UploadMaster 
	Where CustomerId = @custId
		And CreatedBy = @UserId 
		And MasterType = @MasterType
		And FileName = @FileName
		And rowstatus = 1


	if @MasterType <> 'HSN'
	Begin

		Create Table #MasterRecordFieldValues
		(
			slno varchar(50),
			SupplierName varchar(255),
			POCName varchar(255),
			NatureOfBusiness varchar(255),
			EmailId varchar(150),
			MobileNo varchar(50),
			GSTINno varchar(50),
			StateCode varchar(5),
			PANNO varchar(50),
			DateofCompRegistered varchar(50),
			ConstitutionOfBusiness varchar(250),
			Address varchar(250),
			errorcode smallint,
			errormessage varchar(255)
		 )
 
		Begin Try

			Insert Into #MasterRecordFieldValues
			Select 
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 1,@Delimiter) as Slno ,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 2,@Delimiter) as SupplierName,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 3,@Delimiter) as POCName,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 4,@Delimiter) as NatureOfBusiness,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 5,@Delimiter) as EmailId ,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 6,@Delimiter) as MobileNo,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 7,@Delimiter) as GSTINno,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 8,@Delimiter) as StateCode,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 9,@Delimiter) as PANNO,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 10,@Delimiter) as DateofCompRegistered ,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 11,@Delimiter) as ConstitutionOfBusiness,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 12,@Delimiter) as Address,
			
				null,
				null  
			From #MasterRecordValues

		End Try
		Begin Catch
	 
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				select error_message()
			End				
	
		End Catch

		-- Processing Logic

		if exists (Select 1 from #MasterRecordFieldValues)
		Begin
		
			if @MasterType = 'Supplier'
			Begin

				Begin Try

					Delete From #MasterRecordFieldValues
					Where Ltrim(Rtrim(IsNull(SupplierName,''))) = ''
					And Ltrim(Rtrim(IsNull(POCName,''))) = ''
					And Ltrim(Rtrim(IsNull(NatureOfBusiness,''))) = ''
					And Ltrim(Rtrim(IsNull(EmailId,''))) = ''
					And Ltrim(Rtrim(IsNull(MobileNo,''))) = ''
					And Ltrim(Rtrim(IsNull(GSTINno,''))) = ''
					And Ltrim(Rtrim(IsNull(StateCode,''))) = ''
					And Ltrim(Rtrim(IsNull(PANNO,''))) = ''
					And Ltrim(Rtrim(IsNull(DateofCompRegistered,''))) = ''
					And Ltrim(Rtrim(IsNull(ConstitutionOfBusiness,''))) = ''
					And Ltrim(Rtrim(IsNull(Address,''))) = ''

					Insert TBL_Supplier
					(	SupplierName, POCName, NatureOfBusiness, EmailId, MobileNo, 
						GSTINno, StateCode, PANNO, 
						DateofCompRegistered, ConstitutionOfBusiness, Address, 
						CustomerId,CreatedBy, CreatedDate, RowStatus
					)
					Select 
						SupplierName,POCName,NatureOfBusiness,EmailId,MobileNo, 
						GSTINno,StateCode,PANNO, 
						DateofCompRegistered,ConstitutionOfBusiness,Address, 
						@custId,@UserId,getdate(),1
					From #MasterRecordFieldValues t1
					Where Ltrim(Rtrim(IsNull(t1.StateCode,''))) <> ''
					And Not Exists ( SELECT 1 FROM TBL_Supplier t2
										Where t2.GSTINno = t1.GSTINno 
										And t2.EmailId = t1.EmailId
										And t2.MobileNo = t1.MobileNo
										And t2.CustomerId = @custId
										And rowstatus=1 )	

					Select @ProcessedRecordsCount= @@rowcount
					Select @TotalRecordsCount = Count(*) From #MasterRecordFieldValues						
				
			
				End Try
				Begin Catch
					If IsNull(ERROR_MESSAGE(),'') <> ''	
					Begin
						Select 'Supplier -' + 'Error in Supplier Data'
					End				
				End Catch

			End
			else if @MasterType = 'Buyer'
			Begin
				Begin Try

					Delete From #MasterRecordFieldValues
					Where Ltrim(Rtrim(IsNull(SupplierName,''))) = ''
					And Ltrim(Rtrim(IsNull(POCName,''))) = ''
					And Ltrim(Rtrim(IsNull(NatureOfBusiness,''))) = ''
					And Ltrim(Rtrim(IsNull(EmailId,''))) = ''
					And Ltrim(Rtrim(IsNull(MobileNo,''))) = ''
					And Ltrim(Rtrim(IsNull(GSTINno,''))) = ''
					And Ltrim(Rtrim(IsNull(StateCode,''))) = ''
					And Ltrim(Rtrim(IsNull(PANNO,''))) = ''
					And Ltrim(Rtrim(IsNull(DateofCompRegistered,''))) = ''
					And Ltrim(Rtrim(IsNull(ConstitutionOfBusiness,''))) = ''
					And Ltrim(Rtrim(IsNull(Address,''))) = ''


					Insert TBL_Buyer
					(	BuyerName, POCName, NatureOfBusiness, EmailId, MobileNo, 
						GSTINno, StateCode, PANNO, 
						DateofCompRegistered, ConstitutionOfBusiness, Address, 
						CustomerId,CreatedBy, CreatedDate, RowStatus
					)
					Select 
						SupplierName,POCName,NatureOfBusiness,EmailId,MobileNo, 
						GSTINno,StateCode,PANNO, 
						DateofCompRegistered,ConstitutionOfBusiness,Address, 
						@custId,@UserId,getdate(),1
					From #MasterRecordFieldValues t1
					Where Ltrim(Rtrim(IsNull(t1.StateCode,''))) <> ''
					And Not Exists ( SELECT 1 FROM TBL_Buyer t2
										Where t2.GSTINno = t1.GSTINno  
										And t2.EmailId = t1.EmailId
										And t2.MobileNo = t1.MobileNo
										And t2.CustomerId = @custId
										And rowstatus=1) 

					Select @ProcessedRecordsCount= @@rowcount
					Select @TotalRecordsCount = Count(*) From #MasterRecordFieldValues						
				
			
				End Try
				Begin Catch
					If IsNull(ERROR_MESSAGE(),'') <> ''	
					Begin
						Select 'Customer -' + 'Error in Customer Data'
					End				
				End Catch

			End
		End

	End

	else
	Begin
			
		Create Table #MasterHSNFieldValues
		(
			slno varchar(50),
			hsnCode varchar(15),
			hsnDescription varchar(MAX),
			rate varchar(50),
			hsnType	varchar(50),	
			errorcode smallint,
			errormessage varchar(255)
		 )
 
		Begin Try

			Insert Into #MasterHSNFieldValues
			Select 
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 1,@Delimiter) as Slno ,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 2,@Delimiter) as hsnCode,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 3,@Delimiter) as hsnDescription,
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 4,@Delimiter) as rate,			
				dbo.udf_SplitGSTRRecordData(Uploadcontent, 5,@Delimiter) as hsnType,
				null,
				null  
			From #MasterRecordValues

		End Try
		Begin Catch
	 
			If IsNull(ERROR_MESSAGE(),'') <> ''	
			Begin
				select error_message()
			End				
		End Catch

		-- Processing Logic

		if exists (Select 1 from #MasterHSNFieldValues)
		Begin

		
			Begin Try

				INSERT  TBL_HSN_MASTER 
				(	hsnCode,hsnDescription,rate,hsnType,
					CustomerId,	CreatedBy,CreatedDate,rowstatus
				)
				Select 
					hsnCode,hsnDescription,rate,hsnType,
					@custId,@UserId,Getdate(),1 
				From #MasterHSNFieldValues t1
				Where Not Exists ( SELECT 1 FROM TBL_HSN_MASTER t2
									Where t2.hsnCode = t1.hsnCode 
									And t2.hsnDescription = t1.hsnDescription
									And t2.rate = t1.rate
									And t2.CustomerId=@custId 
									And rowstatus=1 )	
				
				Select @ProcessedRecordsCount= @@rowcount
				Select @TotalRecordsCount = Count(*) From  #MasterHSNFieldValues	
					
			End Try
			Begin Catch

				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Select 'HSN -' + error_message()
				End	
							
			End Catch

		End
	End

	 -- Drop Temp Tables

	
	 Return 0

End