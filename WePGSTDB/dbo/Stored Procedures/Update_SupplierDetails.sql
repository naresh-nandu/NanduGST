-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Modify the Supplier details into TBL_Supplier Table
-- exec [Update_SupplierDetails] 1,'Raja','Sales','kk5@gmail.com','9008906454','GSTIN0001',4,'CJTPK10001','04/01/2017','Test','Bangalore',1
-- =============================================
CREATE PROCEDURE [Update_SupplierDetails] 
		@SupplierId		int,
		@SupplierName	nvarchar(255),
		@POCName    nvarchar(255),
		@NatureOfBusiness    nvarchar(255),
		@EmailId    nvarchar(150),
		@MobileNo    nvarchar(50),
		@GSTINno    nvarchar(100),
		@StateCode    int,
		@PANNO    nvarchar(50),
		@DateofCompRegistered    varchar(50),
		@ConstitutionOfBusiness    nvarchar(250),
		@Address    nvarchar(250),
		@CreatedBy    int
AS
BEGIN

	if exists (select supplierId from TBL_Supplier where SupplierId=@SupplierId and RowStatus=1)
	Begin 
		
		--if not exists(select 1 from TBL_Supplier where (EmailId = @EmailId or MobileNo = @MobileNo or GSTINno  =  @GSTINno) and  SupplierId <> @SupplierId)
		--Begin
		Begin Transaction
			Update TBL_Supplier set
			SupplierName=@SupplierName,
			POCName  =  @POCName,
			NatureOfBusiness  =  @NatureOfBusiness,
			EmailId  =  @EmailId,
			MobileNo  =  @MobileNo,
			GSTINno  =  @GSTINno,
			StateCode  =  @StateCode,
			PANNO  =  @PANNO,
			DateofCompRegistered  =  @DateofCompRegistered,
			ConstitutionOfBusiness  =  @ConstitutionOfBusiness,
			Address  =  @Address,
			LastmodifiedBy  =  @CreatedBy,
			LastModifiedDate  = getdate()			 
			 where  SupplierId=@SupplierId and RowStatus=1

			if @@ROWCOUNT=1
				Begin
					Commit
				End
			Else
				Begin
					RollBack
				End
			return 1

		--End
		--Else
		--Begin
		--	return -1
		--End
	End

	
	
END