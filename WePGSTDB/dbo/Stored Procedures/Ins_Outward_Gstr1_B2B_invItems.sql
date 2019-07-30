
-- =============================================
-- Author:		KK
-- Create date: 19-Apr-17
-- Description:	Procedure to insert the B2b Outward Item Details
-- exec [Ins_Outward_Gstr1_B2B_invItems] 4,1,'G','JJHJ',1000,10,20,10,20,10,20,10,20
-- =============================================
Create PROCEDURE [Ins_Outward_Gstr1_B2B_invItems] 
		@InvoiceId		int,
		@num	int,
		@type	varchar(50),
		@hsnsc	varchar(50),
		@Taxvalue	decimal,
		@IGSTRate	decimal,
		@IGSTamount  decimal,
		@CGSTrate	 decimal,
		@CGSTamount	decimal,
		@SGSTrate	decimal,
		@SGSTamount	Decimal,
		@CESSrate	decimal,
		@CESSamount	decimal
AS
Begin
	
	Declare @InvoiceItemsId int
	insert into TBL_GSTR1_B2B_INV_ITMS(invid,num)values(@InvoiceId,@num)
	select @InvoiceItemsId=SCOPE_IDENTITY()
	insert into TBL_GSTR1_B2B_INV_ITMS_DET (itmsid,rt, txval, iamt, camt, samt, csamt)
	values(@InvoiceItemsId,@IGSTRate,@Taxvalue,@IGSTamount,@CGSTamount,@SGSTamount,@CESSamount)
		
END