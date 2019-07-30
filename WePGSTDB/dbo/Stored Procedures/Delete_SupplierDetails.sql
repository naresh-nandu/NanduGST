
-- =============================================
-- Author:		KK
-- Create date: 13-Apr-17
-- Description:	Procedure to Delete the Supplier details from TBL_Supplier Table(i.e. Changing row status as 0)
-- exec [Delete_SupplierDetails] 1,,1
-- =============================================
Create PROCEDURE [Delete_SupplierDetails] 
		@SupplierId		int,		
		@CreatedBy    int
AS
BEGIN

	if exists (select supplierId from TBL_Supplier where SupplierId=@SupplierId and RowStatus=1)
	Begin 
		
			Update TBL_Supplier set RowStatus=0,LastmodifiedBy  = @CreatedBy,LastModifiedDate  = getdate()
			 where  SupplierId=@SupplierId and RowStatus=1
	End	
	
END