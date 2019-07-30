CREATE TABLE [dbo].[TBL_WEP_WALLET] (
    [WalletId]     INT             IDENTITY (1, 1) NOT NULL,
    [TRPId]        INT             NULL,
    [CustEmail]    NVARCHAR (100)  NULL,
    [MobileNo]     NVARCHAR (150)  NULL,
    [ProductType]  NVARCHAR (50)   NULL,
    [Qty]          INT             NULL,
    [Value]        DECIMAL (18, 2) NULL,
    [TotalValue]   DECIMAL (18, 2) NULL,
    [CreatedDate]  DATETIME        NULL,
    [ModifiedDate] DATETIME        NULL
);

