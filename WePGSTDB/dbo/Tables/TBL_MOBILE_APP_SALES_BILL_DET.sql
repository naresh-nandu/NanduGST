CREATE TABLE [dbo].[TBL_MOBILE_APP_SALES_BILL_DET] (
    [SalesBillSubId] INT            IDENTITY (1, 1) NOT NULL,
    [SalesBillId]    INT            NULL,
    [InvoiceNo]      NVARCHAR (50)  NULL,
    [TotalItems]     NVARCHAR (50)  NULL,
    [BillAmount]     NVARCHAR (50)  NULL,
    [CustId]         NVARCHAR (50)  NULL,
    [CustName]       NVARCHAR (100) NULL
);

