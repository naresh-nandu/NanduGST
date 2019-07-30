CREATE TABLE [dbo].[TBL_HSN_MASTER] (
    [hsnId]            INT             IDENTITY (1, 1) NOT NULL,
    [hsnCode]          VARCHAR (15)    NULL,
    [hsnDescription]   NVARCHAR (MAX)  NULL,
    [unitPrice]        DECIMAL (18, 2) NULL,
    [rate]             DECIMAL (18, 2) NULL,
    [CustomerId]       INT             NULL,
    [CreatedBy]        INT             NULL,
    [CreatedDate]      DATETIME        NULL,
    [LastModifiedBy]   INT             NULL,
    [LastModifiedDate] DATETIME        NULL,
    [rowstatus]        BIT             NULL,
    [hsnType]          VARCHAR (25)    NULL,
    CONSTRAINT [PK_TBL_HSN_MASTER] PRIMARY KEY CLUSTERED ([hsnId] ASC),
    CONSTRAINT [FK_TBL_HSN_MASTER_TBL_Customer] FOREIGN KEY ([CustomerId]) REFERENCES [dbo].[TBL_Customer] ([CustId])
);

