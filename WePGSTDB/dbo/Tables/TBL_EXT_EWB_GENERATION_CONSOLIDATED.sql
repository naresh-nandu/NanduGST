CREATE TABLE [dbo].[TBL_EXT_EWB_GENERATION_CONSOLIDATED] (
    [consewbId]    INT           IDENTITY (1, 1) NOT NULL,
    [userGSTIN]    NVARCHAR (15) NULL,
    [vehicleNo]    NVARCHAR (20) NULL,
    [fromPlace]    NVARCHAR (50) NULL,
    [transMode]    NVARCHAR (10) NULL,
    [transDocNo]   NVARCHAR (50) NULL,
    [transDocDate] NVARCHAR (10) NULL,
    [fromState]    INT           NULL,
    [ewbNo]        BIGINT        NULL,
    [rowstatus]    TINYINT       NULL,
    [sourcetype]   VARCHAR (15)  NULL,
    [referenceno]  VARCHAR (50)  NULL,
    [fileid]       INT           NULL,
    [errormessage] VARCHAR (255) NULL,
    [createdby]    INT           NULL,
    [createddate]  DATETIME      NULL,
    [BranchId]     INT           NULL,
    [APIBulkFlag]  VARCHAR (1)   NULL
);



