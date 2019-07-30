-- =============================================
-- Author:		KK
-- Create date: 19-Apr-17
-- Description:	Procedure to insert the B2b Inward Item Details
-- exec [Ins_Inward_Gstr2_B2BA_invItems] 4,1,'G','JJHJ',1000,10,20,10,20,10,20,'IP'
-- =============================================
CREATE PROCEDURE [Ins_Inward_Gstr2_B2BA_invItems] 
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
		@SGSTamount	decimal,
		@CESSrate	decimal,
		@CESSamount	decimal,
		@Elg	varchar(50)
AS
Begin
	
	Declare @InvoiceItemsId int
	insert into TBL_GSTR2_B2BA_INV_ITMS(invid,num)values(@InvoiceId,@num)
	select @InvoiceItemsId=SCOPE_IDENTITY()
	insert into TBL_GSTR2_B2BA_INV_ITMS_DET (itmsid, ty, hsn_sc, txval, irt, iamt, crt, camt,  csrt, csamt, elg)
	values(@InvoiceItemsId,@type,@hsnsc,@Taxvalue,@IGSTRate,@IGSTamount,@CGSTrate,@CGSTamount,@CESSrate,@CESSamount,@Elg)
		
END