CREATE TABLE [dbo].[TBL_GSTR1_D_TXPD_ITMS] (
    [itmsid]   INT             IDENTITY (1, 1) NOT NULL,
    [txpdid]   INT             NOT NULL,
    [rt]       DECIMAL (18, 2) NULL,
    [ad_amt]   DECIMAL (18, 2) NULL,
    [iamt]     DECIMAL (18, 2) NULL,
    [camt]     DECIMAL (18, 2) NULL,
    [samt]     DECIMAL (18, 2) NULL,
    [csamt]    DECIMAL (18, 2) NULL,
    [gstinid]  INT             NULL,
    [gstr1did] INT             NULL,
    PRIMARY KEY CLUSTERED ([itmsid] ASC),
    CONSTRAINT [FK_TBL_GSTR1_D_TXPD_ITMS_TBL_GSTR1_D_TXPD] FOREIGN KEY ([txpdid]) REFERENCES [dbo].[TBL_GSTR1_D_TXPD] ([txpdid])
);

