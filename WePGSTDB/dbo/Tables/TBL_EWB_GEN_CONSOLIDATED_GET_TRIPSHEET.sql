CREATE TABLE [dbo].[TBL_EWB_GEN_CONSOLIDATED_GET_TRIPSHEET] (
    [tripsheetid] INT             IDENTITY (1, 1) NOT NULL,
    [consewbid]   INT             NULL,
    [ewbNo]       BIGINT          NULL,
    [createdby]   INT             NULL,
    [createddate] DATETIME        NULL,
    [CustId]      INT             NULL,
    [userGstin]   VARCHAR (50)    NULL,
    [docNo]       VARCHAR (50)    NULL,
    [docDate]     VARCHAR (50)    NULL,
    [assessValue] DECIMAL (18, 2) NULL,
    [cgstValue]   DECIMAL (18, 2) NULL,
    [sgstValue]   DECIMAL (18, 2) NULL,
    [igstValue]   DECIMAL (18, 2) NULL,
    [cessValue]   DECIMAL (18, 2) NULL,
    [ewbDate]     VARCHAR (30)    NULL
);

