/*
(c) Copyright 2017 WEP Solutions Pvt. Ltd..
All rights reserved

Description : Procedure to import Customer Records
				
Written by  : raja.m@wepindia.com

Date		Who			Decription 
07/03/2018	Raja		Initial Version
03/04/2018	Karthik		Added Transformation Logics and added few more validations
16/04/2018	Raja		Delete Existing Records before Inserting
24/05/2018  Karthik		Added Trans doc date and Branch validations
28/05/2018	Karthik		Added Supply type and sub supply type items validations.
29/05/2018  Karthik     Added Branch Setting and Invoice type validations included.
30/5/2018   Karthik     Added Trans Mode and Vehicle No, Cess Validation si Included.
31/5/2018   Karthik     Added All Tax calculations and Updating error records with proper field values.

*/

/* Sample Procedure Call  

exec usp_Import_CSV_EWB_GEN 225,1,1

 */
 
CREATE PROCEDURE [dbo].[usp_Import_CSV_EWB_GEN]
	@FileId int,
	@UserId int, 
	@CustId int,
	@TotalRecordsCount int = Null out,
	@ProcessedRecordsCount int = Null out,
	@ErrorRecordsCount int = Null out
-- /*mssx*/ With Encryption 
as 
Begin

	Set Nocount on

	Declare @Delimiter char(1),
			@ReferenceNo varchar(250),
			@SourceType varchar(255)

	Select @Delimiter = ','
	Select @SourceType = 'CSV'

	Select @ReferenceNo = ReferenceNo from TBL_Customer where CustId = @CustId
	
	Create Table #TBL_CSV_EWBGEN_RECS
	(
		fileid int NULL,
		slno varchar(250),
		usergstin varchar(15) NULL,
		supplytype varchar(50) NULL,
		subsupplytype varchar(50) NULL,
		doctype varchar(50) NULL,
		docno varchar(50) NULL,
		docdate varchar(50) NULL,
		fromgstin varchar(50) NULL,
		fromtrdname varchar(50) NULL,
		fromaddr1 varchar(50) NULL,
		fromaddr2 varchar(50) NULL,
		fromplace varchar(50) NULL,
		frompincode varchar(50) NULL,
		fromstatecode varchar(50) NULL,
		actfromstatecode varchar(50) NULL,
		togstin varchar(50) NULL,
		totrdname varchar(50) NULL,
		toaddr1 varchar(50) NULL,
		toaddr2 varchar(50) NULL,
		toplace varchar(50) NULL,
		topincode varchar(50) NULL,
		tostatecode varchar(50) NULL,
		acttostatecode varchar(50) NULL,
		totalvalue varchar(50) NULL,
		igstvalue varchar(50) NULL,
		cgstvalue varchar(50) NULL,
		sgstvalue varchar(50) NULL,
		cessvalue varchar(50) NULL,
		totinvvalue varchar(50) NULL,
		transmode varchar(20),
		transdistance varchar(50) NULL,
		transporterid varchar(50) NULL,
		transportername varchar(50) NULL,
		transdocno varchar(50) NULL,
		transdocdate varchar(50) NULL,
		vehicleno varchar(50) NULL,
		vehicletype varchar(250) NULL,
		productname varchar(250) NULL,
		productdesc varchar(250) NULL,
		hsncode varchar(50) NULL,
		quantity varchar(50) NULL,
		qtyunit varchar(50) NULL,
		taxableamount varchar(50) NULL,
		igstrate varchar(50) NULL,
		cgstrate varchar(50) NULL,
		sgstrate varchar(50) NULL,
		cessrate varchar(50) NULL,
		cessadvol varchar(50) NULL,
		branch   varchar(255) NULL,
		InvoiceType varchar(100) NULL,
		AllowDuplication varchar(10) NULL,
		gstinid int,
		errorcode smallint NULL,
		errormessage varchar(255) NULL,
	 )

	 Begin Try

		Insert Into #TBL_CSV_EWBGEN_RECS
		( fileid,
			slno,
			supplytype,
			subsupplytype,
			doctype,
			docno,
			docdate,
			fromgstin,
			fromtrdname,
			fromaddr1,
			fromaddr2,
			fromplace,
			frompincode,
			fromstatecode,
			actfromstatecode,
			togstin,
			totrdname,
			toaddr1,
			toaddr2,
			toplace,
			topincode,
			tostatecode,
			acttostatecode,
			totalvalue,
			igstvalue,
			cgstvalue,
			sgstvalue,
			cessvalue,
			totinvvalue,
			transmode,
			transdistance,
			transporterid,
			transportername,
			transdocno,
			transdocdate,
			vehicleno,
			vehicletype,
			productname,
			productdesc,
			hsncode,
			quantity,
			qtyunit,
			taxableamount,
			igstrate,
			cgstrate,
			sgstrate,
			cessrate,
			cessadvol,
			branch,
			InvoiceType,
			AllowDuplication )

		Select	fileid,
			slno,
			supplytype,
			subsupplytype,
			doctype,
			docno,
			docdate,
			fromgstin,
			fromtrdname,
			fromaddr1,
			fromaddr2,
			fromplace,
			frompincode,
			fromstatecode,
			actfromstatecode,
			togstin,
			totrdname,
			toaddr1,
			toaddr2,
			toplace,
			topincode,
			tostatecode,
			acttostatecode,
			totalvalue,
			igstvalue,
			cgstvalue,
			sgstvalue,
			cessvalue,
			totinvvalue,
			transmode,
			transdistance,
			transporterid,
			transportername,
			transdocno,
			transdocdate,
			vehicleno,
			vehicletype,
			productname,
			productdesc,
			hsncode,
			quantity,
			qtyunit,
			taxableamount,
			igstrate,
			cgstrate,
			sgstrate,
			cessrate,
			cessadvol,
			branch,
			InvoiceType,
			AllowDuplication
		From TBL_CSV_EWB_GEN_RECS t1 
		Where t1.fileid = @FileId
	
	End Try
	Begin Catch
	 
		If IsNull(ERROR_MESSAGE(),'') <> ''	
		Begin
			select error_message()
		End				
	End Catch

	Declare @Gstinid int
	
	Select @TotalRecordsCount = Count(*)
	From #TBL_CSV_EWBGEN_RECS
	
	if exists (Select 1 from #TBL_CSV_EWBGEN_RECS)
	Begin

		Update #TBL_CSV_EWBGEN_RECS Set totalvalue = Ltrim(Rtrim(IsNull(totalvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set igstvalue = Ltrim(Rtrim(IsNull(igstvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set cgstvalue = Ltrim(Rtrim(IsNull(cgstvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set sgstvalue = Ltrim(Rtrim(IsNull(sgstvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set cessvalue = Ltrim(Rtrim(IsNull(cessvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set totinvvalue = Ltrim(Rtrim(IsNull(totinvvalue,'')))
		Update #TBL_CSV_EWBGEN_RECS Set quantity = Ltrim(Rtrim(IsNull(quantity,'')))
		Update #TBL_CSV_EWBGEN_RECS Set taxableamount = Ltrim(Rtrim(IsNull(taxableamount,'')))
		Update #TBL_CSV_EWBGEN_RECS Set igstrate = Ltrim(Rtrim(IsNull(igstrate,'')))
		Update #TBL_CSV_EWBGEN_RECS Set cgstrate = Ltrim(Rtrim(IsNull(cgstrate,'')))
		Update #TBL_CSV_EWBGEN_RECS Set sgstrate = Ltrim(Rtrim(IsNull(sgstrate,'')))
		Update #TBL_CSV_EWBGEN_RECS Set cessrate = Ltrim(Rtrim(IsNull(cessrate,'')))
		Update #TBL_CSV_EWBGEN_RECS Set docNo = Ltrim(Rtrim(IsNull(docNo,'')))
		Update #TBL_CSV_EWBGEN_RECS Set transDistance = Ltrim(Rtrim(IsNull(transDistance,'')))
		
		Declare @LocationReq int, @userGSTIN varchar(15)
		select @LocationReq=LocationReqd from tbl_cust_settings where custid = @Custid and rowstatus =1
			If @LocationReq <> 1
				Begin
					Set @LocationReq = 0
				End
			Else
				Begin
					Set @LocationReq = 1
				End

		-- Supply Type Validation
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -1,
				ErrorMessage = 'Supply Type is mandatory'
			Where Ltrim(Rtrim(IsNull(supplyType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_CSV_EWBGEN_RECS 
			Set SupplyType =
			Case SupplyType
			When  'Inward'  Then  'I'
			When  'Outward'  Then  'O'
			else SupplyType
			End
			where IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -2,
				ErrorMessage = 'Invalid Supply Type'
			Where SupplyType not in ('O','I')
			And IsNull(ErrorCode,0) = 0
			
		-- Sub Supply Type Validation
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -3,
				ErrorMessage = 'Sub Supply Type is mandatory'
			Where Ltrim(Rtrim(IsNull(subSupplyType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
		
		Update #TBL_CSV_EWBGEN_RECS 
			Set subsupplytype =
				Case subsupplytype
				When  'Supply'  Then  '1'--o,I
				When  'Import'  Then  '2'--I
				When  'Export'  Then  '3'--o
				When  'Job Work'  Then  '4'--o
				When  'For Own Use'  Then  '5'--o,I
				When  'Job work Returns'  Then  '6'--I
				When  'Sales Return'  Then  '7'--I
				When  'Others'  Then  '8'--o,I
				When  'SKD/CKD'  Then  '9'--o,I
				When  'Line Sales'  Then  '10'--o
				When  'Recipient Not Known'  Then  '11'--o
				When  'Exhibition or Fairs'  Then  '12'--o,I
				else subsupplytype
				End
				where IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -4,
				ErrorMessage = 'Invalid Outward Sub Supply Type'
			Where subsupplytype not in ('1','3','4','5','8','9','10','11','12')
			And SupplyType in ('O')
			And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -5,
				ErrorMessage = 'Invalid Inward Sub Supply Type'
			Where subsupplytype not in ('1','2','5','6','7','8','9','12')
			And SupplyType in ('I')
			And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS 
			Set usergstin = fromgstin 
			from #TBL_CSV_EWBGEN_RECS 
			Where supplytype = 'O' 
			And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS 
			Set usergstin =  togstin 
			from #TBL_CSV_EWBGEN_RECS 
			Where supplytype = 'I' 
			And IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -6,
				ErrorMessage = 'From Gstin is Not Applicable'
			Where subsupplytype in ('2')
			And SupplyType in ('I')
			And fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -7,
				ErrorMessage = 'To Gstin is Not Applicable'
			Where subsupplytype in ('3')
			And SupplyType in ('O')
			And togstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		-- Document Type Validation	
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -8,
				ErrorMessage = 'DocType is mandatory'
			Where Ltrim(Rtrim(IsNull(docType,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
		
		Update #TBL_CSV_EWBGEN_RECS 
			Set docType =
			Case docType
				When  'Bill of Entry'  Then  'BOE'
				When  'Bill of Supply'  Then  'BOL'
				When  'Challan'  Then  'CHL'
				When  'Credit Note'  Then  'CNT'
				When  'Tax Invoice'  Then  'INV'
				When  'Others'  Then  'OTH'
				else docType
				End
			where IsNull(ErrorCode,0) = 0
		
		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -9,
				ErrorMessage = 'Invalid Document Type'
			Where docType not in ('INV','BOL','BOE','CHL','CNT','OTH')
			And IsNull(ErrorCode,0) = 0

		-- Document No Validation	
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -10,
				ErrorMessage = 'DocNo is mandatory'
			Where Ltrim(Rtrim(IsNull(docNo,''))) = '' 
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -11,
				ErrorMessage = 'Invalid Document No'
			Where dbo.udf_ValidateInvoiceNo(docNo) <> 1
			And IsNull(ErrorCode,0) = 0

		-- Document Date Validation
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -12,
				ErrorMessage = 'DocDate is mandatory'
			Where Ltrim(Rtrim(IsNull(docDate,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
					
		-- From GSTIN Validation
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -13,
				ErrorMessage = 'From Gstin is mandatory'
			Where Ltrim(Rtrim(IsNull(fromGstin,''))) = '' 
			And IsNull(ErrorCode,0) = 0 
			
		Update #TBL_CSV_EWBGEN_RECS
			Set ErrorCode = -14,
				ErrorMessage = 'Invalid From Gstin'
			Where dbo.udf_ValidateGstin(fromgstin) <> 1 
			And subsupplytype not in ('2')
			And fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0

		-- From Pin Code Validation
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -15,
				ErrorMessage = 'From Pin Code is mandatory'
			Where   Ltrim(Rtrim(IsNull(fromPincode,0))) = 0
			And subsupplytype not in ('2') and fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0 

		Update #TBL_CSV_EWBGEN_RECS 
			Set  ErrorCode = -16,
				ErrorMessage = 'Invalid From PinCode'
			Where subsupplytype in ('2') and fromgstin = 'URP'
			And fromPincode <> '999999'
			And IsNull(ErrorCode,0) = 0 

		-- From State Code Validation	
		Update #TBL_CSV_EWBGEN_RECS 
			Set ErrorCode = -17,
				ErrorMessage = 'From State Code is mandatory'
			Where Ltrim(Rtrim(IsNull(fromStateCode,''))) = '' 
			And subsupplytype not in ('2') and fromgstin <> 'URP'
			And IsNull(ErrorCode,0) = 0 
					
		Update #TBL_CSV_EWBGEN_RECS 
			Set fromstatecode =
				Case fromstatecode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else fromstatecode
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -18,
						ErrorMessage = 'Invalid From State Code'
				Where subsupplytype in ('2') and fromgstin = 'URP'
				And (fromStateCode <> '00' or fromStateCode <> '0')
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -19,
					ErrorMessage = 'Invalid From State Code'
				Where fromstatecode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0	
				
			Update #TBL_CSV_EWBGEN_RECS 
				Set fromstatecode = '0' + Ltrim(Rtrim(IsNull(fromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(fromstatecode,'')))) = 1	

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -20,
					ErrorMessage = 'Invalid From State Code'
				Where dbo.udf_ValidatePlaceOfSupply(fromstatecode) <> 1
				And subsupplytype not in ('2') and fromgstin <> 'URP'
				And (toStateCode <> '00' or toStateCode <> '0')
				And IsNull(ErrorCode,0) = 0

		-- To GSTIN Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -21,
					ErrorMessage = 'To Gstin is mandatory'
				Where Ltrim(Rtrim(IsNull(toGstin,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -22,
					ErrorMessage = 'Invalid To Gstin'
				Where dbo.udf_ValidateGstin(togstin) <> 1
				And subsupplytype not in ('3') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0
			
		-- To PinCode Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -23,
					ErrorMessage = 'To Pin Code is mandatory'
				Where   Ltrim(Rtrim(IsNull(toPincode,0))) = 0
				And subsupplytype not in ('3') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -24,
					ErrorMessage = 'Invalid To Pin Code'
				Where subsupplytype in ('3') and togstin = 'URP'
				And toPincode <> '999999'
				And IsNull(ErrorCode,0) = 0 
		
		-- To State Code Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -25,
					ErrorMessage = 'To State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(toStateCode,''))) = ''  
				And subsupplytype not in ('2') and togstin <> 'URP'
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
			Set toStateCode =
				Case toStateCode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else toStateCode
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -26,
						ErrorMessage = 'Invalid To State Code'
				Where subsupplytype in ('3') and togstin = 'URP'
				And (toStateCode <> '00' or toStateCode <> '0')
				And IsNull(ErrorCode,0) = 0 
				
			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -27,
					ErrorMessage = 'Invalid To State Code'
				Where toStateCode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0			
				
			Update #TBL_CSV_EWBGEN_RECS 
				Set toStateCode = '0' + Ltrim(Rtrim(IsNull(toStateCode,'')))
				Where Len(Ltrim(Rtrim(IsNull(toStateCode,'')))) = 1	

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -28,
					ErrorMessage = 'Invalid To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(toStateCode) <> 1
				And subsupplytype not in ('3') and togstin <> 'URP'
				And (toStateCode <> '00' or toStateCode <> '0')
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -29,
					ErrorMessage = 'From Gstin is not registered'
					Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = @custid
						And t1.GstinNo = fromgstin
						And t1.rowstatus = 1) 
					And (isnull(fromgstin,'') <> '' or isnull(fromgstin,'') <> 'URP')
					And SupplyType in ('O') 
					And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -30,
					ErrorMessage = 'TO Gstin is not registered'
					Where Not Exists(Select 1 From
							Tbl_Cust_Gstin t1
						Where t1.custid = @custid
						And t1.GstinNo = togstin
						And t1.rowstatus = 1) 
					And (isnull(togstin,'') <> '' or isnull(togstin,'') <> 'URP')
					And SupplyType in ('I')
					And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -31,
					ErrorMessage = 'User Gstin is not registered'
					Where Not Exists(Select 1 From Tbl_Cust_Gstin t1
						Where t1.custid = @custid And t1.GstinNo = usergstin And t1.rowstatus = 1) 
					And (isnull(usergstin,'') <> '' or isnull(usergstin,'') <> 'URP')
					And IsNull(ErrorCode,0) = 0

			-- Actual From State Code Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -32,
					ErrorMessage = 'Actual From State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(actfromstatecode,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
			Set actfromstatecode =
				Case actfromstatecode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else actfromstatecode
				End
				where IsNull(ErrorCode,0) = 0
									
			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -33,
					ErrorMessage = 'Invalid Actual From State Code'
				Where actfromstatecode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0			
				
			Update #TBL_CSV_EWBGEN_RECS 
				Set actfromstatecode = '0' + Ltrim(Rtrim(IsNull(actfromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(actfromstatecode,'')))) = 1		

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -34,
					ErrorMessage = 'Invalid Actual From State Code'
				Where dbo.udf_ValidatePlaceOfSupply(actfromstatecode) <> 1
				And IsNull(ErrorCode,0) = 0

			-- Actual To State Code Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -35,
					ErrorMessage = 'Actual From State Code is mandatory'
				Where Ltrim(Rtrim(IsNull(acttoStateCode,''))) = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
			Set acttoStateCode =
				Case acttoStateCode
				When  'OTHER COUNTRIES'  Then  '0'
				When  'JAMMU AND KASHMIR'  Then  '1'
				When  'HIMACHAL PRADESH'  Then  '2'
				When  'PUNJAB'  Then  '3'
				When  'CHANDIGARH'  Then  '4'
				When  'UTTARAKHAND'  Then  '5'
				When  'HARYANA'  Then  '6'
				When  'DELHI'  Then  '7'
				When  'RAJASTHAN'  Then  '8'
				When  'UTTAR PRADESH'  Then  '9'
				When  'BIHAR'  Then  '10'
				When  'SIKKIM'  Then  '11'
				When  'ARUNACHAL PRADESH'  Then  '12'
				When  'NAGALAND'  Then  '13'
				When  'MANIPUR'  Then  '14'
				When  'MIZORAM'  Then  '15'
				When  'TRIPURA'  Then  '16'
				When  'MEGHALAYA'  Then  '17'
				When  'ASSAM'  Then  '18'
				When  'WEST BENGAL'  Then  '19'
				When  'JHARKHAND'  Then  '20'
				When  'ORISSA'  Then  '21'
				When  'CHHATTISGARH'  Then  '22'
				When  'MADHYA PRADESH'  Then  '23'
				When  'GUJARAT'  Then  '24'
				When  'DAMAN AND DIU'  Then  '25'
				When  'DADAR AND NAGAR HAVELI'  Then  '26'
				When  'MAHARASTRA'  Then  '27'
				When  'KARNATAKA'  Then  '29'
				When  'GOA'  Then  '30'
				When  'LAKSHADWEEP'  Then  '31'
				When  'KERALA'  Then  '32'
				When  'TAMIL NADU'  Then  '33'
				When  'PONDICHERRY'  Then  '34'
				When  'ANDAMAN AND NICOBAR'  Then  '35'
				When  'TELENGANA'  Then  '36'
				When  'ANDHRA PRADESH'  Then  '37'
				else acttoStateCode
				End
				where IsNull(ErrorCode,0) = 0

									
			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -36,
					ErrorMessage = 'Invalid Actual To State Code'
				Where acttoStateCode not in ('00','01','02','03','04','05','06','07','08','09','0','1','2','3','4','5','6','7','8','9','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0	
				
			Update #TBL_CSV_EWBGEN_RECS 
				Set acttoStateCode = '0' + Ltrim(Rtrim(IsNull(acttoStateCode,'')))
				Where Len(Ltrim(Rtrim(IsNull(acttoStateCode,'')))) = 1		

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -37,
					ErrorMessage = 'Invalid Actual To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(acttoStateCode) <> 1
				And IsNull(ErrorCode,0) = 0

			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -38,
					ErrorMessage = 'Total Value is mandatory'
				Where totalValue = ''
				And IsNull(ErrorCode,0) = 0 
				
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -39,
					ErrorMessage = 'Total Value is Not Numeric'
				Where isnumeric(totalValue) <> 1
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -40,
					ErrorMessage = 'Invalid CGST/SGST Rate'
				Where dbo.udf_ValidateRate(try_convert(Decimal(18,2),cgstRate) + try_convert(Decimal(18,2),sgstRate)) <> 1
				And cgstRate <> '' And sgstRate <> ''
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -41,
					ErrorMessage = 'Invalid IGST Rate'
				Where dbo.udf_ValidateRate(igstRate) <> 1
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -42,
					ErrorMessage = 'Invalid CESS Rate'
				Where cessrate <> '' and isnumeric(try_convert(Decimal(18,2),cessrate)) <> 1
				And IsNull(ErrorCode,0) = 0	
				   
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -43,
					ErrorMessage = 'Tax Amount is mandatory'				
					Where  (
						Convert(Decimal(18,2),cgstRate) > 0
					Or   Convert(Decimal(18,2),sgstRate) > 0
					Or   Convert(Decimal(18,2),igstRate) > 0
					) 
					And   (
						  Convert(Decimal(18,2),cgstValue) = 0
					And   Convert(Decimal(18,2),sgstValue) = 0
					And   Convert(Decimal(18,2),igstValue) = 0
					)
					And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -44,
					ErrorMessage = 'CGST Value is Not Numeric'
				Where isnumeric(cgstValue) <> 1
				And Convert(Decimal(18,2),cgstValue) <> 0
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -45,
					ErrorMessage = 'SGST Value is Not Numeric'
				Where isnumeric(sgstValue) <> 1
				And  Convert(Decimal(18,2),sgstValue) <> 0
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -46,
					ErrorMessage = 'IGST Value is Not Numeric'
				Where isnumeric(igstValue) <> 1
				And  Convert(Decimal(18,2),igstValue) <> 0
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -47,
					ErrorMessage = 'CESS Value is Not Numeric'
				Where IsNumeric(cessValue) <> 1
				And  Convert(Decimal(18,2),cessValue) <> 0
				And IsNull(ErrorCode,0) = 0		
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -48,
					ErrorMessage = 'Taxable Amount is mandatory'
				Where  taxableAmount = '' 
				And IsNull(ErrorCode,0) = 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -49,
					ErrorMessage = 'Igst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) > 0
				And Convert(Decimal(18,2),cgstRate) = 0
				And Convert(Decimal(18,2),sgstrate) = 0
				And  (igstValue < ((Convert(dec(18,2),igstRate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (igstValue > ((Convert(dec(18,2),igstRate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0	
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -50,
					ErrorMessage = 'Cgst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) = 0
				And Convert(Decimal(18,2),cgstRate) > 0
				And Convert(Decimal(18,2),sgstrate) > 0
				And  (cgstvalue < ((Convert(dec(18,2),cgstRate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (cgstvalue > ((Convert(dec(18,2),cgstRate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -51,
					ErrorMessage = 'Sgst Amount is wrong.'
				Where Convert(Decimal(18,2),igstRate) = 0
				And Convert(Decimal(18,2),cgstRate) > 0
				And Convert(Decimal(18,2),sgstrate) > 0
				And  (sgstValue < ((Convert(dec(18,2),sgstrate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (sgstValue > ((Convert(dec(18,2),sgstrate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -52,
					ErrorMessage = 'Cess Amount is wrong.'
				Where Convert(Decimal(18,2),cessrate) > 0
				And  (cessvalue < ((Convert(dec(18,2),cessrate)/100) * Convert(dec(18,2),taxableAmount) - 1.00)) 
				or (cessvalue > ((Convert(dec(18,2),cessrate)/100) * Convert(dec(18,2),taxableAmount) + 1.00))			
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -53,
					ErrorMessage = 'Transport Distance is mandatory'
				Where transDistance = ''
				And IsNull(ErrorCode,0) = 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -54,
					ErrorMessage = 'Transporter Id is mandatory'
				Where  Ltrim(Rtrim(IsNull( transporterId,''))) = ''
				And IsNull(ErrorCode,0) = 0
			 
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -55,
					ErrorMessage = 'Transporter Name is mandatory'
				Where  Ltrim(Rtrim(IsNull( transporterName,''))) = ''
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -56,
					ErrorMessage = 'Invalid Transporter Date'
				Where dbo.udf_Validate_EWB_DocDate(docdate,transDocDate) <> 1
				And isnull(docdate,'')<>'' 
				And isnull(transDocDate,'')<>''
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -57,
					ErrorMessage = 'HSN Code is mandatory'
				Where  Ltrim(Rtrim(IsNull( hsnCode,''))) = ''
				And IsNull(ErrorCode,0) = 0

			-- TransMode Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set transmode =
				Case convert(varchar(10),transmode)
				When  'Road'  Then  '1'
				When  'Rail'  Then  '2'
				When  'Air'  Then  '3'
				When  'Ship'  Then  '4'
				else transmode
				End
				where Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0

			
			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -58,
					ErrorMessage = 'Invalid Transport mode'
				Where transmode not in ('','1','2','3','4')
				And Ltrim(Rtrim(IsNull(transmode,''))) <> '' And IsNull(ErrorCode,0) = 0

			-- Vehicle Type Validation
			Update #TBL_CSV_EWBGEN_RECS 
				Set vehicletype =
				Case vehicletype
				When  'Regular'  Then  'R'
				When  'Over dimensional cargo'  Then  'O'
				else vehicletype
				End
				where IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -59,
					ErrorMessage = 'Vehicle No is Mandatory'
				Where isnull(vehicleno,'')='' and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -60,
					ErrorMessage = 'Invalid Vehicle No'
				Where dbo.udf_ValidateVehicleNo(vehicleno) <> 1 and Ltrim(Rtrim(IsNull(transmode,''))) = '1'
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -61,
					ErrorMessage = 'Invalid Vehicle Type'
				Where vehicletype not in ('','R','O')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set qtyunit =
				Case qtyunit
				When  'BAGS'  Then  'BGS'
				When  'BUNDLES'  Then  'BND'
				When  'BOXES'  Then  'BOX'
				When  'CENTIMETERS'  Then  'CMS'
				When  'DOZENS'  Then  'DZN'
				When  'GRAMS'  Then  'GMS'
				When  'HUNDRED KILOMETERS'  Then  'HKM'
				When  'HUNDRED NUMBERS/UNITS'  Then  'HNO'
				When  'KILOGRAMS'  Then  'KGS'
				When  'KILOMETERS'  Then  'KMS'
				When  'LITRES'  Then  'LTS'
				When  'MILLILITRES'  Then  'MLS'
				When  'METERS'  Then  'MTR'
				When  'METRIC TONNES'  Then  'MTS'
				When  'NUMBERS/UNITS'  Then  'NOS'
				When  'PAIRS'  Then  'PAR'
				When  'QUINTALS'  Then  'QTS'
				When  'THOUSAND NUMBERS/UNITS'  Then  'SNO'
				When  'THOUSAND KILOMETERS'  Then  'TKM'
				When  'THOUSAND LITRES'  Then  'TLT'
				When  'TEN NUMBERS/UNITS'  Then  'TNO'
				When  'TONNES'  Then  'TON'
				else qtyunit
				End
				where IsNull(ErrorCode,0) = 0	
			
			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = 62,
					ErrorMessage = 'Invalid Quantity Unit'
				Where qtyunit not in ('BGS','BND','BOX','CMS','DZN','GMS','HKM','HNO','KGS','KMS','LTS','MLS','MTR','MTS','NOS','PAR','QTS','SNO','TKM','TLT','TNO','TON')
				And IsNull(ErrorCode,0) = 0
			
			Update #TBL_CSV_EWBGEN_RECS -- Introduced due to Excel Format Issue
				Set fromstatecode = '0' + Ltrim(Rtrim(IsNull(fromstatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(fromstatecode,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS -- Introduced due to Excel Format Issue
				Set tostatecode = '0' + Ltrim(Rtrim(IsNull(tostatecode,'')))
				Where Len(Ltrim(Rtrim(IsNull(tostatecode,'')))) = 1 And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS
				Set ErrorCode = -63,
					ErrorMessage = 'Invalid To State Code'
				Where tostatecode not in ('00','01','02','03','04','05','06','07','08','09','10','11','12','13','14','15','16','17','18','19','20','21','22','23','24','25','26','27','29','30','31','32','33','34','35','36','37')
				And IsNull(ErrorCode,0) = 0		

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -64,
					ErrorMessage = 'Invalid To State Code'
				Where dbo.udf_ValidatePlaceOfSupply(tostatecode) <> 1
				And IsNull(ErrorCode,0) = 0	
					
			Update #TBL_CSV_EWBGEN_RECS
				Set #TBL_CSV_EWBGEN_RECS.gstinid = t2.gstinid
				FROM #TBL_CSV_EWBGEN_RECS t1,
						tbl_cust_gstin t2 
				WHERE upper(t1.usergstin) = upper(t2.Gstinno)
				And t2.Custid = @CustId
				And t2.rowstatus =1
				and  IsNull(ErrorCode,0) = 0
		
			if (@LocationReq = 1)
			Begin
			
				Update #TBL_CSV_EWBGEN_RECS
					Set #TBL_CSV_EWBGEN_RECS.branch = t2.branchid
					FROM #TBL_CSV_EWBGEN_RECS t1,
							tbl_Cust_Location t2 
					WHERE upper(ltrim(rtrim(t2.branch))) = upper(ltrim(rtrim(t1.branch)))
					And t2.Custid = @CustId
					And (t2.gstinid = t1.gstinid or isnull(t1.gstinid,0) = 0)
					And t2.rowstatus =1
					and  IsNull(ErrorCode,0) = 0

				Update #TBL_CSV_EWBGEN_RECS
					Set ErrorCode = -54,
						ErrorMessage = 'Invalid Branch name'
					Where  isnumeric(ltrim(rtrim(branch))) <> 1
					And IsNull(ErrorCode,0) = 0
			End
			Else
			Begin
				Update #TBL_CSV_EWBGEN_RECS
					Set branch = 0
					WHERE  IsNull(ErrorCode,0) = 0
			End

			
			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -65,
					ErrorMessage = 'Invalid Invoice Type'
				Where ltrim(rtrim(InvoiceType)) not in ('Regular','SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -66,
					ErrorMessage = 'Igst Amount is mandatory for Deemed Exports / Supplies to SEZ.'
				Where igstValue ='' And InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0	
			
			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -67,
					ErrorMessage = 'Cgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				Where (cgstValue <> '' And Convert(dec(18,2),cgstValue) > 0) and  InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0	

			Update #TBL_CSV_EWBGEN_RECS  
				Set ErrorCode = -68,
					ErrorMessage = 'Sgst Amount is not applicable for Deemed Exports / Supplies to SEZ.'
				Where (sgstValue <> '' And Convert(dec(18,2),sgstValue) > 0) and  InvoiceType in ('SEZ with Payment','SEZ without Payment','Deemed Exports')
				And IsNull(ErrorCode,0) = 0

			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -69,
					ErrorMessage = 'Invalid Transporter Id'
				From #TBL_CSV_EWBGEN_RECS t1
				Where len(ltrim(rtrim(transporterid))) > 15 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -100,
					ErrorMessage = 'Eway Bill data already uploaded'
				From #TBL_CSV_EWBGEN_RECS t1
				Where upper(AllowDuplication) = 'YES' 
				And  IsNull(ErrorCode,0) = 0
				And Exists(Select 1 From  TBL_EWB_GENERATION 
								Where userGstin = t1.UserGstin 
								And docNo = t1.DocNo 
								And DocDate = t1.DocDate 
								And isnull(ewaybillNo,'') = ''
								And isnull(ewayBillDate,'') = '')

		    Update #TBL_CSV_EWBGEN_RECS 
				Set ErrorCode = -101,
					ErrorMessage = 'Eway Bill data already Generated.'
				From #TBL_CSV_EWBGEN_RECS t1
				Where upper(AllowDuplication) = 'NO' 
				And  IsNull(ErrorCode,0) = 0
				And Exists(Select 1 From  TBL_EWB_GENERATION 
								Where userGstin = t1.UserGstin 
								And docNo = t1.DocNo 
								And DocDate = t1.DocDate)

		---- Delete Existing Records Starts		
		--	Select t1.ewbid into #ewbids
		--	FROM TBL_EXT_EWB_GENERATION t1,
		--			#TBL_CSV_EWBGEN_RECS t2 
		--	WHERE t1.docNo = t2.docNo 
		--	And t1.docdate = t2.docdate
		--	and  IsNull(ErrorCode,0) = 0

		--	Delete from TBL_EXT_EWB_GENERATION where ewbid in (select ewbid from #ewbids)
		---- Delete Existing Records Ends

		--  Inserting Customer	
		Begin Try	 
		insert into TBL_EXT_EWB_GENERATION (
					userGSTIN,
					supplytype,
					subsupplytype,
					doctype,
					docno,
					docdate,
					fromgstin,
					fromtrdname,
					fromaddr1,
					fromaddr2,
					fromplace,
					frompincode,
					fromstatecode,
					actFromstatecode,
					togstin,
					totrdname,
					toaddr1,
					toaddr2,
					toplace,
					topincode,
					tostatecode,
					actToStatecode,
					totalvalue,
					igstvalue,
					cgstvalue,
					sgstvalue,
					cessvalue,
					totinvvalue,
					transmode,
					transdistance,
					transporterid,
					transportername,
					transdocno,
					transdocdate,
					vehicleno,
					vehicletype,
					productname,
					productdesc,
					hsncode,
					quantity,
					qtyunit,
					taxableamount,
					igstrate,
					cgstrate,
					sgstrate,
					cessrate, 
					cessadvol,
					rowstatus, sourcetype, referenceno, createdby,createddate, fileid,branchid
					) 
		Select usergstin, supplytype, subsupplytype, doctype, docno, docdate, 
			fromgstin, fromtrdname, fromaddr1, fromaddr2, fromplace, frompincode, fromstatecode, actfromstatecode,
			togstin, totrdname, toaddr1, toaddr2, toplace, topincode, tostatecode, acttostatecode,
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totalvalue,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(igstvalue,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cgstvalue,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(sgstvalue,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cessvalue,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(totinvvalue,''),0)))),
			try_convert(int,transmode,0), transdistance, 
			transporterid, transportername, transdocno, 
			transdocdate, vehicleno, vehicletype, 
			productname, productdesc, hsncode, quantity, qtyunit,
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(taxableamount,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(igstrate,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cgstrate,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(sgstrate,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cessrate,''),0)))),
			Convert(dec(18,2),Ltrim(Rtrim(IsNull(NullIf(cessadvol,''),0)))), 
			1, @SourceType, @ReferenceNo, @UserId, GETDATE(), @FileId,isnull(try_convert(int,branch),NULL)
		From #TBL_CSV_EWBGEN_RECS t1 
		Where IsNull(ErrorCode,0) = 0
		End Try
			Begin Catch
				If IsNull(ERROR_MESSAGE(),'') <> ''	
				Begin
					Update #TBL_CSV_EWBGEN_RECS 
					Set ErrorCode = -102,
						ErrorMessage = 'Error in Eway bill Data'
					From #TBL_CSV_EWBGEN_RECS t1
					Where IsNull(ErrorCode,0) = 0
				End				
			End Catch

		
		Select @ProcessedRecordsCount = Count(*)
			From #TBL_CSV_EWBGEN_RECS fv
			Where IsNull(fv.ErrorCode,0) = 0

		Select @ErrorRecordsCount = Count(*)
			From #TBL_CSV_EWBGEN_RECS fv
			Where IsNull(fv.ErrorCode,0) <> 0

		Update TBL_EWB_RECVD_FILES
			Set filestatus = 0,
			totalrecordscount = @TotalRecordsCount,
			processedrecordscount = @ProcessedRecordsCount,
			errorrecordscount = @ErrorRecordsCount
			Where fileid = @FileId
		
		exec usp_Push_EWB_EXT_SA  @SourceType, @ReferenceNo

	     -- updating error record with proper text values
			Update #TBL_CSV_EWBGEN_RECS 
				Set docType =
				Case docType
				When   'INV'  Then  'Tax Invoice'
				When   'BOL'  Then  'Bill of Supply'
				When   'BOE'  Then  'Bill of Entry'
				When   'CHL'  Then  'Delivery Challan'
				When   'CNT'  Then  'Credit Note'
				When   'OTH'  Then  'Others'
				else docType
				End
				where isnull(docType,'') <>'' And IsNull(ErrorCode,0) <> 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set fromstatecode =
				Case fromstatecode
				When   '0'  Then  'OTHER COUNTRIES'
				When   '00'  Then  'OTHER COUNTRIES'
				When   '1'  Then  'JAMMU AND KASHMIR'
				When   '01'  Then  'JAMMU AND KASHMIR'				
				When   '2'  Then  'HIMACHAL PRADESH'
				When   '02'  Then  'HIMACHAL PRADESH'
				When   '3'  Then  'PUNJAB'
				When   '03'  Then  'PUNJAB'
				When   '4'  Then  'CHANDIGARH'
				When   '04'  Then  'CHANDIGARH'
				When   '5'  Then  'UTTARAKHAND'
				When   '05'  Then  'UTTARAKHAND'
				When   '6'  Then  'HARYANA'
				When   '06'  Then  'HARYANA'
				When   '7'  Then  'DELHI'
				When   '07'  Then  'DELHI'
				When   '8'  Then  'RAJASTHAN'
				When   '08'  Then  'RAJASTHAN'
				When   '9'  Then  'UTTAR PRADESH'
				When   '09'  Then  'UTTAR PRADESH'
				When   '10'  Then  'BIHAR'
				When   '11'  Then  'SIKKIM'
				When   '12'  Then  'ARUNACHAL PRADESH'
				When   '13'  Then  'NAGALAND'
				When   '14'  Then  'MANIPUR'
				When   '15'  Then  'MIZORAM'
				When   '16'  Then  'TRIPURA'
				When   '17'  Then  'MEGHALAYA'
				When   '18'  Then  'ASSAM'
				When   '19'  Then  'WEST BENGAL'
				When   '20'  Then  'JHARKHAND'
				When   '21'  Then  'ORISSA'
				When   '22'  Then  'CHHATTISGARH'
				When   '23'  Then  'MADHYA PRADESH'
				When   '24'  Then  'GUJARAT'
				When   '25'  Then  'DAMAN AND DIU'
				When   '26'  Then  'DADAR AND NAGAR HAVELI'
				When   '27'  Then  'MAHARASTRA'
				When   '29'  Then  'KARNATAKA'
				When   '30'  Then  'GOA'
				When   '31'  Then  'LAKSHADWEEP'
				When   '32'  Then  'KERALA'
				When   '33'  Then  'TAMIL NADU'
				When   '34'  Then  'PONDICHERRY'
				When   '35'  Then  'ANDAMAN AND NICOBAR'
				When   '36'  Then  'TELENGANA'
				When   '37'  Then  'ANDHRA PRADESH'
				else fromstatecode
				End
				where IsNull(fromstatecode,'') <> ''  And IsNull(ErrorCode,0) <> 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set toStateCode =
				Case toStateCode
				When   '0'  Then  'OTHER COUNTRIES'
				When   '00'  Then  'OTHER COUNTRIES'
				When   '1'  Then  'JAMMU AND KASHMIR'
				When   '01'  Then  'JAMMU AND KASHMIR'				
				When   '2'  Then  'HIMACHAL PRADESH'
				When   '02'  Then  'HIMACHAL PRADESH'
				When   '3'  Then  'PUNJAB'
				When   '03'  Then  'PUNJAB'
				When   '4'  Then  'CHANDIGARH'
				When   '04'  Then  'CHANDIGARH'
				When   '5'  Then  'UTTARAKHAND'
				When   '05'  Then  'UTTARAKHAND'
				When   '6'  Then  'HARYANA'
				When   '06'  Then  'HARYANA'
				When   '7'  Then  'DELHI'
				When   '07'  Then  'DELHI'
				When   '8'  Then  'RAJASTHAN'
				When   '08'  Then  'RAJASTHAN'
				When   '9'  Then  'UTTAR PRADESH'
				When   '09'  Then  'UTTAR PRADESH'
				When   '10'  Then  'BIHAR'
				When   '11'  Then  'SIKKIM'
				When   '12'  Then  'ARUNACHAL PRADESH'
				When   '13'  Then  'NAGALAND'
				When   '14'  Then  'MANIPUR'
				When   '15'  Then  'MIZORAM'
				When   '16'  Then  'TRIPURA'
				When   '17'  Then  'MEGHALAYA'
				When   '18'  Then  'ASSAM'
				When   '19'  Then  'WEST BENGAL'
				When   '20'  Then  'JHARKHAND'
				When   '21'  Then  'ORISSA'
				When   '22'  Then  'CHHATTISGARH'
				When   '23'  Then  'MADHYA PRADESH'
				When   '24'  Then  'GUJARAT'
				When   '25'  Then  'DAMAN AND DIU'
				When   '26'  Then  'DADAR AND NAGAR HAVELI'
				When   '27'  Then  'MAHARASTRA'
				When   '29'  Then  'KARNATAKA'
				When   '30'  Then  'GOA'
				When   '31'  Then  'LAKSHADWEEP'
				When   '32'  Then  'KERALA'
				When   '33'  Then  'TAMIL NADU'
				When   '34'  Then  'PONDICHERRY'
				When   '35'  Then  'ANDAMAN AND NICOBAR'
				When   '36'  Then  'TELENGANA'
				When   '37'  Then  'ANDHRA PRADESH'
				else toStateCode
				End
				where IsNull(toStateCode,'') <> ''  And IsNull(ErrorCode,0) <> 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set actfromstatecode =
				Case actfromstatecode
				When   '0'  Then  'OTHER COUNTRIES'
				When   '00'  Then  'OTHER COUNTRIES'
				When   '1'  Then  'JAMMU AND KASHMIR'
				When   '01'  Then  'JAMMU AND KASHMIR'				
				When   '2'  Then  'HIMACHAL PRADESH'
				When   '02'  Then  'HIMACHAL PRADESH'
				When   '3'  Then  'PUNJAB'
				When   '03'  Then  'PUNJAB'
				When   '4'  Then  'CHANDIGARH'
				When   '04'  Then  'CHANDIGARH'
				When   '5'  Then  'UTTARAKHAND'
				When   '05'  Then  'UTTARAKHAND'
				When   '6'  Then  'HARYANA'
				When   '06'  Then  'HARYANA'
				When   '7'  Then  'DELHI'
				When   '07'  Then  'DELHI'
				When   '8'  Then  'RAJASTHAN'
				When   '08'  Then  'RAJASTHAN'
				When   '9'  Then  'UTTAR PRADESH'
				When   '09'  Then  'UTTAR PRADESH'
				When   '10'  Then  'BIHAR'
				When   '11'  Then  'SIKKIM'
				When   '12'  Then  'ARUNACHAL PRADESH'
				When   '13'  Then  'NAGALAND'
				When   '14'  Then  'MANIPUR'
				When   '15'  Then  'MIZORAM'
				When   '16'  Then  'TRIPURA'
				When   '17'  Then  'MEGHALAYA'
				When   '18'  Then  'ASSAM'
				When   '19'  Then  'WEST BENGAL'
				When   '20'  Then  'JHARKHAND'
				When   '21'  Then  'ORISSA'
				When   '22'  Then  'CHHATTISGARH'
				When   '23'  Then  'MADHYA PRADESH'
				When   '24'  Then  'GUJARAT'
				When   '25'  Then  'DAMAN AND DIU'
				When   '26'  Then  'DADAR AND NAGAR HAVELI'
				When   '27'  Then  'MAHARASTRA'
				When   '29'  Then  'KARNATAKA'
				When   '30'  Then  'GOA'
				When   '31'  Then  'LAKSHADWEEP'
				When   '32'  Then  'KERALA'
				When   '33'  Then  'TAMIL NADU'
				When   '34'  Then  'PONDICHERRY'
				When   '35'  Then  'ANDAMAN AND NICOBAR'
				When   '36'  Then  'TELENGANA'
				When   '37'  Then  'ANDHRA PRADESH'
				else actfromstatecode
				End
				where IsNull(actfromstatecode,'') <> ''  And IsNull(ErrorCode,0) <> 0 

  			Update #TBL_CSV_EWBGEN_RECS 
				Set acttoStateCode =
				Case acttoStateCode
				When   '0'  Then  'OTHER COUNTRIES'
				When   '00'  Then  'OTHER COUNTRIES'
				When   '1'  Then  'JAMMU AND KASHMIR'
				When   '01'  Then  'JAMMU AND KASHMIR'				
				When   '2'  Then  'HIMACHAL PRADESH'
				When   '02'  Then  'HIMACHAL PRADESH'
				When   '3'  Then  'PUNJAB'
				When   '03'  Then  'PUNJAB'
				When   '4'  Then  'CHANDIGARH'
				When   '04'  Then  'CHANDIGARH'
				When   '5'  Then  'UTTARAKHAND'
				When   '05'  Then  'UTTARAKHAND'
				When   '6'  Then  'HARYANA'
				When   '06'  Then  'HARYANA'
				When   '7'  Then  'DELHI'
				When   '07'  Then  'DELHI'
				When   '8'  Then  'RAJASTHAN'
				When   '08'  Then  'RAJASTHAN'
				When   '9'  Then  'UTTAR PRADESH'
				When   '09'  Then  'UTTAR PRADESH'
				When   '10'  Then  'BIHAR'
				When   '11'  Then  'SIKKIM'
				When   '12'  Then  'ARUNACHAL PRADESH'
				When   '13'  Then  'NAGALAND'
				When   '14'  Then  'MANIPUR'
				When   '15'  Then  'MIZORAM'
				When   '16'  Then  'TRIPURA'
				When   '17'  Then  'MEGHALAYA'
				When   '18'  Then  'ASSAM'
				When   '19'  Then  'WEST BENGAL'
				When   '20'  Then  'JHARKHAND'
				When   '21'  Then  'ORISSA'
				When   '22'  Then  'CHHATTISGARH'
				When   '23'  Then  'MADHYA PRADESH'
				When   '24'  Then  'GUJARAT'
				When   '25'  Then  'DAMAN AND DIU'
				When   '26'  Then  'DADAR AND NAGAR HAVELI'
				When   '27'  Then  'MAHARASTRA'
				When   '29'  Then  'KARNATAKA'
				When   '30'  Then  'GOA'
				When   '31'  Then  'LAKSHADWEEP'
				When   '32'  Then  'KERALA'
				When   '33'  Then  'TAMIL NADU'
				When   '34'  Then  'PONDICHERRY'
				When   '35'  Then  'ANDAMAN AND NICOBAR'
				When   '36'  Then  'TELENGANA'
				When   '37'  Then  'ANDHRA PRADESH'
				else acttoStateCode
				End
				where IsNull(acttoStateCode,'') <> '' And IsNull(ErrorCode,0) <> 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set subsupplytype =
				Case subsupplytype
				When   '1'  Then  'Supply'
				When   '2'  Then  'Import'
				When   '3'  Then  'Export'
				When   '4'  Then  'Job Work'
				When   '5'  Then  'For Own Use'
				When   '6'  Then  'Job work Returns'
				When   '7'  Then  'Sales Return'
				When   '8'  Then  'Others'
				When   '9'  Then  'SKD/CKD'
				When   '10'  Then  'Line Sales'
				When   '11'  Then  'Recipient Not Known'
				When   '12'  Then  'Exhibition or Fairs'
				else subsupplytype
				End
				where IsNull(subsupplytype,'') <> '' And IsNull(ErrorCode,0) <> 0 

			Update #TBL_CSV_EWBGEN_RECS 
				Set SupplyType =
				Case SupplyType
				When   'O'  Then  'Outward'
				When   'I'  Then  'Inward'
				else SupplyType
				End
				where IsNull(SupplyType,'') <> '' And IsNull(ErrorCode,0) <> 0 
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set transmode =
				Case transmode
				When   '1'  Then  'Road'
				When   '2'  Then  'Rail'
				When   '3'  Then  'Air'
				When   '4'  Then  'Ship'
				else transmode
				End
				where IsNull(transmode,'') <> '' And IsNull(ErrorCode,0) <> 0 
			
			
			Update #TBL_CSV_EWBGEN_RECS 
				Set qtyunit =
				Case qtyunit
				When   'BGS'  Then  'BAGS'
				When   'BND'  Then  'BUNDLES'
				When   'BOX'  Then  'BOXES'
				When   'CMS'  Then  'CENTIMETERS'
				When   'DZN'  Then  'DOZENS'
				When   'GMS'  Then  'GRAMS'
				When   'HKM'  Then  'HUNDRED KILOMETERS'
				When   'HNO'  Then  'HUNDRED NUMBERS/UNITS'
				When   'KGS'  Then  'KILOGRAMS'
				When   'KMS'  Then  'KILOMETERS'
				When   'LTS'  Then  'LITRES'
				When   'MLS'  Then  'MILLILITRES'
				When   'MTR'  Then  'METERS'
				When   'MTS'  Then  'METRIC TONNES'
				When   'NOS'  Then  'NUMBERS/UNITS'
				When   'PAR'  Then  'PAIRS'
				When   'QTS'  Then  'QUINTALS'
				When   'SNO'  Then  'THOUSAND NUMBERS/UNITS'
				When   'TKM'  Then  'THOUSAND KILOMETERS'
				When   'TLT'  Then  'THOUSAND LITRES'
				When   'TNO'  Then  'TEN NUMBERS/UNITS'
				When   'TON'  Then  'TONNES'
				else qtyunit
				End
				where IsNull(qtyunit,'') <> '' And IsNull(ErrorCode,0) <> 0 

			
			Update #TBL_CSV_EWBGEN_RECS 
				Set vehicletype =
				Case vehicletype
				When   'R'  Then  'Regular'
				When   'O'  Then  'Over dimensional cargo'
				else vehicletype
				End
				where IsNull(vehicletype,'') <> '' And IsNull(ErrorCode,0) <> 0 
		--- Completes updating error record with proper text values
		select 
			slno  As  [Sl no],			
			branch   As [Location],
			supplytype  As  [Transaction Type],
			subsupplytype  As  [Transaction Sub- Type],
			doctype  As  [Doc Type],
			docno  As  [Doc No.],
			docdate  As  [Doc Date         (DD-MM-YYYY)],
			InvoiceType As [Invoice Type],
			fromgstin  As  [From GSTN],
			fromtrdname  As  [From Trade Name],
			fromaddr1  As  [From Address 1],
			fromaddr2  As  [From Address 2],
			fromplace  As  [From Place],
			frompincode  As  [From Pincode],
			fromstatecode  As  [From State Code],
			actfromstatecode As [Actual From State Code],
			togstin  As  [To GSTN],
			totrdname  As  [To Trade Name],
			toaddr1  As  [To Address 1],
			toaddr2  As  [To Address 2],
			toplace  As  [To Place],
			topincode  As  [To  Pincode],
			tostatecode  As  [To State Code],
			acttostatecode As [Actual To State Code],
			totalvalue  As  [Total Value],
			igstvalue  As  [Total IGST Value],
			cgstvalue  As  [Total CGST Value],
			sgstvalue  As  [Total SGST Value],
			cessvalue  As  [Total CESS Value],
			totinvvalue As [Total Invoice Value],
			transmode  As  [Trans Mode],
			transdistance  As  [Approximate Distance(In KM)],
			transporterid  As  [Transporter Id],
			transportername  As  [Transporter Name],
			transdocno  As  [Transporter Document No.],
			transdocdate  As  [Transporter Document Date (DD-MM-YYYY)],
			vehicleno  As  [Vehicle No.],
			vehicletype  As  [Vehicle Type],
			productname  As  [HSN/SAC Name],
			productdesc  As  [HSN Description],
			hsncode  As  [HSN/SAC Code],
			quantity  As  [Quantity Of Goods Sold],
			qtyunit  As  [UQC],
			taxableamount  As  [Item Taxable Value  (excluding tax)],
			igstrate  As  [IGST Rate],
			cgstrate  As  [CGST Rate],
			sgstrate  As  [SGST Rate],
			cessrate  As  [Cess Rate],
			cessadvol  As  [Cess Advolerum],
			AllowDuplication as AllowDuplication,
			errorcode,
			errormessage
			from #TBL_CSV_EWBGEN_RECS
			Where IsNull(ErrorCode,0) <> 0

		Select 'Total Record Count :' + convert(varchar(10), @TotalRecordsCount)
		Select 'Processed Record Count :' + convert(varchar(10), @ProcessedRecordsCount)
		Select 'Error Record Count :' + convert(varchar(10), @ErrorRecordsCount)
	    Drop table #TBL_CSV_EWBGEN_RECS	
		

	End 
	 Return 0


End