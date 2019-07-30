CREATE TABLE [dbo].[TBL_EWB_GEN_CANCEL] (
    [ewbcancelid]      INT            IDENTITY (1, 1) NOT NULL,
    [userGSTIN]        NVARCHAR (15)  NULL,
    [ewbNo]            NVARCHAR (12)  NULL,
    [cancelRSNCode]    INT            NULL,
    [cancelRmrk]       NVARCHAR (50)  NULL,
    [ewayBillNo]       NVARCHAR (12)  NULL,
    [cancelDate]       NVARCHAR (30)  NULL,
    [custid]           INT            NULL,
    [createdby]        INT            NULL,
    [createdDate]      NVARCHAR (30)  NULL,
    [status]           NVARCHAR (1)   NULL,
    [errorCodes]       NVARCHAR (MAX) NULL,
    [errorDescription] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TBL_EWB_GEN_CANCEL] PRIMARY KEY CLUSTERED ([ewbcancelid] ASC)
);

