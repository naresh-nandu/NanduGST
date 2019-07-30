CREATE TABLE [dbo].[TBL_MOBILE_APP_PURCHASES_ITMS] (
    [PurchasesSubId] INT           IDENTITY (1, 1) NOT NULL,
    [PurchasesId]    INT           NULL,
    [ItemId]         NVARCHAR (50) NULL,
    [ItemShortName]  NVARCHAR (50) NULL,
    [ItemQty]        NVARCHAR (50) NULL,
    [Amount]         NVARCHAR (50) NULL
);

