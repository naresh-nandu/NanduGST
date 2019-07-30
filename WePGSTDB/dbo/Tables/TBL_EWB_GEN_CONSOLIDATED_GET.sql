CREATE TABLE [dbo].[TBL_EWB_GEN_CONSOLIDATED_GET] (
    [consewbid]        INT            IDENTITY (1, 1) NOT NULL,
    [fromPlace]        NVARCHAR (50)  NULL,
    [fromState]        INT            NULL,
    [vehicleNo]        NVARCHAR (20)  NULL,
    [transMode]        NVARCHAR (1)   NULL,
    [transDocNo]       NVARCHAR (15)  NULL,
    [transDocDate]     NVARCHAR (10)  NULL,
    [status]           NVARCHAR (1)   NULL,
    [errorCodes]       NVARCHAR (MAX) NULL,
    [cEwbNo]           NVARCHAR (50)  NULL,
    [cEWBDate]         NVARCHAR (50)  NULL,
    [createdby]        INT            NULL,
    [createddate]      DATETIME       NULL,
    [CustId]           INT            NULL,
    [flag]             NVARCHAR (1)   NULL,
    [errorDescription] NVARCHAR (MAX) NULL,
    [userGstin]        VARCHAR (50)   NULL
);

