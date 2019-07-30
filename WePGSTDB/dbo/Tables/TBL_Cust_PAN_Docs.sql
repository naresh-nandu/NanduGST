CREATE TABLE [dbo].[TBL_Cust_PAN_Docs] (
    [PANdocId]        INT             IDENTITY (1, 1) NOT NULL,
    [FileName]        VARCHAR (100)   NULL,
    [FileContentType] NVARCHAR (200)  NULL,
    [FileData]        VARBINARY (MAX) NULL,
    [PANId]           INT             NULL,
    [CustId]          INT             NULL,
    [CreatedBy]       INT             NULL,
    [CreatedDate]     DATETIME        NULL,
    [ModifiedBy]      INT             NULL,
    [ModifiedDate]    DATETIME        NULL,
    [rowstatus]       BIT             NULL,
    CONSTRAINT [PK_TBL_Cust_PAN_Docs] PRIMARY KEY CLUSTERED ([PANdocId] ASC)
);

