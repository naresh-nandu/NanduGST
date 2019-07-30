
-- =============================================
-- Author:		KK
-- Create date: 19-Apr-17
-- Description:	Procedure to insert the B2b outward details
-- exec [Ins_Outward_Gstr1_B2B] 'SUP1','TGHGJHGJG657567',1,'01AABCE2207R1Z5','04-04-2017','INV001',4000.00,'10','10','1','INV100201','02-04-2017','HGJHGJHGH',1,0
-- =============================================
CREATE PROCEDURE [Ins_Outward_Gstr1_B2B] 
		@SupplierName    nvarchar(255),
		@SupplierCTIN    nvarchar(15),
		@CustomerId		int,
		@GSTINNo	nvarchar(15),
		@InvoiceDate	varchar(50),
		@InvoiceNo	varchar(50),
		@Value	decimal,
		@POS	varchar(50),
		@ReverseCharge	varchar(50),
		@PRS	varchar(10),
		@od_num	varchar(50),
		@od_dt	varchar(50),
		@Etin	varchar(15),
		@CreatedBy	int,		
		@InvoiceId	int output

AS
Begin
	Declare @Gstnid int	,@fp varchar(20)
	Declare @GSTR1Id int,@b2bId int
	if not exists(select SupplierId from TBL_Supplier where supplierName=@SupplierName and GSTINno=@SupplierCTIN and  rowstatus=1)
	begin
		insert into TBL_Supplier(SupplierName,GSTINno,CustomerId,CreatedBy,CreatedDate,RowStatus)
			values(@SupplierName,@SupplierCTIN,@CustomerId,@CreatedBy,Getdate(),1)		
	End
	select @Gstnid=(select top(1) GSTINId from TBL_Cust_GSTIN where GSTINNo=@GSTINNo order by GSTINId desc) -- Get GSTNID  from Customer GSTN Table
	select @fp = REPLACE(RIGHT(CONVERT(VARCHAR(10), GETDATE(), 103), 7),'/','')  -- Get Currewnt Month and date for Fp
	insert into TBL_GSTR1(gstin,gstinid,fp,gt) values(@GSTINNo,@Gstnid,@fp,NULL)  -- Insert GSTR1 table with respective customer details
	select @GSTR1Id=SCOPE_IDENTITY()
	insert into TBL_GSTR1_B2B(gstr1id,ctin)values(@GSTR1Id,@SupplierCTIN)
	select @b2bId=SCOPE_IDENTITY()
	insert into TBL_GSTR1_B2B_INV(b2bid,inum,idt,val,pos,rchrg,etin)values(@b2bId,@InvoiceNo,@InvoiceDate,@Value,@POS,@ReverseCharge,@Etin)
	set @InvoiceId=SCOPE_IDENTITY()

	return 	@InvoiceId
	
END