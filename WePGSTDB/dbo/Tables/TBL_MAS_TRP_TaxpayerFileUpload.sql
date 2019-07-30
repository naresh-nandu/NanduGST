CREATE TABLE [dbo].[TBL_MAS_TRP_TaxpayerFileUpload] (
    [DocId]    INT             IDENTITY (1, 1) NOT NULL,
    [RegId]    INT             NOT NULL,
    [FileData] VARBINARY (MAX) NULL,
    [DocName]  NVARCHAR (100)  NULL,
    [CustId]   INT             NULL
);

