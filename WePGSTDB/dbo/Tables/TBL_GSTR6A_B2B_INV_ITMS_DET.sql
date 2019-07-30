CREATE TABLE [dbo].[TBL_GSTR6A_B2B_INV_ITMS_DET] (
    [itmdetid] INT             IDENTITY (1, 1) NOT NULL,
    [itmsid]   INT             NOT NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [txval]    DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr6aid] INT             NULL,
    PRIMARY KEY CLUSTERED ([itmdetid] ASC)
);

