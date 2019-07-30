CREATE TABLE [dbo].[TBL_MOBILE_APP_SALES_ITMS] (
    [SalesSubId]    INT           IDENTITY (1, 1) NOT NULL,
    [SalesId]       INT           NULL,
    [ItemId]        NVARCHAR (50) NULL,
    [ItemShortName] NVARCHAR (50) NULL,
    [ItemQty]       NVARCHAR (50) NULL,
    [Amount]        NVARCHAR (50) NULL
);

