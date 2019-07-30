CREATE TABLE [dbo].[TBL_Cust_Docs] (
    [UploadFileId]    INT             IDENTITY (1, 1) NOT NULL,
    [FileName]        VARCHAR (100)   NULL,
    [FileContentType] NVARCHAR (200)  NULL,
    [FileData]        VARBINARY (MAX) NULL,
    [CustomerId]      INT             NULL,
    [CreatedDate]     DATETIME        NULL,
    CONSTRAINT [PK_TBL_Cust_Docs] PRIMARY KEY CLUSTERED ([UploadFileId] ASC)
);

