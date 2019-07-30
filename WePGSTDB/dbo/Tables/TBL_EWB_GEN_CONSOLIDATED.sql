CREATE TABLE [dbo].[TBL_EWB_GEN_CONSOLIDATED] (
    [consewbid]             INT            IDENTITY (1, 1) NOT NULL,
    [userGSTIN]             VARCHAR (15)   NULL,
    [fromPlace]             NVARCHAR (50)  NULL,
    [fromState]             INT            NULL,
    [vehicleNo]             NVARCHAR (20)  NULL,
    [transMode]             NVARCHAR (1)   NULL,
    [transDocNo]            NVARCHAR (15)  NULL,
    [transDocDate]          NVARCHAR (10)  NULL,
    [CustId]                INT            NULL,
    [CreatedBy]             INT            NULL,
    [CreatedDate]           DATETIME       NULL,
    [flag]                  NVARCHAR (1)   NULL,
    [CEWB_status]           NVARCHAR (1)   NULL,
    [cEwbNo]                NVARCHAR (50)  NULL,
    [cEWBDate]              NVARCHAR (50)  NULL,
    [CEWB_errorCodes]       NVARCHAR (MAX) NULL,
    [CEWB_errorDescription] NVARCHAR (MAX) NULL,
    [BranchId]              INT            NULL,
    [APIBulkFlag]           VARCHAR (1)    NULL
);



