CREATE TABLE [dbo].[TBL_Cust_TRP_TaxPayer_Docs_Upload] (
    [UploadId]     INT           IDENTITY (1, 1) NOT NULL,
    [DocId]        INT           NOT NULL,
    [RegId]        INT           NOT NULL,
    [TRPMasterId]  INT           NOT NULL,
    [PANNo]        NVARCHAR (10) NULL,
    [CustId]       INT           NULL,
    [CreatedBy]    INT           NULL,
    [CreatedDate]  DATETIME      NULL,
    [ModifiedBy]   INT           NULL,
    [ModifiedDate] DATETIME      NULL,
    [rowstatus]    BIT           NULL,
    [CompanyName]  VARCHAR (50)  NULL
);

