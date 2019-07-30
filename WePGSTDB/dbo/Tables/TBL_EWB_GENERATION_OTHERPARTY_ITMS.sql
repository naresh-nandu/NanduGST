CREATE TABLE [dbo].[TBL_EWB_GENERATION_OTHERPARTY_ITMS] (
    [itmsid]        INT             IDENTITY (1, 1) NOT NULL,
    [ewbid]         INT             NULL,
    [productName]   NVARCHAR (100)  NULL,
    [productDesc]   NVARCHAR (MAX)  NULL,
    [hsnCode]       INT             NULL,
    [quantity]      DECIMAL (18, 2) NULL,
    [qtyUnit]       NVARCHAR (3)    NULL,
    [taxableAmount] DECIMAL (18, 2) NULL,
    [igstRate]      DECIMAL (18, 2) NULL,
    [cgstRate]      DECIMAL (18, 2) NULL,
    [sgstRate]      DECIMAL (18, 2) NULL,
    [cessRate]      DECIMAL (18, 2) NULL,
    [cessAdvol]     DECIMAL (18, 2) NULL,
    [createdby]     INT             NULL,
    [createddate]   DATETIME        NULL,
    [CustId]        INT             NULL
);

