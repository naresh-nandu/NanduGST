-- =============================================
-- Author:		KK
-- Create date: 19-Apr-17
-- Description:	Procedure to insert the B2b Inward details
-- exec [Ins_Inward_Gstr2_B2B] 'SUP1','TGHGJHGJG657567',1,'01AABCE2207R1Z5','04-04-2017','INV001',4000,'10','10',1,0
-- =============================================
CREATE PROCEDURE [Ins_Inward_Gstr2_B2B] 
		@SupplierName    nvarchar(255),
		@SupplierCTIN    nvarchar(15),
		@CustomerId		int,
		@GSTINNo	nvarchar(15),
		@InvoiceDate	varchar(50),
		@InvoiceNo	varchar(50),
		@Value	decimal,
		@POS	varchar(50),
		@ReverseCharge	varchar(50),
		@CreatedBy	int,		
		@InvoiceId	int output

AS
Begin
	Declare @Gstnid int	,@fp varchar(20)
	Declare @GSTR2Id int,@b2bId int
	if not exists(select SupplierId from TBL_Supplier where supplierName=@SupplierName and GSTINno=@SupplierCTIN and  rowstatus=1)
	begin
		insert into TBL_Supplier(SupplierName,GSTINno,CustomerId,CreatedBy,CreatedDate,RowStatus)
			values(@SupplierName,@SupplierCTIN,@CustomerId,@CreatedBy,Getdate(),1)		
	End
	select @Gstnid=(select top(1) GSTINId from TBL_Cust_GSTIN where GSTINNo=@GSTINNo order by GSTINId desc) -- Get GSTNID  from Customer GSTN Table
	select @fp = REPLACE(RIGHT(CONVERT(VARCHAR(10), GETDATE(), 103), 7),'/','')  -- Get Currewnt Month and date for Fp
	insert into TBL_GSTR2(gstin,gstinid,fp) values(@GSTINNo,@Gstnid,@fp)  -- Insert GSTR2 table with respective customer details
	select @GSTR2Id=SCOPE_IDENTITY()
	insert into TBL_GSTR2_B2B(gstr2id,ctin)values(@GSTR2Id,@SupplierCTIN)
	
select @b2bId=SCOPE_IDENTITY()
	insert into TBL_GSTR2_B2B_INV(b2bid,inum,idt,val,pos,rchrg)values(@b2bId,@InvoiceNo,@InvoiceDate,@Value,@POS,@ReverseCharge)
	set @InvoiceId=SCOPE_IDENTITY()

	return 	@InvoiceId
	
END