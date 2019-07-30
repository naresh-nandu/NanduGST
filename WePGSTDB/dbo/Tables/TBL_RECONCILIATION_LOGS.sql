CREATE TABLE [dbo].[TBL_RECONCILIATION_LOGS] (
    [LogId]       INT           IDENTITY (1, 1) NOT NULL,
    [Gstin]       NVARCHAR (15) NULL,
    [InvoiceId]   INT           NULL,
    [GSTRType]    NVARCHAR (15) NULL,
    [ActionType]  NVARCHAR (15) NULL,
    [Flag]        CHAR (1)      NULL,
    [Chksum]      NVARCHAR (64) NULL,
    [CreatedBy]   INT           NULL,
    [CustId]      INT           NULL,
    [CreatedDate] DATETIME      NULL,
    [Rowstatus]   BIT           NULL,
    CONSTRAINT [PK_TBL_RECONCILIATION_LOGS] PRIMARY KEY CLUSTERED ([LogId] ASC)
);

