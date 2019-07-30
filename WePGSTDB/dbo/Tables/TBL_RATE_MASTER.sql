CREATE TABLE [dbo].[TBL_RATE_MASTER] (
    [rateId]       INT             IDENTITY (1, 1) NOT NULL,
    [rate]         DECIMAL (18, 2) NULL,
    [createdby]    INT             NULL,
    [creationdate] DATETIME        NULL,
    [rowstatus]    BIT             NULL
);

