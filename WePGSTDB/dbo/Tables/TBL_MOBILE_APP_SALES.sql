CREATE TABLE [dbo].[TBL_MOBILE_APP_SALES] (
    [SalesId]     INT           IDENTITY (1, 1) NOT NULL,
    [GSTIN]       NVARCHAR (15) NULL,
    [ReferenceNo] NVARCHAR (50) NULL,
    [InvoiceDate] NVARCHAR (50) NULL,
    [CreatedDate] DATETIME      NULL,
    [MobileAppId] INT           NULL
);

