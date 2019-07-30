CREATE TABLE [dbo].[TBL_MOBILE_APP_TRANSACTIONS_DET] (
    [TransactionsSubId] INT           IDENTITY (1, 1) NOT NULL,
    [TransactionsId]    INT           NULL,
    [PaymentModeName]   NVARCHAR (50) NULL,
    [Amount]            NVARCHAR (50) NULL
);

