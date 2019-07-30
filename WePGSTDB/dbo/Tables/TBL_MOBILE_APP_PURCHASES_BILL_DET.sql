CREATE TABLE [dbo].[TBL_MOBILE_APP_PURCHASES_BILL_DET] (
    [PurchasesBillSubId] INT            IDENTITY (1, 1) NOT NULL,
    [PurchasesBillId]    INT            NULL,
    [InvoiceNo]          NVARCHAR (50)  NULL,
    [TotalItems]         NVARCHAR (50)  NULL,
    [BillAmount]         NVARCHAR (50)  NULL,
    [SupplierId]         NVARCHAR (50)  NULL,
    [SupplierName]       NVARCHAR (100) NULL
);

