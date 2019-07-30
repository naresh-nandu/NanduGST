CREATE TABLE [dbo].[TBL_EWB_GEN_REJECT] (
    [ewbrejectid]      INT            IDENTITY (1, 1) NOT NULL,
    [userGSTIN]        NVARCHAR (15)  NULL,
    [ewbNo]            NVARCHAR (12)  NULL,
    [ewayBillNo]       NVARCHAR (12)  NULL,
    [ewbRejectedDate]  NVARCHAR (50)  NULL,
    [custid]           INT            NULL,
    [createdby]        INT            NULL,
    [createdDate]      NVARCHAR (30)  NULL,
    [status]           NVARCHAR (1)   NULL,
    [errorCode]        NVARCHAR (MAX) NULL,
    [errorDescription] NVARCHAR (MAX) NULL,
    CONSTRAINT [PK_TBL_EWB_GEN_REJECT] PRIMARY KEY CLUSTERED ([ewbrejectid] ASC)
);

