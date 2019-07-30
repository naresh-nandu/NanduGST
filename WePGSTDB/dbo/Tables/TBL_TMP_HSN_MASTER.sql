CREATE TABLE [dbo].[TBL_TMP_HSN_MASTER] (
    [hsnId]          INT             IDENTITY (1, 1) NOT NULL,
    [hsnCode]        VARCHAR (50)    NULL,
    [hsnDescription] NVARCHAR (MAX)  NULL,
    [unitPrice]      DECIMAL (18, 2) NULL,
    [rate]           DECIMAL (18, 2) NULL,
    [CustomerId]     INT             NULL,
    [CreatedBy]      INT             NULL,
    [CreatedDate]    DATETIME        NULL
);

