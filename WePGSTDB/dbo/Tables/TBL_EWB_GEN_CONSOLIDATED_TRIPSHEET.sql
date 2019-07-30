CREATE TABLE [dbo].[TBL_EWB_GEN_CONSOLIDATED_TRIPSHEET] (
    [tripsheetid]   INT             IDENTITY (1, 1) NOT NULL,
    [consewbid]     INT             NULL,
    [ewbNo]         BIGINT          NULL,
    [CustId]        INT             NULL,
    [CreatedBy]     INT             NULL,
    [CreatedDate]   DATETIME        NULL,
    [ewbDate]       NVARCHAR (20)   NULL,
    [userGSTIN]     NVARCHAR (15)   NULL,
    [docNo]         NVARCHAR (50)   NULL,
    [docDate]       NVARCHAR (50)   NULL,
    [fromGstin]     NVARCHAR (15)   NULL,
    [fromTradeName] NVARCHAR (100)  NULL,
    [toGstin]       NVARCHAR (15)   NULL,
    [toTradeName]   NVARCHAR (100)  NULL,
    [totInvValue]   DECIMAL (18, 2) NULL,
    [validUpto]     NVARCHAR (20)   NULL
);

