CREATE TABLE [dbo].[TBL_WEP_WALLET_TRANSACTIONS] (
    [WalletTransId]   INT             IDENTITY (1, 1) NOT NULL,
    [TRPId]           INT             NULL,
    [CustEmail]       VARCHAR (100)   NULL,
    [MobileNo]        VARCHAR (20)    NULL,
    [ProductType]     VARCHAR (50)    NULL,
    [TransactionType] VARCHAR (10)    NULL,
    [Amount]          DECIMAL (18, 2) NULL,
    [CreatedDate]     DATETIME        NULL,
    [GSTRType]        NVARCHAR (20)   NULL,
    [GSTIN]           NVARCHAR (15)   NULL,
    [FP]              NVARCHAR (10)   NULL
);

