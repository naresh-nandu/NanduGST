-- =============================================
-- Author:		KK
-- Create date: 19-Apr-17
-- Description:	Procedure to insert the Supplier details into TBL_Supplier Table
-- exec [Ins_Supplierdetails] 'Stack New','Raja','Sales','kk5@gmail.com','9008906454','GSTIN0001',4,'CJTPK10001','04/01/2017','Test','Bangalore',1,1,0
-- =============================================
CREATE PROCEDURE [Ins_Supplierdetails] 
		@SupplierName    nvarchar(255),
		@POCName    nvarchar(255),
		@NatureOfBusiness    nvarchar(255),
		@EmailId    nvarchar(150),
		@MobileNo    nvarchar(50),
		@GSTINno    nvarchar(100),
		@StateCode    nvarchar(2),
		@PANNO    nvarchar(50),
		@DateofCompRegistered    nvarchar(50),
		@ConstitutionOfBusiness    nvarchar(250),
		@Address    nvarchar(250),
		@CustomerId    int,
		@CreatedBy    int,
		@RetValue	int output

AS
BEGIN
	
	if not exists(select SupplierId from TBL_Supplier where  (GSTINno=UPPER(@GSTINno) or EmailId=@EmailId or MobileNo= @MobileNo) and CustomerId = @CustomerId and rowstatus=1)
	begin

		insert into TBL_Supplier(SupplierName,
								POCName,
								NatureOfBusiness,
								EmailId,
								MobileNo,
								GSTINno,
								StateCode,
								PANNO,
								DateofCompRegistered,
								ConstitutionOfBusiness,
								Address,
								CustomerId,
								CreatedBy,
								CreatedDate,
								RowStatus)
								values(
								@SupplierName,
								@POCName,
								@NatureOfBusiness,
								@EmailId,
								@MobileNo,
								UPPER(@GSTINno),
								@StateCode,
								@PANNO,
								@DateofCompRegistered,
								@ConstitutionOfBusiness,
								@Address,
								@CustomerId,
								@CreatedBy,
								Getdate(),
								1)
			
			set @RetValue=1 -- Supplier Details Save Sucessfully
			print @RetValue	
	End
	else 
		Begin
			set @RetValue=2	-- This supplier is already exists
			
			print @RetValue
		End
	
	return @RetValue
	
END